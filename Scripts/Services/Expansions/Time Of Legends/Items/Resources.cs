namespace Server.Items
{
    [Flipable(0x9BCC, 0x9BCD)]
    public class TigerPelt : Item, ICommodity
    {
        public override int LabelNumber => 1123908;

        [Constructable]
        public TigerPelt()
            : this(1)
        {
        }

        [Constructable]
        public TigerPelt(int amount) : base(0x9BCC)
        {
            Weight = 1.0;
            Stackable = true;
            Amount = amount;

            Hue = 243;
        }

        public TigerPelt(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WhiteTigerPelt : Item, ICommodity
    {
        public override int LabelNumber => 1156201;

        [Constructable]
        public WhiteTigerPelt()
            : this(1)
        {
        }

        [Constructable]
        public WhiteTigerPelt(int amount)
            : base(0x9BCC)
        {
            Weight = 1.0;
            Stackable = true;
            Amount = amount;
            Hue = 0;
        }

        public WhiteTigerPelt(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BlackTigerPelt : Item, ICommodity
    {
        public override int LabelNumber => 1156200;

        [Constructable]
        public BlackTigerPelt()
            : this(1)
        {
        }

        [Constructable]
        public BlackTigerPelt(int amount)
            : base(0x9BCC)
        {
            Weight = 1.0;
            Stackable = true;
            Amount = amount;
            Hue = 1175;
        }

        public BlackTigerPelt(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DragonTurtleScute : Item, ICommodity
    {
        public override int LabelNumber => 1123910;

        [Constructable]
        public DragonTurtleScute()
            : this(1)
        {
        }

        [Constructable]
        public DragonTurtleScute(int amount)
            : base(0x9BCE)
        {
            Weight = 1.0;
            Stackable = true;
            Amount = amount;
        }

        public DragonTurtleScute(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
