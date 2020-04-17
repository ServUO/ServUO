namespace Server.Items
{
    public class DefenderOfTheMagus : MetalShield
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DefenderOfTheMagus()
        {
            Hue = 590;
            Attributes.SpellChanneling = 1;
            Attributes.DefendChance = 10;
            Attributes.CastRecovery = 1;
            //Random Resonance:
            switch (Utility.Random(5))
            {
                case 0:
                    AbsorptionAttributes.ResonanceCold = 10;
                    break;
                case 1:
                    AbsorptionAttributes.ResonanceFire = 10;
                    break;
                case 2:
                    AbsorptionAttributes.ResonanceKinetic = 10;
                    break;
                case 3:
                    AbsorptionAttributes.ResonancePoison = 10;
                    break;
                case 4:
                    AbsorptionAttributes.ResonanceEnergy = 10;
                    break;
            }
            //Random Resist:
            switch (Utility.Random(5))
            {
                case 0:
                    ColdBonus = 10;
                    break;
                case 1:
                    FireBonus = 10;
                    break;
                case 2:
                    PhysicalBonus = 10;
                    break;
                case 3:
                    PoisonBonus = 10;
                    break;
                case 4:
                    EnergyBonus = 10;
                    break;
            }

        }

        public DefenderOfTheMagus(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113851; // Defender of the Magus

        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 1;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 0;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);//version
        }
    }
}
