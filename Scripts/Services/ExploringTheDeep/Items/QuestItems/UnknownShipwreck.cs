using Server.Network;

namespace Server.Items
{
    public class UnknownShipwreck : BaseDecayingItem
    {
        public override int LabelNumber => 1154269;  // Map to an Unknown Shipwreck

        [Constructable]
        public UnknownShipwreck() : base(0x14ED)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 3600;

        public UnknownShipwreck(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 2))
            {
                from.PublicOverheadMessage(MessageType.Regular, 0x22, 1154270); // *You unfurl the map and study it carefully. You recognize Gravewater Lake. In the center of the lake is a large X*
            }
            else
                from.SendLocalizedMessage(500446); // That is too far away.

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