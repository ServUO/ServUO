//Created By Milva

using System;
using Server;

namespace Server.Items
{	


    public class  SantaStatue : Item
                               
	             {
		[Constructable]
		public SantaStatue() : base( 0x4A98)
		{                
			
                              Weight = 3;
                             Name = " Santa Statue";
                             //ItemID = 14943;   
                                                
		}

        public SantaStatue(Serial serial)
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