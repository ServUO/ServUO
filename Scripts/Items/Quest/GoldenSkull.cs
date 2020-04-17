namespace Server.Engines.Quests.Doom
{
    public class GoldenSkull : Item
    {
        public override int LabelNumber => 1061619; // a golden skull

        [Constructable]
        public GoldenSkull()
            : base(Utility.Random(0x1AE2, 3))
        {
            Weight = 1.0;
            Hue = 0x8A5;
            LootType = LootType.Blessed;
        }

        public GoldenSkull(Serial serial)
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