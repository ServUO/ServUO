namespace Server.Items
{
    public class CrushedGlass : Item, ICommodity
    {
        [Constructable]
        public CrushedGlass()
            : this(1)
        {
        }

        [Constructable]
        public CrushedGlass(int amount)
            : base(0x573B)
        {
            Stackable = true;
            Amount = amount;
        }

        public CrushedGlass(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113351;// crushed glass
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
