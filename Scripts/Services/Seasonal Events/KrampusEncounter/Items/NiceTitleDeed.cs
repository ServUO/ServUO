namespace Server.Items
{
    public class NiceTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1158798);  // Nice

        [Constructable]
        public NiceTitleDeed()
        {
        }

        public NiceTitleDeed(Serial serial)
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
