using System;

namespace Server.Items
{
    public class Coal : Item
    {
        [Constructable]
        public Coal()
            : base(0x19b9)
        {
            this.Stackable = false;
            this.LootType = LootType.Blessed;
            this.Hue = 0x965;
        }

        public Coal(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "Coal";
            }
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
    }

    public class BadCard : Item
    {
        [Constructable]
        public BadCard()
            : base(0x14ef)
        {
            int[] m_CardHues = new int[] { 0x45, 0x27, 0x3d0 };
            this.Hue = m_CardHues[Utility.Random(m_CardHues.Length)];
            this.Stackable = false;
            this.LootType = LootType.Blessed;
            this.Movable = true;
        }

        public BadCard(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041428;
            }
        }// Maybe next year youll get a better...
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
    }

    public class Spam : Food
    {
        [Constructable]
        public Spam()
            : base(0x1044)
        {
            this.Stackable = false;
            this.LootType = LootType.Blessed;
        }

        public Spam(Serial serial)
            : base(serial)
        {
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
    }
}