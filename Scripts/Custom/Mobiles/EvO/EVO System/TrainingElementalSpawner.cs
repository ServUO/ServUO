#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Xanthos.Evo
{
	public class TrainingElementalSpawner : Spawner
	{
		[Constructable]
		public TrainingElementalSpawner() : base( "TrainingElemental" )
		{
			HomeRange = 1;
			MinDelay = TimeSpan.FromMinutes( 0 );
			MaxDelay = TimeSpan.FromMinutes( 1 );
			Spawn();
		}

		public TrainingElementalSpawner( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );	// version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}