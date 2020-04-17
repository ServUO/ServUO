namespace Server.Items
{
    public class VoidCore : Item, ICommodity
    {
        public override int LabelNumber => 1113334;  // void core
        public override double DefaultWeight => 0.1;

        [Constructable]
        public VoidCore()
            : this(1)
        {
        }

        [Constructable]
        public VoidCore(int amount)
            : base(0x5728)
        {
            Stackable = true;
            Amount = amount;
        }

        public VoidCore(Serial serial)
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
