using System;

namespace Server.Items
{
    public class DardensDragonTurtleHideHelm : BaseArmor
    {
        [Constructable]
        public DardensDragonTurtleHideHelm()
            : base(0x782D)
        {
            Weight = 4.0;
            AbsorptionAttributes.EaterKinetic = 2;
            Attributes.BonusHits = 4;
            Attributes.LowerRegCost = 15;
            Attributes.BonusStr = 4;

            SetPhysicalBonus = 9;
            SetFireBonus = 8;
            SetColdBonus = 8;
            SetPoisonBonus = 8;
            SetEnergyBonus = 8;
            SetSelfRepair = 3;
            SetAttributes.BonusMana = 15;
            SetAttributes.LowerManaCost = 20;
            // Set myrmidex slayer
        }

        public DardensDragonTurtleHideHelm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber { get { return 1156242; } }// Darden's Armor
        public override SetItem SetID { get { return SetItem.Dardens; } }
        public override int Pieces { get { return 4; } }
        public override int BasePhysicalResistance { get { return 6; } }
        public override int BaseFireResistance { get { return 7; } }
        public override int BaseColdResistance { get { return 7; } }
        public override int BasePoisonResistance { get { return 7; } }
        public override int BaseEnergyResistance { get { return 7; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
        public override int AosStrReq { get { return 30; } }
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            if (this.Weight == 1.0)
                this.Weight = 3.0;
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}