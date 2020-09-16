namespace Server.Items
{
    [Flipable(0x1451, 0x1456)]
    public class DaemonHelm : BaseArmor
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DaemonHelm()
            : base(0x1451)
        {
            Hue = 0x648;
            Weight = 3.0;

            ArmorAttributes.SelfRepair = 1;
        }

        public DaemonHelm(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 7;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override int StrReq => 20;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Bone;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;
        public override int LabelNumber => 1041374;// daemon bone helmet
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
