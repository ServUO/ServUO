using System;
using Server;
using Server.Items;
using Server.Multis;
using Server.Network;

namespace Server.Gumps
{
    public class BaseShipGump : Gump
    {
        public static readonly int CaptainHue   = 31137;
        public static readonly int OfficerHue = 31681;
        public static readonly int CrewHue      = 14273;
        public static readonly int PassengerHue = 1080;
        public static readonly int LabelColor   = 0xFFFFFF;
        public static readonly int NoHue        = 12684;

        public virtual TextDefinition Title { get { return new TextDefinition(1149724); } }

        public BaseShipGump(int x, int y, BaseGalleon galleon)
            : base(x, y)
        {
            AddBackground(0, 0, 350, 400, 2620);

            if (Title != null && Title.Number > 0)
                AddHtmlLocalized(0, 15, 350, 20, Title.Number, LabelColor, false, false); //<CENTER>Passenger and Crew Manifest</CENTER>
            else if (Title != null && Title.String != null)
                AddHtml(0, 15, 350, 20, String.Format("<CENTER><BASEFONT COLOR=#FFFFFF>{0}</CENTER>", Title.String), false, false);

            string shipName = "a ship with no name";

            if (galleon.ShipName != null && galleon.ShipName != String.Empty && galleon.ShipName != "")
                shipName = galleon.ShipName;

            AddHtmlLocalized(10, 40, 80, 20, 1149761, LabelColor, false, false);       //Ship:
            AddHtml(90, 40, 250, 20, Color(shipName, "DarkCyan"), false, false);

            AddHtmlLocalized(10, 60, 80, 20, 1149762, LabelColor, false, false);       //Owner:
            AddHtml(90, 60, 250, 20, Color(galleon.Owner != null ? galleon.Owner.Name : "Unknown", "DarkCyan"), false, false);
        }

        public string Color(string text, string colorName)
        {
            return String.Format("<BASEFONT COLOR={0}>{1}</BASEFONT>", colorName, text);
        }

        public int GetHue(SecurityLevel level)
        {
            switch (level)
            {
                case SecurityLevel.Captain: return CaptainHue;
                case SecurityLevel.Officer: return OfficerHue;
                case SecurityLevel.Crewman: return CrewHue;
                case SecurityLevel.Passenger: return PassengerHue;
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
            }
        }
    }
}