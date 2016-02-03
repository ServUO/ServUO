using System;

namespace Server.Items
{
    public class AloronsTigerPeltLeggings : BaseArmor
    {
        [Constructable]
        public AloronsTigerPeltLeggings()
            : base(0x7824)
        {
            Weight = 3.0;
            AbsorptionAttributes.EaterCold = 2;
            Attributes.BonusDex = 4;
            Attributes.BonusStam = 4;
            Attributes.RegenStam = 3;

            SetHue = 0x09C4;
            //Set Slayer Dinosaur
            SetPhysicalBonus = 8;
            SetFireBonus = 8;
            SetColdBonus = 9;
            SetPoisonBonus = 8;
            SetEnergyBonus = 8;
            SetSelfRepair = 3;
            SetAttributes.BonusMana = 15;
            SetAttributes.LowerManaCost = 20;
        }

        public override int LabelNumber { get { return 1156243; } }// Aloron's Armor
        public override SetItem SetID { get { return SetItem.Alorons; } }
        public override int Pieces { get { return 4; } }
        public override int BasePhysicalResistance { get { return 7; } }
        public override int BaseFireResistance { get { return 7; } }
        public override int BaseColdResistance { get { return 6; } }
        public override int BasePoisonResistance { get { return 7; } }
        public override int BaseEnergyResistance { get { return 7; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
        public override int AosStrReq { get { return 25; } }

        public AloronsTigerPeltLeggings(Serial serial)
            : base(serial)
        {
        }
        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Bone;
            }
        }
        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}