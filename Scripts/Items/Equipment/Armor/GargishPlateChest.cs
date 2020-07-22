namespace Server.Items
{
    public class GargishPlateChest : BaseArmor
    {
        [Constructable]
        public GargishPlateChest()
            : this(0)
        {
        }

        [Constructable]
        public GargishPlateChest(int hue)
            : base(0x30A)
        {
            Weight = 10.0;
            Hue = hue;
        }

        public GargishPlateChest(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 8;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 50;
        public override int InitMaxHits => 65;

        public override int StrReq => 95;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Plate;

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
