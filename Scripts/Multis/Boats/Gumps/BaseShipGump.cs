using Server.Multis;

namespace Server.Gumps
{
    public class BaseShipGump : Gump
    {
        public static readonly int NAHue = 0x5EF7;
        public static readonly int PassengerHue = 0x1CFF;
        public static readonly int CrewHue = 0x1FE7;
        public static readonly int OfficerHue = 0x7FE7;
        public static readonly int DenyHue = 0x7CE7;
        public static readonly int CaptainHue = 0x7DE7;
        public static readonly int LabelColor = 0x7FFF;
        public static readonly int NoHue = 0x3DEF;

        public BaseShipGump(BaseGalleon galleon)
            : base(100, 100)
        {
            AddPage(0);

            AddBackground(0, 0, 320, 385, 0xA3C);
            AddHtmlLocalized(10, 10, 300, 18, 1149724, 0x7FEF, false, false); //<CENTER>Passenger and Crew Manifest</CENTER>

            string shipName = "unnamed ship";

            if (galleon.ShipName != null && galleon.ShipName != string.Empty && galleon.ShipName != "")
                shipName = galleon.ShipName;

            AddHtmlLocalized(10, 38, 75, 18, 1149761, LabelColor, false, false); //Ship:
            AddLabel(80, 38, 0x53, shipName);

            AddHtmlLocalized(10, 56, 75, 18, 1149762, LabelColor, false, false); //Owner:
            AddLabel(80, 56, 0x53, galleon.Owner != null ? galleon.Owner.Name : "Unknown");
        }

        public int GetHue(SecurityLevel level)
        {
            switch (level)
            {
                case SecurityLevel.Captain: return CaptainHue;
                case SecurityLevel.Officer: return OfficerHue;
                case SecurityLevel.Crewman: return CrewHue;
                case SecurityLevel.Passenger: return PassengerHue;
                case SecurityLevel.NA: return NAHue;
                case SecurityLevel.Denied: return DenyHue;
                default: return LabelColor;
            }
        }

        public static int GetLevel(SecurityLevel level)
        {
            switch (level)
            {
                default:
                case SecurityLevel.Denied: return 1149726;
                case SecurityLevel.Passenger: return 1149727;
                case SecurityLevel.Crewman: return 1149728;
                case SecurityLevel.Officer: return 1149729;
                case SecurityLevel.Captain: return 1149730;
                case SecurityLevel.NA: return 1149725;
            }
        }
    }
}
