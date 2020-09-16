using Server.Items;
using Server.Mobiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Server.Regions
{
    public abstract class SpawnDefinition
    {
        public static SpawnDefinition GetSpawnDefinition(XmlElement xml)
        {
            switch (xml.Name)
            {
                case "object":
                    {
                        Type type = null;
                        if (!Region.ReadType(xml, "type", ref type))
                            return null;

                        if (typeof(Mobile).IsAssignableFrom(type))
                        {
                            return SpawnMobile.Get(type);
                        }
                        else if (typeof(Item).IsAssignableFrom(type))
                        {
                            return SpawnItem.Get(type);
                        }
                        else
                        {
                            Console.WriteLine("Invalid type '{0}' in a SpawnDefinition", type.FullName);
                            return null;
                        }
                    }
                case "group":
                    {
                        string group = null;
                        if (!Region.ReadString(xml, "name", ref group))
                            return null;

                        SpawnDefinition def = (SpawnDefinition)SpawnGroup.Table[group];

                        if (def == null)
                        {
                            Console.WriteLine("Could not find group '{0}' in a SpawnDefinition", group);
                            return null;
                        }
                        else
                        {
                            return def;
                        }
                    }
                case "treasureChest":
                    {
                        int itemID = 0xE43;
                        Region.ReadInt32(xml, "itemID", ref itemID, false);

                        BaseTreasureChest.TreasureLevel level = BaseTreasureChest.TreasureLevel.Level2;

                        Region.ReadEnum(xml, "level", ref level, false);

                        return new SpawnTreasureChest(itemID, level);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public abstract ISpawnable Spawn(SpawnEntry entry);

        public abstract bool CanSpawn(params Type[] types);
    }

    public abstract class SpawnType : SpawnDefinition
    {
        private readonly Type m_Type;
        private bool m_Init;
        protected SpawnType(Type type)
        {
            m_Type = type;
            m_Init = false;
        }

        public Type Type => m_Type;
        public abstract int Height { get; }
        public abstract bool Land { get; }
        public abstract bool Water { get; }
        public override ISpawnable Spawn(SpawnEntry entry)
        {
            BaseRegion region = entry.Region;
            Map map = region.Map;

            Point3D loc = entry.RandomSpawnLocation(Height, Land, Water);

            if (loc == Point3D.Zero)
                return null;

            return Construct(entry, loc, map);
        }

        public override bool CanSpawn(params Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == m_Type)
                    return true;
            }

            return false;
        }

        protected void EnsureInit()
        {
            if (m_Init)
                return;

            Init();
            m_Init = true;
        }

        protected virtual void Init()
        {
        }

        protected abstract ISpawnable Construct(SpawnEntry entry, Point3D loc, Map map);
    }

    public class SpawnMobile : SpawnType
    {
        protected bool m_Land;
        protected bool m_Water;
        private static readonly Hashtable m_Table = new Hashtable();
        protected SpawnMobile(Type type)
            : base(type)
        {
        }

        public override int Height => 16;
        public override bool Land
        {
            get
            {
                EnsureInit();
                return m_Land;
            }
        }
        public override bool Water
        {
            get
            {
                EnsureInit();
                return m_Water;
            }
        }
        public static SpawnMobile Get(Type type)
        {
            SpawnMobile sm = (SpawnMobile)m_Table[type];

            if (sm == null)
            {
                sm = new SpawnMobile(type);
                m_Table[type] = sm;
            }

            return sm;
        }

        protected override void Init()
        {
            Mobile mob = (Mobile)Activator.CreateInstance(Type);

            m_Land = !mob.CantWalk;
            m_Water = mob.CanSwim;

            mob.Delete();
        }

        protected override ISpawnable Construct(SpawnEntry entry, Point3D loc, Map map)
        {
            Mobile mobile = CreateMobile();

            BaseCreature creature = mobile as BaseCreature;

            if (creature != null)
            {
                creature.Home = entry.HomeLocation;
                creature.RangeHome = entry.HomeRange;
            }

            if (entry.Direction != SpawnEntry.InvalidDirection)
                mobile.Direction = entry.Direction;

            mobile.OnBeforeSpawn(loc, map);
            mobile.MoveToWorld(loc, map);
            mobile.OnAfterSpawn();

            return mobile;
        }

        protected virtual Mobile CreateMobile()
        {
            return (Mobile)Activator.CreateInstance(Type);
        }
    }

    public class SpawnItem : SpawnType
    {
        protected int m_Height;
        private static readonly Hashtable m_Table = new Hashtable();
        protected SpawnItem(Type type)
            : base(type)
        {
        }

        public override int Height
        {
            get
            {
                EnsureInit();
                return m_Height;
            }
        }
        public override bool Land => true;
        public override bool Water => false;
        public static SpawnItem Get(Type type)
        {
            SpawnItem si = (SpawnItem)m_Table[type];

            if (si == null)
            {
                si = new SpawnItem(type);
                m_Table[type] = si;
            }

            return si;
        }

        protected override void Init()
        {
            Item item = (Item)Activator.CreateInstance(Type);

            m_Height = item.ItemData.Height;

            item.Delete();
        }

        protected override ISpawnable Construct(SpawnEntry entry, Point3D loc, Map map)
        {
            Item item = CreateItem();

            item.OnBeforeSpawn(loc, map);
            item.MoveToWorld(loc, map);
            item.OnAfterSpawn();

            return item;
        }

        protected virtual Item CreateItem()
        {
            return (Item)Activator.CreateInstance(Type);
        }
    }

    public class SpawnTreasureChest : SpawnItem
    {
        private readonly int m_ItemID;
        private readonly BaseTreasureChest.TreasureLevel m_Level;
        public SpawnTreasureChest(int itemID, BaseTreasureChest.TreasureLevel level)
            : base(typeof(BaseTreasureChest))
        {
            m_ItemID = itemID;
            m_Level = level;
        }

        public int ItemID => m_ItemID;
        public BaseTreasureChest.TreasureLevel Level => m_Level;
        protected override void Init()
        {
            m_Height = TileData.ItemTable[m_ItemID & TileData.MaxItemValue].Height;
        }

        protected override Item CreateItem()
        {
            return new BaseTreasureChest(m_ItemID, m_Level);
        }
    }

    public class SpawnGroupElement
    {
        private readonly SpawnDefinition m_SpawnDefinition;
        private readonly int m_Weight;
        public SpawnGroupElement(SpawnDefinition spawnDefinition, int weight)
        {
            m_SpawnDefinition = spawnDefinition;
            m_Weight = weight;
        }

        public SpawnDefinition SpawnDefinition => m_SpawnDefinition;
        public int Weight => m_Weight;
    }

    public class SpawnGroup : SpawnDefinition
    {
        private static readonly Hashtable m_Table = new Hashtable();
        private readonly string m_Name;
        private readonly SpawnGroupElement[] m_Elements;
        private readonly int m_TotalWeight;
        public SpawnGroup(string name, SpawnGroupElement[] elements)
        {
            m_Name = name;
            m_Elements = elements;

            m_TotalWeight = 0;
            for (int i = 0; i < elements.Length; i++)
                m_TotalWeight += elements[i].Weight;
        }

        static SpawnGroup()
        {
            string path = Path.Combine(Core.BaseDirectory, "Data/SpawnDefinitions.xml");
            if (!File.Exists(path))
                return;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                XmlElement root = doc["spawnDefinitions"];
                if (root == null)
                    return;

                foreach (XmlElement xmlDef in root.SelectNodes("spawnGroup"))
                {
                    string name = null;
                    if (!Region.ReadString(xmlDef, "name", ref name))
                        continue;

                    List<SpawnGroupElement> list = new List<SpawnGroupElement>();
                    foreach (XmlNode node in xmlDef.ChildNodes)
                    {
                        XmlElement el = node as XmlElement;

                        if (el != null)
                        {
                            SpawnDefinition def = GetSpawnDefinition(el);
                            if (def == null)
                                continue;

                            int weight = 1;
                            Region.ReadInt32(el, "weight", ref weight, false);

                            SpawnGroupElement groupElement = new SpawnGroupElement(def, weight);
                            list.Add(groupElement);
                        }
                    }

                    SpawnGroupElement[] elements = list.ToArray();
                    SpawnGroup group = new SpawnGroup(name, elements);
                    Register(group);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not load SpawnDefinitions.xml: ");
                Diagnostics.ExceptionLogging.LogException(ex);
            }
        }

        public static Hashtable Table => m_Table;
        public string Name => m_Name;
        public SpawnGroupElement[] Elements => m_Elements;
        public static void Register(SpawnGroup group)
        {
            if (m_Table.Contains(group.Name))
                Console.WriteLine("Warning: Double SpawnGroup name '{0}'", group.Name);
            else
                m_Table[group.Name] = group;
        }

        public override ISpawnable Spawn(SpawnEntry entry)
        {
            int index = Utility.Random(m_TotalWeight);

            for (int i = 0; i < m_Elements.Length; i++)
            {
                SpawnGroupElement element = m_Elements[i];

                if (index < element.Weight)
                    return element.SpawnDefinition.Spawn(entry);

                index -= element.Weight;
            }

            return null;
        }

        public override bool CanSpawn(params Type[] types)
        {
            for (int i = 0; i < m_Elements.Length; i++)
            {
                if (m_Elements[i].SpawnDefinition.CanSpawn(types))
                    return true;
            }

            return false;
        }
    }
}
