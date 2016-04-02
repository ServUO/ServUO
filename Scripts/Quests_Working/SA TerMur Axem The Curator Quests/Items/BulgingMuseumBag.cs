using System;

namespace Server.Items 
{
    public class BulgingMuseumBag : BaseRewardBag
    {
        [Constructable]
        public BulgingMuseumBag()
        {
            this.AddItem(new Gold(10000));

            switch (Utility.Random(9))
            {
                case 0:
                    this.AddItem(new Amber(10));
                    break;
                case 1:
                    this.AddItem(new Amethyst(10));
                    break;
                case 2:
                    this.AddItem(new Citrine(10));
                    break;
                case 3:
                    this.AddItem(new Ruby(10));
                    break;
                case 4:
                    this.AddItem(new Emerald(10));
                    break;
                case 5:
                    this.AddItem(new Diamond(10));
                    break;
                case 6:
                    this.AddItem(new Sapphire(10));
                    break;
                case 7:
                    this.AddItem(new StarSapphire(10));
                    break;
                case 8:
                    this.AddItem(new Tourmaline(10));
                    break;
            }

            switch (Utility.Random(5))
            {
                case 0:
                    this.AddItem(new ElvenFletchings(20));
                    break;
                case 1:
                    this.AddItem(new RelicFragment(20));
                    break;
                case 2:
                    this.AddItem(new DelicateScales(20));
                    break;
                case 3:
                    this.AddItem(new ChagaMushroom(20));
                    break;
                case 4:
                    this.AddItem(new FeyWings(20));
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