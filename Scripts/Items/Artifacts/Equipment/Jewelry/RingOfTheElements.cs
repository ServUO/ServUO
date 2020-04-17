namespace Server.Items
{
    public class RingOfTheElements : GoldRing
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RingOfTheElements()
        {
            Hue = 0x4E9;
            Attributes.Luck = 100;
            Resistances.Fire = 16;
            Resistances.Cold = 16;
            Resistances.Poison = 16;
            Resistances.Energy = 16;
        }

        public RingOfTheElements(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061104;// Ring of the Elements
        public override int ArtifactRarity => 11;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}