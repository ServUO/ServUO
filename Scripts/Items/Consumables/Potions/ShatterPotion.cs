using Server.Network;
using Server.Targeting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class ShatterPotion : BasePotion
    {
        public override int LabelNumber => 1115759;  // Shatter Potion

        public override bool RequireFreeHand => false;

        [Constructable]
        public ShatterPotion()
            : base(0xF0D, PotionEffect.Shatter)
        {
            Hue = 60;
            Weight = 2.0;
        }

        public ShatterPotion(Serial serial)
            : base(serial)
        {
        }

        public override void Drink(Mobile from)
        {
            if (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting))
            {
                from.SendLocalizedMessage(1062725); // You can not use that potion while paralyzed.
                return;
            }

            int delay = GetDelay(from);

            if (delay > 0)
            {
                from.SendLocalizedMessage(1072529, string.Format("{0}\t{1}", delay, delay > 1 ? "seconds." : "second.")); // You cannot use that for another ~1_NUM~ ~2_TIMEUNITS~
                return;
            }

            ThrowTarget targ = from.Target as ThrowTarget;

            if (targ != null && targ.Potion == this)
                return;

            from.RevealingAction();

            if (!m_Users.Contains(from))
                m_Users.Add(from);

            from.Target = new ThrowTarget(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private readonly List<Mobile> m_Users = new List<Mobile>();

        public void Explode_Callback(object state)
        {
            object[] states = (object[])state;

            Explode((Mobile)states[0], (Mobile)states[1], (Map)states[2]);
        }

        public virtual void Explode(Mobile from, Mobile m, Map map)
        {
            if (Deleted || map == null)
                return;

            Consume();

            // Check if any other players are using this potion
            for (int i = 0; i < m_Users.Count; i++)
            {
                ThrowTarget targ = m_Users[i].Target as ThrowTarget;

                if (targ != null && targ.Potion == this)
                    Target.Cancel(from);
            }

            // Effects
            Effects.PlaySound(m.Location, map, 285);

            m.Damage(0);
            Effects.SendPacket(m.Location, m.Map, new ParticleEffect(EffectType.FixedFrom, Serial, Serial.Zero, 0x373A, m.Location, m.Location, 10, 10, false, false, 0, 0, 0, 5051, 1, Serial.Zero, 89, 0));

            int amount = 0;

            if (m.Backpack != null)
            {
                foreach (BasePotion p in m.Backpack.FindItemsByType<BasePotion>())
                {
                    amount += p.Amount;
                }
            }

            if (amount < 20)
            {
                from.SendLocalizedMessage(1115760, from.Name); // ~1_NAME~'s shatter potion hits you, but nothing happens.
            }
            else
            {
                int p = (int)(amount * 0.2);

                if (p > 1)
                {
                    from.SendLocalizedMessage(1115762, string.Format("{0}\t{1}", from.Name, p)); // ~1_NAME~'s shatter potion destroys ~2_NUM~ potions in your inventory.                    
                }
                else
                {
                    from.SendLocalizedMessage(1115761, from.Name); // ~1_NAME~'s shatter potion destroys a potion in your inventory.
                }

                for (int i = 0; i < p; i++)
                {
                    List<BasePotion> potions = m.Backpack.FindItemsByType<BasePotion>();
                    potions[Utility.Random(potions.Count)].Consume();
                }
            }
        }

        #region Delay
        private static readonly Hashtable m_Delay = new Hashtable();

        public static void AddDelay(Mobile m)
        {
            Timer timer = m_Delay[m] as Timer;

            if (timer != null)
                timer.Stop();

            m_Delay[m] = Timer.DelayCall(TimeSpan.FromSeconds(60), new TimerStateCallback(EndDelay_Callback), m);
        }

        public static int GetDelay(Mobile m)
        {
            Timer timer = m_Delay[m] as Timer;

            if (timer != null && timer.Next > DateTime.UtcNow)
                return (int)(timer.Next - DateTime.UtcNow).TotalSeconds;

            return 0;
        }

        private static void EndDelay_Callback(object obj)
        {
            if (obj is Mobile)
                EndDelay((Mobile)obj);
        }

        public static void EndDelay(Mobile m)
        {
            Timer timer = m_Delay[m] as Timer;

            if (timer != null)
            {
                timer.Stop();
                m_Delay.Remove(m);
            }
        }
        #endregion

        private class ThrowTarget : Target
        {
            public ShatterPotion Potion { get; }

            public ThrowTarget(ShatterPotion potion) : base(12, true, TargetFlags.None)
            {
                Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (Potion.Deleted || Potion.Map == Map.Internal)
                    return;

                if (targeted is Mobile)
                {
                    Mobile m = targeted as Mobile;

                    if (m == null || from.Map == null || !from.CanBeHarmful(m))
                        return;

                    // Add delay
                    AddDelay(from);

                    from.RevealingAction();

                    Effects.SendMovingEffect(from, m, Potion.ItemID, 7, 0, false, false, Potion.Hue, 0);
                    Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(Potion.Explode_Callback), new object[] { from, m, from.Map });
                }
            }
        }
    }
}
