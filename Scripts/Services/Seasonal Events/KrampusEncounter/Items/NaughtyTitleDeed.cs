namespace Server.Items
{
    public class NaughtyTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1158797);  // Naughty

        [Constructable]
        public NaughtyTitleDeed()
        {
        }

        public NaughtyTitleDeed(Serial serial)
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
            int v = reader.ReadInt();
        }
    }
}
