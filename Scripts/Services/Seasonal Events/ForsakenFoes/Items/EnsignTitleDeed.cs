namespace Server.Items
{
    public class EnsignTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1159214);  // Ensign

        [Constructable]
        public EnsignTitleDeed()
        {
        }

        public EnsignTitleDeed(Serial serial)
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
