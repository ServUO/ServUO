using System;
using Server.Network;

namespace Server.Items
{
	public class PowerCrystalTwo : Item
	{
		public override string DefaultName
		{
			get { return "power crystal"; }
		}

		[Constructable]
		public PowerCrystalTwo() : base( 0x1F1C )
		{
			Weight = 1.0;
                        Hue = 328;
                        Name = "Dark Matter";
			Light = LightType.DarkCircle300;
		}

		public PowerCrystalTwo( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( this.GetWorldLocation(), 3 ))
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			else
				from.SendAsciiMessage( "This looks like part of a larger contraption." );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}