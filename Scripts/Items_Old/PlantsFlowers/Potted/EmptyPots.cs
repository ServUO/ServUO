using System;

namespace Server.Items
{
    public class SmallEmptyPot : Item
    {
        [Constructable]
        public SmallEmptyPot()
            : base(0x11C6)
        {
            this.Weight = 100;
        }

        public SmallEmptyPot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LargeEmptyPot : Item
    {
        [Constructable]
        public LargeEmptyPot()
            : base(0x11C7)
        {
            this.Weight = 6;
        }

        public LargeEmptyPot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}