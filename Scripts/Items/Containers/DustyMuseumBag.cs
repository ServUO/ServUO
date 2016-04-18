using System;

namespace Server.Items
{
    public class DustyMuseumBag : BaseRewardBag
    {
        [Constructable]
        public DustyMuseumBag()
        {
            this.AddItem(new Gold(6000));

            switch (Utility.Random(9))
            {
                case 0:
                    this.AddItem(new Amber(5));
                    break;
                case 1:
                    this.AddItem(new Amethyst(5));
                    break;
                case 2:
                    this.AddItem(new Citrine(5));
                    break;
                case 3:
                    this.AddItem(new Ruby(5));
                    break;
                case 4:
                    this.AddItem(new Emerald(5));
                    break;
                case 5:
                    this.AddItem(new Diamond(5));
                    break;
                case 6:
                    this.AddItem(new Sapphire(5));
                    break;
                case 7:
                    this.AddItem(new StarSapphire(5));
                    break;
                case 8:
                    this.AddItem(new Tourmaline(5));
                    break;
            }

            switch (Utility.Random(4))
            {
                case 0:
                    this.AddItem(new MagicalResidue(10));
                    break;
                case 1:
                    this.AddItem(new RelicFragment(10));
                    break;
                case 2:
                    this.AddItem(new DelicateScales(10));
                    break;
                case 3:
                    this.AddItem(new ChagaMushroom(10));
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