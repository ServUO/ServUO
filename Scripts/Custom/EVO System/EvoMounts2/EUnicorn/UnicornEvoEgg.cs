using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	public class UnicornEvoEgg : BaseEvoEgg
	{
		public override IEvoCreature GetEvoCreature()
		{
			return new EvoUnicorn( "a baby unicorn" );
		}

		[Constructable]
		public UnicornEvoEgg() : base()
		{
			Name = "a unicorn egg";
            Hue = 1278;
			HatchDuration = 0.01;		// 15 minutes
		}

		public UnicornEvoEgg( Serial serial ) : base ( serial )
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