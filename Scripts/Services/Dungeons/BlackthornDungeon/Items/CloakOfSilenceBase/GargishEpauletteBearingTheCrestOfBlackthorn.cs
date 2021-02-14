namespace Server.Items
{
    public class GargishEpauletteBearingTheCrestOfBlackthorn3 : Cloak
    {
        public override bool IsArtifact => true;

        public override int LabelNumber => 1123326;  // Gargish Epaulette

        [Constructable]
        public GargishEpauletteBearingTheCrestOfBlackthorn3()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            ItemID = 0x9986;
            SkillBonuses.SetValues(0, SkillName.Stealth, 10.0);
            Hue = 2130;

            Layer = Layer.OuterTorso;
        }

        public GargishEpauletteBearingTheCrestOfBlackthorn3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
