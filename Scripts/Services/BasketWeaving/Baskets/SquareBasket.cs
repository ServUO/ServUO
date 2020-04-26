using Server.Items;

namespace Server.Services.BasketWeaving.Baskets
{
    public class SquareBasket : BaseContainer
    {
        [Constructable]
        public SquareBasket()
            : base(0x24D5)
        {
            Weight = 1.0;
        }

        public SquareBasket(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112295;// square basket

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
