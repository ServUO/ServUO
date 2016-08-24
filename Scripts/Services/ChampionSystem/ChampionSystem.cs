using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Server.Gumps;
using Server.Commands;
using System.Xml;

namespace Server.Engines.CannedEvil
{
	public class ChampionSystem
	{
		private static bool m_Enabled = false;
		private static bool m_Initialized = false;
		private static readonly string m_Path = Path.Combine("Saves", "Champions", "ChampionSystem.bin");
		private static readonly string m_ConfigPath = Path.Combine("Config", "ChampionSpawns.xml");
		private static DateTime m_LastRotate;
		private static TimeSpan m_RotateDelay;
		private static List<ChampionSpawn> m_AllSpawns = new List<ChampionSpawn>();
		private static InternalTimer m_Timer;
		private static int m_GoldShowerPiles;
		private static int m_GoldShowerMinAmount;
		private static int m_GoldShowerMaxAmount;
		private static int m_HarrowerGoldPiles;
		private static int m_HarrowerGoldMinAmount;
		private static int m_HarrowerGoldMaxAmount;
		private static int m_PowerScrollAmount;
		private static int m_StatScrollAmount;
		private static int[] m_Rank = new int[16];
		private static int[] m_MaxKill = new int[4];
		private static double m_TranscendenceChance;
		private static double m_ScrollChance;
		private static bool m_ForceGenerate = false;

		public static int GoldShowerPiles { get { return m_GoldShowerPiles; } }
		public static int GoldShowerMinAmount { get { return m_GoldShowerMinAmount; } }
		public static int GoldShowerMaxAmount { get { return m_GoldShowerMaxAmount; } }
		public static int HarrowerGoldShowerPiles { get { return m_HarrowerGoldPiles; } }
		public static int HarrowerGoldShowerMinAmount { get { return m_HarrowerGoldMinAmount; } }
		public static int HarrowerGoldShowerMaxAmount { get { return m_HarrowerGoldMaxAmount; } }
		public static int PowerScrollAmount { get { return m_PowerScrollAmount; } }
		public static int StatScrollAmount { get { return m_StatScrollAmount; } }
		public static int RankForLevel(int l)
		{
			if (l < 0)
				return 0;
			if (l >= m_Rank.Length)
				return 3;
			return m_Rank[l];
		}
		public static int MaxKillsForLevel(int l)
		{
			return m_MaxKill[RankForLevel(l)];
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
		public static double TranscendenceChance { get { return m_TranscendenceChance; } }
		public static double ScrollChance { get { return m_ScrollChance; } }

		public static void Configure()
		{
			m_Enabled = Config.Get("Champions.Enabled", true);
			m_RotateDelay = Config.Get("Champions.RotateDelay", TimeSpan.FromDays(1.0d));
			m_GoldShowerPiles = Config.Get("Champions.GoldPiles", 50);
			m_GoldShowerMinAmount = Config.Get("Champions.GoldMin", 2500);
			m_GoldShowerMaxAmount = Config.Get("Champions.GoldMax", 7500);
			m_HarrowerGoldPiles = Config.Get("Champions.HarrowerGoldPiles", 75);
			m_HarrowerGoldMinAmount = Config.Get("Champions.HarrowerGoldMin", 5000);
			m_HarrowerGoldMaxAmount = Config.Get("Champions.HarrowerGoldMax", 10000);
			m_PowerScrollAmount = Config.Get("Champions.PowerScrolls", 6);
			m_StatScrollAmount = Config.Get("Champions.StatScrolls", 16);
			m_ScrollChance = Config.Get("Champions.ScrollChance", 0.1d) / 100.0d;
			m_TranscendenceChance = Config.Get("Champions.TranscendenceChance", 50.0d) / 100.0d;
			int rank2 = Config.Get("Champions.Rank2RedSkulls", 5);
			int rank3 = Config.Get("Champions.Rank3RedSkulls", 10);
			int rank4 = Config.Get("Champions.Rank4RedSkulls", 10);
			for (int i = 0; i < m_Rank.Length; ++i)
			{
				if (i < rank2)
					m_Rank[i] = 0;
				else if (i < rank3)
					m_Rank[i] = 1;
				else if (i < rank4)
					m_Rank[i] = 2;
				else
					m_Rank[i] = 3;
			}
			m_MaxKill[0] = Config.Get("Champions.Rank1MaxKills", 256);
			m_MaxKill[1] = Config.Get("Champions.Rank2MaxKills", 128);
			m_MaxKill[2] = Config.Get("Champions.Rank3MaxKills", 64);
			m_MaxKill[3] = Config.Get("Champions.Rank4MaxKills", 32);
			EventSink.WorldLoad += EventSink_WorldLoad;
			EventSink.WorldSave += EventSink_WorldSave;
		}
		private static void EventSink_WorldSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(
				m_Path,
				writer =>
				{
					writer.Write(1); // Version
					writer.Write(m_Initialized);
					writer.Write(m_LastRotate);
					writer.WriteItemList(m_AllSpawns, true);
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

					if(version == 0)
					{
						m_ForceGenerate = true;
					}
				});
		}

		public static void Initialize()
		{
			CommandSystem.Register("ChampionInfo", AccessLevel.GameMaster, new CommandEventHandler(ChampionInfo_OnCommand));

			if (!m_Enabled || m_ForceGenerate)
			{
				foreach (ChampionSpawn s in m_AllSpawns.Where(sp => sp != null && !sp.Deleted))
				{
					s.Delete();
				}
				m_Initialized = false;
			}

			if (!m_Enabled)
				return;

			m_Timer = new InternalTimer();

			if (m_Initialized)
				return;

			m_AllSpawns.Clear();

			Utility.PushColor(ConsoleColor.White);
			Console.WriteLine("Generating Champion Spawns");
			Utility.PopColor();

			ChampionSpawn spawn;

			XmlDocument doc = new XmlDocument();
			doc.Load(m_ConfigPath);
			foreach (XmlNode node in doc.GetElementsByTagName("championSystem")[0].ChildNodes)
			{
				if (node.Name.Equals("spawn"))
				{
					spawn = new ChampionSpawn();
					spawn.SpawnName = GetAttr(node, "name", "Unamed Spawner");
					string value = GetAttr(node, "type", null);
					if(value == null)
						spawn.RandomizeType = true;
					else
						spawn.Type = (ChampionSpawnType)Enum.Parse(typeof(ChampionSpawnType), value);
					value = GetAttr(node, "spawnMod", "1.0");
					spawn.SpawnMod = XmlConvert.ToDouble(value);
					value = GetAttr(node, "killsMod", "1.0");
					spawn.KillsMod = XmlConvert.ToDouble(value);
					foreach(XmlNode child in node.ChildNodes)
					{
						if (child.Name.Equals("location"))
						{
							int x = XmlConvert.ToInt32(GetAttr(child, "x", "0"));
							int y = XmlConvert.ToInt32(GetAttr(child, "y", "0"));
							int z = XmlConvert.ToInt32(GetAttr(child, "z", "0"));
							int r = XmlConvert.ToInt32(GetAttr(child, "radius", "0"));
							string mapName = GetAttr(child, "map", "Felucca");
							Map map = Map.Parse(mapName);

							spawn.SpawnRadius = r;
							spawn.MoveToWorld(new Point3D(x, y, z), map);
						}
					}
					spawn.GroupName = GetAttr(node, "group", null);
					m_AllSpawns.Add(spawn);
				}
			}

			Rotate();

			m_Initialized = true;
		}

		private static string GetAttr(XmlNode node, string name, string def)
		{
			XmlAttribute attr = node.Attributes[name];
			if (attr != null)
				return attr.Value;
			return def;
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
			Dictionary<String, List<ChampionSpawn>> groups = new Dictionary<string, List<ChampionSpawn>>();
			m_LastRotate = DateTime.UtcNow;

            foreach (ChampionSpawn spawn in m_AllSpawns.Where(spawn => spawn != null && !spawn.Deleted))
			{
				List<ChampionSpawn> group;
				if (spawn.GroupName == null)
				{
					spawn.AutoRestart = true;
					if (!spawn.Active)
						spawn.Active = true;
					continue;
				}
				if (!groups.TryGetValue(spawn.GroupName, out group))
				{
					group = new List<ChampionSpawn>();
					groups.Add(spawn.GroupName, group);
				}
				group.Add(spawn);
			}

			foreach (string key in groups.Keys)
			{
				List<ChampionSpawn> group = groups[key];
				foreach (ChampionSpawn spawn in group)
				{
					spawn.AutoRestart = false;
				}
				ChampionSpawn s = group[Utility.Random(group.Count)];
				s.AutoRestart = true;
				if (!s.Active)
					s.Active = true;
			}
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
			private static readonly int[] gWidths = { 20, 100, 100, 40, 40, 40, 80, 60, 50, 50, 50, 20 };
			private static readonly int[] gTab;
			private static readonly int gWidth;

            public List<ChampionSpawn> Spawners { get; set; }

			static ChampionSystemGump()
			{
				gWidth = gWidths.Sum();
				int tab = 0;
				gTab = new int[gWidths.Length];
				for(int i = 0; i < gWidths.Length; ++i)
				{
					gTab[i] = tab;
					tab += gWidths[i];
				}
			}

			public ChampionSystemGump()
				: base(40, 40)
			{
                Spawners = m_AllSpawns.Where(spawn => spawn != null && !spawn.Deleted).ToList();

				AddBackground(0, 0, gWidth, gBoarder * 2 + Spawners.Count * gRowHeight + gRowHeight * 2, 0x13BE);

				int top = gBoarder;
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

                for(int i = 0; i < Spawners.Count; i++)
				{
                    ChampionSpawn spawn = Spawners[i];
					AddLabel(gTab[1], top, gFontHue, spawn.SpawnName);
					AddLabel(gTab[2], top, gFontHue, spawn.GroupName != null ? spawn.GroupName : "None");
					AddLabel(gTab[3], top, gFontHue, spawn.X.ToString());
					AddLabel(gTab[4], top, gFontHue, spawn.Y.ToString());
					AddLabel(gTab[5], top, gFontHue, spawn.Z.ToString());
					AddLabel(gTab[6], top, gFontHue, spawn.Map == null ? "null" : spawn.Map.ToString());
					AddLabel(gTab[7], top, gFontHue, spawn.Active ? "Y" : "N");
					AddLabel(gTab[8], top, gFontHue, spawn.AutoRestart ? "Y" : "N");
					AddButton(gTab[9], top, 0xFA5, 0xFA7, 1 + i, GumpButtonType.Reply, 0);
					AddButton(gTab[10], top, 0xFA5, 0xFA7, 1001 + i, GumpButtonType.Reply, 0);
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
					if (idx < 0 || idx >= Spawners.Count)
						return;
                    spawn = Spawners[idx];
					sender.Mobile.MoveToWorld(spawn.Location, spawn.Map);
					sender.Mobile.SendGump(this);
				}
				else if (info.ButtonID > 1000)
				{
					idx = info.ButtonID - 1001;
					if (idx < 0 || idx > Spawners.Count)
						return;
                    spawn = Spawners[idx];
					spawn.SendGump(sender.Mobile);
				}
			}
		}
	}
}