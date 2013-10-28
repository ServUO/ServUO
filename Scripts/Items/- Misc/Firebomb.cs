using System;
using System.Collections.Generic;
using Server.Network;
using Server.Spells;
using Server.Targeting;

namespace Server.Items
{
    public class Firebomb : Item
    {
        private Timer m_Timer;
        private int m_Ticks = 0;
        private Mobile m_LitBy;
        private List<Mobile> m_Users;
        [Constructable]
        public Firebomb()
            : this(0x99B)
        {
        }

        [Constructable]
        public Firebomb(int itemID)
            : base(itemID)
        {
            //Name = "a firebomb";
            this.Weight = 2.0;
            this.Hue = 1260;
        }

        public Firebomb(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            if (Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)))
            {
                // to prevent exploiting for pvp
                from.SendLocalizedMessage(1075857); // You cannot use that while paralyzed.
                return;
            }

            if (this.m_Timer == null)
            {
                this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), new TimerCallback(OnFirebombTimerTick));
                this.m_LitBy = from;
                from.SendLocalizedMessage(1060582); // You light the firebomb.  Throw it now!
            }
            else
                from.SendLocalizedMessage(1060581); // You've already lit it!  Better throw it now!

            if (this.m_Users == null)
                this.m_Users = new List<Mobile>();

            if (!this.m_Users.Contains(from))
                this.m_Users.Add(from);

            from.Target = new ThrowTarget(this);
        }

        private void OnFirebombTimerTick()
        {
            if (this.Deleted)
            {
                this.m_Timer.Stop();
                return;
            }

            if (this.Map == Map.Internal && this.HeldBy == null)
                return;

            switch ( this.m_Ticks )
            {
                case 0:
                case 1:
                case 2:
                    {
                        ++this.m_Ticks;

                        if (this.HeldBy != null)
                            this.HeldBy.PublicOverheadMessage(MessageType.Regular, 957, false, this.m_Ticks.ToString());
                        else if (this.RootParent == null)
                            this.PublicOverheadMessage(MessageType.Regular, 957, false, this.m_Ticks.ToString());
                        else if (this.RootParent is Mobile)
                            ((Mobile)this.RootParent).PublicOverheadMessage(MessageType.Regular, 957, false, this.m_Ticks.ToString());

                        break;
                    }
                default:
                    {
                        if (this.HeldBy != null)
                            this.HeldBy.DropHolding();

                        if (this.m_Users != null)
                        {
                            foreach (Mobile m in this.m_Users)
                            {
                                ThrowTarget targ = m.Target as ThrowTarget;

                                if (targ != null && targ.Bomb == this)
                                    Target.Cancel(m);
                            }

                            this.m_Users.Clear();
                            this.m_Users = null;
                        }

                        if (this.RootParent is Mobile)
                        {
                            Mobile parent = (Mobile)this.RootParent;
                            parent.SendLocalizedMessage(1060583); // The firebomb explodes in your hand!
                            AOS.Damage(parent, Utility.Random(3) + 4, 0, 100, 0, 0, 0);
                        }
                        else if (this.RootParent == null)
                        {
                            List<Mobile> toDamage = new List<Mobile>();
                            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 1);

                            foreach (Mobile m in eable)
                            {
                                toDamage.Add(m);
                            }
                            eable.Free();

                            Mobile victim;
                            for (int i = 0; i < toDamage.Count; ++i)
                            {
                                victim = toDamage[i];

                                if (this.m_LitBy == null || (SpellHelper.ValidIndirectTarget(this.m_LitBy, victim) && this.m_LitBy.CanBeHarmful(victim, false)))
                                {
                                    if (this.m_LitBy != null)
                                        this.m_LitBy.DoHarmful(victim);

                                    AOS.Damage(victim, this.m_LitBy, Utility.Random(3) + 4, 0, 100, 0, 0, 0);
                                }
                            }
                            (new FirebombField(this.m_LitBy, toDamage)).MoveToWorld(this.Location, this.Map);
                        }

                        this.m_Timer.Stop();
                        this.Delete();
                        break;
                    }
            }
        }

        private void OnFirebombTarget(Mobile from, object obj)
        {
            if (this.Deleted || this.Map == Map.Internal || !this.IsChildOf(from.Backpack))
                return;

            IPoint3D p = obj as IPoint3D;

            if (p == null)
                return;

            SpellHelper.GetSurfaceTop(ref p);

            from.RevealingAction();

            IEntity to;

            if (p is Mobile)
                to = (Mobile)p;
            else
                to = new Entity(Serial.Zero, new Point3D(p), this.Map);

            Effects.SendMovingEffect(from, to, this.ItemID, 7, 0, false, false, this.Hue, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(FirebombReposition_OnTick), new object[] { p, this.Map });
            this.Internalize();
        }

        private void FirebombReposition_OnTick(object state)
        {
            if (this.Deleted)
                return;

            object[] states = (object[])state;
            IPoint3D p = (IPoint3D)states[0];
            Map map = (Map)states[1];

            this.MoveToWorld(new Point3D(p), map);
        }

        private class ThrowTarget : Target
        {
            private readonly Firebomb m_Bomb;
            public ThrowTarget(Firebomb bomb)
                : base(12, true, TargetFlags.None)
            {
                this.m_Bomb = bomb;
            }

            public Firebomb Bomb
            {
                get
                {
                    return this.m_Bomb;
                }
            }
            protected override void OnTarget(Mobile from, object targeted)
            {
                this.m_Bomb.OnFirebombTarget(from, targeted);
            }
        }
    }

    public class FirebombField : Item
    {
        private readonly List<Mobile> m_Burning;
        private readonly Timer m_Timer;
        private readonly Mobile m_LitBy;
        private readonly DateTime m_Expire;
        public FirebombField(Mobile litBy, List<Mobile> toDamage)
            : base(0x376A)
        {
            this.Movable = false;
            this.m_LitBy = litBy;
            this.m_Expire = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            this.m_Burning = toDamage;
            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), new TimerCallback(OnFirebombFieldTimerTick));
        }

        public FirebombField(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            // Don't serialize these...
        }

        public override void Deserialize(GenericReader reader)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (this.ItemID == 0x398C && this.m_LitBy == null || (SpellHelper.ValidIndirectTarget(this.m_LitBy, m) && this.m_LitBy.CanBeHarmful(m, false)))
            {
                if (this.m_LitBy != null)
                    this.m_LitBy.DoHarmful(m);

                AOS.Damage(m, this.m_LitBy, 2, 0, 100, 0, 0, 0);
                m.PlaySound(0x208);

                if (!this.m_Burning.Contains(m))
                    this.m_Burning.Add(m);
            }

            return true;
        }

        private void OnFirebombFieldTimerTick()
        {
            if (this.Deleted)
            {
                this.m_Timer.Stop();
                return;
            }

            if (this.ItemID == 0x376A)
            {
                this.ItemID = 0x398C;
                return;
            }

            Mobile victim;
            for (int i = 0; i < this.m_Burning.Count;)
            {
                victim = this.m_Burning[i];

                if (victim.Location == this.Location && victim.Map == this.Map && (this.m_LitBy == null || (SpellHelper.ValidIndirectTarget(this.m_LitBy, victim) && this.m_LitBy.CanBeHarmful(victim, false))))
                {
                    if (this.m_LitBy != null)
                        this.m_LitBy.DoHarmful(victim);

                    AOS.Damage(victim, this.m_LitBy, Utility.Random(3) + 4, 0, 100, 0, 0, 0);
                    ++i;
                }
                else
                    this.m_Burning.RemoveAt(i);
            }

            if (DateTime.UtcNow >= this.m_Expire)
            {
                this.m_Timer.Stop();
                this.Delete();
            }
        }
    }
}