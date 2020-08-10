#region References
using Server.Network;
using Server.Spells;
using Server.Targeting;
using System;
using System.Linq;
#endregion

namespace Server.Items
{
    public abstract class BaseExplosionPotion : BasePotion
    {
        private const int ExplosionRange = 2; // How long is the blast radius?
        private Timer m_Timer;

        public BaseExplosionPotion(PotionEffect effect)
            : base(0xF0D, effect)
        { }

        public BaseExplosionPotion(Serial serial)
            : base(serial)
        { }

        public abstract int MinDamage { get; }
        public abstract int MaxDamage { get; }
        public override bool RequireFreeHand => false;

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

        public virtual object FindParent(Mobile from)
        {
            Mobile m = HeldBy;

            if (m != null && m.Holding == this)
            {
                return m;
            }

            object obj = RootParent;

            if (obj != null)
            {
                return obj;
            }

            if (Map == Map.Internal)
            {
                return from;
            }

            return this;
        }

        public override void Drink(Mobile from)
        {
            if (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting))
            {
                from.SendLocalizedMessage(1062725); // You can not use a purple potion while paralyzed.
                return;
            }

            ThrowTarget targ = from.Target as ThrowTarget;
            Stackable = false; // Scavenged explosion potions won't stack with those ones in backpack, and still will explode.

            if (targ != null && targ.Potion == this)
            {
                return;
            }

            from.RevealingAction();
            from.Target = new ThrowTarget(this);

            if (m_Timer == null)
            {
                from.SendLocalizedMessage(500236); // You should throw it now!

                m_Timer = Timer.DelayCall(
                        TimeSpan.FromSeconds(1.0),
                        TimeSpan.FromSeconds(1.25),
                        5,
                        new TimerStateCallback(Detonate_OnTick),
                        new object[] { from, 3 }); // 3.6 seconds explosion delay
            }
        }

        public void Explode(Mobile from, bool direct, Point3D loc, Map map)
        {
            if (Deleted || from == null)
            {
                return;
            }

            bool damageThrower = false;

            if (from.Target is ThrowTarget && ((ThrowTarget)from.Target).Potion == this)
            {
                Target.Cancel(from);
            }

            if (IsChildOf(from.Backpack) || Parent == from)
            {
                damageThrower = true;
            }

            Consume();

            if (map == null)
            {
                return;
            }

            Effects.PlaySound(loc, map, 0x307);

            Effects.SendLocationEffect(loc, map, 0x36B0, 9, 10, 0, 0);
            int alchemyBonus = 0;

            if (direct)
            {
                alchemyBonus = (int)(from.Skills.Alchemy.Value / 5);
            }

            int min = Scale(from, MinDamage);
            int max = Scale(from, MaxDamage);

            System.Collections.Generic.List<Mobile> list = SpellHelper.AcquireIndirectTargets(from, loc, map, ExplosionRange, false).OfType<Mobile>().ToList();

            if (damageThrower && !list.Contains(from))
            {
                list.Add(from);
            }

            foreach (Mobile m in list)
            {
                from.DoHarmful(m);

                int damage = Utility.RandomMinMax(min, max);

                damage += alchemyBonus;

                if (list.Count > 2)
                {
                    damage /= list.Count - 1;
                }

                AOS.Damage(m, from, damage, 0, 100, 0, 0, 0, Server.DamageType.SpellAOE);
            }

            list.Clear();
        }

        private void Detonate_OnTick(object state)
        {
            if (Deleted)
            {
                return;
            }

            object[] states = (object[])state;
            Mobile from = (Mobile)states[0];
            int timer = (int)states[1];

            object parent = FindParent(from);

            if (timer == 0)
            {
                Point3D loc;
                Map map;

                if (parent is Item)
                {
                    Item item = (Item)parent;

                    loc = item.GetWorldLocation();
                    map = item.Map;
                }
                else if (parent is Mobile)
                {
                    Mobile m = (Mobile)parent;

                    loc = m.Location;
                    map = m.Map;
                }
                else
                {
                    return;
                }

                Explode(from, true, loc, map);
                m_Timer = null;
            }
            else
            {
                if (parent is Item)
                {
                    ((Item)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }
                else if (parent is Mobile)
                {
                    ((Mobile)parent).PublicOverheadMessage(MessageType.Regular, 0x22, false, timer.ToString());
                }

                states[1] = timer - 1;
            }
        }

        private void Reposition_OnTick(object state)
        {
            if (Deleted)
            {
                return;
            }

            object[] states = (object[])state;
            Mobile from = (Mobile)states[0];
            Point3D p = (Point3D)states[1];
            Map map = (Map)states[2];

            MoveToWorld(p, map);
        }

        private class ThrowTarget : Target
        {
            private readonly BaseExplosionPotion m_Potion;

            public ThrowTarget(BaseExplosionPotion potion)
                : base(12, true, TargetFlags.None)
            {
                m_Potion = potion;
            }

            public BaseExplosionPotion Potion => m_Potion;

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Potion.Deleted || m_Potion.Map == Map.Internal)
                {
                    return;
                }

                IPoint3D p = targeted as IPoint3D;

                if (p == null)
                {
                    return;
                }

                Map map = from.Map;

                if (map == null)
                {
                    return;
                }

                SpellHelper.GetSurfaceTop(ref p);
                from.RevealingAction();
                IEntity to;

                var point = new Point3D(p);
                to = new Entity(Serial.Zero, point, map);

                Effects.SendMovingEffect(from, to, m_Potion.ItemID, 7, 0, false, false, m_Potion.Hue, 0);

                if (m_Potion.Amount > 1)
                {
                    Mobile.LiftItemDupe(m_Potion, 1);
                }

                m_Potion.Internalize();
                Timer.DelayCall(
                    TimeSpan.FromSeconds(1.0), new TimerStateCallback(m_Potion.Reposition_OnTick), new object[] { from, point, map });
            }
        }
    }
}
