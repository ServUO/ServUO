using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Server.Commands;
using Server.Items;
using CPA = Server.CommandPropertyAttribute;

namespace Server.Mobiles
{
    public class Spawner : Item, ISpawner
    {
        private static WarnTimer m_WarnTimer;
        private int m_Team;
        private int m_HomeRange;
        private int m_WalkingRange;
        private int m_Count;
        private TimeSpan m_MinDelay;
        private TimeSpan m_MaxDelay;
        private List<string> m_SpawnNames;
        private List<ISpawnable> m_Spawned;
        private DateTime m_End;
        private InternalTimer m_Timer;
        private bool m_Running;
        private bool m_Group;
        private WayPoint m_WayPoint;
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
        public Spawner(int amount, int minDelay, int maxDelay, int team, int homeRange, string spawnName)
            : base(0x1f13)
        {
            List<string> spawnNames = new List<string>();

            if (!String.IsNullOrEmpty(spawnName))
                spawnNames.Add(spawnName);

            this.InitSpawner(amount, TimeSpan.FromMinutes(minDelay), TimeSpan.FromMinutes(maxDelay), team, homeRange, spawnNames);
        }

        public Spawner(int amount, TimeSpan minDelay, TimeSpan maxDelay, int team, int homeRange, List<string> spawnNames)
            : base(0x1f13)
        {
            this.InitSpawner(amount, minDelay, maxDelay, team, homeRange, spawnNames);
        }

        public Spawner(Serial serial)
            : base(serial)
        {
        }

        public bool IsFull
        {
            get
            {
                return (this.m_Spawned.Count >= this.m_Count);
            }
        }
        public bool IsEmpty
        {
            get
            {
                return (this.m_Spawned.Count == 0);
            }
        }
        public List<string> SpawnNames
        {
            get
            {
                return this.m_SpawnNames;
            }
            set
            {
                this.m_SpawnNames = value;
                if (this.m_SpawnNames.Count < 1)
                    this.Stop();

                this.InvalidateProperties();
            }
        }
        public List<ISpawnable> Spawned
        {
            get
            {
                return this.m_Spawned;
            }
        }
        public virtual int SpawnNamesCount
        {
            get
            {
                return this.m_SpawnNames.Count;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int Count
        {
            get
            {
                return this.m_Count;
            }
            set
            {
                this.m_Count = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public WayPoint WayPoint
        {
            get
            {
                return this.m_WayPoint;
            }
            set
            {
                this.m_WayPoint = value;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public bool Running
        {
            get
            {
                return this.m_Running;
            }
            set
            {
                if (value)
                    this.Start();
                else
                    this.Stop();

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int HomeRange
        {
            get
            {
                return this.m_HomeRange;
            }
            set
            {
                this.m_HomeRange = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int WalkingRange
        {
            get
            {
                return this.m_WalkingRange;
            }
            set
            {
                this.m_WalkingRange = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int Team
        {
            get
            {
                return this.m_Team;
            }
            set
            {
                this.m_Team = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public TimeSpan MinDelay
        {
            get
            {
                return this.m_MinDelay;
            }
            set
            {
                this.m_MinDelay = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public TimeSpan MaxDelay
        {
            get
            {
                return this.m_MaxDelay;
            }
            set
            {
                this.m_MaxDelay = value;
                this.InvalidateProperties();
            }
        }
        public DateTime End
        {
            get
            {
                return this.m_End;
            }
            set
            {
                this.m_End = value;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public TimeSpan NextSpawn
        {
            get
            {
                if (this.m_Running)
                    return this.m_End - DateTime.UtcNow;
                else
                    return TimeSpan.FromSeconds(0);
            }
            set
            {
                this.Start();
                this.DoTimer(value);
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public bool Group
        {
            get
            {
                return this.m_Group;
            }
            set
            {
                this.m_Group = value;
                this.InvalidateProperties();
            }
        }
        public override string DefaultName
        {
            get
            {
                return "Spawner";
            }
        }
        public Point3D HomeLocation
        {
            get
            {
                return this.Location;
            }
        }
        bool ISpawner.UnlinkOnTaming
        {
            get
            {
                return true;
            }
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

        public override void OnAfterDuped(Item newItem)
        {
            Spawner s = newItem as Spawner;

            if (s == null)
                return;

            s.m_SpawnNames = new List<string>(this.m_SpawnNames);
            s.m_Spawned = new List<ISpawnable>();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel < AccessLevel.Spawner)
                return;

            SpawnerGump g = new SpawnerGump(this);
            from.SendGump(g);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Running)
            {
                list.Add(1060742); // active

                list.Add(1060656, this.m_Count.ToString()); // amount to make: ~1_val~
                list.Add(1061169, this.m_HomeRange.ToString()); // range ~1_val~
                list.Add(1060658, "walking range\t{0}", this.m_WalkingRange); // ~1_val~: ~2_val~

                list.Add(1060659, "group\t{0}", this.m_Group); // ~1_val~: ~2_val~
                list.Add(1060660, "team\t{0}", this.m_Team); // ~1_val~: ~2_val~
                list.Add(1060661, "speed\t{0} to {1}", this.m_MinDelay, this.m_MaxDelay); // ~1_val~: ~2_val~

                if (this.m_SpawnNames.Count != 0)
                    list.Add(this.SpawnedStats());
            }
            else
            {
                list.Add(1060743); // inactive
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (this.m_Running)
                this.LabelTo(from, "[Running]");
            else
                this.LabelTo(from, "[Off]");
        }

        public void Start()
        {
            if (!this.m_Running)
            {
                if (this.SpawnNamesCount > 0)
                {
                    this.m_Running = true;
                    this.DoTimer();
                }
            }
        }

        public void Stop()
        {
            if (this.m_Running)
            {
                if (this.m_Timer != null)
                    this.m_Timer.Stop();

                this.m_Running = false;
            }
        }

        public void Defrag()
        {
            bool removed = false;

            for (int i = 0; i < this.m_Spawned.Count; ++i)
            {
                ISpawnable e = this.m_Spawned[i];

                bool toRemove = false;

                if (e is Item)
                {
                    Item item = (Item)e;

                    if (item.Deleted || item.Parent != null)
                        toRemove = true;
                }
                else if (e is Mobile)
                {
                    Mobile m = (Mobile)e;

                    if (m.Deleted)
                    {
                        toRemove = true;
                    }
                    else if (m is BaseCreature)
                    {
                        BaseCreature bc = (BaseCreature)m;

                        if (bc.Controlled || bc.IsStabled)
                        {
                            toRemove = true;
                        }
                    }
                }

                if (toRemove)
                {
                    this.m_Spawned.RemoveAt(i);
                    --i;
                    removed = true;
                }
            }

            if (removed)
                this.InvalidateProperties();
        }

        public void OnTick()
        {
            this.DoTimer();

            if (this.m_Group)
            {
                this.Defrag();

                if (this.m_Spawned.Count == 0)
                {
                    this.Respawn();
                }
                else
                {
                    return;
                }
            }
            else
            {
                this.Spawn();
            }
        }

        public virtual void Respawn()
        {
            this.RemoveSpawned();

            for (int i = 0; i < this.m_Count; i++)
                this.Spawn();
        }

        public virtual void Spawn()
        {
            if (this.SpawnNamesCount > 0)
                this.Spawn(Utility.Random(this.SpawnNamesCount));
        }

        public void Spawn(string creatureName)
        {
            for (int i = 0; i < this.m_SpawnNames.Count; i++)
            {
                if (this.m_SpawnNames[i] == creatureName)
                {
                    this.Spawn(i);
                    break;
                }
            }
        }

        public virtual bool CheckSpawnerFull()
        {
            return (this.m_Spawned.Count >= this.m_Count);
        }

        public void Spawn(int index)
        {
            Map map = this.Map;

            if (map == null || map == Map.Internal || this.SpawnNamesCount == 0 || index >= this.SpawnNamesCount || this.Parent != null)
                return;

            this.Defrag();

            if (this.CheckSpawnerFull())
                return;

            ISpawnable spawned = this.CreateSpawnedObject(index);

            if (spawned == null)
                return;

            spawned.Spawner = this;
            this.m_Spawned.Add(spawned);

            Point3D loc = (spawned is BaseVendor ? this.Location : this.GetSpawnPosition(spawned));

            spawned.OnBeforeSpawn(loc, map);

            this.InvalidateProperties();

            spawned.MoveToWorld(loc, map);

            if (spawned is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)spawned;

                if (this.m_WalkingRange >= 0)
                    bc.RangeHome = this.m_WalkingRange;
                else
                    bc.RangeHome = this.m_HomeRange;

                bc.CurrentWayPoint = this.m_WayPoint;

                if (this.m_Team > 0)
                    bc.Team = this.m_Team;

                bc.Home = this.HomeLocation;
            }
        }

        public Point3D GetSpawnPosition()
        {
            return this.GetSpawnPosition(null);
        }

        public Point3D GetSpawnPosition(ISpawnable spawned)
        {
            Map map = this.Map;

            if (map == null)
                return this.Location;

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

                if (this.m_HomeRange > 0)
                {
                    x = this.Location.X + (Utility.Random((this.m_HomeRange * 2) + 1) - this.m_HomeRange);
                    y = this.Location.Y + (Utility.Random((this.m_HomeRange * 2) + 1) - this.m_HomeRange);
                }
                else
                {
                    x = this.Location.X;
                    y = this.Location.Y;
                }

                int mapZ = map.GetAverageZ(x, y);

                if (waterMob)
                {
                    if (IsValidWater(map, x, y, this.Z))
                        return new Point3D(x, y, this.Z);
                    else if (IsValidWater(map, x, y, mapZ))
                        return new Point3D(x, y, mapZ);
                }

                if (!waterOnlyMob)
                {
                    if (map.CanSpawnMobile(x, y, this.Z))
                        return new Point3D(x, y, this.Z);
                    else if (map.CanSpawnMobile(x, y, mapZ))
                        return new Point3D(x, y, mapZ);
                }
            }

            return this.Location;
        }

        public void DoTimer()
        {
            if (!this.m_Running)
                return;

            int minSeconds = (int)this.m_MinDelay.TotalSeconds;
            int maxSeconds = (int)this.m_MaxDelay.TotalSeconds;

            TimeSpan delay = TimeSpan.FromSeconds(Utility.RandomMinMax(minSeconds, maxSeconds));
            this.DoTimer(delay);
        }

        public virtual void DoTimer(TimeSpan delay)
        {
            if (!this.m_Running)
                return;

            this.m_End = DateTime.UtcNow + delay;

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = new InternalTimer(this, delay);
            this.m_Timer.Start();
        }

        public string SpawnedStats()
        {
            this.Defrag();

            Dictionary<string, int> counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (string entry in this.m_SpawnNames)
            {
                string name = ParseType(entry);
                Type type = ScriptCompiler.FindTypeByName(name);

                if (type == null)
                    counts[name] = 0;
                else
                    counts[type.Name] = 0;
            }

            foreach (ISpawnable spawned in this.m_Spawned)
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

        public int CountCreatures(string creatureName)
        {
            this.Defrag();

            int count = 0;

            for (int i = 0; i < this.m_Spawned.Count; ++i)
                if (Insensitive.Equals(creatureName, this.m_Spawned[i].GetType().Name))
                    ++count;

            return count;
        }

        public void RemoveSpawned(string creatureName)
        {
            this.Defrag();

            for (int i = this.m_Spawned.Count - 1; i >= 0; --i)
            {
                IEntity e = this.m_Spawned[i];

                if (Insensitive.Equals(creatureName, e.GetType().Name))
                    e.Delete();
            }

            this.InvalidateProperties();
        }

        public void RemoveSpawned()
        {
            this.Defrag();

            for (int i = this.m_Spawned.Count - 1; i >= 0; --i)
                this.m_Spawned[i].Delete();

            this.InvalidateProperties();
        }

        public void BringToHome()
        {
            this.Defrag();

            for (int i = 0; i < this.m_Spawned.Count; ++i)
            {
                ISpawnable e = this.m_Spawned[i];

                e.MoveToWorld(this.Location, this.Map);
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            this.RemoveSpawned();

            if (this.m_Timer != null)
                this.m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)4); // version
            writer.Write(this.m_WalkingRange);

            writer.Write(this.m_WayPoint);

            writer.Write(this.m_Group);

            writer.Write(this.m_MinDelay);
            writer.Write(this.m_MaxDelay);
            writer.Write(this.m_Count);
            writer.Write(this.m_Team);
            writer.Write(this.m_HomeRange);
            writer.Write(this.m_Running);

            if (this.m_Running)
                writer.WriteDeltaTime(this.m_End);

            writer.Write(this.m_SpawnNames.Count);

            for (int i = 0; i < this.m_SpawnNames.Count; ++i)
                writer.Write(this.m_SpawnNames[i]);

            writer.Write(this.m_Spawned.Count);

            for (int i = 0; i < this.m_Spawned.Count; ++i)
            {
                IEntity e = this.m_Spawned[i];

                if (e is Item)
                    writer.Write((Item)e);
                else if (e is Mobile)
                    writer.Write((Mobile)e);
                else
                    writer.Write(Serial.MinusOne);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 4:
                    {
                        this.m_WalkingRange = reader.ReadInt();

                        goto case 3;
                    }
                case 3:
                case 2:
                    {
                        this.m_WayPoint = reader.ReadItem() as WayPoint;

                        goto case 1;
                    }

                case 1:
                    {
                        this.m_Group = reader.ReadBool();

                        goto case 0;
                    }

                case 0:
                    {
                        this.m_MinDelay = reader.ReadTimeSpan();
                        this.m_MaxDelay = reader.ReadTimeSpan();
                        this.m_Count = reader.ReadInt();
                        this.m_Team = reader.ReadInt();
                        this.m_HomeRange = reader.ReadInt();
                        this.m_Running = reader.ReadBool();

                        TimeSpan ts = TimeSpan.Zero;

                        if (this.m_Running)
                            ts = reader.ReadDeltaTime() - DateTime.UtcNow;

                        int size = reader.ReadInt();

                        this.m_SpawnNames = new List<string>(size);

                        for (int i = 0; i < size; ++i)
                        {
                            string creatureString = reader.ReadString();

                            this.m_SpawnNames.Add(creatureString);
                            string typeName = ParseType(creatureString);

                            if (ScriptCompiler.FindTypeByName(typeName) == null)
                            {
                                if (m_WarnTimer == null)
                                    m_WarnTimer = new WarnTimer();

                                m_WarnTimer.Add(this.Location, this.Map, typeName);
                            }
                        }

                        int count = reader.ReadInt();

                        this.m_Spawned = new List<ISpawnable>(count);

                        for (int i = 0; i < count; ++i)
                        {
                            ISpawnable e = World.FindEntity(reader.ReadInt()) as ISpawnable;

                            if (e != null)
                            {
                                e.Spawner = this;
                                this.m_Spawned.Add(e);
                            }
                        }

                        if (this.m_Running)
                            this.DoTimer(ts);

                        break;
                    }
            }

            if (version < 3 && this.Weight == 0)
                this.Weight = -1;
        }

        protected virtual ISpawnable CreateSpawnedObject(int index)
        {
            if (index >= this.m_SpawnNames.Count)
                return null;

            Type type = ScriptCompiler.FindTypeByName(ParseType(this.m_SpawnNames[index]));

            if (type != null)
            {
                try
                {
                    return Build(type, CommandSystem.Split(this.m_SpawnNames[index]));
                }
                catch
                {
                }
            }

            return null;
        }

        private void InitSpawner(int amount, TimeSpan minDelay, TimeSpan maxDelay, int team, int homeRange, List<string> spawnNames)
        {
            this.Visible = false;
            this.Movable = false;
            this.m_Running = true;
            this.m_Group = false;
            this.m_MinDelay = minDelay;
            this.m_MaxDelay = maxDelay;
            this.m_Count = amount;
            this.m_Team = team;
            this.m_HomeRange = homeRange;
            this.m_WalkingRange = -1;
            this.m_SpawnNames = spawnNames;
            this.m_Spawned = new List<ISpawnable>();
            this.DoTimer(TimeSpan.FromSeconds(1));
        }

        void ISpawner.Remove(ISpawnable spawn)
        {
            this.m_Spawned.Remove(spawn);

            this.InvalidateProperties();
        }

        private class InternalTimer : Timer
        {
            private readonly Spawner m_Spawner;
            public InternalTimer(Spawner spawner, TimeSpan delay)
                : base(delay)
            {
                if (spawner.IsFull)
                    this.Priority = TimerPriority.FiveSeconds;
                else
                    this.Priority = TimerPriority.OneSecond;

                this.m_Spawner = spawner;
            }

            protected override void OnTick()
            {
                if (this.m_Spawner != null)
                    if (!this.m_Spawner.Deleted)
                        this.m_Spawner.OnTick();
            }
        }

        private class WarnTimer : Timer
        {
            private readonly List<WarnEntry> m_List;
            public WarnTimer()
                : base(TimeSpan.FromSeconds(1.0))
            {
                this.m_List = new List<WarnEntry>();
                this.Start();
            }

            public void Add(Point3D p, Map map, string name)
            {
                this.m_List.Add(new WarnEntry(p, map, name));
            }

            protected override void OnTick()
            {
                try
                {
                    Console.WriteLine("Warning: {0} bad spawns detected, logged: 'badspawn.log'", this.m_List.Count);

                    using (StreamWriter op = new StreamWriter("badspawn.log", true))
                    {
                        op.WriteLine("# Bad spawns : {0}", DateTime.UtcNow);
                        op.WriteLine("# Format: X Y Z F Name");
                        op.WriteLine();

                        foreach (WarnEntry e in this.m_List)
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
                    this.m_Point = p;
                    this.m_Map = map;
                    this.m_Name = name;
                }
            }
        }
    }
}