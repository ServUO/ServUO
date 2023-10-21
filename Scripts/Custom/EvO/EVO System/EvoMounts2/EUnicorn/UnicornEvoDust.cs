using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Xanthos.Evo
{
	public class UnicornEvoDust : BaseEvoDust
	{
		[Constructable]
		public UnicornEvoDust() : this( 500 )
		{
		}

		[Constructable]
		public UnicornEvoDust( int amount ) : base( amount )
		{
			Amount = amount;
			Name = "unicorn dust";
			Hue = 1278;
		}

		public UnicornEvoDust( Serial serial ) : base ( serial )
		{
		}

		public override BaseEvoDust NewDust()
		{
			return new UnicornEvoDust();
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