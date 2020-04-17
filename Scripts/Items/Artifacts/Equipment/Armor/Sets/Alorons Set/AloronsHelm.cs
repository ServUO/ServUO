namespace Server.Items
{
    public class AloronsHelm : TigerPeltHelm
    {
        public override int LabelNumber => 1156243;  // Aloron's Armor

        public override SetItem SetID => SetItem.Aloron;
        public override int Pieces => 4;

        public override int BasePhysicalResistance => 7;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 7;

        [Constructable]
        public AloronsHelm()
        {
            AbsorptionAttributes.EaterCold = 2;
            Attributes.BonusDex = 4;
            Attributes.BonusStam = 4;
            Attributes.RegenStam = 3;

            SetAttributes.BonusMana = 15;
            SetAttributes.LowerManaCost = 20;
            SetSelfRepair = 3;

            SetPhysicalBonus = 8;
            SetFireBonus = 8;
            SetColdBonus = 9;
            SetPoisonBonus = 8;
            SetEnergyBonus = 8;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1156345); // Dinosaur Slayer
        }

        public AloronsHelm(Serial serial)
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