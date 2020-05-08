namespace Server.Items
{
    public class ProtectoroftheBattleMage : LeatherChest
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113761;  // Protector of the Battle Mage
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 16;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 8;

        [Constructable]
        public ProtectoroftheBattleMage()
            : base()
        {
            Hue = 1159;
            Attributes.LowerManaCost = 8;
            Attributes.RegenMana = 2;
            Attributes.LowerRegCost = 10;
            Attributes.SpellDamage = 5;
            AbsorptionAttributes.CastingFocus = 3;
        }

        public ProtectoroftheBattleMage(Serial serial)
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
