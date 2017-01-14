using System;
using Server.Items;

namespace Server.Items
{
	public class InitiationCap : LeatherCap
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber{ get{ return 1116255; } } // Armor of Initiation
		
		public override SetItem SetID{ get{ return SetItem.Initiation; } }
		public override int Pieces{ get{ return 6; } }
	
		public override int BasePhysicalResistance{ get{ return 7; } }
		public override int BaseFireResistance{ get{ return 4; } }
		public override int BaseColdResistance{ get{ return 4; } }
		public override int BasePoisonResistance{ get{ return 6; } }
		public override int BaseEnergyResistance{ get{ return 4; } }
		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		[Constructable]
		public InitiationCap() : base()
		{

			Weight = 1;	
			this.Hue = 0x9C4;
			//this.Attributes.Brittle = 1; //If you have imbuing add this part in!!!!
			this.LootType = LootType.Blessed;
			
			SetHue = 0x30;	
			SetPhysicalBonus = 2;
			SetFireBonus = 5;
			SetColdBonus = 5;
			SetPoisonBonus = 3;
			SetEnergyBonus = 5;
		}

		public InitiationCap( Serial serial ) : base( serial )
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