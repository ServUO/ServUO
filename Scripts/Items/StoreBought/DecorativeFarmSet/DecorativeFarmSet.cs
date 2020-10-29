namespace Server.Items
{
    public class DecorativeFarmSet : LargeCrate
    {
        public override int LabelNumber => 1159046; // Decorative Farm Set

        [Constructable]
        public DecorativeFarmSet()
        {
            DropItem(new DecorativeButterChurn());
            DropItem(new DecorativeFeedingTrough());
            DropItem(new DecorativePlough());
            DropItem(new DecorativeWishingWell());
            DropItem(new HangingFarmToolDeed());
            DropItem(new MagicMilkPail());
        }

        public DecorativeFarmSet(Serial serial)
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
            reader.ReadInt();
        }
    }
}
