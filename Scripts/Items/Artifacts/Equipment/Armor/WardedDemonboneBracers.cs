namespace Server.Items
{
    public class WardedDemonboneBracers : BoneArms
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1115775;  // Warded Demonbone Bracers

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override int PhysicalResistance => 6;
        public override int FireResistance => 8;
        public override int ColdResistance => 4;
        public override int PoisonResistance => 5;
        public override int EnergyResistance => 5;

        [Constructable]
        public WardedDemonboneBracers()
        {
            Hue = 0x2E2;
            AbsorptionAttributes.CastingFocus = 2;
            Attributes.RegenMana = 1;
            Attributes.LowerManaCost = 6;
            Attributes.LowerRegCost = 12;
            ArmorAttributes.MageArmor = 1;
        }

        public WardedDemonboneBracers(Serial serial)
            : base(serial)
        {
        }

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