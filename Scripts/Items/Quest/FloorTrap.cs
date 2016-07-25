using System;
using System.Collections.Generic;
using System.Text;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
    public class FloorTrap : BaseTrap
    {
        public override int LabelNumber { get { return 1113296; } } // Armed Floor Trap.

        private Timer m_Concealing;
        private Timer m_Expire;
        private double m_TinkerLevel;
        private double m_HidingLevel;
        private PlayerMobile m_Owner;

        public TimeSpan ConcealPeriod { get { return TimeSpan.FromSeconds(30.0); } }
        public TimeSpan ExpirePeriod { get { return TimeSpan.FromHours(2.0); } }
        public double HidingLevel { get { return m_HidingLevel * 0.8; } }
        public double TinkerLevel { get { return m_TinkerLevel * 0.8; } }

        public override bool PassivelyTriggered { get { return false; } }
        public override TimeSpan PassiveTriggerDelay { get { return TimeSpan.Zero; } }
        public override int PassiveTriggerRange { get { return 0; } }
        public override TimeSpan ResetDelay { get { return TimeSpan.FromSeconds(0.0); } }


        [Constructable]
        public FloorTrap(double tinkering, double hiding, Mobile from)
            : base(0x4004)
        {
            this.m_TinkerLevel = tinkering;
            this.m_HidingLevel = hiding;
            this.m_Owner = (PlayerMobile)from;

            BeginExpire();
        }

        [Constructable]
        public FloorTrap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_TinkerLevel);
            writer.Write(m_HidingLevel);
            writer.Write(m_Owner);

            if (Visible)
                BeginConceal();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_TinkerLevel = reader.ReadDouble();
            m_HidingLevel = reader.ReadDouble();
            m_Owner = (PlayerMobile)reader.ReadMobile();

            if (Visible)
                BeginConceal();
        }

        public virtual void BeginConceal()
        {
            if (m_Concealing != null)
                m_Concealing.Stop();

            m_Concealing = Timer.DelayCall(ConcealPeriod, new TimerCallback(Conceal));
        }

        public virtual void Conceal()
        {
            if (m_Concealing != null)
                m_Concealing.Stop();

            m_Concealing = null;

            if (!Deleted)
                Visible = false;
        }

        public void BeginExpire()
        {
            m_Expire = Timer.DelayCall(ExpirePeriod, new TimerCallback(Expire));
        }

        public void Expire()
        {
            if (m_Concealing != null)
                m_Concealing.Stop();
            if (m_Expire != null)
                m_Expire.Stop();

            this.Delete();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            m_Owner.FloorTrapsPlaced--;
        }

        public override void OnTrigger(Mobile from)
        {
            if (from.IsPlayer() || from.YellowHealthbar)
            {
                if (m_Concealing == null && Visible)
                    Visible = false;

                return;
            }

            int minDamage = (int)Math.Ceiling(50 * m_TinkerLevel / 100);
            int maxDamage = (int)Math.Ceiling(70 * m_TinkerLevel / 100);

            AOS.Damage(from, Utility.RandomMinMax(minDamage, maxDamage), 100, 0, 0, 0, 0);

            Effects.PlaySound(from.Location, from.Map, 0x22B);
            m_Owner.RevealingAction();

            Expire();
        }

        internal void ExecuteTrap(Mobile from)
        {
            int minDamage = (int)Math.Ceiling(50 * m_TinkerLevel / 100);
            int maxDamage = (int)Math.Ceiling(70 * m_TinkerLevel / 100);

            AOS.Damage(from, Utility.RandomMinMax(minDamage, maxDamage), 100, 0, 0, 0, 0);

            Effects.PlaySound(from.Location, from.Map, 0x22B);

            m_Owner.RevealingAction();

            Expire();
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m.Location == oldLocation)
                return;

            if (m.IsPlayer() || m.YellowHealthbar)
                return;

            if (m is BaseCreature)
            {
                if (((BaseCreature)m).Controlled)
                    return;
            }

            if (CheckRange(m.Location, oldLocation, 0))
            {
                if (RevealOnTrigger)
                    this.Visible = true;

                OnTrigger(m);
            }
        }

        public void FailedRemoveTrapAttempt()
        {
            if (m_Owner != null)
                m_Owner.SendLocalizedMessage(1113310);
        }
    }
}
