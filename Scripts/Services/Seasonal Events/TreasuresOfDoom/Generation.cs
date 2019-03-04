using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.Points;
using Server.Engines.TreasuresOfDoom;

namespace Server.Engines.TreasuresOfDoom
{
    public static class TreasuresOfDoomGeneration
    {
        public static void Initialize()
        {
            EventSink.WorldSave += OnWorldSave;

            if (Carey.Instance != null)
            {
                Carey.Instance.MoveToWorld(Carey.SpawnLocation, Map.Malas);
            }

            if (Elizabeth.Instance != null)
            {
                Elizabeth.Instance.MoveToWorld(Elizabeth.SpawnLocation, Map.Malas);
            }

            if (PointsSystem.TreasuresOfDoom.Enabled)
            {
                if (VaseSpawner.Instance == null)
                {
                    VaseSpawner.Instance = new VaseSpawner();
                }

                VaseSpawner.Instance.CheckVases();
            }
            else if (VaseSpawner.Instance != null)
            {
                if (VaseSpawner.Instance.Vases != null)
                {
                    ColUtility.SafeDelete(VaseSpawner.Instance.Vases);
                }

                VaseSpawner.Instance = null;
            }
        }

        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            CheckEnabled(true);
        }

        public static void CheckEnabled(bool timed = false)
        {
            var doom = PointsSystem.TreasuresOfDoom;

            if (doom.Enabled && !doom.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Disabling Treasures of Doom");

                        Remove();
                        doom.Enabled = false;
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Disabling Treasures of Doom");

                    Remove();
                    doom.Enabled = false;
                }
            }
            else if (!doom.Enabled && doom.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Enabling Treasures of Doom");

                        Generate();
                        doom.Enabled = true;
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Enabling Treasures of Doom");

                    Generate();
                    doom.Enabled = true;
                }
            }
        }

        public static void Generate()
        {
            if (Carey.Instance == null)
            {
                Carey.Instance = new Carey();
                Carey.Instance.MoveToWorld(Carey.SpawnLocation, Map.Malas);

                Carey.Instance.Home = Carey.SpawnLocation;
                Carey.Instance.RangeHome = 2;
            }

            if (Elizabeth.Instance == null)
            {
                Elizabeth.Instance = new Elizabeth();
                Elizabeth.Instance.MoveToWorld(Elizabeth.SpawnLocation, Map.Malas);

                Elizabeth.Instance.Home = Elizabeth.SpawnLocation;
                Elizabeth.Instance.RangeHome = 2;
            }

            if (Owain.Instance == null)
            {
                Owain.Instance = new Owain();
                Owain.Instance.MoveToWorld(Owain.SpawnLocation, Map.Malas);

                Owain.Instance.Home = Owain.SpawnLocation;
                Owain.Instance.RangeHome = 10;
            }

            if (VaseSpawner.Instance == null)
            {
                VaseSpawner.Instance = new VaseSpawner();
            }

            VaseSpawner.Instance.CheckVases();

            Point3D p = new Point3D(395, 220, -18);

            if (Map.Malas.FindItem<DoomPlaque>(p) == null)
            {
                var plaque = new DoomPlaque();
                plaque.MoveToWorld(p, Map.Malas);
            }

            p = new Point3D(388, 221, -20);

            if (Map.Malas.FindItem<DoomSign>(p) == null)
            {
                var plaque = new DoomSign();
                plaque.MoveToWorld(p, Map.Malas);
            }

            p = new Point3D(66, 223, -1);

            if (Map.Malas.FindItem<Moongate>(p) == null)
            {
                var moongate = new Moongate();
                moongate.ItemID = 0x4BCB;
                moongate.Hue = 2676;
                moongate.Dispellable = false;
                moongate.Target = new Point3D(396, 220, -20);
                moongate.TargetMap = Map.Malas;

                moongate.MoveToWorld(p, Map.Malas);
            }
        }

        public static void Remove()
        {
            if (Carey.Instance != null)
            {
                Carey.Instance.Delete();
            }

            if (VaseSpawner.Instance != null)
            {
                if (VaseSpawner.Instance.Vases != null)
                {
                    ColUtility.SafeDelete(VaseSpawner.Instance.Vases);
                }

                VaseSpawner.Instance = null;
            }
        }
    }
}