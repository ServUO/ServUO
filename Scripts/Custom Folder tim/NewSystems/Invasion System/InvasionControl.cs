using System;
using System.Collections.Generic;
using System.IO;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Customs.Invasion_System
{
    public static class InvasionControl
    {
        public static List<TownInvasion> Invasions = new List<TownInvasion>();

        static Mobile _caller;

        public static void Initialize()
        {
            CommandSystem.Register("ListInvasions", AccessLevel.Administrator, ListInvasions_OnCommand);
        }

        [Usage("ListInvasions")]
        [Description("Lists all active invasions.")]
        public static void ListInvasions_OnCommand(CommandEventArgs e)
        {
            _caller = e.Mobile;

            if (Invasions.Count == 0)
            {
                _caller.SendMessage("There are no invasions!");
                return;
            }
            _caller.SendGump(new InvasionGump(Invasions));
        }
    }

    public class InvasionGump : Gump
    {
        public InvasionGump(List<TownInvasion> invasions) : base(100, 100)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddBackground(100, 100, 620, 500, 5054);
            AddAlphaRegion(100, 100, 620, 500);

            int y = 110;
            int x = 110;
            int id = 0;

            AddHtml(x, y, 120, 20, "<BASEFONT COLOR=#ff33cc><BIG>Town", false, false);
            AddHtml(x+120, y, 120, 20, "<BASEFONT COLOR=#ff33cc><BIG>Monster Type", false, false);
            AddHtml(x+240, y, 120, 20, "<BASEFONT COLOR=#ff33cc><BIG>Champion", false, false);
            AddHtml(x+360, y, 140, 20, "<BASEFONT COLOR=#ff33cc><BIG>Time Scheduled", false, false);
            AddHtml(x+520, y, 50, 20, "<BASEFONT COLOR=#ff33cc><BIG>Stop", false, false);
            AddHtml(x+565, y, 50, 20, "<BASEFONT COLOR=#ff33cc><BIG>Props", false, false);

            foreach ( var invasion in invasions)
            {
                y += 30;
                id += 1;
                AddLabel(x, y, 2100, invasion.InvasionTown.ToString());
                AddLabel(x+120, y, 2100, invasion.TownMonsterType.ToString());
                AddLabel(x+240, y, 2100, invasion.TownChampionType.ToString());
                AddLabel(x+360, y, 2100, invasion.IsRunning ? "Active" : invasion.StartTime.ToString());
                AddButton(x+525, y, 210, 211, id, GumpButtonType.Reply, 0);
                AddButton(x+570, y, 210, 211, id+100, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0)
                return;

            if (info.ButtonID > 100)
            {
                int i = info.ButtonID - 101;
                var prop = InvasionControl.Invasions[i];
                from.SendGump(new PropertiesGump(from, prop));
            }
            else if (info.ButtonID >= 0 && info.ButtonID < 100)
            {
                int i = info.ButtonID - 1;
                TownInvasion invasion = InvasionControl.Invasions[i];

                invasion.OnStop();
                from.SendMessage("You have deleted the selected invasion!");
            }
        }
    }

    public static class InvasionPersistence
    {
        private static string FilePath = Path.Combine("Saves", "Invasions", "Persistence.bin");

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        private static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0); // version

                    writer.Write(InvasionControl.Invasions.Count);

                    foreach (var m in InvasionControl.Invasions)
                    {
                        m.Serialize(writer);
                    }
                });
        }

        private static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    var version = reader.ReadInt();

                    switch (version)
                    {
                        case 0:
                        {
                            var count = reader.ReadInt();

                            for (var i = 0; i < count; ++i)
                            {
                                var invasion = new TownInvasion(reader);

                                InvasionControl.Invasions.Add(invasion);
                            }
                        }
                            break;
                    }
                });
        }
    }
}
