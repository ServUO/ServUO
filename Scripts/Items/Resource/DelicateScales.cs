namespace Server.Items
{
    public class DelicateScales : Item, ICommodity
    {
        [Constructable]
        public DelicateScales()
            : this(1)
        {
        }

        [Constructable]
        public DelicateScales(int amount)
            : base(0x573A)
        {
            Stackable = true;
            Amount = amount;
        }

        public DelicateScales(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113349;// delicate scales
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
