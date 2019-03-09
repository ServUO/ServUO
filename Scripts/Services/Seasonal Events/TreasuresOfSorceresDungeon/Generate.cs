using System.Linq;
using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Engines.Points;

namespace Server.Engines.SorcerersDungeon
{
    public static class SorcerersDungeonGenerate
    {
        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            CheckEnabled(true);
        }

        public static void CheckEnabled(bool timed = false)
        {
            var sd = PointsSystem.SorcerersDungeon;

            if (sd.Enabled && !sd.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Disabling Treasures of Scorcerer's Dungeon");

                        Remove();
                        sd.Enabled = false;
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Disabling Treasures of Scorcerer's Dungeon");

                    Remove();
                    sd.Enabled = false;
                }
            }
            else if (!sd.Enabled && sd.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Enabling Treasures of Scorcerer's Dungeon");

                        Generate();
                        sd.Enabled = true;
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Enabling Treasures of Scorcerer's Dungeon");

                    Generate();
                    sd.Enabled = true;
                }
            }
        }

        public static void Generate()
        {
            Map map = Map.Ilshenar;

            if (SorcerersDungeonResearcher.Instance == null)
            {
                SorcerersDungeonResearcher.Instance = new SorcerersDungeonResearcher();
                SorcerersDungeonResearcher.Instance.MoveToWorld(new Point3D(536, 456, -53), map);
            }

            if (map.FindItem<Static>(new Point3D(546, 460, 6)) == null)
            {
                Static st = new Static(0x9D2B);
                st.MoveToWorld(new Point3D(546, 460, 6), map);

                st = new Static(0x9D2C);
                st.MoveToWorld(new Point3D(548, 460, 6), map);

                st = new Static(0x9D2D);
                st.MoveToWorld(new Point3D(548, 458, 6), map);
            }

            if (map.FindItem<Static>(new Point3D(545, 462, -53)) == null)
            {
                var st = new Static(0x9F34);
                st.MoveToWorld(new Point3D(545, 462, -53), map);
            }

            if (map.FindItem<Static>(new Point3D(550, 462, -53)) == null)
            {
                var st = new Static(0x9F34);
                st.MoveToWorld(new Point3D(550, 462, -53), map);
            }

            if (map.FindItem<Static>(new Point3D(545, 463, -55)) == null)
            {
                var st = new Static(0x9F28);
                st.MoveToWorld(new Point3D(545, 463, -55), map);
            }

            if (map.FindItem<Static>(new Point3D(550, 463, -55)) == null)
            {
                var st = new Static(0x9F24);
                st.MoveToWorld(new Point3D(550, 463, -55), map);
            }

            if (TOSDSpawner.Instance == null)
            {
                var spawner = new TOSDSpawner();
                spawner.BeginTimer();
            }
        }

        public static void Remove()
        {
            if (TOSDSpawner.Instance != null)
            {
                TOSDSpawner.Instance.Deactivate();
            }
        }
    }
}
