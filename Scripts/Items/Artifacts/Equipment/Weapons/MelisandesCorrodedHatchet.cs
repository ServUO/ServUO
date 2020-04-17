namespace Server.Items
{
    public class MelisandesCorrodedHatchet : Hatchet
    {
        public override bool IsArtifact => true;
        [Constructable]
        public MelisandesCorrodedHatchet()
        {
            Hue = 0x494;
            SkillBonuses.SetValues(0, SkillName.Lumberjacking, 5.0);
            Attributes.SpellChanneling = 1;
            Attributes.WeaponSpeed = 15;
            Attributes.WeaponDamage = -50;
            WeaponAttributes.SelfRepair = 4;
        }

        public MelisandesCorrodedHatchet(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072115;// Melisande's Corroded Hatchet
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