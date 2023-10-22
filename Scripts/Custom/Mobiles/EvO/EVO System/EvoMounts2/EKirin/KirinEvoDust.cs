using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Xanthos.Evo
{
	public class KirinEvoDust : BaseEvoDust
	{
		[Constructable]
		public KirinEvoDust() : this( 500 )
		{
		}

		[Constructable]
		public KirinEvoDust( int amount ) : base( amount )
		{
			Amount = amount;
			Name = "kirin dust";
			Hue = 91;
		}

		public KirinEvoDust( Serial serial ) : base ( serial )
		{
		}

		public override BaseEvoDust NewDust()
		{
			return new KirinEvoDust();
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