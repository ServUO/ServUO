namespace Server.Items
{
    public class ScatteredCrystals : PeerlessKey
    {
        [Constructable]
        public ScatteredCrystals()
            : base(0x2248)
        {
            LootType = LootType.Blessed;
            Weight = 1;
            Hue = 0x47E;
        }

        public ScatteredCrystals(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074264;// scattered crystals
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