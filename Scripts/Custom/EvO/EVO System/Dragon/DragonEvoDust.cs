using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Xanthos.Evo
{
	public class RaelisDragonDust : BaseEvoDust
	{
		[Constructable]
		public RaelisDragonDust() : this( 500 )
		{
		}

		[Constructable]
		public RaelisDragonDust( int amount ) : base( amount )
		{
			Amount = amount;
			Name = "Dragon Dust";
			Hue = 1153;
		}

		public RaelisDragonDust( Serial serial ) : base ( serial )
		{
		}

		public override BaseEvoDust NewDust()
		{
			return new RaelisDragonDust();
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