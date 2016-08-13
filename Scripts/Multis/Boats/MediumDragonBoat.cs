using System;
using Server;
using Server.Items;

namespace Server.Multis
{
	public class MediumDragonBoat : BaseBoat
	{
		public override int NorthID{ get{ return 0xC; } }
		public override int  EastID{ get{ return 0xD; } }
		public override int SouthID{ get{ return 0xE; } }
		public override int  WestID{ get{ return 0xF; } }

		public override int HoldDistance{ get{ return 4; } }
		public override int TillerManDistance{ get{ return -5; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, 0 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -2, 0 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 1, 3 ); } }

		public override BaseDockedBoat DockedBoat{ get{ return new MediumDockedDragonBoat( this ); } }

		[Constructable]
		public MediumDragonBoat(Direction d) : base(d, true)
		{
		}

		public MediumDragonBoat( Serial serial ) : base( serial )
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

	public class MediumDragonBoatDeed : BaseBoatDeed
	{
		public override int LabelNumber{ get{ return 1041208; } } // medium dragon ship deed
        public override BaseBoat Boat { get { return new MediumDragonBoat(this.BoatDirection); } }

		[Constructable]
		public MediumDragonBoatDeed() : base( 0xC, Point3D.Zero )
		{
		}

		public MediumDragonBoatDeed( Serial serial ) : base( serial )
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

	public class MediumDockedDragonBoat : BaseDockedBoat
	{
        public override int LabelNumber { get { return 1116744; } } //Medium Dragon Ship
        public override BaseBoat Boat { get { return new MediumDragonBoat(this.BoatDirection); } }

		public MediumDockedDragonBoat( BaseBoat boat ) : base( 0xC, Point3D.Zero, boat )
		{
		}

		public MediumDockedDragonBoat( Serial serial ) : base( serial )
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