using Server.ContextMenus;
using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Engines.TreasuresOfDoom
{
    public class VaseSpawner : ISpawner
    {
        public List<AncientClayVase> Vases { get; set; }
        public readonly int VaseCount = 3;
        public readonly Rectangle2D SpawnRecs = new Rectangle2D(257, 5, 249, 257);

        public static readonly int MinSpawn = 1;
        public static readonly int MaxSpawn = 3;

        public bool UnlinkOnTaming => false;
        public Point3D HomeLocation => Point3D.Zero;
        public int HomeRange => 0;

        public static VaseSpawner Instance { get; set; }

        public void Remove(ISpawnable spawn)
        {
            if (spawn is AncientClayVase)
            {
                RemoveVase((AncientClayVase)spawn);
            }
        }

        public void GetSpawnProperties(ISpawnable spawn, ObjectPropertyList list)
        {
        }

        public void GetSpawnContextEntries(ISpawnable spawn, Mobile m, List<ContextMenuEntry> list)
        {
        }

        public void AddVase(AncientClayVase vase)
        {
            if (Vases == null)
            {
                Vases = new List<AncientClayVase>();
            }

            if (!Vases.Contains(vase))
            {
                Vases.Add(vase);
            }
        }

        public void RemoveVase(AncientClayVase vase)
        {
            if (Vases != null)
            {
                if (Vases.Contains(vase))
                {
                    Vases.Remove(vase);
                }

                if (TreasuresOfDoomEvent.Instance.Running && Vases.Count < VaseCount)
                {
                    Timer.DelayCall(TimeSpan.FromMinutes(Utility.RandomMinMax(MinSpawn, MaxSpawn)), () =>
                    {
                        CreateVases(1);
                    });
                }
            }
        }

        public void CheckVases()
        {
            int count = 0;

            if (Vases == null)
            {
                Vases = new List<AncientClayVase>();
            }

            count = VaseCount - Vases.Count;

            if (count > 0)
            {
                CreateVases(count);
            }
        }

        public void CreateVases(int count)
        {
            for (int i = 0; i < count; i++)
            {
                AncientClayVase vase = new AncientClayVase(true);
                ItemFlags.SetStealable(vase, true);
                vase.Movable = false;

                vase.Spawner = this;

                AddVase(vase);

                Point3D p = Point3D.Zero;

                do
                {
                    p = Map.Malas.GetRandomSpawnPoint(SpawnRecs);
                }
                while (p == Point3D.Zero || !Map.Malas.CanSpawnMobile(p));

                vase.OnBeforeSpawn(p, Map.Malas);
                vase.MoveToWorld(p, Map.Malas);
            }
        }

        public static void AddToSpawner(AncientClayVase vase)
        {
            if (Instance == null)
            {
                Instance = new VaseSpawner();
            }

            Instance.AddVase(vase);
        }
    }
}
