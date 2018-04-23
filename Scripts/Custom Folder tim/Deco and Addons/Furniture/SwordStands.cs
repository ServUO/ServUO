using System;

namespace Server.Items
{
	[FlipableAttribute(0x2A45, 0x2A46)]
	public class RedSword : Item
	{
		[Constructable]
		public RedSword() : base(0x2A45)
		{
			Weight=10.0;
		
		}
		public RedSword( Serial serial ) : base( serial ){}
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
	
	[FlipableAttribute(0x2A47, 0x2A48)]
	public class BlackSword : Item
	{
		[Constructable]
		public BlackSword() : base(0x2A47)
		{ 
			Weight=10.0;
		}
		public BlackSword( Serial serial ) : base( serial ){}
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
	[FlipableAttribute(0x2A49, 0x2A4A)]
	public class PurpleSword : Item
	{
		[Constructable]
		public PurpleSword() : base(0x2A49)
		{
			Weight=10.0;
		}
		public PurpleSword( Serial serial ) : base( serial ){}
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

	[FlipableAttribute(0x2A4B, 0x2A4C)]
	public class GreenSword : Item
	{
		[Constructable]
		public GreenSword() : base(0x2A4B)
		{
			Weight=10.0;
	
		}
		public GreenSword( Serial serial ) : base( serial ){}
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

	[FlipableAttribute(0x2855, 0x2856)]
	public class WhiteSword : Item
	{
		[Constructable]
		public WhiteSword() : base(0x2855)
		{
			Weight=10.0;
	
		}
		public WhiteSword( Serial serial ) : base( serial ){}
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

	[FlipableAttribute(0x2844, 0x2845)]
	public class WoodEmptyStand : Item
	{
		[Constructable]
		public WoodEmptyStand() : base(0x2844)
		{
			Weight=3.0;
		
		}
		public WoodEmptyStand( Serial serial ) : base( serial ){}
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

	[FlipableAttribute(0x2842, 0x2843)]
	public class BlackEmptyStand : Item
	{
		[Constructable]
		public BlackEmptyStand() : base(0x2842)
		{
			Weight=3.0;		
		
		}
		public BlackEmptyStand( Serial serial ) : base( serial ){}
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
	[FlipableAttribute(0x291E, 0x291F)]
	public class GoldSwordD : Item
	{
		[Constructable]
		public GoldSwordD() : base(0x291E)
		{
			Weight=10.0;
		
		}
		public GoldSwordD( Serial serial ) : base( serial ){}
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