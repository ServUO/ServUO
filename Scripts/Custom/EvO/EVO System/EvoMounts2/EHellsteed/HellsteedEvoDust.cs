using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Xanthos.Evo
{
	public class HellsteedEvoDust : BaseEvoDust
	{
		[Constructable]
		public HellsteedEvoDust() : this( 500 )
		{
		}

		[Constructable]
		public HellsteedEvoDust( int amount ) : base( amount )
		{
			Amount = amount;
			Name = "hellsteed dust";
			Hue = 1268;
		}

		public HellsteedEvoDust( Serial serial ) : base ( serial )
		{
		}

		public override BaseEvoDust NewDust()
		{
			return new HellsteedEvoDust();
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