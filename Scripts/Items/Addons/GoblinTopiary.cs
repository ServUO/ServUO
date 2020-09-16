namespace Server.Items
{
    [Flipable(0x4919, 0x491A)]
    public class GoblinTopiary : Item
    {

        public override int LabelNumber => 1070878;  // a decorative topiary

        [Constructable]
        public GoblinTopiary() : base(0x4919)
        {
            Weight = 1.0;
            Name = ("a goblin topiary");
        }

        public GoblinTopiary(Serial serial) : base(serial)
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