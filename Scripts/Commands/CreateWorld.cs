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
			new CommandEntry("Moongates",       "Moongen",			"MoonGenDelete", 101),
			new CommandEntry("Doors",           "DoorGen",			"DoorGenDelete", 102), // Improve
			new CommandEntry("Signs",           "SignGen",			"SignGenDelete", 103), // Improve
			new CommandEntry("Teleporters",     "TelGen",			"TelGenDelete", 104), // Improve
			new CommandEntry("Doom Lamp",       "GenLeverPuzzle",   "LampPuzzleDelete", 105),
			new CommandEntry("Doom Gauntlet",   "GenGauntlet",      "DeleteGauntlet", 106),
			new CommandEntry("Champions",       "GenChampions",     "DeleteChampions", 107),
			new CommandEntry("Khaldun",         "GenKhaldun",		"", 108), // Implement
			new CommandEntry("Stealables",      "GenStealArties",   "RemoveStealArties", 109),
			new CommandEntry("Solen Hives",     "SHTelGen",         "SHTelGenDelete", 110),
			new CommandEntry("Malas Secrets",   "SecretLocGen",		"", 111), // Implement
			new CommandEntry("Factions",        "GenerateFactions",	"", 112), // Implement
			new CommandEntry("Primeival Lich",  "GenLichPuzzle",	"", 113), // Implement
			new CommandEntry("Decorations",     "Decorate",         "DecorateDelete", 114), // Improve
			new CommandEntry("ML Decorations",  "DecorateML",		"", 115), // Implement
			new CommandEntry("SA Decorations",  "DecorateSA",		"", 116), // Implement
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

		[Usage("CreateWorld")]
		[Description("Generates the world with a menu.")]
		private static void Create_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new CreateWorldGump(e, GumpType.Create));
		}

		[Usage("DeleteWorld")]
		[Description("Undoes world generation with a menu.")]
		private static void Delete_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new CreateWorldGump(e, GumpType.Delete));
		}

		[Usage("RecreateWorld")]
		[Description("Re-generates the world with a menu.")]
		private static void Recreate_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump(new CreateWorldGump(e, GumpType.Recreate));
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
				this.AddLabel(20, y + 1, 200, entry.Name);
				this.AddCheck(180, y - 2, 210, 211, true, entry.checkId);
				y += 25;
			}
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
					World.Broadcast(0x35, false, "The world is generating. This may take some time...");
					ArrayList selections = new ArrayList(info.Switches);
					string prefix = Server.Commands.CommandSystem.Prefix;
					foreach (int sel in selections)
					{
						foreach (CreateWorld.CommandEntry entry in CreateWorld.Commands)
						{
							if (entry.checkId == sel)
							{
								switch (m_Type)
								{
									case CreateWorld.GumpType.Create:
										from.Say("Generating " + entry.Name);
										CommandSystem.Handle(from, prefix + entry.CreateCommand);
										break;
									case CreateWorld.GumpType.Delete:
										if (!String.IsNullOrEmpty(entry.DeleteCommand))
										{
											from.Say("Recreating " + entry.Name);
											CommandSystem.Handle(from, prefix + entry.DeleteCommand);
										}
										break;
									case CreateWorld.GumpType.Recreate:
										if(!String.IsNullOrEmpty(entry.DeleteCommand))
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
					break;
			}
        }
    }
}
