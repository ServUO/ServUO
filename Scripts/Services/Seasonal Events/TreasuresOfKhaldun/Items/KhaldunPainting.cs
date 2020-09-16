namespace Server.Items
{
    [Flipable(0xA1DE, 0xA1DF)]
    public class ZombiePainting : Item
    {
        public override int LabelNumber => 1023744;  // painting

        [Constructable]
        public ZombiePainting()
            : base(0x4C62)
        {
            Weight = 10.0;
        }

        public ZombiePainting(Serial serial)
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

    [Flipable(0xA1E0, 0xA1E1)]
    public class SkeletonPortrait : Item
    {
        public override int LabelNumber => 1023744;  // painting

        [Constructable]
        public SkeletonPortrait()
            : base(0xA1E0)
        {
            Weight = 10.0;
        }

        public SkeletonPortrait(Serial serial)
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

    [Flipable(0xA1E2, 0xA1E3)]
    public class LichPainting : Item
    {
        public override int LabelNumber => 1023744;  // painting

        [Constructable]
        public LichPainting()
            : base(0xA1E2)
        {
            Weight = 10.0;
        }

        public LichPainting(Serial serial)
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
}
