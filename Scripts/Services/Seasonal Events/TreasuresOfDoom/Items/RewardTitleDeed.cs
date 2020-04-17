namespace Server.Items
{
    public class TreasuresOfDoomRewardDeed : BaseRewardTitleDeed
    {
        private TextDefinition _Title;

        public override TextDefinition Title => _Title;

        public TreasuresOfDoomRewardDeed(int localization)
        {
            _Title = localization;
        }

        public TreasuresOfDoomRewardDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            TextDefinition.Serialize(writer, _Title);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Title = TextDefinition.Deserialize(reader);
        }
    }
}