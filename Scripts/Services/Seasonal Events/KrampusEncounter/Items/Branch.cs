namespace Server.Items
{
    public class GoldBranch : Item
    {
        public override int LabelNumber => 1158835;  // branch

        [Constructable]
        public GoldBranch()
            : base(Utility.RandomList(3458, 3473))
        {
            Hue = 2721;
        }
        public GoldBranch(Serial serial)
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

    public class SilverBranch : Item
    {
        public override int LabelNumber => 1158835;  // branch

        [Constructable]
        public SilverBranch()
            : base(Utility.RandomList(3458, 3473))
        {
            Hue = 2500;
        }
        public SilverBranch(Serial serial)
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
