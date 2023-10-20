using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	public class RaelisDragonEgg : BaseEvoEgg
	{
		public override IEvoCreature GetEvoCreature()
		{
			return new RaelisDragon( "a dragon hatchling" );
		}

		[Constructable]
		public RaelisDragonEgg() : base()
		{
			Name = "a dragon egg";
			HatchDuration = 0.01;		// 15 minutes
		}

		public RaelisDragonEgg( Serial serial ) : base ( serial )
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