using System;

namespace Server.Items
{
    public class WaterloggedBoots : BaseShoes
    {
        [Constructable]
        public WaterloggedBoots()
            : base(0x1711)
        {
            if (Utility.RandomBool())
            {
                // thigh boots
                this.ItemID = 0x1711;
                this.Weight = 4.0;
            }
            else
            {
                // boots
                this.ItemID = 0x170B;
                this.Weight = 3.0;
            }
        }

        public WaterloggedBoots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074364;
            }
        }// Waterlogged boots
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1073634); // An aquarium decoration
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