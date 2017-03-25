// ChangeSeason Command v1.1.0
// Author: Felladrin
// Started: 2013-08-17
// Updated: 2016-01-04

using Server;
using Server.Commands;
using Server.Network;

namespace Felladrin.Commands
{
    public static class ChangeSeason
    {
        public static void Initialize()
        {
            CommandSystem.Register("ChangeSeason", AccessLevel.Administrator, new CommandEventHandler(ChangeSeason_OnCommand));
        }

        [Usage("ChangeSeason [Spring|Summer|Autumn|Winter|Desolation]")]
        [Description("Changes the season of your current map.")]
        public static void ChangeSeason_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            
            if (e.Length != 1)
            {
                m.SendMessage("Usage: [ChangeSeason [Spring|Summer|Autumn|Winter|Desolation]");
                return;
            }

            int season;

            switch (e.GetString(0).ToLower())
            {
                case "spring":
                    season = 0;
                    break;
                case "summer":
                    season = 1;
                    break;
                case "autumn":
                case "fall":
                    season = 2;
                    break;
                case "winter":
                    season = 3;
                    break;
                case "desolation":
                    season = 4;
                    break;
                default:
                    m.SendMessage("Usage: [ChangeSeason [Spring|Summer|Autumn|Winter|Desolation]");
                    return;
            }

            Map map = m.Map;
            map.Season = season;

            foreach (NetState ns in NetState.Instances)
            {
                if (ns.Mobile == null || ns.Mobile.Map != map)
                    continue;

                ns.Send(SeasonChange.Instantiate(ns.Mobile.GetSeason(), true));
                ns.Mobile.SendEverything();
            }

            m.SendMessage("{0} season changed to {1}.", map.Name, e.GetString(0));
        }
    }
}