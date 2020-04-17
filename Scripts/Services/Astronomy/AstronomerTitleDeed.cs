namespace Server.Items
{
    public class AstronomerTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1158523;  // Astronomer

        public AstronomerTitleDeed()
        {
        }

        public AstronomerTitleDeed(Serial serial)
            : base(serial)
        {
        }

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