namespace Server.Items
{
    public class AnimatedLegsoftheInsaneTinker : PlateLegs
    {
        public override bool IsArtifact => true;
        [Constructable]
        public AnimatedLegsoftheInsaneTinker()
            : base()
        {
            Hue = 2310;
            Attributes.BonusDex = 5;
            Attributes.RegenStam = 2;
            Attributes.WeaponDamage = 10;
            Attributes.WeaponSpeed = 10;
            ArmorAttributes.LowerStatReq = 50;
        }

        public AnimatedLegsoftheInsaneTinker(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113760; // Animated Legs of the Insane Tinker

        public override int BasePhysicalResistance => 17;
        public override int BaseFireResistance => 15;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 2;
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
            writer.Write(0); //version
        }
    }
}
