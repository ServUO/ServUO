// 0x1E06 => South
// 0x1E03 => West
// 0x1E04 => North
// 0x1E05 => East

namespace Server.Mobiles
{
    public class BloodyFootPrints : Item
    {
        [Constructable]
        public BloodyFootPrints(int itemID)
            : base(itemID)
        {
            Movable = false;
            Hue = 1975;
        }

        public BloodyFootPrints(Serial serial)
            : base(serial)
        {
        }

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
