using System;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.Points;

namespace Server.Engines.RisingTide
{
    public static class RisingTideGeneration
    {
        public static void Initialize()
        {
            EventSink.WorldSave += OnWorldSave;

            if (BlackMarketMerchant.InstanceTram != null)
            {
                BlackMarketMerchant.InstanceTram.MoveToWorld(BlackMarketMerchant.SpawnLocation, Map.Trammel);
            }

            if (BlackMarketMerchant.InstanceFel != null)
            {
                BlackMarketMerchant.InstanceFel.MoveToWorld(BlackMarketMerchant.SpawnLocation, Map.Felucca);
            }

            if (PointsSystem.RisingTide.Enabled && PlunderBeaconSpawner.Spawner == null)
            {
                new PlunderBeaconSpawner();
            }
        }

        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            CheckEnabled(true);
        }

        public static void CheckEnabled(bool timed = false)
        {
            var risingTide = PointsSystem.RisingTide;

            if (risingTide.Enabled && !risingTide.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Disabling Rising Tide");

                        Remove();
                        risingTide.Enabled = false;
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Disabling Rising Tide");

                    Remove();
                    risingTide.Enabled = false;
                }
            }
            else if (!risingTide.Enabled && risingTide.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Enabling Rising Tide");

                        Generate();
                        risingTide.Enabled = true;
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Enabling Rising Tide");

                    Generate();
                    risingTide.Enabled = true;
                }
            }
        }

        public static void Generate()
        {
            if (BlackMarketMerchant.InstanceTram == null)
            {
                BlackMarketMerchant.InstanceTram = new BlackMarketMerchant();
                BlackMarketMerchant.InstanceTram.MoveToWorld(BlackMarketMerchant.SpawnLocation, Map.Trammel);

                BlackMarketMerchant.InstanceTram.Home = BlackMarketMerchant.SpawnLocation;
                BlackMarketMerchant.InstanceTram.RangeHome = 2;
            }

            if (BlackMarketMerchant.InstanceFel == null)
            {
                BlackMarketMerchant.InstanceFel = new BlackMarketMerchant();
                BlackMarketMerchant.InstanceFel.MoveToWorld(BlackMarketMerchant.SpawnLocation, Map.Felucca);

                BlackMarketMerchant.InstanceFel.Home = BlackMarketMerchant.SpawnLocation;
                BlackMarketMerchant.InstanceFel.RangeHome = 2;
            }

            if (PlunderBeaconSpawner.Spawner == null)
            {
                new PlunderBeaconSpawner();
            }
        }

        public static void Remove()
        {
            if (PlunderBeaconSpawner.Spawner != null)
            {
                PlunderBeaconSpawner.Spawner.SystemDeactivate();
            }
        }
    }
}
