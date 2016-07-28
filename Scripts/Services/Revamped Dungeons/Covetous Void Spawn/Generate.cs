using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Commands;
using Server.Items;
using System.Linq;

namespace Server.Engines.VoidPool
{
	public static class Generate
	{
        public static void Initialize()
        {
            CommandSystem.Register("SetupNewCovetous", AccessLevel.Administrator, Setup);
            CommandSystem.Register("DeleteCovetous", AccessLevel.Administrator, Delete);
        }

        private static void Delete(CommandEventArgs e)
        {
            WeakEntityCollection.Delete("newcovetous");
            VoidPoolController.InstanceTram = null;
            VoidPoolController.InstanceFel = null;
        }

        public static void Setup(CommandEventArgs e)
        {
            if (VoidPoolController.InstanceTram != null || VoidPoolController.InstanceFel != null)
                e.Mobile.SendMessage("This has already been setup!");
            else
            {
                var one = new VoidPoolController(Map.Trammel);
                WeakEntityCollection.Add("newcovetous", one);
                one.MoveToWorld(new Point3D(5605, 1998, 10), Map.Trammel);

                var two = new VoidPoolController(Map.Felucca);
                WeakEntityCollection.Add("newcovetous", two);
                two.MoveToWorld(new Point3D(5605, 1998, 10), Map.Felucca);

                int id = 0;
                int hue = 0;

                for (int x = 5497; x <= 5503; x++)
                {
                    for (int y = 1995; y <= 2001; y++)
                    {
                        if (x == 5497 && y == 1995) id = 1886;
                        else if (x == 5497 && y == 2001) id = 1887;
                        else if (x == 5503 && y == 1995) id = 1888;
                        else if (x == 5503 && y == 2001) id = 1885;
                        else if (x == 5497) id = 1874;
                        else if (x == 5503) id = 1876;
                        else if (y == 1995) id = 1873;
                        else if (y == 2001) id = 1875;
                        else
                        {
                            //id = 1168;
                            id = Utility.Random(8511, 6);
                        }

                        hue = id >= 8511 ? 0 : 1954;

                        var item = new Static(id);
                        item.Name = "Void Pool";
                        item.Hue = hue;
                        WeakEntityCollection.Add("newcovetous", item);
                        item.MoveToWorld(new Point3D(x, y, 5), Map.Trammel);

                        item = new Static(id);
                        item.Name = "Void Pool";
                        item.Hue = hue;
                        WeakEntityCollection.Add("newcovetous", item);
                        item.MoveToWorld(new Point3D(x, y, 5), Map.Felucca);
                    }
                }

                AddWaypoints();
            }
        }

        public static void AddWaypoints()
        {
            var one = VoidPoolController.InstanceTram;
            var two = VoidPoolController.InstanceFel;

            if (one == null || two == null)
                return;

            foreach (var w in one.WaypointsA.Where(w => w != null && !w.Deleted)) w.Delete();
            foreach (var w in one.WaypointsB.Where(w => w != null && !w.Deleted)) w.Delete();

            foreach(var w in two.WaypointsA.Where(w => w != null && !w.Deleted)) w.Delete();
            foreach (var w in two.WaypointsB.Where(w => w != null && !w.Deleted)) w.Delete();

            // patha
            var wp = new WayPoint();
            wp.MoveToWorld(new Point3D(5590, 2024, 0), Map.Trammel);
            one.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5590, 2024, 0), Map.Felucca);
            two.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5578, 2029, 0), Map.Trammel);
            one.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5578, 2029, 0), Map.Felucca);
            two.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5566, 2027, 0), Map.Trammel);
            one.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5566, 2027, 0), Map.Felucca);
            two.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5555, 2021, 0), Map.Trammel);
            one.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5555, 2021, 0), Map.Felucca);
            two.WaypointsA.Add(wp);

            wp = new WayPoint();
            wp.MoveToWorld(new Point3D(5545, 2015, 0), Map.Trammel);
            one.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5545, 2015, 0), Map.Felucca);
            two.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5537, 2020, 0), Map.Trammel);
            one.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5537, 2020, 0), Map.Felucca);
            two.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5527, 2015, 0), Map.Trammel);
            one.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5527, 2015, 0), Map.Felucca);
            two.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5509, 2005, 0), Map.Trammel);
            one.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5509, 2005, 0), Map.Felucca);
            two.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5500, 1998, 0), Map.Trammel);
            one.WaypointsA.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5500, 1998, 0), Map.Felucca);
            two.WaypointsA.Add(wp);

            // pathb
            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5469, 2016, 0), Map.Trammel);
            one.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5469, 2016, 0), Map.Felucca);
            two.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5478, 2025, 0), Map.Trammel);
            one.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5478, 2025, 0), Map.Felucca);
            two.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5484, 2029, 0), Map.Trammel);
            one.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5484, 2029, 0), Map.Felucca);
            two.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5490, 2027, 0), Map.Trammel);
            one.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5490, 2027, 0), Map.Felucca);
            two.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5504, 2027, 0), Map.Trammel);
            one.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5504, 2027, 0), Map.Felucca);
            two.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5516, 2020, 0), Map.Trammel);
            one.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5516, 2020, 0), Map.Felucca);
            two.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5524, 2012, 0), Map.Trammel);
            one.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5524, 2012, 0), Map.Felucca);
            two.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5513, 2005, 0), Map.Trammel);
            one.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5513, 2005, 0), Map.Felucca);
            two.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5502, 2004, 0), Map.Trammel);
            one.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5502, 2004, 0), Map.Felucca);
            two.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5500, 1998, 0), Map.Trammel);
            one.WaypointsB.Add(wp);

            wp = new WayPoint();
            WeakEntityCollection.Add("newcovetous", wp);
            wp.MoveToWorld(new Point3D(5500, 1998, 0), Map.Felucca);
            two.WaypointsB.Add(wp);

            for (int i = 0; i < one.WaypointsA.Count; i++)
            {
                WayPoint waypoint = one.WaypointsA[i];

                if (i < one.WaypointsA.Count - 1)
                    waypoint.NextPoint = one.WaypointsA[i + 1];
            }

            for (int i = 0; i < one.WaypointsB.Count; i++)
            {
                WayPoint waypoint = one.WaypointsB[i];

                if (i < one.WaypointsB.Count - 1)
                    waypoint.NextPoint = one.WaypointsB[i + 1];
            }

            for (int i = 0; i < two.WaypointsA.Count; i++)
            {
                WayPoint waypoint = two.WaypointsA[i];

                if (i < two.WaypointsA.Count - 1)
                    waypoint.NextPoint = two.WaypointsA[i + 1];
            }

            for (int i = 0; i < two.WaypointsB.Count; i++)
            {
                WayPoint waypoint = two.WaypointsB[i];

                if (i < two.WaypointsB.Count - 1)
                    waypoint.NextPoint = two.WaypointsB[i + 1];
            }

            one.WaypointACount = one.WaypointsA.Count;
            one.WaypointBCount = one.WaypointsB.Count;

            two.WaypointACount = two.WaypointsA.Count;
            two.WaypointBCount = two.WaypointsB.Count;
        }
	}
}