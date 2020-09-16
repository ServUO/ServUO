using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x13db, 0x13e2)]
    public class RangerChest : BaseArmor, IRepairable
    {
        public CraftSystem RepairSystem => DefTailoring.CraftSystem;

        public override bool IsArtifact => true;
        [Constructable]
        public RangerChest()
            : base(0x13DB)
        {
            Weight = 8.0;
            Hue = 0x59C;
        }

        public RangerChest(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 4;
        public override int InitMinHits => 35;
        public override int InitMaxHits => 45;
        public override int StrReq => 35;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Studded;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;
        public override int LabelNumber => 1041497;// studded tunic, ranger armor
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
