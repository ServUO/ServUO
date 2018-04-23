using System;
using Server;
using System.IO;
using System.Collections;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
	public class ArenaExitMoongate : Item
	{
		[Constructable]
		public ArenaExitMoongate() : base( 0xF6C )
		{
			Movable = false;
			Hue = 393;
			Name = "Arena Exit Moongate";
			Light = LightType.Circle300;
		}

		public ArenaExitMoongate( Serial serial ) : base( serial )
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			m.CloseGump( typeof( ArenaEnd ) );
			m.SendGump( new ArenaEnd() );

			return true;
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
