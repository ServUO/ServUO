/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class MLTree19Addon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new MLTree19AddonDeed();
			}
		}

		[ Constructable ]
		public MLTree19Addon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 15038 );
			AddComponent( ac, 3, -1, 0 );
			ac = new AddonComponent( 15037 );
			AddComponent( ac, 3, 0, 0 );
			ac = new AddonComponent( 15036 );
			AddComponent( ac, 3, 1, 0 );
			ac = new AddonComponent( 15035 );
			AddComponent( ac, -2, 2, 0 );
			ac = new AddonComponent( 15034 );
			AddComponent( ac, -1, 2, 0 );
			ac = new AddonComponent( 15033 );
			AddComponent( ac, 0, 2, 0 );
			ac = new AddonComponent( 15032 );
			AddComponent( ac, 1, 2, 0 );
			ac = new AddonComponent( 15031 );
			AddComponent( ac, 2, 2, 0 );
			ac = new AddonComponent( 15030 );
			AddComponent( ac, 3, 2, 0 );

		}

		public MLTree19Addon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class MLTree19AddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new MLTree19Addon();
			}
		}

		[Constructable]
		public MLTree19AddonDeed()
		{
			Name = "MLTree19";
		}

		public MLTree19AddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}