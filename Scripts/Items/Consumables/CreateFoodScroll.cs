namespace Server.Items
{
    public class CreateFoodScroll : SpellScroll
    {
        [Constructable]
        public CreateFoodScroll()
            : this(1)
        {
        }

        [Constructable]
        public CreateFoodScroll(int amount)
            : base(1, 0x1F2F, amount)
        {
        }

        public CreateFoodScroll(Serial serial)
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