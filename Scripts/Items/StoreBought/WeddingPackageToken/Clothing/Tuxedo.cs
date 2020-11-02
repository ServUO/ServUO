namespace Server.Items
{
    [Flipable(0x9EFB, 0x9EFC)]
    public class Tuxedo : BaseShirt
    {
        public override int LabelNumber => 1124723; // tuxedo

        [Constructable]
        public Tuxedo()
            : this(0)
        {
        }

        [Constructable]
        public Tuxedo(int hue)
            : base(0x9EFB, hue)
        {
			LootType = LootType.Blessed;
			Weight = 3.0;
        }

        public Tuxedo(Serial serial)
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
            reader.ReadInt();
        }
    }
}
