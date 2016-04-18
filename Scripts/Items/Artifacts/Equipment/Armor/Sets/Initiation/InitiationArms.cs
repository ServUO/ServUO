using System;
using Server.Items;

namespace Server.Items
{
	public class InitiationArms : LeatherArms
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
		public InitiationArms() : base()
		{

			this.Weight = 2;	
			this.Hue = 0x9C4;
			this.LootType = LootType.Blessed;
			//this.Attributes.Brittle = 1; //If you have imbuing add this part in!!!!
				
			SetHue = 0x30;	
			SetPhysicalBonus = 2;
			SetFireBonus = 5;
			SetColdBonus = 5;
			SetPoisonBonus = 3;
			SetEnergyBonus = 5;
		}

		public InitiationArms( Serial serial ) : base( serial )
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