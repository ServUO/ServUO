namespace Server.Items
{
    public class BracersofAlchemicalDevastation : BoneArms
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1153523; //Bracers of Alchemical Devastation [Replica]

        [Constructable]
        public BracersofAlchemicalDevastation()
        {
            Attributes.RegenMana = 4;
            Attributes.CastRecovery = 3;
            ArmorAttributes.MageArmor = 1;
            WeaponAttributes.HitLightning = 35;
        }

        public BracersofAlchemicalDevastation(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 8;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 8;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override bool CanFortify => false;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
