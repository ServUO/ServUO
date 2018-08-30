//Created By Milva

using System;
using Server;

namespace Server.Items
{	


    public class  TownChristmasTree : Item
                               
	             {
		[Constructable]
		public TownChristmasTree () : base( 0x9DBB)
		{                
			
                              Weight = 2;
                             Name = " Town Christmas Tree";
                                
                                                
		}

        public TownChristmasTree(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}