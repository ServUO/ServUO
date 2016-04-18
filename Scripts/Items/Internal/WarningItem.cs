using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Items
{
    public class WarningItem : Item
    {
        private string m_WarningString;
        private int m_WarningNumber;
        private int m_Range;
        private TimeSpan m_ResetDelay;
        private bool m_Broadcasting;
        private DateTime m_LastBroadcast;
        [Constructable]
        public WarningItem(int itemID, int range, int warning)
            : base(itemID)
        {
            if (range > 18)
                range = 18;

            this.Movable = false;

            this.m_WarningNumber = warning;
            this.m_Range = range;
        }

        [Constructable]
        public WarningItem(int itemID, int range, string warning)
            : base(itemID)
        {
            if (range > 18)
                range = 18;

            this.Movable = false;

            this.m_WarningString = warning;
            this.m_Range = range;
        }

        public WarningItem(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string WarningString
        {
            get
            {
                return this.m_WarningString;
            }
            set
            {
                this.m_WarningString = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int WarningNumber
        {
            get
            {
                return this.m_WarningNumber;
            }
            set
            {
                this.m_WarningNumber = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Range
        {
            get
            {
                return this.m_Range;
            }
            set
            {
                if (value > 18)
                    value = 18;
                this.m_Range = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ResetDelay
        {
            get
            {
                return this.m_ResetDelay;
            }
            set
            {
                this.m_ResetDelay = value;
            }
        }
        public virtual bool OnlyToTriggerer
        {
            get
            {
                return false;
            }
        }
        public virtual int NeighborRange
        {
            get
            {
                return 5;
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }
        public virtual void SendMessage(Mobile triggerer, bool onlyToTriggerer, string messageString, int messageNumber)
        {
            if (onlyToTriggerer)
            {
                if (messageString != null)
                    triggerer.SendMessage(messageString);
                else
                    triggerer.SendLocalizedMessage(messageNumber);
            }
            else
            {
                if (messageString != null)
                    this.PublicOverheadMessage(MessageType.Regular, 0x3B2, false, messageString);
                else
                    this.PublicOverheadMessage(MessageType.Regular, 0x3B2, messageNumber);
            }
        }

        public virtual void Broadcast(Mobile triggerer)
        {
            if (this.m_Broadcasting || (DateTime.UtcNow < (this.m_LastBroadcast + this.m_ResetDelay)))
                return;

            this.m_LastBroadcast = DateTime.UtcNow;

            this.m_Broadcasting = true;

            this.SendMessage(triggerer, this.OnlyToTriggerer, this.m_WarningString, this.m_WarningNumber);

            if (this.NeighborRange >= 0)
            {
                List<WarningItem> list = new List<WarningItem>();

                foreach (Item item in this.GetItemsInRange(this.NeighborRange))
                {
                    if (item != this && item is WarningItem)
                        list.Add((WarningItem)item);
                }

                for (int i = 0; i < list.Count; i++)
                    list[i].Broadcast(triggerer);
            }

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(InternalCallback));
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Player && Utility.InRange(m.Location, this.Location, this.m_Range) && !Utility.InRange(oldLocation, this.Location, this.m_Range))
                this.Broadcast(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((string)this.m_WarningString);
            writer.Write((int)this.m_WarningNumber);
            writer.Write((int)this.m_Range);

            writer.Write((TimeSpan)this.m_ResetDelay);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_WarningString = reader.ReadString();
                        this.m_WarningNumber = reader.ReadInt();
                        this.m_Range = reader.ReadInt();
                        this.m_ResetDelay = reader.ReadTimeSpan();

                        break;
                    }
            }
        }

        private void InternalCallback()
        {
            this.m_Broadcasting = false;
        }
    }

    public class HintItem : WarningItem
    {
        private string m_HintString;
        private int m_HintNumber;
        [Constructable]
        public HintItem(int itemID, int range, int warning, int hint)
            : base(itemID, range, warning)
        {
            this.m_HintNumber = hint;
        }

        [Constructable]
        public HintItem(int itemID, int range, string warning, string hint)
            : base(itemID, range, warning)
        {
            this.m_HintString = hint;
        }

        public HintItem(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string HintString
        {
            get
            {
                return this.m_HintString;
            }
            set
            {
                this.m_HintString = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int HintNumber
        {
            get
            {
                return this.m_HintNumber;
            }
            set
            {
                this.m_HintNumber = value;
            }
        }
        public override bool OnlyToTriggerer
        {
            get
            {
                return true;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            this.SendMessage(from, true, this.m_HintString, this.m_HintNumber);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((string)this.m_HintString);
            writer.Write((int)this.m_HintNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_HintString = reader.ReadString();
                        this.m_HintNumber = reader.ReadInt();

                        break;
                    }
            }
        }
    }
}