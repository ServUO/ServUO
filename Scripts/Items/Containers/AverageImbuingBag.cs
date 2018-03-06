using System;

namespace Server.Items
{
    public class AverageImbuingBag : BaseRewardBag
    {
        [Constructable]
        public AverageImbuingBag()
        {

            switch (Utility.Random(2))
            {
                case 0:
                    DropItem(new SpiderCarapace());
                    break;
                case 1:
                    DropItem(new SilverSnakeSkin());
                    break;
            }
        }

        public AverageImbuingBag(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113768; //Average Imbuing Bag
            }
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