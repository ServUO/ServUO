using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Xanthos.Evo
{
	public class CuSidheEvoDust : BaseEvoDust
	{
		[Constructable]
		public CuSidheEvoDust() : this( 500 )
		{
		}

		[Constructable]
		public CuSidheEvoDust( int amount ) : base( amount )
		{
			Amount = amount;
			Name = " cu sidhe dust";
			Hue = 1167;
		}

        public CuSidheEvoDust(Serial serial)
            : base(serial)
		{
		}

		public override BaseEvoDust NewDust()
		{
            return new CuSidheEvoDust();
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