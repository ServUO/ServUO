using System;
using Server.Network;

namespace Server.Items
{

	[FlipableAttribute( 0x49A0, 0x49B4 )] 
	public class SacrificeTapestry : Item
	{
		[Constructable]
		public SacrificeTapestry() : base( 0x49A0 )
		{
			Weight = 10;
		}
	

		public SacrificeTapestry( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x49A1, 0x49B9 )] 
	public class SpiritualityTapestry : Item
	{
		[Constructable]
		public SpiritualityTapestry() : base( 0x49A1 )
		{
			Weight = 10;
		}

		public SpiritualityTapestry( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x49B2, 0x49BE )] 
	public class HonorTapestry : Item
	{
		[Constructable]
		public HonorTapestry() : base( 0x49B2 )
		{
			Weight = 10;
		}

		public HonorTapestry( Serial serial ) : base( serial )
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
	
	[FlipableAttribute( 0x49A3, 0x49C0 )] 
	public class HumilityTapestry : Item
	{
		[Constructable]
		public HumilityTapestry() : base( 0x49A3 )
		{
			Weight = 10;
		}

		public HumilityTapestry( Serial serial ) : base( serial )
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
	
	[FlipableAttribute( 0x49A7, 0x49BA )] 
	public class JusticeTapestry : Item
	{
		[Constructable]
		public JusticeTapestry() : base( 0x49A7 )
		{
			Weight = 10;
		}

		public JusticeTapestry( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x49A8, 0x49B5 )] 
	public class CompassionTapestry : Item
	{
		[Constructable]
		public CompassionTapestry() : base( 0x49A8 )
		{
			Weight = 10;
		}

		public CompassionTapestry( Serial serial ) : base( serial )
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
	
	[FlipableAttribute( 0x49B3, 0x49BB )] 
	public class ValorTapestry : Item
	{
		[Constructable]
		public ValorTapestry() : base( 0x49B3 )
		{
			Weight = 10;
		}

		public ValorTapestry( Serial serial ) : base( serial )
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
	
	[FlipableAttribute( 0x49A2, 0x49BF )] 
	public class HonestyTapestry : Item
	{
		[Constructable]
		public HonestyTapestry() : base( 0x49A2 )
		{
			Weight = 10;
		}

		public HonestyTapestry( Serial serial ) : base( serial )
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