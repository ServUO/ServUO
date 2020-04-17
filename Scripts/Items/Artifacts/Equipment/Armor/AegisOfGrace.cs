namespace Server.Items
{
    public class AegisOfGrace : DragonHelm
    {
        public override int LabelNumber => 1075047;  // Aegis of Grace
        public override bool IsArtifact => true;

        [Constructable]
        public AegisOfGrace()
        {
            SkillBonuses.SetValues(0, SkillName.MagicResist, 10.0);
            Attributes.DefendChance = 20;
            ArmorAttributes.SelfRepair = 2;

            if (Utility.RandomBool())
            {
                ItemID = 0x2B6E;
                Weight = 2.0;
                StrRequirement = 10;

                MeditationAllowance = ArmorMeditationAllowance.All;
            }
        }

        public AegisOfGrace(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 9;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 15;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Dragon;
        public override CraftResource DefaultResource => CraftResource.Iron;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            if (version == 0 && ItemID == 0x2B6E)
            {
                MeditationAllowance = ArmorMeditationAllowance.All;
            }
        }
    }
}
