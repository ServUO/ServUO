namespace Server.Items
{
    public class CuffsOfTheArchmage : BoneArms
    {
        public override int LabelNumber => 1157348;  // cuffs of the archmage
        public override bool IsArtifact => true;

        [Constructable]
        public CuffsOfTheArchmage()
        {
            SkillBonuses.SetValues(0, SkillName.MagicResist, 15.0);
            Attributes.BonusMana = 5;
            Attributes.RegenMana = 4;
            Attributes.SpellDamage = 20;
            ArmorAttributes.MageArmor = 1;
        }

        public CuffsOfTheArchmage(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 15;
        public override int BaseFireResistance => 15;
        public override int BaseColdResistance => 15;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 15;
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

    public class GargishCuffsOfTheArchmage : GargishStoneArms
    {
        public override int LabelNumber => 1157348;  // cuffs of the archmage
        public override bool IsArtifact => true;

        [Constructable]
        public GargishCuffsOfTheArchmage()
        {
            SkillBonuses.SetValues(0, SkillName.MagicResist, 15.0);
            Attributes.BonusMana = 5;
            Attributes.RegenMana = 4;
            Attributes.SpellDamage = 20;
            ArmorAttributes.MageArmor = 1;
        }

        public GargishCuffsOfTheArchmage(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 15;
        public override int BaseFireResistance => 15;
        public override int BaseColdResistance => 15;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 15;
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
