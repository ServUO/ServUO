namespace Server.Items
{
    public class TalonBite : OrnateAxe
    {
        public override bool IsArtifact => true;
        [Constructable]
        public TalonBite()
        {
            Hue = 0x47E;
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
            Attributes.BonusDex = 8;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 35;
            WeaponAttributes.HitHarm = 33;
            WeaponAttributes.UseBestSkill = 1;
        }

        public TalonBite(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075029;// Talon Bite
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