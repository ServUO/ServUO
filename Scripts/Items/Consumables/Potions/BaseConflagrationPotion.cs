using Server.Spells;
using Server.Targeting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public abstract class BaseConflagrationPotion : BasePotion
    {
        public abstract int MinDamage { get; }
        public abstract int MaxDamage { get; }

        public override bool RequireFreeHand => false;

        public BaseConflagrationPotion(PotionEffect effect)
            : base(0xF06, effect)
        {
            Hue = 0x489;
        }

        public BaseConflagrationPotion(Serial serial)
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

            // Effects
            Effects.PlaySound(loc, map, 0x20C);

            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    Point3D p = new Point3D(loc.X + i, loc.Y + j, loc.Z);
                    SpellHelper.AdjustField(ref p, map, 16, true);

                    if (SpellHelper.CheckField(p, map) && map.LineOfSight(new Point3D(loc.X, loc.Y, loc.Z + 14), p))
                        new InternalItem(from, p, map, MinDamage, MaxDamage);
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

            public BaseConflagrationPotion Potion => m_Potion;

            public ThrowTarget(BaseConflagrationPotion potion)
                : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Potion.Deleted || m_Potion.Map == Map.Internal)
                    return;

                IPoint3D p = targeted as IPoint3D;

                if (p == null || from.Map == null)
                    return;

                // Add delay
                if (from.AccessLevel == AccessLevel.Player)
                {
                    AddDelay(from);
                }

                SpellHelper.GetSurfaceTop(ref p);

                from.RevealingAction();

                IEntity to;

                if (p is Mobile)
                    to = (Mobile)p;
                else
                    to = new Entity(Serial.Zero, new Point3D(p), from.Map);

                Effects.SendMovingEffect(from, to, 0xF0D, 7, 0, false, false, m_Potion.Hue, 0);
                Timer.DelayCall(TimeSpan.FromSeconds(1.5), new TimerStateCallback(m_Potion.Explode_Callback), new object[] { from, new Point3D(p), from.Map });
            }
        }

        public class InternalItem : Item
        {
            private Mobile m_From;
            private int m_MinDamage;
            private int m_MaxDamage;
            private DateTime m_End;
            private Timer m_Timer;

            public Mobile From => m_From;

            public override bool BlocksFit => true;

            public InternalItem(Mobile from, Point3D loc, Map map, int min, int max)
                : base(0x398C)
            {
                Movable = false;
                Light = LightType.Circle300;

                MoveToWorld(loc, map);

                m_From = from;
                m_End = DateTime.UtcNow + TimeSpan.FromSeconds(10);

                SetDamage(min, max);

                m_Timer = new InternalTimer(this, m_End);
                m_Timer.Start();
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (m_Timer != null)
                    m_Timer.Stop();
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public int GetDamage()
            {
                return Utility.RandomMinMax(m_MinDamage, m_MaxDamage);
            }

            private void SetDamage(int min, int max)
            {
                /* 	new way to apply alchemy bonus according to Stratics' calculator.
                this gives a mean to values 25, 50, 75 and 100. Stratics' calculator is outdated.
                Those goals will give 2 to alchemy bonus. It's not really OSI-like but it's an approximation. */
                m_MinDamage = min;
                m_MaxDamage = max;

                if (m_From == null)
                    return;

                int alchemySkill = m_From.Skills.Alchemy.Fixed;
                int alchemyBonus = alchemySkill / 125 + alchemySkill / 250;

                m_MinDamage = Scale(m_From, m_MinDamage + alchemyBonus);
                m_MaxDamage = Scale(m_From, m_MaxDamage + alchemyBonus);
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write(0); // version

                writer.Write(m_From);
                writer.Write(m_End);
                writer.Write(m_MinDamage);
                writer.Write(m_MaxDamage);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                m_From = reader.ReadMobile();
                m_End = reader.ReadDateTime();
                m_MinDamage = reader.ReadInt();
                m_MaxDamage = reader.ReadInt();

                m_Timer = new InternalTimer(this, m_End);
                m_Timer.Start();
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (Visible && m_From != null && m != m_From && SpellHelper.ValidIndirectTarget(m_From, m) && m_From.CanBeHarmful(m, false))
                {
                    m_From.DoHarmful(m);

                    AOS.Damage(m, m_From, GetDamage(), 0, 100, 0, 0, 0);
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
                    m_Item = item;
                    m_End = end;

                    Priority = TimerPriority.FiftyMS;
                }

                protected override void OnTick()
                {
                    if (m_Item.Deleted)
                        return;

                    if (DateTime.UtcNow > m_End)
                    {
                        m_Item.Delete();
                        Stop();
                        return;
                    }

                    Mobile from = m_Item.From;

                    if (m_Item.Map == null || from == null)
                        return;

                    List<Mobile> mobiles = new List<Mobile>();
                    IPooledEnumerable eable = m_Item.GetMobilesInRange(0);

                    foreach (Mobile mobile in eable)
                        mobiles.Add(mobile);
                    eable.Free();

                    for (int i = 0; i < mobiles.Count; i++)
                    {
                        Mobile m = mobiles[i];

                        if ((m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && m != from && SpellHelper.ValidIndirectTarget(from, m) && from.CanBeHarmful(m, false))
                        {
                            if (from != null)
                                from.DoHarmful(m);

                            AOS.Damage(m, from, m_Item.GetDamage(), 0, 100, 0, 0, 0);
                            m.PlaySound(0x208);
                        }
                    }

                    ColUtility.Free(mobiles);
                }
            }
        }
    }
}
