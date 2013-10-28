using System;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Server.Items
{
    public abstract class BaseConfusionBlastPotion : BasePotion
    {
        public abstract int Radius { get; }

        public override bool RequireFreeHand
        {
            get
            {
                return false;
            }
        }

        public BaseConfusionBlastPotion(PotionEffect effect)
            : base(0xF06, effect)
        {
            this.Hue = 0x48D;
        }

        public BaseConfusionBlastPotion(Serial serial)
            : base(serial)
        {
        }

        public override void Drink(Mobile from)
        {
            if (Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
            {
                from.SendLocalizedMessage(1062725); // You can not use that potion while paralyzed.
                return;
            }

            int delay = GetDelay(from);

            if (delay > 0)
            {
                from.SendLocalizedMessage(1072529, String.Format("{0}\t{1}", delay, delay > 1 ? "seconds." : "second.")); // You cannot use that for another ~1_NUM~ ~2_TIMEUNITS~
                return;
            }

            ThrowTarget targ = from.Target as ThrowTarget;

            if (targ != null && targ.Potion == this)
                return;

            from.RevealingAction();

            if (!this.m_Users.Contains(from))
                this.m_Users.Add(from);

            from.Target = new ThrowTarget(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
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

            this.Explode((Mobile)states[0], (Point3D)states[1], (Map)states[2]);
        }

        public virtual void Explode(Mobile from, Point3D loc, Map map)
        {
            if (this.Deleted || map == null)
                return;

            this.Consume();

            // Check if any other players are using this potion
            for (int i = 0; i < this.m_Users.Count; i ++)
            {
                ThrowTarget targ = this.m_Users[i].Target as ThrowTarget;

                if (targ != null && targ.Potion == this)
                    Target.Cancel(from);
            }

            // Effects
            Effects.PlaySound(loc, map, 0x207);

            Geometry.Circle2D(loc, map, this.Radius, new DoEffect_Callback(BlastEffect), 270, 90);

            Timer.DelayCall(TimeSpan.FromSeconds(0.3), new TimerStateCallback(CircleEffect2), new object[] { loc, map });

            foreach (Mobile mobile in map.GetMobilesInRange(loc, this.Radius))
            {
                if (mobile is BaseCreature)
                {
                    BaseCreature mon = (BaseCreature)mobile;

                    if (mon.Controlled || mon.Summoned)
                        continue;

                    mon.Pacify(from, DateTime.UtcNow + TimeSpan.FromSeconds(5.0)); // TODO check
                }
            }
        }

        #region Effects
        public virtual void BlastEffect(Point3D p, Map map)
        {
            if (map.CanFit(p, 12, true, false))
                Effects.SendLocationEffect(p, map, 0x376A, 4, 9);
        }
		
        public void CircleEffect2(object state)
        {
            object[] states = (object[])state;
				
            Geometry.Circle2D((Point3D)states[0], (Map)states[1], this.Radius, new DoEffect_Callback(BlastEffect), 90, 270);
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
            private readonly BaseConfusionBlastPotion m_Potion;

            public BaseConfusionBlastPotion Potion
            {
                get
                {
                    return this.m_Potion;
                }
            }

            public ThrowTarget(BaseConfusionBlastPotion potion)
                : base(12, true, TargetFlags.None)
            {
                this.m_Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Potion.Deleted || this.m_Potion.Map == Map.Internal)
                    return;

                IPoint3D p = targeted as IPoint3D;

                if (p == null || from.Map == null)
                    return;

                // Add delay
                BaseConfusionBlastPotion.AddDelay(from);

                SpellHelper.GetSurfaceTop(ref p);

                from.RevealingAction();

                IEntity to;

                if (p is Mobile)
                    to = (Mobile)p;
                else
                    to = new Entity(Serial.Zero, new Point3D(p), from.Map);

                Effects.SendMovingEffect(from, to, 0xF0D, 7, 0, false, false, this.m_Potion.Hue, 0);
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(this.m_Potion.Explode_Callback), new object[] { from, new Point3D(p), from.Map });
            }
        }
    }
}