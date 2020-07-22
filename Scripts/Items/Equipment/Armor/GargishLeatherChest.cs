namespace Server.Items
{
    public class GargishLeatherChest : BaseArmor
    {
        [Constructable]
        public GargishLeatherChest()
            : this(0)
        {
        }

        [Constructable]
        public GargishLeatherChest(int hue)
            : base(0x0304)
        {
            Weight = 8.0;
            Hue = hue;
        }

        public GargishLeatherChest(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 6;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 50;

        public override int StrReq => 25;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

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
