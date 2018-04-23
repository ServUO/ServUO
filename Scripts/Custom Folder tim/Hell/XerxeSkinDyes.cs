using System;
using Server;

namespace Server.Items
{
	public class XerxeSkinDyes : Item
	{

	
		[Constructable]
		public XerxeSkinDyes() : base( 0x185d )
		{
			Weight = 1.0;
            Name = "Xerxe's Skin Dyes";
            Hue = 1070;
		}

        public XerxeSkinDyes(Serial serial)
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

