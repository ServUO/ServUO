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
                    if (item is SHTeleporter && item.Z == p.Z)
                    {
                        eable.Free();
                        return (SHTeleporter)item;
                    }
                }

                eable.Free();
                return null;
            }

            public SHTeleporter AddSHT(Map map, bool ext, int x, int y, int z)
            {
                Point3D p = new Point3D(x, y, z);
                SHTeleporter tele = FindSHTeleporter(map, p);

                if (tele != null)
                {
                    /*tele = new SHTeleporter(ext);
                    tele.MoveToWorld(p, map);*/
                    m_Count++;
                }

                return tele;
            }

            /*public static void Link(SHTeleporter tele1, SHTeleporter tele2)
            {
            tele1.ChangeDest(tele2);
            tele2.ChangeDest(tele1);
            }*/
            public void AddSHTCouple(Map map, bool ext1, int x1, int y1, int z1, bool ext2, int x2, int y2, int z2)
            {
                SHTeleporter tele1 = AddSHT(map, ext1, x1, y1, z1);
                SHTeleporter tele2 = AddSHT(map, ext2, x2, y2, z2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }
                //Link(tele1, tele2);
            }

            public void AddSHTCouple(bool ext1, int x1, int y1, int z1, bool ext2, int x2, int y2, int z2)
            {
                AddSHTCouple(Map.Trammel, ext1, x1, y1, z1, ext2, x2, y2, z2);
                AddSHTCouple(Map.Felucca, ext1, x1, y1, z1, ext2, x2, y2, z2);
            }

            public int CreateSHTeleporters()
            {
                SHTeleporter tele1, tele2;

                AddSHTCouple(true, 2608, 763, 0, false, 5918, 1794, 0);
                AddSHTCouple(false, 5897, 1877, 0, false, 5871, 1867, 0);
                AddSHTCouple(false, 5852, 1848, 0, false, 5771, 1867, 0);

                tele1 = AddSHT(Map.Trammel, false, 5747, 1895, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Trammel, false, 5658, 1898, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Felucca, false, 5747, 1895, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Felucca, false, 5658, 1898, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                AddSHTCouple(false, 5727, 1894, 0, false, 5756, 1794, 0);
                AddSHTCouple(false, 5784, 1929, 0, false, 5700, 1929, 0);

                tele1 = AddSHT(Map.Trammel, false, 5711, 1952, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Trammel, false, 5657, 1954, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Felucca, false, 5711, 1952, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Felucca, false, 5657, 1954, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Trammel, false, 5655, 2018, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Trammel, true, 1690, 2789, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Felucca, false, 5655, 2018, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Felucca, true, 1690, 2789, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                AddSHTCouple(false, 5809, 1905, 0, false, 5876, 1891, 0);

                tele1 = AddSHT(Map.Trammel, false, 5814, 2015, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Trammel, false, 5913, 1893, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Felucca, false, 5814, 2015, 0);
                //tele1.LeftTele.TeleOffset = new Point3D(-1, 3, 0);
                tele2 = AddSHT(Map.Felucca, false, 5913, 1893, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                AddSHTCouple(false, 5919, 2021, 0, true, 1724, 814, 0);

                tele1 = AddSHT(Map.Trammel, false, 5654, 1791, 0);
                tele2 = AddSHT(Map.Trammel, true, 730, 1451, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Trammel, false, 5734, 1859, 0)/*.ChangeDest(tele2)*/;
                if (tele1 != null)
                {
                    tele1.Delete();
                }

                tele1 = AddSHT(Map.Felucca, false, 5654, 1791, 0);
                tele2 = AddSHT(Map.Felucca, true, 730, 1451, 0);
                //Link(tele1, tele2);

                if (tele1 != null)
                {
                    tele1.Delete();
                }
                if (tele2 != null)
                {
                    tele2.Delete();
                }

                tele1 = AddSHT(Map.Felucca, false, 5734, 1859, 0)/*.ChangeDest(tele2)*/;

                if (tele1 != null)
                {
                    tele1.Delete();
                }

                return m_Count;
            }
        }
    }
}
