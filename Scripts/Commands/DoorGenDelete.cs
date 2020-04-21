using Server.Commands;
using Server.Items;
using System.Collections.Generic;

namespace Server
{
    public class DoorGeneratorDelete
    {
        private static readonly Rectangle2D[] m_BritRegions = new Rectangle2D[]
        {
            new Rectangle2D(new Point2D(250, 750), new Point2D(775, 1330)),
            new Rectangle2D(new Point2D(525, 2095), new Point2D(925, 2430)),
            new Rectangle2D(new Point2D(1025, 2155), new Point2D(1265, 2310)),
            new Rectangle2D(new Point2D(1635, 2430), new Point2D(1705, 2508)),
            new Rectangle2D(new Point2D(1775, 2605), new Point2D(2165, 2975)),
            new Rectangle2D(new Point2D(1055, 3520), new Point2D(1570, 4075)),
            new Rectangle2D(new Point2D(2860, 3310), new Point2D(3120, 3630)),
            new Rectangle2D(new Point2D(2470, 1855), new Point2D(3950, 3045)),
            new Rectangle2D(new Point2D(3425, 990), new Point2D(3900, 1455)),
            new Rectangle2D(new Point2D(4175, 735), new Point2D(4840, 1600)),
            new Rectangle2D(new Point2D(2375, 330), new Point2D(3100, 1045)),
            new Rectangle2D(new Point2D(2100, 1090), new Point2D(2310, 1450)),
            new Rectangle2D(new Point2D(1495, 1400), new Point2D(1550, 1475)),
            new Rectangle2D(new Point2D(1085, 1520), new Point2D(1415, 1910)),
            new Rectangle2D(new Point2D(1410, 1500), new Point2D(1745, 1795)),
            new Rectangle2D(new Point2D(5120, 2300), new Point2D(6143, 4095))
        };
        private static readonly Rectangle2D[] m_IlshRegions = new Rectangle2D[]
        {
            new Rectangle2D(new Point2D(0, 0), new Point2D(288 * 8, 200 * 8))
        };
        private static readonly Rectangle2D[] m_MalasRegions = new Rectangle2D[]
        {
            new Rectangle2D(new Point2D(0, 0), new Point2D(320 * 8, 256 * 8))
        };
        private static readonly int[] m_SouthFrames = new int[]
        {
            0x0006,
            0x0008,
            0x000B,
            0x001A,
            0x001B,
            0x001F,
            0x0038,
            0x0057,
            0x0059,
            0x005B,
            0x005D,
            0x0080,
            0x0081,
            0x0082,
            0x0084,
            0x0090,
            0x0091,
            0x0094,
            0x0096,
            0x0099,
            0x00A6,
            0x00A7,
            0x00AA,
            0x00AE,
            0x00B0,
            0x00B3,
            0x00C7,
            0x00C9,
            0x00F8,
            0x00FA,
            0x00FD,
            0x00FE,
            0x0100,
            0x0103,
            0x0104,
            0x0106,
            0x0109,
            0x0127,
            0x0129,
            0x012B,
            0x012D,
            0x012F,
            0x0131,
            0x0132,
            0x0134,
            0x0135,
            0x0137,
            0x0139,
            0x013B,
            0x014C,
            0x014E,
            0x014F,
            0x0151,
            0x0153,
            0x0155,
            0x0157,
            0x0158,
            0x015A,
            0x015D,
            0x015E,
            0x015F,
            0x0162,
            0x01CF,
            0x01D1,
            0x01D4,
            0x01FF,
            0x0204,
            0x0206,
            0x0208,
            0x020A
        };
        private static readonly int[] m_NorthFrames = new int[]
        {
            0x0006,
            0x0008,
            0x000D,
            0x001A,
            0x001B,
            0x0020,
            0x003A,
            0x0057,
            0x0059,
            0x005B,
            0x005D,
            0x0080,
            0x0081,
            0x0082,
            0x0084,
            0x0090,
            0x0091,
            0x0094,
            0x0096,
            0x0099,
            0x00A6,
            0x00A7,
            0x00AC,
            0x00AE,
            0x00B0,
            0x00C7,
            0x00C9,
            0x00F8,
            0x00FA,
            0x00FD,
            0x00FE,
            0x0100,
            0x0103,
            0x0104,
            0x0106,
            0x0109,
            0x0127,
            0x0129,
            0x012B,
            0x012D,
            0x012F,
            0x0131,
            0x0132,
            0x0134,
            0x0135,
            0x0137,
            0x0139,
            0x013B,
            0x014C,
            0x014E,
            0x014F,
            0x0151,
            0x0153,
            0x0155,
            0x0157,
            0x0158,
            0x015A,
            0x015D,
            0x015E,
            0x015F,
            0x0162,
            0x01CF,
            0x01D1,
            0x01D4,
            0x01FF,
            0x0201,
            0x0204,
            0x0208,
            0x020A
        };
        private static readonly int[] m_EastFrames = new int[]
        {
            0x0007,
            0x000A,
            0x001A,
            0x001C,
            0x001E,
            0x0037,
            0x0058,
            0x0059,
            0x005C,
            0x005E,
            0x0080,
            0x0081,
            0x0082,
            0x0084,
            0x0090,
            0x0092,
            0x0095,
            0x0097,
            0x0098,
            0x00A6,
            0x00A8,
            0x00AB,
            0x00AE,
            0x00AF,
            0x00B2,
            0x00C7,
            0x00C8,
            0x00EA,
            0x00F8,
            0x00F9,
            0x00FC,
            0x00FE,
            0x00FF,
            0x0102,
            0x0104,
            0x0105,
            0x0108,
            0x0127,
            0x0128,
            0x012B,
            0x012C,
            0x012E,
            0x0130,
            0x0132,
            0x0133,
            0x0135,
            0x0136,
            0x0138,
            0x013A,
            0x014C,
            0x014D,
            0x014F,
            0x0150,
            0x0152,
            0x0154,
            0x0156,
            0x0158,
            0x0159,
            0x015C,
            0x015E,
            0x0160,
            0x0163,
            0x01CF,
            0x01D0,
            0x01D3,
            0x01FF,
            0x0203,
            0x0205,
            0x0207,
            0x0209
        };
        private static readonly int[] m_WestFrames = new int[]
        {
            0x0007,
            0x000C,
            0x001A,
            0x001C,
            0x0021,
            0x0039,
            0x0058,
            0x0059,
            0x005C,
            0x005E,
            0x0080,
            0x0081,
            0x0082,
            0x0084,
            0x0090,
            0x0092,
            0x0095,
            0x0097,
            0x0098,
            0x00A6,
            0x00A8,
            0x00AD,
            0x00AE,
            0x00AF,
            0x00B5,
            0x00C7,
            0x00C8,
            0x00EA,
            0x00F8,
            0x00F9,
            0x00FC,
            0x00FE,
            0x00FF,
            0x0102,
            0x0104,
            0x0105,
            0x0108,
            0x0127,
            0x0128,
            0x012C,
            0x012E,
            0x0130,
            0x0132,
            0x0133,
            0x0135,
            0x0136,
            0x0138,
            0x013A,
            0x014C,
            0x014D,
            0x014F,
            0x0150,
            0x0152,
            0x0154,
            0x0156,
            0x0158,
            0x0159,
            0x015C,
            0x015E,
            0x0160,
            0x0163,
            0x01CF,
            0x01D0,
            0x01D3,
            0x01FF,
            0x0200,
            0x0203,
            0x0207,
            0x0209
        };
        private static Map m_Map;
        private static int m_Count;
        public static void Initialize()
        {
            CommandSystem.Register("DoorGenDelete", AccessLevel.Administrator, DoorGenDelete_OnCommand);
        }

        [Usage("DoorGenDelete")]
        [Description("Deletes doors by analyzing the map.")]
        public static void DoorGenDelete_OnCommand(CommandEventArgs e)
        {
            WeakEntityCollection.Delete("door");
            // Retained for backward compatibility
            Delete();
        }

        public static void Delete()
        {
            World.Broadcast(0x35, true, "Deleting doors, please wait.");

            Network.NetState.FlushAll();
            Network.NetState.Pause();

            m_Map = Map.Trammel;
            m_Count = 0;

            for (int i = 0; i < m_BritRegions.Length; ++i)
                Delete(m_BritRegions[i]);

            int trammelCount = m_Count;

            m_Map = Map.Felucca;
            m_Count = 0;

            for (int i = 0; i < m_BritRegions.Length; ++i)
                Delete(m_BritRegions[i]);

            int feluccaCount = m_Count;

            m_Map = Map.Ilshenar;
            m_Count = 0;

            for (int i = 0; i < m_IlshRegions.Length; ++i)
                Delete(m_IlshRegions[i]);

            int ilshenarCount = m_Count;

            m_Map = Map.Malas;
            m_Count = 0;

            for (int i = 0; i < m_MalasRegions.Length; ++i)
                Delete(m_MalasRegions[i]);

            int malasCount = m_Count;

            Network.NetState.Resume();

            World.Broadcast(0x35, true, "Door deletion complete. Trammel: {0}; Felucca: {1}; Ilshenar: {2}; Malas: {3};", trammelCount, feluccaCount, ilshenarCount, malasCount);
        }

        public static void DeleteDoor(int x, int y, int z)
        {
            int doorZ = z;
            int doorTop = doorZ + 20;

            m_Count += DeleteBaseDoor(m_Map, new Point3D(x, y, z));
        }

        public static void Delete(Rectangle2D region)
        {
            for (int rx = 0; rx < region.Width; ++rx)
            {
                for (int ry = 0; ry < region.Height; ++ry)
                {
                    int vx = rx + region.X;
                    int vy = ry + region.Y;

                    StaticTile[] tiles = m_Map.Tiles.GetStaticTiles(vx, vy);

                    for (int i = 0; i < tiles.Length; ++i)
                    {
                        StaticTile tile = tiles[i];

                        int id = tile.ID;
                        int z = tile.Z;

                        if (IsWestFrame(id))
                        {
                            if (IsEastFrame(vx + 2, vy, z))
                            {
                                DeleteDoor(vx + 1, vy, z);
                            }
                            else if (IsEastFrame(vx + 3, vy, z))
                            {
                                /*BaseDoor first = */
                                DeleteDoor(vx + 1, vy, z);
                                /*BaseDoor second = */
                                DeleteDoor(vx + 2, vy, z);
                                /*if (first != null && second != null)
                                {
                                first.Link = second;
                                second.Link = first;
                                }
                                else
                                {
                                if (first != null)
                                first.Delete();
                                if (second != null)
                                second.Delete();
                                }*/
                            }
                        }
                        else if (IsNorthFrame(id))
                        {
                            if (IsSouthFrame(vx, vy + 2, z))
                            {
                                DeleteDoor(vx, vy + 1, z);
                            }
                            else if (IsSouthFrame(vx, vy + 3, z))
                            {
                                /*BaseDoor first = */
                                DeleteDoor(vx, vy + 1, z);
                                /*BaseDoor second = */
                                DeleteDoor(vx, vy + 2, z);
                                /*if (first != null && second != null)
                                {
                                first.Link = second;
                                second.Link = first;
                                }
                                else
                                {
                                if (first != null)
                                first.Delete();
                                if (second != null)
                                second.Delete();
                                }*/
                            }
                        }
                    }
                }
            }
        }

        public static bool IsFrame(int id, int[] list)
        {
            id &= 0x3FFF;

            if (id > list[list.Length - 1])
                return false;

            for (int i = 0; i < list.Length; ++i)
            {
                int delta = id - list[i];

                if (delta < 0)
                    return false;
                else if (delta == 0)
                    return true;
            }

            return false;
        }

        public static bool IsNorthFrame(int id)
        {
            return IsFrame(id, m_NorthFrames);
        }

        public static bool IsSouthFrame(int id)
        {
            return IsFrame(id, m_SouthFrames);
        }

        public static bool IsWestFrame(int id)
        {
            return IsFrame(id, m_WestFrames);
        }

        public static bool IsEastFrame(int id)
        {
            return IsFrame(id, m_EastFrames);
        }

        public static bool IsEastFrame(int x, int y, int z)
        {
            StaticTile[] tiles = m_Map.Tiles.GetStaticTiles(x, y);

            for (int i = 0; i < tiles.Length; ++i)
            {
                StaticTile tile = tiles[i];

                if (tile.Z == z && IsEastFrame(tile.ID))
                    return true;
            }

            return false;
        }

        public static bool IsSouthFrame(int x, int y, int z)
        {
            StaticTile[] tiles = m_Map.Tiles.GetStaticTiles(x, y);

            for (int i = 0; i < tiles.Length; ++i)
            {
                StaticTile tile = tiles[i];

                if (tile.Z == z && IsSouthFrame(tile.ID))
                    return true;
            }

            return false;
        }

        private static int DeleteBaseDoor(Map map, Point3D p)
        {
            Queue<Item> m_Queue = new Queue<Item>();

            IPooledEnumerable eable = map.GetItemsInRange(p, 0);

            foreach (Item item in eable)
            {
                if (item is BaseDoor)
                {
                    int delta = item.Z - p.Z;

                    if (delta >= -12 && delta <= 12)
                        m_Queue.Enqueue(item);
                }
            }

            eable.Free();

            int count = m_Queue.Count;

            while (m_Queue.Count > 0)
                (m_Queue.Dequeue()).Delete();

            return count;
        }
    }
}
