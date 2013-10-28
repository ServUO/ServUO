using System;

namespace Server.Items
{
    public class OrdersFromMinax : Item
    {
        [Constructable]
        public OrdersFromMinax()
            : base(0x2279)
        {
            this.LootType = LootType.Blessed;
        }

        public OrdersFromMinax(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074639;
            }
        }// Orders from Minax
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