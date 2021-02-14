namespace Server.Items
{
    [Flipable(0xA5F6, 0xA5F7)]
    public class PrideStainedGlassWindow : Item
    {
        public override int LabelNumber => 1126510; // stained glass window

        [Constructable]
        public PrideStainedGlassWindow()
            : base(0xA5F6)
        {
        }

        public PrideStainedGlassWindow(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
	
	[Flipable(0xA5F8, 0xA5F9)]
    public class CovetousStainedGlassWindow : Item
    {
        public override int LabelNumber => 1126510; // stained glass window

        [Constructable]
        public CovetousStainedGlassWindow()
            : base(0xA5F8)
        {
        }

        public CovetousStainedGlassWindow(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
	
	[Flipable(0xA5FA, 0xA5FB)]
    public class JusticeStainedGlassWindow : Item
    {
        public override int LabelNumber => 1126510; // stained glass window

        [Constructable]
        public JusticeStainedGlassWindow()
            : base(0xA5FA)
        {
        }

        public JusticeStainedGlassWindow(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
	
	[Flipable(0xA5FC, 0xA5FD)]
    public class HumilityStainedGlassWindow : Item
    {
        public override int LabelNumber => 1126510; // stained glass window

        [Constructable]
        public HumilityStainedGlassWindow()
            : base(0xA5FC)
        {
        }

        public HumilityStainedGlassWindow(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
