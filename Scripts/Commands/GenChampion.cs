using System;
using Server.Engines.CannedEvil;

namespace Server.Commands
{
    public class GenChampionSpawn
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenChampions", AccessLevel.Administrator, new CommandEventHandler(Champ_OnCommand));
        }

        [Usage("GenChampions")]
        [Description("Install ChampionSpawnController at 1415 1695 0.")]
        public static void Champ_OnCommand(CommandEventArgs e)
        {
            Map map1 = Map.Felucca;

            ChampionSpawnController controller = new ChampionSpawnController();

            controller.Active = true;
            controller.MoveToWorld(new Point3D(1415, 1695, 0), map1);

            e.Mobile.SendMessage("Done. Look for ChampionSpawnController at 1415 1695 0 in Felucca.");
        }
    }
}