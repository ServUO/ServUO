using System;
namespace Server.Items
{
	public class ThrowPillow : Item
	{
		[Constructable]
		public ThrowPillow() : base( 0x1944 )
		{
			LootType = LootType.Blessed;
		}

		public ThrowPillow( Serial serial ) : base( serial )
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
