using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Xanthos.Evo
{
	public class RidablePolarBearEvoDust : BaseEvoDust
	{
		[Constructable]
		public RidablePolarBearEvoDust() : this( 500 )
		{
		}

		[Constructable]
		public RidablePolarBearEvoDust( int amount ) : base( amount )
		{
			Amount = amount;
			Name = "ridable polar bear dust";
			Hue = 1360;
		}

        public RidablePolarBearEvoDust(Serial serial)
            : base(serial)
		{
		}

		public override BaseEvoDust NewDust()
		{
            return new RidablePolarBearEvoDust();
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