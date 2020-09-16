using Server.Engines.CityLoyalty;
using Server.Mobiles;
using Server.Items;
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Regions
{
    public class TeleportRegion : BaseRegion, ITeleporter
    {
        public static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(400);

        public Dictionary<WorldLocation, WorldLocation> TeleLocs { get; set; }

        public TeleportRegion(string name, Map map, Rectangle3D[] recs, Dictionary<WorldLocation, WorldLocation> points)
            : base(name, map, GetParent(recs, map), recs)
        {
            TeleLocs = points;

            if (points != null)
            {
                GoLocation = points.Keys.FirstOrDefault(l => l.Location != Point3D.Zero).Location;
            }
        }

        public TeleportRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        private static Region GetParent(Rectangle3D[] recs, Map map)
        {
            return Find(new Point3D(recs[0].Start.X, recs[0].Start.Y, recs[0].Start.Z), map);
        }

        public override void OnEnter(Mobile m)
        {
            if (m is PlayerMobile && m.CanBeginAction(typeof(Teleporter)) && !CityTradeSystem.HasTrade(m))
            {
                DoTeleport(m);
            }
        }

        public virtual void DoTeleport(Mobile m)
        {
            WorldLocation loc = TeleLocs.Keys.FirstOrDefault(l => l.Location.X == m.X && l.Location.Y == m.Y && l.Location.Z >= m.Z - 5 && l.Location.Z <= m.Z + 5 && l.Map == m.Map);

            if (loc != null)
            {
                var destinationPoint = TeleLocs[loc].Location;
                var destinationMap = TeleLocs[loc].Map;

                if (destinationPoint != Point3D.Zero && destinationMap != null && destinationMap != Map.Internal)
                {
                    m.BeginAction(typeof(Teleporter));
                    m.Frozen = true;

                    Timer.DelayCall(TimeSpan.FromMilliseconds(400), () =>
                    {
                        BaseCreature.TeleportPets(m, destinationPoint, destinationMap);
                        m.MoveToWorld(destinationPoint, destinationMap);

                        m.Frozen = false;

                        Timer.DelayCall(TimeSpan.FromMilliseconds(250), () => m.EndAction(typeof(Teleporter)));
                    });
                }
            }
        }

        public static void Initialize()
        {
            Timer.DelayCall(() =>
            {
                string filePath = Path.Combine("Data", "TeleporterRegions.xml");

                if (!File.Exists(filePath))
                    return;

                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                XmlElement root = doc["TeleporterRegions"];
                var unique = 1;

                BuildTeleporters("Teleporter", root, ref unique);

                if (Siege.SiegeShard)
                {
                    BuildTeleporters("SiegeTeleporter", root, ref unique);
                }

                Console.WriteLine("Initialized {0} Teleporter Regions.", (unique - 1).ToString());
            });
        }

        private static void BuildTeleporters(string elementName, XmlElement root, ref int unique)
        {
            foreach (XmlElement region in root.GetElementsByTagName(elementName))
            {
                var list = new Dictionary<WorldLocation, WorldLocation>();

                Map locMap = null;
                Map teleMap = null;

                foreach (XmlElement tile in region.GetElementsByTagName("tiles"))
                {
                    Point3D from = Point3D.Parse(Utility.GetAttribute(tile, "from", "(0, 0, 0)"));
                    Map fromMap = Map.Parse(Utility.GetAttribute(tile, "frommap", null));

                    Point3D to = Point3D.Parse(Utility.GetAttribute(tile, "to", "(0, 0, 0)"));
                    Map toMap = Map.Parse(Utility.GetAttribute(tile, "tomap", null));

                    int id = Utility.ToInt32(Utility.GetAttribute(tile, "ItemID", "-1"));
                    int hue = Utility.ToInt32(Utility.GetAttribute(tile, "Hue", "-1"));

                    if (fromMap == null)
                    {
                        throw new ArgumentException(string.Format("Map parsed as null: {0}", from));
                    }

                    if (toMap == null)
                    {
                        throw new ArgumentException(string.Format("Map parsed as null: {0}", to));
                    }

                    if (Siege.SiegeShard && (fromMap == Map.Trammel || toMap == Map.Trammel))
                    {
                        continue;
                    }

                    list.Add(new WorldLocation(from, fromMap), new WorldLocation(to, toMap));

                    if (list.Count == 1)
                    {
                        locMap = fromMap;
                        teleMap = toMap;
                    }

                    if (id > -1)
                    {
                        if (!fromMap.FindItems<Static>(from, 0).Any(s => s.ItemID == id))
                        {
                            var st = new Static(id);

                            if (hue > -1)
                            {
                                st.Hue = hue;
                            }

                            st.MoveToWorld(from, fromMap);
                        }
                    }
                }

                if (list.Count > 0)
                {
                    Rectangle3D[] recs = new Rectangle3D[list.Count];
                    var i = 0;

                    foreach (var kvp in list)
                    {
                        recs[i++] = new Rectangle3D(kvp.Key.Location.X, kvp.Key.Location.Y, kvp.Key.Location.Z - 5, 1, 1, 10);
                    }

                    TeleportRegion teleRegion;

                    if (!Siege.SiegeShard && locMap != null && locMap.Rules != MapRules.FeluccaRules && teleMap.Rules == MapRules.FeluccaRules)
                    {
                        teleRegion = new TeleportRegionPVPWarning(string.Format("Teleport Region {0}", unique.ToString()), locMap, recs, list);
                    }
                    else
                    {
                        teleRegion = new TeleportRegion(string.Format("Teleport Region {0}", unique.ToString()), locMap, recs, list);
                    }

                    teleRegion.Register();

                    unique++;
                }
            }
        }
    }

    public class TeleportRegionPVPWarning : TeleportRegion
    {
        public TeleportRegionPVPWarning(string name, Map map, Rectangle3D[] recs, Dictionary<WorldLocation, WorldLocation> points)
            : base(name, map, recs, points)
        {
        }

        public override void OnEnter(Mobile m)
        {
            if (m.CanBeginAction(typeof(Teleporter)))
            {
                var pm = m as PlayerMobile;

                if (pm != null)
                {
                    if (pm.DisabledPvpWarning)
                    {
                        DoTeleport(m);
                    }
                    else if (!pm.HasGump(typeof(PvpWarningGump)))
                    {
                        pm.SendGump(new PvpWarningGump(m, this));
                    }
                }
            }
        }
    }
}
