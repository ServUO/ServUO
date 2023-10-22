using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Xanthos.Evo
{
	public class NightmareEvoDust : BaseEvoDust
	{
		[Constructable]
		public NightmareEvoDust() : this( 500 )
		{
		}

		[Constructable]
		public NightmareEvoDust( int amount ) : base( amount )
		{
			Amount = amount;
			Name = "nightmare dust";
			Hue = 1153;
		}

		public NightmareEvoDust( Serial serial ) : base ( serial )
		{
		}

		public override BaseEvoDust NewDust()
		{
			return new NightmareEvoDust();
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