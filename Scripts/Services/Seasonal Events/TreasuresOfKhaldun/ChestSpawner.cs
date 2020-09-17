using Server.ContextMenus;
using Server.Items;

using System;
using System.Collections.Generic;

namespace Server.Engines.Khaldun
{
    public class ChestSpawner : ISpawner
    {
        public List<KhaldunChest> Chests { get; set; }
        public readonly int ChestCount = 5;
        public readonly Rectangle2D SpawnRecs = new Rectangle2D(5383, 1287, 241, 216);

        public bool UnlinkOnTaming => false;
        public Point3D HomeLocation => Point3D.Zero;
        public int HomeRange => 0;

        public static ChestSpawner InstanceFel { get; set; }
        public static ChestSpawner InstanceTram { get; set; }

        public void Remove(ISpawnable spawn)
        {
            var chest = spawn as KhaldunChest;

            if (chest != null)
            {
                RemoveChest(chest);
            }
        }

        public void GetSpawnProperties(ISpawnable spawn, ObjectPropertyList list)
        {
            // We're not using this
        }

        public void GetSpawnContextEntries(ISpawnable spawn, Mobile m, List<ContextMenuEntry> list)
        {
            // We're not using this
        }

        public void AddChest(KhaldunChest Chest)
        {
            if (Chests == null)
            {
                Chests = new List<KhaldunChest>();
            }

            if (!Chests.Contains(Chest))
            {
                Chests.Add(Chest);
            }
        }

        public void RemoveChest(KhaldunChest Chest)
        {
            if (Chests != null)
            {
                if (Chests.Contains(Chest))
                {
                    Chests.Remove(Chest);
                }

                if (TreasuresOfKhaldunEvent.Instance.Running && Chests.Count < ChestCount)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60)), () =>
                    {
                        CreateChests(1);
                    });
                }
            }
        }

        public void CheckChests()
        {
            int count = 0;

            if (Chests == null)
            {
                Chests = new List<KhaldunChest>();
            }

            count = ChestCount - Chests.Count;

            if (count > 0)
            {
                CreateChests(count);
            }
        }

        public void CreateChests(int count)
        {
            var map = this == InstanceTram ? Map.Trammel : Map.Felucca;

            for (int i = 0; i < count; i++)
            {
                var Chest = new KhaldunChest
                {
                    Movable = false,
                    Spawner = this
                };

                AddChest(Chest);

                Point3D p;

                do
                {
                    p = map.GetRandomSpawnPoint(SpawnRecs);
                }
                while (p == Point3D.Zero || !map.CanSpawnMobile(p));

                Chest.OnBeforeSpawn(p, map);
                Chest.MoveToWorld(p, map);
            }
        }
    }
}
