using Server.Commands;
using Server.ContextMenus;
using Server.Mobiles;
using System;
using System.Collections;
using System.Collections.Generic;

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
            m_ID = id;
            m_Region = region;
            m_Home = home;
            m_Range = range;
            m_Direction = direction;
            m_Definition = definition;
            m_SpawnedObjects = new List<ISpawnable>();
            m_Max = max;
            m_MinSpawnTime = minSpawnTime;
            m_MaxSpawnTime = maxSpawnTime;
            m_Running = false;

            if (m_Table.Contains(id))
                Console.WriteLine("Warning: double SpawnEntry ID '{0}'", id);
            else
                m_Table[id] = this;
        }

        public static Hashtable Table => m_Table;
        // When a creature's AI is deactivated (PlayerRangeSensitive optimization) does it return home?
        public bool ReturnOnDeactivate => true;
        // Are creatures unlinked on taming (true) or should they also go out of the region (false)?
        public bool UnlinkOnTaming => false;
        // Are unlinked and untamed creatures removed after 20 hours?
        public bool RemoveIfUntamed => true;
        public int ID => m_ID;
        public BaseRegion Region => m_Region;
        public Point3D HomeLocation => m_Home;
        public int HomeRange => m_Range;
        public Direction Direction => m_Direction;
        public SpawnDefinition Definition => m_Definition;
        public List<ISpawnable> SpawnedObjects => m_SpawnedObjects;
        public int Max => m_Max;
        public TimeSpan MinSpawnTime => m_MinSpawnTime;
        public TimeSpan MaxSpawnTime => m_MaxSpawnTime;
        public bool Running => m_Running;
        public bool Complete => m_SpawnedObjects.Count >= m_Max;
        public bool Spawning => m_Running && !Complete;

        public virtual void GetSpawnProperties(ISpawnable spawn, ObjectPropertyList list)
        { }

        public virtual void GetSpawnContextEntries(ISpawnable spawn, Mobile m, List<ContextMenuEntry> list)
        { }

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

            CommandSystem.Register("RespawnAllRegions", AccessLevel.Administrator, RespawnAllRegions_OnCommand);
            CommandSystem.Register("RespawnRegion", AccessLevel.GameMaster, RespawnRegion_OnCommand);
            CommandSystem.Register("DelAllRegionSpawns", AccessLevel.Administrator, DelAllRegionSpawns_OnCommand);
            CommandSystem.Register("DelRegionSpawns", AccessLevel.GameMaster, DelRegionSpawns_OnCommand);
            CommandSystem.Register("StartAllRegionSpawns", AccessLevel.Administrator, StartAllRegionSpawns_OnCommand);
            CommandSystem.Register("StartRegionSpawns", AccessLevel.GameMaster, StartRegionSpawns_OnCommand);
            CommandSystem.Register("StopAllRegionSpawns", AccessLevel.Administrator, StopAllRegionSpawns_OnCommand);
            CommandSystem.Register("StopRegionSpawns", AccessLevel.GameMaster, StopRegionSpawns_OnCommand);
        }

        public Point3D RandomSpawnLocation(int spawnHeight, bool land, bool water)
        {
            return m_Region.RandomSpawnLocation(spawnHeight, land, water, m_Home, m_Range);
        }

        public void Start()
        {
            if (m_Running)
                return;

            m_Running = true;
            CheckTimer();
        }

        public void Stop()
        {
            if (!m_Running)
                return;

            m_Running = false;
            CheckTimer();
        }

        public void DeleteSpawnedObjects()
        {
            InternalDeleteSpawnedObjects();

            m_Running = false;
            CheckTimer();
        }

        public void Respawn()
        {
            InternalDeleteSpawnedObjects();

            for (int i = 0; !Complete && i < m_Max; i++)
                Spawn();

            m_Running = true;
            CheckTimer();
        }

        public void Delete()
        {
            m_Max = 0;
            InternalDeleteSpawnedObjects();

            if (m_SpawnTimer != null)
            {
                m_SpawnTimer.Stop();
                m_SpawnTimer = null;
            }

            if (m_Table[m_ID] == this)
                m_Table.Remove(m_ID);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(m_SpawnedObjects.Count);

            for (int i = 0; i < m_SpawnedObjects.Count; i++)
            {
                ISpawnable spawn = m_SpawnedObjects[i];

                int serial = spawn.Serial;

                writer.Write(serial);
            }

            writer.Write(m_Running);

            if (m_SpawnTimer != null)
            {
                writer.Write(true);
                writer.WriteDeltaTime(m_NextSpawn);
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
                    Add(spawnableEntity);
            }

            m_Running = reader.ReadBool();

            if (reader.ReadBool())
            {
                m_NextSpawn = reader.ReadDeltaTime();

                if (Spawning)
                {
                    if (m_SpawnTimer != null)
                        m_SpawnTimer.Stop();

                    TimeSpan delay = m_NextSpawn - DateTime.UtcNow;
                    m_SpawnTimer = Timer.DelayCall(delay > TimeSpan.Zero ? delay : TimeSpan.Zero, TimerCallback);
                }
            }

            CheckTimer();
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
            ISpawnable spawn = m_Definition.Spawn(this);

            if (spawn != null)
                Add(spawn);
        }

        private void Add(ISpawnable spawn)
        {
            m_SpawnedObjects.Add(spawn);

            spawn.Spawner = this;

            if (spawn is BaseCreature)
                ((BaseCreature)spawn).RemoveIfUntamed = RemoveIfUntamed;
        }

        void ISpawner.Remove(ISpawnable spawn)
        {
            m_SpawnedObjects.Remove(spawn);

            CheckTimer();
        }

        private TimeSpan RandomTime()
        {
            int min = (int)m_MinSpawnTime.TotalSeconds;
            int max = (int)m_MaxSpawnTime.TotalSeconds;

            int rand = Utility.RandomMinMax(min, max);
            return TimeSpan.FromSeconds(rand);
        }

        private void CheckTimer()
        {
            if (Spawning)
            {
                if (m_SpawnTimer == null)
                {
                    TimeSpan time = RandomTime();
                    m_SpawnTimer = Timer.DelayCall(time, TimerCallback);
                    m_NextSpawn = DateTime.UtcNow + time;
                }
            }
            else if (m_SpawnTimer != null)
            {
                m_SpawnTimer.Stop();
                m_SpawnTimer = null;
            }
        }

        private void TimerCallback()
        {
            int amount = Math.Max((m_Max - m_SpawnedObjects.Count) / 3, 1);

            for (int i = 0; i < amount; i++)
                Spawn();

            m_SpawnTimer = null;
            CheckTimer();
        }

        private void InternalDeleteSpawnedObjects()
        {
            foreach (ISpawnable spawnable in m_SpawnedObjects)
            {
                spawnable.Spawner = null;

                bool uncontrolled = !(spawnable is BaseCreature) || !((BaseCreature)spawnable).Controlled;

                if (uncontrolled)
                    spawnable.Delete();
            }

            m_SpawnedObjects.Clear();
        }
    }
}
