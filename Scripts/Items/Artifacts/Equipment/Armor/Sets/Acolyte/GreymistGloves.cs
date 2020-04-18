namespace Server.Items
{
    public class GreymistGloves : LeatherGloves
    {
        public override bool IsArtifact => true;
        [Constructable]
        public GreymistGloves()
            : base()
        {
            SetHue = 0xCB;

            Attributes.BonusMana = 2;
            Attributes.SpellDamage = 2;

            SetAttributes.Luck = 100;
            SetAttributes.NightSight = 1;

            SetSelfRepair = 3;

            SetPhysicalBonus = 3;
            SetFireBonus = 3;
            SetColdBonus = 3;
            SetPoisonBonus = 3;
            SetEnergyBonus = 3;
        }

        public GreymistGloves(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074307;// Greymist Armor
        public override SetItem SetID => SetItem.Acolyte;
        public override int Pieces => 4;
        public override int BasePhysicalResistance => 7;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 4;
        public override int BaseEnergyResistance => 4;
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