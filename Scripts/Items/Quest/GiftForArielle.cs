using System;

namespace Server.Items
{
    public class GiftForArielle : Item
    {
        [Constructable]
        public GiftForArielle()
            : base(0x1882)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1;
            this.Hue = 0x2C4;
        }

        public GiftForArielle(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074356;
            }
        }// gift for arielle
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