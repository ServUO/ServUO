namespace Server.Items
{
    public class ProphetTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1158683;  // Prophet

        [Constructable]
        public ProphetTitleDeed()
        {
        }

        public ProphetTitleDeed(Serial serial)
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
