using System;
using Server;

namespace Server.Items
{
	public class XDGuardPostAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new XDGuardPostDeed(); } }

		[Constructable]
		public XDGuardPostAddon()
		{
			AddComponent( new AddonComponent( 0x001D ), 0, 0, 0 );

			for ( int y = 0; y < 14; ++y )
				AddComponent( new AddonComponent( 0x001B ), 0, 1 + y, 0 );

			for ( int x = 0; x < 14; ++x )
				AddComponent( new AddonComponent( 0x001C ), 1 + x, 0, 0 );

			for ( int y = 0; y < 14; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 1, 1 + y, 0 );

			for ( int x = 0; x < 13; ++x )
				AddComponent( new AddonComponent( 0x001C ), 1 + x, 14, 0 );

			for ( int y = 0; y < 6; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 2, 1 + y, 0 );

			for ( int x = 0; x < 13; ++x )
				AddComponent( new AddonComponent( Utility.Random( 0x0521, 4 ) ), 2 + x, 7, 0 );

			for ( int y = 0; y < 7; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 2, 8 + y, 0 );

			for ( int x = 0; x < 12; ++x )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 3 + x, 1, 0 );

			for ( int y = 0; y < 5; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0521, 4 ) ), 3, 2 + y, 0 );

			for ( int y = 0; y < 6; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0521, 4 ) ), 3, 8 + y, 0 );

			for ( int x = 0; x < 12; ++x )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 3 + x, 14, 0 );

			for ( int x = 0; x < 2; ++x )
				for ( int y = 0; y < 5; ++y )
					AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 4 + x, 2 + y, 0 );

			for ( int x = 0; x < 2; ++x )
				for ( int y = 0; y < 6; ++y )
					AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 4 + x, 8 + y, 0 );

			for ( int x = 0; x < 9; ++x )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 6 + x, 2, 0 );

			for ( int y = 0; y < 4; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0521, 4 ) ), 6, 3 + y, 0 );

			for ( int y = 0; y < 4; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0521, 4 ) ), 6, 8 + y, 0 );

			for ( int y = 0; y < 2; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 6, 12 + y, 0 );

			for ( int x = 0; x < 2; ++x )
				for ( int y = 0; y < 4; ++y )
					AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 7 + x, 3 + y, 0 );

			for ( int x = 0; x < 2; ++x )
				for ( int y = 0; y < 6; ++y )
					AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 7 + x, 8 + y, 0 );

			for ( int x = 0; x < 6; ++x )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 9 + x, 3, 0 );

			for ( int y = 0; y < 3; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0521, 4 ) ), 9, 4 + y, 0 );

			for ( int y = 0; y < 3; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0521, 4 ) ), 9, 8 + y, 0 );

			for ( int y = 0; y < 3; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 9, 11 + y, 0 );

			for ( int x = 0; x < 2; ++x )
				for ( int y = 0; y < 3; ++y )
					AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 10 + x, 4 + y, 0 );

			for ( int x = 0; x < 2; ++x )
				for ( int y = 0; y < 6; ++y )
					AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 10 + x, 8 + y, 0 );

			for ( int x = 0; x < 2; ++x )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 12 + x, 4, 0 );

			for ( int y = 0; y < 2; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0521, 4 ) ), 12, 5 + y, 0 );

			for ( int y = 0; y < 2; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0521, 4 ) ), 12, 8 + y, 0 );

			for ( int y = 0; y < 4; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 12, 10 + y, 0 );

			AddComponent( new AddonComponent( 0x001B ), 13, 4, 0 );

			for ( int y = 0; y < 2; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 13, 5 + y, 0 );

			AddComponent( new AddonComponent( 0x002B ), 13, 5, 0 );

			for ( int y = 0; y < 6; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 13, 8 + y, 0 );

			AddComponent( new AddonComponent( 0x002A ), 13, 10, 0 );
			AddComponent( new AddonComponent( 0x001B ), 13, 11, 0 );

			for ( int y = 0; y < 2; ++y )
				AddComponent( new AddonComponent( 0x001B ), 14, 1 + y, 0 );

			AddComponent( new AddonComponent( 0x001A ), 14, 3, 0 );

			for ( int y = 0; y < 3; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0521, 4 ) ), 14, 4 + y, 0 );

			for ( int y = 0; y < 4; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0521, 4 ) ), 14, 8 + y, 0 );

			AddComponent( new AddonComponent( 0x001C ), 14, 11, 0 );

			for ( int y = 0; y < 2; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x051D, 4 ) ), 14, 12 + y, 0 );

			for ( int y = 0; y < 2; ++y )
				AddComponent( new AddonComponent( 0x001B ), 14, 12 + y, 0 );

			AddComponent( new AddonComponent( 0x001A ), 14, 14, 0 );
			AddComponent( new AddonComponent( 0x0030 ), 0, 0, 20 );
			AddComponent( new AddonComponent( 0x0030 ), 0, 0, 23 );

			for ( int y = 0; y < 14; ++y )
				AddComponent( new AddonComponent( 0x002E ), 0, 1 + y, 20 );

			AddComponent( new AddonComponent( 0x0030 ), 0, 14, 23 );

			for ( int x = 0; x < 14; ++x )
				AddComponent( new AddonComponent( 0x002F ), 1 + x, 0, 20 );

			for ( int x = 0; x < 13; ++x )
				for ( int y = 0; y < 14; ++y )
					AddComponent( new AddonComponent( Utility.Random( 0x0519, 4 ) ), 1 + x, 1 + y, 20 );

			for ( int x = 0; x < 13; ++x )
				AddComponent( new AddonComponent( 0x002F ), 1 + x, 14, 20 );

			AddComponent( new AddonComponent( 0x0E74 ), 4, 4, 20 );
			AddComponent( new AddonComponent( 0x0E74 ), 4, 11, 20 );
			AddComponent( new AddonComponent( 0x0E73 ), 4, 13, 20 );
			AddComponent( new AddonComponent( 0x0E8D ), 6, 3, 20 );
			AddComponent( new AddonComponent( 0x0E8C ), 6, 4, 20 );
			AddComponent( new AddonComponent( 0x0E8B ), 6, 5, 20 );
			AddComponent( new AddonComponent( 0x0E6E ), 6, 11, 20 );
			AddComponent( new AddonComponent( 0x0E92 ), 6, 12, 20 );
			AddComponent( new AddonComponent( 0x0E6D ), 6, 13, 20 );
			AddComponent( new AddonComponent( 0x0E73 ), 7, 2, 20 );
			AddComponent( new AddonComponent( 0x0E74 ), 7, 4, 20 );
			AddComponent( new AddonComponent( 0x0E74 ), 7, 11, 20 );
			AddComponent( new AddonComponent( 0x0E73 ), 8, 5, 20 );
			AddComponent( new AddonComponent( 0x0030 ), 13, 3, 20 );

			for ( int x = 0; x < 2; ++x )
				AddComponent( new AddonComponent( 0x0030 ), 13 + x, 3, 23 );

			for ( int y = 0; y < 8; ++y )
				AddComponent( new AddonComponent( 0x002E ), 13, 4 + y, 20 );

			for ( int y = 0; y < 6; ++y )
				AddComponent( new AddonComponent( 0x002E ), 13, 5 + y, 23 );

			for ( int y = 0; y < 4; ++y )
				AddComponent( new AddonComponent( 0x002E ), 13, 6 + y, 26 );

			for ( int y = 0; y < 2; ++y )
				AddComponent( new AddonComponent( 0x002E ), 13, 7 + y, 29 );

			AddComponent( new AddonComponent( 0x0030 ), 13, 7, 32 );

			for ( int x = 0; x < 2; ++x )
				AddComponent( new AddonComponent( 0x0030 ), 13 + x, 11, 23 );

			AddComponent( new AddonComponent( 0x0030 ), 14, 0, 23 );

			for ( int y = 0; y < 3; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0519, 4 ) ), 14, 1 + y, 20 );

			for ( int y = 0; y < 2; ++y )
				AddComponent( new AddonComponent( 0x002E ), 14, 1 + y, 20 );

			AddComponent( new AddonComponent( 0x002D ), 14, 3, 20 );
			AddComponent( new AddonComponent( 0x002F ), 14, 11, 20 );

			for ( int y = 0; y < 3; ++y )
				AddComponent( new AddonComponent( Utility.Random( 0x0519, 4 ) ), 14, 12 + y, 20 );

			for ( int y = 0; y < 2; ++y )
				AddComponent( new AddonComponent( 0x002E ), 14, 12 + y, 20 );

			AddComponent( new AddonComponent( 0x002D ), 14, 14, 20 );
			AddComponent( new AddonComponent( 0x0030 ), 14, 14, 23 );
		}

		public XDGuardPostAddon( Serial serial ) : base( serial )
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

	public class XDGuardPostDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new XDGuardPostAddon(); } }

		[Constructable]
		public XDGuardPostDeed()
		{
			Name = "XD_Guard_Post";
		}

		public XDGuardPostDeed( Serial serial ) : base( serial )
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