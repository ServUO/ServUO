namespace Server.Items
{
    [Flipable(0x4042, 0x4043)]
    public class GargoyleVase : Item
    {
        [Constructable]
        public GargoyleVase()
            : base(0x4042)
        {
            Weight = 10;
        }

        public GargoyleVase(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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
