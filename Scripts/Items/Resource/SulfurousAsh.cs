namespace Server.Items
{
    public class SulfurousAsh : BaseReagent, ICommodity
    {
        [Constructable]
        public SulfurousAsh()
            : this(1)
        {
        }

        [Constructable]
        public SulfurousAsh(int amount)
            : base(0xF8C, amount)
        {
        }

        public SulfurousAsh(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;
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