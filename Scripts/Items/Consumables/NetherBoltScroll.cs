namespace Server.Items
{
    public class NetherBoltScroll : SpellScroll
    {
        [Constructable]
        public NetherBoltScroll()
            : this(1)
        {
        }

        [Constructable]
        public NetherBoltScroll(int amount)
            : base(677, 0x2D9E, amount)
        {
        }

        public NetherBoltScroll(Serial serial)
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