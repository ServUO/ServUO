//Created by Peoharen
using System;

namespace Server.Items
{
	public class SWPaperSEDoor : BaseDoor
	{
		[Constructable]
		public SWPaperSEDoor() : base( 0x2A05, 0x2A06, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public SWPaperSEDoor( Serial serial ) : base( serial )
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

	public class SEPaperSEDoor : BaseDoor
	{
		[Constructable]
		public SEPaperSEDoor() : base( 0x2A07, 0x2A08, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public SEPaperSEDoor( Serial serial ) : base( serial )
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

	public class ESPaperSEDoor : BaseDoor
	{
		[Constructable]
		public ESPaperSEDoor() : base( 0x2A09, 0x2A0A, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public ESPaperSEDoor( Serial serial ) : base( serial )
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

	public class ENPaperSEDoor : BaseDoor
	{
		[Constructable]
		public ENPaperSEDoor() : base( 0x2A0B, 0x2A0C, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public ENPaperSEDoor( Serial serial ) : base( serial )
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

	public class SWClothSEDoor : BaseDoor
	{
		[Constructable]
		public SWClothSEDoor() : base( 0x2A0D, 0x2A0E, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public SWClothSEDoor( Serial serial ) : base( serial )
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

	public class SEClothSEDoor : BaseDoor
	{
		[Constructable]
		public SEClothSEDoor() : base( 0x2A0F, 0x2A10, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public SEClothSEDoor( Serial serial ) : base( serial )
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

	public class ESClothSEDoor : BaseDoor
	{
		[Constructable]
		public ESClothSEDoor() : base( 0x2A11, 0x2A12, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public ESClothSEDoor( Serial serial ) : base( serial )
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

	public class ENClothSEDoor : BaseDoor
	{
		[Constructable]
		public ENClothSEDoor() : base( 0x2A13, 0x2A14, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public ENClothSEDoor( Serial serial ) : base( serial )
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

	public class SWWoodenSEDoor : BaseDoor
	{
		[Constructable]
		public SWWoodenSEDoor() : base( 0x2A16, 0x2A17, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public SWWoodenSEDoor( Serial serial ) : base( serial )
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

	public class SEWoodenSEDoor : BaseDoor
	{
		[Constructable]
		public SEWoodenSEDoor() : base( 0x2A17, 0x2A18, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public SEWoodenSEDoor( Serial serial ) : base( serial )
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

	public class ESWoodenSEDoor : BaseDoor
	{
		[Constructable]
		public ESWoodenSEDoor() : base( 0x2A19, 0x2A1A, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public ESWoodenSEDoor( Serial serial ) : base( serial )
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

	public class ENWoodenSEDoor : BaseDoor
	{
		[Constructable]
		public ENWoodenSEDoor() : base( 0x2A1B, 0x2A1C, 0x539, 0x539, new Point3D( 0, 0, 0 ) )
		{
		}

		public ENWoodenSEDoor( Serial serial ) : base( serial )
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