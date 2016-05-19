using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Server.Items;

namespace Server.Services.Virtues
{
    class Honesty
    {
        private static List<Item> _HonestyItems = new List<Item>();
        static List<Point2D> felPoints = new List<Point2D>();
        List<Point2D> tramPoints = new List<Point2D>();
        public static void Initialize()
        {
            VirtueGump.Register(106, OnVirtueUsed);
            EventSink.AfterWorldSave += EventSinkAfterWorldSave;
        }


        public static void OnVirtueUsed(Mobile from)
        {
            from.SendLocalizedMessage(1053001); // This virtue is not activated through the virtue menu.
        }

        private static void EventSinkAfterWorldSave(AfterWorldSaveEventArgs worldSaveEventArgs)
        {

            //Heavy operation, let's use a seperate thread.
            GenerateHonestyItems();

        }

        private static void GenerateHonestyItems()
        {
            
            foreach (string line in File.ReadLines(Path.Combine("Data", "Felucca.MapPoints")))
            {
                var coords = line.Split(',');

                Point2D p2d = new Point2D(Int32.Parse(coords[0]), Int32.Parse(coords[1]));
                felPoints.Add(p2d);
            }

            if (_HonestyItems.Count == 0)
            {
                var list =
                    World.Items.Values.Where(
                        m =>
                            m.HonestyItem).ToList();

                _HonestyItems.AddRange(list);
            }

            foreach (Item i in _HonestyItems)
            {
                if (ItemFlags.GetTaken(i))
                {
                    _HonestyItems.Remove(i);
                }
            }

            while (_HonestyItems.Count < 1000)
            {
                Item toSpawn = Loot.RandomArmorOrShieldOrWeapon();
                ItemFlags.SetTaken(toSpawn, false);
                PlaceItemOnWorld(toSpawn);
                _HonestyItems.Add(toSpawn);
            }

            //Required because of the world mobiles collection changing outside of the thread. 
            foreach (Item i in _HonestyItems)
            {
                i.HonestyItem = true;
            }

            felPoints.Clear();
            felPoints.TrimExcess();
        }

        private static void PlaceItemOnWorld(Item item)
        {
            Rectangle2D rect = new Rectangle2D(0, 0, 5119, 4095);

            var placeCoords = new Point3D();

            Map map = Utility.RandomBool() ? Map.Trammel : Map.Felucca;

            while (true)
            {
                
                var point = felPoints[Utility.Random(felPoints.Count - 1)];

                if (!TreasureMap.ValidateLocation(point.X, point.Y, Map.Felucca)) continue;
                placeCoords.X = point.X;
                placeCoords.Y = point.Y;
                placeCoords.Z = map.GetAverageZ(point.X, point.Y);

                break;
            }

            item.MoveToWorld(placeCoords, map);
        }

    }
}
