using Server.Network;

namespace Server.Items
{
    public class BrokenShipwreckRemains : BaseDecayingItem
    {
        public override int LabelNumber => 1154216;  // Broken Remains of a Shipwreck

        [Constructable]
        public BrokenShipwreckRemains() : this(1)
        {
        }

        [Constructable]
        public BrokenShipwreckRemains(int amount) : base(0xC2D)
        {
            Hue = 2969;
            Weight = 25.0;
            Amount = amount;
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 3600;

        public override void OnDoubleClick(Mobile from)
        {
            from.PublicOverheadMessage(MessageType.Regular, 0x559, 1154217); // *You carefully examine the sodden remains of the wooden ship. You discern the ship was recently foundered. You decide to show it to the Salvage Master*
        }

        public BrokenShipwreckRemains(Serial serial) : base(serial)
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