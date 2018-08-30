// BankChest.cs by Alari (alarihyena@gmail.com)
// "by" isn't really applicable here, I saw
// someone's bankstone script and figured
// I'd make it a chest or something. Then
// someone else posted THEIR bankchest script
// I wasn't even going to post this at all, I
// figured it was too simplistic.

using System;
using Server.Items;
using System.Collections;
using Server.Network;
using Server.Targeting;
using Server.Prompts;

namespace Server.Items
{
	[FlipableAttribute( 0xE41, 0xE40 )]	// metal & gold
	public class BankChest : Item
	{
		// this does not work like expected. =(
		/*public new bool Movable 
		{
			get { return false; }
			set {}
		}*/

		[Constructable]
		public BankChest() : this( 0xE40 )
		{
		}

		[Constructable]
		public BankChest( int itemID ) : base( itemID )
		{
			Movable = false;
			Weight = 10000; // overkill
			Name = "a bank chest (opens your bank box)";

			// These hues are all very light-colored hues,
			// one step from gray on the brighest setting.
			// Makes them stand out just a bit.
			Hue = Utility.RandomList( 806, 811, 816, 821, 826, 831, 836, 841, 846, 851, 856, 861, 866, 871, 876, 881, 886, 891, 896, 901 );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 4 ) )
			{
				BankBox box = from.BankBox;
				if ( from.Criminal )
				{
					from.SendMessage( "Thou canst not deposit or withdraw from thy bank box if thou art a criminal!" );
				}
				else if ( box != null )
				{
					if ( box.MaxItems < 300 )  // my li'l update. ;)
						box.MaxItems = 300;

					box.Open();
				}
				else
				{
					from.SendMessage( "Error! Thy bank box was not found!" );
				}
			}
			else
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			}
		}

		public BankChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}