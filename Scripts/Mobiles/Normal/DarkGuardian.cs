using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dark guardians's corpse" )]
	public class DarkGuardian : Lich
	{
		[Constructable]
		public DarkGuardian()
		{
            Name = "a dark guardian";
            Hue = 931;
		}

		public override int TreasureMapLevel{ get{ return Utility.RandomMinMax(1, 3); } }

		public DarkGuardian( Serial serial ) : base( serial )
		{
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