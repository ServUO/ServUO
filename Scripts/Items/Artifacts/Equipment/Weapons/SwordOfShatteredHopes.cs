namespace Server.Items
{
    public class SwordOfShatteredHopes : GlassSword
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1112770;  // Sword of Shattered Hopes

        [Constructable]
        public SwordOfShatteredHopes()
            : base()
        {
            Hue = 91;
            WeaponAttributes.HitDispel = 25;
            WeaponAttributes.SplinteringWeapon = 20;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.ResistFireBonus = 15;
        }

        public SwordOfShatteredHopes(Serial serial)
            : base(serial)
        {
        }

        public override int ArtifactRarity => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
                WeaponAttributes.SplinteringWeapon = 20;
        }
    }
}