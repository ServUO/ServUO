//#define TestMode
using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;

namespace Server.Gumps
{
	public class NewMinerBonusGump : Gump
	{
		private TurboSlot m_Slot;
		private int[] m_Symbols;
        private int m_TotalWon;

        private int[] buttonx = new int[] { 86, 156, 140, 205, 242, 359, 158, 245, 399 };
        private int[] buttony = new int[] { 198, 158, 236, 85, 142, 98, 309, 107, 363 };
        private bool[] m_b;
        private int[] m_c;

		public NewMinerBonusGump( TurboSlot Slot, int[] Symbols, bool initialize, bool[] b, int[] c, int totalwon ) : base( 25, 25 )
		{
			m_Slot = Slot;
			m_Symbols = Symbols;
            m_b = b;
            m_c = c;
            m_TotalWon = totalwon;
            if (initialize)
            {
                double zerochance = .40;
                bool fiftyk = false;
                for (int i = 0; i < 9; i++)
                {
                    b[i] = false;
                    if (fiftyk || ( i == 3 && 0.10 < Utility.RandomDouble())) // Only allow one fifty K spot and worsen the odds and rarely on the top button! :P
                    {
                        if (zerochance < Utility.RandomDouble())
                            c[i] = Utility.RandomList(5000, 5000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500, 500, 500, 500, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50);
                        else
                            c[i] = 0;
                        if (i == 1) c[i] = 111;
                    }
                    else
                    {
                        if (zerochance < Utility.RandomDouble())
                            c[i] = Utility.RandomList(50000, 10000, 5000, 5000, 1000, 1000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500, 500, 500, 100, 100, 100, 100, 100, 100, 100, 100, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50);
                        else
                            c[i] = 0;
                        if (c[i] == 50000)
                        {
                            fiftyk = true;
                            zerochance += .28;
                        }
                    }
                    if (c[i] != 0)
                            zerochance += .05;
                }
            }
            Disposable = true;
            Dragable = true;
            Closable = true;
            for (int i = 0; i < 9; i++)
            {
                if (!b[i])
                {
                    Disposable = false;
                    Dragable = true;
                    Closable = false;
                    break;
                }
            }
			Resizable=false;
			AddPage(0);
			AddBackground(52, 25, 393, 430, 5120);
			AddImage(59, 63, 5528);
			AddImage(60, 30, 5573);
			AddLabel(123, 26, 1149, @"Pick a location to mine, If you mine up coal you lose.");
			AddLabel(123, 42, 1149, @"You keep mining till you find coal or all are gone.");

            for (int i = 0; i < 9; i++)
            {
                if (!b[i])
                {
                    AddButton(buttonx[i], buttony[i], 2117, 2118, i + 1, GumpButtonType.Reply, 0);
#if TestMode 
                    if (m_Slot.TestMode)
                        AddLabel(buttonx[i], buttony[i]+ 10, 1160, c[i].ToString());
#endif
                }
                else
                {
                    AddImage(buttonx[i], buttony[i], 5231);
                    if (c[i] != 0)
                        AddLabel(buttonx[i], buttony[i], 1160, c[i].ToString());
                    else
                        AddLabel(buttonx[i], buttony[i], 1160, @"!! Coal !!");
                }
            }
		}

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (from == null)
                return;
            if (m_Slot.InUseBy == null)
            {
                from.SendMessage("Someone else played this machine while you were idle. Double click it again to resume play.");
                return;
            }
            if (from.Serial != m_Slot.InUseBy.Serial)
            {
                from.SendMessage("You have left this machine idle too long and {0} has taken it over!", m_Slot.InUseBy.Name);
                return;
            }
            if (info.ButtonID < 0 || info.ButtonID > 10)
            {
                from.SendMessage("Invalid button detected, using Razor maybe? Bonus round over.");
                from.CloseGump(typeof(TurboSlotGump));
                from.SendGump(new TurboSlotGump(m_Slot, m_Symbols));
                return;
            }
            m_Slot.LastPlayed = DateTime.Now;
            if (info.ButtonID == 0)
            {
                from.CloseGump(typeof(TurboSlotGump));
                from.SendGump(new TurboSlotGump(m_Slot, m_Symbols));
                return;
            }
            int buttonindex = info.ButtonID - 1;
            if (m_c[buttonindex] == 0)
            {
                from.SendMessage("You found coal!");
                from.SendMessage("You may now close this menu out to continue playing.");
                from.SendGump(new NewMinerBonusGump(m_Slot, m_Symbols, false, new bool[] { true, true, true, true, true, true, true, true, true }, m_c, m_TotalWon));
                if (m_TotalWon == 0)
                    m_Slot.PublicOverheadMessage(0, m_Slot.Hue, false, "Bonus Round Over");
                else
                {
                    string text = String.Format("Bonus Round: {0} gold!", m_TotalWon);
                    m_Slot.PublicOverheadMessage(0, m_Slot.Hue, false, text);
                }
            }
            else
            {
                if (m_b[buttonindex])
                {
                    m_Slot.Active = false;
                    m_Slot.SlotWon = 0;
                    m_Slot.ErrorCode = 9001; 
                    from.SendMessage("An Invalid choice has been detected and a Casino Boss has been notified.");
                    from.SendMessage("Trying to cheat? Expect a visit shortly! No gold for you!");
                    from.SendMessage("This slot machine is offline for maintenance.");
                    return;
                }
                m_Slot.SlotWon += m_c[buttonindex];
                m_Slot.LastPay += m_c[buttonindex];
                m_TotalWon += m_c[buttonindex];
                from.SendMessage("You have won {0}, pick another spot to mine.", m_c[buttonindex]);
                m_b[buttonindex] = true;
                from.SendGump(new NewMinerBonusGump(m_Slot, m_Symbols, false, m_b, m_c, m_TotalWon));
            }

        }
	}
}