using System;
using System.Collections;
using Server.Items;

namespace Server.Commands
{
    public class GenTeleporterDelete
    {
        public GenTeleporterDelete()
        {
        }

        public static void Initialize()
        {
            CommandSystem.Register("TelGenDelete", AccessLevel.Administrator, new CommandEventHandler(GenTeleporterDelete_OnCommand));
        }

        [Usage("TelGenDelete")]
        [Description("Deletes world/dungeon teleporters for all facets.")]
        public static void GenTeleporterDelete_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Deleting teleporters, please wait.");

            int count = new TeleportersCreator().CreateTeleporters();
            count += new SHGenTeleporterDelete.SHTeleporterCreator().CreateSHTeleporters();

            e.Mobile.SendMessage("Teleporter deleting complete. {0} teleporters were deleted.", count);
        }

        public class TeleportersCreator
        {
            private static readonly Queue m_Queue = new Queue();
            private int m_Count;
            public TeleportersCreator()
            {
            }

            public static bool FindTeleporter(Map map, Point3D p)
            {
                IPooledEnumerable eable = map.GetItemsInRange(p, 0);

                foreach (Item item in eable)
                {
                    if (item is Teleporter && !(item is KeywordTeleporter) && !(item is SkillTeleporter))
                    {
                        int delta = item.Z - p.Z;

                        if (delta >= -12 && delta <= 12)
                            m_Queue.Enqueue(item);
                    }
                }

                eable.Free();

                while (m_Queue.Count > 0)
                    ((Item)m_Queue.Dequeue()).Delete();

                return false;
            }

            public void CreateTeleporter(Point3D pointLocation, Point3D pointDestination, Map mapLocation, Map mapDestination, bool back)
            {
                if (!FindTeleporter(mapLocation, pointLocation))
                {
                    this.m_Count++;
                }

                if (back && !FindTeleporter(mapDestination, pointDestination))
                {
                    this.m_Count++;
                }
            }

            public void CreateTeleporter(int xLoc, int yLoc, int zLoc, int xDest, int yDest, int zDest, Map map, bool back)
            {
                this.CreateTeleporter(new Point3D(xLoc, yLoc, zLoc), new Point3D(xDest, yDest, zDest), map, map, back);
            }

            public void CreateTeleporter(int xLoc, int yLoc, int zLoc, int xDest, int yDest, int zDest, Map mapLocation, Map mapDestination, bool back)
            {
                this.CreateTeleporter(new Point3D(xLoc, yLoc, zLoc), new Point3D(xDest, yDest, zDest), mapLocation, mapDestination, back);
            }

            public void DestroyTeleporter(int x, int y, int z, Map map)
            {
                Point3D p = new Point3D(x, y, z);
                IPooledEnumerable eable = map.GetItemsInRange(p, 0);

                foreach (Item item in eable)
                {
                    if (item is Teleporter && !(item is KeywordTeleporter) && !(item is SkillTeleporter) && item.Z == p.Z)
                        m_Queue.Enqueue(item);
                }

                eable.Free();

                while (m_Queue.Count > 0)
                    ((Item)m_Queue.Dequeue()).Delete();
            }

            public void CreateTeleportersMap(Map map)
            {
                // Shame
                this.CreateTeleporter(512, 1559, 0, 5394, 127, 0, map, true);
                this.CreateTeleporter(513, 1559, 0, 5395, 127, 0, map, true);
                this.CreateTeleporter(514, 1559, 0, 5396, 127, 0, map, true);
                this.CreateTeleporter(5490, 19, -25, 5514, 10, 5, map, true);
                this.CreateTeleporter(5875, 19, -5, 5507, 162, 5, map, true);
                this.CreateTeleporter(5604, 102, 5, 5514, 147, 25, map, true);

                this.CreateTeleporter(5513, 176, 5, 5540, 187, 0, map, false);
                this.CreateTeleporter(5538, 170, 5, 5517, 176, 0, map, false);

                // Hythloth
                this.CreateTeleporter(4721, 3813, 0, 5904, 16, 64, map, true);
                this.CreateTeleporter(4722, 3813, 0, 5905, 16, 64, map, true);
                this.CreateTeleporter(4723, 3813, 0, 5906, 16, 64, map, true);
                this.CreateTeleporter(6040, 192, 12, 6059, 88, 24, map, true);
                this.CreateTeleporter(6040, 193, 12, 6059, 89, 24, map, true);
                this.CreateTeleporter(6040, 194, 12, 6059, 90, 24, map, true);

                this.DestroyTeleporter(5920, 168, 16, map);
                this.DestroyTeleporter(5920, 169, 17, map);
                this.DestroyTeleporter(5920, 170, 16, map);

                this.CreateTeleporter(5920, 168, 16, 6083, 144, -20, map, false);
                this.CreateTeleporter(5920, 169, 16, 6083, 145, -20, map, false);
                this.CreateTeleporter(5920, 170, 16, 6083, 146, -20, map, false);

                this.CreateTeleporter(6083, 144, -20, 5920, 168, 22, map, false);
                this.CreateTeleporter(6083, 145, -20, 5920, 169, 22, map, false);
                this.CreateTeleporter(6083, 146, -20, 5920, 170, 22, map, false);

                this.DestroyTeleporter(5906, 96, 0, map);

                this.CreateTeleporter(5972, 168, 0, 5905, 100, 0, map, false);
                this.CreateTeleporter(5906, 96, 0, 5977, 169, 0, map, false);

                // Covetous
                this.CreateTeleporter(2498, 916, 0, 5455, 1864, 0, map, true);
                this.CreateTeleporter(2499, 916, 0, 5456, 1864, 0, map, true);
                this.CreateTeleporter(2500, 916, 0, 5457, 1864, 0, map, true);
                this.CreateTeleporter(2384, 836, 0, 5615, 1996, 0, map, true);
                this.CreateTeleporter(2384, 837, 0, 5615, 1997, 0, map, true);
                this.CreateTeleporter(2384, 838, 0, 5615, 1998, 0, map, true);
                this.CreateTeleporter(2420, 883, 0, 5392, 1959, 0, map, true);
                this.CreateTeleporter(2421, 883, 0, 5393, 1959, 0, map, true);
                this.CreateTeleporter(2422, 883, 0, 5394, 1959, 0, map, true);
                this.CreateTeleporter(2455, 858, 0, 5388, 2027, 0, map, true);
                this.CreateTeleporter(2456, 858, 0, 5389, 2027, 0, map, true);
                this.CreateTeleporter(2457, 858, 0, 5390, 2027, 0, map, true);
                this.CreateTeleporter(2544, 850, 0, 5578, 1927, 0, map, true);
                this.CreateTeleporter(2545, 850, 0, 5579, 1927, 0, map, true);
                this.CreateTeleporter(2546, 850, 0, 5580, 1927, 0, map, true);

                this.CreateTeleporter(5551, 1805, 12, 5556, 1825, -3, map, false);
                this.CreateTeleporter(5552, 1805, 12, 5557, 1825, -3, map, false);
                this.CreateTeleporter(5553, 1805, 12, 5557, 1825, -3, map, false);

                this.DestroyTeleporter(5551, 1807, 0, map);
                this.DestroyTeleporter(5552, 1807, 0, map);
                this.DestroyTeleporter(5553, 1807, 0, map);

                this.CreateTeleporter(5556, 1826, -10, 5551, 1806, 7, map, false);
                this.CreateTeleporter(5557, 1826, -10, 5552, 1806, 7, map, false);

                this.DestroyTeleporter(5556, 1825, -7, map);
                this.DestroyTeleporter(5556, 1827, -13, map);
                this.DestroyTeleporter(5557, 1825, -7, map);
                this.DestroyTeleporter(5557, 1827, -13, map);
                this.DestroyTeleporter(5558, 1825, -7, map);

                this.DestroyTeleporter(5468, 1804, 0, map);
                this.DestroyTeleporter(5468, 1805, 0, map);
                this.DestroyTeleporter(5468, 1806, 0, map);

                this.CreateTeleporter(5466, 1804, 12, 5593, 1840, -3, map, false);
                this.CreateTeleporter(5466, 1805, 12, 5593, 1841, -3, map, false);
                this.CreateTeleporter(5466, 1806, 12, 5593, 1842, -3, map, false);

                this.DestroyTeleporter(5595, 1840, -14, map);
                this.DestroyTeleporter(5595, 1840, -14, map);

                this.CreateTeleporter(5594, 1840, -9, 5467, 1804, 7, map, false);
                this.CreateTeleporter(5594, 1841, -9, 5467, 1805, 7, map, false);

                // Wrong
                this.CreateTeleporter(5824, 631, 5, 2041, 215, 15, map, true);
                this.CreateTeleporter(5825, 631, 5, 2042, 215, 15, map, true);
                //CreateTeleporter(5826, 631, 10, 2043, 215, 14, map, true);
                this.CreateTeleporter(5698, 662, 0, 5793, 527, 10, map, false);

                this.DestroyTeleporter(5863, 525, 15, map);
                this.DestroyTeleporter(5863, 526, 15, map);
                this.DestroyTeleporter(5863, 527, 15, map);
                this.DestroyTeleporter(5868, 537, 15, map);
                this.DestroyTeleporter(5868, 538, 15, map);
                this.DestroyTeleporter(5869, 538, 15, map);
                this.DestroyTeleporter(5733, 554, 20, map);
                this.DestroyTeleporter(5862, 527, 15, map);

                // Deceit
                this.CreateTeleporter(4110, 430, 5, 5187, 639, 0, map, false);
                this.CreateTeleporter(4111, 430, 5, 5188, 639, 0, map, false);
                this.CreateTeleporter(4112, 430, 5, 5189, 639, 0, map, false);
                this.CreateTeleporter(5187, 639, 0, 4110, 430, 5, map, false);
                this.CreateTeleporter(5188, 639, 0, 4111, 430, 5, map, false);
                this.CreateTeleporter(5189, 639, 0, 4112, 430, 5, map, false);
                this.CreateTeleporter(5216, 586, -13, 5304, 533, 2, map, false);
                this.CreateTeleporter(5217, 586, -13, 5305, 533, 2, map, false);
                this.CreateTeleporter(5218, 586, -13, 5306, 533, 2, map, false);
                this.CreateTeleporter(5304, 532, 7, 5216, 585, -8, map, false);
                this.CreateTeleporter(5305, 532, 7, 5217, 585, -8, map, false);
                this.CreateTeleporter(5306, 532, 7, 5218, 585, -8, map, false);
                this.CreateTeleporter(5218, 761, -28, 5305, 651, 7, map, false);
                this.CreateTeleporter(5219, 761, -28, 5306, 651, 7, map, false);
                this.CreateTeleporter(5305, 650, 12, 5218, 760, -23, map, false);
                this.CreateTeleporter(5306, 650, 12, 5219, 760, -23, map, false);
                this.CreateTeleporter(5346, 578, 5, 5137, 649, 5, map, true);

                this.CreateTeleporter(5186, 639, 0, 4110, 430, 5, map, false);

                // Despise
                this.CreateTeleporter(5504, 569, 46, 5574, 628, 37, map, false);
                this.CreateTeleporter(5504, 570, 46, 5574, 629, 37, map, false);
                this.CreateTeleporter(5504, 571, 46, 5574, 630, 37, map, false);
                this.CreateTeleporter(5572, 632, 17, 5521, 672, 27, map, false);
                this.CreateTeleporter(5572, 633, 17, 5521, 673, 27, map, false);
                this.CreateTeleporter(5572, 634, 17, 5521, 674, 27, map, false);
                this.CreateTeleporter(5573, 628, 42, 5503, 569, 51, map, false);
                this.CreateTeleporter(5573, 629, 42, 5503, 570, 51, map, false);
                this.CreateTeleporter(5573, 630, 42, 5503, 571, 51, map, false);
                this.CreateTeleporter(5588, 632, 30, 1296, 1082, 0, map, true);
                this.CreateTeleporter(5588, 630, 30, 1296, 1080, 0, map, true);
                this.CreateTeleporter(5588, 631, 30, 1296, 1081, 0, map, true);
                this.CreateTeleporter(5522, 672, 32, 5573, 632, 22, map, false);
                this.CreateTeleporter(5522, 673, 32, 5573, 633, 22, map, false);
                this.CreateTeleporter(5522, 674, 32, 5573, 634, 22, map, false);
                this.CreateTeleporter(5386, 756, -8, 5408, 859, 47, map, false);
                this.CreateTeleporter(5386, 757, -8, 5408, 860, 47, map, false);
                this.CreateTeleporter(5386, 755, -8, 5408, 858, 47, map, false);
                this.CreateTeleporter(5409, 860, 52, 5387, 757, -3, map, false);
                this.CreateTeleporter(5409, 858, 52, 5387, 755, -3, map, false);
                this.CreateTeleporter(5409, 859, 52, 5387, 756, -3, map, false);

                // Destard
                this.CreateTeleporter(5242, 1007, 4, 1175, 2635, 0, map, true);
                this.CreateTeleporter(5243, 1007, 3, 1176, 2635, 0, map, true);
                this.CreateTeleporter(5244, 1007, 4, 1177, 2635, 0, map, true);
                this.CreateTeleporter(5142, 797, 22, 5129, 908, -23, map, false);
                this.CreateTeleporter(5143, 797, 22, 5130, 908, -23, map, false);
                this.CreateTeleporter(5144, 797, 22, 5131, 908, -23, map, false);
                this.CreateTeleporter(5145, 797, 22, 5132, 908, -23, map, false);
                this.CreateTeleporter(5153, 808, -25, 5134, 984, 17, map, false);
                this.CreateTeleporter(5153, 809, -25, 5134, 985, 17, map, false);
                this.CreateTeleporter(5153, 810, -25, 5134, 986, 17, map, false);
                this.CreateTeleporter(5153, 811, -25, 5134, 987, 17, map, false);
                this.CreateTeleporter(5129, 909, -28, 5142, 798, 17, map, false);
                this.CreateTeleporter(5130, 909, -28, 5143, 798, 17, map, false);
                this.CreateTeleporter(5131, 909, -28, 5144, 798, 17, map, false);
                this.CreateTeleporter(5132, 909, -28, 5145, 798, 17, map, false);
                this.CreateTeleporter(5133, 984, 22, 5152, 808, -19, map, false);
                this.CreateTeleporter(5133, 985, 22, 5152, 809, -19, map, false);
                this.CreateTeleporter(5133, 986, 22, 5152, 810, -19, map, false);
                this.CreateTeleporter(5133, 987, 22, 5152, 811, -19, map, false);

                // Buccaneer's Den underground tunnels

                this.DestroyTeleporter(2666, 2073, 5, map);
                this.DestroyTeleporter(2669, 2072, -20, map);
                this.DestroyTeleporter(2669, 2073, -20, map);
                this.DestroyTeleporter(2649, 2194, 4, map);
                this.DestroyTeleporter(2649, 2195, -14, map);

                this.CreateTeleporter(2603, 2121, -20, 2605, 2130, 8, map, false);
                this.CreateTeleporter(2603, 2120, -20, 2605, 2130, 8, map, false);
                this.CreateTeleporter(2669, 2071, -20, 2666, 2099, 3, map, false);
                this.CreateTeleporter(2669, 2072, -20, 2666, 2099, 3, map, false);
                this.CreateTeleporter(2669, 2073, -20, 2666, 2099, 3, map, false);
                this.CreateTeleporter(2676, 2241, -18, 2691, 2234, 2, map, false);
                this.CreateTeleporter(2676, 2242, -18, 2691, 2234, 2, map, false);
                this.CreateTeleporter(2758, 2092, -20, 2756, 2097, 38, map, false);
                this.CreateTeleporter(2759, 2092, -20, 2756, 2097, 38, map, false);
                this.CreateTeleporter(2685, 2063, 39, 2685, 2063, -20, map, false); // that should not be a teleporter: on OSI you simply fall under the ground 

                // Misc
                this.CreateTeleporter(5217, 18, 15, 5204, 74, 17, map, false);
                this.CreateTeleporter(5200, 71, 17, 5211, 22, 15, map, false);
                this.CreateTeleporter(1997, 81, 7, 5881, 242, 0, map, false);
                this.CreateTeleporter(5705, 145, -45, 5705, 304, 7, map, false);
                this.CreateTeleporter(5705, 146, -45, 5705, 305, 7, map, false);
                this.CreateTeleporter(5705, 147, -45, 5705, 306, 7, map, false);
                this.CreateTeleporter(5874, 146, 27, 5208, 2323, 31, map, false);
                this.CreateTeleporter(5875, 146, 27, 5208, 2322, 32, map, false);
                this.CreateTeleporter(5876, 146, 27, 5208, 2322, 32, map, false);
                this.CreateTeleporter(5877, 146, 27, 5208, 2322, 32, map, false);
                this.CreateTeleporter(5923, 169, 1, 5925, 171, 22, map, false);
                this.CreateTeleporter(2399, 198, 0, 5753, 436, 79, map, false);
                this.CreateTeleporter(2400, 198, 0, 5754, 436, 80, map, false);
                this.DestroyTeleporter(5166, 245, 15, map);
                this.DestroyTeleporter(1361, 883, 0, map);
                this.CreateTeleporter(5191, 152, 0, 1367, 891, 0, map, false);
                this.CreateTeleporter(5849, 239, -25, 5831, 324, 27, map, false);
                this.CreateTeleporter(5850, 239, -25, 5832, 324, 26, map, false);
                this.CreateTeleporter(5851, 239, -25, 5833, 324, 28, map, false);
                this.CreateTeleporter(5852, 239, -25, 5834, 324, 27, map, false);
                this.CreateTeleporter(5853, 239, -23, 5835, 324, 27, map, false);
                this.CreateTeleporter(5882, 241, 0, 1998, 81, 5, map, false);
                this.CreateTeleporter(5882, 242, 0, 1998, 81, 5, map, false);
                this.CreateTeleporter(5882, 243, 0, 1998, 81, 5, map, false);
                this.CreateTeleporter(5706, 304, 12, 5706, 145, -45, map, false);
                this.CreateTeleporter(5706, 305, 12, 5706, 146, -45, map, false);
                this.CreateTeleporter(5706, 306, 12, 5706, 147, -45, map, false);
                this.CreateTeleporter(5748, 362, 2, 313, 786, -24, map, false);
                this.CreateTeleporter(5749, 362, 0, 313, 786, -24, map, false);
                this.CreateTeleporter(5750, 362, 3, 314, 786, -24, map, false);
                this.CreateTeleporter(5753, 324, 21, 5670, 2391, 40, map, false);
                this.CreateTeleporter(5831, 323, 34, 5849, 238, -25, map, false);
                this.CreateTeleporter(5832, 323, 34, 5850, 238, -25, map, false);
                this.CreateTeleporter(5833, 323, 33, 5851, 238, -25, map, false);
                this.CreateTeleporter(5834, 323, 33, 5852, 238, -25, map, false);
                this.CreateTeleporter(5835, 323, 33, 5853, 238, -23, map, false);
                this.CreateTeleporter(5658, 423, 8, 5697, 3659, 2, map, false);
                this.CreateTeleporter(5686, 385, 2, 2777, 894, -23, map, false);
                this.CreateTeleporter(5686, 386, 2, 2777, 894, -23, map, false);
                this.CreateTeleporter(5686, 387, 2, 2777, 895, -23, map, false);
                this.CreateTeleporter(5731, 445, -18, 6087, 3676, 18, map, false);
                this.CreateTeleporter(5753, 437, 78, 2400, 199, 0, map, false);
                this.CreateTeleporter(5850, 432, 0, 5127, 3143, 97, map, false);
                this.CreateTeleporter(5850, 433, -2, 5127, 3143, 97, map, false);
                this.CreateTeleporter(5850, 434, -1, 5127, 3143, 97, map, false);
                this.CreateTeleporter(5850, 431, 2, 5127, 3143, 97, map, false);
                this.CreateTeleporter(5826, 465, -1, 1987, 2063, -40, map, false);
                this.CreateTeleporter(5827, 465, -1, 1988, 2063, -40, map, false);
                this.CreateTeleporter(5828, 465, 0, 1989, 2063, -40, map, false);
                this.CreateTeleporter(313, 786, -24, 5748, 361, 2, map, false);
                this.CreateTeleporter(314, 786, -24, 5749, 361, 2, map, false);
                this.CreateTeleporter(2776, 895, -23, 5685, 387, 2, map, false);
                //CreateTeleporter( 4545, 851, 30, 5736, 3196, 8, map, false );
                this.DestroyTeleporter(4545, 851, 30, map);
                this.CreateTeleporter(4540, 898, 32, 4442, 1122, 5, map, false);
                this.CreateTeleporter(4300, 968, 5, 4442, 1122, 5, map, false);
                this.CreateTeleporter(4436, 1107, 5, 4300, 992, 5, map, false);
                this.CreateTeleporter(4443, 1137, 5, 4487, 1475, 5, map, false);
                this.CreateTeleporter(4449, 1107, 5, 4539, 890, 28, map, false);
                this.CreateTeleporter(4449, 1115, 5, 4671, 1135, 10, map, false);
                this.CreateTeleporter(4663, 1134, 13, 4442, 1122, 5, map, false);
                this.CreateTeleporter(5701, 1320, 16, 5786, 1336, -8, map, false);
                this.CreateTeleporter(5702, 1320, 16, 5787, 1336, -8, map, false);
                this.CreateTeleporter(5703, 1320, 16, 5788, 1336, -8, map, false);
                this.CreateTeleporter(5786, 1335, -13, 5701, 1319, 13, map, false);
                this.CreateTeleporter(5787, 1335, -13, 5702, 1319, 13, map, false);
                this.CreateTeleporter(5788, 1335, -13, 5703, 1319, 13, map, false);
                this.CreateTeleporter(6005, 1380, 1, 5151, 4063, 37, map, false);
                this.CreateTeleporter(6005, 1378, 0, 5151, 4062, 37, map, false);
                this.CreateTeleporter(6005, 1379, 2, 5151, 4062, 37, map, false);
                this.CreateTeleporter(6025, 1344, -26, 5137, 3664, 27, map, false);
                this.CreateTeleporter(6025, 1345, -26, 5137, 3664, 27, map, false);
                this.CreateTeleporter(6025, 1346, -26, 5137, 3665, 31, map, false);
                this.CreateTeleporter(5687, 1424, 38, 2923, 3406, 8, map, false);
                this.CreateTeleporter(5792, 1416, 41, 5758, 2908, 15, map, false);
                this.CreateTeleporter(5792, 1417, 41, 5758, 2909, 15, map, false);
                this.CreateTeleporter(5792, 1415, 41, 5758, 2907, 15, map, false);
                this.CreateTeleporter(5899, 1411, 43, 1630, 3320, 0, map, false);
                this.CreateTeleporter(5900, 1411, 42, 1630, 3320, 0, map, false);
                this.CreateTeleporter(5918, 1412, -29, 5961, 1409, 59, map, false);
                this.CreateTeleporter(5918, 1410, -29, 5961, 1408, 59, map, false);
                this.CreateTeleporter(5918, 1411, -29, 5961, 1408, 59, map, false);
                this.CreateTeleporter(5961, 1408, 59, 5918, 1411, -29, map, false);
                this.CreateTeleporter(5961, 1409, 59, 5918, 1412, -29, map, false);
                this.CreateTeleporter(6125, 1411, 15, 6075, 3332, 4, map, false);
                this.CreateTeleporter(6126, 1411, 15, 6075, 3332, 4, map, false);
                this.CreateTeleporter(6127, 1411, 15, 6075, 3332, 4, map, false);
                this.CreateTeleporter(6137, 1409, 2, 6140, 1432, 4, map, false);
                this.CreateTeleporter(6138, 1409, 2, 6140, 1432, 4, map, false);
                this.CreateTeleporter(6140, 1431, 4, 6137, 1408, 2, map, false);
                this.CreateTeleporter(4496, 1475, 15, 4442, 1122, 5, map, false);
                this.CreateTeleporter(6031, 1501, 42, 1491, 1642, 24, map, false);
                this.CreateTeleporter(6031, 1499, 42, 1491, 1640, 24, map, false);
                this.CreateTeleporter(1491, 1640, 24, 6032, 1499, 31, map, false);
                this.CreateTeleporter(1491, 1642, 24, 6032, 1501, 31, map, false);
                this.DestroyTeleporter(5341, 1602, 0, map);
                this.CreateTeleporter(5340, 1599, 40, 5426, 3122, -74, map, false);
                this.CreateTeleporter(5341, 1599, 40, 5426, 3122, -74, map, false);
                this.CreateTeleporter(1987, 2062, -40, 5826, 464, 0, map, false);
                this.CreateTeleporter(1988, 2062, -40, 5827, 464, -1, map, false);
                this.CreateTeleporter(1989, 2062, -40, 5828, 464, -1, map, false);
                this.CreateTeleporter(5203, 2327, 27, 5876, 147, 25, map, false);
                this.CreateTeleporter(5207, 2322, 27, 5877, 147, 25, map, false);
                this.CreateTeleporter(5207, 2323, 26, 5876, 147, 25, map, false);
                this.CreateTeleporter(5670, 2391, 40, 5753, 325, 10, map, false);
                this.CreateTeleporter(5974, 2697, 35, 2985, 2890, 35, map, false);
                this.CreateTeleporter(5267, 2757, 35, 424, 3283, 35, map, false);
                this.CreateTeleporter(5757, 2908, 14, 5791, 1416, 38, map, false);
                this.CreateTeleporter(5757, 2909, 15, 5791, 1417, 40, map, false);
                this.CreateTeleporter(5757, 2907, 15, 5791, 1415, 38, map, false);
                this.CreateTeleporter(1653, 2963, 0, 1677, 2987, 0, map, false);
                this.CreateTeleporter(1677, 2987, 0, 1675, 2987, 20, map, false);
                this.CreateTeleporter(5426, 3123, -80, 5341, 1602, 0, map, false);
                this.CreateTeleporter(5126, 3143, 99, 5849, 432, 1, map, false);
                //CreateTeleporter( 5736, 3196, 8, 4545, 851, 30, map, false );
                this.DestroyTeleporter(5736, 3196, 8, map);
                this.CreateTeleporter(424, 3283, 35, 5267, 2757, 35, map, false);
                this.CreateTeleporter(1629, 3320, 0, 5899, 1411, 43, map, false);
                this.CreateTeleporter(6075, 3332, 4, 6126, 1410, 15, map, false);
                this.CreateTeleporter(2923, 3405, 6, 5687, 1423, 39, map, false);
                this.CreateTeleporter(1142, 3621, 5, 1414, 3828, 5, map, false);
                this.CreateTeleporter(5137, 3664, 27, 6025, 1344, -26, map, false);
                this.CreateTeleporter(5137, 3665, 31, 6025, 1345, -26, map, false);
                this.CreateTeleporter(5697, 3660, -5, 5658, 424, 0, map, false);
                this.CreateTeleporter(6086, 3676, 16, 5731, 446, -16, map, false);
                this.CreateTeleporter(1409, 3824, 5, 1124, 3623, 5, map, false);
                this.CreateTeleporter(1419, 3832, 5, 1466, 4015, 5, map, false);
                this.CreateTeleporter(1406, 3996, 5, 1414, 3828, 5, map, false);
                this.CreateTeleporter(5150, 4062, 38, 6005, 1378, 0, map, false);
                this.CreateTeleporter(5150, 4063, 38, 6005, 1380, 1, map, false);
                this.CreateTeleporter(5906, 4069, 26, 2494, 3576, 5, map, true);
                this.CreateTeleporter(2985, 2890, 35, 5974, 2697, 35, map, false);

                // Mondain's Legacy dungeons

                // Sanctuary
                this.CreateTeleporter(6172, 21, 0, 765, 1645, 0, map, false); // Entrance gate
                this.CreateTeleporter(6172, 22, 0, 765, 1646, 0, map, false);
                this.CreateTeleporter(6172, 23, 0, 765, 1647, 0, map, false);
                this.CreateTeleporter(766, 1645, 0, 6174, 21, 0, map, false);
                this.CreateTeleporter(766, 1646, 0, 6174, 22, 0, map, false);
                this.CreateTeleporter(766, 1647, 0, 6174, 23, 0, map, false);

                this.CreateTeleporter(6173, 176, -10, 6233, 15, -10, map, false); // Zone change
                this.CreateTeleporter(6174, 176, -10, 6234, 15, -10, map, false);
                this.CreateTeleporter(6175, 176, -10, 6235, 15, -10, map, false);
                this.CreateTeleporter(6176, 176, -10, 6236, 15, -10, map, false);
                this.CreateTeleporter(6177, 176, -10, 6237, 15, -10, map, false);
                this.CreateTeleporter(6178, 176, -10, 6238, 15, -10, map, false);
                this.CreateTeleporter(6179, 176, -10, 6239, 15, -10, map, false);
                this.CreateTeleporter(6233, 14, -10, 6172, 174, -10, map, false);
                this.CreateTeleporter(6234, 14, -10, 6173, 174, -10, map, false);
                this.CreateTeleporter(6235, 14, -10, 6174, 174, -10, map, false);
                this.CreateTeleporter(6236, 14, -10, 6175, 174, -10, map, false);
                this.CreateTeleporter(6237, 14, -10, 6176, 174, -10, map, false);
                this.CreateTeleporter(6238, 14, -10, 6177, 174, -10, map, false);
                this.CreateTeleporter(6239, 14, -10, 6177, 174, -10, map, false);
                this.CreateTeleporter(6240, 14, -10, 6178, 174, -10, map, false);

                this.CreateTeleporter(6256, 97, -4, 6257, 95, -10, map, false); // Ladders
                this.CreateTeleporter(6260, 97, -4, 6260, 95, -10, map, false);
                this.CreateTeleporter(6263, 97, -4, 6262, 95, -10, map, false);
                this.CreateTeleporter(6269, 97, -4, 6269, 95, -10, map, false);
                this.CreateTeleporter(6273, 97, -4, 6272, 95, -10, map, false);
                this.CreateTeleporter(6262, 95, -10, 6262, 99, -10, map, false);

                this.CreateTeleporter(6159, 130, 0, 6317, 63, -20, map, false); // Holes
                this.CreateTeleporter(6164, 73, 0, 6320, 22, -20, map, false);
                this.CreateTeleporter(6161, 163, -10, 6321, 106, -20, map, false);
                this.CreateTeleporter(6276, 40, -10, 6374, 124, -20, map, false);
                this.CreateTeleporter(6211, 106, 0, 6355, 34, -20, map, false);

                this.CreateTeleporter(6316, 62, -5, 6160, 131, 0, map, false); // Cave ladders
                this.CreateTeleporter(6319, 19, -20, 6165, 74, 0, map, false); // This actually goes to (6165, 75) inside the wall...
                this.CreateTeleporter(6356, 32, -9, 6210, 105, 0, map, false);
                this.CreateTeleporter(6374, 125, -20, 6277, 41, -10, map, false);

                this.CreateTeleporter(6373, 49, -20, 801, 1682, 0, map, false);
                this.CreateTeleporter(6374, 49, -20, 801, 1682, 0, map, false);
                this.CreateTeleporter(6375, 49, -20, 801, 1682, 0, map, false);
                this.CreateTeleporter(6376, 49, -20, 801, 1682, 0, map, false);
                this.CreateTeleporter(6377, 49, -20, 801, 1682, 0, map, false);

                // Painted Caves
                this.CreateTeleporter(1714, 2996, 0, 6308, 892, 0, map, false); // Entrance
                this.CreateTeleporter(1714, 2997, 0, 6308, 892, 0, map, false);
                this.CreateTeleporter(6310, 890, 0, 1716, 2997, 0, map, false);
                this.CreateTeleporter(6310, 891, 0, 1716, 2997, 0, map, false);
                this.CreateTeleporter(6310, 892, 0, 1716, 2997, 0, map, false);
                this.CreateTeleporter(6310, 893, 0, 1716, 2997, 0, map, false);
            }

            public void CreateTeleportersMap2(Map map)
            {
                // Dungeon of rock
                this.CreateTeleporter(2186, 294, -27, 2186, 33, -27, map, true);
                this.CreateTeleporter(2187, 294, -27, 2187, 33, -27, map, true);
                this.CreateTeleporter(2188, 294, -27, 2188, 33, -27, map, true);
                this.CreateTeleporter(2189, 294, -27, 2189, 33, -27, map, true);
                this.CreateTeleporter(2189, 320, -7, 1788, 569, 74, map, true);
                this.CreateTeleporter(2188, 320, -7, 1787, 569, 74, map, true);
                this.CreateTeleporter(2187, 320, -7, 1787, 569, 74, map, false);

                // Spider Cave
                this.DestroyTeleporter(1783, 993, -28, map);
                this.DestroyTeleporter(1784, 993, -28, map);
                this.DestroyTeleporter(1785, 993, -28, map);
                this.DestroyTeleporter(1786, 993, -28, map);
                this.DestroyTeleporter(1787, 993, -28, map);
                this.DestroyTeleporter(1788, 993, -28, map);

                this.DestroyTeleporter(1419, 910, -10, map);
                this.DestroyTeleporter(1420, 910, -10, map);
                this.DestroyTeleporter(1421, 910, -10, map);
                this.DestroyTeleporter(1422, 910, -10, map);

                this.CreateTeleporter(1419, 909, -10, 1784, 994, -28, map, true);
                this.CreateTeleporter(1420, 909, -10, 1785, 994, -28, map, true);
                this.CreateTeleporter(1421, 909, -10, 1786, 994, -28, map, true);
                this.CreateTeleporter(1787, 994, -28, 1421, 909, -10, map, false);
                this.CreateTeleporter(1788, 994, -28, 1421, 909, -10, map, false);
                this.CreateTeleporter(1783, 994, -28, 1419, 909, -10, map, false);

                this.CreateTeleporter(1861, 980, -28, 1490, 877, 10, map, true);
                this.CreateTeleporter(1861, 981, -28, 1490, 878, 10, map, true);
                this.CreateTeleporter(1861, 982, -28, 1490, 879, 10, map, true);
                this.CreateTeleporter(1861, 983, -28, 1490, 880, 10, map, true);
                this.CreateTeleporter(1861, 984, -28, 1490, 880, 10, map, false);
                this.CreateTeleporter(1516, 879, 10, 1363, 1105, -26, map, true);

                // Spectre Dungeon
                this.CreateTeleporter(1362, 1031, -13, 1981, 1107, -16, map, true);
                this.CreateTeleporter(1363, 1031, -13, 1982, 1107, -16, map, true);
                this.CreateTeleporter(1364, 1031, -13, 1983, 1107, -16, map, true);
                this.CreateTeleporter(1980, 1107, -16, 1362, 1031, -13, map, false);
                this.CreateTeleporter(1984, 1107, -16, 1364, 1031, -13, map, false);

                // BLOOD DUNGEON
                this.CreateTeleporter(1745, 1236, -30, 2112, 829, -11, map, true);
                this.CreateTeleporter(1746, 1236, -30, 2113, 829, -11, map, true);
                this.CreateTeleporter(1747, 1236, -30, 2114, 829, -11, map, true);
                this.CreateTeleporter(1748, 1236, -30, 2115, 829, -11, map, true);
                this.CreateTeleporter(2116, 829, -11, 1748, 1236, -30, map, false);

                // Mushroom Cave
                this.CreateTeleporter(1456, 1328, -27, 1479, 1494, -28, map, true);
                this.CreateTeleporter(1456, 1329, -27, 1479, 1495, -28, map, true);
                this.CreateTeleporter(1456, 1330, -27, 1479, 1496, -28, map, true);
                this.CreateTeleporter(1479, 1493, -28, 1456, 1328, -27, map, false);
                this.CreateTeleporter(1479, 1497, -28, 1456, 1330, -27, map, false);

                // RATMAN CAVE
                this.CreateTeleporter(1029, 1155, -24, 1349, 1512, -3, map, true);
                this.CreateTeleporter(1029, 1154, -24, 1349, 1511, -3, map, true);
                this.CreateTeleporter(1029, 1153, -24, 1349, 1510, -3, map, true);
                this.CreateTeleporter(1349, 1509, -3, 1030, 1153, -24, map, false);
                this.CreateTeleporter(1268, 1508, -28, 1250, 1508, -28, map, true);
                this.CreateTeleporter(1268, 1509, -28, 1250, 1509, -28, map, true);
                this.CreateTeleporter(1268, 1510, -28, 1250, 1510, -28, map, true);
                this.CreateTeleporter(1250, 1511, -28, 1268, 1510, -28, map, false);
                this.CreateTeleporter(1250, 1512, -28, 1268, 1510, -28, map, false);

                // Serpentine Passage
                this.DestroyTeleporter(532, 1532, -7, map);
                this.DestroyTeleporter(533, 1532, -7, map);
                this.DestroyTeleporter(534, 1532, -7, map);

                this.CreateTeleporter(810, 874, -39, 532, 1532, -7, map, false);
                this.CreateTeleporter(811, 874, -39, 533, 1532, -7, map, false);
                this.CreateTeleporter(812, 874, -39, 534, 1532, -7, map, false);
                this.CreateTeleporter(531, 1533, -4, 810, 875, -40, map, false);
                this.CreateTeleporter(532, 1533, -4, 811, 875, -39, map, false);
                this.CreateTeleporter(533, 1533, -4, 812, 875, -39, map, false);
                this.CreateTeleporter(534, 1533, -4, 813, 875, -40, map, false);

                this.CreateTeleporter(393, 1587, -13, 78, 1366, -36, map, false);
                this.CreateTeleporter(394, 1587, -13, 79, 1366, -36, map, false);
                this.CreateTeleporter(395, 1587, -13, 80, 1366, -36, map, false);
                this.CreateTeleporter(396, 1587, -13, 81, 1366, -36, map, false);
                this.CreateTeleporter(78, 1365, -41, 393, 1586, -16, map, false);
                this.CreateTeleporter(79, 1365, -41, 394, 1586, -16, map, false);
                this.CreateTeleporter(80, 1365, -41, 395, 1586, -16, map, false);
                this.CreateTeleporter(81, 1365, -41, 396, 1586, -16, map, false);
                this.DestroyTeleporter(82, 1365, -38, map);

                // ANKH DUNGEON
                this.DestroyTeleporter(4, 1267, -11, map);
                this.DestroyTeleporter(4, 1268, -11, map);
                this.DestroyTeleporter(4, 1269, -11, map);
                this.CreateTeleporter(668, 928, -84, 3, 1267, -8, map, true);
                this.CreateTeleporter(668, 929, -84, 3, 1268, -8, map, true);
                this.CreateTeleporter(668, 930, -82, 3, 1269, -8, map, true);

                this.DestroyTeleporter(154, 1473, -8, map);
                this.DestroyTeleporter(155, 1473, -8, map);
                this.DestroyTeleporter(156, 1473, -8, map);
                this.CreateTeleporter(575, 1156, -121, 154, 1473, -8, map, false);
                this.CreateTeleporter(576, 1156, -121, 155, 1473, -8, map, false);
                this.CreateTeleporter(577, 1156, -121, 156, 1473, -8, map, false);
                this.CreateTeleporter(154, 1472, -8, 575, 1155, -121, map, false);
                this.CreateTeleporter(155, 1472, -8, 576, 1155, -120, map, false);
                this.CreateTeleporter(156, 1472, -8, 577, 1155, -120, map, false);

                this.DestroyTeleporter(10, 1518, -28, map);
                this.DestroyTeleporter(10, 1518, -27, map);
                this.DestroyTeleporter(10, 1518, -27, map);
                this.CreateTeleporter(10, 872, -28, 10, 1518, -27, map, false);
                this.CreateTeleporter(11, 872, -28, 11, 1518, -28, map, false);
                this.CreateTeleporter(12, 872, -27, 12, 1518, -28, map, false);
                this.CreateTeleporter(10, 1519, -28, 10, 873, -28, map, false);
                this.CreateTeleporter(11, 1519, -27, 11, 873, -28, map, false);
                this.CreateTeleporter(12, 1519, -27, 12, 873, -27, map, false);

                // Ratman Lair 
                this.CreateTeleporter(636, 813, -62, 164, 743, -28, map, false);
                this.CreateTeleporter(164, 746, -16, 636, 815, -52, map, false);

                // WISP DUNGEON
                this.CreateTeleporter(348, 1427, 15, 18, 1198, -5, map, true);
                this.CreateTeleporter(349, 1427, 15, 19, 1198, -5, map, true);
                this.CreateTeleporter(350, 1427, 15, 20, 1198, -5, map, true);
                this.CreateTeleporter(351, 1427, 15, 21, 1198, -5, map, true);
                this.CreateTeleporter(712, 1490, -3, 686, 1490, -28, map, false);
                this.CreateTeleporter(712, 1491, -3, 686, 1491, -28, map, false);
                this.CreateTeleporter(712, 1492, -3, 686, 1492, -28, map, false);
                this.CreateTeleporter(712, 1493, -3, 686, 1493, -28, map, false);
                this.CreateTeleporter(694, 1490, -53, 719, 1490, -28, map, false);
                this.CreateTeleporter(694, 1491, -53, 719, 1491, -28, map, false);
                this.CreateTeleporter(694, 1492, -53, 719, 1492, -28, map, false);
                this.CreateTeleporter(694, 1493, -53, 719, 1493, -28, map, false);
                this.CreateTeleporter(775, 1467, -28, 658, 1498, -28, map, false);
                this.DestroyTeleporter(658, 1498, -28, map);
                this.CreateTeleporter(838, 1550, -6, 728, 1505, -28, map, false);
                this.CreateTeleporter(838, 1551, -6, 728, 1506, -28, map, false);
                this.CreateTeleporter(838, 1552, -6, 728, 1507, -28, map, false);
                this.CreateTeleporter(838, 1553, -6, 728, 1508, -28, map, false);
                this.CreateTeleporter(722, 1505, -53, 833, 1550, -28, map, false);
                this.CreateTeleporter(722, 1506, -53, 833, 1551, -28, map, false);
                this.CreateTeleporter(722, 1507, -53, 833, 1552, -28, map, false);
                this.CreateTeleporter(722, 1508, -53, 833, 1553, -28, map, false);
                this.CreateTeleporter(954, 1425, -53, 874, 1490, 2, map, false);
                this.CreateTeleporter(954, 1426, -53, 874, 1491, 2, map, false);
                this.CreateTeleporter(954, 1427, -53, 874, 1492, 2, map, false);
                this.CreateTeleporter(954, 1428, -53, 874, 1493, 2, map, false);
                this.CreateTeleporter(954, 1429, -53, 874, 1493, 2, map, false);
                this.CreateTeleporter(879, 1490, 24, 960, 1425, -28, map, false);
                this.CreateTeleporter(879, 1491, 24, 960, 1426, -28, map, false);
                this.CreateTeleporter(879, 1492, 24, 960, 1427, -28, map, false);
                this.CreateTeleporter(879, 1493, 24, 960, 1428, -28, map, false);
                this.CreateTeleporter(948, 1464, -56, 951, 1442, -6, map, true);
                this.CreateTeleporter(948, 1465, -56, 951, 1443, -6, map, true);
                this.CreateTeleporter(948, 1466, -56, 951, 1444, -6, map, true);
                this.CreateTeleporter(948, 1467, -56, 951, 1445, -6, map, true);
                this.CreateTeleporter(871, 1433, -6, 897, 1449, -28, map, false);
                this.CreateTeleporter(871, 1434, -6, 897, 1450, -28, map, false);
                this.CreateTeleporter(871, 1435, -6, 897, 1451, -28, map, false);
                this.CreateTeleporter(871, 1436, -6, 897, 1452, -28, map, false);
                this.CreateTeleporter(871, 1437, -6, 897, 1453, -28, map, false);
                this.CreateTeleporter(892, 1449, -51, 866, 1433, -28, map, false);
                this.CreateTeleporter(892, 1450, -51, 866, 1434, -28, map, false);
                this.CreateTeleporter(892, 1451, -51, 866, 1435, -28, map, false);
                this.CreateTeleporter(892, 1452, -51, 866, 1436, -28, map, false);
                this.CreateTeleporter(892, 1453, -51, 866, 1437, -28, map, false);
                this.CreateTeleporter(812, 1546, -6, 848, 1434, -28, map, false);
                this.CreateTeleporter(812, 1547, -6, 848, 1435, -28, map, false);
                this.CreateTeleporter(812, 1548, -6, 848, 1436, -28, map, false);
                this.CreateTeleporter(812, 1549, -6, 848, 1437, -28, map, false);
                this.CreateTeleporter(843, 1434, -51, 807, 1546, -28, map, false);
                this.CreateTeleporter(843, 1435, -51, 807, 1547, -28, map, false);
                this.CreateTeleporter(843, 1436, -51, 807, 1548, -28, map, false);
                this.CreateTeleporter(843, 1437, -51, 807, 1549, -28, map, false);
                this.CreateTeleporter(751, 1473, -28, 763, 1479, -28, map, false);
                this.DestroyTeleporter(763, 1479, -28, map);
                this.CreateTeleporter(751, 1479, -28, 763, 1555, -28, map, false);
                this.DestroyTeleporter(763, 1555, -28, map);
                this.CreateTeleporter(752, 1549, -28, 751, 1484, -28, map, false);
                this.DestroyTeleporter(751, 1484, -28, map);
                this.CreateTeleporter(775, 1492, -28, 827, 1515, -28, map, false);
                this.DestroyTeleporter(827, 1515, -28, map);
                this.DestroyTeleporter(1013, 1506, 0, map);
                this.DestroyTeleporter(1013, 1507, 0, map);
                this.DestroyTeleporter(1013, 1508, 0, map);
                this.DestroyTeleporter(1013, 1509, 0, map);
                this.CreateTeleporter(904, 1360, -21, 1014, 1506, 0, map, true);
                this.CreateTeleporter(904, 1361, -21, 1014, 1507, 0, map, true);
                this.CreateTeleporter(904, 1362, -21, 1014, 1508, 0, map, true);
                this.CreateTeleporter(904, 1363, -21, 1014, 1509, 0, map, true);

                /*CreateTeleporter( 650, 1297, -58, 626, 1526, -28, map, true );
                CreateTeleporter( 651, 1297, -58, 627, 1526, -28, map, true );
                CreateTeleporter( 652, 1297, -58, 628, 1526, -28, map, true );
                CreateTeleporter( 653, 1297, -58, 629, 1526, -28, map, true );*/

                // Update: remove old teleporters
                this.DestroyTeleporter(650, 1297, -58, map);
                this.DestroyTeleporter(651, 1297, -58, map);
                this.DestroyTeleporter(652, 1297, -58, map);
                this.DestroyTeleporter(653, 1297, -58, map);
                this.DestroyTeleporter(626, 1526, -28, map);
                this.DestroyTeleporter(627, 1526, -28, map);
                this.DestroyTeleporter(628, 1526, -28, map);
                this.DestroyTeleporter(629, 1526, -28, map);

                // Update: add new ones
                for (int i = 0; i < 4; ++i)
                {
                    this.CreateTeleporter(650 + i, 1297, -58, 626 + i, 1526, -28, map, false);
                    this.CreateTeleporter(626 + i, 1527, -28, 650 + i, 1298, i == 1 ? -59 : -58, map, false);
                }

                // WISP DUNGEON MAZE
                this.CreateTeleporter(747, 1539, -28, 785, 1514, -28, map, false);
                this.CreateTeleporter(785, 1524, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(747, 1555, -28, 785, 1570, -28, map, false);
                this.CreateTeleporter(784, 1580, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(791, 1548, -28, 781, 1547, -28, map, false);
                this.CreateTeleporter(782, 1550, -28, 783, 1538, -28, map, false);
                this.CreateTeleporter(783, 1539, -28, 787, 1542, -28, map, false);
                this.CreateTeleporter(787, 1543, -28, 781, 1554, -28, map, false);
                this.CreateTeleporter(781, 1555, -28, 789, 1556, -28, map, false);
                this.CreateTeleporter(789, 1557, -28, 785, 1550, -28, map, false);
                this.CreateTeleporter(784, 1550, -28, 777, 1554, -28, map, false);
                this.CreateTeleporter(778, 1554, -28, 787, 1538, -28, map, false);
                this.CreateTeleporter(787, 1537, -28, 781, 1546, -28, map, false);
                this.CreateTeleporter(781, 1545, -28, 789, 1552, -28, map, false);
                this.CreateTeleporter(789, 1553, -28, 789, 1546, -28, map, false);
                this.CreateTeleporter(789, 1545, -28, 779, 1541, -28, map, false);
                this.CreateTeleporter(780, 1541, -28, 785, 1554, -28, map, false);
                this.CreateTeleporter(785, 1555, -28, 783, 1542, -28, map, false);
                this.CreateTeleporter(782, 1542, -28, 785, 1546, -28, map, false);
                this.CreateTeleporter(784, 1546, -28, 776, 1548, -28, map, false);
                this.CreateTeleporter(777, 1555, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(777, 1553, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(776, 1549, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(777, 1548, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(777, 1547, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(776, 1546, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(778, 1541, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(779, 1540, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(779, 1542, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(780, 1554, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(781, 1553, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(782, 1554, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(780, 1550, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(781, 1549, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(781, 1551, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(780, 1546, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(781, 1547, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(782, 1546, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(783, 1543, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(784, 1542, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(783, 1541, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(782, 1538, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(783, 1537, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(784, 1538, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(786, 1538, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(787, 1539, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(788, 1538, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(788, 1542, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(787, 1541, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(786, 1542, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(785, 1545, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(786, 1546, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(785, 1547, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(785, 1549, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(785, 1551, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(786, 1550, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(784, 1554, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(785, 1553, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(786, 1554, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(788, 1556, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(789, 1555, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(790, 1556, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(788, 1552, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(789, 1551, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(790, 1552, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(790, 1546, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(788, 1546, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(789, 1547, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(791, 1545, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(791, 1546, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(791, 1547, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(791, 1549, -28, 798, 1547, -28, map, false);
                this.CreateTeleporter(791, 1550, -28, 798, 1547, -28, map, false);

                // Sorcerer`s Dungeon
                this.CreateTeleporter(546, 455, -40, 426, 113, -28, map, true);
                this.CreateTeleporter(547, 455, -40, 427, 113, -28, map, true);
                this.CreateTeleporter(548, 455, -40, 428, 113, -28, map, true);
                this.CreateTeleporter(429, 113, -28, 548, 455, -40, map, false);

                this.CreateTeleporter(242, 27, -16, 372, 31, -31, map, false); // stairs - 0x754 
                this.CreateTeleporter(242, 26, -16, 372, 30, -31, map, false); // stairs - 0x754 
                this.CreateTeleporter(242, 25, -16, 372, 29, -31, map, false); // stairs - 0x754 
                this.CreateTeleporter(371, 31, -36, 241, 27, -18, map, false); // stairs - 0x754 
                this.CreateTeleporter(371, 30, -36, 241, 26, -18, map, false); // stairs - 0x754 

                this.DestroyTeleporter(371, 29, -36, map);	//To remove old erroneous teleporter

                this.CreateTeleporter(371, 29, -36, 241, 25, -18, map, false); // stairs - 0x754

                this.CreateTeleporter(272, 141, -16, 555, 427, -1, map, false); // stairs - 1st 0x753 
                this.CreateTeleporter(273, 141, -16, 556, 427, -1, map, false); // stairs - 1st 0x753 
                this.CreateTeleporter(274, 141, -16, 557, 427, -1, map, false); // stairs - 1st 0x753 
                this.CreateTeleporter(555, 426, -6, 272, 140, -21, map, false); // stairs - 1st 0x753 
                this.CreateTeleporter(556, 426, -6, 273, 140, -21, map, false); // stairs - 1st 0x753 
                this.CreateTeleporter(557, 426, -6, 274, 140, -21, map, false); // stairs - 1st 0x753 

                this.CreateTeleporter(265, 130, -31, 284, 72, -21, map, false); // stairs - 0x753 
                this.CreateTeleporter(266, 130, -31, 285, 72, -21, map, false); // stairs - 0x753 
                this.CreateTeleporter(267, 130, -31, 286, 72, -21, map, false); // stairs - 0x753 
                this.CreateTeleporter(268, 130, -31, 287, 72, -21, map, false); // stairs - 0x753 
                this.CreateTeleporter(284, 73, -16, 265, 131, -28, map, false); // stairs - 0x753 
                this.CreateTeleporter(285, 73, -16, 266, 131, -28, map, false); // stairs - 0x753 
                this.CreateTeleporter(286, 73, -16, 267, 131, -28, map, false); // stairs - 0x753 
                this.CreateTeleporter(287, 73, -16, 268, 131, -28, map, false); // stairs - 0x753 

                this.CreateTeleporter(284, 67, -30, 131, 128, -21, map, false); // stairs - 0x753 
                this.CreateTeleporter(285, 67, -30, 132, 128, -21, map, false); // stairs - 0x753 
                this.CreateTeleporter(286, 67, -30, 133, 128, -21, map, false); // stairs - 0x753 
                this.CreateTeleporter(287, 67, -30, 134, 128, -21, map, false); // stairs - 0x753 
                this.CreateTeleporter(131, 129, -16, 284, 68, -28, map, false); // stairs - 0x753 
                this.CreateTeleporter(132, 129, -16, 285, 68, -28, map, false); // stairs - 0x753 
                this.CreateTeleporter(133, 129, -16, 286, 68, -28, map, false); // stairs - 0x753 
                this.CreateTeleporter(134, 129, -16, 287, 68, -28, map, false); // stairs - 0x753 

                this.CreateTeleporter(358, 40, -36, 156, 88, -18, map, false); // stairs - 0x73A 
                this.CreateTeleporter(358, 41, -36, 156, 89, -18, map, false); // stairs - 0x73A 
                this.CreateTeleporter(358, 42, -36, 156, 90, -18, map, false); // stairs - 0x73A 
                this.CreateTeleporter(155, 88, -16, 357, 40, -31, map, false); // stairs - 0x73A 
                this.CreateTeleporter(155, 89, -16, 357, 41, -31, map, false); // stairs - 0x73A 
                this.CreateTeleporter(155, 90, -16, 357, 42, -31, map, false); // stairs - 0x73A 

                this.CreateTeleporter(259, 90, -28, 236, 113, -28, map, true);

                // Ancient Lair

                this.DestroyTeleporter(83, 747, -28, map);
                this.DestroyTeleporter(84, 747, -28, map);
                this.DestroyTeleporter(85, 747, -28, map);
                this.DestroyTeleporter(86, 747, -28, map);
                this.CreateTeleporter(938, 494, -40, 83, 749, -23, map, true);
                this.CreateTeleporter(939, 494, -40, 84, 749, -23, map, true);
                this.CreateTeleporter(940, 494, -40, 85, 749, -23, map, true);
                this.CreateTeleporter(941, 494, -40, 86, 749, -23, map, true);

                // Lizard Passage

                this.DestroyTeleporter(313, 1330, -39, map);
                this.DestroyTeleporter(314, 1330, -37, map);
                this.DestroyTeleporter(315, 1330, -35, map);
                this.CreateTeleporter(313, 1329, -40, 327, 1593, -13, map, true);
                this.CreateTeleporter(314, 1329, -38, 328, 1593, -13, map, true);
                this.CreateTeleporter(315, 1329, -36, 329, 1593, -13, map, true);
                this.CreateTeleporter(330, 1593, -13, 315, 1329, -35, map, false);

                this.DestroyTeleporter(225, 1335, -20, map);
                this.DestroyTeleporter(226, 1335, -20, map);
                this.DestroyTeleporter(227, 1335, -19, map);
                this.CreateTeleporter(265, 1587, -28, 225, 1334, -20, map, true);
                this.CreateTeleporter(266, 1587, -28, 226, 1334, -20, map, true);
                this.CreateTeleporter(267, 1587, -28, 227, 1334, -20, map, true);

                // Central Ilshenar
                /*CreateTeleporter( 1139, 593, -80, 1237, 582, -19, map, true );
                CreateTeleporter( 1140, 593, -80, 1237, 583, -19, map, true );
                CreateTeleporter( 1141, 593, -80, 1237, 584, -19, map, true );
                CreateTeleporter( 1142, 593, -80, 1237, 585, -19, map, true );
                CreateTeleporter( 912, 451, -80, 708, 667, -39, map, true );
                CreateTeleporter( 912, 452, -80, 709, 667, -39, map, true );
                CreateTeleporter( 912, 453, -80, 710, 667, -39, map, true );
                CreateTeleporter( 711, 667, -39, 912, 453, -80, map, false );*/

                // Update: remove old teleporters..
                this.DestroyTeleporter(1139, 593, -80, map);
                this.DestroyTeleporter(1140, 593, -80, map);
                this.DestroyTeleporter(1141, 593, -80, map);
                this.DestroyTeleporter(1142, 593, -80, map);
                this.DestroyTeleporter(1237, 582, -19, map);
                this.DestroyTeleporter(1237, 583, -19, map);
                this.DestroyTeleporter(1237, 584, -19, map);
                this.DestroyTeleporter(1237, 585, -19, map);

                this.DestroyTeleporter(912, 451, -80, map);
                this.DestroyTeleporter(912, 452, -80, map);
                this.DestroyTeleporter(912, 453, -80, map);
                this.DestroyTeleporter(708, 667, -39, map);
                this.DestroyTeleporter(709, 667, -39, map);
                this.DestroyTeleporter(710, 667, -39, map);
                this.DestroyTeleporter(711, 667, -39, map);

                // Update: add new ones...
                this.CreateTeleporter(1139, 592, -80, 1238, 583, -19, map, false);
                this.CreateTeleporter(1140, 592, -80, 1238, 584, -19, map, false);
                this.CreateTeleporter(1141, 592, -80, 1238, 585, -19, map, false);
                this.CreateTeleporter(1142, 592, -80, 1238, 585, -19, map, false);
                this.CreateTeleporter(1237, 583, -19, 1139, 593, -80, map, false);
                this.CreateTeleporter(1237, 584, -19, 1140, 593, -80, map, false);
                this.CreateTeleporter(1237, 585, -19, 1141, 593, -80, map, false);

                this.CreateTeleporter(709, 667, -39, 912, 451, -80, map, false);
                this.CreateTeleporter(710, 667, -39, 912, 452, -80, map, false);
                this.CreateTeleporter(711, 667, -39, 912, 453, -80, map, false);

                this.CreateTeleporter(911, 451, -80, 709, 668, -39, map, false);
                this.CreateTeleporter(911, 452, -80, 710, 668, -39, map, false);
                this.CreateTeleporter(911, 453, -80, 711, 668, -38, map, false);

                // Exodus Dungeon
                this.CreateTeleporter(827, 777, -80, 1975, 114, -28, map, false);
                this.CreateTeleporter(827, 778, -80, 1975, 114, -28, map, false);
                this.CreateTeleporter(827, 779, -80, 1975, 114, -28, map, false);
                this.CreateTeleporter(828, 777, -80, 1975, 114, -28, map, false);
                this.CreateTeleporter(828, 778, -80, 1975, 114, -28, map, false);
                this.CreateTeleporter(828, 779, -80, 1975, 114, -28, map, false);
                this.CreateTeleporter(829, 777, -80, 1975, 114, -28, map, false);
                this.CreateTeleporter(829, 778, -80, 1975, 114, -28, map, false);
                this.CreateTeleporter(829, 779, -80, 1975, 114, -28, map, false);

                this.CreateTeleporter(1978, 114, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1978, 115, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1978, 116, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1978, 117, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1979, 114, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1979, 115, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1979, 116, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1979, 117, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1980, 114, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1980, 115, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1980, 116, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1980, 117, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1981, 114, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1981, 115, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1981, 116, -28, 835, 778, -80, map, false);
                this.CreateTeleporter(1981, 117, -28, 835, 778, -80, map, false);
            }

            public void CreateTeleportersMap3(Map map)
            {
                // CreateTeleporter( 408, 254, 2, 428, 319, 2, map, false ); // for doom quest; use blockers to avoid players teleporting into the ship! 
                // CreateTeleporter( 428, 321, 2, 422, 328, -1, map, false ); // for doom quest; use blockers to avoid players teleporting into the ship!
                // Doom Dungeon
                this.CreateTeleporter(2317, 1269, -110, 381, 132, 33, map, false);
                this.CreateTeleporter(2317, 1268, -110, 381, 132, 33, map, false);
                this.CreateTeleporter(2317, 1267, -110, 381, 132, 33, map, false);
                this.CreateTeleporter(2317, 1266, -110, 381, 132, 33, map, false);
                this.CreateTeleporter(2316, 1269, -110, 381, 132, 33, map, false);
                this.CreateTeleporter(2315, 1269, -110, 381, 132, 33, map, false);
                this.CreateTeleporter(2315, 1268, -110, 381, 132, 33, map, false);
                this.CreateTeleporter(2315, 1267, -109, 381, 132, 33, map, false);
                this.CreateTeleporter(2316, 1267, -110, 381, 132, 33, map, false);
                this.CreateTeleporter(496, 49, 6, 2350, 1270, -85, map, false);
                this.DestroyTeleporter(433, 326, 4, map);
                this.DestroyTeleporter(365, 15, -1, map);

                //Yomotsu Mines Exit
                this.CreateTeleporter(3, 128, -1, 259, 785, 64, map, Map.Tokuno, false);
                this.CreateTeleporter(4, 128, -1, 259, 785, 64, map, Map.Tokuno, false);
                this.CreateTeleporter(5, 128, -1, 259, 785, 64, map, Map.Tokuno, false);
                this.CreateTeleporter(6, 128, -1, 259, 785, 64, map, Map.Tokuno, false);
                this.CreateTeleporter(7, 128, -1, 259, 785, 64, map, Map.Tokuno, false);
                this.CreateTeleporter(8, 128, -1, 259, 785, 64, map, Map.Tokuno, false);

                //Fan Dancer Exit
                this.CreateTeleporter(64, 336, 11, 983, 195, 24, map, Map.Tokuno, false);
                this.CreateTeleporter(64, 337, 11, 983, 195, 24, map, Map.Tokuno, false);
                this.CreateTeleporter(64, 338, 11, 983, 195, 24, map, Map.Tokuno, false);
                this.CreateTeleporter(64, 339, 11, 983, 195, 24, map, Map.Tokuno, false);

                //Fan Dancer Levels
                this.CreateTeleporter(66, 351, -7, 63, 524, -1, map, false);
                this.CreateTeleporter(66, 352, -7, 63, 524, -1, map, false);
                this.CreateTeleporter(66, 353, -7, 63, 524, -1, map, false);
                this.CreateTeleporter(66, 354, -7, 63, 524, -1, map, false);

                this.CreateTeleporter(61, 523, 6, 63, 352, -3, map, false);
                this.CreateTeleporter(61, 524, 6, 63, 352, -3, map, false);
                this.CreateTeleporter(61, 525, 6, 63, 352, -3, map, false);
                this.CreateTeleporter(61, 526, 6, 63, 352, -3, map, false);

                this.CreateTeleporter(103, 555, -6, 76, 691, -1, map, false);
                this.CreateTeleporter(103, 556, -6, 76, 691, -1, map, false);
                this.CreateTeleporter(103, 557, -6, 76, 691, -1, map, false);
                this.CreateTeleporter(103, 558, -6, 76, 691, -1, map, false);

                this.CreateTeleporter(73, 688, 6, 100, 556, -2, map, false);
                this.CreateTeleporter(73, 689, 6, 100, 556, -2, map, false);
                this.CreateTeleporter(73, 690, 6, 100, 556, -2, map, false);
                this.CreateTeleporter(73, 691, 6, 100, 556, -2, map, false);
                this.CreateTeleporter(73, 692, 6, 100, 556, -2, map, false);
                this.CreateTeleporter(73, 693, 6, 100, 556, -2, map, false);
                this.CreateTeleporter(73, 694, 6, 100, 556, -2, map, false);

                //Ninja cave
                this.CreateTeleporter(384, 810, -1, 403, 1167, 0, map, false);

                this.CreateTeleporter(403, 1169, 0, 385, 811, -1, map, false);
                this.CreateTeleporter(404, 1169, 0, 385, 808, -1, map, false);
                this.CreateTeleporter(405, 1169, 0, 385, 808, -1, map, false);

                // Dungeon Labyrinth
                this.CreateTeleporter(328, 1972, 5, 1731, 978, -80, map, false); // Door exit
                this.CreateTeleporter(328, 1973, 5, 1731, 978, -80, map, false);
                this.CreateTeleporter(328, 1974, 5, 1731, 978, -80, map, false);
                this.CreateTeleporter(328, 1975, 5, 1731, 978, -80, map, false);

                // Dungeon Bedlam
                this.CreateTeleporter(84, 1673, -2, 156, 1613, 0, map, false);
                this.CreateTeleporter(156, 1609, 17, 87, 1673, 0, map, false);
                this.CreateTeleporter(157, 1609, 17, 87, 1673, 0, map, false);
            }

            public void CreateTeleportersMap4(Map map)
            {
                //Yomotso Mines Entrance
                this.CreateTeleporter(257, 783, 63, 5, 128, -1, map, Map.Malas, false);
                this.CreateTeleporter(258, 783, 63, 5, 128, -1, map, Map.Malas, false);
                this.CreateTeleporter(259, 783, 63, 5, 128, -1, map, Map.Malas, false);
                this.CreateTeleporter(260, 783, 63, 5, 128, -1, map, Map.Malas, false);

                //Fan dancer Entrance
                this.CreateTeleporter(988, 194, 15, 67, 337, -1, map, Map.Malas, false);
                this.CreateTeleporter(988, 195, 15, 67, 337, -1, map, Map.Malas, false);
                this.CreateTeleporter(987, 196, 15, 67, 337, -1, map, Map.Malas, false);
                this.CreateTeleporter(988, 197, 18, 67, 337, -1, map, Map.Malas, false);
            }

            public void CreateTeleportersTrammel(Map map)
            {
                // Haven
                this.CreateTeleporter(3632, 2566, 0, 3632, 2566, 20, map, true);
            }

            public void CreateTeleportersFelucca(Map map)
            {
                // Star room
                this.CreateTeleporter(5140, 1773, 0, 5171, 1586, 0, map, false);
            }

            public int CreateTeleporters()
            {
                this.CreateTeleportersMap(Map.Felucca);
                this.CreateTeleportersMap(Map.Trammel);
                this.CreateTeleportersTrammel(Map.Trammel);
                this.CreateTeleportersFelucca(Map.Felucca);
                this.CreateTeleportersMap2(Map.Ilshenar);
                this.CreateTeleportersMap3(Map.Malas);
                this.CreateTeleportersMap4(Map.Tokuno);
                return this.m_Count;
            }
        }
    }
}