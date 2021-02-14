namespace Server.Items
{
    public class MaleKimonoBearingTheCrestOfBlackthorn5 : MaleKimono
    {
        public override bool IsArtifact => true;

        [Constructable]
        public MaleKimonoBearingTheCrestOfBlackthorn5()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusHits = 3;
            Attributes.RegenHits = 1;
            Hue = 132;
        }

        public MaleKimonoBearingTheCrestOfBlackthorn5(Serial serial)
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
