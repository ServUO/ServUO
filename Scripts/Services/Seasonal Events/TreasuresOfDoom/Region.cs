using Server.Spells;

namespace Server.Regions
{
    public class MonestaryRegion : BaseRegion
    {
        public static void Initialize()
        {
            new MonestaryRegion();
        }

        public MonestaryRegion()
            : base("Doom Monestary", Map.Malas, DefaultPriority, new Rectangle2D(64, 204, 99, 37))
        {
            GoLocation = new Point3D(79, 223, -1);

            Register();
        }

        public override bool CheckTravel(Mobile traveller, Point3D p, TravelCheckType type)
        {
            if (traveller.AccessLevel > AccessLevel.Player)
            {
                return true;
            }

            return type == TravelCheckType.TeleportTo || type == TravelCheckType.TeleportFrom;
        }
    }
}
