namespace Server.Items
{
    public class GrizzleTunic : BoneChest
    {
        public override bool IsArtifact => true;
        [Constructable]
        public GrizzleTunic()
            : base()
        {
            this.SetHue = 0x278;

            this.ArmorAttributes.MageArmor = 1;
            this.Attributes.BonusHits = 5;
            this.Attributes.NightSight = 1;

            this.SetAttributes.DefendChance = 10;
            this.SetAttributes.BonusStr = 12;

            this.SetSelfRepair = 3;

            this.SetPhysicalBonus = 3;
            this.SetFireBonus = 5;
            this.SetColdBonus = 3;
            this.SetPoisonBonus = 3;
            this.SetEnergyBonus = 5;
        }

        public GrizzleTunic(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074467;// Tunic of the Grizzle
        public override SetItem SetID => SetItem.Grizzle;
        public override int Pieces => 5;
        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 10;
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