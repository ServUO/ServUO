using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.NewMagincia
{
	public class BaseBazaarGump : Gump
	{
        public const int RedColor = 0xB22222;
        public const int BlueColor = 0x0000FF;
        public const int OrangeColor = 0xB8860B;
        public const int GreenColor = 0x008000;
        public const int YellowColor = 0xFFFF00;
        public const int GrayColor = 0x808080;

        public const int RedColor16 = 0x4800;
        public const int BlueColor16 = 0x001F;
        public const int OrangeColor16 = 0xB104;
        public const int GreenColor16 = 0x0400;
        public const int YellowColor16 = 0xFFE0;
        public const int GrayColor16 = 0xC618;

        public const int LabelHueBlue = 0x4F1;
		
        public BaseBazaarGump() : this(520, 520)
        {
        }

		public BaseBazaarGump(int width, int height) : base(50, 50)
		{
            AddBackground(0, 0, width, height, 9300);
			
			AddButton(480, 490, 4020, 4022, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(430, 490, 60, 16, 3000363, false, false); // CLOSE
		}
		
		protected string Color(string str, int color)
		{
			return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, str);
		}

        protected string FormatAmt(int amount)
		{
            if (amount == 0)
                return "0";
			return amount.ToString("###,###,###");
		}

        protected string FormatStallName(string str)
        {
            return String.Format("<DIV ALIGN=CENTER><i>{0}</i></DIV>", str);
        }

        protected string FormatBrokerName(string str)
        {
            return String.Format("<DIV ALIGN=CENTER>{0}</DIV>", str);
        }

        protected string AlignRight(string str)
        {
            return String.Format("<DIV ALIGN=RIGHT>{0}</DIV>", str);
        }

        protected string AlignLeft(string str)
        {
            return String.Format("<DIV ALIGN=LEFT>{0}</DIV>", str);
        }
	}
}