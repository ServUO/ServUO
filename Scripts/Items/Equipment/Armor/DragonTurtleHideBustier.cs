namespace Server.Items
{
    public class DragonTurtleHideBustier : BaseArmor
    {
        public override int BasePhysicalResistance => 3;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 4;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 2;

        public override int InitMinHits => 35;
        public override int InitMaxHits => 45;

        public override int StrReq => 30;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        // We like to cross dress here!
        public override bool AllowMaleWearer => true;

        public override int LabelNumber => 1109635;  // Dragon Turtle Hide Bustier

        [Constructable]
        public DragonTurtleHideBustier()
            : base(0x782B)
        {
            Weight = 6.0;
        }

        public DragonTurtleHideBustier(Serial serial)
            : base(serial)
        {
        }

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
