using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;

namespace Server.Commands
{
	public class Test
	{
		public static void Initialize()
		{
			CommandSystem.Register("Test", AccessLevel.GameMaster, new CommandEventHandler(Test_DoCommand));
		}

		private static void Test_DoCommand(CommandEventArgs e)
		{
			if (e.ArgString == "tmaps")
				Test_Tmaps(e);
			else if (e.ArgString == "tmapdrop")
				Test_TmapDrop(e);
		}

		private static void Test_TmapDrop(CommandEventArgs e)
		{
			int count = 0;
			int itters = 10000;
			for(int i = 0; i < itters; ++i)
			{
				Mobiles.Ettin mob = new Mobiles.Ettin();
				mob.NoKillAwards = false;
				mob.OnBeforeDeath();
				if (mob.Backpack.FindItemByType(typeof(TreasureMap)) != null)
					++count;
			}
			double percent = ((double)count / (double)itters) * 100;
			e.Mobile.SendMessage("Generated {0} Ettins, {1} ({2:0.00}%) dropped TMaps. Expected {3:0.00}%",
				itters, count, percent, TreasureMap.LootChance * 100);
		}

		private static void Test_Tmaps(CommandEventArgs e)
		{
			Dictionary<UInt64, int> stats = new Dictionary<ulong, int>();
			for(int i = 0; i < 10000; ++i)
			{
				TreasureMap tmap = new TreasureMap(
					Utility.Random(7),
					Map.Felucca
				);
				UInt64 key = (UInt64)(tmap.ChestLocation.X | (tmap.ChestLocation.Y << 32));
				if (!stats.ContainsKey(key))
				{
					stats.Add(key, 1);
				}
				else
				{
					stats[key] = stats[key] + 1;
				}
			}

			e.Mobile.SendMessage(String.Format("Total Locations: {0}", stats.Count));

			int expectedAverage = 10000 / stats.Count;
			int maxDeviation = 0;
			foreach(int count in stats.Values)
			{
				maxDeviation = Math.Max(Math.Abs(expectedAverage - count), maxDeviation);
			}

			e.Mobile.SendMessage(String.Format("Max Deviation: {0}", maxDeviation));
		}
	}
}
