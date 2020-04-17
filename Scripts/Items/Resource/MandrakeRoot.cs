namespace Server.Items
{
    public class MandrakeRoot : BaseReagent, ICommodity
    {
        [Constructable]
        public MandrakeRoot()
            : this(1)
        {
        }

        [Constructable]
        public MandrakeRoot(int amount)
            : base(0xF86, amount)
        {
        }

        public MandrakeRoot(Serial serial)
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