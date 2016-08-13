using System;
using Server;
using Server.Items;

namespace Server.Multis
{
	public class LargeDragonBoat : BaseBoat
	{
		public override int NorthID{ get{ return 0x14; } }
		public override int  EastID{ get{ return 0x15; } }
		public override int SouthID{ get{ return 0x16; } }
		public override int  WestID{ get{ return 0x17; } }

		public override int HoldDistance{ get{ return 5; } }
		public override int TillerManDistance{ get{ return -5; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, -1 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -2, -1 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 0, 3 ); } }

		public override BaseDockedBoat DockedBoat{ get{ return new LargeDockedDragonBoat( this ); } }

		[Constructable]
		public LargeDragonBoat(Direction d) : base(d, true)
		{
		}

		public LargeDragonBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}

	public class LargeDragonBoatDeed : BaseBoatDeed
	{
		public override int LabelNumber{ get{ return 1041210; } }// large dragon ship deed
        public override BaseBoat Boat { get { return new LargeDragonBoat(this.BoatDirection); } }

		[Constructable]
		public LargeDragonBoatDeed() : base( 0x14, new Point3D( 0, -1, 0 ) )
		{
		}

		public LargeDragonBoatDeed( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}

	public class LargeDockedDragonBoat : BaseDockedBoat
	{
        public override int LabelNumber { get { return 1116746; } } //Large  Dragon Ship
		public override BaseBoat Boat{ get{ return new LargeDragonBoat(this.BoatDirection); } }

		public LargeDockedDragonBoat( BaseBoat boat ) : base( 0x14, new Point3D( 0, -1, 0 ), boat )
		{
		}

		public LargeDockedDragonBoat( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}
	}
}