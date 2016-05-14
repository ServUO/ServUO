using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Items;

namespace Server.Services.Virtues
{
    class Honesty
    {
        private static List<Item> _HonestyItems = new List<Item>();

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
            Task.Factory.StartNew(GenerateHonestyItems);

        }

        private static void GenerateHonestyItems()
        {
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

            //Does not work in above loop, throws collection changed exception. Investigate later. 
            foreach (Item i in _HonestyItems)
            {
                i.HonestyItem = true;
            }
        }

        private static void PlaceItemOnWorld(Item item)
        {
            Rectangle2D rect = new Rectangle2D(0, 0, 5119, 4095);

            var placeCoords = new Point3D();

            Map map = Utility.RandomBool() ? Map.Trammel : Map.Felucca;

            while (true)
            {
                var x = Utility.Random(rect.X, rect.Width);
                var y = Utility.Random(rect.Y, rect.Height);

                if (!TreasureMap.ValidateLocation(x, y, map)) continue;
                placeCoords.X = x;
                placeCoords.Y = y;
                placeCoords.Z = map.GetAverageZ(x, y);

                break;
            }

            item.MoveToWorld(placeCoords, map);
        }

    }
}
