using System;

namespace Server.Items 
{
    public class BulgingMuseumBag : BaseRewardBag
    {
        [Constructable]
        public BulgingMuseumBag()
        {
            DropItem(new Gold(10000));

            switch (Utility.Random(9))
            {
                case 0:
                    DropItem(new Amber(10));
                    break;
                case 1:
                    DropItem(new Amethyst(10));
                    break;
                case 2:
                    DropItem(new Citrine(10));
                    break;
                case 3:
                    DropItem(new Ruby(10));
                    break;
                case 4:
                    DropItem(new Emerald(10));
                    break;
                case 5:
                    DropItem(new Diamond(10));
                    break;
                case 6:
                    DropItem(new Sapphire(10));
                    break;
                case 7:
                    DropItem(new StarSapphire(10));
                    break;
                case 8:
                    DropItem(new Tourmaline(10));
                    break;
            }

            switch (Utility.Random(5))
            {
                case 0:
                    DropItem(new ElvenFletching(20));
                    break;
                case 1:
                    DropItem(new RelicFragment(20));
                    break;
                case 2:
                    DropItem(new DelicateScales(20));
                    break;
                case 3:
                    DropItem(new ChagaMushroom(20));
                    break;
                case 4:
                    DropItem(new FeyWings(20));
                    break;
            }
        }

        public BulgingMuseumBag(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112995;
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