//Created by Peoharen
using System;

namespace Server.Items
{
	public class NorthWestDoor : BaseDoor
	{
		[Constructable]
		public NorthWestDoor() : base( 0x679, 0x67A, 0xEC, 0xF3, new Point3D( -1, 0, 0 ) )
		{
		}

		public NorthWestDoor( Serial serial ) : base( serial )
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

	public class NorthEastDoor : BaseDoor
	{
		[Constructable]
		public NorthEastDoor() : base( 0x67B, 0x67C, 0xEC, 0xF3, new Point3D( 1, -1, 0 ) )
		{
		}

		public NorthEastDoor( Serial serial ) : base( serial )
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

	public class SouthWestDoor : BaseDoor
	{
		[Constructable]
		public SouthWestDoor() : base( 0x675, 0x676, 0xEC, 0xF3, new Point3D( -1, 1, 0 ) )
		{
		}

		public SouthWestDoor( Serial serial ) : base( serial )
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

	public class SouthEastDoor : BaseDoor
	{
		[Constructable]
		public SouthEastDoor() : base( 0x677, 0x678, 0xEC, 0xF3, new Point3D( 1, 1, 0 ) )
		{
		}

		public SouthEastDoor( Serial serial ) : base( serial )
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

	public class WestNorthDoor : BaseDoor
	{
		[Constructable]
		public WestNorthDoor() : base( 0x683, 1, 0xEC, 0xF3, new Point3D( -1, -1, 0 ) )
		{
		}

		public WestNorthDoor( Serial serial ) : base( serial )
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

	public class WestSouthDoor : BaseDoor
	{
		[Constructable]
		public WestSouthDoor() : base( 0x681, 1, 0xEC, 0xF3, new Point3D( -1, 1, 0 ) )
		{
		}

		public WestSouthDoor( Serial serial ) : base( serial )
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

	public class EastNorthDoor : BaseDoor
	{
		[Constructable]
		public EastNorthDoor() : base( 0x67F, 1, 0xEC, 0xF3, new Point3D( 1, -1, 0 ) )
		{
		}

		public EastNorthDoor( Serial serial ) : base( serial )
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

	public class EastSouthDoor : BaseDoor
	{
		[Constructable]
		public EastSouthDoor() : base( 0x67D, 0x67E, 0xEC, 0xF3, new Point3D( 0, 1, 0 ) )
		{
		}

		public EastSouthDoor( Serial serial ) : base( serial )
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
/*
	public class Door : BaseDoor
	{
		[Constructable]
		public Door() : base( , , 0xEC, 0xF3, new Point3D( 0, 0, 0 ) )
		{
		}

		public Door( Serial serial ) : base( serial )
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
*/