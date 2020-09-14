#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Server.Engines.CannedEvil;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Regions;
#endregion

namespace Server.Services.Virtues
{
    public static class HonestyVirtue
    {
        public static bool Enabled { get; set; }
        public static int MaxGeneration { get; set; }
        public static bool TrammelGeneration { get; set; }
        public static bool UseSpawnArea { get; set; }

        private static readonly string[] _Regions =
            {"Britain", "Minoc", "Magincia", "Trinsic", "Jhelom", "Moonglow", "Skara Brae", "Yew"};

        private const TileFlag _Filter = TileFlag.Wet | TileFlag.Roof | TileFlag.Impassable;

        private static readonly HashSet<Item> _Items;

        private static SpawnArea _FeluccaArea, _TrammelArea;

        static HonestyVirtue()
        {
            Enabled = Config.Get("Honesty.Enabled", true);
            MaxGeneration = Config.Get("Honesty.MaxGeneration", 1000);
            TrammelGeneration = !Siege.SiegeShard && Config.Get("Honesty.TrammelGeneration", true);
            UseSpawnArea = Config.Get("Honesty.UseSpawnArea", true);

            _Items = new HashSet<Item>(MaxGeneration);
        }

        private static void GenerateImages()
        {
            if (!Directory.Exists("Honesty"))
            {
                Directory.CreateDirectory("Honesty");
            }

            if (_FeluccaArea != null)
            {
                _FeluccaArea.Image.Save("Honesty/Felucca.png", ImageFormat.Png);
            }

            if (_TrammelArea != null)
            {
                _TrammelArea.Image.Save("Honesty/Trammel.png", ImageFormat.Png);
            }
        }

        public static void Initialize()
        {
            EventSink.ItemDeleted += OnItemDeleted;
            EventSink.AfterWorldSave += OnAfterSave;

            VirtueGump.Register(106, OnVirtueUsed);

            if (Enabled)
            {
                _Items.UnionWith(World.Items.Values.Where(item => item.HonestyItem));

                GenerateHonestyItems();

                if (UseSpawnArea)
                {
                    Task.Factory.StartNew(GenerateImages);
                }
            }
        }

        public static bool IsHonestyItem(Item item)
        {
            return item.HasSocket<HonestyItemSocket>();
        }

        private static void OnItemDeleted(ItemDeletedEventArgs e)
        {
            if (IsHonestyItem(e.Item))
            {
                _Items.Remove(e.Item);
            }
        }

        private static void OnAfterSave(AfterWorldSaveEventArgs e)
        {
            World.WaitForWriteCompletion();

            PruneTaken();

            if (Enabled)
            {
                GenerateHonestyItems();
            }
        }

        public static void OnVirtueUsed(Mobile from)
        {
            if (Enabled)
            {
                from.SendLocalizedMessage(1053001); // This virtue is not activated through the virtue menu.
            }
        }

        private static void PruneTaken()
        {
            _Items.RemoveWhere(ItemFlags.GetTaken);
        }

        private static Point3D GetRandom(Map map)
        {
            if (map == null)
                return Point3D.Zero;

            int fw = map.MapID <= 1 ? 5119 : map.Width;
            int fh = map.MapID <= 1 ? 4095 : map.Height;

            int x, y, z;

            do
            {
                x = Utility.RandomMinMax(0, fw);
                y = Utility.RandomMinMax(0, fh);
                z = map.Tiles.GetLandTile(x, y).Z;
            }
            while (!ValidateSpawnPoint(map, x, y, z));

            return new Point3D(x, y, z);
        }

        private static bool ValidateSpawnPoint(Map map, int x, int y, int z)
        {
            LandTile lt = map.Tiles.GetLandTile(x, y);
            LandData ld = TileData.LandTable[lt.ID];

            if (lt.Ignored || (ld.Flags & TileFlag.Impassable) > 0 || (ld.Flags & TileFlag.Wet) > 0 || (ld.Flags & TileFlag.Roof) > 0)
            {
                return false;
            }

            for (int i = 0; i < HousePlacement.RoadIDs.Length; i += 2)
            {
                if (lt.ID >= HousePlacement.RoadIDs[i] && lt.ID <= HousePlacement.RoadIDs[i + 1])
                {
                    return false;
                }
            }

            Point3D p = new Point3D(x, y, lt.Z);

            Region reg = Region.Find(p, map);

            //no-go in towns, houses, dungeons and champspawns
            if (reg != null && (reg.IsPartOf<TownRegion>() || reg.IsPartOf<DungeonRegion>() || reg.IsPartOf<ChampionSpawnRegion>() || reg.IsPartOf<HouseRegion>()))
            {
                return false;
            }

            //check for house within 5 tiles
            for (p.X = x - 5; p.X <= x + 5; p.X++)
            {
                for (p.Y = y - 5; p.Y <= y + 5; p.Y++)
                {
                    if (BaseHouse.FindHouseAt(p, map, Region.MaxZ - p.Z) != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void GenerateHonestyItems()
        {
            CheckChests();

            Utility.PushColor(ConsoleColor.Yellow);
            Console.WriteLine("[Honesty]: Generating...");
            Utility.PopColor();

            Stopwatch sw = new Stopwatch();
            double s = 0.0;

            if (UseSpawnArea)
            {
                if (_FeluccaArea == null)
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.Write("[Honesty]: Felucca - Reticulating splines...");
                    Utility.PopColor();

                    sw.Restart();

                    _FeluccaArea = SpawnArea.Instantiate(Map.Felucca.DefaultRegion, _Filter, ValidateSpawnPoint, true);

                    sw.Stop();

                    s += sw.Elapsed.TotalSeconds;

                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine("done ({0:F2} seconds)", sw.Elapsed.TotalSeconds);
                    Utility.PopColor();
                }

                if (_TrammelArea == null && TrammelGeneration)
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.Write("[Honesty]: Trammel - Reticulating splines...");
                    Utility.PopColor();

                    sw.Restart();

                    _TrammelArea = SpawnArea.Instantiate(Map.Trammel.DefaultRegion, _Filter, ValidateSpawnPoint, true);

                    sw.Stop();

                    s += sw.Elapsed.TotalSeconds;

                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine("done ({0:F2} seconds)", sw.Elapsed.TotalSeconds);
                    Utility.PopColor();
                }
            }

            try
            {
                Map facet;
                Point3D loc;
                Item item;

                int count = MaxGeneration - _Items.Count;

                if (count > 0)
                {
                    Utility.PushColor(ConsoleColor.Yellow);
                    Console.Write("[Honesty]: Creating {0:#,0} lost items...", count);
                    Utility.PopColor();

                    sw.Restart();

                    Item[] spawned = new Item[count];

                    for (int i = 0; i < spawned.Length; i++)
                    {
                        try
                        {
                            item = Loot.RandomArmorOrShieldOrWeapon();

                            if (item != null && !item.Deleted)
                            {
                                spawned[i] = item;
                            }
                        }
                        catch (Exception e)
                        {
                            Server.Diagnostics.ExceptionLogging.LogException(e);
                        }
                    }

                    for (int i = 0; i < spawned.Length; i++)
                    {
                        item = spawned[i];

                        if (item == null)
                        {
                            continue;
                        }

                        try
                        {
                            if (UseSpawnArea)
                            {
                                SpawnArea area = _TrammelArea != null && Utility.RandomBool() ? _TrammelArea : _FeluccaArea;
                                facet = area.Facet;

                                loc = area.GetRandom();
                            }
                            else
                            {
                                facet = TrammelGeneration && Utility.RandomBool() ? Map.Trammel : Map.Felucca;
                                loc = GetRandom(facet);
                            }

                            if (loc == Point3D.Zero)
                            {
                                continue;
                            }

                            RunicReforging.GenerateRandomItem(item, 0, 100, 1000);

                            item.AttachSocket(new HonestyItemSocket());
                            item.HonestyItem = true;

                            _Items.Add(item);

                            ItemFlags.SetTaken(item, false);

                            item.OnBeforeSpawn(loc, facet);
                            item.MoveToWorld(loc, facet);
                            item.OnAfterSpawn();
                        }
                        catch (Exception e)
                        {
                            item.Delete();
                            Server.Diagnostics.ExceptionLogging.LogException(e);
                        }
                        finally
                        {
                            spawned[i] = null;
                        }
                    }

                    sw.Stop();

                    s += sw.Elapsed.TotalSeconds;

                    Utility.PushColor(ConsoleColor.Green);
                    Console.WriteLine("done ({0:F2} seconds)", sw.Elapsed.TotalSeconds);
                    Utility.PopColor();
                }
            }
            catch (Exception e)
            {
                Server.Diagnostics.ExceptionLogging.LogException(e);
            }

            Utility.PushColor(ConsoleColor.Yellow);
            Console.Write("[Honesty]:");
            Utility.PopColor();
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine(" Generation completed in {0:F2} seconds.", s);
            Utility.PopColor();
        }

        public static void AssignOwner(HonestyItemSocket socket)
        {
            if (socket != null)
            {
                socket.HonestyRegion = _Regions[Utility.Random(_Regions.Length)];

                if (!string.IsNullOrWhiteSpace(socket.HonestyRegion) && BaseVendor.AllVendors.Count >= 10)
                {
                    List<BaseVendor> matchedVendors = BaseVendor.AllVendors.Where(vendor => (vendor.Map == socket.Owner.Map && vendor.Region.IsPartOf(socket.HonestyRegion))).ToList();

                    if (matchedVendors.Count > 0)
                    {
                        socket.HonestyOwner = matchedVendors[Utility.Random(matchedVendors.Count)];
                    }
                    else
                    {
                        // fallback in case there are no vendors generated in the specific region
                        socket.HonestyOwner = BaseVendor.AllVendors[Utility.Random(BaseVendor.AllVendors.Count)];
                    }
                }
            }
        }

        private static void CheckChests()
        {
            foreach (Point3D loc in _ChestLocations)
            {
                CheckLocation(loc, Map.Trammel);
                CheckLocation(loc, Map.Felucca);
            }
        }

        private static void CheckLocation(Point3D pnt, Map map)
        {
            IPooledEnumerable<Item> eable = map.GetItemsInRange(pnt, 0);

            foreach (Item item in eable)
            {
                if (item is HonestyChest)
                {
                    eable.Free();
                    return;
                }
            }

            eable.Free();

            HonestyChest chest = new HonestyChest();

            chest.MoveToWorld(pnt, map);
        }

        private static readonly Point3D[] _ChestLocations =
        {
            new Point3D(1809, 2825, 0), new Point3D(1323, 3768, 0), new Point3D(3784, 2247, 20), new Point3D(591, 2144, 0),
            new Point3D(1648, 1603, 20), new Point3D(2509, 544, 0), new Point3D(4463, 1156, 0), new Point3D(650, 824, 0)
        };
    }

    public class HonestyChest : Container
    {
        public override int LabelNumber => 1151529;  // lost and found box

        [Constructable]
        public HonestyChest()
            : base(0x9A9)
        {
            Movable = false;
        }

        public HonestyChest(Serial serial)
            : base(serial)
        { }

        public override void OnDoubleClick(Mobile m)
        { }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            return CheckGain(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            return CheckGain(from, item);
        }

        public bool CheckGain(Mobile from, Item item)
        {
            if (from == null || from.Deleted)
            {
                return false;
            }

            if (item == null || !item.HonestyItem)
            {
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1151530, from.NetState); // This is not a lost item.
                return false;
            }

            Region reg = Region.Find(Location, Map);

            bool gainedPath = false;
            HonestyItemSocket honestySocket = item.GetSocket<HonestyItemSocket>();

            if (honestySocket != null && honestySocket.HonestyRegion == reg.Name)
            {
                VirtueHelper.Award(from, VirtueName.Honesty, 60, ref gainedPath);
            }
            else
            {
                VirtueHelper.Award(from, VirtueName.Honesty, 30, ref gainedPath);
            }

            PrivateOverheadMessage(
                MessageType.Regular,
                0x3B2,
                1151523,
                from.NetState); // You place the item in the lost and found.  You have gained some Honesty!

            if (gainedPath)
            {
                from.SendMessage("You have gained a path in Honesty!");
            }

            item.Delete();

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}
