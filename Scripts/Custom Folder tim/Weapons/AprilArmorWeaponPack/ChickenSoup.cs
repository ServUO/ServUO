using System;
using Server;

namespace Server.Items
{
	public class ChickenSoup : Item
	{

		public override int LabelNumber{ get{ return 1062926; } }

		[Constructable]
		public ChickenSoup() : this( 1 )
		{
		}

		[Constructable]
		public ChickenSoup( int amount ) : base( 0x1606 )
		{
                        
			Name = "Chicken Soup";
			Stackable = true;
			Amount = amount;
			Movable = true;
			Weight = 3.0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042038 );
			}
			else if ( from.GetStatMod( "ChickenSoup" ) != null )
			{
				from.SendLocalizedMessage( 1062927 ); // This gives the player the message "You have eaten one of these recently and eating another would provide no benefit."
			}
			else
			{
				from.PlaySound( 0x1EE );
				from.AddStatMod( new StatMod( StatType.Str, "ChickenSoup", 5, TimeSpan.FromMinutes( 5.0 ) ) ); // this makes DantesInks give you 5 INT for 15 mins when eaten

				Consume();
			}
		}

		public ChickenSoup( Serial serial ) : base( serial )
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