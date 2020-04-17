namespace Server.Items
{
    public class SeekerOfTheFallenStarTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1158682;  // Seeker of the Fallen Star

        [Constructable]
        public SeekerOfTheFallenStarTitleDeed()
        {
        }

        public SeekerOfTheFallenStarTitleDeed(Serial serial)
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
