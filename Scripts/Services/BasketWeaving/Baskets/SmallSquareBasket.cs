using Server.Items;

namespace Server.Services.BasketWeaving.Baskets
{
    public class SmallSquareBasket : BaseContainer
    {
        [Constructable]
        public SmallSquareBasket()
            : base(0x24D9)
        {
            Weight = 1.0;
        }

        public SmallSquareBasket(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112296;// small square basket

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
