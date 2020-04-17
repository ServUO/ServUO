using Server.Network;
using Server.Spells;
using Server.Targeting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class FearEssence : BasePotion
    {
        public override int LabelNumber => 1115744;  // fear essence
        public virtual int Radius => 20;
        public override bool RequireFreeHand => false;

        [Constructable]
        public FearEssence()
            : base(0xF0D, PotionEffect.FearEssence)
        {
            Hue = 5;
            Weight = 2.0;
        }

        public FearEssence(Serial serial)
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

            Explode((Mobile)states[0], (Point3D)states[1], (Map)states[2]);
        }

        public virtual void Explode(Mobile from, Point3D loc, Map map)
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

            Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback(TarEffect), new object[] { loc, map });
            IPooledEnumerable eable = map.GetMobilesInRange(loc, Radius);

            foreach (Mobile mobile in eable)
            {
                if (mobile != from && from.CanBeHarmful(mobile, false))
                {
                    double chance = ((4 * mobile.Skills[SkillName.MagicResist].Value) + 150) / 700;

                    if (chance < Utility.RandomDouble())
                    {
                        mobile.SendLocalizedMessage(1115815); // You resist the effects of the Fear Essence.
                    }
                    else
                    {
                        Point3D p = mobile.Location;
                        Effects.SendPacket(p, mobile.Map, new ParticleEffect(EffectType.FixedFrom, Serial, Serial.Zero, 0x376A, p, p, 9, 32, false, false, 0, 0, 0, 5039, 1, Serial.Zero, 254, 0));
                        mobile.Damage(0, from);
                        mobile.Paralyze(TimeSpan.FromSeconds(3));
                        from.DoHarmful(mobile);
                    }
                }
            }

            eable.Free();
        }

        #region Effects
        public virtual void TarEffect(object state)
        {
            object[] states = (object[])state;

            Point3D p = (Point3D)states[0];
            Map map = (Map)states[1];

            Effects.PlaySound((Point3D)states[0], (Map)states[1], 1619);

            for (int x = -4; x <= 4; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Effects.SendPacket(p, map, new HuedEffect(EffectType.Moving, Serial.Zero, Serial.Zero, 0x3E03, p, new Point3D(p.X + x, p.Y + y, p.Z), 0, 0, false, false, 5, 0));
                }
            }
        }
        #endregion

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
            public FearEssence Potion { get; }

            public ThrowTarget(FearEssence potion)
                : base(12, true, TargetFlags.Harmful)
            {
                Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (Potion.Deleted || Potion.Map == Map.Internal)
                    return;

                IPoint3D p = targeted as IPoint3D;

                if (p == null || from.Map == null)
                    return;

                // Add delay
                AddDelay(from);

                SpellHelper.GetSurfaceTop(ref p);

                from.RevealingAction();

                IEntity to;

                if (p is Mobile)
                    to = (Mobile)p;
                else
                    to = new Entity(Serial.Zero, new Point3D(p), from.Map);

                Effects.SendMovingEffect(from, to, Potion.ItemID, 7, 0, false, false, Potion.Hue, 0);
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(Potion.Explode_Callback), new object[] { from, new Point3D(p), from.Map });
            }
        }
    }
}
