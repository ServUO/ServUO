namespace Server.Items
{
    public class KrampusMinionBoots : BaseShoes
    {
        public override int LabelNumber => 1125637;  // krampus minion boots

        [Constructable]
        public KrampusMinionBoots()
            : this(0)
        {
            Weight = 2.0;
        }

        [Constructable]
        public KrampusMinionBoots(int hue)
            : base(0xA28D, hue)
        {
        }

        public KrampusMinionBoots(Serial serial)
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
