using System;
using Server;

namespace Server.Items
{
 
	public class MonksRobe : BaseOuterTorso
	{
		public override int LabelNumber{ get{ return 1076584; } } // A Monk's Robe
            
            [Constructable]
		public MonksRobe() : this( 0x21E )
		{
		}

		[Constructable]
		public MonksRobe( int hue ) : base( 0x2683, hue )
		{
			
			
			Weight = 1.0;
		}

		public override bool Dye( Mobile from, DyeTub sender )
		{
			from.SendLocalizedMessage( sender.FailMessage );
			return false;
		}

		public MonksRobe( Serial serial ) : base( serial )
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
