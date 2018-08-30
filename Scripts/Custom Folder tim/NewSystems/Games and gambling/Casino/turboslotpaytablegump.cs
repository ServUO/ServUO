using System;
using Server;
using Server.Items;
using Server.Gumps;

namespace Server.Gumps
{
	public class TurboSlotPayTableGump : Gump
	{
		private TurboSlot m_Slot;
		private int[] m_Symbols;
		private int[] m_jackpotmultiplier;

		public enum BonusRoundType { None, MinerMadness }
        public enum ScatterType { None, Any, LeftOnly };
        public enum SlotThemeType { Classic, ClassicII, ClassicIII, FarmerFaceoff, GruesomeGambling, Holiday1, LadyLuck, MinerMadness, OffToTheRaces, Pirates, PowerScrolls, StatScrolls, TailorTreats, TrophyHunter }       

		public TurboSlotPayTableGump( TurboSlot Slot, int[] Symbols) : base( 25, 25 )
		{
			m_Slot = Slot;
			m_Symbols = Symbols;
			
			int anybarsindex = m_Symbols[10];
			int totalsymbols = m_Symbols[11];
			int dispx = m_Symbols[12];
			int dispy = m_Symbols[13];
			int backgroundcolor = m_Symbols[14];
			int titlecolor = m_Symbols[15];
            int titlestyle = m_Symbols[16];
			int headingcolor1 = m_Symbols[17];
			int headingcolor2 = m_Symbols[18];
			int paycolor = m_Symbols[19];
            decimal payout;
            int tilecolor = 9304;
            if ((SlotThemeType)m_Slot.SlotTheme == SlotThemeType.GruesomeGambling || (SlotThemeType)m_Slot.SlotTheme == SlotThemeType.StatScrolls)
                tilecolor = 9204;
            else if ((SlotThemeType)m_Slot.SlotTheme == SlotThemeType.Holiday1)
                tilecolor = 0xBBC;
            else if ((SlotThemeType)m_Slot.SlotTheme == SlotThemeType.FarmerFaceoff || (SlotThemeType)m_Slot.SlotTheme == SlotThemeType.OffToTheRaces
                                    || (SlotThemeType)m_Slot.SlotTheme == SlotThemeType.TrophyHunter)
                tilecolor = 2524;

			string text = null;
			if (backgroundcolor == 0)
				text = String.Format( "<body bgcolor=\"black\"><basefont SIZE={0} COLOR=\"#{1:x6}\"><CENTER>{2}</CENTER></basefont></body>",titlestyle, titlecolor, m_Slot.Name);
			else if (backgroundcolor < 0)
				text = String.Format( "<basefont SIZE={0} COLOR=\"#{1:x6}\"><CENTER>{2}</CENTER></basefont>",titlestyle, titlecolor, m_Slot.Name);
			else
				text = String.Format( "<body bgcolor=\"#{0:x6}\"><basefont SIZE={1} COLOR=\"#{2:x6}\"><CENTER>{3}</CENTER></basefont></body>",backgroundcolor, titlestyle, titlecolor, m_Slot.Name);

			bool progressive = false;
			int ProgressiveHue = 0;

			if ((m_Slot.ProgSlotMaster != null && !m_Slot.ProgSlotMaster.Deleted && ((TurboSlot) m_Slot.ProgSlotMaster).ProgIsMaster) || m_Slot.ProgIsMaster)
			{
				progressive = true;
				if (m_Slot.ProgIsMaster)
					ProgressiveHue = m_Slot.Hue;
				else
					ProgressiveHue = ((TurboSlot) m_Slot.ProgSlotMaster).Hue;
				ProgressiveHue += -1;
				if (ProgressiveHue < 0)
					ProgressiveHue = 1149;
			}


			bool bonusround = ((BonusRoundType) m_Slot.BonusRound != BonusRoundType.None);// ? true:false;
			int tiledlength = (30 * totalsymbols) + (dispy * 7);
			//int backgroundlength = 425 (30 * totalsymbols) + (dispy * 10);
			int backgroundlength = 185 + (30 * totalsymbols) + (dispy * 10);
            if ((ScatterType)m_Slot.ScatterPay == ScatterType.None)
				backgroundlength = (backgroundlength - 90) - (dispy*2);
            else if ((ScatterType)m_Slot.ScatterPay == ScatterType.LeftOnly)
            {
                backgroundlength = (backgroundlength - 30);
                tiledlength += 60 + (dispy * 2);
            }

			if (m_Slot.AnyBars)
				tiledlength += 30 + dispy;
			else
				backgroundlength = (backgroundlength - 30) - dispy;
			if (bonusround)
			{
				tiledlength -= (30 + dispy);
				backgroundlength = backgroundlength + 30;
			}
            if ((SlotThemeType)m_Slot.SlotTheme == SlotThemeType.PowerScrolls)
            {
                tiledlength += 35;
                backgroundlength += 35;
            }
			Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;
			AddPage(0);
            int backgroundx = 0;
            if ((int)m_Slot.JackpotRewards != 0)
                    backgroundx = 35;
			AddBackground(37, 34, 180+backgroundx+dispx*3, backgroundlength, 5120);

			AddHtml( 45, 41, 165+backgroundx+dispx*3, 22, @text, (bool)false, (bool)false);

			AddImageTiled(45, 84, 102+dispx*3, tiledlength, tilecolor);
			AddLabel(45, 64, headingcolor1, @"Pay Table");
            if (progressive)
            {
                m_Slot.GetJackpotPayoutStr(0, out payout);
                payout = payout * 100;
                AddLabel(147 + dispx * 3, 67, ProgressiveHue, @"(Progessive)");
            }
			int starty = 85;
			for (int i = 0; i < totalsymbols ; i++)
			{
				if (!bonusround || (bonusround && i != m_Symbols[8]))
				{
					ShowSymbol(45, starty, m_Symbols[i]);
					ShowSymbol(75+dispx, starty, m_Symbols[i]);
					ShowSymbol(105+dispx*2, starty, m_Symbols[i]);
                    if (i == 0 && progressive)
                        ShowPayout(155 + dispx * 3, starty, paycolor, m_Slot.GetJackpotPayoutStr(0, out payout));
                    else
					    ShowPayout(155+dispx*3, starty, paycolor, m_Slot.GetJackpotPayoutStr(i, out payout));
					starty += 30 + dispy;
				}
				if (i==anybarsindex && m_Slot.AnyBars)
				{
					ShowSymbol(45,  starty, m_Symbols[0]);
					ShowSymbol(105+dispx*2, starty, m_Symbols[anybarsindex]);
					AddLabel(46+dispx+dispx/2, starty+3, headingcolor2, @"Any Three Above"); // was 65
					ShowPayout(155+dispx*3, starty, paycolor, m_Slot.GetJackpotPayoutStr(8, out payout));
					starty += 30 + dispy;
				}

			}

            if ((ScatterType)Slot.ScatterPay != ScatterType.None)
            {
                if ((ScatterType)Slot.ScatterPay == ScatterType.Any)
                {
                    starty += 5;
                    int ScatterSymbol = m_Symbols[m_Symbols[9]];
                    if ((ScatterType)m_Slot.ScatterPay == ScatterType.Any)
                        AddLabel(45, starty, headingcolor1, @"Scatter Pay");
                    else
                        AddLabel(45, starty, headingcolor1, @"Extra Chance");
                    starty += 25;
                    AddImageTiled(45, starty - 4, 101 + dispx * 3, 64 + dispy, tilecolor);
                    ShowSymbol(45, starty, ScatterSymbol);
                    AddLabel(74 + dispx + dispx / 2, starty - 2, headingcolor2, @"Any One");
                    ShowPayout(154 + dispx * 3, starty, paycolor, m_Slot.GetJackpotPayoutStr(9, out payout));
                    starty += 30 + dispy / 2;
                    ShowSymbol(45, starty, ScatterSymbol);
                    ShowSymbol(75 + dispx, starty, ScatterSymbol);
                    AddLabel(74 + dispx + dispx / 2, starty - 2, headingcolor2, @"Any Two");
                    ShowPayout(154 + dispx * 3, starty, paycolor, m_Slot.GetJackpotPayoutStr(10, out payout));
                }
                else
                {
                    int ScatterSymbol = m_Symbols[m_Symbols[9]];
                    ShowSymbol(45, starty, ScatterSymbol);
                    ShowSymbol(75 + dispx, starty, ScatterSymbol);
                    AddLabel(105 + dispx*2 + dispx / 2, starty+2, headingcolor2, @"Any");
                    ShowPayout(154 + dispx * 3, starty, paycolor, m_Slot.GetJackpotPayoutStr(10, out payout));
                    starty += 30 + dispy;
                    ShowSymbol(45, starty, ScatterSymbol);
                    AddLabel(75 + dispx + dispx / 2, starty+2, headingcolor2, @"Any");
                    AddLabel(105 + dispx*2 + dispx / 2, starty+2, headingcolor2, @"Any");
                    ShowPayout(154 + dispx * 3, starty, paycolor, m_Slot.GetJackpotPayoutStr(9, out payout));
                }
				starty += 35 + dispy;
			}
			if (bonusround)
			{
				int BonusSymbol = m_Symbols[m_Symbols[8]];
				AddLabel(45, starty, headingcolor1, @"Bonus Round"); //426
				starty += 25;
				AddImageTiled(45, starty-4, 101+dispx*3, 35+dispy/2, tilecolor);
				ShowSymbol(45, starty, BonusSymbol);
				ShowSymbol(75+dispx, starty, BonusSymbol);
				ShowSymbol(105+dispx*2, starty, BonusSymbol);
			}
		}

        private void ShowSymbol(int x, int y, int symbol)
        {
            string text = null;
            switch (symbol)
            {
                case 0:
                    break;

                case 90001:
                    text = String.Format("<BASEFONT SIZE=6 COLOR=RED><CENTER><B>7</B></CENTER></BASEFONT>");
                    AddHtml(x, y, 40, 50, @text, (bool)false, (bool)false);
                    break;

                case 90002:
                    text = String.Format("<BASEFONT SIZE=6 COLOR=BLACK><CENTER><B>7</B></CENTER></BASEFONT>");
                    AddHtml(x, y, 40, 50, @text, (bool)false, (bool)false);
                    break;

                case 90003:
                    text = String.Format("<BASEFONT SIZE=6 COLOR=WHITE><CENTER><B>7</B></CENTER></BASEFONT>");
                    AddHtml(x, y, 40, 50, @text, (bool)false, (bool)false);
                    break;

                case 90004:
                    text = String.Format("<BASEFONT SIZE=6 COLOR=BLUE><CENTER><B>7</B></CENTER></BASEFONT>");
                    AddHtml(x, y, 40, 50, @text, (bool)false, (bool)false);
                    break;

                case 90100: //White "Lucky" Necklace & Golden Robe
                    AddItem(x, y, 7940, 2213);
                    AddItem(x - 8, y + 3, 4232, 1150);
                    break;

                case 90101: //Golden Cloak and Sash
                    AddItem(x, y, 5424, 2213);
                    break;

                case 90102: //Golden Sash
                    AddItem(x, y + 5, 5441, 2213);
                    break;

                case 90103: //Golden Bars
                    AddItem(x, y, 7153, 2213);
                    break;

                case 90104: //Golden Horseshoes
                    AddItem(x, y, 4022, 2213);
                    break;

                case 90200: // Barbed Kit
                    AddItem(x, y, 3997, 2128);
                    break;

                case 90201: // Horned Kit
                    AddItem(x, y, 3997, 2116);
                    break;

                case 90202: // Spined Kit
                    AddItem(x, y, 3997, 2219);
                    break;

                case 90203: // Purple Fancy Shirt
                    AddItem(x, y, 7933, 23);
                    break;

                case 90204: // Colored Folded Cloth
                    AddItem(x, y, 5984, 1151);
                    break;

                case 90250: // Sea Horse
                    AddItem(x, y, 8479, 694);
                    break;

                case 90300: // Stat Scroll
                    AddItem(x, y+5, 5359, 1153);
                    break;

                case 90301: // TrueHarrower
                    AddItem(x, y, 9736, 1175);
                    break;

                case 90302: // Red Corpser
                    AddItem(x, y, 8402, 36);
                    break;

                case 90303: // Fake Harrower
                    AddItem(x, y-15, 9659);
                    break;

                case 90304: // OrnateCrown
                    AddItem(x, y, 5201, 1269);
                    break;

                case 90305: // Summon Altars
                    AddItem(x, y, 6587, 1109);
                    break;

                case 90306: // Red Skull Candle
                    AddItem(x, y, 6228, 37);
                    break;

                default:
                    AddItem(x, y, symbol);
                    break;
            }
        }
		private void ShowPayout ( int posx, int posy, int color, string[] paystring )
		{
			if (paystring[1] != null)
				posy -= 5;
			for (int i = 0; i < paystring.Length ; i++)
			{
				if (paystring[i] != null)
				{	
					AddLabel(posx, posy, color, paystring[i]);
					posy += 17;
				}
			}
		}
	}
}