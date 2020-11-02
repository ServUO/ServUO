namespace Server.Items
{
    [Flipable(0x9EF7, 0x9EF8)]
    public class TuxedoPants : BasePants
    {
        public override int LabelNumber => 1124719; // tuxedo pants

        [Constructable]
        public TuxedoPants()
            : this(0)
        {
        }

        [Constructable]
        public TuxedoPants(int hue)
            : base(0x9EF7, hue)
        {
			LootType = LootType.Blessed;
			Weight = 3.0;
        }

        public TuxedoPants(Serial serial)
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
