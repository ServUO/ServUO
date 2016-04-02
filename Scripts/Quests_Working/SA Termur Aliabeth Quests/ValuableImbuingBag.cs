using System;

namespace Server.Items
{
    public class ValuableImbuingBag : BaseRewardBag
    {
        [Constructable]
        public ValuableImbuingBag()
        {

            switch (Utility.Random(2))
            {
                case 0:
                    this.AddItem(new UndyingFlesh());
                    break;
                case 1:
                    this.AddItem(new DaemonClaw());
                    break;
              /*  case 2:
                    this.AddItem(new SpiderCarapace());
                    break;
                case 3:
                    this.AddItem(new VialOfVitriol());
                    break;
                case 4:
                    this.AddItem(new LavaSerpenCrust());
                    break;
                case 5:
                    this.AddItem(new GoblinBlood());
                    break;*/
            }
        }

        public ValuableImbuingBag(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113769; //Valuable Imbuing Bag
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