namespace Server.Items
{
    public class ZealotOfKhalAnkurTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => 1158684;  // Zealot of Khal Ankur

        [Constructable]
        public ZealotOfKhalAnkurTitleDeed()
        {
        }

        public ZealotOfKhalAnkurTitleDeed(Serial serial)
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
