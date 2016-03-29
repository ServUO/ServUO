using System;


namespace Server.Items
{
    [Furniture]
    [Flipable(0xB2D, 0xB2C)]
    public class WoodenBench : CraftableFurniture
    {
        [Constructable]
        public WoodenBench()
            : base(0xB2D)
        {
            this.Weight = 6;
        }

        public WoodenBench(Serial serial)
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