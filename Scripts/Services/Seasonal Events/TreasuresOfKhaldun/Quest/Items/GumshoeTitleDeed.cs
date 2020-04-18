namespace Server.Items
{
    public class GumshoeTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1158638;  // Gumpshoe

        [Constructable]
        public GumshoeTitleDeed()
        {
        }

        public GumshoeTitleDeed(Serial serial)
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
