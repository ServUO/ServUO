using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;

namespace Server.Regions
{
    public class SpawnEntry : ISpawner
    {
        public static readonly TimeSpan DefaultMinSpawnTime = TimeSpan.FromMinutes(2.0);
        public static readonly TimeSpan DefaultMaxSpawnTime = TimeSpan.FromMinutes(5.0);
        public static readonly Direction InvalidDirection = Direction.Running;
        private static readonly Hashtable m_Table = new Hashtable();
        private static List<IEntity> m_RemoveList;
        private readonly int m_ID;
        private readonly BaseRegion m_Region;
        private readonly Point3D m_Home;
        private readonly int m_Range;
        private readonly Direction m_Direction;
        private readonly SpawnDefinition m_Definition;
        private readonly List<ISpawnable> m_SpawnedObjects;
        private readonly TimeSpan m_MinSpawnTime;
        private readonly TimeSpan m_MaxSpawnTime;
        private int m_Max;
        private bool m_Running;
        private DateTime m_NextSpawn;
        private Timer m_SpawnTimer;
        public SpawnEntry(int id, BaseRegion region, Point3D home, int range, Direction direction, SpawnDefinition definition, int max, TimeSpan minSpawnTime, TimeSpan maxSpawnTime)
        {
            this.m_ID = id;
            this.m_Region = region;
            this.m_Home = home;
            this.m_Range = range;
            this.m_Direction = direction;
            this.m_Definition = definition;
            this.m_SpawnedObjects = new List<ISpawnable>();
            this.m_Max = max;
            this.m_MinSpawnTime = minSpawnTime;
            this.m_MaxSpawnTime = maxSpawnTime;
            this.m_Running = false;

            if (m_Table.Contains(id))
                Console.WriteLine("Warning: double SpawnEntry ID '{0}'", id);
            else
                m_Table[id] = this;
        }

        public static Hashtable Table
        {
            get
            {
                return m_Table;
            }
        }
        // When a creature's AI is deactivated (PlayerRangeSensitive optimization) does it return home?
        public bool ReturnOnDeactivate
        {
            get
            {
                return true;
            }
        }
        // Are creatures unlinked on taming (true) or should they also go out of the region (false)?
        public bool UnlinkOnTaming
        {
            get
            {
                return false;
            }
        }
        // Are unlinked and untamed creatures removed after 20 hours?
        public bool RemoveIfUntamed
        {
            get
            {
                return true;
            }
        }
        public int ID
        {
            get
            {
                return this.m_ID;
            }
        }
        public BaseRegion Region
        {
            get
            {
                return this.m_Region;
            }
        }
        public Point3D HomeLocation
        {
            get
            {
                return this.m_Home;
            }
        }
        public int HomeRange
        {
            get
            {
                return this.m_Range;
            }
        }
        public Direction Direction
        {
            get
            {
                return this.m_Direction;
            }
        }
        public SpawnDefinition Definition
        {
            get
            {
                return this.m_Definition;
            }
        }
        public List<ISpawnable> SpawnedObjects
        {
            get
            {
                return this.m_SpawnedObjects;
            }
        }
        public int Max
        {
            get
            {
                return this.m_Max;
            }
        }
        public TimeSpan MinSpawnTime
        {
            get
            {
                return this.m_MinSpawnTime;
            }
        }
        public TimeSpan MaxSpawnTime
        {
            get
            {
                return this.m_MaxSpawnTime;
            }
        }
        public bool Running
        {
            get
            {
                return this.m_Running;
            }
        }
        public bool Complete
        {
            get
            {
                return this.m_SpawnedObjects.Count >= this.m_Max;
            }
        }
        public bool Spawning
        {
            get
            {
                return this.m_Running && !this.Complete;
            }
        }
        public static void Remove(GenericReader reader, int version)
        {
            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                int serial = reader.ReadInt();
                IEntity entity = World.FindEntity(serial);

                if (entity != null)
                {
                    if (m_RemoveList == null)
                        m_RemoveList = new List<IEntity>();

                    m_RemoveList.Add(entity);
                }
            }

            reader.ReadBool(); // m_Running

            if (reader.ReadBool())
                reader.ReadDeltaTime(); // m_NextSpawn
        }

        public static void Initialize()
        {
            if (m_RemoveList != null)
            {
                foreach (IEntity ent in m_RemoveList)
                {
                    ent.Delete();
                }

                m_RemoveList = null;
            }

            SpawnPersistence.EnsureExistence();

            CommandSystem.Register("RespawnAllRegions", AccessLevel.Administrator, new CommandEventHandler(RespawnAllRegions_OnCommand));
            CommandSystem.Register("RespawnRegion", AccessLevel.GameMaster, new CommandEventHandler(RespawnRegion_OnCommand));
            CommandSystem.Register("DelAllRegionSpawns", AccessLevel.Administrator, new CommandEventHandler(DelAllRegionSpawns_OnCommand));
            CommandSystem.Register("DelRegionSpawns", AccessLevel.GameMaster, new CommandEventHandler(DelRegionSpawns_OnCommand));
            CommandSystem.Register("StartAllRegionSpawns", AccessLevel.Administrator, new CommandEventHandler(StartAllRegionSpawns_OnCommand));
            CommandSystem.Register("StartRegionSpawns", AccessLevel.GameMaster, new CommandEventHandler(StartRegionSpawns_OnCommand));
            CommandSystem.Register("StopAllRegionSpawns", AccessLevel.Administrator, new CommandEventHandler(StopAllRegionSpawns_OnCommand));
            CommandSystem.Register("StopRegionSpawns", AccessLevel.GameMaster, new CommandEventHandler(StopRegionSpawns_OnCommand));
        }

        public Point3D RandomSpawnLocation(int spawnHeight, bool land, bool water)
        {
            return this.m_Region.RandomSpawnLocation(spawnHeight, land, water, this.m_Home, this.m_Range);
        }

        public void Start()
        {
            if (this.m_Running)
                return;

            this.m_Running = true;
            this.CheckTimer();
        }

        public void Stop()
        {
            if (!this.m_Running)
                return;

            this.m_Running = false;
            this.CheckTimer();
        }

        public void DeleteSpawnedObjects()
        {
            this.InternalDeleteSpawnedObjects();

            this.m_Running = false;
            this.CheckTimer();
        }

        public void Respawn()
        {
            this.InternalDeleteSpawnedObjects();

            for (int i = 0; !this.Complete && i < this.m_Max; i++)
                this.Spawn();

            this.m_Running = true;
            this.CheckTimer();
        }

        public void Delete()
        {
            this.m_Max = 0;
            this.InternalDeleteSpawnedObjects();

            if (this.m_SpawnTimer != null)
            {
                this.m_SpawnTimer.Stop();
                this.m_SpawnTimer = null;
            }

            if (m_Table[this.m_ID] == this)
                m_Table.Remove(this.m_ID);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)this.m_SpawnedObjects.Count);

            for (int i = 0; i < this.m_SpawnedObjects.Count; i++)
            {
                ISpawnable spawn = this.m_SpawnedObjects[i];

                int serial = spawn.Serial;

                writer.Write((int)serial);
            }

            writer.Write((bool)this.m_Running);

            if (this.m_SpawnTimer != null)
            {
                writer.Write(true);
                writer.WriteDeltaTime((DateTime)this.m_NextSpawn);
            }
            else
            {
                writer.Write(false);
            }
        }

        public void Deserialize(GenericReader reader, int version)
        {
            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                int serial = reader.ReadInt();
                ISpawnable spawnableEntity = World.FindEntity(serial) as ISpawnable;

                if (spawnableEntity != null)
                    this.Add(spawnableEntity);
            }

            this.m_Running = reader.ReadBool();

            if (reader.ReadBool())
            {
                this.m_NextSpawn = reader.ReadDeltaTime();

                if (this.Spawning)
                {
                    if (this.m_SpawnTimer != null)
                        this.m_SpawnTimer.Stop();

                    TimeSpan delay = this.m_NextSpawn - DateTime.UtcNow;
                    this.m_SpawnTimer = Timer.DelayCall(delay > TimeSpan.Zero ? delay : TimeSpan.Zero, new TimerCallback(TimerCallback)); 
                }
            }

            this.CheckTimer();
        }

        private static BaseRegion GetCommandData(CommandEventArgs args)
        {
            Mobile from = args.Mobile;

            Region reg;
            if (args.Length == 0)
            {
                reg = from.Region;
            }
            else
            {
                string name = args.GetString(0);
                //reg = (Region) from.Map.Regions[name];

                if (!from.Map.Regions.TryGetValue(name, out reg))
                {
                    from.SendMessage("Could not find region '{0}'.", name);
                    return null;
                }
            }

            BaseRegion br = reg as BaseRegion;

            if (br == null || br.Spawns == null)
            {
                from.SendMessage("There are no spawners in region '{0}'.", reg);
                return null;
            }

            return br;
        }

        [Usage("RespawnAllRegions")]
        [Description("Respawns all regions and sets the spawners as running.")]
        private static void RespawnAllRegions_OnCommand(CommandEventArgs args)
        {
            foreach (SpawnEntry entry in m_Table.Values)
            {
                entry.Respawn();
            }

            args.Mobile.SendMessage("All regions have respawned.");
        }

        [Usage("RespawnRegion [<region name>]")]
        [Description("Respawns the region in which you are (or that you provided) and sets the spawners as running.")]
        private static void RespawnRegion_OnCommand(CommandEventArgs args)
        {
            BaseRegion region = GetCommandData(args);

            if (region == null)
                return;

            for (int i = 0; i < region.Spawns.Length; i++)
                region.Spawns[i].Respawn();

            args.Mobile.SendMessage("Region '{0}' has respawned.", region);
        }

        [Usage("DelAllRegionSpawns")]
        [Description("Deletes all spawned objects of every regions and sets the spawners as not running.")]
        private static void DelAllRegionSpawns_OnCommand(CommandEventArgs args)
        {
            foreach (SpawnEntry entry in m_Table.Values)
            {
                entry.DeleteSpawnedObjects();
            }

            args.Mobile.SendMessage("All region spawned objects have been deleted.");
        }

        [Usage("DelRegionSpawns [<region name>]")]
        [Description("Deletes all spawned objects of the region in which you are (or that you provided) and sets the spawners as not running.")]
        private static void DelRegionSpawns_OnCommand(CommandEventArgs args)
        {
            BaseRegion region = GetCommandData(args);

            if (region == null)
                return;

            for (int i = 0; i < region.Spawns.Length; i++)
                region.Spawns[i].DeleteSpawnedObjects();

            args.Mobile.SendMessage("Spawned objects of region '{0}' have been deleted.", region);
        }

        [Usage("StartAllRegionSpawns")]
        [Description("Sets the region spawners of all regions as running.")]
        private static void StartAllRegionSpawns_OnCommand(CommandEventArgs args)
        {
            foreach (SpawnEntry entry in m_Table.Values)
            {
                entry.Start();
            }

            args.Mobile.SendMessage("All region spawners have started.");
        }

        [Usage("StartRegionSpawns [<region name>]")]
        [Description("Sets the region spawners of the region in which you are (or that you provided) as running.")]
        private static void StartRegionSpawns_OnCommand(CommandEventArgs args)
        {
            BaseRegion region = GetCommandData(args);

            if (region == null)
                return;

            for (int i = 0; i < region.Spawns.Length; i++)
                region.Spawns[i].Start();

            args.Mobile.SendMessage("Spawners of region '{0}' have started.", region);
        }

        [Usage("StopAllRegionSpawns")]
        [Description("Sets the region spawners of all regions as not running.")]
        private static void StopAllRegionSpawns_OnCommand(CommandEventArgs args)
        {
            foreach (SpawnEntry entry in m_Table.Values)
            {
                entry.Stop();
            }

            args.Mobile.SendMessage("All region spawners have stopped.");
        }

        [Usage("StopRegionSpawns [<region name>]")]
        [Description("Sets the region spawners of the region in which you are (or that you provided) as not running.")]
        private static void StopRegionSpawns_OnCommand(CommandEventArgs args)
        {
            BaseRegion region = GetCommandData(args);

            if (region == null)
                return;

            for (int i = 0; i < region.Spawns.Length; i++)
                region.Spawns[i].Stop();

            args.Mobile.SendMessage("Spawners of region '{0}' have stopped.", region);
        }

        private void Spawn()
        {
            ISpawnable spawn = this.m_Definition.Spawn(this);

            if (spawn != null)
                this.Add(spawn);
        }

        private void Add(ISpawnable spawn)
        {
            this.m_SpawnedObjects.Add(spawn);

            spawn.Spawner = this;

            if (spawn is BaseCreature)
                ((BaseCreature)spawn).RemoveIfUntamed = this.RemoveIfUntamed;
        }

        void ISpawner.Remove(ISpawnable spawn)
        {
            this.m_SpawnedObjects.Remove(spawn);

            this.CheckTimer();
        }

        private TimeSpan RandomTime()
        {
            int min = (int)this.m_MinSpawnTime.TotalSeconds;
            int max = (int)this.m_MaxSpawnTime.TotalSeconds;

            int rand = Utility.RandomMinMax(min, max);
            return TimeSpan.FromSeconds(rand);
        }

        private void CheckTimer()
        {
            if (this.Spawning)
            {
                if (this.m_SpawnTimer == null)
                {
                    TimeSpan time = this.RandomTime();
                    this.m_SpawnTimer = Timer.DelayCall(time, new TimerCallback(TimerCallback));
                    this.m_NextSpawn = DateTime.UtcNow + time;
                }
            }
            else if (this.m_SpawnTimer != null)
            {
                this.m_SpawnTimer.Stop();
                this.m_SpawnTimer = null;
            }
        }

        private void TimerCallback()
        {
            int amount = Math.Max((this.m_Max - this.m_SpawnedObjects.Count) / 3, 1);

            for (int i = 0; i < amount; i++)
                this.Spawn();

            this.m_SpawnTimer = null;
            this.CheckTimer();
        }

        private void InternalDeleteSpawnedObjects()
        {
            foreach (ISpawnable spawnable in this.m_SpawnedObjects)
            {
                spawnable.Spawner = null;

                bool uncontrolled = !(spawnable is BaseCreature) || !((BaseCreature)spawnable).Controlled;

                if (uncontrolled)
                    spawnable.Delete();
            }

            this.m_SpawnedObjects.Clear();
        }
    }
}