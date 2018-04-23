using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;

namespace Server.Items
{
	public class MaginciaContractOfEmployment : Item
	{
		public override int LabelNumber{ get{ return 1150636; } } // Commodity Broker

		[Constructable]
		public MaginciaContractOfEmployment() : base( 0x14F0 )
		{
			Weight = 1.0;
			//LootType = LootType.Blessed;
		}

		public MaginciaContractOfEmployment( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if (  from.Region.Name == "Magincia"  )
			{
				//from.SendLocalizedMessage( 503248 ); // Your godly powers allow you to place this vendor whereever you wish.

				Mobile v = new PlayerVendor( from, BaseHouse.FindHouseAt( from ) );

				v.Direction = from.Direction & Direction.Mask;
				v.MoveToWorld( from.Location, from.Map );

				v.SayTo( from, 503246 ); // Ah! it feels good to be working again.

				this.Delete();
			}
			else
			{
			
				from.SendLocalizedMessage( 1062677 ); // You cannot place a vendor or barkeep at this location.
	
			}
		}
	}
}