using System;

namespace Server.Items
{
    public class TransparentHeart : GoldEarrings
    {
        [Constructable]
        public TransparentHeart()
            : base()
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1;
            this.Hue = 0x4AB;
        }

        public TransparentHeart(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075400;
            }
        }// Transparent Heart
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