using System;

namespace Server.Items
{
    public class ThorvaldsMedallion : Item
    {
        [Constructable]
        public ThorvaldsMedallion()
            : base(0x2AAA)
        {
            this.LootType = LootType.Blessed;
            this.Hue = 0x47F; // TODO check
        }

        public ThorvaldsMedallion(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074232;
            }
        }// Thorvald's Medallion
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