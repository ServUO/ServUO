using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0xE41, 0xE40 )]	// Metal & Gold chest
	public class BankChestComponent : AddonComponent
	{
		public BankChestComponent() : this( Utility.RandomList( 0xE40, 0xE41 ) )
		{
		}

		public BankChestComponent( int itemID ) : base( itemID )
		{
			Hue = 55;
		}

		public override void AddNameProperty(ObjectPropertyList list)
		{
			list.Add( 1062824 );  // Your Bank Box
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.AccessLevel > AccessLevel.Player || from.InRange( this.GetWorldLocation(), 1 ) )
			{
				BankBox box = from.BankBox;
				
				if ( box != null )
				{
					if ( box.MaxItems < 300 )  // my li'l update. ;)
						box.MaxItems = 300;

					box.Open();
				}
				else
				{
					from.SendMessage( "Error! Bank box not found!" );
				}
			}
			else
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			}
		}
		
		public BankChestComponent( Serial serial ) : base( serial )
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

	public class BankChestAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BankChestDeed(); } }
		
		[Constructable]
		public BankChestAddon()
		{
			AddComponent( new BankChestComponent(), 0, 0, 0 );
		}
		
		public BankChestAddon( Serial serial ) : base( serial )
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

	public class BankChestDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new BankChestAddon(); } }
		
		[Constructable]
		public BankChestDeed()
		{
			Name = "a bank chest deed";
		}
		
		public BankChestDeed( Serial serial ) : base( serial )
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