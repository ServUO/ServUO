using System;
using Server;
using Server.Items;
using Server.Lokai;

namespace Server.Items
{
	public class LinkedGateBag : Bag
	{
		public override string DefaultName
		{
			get { return "a Bag of Linked Gates"; }
		}

		[Constructable]
		public LinkedGateBag() : this( "Alpha" )
		{
			Movable = true;
			Hue = 0x105;
		}

		[Constructable]
		public LinkedGateBag( string name )
		{
			LinkedGate gateA = new LinkedGate();
			LinkedGate gateB = new LinkedGate();
			gateA.MateGate = gateB;
			gateB.MateGate = gateA;
			gateA.Name = name;
			gateB.Name = name;
			DropItem( gateA );
			DropItem( gateB );
		}

		public LinkedGateBag( Serial serial ) : base( serial )
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