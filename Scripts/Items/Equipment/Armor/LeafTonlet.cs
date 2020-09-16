using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTailoring), typeof(GargishLeatherLegs))]
    [Flipable(0x2FCA, 0x3180)]
    public class LeafTonlet : BaseArmor
    {
        [Constructable]
        public LeafTonlet()
            : base(0x2FCA)
        {
            Weight = 2.0;
        }

        public LeafTonlet(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 4;
        public override int BaseEnergyResistance => 4;
        public override int InitMinHits => 30;
        public override int InitMaxHits => 40;
        public override int StrReq => 10;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
