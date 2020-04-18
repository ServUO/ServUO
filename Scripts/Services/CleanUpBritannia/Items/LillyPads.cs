namespace Server.Items
{
    public class LillyPads : Item
    {
        [Constructable]
        public LillyPads()
            : base(0xDBE)
        {
            Weight = 1.0;
        }

        public LillyPads(Serial serial)
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