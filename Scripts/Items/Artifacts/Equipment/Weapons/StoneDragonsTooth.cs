namespace Server.Items
{
    public class StoneDragonsTooth : GargishDagger
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113523;  // Stone Dragon's Tooth

        [Constructable]
        public StoneDragonsTooth()
            : base()
        {
            Hue = 2407;
            Attributes.WeaponSpeed = 10;
            Attributes.WeaponDamage = 50;
            Attributes.RegenHits = 3;
            WeaponAttributes.HitMagicArrow = 40;
            WeaponAttributes.HitLowerDefend = 30;
            WeaponAttributes.ResistFireBonus = 10;
            AbsorptionAttributes.EaterPoison = 10;
            AosElementDamages.Poison = 100;
        }

        public StoneDragonsTooth(Serial serial)
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