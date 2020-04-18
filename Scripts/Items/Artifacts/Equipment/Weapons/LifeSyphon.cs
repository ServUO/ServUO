namespace Server.Items
{
    public class LifeSyphon : BloodBlade
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113524;  // Life Syphon

        [Constructable]
        public LifeSyphon()
            : base()
        {
            Hue = 1172;
            WeaponAttributes.BloodDrinker = 1;
            WeaponAttributes.HitHarm = 30;
            WeaponAttributes.HitLeechHits = 100;
            Attributes.BonusHits = 10;
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 50;
        }

        public LifeSyphon(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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