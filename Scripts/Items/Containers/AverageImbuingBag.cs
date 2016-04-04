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
                    this.AddItem(new SpiderCarapace());
                    break;
                case 1:
                    this.AddItem(new SilverSnakeSkin());
                    break;
               /* case 0:
                    this.AddItem(new CrystalShards());
                    break;
                case 1:
                    this.AddItem(new CrushedGlass());
                    break;
                case 2:
                    this.AddItem(new SeedRenewal());
                    break;
                case 3:
                    this.AddItem(new ElvenFletchings());
                    break;
                case 4:
                    this.AddItem(new ArcanicRuneStone());
                    break;
                case 5:
                    this.AddItem(new CrystallineBlackrock());
                    break;
                case 6:
                    this.AddItem(new DelicateScales());
                    break;*/
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