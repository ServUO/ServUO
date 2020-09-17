using Server.Items;
using Server.Engines.SeasonalEvents;

namespace Server.Engines.TreasuresOfDoom
{
    public class TreasuresOfDoomEvent : SeasonalEvent
    {
        public static TreasuresOfDoomEvent Instance { get; set; }

        public TreasuresOfDoomEvent(EventType type, string name, EventStatus status)
            : base(type, name, status)
        {
            Instance = this;
        }

        public TreasuresOfDoomEvent(EventType type, string name, EventStatus status, int month, int day, int duration)
            : base(type, name, status, month, day, duration)
        {
            Instance = this;
        }

        protected override void Generate()
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
                DoomPlaque plaque = new DoomPlaque();
                plaque.MoveToWorld(p, Map.Malas);
            }

            p = new Point3D(388, 221, -20);

            if (Map.Malas.FindItem<DoomSign>(p) == null)
            {
                DoomSign plaque = new DoomSign();
                plaque.MoveToWorld(p, Map.Malas);
            }

            p = new Point3D(66, 223, -1);

            if (Map.Malas.FindItem<Moongate>(p) == null)
            {
                Moongate moongate = new Moongate
                {
                    ItemID = 0x4BCB,
                    Hue = 2676,
                    Dispellable = false,
                    Target = new Point3D(396, 220, -20),
                    TargetMap = Map.Malas
                };

                moongate.MoveToWorld(p, Map.Malas);
            }
        }

        protected override void Remove()
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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = InheritInsertion ? 0 : reader.ReadInt();
        }

        public static void Initialize()
        {
            if (Carey.Instance != null)
            {
                Carey.Instance.MoveToWorld(Carey.SpawnLocation, Map.Malas);
            }

            if (Elizabeth.Instance != null)
            {
                Elizabeth.Instance.MoveToWorld(Elizabeth.SpawnLocation, Map.Malas);
            }

            if (Instance.Running)
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
    }
}
