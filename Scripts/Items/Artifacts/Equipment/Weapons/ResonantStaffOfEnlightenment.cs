namespace Server.Items
{
    public class ResonantStaffofEnlightenment : QuarterStaff
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113757;  // Resonant Staff of Enlightenment

        [Constructable]
        public ResonantStaffofEnlightenment()
        {
            Hue = 2401;
            WeaponAttributes.HitMagicArrow = 40;
            WeaponAttributes.MageWeapon = 20;
            Attributes.SpellChanneling = 1;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = -40;
            Attributes.LowerManaCost = 5;
            AbsorptionAttributes.ResonanceCold = 20;
            AosElementDamages.Cold = 100;
            Attributes.BonusInt = 5;
        }

        public ResonantStaffofEnlightenment(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}