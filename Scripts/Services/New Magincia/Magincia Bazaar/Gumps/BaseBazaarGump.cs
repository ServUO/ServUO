using Server.Gumps;

namespace Server.Engines.NewMagincia
{
    public class BaseBazaarGump : Gump
    {
        public const int RedColor = 0xB22222;
        public const int BlueColor = 0x000080;
        public const int OrangeColor = 0x804000;
        public const int GreenColor = 0x008040;
        public const int DarkGreenColor = 0x008000;
        public const int YellowColor = 0xFFFF00;
        public const int GrayColor = 0x808080;

        public const int RedColor16 = 0x4000;
        public const int BlueColor16 = 0x10;
        public const int OrangeColor16 = 0x4100;
        public const int GreenColor16 = 0x208;
        public const int DarkGreenColor16 = 0x200;
        public const int YellowColor16 = 0xFFE0;
        public const int GrayColor16 = 0xC618;

        public const int LabelHueBlue = 0xCC;

        public BaseBazaarGump() : this(520, 700)
        {
        }

        public BaseBazaarGump(int width, int height) : base(100, 100)
        {
            AddBackground(0, 0, width, height, 9300);

            if (!(this is CommodityTargetGump))
            {
                AddButton(width - 40, height - 30, 4020, 4022, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(width - 150, height - 30, 100, 20, 1114514, "#1060675", 0x0, false, false); // CLOSE
            }
        }

        protected string Color(string str, int color)
        {
            return string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, str);
        }

        protected string FormatAmt(int amount)
        {
            return amount.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        }

        protected string FormatStallName(string str)
        {
            return string.Format("<DIV ALIGN=CENTER><i>{0}</i></DIV>", str);
        }

        protected string FormatBrokerName(string str)
        {
            return string.Format("<DIV ALIGN=CENTER>{0}</DIV>", str);
        }

        protected string AlignRight(string str)
        {
            return string.Format("<DIV ALIGN=RIGHT>{0}</DIV>", str);
        }

        protected string AlignLeft(string str)
        {
            return string.Format("<DIV ALIGN=LEFT>{0}</DIV>", str);
        }
    }
}
