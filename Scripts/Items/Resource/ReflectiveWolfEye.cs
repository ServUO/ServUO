namespace Server.Items
{
    public class ReflectiveWolfEye : Item, ICommodity
    {
        [Constructable]
        public ReflectiveWolfEye()
            : this(1)
        {
        }

        [Constructable]
        public ReflectiveWolfEye(int amount)
            : base(0x5749)
        {
            Stackable = true;
            Amount = amount;
        }

        public ReflectiveWolfEye(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113362;// reflective wolf eye
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
