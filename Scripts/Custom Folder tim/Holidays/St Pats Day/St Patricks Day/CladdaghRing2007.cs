using System;

namespace Server.Items
{
	public class CladdaghRing2007 : BaseRing
	{
		[Constructable]
		public CladdaghRing2007() : base( 0x108A )
		{
			Name = "Claddagh Ring 2007";
			Hue = 2244;
			Weight = 0.1;
			Attributes.Luck = 100;
			LootType = LootType.Blessed;			
		}

		public CladdaghRing2007( Serial serial ) : base( serial )
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