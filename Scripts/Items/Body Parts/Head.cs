using System;

namespace Server.Items
{
    public enum HeadType
    {
        Regular,
        Duel,
        Tournament
    }

    public class Head : Item
    {
        private string m_PlayerName;
        private HeadType m_HeadType;
        [Constructable]
        public Head()
            : this(null)
        {
        }

        [Constructable]
        public Head(string playerName)
            : this(HeadType.Regular, playerName)
        {
        }

        [Constructable]
        public Head(HeadType headType, string playerName)
            : base(0x1DA0)
        {
            this.m_HeadType = headType;
            this.m_PlayerName = playerName;
        }

        public Head(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string PlayerName
        {
            get
            {
                return this.m_PlayerName;
            }
            set
            {
                this.m_PlayerName = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public HeadType HeadType
        {
            get
            {
                return this.m_HeadType;
            }
            set
            {
                this.m_HeadType = value;
            }
        }
        public override string DefaultName
        {
            get
            {
                if (this.m_PlayerName == null)
                    return base.DefaultName;

                switch ( this.m_HeadType )
                {
                    default:
                        return String.Format("the head of {0}", this.m_PlayerName);

                    case HeadType.Duel:
                        return String.Format("the head of {0}, taken in a duel", this.m_PlayerName);

                    case HeadType.Tournament:
                        return String.Format("the head of {0}, taken in a tournament", this.m_PlayerName);
                }
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((string)this.m_PlayerName);
            writer.WriteEncodedInt((int)this.m_HeadType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    this.m_PlayerName = reader.ReadString();
                    this.m_HeadType = (HeadType)reader.ReadEncodedInt();
                    break;
                case 0:
                    string format = this.Name;

                    if (format != null)
                    {
                        if (format.StartsWith("the head of "))
                            format = format.Substring("the head of ".Length);

                        if (format.EndsWith(", taken in a duel"))
                        {
                            format = format.Substring(0, format.Length - ", taken in a duel".Length);
                            this.m_HeadType = HeadType.Duel;
                        }
                        else if (format.EndsWith(", taken in a tournament"))
                        {
                            format = format.Substring(0, format.Length - ", taken in a tournament".Length);
                            this.m_HeadType = HeadType.Tournament;
                        }
                    }

                    this.m_PlayerName = format;
                    this.Name = null;

                    break;
            }
        }
    }
}