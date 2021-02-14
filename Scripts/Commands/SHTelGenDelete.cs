using Server.Items;
using System;

namespace Server.Commands
{
    public class SHGenTeleporterDelete
    {
        public static void Initialize()
        {
            CommandSystem.Register("SHTelGenDelete", AccessLevel.Administrator, SHTelGenDelete_OnCommand);
        }

        [Usage("SHTelGenDelete")]
        [Description("Deletes solen hives teleporters.")]
        public static void SHTelGenDelete_OnCommand(CommandEventArgs e)
        {
            World.Broadcast(0x35, true, "Solen hives teleporters are being deleted, please wait.");

            DateTime startTime = DateTime.UtcNow;

            int count = new SHTeleporterCreator().CreateSHTeleporters();

            DateTime endTime = DateTime.UtcNow;

            World.Broadcast(0x35, true, "{0} solen hives teleporters have been deleted. The entire process took {1:F1} seconds.", count, (endTime - startTime).TotalSeconds);
        }

        public class SHTeleporterCreator
        {
            private int m_Count;
            public SHTeleporterCreator()
            {
                m_Count = 0;
            }

            public static SHTeleporter FindSHTeleporter(Map map, Point3D p)
            {
                IPooledEnumerable eable = map.GetItemsInRange(p, 0);

                foreach (Item item in eable)
                {
                    if (item is SHTeleporter teleporter && teleporter.Z == p.Z)
                    {
                        eable.Free();
                        return teleporter;
                    }
                }

                eable.Free();
                return null;
            }

            public SHTeleporter AddSHT(Map map, int x, int y, int z)
            {
                Point3D p = new Point3D(x, y, z);
                SHTeleporter tele = FindSHTeleporter(map, p);

                if (tele != null)
                {
                    m_Count++;
                }

                return tele;
            }

            public void AddSHTCouple(Map map, int x1, int y1, int z1, int x2, int y2, int z2)
            {
                SHTeleporter tele1 = AddSHT(map, x1, y1, z1);
                SHTeleporter tele2 = AddSHT(map, x2, y2, z2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }
            }

            public void AddSHTCouple(int x1, int y1, int z1, int x2, int y2, int z2)
            {
                AddSHTCouple(Map.Trammel, x1, y1, z1, x2, y2, z2);
                AddSHTCouple(Map.Felucca, x1, y1, z1, x2, y2, z2);
            }

            public int CreateSHTeleporters()
            {
                SHTeleporter tele1, tele2;

                AddSHTCouple(2608, 763, 0, 5918, 1794, 0);
                AddSHTCouple(5897, 1877, 0, 5871, 1867, 0);
                AddSHTCouple(5852, 1848, 0, 5771, 1867, 0);

                tele1 = AddSHT(Map.Trammel, 5747, 1895, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Trammel, 5658, 1898, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Felucca, 5747, 1895, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Felucca, 5658, 1898, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                AddSHTCouple(5727, 1894, 0, 5756, 1794, 0);
                AddSHTCouple(5784, 1929, 0, 5700, 1929, 0);

                tele1 = AddSHT(Map.Trammel, 5711, 1952, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Trammel, 5657, 1954, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Felucca, 5711, 1952, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Felucca, 5657, 1954, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Trammel, 5655, 2018, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Trammel, 1690, 2789, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Felucca, 5655, 2018, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Felucca, 1690, 2789, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                AddSHTCouple(5809, 1905, 0, 5876, 1891, 0);

                tele1 = AddSHT(Map.Trammel, 5814, 2015, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Trammel, 5913, 1893, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Felucca, 5814, 2015, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Felucca, 5913, 1893, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                AddSHTCouple(5919, 2021, 0, 1724, 814, 0);

                tele1 = AddSHT(Map.Trammel, 5654, 1791, 0);
                tele2 = AddSHT(Map.Trammel, 730, 1451, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Trammel, 5734, 1859, 0)/*.ChangeDest(tele2)*/;
                if (tele1 != null)
                {
                    tele1.Delete();
                }

                tele1 = AddSHT(Map.Felucca, 5654, 1791, 0);
                tele2 = AddSHT(Map.Felucca, 730, 1451, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Felucca, 5734, 1859, 0)/*.ChangeDest(tele2)*/;

                if (tele1 != null)
                {
                    tele1.Delete();
                }

                return m_Count;
            }
        }
    }
}
