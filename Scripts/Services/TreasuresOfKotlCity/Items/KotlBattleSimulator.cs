using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;
using System.Linq;
using Server.Items;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class KotlBattleSimulator : Item, ISpawner
    {
        public static KotlBattleSimulator Instance { get; set; }

        public static int Levels = 7;
        public static Rectangle2D SpawnBounds = new Rectangle2D(512, 2280, 63, 55);
        public static Type[] SpawnTypes = new Type[] { typeof(SpectralKotlWarrior), typeof(SpectralMyrmidexWarrior) };

        public static TimeSpan NextSpawnDuration { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(2, 25)); } }

        private bool _Active;
        private bool _Spawning;

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
        public int SpawnCount { get { return Spawn != null ? Spawn.Count : 0; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Kills { get; set; }

        public List<ISpawnable> Spawn { get; set; }

        public bool UnlinkOnTaming { get { return true; } }
        public Point3D HomeLocation { get { return new Point3D(544, 2303, 0); } }
        public int HomeRange { get { return 30; } }

        public int SpawnPerWave { get { return 10 + (Level * 2); } }
        public int KillsPerWave { get { return 20 + (Level * 2); } }

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

            for (int i = 0; i < SpawnPerWave; i++)
            {
                Timer.DelayCall(NextSpawnDuration, DoSpawn);
            }
        }

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
            if (WheelsOfTime.RockBarrier == null || WheelsOfTime.RockBarrier.Deleted)
            {
                WheelsOfTime.RockBarrier = new KotlWallAddon();
            }

            WheelsOfTime.RockBarrier.MoveToWorld(WheelsOfTime.RockBarrierLocation, Map.TerMur);

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
                loc = SpawnBounds.GetRandomSpawnPoint(Map.TerMur); 
            }
            while (!Map.TerMur.CanSpawnMobile(loc));

            BaseCreature bc = Activator.CreateInstance(SpawnTypes[Utility.Random(SpawnTypes.Length)]) as BaseCreature;

            if (bc != null)
            {
                bc.Spawner = this;
                bc.MoveToWorld(loc, Map.TerMur);

                if (Spawn == null)
                    Spawn = new List<ISpawnable>();

                Spawn.Add(bc);

                bc.Home = HomeLocation;
                bc.RangeHome = HomeRange;
            }
        }

        public KotlBattleSimulator(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

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
        }
    }
}