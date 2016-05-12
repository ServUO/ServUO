using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;

namespace Server.Services.Virtues
{
	class Honesty
	{
        private static List<Item> HonestyItems = new List<Item>();

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
            for (int i = 0; i < 50; i++)
            {
                var toSpawn = Loot.RandomArmorOrShieldOrWeapon();
                ItemFlags.SetTaken(toSpawn, false);
                toSpawn.HonestyItem = true;
                HonestyItems.Add(toSpawn);

                PlaceItemOnWorld(toSpawn);
            }
        }

	    private static void PlaceItemOnWorld(Item item)
	    {
	        Rectangle2D rect = new Rectangle2D(0, 0, 5119, 4095);
            int x = 0;
            int y = 0;

            var placeCoords = new Point3D();

	        Map map = Utility.RandomBool() ? Map.Trammel : Map.Felucca;

            while (true)
            {
                x = Utility.Random(rect.X, rect.Width);
                y = Utility.Random(rect.Y, rect.Height);

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
