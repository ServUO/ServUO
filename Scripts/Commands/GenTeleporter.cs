using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using System.IO;

namespace Server.Commands
{
    class Location : IComparable
    {
        public int X;
        public int Y;
        public int Z;
        public Map Map;
        public Location(int x, int y, int z, Map m)
        {
            X = x;
            Y = y;
            Z = z;
            Map = m;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Location))
                return GetHashCode().CompareTo(obj.GetHashCode());

            Location l = (Location)obj;
            if (l.Map.MapID != Map.MapID)
                return l.Map.MapID - Map.MapID;
            if (l.X != X)
                return l.X - X;
            if (l.Y != Y)
                return l.Y - Y;
            return l.Z - Z;
        }

        public override int GetHashCode()
        {
            string hashString = String.Format("{0}-{1}-{2}-{3}",
                X, Y, Z, Map.MapID);
            return hashString.GetHashCode();
        }
    }
    class TelDef
    {
        public Location Source;
        public Location Destination;
        public bool Back;
        public TelDef(Location s, Location d, bool b)
        {
            Source = s;
            Destination = d;
            Back = b;
        }
    }
    public class GenTeleporter
    {
        private static string m_Path = Path.Combine("Data", "teleporters.csv");
        private static char[] m_Sep = { ',' };

        public GenTeleporter()
        {
        }

        public static void Initialize()
        {
            CommandSystem.Register("TelGen", AccessLevel.Administrator, new CommandEventHandler(GenTeleporter_OnCommand));
            CommandSystem.Register("TelGenDelete", AccessLevel.Administrator, new CommandEventHandler(TelGenDelete_OnCommand));
        }

        private static void TelGenDelete_OnCommand(CommandEventArgs e)
        {
            WeakEntityCollection.Delete("tel");
        }

        [Usage("TelGen")]
        [Description("Generates world/dungeon teleporters for all facets.")]
        public static void GenTeleporter_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Generating teleporters, please wait.");

            TeleportersCreator c = new TeleportersCreator();

            StreamReader reader = new StreamReader(m_Path);

            string line;
            int lineNum = 0;
            while((line = reader.ReadLine()) != null)
            {
                ++lineNum;
                line = line.Trim();
                if (line.StartsWith("#"))
                    continue;
                string[] parts = line.Split(m_Sep);
                if(parts.Length != 9)
                {
                    e.Mobile.SendMessage(33, String.Format("Bad teleporter definition on line {0}", lineNum));
                    continue;
                }
                try
                {
                    c.CreateTeleporter(
                        int.Parse(parts[0]),
                        int.Parse(parts[1]),
                        int.Parse(parts[2]),
                        int.Parse(parts[4]),
                        int.Parse(parts[5]),
                        int.Parse(parts[6]),
                        Map.Parse(parts[3]),
                        Map.Parse(parts[7]),
                        bool.Parse(parts[8])
                    );
                }
                catch (FormatException)
                {
                    e.Mobile.SendMessage(33, String.Format("Bad number format on line {0}", lineNum));
                }
                catch(ArgumentException ex)
                {
                    e.Mobile.SendMessage(33, String.Format("Argument Execption {0} on line {1}", ex.Message, lineNum));
                }
            }
            reader.Close();

            e.Mobile.SendMessage("Teleporter generating complete.");
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
				
                    Teleporter tel = new Teleporter(pointDestination, mapDestination);
					WeakEntityCollection.Add("tel", tel);

                    tel.MoveToWorld(pointLocation, mapLocation);
                }

                if (back && !FindTeleporter(mapDestination, pointDestination))
                {
                    this.m_Count++;

                    Teleporter telBack = new Teleporter(pointLocation, mapLocation);
					WeakEntityCollection.Add("tel", telBack);

                    telBack.MoveToWorld(pointDestination, mapDestination);
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
        }
    }
}