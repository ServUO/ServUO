using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	[Furniture]
    [FlipableAttribute(0x494E, 0x494D)]
	public class GargoyleStatue  : BaseContainer
	{ 
		[Constructable] 
		public GargoyleStatue() : base( 0x494E ) 
		{ 
			Name = "Gargoyle Statue";
			Weight = 5.0;
		}

        public GargoyleStatue(Serial serial)
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