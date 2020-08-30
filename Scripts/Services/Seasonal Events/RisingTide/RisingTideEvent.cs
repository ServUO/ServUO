using Server.Items;
using Server.Engines.SeasonalEvents;

namespace Server.Engines.RisingTide
{
    public class RisingTideEvent : SeasonalEvent
    {
        public static RisingTideEvent Instance { get; set; }

        public RisingTideEvent(EventType type, string name, EventStatus status)
            : base(type, name, status)
        {
            Instance = this;
        }

        public RisingTideEvent(EventType type, string name, EventStatus status, int month, int day, int duration)
            : base(type, name, status, month, day, duration)
        {
            Instance = this;
        }

        protected override void Generate()
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
                PlunderBeaconSpawner.Spawner = new PlunderBeaconSpawner();
            }
        }

        protected override void Remove()
        {
            if (PlunderBeaconSpawner.Spawner != null)
            {
                PlunderBeaconSpawner.Spawner.SystemDeactivate();
            }
        }

        public static void Initialize()
        {
            if (BlackMarketMerchant.InstanceTram != null)
            {
                BlackMarketMerchant.InstanceTram.MoveToWorld(BlackMarketMerchant.SpawnLocation, Map.Trammel);
            }

            if (BlackMarketMerchant.InstanceFel != null)
            {
                BlackMarketMerchant.InstanceFel.MoveToWorld(BlackMarketMerchant.SpawnLocation, Map.Felucca);
            }

            if (Instance.Running && PlunderBeaconSpawner.Spawner == null)
            {
                PlunderBeaconSpawner.Spawner = new PlunderBeaconSpawner();
            }
        }
    }
}
