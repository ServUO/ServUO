namespace Server.Items
{
    [Flipable(0x144e, 0x1453)]
    public class DaemonArms : BaseArmor
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DaemonArms()
            : base(0x144E)
        {
            Weight = 2.0;
            Hue = 0x648;

            ArmorAttributes.SelfRepair = 1;
        }

        public DaemonArms(Serial serial)
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
        public override int StrReq => 55;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Bone;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;
        public override int LabelNumber => 1041371;// daemon bone arms
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
