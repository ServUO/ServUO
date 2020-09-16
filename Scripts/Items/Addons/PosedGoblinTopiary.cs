namespace Server.Items
{
    [Flipable(0x4960, 0x4966)]
    public class PosedGoblinTopiary : Item
    {
        public override int LabelNumber => 1070878;  // a decorative topiary

        [Constructable]
        public PosedGoblinTopiary() : base(0x4960)
        {
            Weight = 1.0;
            Name = ("a posed goblin topiary");
        }

        public PosedGoblinTopiary(Serial serial) : base(serial)
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