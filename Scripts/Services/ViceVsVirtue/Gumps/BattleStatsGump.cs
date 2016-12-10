using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Guilds;
using Server.Network;
using System.Collections.Generic;
using System.Globalization;

namespace Server.Engines.VvV
{
    public class BattleStatsGump : Gump
    {
        public VvVBattle Battle { get; set; }

        public static readonly int Color16 = Engines.Quests.BaseQuestGump.C32216(0xB22222);

        public BattleStatsGump(PlayerMobile pm, VvVBattle battle)
            : base(50, 50)
        {
            Battle = battle;
            AddBackground(0, 0, 500, 500, 9380);

            Guild leader = battle.GetLeader();
            Guild myGuild = pm.Guild as Guild;

            if (leader == null || myGuild == null)
                return;

            AddHtmlLocalized(0, 40, 500, 20, 1154645, "#1154945", Color16, false, false); // The Battle between Vice and Virtue has ended!
            AddHtml(40, 65, 420, 20, String.Format("<basefont color=#B22222>{0} [{1}] has won the battle!", leader.Name, leader.Abbreviation), false, false);

            int y = 90;

            if (leader.Alliance != null && Battle.HasAlliance(leader))
            {
                AddHtml(40, y, 420, 20, String.Format("<basefont color=#B22222>The {0} Alliance has won the battle!", leader.Alliance.Name), false, false);
                y += 25;
            }

            // TODO: Does score reflect everyone, alliance, or just that player! For now, it will be entire alliance

            int silver = 0, score = 0, kills = 0, assists = 0, deaths = 0, stolen = 0, returned = 0, vicereturned = 0, virtuereturned = 0, disarmed = 0;
            List<Guild> alliance = Battle.GetAlliance(myGuild);

            foreach (Guild g in alliance)
            {
                VvVGuildBattleStats stats = Battle.GetGuildStats(g);

                kills += stats.Kills;
                assists += stats.Assists;
                deaths += stats.Deaths;
                stolen += stats.Stolen;
                returned += stats.ReturnedSigils;
                vicereturned += stats.ViceReturned;
                virtuereturned += stats.VirtueReturned;
                disarmed += stats.Disarmed;
            }

            silver += (int)ViceVsVirtueSystem.Instance.GetPoints(pm);

            VvVPlayerEntry entry = ViceVsVirtueSystem.Instance.GetPlayerEntry<VvVPlayerEntry>(pm);

            if (entry != null)
            {
                score = entry.Score;
            }

            AddHtmlLocalized(40, y, 420, 20, 1154947, silver.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false); // Total Silver Points: ~1_val~
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154948, score.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false); // Total Score: ~1_val~
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154949, kills.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154950, assists.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154951, deaths.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154952, stolen.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154953, returned.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154954, vicereturned.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154955, virtuereturned.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154956, disarmed.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;
        }
    }
}