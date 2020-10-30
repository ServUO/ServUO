namespace Server.Items
{
    [Flipable(0xA300, 0xA301)]
    public class DecorativeWishingWell : Item
    {
        public override int LabelNumber => 1125752; // well

        [Constructable]
        public DecorativeWishingWell()
            : base(0xA300)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeWishingWell(Serial serial)
            : base(serial)
        {
        }

        public void CheckWaterSource(Mobile from, BaseBeverage beverage)
        {
            if (IsLockedDown)
            {
                beverage.Content = BeverageType.Water;
                beverage.Poison = null;
                beverage.Poisoner = null;

                beverage.Quantity = beverage.MaxQuantity;

                from.SendLocalizedMessage(1010089); // You fill the container with water.
            }
            else
            {
                from.SendLocalizedMessage(1114298); // This must be locked down in order to use it.
            }
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
