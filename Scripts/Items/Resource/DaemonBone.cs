namespace Server.Items
{
    // TODO: Commodity?
    public class DaemonBone : BaseReagent
    {
        [Constructable]
        public DaemonBone()
            : this(1)
        {
        }

        [Constructable]
        public DaemonBone(int amount)
            : base(0xF80, amount)
        {
        }

        public DaemonBone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}