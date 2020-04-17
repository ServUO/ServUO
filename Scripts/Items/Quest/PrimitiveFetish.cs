namespace Server.Items
{
    public class PrimitiveFetish : Item
    {
        [Constructable]
        public PrimitiveFetish()
            : base(0x23F)
        {
            LootType = LootType.Blessed;
            Hue = 0x244;
        }

        public PrimitiveFetish(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074675;// Primitive Fetish
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