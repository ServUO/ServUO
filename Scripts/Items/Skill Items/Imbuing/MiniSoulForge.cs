using System;
using Server;

namespace Server.Items
{
	public class MiniSoulForge : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new MiniSoulForgeDeed(); } }

		[Constructable]
		public MiniSoulForge()
		{
			AddComponent( new AddonComponent( 17607 ), 0, 0, 0 );
		}

		public MiniSoulForge( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
	public class MiniSoulForgeDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new MiniSoulForge(); } }
		
		[Constructable]
		public MiniSoulForgeDeed()
		{
			Name = "SoulForge";
		}

		public MiniSoulForgeDeed( Serial serial ) : base( serial )
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