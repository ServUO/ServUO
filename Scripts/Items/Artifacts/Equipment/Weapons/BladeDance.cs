namespace Server.Items
{
    public class BladeDance : RuneBlade
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BladeDance()
        {
            Hue = 0x66C;
            Attributes.BonusMana = 8;
            Attributes.SpellChanneling = 1;
            Attributes.WeaponDamage = 30;
            WeaponAttributes.HitLeechMana = 20;
            WeaponAttributes.UseBestSkill = 1;
        }

        public BladeDance(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075033;// Blade Dance
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