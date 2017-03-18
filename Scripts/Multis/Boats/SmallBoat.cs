using System;
using Server;
using Server.Items;

namespace Server.Multis
{
	public class SmallBoat : BaseBoat
	{
		public override int NorthID{ get{ return 0x0; } }
		public override int  EastID{ get{ return 0x1; } }
		public override int SouthID{ get{ return 0x2; } }
		public override int  WestID{ get{ return 0x3; } }

		public override int HoldDistance{ get{ return 4; } }
		public override int TillerManDistance{ get{ return -4; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, 0 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -2, 0 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 1, 3 ); } }

		public override BaseDockedBoat DockedBoat{ get{ return new SmallDockedBoat( this ); } }

		[Constructable]
		public SmallBoat(Direction d) : base(d, true)
		{
		}

		public SmallBoat( Serial serial ) : base( serial )
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

	public class SmallBoatDeed : BaseBoatDeed
	{
		public override int LabelNumber{ get{ return 1041205; } } // small ship deed
        public override BaseBoat Boat { get { return new SmallBoat(this.BoatDirection); } }

		[Constructable]
		public SmallBoatDeed() : base( 0x0, Point3D.Zero )
		{
		}

		public SmallBoatDeed( Serial serial ) : base( serial )
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

	public class SmallDockedBoat : BaseDockedBoat
	{
        public override int LabelNumber { get { return 1116741; } } //Small Ship
        public override BaseBoat Boat { get { return new SmallBoat(this.BoatDirection); } }

		public SmallDockedBoat( BaseBoat boat ) : base( 0x0, Point3D.Zero, boat )
		{
		}

		public SmallDockedBoat( Serial serial ) : base( serial )
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