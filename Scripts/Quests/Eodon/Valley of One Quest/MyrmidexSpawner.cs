using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class MyrmidexHill : Item
    {
        private readonly Type[] _SpawnList =
        {
            typeof(MyrmidexLarvae), typeof(MyrmidexDrone), typeof(MyrmidexWarrior)
        };

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextSpawn { get; set; }

        public EodonTribeRegion Zone { get; set; }

        public List<BaseCreature> Spawn { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Focus { get; set; }

        public int SpawnCount => Utility.RandomMinMax(6, 9);

        public int HasSpawned { get; set; }

        public MyrmidexHill(EodonTribeRegion zone, Mobile focus)
            : base(8754)
        {
            Movable = false;

            Focus = focus;
            Zone = zone;
            Spawn = new List<BaseCreature>();
        }

        public override bool HandlesOnMovement => NextSpawn < DateTime.UtcNow;
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.InRange(Location, 7) && m.AccessLevel == AccessLevel.Player &&
                (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)))
            {
                Focus = m;
                DoSpawn();
            }
        }

        public void DoSpawn()
        {
            Map map = Map;

            if (Spawn == null)
                return;

            ColUtility.ForEach(Spawn.Where(bc => bc == null || !bc.Alive || bc.Deleted), bc => Spawn.Remove(bc));

            if (map != null && map != Map.Internal && !Deleted && Spawn.Count == 0 && HasSpawned < 3)
            {
                HasSpawned++;
                NextSpawn = DateTime.UtcNow + TimeSpan.FromMinutes(Utility.RandomMinMax(2, 5));

                int time = 333;
                for (int i = 0; i < SpawnCount - Spawn.Count; i++)
                {
                    Timer.DelayCall(TimeSpan.FromMilliseconds(time), () =>
                    {
                        Point3D p = Location;

                        for (int j = 0; j < 25; j++)
                        {
                            int x = Utility.RandomMinMax(X - 3, X + 3);
                            int y = Utility.RandomMinMax(Y - 3, Y + 3);
                            int z = map.GetAverageZ(x, y);

                            if (map.CanSpawnMobile(x, y, z) && InLOS(new Point3D(x, y, z)))
                            {
                                p = new Point3D(x, y, z);
                                break;
                            }
                        }

                        BaseCreature bc = Activator.CreateInstance(_SpawnList[Utility.Random(_SpawnList.Length)]) as BaseCreature;

                        if (bc != null)
                        {
                            Spawn.Add(bc);
                            bc.MoveToWorld(p, map);

                            Timer.DelayCall(creature => creature.Combatant = Focus, bc);
                        }
                    });

                    time += 333;
                }
            }
        }

        public void CheckSpawn()
        {
            if (Spawn == null)
                Delete();
            else
            {
                int count = 0;
                ColUtility.ForEach(Spawn.Where(bc => bc != null && bc.Alive), bc => count++);

                if (count == 0)
                    Delete();
            }

        }

        public override void Delete()
        {
            base.Delete();

            if (Spawn != null)
            {
                ColUtility.Free(Spawn);
                Spawn = null;
            }
        }

        public MyrmidexHill(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(HasSpawned);

            writer.Write(Spawn == null ? 0 : Spawn.Count);

            if (Spawn != null)
            {
                Spawn.ForEach(bc => writer.Write(bc));
            }

            Timer.DelayCall(TimeSpan.FromMinutes(1), CheckSpawn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    HasSpawned = reader.ReadInt();
                    goto case 0;
                case 0:
                    int count = reader.ReadInt();
                    if (count > 0)
                    {
                        Spawn = new List<BaseCreature>();

                        for (int i = 0; i < count; i++)
                        {
                            BaseCreature bc = reader.ReadMobile() as BaseCreature;

                            if (bc != null)
                                Spawn.Add(bc);
                        }
                    }

                    break;
            }

            if (Spawn == null || Spawn.Count == 0)
                Delete();
            else
            {
                Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                {
                    EodonTribeRegion r = Region.Find(Location, Map) as EodonTribeRegion;

                    if (r != null)
                        Zone = r;
                });
            }
        }
    }

    public class EodonTribeRegion : Region
    {
        public static void Initialize()
        {
            _Zones[0] = new EodonTribeRegion(EodonTribe.Jukari, new Rectangle2D[] { new Rectangle2D(640, 2046, 115, 115) }, 6);
            _Zones[1] = new EodonTribeRegion(EodonTribe.Kurak, new Rectangle2D[] { new Rectangle2D(291, 1817, 125, 90) }, 6);
            _Zones[2] = new EodonTribeRegion(EodonTribe.Barrab, new Rectangle2D[] { new Rectangle2D(134, 1767, 33, 20), new Rectangle2D(142, 1786, 57, 80), new Rectangle2D(145, 1750, 20, 20) }, 5);
            _Zones[3] = new EodonTribeRegion(EodonTribe.Barako, new Rectangle2D[] { new Rectangle2D(620, 1677, 95, 100) }, 5);
            _Zones[4] = new EodonTribeRegion(EodonTribe.Urali, new Rectangle2D[] { new Rectangle2D(320, 1551, 160, 72) }, 5);
            _Zones[5] = new EodonTribeRegion(EodonTribe.Sakkhra, new Rectangle2D[] { new Rectangle2D(482, 1375, 200, 200) }, 8);
        }

        public static EodonTribeRegion[] _Zones = new EodonTribeRegion[6];

        public int MaxSpawns { get; }
        public EodonTribe Tribe { get; set; }
        public int Spawns => GetItemCount(i => i is MyrmidexHill);

        public EodonTribeRegion(EodonTribe tribe, Rectangle2D[] rec, int maxSpawns)
            : base(tribe + " tribe", Map.TerMur, DefaultPriority, rec)
        {
            Tribe = tribe;
            Register();

            MaxSpawns = maxSpawns;
        }

        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            if (Tribe != EodonTribe.Barrab && Spawns < MaxSpawns)
            {
                double chance = Utility.RandomDouble();

                if (0.005 > chance && (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)) && m.AccessLevel == AccessLevel.Player)
                {
                    MyrmidexHill hill = new MyrmidexHill(this, m);
                    Point3D p = m.Location;

                    for (int i = 0; i < 10; i++)
                    {
                        int x = Utility.RandomMinMax(p.X - 5, p.X + 5);
                        int y = Utility.RandomMinMax(p.Y - 5, p.Y + 5);
                        int z = Map.GetAverageZ(x, y);

                        if (Map.CanFit(x, y, z, 16, false, false, true))
                        {
                            p = new Point3D(x, y, z);
                            break;
                        }
                    }

                    hill.MoveToWorld(p, Map);
                    hill.DoSpawn();
                }
            }
        }
    }
}
