namespace Server.Items
{
    public class PolymorphScroll : SpellScroll
    {
        [Constructable]
        public PolymorphScroll()
            : this(1)
        {
        }

        [Constructable]
        public PolymorphScroll(int amount)
            : base(55, 0x1F64, amount)
        {
        }

        public PolymorphScroll(Serial serial)
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