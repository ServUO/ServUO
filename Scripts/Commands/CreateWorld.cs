using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Gumps;
using Server.Network;
using System.Linq;
using System.IO;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands 
{
	public class CreateWorld
	{
		public enum GumpType
		{
			Create,
			Delete,
			Recreate,
		}

        public enum Category
        {
            All,
            Decoration,
            Spawn,
            System,
            Dungeon,
            RevampedDungeon,
            Expansion
        }

		public class CommandEntry
		{
            public string Name { get; set; }
            public string CreateCommand { get; set; }
            public string DeleteCommand { get; set; }
            public int CheckID { get; set; }
            public int Delay { get; set; }

            public Category Category { get; set; }
            public Expansion RequiredExpansion { get; set; }

			public CommandEntry(string n, string c, string d, Category cat, Expansion expansion, int i, int delay = 0)
			{
				Name = n;
				CreateCommand = c;
				DeleteCommand = d;
                Category = cat;
                RequiredExpansion = expansion;
				CheckID = i;
                Delay = delay;
			}
		}

		public static List<CommandEntry> Commands = new List<CommandEntry>(new CommandEntry[] 
        {
			new CommandEntry("Moongates",           "Moongen",			"MoonGenDelete",		Category.Decoration,      Expansion.None,   101),
			new CommandEntry("Doors",               "DoorGen",			"DoorGenDelete",		Category.Decoration,      Expansion.None,   102),
			new CommandEntry("Signs",               "SignGen",			"SignGenDelete",		Category.Decoration,      Expansion.None,   103),
			new CommandEntry("Teleporters",         "TelGen",			"TelGenDelete",			Category.Decoration,      Expansion.None,   104),
			new CommandEntry("Doom Lamp",           "GenLeverPuzzle",   "LampPuzzleDelete",		Category.System,          Expansion.AOS,    105),
			new CommandEntry("Doom Gauntlet",       "GenGauntlet",      "DeleteGauntlet",		Category.Dungeon,         Expansion.AOS,    106),
            new CommandEntry("Khaldun",             "GenKhaldun",       "DeleteKhaldun",        Category.Dungeon,         Expansion.None,   107),
            new CommandEntry("Stealables",          "GenStealArties",   "RemoveStealArties",	Category.Spawn,           Expansion.AOS,    108),
			new CommandEntry("Solen Hives",         "SHTelGen",         "SHTelGenDelete",		Category.Dungeon,         Expansion.LBR,    109),
			new CommandEntry("Malas Secrets",       "SecretLocGen",     "SecretLocDelete",		Category.System,          Expansion.AOS,    110),
			new CommandEntry("Factions",            "GenerateFactions",	"DeleteFactions",		Category.System,          Expansion.None,   111),
			new CommandEntry("Decorations",         "Decorate",         "DecorateDelete",		Category.Decoration,      Expansion.None,   113),
			new CommandEntry("ML Decorations",      "DecorateML",		"DecorateMLDelete",		Category.Decoration,      Expansion.ML,     114),
			new CommandEntry("SA Decorations",      "DecorateSA",		"DecorateSADelete",		Category.Decoration,      Expansion.SA,     115),
			new CommandEntry("Spawners",		    "XmlLoad Spawns",	"WipeAllXmlSpawners",	Category.Spawn,           Expansion.None,   116),
            new CommandEntry("New Despise",         "SetupDespise",     "DeleteDespise",        Category.RevampedDungeon, Expansion.SA,     117),
            new CommandEntry("New Covetous",        "SetupNewCovetous", "DeleteCovetous",       Category.RevampedDungeon, Expansion.SA,     118),
            new CommandEntry("New Shame",           "GenerateNewShame", "DeleteShame",          Category.RevampedDungeon, Expansion.SA,     119),
            new CommandEntry("New Magincia",        "GenNewMagincia",   "DeleteNewMagincia",    Category.Decoration,      Expansion.None,   120),
            new CommandEntry("High Seas",           "DecorateHS",       "DeleteHS",             Category.Expansion,       Expansion.HS,     121),
            new CommandEntry("City Loyalty",        "SetupCityLoyaltySystem","DeleteCityLoyaltySystem",Category.System,     Expansion.SA,   122),
            new CommandEntry("Castle Blackthorn",   "GenBlackthorn",                null,       Category.RevampedDungeon, Expansion.SA,     123),
            new CommandEntry("TOL Decorations",     "DecorateTOL",                  null,       Category.Decoration,      Expansion.TOL,    124),
            new CommandEntry("New Wrong",           "GenWrongRevamp",               null,       Category.RevampedDungeon, Expansion.SA,     125),
            new CommandEntry("Kotl City",           "GenerateTreasuresOfKotlCity",  null,       Category.System,          Expansion.TOL,    126),
            new CommandEntry("Fillable Containers", "CheckFillables",               null,       Category.Spawn,           Expansion.None,   127, 5),
            new CommandEntry("Champ Spawns",        "GenChampSpawns",   "DelChampSpawns",       Category.Spawn,           Expansion.None,   128),
		});

        public static bool WorldCreating { get; set; }

        public CreateWorld()
        {
        }

        public static void Initialize() 
        { 
            CommandSystem.Register("Createworld", AccessLevel.Administrator, new CommandEventHandler(Create_OnCommand));
			CommandSystem.Register("DeleteWorld", AccessLevel.Administrator, new CommandEventHandler(Delete_OnCommand));
			CommandSystem.Register("RecreateWorld", AccessLevel.Administrator, new CommandEventHandler(Recreate_OnCommand));
		}

		[Usage("CreateWorld [nogump]")]
		[Description("Generates the world with a menu. If nogump argument is given, no gump will be displayed, all options will be assumed true, and the action will proceed immediately.")]
		private static void Create_OnCommand(CommandEventArgs e)
		{
			if (String.IsNullOrEmpty(e.ArgString))
			{
                if (e.Mobile is PlayerMobile)
                    BaseGump.SendGump(new NewCreateWorldGump((PlayerMobile)e.Mobile, GumpType.Create));
                else
				    e.Mobile.SendGump(new CreateWorldGump(e, GumpType.Create));
			}
			else if (e.ArgString.ToLower().Equals("nogump"))
			{
				DoAllCommands(GumpType.Create, e.Mobile);
			}
			else
			{
				if (e.Mobile != null)
					e.Mobile.SendMessage("Usage: CreateWorld [nogump]");
			}
		}

		[Usage("DeleteWorld [nogump]")]
		[Description("Undoes world generation with a menu. If nogump argument is given, no gump will be displayed, all options will be assumed true, and the action will proceed immediately.")]
		private static void Delete_OnCommand(CommandEventArgs e)
		{
			if (String.IsNullOrEmpty(e.ArgString))
			{
                if (e.Mobile is PlayerMobile)
                    BaseGump.SendGump(new NewCreateWorldGump((PlayerMobile)e.Mobile, GumpType.Delete));
                else
				    e.Mobile.SendGump(new CreateWorldGump(e, GumpType.Delete));
			}
			else if (e.ArgString.ToLower().Equals("nogump"))
			{
				DoAllCommands(GumpType.Delete, e.Mobile);
			}
			else
			{
				if (e.Mobile != null)
					e.Mobile.SendMessage("Usage: DeleteWorld [nogump]");
			}
		}

		[Usage("RecreateWorld [nogump]")]
		[Description("Re-generates the world with a menu. If nogump argument is given, no gump will be displayed, all options will be assumed true, and the action will proceed immediately.")]
		private static void Recreate_OnCommand(CommandEventArgs e)
		{
			if (String.IsNullOrEmpty(e.ArgString))
			{
				e.Mobile.SendGump(new CreateWorldGump(e, GumpType.Recreate));
			}
			else if (e.ArgString.ToLower().Equals("nogump"))
			{
				DoAllCommands(GumpType.Recreate, e.Mobile);
			}
			else
			{
				if (e.Mobile != null)
					e.Mobile.SendMessage("Usage: RecreateWorld [nogump]");
			}

		}

		public static void DoAllCommands(GumpType type, Mobile from)
		{
			List<int> ids = new List<int>();
			foreach (CommandEntry entry in Commands)
			{
				ids.Add(entry.CheckID);
			}
			DoCommands(ids.ToArray(), type, from);
		}

		public static void DoCommands(int[] selections, GumpType type, Mobile from)
		{
			World.Broadcast(0x35, false, "The world is generating. This may take some time...");
			string prefix = Server.Commands.CommandSystem.Prefix;

            string error = null;
            WorldCreating = true;

			foreach (int sel in selections)
			{
				foreach (CreateWorld.CommandEntry entry in CreateWorld.Commands)
				{
					if (entry.CheckID == sel)
					{
						switch (type)
						{
							case CreateWorld.GumpType.Create:
								from.Say("Generating " + entry.Name);

                                if (CanGenerate(entry, ref error))
                                {
                                    if (entry.Delay > 0)
                                    {
                                        DoDelayedCommand(from, TimeSpan.FromMinutes(entry.Delay), prefix + entry.CreateCommand);
                                    }
                                    else
                                    {
                                        CommandSystem.Handle(from, prefix + entry.CreateCommand);
                                    }

                                    if (CreateWorldData.CreateTable.ContainsKey(sel))
                                        CreateWorldData.CreateTable[sel] = true;
                                }

								break;
							case CreateWorld.GumpType.Delete:
								if (!String.IsNullOrEmpty(entry.DeleteCommand))
								{
									from.Say("Deleting " + entry.Name);
									CommandSystem.Handle(from, prefix + entry.DeleteCommand);

                                    if (CreateWorldData.CreateTable.ContainsKey(sel))
                                        CreateWorldData.CreateTable[sel] = false;
								}
								break;
						}
					}
				}
			}

            if (error != null)
            {
                from.SendGump(new BasicInfoGump(error, "World Generation Error"));
            }

            WorldCreating = false;
			World.Broadcast(0x35, false, "World generation complete.");
		}

        public static bool CanGenerate(CommandEntry entry, ref string error)
        {
            if (CreateWorldData.CreateTable.ContainsKey(entry.CheckID) && CreateWorldData.CreateTable[entry.CheckID])
            {
                string er = String.Format("<br>- {0} have been generated already.", entry.Name);
                Console.WriteLine(er);

                error += er;

                return false;
            }

            if (entry.CheckID == 127)
            {
                if (CreateWorldData.HasGenerated(116) && CreateWorldData.HasGenerated(113))
                {
                    return true;
                }

                string er = String.Format("<br>- Cannot generate {0}. You need to generate Decorations and Spawners first.", entry.Name);
                Console.WriteLine(er);

                error += er;

                return false;
            }

            if (entry.Category == Category.RevampedDungeon)
            {
                if (CreateWorldData.HasGenerated(116))
                {
                    return true;
                }
                else
                {
                    string er = String.Format("<br>- Cannot generate {0}. You need to generate Spawners first.", entry.Name);
                    Console.WriteLine(er);

                    error += er;

                    return false;
                }
            }

            return true;
        }

        public static void DoDelayedCommand(Mobile from, TimeSpan delay, string command)
        {
            Console.WriteLine("Setting delayed create command: {0} [{1}] minutes", command, delay.TotalMinutes);

            Timer.DelayCall(delay, () =>
                {
                    CommandSystem.Handle(from, command);
                });
        }

	}
}

namespace Server.Gumps
{
    public class CreateWorldGump : Gump
    {
        private readonly CommandEventArgs m_CommandEventArgs;
		private CreateWorld.GumpType m_Type;

        public CreateWorldGump(CommandEventArgs e, CreateWorld.GumpType type)
            : base(50,50)
        {
			m_Type = type;
            m_CommandEventArgs = e;
            Closable = true;
            Dragable = true;

            AddPage(1);

			int items = CreateWorld.Commands.Count;

            if (!Server.Factions.Settings.Enabled)
                items--;

			AddBackground(0, 0, 240, 75 + items * 25, 5054);
			switch (m_Type)
			{
				case CreateWorld.GumpType.Create:
					AddLabel(40, 2, 200, "CREATE WORLD GUMP");
					break;
				case CreateWorld.GumpType.Delete:
					AddLabel(40, 2, 200, "DELETE WORLD GUMP");
					break;
				case CreateWorld.GumpType.Recreate:
					AddLabel(40, 2, 200, "RECREATE WORLD GUMP");
					break;
			}

			AddImageTiled(10, 20, 220, 10 + items * 25, 3004);
			int y = 25;

			foreach(CreateWorld.CommandEntry entry in CreateWorld.Commands)
			{
                if (entry.Name == "Factions" && !Server.Factions.Settings.Enabled)
                    continue;

                bool created = CreateWorldData.CreateTable.ContainsKey(entry.CheckID) && CreateWorldData.CreateTable[entry.CheckID];

				AddLabel(20, y + 1, created ? 200 : 338, String.Format("{0} {1}", entry.Name, created ? "[created]" : "[not created]"));
				AddCheck(210, y - 2, 210, 211, m_Type == CreateWorld.GumpType.Create ? !created : created, entry.CheckID);

				y += 25;
			}

            y = 25 + (items * 25);

			AddButton(60, y + 15, 247, 249, 1, GumpButtonType.Reply, 0);
			AddButton(130, y + 15, 241, 243, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info) 
        { 
            Mobile from = state.Mobile;

			switch (info.ButtonID)
			{
				case 0: // Closed or Cancel
					return;
				case 1:
					CreateWorld.DoCommands(info.Switches, m_Type, from);
					break;
			}
        }
    }

    public class NewCreateWorldGump : BaseGump
    {
        public CreateWorld.GumpType GumpType { get; set; }
        public CreateWorld.Category CategoryFilter { get; set; }

        public bool CheckAll { get; set; }
        public bool UncheckAll { get; set; }

        public NewCreateWorldGump(PlayerMobile pm, CreateWorld.GumpType type)
            : base(pm, 50, 50)
        {
            GumpType = type;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 600, 500, 5054);

            AddImageTiled(10, 10, 140, 28, 3004);
            AddImageTiled(152, 10, 438, 28, 3004);

            AddImageTiled(10, 40, 140, 430, 3004);
            AddImageTiled(152, 40, 438, 430, 3004);

            string label = GumpType == CreateWorld.GumpType.Create ? "CREATE WORLD GUMP" : "DELETE WORLD GUMP";
            switch (GumpType)
            {
                default:
                case CreateWorld.GumpType.Create:
                    label = "CREATE WORLD GUMP";
                    break;
                case CreateWorld.GumpType.Delete:
                    label = "DELETE WORLD GUMP";
                    break;
            }

            AddHtml(152, 15, 450, 20, ColorAndCenter("#00FFFF", label), false, false);
            AddHtml(12, 15, 140, 20, ColorAndCenter("#696969", String.Format("Your Expansion: {0}", Core.Expansion.ToString())), false, false);

            for (int i = 0; i < 6; i++)
            {
                CreateWorld.Category cat = (CreateWorld.Category)i;
                int id = CategoryFilter == cat || CategoryFilter == CreateWorld.Category.All ? 4030 : 4029;

                AddButton(12, 55 + (i * 25), id, id == 4030 ? 4029 : 4030, i + 500, GumpButtonType.Reply, 0);
                AddHtml(45, 55 + (i * 25), 100, 20, Color("#696969", cat.ToString()), false, false);
            }

            List<CreateWorld.CommandEntry> commands = new List<CreateWorld.CommandEntry>(CreateWorld.Commands.Where(c =>
                CategoryFilter == CreateWorld.Category.All || CategoryFilter == c.Category));

            int perpage = CreateWorld.Commands.Count / 2;
            int x = 154;
            int y = 55;

            for (int i = 0; i < commands.Count; i++)
            {
                var entry = commands[i];
                bool created = CreateWorldData.CreateTable[entry.CheckID];
                bool meetsExpansion = entry.RequiredExpansion <= Core.Expansion;

                bool check;

                if (CheckAll)
                    check = true;
                else if (UncheckAll)
                    check = false;
                else
                    check = GumpType == CreateWorld.GumpType.Create ? !created : created;

                if (meetsExpansion)
                {
                    AddLabel(x + 21, y, created ? 200 : 338, String.Format("{0} {1}", entry.Name, created ? "[created]" : "[not created]"));
                    AddCheck(x, y - 2, 210, 211, check, entry.CheckID);
                }
                else
                {
                    AddLabel(x + 21, y, created ? 200 : 338, String.Format("{0} {1}", entry.Name, "[Wrong Expansion]"));
                }

                y += 20;

                if (i == perpage)
                {
                    x = 380;
                    y = 55;
                }
            }

            AddButton(154, 426, 4005, 4007, 2500, GumpButtonType.Reply, 0);
            AddHtml(187, 426, 150, 20, Color("#696969", "Check All"), false, false);

            AddButton(154, 448, 4017, 4019, 2501, GumpButtonType.Reply, 0);
            AddHtml(187, 448, 150, 20, Color("#696969", "Uncheck All"), false, false);

            AddButton(240, 473, 247, 249, 1, GumpButtonType.Reply, 0);
            AddButton(303, 473, 241, 243, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: // Closed or Cancel
                    return;
                case 1:
                    CreateWorld.DoCommands(info.Switches, GumpType, User);
                    break;
                case 2500:
                    CheckAll = true;
                    UncheckAll = false;
                    Refresh();
                    break;
                case 2501:
                    CheckAll = false;
                    UncheckAll = true;
                    Refresh();
                    break;
                default:
                    int id = info.ButtonID - 500;

                    if (id >= 0 && id <= 6)
                    {
                        CategoryFilter = (CreateWorld.Category)id;
                    }

                    Refresh();
                    break;
            }
        }
    }

    public static class CreateWorldData
    {
        public static Dictionary<int, bool> CreateTable { get; set; }

        public static bool HasGenerated(int index)
        {
            switch (index)
            {
                case 101:
                    return PublicMoongate.Moongates.Count > 0;
                case 102:
                    return WeakEntityCollection.HasCollection("door");
                case 103:
                    return WeakEntityCollection.HasCollection("sign");
                case 104:
                    return WeakEntityCollection.HasCollection("tel");
                case 105:
                    return WeakEntityCollection.HasCollection("LeverPuzzleController");
                case 106:
                    return WeakEntityCollection.HasCollection("doom");
                case 107:
                    return WeakEntityCollection.HasCollection("khaldun");
                case 108:
                    return StealableArtifactsSpawner.Instance != null;
                case 109:
                    return SHTeleporter.SHTeleporterCreator.FindSHTeleporter(Map.Trammel, new Point3D(5747, 1895, 0)) != null;
                case 110:
                    return WeakEntityCollection.HasCollection("malas");
                case 111:
                    return WeakEntityCollection.HasCollection("factions");
                case 113:
                    return WeakEntityCollection.HasCollection("deco");
                case 114:
                    return WeakEntityCollection.HasCollection("ml");
                case 115:
                    return WeakEntityCollection.HasCollection("sa");
                case 116:
                    return World.Items.Values.Where(i => i != null && (i is XmlSpawner || i is Spawner)).Count() > 1000;
                case 117:
                    return WeakEntityCollection.HasCollection("despise");
                case 118:
                    return WeakEntityCollection.HasCollection("newcovetous");
                case 119:
                    return WeakEntityCollection.HasCollection("newshame");
                case 120:
                    return Server.Engines.NewMagincia.MaginciaBazaar.Instance != null;
                case 121:
                    return WeakEntityCollection.HasCollection("highseas") || CharydbisSpawner.SpawnInstance != null;
                case 122:
                    return Server.Engines.CityLoyalty.CityLoyaltySystem.Cities != null && Server.Engines.CityLoyalty.CityLoyaltySystem.Cities.Count > 0 && Server.Engines.CityLoyalty.CityLoyaltySystem.Cities[0].Stone != null;
                case 123:
                    return HasItem(typeof(DungeonHitchingPost), new Point3D(6428, 2677, 0), Map.Trammel) &&
                           HasItem(typeof(DungeonHitchingPost), new Point3D(6428, 2677, 0), Map.Felucca);
                case 124:
                    return WeakEntityCollection.HasCollection("tol");
                case 125:
                    return BedrollSpawner.Instances != null && BedrollSpawner.Instances.Count > 0;
                case 126:
                    return Server.Engines.TreasuresOfKotlCity.KotlBattleSimulator.Instance != null;
                case 128:
                    return Server.Engines.CannedEvil.ChampionSystem.AllSpawns.Count > 0;
            }

            return false;
        }

        public static bool HasItem(Type type, Point3D p, Map map)
        {
            IPooledEnumerable eable = map.GetItemsInRange(p, 0);

            foreach (Item item in eable)
            {
                if (item.GetType() == type)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        public static void Initialize()
        {
            if (!_HasRan)
            {
                CreateTable = new Dictionary<int, bool>();

                foreach (CreateWorld.CommandEntry entry in CreateWorld.Commands)
                {
                    CreateTable[entry.CheckID] = HasGenerated(entry.CheckID);
                }
            }
        }

        public static readonly string FilePath = Path.Combine("Saves/Misc", "Persistence.bin");
        private static bool _HasRan;

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(1);
                    writer.Write(true);

                    writer.Write(CreateTable.Count);
                    foreach (var kvp in CreateTable)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value);
                    }
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();
                    _HasRan = reader.ReadBool();

                    CreateTable = new Dictionary<int, bool>();

                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        CreateTable[reader.ReadInt()] = reader.ReadBool();
                    }

                    if (version == 0)
                    {
                        CreateTable[128] = HasGenerated(128);
                    }
                });
        }
    }
}
