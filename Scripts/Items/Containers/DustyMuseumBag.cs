using System;

namespace Server.Items
{
    public class DustyMuseumBag : BaseRewardBag
    {
        [Constructable]
        public DustyMuseumBag()
        {
            DropItem(new Gold(6000));

            switch (Utility.Random(9))
            {
                case 0:
                    DropItem(new Amber(5));
                    break;
                case 1:
                    DropItem(new Amethyst(5));
                    break;
                case 2:
                    DropItem(new Citrine(5));
                    break;
                case 3:
                    DropItem(new Ruby(5));
                    break;
                case 4:
                    DropItem(new Emerald(5));
                    break;
                case 5:
                    DropItem(new Diamond(5));
                    break;
                case 6:
                    DropItem(new Sapphire(5));
                    break;
                case 7:
                    DropItem(new StarSapphire(5));
                    break;
                case 8:
                    DropItem(new Tourmaline(5));
                    break;
            }

            switch (Utility.Random(4))
            {
                case 0:
                    DropItem(new MagicalResidue(10));
                    break;
                case 1:
                    DropItem(new RelicFragment(10));
                    break;
                case 2:
                    DropItem(new DelicateScales(10));
                    break;
                case 3:
                    DropItem(new ChagaMushroom(10));
                    break;
            }
        }

        public DustyMuseumBag(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112994;
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