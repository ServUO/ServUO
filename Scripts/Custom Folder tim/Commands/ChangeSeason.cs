//   ___|========================|___
//   \  |  Written by Felladrin  |  /   This script was released on RunUO Community under the GPL licensing terms.
//    > |      August 2013       | <
//   /__|========================|__\   [Change Season] - Current version: 1.0 (August 17, 2013)

namespace Server.Commands
{
    public class ChangeSeason
    {
        public static void Initialize()
        {
            CommandSystem.Register("ChangeSeason", AccessLevel.Administrator, new CommandEventHandler(ChangeSeason_OnCommand));
        }

        [Usage("ChangeSeason [Spring|Summer|Autumn|Winter|Desolation]")]
        [Description("Changes the current season of all facets to the specified one.")]
        public static void ChangeSeason_OnCommand(CommandEventArgs e)
        {
            if (e.Length != 1)
            {
                e.Mobile.SendMessage("Usage: [ChangeSeason [Spring|Summer|Autumn|Winter|Desolation]");
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
                    e.Mobile.SendMessage("Usage: [ChangeSeason [Spring|Summer|Autumn|Winter|Desolation]");
                    return;
            }

            foreach (Map map in Map.AllMaps)
            {
                map.Season = season;

                foreach (Network.NetState ns in Network.NetState.Instances)
                {
                    if (ns.Mobile == null)
                        continue;

                    ns.Send(Network.SeasonChange.Instantiate(ns.Mobile.GetSeason(), true));
                    ns.Mobile.SendEverything();
                }
            }
        }
    }
}