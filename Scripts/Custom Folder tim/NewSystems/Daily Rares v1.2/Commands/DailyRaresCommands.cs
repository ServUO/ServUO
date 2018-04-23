using System;
using System.IO;
using System.Collections;

using Server;
using Server.Mobiles; 
using Server.Gumps; 
using Server.Targeting;
using Server.Scripts.Commands;
using Server.Items;
using Server.Commands;

namespace Server.Scripts.Commands
{
	public class DailyRareCommands
	{
		public static void Initialize()
		{
			CommandSystem.Register( "ForceSpawnDailys", AccessLevel.Administrator, new CommandEventHandler( ForceSpawnDailys_OnCommand ) );
			CommandSystem.Register( "ForceResetDailys", AccessLevel.Administrator, new CommandEventHandler( ForceResetDailys_OnCommand ) );
		}

		[Usage( "ForceSpawnDailys" )]
		[Description( "Force Spawns All Daily Rares" )]
		private static void ForceSpawnDailys_OnCommand( CommandEventArgs e )
		{
			DailyRaresSystem.StartRareSpawn( true );
			
			//if ( Create() )
			//	from.SendMessage( "All Daily Rares spawners generated." );
			//else
			//	from.SendMessage( "All Daily Rares spawners already present." );
		}

		[Usage( "ForceResetDailys" )]
		[Description( "Will reset the daily rare system. Deleting all spawned rares. and update water barrels / tubs / buckets" )]
		private static void ForceResetDailys_OnCommand( CommandEventArgs e )
		{
			DailyRaresSystem.StartRareSpawn( false );
			
			//if ( Create() )
			//	from.SendMessage( "All Daily Rares Reset." );
			//else
			//	from.SendMessage( "All Daily Rares Failed To Reset." );
		}
	}
}