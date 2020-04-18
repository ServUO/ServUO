namespace Server.Items
{
    public class WizardsCurio : BaseTalisman
    {
        public override bool IsArtifact => true;

        [Constructable]
        public WizardsCurio()
            : base(0x2F58)
        {
            Hue = 1912;
            SkillBonuses.SetValues(0, SkillName.EvalInt, 10.0);
            Attributes.RegenMana = 1;
            Attributes.LowerRegCost = 10;
            Attributes.SpellDamage = 5;
        }

        public WizardsCurio(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1154729;// Wizard's Curio
        public override bool ForceShowName => true;
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