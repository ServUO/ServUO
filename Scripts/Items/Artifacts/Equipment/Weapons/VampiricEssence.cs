namespace Server.Items
{
    public class VampiricEssence : Cutlass
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113873;  // Vampiric Essence

        [Constructable]
        public VampiricEssence()
        {
            Hue = 39;
            WeaponAttributes.HitLeechHits = 100;
            WeaponAttributes.HitHarm = 50;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 50;
            AosElementDamages.Cold = 100;
            WeaponAttributes.BloodDrinker = 1;
        }

        public VampiricEssence(Serial serial)
            : base(serial)
        {
        }

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