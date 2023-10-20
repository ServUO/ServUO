using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Xanthos.Evo
{
	public class RidableLlamaEvoDust : BaseEvoDust
	{
		[Constructable]
		public RidableLlamaEvoDust() : this( 500 )
		{
		}

		[Constructable]
		public RidableLlamaEvoDust( int amount ) : base( amount )
		{
			Amount = amount;
			Name = "ridable llama dust";
			Hue = 32;
		}

        public RidableLlamaEvoDust(Serial serial)
            : base(serial)
		{
		}

		public override BaseEvoDust NewDust()
		{
            return new RidableLlamaEvoDust();
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