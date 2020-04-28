using Server.Items;

namespace Server.Services.BasketWeaving.Baskets
{
    public class SmallRoundBasket : BaseContainer
    {
        [Constructable]
        public SmallRoundBasket()
            : base(0x24DD)
        {
            Weight = 1.0;
        }

        public SmallRoundBasket(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112298;// small round basket

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
