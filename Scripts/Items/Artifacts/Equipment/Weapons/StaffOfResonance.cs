namespace Server.Items
{
    public class StaffOfResonance : GlassStaff
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113527;  // Staff of Resonance

        [Constructable]
        public StaffOfResonance()
        {
            switch (Utility.Random(5)) // Random resonance property
            {
                case 0: AbsorptionAttributes.ResonanceFire = 20; break;
                case 1: AbsorptionAttributes.ResonanceCold = 20; break;
                case 2: AbsorptionAttributes.ResonancePoison = 20; break;
                case 3: AbsorptionAttributes.ResonanceEnergy = 20; break;
                case 4: AbsorptionAttributes.ResonanceKinetic = 20; break;
            }
            Attributes.SpellChanneling = 1;
            WeaponAttributes.MageWeapon = 10;
            WeaponAttributes.HitHarm = 50;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = -40;
            Attributes.LowerManaCost = 5;
            AosElementDamages.Poison = 100;
            Hue = 1451; //Hue not exact
        }

        public StaffOfResonance(Serial serial)
            : base(serial)
        {
        }

        public override int ArtifactRarity => 5;

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