namespace Server.Items
{
    [Flipable(0x2B74, 0x316B)]
    public class LeafweaveChest : HideChest
    {
        public override bool IsArtifact => true;
        [Constructable]
        public LeafweaveChest()
            : base()
        {
            SetHue = 0x47E;

            Attributes.RegenMana = 1;
            ArmorAttributes.MageArmor = 1;

            SetAttributes.BonusInt = 10;
            SetAttributes.SpellDamage = 15;

            SetSelfRepair = 3;

            SetPhysicalBonus = 4;
            SetFireBonus = 5;
            SetColdBonus = 3;
            SetPoisonBonus = 4;
            SetEnergyBonus = 4;
        }

        public LeafweaveChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074299;// Elven Leafweave
        public override SetItem SetID => SetItem.Mage;
        public override int Pieces => 4;
        public override int BasePhysicalResistance => 4;
        public override int BaseFireResistance => 9;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 8;
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