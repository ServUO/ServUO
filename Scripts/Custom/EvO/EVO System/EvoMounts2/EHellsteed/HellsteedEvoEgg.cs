using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	public class HellsteedEvoEgg : BaseEvoEgg
	{
		public override IEvoCreature GetEvoCreature()
		{
			return new EvoHellsteed( "a baby hellsteed" );
		}

		[Constructable]
		public HellsteedEvoEgg() : base()
		{
			Name = "a hellsteed egg";
            Hue = 1268;
			HatchDuration = 0.01;		// 15 minutes
		}

		public HellsteedEvoEgg( Serial serial ) : base ( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}