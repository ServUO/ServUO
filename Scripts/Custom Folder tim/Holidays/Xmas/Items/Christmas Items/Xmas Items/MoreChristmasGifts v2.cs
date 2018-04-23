using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	
	[FlipableAttribute( 0x9E1B, 0x9E1C )]
	public class StandingCandyCane : Item
	{

		[Constructable]
		public StandingCandyCane() : base( 0x9E1B )
		{
			Weight = 1.0;
			Name = "A Candy Cane";
			LootType = LootType.Blessed;
		}
		

		public StandingCandyCane( Serial serial ) : base( serial )
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
	
	[FlipableAttribute( 0x9DC3, 0x9DC4 )]
	public class GreenStandingCandyCane : Item
	{

		[Constructable]
		public GreenStandingCandyCane() : base( 0x9DC3 )
		{
			Weight = 1.0;
			Name = "A Candy Cane";
			LootType = LootType.Blessed;
		}
		

		public GreenStandingCandyCane( Serial serial ) : base( serial )
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

	
	public class SandTile1 : Item
	{
		[Constructable]
		public SandTile1() : base( 0x9DEC )
		{
			Weight = 1.0;
			Name = "sand";
			LootType = LootType.Blessed;
		}

		public SandTile1( Serial serial ) : base( serial )
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
	
	public class SandTile2 : Item
	{
		[Constructable]
		public SandTile2() : base( 0x9DEB )
		{
			Weight = 1.0;
			Name = "sand";
			LootType = LootType.Blessed;
		}

		public SandTile2( Serial serial ) : base( serial )
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
	
	public class SandTile3 : Item
	{
		[Constructable]
		public SandTile3() : base( 0x9DEA )
		{
			Weight = 1.0;
			Name = "sand";
			LootType = LootType.Blessed;
		}

		public SandTile3( Serial serial ) : base( serial )
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
	
	public class SandTile4 : Item
	{
		[Constructable]
		public SandTile4() : base( 0x9DE9 )
		{
			Weight = 1.0;
			Name = "sand";
			LootType = LootType.Blessed;
		}

		public SandTile4( Serial serial ) : base( serial )
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