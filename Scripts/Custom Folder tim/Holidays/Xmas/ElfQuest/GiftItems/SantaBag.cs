//Created By Milva

using System;
using Server;

namespace Server.Items
{	


    public class  SantaBag : Item
                               
	             {
		[Constructable]
		public SantaBag() : base( 0x9DB5)
		{                
			
                              Weight = 2;
                             Name = " Santa Bag";
                             //ItemID = 14943;   
                                                
		}

        public SantaBag(Serial serial)
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