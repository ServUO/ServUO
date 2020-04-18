namespace Server.Items
{
    public class NexusCore : Item
    {
        [Constructable]
        public NexusCore() : base(0x4B82)
        {
            Stackable = false;
        }

        public NexusCore(Serial serial) : base(serial)
        {
        }

        public override int LabelNumber => 1153501;  // nexus core 

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                Stackable = false;
        }
    }
}