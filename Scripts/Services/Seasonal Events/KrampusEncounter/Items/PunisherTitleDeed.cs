namespace Server.Items
{
    public class PunisherTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1158795);  // Punisher

        [Constructable]
        public PunisherTitleDeed()
        {
        }

        public PunisherTitleDeed(Serial serial)
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
