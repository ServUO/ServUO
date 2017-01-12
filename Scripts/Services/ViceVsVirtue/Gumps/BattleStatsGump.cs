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
            BattleTeam leader = battle.GetLeader();
            Guild myGuild = pm.Guild as Guild;

            if (leader == null || leader.Guild == null || myGuild == null)
                return;

            AddBackground(0, 0, 500, 500, 9380);

            AddHtmlLocalized(0, 40, 500, 20, 1154645, "#1154945", Color16, false, false); // The Battle between Vice and Virtue has ended!
            AddHtml(40, 65, 420, 20, String.Format("<basefont color=#B22222>{0} [{1}] has won the battle!", leader.Guild.Name, leader.Guild.Abbreviation), false, false);

            int y = 90;

            if (leader.Guild.Alliance != null)
            {
                AddHtml(40, y, 420, 20, String.Format("<basefont color=#B22222>The {0} Alliance has won the battle!", leader.Guild.Alliance.Name), false, false);
                y += 25;
            }

            BattleTeam team = Battle.GetTeam(myGuild);

            //TODO: Are totals the PLAYERS OVERALL totals, or the guild/alliance totals for that battle???  Or that players totals for that battle
            /*silver += (int)ViceVsVirtueSystem.Instance.GetPoints(pm);

            VvVPlayerEntry entry = ViceVsVirtueSystem.Instance.GetPlayerEntry<VvVPlayerEntry>(pm);

            if (entry != null)
            {
                score = entry.Score;
            }*/

            AddHtmlLocalized(40, y, 420, 20, 1154947, team.Silver.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false); // Total Silver Points: ~1_val~
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154948, team.Score.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false); // Total Score: ~1_val~
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154949, team.Kills.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154950, team.Assists.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154951, team.Deaths.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154952, team.Stolen.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154953, team.ReturnedSigils.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154954, team.ViceReturned.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154955, team.VirtueReturned.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;

            AddHtmlLocalized(40, y, 420, 20, 1154956, team.Disarmed.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Color16, false, false);
            y += 25;
        }
    }
}