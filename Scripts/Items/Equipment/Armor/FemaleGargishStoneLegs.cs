namespace Server.Items
{
    public class FemaleGargishStoneLegs : BaseArmor
    {
        [Constructable]
        public FemaleGargishStoneLegs()
            : this(0)
        {
        }

        [Constructable]
        public FemaleGargishStoneLegs(int hue)
            : base(0x289)
        {
            Weight = 15.0;
            Hue = hue;
        }

        public FemaleGargishStoneLegs(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 4;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 6;

        public override int InitMinHits => 40;
        public override int InitMaxHits => 50;

        public override int StrReq => 40;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Stone;

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
