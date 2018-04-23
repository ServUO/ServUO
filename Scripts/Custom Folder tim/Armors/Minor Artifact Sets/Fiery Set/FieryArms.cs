//This Suit is Based of the Blaze of Death 
//To prevent conflicts with Blaze Ore and Death set already in UO 
//I named it Fiery.

using System;
using Server;

namespace Server.Items
{
	public class FieryArms : StuddedArms
	{
		public override int LabelNumber{ get{ return 1063486; } }
		
		public override int BasePhysicalResistance{ get{ return 15; } }
		//I set this so high because I removed the BoD's 50% Fireball and FireArea
		public override int BaseFireResistance{ get{ return 30; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public FieryArms()
		{
			Name = "Fiery Arms";
		
			Hue = Utility.RandomBool() ? 1256 : 1260;
			
			Attributes.WeaponSpeed = 5;
			Attributes.WeaponDamage = 10;
			ArmorAttributes.LowerStatReq = 100;
		}

		public FieryArms( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}