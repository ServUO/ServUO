namespace Server.Items
{
    public class InitiationGloves : LeatherGloves
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1116255;  // Armor of Initiation

        public override SetItem SetID => SetItem.Initiation;
        public override int Pieces => 6;

        public override int BasePhysicalResistance => 7;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 4;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 4;
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;

        [Constructable]
        public InitiationGloves() : base()
        {

            Weight = 1;
            Hue = 0x9C4;
            //this.Attributes.Brittle = 1; //If you have imbuing add this part in!!!!
            LootType = LootType.Blessed;

            SetHue = 0x30;
            SetPhysicalBonus = 2;
            SetFireBonus = 5;
            SetColdBonus = 5;
            SetPoisonBonus = 3;
            SetEnergyBonus = 5;
        }

        public InitiationGloves(Serial serial) : base(serial)
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