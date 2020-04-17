using Server.Network;

namespace Server.Items
{
    public class OrcishSchematics : BaseDecayingItem
    {
        public override int LabelNumber => 1154232;  // Schematic for an Orcish Drilling Machine

        [Constructable]
        public OrcishSchematics() : base(0x2258)
        {
            Hue = 1945;
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154233); // *It appears to be the crude schematic to a drilling machine of Orcish origin. It is poorly devised and looks as if one were to build it the machine would explode*
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 3600;
        public override bool UseSeconds => false;

        public OrcishSchematics(Serial serial) : base(serial)
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
