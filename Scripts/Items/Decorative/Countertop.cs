using System;

namespace Server.Items
{
    [Furniture]
    public class Countertop : Item
    {
        public override int LabelNumber { get { return 1125714; } } // countertop

        [Constructable]
        public Countertop()
            : base(0xA2DA)
        {
            LootType = LootType.Blessed;
        }

        public Countertop(Serial serial)
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
