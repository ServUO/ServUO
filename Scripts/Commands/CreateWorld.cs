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

		public struct CommandEntry
		{
			public string Name;
			public string CreateCommand;
			public string DeleteCommand;
			public int checkId;
			public CommandEntry(string n, string c, string d, int i)
			{
				Name = n;
				CreateCommand = c;
				DeleteCommand = d;
				checkId = i;
			}
		}

		public static List<CommandEntry> Commands = new List<CommandEntry>(new CommandEntry[] 
        {
			new CommandEntry("Moongates",       "Moongen",			"MoonGenDelete",		101),
			new CommandEntry("Doors",           "DoorGen",			"DoorGenDelete",		102),
			new CommandEntry("Signs",           "SignGen",			"SignGenDelete",		103),
			new CommandEntry("Teleporters",     "TelGen",			"TelGenDelete",			104),
			new CommandEntry("Doom Lamp",       "GenLeverPuzzle",   "LampPuzzleDelete",		105),
			new CommandEntry("Doom Gauntlet",   "GenGauntlet",      "DeleteGauntlet",		106),
            new CommandEntry("Khaldun",         "GenKhaldun",       "DeleteKhaldun",        107),
            new CommandEntry("Stealables",      "GenStealArties",   "RemoveStealArties",	108),
			new CommandEntry("Solen Hives",     "SHTelGen",         "SHTelGenDelete",		109),
			new CommandEntry("Malas Secrets",   "SecretLocGen",     "SecretLocDelete",		110),
			new CommandEntry("Factions",        "GenerateFactions",	"DeleteFactions",		111),
			//new CommandEntry("Primeival Lich",  "GenLichPuzzle",	"DeleteLichPuzzle",		112), Moved to DecorateSA, command remains
			new CommandEntry("Decorations",     "Decorate",         "DecorateDelete",		113),
			new CommandEntry("ML Decorations",  "DecorateML",		"DecorateMLDelete",		114),
			new CommandEntry("SA Decorations",  "DecorateSA",		"DecorateSADelete",		115),
			new CommandEntry("Spawners",		"XmlLoad Spawns",	"XmlSpawnerWipeAll",	116),
            new CommandEntry("New Despise",         "SetupDespise",     "DeleteDespise",        117),
            new CommandEntry("New Covetous",        "SetupNewCovetous", "DeleteCovetous",       118),
            new CommandEntry("New Shame",           "GenerateNewShame", "DeleteShame",          119),
            new CommandEntry("New Magincia",    "GenNewMagincia",   "DeleteNewMagincia",    120),
            new CommandEntry("High Seas",       "DecorateHS",       "DeleteHS",             121),
            new CommandEntry("City Loyalty",    "SetupCityLoyaltySystem",   "DeleteCityLoyaltySystem",             122),
            new CommandEntry("Castle Blackthorn",    "GenBlackthorn",       null,                                  123),
            new CommandEntry("Time of Legends",      "DecorateTOL",         null,                                  124),
            new CommandEntry("New Wrong",      "GenWrongRewamp",            null,                                  125),
            new CommandEntry("Kotl City",      "GenerateTreasuresOfKotlCity", null,  126),
		});

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
				ids.Add(entry.checkId);
			}
			DoCommands(ids.ToArray(), type, from);
		}

		public static void DoCommands(int[] selections, GumpType type, Mobile from)
		{
			World.Broadcast(0x35, false, "The world is generating. This may take some time...");
			string prefix = Server.Commands.CommandSystem.Prefix;
			foreach (int sel in selections)
			{
				foreach (CreateWorld.CommandEntry entry in CreateWorld.Commands)
				{
					if (entry.checkId == sel)
					{
						switch (type)
						{
							case CreateWorld.GumpType.Create:
								from.Say("Generating " + entry.Name);
								CommandSystem.Handle(from, prefix + entry.CreateCommand);

                                if (CreateWorldData.CreateTable.ContainsKey(entry.checkId))
                                    CreateWorldData.CreateTable[sel] = true;

								break;
							case CreateWorld.GumpType.Delete:
								if (!String.IsNullOrEmpty(entry.DeleteCommand))
								{
									from.Say("Deleting " + entry.Name);
									CommandSystem.Handle(from, prefix + entry.DeleteCommand);

                                    if (CreateWorldData.CreateTable.ContainsKey(entry.checkId))
                                        CreateWorldData.CreateTable[sel] = false;
								}
								break;
							case CreateWorld.GumpType.Recreate:
								if (!String.IsNullOrEmpty(entry.DeleteCommand))
								{
									from.Say("Recreating " + entry.Name);
									CommandSystem.Handle(from, prefix + entry.DeleteCommand);
									CommandSystem.Handle(from, prefix + entry.CreateCommand);

                                    if (CreateWorldData.CreateTable.ContainsKey(entry.checkId))
                                        CreateWorldData.CreateTable[sel] = true;
								}
								break;
						}
					}
				}
			}
			World.Broadcast(0x35, false, "World generation complete.");
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
            this.m_CommandEventArgs = e;
            this.Closable = true;
            this.Dragable = true;

            this.AddPage(1);

			int items = CreateWorld.Commands.Count;

            if (!Server.Factions.Settings.Enabled)
                items--;

			this.AddBackground(0, 0, 240, 75 + items * 25, 5054);
			switch (m_Type)
			{
				case CreateWorld.GumpType.Create:
					this.AddLabel(40, 2, 200, "CREATE WORLD GUMP");
					break;
				case CreateWorld.GumpType.Delete:
					this.AddLabel(40, 2, 200, "DELETE WORLD GUMP");
					break;
				case CreateWorld.GumpType.Recreate:
					this.AddLabel(40, 2, 200, "RECREATE WORLD GUMP");
					break;
			}

			this.AddImageTiled(10, 20, 220, 10 + items * 25, 3004);
			int y = 25;

			foreach(CreateWorld.CommandEntry entry in CreateWorld.Commands)
			{
                if (entry.Name == "Factions" && !Server.Factions.Settings.Enabled)
                    continue;

                bool created = CreateWorldData.CreateTable.ContainsKey(entry.checkId) && CreateWorldData.CreateTable[entry.checkId];

				this.AddLabel(20, y + 1, created ? 200 : 338, String.Format("{0} {1}", entry.Name, created ? "[created]" : "[not created]"));
				this.AddCheck(210, y - 2, 210, 211, m_Type == CreateWorld.GumpType.Create ? !created : created, entry.checkId);

				y += 25;
			}

            y = 25 + (items * 25);

			this.AddButton(60, y + 15, 247, 249, 1, GumpButtonType.Reply, 0);
			this.AddButton(130, y + 15, 241, 243, 0, GumpButtonType.Reply, 0);
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
                    return World.Items.Values.Where(i => i != null && i is XmlSpawner).Count() > 1000;
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
            CreateTable = new Dictionary<int, bool>();

            foreach (CreateWorld.CommandEntry entry in CreateWorld.Commands)
            {
                CreateTable[entry.checkId] = HasGenerated(entry.checkId);
            }
        }
    }
}
