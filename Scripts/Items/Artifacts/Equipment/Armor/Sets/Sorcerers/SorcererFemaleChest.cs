using System;
using Server.Items;

namespace Server.Items
{
	public class SorcererFemaleChest : FemaleLeatherChest
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber{ get{ return 1080469; } } // Thank You Paradyme
		
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
			SetHue = 0x1F4;	
			Weight = 1;	
			
			Attributes.BonusInt = 1;
			Attributes.LowerRegCost = 10;
			
			SetAttributes.BonusInt = 6;
			SetAttributes.RegenMana = 2;
			SetAttributes.DefendChance = 10;
			SetAttributes.LowerManaCost = 5;
			SetAttributes.LowerRegCost = 40;			
			SetPhysicalBonus = 28;
			SetFireBonus = 28;
			SetColdBonus = 28;
			SetPoisonBonus = 28;
			SetEnergyBonus = 28;
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