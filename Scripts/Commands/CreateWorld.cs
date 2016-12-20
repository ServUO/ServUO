using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Gumps;
using Server.Network;

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
		public static List<CommandEntry> Commands = new List<CommandEntry>(new CommandEntry[] {
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
			new CommandEntry("Primeival Lich",  "GenLichPuzzle",	"DeleteLichPuzzle",		112),
			new CommandEntry("Decorations",     "Decorate",         "DecorateDelete",		113),
			new CommandEntry("ML Decorations",  "DecorateML",		"DecorateMLDelete",		114),
			new CommandEntry("SA Decorations",  "DecorateSA",		"DecorateSADelete",		115),
			new CommandEntry("Spawners",		"XmlLoad Spawns",	"XmlSpawnerWipeAll",	116),
            new CommandEntry("Despise",         "SetupDespise",     "DeleteDespise",        117),
            new CommandEntry("Covetous",        "SetupNewCovetous", "DeleteCovetous",       118),
            new CommandEntry("Shame",           "GenerateNewShame", "DeleteShame",          119),
            new CommandEntry("New Magincia",    "GenNewMagincia",   "DeleteNewMagincia",    120),
            new CommandEntry("High Seas",       "DecorateHS",       "DeleteHS",             121),
            new CommandEntry("City Loyalty",    "SetupCityLoyaltySystem",   "DeleteCityLoyaltySystem",             122),
            new CommandEntry("Castle Blackthorn",    "GenBlackthorn",       null,                                  123),
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
								break;
							case CreateWorld.GumpType.Delete:
								if (!String.IsNullOrEmpty(entry.DeleteCommand))
								{
									from.Say("Deleting " + entry.Name);
									CommandSystem.Handle(from, prefix + entry.DeleteCommand);
								}
								break;
							case CreateWorld.GumpType.Recreate:
								if (!String.IsNullOrEmpty(entry.DeleteCommand))
								{
									from.Say("Recreating " + entry.Name);
									CommandSystem.Handle(from, prefix + entry.DeleteCommand);
									CommandSystem.Handle(from, prefix + entry.CreateCommand);
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

				this.AddLabel(20, y + 1, 200, entry.Name);
				this.AddCheck(180, y - 2, 210, 211, true, entry.checkId);
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
}
