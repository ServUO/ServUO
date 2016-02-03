using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Server.Gumps;
using Server.Commands;

namespace Server.Engines.CannedEvil
{
	public class ChampionSystem
	{
		private static bool m_Enabled = false;
		private static bool m_Initialized = false;
		private static readonly string m_Path = Path.Combine("Saves", "Champions", "ChampionSystem.bin");
		private static DateTime m_LastRotate;
		private static TimeSpan m_RotateDelay;
		private static List<ChampionSpawn> m_AllSpawns = new List<ChampionSpawn>();
		private static List<ChampionSpawn> m_DungeonSpawns = new List<ChampionSpawn>();
		private static List<ChampionSpawn> m_LostLandsSpawns = new List<ChampionSpawn>();
		private static InternalTimer m_Timer;
		private static int m_GoldShowerPiles;
		private static int m_GoldShowerMinAmount;
		private static int m_GoldShowerMaxAmount;
		private static int m_PowerScrollAmount;

		public static int GoldShowerPiles { get { return m_GoldShowerPiles; } }
		public static int GoldShowerMinAmount { get { return m_GoldShowerMinAmount; } }
		public static int GoldShowerMaxAmount { get { return m_GoldShowerMaxAmount; } }
		public static int PowerScrollAmount { get { return m_PowerScrollAmount; } }

		private static void Configure()
		{
			m_Enabled = Config.Get("Champions.Enabled", true);
			m_RotateDelay = Config.Get("Champions.RotateDelay", TimeSpan.FromDays(1.0d));
			m_GoldShowerPiles = Config.Get("Champions.GoldPiles", 50);
			m_GoldShowerMinAmount = Config.Get("Champions.GoldMin", 2500);
			m_GoldShowerMaxAmount = Config.Get("Champions.GoldMax", 7500);
			m_PowerScrollAmount = Config.Get("Champions.PowerScrolls", 6);
			EventSink.WorldLoad += EventSink_WorldLoad;
			EventSink.WorldSave += EventSink_WorldSave;
		}

		private static void EventSink_WorldSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(
				m_Path,
				writer =>
				{
					writer.Write(0); // Version
					writer.Write(m_Initialized);
					writer.Write(m_LastRotate);
					writer.WriteItemList(m_AllSpawns, true);
					writer.WriteItemList(m_DungeonSpawns, true);
					writer.WriteItemList(m_LostLandsSpawns, true);
				});
		}

		private static void EventSink_WorldLoad()
		{
			Persistence.Deserialize(
				m_Path,
				reader =>
				{
					int version = reader.ReadInt();
					m_Initialized = reader.ReadBool();
					m_LastRotate = reader.ReadDateTime();
					m_AllSpawns.AddRange(reader.ReadItemList().Cast<ChampionSpawn>());
					m_DungeonSpawns.AddRange(reader.ReadItemList().Cast<ChampionSpawn>());
					m_LostLandsSpawns.AddRange(reader.ReadItemList().Cast<ChampionSpawn>());
				});
		}

		private static void Initialize()
		{
			CommandSystem.Register("ChampionInfo", AccessLevel.GameMaster, new CommandEventHandler(ChampionInfo_OnCommand));

			if (!m_Enabled)
			{
				foreach (ChampionSpawn s in m_AllSpawns)
				{
					s.Delete();
				}
				m_Initialized = false;
				return;
			}

			m_Timer = new InternalTimer();

			if (m_Initialized)
				return;

			ChampionSpawn spawn;

			// Dungeon Spawns

			// Deceit
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Deceit";
			spawn.Type = ChampionSpawnType.UnholyTerror;
			spawn.MoveToWorld(new Point3D(5178, 708, 20), Map.Felucca);
			m_DungeonSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Despise
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Despise";
			spawn.Type = ChampionSpawnType.VerminHorde;
			spawn.MoveToWorld(new Point3D(5557, 824, 65), Map.Felucca);
			m_DungeonSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Destard
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Destard";
			spawn.Type = ChampionSpawnType.ColdBlood;
			spawn.MoveToWorld(new Point3D(5259, 837, 61), Map.Felucca);
			m_DungeonSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Fire
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Fire";
			spawn.Type = ChampionSpawnType.Abyss;
			spawn.MoveToWorld(new Point3D(5814, 1350, 2), Map.Felucca);
			m_DungeonSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Terathan Keep
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Tera Keep";
			spawn.Type = ChampionSpawnType.Arachnid;
			spawn.MoveToWorld(new Point3D(5190, 1605, 20), Map.Felucca);
			m_DungeonSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Abyssal Lair
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Abyss";
			spawn.Type = ChampionSpawnType.Terror;
			spawn.MoveToWorld(new Point3D(6995, 733, 76), Map.Felucca);
			m_DungeonSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Primeval Lich
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Lich";
			spawn.Type = ChampionSpawnType.Infuse;
			spawn.MoveToWorld(new Point3D(7000, 1004, 5), Map.Felucca);
			m_DungeonSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Lost Lands Spawns

			// Desert
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Desert";
			spawn.RandomizeType = true;
			spawn.MoveToWorld(new Point3D(5636, 2916, 37), Map.Felucca);
			m_LostLandsSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Tortoise
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Tortoise";
			spawn.RandomizeType = true;
			spawn.MoveToWorld(new Point3D(5724, 3991, 42), Map.Felucca);
			m_LostLandsSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Ice West
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Ice West";
			spawn.RandomizeType = true;
			spawn.MoveToWorld(new Point3D(5511, 2360, 40), Map.Felucca);
			m_LostLandsSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Oasis
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Oasis";
			spawn.RandomizeType = true;
			spawn.MoveToWorld(new Point3D(5549, 2640, 15), Map.Felucca);
			m_LostLandsSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Terra Sanctum
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Terra";
			spawn.RandomizeType = true;
			spawn.MoveToWorld(new Point3D(6035, 2944, 52), Map.Felucca);
			m_LostLandsSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Lord Oaks
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Lord Oaks";
			spawn.Type = ChampionSpawnType.ForestLord;
			spawn.MoveToWorld(new Point3D(5559, 3757, 21), Map.Felucca);
			m_DungeonSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Marble
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Marble";
			spawn.RandomizeType = true;
			spawn.MoveToWorld(new Point3D(5267, 3171, 104), Map.Felucca);
			m_LostLandsSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Hoppers Bog
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Hoppers Bog";
			spawn.RandomizeType = true;
			spawn.MoveToWorld(new Point3D(5954, 3475, 25), Map.Felucca);
			m_LostLandsSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Khaldun
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Khaldun";
			spawn.RandomizeType = true;
			spawn.MoveToWorld(new Point3D(5982, 3882, 20), Map.Felucca);
			m_LostLandsSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Ice East
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Ice East";
			spawn.RandomizeType = true;
			spawn.MoveToWorld(new Point3D(6038, 2400, 46), Map.Felucca);
			m_LostLandsSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Damwin Thicket
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Damwin";
			spawn.RandomizeType = true;
			spawn.MoveToWorld(new Point3D(5281, 3368, 51), Map.Felucca);
			m_LostLandsSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// City of Death
			spawn = new ChampionSpawn();
			spawn.SpawnName = "City of Death";
			spawn.RandomizeType = true;
			spawn.MoveToWorld(new Point3D(5207, 3637, 20), Map.Felucca);
			m_LostLandsSpawns.Add(spawn);
			m_AllSpawns.Add(spawn);

			// Ilshenar

			// Valor
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Valor";
			spawn.RandomizeType = true;
			spawn.AutoRestart = true;
			spawn.Active = true;
			spawn.MoveToWorld(new Point3D(382, 328, -30), Map.Ilshenar);
			m_AllSpawns.Add(spawn);

			// Humility
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Humility";
			spawn.RandomizeType = true;
			spawn.AutoRestart = true;
			spawn.Active = true;
			spawn.MoveToWorld(new Point3D(462, 926, -67), Map.Ilshenar);
			m_AllSpawns.Add(spawn);

			// Spirituality
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Spirituality";
			spawn.AutoRestart = true;
			spawn.Active = true;
			spawn.Type = ChampionSpawnType.ForestLord;
			spawn.MoveToWorld(new Point3D(1645, 1107, 8), Map.Ilshenar);
			m_AllSpawns.Add(spawn);

			// Twisted Glade
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Glade";
			spawn.AutoRestart = true;
			spawn.Active = true;
			spawn.Type = ChampionSpawnType.Glade;
			spawn.MoveToWorld(new Point3D(2212, 1260, 25), Map.Ilshenar);
			m_AllSpawns.Add(spawn);

			// Tokuno

			// Sleeping Dragon
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Dragon";
			spawn.AutoRestart = true;
			spawn.Active = true;
			spawn.Type = ChampionSpawnType.SleepingDragon;
			spawn.MoveToWorld(new Point3D(948, 434, 29), Map.Tokuno);
			m_AllSpawns.Add(spawn);

			// Malas

			// Bedlam
			spawn = new ChampionSpawn();
			spawn.SpawnName = "Bedlam";
			spawn.AutoRestart = true;
			spawn.Active = true;
			spawn.Type = ChampionSpawnType.Corrupt;
			spawn.MoveToWorld(new Point3D(174, 1629, 8), Map.Malas);
			m_AllSpawns.Add(spawn);

			Rotate();

			m_Initialized = true;
		}

		[Usage("ChampionInfo")]
		[Description("Opens a UI that displays information about the champion system")]
		private static void ChampionInfo_OnCommand(CommandEventArgs e)
		{
			if (!m_Enabled)
			{
				e.Mobile.SendMessage("The champion system is not enabled.");
				return;
			}
			if (m_AllSpawns.Count <= 0)
			{
				e.Mobile.SendMessage("The champion system is enabled but no altars exist");
				return;
			}
			e.Mobile.SendGump(new ChampionSystemGump());
		}

		private static void Rotate()
		{
			m_LastRotate = DateTime.UtcNow;

			foreach (ChampionSpawn spawn in m_DungeonSpawns)
			{
				spawn.AutoRestart = false;
			}
			foreach (ChampionSpawn spawn in m_LostLandsSpawns)
			{
				spawn.AutoRestart = false;
			}

			ChampionSpawn s;
			s = m_DungeonSpawns[Utility.Random(m_DungeonSpawns.Count)];
			s.AutoRestart = true;
			if (!s.Active)
				s.Active = true;
			s = m_LostLandsSpawns[Utility.Random(m_LostLandsSpawns.Count)];
			s.AutoRestart = true;
			if (!s.Active)
				s.Active = true;
		}

		private static void OnSlice()
		{
			if (DateTime.Now > m_LastRotate + m_RotateDelay)
				Rotate();
		}

		private class InternalTimer : Timer
		{
			public InternalTimer()
				: base(TimeSpan.FromMinutes(1.0d))
			{
				this.Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				OnSlice();
			}
		}

		private class ChampionSystemGump : Gump
		{
			private const int gBoarder = 20;
			private const int gRowHeight = 25;
			private const int gFontHue = 0;

			public ChampionSystemGump()
				: base(40, 40)
			{
				/* 20      60          20  20  20  40     30      30   30 30   20      = 320
				 * Boarder SpawnName   X   Y   Z   Map    Active  Auto Go Info Boarder
				 */
				AddBackground(0, 0, 320, gBoarder * 2 + m_AllSpawns.Count * gRowHeight + gRowHeight * 2, 0x13BE);

				int top = gBoarder;
				AddLabel(gBoarder, top, gFontHue, "Champion Spawn System Gump");
				top += gRowHeight;

				AddLabel(gBoarder + 0, top, gFontHue, "Spawn");
				AddLabel(gBoarder + 60, top, gFontHue, "X");
				AddLabel(gBoarder + 80, top, gFontHue, "Y");
				AddLabel(gBoarder + 100, top, gFontHue, "Z");
				AddLabel(gBoarder + 120, top, gFontHue, "Map");
				AddLabel(gBoarder + 160, top, gFontHue, "Active");
				AddLabel(gBoarder + 190, top, gFontHue, "Auto");
				AddLabel(gBoarder + 220, top, gFontHue, "Go");
				AddLabel(gBoarder + 250, top, gFontHue, "Info");
				top += gRowHeight;

				for (int i = 0; i < m_AllSpawns.Count; ++i)
				{
					ChampionSpawn spawn = m_AllSpawns[i];
					AddLabel(gBoarder + 0, top, gFontHue, spawn.SpawnName);
					AddLabel(gBoarder + 60, top, gFontHue, spawn.X.ToString());
					AddLabel(gBoarder + 80, top, gFontHue, spawn.Y.ToString());
					AddLabel(gBoarder + 100, top, gFontHue, spawn.Z.ToString());
					AddLabel(gBoarder + 120, top, gFontHue, spawn.Map.ToString());
					AddLabel(gBoarder + 160, top, gFontHue, spawn.Active ? "Y" : "N");
					AddLabel(gBoarder + 190, top, gFontHue, spawn.AutoRestart ? "Y" : "N");
					AddButton(gBoarder + 220, top, 0xFA5, 0xFA7, 1 + i, GumpButtonType.Reply, 0);
					AddButton(gBoarder + 250, top, 0xFA5, 0xFA7, 1001 + i, GumpButtonType.Reply, 0);
					top += gRowHeight;
				}
			}

			public override void OnResponse(Network.NetState sender, RelayInfo info)
			{
				ChampionSpawn spawn;
				int idx;

				if (info.ButtonID > 0 && info.ButtonID <= 1000)
				{
					idx = info.ButtonID - 1;
					if (idx < 0 || idx >= m_AllSpawns.Count)
						return;
					spawn = m_AllSpawns[idx];
					sender.Mobile.MoveToWorld(spawn.Location, spawn.Map);
					sender.Mobile.SendGump(this);
				}
				else if (info.ButtonID > 1000)
				{
					idx = info.ButtonID - 1001;
					if (idx < 0 || idx > m_AllSpawns.Count)
						return;
					spawn = m_AllSpawns[idx];
					spawn.SendGump(sender.Mobile);
				}
			}
		}
	}
}