namespace Server.Items
{
    public class DardensLegs : DragonTurtleHideLegs
    {
        public override int LabelNumber => 1156242;  // Darden's Armor

        public override SetItem SetID => SetItem.Darden;
        public override int Pieces => 4;

        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 7;

        [Constructable]
        public DardensLegs()
        {
            AbsorptionAttributes.EaterKinetic = 2;
            Attributes.BonusStr = 4;
            Attributes.BonusHits = 4;
            Attributes.LowerRegCost = 15;

            SetAttributes.BonusMana = 15;
            SetAttributes.LowerManaCost = 20;
            SetSelfRepair = 3;

            SetPhysicalBonus = 9;
            SetFireBonus = 8;
            SetColdBonus = 8;
            SetPoisonBonus = 8;
            SetEnergyBonus = 8;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1156346); // Myrmidex Slayer
        }

        public DardensLegs(Serial serial)
            : base(serial)
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