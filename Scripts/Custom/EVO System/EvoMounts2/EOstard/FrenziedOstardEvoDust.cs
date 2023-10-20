using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Xanthos.Evo
{
	public class FrenziedOstardEvoDust : BaseEvoDust
	{
		[Constructable]
		public FrenziedOstardEvoDust() : this( 500 )
		{
		}

		[Constructable]
		public FrenziedOstardEvoDust( int amount ) : base( amount )
		{
			Amount = amount;
			Name = "frenzied ostard dust";
			Hue = 1173;
		}

        public FrenziedOstardEvoDust(Serial serial)
            : base(serial)
		{
		}

		public override BaseEvoDust NewDust()
		{
            return new FrenziedOstardEvoDust();
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