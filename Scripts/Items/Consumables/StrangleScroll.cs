namespace Server.Items
{
    public class StrangleScroll : SpellScroll
    {
        [Constructable]
        public StrangleScroll()
            : this(1)
        {
        }

        [Constructable]
        public StrangleScroll(int amount)
            : base(110, 0x226A, amount)
        {
        }

        public StrangleScroll(Serial serial)
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