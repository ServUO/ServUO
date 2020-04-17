namespace Server.Items
{
    public class CrystallineFragments : Item
    {
        [Constructable]
        public CrystallineFragments()
            : base(0x223B)
        {
            LootType = LootType.Blessed;
            Hue = 0x47E;
        }

        public CrystallineFragments(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073160;// Crystalline Fragments
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