namespace Server.Items
{
    public class BronzedArmorValkyrie : FemaleLeatherChest
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BronzedArmorValkyrie()
        {
            Attributes.BonusHits = 5;
            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusStam = 5;
            Attributes.RegenStam = 3;
            Attributes.LowerManaCost = 10;
            Hue = 1863; // Hue not exact
        }

        public BronzedArmorValkyrie(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1149957; // Bronzed Armor of the Valkyrie

        public override int BasePhysicalResistance => 11;
        public override int BaseFireResistance => 14;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 11;
        public override int BaseEnergyResistance => 8;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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
