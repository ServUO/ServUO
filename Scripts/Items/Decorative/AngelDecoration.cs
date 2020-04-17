namespace Server.Items
{
    [Flipable(0x46FA, 0x46FB)]
    public class AngelDecoration : Item
    {
        [Constructable]
        public AngelDecoration()
            : base(0x46FA)
        {
            LootType = LootType.Blessed;
        }

        public AngelDecoration(Serial serial)
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