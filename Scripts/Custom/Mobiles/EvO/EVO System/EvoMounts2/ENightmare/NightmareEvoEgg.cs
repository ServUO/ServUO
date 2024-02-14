using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	public class NightmareEvoEgg : BaseEvoEgg
	{
		public override IEvoCreature GetEvoCreature()
		{
			return new EvoNightmare( "a baby nightmare" );
		}

		[Constructable]
		public NightmareEvoEgg() : base()
		{
			Name = "a nightmare egg";
            Hue = 1153;
			HatchDuration = 0.01;		// 15 minutes
		}

		public NightmareEvoEgg( Serial serial ) : base ( serial )
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