namespace Server.Items
{
    public class CommanderTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1159215);  // Commander

        [Constructable]
        public CommanderTitleDeed()
        {
        }

        public CommanderTitleDeed(Serial serial)
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
