namespace Server.Items
{
    public class BulgingMuseumBag : BaseRewardBag
    {
        [Constructable]
        public BulgingMuseumBag()
        {
            DropItem(new Gold(10000));
            DropItem(new TerMurQuestRewardBook());

            for (int i = 0; i < Utility.RandomMinMax(10, 15); i++)
            {
                DropItemStacked(Loot.RandomGem());
            }

            for (int i = 0; i < Utility.RandomMinMax(5, 7); i++)
            {
                DropItemStacked(Loot.RandomMLResource());
            }
        }

        public BulgingMuseumBag(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112995;
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
