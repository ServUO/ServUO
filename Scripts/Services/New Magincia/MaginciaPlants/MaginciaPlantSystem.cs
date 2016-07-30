using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Network;
using Server.Gumps;
using Server.Engines.NewMagincia;

namespace Server.Engines.Plants
{
    public class MaginciaPlantSystem : Item
    {
        public static readonly bool Enabled = true;
        public static readonly int PlantDelay = 4;

        private Dictionary<Mobile, DateTime> m_PlantDelayTable = new Dictionary<Mobile, DateTime>();
        public Dictionary<Mobile, DateTime> PlantDelayTable { get { return m_PlantDelayTable; } }

        private static MaginciaPlantSystem m_FelInstance;
        private static MaginciaPlantSystem m_TramInstance;

        public static MaginciaPlantSystem FelInstance { get { return m_FelInstance; } }
        public static MaginciaPlantSystem TramInstance { get { return m_TramInstance; } }

        public static void Initialize()
        {
            if (Enabled)
            {
                if (m_FelInstance == null)
                {
                    m_FelInstance = new MaginciaPlantSystem();
                    m_FelInstance.MoveToWorld(new Point3D(3715, 2049, 5), Map.Felucca);
                }
                if (m_TramInstance == null)
                {
                    m_TramInstance = new MaginciaPlantSystem();
                    m_TramInstance.MoveToWorld(new Point3D(3715, 2049, 5), Map.Trammel);
                }
            }
        }

        public MaginciaPlantSystem() : base(3240)
        {
            Movable = false;
        }

        public bool CheckPlantDelay(Mobile from)
        {
            if (m_PlantDelayTable.ContainsKey(from))
            {
                if (m_PlantDelayTable[from] > DateTime.UtcNow)
                {
                    TimeSpan left = m_PlantDelayTable[from] - DateTime.UtcNow;

                    from.SendLocalizedMessage(1150459, String.Format("{0}\t{1}\t{2}", left.Days.ToString(), left.Hours.ToString(), left.Minutes.ToString()));
                    return false;
                }
            }

            return true;
        }

        public void OnPlantDelete(Mobile from)
        {
            if (m_PlantDelayTable.ContainsKey(from))
                m_PlantDelayTable.Remove(from);
        }

        public void OnPlantPlanted(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                m_PlantDelayTable[from] = DateTime.UtcNow + TimeSpan.FromDays(PlantDelay);
            else
                from.SendMessage("As staff, you bypass the {0} day plant delay.", PlantDelay);
        }

        public override void Delete()
        {
        }

        public static bool CanAddPlant(Mobile from, Point3D p)
        {
            if (!IsValidLocation(p))
            {
                from.SendLocalizedMessage(1150457); // The ground here is not good for gardening.
                return false;
            }

            Map map = from.Map;

            IPooledEnumerable eable = map.GetItemsInRange(p, 17);
            int plantCount = 0;

            foreach (Item item in eable)
            {
                if (item is MaginciaPlantItem)
                {
                    if(item.Location != p)
                        plantCount++;
                    else
                    {
                        from.SendLocalizedMessage(1150367); // This plot already has a plant!
                        eable.Free();
                        return false;
                    }
                }
                else if (!item.Movable && item.Location == p)
                {
                    from.SendLocalizedMessage(1150457); // The ground here is not good for gardening.
                    eable.Free();
                    return false;
                }
            }

            eable.Free();

            if (plantCount > 34)
            {
                from.SendLocalizedMessage(1150491); // There are too many objects in this area to plant (limit 34 per 17x17 area).
                return false;
            }

            StaticTile[] staticTiles = map.Tiles.GetStaticTiles(p.X, p.Y, true);

            if (staticTiles.Length > 0)
            {
                from.SendLocalizedMessage(1150457); // The ground here is not good for gardening.
                return false;
            }

            return true;
        }

        public static bool CheckDelay(Mobile from)
        {
            MaginciaPlantSystem system = null;
            Map map = from.Map;
            
            if (map == Map.Trammel)
                system = m_TramInstance;
            else if (map == Map.Felucca)
                system = m_FelInstance;

            if (system == null)
            {
                from.SendLocalizedMessage(1150457); // The ground here is not good for gardening.
                return false;
            }

            return system.CheckPlantDelay(from);
        }

        public static bool IsValidLocation(Point3D p)
        {
            /*foreach (Rectangle2D rec in m_MagGrowBounds)
            {
                if (rec.Contains(p))
                    return true;
            }*/
            foreach (Rectangle2D rec in m_NoGrowZones)
            {
                if (rec.Contains(p))
                    return false;
            }

            foreach (Rectangle2D rec in MaginciaLottoSystem.MagHousingZones)
            {
                Rectangle2D newRec = new Rectangle2D(rec.X - 2, rec.Y - 2, rec.Width + 4, rec.Height + 7);

                if (newRec.Contains(p))
                    return false;
            }

            return true;
        }

        public static void OnPlantDelete(Mobile owner, Map map)
        {
            if (owner == null || map == null)
                return;

            if (map == Map.Trammel)
                m_TramInstance.OnPlantDelete(owner);
            else if (map == Map.Felucca)
                m_FelInstance.OnPlantDelete(owner);
        }

        public static void OnPlantPlanted(Mobile from, Map map)
        {
            if (map == Map.Felucca)
                m_FelInstance.OnPlantPlanted(from);
            else if (map == Map.Trammel)
                m_TramInstance.OnPlantPlanted(from);
        }

        public static Rectangle2D[] MagGrowBounds { get { return m_MagGrowBounds; } }
        private static Rectangle2D[] m_MagGrowBounds = new Rectangle2D[]
        {
            new Rectangle2D(3663, 2103, 19, 19),
            new Rectangle2D(3731, 2199, 7, 7),
        };

        private static Rectangle2D[] m_NoGrowZones = new Rectangle2D[]
        {
            new Rectangle2D(3683, 2144, 21, 40),
            new Rectangle2D(3682, 2189, 39, 44),
            new Rectangle2D(3654, 2233, 23, 30),
            new Rectangle2D(3727, 2217, 15, 45),
            new Rectangle2D(3558, 2134, 8, 8), 
            new Rectangle2D(3679, 2018, 70, 28)
        };

        public void DefragPlantDelayTable()
        {
            List<Mobile> toRemove = new List<Mobile>();

            foreach (KeyValuePair<Mobile, DateTime> kvp in m_PlantDelayTable)
            {
                if (kvp.Value < DateTime.UtcNow)
                    toRemove.Add(kvp.Key);
            }

            foreach (Mobile m in toRemove)
                m_PlantDelayTable.Remove(m);
        }

        public MaginciaPlantSystem(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            DefragPlantDelayTable();

            writer.Write(m_PlantDelayTable.Count);
            foreach (KeyValuePair<Mobile, DateTime> kvp in m_PlantDelayTable)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            int c = reader.ReadInt();

            for (int i = 0; i < c; i++)
            {
                Mobile m = reader.ReadMobile();
                DateTime dt = reader.ReadDateTime();

                if (m != null && dt > DateTime.UtcNow)
                    m_PlantDelayTable[m] = dt;
            }

            if (this.Map == Map.Felucca)
                m_FelInstance = this;

            else if (this.Map == Map.Trammel)
                m_TramInstance = this;

            else
                Delete();
        }
    }
}