namespace Server.Items
{
    public class DaemonClaw : Item, ICommodity
    {
        [Constructable]
        public DaemonClaw()
            : this(1)
        {
        }

        [Constructable]
        public DaemonClaw(int amount)
            : base(0x5721)
        {
            Stackable = true;
            Amount = amount;
        }

        public DaemonClaw(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113330;// daemon claw
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
