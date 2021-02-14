namespace Server.Items
{
    public class MaleKimonoBearingTheCrestOfBlackthorn1 : MaleKimono
    {
        public override bool IsArtifact => true;

        [Constructable]
        public MaleKimonoBearingTheCrestOfBlackthorn1()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusHits = 3;
            Attributes.BonusInt = 5;
            Hue = 2075;
        }

        public MaleKimonoBearingTheCrestOfBlackthorn1(Serial serial)
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
