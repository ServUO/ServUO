using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using System.IO;
using Server.Engines.Harvest;

namespace Server.Items
{
    public class HarvestMap : Item, IUsesRemaining
    {
        public const int DecayPeriod = 24;

        private CraftResource _Resource;
        private int _UsesRemaining;
        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return _Resource; }
            set
            {
                if (_Resource != value)
                {
                    _Resource = value;
                    Hue = CraftResources.GetHue(_Resource);
                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return _UsesRemaining; }
            set
            {
                _UsesRemaining = value;

                if (_UsesRemaining <= 0 && this.RootParent is Mobile)
                    ((Mobile)RootParent).SendMessage("Your map's magic is exhausted.");

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowUsesRemaining { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expires { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point2D Target { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map TargetMap { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsMinerMap { get { return _Resource >= CraftResource.Iron && _Resource <= CraftResource.Valorite; } }

        [Constructable]
        public HarvestMap(CraftResource resource)
            : base(0x14EC)
        {
            Resource = resource;
            GetRandomLocation();

            UsesRemaining = Utility.RandomMinMax(235, 255);
            ShowUsesRemaining = true;

            Expires = DateTime.UtcNow + TimeSpan.FromHours(DecayPeriod);
            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), CheckDecay);
        }

        public void CheckDecay()
        {
            if (Expires < DateTime.UtcNow)
                Decay();
            else
                InvalidateProperties();
        }

        public void Decay()
        {
            if (RootParent is Mobile)
            {
                Mobile parent = (Mobile)RootParent;

                if (Name == null)
                    parent.SendLocalizedMessage(1072515, "#" + LabelNumber); // The ~1_name~ expired...
                else
                    parent.SendLocalizedMessage(1072515, Name); // The ~1_name~ expired...

                Effects.SendLocationParticles(EffectItem.Create(parent.Location, parent.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(parent.Location, parent.Map, 0x201);
            }
            else
            {
                Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(this.Location, this.Map, 0x201);
            }

            Delete();
        }

        public override void Delete()
        {
            base.Delete();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1152598, String.Format("#{0}\t#{1}", CraftResources.GetLocalizationNumber(Resource), IsMinerMap ? "1152604" : "1152605")); // ~1_RES~ ~2_TYPE~ Map
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int left = 0;
            if (DateTime.UtcNow < Expires)
                left = (int)(Expires - DateTime.UtcNow).TotalSeconds;

            list.Add(1060584, UsesRemaining.ToString()); // uses remaining: ~1_val~
            list.Add(1072517, left.ToString()); // Lifespan: ~1_val~ seconds
            list.Add(1061114, GetSextantLocation()); // Location: ~1_val~

            if (TargetMap == null || TargetMap == Map.Internal)
                list.Add(1060800); // unknown
            else
                list.Add(TargetMap.MapID + 1150548);
        }

        private string GetSextantLocation()
        {
            if (TargetMap == null)
                return "Unknown";

            int x = Target.X;
            int y = Target.Y;

            int xLong = 0, yLat = 0;
            int xMins = 0, yMins = 0;
            bool xEast = false, ySouth = false;

            if (Sextant.Format(new Point3D(x, y, TargetMap.GetAverageZ(x, y)), TargetMap, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
            {
                return String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
            }

            return "Unknown";
        }

        public void GetRandomLocation()
        {
            Map map;

            switch (Utility.Random(6))
            {
                default:
                case 0: map = Map.Felucca; break;
                case 1: map = Map.Trammel; break;
                case 2: map = Map.Ilshenar; break;
                case 3: map = Map.Malas; break;
                case 4: map = Map.Tokuno; break;
                case 5: map = Map.TerMur; break;
            }

            TargetMap = map;
            Dictionary<Map, List<Point2D>> table;

            if (IsMinerMap)
                table = MinerTable;
            else
                table = LumberTable;

            if (!table.ContainsKey(map))
                table[map] = LoadLocsFor(map, this);
            else if (table[map] == null)
                table[map] = LoadLocsFor(map, this);

            Target = table[map][Utility.Random(table[map].Count)];
        }

        public static HarvestMap CheckMapOnHarvest(Mobile from, object harvested, HarvestDefinition def)
        {
            Map map = from.Map;

            if (harvested is IPoint3D && from.Backpack != null)
            {
                IPoint3D p = harvested as IPoint3D;

                Item[] items = from.Backpack.FindItemsByType(typeof(HarvestMap));

                foreach (Item item in items)
                {
                    HarvestMap harvestmap = item as HarvestMap;

                    if (harvestmap != null && harvestmap.TargetMap == map && harvestmap.UsesRemaining > 0 
                        && def.GetBank(map, p.X, p.Y) == def.GetBank(harvestmap.TargetMap, harvestmap.Target.X, harvestmap.Target.Y))
                    {
                        return harvestmap;
                    }
                }
            }

            return null;
        }

        public static Dictionary<Map, List<Point2D>> MinerTable;
        public static Dictionary<Map, List<Point2D>> LumberTable;

        public static void Initialize()
        {
            MinerTable = new Dictionary<Map, List<Point2D>>();
            LumberTable = new Dictionary<Map, List<Point2D>>();
        }

        public static List<Point2D> LoadLocsFor(Map map, HarvestMap hMap)
        {
            string path = String.Format("Data/HarvestLocs/{0}_{1}.cfg", hMap.IsMinerMap ? "MinerLocs" : "LumberLocs", map.ToString());

            if (!File.Exists(path))
            {
                Console.WriteLine("Warning! {0} does not exist for harvest maps...", path);
                return null;
            }

            List<Point2D> list = new List<Point2D>();

            using (StreamReader ip = new StreamReader(path))
            {
                string line;

                while ((line = ip.ReadLine()) != null)
                {
                    if (line.Length == 0 || line.StartsWith("#"))
                        continue;

                    var split = line.Split('\t');

                    int x, y = 0;

                    if (int.TryParse(split[0], out x) && int.TryParse(split[1], out y) && (x > 0 || y > 0))
                    {
                        list.Add(new Point2D(x, y));
                    }
                }
            }

            return list;
        }

        public HarvestMap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Resource);
            writer.Write(Expires);
            writer.Write(UsesRemaining);

            writer.Write(Target);
            writer.Write(TargetMap);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Resource = (CraftResource)reader.ReadInt();
            Expires = reader.ReadDateTime();
            UsesRemaining = reader.ReadInt();

            Target = reader.ReadPoint2D();
            TargetMap = reader.ReadMap();

            if (Expires < DateTime.UtcNow)
                Decay();
            else
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), CheckDecay);
        }
    }
}