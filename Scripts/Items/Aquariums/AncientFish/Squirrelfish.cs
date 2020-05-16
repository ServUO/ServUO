namespace Server.Items
{
    public class Squirrelfish : BaseFish
    {
        [Constructable]
        public Squirrelfish()
            : base(0xA365)
        {
        }

        public Squirrelfish(Serial serial)
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
