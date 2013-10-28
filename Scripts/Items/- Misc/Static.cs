using System;

namespace Server.Items
{
    public class Static : Item
    {
        public Static()
            : base(0x80)
        {
            this.Movable = false;
        }

        [Constructable]
        public Static(int itemID)
            : base(itemID)
        {
            this.Movable = false;
        }

        [Constructable]
        public Static(int itemID, int count)
            : this(Utility.Random(itemID, count))
        {
        }

        public Static(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 0)
                this.Weight = -1;
        }
    }

    public class LocalizedStatic : Static
    {
        private int m_LabelNumber;
        [Constructable]
        public LocalizedStatic(int itemID)
            : this(itemID, itemID < 0x4000 ? 1020000 + itemID : 1078872 + itemID)
        {
        }

        [Constructable]
        public LocalizedStatic(int itemID, int labelNumber)
            : base(itemID)
        {
            this.m_LabelNumber = labelNumber;
        }

        public LocalizedStatic(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Number
        {
            get
            {
                return this.m_LabelNumber;
            }
            set
            {
                this.m_LabelNumber = value;
                this.InvalidateProperties();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return this.m_LabelNumber;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)0); // version
            writer.WriteEncodedInt((int)this.m_LabelNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadByte();

            switch ( version )
            {
                case 0:
                    {
                        this.m_LabelNumber = reader.ReadEncodedInt();
                        break;
                    }
            }
        }
    }
}