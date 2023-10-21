using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	public class RidablePolarBearEvoEgg : BaseEvoEgg
	{
		public override IEvoCreature GetEvoCreature()
		{
			return new EvoRidablePolarBear( "a baby ridable polar bear" );
		}

		[Constructable]
		public RidablePolarBearEvoEgg() : base()
		{
			Name = "a ridable polar bear egg";
            Hue = 1360;
			HatchDuration = 0.01;		// 15 minutes
		}

        public RidablePolarBearEvoEgg(Serial serial)
            : base(serial)
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