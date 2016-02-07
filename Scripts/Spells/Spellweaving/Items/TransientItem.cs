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
            this.m_CreationTime = DateTime.UtcNow;
            this.m_LifeSpan = lifeSpan;

            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), new TimerCallback(CheckExpiry));
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
                return this.m_LifeSpan;
            }
            set
            {
                this.m_LifeSpan = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime CreationTime
        {
            get
            {
                return this.m_CreationTime;
            }
            set
            {
                this.m_CreationTime = value;
            }
        }
        public override bool Nontransferable
        {
            get
            {
                return true;
            }
        }
        public virtual TextDefinition InvalidTransferMessage
        {
            get
            {
                return null;
            }
        }
        public override void HandleInvalidTransfer(Mobile from)
        {
            if (this.InvalidTransferMessage != null)
                TextDefinition.SendMessageTo(from, this.InvalidTransferMessage);

            this.Delete();
        }

        public virtual void Expire(Mobile parent)
        {
            if (parent != null)
                parent.SendLocalizedMessage(1072515, (this.Name == null ? String.Format("#{0}", this.LabelNumber) : this.Name)); // The ~1_name~ expired...

            Effects.PlaySound(this.GetWorldLocation(), this.Map, 0x201);

            this.Delete();
        }

        public virtual void SendTimeRemainingMessage(Mobile to)
        {
            to.SendLocalizedMessage(1072516, String.Format("{0}\t{1}", (this.Name == null ? String.Format("#{0}", this.LabelNumber) : this.Name), (int)this.m_LifeSpan.TotalSeconds)); // ~1_name~ will expire in ~2_val~ seconds!
        }

        public override void OnDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            base.OnDelete();
        }

        public virtual void CheckExpiry()
        {
            if ((this.m_CreationTime + this.m_LifeSpan) < DateTime.UtcNow)
                this.Expire(this.RootParent as Mobile);
            else
                this.InvalidateProperties();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            TimeSpan remaining = ((this.m_CreationTime + this.m_LifeSpan) - DateTime.UtcNow);

            list.Add(1072517, ((int)remaining.TotalSeconds).ToString()); // Lifespan: ~1_val~ seconds
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(this.m_LifeSpan);
            writer.Write(this.m_CreationTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_LifeSpan = reader.ReadTimeSpan();
            this.m_CreationTime = reader.ReadDateTime();

            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), new TimerCallback(CheckExpiry));
        }
    }
}