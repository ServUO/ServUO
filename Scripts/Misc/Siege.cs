using System;
using Server.Spells;

namespace Server
{
    public static class Siege
    {
        public static bool SiegeShard = Config.Get("Siege.IsSiege", false);

        /// <summary>
        /// Called in SpellHelper.cs CheckTravel method
        /// </summary>
        /// <param name="m"></param>
        /// <param name="type"></param>
        /// <returns>False fails travel check. True must pass other travel checks in SpellHelper.cs</returns>
        public static bool CheckTravel(Mobile m, Point3D p, Map destMap, TravelCheckType type)
        {
            switch (type)
            {
                case TravelCheckType.RecallFrom:
                case TravelCheckType.RecallTo:
                    {
                        m.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
                        return false;
                    }
                case TravelCheckType.GateFrom:
                case TravelCheckType.GateTo:
                case TravelCheckType.Mark:
                    {
                        return CanTravelTo(m, p, map);
                    }
                case TravelCheckType.TeleportFrom:
                case TravelCheckType.TeleportTo:
                    {
                        return true;
                    }
            }
        }

        public static bool CanTravelTo(Mobile m, IPoint3D p, Map map)
        {
            return Region.Find(p, map) is DungeonRegion || SpellHelper.IsAnyT2A(map, p) || SpellHelper.IsIlshenar(map, p);
        }
    }
}