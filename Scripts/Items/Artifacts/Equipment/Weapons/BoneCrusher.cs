namespace Server.Items
{
    public class BoneCrusher : WarMace
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BoneCrusher()
        {
            Hue = 0x60C;
            WeaponAttributes.HitLowerDefend = 50;
            Attributes.BonusStr = 10;
            Attributes.WeaponDamage = 75;
        }

        public BoneCrusher(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061596;// Bone Crusher
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