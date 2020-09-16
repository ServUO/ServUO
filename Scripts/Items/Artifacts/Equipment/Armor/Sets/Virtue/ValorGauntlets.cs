namespace Server.Items
{
    [Flipable(0x2B0C, 0x2B0D)]
    public class ValorGauntlets : BaseArmor
    {
        public override bool IsArtifact => true;
        [Constructable]
        public ValorGauntlets()
            : base(0x2B0C)
        {
            LootType = LootType.Blessed;
            Weight = 4.0;
            SetHue = 0;
            Hue = 0x226;

            SetSelfRepair = 5;

            SetPhysicalBonus = 5;
            SetFireBonus = 5;
            SetColdBonus = 5;
            SetPoisonBonus = 5;
            SetEnergyBonus = 5;
        }

        public ValorGauntlets(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075238;// Gauntlets of Valor (Virtue Armor Set)
        public override SetItem SetID => SetItem.Virtue;
        public override int Pieces => 8;
        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 9;
        public override int BaseEnergyResistance => 6;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override int StrReq => 50;
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