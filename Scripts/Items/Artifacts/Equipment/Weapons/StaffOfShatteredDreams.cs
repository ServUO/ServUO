namespace Server.Items
{
    public class StaffOfShatteredDreams : GlassStaff
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1112771;  // Staff of Shattered Dreams

        [Constructable]
        public StaffOfShatteredDreams()
            : base()
        {
            Hue = 1151;
            WeaponAttributes.HitDispel = 25;
            WeaponAttributes.SplinteringWeapon = 20;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.ResistFireBonus = 15;
            Attributes.CastSpeed = -1;
            Attributes.SpellChanneling = 1;
        }

        public StaffOfShatteredDreams(Serial serial)
            : base(serial)
        {
        }

        public override int ArtifactRarity => 11;
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