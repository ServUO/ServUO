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
                    DropItem(new UndyingFlesh());
                    break;
                case 1:
                    DropItem(new DaemonClaw());
                    break;
                    /*  case 2:
                          DropItem(new SpiderCarapace());
                          break;
                      case 3:
                          DropItem(new VialOfVitriol());
                          break;
                      case 4:
                          DropItem(new LavaSerpenCrust());
                          break;
                      case 5:
                          DropItem(new GoblinBlood());
                          break;*/
            }
        }

        public ValuableImbuingBag(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113769; //Valuable Imbuing Bag
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}