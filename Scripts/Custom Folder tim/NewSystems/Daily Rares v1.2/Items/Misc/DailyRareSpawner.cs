using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class DailyRareSpawner : Spawner
	{
		// Basicly a normal spawner but this is so can be easily deleted later by control panel.
		// And dont let the name fool you these do not spawn daily rares but extras needed for system
		// The DailyRaresSystem.cs spawns the daily rares.

		[Constructable]
		public DailyRareSpawner()
		{
			Name = "Daily Rare Spawner";
			ItemID = 0x1ECD;
		}

		public DailyRareSpawner( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}