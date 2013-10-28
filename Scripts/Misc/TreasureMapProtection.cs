using System;
using System.IO;
using Server.Regions;

namespace Server
{
    public class TreasureRegion : BaseRegion
    {
        private const int Range = 5;// No house may be placed within 5 tiles of the treasure
        public TreasureRegion(int x, int y, Map map)
            : base(null, map, Region.DefaultPriority, new Rectangle2D(x - Range, y - Range, 1 + (Range * 2), 1 + (Range * 2)))
        {
            this.GoLocation = new Point3D(x, y, map.GetAverageZ(x, y));

            this.Register();
        }

        public static void Initialize()
        {
            string filePath = Path.Combine(Core.BaseDirectory, "Data/treasure.cfg");
            int i = 0, x = 0, y = 0;

            if (File.Exists(filePath))
            {
                using (StreamReader ip = new StreamReader(filePath))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        i++;

                        try
                        {
                            string[] split = line.Split(' ');

                            x = Convert.ToInt32(split[0]);
                            y = Convert.ToInt32(split[1]);

                            try
                            {
                                new TreasureRegion(x, y, Map.Felucca);
                                new TreasureRegion(x, y, Map.Trammel);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("{0} {1} {2} {3}", i, x, y, e);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Warning: Error in Line '{0}' of Data/treasure.cfg", line);
                        }
                    }
                }
            }
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return false;
        }

        public override void OnEnter(Mobile m)
        {
            if (m.IsStaff())
                m.SendMessage("You have entered a protected treasure map area.");
        }

        public override void OnExit(Mobile m)
        {
            if (m.IsStaff())
                m.SendMessage("You have left a protected treasure map area.");
        }
    }
}