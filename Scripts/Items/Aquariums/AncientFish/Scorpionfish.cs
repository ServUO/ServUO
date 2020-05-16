namespace Server.Items
{
    public class Scorpionfish : BaseFish
    {
        [Constructable]
        public Scorpionfish()
            : base(0xA36B)
        {
        }

        public Scorpionfish(Serial serial)
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
