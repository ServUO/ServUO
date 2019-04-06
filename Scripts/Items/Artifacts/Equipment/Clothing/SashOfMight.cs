using System;
using Server.Misc;

namespace Server.Items
{
	[FlipableAttribute( 0x1541, 0x1542 )] 
	public class SashOfMight : BodySash 
	{

		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1075412; } } // Sash of Might
     		
		[Constructable] 
		public SashOfMight() : base( 0x1541 ) 
		{ 
			Hue = 0x481;
		} 

        public SashOfMight(Serial serial): base(serial) 
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
