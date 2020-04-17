using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class KotlBattleSimulator : Item, ISpawner
    {
        public static KotlBattleSimulator Instance { get; set; }

        public static int Levels = 7;
        public static Rectangle2D SpawnBounds = new Rectangle2D(500, 2272, 87, 71);
        public static Type[] SpawnTypes = new Type[] { typeof(SpectralKotlWarrior), typeof(SpectralMyrmidexWarrior) };

        public static TimeSpan BattleDuration = TimeSpan.FromMinutes(60);
        public static TimeSpan NextSpawnDuration => TimeSpan.FromSeconds(Utility.RandomMinMax(2, 25));

        private bool _Active;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return _Active; }
            set
            {
                bool active = _Active;
                _Active = value;

                if (!active && _Active)
                {
                    StartSpawn();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnCount => Spawn != null ? Spawn.Count : 0;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Kills { get; set; }

        public List<ISpawnable> Spawn { get; set; }

        public bool UnlinkOnTaming => true;
        public Point3D HomeLocation => new Point3D(544, 2303, 0);
        public int HomeRange => 30;

        public int SpawnPerWave => 10 + (Level * 2);
        public int KillsPerWave => 20 + (Level * 2);

        public Timer Timer { get; set; }

        public KotlBattleSimulator()
            : base(40147)
        {
            Visible = false;
            Movable = false;

            Instance = this;
        }

        private void StartSpawn()
        {
            Kills = 0;
            Level = 0;

            StartTimer();

            for (int i = 0; i < SpawnPerWave; i++)
            {
                Timer.DelayCall(NextSpawnDuration, DoSpawn);
            }
        }

        public void StartTimer()
        {
            EndTimer();

            Timer = Timer.DelayCall(BattleDuration, EndSimulation);
            Timer.Start();
        }

        public void EndTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }

        public virtual void GetSpawnProperties(ISpawnable spawn, ObjectPropertyList list)
        { }

        public virtual void GetSpawnContextEntries(ISpawnable spawn, Mobile user, List<ContextMenuEntry> list)
        { }

        public void Remove(ISpawnable spawn)
        {
            if (Spawn != null && Spawn.Contains(spawn))
            {
                Kills++;
                Spawn.Remove(spawn);

                if (Kills >= KillsPerWave)
                {
                    if (Level >= Levels)
                    {
                        EndSimulation();
                        return;
                    }
                    else
                    {
                        IncreaseLevel();
                    }
                }

                Timer.DelayCall(NextSpawnDuration, DoSpawn);
            }
        }

        private void IncreaseLevel()
        {
            int oldMaxCount = SpawnPerWave;

            Level++;
            Kills = 0;

            int inc = SpawnPerWave - oldMaxCount;

            if (inc > 0)
            {
                for (int i = 0; i < inc; i++)
                {
                    Timer.DelayCall(NextSpawnDuration, DoSpawn);
                }
            }
        }

        public void EndSimulation()
        {
            EndTimer();

            if (WheelsOfTime.Instance != null && (WheelsOfTime.Instance.RockBarrier == null || WheelsOfTime.Instance.RockBarrier.Deleted))
            {
                WheelsOfTime.Instance.RockBarrier = new KotlWallAddon();
            }

            WheelsOfTime.Instance.RockBarrier.MoveToWorld(WheelsOfTime.RockBarrierLocation, Map.TerMur);

            if (WheelsOfTime.Instance != null)
            {
                WheelsOfTime.Instance.TimeWarpEnds = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            }

            PowerCoreDockingStation.Stations.ForEach(station =>
            {
                station.Deactivate();
            });

            Active = false;
            Level = 0;
            Kills = 0;

            // What about spawn?
            ColUtility.Free(Spawn);
        }

        private void DoSpawn()
        {
            if (!_Active)
                return;

            Point3D loc;

            do
            {
                loc = Map.TerMur.GetRandomSpawnPoint(SpawnBounds);
            }
            while (!Map.TerMur.CanSpawnMobile(loc));

            bool strong = Map.Tiles.GetStaticTiles(loc.X, loc.Y, true).Length > 0;
            BaseCreature bc;

            switch (Utility.Random(2))
            {
                default:
                case 0: bc = new SpectralKotlWarrior(strong); break;
                case 1: bc = new SpectralMyrmidexWarrior(strong); break;
            }

            if (bc != null)
            {
                bc.Spawner = this;

                bc.Home = HomeLocation;
                bc.RangeHome = HomeRange;

                bc.OnBeforeSpawn(loc, Map.TerMur);
                bc.MoveToWorld(loc, Map.TerMur);
                bc.OnAfterSpawn();

                if (Spawn == null)
                    Spawn = new List<ISpawnable>();

                Spawn.Add(bc);
            }
        }

        public KotlBattleSimulator(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // Version

            writer.Write(_Active);
            writer.Write(Level);
            writer.Write(Kills);

            if (Spawn != null)
            {
                writer.Write(Spawn.Count);
                Spawn.ForEach(sp => writer.Write(sp as Mobile));
            }
            else
            {
                writer.Write(0);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Instance = this;

            _Active = reader.ReadBool();
            Level = reader.ReadInt();
            Kills = reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                BaseCreature bc = reader.ReadMobile() as BaseCreature;

                if (bc != null)
                {
                    bc.Spawner = this;
                    bc.Home = HomeLocation;
                    bc.RangeHome = HomeRange;

                    if (Spawn == null)
                        Spawn = new List<ISpawnable>();

                    Spawn.Add(bc);
                }
            }

            if (_Active)
            {
                StartTimer();

                if (Spawn == null || Spawn.Count == 0)
                {
                    if (Spawn == null)
                        Spawn = new List<ISpawnable>();

                    if (Level >= Levels)
                    {
                        EndSimulation();
                        return;
                    }
                    else
                    {
                        IncreaseLevel();
                    }
                }
                else
                {
                    int toSpawn = Math.Max(0, SpawnPerWave - (Spawn.Count + Kills));

                    if (toSpawn > 0)
                    {
                        for (int i = 0; i < toSpawn; i++)
                        {
                            Timer.DelayCall(NextSpawnDuration, DoSpawn);
                        }
                    }
                }
            }

            // Teleporter Fix
            if (version == 0)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(20), () =>
                    {
                        if (Map != null)
                        {
                            IPooledEnumerable eable = Map.GetItemsInRange(new Point3D(644, 2308, 0), 0);

                            foreach (Item i in eable)
                            {
                                if (i is Teleporter)
                                {
                                    ((Teleporter)i).PointDest = new Point3D(543, 2479, 2);
                                }
                            }
                        }
                    });
            }
        }
    }
}
