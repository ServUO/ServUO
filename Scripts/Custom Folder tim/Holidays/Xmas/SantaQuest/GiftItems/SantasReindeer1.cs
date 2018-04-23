//Created By Milva

using System;
using Server;

namespace Server.Items
{	


    public class  SantasReindeer1A : Item
                               
	             {
		[Constructable]
		public SantasReindeer1A () : base( 0x3A67)
		{                
			
                              Weight = 3;
                             Name = "Santa's Reindeer";
                             ItemID = 14951;   
                                                
		}

        public SantasReindeer1A(Serial serial)
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