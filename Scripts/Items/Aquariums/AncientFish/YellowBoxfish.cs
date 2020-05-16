namespace Server.Items
{
    public class YellowBoxfish : BaseFish
    {
        [Constructable]
        public YellowBoxfish()
            : base(0xA371)
        {
        }

        public YellowBoxfish(Serial serial)
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
