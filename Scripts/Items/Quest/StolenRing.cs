namespace Server.Items
{
    public class StolenRing : SilverRing
    {

        [Constructable]
        public StolenRing()
            : base()
        {
            LootType = LootType.Blessed;
            Weight = 1;
        }

        public override int LabelNumber => 1073124;// A ring engraved: "Beloved Ciala"

        public StolenRing(Serial serial)
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