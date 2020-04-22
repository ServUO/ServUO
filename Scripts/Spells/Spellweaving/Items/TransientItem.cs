using System;

namespace Server.Items
{
    public class TransientItem : Item
    {
        private TimeSpan m_LifeSpan;
        private DateTime m_CreationTime;
        private Timer m_Timer;
        [Constructable]
        public TransientItem(int itemID, TimeSpan lifeSpan)
            : base(itemID)
        {
            m_CreationTime = DateTime.UtcNow;
            m_LifeSpan = lifeSpan;

            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), CheckExpiry);
        }

        public TransientItem(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan LifeSpan
        {
            get
            {
                return m_LifeSpan;
            }
            set
            {
                m_LifeSpan = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime CreationTime
        {
            get
            {
                return m_CreationTime;
            }
            set
            {
                m_CreationTime = value;
            }
        }
        public override bool Nontransferable => true;
        public virtual TextDefinition InvalidTransferMessage => null;
        public override void HandleInvalidTransfer(Mobile from)
        {
            if (InvalidTransferMessage != null)
                TextDefinition.SendMessageTo(from, InvalidTransferMessage);

            Delete();
        }

        public virtual void Expire(Mobile parent)
        {
            if (parent != null)
                parent.SendLocalizedMessage(1072515, (Name == null ? String.Format("#{0}", LabelNumber) : Name)); // The ~1_name~ expired...

            Effects.PlaySound(GetWorldLocation(), Map, 0x201);

            Delete();
        }

        public virtual void SendTimeRemainingMessage(Mobile to)
        {
            to.SendLocalizedMessage(1072516, String.Format("{0}\t{1}", (Name == null ? String.Format("#{0}", LabelNumber) : Name), (int)m_LifeSpan.TotalSeconds)); // ~1_name~ will expire in ~2_val~ seconds!
        }

        public override void OnDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            base.OnDelete();
        }

        public virtual void CheckExpiry()
        {
            if ((m_CreationTime + m_LifeSpan) < DateTime.UtcNow)
                Expire(RootParent as Mobile);
            else
                InvalidateProperties();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            TimeSpan remaining = ((m_CreationTime + m_LifeSpan) - DateTime.UtcNow);

            list.Add(1072517, ((int)remaining.TotalSeconds).ToString()); // Lifespan: ~1_val~ seconds
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_LifeSpan);
            writer.Write(m_CreationTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_LifeSpan = reader.ReadTimeSpan();
            m_CreationTime = reader.ReadDateTime();

            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), CheckExpiry);
        }
    }
}
