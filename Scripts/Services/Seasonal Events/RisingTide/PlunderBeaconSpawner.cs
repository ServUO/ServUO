using Server.Multis;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class PlunderBeaconSpawner
    {
        public enum PlunderZone
        {
            None = -1,
            Tram,
            Fel,
            Tokuno1,
            Tokuno2,
            Tokuno3,
            Tokuno4
        }

        public static PlunderBeaconSpawner Spawner { get; set; }
        public Dictionary<PlunderZone, List<PlunderBeaconAddon>> PlunderBeacons { get; set; }

        public static void AddPlunderBeacon(PlunderZone zone, PlunderBeaconAddon beacon)
        {
            if (Spawner == null)
                return;

            if (!Spawner.PlunderBeacons[zone].Contains(beacon))
            {
                Spawner.PlunderBeacons[zone].Add(beacon);
            }
        }

        public void RemovePlunderBeacon(PlunderBeaconAddon beacon)
        {
            if (Spawner == null || Spawner.PlunderBeacons == null)
                return;

            foreach (KeyValuePair<PlunderZone, List<PlunderBeaconAddon>> kvp in Spawner.PlunderBeacons)
            {
                if (kvp.Value.Contains(beacon))
                {
                    kvp.Value.Remove(beacon);
                }
            }
        }

        public void SystemDeactivate()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }

            List<PlunderBeaconAddon> list = new List<PlunderBeaconAddon>();

            foreach (KeyValuePair<PlunderZone, List<PlunderBeaconAddon>> kvp in PlunderBeacons)
            {
                list.AddRange(kvp.Value);
            }

            foreach (PlunderBeaconAddon beacon in list)
            {
                beacon.Delete();
            }

            PlunderBeacons.Clear();
            Spawner = null;
        }

        private readonly Rectangle2D[] _Zones =
        {
            new Rectangle2D(1574, 3620, 766, 465),
            new Rectangle2D(1574, 3620, 766, 465),
            new Rectangle2D(403, 843, 80, 335),
            new Rectangle2D(631, 20, 189, 110),
            new Rectangle2D(1037, 20, 190, 150),
            new Rectangle2D(1274, 977, 141, 221)
        };

        private readonly int[] _SpawnCount =
        {
            5, 5, 3, 3, 3, 3
        };

        public Timer Timer { get; set; }

        public PlunderBeaconSpawner()
        {
            Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), TickTock);

            PlunderBeacons = new Dictionary<PlunderZone, List<PlunderBeaconAddon>>();
            PlunderBeacons[PlunderZone.Tram] = new List<PlunderBeaconAddon>();
            PlunderBeacons[PlunderZone.Fel] = new List<PlunderBeaconAddon>();
            PlunderBeacons[PlunderZone.Tokuno1] = new List<PlunderBeaconAddon>();
            PlunderBeacons[PlunderZone.Tokuno2] = new List<PlunderBeaconAddon>();
            PlunderBeacons[PlunderZone.Tokuno3] = new List<PlunderBeaconAddon>();
            PlunderBeacons[PlunderZone.Tokuno4] = new List<PlunderBeaconAddon>();
        }

        public void TickTock()
        {
            CheckSpawn();
        }

        public void CheckSpawn()
        {
            foreach (int i in Enum.GetValues(typeof(PlunderZone)))
            {
                if (i == -1)
                    continue;

                PlunderZone zone = (PlunderZone)i;
                int low = _SpawnCount[i] - PlunderBeacons[zone].Count;

                if (low > 0)
                {
                    Spawn(zone, low);
                }
            }
        }

        public void Spawn(PlunderZone zone, int amount)
        {
            Map map = Map.Trammel;

            if (zone == PlunderZone.Fel)
                map = Map.Felucca;
            else if (zone > PlunderZone.Fel)
                map = Map.Tokuno;

            for (int i = 0; i < amount; i++)
            {
                Rectangle2D rec = _Zones[(int)zone];
                Point3D p;

                while (true)
                {
                    p = map.GetRandomSpawnPoint(rec); //new Point3D(rec.X + Utility.Random(rec.Width), rec.Y + Utility.RandomMinMax(rec.Start.X, rec.Height), -5);

                    if (p.Z != -5)
                        p.Z = -5;

                    Rectangle2D bounds = new Rectangle2D(p.X - 7, p.Y - 7, 15, 15);

                    bool badSpot = false;

                    for (int x = bounds.Start.X; x <= bounds.Start.X + bounds.Width; x++)
                    {
                        for (int y = bounds.Start.Y; y <= bounds.Start.Y + bounds.Height; y++)
                        {
                            if (BaseBoat.FindBoatAt(new Point3D(x, y, -5), map) != null || !SpecialFishingNet.ValidateDeepWater(map, x, y))
                            {
                                badSpot = true;
                                break;
                            }
                        }

                        if (badSpot)
                            break;
                    }

                    if (!badSpot)
                    {
                        IPooledEnumerable eable = map.GetMobilesInBounds(bounds);

                        foreach (Mobile m in eable)
                        {
                            if (m.AccessLevel == AccessLevel.Player)
                            {
                                badSpot = true;
                                break;
                            }
                        }

                        eable.Free();

                        if (!badSpot)
                        {
                            PlunderBeaconAddon beacon = new PlunderBeaconAddon();
                            beacon.MoveToWorld(p, map);

                            PlunderBeacons[zone].Add(beacon);
                            break;
                        }
                    }
                }
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(PlunderBeacons.Count);

            foreach (KeyValuePair<PlunderZone, List<PlunderBeaconAddon>> kvp in PlunderBeacons)
            {
                writer.Write((int)kvp.Key);
                writer.WriteItemList(kvp.Value);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            reader.ReadInt();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                PlunderBeacons[(PlunderZone)reader.ReadInt()] = reader.ReadStrongItemList<PlunderBeaconAddon>();
            }
        }
    }
}
