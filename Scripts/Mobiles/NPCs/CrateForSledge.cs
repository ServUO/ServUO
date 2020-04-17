namespace Server.Items
{
    public class CrateForSledge : Item
    {
        [Constructable]
        public CrateForSledge()
            : base(0x1FFF)
        {
            Weight = 5.0;
            LootType = LootType.Blessed;
        }

        public CrateForSledge(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074520;// Crate for Sledge
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