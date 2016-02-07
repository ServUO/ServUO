using System;
using Server.Items;

namespace Server.Items
{
	public class KnightsPlateHelm : PlateHelm
	{
		public override int LabelNumber{ get{ return 1080158; } } // Thank you Paradyme
		
		public override SetItem SetID{ get{ return SetItem.Knights; } }
		public override int Pieces{ get{ return 6; } }
		
		public override int BasePhysicalResistance{ get{ return 7; } }
		public override int BaseFireResistance{ get{ return 7; } }
		public override int BaseColdResistance{ get{ return 7; } }
		public override int BasePoisonResistance{ get{ return 7; } }
		public override int BaseEnergyResistance{ get{ return 7; } }
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public KnightsPlateHelm() : base()
		{
		
			SetHue = 0x47E;	
            Weight = 5;			
			
			Attributes.BonusHits = 1;
			
			SetAttributes.BonusHits = 6;
			SetAttributes.RegenHits = 2;
			SetAttributes.RegenMana = 2;			
			SetAttributes.AttackChance = 10;
			SetAttributes.DefendChance = 10;
			
			SetPhysicalBonus = 28;
			SetFireBonus = 28;
			SetColdBonus = 28;
			SetPoisonBonus = 28;
			SetEnergyBonus = 28;
		}

		public KnightsPlateHelm( Serial serial ) : base( serial )
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