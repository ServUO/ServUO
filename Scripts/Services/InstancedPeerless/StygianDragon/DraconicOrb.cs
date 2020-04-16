namespace Server.Items
{
    public class DraconicOrb : PeerlessKey
    {
        public override int LabelNumber => 1113515; // Draconic Orb (Lesser)

        [Constructable]
        public DraconicOrb()
            : base(0x573E)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = 0x80F;
        }

        public override int Lifespan => 43200;

        public DraconicOrb(Serial serial)
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
            /*int version = */
            reader.ReadInt();
        }
    }
}