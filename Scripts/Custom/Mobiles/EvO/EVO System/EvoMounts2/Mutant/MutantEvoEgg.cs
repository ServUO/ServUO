using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	public class MutantEvoEgg : BaseEvoEgg
	{
		public override IEvoCreature GetEvoCreature()
		{
			return new EvoMutant( "a Mutant hatchling" );
		}

		[Constructable]
		public MutantEvoEgg() : base()
		{
			Name = "a mutant steed egg";
			Hue = 1175;
			ItemID = 3165;
			HatchDuration = 0.01;		// 15 minutes
		}

		public MutantEvoEgg( Serial serial ) : base ( serial )
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