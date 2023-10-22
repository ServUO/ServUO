using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Xanthos.Evo
{
	public class MutantEvoDust : BaseEvoDust
	{
		[Constructable]
		public MutantEvoDust() : this( 500 )
		{
		}

		[Constructable]
		public MutantEvoDust( int amount ) : base( amount )
		{
			Amount = amount;
			Name = "Mutant Dust";
			Hue = 1175;
		}

		public MutantEvoDust( Serial serial ) : base ( serial )
		{
		}

		public override BaseEvoDust NewDust()
		{
			return new MutantEvoDust();
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