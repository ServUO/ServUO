using System;
using Server;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Engines.CityLoyalty;

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