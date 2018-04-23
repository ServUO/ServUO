using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	[Furniture]
    [FlipableAttribute(0x493C, 0x493B)]
	public class GargoyleStatue2  : BaseContainer
	{ 
		[Constructable] 
		public GargoyleStatue2() : base( 0x493C ) 
		{ 
			Name = "Gargoyle Statue";
			Weight = 5.0;
		}

        public GargoyleStatue2(Serial serial)
            : base(serial) 
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