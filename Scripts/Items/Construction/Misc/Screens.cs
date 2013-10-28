using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x24D0, 0x24D1, 0x24D2, 0x24D3, 0x24D4)]
    public class BambooScreen : Item
    {
        [Constructable]
        public BambooScreen()
            : base(0x24D0)
        {
            this.Weight = 20.0;
        }

        public BambooScreen(Serial serial)
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

    [Furniture]
    [Flipable(0x24CB, 0x24CC, 0x24CD, 0x24CE, 0x24CF)]
    public class ShojiScreen : Item
    {
        [Constructable]
        public ShojiScreen()
            : base(0x24CB)
        {
            this.Weight = 20.0;
        }

        public ShojiScreen(Serial serial)
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