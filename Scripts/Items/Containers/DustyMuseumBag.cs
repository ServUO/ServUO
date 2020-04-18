namespace Server.Items
{
    public class DustyMuseumBag : BaseRewardBag
    {
        [Constructable]
        public DustyMuseumBag()
        {
            DropItem(new Gold(6000));
            DropItem(new TerMurQuestRewardBook());

            for (int i = 0; i < Utility.RandomMinMax(7, 9); i++)
            {
                DropItemStacked(Loot.RandomGem());
            }

            for (int i = 0; i < Utility.RandomMinMax(3, 5); i++)
            {
                DropItemStacked(Loot.RandomMLResource());
            }
        }

        public DustyMuseumBag(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112994;
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
