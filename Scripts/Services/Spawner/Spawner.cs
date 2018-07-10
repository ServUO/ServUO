using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using Server.Commands;
using Server.Items;
using CPA = Server.CommandPropertyAttribute;
using Server.Gumps;

namespace Server.Mobiles
{
    public class Spawner : Item, ISpawner
    {
        private static WarnTimer m_WarnTimer;
        private int m_Team;
        private int m_HomeRange;
        private int m_SpawnRange;
        private int m_WalkingRange;
        private TimeSpan m_MinDelay;
        private TimeSpan m_MaxDelay;
        private DateTime m_End;
        private InternalTimer m_Timer;
        private bool m_Running;
        private bool m_Group;
        private WayPoint m_WayPoint;

        private List<SpawnObject> m_SpawnObjects;
        private int m_MaxCount;

        [Constructable]
        public Spawner()
            : this(null)
        {
        }

        [Constructable]
        public Spawner(string spawnName)
            : this(1, 5, 10, 0, 4, spawnName)
        {
        }

        [Constructable]
        public Spawner(int amount, int minDelay, int maxDelay, int team, int spawnRange, string spawnName)
            : base(0x1f13)
        {
            List<SpawnObject> objects = new List<SpawnObject>();

            if (!String.IsNullOrEmpty(spawnName))
                objects.Add(new SpawnObject(spawnName));

            InitSpawner(amount, TimeSpan.FromMinutes(minDelay), TimeSpan.FromMinutes(maxDelay), team, spawnRange, objects);
        }

        public Spawner(int amount, TimeSpan minDelay, TimeSpan maxDelay, int team, int spawnRange, List<string> spawnNames)
            : base(0x1f13)
        {
            List<SpawnObject> objects = new List<SpawnObject>();

            foreach (var name in spawnNames)
            {
                objects.Add(new SpawnObject(name));
            }

            InitSpawner(amount, minDelay, maxDelay, team, spawnRange, objects);
        }

        public Spawner(int amount, TimeSpan minDelay, TimeSpan maxDelay, int team, int spawnRange, List<SpawnObject> objects)
            : base(0x1f13)
        {
            InitSpawner(amount, minDelay, maxDelay, team, spawnRange, objects);
        }

        public Spawner(Serial serial)
            : base(serial)
        {
        }

        public bool IsFull { get { return (SpawnCount >= m_MaxCount); } }
        public bool IsEmpty { get { return (SpawnCount == 0); } }

        public List<SpawnObject> SpawnObjects
        {
            get { return m_SpawnObjects; }
            set
            {
                m_SpawnObjects = value;

                if (m_SpawnObjects.Count < 1)
                    Stop();

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.Spawner)]
        public int MaxCount
        {
            get { return m_MaxCount; }
            set
            {
                m_MaxCount = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.Spawner)]
        public virtual int SpawnObjectCount {  get{ return m_SpawnObjects.Count; } }

        [CommandProperty(AccessLevel.Spawner)]
        public WayPoint WayPoint { get { return m_WayPoint; } set { m_WayPoint = value; } }

        [CommandProperty(AccessLevel.Spawner)]
        public bool Running
        {
            get { return m_Running; }
            set
            {
                if (value)
                    Start();
                else
                    Stop();

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.Spawner)]
        public int HomeRange { get { return m_HomeRange; } set { m_HomeRange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.Spawner)]
        public int SpawnRange { get { return m_SpawnRange; } set { m_SpawnRange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.Spawner)]
        public int WalkingRange { get { return m_WalkingRange; } set { m_WalkingRange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.Spawner)]
        public int Team { get { return m_Team;  } set { m_Team = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.Spawner)]
        public TimeSpan MinDelay 
        { 
            get { return m_MinDelay; } 
            set 
            {
                var old = m_MinDelay;

                m_MinDelay = value;
                
                if(old != m_MinDelay && m_Running)
                    DoTimer(); 
                
                InvalidateProperties();
            } 
        }

        [CommandProperty(AccessLevel.Spawner)]
        public TimeSpan MaxDelay
        { 
            get { return m_MaxDelay; } 
            set 
            {
                var old = m_MaxDelay;

                m_MaxDelay = value;

                if (old != m_MaxDelay && m_Running)
                    DoTimer(); 
                
                InvalidateProperties();
            }
        }

        public DateTime End { get { return m_End; } set { m_End = value; } }

        [CommandProperty(AccessLevel.Spawner)]
        public TimeSpan NextSpawn
        {
            get
            {
                if (m_Running)
                    return m_End - DateTime.UtcNow;
                else
                    return TimeSpan.FromSeconds(0);
            }
            set
            {
                Start();
                DoTimer(value);
            }
        }

        [CommandProperty(AccessLevel.Spawner)]
        public bool Group { get { return m_Group; } set { m_Group = value; InvalidateProperties(); } }

        public override string DefaultName { get { return "Spawner"; } }
        public Point3D HomeLocation { get { return Location; } }
        bool ISpawner.UnlinkOnTaming { get { return true; } }

        [CommandProperty(AccessLevel.Spawner)]
        public int SpawnCount
        {
            get
            {
                int count = 0;

                foreach (var so in m_SpawnObjects)
                {
                    count += GetSpawnCount(so);
                }

                return count;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool GuardImmune { get; set; }

        public override void OnAfterDuped(Item newItem)
        {
            Spawner s = newItem as Spawner;

            if (s == null)
                return;

            s.m_SpawnObjects = new List<SpawnObject>(m_SpawnObjects);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.Player || from.AccessLevel < AccessLevel.Spawner)
                return;

            SpawnerGump gump = BaseGump.GetGump<SpawnerGump>((PlayerMobile)from, g => g.Spawner == this);

            if (gump != null)
            {
                gump.Refresh();
            }
            else
            {
                BaseGump.SendGump(new SpawnerGump(from, this));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Running)
            {
                list.Add(1060742); // active

                list.Add(1060656, m_MaxCount.ToString()); // amount to make: ~1_val~
                list.Add(1061169, m_HomeRange.ToString()); // range ~1_val~
                list.Add(1060658, "walking range\t{0}", m_WalkingRange); // ~1_val~: ~2_val~

                list.Add(1060659, "group\t{0}", m_Group); // ~1_val~: ~2_val~
                list.Add(1060660, "team\t{0}", m_Team); // ~1_val~: ~2_val~
                list.Add(1060661, "speed\t{0} to {1}", m_MinDelay, m_MaxDelay); // ~1_val~: ~2_val~

                if (m_SpawnObjects.Count != 0)
                    list.Add(SpawnedStats());
            }
            else
            {
                list.Add(1060743); // inactive
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Running)
                LabelTo(from, "[Running]");
            else
                LabelTo(from, "[Off]");
        }

        public void Start()
        {
            if (!m_Running)
            {
                if (SpawnObjectCount > 0)
                {
                    m_Running = true;
                    DoTimer();
                }
            }
        }

        public void Stop()
        {
            if (m_Running)
            {
                if (m_Timer != null)
                    m_Timer.Stop();

                m_Running = false;
            }
        }

        public void Defrag()
        {
            bool removed = false;
            List<ISpawnable> toRemove = new List<ISpawnable>();

            for (int i = 0; i < m_SpawnObjects.Count; ++i)
            {
                foreach (var e in m_SpawnObjects[i].SpawnedObjects)
                {
                    bool remove = false;

                    if (e is Item)
                    {
                        Item item = (Item)e;

                        if (item.Deleted || item.Parent != null)
                            remove = true;
                    }
                    else if (e is Mobile)
                    {
                        Mobile m = (Mobile)e;

                        if (m.Deleted)
                        {
                            remove = true;
                        }
                        else if (m is BaseCreature)
                        {
                            BaseCreature bc = (BaseCreature)m;

                            if (bc.Controlled || bc.IsStabled)
                            {
                                remove = true;
                            }
                        }
                    }

                    if (remove)
                    {
                        toRemove.Add(e);
                        removed = true;
                    }
                }
            }

            if (removed)
            {
                foreach (var spawnable in toRemove)
                {
                    RemoveSpawn(spawnable);
                }

                InvalidateProperties();
            }

            ColUtility.Free(toRemove);
        }

        public IEnumerable<ISpawnable> GetSpawn()
        {
            if (m_SpawnObjects == null || m_SpawnObjects.Count == 0)
                yield break;

            foreach (var so in m_SpawnObjects)
            {
                foreach (var spawnable in so.SpawnedObjects)
                {
                    yield return spawnable;
                }
            }
        }

        public void RemoveSpawn(ISpawnable e)
        {
            SpawnObject so = m_SpawnObjects.FirstOrDefault(s => s.SpawnedObjects.Contains(e));

            if (so != null)
            {
                so.SpawnedObjects.Remove(e);
            }
        }

        public bool AddSpawnObject(SpawnObject so)
        {
            if(m_SpawnObjects.FirstOrDefault(s => ParseType(s.SpawnName.ToLower()) == ParseType(so.SpawnName.ToLower())) == null
                && m_SpawnObjects.Count < SpawnerGump.MaxEntries)
            {
                SpawnObjects.Add(so);
                return true;
            }

            return false;
        }

        public void OnTick()
        {
            DoTimer();

            if (m_Group)
            {
                Defrag();

                if (SpawnCount == 0)
                {
                    Respawn();
                }
                else
                {
                    return;
                }
            }
            else
            {
                Spawn();
            }
        }

        public virtual bool CheckSpawnerFull()
        {
            return (SpawnCount >= m_MaxCount);
        }

        public virtual void Respawn()
        {
            RemoveSpawned();

            for (int i = 0; i < m_MaxCount; i++)
                Spawn();
        }

        public virtual void Spawn()
        {
            List<SpawnObject> avail = GetAvailableSpawnObjects();

            if (avail != null && avail.Count > 0)
            {
                Spawn(avail[Utility.Random(avail.Count)]);
            }
        }

        public void Spawn(int index)
        {
            if (index >= 0 && index < m_SpawnObjects.Count)
            {
                var so = m_SpawnObjects[index];

                if (m_Group)
                {
                    int toSpawn = so.MaxCount - GetSpawnCount(so);

                    for (int i = 0; i < toSpawn; i++)
                    {
                        Spawn(so);
                    }
                }
                else
                {
                    Spawn(so);
                }
            }
        }

        public void Spawn(SpawnObject so)
        {
            Map map = Map;

            if (map == null || map == Map.Internal || SpawnObjectCount == 0 || so == null || Parent != null || GetSpawnCount(so) >= so.MaxCount)
                return;

            Defrag();

            if (CheckSpawnerFull())
                return;

            ISpawnable spawned = CreateSpawnedObject(so);

            if (spawned == null)
                return;

            spawned.Spawner = this;
            so.SpawnedObjects.Add(spawned);

            Point3D loc = (spawned is BaseVendor ? Location : GetSpawnPosition(spawned));

            spawned.OnBeforeSpawn(loc, map);

            spawned.MoveToWorld(loc, map);

            if (spawned is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)spawned;

                if (m_WalkingRange >= 0)
                    bc.RangeHome = m_WalkingRange;
                else
                    bc.RangeHome = m_HomeRange;

                bc.CurrentWayPoint = m_WayPoint;

                if (m_Team > 0)
                    bc.Team = m_Team;

                bc.Home = HomeLocation;
            }

            if (spawned is Mobile)
            {
                Mobile m = (Mobile)spawned;

                m.GuardImmune = GuardImmune;
            }

            spawned.OnAfterSpawn();

            InvalidateProperties();
        }

        public Point3D GetSpawnPosition()
        {
            return GetSpawnPosition(null);
        }

        public Point3D GetSpawnPosition(ISpawnable spawned)
        {
            Map map = Map;

            if (map == null)
                return Location;

            bool waterMob, waterOnlyMob;

            if (spawned is Mobile)
            {
                Mobile mob = (Mobile)spawned;

                waterMob = mob.CanSwim;
                waterOnlyMob = (mob.CanSwim && mob.CantWalk);
            }
            else
            {
                waterMob = false;
                waterOnlyMob = false;
            }

            // Try 10 times to find a spawnable location.
            for (int i = 0; i < 10; ++i)
            {
                int x, y;

                if (m_HomeRange > 0)
                {
                    x = Location.X + (Utility.Random((m_SpawnRange * 2) + 1) - m_HomeRange);
                    y = Location.Y + (Utility.Random((m_SpawnRange * 2) + 1) - m_HomeRange);
                }
                else
                {
                    x = Location.X;
                    y = Location.Y;
                }

                int mapZ = map.GetAverageZ(x, y);

                if (waterMob)
                {
                    if (IsValidWater(map, x, y, Z))
                        return new Point3D(x, y, Z);
                    else if (IsValidWater(map, x, y, mapZ))
                        return new Point3D(x, y, mapZ);
                }

                if (!waterOnlyMob)
                {
                    if (map.CanSpawnMobile(x, y, Z))
                        return new Point3D(x, y, Z);
                    else if (map.CanSpawnMobile(x, y, mapZ))
                        return new Point3D(x, y, mapZ);
                }
            }

            return Location;
        }

        public List<SpawnObject> GetAvailableSpawnObjects()
        {
            if (SpawnObjectCount == 0)
                return null;

            List<SpawnObject> objects = null;

            foreach (var so in m_SpawnObjects)
            {
                if (so.CurrentCount < so.MaxCount)
                {
                    if(objects == null)
                        objects = new List<SpawnObject>();

                    objects.Add(so);
                }
            }

            return objects;
        }

        public int GetSpawnCount(SpawnObject so)
        {
            return so != null ? so.CurrentCount : 0;
        }

        public void DoTimer()
        {
            if (!m_Running)
                return;

            int minSeconds = (int)m_MinDelay.TotalSeconds;
            int maxSeconds = (int)m_MaxDelay.TotalSeconds;

            TimeSpan delay = TimeSpan.FromSeconds(Utility.RandomMinMax(minSeconds, maxSeconds));
            DoTimer(delay);
        }

        public virtual void DoTimer(TimeSpan delay)
        {
            if (!m_Running)
                return;

            m_End = DateTime.UtcNow + delay;

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            m_Timer = new InternalTimer(this, delay);
            m_Timer.Start();
        }

        public string SpawnedStats()
        {
            Defrag();

            Dictionary<string, int> counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in m_SpawnObjects)
            {
                string name = ParseType(entry.SpawnName);
                Type type = ScriptCompiler.FindTypeByName(name);

                if (type == null)
                    counts[name] = 0;
                else
                    counts[type.Name] = 0;
            }

            foreach (var spawned in GetSpawn())
            {
                string name = spawned.GetType().Name;

                if (counts.ContainsKey(name))
                    ++counts[name];
                else
                    counts[name] = 1;
            }

            List<string> names = new List<string>(counts.Keys);
            names.Sort();

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < names.Count; ++i)
                result.AppendFormat("{0}{1}: {2}", (i == 0) ? "" : "<BR>", names[i], counts[names[i]]);

            return result.ToString();
        }

        public int CountCreatures(SpawnObject so)
        {
            Defrag();

            return so.CurrentCount;
        }

        public void RemoveSpawned(int index)
        {
            if (index >= 0 && index < m_SpawnObjects.Count)
            {
                var so = m_SpawnObjects[index];

                if (m_Group)
                {
                    int count = GetSpawnCount(so);

                    for (int i = 0; i < count; i++)
                    {
                        RemoveSpawned(so);
                    }
                }
                else
                {
                    RemoveSpawned(so);
                }
            }
        }

        public void RemoveSpawned(SpawnObject so)
        {
            Defrag();

            if (so.CurrentCount > 0)
                so.SpawnedObjects[0].Delete();

            InvalidateProperties();
        }

        public void RemoveSpawned()
        {
            Defrag();

            List<ISpawnable> toRemove = new List<ISpawnable>();

            foreach (var spawn in GetSpawn())
            {
                toRemove.Add(spawn);
            }

            foreach (var spawn in toRemove)
                spawn.Delete();

            ColUtility.Free(toRemove);
            InvalidateProperties();
        }

        public void BringToHome()
        {
            Defrag();

            foreach (var spawn in GetSpawn())
            {
                spawn.MoveToWorld(Location, Map);
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            RemoveSpawned();

            if (m_Timer != null)
                m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)7); // version

            writer.Write(GuardImmune);

            writer.Write(m_SpawnRange);

            writer.Write(m_WalkingRange);

            writer.Write(m_WayPoint);

            writer.Write(m_Group);

            writer.Write(m_MinDelay);
            writer.Write(m_MaxDelay);
            writer.Write(m_MaxCount);
            writer.Write(m_Team);
            writer.Write(m_HomeRange);
            writer.Write(m_Running);

            if (m_Running)
                writer.WriteDeltaTime(m_End);

            writer.Write(m_SpawnObjects.Count);
            for (int i = 0; i < m_SpawnObjects.Count; ++i)
            {
                m_SpawnObjects[i].Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 7:
                    {
                        GuardImmune = reader.ReadBool();

                        goto case 6;
                    }
                case 6:
                    {
                        m_SpawnRange = reader.ReadInt();

                        goto case 5;
                    }
                case 5:
                case 4:
                    {
                        m_WalkingRange = reader.ReadInt();

                        goto case 3;
                    }
                case 3:
                case 2:
                    {
                        m_WayPoint = reader.ReadItem() as WayPoint;

                        goto case 1;
                    }

                case 1:
                    {
                        m_Group = reader.ReadBool();

                        goto case 0;
                    }

                case 0:
                    {
                        m_MinDelay = reader.ReadTimeSpan();
                        m_MaxDelay = reader.ReadTimeSpan();
                        m_MaxCount = reader.ReadInt();
                        m_Team = reader.ReadInt();
                        m_HomeRange = reader.ReadInt();
                        m_Running = reader.ReadBool();

                        TimeSpan ts = TimeSpan.Zero;

                        if (m_Running)
                            ts = reader.ReadDeltaTime() - DateTime.UtcNow;

                        int size = reader.ReadInt();

                        m_SpawnObjects = new List<SpawnObject>(size);

                        for (int i = 0; i < size; ++i)
                        {
                            if (version > 4)
                            {
                                SpawnObject so = new SpawnObject(reader);

                                if (AddSpawnObject(so))
                                {
                                    string typeName = ParseType(so.SpawnName);

                                    if (ScriptCompiler.FindTypeByName(typeName) == null)
                                    {
                                        if (m_WarnTimer == null)
                                            m_WarnTimer = new WarnTimer();

                                        m_WarnTimer.Add(Location, Map, typeName);
                                    }
                                }
                            }
                            else
                            {
                                string creatureString = reader.ReadString();

                                AddSpawnObject(new SpawnObject(creatureString));
                                string typeName = ParseType(creatureString);

                                if (ScriptCompiler.FindTypeByName(typeName) == null)
                                {
                                    if (m_WarnTimer == null)
                                        m_WarnTimer = new WarnTimer();

                                    m_WarnTimer.Add(Location, Map, typeName);
                                }
                            }
                        }

                        if (version < 5)
                        {
                            int count = reader.ReadInt();
                            for (int i = 0; i < count; ++i)
                            {
                                ISpawnable e = World.FindEntity(reader.ReadInt()) as ISpawnable;

                                if (e != null)
                                {
                                    e.Delete(); // lets make this easy
                                }
                            }
                        }

                        if (m_Running)
                            DoTimer(ts);

                        break;
                    }
            }

            if (version < 3 && Weight == 0)
                Weight = -1;
        }

        protected virtual ISpawnable CreateSpawnedObject(SpawnObject obj)
        {
            if (!m_SpawnObjects.Contains(obj))
                return null;

            Type type = ScriptCompiler.FindTypeByName(ParseType(obj.SpawnName));

            if (type != null)
            {
                try
                {
                    return Build(type, CommandSystem.Split(obj.SpawnName));
                }
                catch
                {
                }
            }

            return null;
        }

        private void InitSpawner(int amount, TimeSpan minDelay, TimeSpan maxDelay, int team, int spawnRange, List<SpawnObject> spawnObjects)
        {
            Visible = false;
            Movable = false;
            m_Running = true;
            m_Group = false;
            m_MinDelay = minDelay;
            m_MaxDelay = maxDelay;
            m_MaxCount = amount;
            m_Team = team;
            m_SpawnRange = spawnRange;
            m_WalkingRange = -1;
            m_SpawnObjects = spawnObjects;
            DoTimer(TimeSpan.FromSeconds(1));

            m_HomeRange = 5;

            int max = 1;

            if (spawnObjects != null)
            {
                foreach (var obj in spawnObjects)
                {
                    max += obj.MaxCount;
                }
            }
        }

        void ISpawner.Remove(ISpawnable spawn)
        {
            RemoveSpawn(spawn);
            InvalidateProperties();
        }

        public static string ParseType(string s)
        {
            return s.Split(null, 2)[0];
        }

        public static ISpawnable Build(Type type, string[] args)
        {
            bool isISpawnable = typeof(ISpawnable).IsAssignableFrom(type);

            if (!isISpawnable)
            {
                return null;
            }

            Add.FixArgs(ref args);

            string[,] props = null;

            for (int i = 0; i < args.Length; ++i)
            {
                if (Insensitive.Equals(args[i], "set"))
                {
                    int remains = args.Length - i - 1;

                    if (remains >= 2)
                    {
                        props = new string[remains / 2, 2];

                        remains /= 2;

                        for (int j = 0; j < remains; ++j)
                        {
                            props[j, 0] = args[i + (j * 2) + 1];
                            props[j, 1] = args[i + (j * 2) + 2];
                        }

                        Add.FixSetString(ref args, i);
                    }

                    break;
                }
            }

            PropertyInfo[] realProps = null;

            if (props != null)
            {
                realProps = new PropertyInfo[props.GetLength(0)];

                PropertyInfo[] allProps = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

                for (int i = 0; i < realProps.Length; ++i)
                {
                    PropertyInfo thisProp = null;

                    string propName = props[i, 0];

                    for (int j = 0; thisProp == null && j < allProps.Length; ++j)
                    {
                        if (Insensitive.Equals(propName, allProps[j].Name))
                            thisProp = allProps[j];
                    }

                    if (thisProp != null)
                    {
                        CPA attr = Properties.GetCPA(thisProp);

                        if (attr != null && AccessLevel.Spawner >= attr.WriteLevel && thisProp.CanWrite && !attr.ReadOnly)
                            realProps[i] = thisProp;
                    }
                }
            }

            ConstructorInfo[] ctors = type.GetConstructors();

            for (int i = 0; i < ctors.Length; ++i)
            {
                ConstructorInfo ctor = ctors[i];

                if (!Add.IsConstructable(ctor, AccessLevel.Spawner))
                    continue;

                ParameterInfo[] paramList = ctor.GetParameters();

                if (args.Length == paramList.Length)
                {
                    object[] paramValues = Add.ParseValues(paramList, args);

                    if (paramValues == null)
                        continue;

                    object built = ctor.Invoke(paramValues);

                    if (built != null && realProps != null)
                    {
                        for (int j = 0; j < realProps.Length; ++j)
                        {
                            if (realProps[j] == null)
                                continue;

                            Properties.InternalSetValue(built, realProps[j], props[j, 1]);
                        }
                    }

                    return (ISpawnable)built;
                }
            }

            return null;
        }

        public static bool IsValidWater(Map map, int x, int y, int z)
        {
            if (!Region.Find(new Point3D(x, y, z), map).AllowSpawn() || !map.CanFit(x, y, z, 16, false, true, false))
                return false;

            LandTile landTile = map.Tiles.GetLandTile(x, y);

            if (landTile.Z == z && (TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Wet) != 0)
                return true;

            StaticTile[] staticTiles = map.Tiles.GetStaticTiles(x, y, true);

            for (int i = 0; i < staticTiles.Length; ++i)
            {
                StaticTile staticTile = staticTiles[i];

                if (staticTile.Z == z && (TileData.ItemTable[staticTile.ID & TileData.MaxItemValue].Flags & TileFlag.Wet) != 0)
                    return true;
            }

            return false;
        }

        private class InternalTimer : Timer
        {
            private readonly Spawner m_Spawner;
            public InternalTimer(Spawner spawner, TimeSpan delay)
                : base(delay)
            {
                if (spawner.IsFull)
                    Priority = TimerPriority.FiveSeconds;
                else
                    Priority = TimerPriority.OneSecond;

                m_Spawner = spawner;
            }

            protected override void OnTick()
            {
                if (m_Spawner != null && !m_Spawner.Deleted)
                    m_Spawner.OnTick();
            }
        }

        private class WarnTimer : Timer
        {
            private readonly List<WarnEntry> m_List;
            public WarnTimer()
                : base(TimeSpan.FromSeconds(1.0))
            {
                m_List = new List<WarnEntry>();
                Start();
            }

            public void Add(Point3D p, Map map, string name)
            {
                m_List.Add(new WarnEntry(p, map, name));
            }

            protected override void OnTick()
            {
                try
                {
                    Console.WriteLine("Warning: {0} bad spawns detected, logged: 'badspawn.log'", m_List.Count);

                    using (StreamWriter op = new StreamWriter("badspawn.log", true))
                    {
                        op.WriteLine("# Bad spawns : {0}", DateTime.UtcNow);
                        op.WriteLine("# Format: X Y Z F Name");
                        op.WriteLine();

                        foreach (WarnEntry e in m_List)
                            op.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", e.m_Point.X, e.m_Point.Y, e.m_Point.Z, e.m_Map, e.m_Name);

                        op.WriteLine();
                        op.WriteLine();
                    }
                }
                catch
                {
                }
            }

            private class WarnEntry
            {
                public readonly Point3D m_Point;
                public readonly Map m_Map;
                public readonly string m_Name;
                public WarnEntry(Point3D p, Map map, string name)
                {
                    m_Point = p;
                    m_Map = map;
                    m_Name = name;
                }
            }
        }
    }
}