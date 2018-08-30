//Created by Milva
using System;
using Server;
using Server.Items;
namespace Server.Items
{
        
	public class RedPaintBarrel :  Item
	{

        [Constructable]
		public RedPaintBarrel ()
		{
			Weight = 1.0; 
            Name = "Barrel Of Red Paint"; 
            ItemID = 4014;  
            Hue = 33;                  
        }

        public RedPaintBarrel(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}