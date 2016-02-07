using System;
using System.Collections;
using System.IO;
using Server.Mobiles;

namespace Server.Commands
{
    public class RunUOSpawnerExporter
    {
        public const bool Enabled = true;
        public static void Initialize()
        {
            CommandSystem.Register("RunUOSpawnerExporter", AccessLevel.Administrator, new CommandEventHandler(RunUOSpawnerExporter_OnCommand));
            CommandSystem.Register("RSE", AccessLevel.Administrator, new CommandEventHandler(RunUOSpawnerExporter_OnCommand));
        }

        public static int ConvertToInt(TimeSpan ts)
        {
            return ((ts.Hours * 60) + ts.Minutes + (ts.Seconds / 60));
        }

        [Usage("RunUOSpawnerExporter")]
        [Aliases("RSE")]
        [Description("Convert RunUO Spawners to PremiumSpawners.")]
        public static void RunUOSpawnerExporter_OnCommand(CommandEventArgs e)
        {
            Map map = e.Mobile.Map;
            ArrayList list = new ArrayList();
            ArrayList entries = new ArrayList();

            if (!Directory.Exists(@".\Export\"))
                Directory.CreateDirectory(@".\Export\");

            using (StreamWriter op = new StreamWriter(String.Format(@".\Export\{0}.map", map)))
            {
                if (map == null || map == Map.Internal)
                {
                    e.Mobile.SendMessage("You may not run that command here.");
                    return;
                }

                e.Mobile.SendMessage("Converting Spawners...");

                op.WriteLine("#######################################");
                op.WriteLine("## Converted By RunUOSpawnerExporter ##");
                op.WriteLine("##        Developed by Nerun         ##");
                op.WriteLine("#######################################");

                foreach (Item item in World.Items.Values)
                {
                    if (item.Map == map && item.Parent == null && item is Spawner)
                        list.Add(item);
                }

                foreach (Item item in World.Items.Values)
                {
                    if (item.Map == map && item.Parent == null && item is Spawner)
                    {
                        string mapfinal = "";

                        if (map == Map.Maps[0])
                        {
                            mapfinal = "1";
                        }
                        if (map == Map.Maps[1])
                        {
                            mapfinal = "2";
                        }
                        if (map == Map.Maps[2])
                        {
                            mapfinal = "3";
                        }
                        if (map == Map.Maps[3])
                        {
                            mapfinal = "4";
                        }
                        if (map == Map.Maps[4])
                        {
                            mapfinal = "5";
                        }

                        Spawner spawner = ((Spawner)item);

                        if (spawner.SpawnNames.Count > 0)
                        {
                            int MinDelay = ConvertToInt(spawner.MinDelay);

                            if (MinDelay < 1)
                            {
                                MinDelay = 1;
                            }

                            int MaxDelay = ConvertToInt(spawner.MaxDelay);

                            if (MaxDelay < MinDelay)
                            {
                                MaxDelay = MinDelay;
                            }

                            string towrite = "*|" + spawner.SpawnNames[0];

                            for (int i = 1; i < spawner.SpawnNames.Count; ++i)
                            {
                                towrite = towrite + ":" + spawner.SpawnNames[i].ToString();
                            }

                            op.WriteLine("{0}||||||{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|1|{9}|0|0|0|0|0", towrite, spawner.X, spawner.Y, spawner.Z, mapfinal, MinDelay, MaxDelay, spawner.WalkingRange, spawner.HomeRange, spawner.Count);
                        }
                        else
                        {
                            op.WriteLine("## No creatures in spawner at: {0} {1} {2}, map: {3}", spawner.X, spawner.Y, spawner.Z, mapfinal);
                        }
                    }
                }

                e.Mobile.SendMessage(String.Format("You exported {0} RunUO Spawner{1} from this facet.", list.Count, list.Count == 1 ? "" : "s"));
            }
        }
    }
}