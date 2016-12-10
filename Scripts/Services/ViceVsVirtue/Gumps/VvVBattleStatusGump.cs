using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Guilds;
using Server.Network;
using System.Collections.Generic;

namespace Server.Engines.VvV
{
    public class VvVBattleStatusGump : Gump
    {
        public PlayerMobile User { get; set; }
        public VvVBattle Battle { get; set; }

        public VvVBattleStatusGump(PlayerMobile pm, VvVBattle battle) : base(50, 50)
        {
            User = pm;
            Battle = battle;

            AddGumpLayout();
        }

        public void AddGumpLayout()
        {
            AddImage(0, 0, 30566);

            if (DateTime.UtcNow >= Battle.NextSigilSpawn && Battle.Sigil != null && !Battle.Sigil.Deleted)
                AddImage(200, 300, 30583);

            List<VvVGuildBattleStats> guilds = new List<VvVGuildBattleStats>();
            foreach (Guild g in Battle.Participants)
            {
                VvVGuildBattleStats stats = Battle.GetGuildStats(g);

                if (stats != null)
                    guilds.Add(stats);
            }

            guilds.Sort();
            double offset = 216 / (double)VvVBattle.ScoreToWin; 

            for (int i = 0; i < guilds.Count; i++)
            {
                VvVGuildBattleStats stats = guilds[i];

                AddHtml(87, 118 + (30 * i), 50, 20, String.Format("<basefont color=#FFFFFF>{0}", stats.Guild.Abbreviation), false, false);
                AddImageTiled(145, 120 + (30 * i), (int)Math.Min(216, (stats.Points * offset)), 12, 30584);

                if (i == 2)  // stupid gump only allows 3 to be shown
                    break;
            }

            int count = Battle.Messages.Count - 1;
            int y = 205;

            for (int i = count; i >= 0; i--)
            {
                if (i <= count - 3)
                    break;

                AddHtml(100, y, 250, 16, String.Format("<basefont color=#00CED1>{0}", Battle.Messages[i]), false, false);

                y += 16;
            }

            Guild gu = User.Guild as Guild;

            if (gu != null)
            {
                VvVGuildBattleStats stats = Battle.GetGuildStats(gu);

                AddHtml(87, 268, 50, 20, String.Format("<basefont color=#FFFFFF>{0}", gu.Abbreviation), false, false);
                AddImageTiled(145, 270, (int)Math.Min(216, (stats.Points * offset)), 12, 30584);
            }

            TimeSpan left = (Battle.StartTime + TimeSpan.FromMinutes(VvVBattle.Duration)) - DateTime.UtcNow;
            AddHtml(207, 21, 60, 20, "<basefont color=red>" + String.Format("{0:mm\\:ss}", left), false, false);
        }

        public void Refresh()
        {
            Entries.Clear();
            Entries.TrimExcess();
            AddGumpLayout();
            User.CloseGump(this.GetType());
            User.SendGump(this, false);
        }
    }
}