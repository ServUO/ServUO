using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.CannedEvil
{
	public class ChampionSystem
	{
		private static readonly string m_SavePath = Path.Combine(Core.BaseDirectory, "Saves", "Champions", "ChampionSystem.bin");
		private static readonly string m_ConfigPath = Path.Combine(Core.BaseDirectory, "Data", "ChampionSpawns.xml");

		private static readonly int[] m_DefaultRankRedSkulls = new[] { 5, 10, 13 };
		private static readonly int[] m_DefaultRankMaxKills = new[] { 256, 128, 64, 32 };

		private static bool m_Initialized;

		private static DateTime m_LastRotate;

		public static bool Enabled { get => Config.Get("Champions.Enabled", true); set => Config.Set("Champions.Enabled", value); }

		public static TimeSpan RotateDelay { get => Config.Get("Champions.RotateDelay", TimeSpan.FromDays(1.0d)); set => Config.Set("Champions.RotateDelay", value); }

		public static int GoldShowerPiles { get => Config.Get("Champions.GoldPiles", 50); set => Config.Set("Champions.GoldPiles", value); }
		public static int GoldShowerMinAmount { get => Config.Get("Champions.GoldMin", 4000); set => Config.Set("Champions.GoldMin", value); }
		public static int GoldShowerMaxAmount { get => Config.Get("Champions.GoldMax", 5500); set => Config.Set("Champions.GoldMax", value); }

		public static int HarrowerGoldShowerPiles { get => Config.Get("Champions.HarrowerGoldPiles", 75); set => Config.Set("Champions.HarrowerGoldPiles", value); }
		public static int HarrowerGoldShowerMinAmount { get => Config.Get("Champions.HarrowerGoldMin", 5000); set => Config.Set("Champions.HarrowerGoldMin", value); }
		public static int HarrowerGoldShowerMaxAmount { get => Config.Get("Champions.HarrowerGoldMax", 10000); set => Config.Set("Champions.HarrowerGoldMax", value); }

		public static int PowerScrollAmount { get => Config.Get("Champions.PowerScrolls", 6); set => Config.Set("Champions.PowerScrolls", value); }
		public static int StatScrollAmount { get => Config.Get("Champions.StatScrolls", 16); set => Config.Set("Champions.StatScrolls", value); }

		public static double ScrollChance { get => Config.Get("Champions.ScrollChance", 0.01); set => Config.Set("Champions.ScrollChance", value); }
		public static double TranscendenceChance { get => Config.Get("Champions.TranscendenceChance", 0.50); set => Config.Set("Champions.TranscendenceChance", value); }

		public static int[] RankRedSkulls { get => Config.GetArray("Champions.RankRedSkulls", m_DefaultRankRedSkulls); set => Config.SetArray("Champions.RankRedSkulls", value); }
		public static int[] RankMaxKills { get => Config.GetArray("Champions.RankMaxKills", m_DefaultRankMaxKills); set => Config.SetArray("Champions.RankMaxKills", value); }

		public static List<ChampionSpawn> AllSpawns => ChampionSpawn.AllSpawns;

		public static int RankForLevel(int l)
		{
			var list = RankMaxKills;

			if (list != null && l >= 0)
			{
				if (l < list.Length)
				{
					return list[l];
				}

				return 3;
			}

			return 0;
		}

		public static int MaxKillsForLevel(int l)
		{
			var list = RankMaxKills;

			if (list != null && list.Length > 0)
			{
				var rank = RankForLevel(l);

				if (rank >= 0 && rank < list.Length)
				{
					return list[rank];
				}
			}

			return 0;
		}

		public static double SpawnRadiusModForLevel(int l)
		{
			switch (RankForLevel(l))
			{
				case 0: return 1.0d;
				case 1: return 0.75d;
				case 2: return 0.5d;
				default: return 0.25d;
			}
		}

		public static void Configure()
		{
			EventSink.WorldLoad += EventSink_WorldLoad;
			EventSink.WorldSave += EventSink_WorldSave;

			CommandSystem.Register("GenChampSpawns", AccessLevel.GameMaster, GenSpawns_OnCommand);
			CommandSystem.Register("DelChampSpawns", AccessLevel.GameMaster, DelSpawns_OnCommand);
			CommandSystem.Register("ChampionInfo", AccessLevel.GameMaster, ChampionInfo_OnCommand);
		}

		public static void Initialize()
		{
			Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0), OnSlice);
		}

		private static void EventSink_WorldSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(m_SavePath, writer =>
			{
				writer.Write(2); // Version

				writer.Write(m_Initialized);
				writer.Write(m_LastRotate);
			});
		}

		private static void EventSink_WorldLoad()
		{
			Persistence.Deserialize(m_SavePath, reader =>
			{
				var version = reader.ReadInt();

				m_Initialized = reader.ReadBool();
				m_LastRotate = reader.ReadDateTime();

				if (version < 2)
				{
					reader.ReadStrongItemList<ChampionSpawn>();
				}
			});
		}

		public static void GenSpawns_OnCommand(CommandEventArgs e)
		{
			LoadSpawns();

			e.Mobile.SendMessage("Champ Spawns Generated!");
		}

		public static void DelSpawns_OnCommand(CommandEventArgs e)
		{
			RemoveSpawns();

			m_Initialized = false;

			e.Mobile.SendMessage("Champ Spawns Removed!");
		}

		[Usage("ChampionInfo")]
		[Description("Opens a UI that displays information about the champion system")]
		private static void ChampionInfo_OnCommand(CommandEventArgs e)
		{
			if (!Enabled)
			{
				e.Mobile.SendMessage("The champion system is not enabled.");
				return;
			}

			if (AllSpawns.Count == 0)
			{
				e.Mobile.SendMessage("The champion system is enabled but no altars exist");
				return;
			}

			e.Mobile.SendGump(new ChampionSystemGump());
		}

		public static void LoadSpawns()
		{
			if (m_Initialized)
			{
				return;
			}

			RemoveSpawns();

			Utility.WriteLine(ConsoleColor.White, "Generating Champion Spawns");

			ChampionSpawn spawn;

			var doc = new XmlDocument();

			doc.Load(m_ConfigPath);

			foreach (XmlNode node in doc.GetElementsByTagName("championSystem")[0].ChildNodes)
			{
				if (node.Name.Equals("spawn"))
				{
					spawn = new ChampionSpawn
					{
						SpawnName = GetAttr(node, "name", "Unamed Spawner")
					};

					var value = GetAttr(node, "type", null);

					if (value == null)
					{
						spawn.RandomizeType = true;
					}
					else if (Enum.TryParse(value, out ChampionSpawnType type))
					{
						spawn.Type = type;
					}

					spawn.SpawnMod = XmlConvert.ToDouble(GetAttr(node, "spawnMod", "1.0"));
					spawn.KillsMod = XmlConvert.ToDouble(GetAttr(node, "killsMod", "1.0"));

					foreach (XmlNode child in node.ChildNodes)
					{
						if (child.Name.Equals("location"))
						{
							var x = XmlConvert.ToInt32(GetAttr(child, "x", "0"));
							var y = XmlConvert.ToInt32(GetAttr(child, "y", "0"));
							var z = XmlConvert.ToInt32(GetAttr(child, "z", "0"));
							var r = XmlConvert.ToInt32(GetAttr(child, "radius", "0"));

							var mapName = GetAttr(child, "map", "Felucca");
							var map = Map.Parse(mapName);

							spawn.SpawnRadius = r;

							spawn.MoveToWorld(new Point3D(x, y, z), map);
						}
					}

					spawn.GroupName = GetAttr(node, "group", null);

					if (spawn.Type == ChampionSpawnType.Infuse)
					{
						PrimevalLichPuzzle.GenLichPuzzle(null);
					}
				}
			}

			Rotate();

			m_Initialized = true;
		}

		public static void RemoveSpawns()
		{
			ColUtility.IterateReverse(AllSpawns, s => s.Delete());
		}

		private static string GetAttr(XmlNode node, string name, string def)
		{
			var attr = node.Attributes[name];

			if (attr != null)
			{
				return attr.Value;
			}

			return def;
		}

		private static void Rotate()
		{
			m_LastRotate = DateTime.UtcNow;

			foreach (var g in AllSpawns.GroupBy(s => s.GroupName))
			{
				if (g.Key == null)
				{
					foreach (var s in g)
					{
						s.AutoRestart = true;
						s.Active = true;
					}
				}
				else
				{
					ChampionSpawn e = null;

					foreach (var s in g)
					{
						s.AutoRestart = false;

						if (e == null || Utility.RandomBool())
						{
							e = s;
						}
					}

					if (e != null)
					{
						e.AutoRestart = true;
						e.Active = true;
					}
				}
			}
		}

		private static void OnSlice()
		{
			if (Enabled)
			{
				LoadSpawns();
			}
			else
			{
				RemoveSpawns();
			}

			if (Enabled && DateTime.UtcNow > m_LastRotate + RotateDelay)
			{
				Rotate();
			}
		}

		private class ChampionSystemGump : Gump
		{
			private const int gBoarder = 20;
			private const int gRowHeight = 25;
			private const int gFontHue = 0;

			private static readonly int[] gWidths = { 20, 100, 100, 40, 40, 40, 80, 60, 50, 50, 50, 20 };
			private static readonly int[] gTab;
			private static readonly int gWidth;

			static ChampionSystemGump()
			{
				gTab = new int[gWidths.Length];

				for (var i = 0; i < gWidths.Length; i++)
				{
					gTab[i] = gWidth;
					gWidth += gWidths[i];
				}
			}

			public ChampionSystemGump()
				: base(40, 40)
			{
				AddBackground(0, 0, gWidth, gBoarder * 2 + AllSpawns.Count * gRowHeight + gRowHeight * 2, 0x13BE);

				var top = gBoarder;

				AddLabel(gBoarder, top, gFontHue, "Champion Spawn System Gump");

				top += gRowHeight;

				AddLabel(gTab[1], top, gFontHue, "Spawn Name");
				AddLabel(gTab[2], top, gFontHue, "Spawn Group");
				AddLabel(gTab[3], top, gFontHue, "X");
				AddLabel(gTab[4], top, gFontHue, "Y");
				AddLabel(gTab[5], top, gFontHue, "Z");
				AddLabel(gTab[6], top, gFontHue, "Map");
				AddLabel(gTab[7], top, gFontHue, "Active");
				AddLabel(gTab[8], top, gFontHue, "Auto");
				AddLabel(gTab[9], top, gFontHue, "Go");
				AddLabel(gTab[10], top, gFontHue, "Info");

				top += gRowHeight;

				for (var i = 0; i < AllSpawns.Count; i++)
				{
					var spawn = AllSpawns[i];

					AddLabel(gTab[1], top, gFontHue, spawn.SpawnName);
					AddLabel(gTab[2], top, gFontHue, spawn.GroupName ?? "None");
					AddLabel(gTab[3], top, gFontHue, spawn.X.ToString());
					AddLabel(gTab[4], top, gFontHue, spawn.Y.ToString());
					AddLabel(gTab[5], top, gFontHue, spawn.Z.ToString());
					AddLabel(gTab[6], top, gFontHue, spawn.Map?.ToString() ?? "null");
					AddLabel(gTab[7], top, gFontHue, spawn.Active ? "Y" : "N");
					AddLabel(gTab[8], top, gFontHue, spawn.AutoRestart ? "Y" : "N");
					AddButton(gTab[9], top, 0xFA5, 0xFA7, 1 + i, GumpButtonType.Reply, 0);
					AddButton(gTab[10], top, 0xFA5, 0xFA7, 1001 + i, GumpButtonType.Reply, 0);

					top += gRowHeight;
				}
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if (info.ButtonID > 0 && info.ButtonID <= 1000)
				{
					var idx = info.ButtonID - 1;

					if (idx < 0 || idx >= AllSpawns.Count)
					{
						sender.Mobile.SendGump(this);
						return;
					}

					var spawn = AllSpawns[idx];

					sender.Mobile.MoveToWorld(spawn.Location, spawn.Map);
					sender.Mobile.SendGump(this);
				}
				else if (info.ButtonID > 1000)
				{
					var idx = info.ButtonID - 1001;

					if (idx < 0 || idx > AllSpawns.Count)
					{
						sender.Mobile.SendGump(this);
						return;
					}

					var spawn = AllSpawns[idx];

					spawn.SendGump(sender.Mobile);
				}
			}
		}
	}
}
