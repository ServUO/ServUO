namespace Server.Items
{
    public class BombardScroll : SpellScroll
    {
        [Constructable]
        public BombardScroll()
            : this(1)
        {
        }

        [Constructable]
        public BombardScroll(int amount)
            : base(688, 0x2DA9, amount)
        {
        }

        public BombardScroll(Serial serial)
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