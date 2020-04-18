namespace Server.Items
{
    public class EmptyOilFlask : Item
    {
        public override int LabelNumber => 1150866;  // empty oil flask

        [Constructable]
        public EmptyOilFlask()
            : this(1)
        {
        }

        [Constructable]
        public EmptyOilFlask(int amount)
            : base(0x1C18)
        {
            Stackable = true;
            Amount = amount;
        }

        public EmptyOilFlask(Serial serial)
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

    [TypeAlias("Server.Items.FlaskOfOil ")]
    public class OilFlask : Item
    {
        [Constructable]
        public OilFlask()
            : this(1)
        {
        }

        [Constructable]
        public OilFlask(int amount)
            : base(0x1C18)
        {
            Stackable = true;
            Amount = amount;
        }

        public OilFlask(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1027199; // Oil Flask
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
