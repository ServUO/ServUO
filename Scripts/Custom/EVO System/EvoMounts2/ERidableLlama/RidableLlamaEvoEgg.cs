using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	public class RidableLlamaEvoEgg : BaseEvoEgg
	{
		public override IEvoCreature GetEvoCreature()
		{
            return new EvoRidableLlama("a baby ridable llama");
		}

		[Constructable]
		public RidableLlamaEvoEgg() : base()
		{
			Name = "a ridable llama egg";
            Hue = 32;
			HatchDuration = 0.01;		// 15 minutes
		}

        public RidableLlamaEvoEgg(Serial serial)
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