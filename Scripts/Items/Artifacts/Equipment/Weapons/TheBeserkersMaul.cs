namespace Server.Items
{
    public class TheBeserkersMaul : Maul
    {
        public override bool IsArtifact => true;
        [Constructable]
        public TheBeserkersMaul()
        {
            Hue = 0x21;
            Attributes.WeaponSpeed = 75;
            Attributes.WeaponDamage = 50;
        }

        public TheBeserkersMaul(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061108;// The Berserker's Maul
        public override int ArtifactRarity => 11;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}