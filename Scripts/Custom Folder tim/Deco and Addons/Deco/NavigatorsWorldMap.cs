using System;
namespace Server.Items
{
	[Flipable( 0x14EB, 0x14EC )] // child class doesn't get the flipable attribute of the parent
	public class NavigatorsWorldMap : WorldMap
	{
		public override int LabelNumber{ get{ return 1075500; } } // Navigator's World Map

		[Constructable]
		public NavigatorsWorldMap()
		{
			LootType = LootType.Blessed;
		}

		public NavigatorsWorldMap( Serial serial ) : base( serial )
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
