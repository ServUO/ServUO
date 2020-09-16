using Server.Commands;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.ShameRevamped
{
    public static class ShameGenerator
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenerateNewShame", AccessLevel.Administrator, Generate);
            CommandSystem.Register("DeleteShame", AccessLevel.Administrator, Delete);
        }

        private static void Delete(CommandEventArgs e)
        {
            WeakEntityCollection.Delete("newshame");
            ResetOldSpawners(false);
        }

        public static void Generate(CommandEventArgs e)
        {
            RemoveItems();

            CommandSystem.Handle(e.Mobile, CommandSystem.Prefix + "XmlLoad RevampedSpawns/ShameRevamped.xml");

            // Level 1
            Point3D altarLoc = new Point3D(5403, 43, 30);

            if (!CheckForAltar(altarLoc, Map.Trammel))
            {
                ShameAltar altar = new ShameAltar(typeof(QuartzElemental), new Point3D(5490, 19, -25), new Point3D(5514, 10, 5), new Point3D(5387, 11, 30), 10);
                WeakEntityCollection.Add("newshame", altar);
                altar.MoveToWorld(altarLoc, Map.Trammel);
            }

            if (!CheckForAltar(altarLoc, Map.Felucca))
            {
                ShameAltar altar = new ShameAltar(typeof(QuartzElemental), new Point3D(5490, 19, -25), new Point3D(5514, 10, 5), new Point3D(5387, 11, 30), 10);
                WeakEntityCollection.Add("newshame", altar);
                altar.MoveToWorld(altarLoc, Map.Felucca);
            }

            // Level 2
            altarLoc = new Point3D(5577, 54, 2);

            if (!CheckForAltar(altarLoc, Map.Trammel))
            {
                ShameAltar altar = new ShameAltar(typeof(FlameElemental), new Point3D(5604, 102, 5), new Point3D(5514, 147, 25), new Point3D(5571, 115, 3), 20);
                WeakEntityCollection.Add("newshame", altar);
                altar.MoveToWorld(altarLoc, Map.Trammel);
            }

            if (!CheckForAltar(altarLoc, Map.Felucca))
            {
                ShameAltar altar = new ShameAltar(typeof(FlameElemental), new Point3D(5604, 102, 5), new Point3D(5514, 147, 25), new Point3D(5571, 115, 3), 20);
                WeakEntityCollection.Add("newshame", altar);
                altar.MoveToWorld(altarLoc, Map.Felucca);
            }

            // Level 3
            altarLoc = new Point3D(5390, 145, 20);

            if (!CheckForAltar(altarLoc, Map.Trammel))
            {
                ShameAltar altar = new ShameAltar(typeof(WindElemental), new Point3D(5538, 170, 5), new Point3D(5513, 176, 5), new Point3D(5618, 223, 0), 30);
                WeakEntityCollection.Add("newshame", altar);
                altar.MoveToWorld(altarLoc, Map.Trammel);
            }

            if (!CheckForAltar(altarLoc, Map.Felucca))
            {
                ShameAltar altar = new ShameAltar(typeof(WindElemental), new Point3D(5538, 170, 5), new Point3D(5513, 176, 5), new Point3D(5618, 223, 0), 30);
                WeakEntityCollection.Add("newshame", altar);
                altar.MoveToWorld(altarLoc, Map.Felucca);
            }

            // Wall 1
            Dictionary<Point3D, int> dictionary = null;
            altarLoc = new Point3D(5403, 82, 10);

            if (!CheckForAltar(altarLoc, Map.Trammel))
            {
                dictionary = new Dictionary<Point3D, int>();
                dictionary[new Point3D(-1, 0, 0)] = 2272;
                dictionary[new Point3D(0, 0, 0)] = 2272;
                dictionary[new Point3D(2, 0, 0)] = 2272;
                dictionary[new Point3D(1, 0, 0)] = 2272;

                ShameWall wall = new ShameWall(dictionary, altarLoc, new Point3D(5405, 90, 10), Map.Trammel);
                WeakEntityCollection.Add("newshame", wall);
                wall.MoveToWorld(altarLoc, Map.Trammel);
                ShameWall.AddTeleporters(wall);
            }

            if (!CheckForAltar(altarLoc, Map.Felucca))
            {
                dictionary = new Dictionary<Point3D, int>();
                dictionary[new Point3D(-1, 0, 0)] = 2272;
                dictionary[new Point3D(0, 0, 0)] = 2272;
                dictionary[new Point3D(2, 0, 0)] = 2272;
                dictionary[new Point3D(1, 0, 0)] = 2272;

                ShameWall wall = new ShameWall(dictionary, altarLoc, new Point3D(5405, 90, 10), Map.Felucca);
                WeakEntityCollection.Add("newshame", wall);
                wall.MoveToWorld(altarLoc, Map.Felucca);
                ShameWall.AddTeleporters(wall);
            }

            // Wall 2
            altarLoc = new Point3D(5465, 26, -10);

            if (!CheckForAltar(altarLoc, Map.Trammel))
            {
                dictionary = new Dictionary<Point3D, int>();
                dictionary[new Point3D(0, -1, 0)] = 2272;
                dictionary[new Point3D(0, 0, 0)] = 2272;
                dictionary[new Point3D(0, 1, 0)] = 2272;
                dictionary[new Point3D(0, 2, 0)] = 2272;

                ShameWall wall = new ShameWall(dictionary, altarLoc, new Point3D(5472, 26, -30), Map.Trammel);
                WeakEntityCollection.Add("newshame", wall);
                wall.MoveToWorld(altarLoc, Map.Trammel);
                ShameWall.AddTeleporters(wall);
            }

            if (!CheckForAltar(altarLoc, Map.Felucca))
            {
                dictionary = new Dictionary<Point3D, int>();
                dictionary[new Point3D(0, -1, 0)] = 2272;
                dictionary[new Point3D(0, 0, 0)] = 2272;
                dictionary[new Point3D(0, 1, 0)] = 2272;
                dictionary[new Point3D(0, 2, 0)] = 2272;

                ShameWall wall = new ShameWall(dictionary, altarLoc, new Point3D(5472, 26, -30), Map.Felucca);
                WeakEntityCollection.Add("newshame", wall);
                wall.MoveToWorld(altarLoc, Map.Felucca);
                ShameWall.AddTeleporters(wall);
            }

            // Wall 3
            altarLoc = new Point3D(5619, 57, 0);

            if (!CheckForAltar(altarLoc, Map.Trammel))
            {
                dictionary = new Dictionary<Point3D, int>();
                dictionary[new Point3D(-1, 0, 0)] = 1059;
                dictionary[new Point3D(0, 0, 0)] = 1059;
                dictionary[new Point3D(1, 0, 0)] = 1059;

                ShameWall wall = new ShameWall(dictionary, altarLoc, new Point3D(5621, 43, 0), Map.Trammel);
                WeakEntityCollection.Add("newshame", wall);
                wall.MoveToWorld(altarLoc, Map.Trammel);
                ShameWall.AddTeleporters(wall);
            }

            if (!CheckForAltar(altarLoc, Map.Felucca))
            {
                dictionary = new Dictionary<Point3D, int>();
                dictionary[new Point3D(-1, 0, 0)] = 1059;
                dictionary[new Point3D(0, 0, 0)] = 1059;
                dictionary[new Point3D(1, 0, 0)] = 1059;

                ShameWall wall = new ShameWall(dictionary, altarLoc, new Point3D(5621, 43, 0), Map.Felucca);
                WeakEntityCollection.Add("newshame", wall);
                wall.MoveToWorld(altarLoc, Map.Felucca);
                ShameWall.AddTeleporters(wall);
            }

            e.Mobile.SendMessage("Shame Revamped setup!");
        }

        public static void RemoveItems()
        {
            RemoveItem(new Point3D(5490, 19, -25), Map.Trammel, typeof(Teleporter));
            RemoveItem(new Point3D(5490, 19, -25), Map.Felucca, typeof(Teleporter));

            RemoveItem(new Point3D(5604, 102, 5), Map.Trammel, typeof(Teleporter));
            RemoveItem(new Point3D(5604, 102, 5), Map.Felucca, typeof(Teleporter));

            RemoveItem(new Point3D(5538, 170, 5), Map.Trammel, typeof(Teleporter));
            RemoveItem(new Point3D(5538, 170, 5), Map.Felucca, typeof(Teleporter));

            ResetOldSpawners();
        }

        public static void ResetOldSpawners(bool reset = true)
        {
            Region r = Region.Find(new Point3D(5538, 170, 5), Map.Trammel);
            foreach (XmlSpawner spawner in r.GetEnumeratedItems().OfType<XmlSpawner>())
            {
                if (spawner.Name.ToLower() != "shame_revamped" && spawner.Name.ToLower() != "shame_chest")
                {
                    if (reset)
                        spawner.DoReset = true;
                    else
                        spawner.DoRespawn = true;
                }
            }

            r = Region.Find(new Point3D(5538, 170, 5), Map.Felucca);
            foreach (XmlSpawner spawner in r.GetEnumeratedItems().OfType<XmlSpawner>())
            {
                if (spawner.Name.ToLower() != "shame_revamped" && spawner.Name.ToLower() != "shame_chest")
                {
                    if (reset)
                        spawner.DoReset = true;
                    else
                        spawner.DoRespawn = true;
                }
            }
        }

        public static bool RemoveItem(Point3D p, Map map, Type t)
        {
            IPooledEnumerable eable = map.GetItemsInRange(p, 0);

            foreach (Item i in eable)
            {
                if (i.GetType() == t)
                {
                    i.Delete();
                    eable.Free();
                    return true;
                }
            }
            eable.Free();
            return false;
        }

        public static bool CheckForAltar(Point3D p, Map map)
        {
            IPooledEnumerable eable = map.GetItemsInRange(p, 0);

            foreach (Item i in eable)
            {
                if (i is ShameAltar || i is ShameWall)
                {
                    eable.Free();
                    return true;
                }
            }
            eable.Free();

            return false;
        }
    }
}
