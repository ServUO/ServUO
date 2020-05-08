namespace Server.Items
{
    public class LeggingsOfEmbers : PlateLegs
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1062911;// Royal Leggings of Embers
        public override int BasePhysicalResistance => 15;
        public override int BaseFireResistance => 25;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 15;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public LeggingsOfEmbers()
        {
            Hue = 0x2C;
            LootType = LootType.Blessed;
            ArmorAttributes.SelfRepair = 10;
            ArmorAttributes.MageArmor = 1;
            ArmorAttributes.LowerStatReq = 100;
        }

        public LeggingsOfEmbers(Serial serial)
            : base(serial)
        {
        }

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
