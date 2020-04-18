namespace Server.Items
{
    public class DecorativeKitchenSet : Backpack
    {
        public override int LabelNumber => 1158970;  // Decorative Kitchen Set

        [Constructable]
        public DecorativeKitchenSet()
        {
            DropItem(new PieSafe());
            DropItem(new ChinaCabinet());
            DropItem(new ButcherBlock());
            DropItem(new WashBasinDeed());
            DropItem(new WoodStoveDeed());

            Bag bag = new Bag();
            bag.DropItem(new Countertop());
            bag.DropItem(new Countertop());
            bag.DropItem(new Countertop());
            bag.DropItem(new Countertop());

            DropItem(bag);
        }

        public DecorativeKitchenSet(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
