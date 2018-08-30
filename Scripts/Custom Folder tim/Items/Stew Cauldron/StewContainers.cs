// based off training dummy script (cauldron is [flipable)
using System;
using Server;

namespace Server.Items
{
	[Flipable( 0x974, 0x975 )]
	public class StewCauldron : AddonComponent
	{
		public StewCauldron() : this( 0x974 )
		{
		}

		public StewCauldron( int itemID ) : base( itemID )
		{
		}

		public StewCauldron( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class StewCauldronEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new StewCauldronEastDeed(); } }

		[Constructable]
		public StewCauldronEastAddon()
		{
			AddComponent( new StewCauldron( 0x975 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x970 ), 0, 0, 8 );
		}

		public StewCauldronEastAddon( Serial serial ) : base( serial )
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

	public class StewCauldronEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new StewCauldronEastAddon(); } }
		public override int LabelNumber{ get{ return 1022420; } } // cauldron

		[Constructable]
		public StewCauldronEastDeed()
		{
			Name = "Stew Cauldron East Deed";
		}

		public StewCauldronEastDeed( Serial serial ) : base( serial )
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

	public class StewCauldronSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new StewCauldronSouthDeed(); } }

		[Constructable]
		public StewCauldronSouthAddon()
		{
			AddComponent( new StewCauldron( 0x974 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x970 ), 0, 0, 8 );
		}

		public StewCauldronSouthAddon( Serial serial ) : base( serial )
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

	public class StewCauldronSouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new StewCauldronSouthAddon(); } }
		public override int LabelNumber{ get{ return 1022420; } } // cauldron

		[Constructable]
		public StewCauldronSouthDeed()
		{
			Name = "Stew Cauldron South Deed";
		}

		public StewCauldronSouthDeed( Serial serial ) : base( serial )
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

	public class StewKettleAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new StewKettleDeed(); } }

		[Constructable]
		public StewKettleAddon()
		{
			AddComponent( new AddonComponent( 0x9ED ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x970 ), 0, 0, 6 );
		}

		public StewKettleAddon( Serial serial ) : base( serial )
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

	public class StewKettleDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new StewKettleAddon(); } }
		public override int LabelNumber{ get{ return 1022541; } } // kettle

		[Constructable]
		public StewKettleDeed()
		{
			Name = "Stew Kettle Deed";
		}

		public StewKettleDeed( Serial serial ) : base( serial )
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