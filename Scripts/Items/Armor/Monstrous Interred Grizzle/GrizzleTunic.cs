using System;

namespace Server.Items
{
    public class GrizzleTunic : BoneChest
    {
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

        public override int LabelNumber
        {
            get
            {
                return 1074467;
            }
        }// Tunic of the Grizzle
        public override SetItem SetID
        {
            get
            {
                return SetItem.Grizzle;
            }
        }
        public override int Pieces
        {
            get
            {
                return 5;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 7;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 10;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }
}