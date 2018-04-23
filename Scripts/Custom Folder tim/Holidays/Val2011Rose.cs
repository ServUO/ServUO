using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x18D9, 0x18DA )]
	public class Val2011Rose : Item /* TODO: when dye tub changes are implemented, furny dyable this */
	{
		//public override int LabelNumber { get { return 1023760; } } // A Rose in a Vase	1023760

		[Constructable]
		public Val2011Rose( ) : base( 0x18D9 )
		{
			Name = "A Freshly Picked Rose";
			Hue = Utility.RandomList( 32, 1150, 2349 );
			LootType = LootType.Blessed;
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060662, "Valentine's Day\t2011" );
		}

		public Val2011Rose( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}