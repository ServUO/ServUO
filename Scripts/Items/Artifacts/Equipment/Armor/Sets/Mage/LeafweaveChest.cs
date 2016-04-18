using System;

namespace Server.Items
{
    [FlipableAttribute(0x2B74, 0x316B)]
    public class LeafweaveChest : HideChest
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LeafweaveChest()
            : base()
        {
            this.SetHue = 0x47E;
			
            this.Attributes.RegenMana = 1;
            this.ArmorAttributes.MageArmor = 1;
			
            this.SetAttributes.BonusInt = 10;
            this.SetAttributes.SpellDamage = 15;
			
            this.SetSelfRepair = 3;
			
            this.SetPhysicalBonus = 4;
            this.SetFireBonus = 5;
            this.SetColdBonus = 3;
            this.SetPoisonBonus = 4;
            this.SetEnergyBonus = 4;
        }

        public LeafweaveChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074299;
            }
        }// Elven Leafweave
        public override SetItem SetID
        {
            get
            {
                return SetItem.Mage;
            }
        }
        public override int Pieces
        {
            get
            {
                return 4;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 4;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 9;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 8;
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