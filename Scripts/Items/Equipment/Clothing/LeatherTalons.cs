namespace Server.Items
{
    [Flipable(0x41D8, 0x41D9)]
    public class LeatherTalons : BaseShoes
    {
        [Constructable]
        public LeatherTalons()
            : this(0)
        {
        }

        [Constructable]
        public LeatherTalons(int hue)
            : base(0x41D8, hue)
        {
            Weight = 3.0;
        }

        public LeatherTalons(Serial serial)
            : base(serial)
        {
        }

        public override CraftResource DefaultResource => CraftResource.RegularLeather;
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
