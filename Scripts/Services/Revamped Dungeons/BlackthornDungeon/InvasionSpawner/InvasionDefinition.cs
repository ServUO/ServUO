namespace Server.Engines.Blackthorn
{
    public class InvasionDefinition
    {
        public Rectangle2D[] SpawnRecs { get; set; }
        public Point3D BeaconLoc { get; set; }

        public InvasionDefinition(Rectangle2D[] spawn, Point3D beaconLoc)
        {
            SpawnRecs = spawn;
            BeaconLoc = beaconLoc;
        }
    }
}