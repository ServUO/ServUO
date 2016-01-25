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
			new CommandEntry("Moongates",       "Moongen",			"", 101),
			new CommandEntry("Doors",           "DoorGen",			"", 102),
			new CommandEntry("Signs",           "SignGen",			"", 103),
			new CommandEntry("Teleporters",     "TelGen",			"", 104),
			new CommandEntry("Doom Lamp",       "GenLeverPuzzle",	"", 105),
			new CommandEntry("Doom Gauntlet",   "GenGauntlet",		"", 106),
			new CommandEntry("Champions",       "GenChampions",		"", 107),
			new CommandEntry("Khaldun",         "GenKhaldun",		"", 108),
			new CommandEntry("Stealables",      "GenStealArties",	"", 109),
			new CommandEntry("Solen Hives",     "SHTelGen",			"", 110),
			new CommandEntry("Malas Secrets",   "SecretLocGen",		"", 111),
			new CommandEntry("Factions",        "GenerateFactions",	"", 112),
			new CommandEntry("Primeival Lich",  "GenLichPuzzle",	"", 113),
			new CommandEntry("Decorations",     "Decorate",			"", 114),
			new CommandEntry("ML Decorations",  "DecorateML",		"", 115),
			new CommandEntry("SA Decorations",  "DecorateSA",		"", 116),
		});
        public CreateWorld()
        {
        }

        public static void Initialize() 
        { 
            CommandSystem.Register("Createworld", AccessLevel.Administrator, new CommandEventHandler(Create_OnCommand)); 
        }

        [Usage("[createworld")]
        [Description("Create world with a menu.")]
        private static void Create_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new CreateWorldGump(e));
        }
    }
}

namespace Server.Gumps
{
    public class CreateWorldGump : Gump
    {
        private readonly CommandEventArgs m_CommandEventArgs;
        public CreateWorldGump(CommandEventArgs e)
            : base(50,50)
        {
            this.m_CommandEventArgs = e;
            this.Closable = true;
            this.Dragable = true;

            this.AddPage(1);

			int items = CreateWorld.Commands.Count;
			this.AddBackground(0, 0, 240, 75 + items * 25, 5054);
			this.AddLabel(40, 2, 200, "CREATE WORLD GUMP");
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
								from.Say("Generating " + entry.Name);
								CommandSystem.Handle(from, prefix + entry.CreateCommand);
							}
						}
					}
					World.Broadcast(0x35, false, "World generation complete.");
					break;
			}
        }
    }
}
