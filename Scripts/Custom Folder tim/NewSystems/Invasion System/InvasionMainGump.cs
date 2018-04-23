using System;
using System.Collections.Generic;
using System.Globalization;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Customs.Invasion_System
{
    public class InvasionMainGump : Gump
    {
        static Mobile _caller;
        public InvasionTowns SelectedTown;
        public TownMonsterType SelectedMonster;
        public TownChampionType SelectedChamp;
        private Timer _mStartTimer;

        public static void Initialize()
        {
            CommandSystem.Register("InvasionSystem", AccessLevel.Administrator, InvasionMainGump_OnCommand);
        }

        [Usage("InvasionSystem")]
        [Description("Makes a call to your custom gump.")]
        public static void InvasionMainGump_OnCommand(CommandEventArgs e)
        {
            _caller = e.Mobile;

            if (_caller.HasGump(typeof(InvasionMainGump)))
                _caller.CloseGump(typeof(InvasionMainGump));

            _caller.SendGump(new InvasionMainGump(_caller));
        }

        public InvasionMainGump(Mobile from) : this()
        {
            _caller = from;
        }

        public InvasionMainGump() : base( 0, 0 )
        {
            Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;
            int y;

			AddBackground(60, 44, 672, 448, 9200);
			AddLabel(300, 80, 53, @"Ravenwolfe's Invasion System");

            AddLabel(387, 119, 0, @"Invasion Time (UTC):");
            AddImageTiled(512, 119, 170, 20, 0xBBC);
            AddTextEntry(514, 119, 168, 20, 23, 5, DateTime.UtcNow.ToString(), 22);

            AddLabel(120, 120, 0, @"Randomize All");
            AddCheck(90, 120, 210, 211, false, 0);

            // City Section
            AddLabel(210, 175, 0, @"City");
            AddGroup(1);
            var cityLength = Enum.GetNames(typeof(InvasionTowns)).Length;
            y = 200;
            for (int i = 0; i < cityLength; i++)
            {
                y += 25;
                if (i < 9)
                { 
                    AddLabel(120, y, 0, Enum.GetName(typeof(InvasionTowns), i));
                    AddRadio(90, y, 208, 209, false, i + 100);
                }
                else
                {
                    int y2 = y - 225;
                    AddLabel(290, y2, 0, Enum.GetName(typeof(InvasionTowns), i));
                    AddRadio(250, y2, 208, 209, false, i + 100);
                }
            }
            AddLabel(120, 450, 0, @"Random City");
            AddRadio(90, 450, 208, 209, false, 13);

            //Monster Section
			AddLabel(387, 175, 0, @"Monster Type");
            AddGroup(2);
            var monsterLength = Enum.GetNames(typeof(TownMonsterType)).Length;
            y = 200;
            for (int i = 0; i < monsterLength; i++)
            {
                y += 25;
                AddLabel(410, y, 0, Enum.GetName(typeof(TownMonsterType),i));
                AddRadio(387, y, 208, 209, false, i + 200);
            }

            //Champion Section
            AddLabel(594, 175, 0, @"Champion");
            AddGroup(3);
            var champLength = Enum.GetNames(typeof(TownChampionType)).Length;
            y = 200;
            for (int i = 0; i < champLength; i++)
            {
                y += 25;
                AddLabel(620, y, 0, Enum.GetName(typeof(TownChampionType), i));
                AddRadio(590, y, 208, 209, false, i + 300);
            }

            AddButton(525, 450, 247, 248, 1, GumpButtonType.Reply, 0); // Okay
			AddButton(625, 450, 241, 248, 0, GumpButtonType.Reply, 0); // Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 1)
            {
                string time = info.GetTextEntry(5).Text;
                DateTime timeToStart;

                DateTime.TryParse(time, out timeToStart);

                if (timeToStart.Equals(DateTime.MinValue))
                {
                    from.SendMessage("You entered an invalid date. Please follow the pattern.");
                    return;
                }

                if (timeToStart < DateTime.UtcNow)
                {
                    from.SendMessage("You must pick a time in the future!");
                    from.SendMessage("Time is {0}, you picked {1}", DateTime.UtcNow, timeToStart);
                    return;
                }

                if (info.IsSwitched(0))
                {
                    SelectedTown = (InvasionTowns)Utility.Random(0, Enum.GetNames(typeof(InvasionTowns)).Length);
                    SelectedMonster = (TownMonsterType)Utility.Random(0, Enum.GetNames(typeof(TownMonsterType)).Length);
                    SelectedChamp = (TownChampionType)Utility.Random(0, Enum.GetNames(typeof(TownChampionType)).Length);
                }
                else
                {
                    if (info.IsSwitched(13))
                    {
                        SelectedTown = (InvasionTowns)Utility.Random(0, Enum.GetNames(typeof(InvasionTowns)).Length);
                    }
                    else
                    {
                        for (int i = 100; i < 200; i++)
                        {
                            if (info.IsSwitched(i))
                            {
                                SelectedTown = (InvasionTowns)i - 100;
                                break;
                            }
                        }
                    }

                    for (int i = 200; i < 300; i++)
                    {
                        if (info.IsSwitched(i))
                        {
                            SelectedMonster = (TownMonsterType)i - 200;
                            break;
                        }
                    }

                    for (int i = 300; i < 400; i++)
                    {
                        if (info.IsSwitched(i))
                        {
                            SelectedChamp = (TownChampionType)i - 300;
                            break;
                        }
                    }
                }

                from.SendMessage("You have scheduled an invasion for {0} at {1}", SelectedTown, timeToStart);

                new TownInvasion(SelectedTown, SelectedMonster, SelectedChamp, timeToStart);
             }
        }
    }
}