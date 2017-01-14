using System;
using Server.Items;

namespace Server.Items
{
	public class SorcererFemaleChest : FemaleLeatherChest
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber{ get{ return 1080469; } } // Sorcerer's Female Armor

        public override SetItem SetID{ get{ return SetItem.Sorcerer; } }
		public override int Pieces{ get{ return 6; } }
	
		public override int BasePhysicalResistance{ get{ return 7; } }
		public override int BaseFireResistance{ get{ return 7; } }
		public override int BaseColdResistance{ get{ return 7; } }
		public override int BasePoisonResistance{ get{ return 7; } }
		public override int BaseEnergyResistance{ get{ return 7; } }
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public SorcererFemaleChest() : base()
		{
            this.Hue = 1165;            
			this.Weight = 1;

            this.Attributes.BonusInt = 1;
            this.Attributes.LowerRegCost = 10;

            this.SetAttributes.BonusInt = 6;
            this.SetAttributes.RegenMana = 2;
            this.SetAttributes.DefendChance = 10;
            this.SetAttributes.LowerManaCost = 5;
            this.SetAttributes.LowerRegCost = 40;
            
            this.SetHue = 1165;

            this.SetPhysicalBonus = 28;
            this.SetFireBonus = 28;
            this.SetColdBonus = 28;
            this.SetPoisonBonus = 28;
            this.SetEnergyBonus = 28;
		}

		public SorcererFemaleChest( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
}