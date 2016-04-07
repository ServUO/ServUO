using System;

namespace Server.Items
{
    public class MirrorOfPurification : Item
    {
        [Constructable]
        public MirrorOfPurification()
            : base(0x1008)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 5.0;
            this.Hue = 0x530;
        }

        public MirrorOfPurification(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075304;
            }
        }// Mirror of Purification
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