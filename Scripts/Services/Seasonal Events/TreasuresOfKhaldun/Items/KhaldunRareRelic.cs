namespace Server.Items
{
    public class RelicOfHydros : Item
    {
        public override int LabelNumber => 1125458;  // relic of hydros

        [Constructable]
        public RelicOfHydros()
            : base(0xA1DA)
        {
            Weight = 10.0;
        }

        public RelicOfHydros(Serial serial)
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
            var version = reader.ReadInt();
        }
    }

    public class RelicOfLithos : Item
    {
        public override int LabelNumber => 1125461;  // relic of lithos

        [Constructable]
        public RelicOfLithos()
            : base(0xA1DD)
        {
            Weight = 10.0;
        }

        public RelicOfLithos(Serial serial)
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
            var version = reader.ReadInt();
        }
    }

    public class RelicOfPyros : Item
    {
        public override int LabelNumber => 1125459;  // relic of pyros

        [Constructable]
        public RelicOfPyros()
            : base(0xA1DB)
        {
            Weight = 10.0;
        }

        public RelicOfPyros(Serial serial)
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
            var version = reader.ReadInt();
        }
    }

    public class RelicOfStratos : Item
    {
        public override int LabelNumber => 1125460;  // relic of stratos

        [Constructable]
        public RelicOfStratos()
            : base(0xA1DC)
        {
            Weight = 10.0;
        }

        public RelicOfStratos(Serial serial)
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
            var version = reader.ReadInt();
        }
    }
}
