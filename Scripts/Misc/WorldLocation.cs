namespace Server
{
    public class WorldLocation
    {
        public Point3D Location { get; set; }
        public Map Map { get; set; }

        public WorldLocation(int x, int y, int z, Map map)
            : this(new Point3D(x, y, z), map)
        {
        }

        public WorldLocation(Point3D p, Map map)
        {
            Location = p;
            Map = map;
        }

        public WorldLocation(IEntity e)
        {
            Location = e.Location;
            Map = e.Map;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}) [{3}]", Location.X, Location.Y, Location.Z, Map == null ? "(Null)" : Map.ToString());
        }
    }
}
