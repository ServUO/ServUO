using System;

namespace Server.Items
{
    public class PortraitOfTheBride : Item
    {
        [Constructable]
        public PortraitOfTheBride()
            : base(0xE9F)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 10.0;
        }

        public PortraitOfTheBride(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075300;
            }
        }// Portrait of the Bride
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