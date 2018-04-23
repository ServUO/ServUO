using System;

namespace Server.Items
{
	public class AncientStonePlanter : Item
	{
		[Constructable]
		public AncientStonePlanter() : base( 0x44EF )
		{
			Name = "Ancient Stone Planter";
			Weight = 1.0;
		}

		public AncientStonePlanter( Serial serial ) : base( serial )
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

	public class GrecianPlanter : Item
	{
		[Constructable]
		public GrecianPlanter() : base( 0x44F0 )
		{
			Name = "Grecian Planter";
			Weight = 1.0;
		}

		public GrecianPlanter( Serial serial ) : base( serial )
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
	
	public class ModernPlanter : Item
	{
		[Constructable]
		public ModernPlanter() : base( 0x44F1 )
		{
			Name = "Modern Planter";
			Weight = 1.0;
		}

		public ModernPlanter( Serial serial ) : base( serial )
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
	
	public class OakBarrel : Item
	{
		[Constructable]
		public OakBarrel() : base( 0x44F2 )
		{
			Name = "Oak Barrel";
			Weight = 1.0;
		}

		public OakBarrel( Serial serial ) : base( serial )
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