namespace Server.Items
{
    [Flipable(0x2B08, 0x2B09)]
    public class JusticeBreastplate : BaseArmor
    {
        public override bool IsArtifact => true;
        [Constructable]
        public JusticeBreastplate()
            : base(0x2B08)
        {
            LootType = LootType.Blessed;
            Weight = 7.0;
            SetHue = 0;
            Hue = 0x226;

            SetSelfRepair = 5;

            SetPhysicalBonus = 5;
            SetFireBonus = 5;
            SetColdBonus = 5;
            SetPoisonBonus = 5;
            SetEnergyBonus = 5;
        }

        public JusticeBreastplate(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075190;// Breastplate of Justice (Virtue Armor Set)
        public override SetItem SetID => SetItem.Virtue;
        public override int Pieces => 8;
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 8;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override int StrReq => 65;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Plate;
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