using System;
using System.Collections;
using System.Collections.Generic;
using Server.Spells;
using Server.Targeting;

namespace Server.Items
{
    public abstract class BaseConflagrationPotion : BasePotion
    {
        public abstract int MinDamage { get; }
        public abstract int MaxDamage { get; }

        public override bool RequireFreeHand
        {
            get
            {
                return false;
            }
        }

        public BaseConflagrationPotion(PotionEffect effect)
            : base(0xF06, effect)
        {
            this.Hue = 0x489;
        }

        public BaseConflagrationPotion(Serial serial)
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
            Effects.PlaySound(loc, map, 0x20C);

            for (int i = -2; i <= 2; i ++)
            {
                for (int j = -2; j <= 2; j ++)
                {
                    Point3D p = new Point3D(loc.X + i, loc.Y + j, loc.Z);

                    if (map.CanFit(p, 12, true, false) && from.InLOS(p))
                        new InternalItem(from, p, map, this.MinDamage, this.MaxDamage);
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

            m_Delay[m] = Timer.DelayCall(TimeSpan.FromSeconds(30), new TimerStateCallback(EndDelay_Callback), m);	
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
            private readonly BaseConflagrationPotion m_Potion;

            public BaseConflagrationPotion Potion
            {
                get
                {
                    return this.m_Potion;
                }
            }

            public ThrowTarget(BaseConflagrationPotion potion)
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
                BaseConflagrationPotion.AddDelay(from);

                SpellHelper.GetSurfaceTop(ref p);

                from.RevealingAction();

                IEntity to;

                if (p is Mobile)
                    to = (Mobile)p;
                else
                    to = new Entity(Serial.Zero, new Point3D(p), from.Map);

                Effects.SendMovingEffect(from, to, 0xF0D, 7, 0, false, false, this.m_Potion.Hue, 0);
                Timer.DelayCall(TimeSpan.FromSeconds(1.5), new TimerStateCallback(this.m_Potion.Explode_Callback), new object[] { from, new Point3D(p), from.Map });
            }
        }

        public class InternalItem : Item
        {
            private Mobile m_From;
            private int m_MinDamage;
            private int m_MaxDamage;
            private DateTime m_End;
            private Timer m_Timer;

            public Mobile From
            {
                get
                {
                    return this.m_From;
                }
            }

            public override bool BlocksFit
            {
                get
                {
                    return true;
                }
            }

            public InternalItem(Mobile from, Point3D loc, Map map, int min, int max)
                : base(0x398C)
            {
                this.Movable = false;
                this.Light = LightType.Circle300;

                this.MoveToWorld(loc, map);

                this.m_From = from;
                this.m_End = DateTime.UtcNow + TimeSpan.FromSeconds(10);

                this.SetDamage(min, max);

                this.m_Timer = new InternalTimer(this, this.m_End);
                this.m_Timer.Start();
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (this.m_Timer != null)
                    this.m_Timer.Stop();
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public int GetDamage()
            {
                return Utility.RandomMinMax(this.m_MinDamage, this.m_MaxDamage);
            }

            private void SetDamage(int min, int max)
            {
                /* 	new way to apply alchemy bonus according to Stratics' calculator.
                this gives a mean to values 25, 50, 75 and 100. Stratics' calculator is outdated.
                Those goals will give 2 to alchemy bonus. It's not really OSI-like but it's an approximation. */
                this.m_MinDamage = min;
                this.m_MaxDamage = max;

                if (this.m_From == null)
                    return;

                int alchemySkill = this.m_From.Skills.Alchemy.Fixed;
                int alchemyBonus = alchemySkill / 125 + alchemySkill / 250 ;

                this.m_MinDamage = Scale(this.m_From, this.m_MinDamage + alchemyBonus);
                this.m_MaxDamage = Scale(this.m_From, this.m_MaxDamage + alchemyBonus);
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version

                writer.Write((Mobile)this.m_From);
                writer.Write((DateTime)this.m_End);
                writer.Write((int)this.m_MinDamage);
                writer.Write((int)this.m_MaxDamage);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();
				
                this.m_From = reader.ReadMobile();
                this.m_End = reader.ReadDateTime();
                this.m_MinDamage = reader.ReadInt();
                this.m_MaxDamage = reader.ReadInt();

                this.m_Timer = new InternalTimer(this, this.m_End);
                this.m_Timer.Start();
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (this.Visible && this.m_From != null && (!Core.AOS || m != this.m_From) && SpellHelper.ValidIndirectTarget(this.m_From, m) && this.m_From.CanBeHarmful(m, false))
                {
                    this.m_From.DoHarmful(m);

                    AOS.Damage(m, this.m_From, this.GetDamage(), 0, 100, 0, 0, 0);
                    m.PlaySound(0x208);
                }

                return true;
            }

            private class InternalTimer : Timer
            {
                private readonly InternalItem m_Item;
                private readonly DateTime m_End;

                public InternalTimer(InternalItem item, DateTime end)
                    : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
                {
                    this.m_Item = item;
                    this.m_End = end;

                    this.Priority = TimerPriority.FiftyMS;
                }

                protected override void OnTick()
                {
                    if (this.m_Item.Deleted)
                        return;

                    if (DateTime.UtcNow > this.m_End)
                    {
                        this.m_Item.Delete();
                        this.Stop();
                        return;
                    }

                    Mobile from = this.m_Item.From;

                    if (this.m_Item.Map == null || from == null)
                        return;
					
                    List<Mobile> mobiles = new List<Mobile>();

                    foreach (Mobile mobile in this.m_Item.GetMobilesInRange(0))
                        mobiles.Add(mobile);

                    for (int i = 0; i < mobiles.Count; i++)
                    {
                        Mobile m = mobiles[i];
						
                        if ((m.Z + 16) > this.m_Item.Z && (this.m_Item.Z + 12) > m.Z && (!Core.AOS || m != from) && SpellHelper.ValidIndirectTarget(from, m) && from.CanBeHarmful(m, false))
                        {
                            if (from != null)
                                from.DoHarmful(m);
							
                            AOS.Damage(m, from, this.m_Item.GetDamage(), 0, 100, 0, 0, 0);
                            m.PlaySound(0x208);
                        }
                    }
                }
            }
        }
    }
}