using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
    public class CommodityBrokerGump : BaseBazaarGump
    {
        private readonly CommodityBroker m_Broker;

        public CommodityBrokerGump(CommodityBroker broker, Mobile from)
            : base(520, 520)
        {
            m_Broker = broker;

            AddHtmlLocalized(10, 10, 500, 18, 1114513, "#1150636", RedColor16, false, false);  // Commodity Broker

            if (m_Broker.Plot.ShopName != null && m_Broker.Plot.ShopName.Length > 0)
            {
                AddHtml(10, 37, 500, 18, Color(FormatStallName(m_Broker.Plot.ShopName), BlueColor), false, false);
            }
            else
            {
                AddHtmlLocalized(10, 37, 500, 18, 1114513, "#1150314", BlueColor16, false, false); // This Shop Has No Name
            }

            AddHtmlLocalized(10, 55, 260, 18, 1114514, "#1150313", BlueColor16, false, false); // Proprietor:
            AddHtml(280, 55, 260, 18, Color(string.Format("{0}", broker.Name), BlueColor), false, false);

            AddHtmlLocalized(10, 100, 500, 18, 1114513, "#1150328", GreenColor16, false, false); // OWNER MENU

            AddButton(150, 150, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(190, 150, 200, 18, 1150392, OrangeColor16, false, false); // INFORMATION

            AddHtmlLocalized(38, 180, 200, 18, 1150199, RedColor16, false, false); // Broker Account Balance
            AddHtml(190, 180, 300, 18, FormatAmt(broker.BankBalance), false, false);

            int balance = Banker.GetBalance(from);
            AddHtmlLocalized(68, 200, 200, 18, 1150149, GreenColor16, false, false); // Your Bank Balance:
            AddHtml(190, 200, 200, 18, FormatAmt(balance), false, false);

            AddHtmlLocalized(32, 230, 200, 18, 1150329, OrangeColor16, false, false); // Broker Sales Comission
            AddHtmlLocalized(190, 230, 100, 18, 1150330, OrangeColor16, false, false); // 5%

            AddHtmlLocalized(109, 250, 200, 18, 1150331, OrangeColor16, false, false); // Weekly Fee:
            AddHtml(190, 250, 100, 18, FormatAmt(broker.GetWeeklyFee()), false, false);

            AddHtmlLocalized(113, 280, 200, 18, 1150332, OrangeColor16, false, false); // Shop Name:
            AddBackground(190, 280, 285, 22, 9350);
            AddTextEntry(191, 280, 285, 20, LabelHueBlue, 0, m_Broker.Plot.ShopName == null ? "" : m_Broker.Plot.ShopName);
            AddButton(480, 280, 4014, 4016, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(85, 305, 150, 18, 1150195, OrangeColor16, false, false); // Withdraw Funds
            AddBackground(190, 305, 285, 22, 9350);
            AddTextEntry(191, 305, 285, 20, LabelHueBlue, 1, "");
            AddButton(480, 305, 4014, 4016, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(97, 330, 150, 18, 1150196, OrangeColor16, false, false); // Deposit Funds
            AddBackground(190, 330, 285, 22, 9350);
            AddTextEntry(191, 330, 285, 20, LabelHueBlue, 2, "");
            AddButton(480, 330, 4014, 4016, 4, GumpButtonType.Reply, 0);

            AddButton(150, 365, 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddHtmlLocalized(190, 365, 200, 18, 1150192, OrangeColor16, false, false); // ADD TO INVENTORY

            AddButton(150, 390, 4005, 4007, 6, GumpButtonType.Reply, 0);
            AddHtmlLocalized(190, 390, 300, 18, 1150193, OrangeColor16, false, false); // VIEW INVENTORY / REMOVE ITEMS

            AddButton(150, 415, 4005, 4007, 7, GumpButtonType.Reply, 0);
            AddHtmlLocalized(190, 415, 300, 18, 1150194, OrangeColor16, false, false); // SET PRICES AND LIMITS
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (m_Broker == null || m_Broker.Plot == null)
                return;

            switch (info.ButtonID)
            {
                default:
                case 0: return;
                case 1:
                    from.SendGump(new BazaarInformationGump(0, 1150637, new CommodityBrokerGump(m_Broker, from)));
                    return;
                case 2: // Set Shop Name
                    TextRelay tr = info.TextEntries[0];
                    string text = tr.Text;

                    if (!m_Broker.Plot.TrySetShopName(from, text))
                        from.SendLocalizedMessage(1150775); // Shop names are limited to 40 characters in length. Shop names must pass an obscenity filter check. The text you have entered is not valid.
                    break;
                case 3: // Withdraw Funds
                    TextRelay tr1 = info.TextEntries[1];
                    string text1 = tr1.Text;
                    int amount = 0;

                    try
                    {
                        amount = Convert.ToInt32(text1);
                    }
                    catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }

                    if (amount > 0)
                    {
                        m_Broker.TryWithdrawFunds(from, amount);
                    }
                    break;
                case 4: // Deposit Funds
                    TextRelay tr2 = info.TextEntries[2];
                    string text2 = tr2.Text;
                    int amount1 = 0;

                    try
                    {
                        amount1 = Convert.ToInt32(text2);
                    }
                    catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }

                    if (amount1 > 0)
                    {
                        m_Broker.TryDepositFunds(from, amount1);
                    }
                    break;
                case 5: // ADD TO INVENTORY
                    if (m_Broker.BankBalance < 0)
                    {
                        from.SendGump(new BazaarInformationGump(1150623, 1150615));
                        return;
                    }

                    from.Target = new InternalTarget(m_Broker);
                    from.SendGump(new CommodityTargetGump(m_Broker));
                    return;
                case 6: // VIEW INVENTORY / REMOVE ITEMS
                    if (m_Broker.BankBalance < 0)
                    {
                        from.SendGump(new BazaarInformationGump(1150623, 1150615));
                        return;
                    }
                    from.SendGump(new ViewInventoryGump(m_Broker));
                    return;
                case 7: // SET PRICES AND LIMITS
                    if (m_Broker.BankBalance < 0)
                    {
                        from.SendGump(new BazaarInformationGump(1150623, 1150615));
                        return;
                    }
                    from.SendGump(new SetPricesAndLimitsGump(m_Broker));
                    return;
            }

            from.SendGump(new CommodityBrokerGump(m_Broker, from));
        }

        public class InternalTarget : Target
        {
            private readonly CommodityBroker m_Broker;
            private bool m_HasPickedCommodity;

            public InternalTarget(CommodityBroker broker) : this(broker, false)
            {
            }

            public InternalTarget(CommodityBroker broker, bool haspicked) : base(-1, false, TargetFlags.None)
            {
                m_Broker = broker;
                m_HasPickedCommodity = haspicked;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from.HasGump(typeof(CommodityTargetGump)))
                    from.CloseGump(typeof(CommodityTargetGump));

                if (targeted is Item && (targeted is ICommodity || targeted is CommodityDeed))
                {
                    Item item = (Item)targeted;

                    if (item.IsChildOf(from.Backpack))
                    {
                        if (item is CommodityDeed && ((CommodityDeed)item).Commodity == null)
                        {
                            from.SendLocalizedMessage(1150222); // That item is not a commodity that a broker can trade.
                        }
                        else if (m_Broker.CommodityEntries.Count < CommodityBroker.MaxEntries)
                        {
                            if (!m_Broker.TryAddBrokerEntry(item, from))    // Adds new entry									// Already has an entry, adds item to inventory for selling
                                m_Broker.AddInventory(from, item);          // or adds to the stock

                            m_HasPickedCommodity = true;
                        }
                        else
                        {
                            from.SendMessage("You can only have a maximum of {0} commodities on your merchant at a time.", CommodityBroker.MaxEntries.ToString());
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1054107); // This item must be in your backpack.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1150222); // That item is not a commodity that a broker can trade.
                }

                from.Target = new InternalTarget(m_Broker, m_HasPickedCommodity);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (from.HasGump(typeof(CommodityTargetGump)))
                    from.CloseGump(typeof(CommodityTargetGump));

                if (m_HasPickedCommodity)
                    from.SendGump(new SetPricesAndLimitsGump(m_Broker));
                else
                    from.SendGump(new CommodityBrokerGump(m_Broker, from));
            }
        }
    }

    public class CommodityTargetGump : BaseBazaarGump
    {
        private readonly CommodityBroker m_Broker;

        public CommodityTargetGump(CommodityBroker broker)
            : base(520, 520)
        {
            m_Broker = broker;

            AddHtmlLocalized(10, 10, 500, 18, 1114513, "#1150636", RedColor16, false, false);  // Commodity Broker

            if (m_Broker.Plot.ShopName != null && m_Broker.Plot.ShopName.Length > 0)
            {
                AddHtml(10, 37, 500, 18, Color(FormatStallName(m_Broker.Plot.ShopName), BlueColor), false, false);
            }
            else
            {
                AddHtmlLocalized(10, 37, 500, 18, 1114513, "#1150314", BlueColor16, false, false); // This Shop Has No Name
            }

            AddHtmlLocalized(10, 55, 260, 18, 1114514, "#1150313", BlueColor16, false, false); // Proprietor:
            AddHtml(280, 55, 260, 18, Color(string.Format("{0}", broker.Name), BlueColor), false, false);

            /* Target commodity items or filled commodity deeds in your backpack to add them to the 
			 * broker's inventory. These items will be retrievable, and the broker will not trade them 
			 * until you establish prices.<br><br>When done, press the [ESC] key to cancel your targeting 
			 * cursor, or click the MAIN MENU button below.*/

            AddHtmlLocalized(10, 100, 500, 400, 1150209, OrangeColor16, false, false);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (info.ButtonID == 1)
            {
                if (from.Target is CommodityBrokerGump.InternalTarget)
                    Target.Cancel(from);
            }
        }
    }

    public class SetPricesAndLimitsGump : BaseBazaarGump
    {
        private readonly CommodityBroker m_Broker;
        private int m_Index;
        private readonly int m_Page;

        public SetPricesAndLimitsGump(CommodityBroker broker) : this(broker, -1, 0) { }

        public SetPricesAndLimitsGump(CommodityBroker broker, int index, int page)
            : base(520, 520)
        {
            m_Broker = broker;
            m_Index = index;
            m_Page = page;

            AddHtmlLocalized(10, 10, 500, 18, 1114513, "#1150636", RedColor16, false, false);  // Commodity Broker

            AddButton(150, 40, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(190, 40, 200, 18, 1150392, OrangeColor16, false, false); // INFORMATION

            AddHtmlLocalized(100, 65, 100, 18, 1150140, OrangeColor16, false, false); // ITEM
            AddHtmlLocalized(170, 65, 100, 18, 1150204, OrangeColor16, false, false); // BUY AT
            AddHtmlLocalized(230, 65, 100, 18, 1150645, OrangeColor16, false, false); // BUY LMT
            AddHtmlLocalized(315, 65, 100, 18, 1150205, OrangeColor16, false, false); // SELL AT
            AddHtmlLocalized(380, 65, 100, 18, 1150646, OrangeColor16, false, false); // SELL LMT
            AddHtmlLocalized(475, 65, 100, 18, 1150647, OrangeColor16, false, false); // EDIT

            int y = 85;
            int perPage = 10;

            if (index > -1)
                m_Page = index <= 0 ? 0 : index / perPage;

            int start = page * perPage;
            int count = 1;

            for (int i = start; i < broker.CommodityEntries.Count && /*index <= perPage * (m_Page + 1) &&*/ count <= perPage; i++)
            {
                int col = i == m_Index ? YellowColor : OrangeColor;
                int col16 = col == YellowColor ? YellowColor16 : OrangeColor16;
                CommodityBrokerEntry entry = broker.CommodityEntries[i];

                AddHtmlLocalized(1, y, 130, 18, 1114514, string.Format("#{0}", entry.Label), col16, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
                AddHtml(170, y, 45, 18, Color(AlignRight(FormatAmt(entry.BuyPricePer)), col), false, false);
                AddHtml(230, y, 80, 18, Color(AlignRight(FormatAmt(entry.BuyLimit)), col), false, false);
                AddHtml(315, y, 45, 18, Color(AlignRight(FormatAmt(entry.SellPricePer)), col), false, false);
                AddHtml(380, y, 80, 18, Color(AlignRight(FormatAmt(entry.SellLimit)), col), false, false);

                AddButton(475, y, 4014, 4016, 2 + i, GumpButtonType.Reply, 0);

                y += 22;
                count++;
            }

            if (m_Page > 0) // back
                AddButton(162, 321, 4014, 4016, 400, GumpButtonType.Reply, 0);

            if (broker.CommodityEntries.Count - start > perPage) // forward
                AddButton(390, 321, 4005, 4007, 401, GumpButtonType.Reply, 0);

            AddHtmlLocalized(124, 345, 100, 18, 1150204, BlueColor16, false, false); // BUY AT
            AddBackground(190, 345, 300, 18, 9350);
            AddTextEntry(191, 344, 320, 22, LabelHueBlue, 0, "");

            AddHtmlLocalized(114, 370, 100, 18, 1150645, BlueColor16, false, false); // BUY LMT
            AddBackground(190, 370, 300, 18, 9350);
            AddTextEntry(191, 369, 320, 22, LabelHueBlue, 1, "");

            AddHtmlLocalized(118, 395, 100, 18, 1150205, BlueColor16, false, false); // SELL AT
            AddBackground(190, 395, 300, 18, 9350);
            AddTextEntry(191, 394, 320, 22, LabelHueBlue, 2, "");

            AddHtmlLocalized(108, 420, 100, 18, 1150646, BlueColor16, false, false); // SELL LMT
            AddBackground(190, 420, 300, 18, 9350);
            AddTextEntry(191, 419, 320, 22, LabelHueBlue, 3, "");

            AddButton(140, 450, 4005, 4007, 500, GumpButtonType.Reply, 0);
            AddHtmlLocalized(190, 450, 200, 18, 1150232, RedColor16, false, false); // SET PRICES

            AddButton(10, 490, 4014, 4016, 501, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 100, 18, 1149777, BlueColor16, false, false); // MAIN MENU
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (info.ButtonID == 0)
                return;
            if (info.ButtonID == 1)
            {
                from.SendGump(new BazaarInformationGump(0, 1150644, new SetPricesAndLimitsGump(m_Broker, m_Index, m_Page)));
                return;
            }
            else if (info.ButtonID == 400) // back page
            {
                from.SendGump(new SetPricesAndLimitsGump(m_Broker, -1, m_Page - 1));
                return;
            }
            else if (info.ButtonID == 401) // forward page
            {
                from.SendGump(new SetPricesAndLimitsGump(m_Broker, -1, m_Page + 1));
                return;
            }
            else if (info.ButtonID == 500) // SET PRICES
            {
                if (m_Index >= 0 && m_Index < m_Broker.CommodityEntries.Count)
                {
                    CommodityBrokerEntry entry = m_Broker.CommodityEntries[m_Index];

                    int buyAt = entry.BuyPricePer;
                    int buyLmt = entry.BuyLimit;
                    int sellAt = entry.SellPricePer;
                    int sellLmt = entry.SellLimit;

                    TextRelay relay1 = info.TextEntries[0];
                    TextRelay relay2 = info.TextEntries[1];
                    TextRelay relay3 = info.TextEntries[2];
                    TextRelay relay4 = info.TextEntries[3];
                    try
                    {
                        buyAt = Convert.ToInt32(relay1.Text);
                    }
                    catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                    try
                    {
                        buyLmt = Convert.ToInt32(relay2.Text);
                    }
                    catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                    try
                    {
                        sellAt = Convert.ToInt32(relay3.Text);
                    }
                    catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                    try
                    {
                        sellLmt = Convert.ToInt32(relay4.Text);
                    }
                    catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }

                    if (buyLmt < 0 || buyLmt > 60000 || sellLmt < 0 || sellLmt > 60000)
                        from.SendLocalizedMessage(1150776); // You have entered an invalid numeric value. Negative values are not allowed. Trade quantities are limited to 60,000 per transaction.
                    else
                    {
                        entry.BuyPricePer = buyAt;
                        entry.BuyLimit = buyLmt;
                        entry.SellPricePer = sellAt;
                        entry.SellLimit = sellLmt;
                    }
                }
                else
                    from.SendLocalizedMessage(1150642); // You did not select a commodity.
            }
            else if (info.ButtonID == 501) // Main Menu
            {
                from.SendGump(new CommodityBrokerGump(m_Broker, from));
                return;
            }
            else
            {
                int id = info.ButtonID - 2;

                if (id >= 0 && id < m_Broker.CommodityEntries.Count)
                    m_Index = id;
            }

            from.SendGump(new SetPricesAndLimitsGump(m_Broker, m_Index, m_Page));
        }
    }

    public class ViewInventoryGump : BaseBazaarGump
    {
        private readonly CommodityBroker m_Broker;
        private readonly int m_Index;
        private readonly int m_Page;

        public ViewInventoryGump(CommodityBroker broker) : this(broker, -1)
        {
        }

        public ViewInventoryGump(CommodityBroker broker, int index) : this(broker, index, 0)
        {
        }

        public ViewInventoryGump(CommodityBroker broker, int index, int page)
            : base(660, 520)
        {
            m_Broker = broker;
            m_Index = index;
            m_Page = page;

            AddHtmlLocalized(10, 10, 640, 18, 1114513, "#1150636", RedColor16, false, false);  // Commodity Broker
            AddHtmlLocalized(10, 40, 640, 60, 1150641, OrangeColor16, false, false); // Click the button next to the commodity...

            AddHtmlLocalized(235, 125, 150, 18, 1150140, OrangeColor16, false, false); // ITEM
            AddHtmlLocalized(360, 125, 150, 18, 1150201, OrangeColor16, false, false); // IN STOCK
            AddHtmlLocalized(440, 125, 150, 18, 1150639, OrangeColor16, false, false); // SELECT
            AddHtmlLocalized(540, 125, 150, 18, 1150855, OrangeColor16, false, false); // DOWN
            AddHtmlLocalized(600, 125, 150, 18, 1150856, OrangeColor16, false, false); // UP

            int y = 150;
            int perPage = 10;

            if (index > -1)
                m_Page = index <= 0 ? 0 : index / perPage;

            int start = page * perPage;
            int count = 1;

            for (int i = start; i < broker.CommodityEntries.Count && /*index <= perPage &&*/ count <= perPage; i++)
            {
                int col = i == m_Index ? YellowColor : OrangeColor;
                int col16 = col == YellowColor ? YellowColor16 : OrangeColor16;
                CommodityBrokerEntry entry = broker.CommodityEntries[i];

                AddHtmlLocalized(1, y, 260, 18, 1114514, string.Format("#{0}", entry.Label), col16, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
                AddHtml(360, y, 55, 18, Color(AlignRight(FormatAmt(entry.Stock)), col), false, false);

                AddButton(440, y, 4014, 4016, 1000 + i, GumpButtonType.Reply, 0); // SELECT

                if (i > 0)
                    AddButton(600, y, 250, 251, 1500 + i, GumpButtonType.Reply, 0); // UP

                if (i < broker.CommodityEntries.Count - 1)
                    AddButton(540, y, 252, 253, 2000 + i, GumpButtonType.Reply, 0); // DOWN

                y += 20;
                count++;
            }

            if (m_Page > 0) // back
                AddButton(162, 350, 4014, 4016, 400, GumpButtonType.Reply, 0);

            if (broker.CommodityEntries.Count - start > perPage) // forward
                AddButton(390, 350, 4005, 4007, 401, GumpButtonType.Reply, 0);

            AddHtmlLocalized(160, 415, 150, 18, 1150202, OrangeColor16, false, false); // WITHDRAW
            AddBackground(250, 415, 360, 22, 9350);
            AddTextEntry(251, 415, 358, 20, LabelHueBlue, 0, "");
            AddButton(620, 415, 4014, 4016, 1, GumpButtonType.Reply, 0);

            if (index > -1)
            {
                AddHtml(50, 460, 200, 18, Color("Remove from Inventory", RedColor), false, false);
                AddButton(10, 460, 4020, 4022, 3, GumpButtonType.Reply, 0);
            }

            AddButton(10, 490, 4014, 4016, 501, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 100, 18, 1149777, BlueColor16, false, false); // MAIN MENU
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0: return;
                case 1: // Withdraw
                    if (m_Index >= 0 && m_Index < m_Broker.CommodityEntries.Count)
                    {
                        CommodityBrokerEntry entry = m_Broker.CommodityEntries[m_Index];
                        int amount = 0;

                        TextRelay relay = info.TextEntries[0];

                        try
                        {
                            amount = Convert.ToInt32(relay.Text);
                        }
                        catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }

                        if (amount <= 0 || amount > entry.Stock)
                            from.SendLocalizedMessage(1150215); // You have entered an invalid value, or a non-numeric value. Please try again.
                        else
                            m_Broker.WithdrawInventory(from, amount, entry);
                    }
                    else
                        from.SendLocalizedMessage(1150642); // You did not select a commodity.
                    break;
                case 2: // Main Menu
                    from.SendGump(new CommodityBrokerGump(m_Broker, from));
                    return;
                case 3: // Remove Entry
                    if (m_Index >= 0 && m_Index < m_Broker.CommodityEntries.Count)
                    {
                        from.SendGump(new ConfirmRemoveEntryGump(m_Broker, m_Index));
                        return;
                    }
                    else
                        from.SendLocalizedMessage(1150642); // You did not select a commodity.
                    break;
                case 400:
                    from.SendGump(new ViewInventoryGump(m_Broker, -1, m_Page - 1));
                    return;
                case 401:
                    from.SendGump(new ViewInventoryGump(m_Broker, -1, m_Page + 1));
                    return;
                case 501:
                    from.SendGump(new CommodityBrokerGump(m_Broker, from));
                    return;
                default:
                    {
                        if (info.ButtonID < 1500) // SELECT ENTRY
                        {
                            int id = info.ButtonID - 1000;

                            if (id >= 0 && id < m_Broker.CommodityEntries.Count)
                            {
                                from.SendGump(new ViewInventoryGump(m_Broker, id, m_Page));
                                return;
                            }
                        }
                        else if (info.ButtonID < 2000) // UP
                        {
                            int id = info.ButtonID - 1500;

                            if (id > 0 && id < m_Broker.CommodityEntries.Count)
                            {
                                CommodityBrokerEntry entry = m_Broker.CommodityEntries[id];
                                int idx = m_Broker.CommodityEntries.IndexOf(entry);

                                m_Broker.CommodityEntries.Remove(entry);
                                m_Broker.CommodityEntries.Insert(idx - 1, entry);
                                //m_Broker.CommodityEntries[id] = m_Broker.CommodityEntries[id-1];
                                //m_Broker.CommodityEntries[id-1] = entry;
                            }
                        }
                        else // DOWN
                        {
                            int id = info.ButtonID - 2000;

                            if (id >= 0 && id < m_Broker.CommodityEntries.Count - 1)
                            {
                                CommodityBrokerEntry entry = m_Broker.CommodityEntries[id];
                                int idx = m_Broker.CommodityEntries.IndexOf(entry);

                                m_Broker.CommodityEntries.Remove(entry);
                                m_Broker.CommodityEntries.Insert(idx + 1, entry);
                                //m_Broker.CommodityEntries[id] = m_Broker.CommodityEntries[id+1];
                                //m_Broker.CommodityEntries[id+1] = entry;
                            }
                        }

                        break;
                    }
            }

            from.SendGump(new ViewInventoryGump(m_Broker, m_Index));
        }
    }

    public class CommodityInventoryGump : BaseBazaarGump
    {
        private readonly CommodityBroker m_Broker;
        private readonly List<CommodityBrokerEntry> m_Entries;
        private bool m_Buy;
        private readonly int m_Page;
        private int m_Index;

        public CommodityInventoryGump(CommodityBroker broker) : this(broker, -1, true, 0, 0)
        {
        }

        public CommodityInventoryGump(CommodityBroker broker, int index, bool buy, int page, int cliloc)
            : base(655, 520)
        {
            m_Broker = broker;
            m_Entries = broker.CommodityEntries;
            m_Buy = buy;
            m_Page = page;
            m_Index = index;

            AddHtmlLocalized(10, 10, 640, 18, 1114513, "#1150636", RedColor16, false, false);  // Commodity Broker

            if (m_Broker.Plot.ShopName != null && m_Broker.Plot.ShopName.Length > 0)
            {
                AddHtml(10, 37, 640, 18, Color(FormatStallName(m_Broker.Plot.ShopName), BlueColor), false, false);
            }
            else
            {
                AddHtmlLocalized(10, 37, 640, 18, 1114513, "#1150314", BlueColor16, false, false); // This Shop Has No Name
            }

            AddHtmlLocalized(10, 55, 310, 18, 1114514, "#1150313", BlueColor16, false, false); // Proprietor:
            AddHtml(330, 55, 320, 18, Color(string.Format("{0}", broker.Name), BlueColor), false, false);

            if (cliloc != 0)
            {
                if (cliloc == 1150654)
                {
                    /* This broker trades in commodities. Commodity brokers can buy and sell most commodities, either in item or deed form.
                    The menu shows all commodities this broker will deal in.<BR><BR>The center column lists the commodities by name.
                    The columns on the left indicate how much of the commodity the broker has available for purchase, and the price per unit.
                    The right-hand columns indicate how much of the commodity the broker is willing to buy from you and the price it will pay per unit.
                    <BR><BR>If you wish to buy that commodity, click the button to the left of it to select it, then enter the quantity you wish to buy in the QUANTITY field below.
                    If you wish to sell a commodity to the broker, click the button to the right of it and enter the quantity you wish to sell below. Finally, click the "TRADE" button.
                    You will be presented with a confirmation window that will show the item being traded, the quantity, and how much gold will change hands.<BR><BR>If a broker lists a price but shows 0 quantity available,
                    the broker may be out of stock (unable to sell to you) or may have reached its purchasing limit (unable to buy from you). All gold transactions are between your bank box and the commodity broker.
                    Gold or bank checks in your backpack cannot be used for these trades. Commodities are traded between your backpack and the broker, and commodity items/deeds in your bank box cannot be used for these trades.
                    */
                    AddHtmlLocalized(10, 127, 640, 354, 1150654, OrangeColor16, true, true);
                }
                else
                {
                    AddHtmlLocalized(10, 127, 640, 354, 1114513, string.Format("#{0}", cliloc), OrangeColor16, false, false);
                }

                AddButton(10, 490, 0xFAE, 0xFAF, 999, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 490, 210, 20, 1149777, BlueColor16, false, false); // MAIN MENU
            }
            else
            {

                AddButton(190, 91, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(230, 91, 420, 22, 1150392, OrangeColor16, false, false); // INFORMATION

                AddHtmlLocalized(10, 124, 54, 20, 1150656, OrangeColor16, false, false); // BUY
                AddHtmlLocalized(70, 124, 54, 20, 1150659, OrangeColor16, false, false); // PRICE
                AddHtmlLocalized(130, 124, 109, 20, 1150658, OrangeColor16, false, false); // LIMIT
                AddHtmlLocalized(245, 124, 164, 20, 1114513, "#1150655", OrangeColor16, false, false); // COMMODITY
                AddHtmlLocalized(415, 124, 54, 20, 1114514, "#1150659", OrangeColor16, false, false); // PRICE
                AddHtmlLocalized(475, 124, 109, 20, 1114514, "#1150658", OrangeColor16, false, false); // LIMIT
                AddHtmlLocalized(590, 124, 54, 20, 1114514, "#1150657", OrangeColor16, false, false); // SELL

                if (m_Entries.Count != 0)
                {
                    int y = 153;
                    int perPage = 10;

                    if (index > -1)
                        m_Page = index <= 0 ? 0 : index / perPage;

                    int start = page * perPage;
                    int count = 0;

                    for (int i = start; i < m_Entries.Count && /*index < perPage*/ count <= perPage; i++)
                    {
                        CommodityBrokerEntry entry = m_Entries[i];

                        int stock = entry.Stock;
                        int buyLimit = entry.ActualSellLimit;
                        int sellLimit = entry.ActualBuyLimit;

                        // Sell to player
                        if (entry.SellPricePer > 0)
                        {
                            AddHtml(70, y, 54, 20, Color(FormatAmt(entry.SellPricePer), OrangeColor), false, false);
                        }
                        else
                        {
                            AddHtmlLocalized(70, y, 54, 20, 1150397, OrangeColor16, false, false); // N/A
                        }

                        if (entry.SellPricePer > 0 && entry.SellLimit > 0)
                        {
                            AddHtml(130, y, 109, 20, Color(FormatAmt(buyLimit), OrangeColor), false, false);
                        }

                        // what we're selling/buying
                        AddHtmlLocalized(200, y, 164, 20, 1114514, string.Format("#{0}", entry.Label), OrangeColor16, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
                        AddTooltip(entry.Label);

                        // buy from player

                        if (entry.BuyPricePer > 0)
                        {
                            AddHtml(415, y, 54, 20, Color(AlignRight(FormatAmt(entry.BuyPricePer)), OrangeColor), false, false);
                        }
                        else
                        {
                            AddHtmlLocalized(415, y, 54, 20, 1150397, OrangeColor16, false, false); // N/A
                        }

                        if (entry.BuyPricePer > 0 && entry.BuyLimit > 0)
                        {
                            AddHtml(475, y, 109, 20, (Color(AlignRight(FormatAmt(sellLimit)), OrangeColor)), false, false);
                        }

                        //Buttons
                        if (entry.PlayerCanBuy(0))
                            AddRadio(10, y, 0xFA5, 0xFA6, false, 2 + i);

                        if (entry.PlayerCanSell(0))
                            AddRadio(590, y, 0xFAE, 0xFAF, false, 102 + i);

                        y += 20;
                        count++;
                    }

                    if (m_Page > 0) // back
                    {
                        AddButton(60, 362, 0xFAE, 0xFAF, 400, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(100, 362, 230, 40, 1044044, OrangeColor16, false, false); // PREV PAGE
                    }

                    if (m_Entries.Count - start > perPage) // forward
                    {
                        AddButton(570, 362, 0xFA5, 0xFA6, 401, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(330, 362, 230, 40, 1114514, "#1044045", OrangeColor16, false, false); // NEXT PAGE
                    }

                    AddHtmlLocalized(10, 421, 210, 22, 1114514, "#1150662", BlueColor16, false, false); // QUANTITY
                    AddBackground(230, 421, 420, 22, 0x2486);
                    AddTextEntry(232, 423, 416, 18, LabelHueBlue, 0, "");

                    AddButton(190, 445, 4005, 4007, 500, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(230, 445, 420, 22, 1077761, OrangeColor16, false, false); // TRADE
                }
                else
                {
                    AddHtmlLocalized(10, 153, 640, 20, 1114513, "#1150638", OrangeColor16, false, false); // There are no commodities in this broker's inventory.
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0: return;
                case 999:
                    from.SendGump(new CommodityInventoryGump(m_Broker, m_Index, m_Buy, m_Page, 0));
                    return;
                case 1:
                    from.SendGump(new CommodityInventoryGump(m_Broker, m_Index, m_Buy, m_Page, 1150654));
                    return;
                case 400:
                    from.SendGump(new CommodityInventoryGump(m_Broker, -1, m_Buy, m_Page - 1, 0));
                    return;
                case 401:
                    from.SendGump(new CommodityInventoryGump(m_Broker, -1, m_Buy, m_Page + 1, 0));
                    return;
                case 500: // TRADE
                    {
                        for (int i = 0; i < m_Entries.Count; i++)
                        {
                            int buyid = 2 + i;
                            int sellid = 102 + i;

                            if (info.IsSwitched(buyid))
                            {
                                if (m_Broker.CommodityEntries.Contains(m_Entries[i]))
                                {
                                    m_Buy = true;
                                    m_Index = i;
                                }
                            }
                            else if (info.IsSwitched(sellid))
                            {
                                if (m_Broker.CommodityEntries.Contains(m_Entries[i]))
                                {
                                    m_Buy = false;
                                    m_Index = i;
                                }
                            }
                        }

                        if (m_Index >= 0 && m_Index < m_Entries.Count)
                        {
                            CommodityBrokerEntry entry = m_Entries[m_Index];

                            if (!m_Broker.CommodityEntries.Contains(entry))
                            {
                                from.SendGump(new CommodityInventoryGump(m_Broker, m_Index, m_Buy, m_Page, 1150244)); // Transaction no longer available
                                return;
                            }

                            int amount = 0;
                            TextRelay relay = info.TextEntries[0];

                            try
                            {
                                amount = Convert.ToInt32(relay.Text);
                            }
                            catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }

                            if (amount > 0)
                            {
                                if (m_Buy && Banker.GetBalance(from) < entry.SellPricePer * amount)
                                {
                                    /* You do not have the funds needed to make this trade available in your bank box. Brokers are only able to transfer funds from your bank box.
                                    Please deposit the necessary funds into your bank box and try again.
                                    */
                                    from.SendGump(new CommodityInventoryGump(m_Broker, m_Index, m_Buy, m_Page, 1150252));
                                    return;
                                }
                                else if (!m_Buy && !m_Broker.SellCommodityControl(from, entry, amount))
                                {
                                    /* You do not have the requested amount of that commodity (either in item or deed form) in your backpack to trade.
                                     * Note that commodities cannot be traded from your bank box.
                                    */
                                    from.SendGump(new CommodityInventoryGump(m_Broker, m_Index, m_Buy, m_Page, 1150667));
                                    return;
                                }
                                else if (m_Buy && entry.PlayerCanBuy(amount))
                                {
                                    from.SendGump(new ConfirmBuyCommodityGump(m_Broker, amount, entry, true, new CommodityInventoryGump(m_Broker, m_Index, m_Buy, m_Page, 0)));
                                    return;
                                }
                                else if (!m_Buy && entry.PlayerCanSell(amount))
                                {
                                    from.SendGump(new ConfirmBuyCommodityGump(m_Broker, amount, entry, false, new CommodityInventoryGump(m_Broker, m_Index, m_Buy, m_Page, 0)));
                                    return;
                                }
                                else
                                {
                                    from.SendGump(new CommodityInventoryGump(m_Broker, m_Index, m_Buy, m_Page, 1150215)); // You have entered an invalid value, or a non-numeric value. Please try again.
                                    return;
                                }
                            }
                            else
                            {
                                from.SendGump(new CommodityInventoryGump(m_Broker, m_Index, m_Buy, m_Page, 1150215)); // You have entered an invalid value, or a non-numeric value. Please try again.
                                return;
                            }
                        }
                        else
                        {
                            from.SendGump(new CommodityInventoryGump(m_Broker, m_Index, m_Buy, m_Page, 1150642)); // You did not select a commodity.
                            return;
                        }
                    }
            }
        }
    }

    public class ConfirmRemoveEntryGump : BaseConfirmGump
    {
        private readonly CommodityBroker m_Broker;
        private readonly CommodityBrokerEntry m_Entry;
        private readonly int m_Index;

        public override string TitleString => "Remove Commodity Confirmation";
        public override string LabelString => "Are you sure you want to remove this entry from your commodity broker? Any unused stock will be placed in your bankbox.";

        public ConfirmRemoveEntryGump(CommodityBroker broker, int index)
        {
            m_Broker = broker;
            m_Index = index;

            if (index >= 0 && index < m_Broker.CommodityEntries.Count)
                m_Entry = m_Broker.CommodityEntries[index];
        }

        public override void Confirm(Mobile from)
        {
            if (m_Entry != null && m_Broker.CommodityEntries.Contains(m_Entry))
                m_Broker.RemoveEntry(from, m_Entry);

            from.SendGump(new ViewInventoryGump(m_Broker));
        }

        public override void Refuse(Mobile from)
        {
            from.SendGump(new ViewInventoryGump(m_Broker, m_Index));
        }
    }

    public class ConfirmBuyCommodityGump : BaseBazaarGump
    {
        private readonly Gump _Gump;
        private readonly CommodityBroker m_Broker;
        private readonly int m_Amount;
        private readonly CommodityBrokerEntry m_Entry;

        public ConfirmBuyCommodityGump(CommodityBroker broker, int amount, CommodityBrokerEntry entry, bool buy, Gump g)
            : base(660, 520)
        {
            m_Broker = broker;
            m_Amount = amount;
            m_Entry = entry;
            _Gump = g;

            AddHtmlLocalized(10, 10, 640, 18, 1114513, "#1150636", RedColor16, false, false);  // Commodity Broker

            if (m_Broker.Plot.ShopName != null && m_Broker.Plot.ShopName.Length > 0)
            {
                AddHtml(10, 37, 640, 18, Color(FormatStallName(m_Broker.Plot.ShopName), BlueColor), false, false);
            }
            else
            {
                AddHtmlLocalized(10, 37, 640, 18, 1114513, "#1150314", BlueColor16, false, false); // This Shop Has No Name
            }

            AddHtmlLocalized(10, 55, 310, 18, 1114514, "#1150313", BlueColor16, false, false); // Proprietor:
            AddHtml(330, 55, 320, 18, Color(string.Format("{0}", broker.Name), BlueColor), false, false);

            AddHtmlLocalized(10, 127, 640, 64, 1114513, "#1150666", RedColor16, false, false); // Please review the details of this transaction. If you wish to make this trade, click the TRADE button below. Otherwise, click the MAIN MENU button to return to the price list.

            if (buy)
            {
                int cost = entry.SellPricePer * amount;

                AddHtmlLocalized(10, 193, 210, 18, 1114514, "#1150144", OrangeColor16, false, false); // You are BUYING:
                AddHtmlLocalized(230, 193, 420, 18, m_Broker.GetLabelID(entry), DarkGreenColor16, false, false);

                AddHtmlLocalized(10, 213, 210, 18, 1114514, "#1150152", OrangeColor16, false, false); // Quantity to Buy:
                AddHtml(230, 213, 420, 18, Color(string.Format("{0}", amount), DarkGreenColor), false, false);

                AddHtmlLocalized(10, 233, 210, 18, 1114514, "#1150246", OrangeColor16, false, false); // Total Cost:
                AddHtml(230, 233, 420, 18, Color(FormatAmt(cost), DarkGreenColor), false, false);

                AddHtmlLocalized(230, 253, 420, 22, 1150143, OrangeColor16, false, false); // TRADE
                AddButton(190, 253, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0);
            }
            else
            {
                int cost = entry.BuyPricePer * amount;

                AddHtmlLocalized(10, 193, 210, 18, 1114514, "#1150145", OrangeColor16, false, false); // You are SELLING:
                AddHtmlLocalized(230, 193, 420, 18, m_Broker.GetLabelID(entry), DarkGreenColor16, false, false);

                AddHtmlLocalized(10, 213, 210, 18, 1114514, "#1150153", OrangeColor16, false, false); // Quantity to Sell:
                AddHtml(230, 213, 420, 18, Color(string.Format("{0}", amount), DarkGreenColor), false, false);

                AddHtmlLocalized(10, 233, 210, 18, 1114514, "#1150251", OrangeColor16, false, false); // Gold You Will Receive:
                AddHtml(230, 233, 420, 18, Color(FormatAmt(cost), DarkGreenColor), false, false);

                AddHtmlLocalized(230, 253, 420, 22, 1150143, OrangeColor16, false, false); // TRADE
                AddButton(190, 253, 0xFA5, 0xFA6, 2, GumpButtonType.Reply, 0);
            }

            AddButton(10, 490, 0xFAE, 0xFAF, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 210, 20, 1149777, BlueColor16, false, false); // MAIN MENU
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                default:
                case 0: break;
                case 1: //BUY
                    m_Broker.TryBuyCommodity(from, m_Entry, m_Amount);
                    break;
                case 2: //SELL
                    m_Broker.TrySellCommodity(from, m_Entry, m_Amount);
                    break;
                case 3: //MAIN MENU
                    from.SendGump(_Gump);
                    break;
            }
        }
    }
}
