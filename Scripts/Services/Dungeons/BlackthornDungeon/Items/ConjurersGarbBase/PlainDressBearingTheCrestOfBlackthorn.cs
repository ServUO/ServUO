namespace Server.Items
{
    public class PlainDressBearingTheCrestOfBlackthorn7 : PlainDress
    {
        public override bool IsArtifact => true;

        [Constructable]
        public PlainDressBearingTheCrestOfBlackthorn7()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.RegenMana = 2;
            Attributes.DefendChance = 5;
            Attributes.Luck = 140;
            Hue = 1194;
        }

        public PlainDressBearingTheCrestOfBlackthorn7(Serial serial)
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
