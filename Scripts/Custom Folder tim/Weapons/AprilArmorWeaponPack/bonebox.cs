using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Network;
using Server.Mobiles;
using Server.ContextMenus;

namespace Server.Items
{
	public class BoneBox : BaseContainer
	{

		public override int DefaultGumpID{ get{ return 0x9; } }
		public override int DefaultDropSound{ get{ return 0x42; } }


		[Constructable]
		public BoneBox() : base( 0xED2 )
               {
			Weight = 10.0;
               }

		public BoneBox( Serial serial ) : base( serial )
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