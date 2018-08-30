using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	[Furniture]
	[FlipableAttribute( 0x4910, 0x4911 )] 
	public class GothicChest : BaseContainer 
	{ 
		[Constructable] 
		public GothicChest() : base( 0x4910 ) 
		{ 
			Name = "Gothic Chest";
			Weight = 5.0;
		} 
		public override int DefaultGumpID{ get{ return 0x49; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public GothicChest( Serial serial ) : base( serial ) 
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