using System;
using Server.Items;
using Server.Network;

#if XMLSPAWNER
using System.Xml;
#endif

namespace Server.Gumps
{
    public class KenoGump : Gump
    {
        private readonly KenoBoard m_Keno;
        private readonly KenoBoard.PlayerInfo m_Player;
        private readonly byte[] m_Selected;
        private float[] m_PayOutTable;
        private byte m_TotalSelected;
        private byte[] m_MachinePicks;
        private bool m_PickButtons;
        public enum PayMethod { Standard, QuickPick, BottomHalf, TopHalf, RightHalf, LeftHalf, Odd, Even, Edges, Kool20, EZBucks, BancoSpecial, Millionare10 }
        private PayMethod m_PayTable;
        private PayMethod m_CPayMethod;
        private int m_MinPicks = 1;
        private int m_MaxPicks = 15;
        private byte m_QuickPickCount;
        private readonly Mobile m_From;
        private InternalTimer m_GumpTimer;

        public KenoGump(KenoBoard Keno, Mobile From, KenoBoard.PlayerInfo player, bool played, int PayTable, float[] PayOutTable, int CPayMethod, byte QuickPickCount, bool PickButtons, byte[] Selected, byte TotalSelected, byte[] MachinePicks)
            : base(400, 263)
        {
            m_Keno = Keno;
            m_Selected = Selected;
            m_TotalSelected = TotalSelected;
            m_MachinePicks = MachinePicks;
            m_PickButtons = PickButtons;
            m_PayTable = (PayMethod)PayTable;
            m_QuickPickCount = QuickPickCount;
            m_CPayMethod = (PayMethod)CPayMethod;
            m_PayOutTable = PayOutTable;
            m_Player = player;
            m_From = From;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            AddPage(0);
            AddBackground(0, 0, 650, 340, 9250);
            string text = String.Format("Cost: {0}", player.Cost);
            AddLabel(380, 15, 2213, text);
            AddButton(440, 15, 0x983, 0x984, 101, GumpButtonType.Reply, 0);
            AddButton(440, 25, 0x985, 0x986, 102, GumpButtonType.Reply, 0);
            AddLabel(227 - ((m_Keno.Name.Length/2) * 7), 10, (m_Keno.Hue == 907 ? 0 : m_Keno.Hue - 1), m_Keno.Name);
            AddLabel(475, 25, 1149, "Number Selection Method");
            AddLabel(485, 50, 1149, "Special:");
            AddLabel(485, 75, 1149, "Standard");
            AddLabel(485, 100, 1149, "Quick Pick");
            AddLabel(485, 166, 1149, "Total Matched:");
            AddLabel(460, 210, 1149, "Credits:");
            AddLabel(460, 230, 1149, "Last Pay:");
            if (player.mobile.AccessLevel >= AccessLevel.GameMaster)
            {
                int paybackhue = 66;
                if (m_Keno.WinningPercentage > 95.0)
                {
                    paybackhue = 37;
                }
                AddLabel(475, 185, 1152, "Payout Percentage:");
                text = String.Format("{0:##0.00%}", m_Keno.WinningPercentage/100);
                AddLabel(590, 185, paybackhue, text);
            }
            AddImageTiled(15, 175, 455, 4, 2624);
            AddImageTiled(450, 30, 3, 290, 2624);
            int numberx = 20;
            int numbery = 35;
            int bottomhalf = 0;
            int totalhit = 0;
            int tophalf = 0;
            AddButton(460, 50, ((m_PayTable >= PayMethod.BottomHalf && m_PayTable <= PayMethod.Millionare10) ? 0xD3 : 0xD2), ((m_PayTable >= PayMethod.BottomHalf && m_PayTable <= PayMethod.Edges) ? 0xD2 : 0xD3), 4000, GumpButtonType.Reply, 0);
            AddButton(460, 75, (m_PayTable == PayMethod.Standard ? 0xD3 : 0xD2), (m_PayTable == PayMethod.Standard ? 0xD2 : 0xD3), 4001, GumpButtonType.Reply, 0);
            AddButton(460, 100, (m_PayTable == PayMethod.QuickPick ? 0xD3 : 0xD2), (m_PayTable == PayMethod.QuickPick ? 0xD2 : 0xD3), 4002, GumpButtonType.Reply, 0);
            AddButton(460, 125, 4026, 4027, 520, GumpButtonType.Reply, 0);
            AddLabel(495, 125, 1149, @"View Pay Table");

            AddButton(460, 265, 4020, 4021, 200, GumpButtonType.Reply, 0);
            /*if (m_Keno.FreeSpin)
                AddLabel(65, 307, 1149, @"Free");
            else */
                AddLabel(495, 265, 1149, @"Play");
            AddButton(540, 265, 4029, 4030, 201, GumpButtonType.Reply, 0); //CASHOUT
            AddLabel(575, 265, 1149, @"Cash Out");
            AddButton(457, 290, 4037, 4036, 202, GumpButtonType.Reply, 0); //ATM
            AddLabel(495, 299, 1149, @"ATM");

            if (player.mobile.AccessLevel >= AccessLevel.GameMaster) 
            {
                AddButton(540, 295, 0xFAB, 0xFAD, 999, GumpButtonType.Reply, 0); //Props
                AddLabel(575, 295, 0x384, @"Props");
            }
            if (m_PayTable == PayMethod.Standard)
            {           
                text = String.Format("Min: {0}", m_MinPicks);
                AddLabel(585, 68, 1149, text);
                text = String.Format("Max: {0}", m_MaxPicks);
                AddLabel(585, 83, 1149, text);
                text = String.Format(": {0}", m_TotalSelected);
                AddLabel(537, 75, 1149, text);
                DisplaySpecial(m_CPayMethod);  
            }
            else if (m_PayTable == PayMethod.QuickPick)
            {
                AddButton(565, 100, 0x983, 0x984, 501, GumpButtonType.Reply, 0);
                AddButton(565, 110, 0x985, 0x986, 502, GumpButtonType.Reply, 0);
                AddLabel(550, 100, 1149, m_QuickPickCount.ToString());
                DisplaySpecial(m_CPayMethod);  
            }
            else if (m_PayTable >= PayMethod.BottomHalf && m_PayTable <= PayMethod.Millionare10)
            {
                m_PayTable = m_CPayMethod;
                DisplaySpecial(m_PayTable);                
                AddButton(610, 50, 0x983, 0x984, 601, GumpButtonType.Reply, 0);
                AddButton(610, 60, 0x985, 0x986, 602, GumpButtonType.Reply, 0);
            }
            for (int i = 0; i < 20; i++)
                if (m_MachinePicks[i] != 255)
                    m_Selected[m_MachinePicks[i]] += 2;
            int numberHue = 0;
            for (int i = 0; i < 80; i++)
            {
                switch (m_Selected[i])
                {
                    case 0:
                        numberHue = 0;
                        break;
                    case 1:
                        numberHue = 1149;
                        break;
                    case 2:
                        numberHue = 37;
                        m_Selected[i] = 0;
                        if (i < 40)
                            tophalf++;
                        else
                            bottomhalf++;
                        break;
                    case 3:
                        numberHue = 66;
                        totalhit++;
                        m_Selected[i] = 1;
                        if (i < 40)
                            tophalf++;
                        else
                            bottomhalf++;
                        break;
                }
                if (m_PickButtons)
                    AddButton(numberx - 3, numbery - 5, 4202, 4202, i + 1, GumpButtonType.Reply, 0);
                AddLabel(numberx, numbery, numberHue, (i + 1).ToString());
                numberx += 45;
                if (( i + 1) % 10 == 0)
                {
                    numbery += 37;
                    numberx = 20;
                }
            }
            for (int i = 0; i < 20; i++)
                    m_MachinePicks[i] = 255;
            AddLabel(455, 155, 957, tophalf.ToString());
            AddLabel(455, 180, 957, bottomhalf.ToString());
            text = String.Format("{0}/{1}", totalhit,m_TotalSelected);
            AddLabel(575, 166, 66, text);

            if (played)
            {
                int wonhue = 957;
                int won = (int)(m_Player.Cost * m_PayOutTable[totalhit]);
                if (won == 0)
                {
                    if (Utility.RandomDouble() < .01 && m_From != null && !m_From.Deleted)
                    {
                        if (m_From.Female)
                            m_From.PlaySound(Utility.RandomList(1372, 1373, 816, 796, 782));
                        else
                            m_From.PlaySound(Utility.RandomList(1372, 1373, 1090, 1068, 1053));
                    }
                    if ((int)m_Keno.SecurityChatter == 2)
                    {
                        text = String.Format("{0} lost {1}.", m_Player.mobile.Name, m_Player.Cost);
                        m_Keno.SecurityCamera(2, text);
                        text = String.Format("OnCredit={1}.", m_Player.mobile.Name, m_Player.OnCredit);
                        m_Keno.SecurityCamera(2, text);
                    }
                }
                else
                {
                    m_Keno.ProcessPlay(m_From, m_Player, won);
                    wonhue = 2213;
                }
                AddLabel(525, 230, wonhue, won.ToString());
                m_Keno.IncrementStats(totalhit);
                if (Utility.RandomDouble() < .0008 && m_From != null && !m_From.Deleted)
                    CEOCookie(m_Keno.Hue);
            }
            else
                AddLabel(525, 230, 957, "0");
            AddLabel(525, 210, 2213, m_Keno.OnCredit(m_From, m_Player, 0).ToString());
        }

        private void DisplaySpecial(PayMethod m_PayTable)
        {
            switch (m_PayTable)
            {
                case PayMethod.BottomHalf:
                    AddLabel(530, 50, 1149, "Bottom");
                    break;
                case PayMethod.TopHalf:
                    AddLabel(530, 50, 1149, "Top");
                    break;
                case PayMethod.RightHalf:
                    AddLabel(530, 50, 1149, "Right");
                    break;
                case PayMethod.LeftHalf:
                    AddLabel(530, 50, 1149, "Left");
                    break;
                case PayMethod.Odd:
                    AddLabel(530, 50, 1149, "Odd");
                    break;
                case PayMethod.Even:
                    AddLabel(530, 50, 1149, "Even");
                    break;
                case PayMethod.Edges:
                    AddLabel(530, 50, 1149, "Edges");
                    break;
                case PayMethod.Kool20:
                    AddLabel(530, 50, 1149, "Kool 20");
                    break;
                case PayMethod.EZBucks:
                    AddLabel(530, 50, 1149, "EZ Bucks");
                    break;
                case PayMethod.BancoSpecial:
                    AddLabel(530, 50, 1149, "Banco Special");
                    break;
                case PayMethod.Millionare10:
                    AddLabel(530, 50, 1149, "Millionare 10");
                    break;
            }
        }

        private void SpecialPick(PayMethod m_PayTable)
        {
            m_TotalSelected = 40;
            switch (m_PayTable)
            {
                case PayMethod.BottomHalf:
                    for (int i = 40; i < 80; i++)
                        m_Selected[i] = 1;
                    m_PickButtons = false;
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.BottomHalf, m_Player.Cost, 40);
                    break;
                case PayMethod.TopHalf:
                    for (int i = 0; i < 40; i++)
                        m_Selected[i] = 1;
                    m_PickButtons = false;
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.TopHalf, m_Player.Cost, 40);
                    break;
                case PayMethod.RightHalf:
                    for (int i = 0; i < 8; i++)
                        for (int h = 0; h < 5; h++)
                            m_Selected[i * 10 + 5 + h] = 1;
                    m_PickButtons = false;
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.RightHalf, m_Player.Cost, 40);
                    break;
                case PayMethod.LeftHalf:
                    for (int i = 0; i < 8; i++)
                        for (int h = 0; h < 5; h++)
                            m_Selected[i * 10 + h] = 1;
                    m_PickButtons = false;
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.LeftHalf, m_Player.Cost, 40);
                    break;
                case PayMethod.Odd:
                    for (int i = 0; i < 80; i += 2)
                        m_Selected[i] = 1;
                    m_PickButtons = false;
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.Odd, m_Player.Cost, 40);
                    break;
                case PayMethod.Even:
                    for (int i = 1; i < 80; i += 2)
                        m_Selected[i] = 1;
                    m_PickButtons = false;
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.Even, m_Player.Cost, 40);
                    break;
                case PayMethod.Edges:
                    for (int i = 0; i < 10; i++)
                        m_Selected[i] = 1;
                    for (int i = 70; i < 80; i++)
                        m_Selected[i] = 1;
                    for (int i = 19; i < 70; i += 10)
                        m_Selected[i] = 1;
                    for (int i = 10; i < 61; i += 10)
                        m_Selected[i] = 1;
                    m_PayTable = PayMethod.Edges;
                    m_PickButtons = false;
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.Edges, m_Player.Cost, 32);
                    m_TotalSelected = 32;
                    break;
                case PayMethod.Kool20:
                    byte[] numbers = m_Keno.PickNumbers(20);
                    ClearBoard();
                    for (int i = 0; i < 20; i++)
                        m_Selected[numbers[i]] = 1;
                    m_PickButtons = false;
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.Kool20, m_Player.Cost, 20);
                    m_TotalSelected = 20;
                    break;
                case PayMethod.EZBucks:
                    numbers = m_Keno.PickNumbers(20);
                    ClearBoard();
                    for (int i = 0; i < 20; i++)
                        m_Selected[numbers[i]] = 1;
                    m_PickButtons = false;
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.EZBucks, m_Player.Cost, 20);
                    m_TotalSelected = 20;
                    break;
                case PayMethod.BancoSpecial:
                    numbers = m_Keno.PickNumbers(20);
                    ClearBoard();
                    for (int i = 0; i < 20; i++)
                        m_Selected[numbers[i]] = 1;
                    m_PickButtons = false;
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.BancoSpecial, m_Player.Cost, 20);
                    m_TotalSelected = 20;
                    break;
                case PayMethod.Millionare10:
                    numbers = m_Keno.PickNumbers(10);
                    ClearBoard();
                    for (int i = 0; i < 10; i++)
                        m_Selected[numbers[i]] = 1;
                    m_PickButtons = false;
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.Millionare10, m_Player.Cost, 10);
                    m_TotalSelected = 10;
                    break;
            }
        }

        private void CEOCookie(int hue)
        {
            AddImageTiled(155, 50, 142, 230, 990);
            AddLabel(70, 275, hue, "Keno Boss, CEO, says \"Hello! Enjoying my Keno?\" :)");
            m_From.PlaySound(Utility.RandomList(1358, 1359, 1360, 1361, 1362, 1363, 1368, 1382));
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            bool played = false;
            if (from == null || m_Keno == null )
                return;
            if (from != m_Player.mobile)
            {
                from.SendMessage("{0} does not recognize you as a valid player!", m_Keno.Name);
                m_Keno.RemovePlayer(from, m_Player);
                from.CloseGump(typeof(KenoPayTableGump));
                return;
            }
            if (m_Keno.Deleted || !m_Keno.Active)
            {
                m_Keno.RemovePlayer(from, m_Player);
                from.SendMessage("{0} is offline.", m_Keno.Name);
                from.CloseGump(typeof(KenoPayTableGump));
                return;
            }
            if (!from.Alive)
            {
                from.SendMessage("Ghosts can not play this game.");
                m_Keno.RemovePlayer(from, m_Player);
                from.CloseGump(typeof(KenoPayTableGump));
                return;
            }
            if (!from.InRange(m_Keno.GetWorldLocation(), 9) || !from.InLOS(m_Keno))
            {
                from.SendMessage("You are too far away from {0} and quit playing.", m_Keno.Name);
                m_Keno.RemovePlayer(from, m_Player);
                from.CloseGump(typeof(KenoPayTableGump));
                return;
            }
            if (from.Hidden && from.AccessLevel == AccessLevel.Player) // Don't let someone sit at the KenoBoard and play hidden
            {
                from.Hidden = false;
                from.SendMessage("Playing {0} reveals you!", m_Keno.Name);
            }
            m_Player.LastPlayed = DateTime.Now;
            if (info.ButtonID == 999 && (from.AccessLevel >= AccessLevel.GameMaster))
            {
#if XMLSPAWNER
                from.SendGump(new XmlPropertiesGump(from, m_Keno));
#else
                from.SendGump( new PropertiesGump( from, m_Keno ) );
#endif
            }
            if (info.ButtonID > 0 && info.ButtonID < 81 && m_PayTable == PayMethod.Standard)
            {
                if (m_PickButtons)
                {
                    if (m_Selected[info.ButtonID - 1] == 0)
                    {
                        if (m_TotalSelected < 15)
                        {
                            m_TotalSelected++;
                            m_Selected[info.ButtonID - 1] = 1;
                        }
                        else
                        {
                            // too many numbers
                            from.SendMessage("The maximum numbers you can select is 15.");
                        }
                    }
                    else
                    {
                        if (m_TotalSelected > 0)
                        {
                            m_TotalSelected--;
                            m_Selected[info.ButtonID - 1] = 0; ;
                        }
                        else
                        {
                            // How did we get below zero?
                            from.SendMessage("Invalid button detected.");
                            string text = String.Format("{0}:Invalid button detected.", from.Name);
                            m_Keno.SecurityCamera(0, text);
                        }
                    }
                    m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.Standard, m_Player.Cost, m_TotalSelected);
                }
                else
                {
                    //packet fudging/cheating??
                    from.SendMessage("Invalid button detected.");
                    string text = String.Format("{0}:Invalid button detected.", from.Name);
                    m_Keno.SecurityCamera(0, text);
                }
            }
            else if (info.ButtonID == 101) // && m_TotalSelected != 0)  // Inc Cost
            {
                m_Player.IncCost();
                m_PayOutTable = m_Keno.GetPayTable((int)m_PayTable, m_Player.Cost, m_TotalSelected);
            }
            else if (info.ButtonID == 102) // && m_TotalSelected != 0)  // Dec Cost
            {
                m_Player.DecCost();
                m_PayOutTable = m_Keno.GetPayTable((int)m_PayTable, m_Player.Cost, m_TotalSelected);
            }
            else if (info.ButtonID == 200) // Play
            {
                if (m_TotalSelected == 0)
                {
                    from.SendMessage("You need to select some numbers first to play!");
                }
                else if (m_Keno.Profile)
                {
                    m_Player.OnCredit = 0;
                    played = PlayKeno(from, m_Player.Cost);
                }
                else
                {
                    int amount = 0;
                    if (!from.InRange(m_Keno.GetWorldLocation(), 10) || !from.InLOS(m_Keno))
                    {
                        from.SendMessage("You are too far away from the Keno Board.");
                        m_Keno.RemovePlayer(from, m_Player);
                        return;
                    }
                    if (from.Backpack.ConsumeTotal(typeof(CasinoToken), 1))
                    {
                        played = PlayKeno(from, 0);
                    }
                    else if (from.Backpack.ConsumeTotal(typeof(Gold), m_Player.Cost))
                    {
                        played = PlayKeno(from, 0);
                    }
                    else if (m_Keno.OnCredit(from, m_Player, 0) >= m_Player.Cost)
                    {
                        played = PlayKeno(from, m_Player.Cost);
                    }
                    else if (m_Keno.CashCheck(from, m_Player, out amount))
                    {
                        from.SendMessage("Cashing bank check for {0} gold from your backpack, you may now spin again.", amount);
                    }
                    else
                        from.SendMessage("You must have at least {0} gold, or credits on the machine to play.", m_Player.Cost);
                }
            }
            else if (info.ButtonID == 201 ||info.ButtonID == 0)  // Cash Out
            {
                m_Keno.RemovePlayer(from, m_Player);
                from.SendMessage("You quit playing {0}.", m_Keno.Name);
                from.CloseGump(typeof(KenoPayTableGump));
                return;
            }
            else if (info.ButtonID == 202)  // ATM
            {
                if (m_Keno.OnCredit(from, m_Player, 0) >= m_Keno.CreditATMLimit)
                {
                    from.SendMessage("This machine is at or over its credit limit.");
                }
                else
                {
                    int amount = (m_Keno.CreditATMLimit - m_Keno.OnCredit(from, m_Player, 0) < m_Keno.CreditATMIncrements) ? m_Keno.CreditATMLimit - m_Keno.OnCredit(from, m_Player, 0) : m_Keno.CreditATMIncrements;
                    if (from.BankBox.ConsumeTotal(typeof(Gold), amount))
                    {
                        m_Keno.OnCredit(from, m_Player, amount);
                        from.SendMessage("{0} gold has been withdrawn from your bank and added to this machine's credit counter.", amount);
                        Effects.PlaySound(new Point3D(m_Keno.X, m_Keno.Y, m_Keno.Z), m_Keno.Map, 501);
                        string text = String.Format("{0}:ATM={1}.", m_Player.mobile.Name, amount);
                        m_Keno.SecurityCamera(amount > 5000 ? 0 : 1, text);
                        text = String.Format("OnCredit={1}.", m_Player.mobile.Name, m_Player.OnCredit);
                        m_Keno.SecurityCamera(m_Player.OnCredit > 10000 ? 1 : 2, text);
                    }
                    else
                        from.SendMessage("Insufficient funds for ATM withdrawal.");
                }
            }
            else if (info.ButtonID == 520)  // Pay table gump
            {
                if (m_TotalSelected == 0)
                {
                    from.SendMessage("You need to select some numbers to see a valid Pay Table.");
                    from.CloseGump(typeof(KenoPayTableGump));
                }
                else
                {
                    from.CloseGump(typeof(KenoPayTableGump));
                    from.SendGump(new KenoPayTableGump((int)m_PayTable, m_PayOutTable, m_Player.Cost, m_TotalSelected));
                }
            }
            else if (m_PayTable == PayMethod.QuickPick && info.ButtonID > 500 && info.ButtonID < 503)
            {
                if (info.ButtonID == 501)
                {
                    m_QuickPickCount++;
                    if (m_QuickPickCount > 15)
                        m_QuickPickCount = 1;
                }
                else
                {
                    m_QuickPickCount--;
                    if (m_QuickPickCount < 1)
                        m_QuickPickCount = 15;
                }
                byte[] numbers = m_Keno.PickNumbers(m_QuickPickCount);
                ClearBoard();
                for (int i = 0; i < m_QuickPickCount; i++)
                    m_Selected[numbers[i]] = 1;
                m_PickButtons = false;
                m_TotalSelected = m_QuickPickCount;
                m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.QuickPick, m_Player.Cost, m_TotalSelected);
            }
            else if (m_PayTable >= PayMethod.BottomHalf && m_PayTable <= PayMethod.Millionare10 && info.ButtonID > 600 && info.ButtonID < 603)
            {
                if (info.ButtonID == 601)
                {
                    m_PayTable++;
                    if (m_PayTable > PayMethod.Millionare10)
                        m_PayTable = PayMethod.BottomHalf;
                }
                else
                {
                    m_PayTable--;
                    if (m_PayTable < PayMethod.BottomHalf)
                        m_PayTable = PayMethod.Millionare10;
                }
                ClearBoard();
                DisplaySpecial(m_PayTable);
                SpecialPick(m_PayTable);
                m_CPayMethod = m_PayTable;
            }
            else if (info.ButtonID > 3999 && info.ButtonID < 4005)
            {
                switch (info.ButtonID)
                {
                    case 4000:                              // Special
                        m_PayTable = m_CPayMethod;
                        ClearBoard();
                        SpecialPick(m_PayTable);
                        break;
                    case 4001:                              // Manual
                        ClearBoard();
                        m_TotalSelected = 0;
                        m_PayTable = PayMethod.Standard;
                        m_PickButtons = true;
                        break;
                    default:                                // Quick Pick
                        byte[] numbers = m_Keno.PickNumbers(m_QuickPickCount);
                        ClearBoard();
                        for (int i = 0; i < m_QuickPickCount; i++)
                            m_Selected[numbers[i]] = 1;
                        m_PayTable = PayMethod.QuickPick;
                        m_PickButtons = false;
                        m_TotalSelected = m_QuickPickCount;
                        m_PayOutTable = m_Keno.GetPayTable((int)PayMethod.QuickPick, m_Player.Cost, m_TotalSelected);
                        break;
                }
            }
            if (m_Keno.Throttle && played)
                ActivateGumpTimer(m_Keno, m_From, m_Player, played, (int)m_PayTable, m_PayOutTable, (int)m_CPayMethod, m_QuickPickCount, m_PickButtons, m_Selected, m_TotalSelected, m_MachinePicks);
            else
                from.SendGump(new KenoGump(m_Keno, m_From, m_Player, played, (int)m_PayTable, m_PayOutTable, (int)m_CPayMethod, m_QuickPickCount, m_PickButtons, m_Selected, m_TotalSelected, m_MachinePicks));    
        }

        private bool PlayKeno(Mobile from, int cost)
        {
            if (m_Keno.Profile)
            {
                m_Keno.ProfileTable(m_Keno, m_Player, true, (int)m_PayTable, m_PayOutTable, (int)m_CPayMethod, m_QuickPickCount, m_PickButtons, m_Selected, m_TotalSelected, m_MachinePicks);
                return true;
            }
            m_MachinePicks = m_Keno.PickNumbers(20);
            m_Keno.OnCredit(from, m_Player, -cost);
            if (m_TotalSelected != 0)
                return true;
            return false;
        }

        private void ClearBoard()
        {
            for (int i = 0; i < 80; i++)
                m_Selected[i] = 0;
            for (int i = 0; i < 20; i++)
                m_MachinePicks[i] = 255;

        }

        private void ActivateGumpTimer(KenoBoard Keno, Mobile From, KenoBoard.PlayerInfo player, bool played, int PayTable, float[] PayOutTable, int CPayMethod, byte QuickPickCount, bool PickButtons, byte[] Selected, byte TotalSelected, byte[] MachinePicks)
        {
            if (m_GumpTimer != null)
                m_GumpTimer.Stop();
            m_GumpTimer = new InternalTimer(Keno, From, player, played, PayTable, PayOutTable, CPayMethod, QuickPickCount, PickButtons, Selected, TotalSelected, MachinePicks, TimeSpan.FromSeconds(Keno.ThrottleSeconds));//TimeSpan.FromTicks(Keno.ThrottleTicks)); 
            m_GumpTimer.Start();
        }

        private class InternalTimer : Timer
        {
            private readonly KenoBoard m_Keno;
            private readonly Mobile m_From;
            private readonly KenoBoard.PlayerInfo m_player;
            private readonly bool m_played;
            private readonly int m_PayTable;
            private readonly float[] m_PayOutTable;
            private readonly int m_CPayMethod;
            private readonly byte m_QuickPickCount;
            private readonly bool m_PickButtons;
            private readonly byte[] m_Selected;
            private readonly byte m_TotalSelected;
            private readonly byte[] m_MachinePicks;

            public InternalTimer(KenoBoard Keno, Mobile From, KenoBoard.PlayerInfo player, bool played, int PayTable, float[] PayOutTable, int CPayMethod, byte QuickPickCount, bool PickButtons, byte[] Selected, byte TotalSelected, byte[] MachinePicks, TimeSpan delay)
                : base(delay)
            {
                //Priority = TimerPriority.OneSecond;
                Priority = TimerPriority.TwoFiftyMS;
                m_Keno = Keno;
                m_From = From;
                m_player = player;
                m_played = played;
                m_PayTable = PayTable;
                m_PayOutTable = PayOutTable;
                m_CPayMethod = CPayMethod;
                m_QuickPickCount = QuickPickCount;
                m_PickButtons = PickButtons;
                m_Selected = Selected;
                m_TotalSelected = TotalSelected;
                m_MachinePicks = MachinePicks;
            }

            protected override void OnTick()
            {
                if (m_Keno != null && !m_Keno.Deleted)
                    m_From.SendGump(new KenoGump(m_Keno, m_From, m_player, m_played, m_PayTable, m_PayOutTable, m_CPayMethod, m_QuickPickCount, m_PickButtons, m_Selected, m_TotalSelected, m_MachinePicks));
            }
        }
    }
}
