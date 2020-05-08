namespace Server.Items
{
    public class WallOfHungryMouths : HeaterShield
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113722;// Wall of Hungry Mouths
        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 1;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 0;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public WallOfHungryMouths()
        {
            Hue = 1034;
            AbsorptionAttributes.EaterEnergy = 20;
            AbsorptionAttributes.EaterPoison = 20;
            AbsorptionAttributes.EaterCold = 20;
            AbsorptionAttributes.EaterFire = 20;
        }

        public WallOfHungryMouths(Serial serial)
            : base(serial)
        {
        }

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
