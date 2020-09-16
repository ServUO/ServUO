using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
    public class PetBrokerGump : BaseBazaarGump
    {
        private readonly PetBroker m_Broker;

        public PetBrokerGump(PetBroker broker, Mobile from)
        {
            m_Broker = broker;

            AddHtmlLocalized(215, 10, 200, 18, 1150311, RedColor16, false, false); // Animal Broker

            if (m_Broker.Plot.ShopName != null && m_Broker.Plot.ShopName.Length > 0)
                AddHtml(173, 40, 173, 18, Color(FormatStallName(m_Broker.Plot.ShopName), BlueColor), false, false);
            else
                AddHtmlLocalized(180, 40, 200, 18, 1150314, BlueColor16, false, false); // This Shop Has No Name

            AddHtml(173, 65, 173, 18, Color(FormatBrokerName(string.Format("Proprietor: {0}", broker.Name)), BlueColor), false, false);

            AddHtmlLocalized(215, 100, 200, 18, 1150328, GreenColor16, false, false); // Owner Menu

            AddButton(150, 150, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(190, 150, 200, 18, 1150392, OrangeColor16, false, false); // INFORMATION

            AddHtmlLocalized(39, 180, 200, 18, 1150199, RedColor16, false, false); // Broker Account Balance
            AddHtml(190, 180, 300, 18, FormatAmt(broker.BankBalance), false, false);

            int balance = Banker.GetBalance(from);
            AddHtmlLocalized(68, 200, 200, 18, 1150149, GreenColor16, false, false); // Your Bank Balance:
            AddHtml(190, 200, 200, 18, FormatAmt(balance), false, false);

            AddHtmlLocalized(32, 230, 200, 18, 1150329, OrangeColor16, false, false); // Broker Sales Comission
            AddHtmlLocalized(190, 230, 100, 18, 1150330, false, false); // 5%

            AddHtmlLocalized(110, 250, 200, 18, 1150331, OrangeColor16, false, false); // Weekly Fee:
            AddHtml(190, 250, 250, 18, FormatAmt(broker.GetWeeklyFee()), false, false);

            AddHtmlLocalized(113, 280, 200, 18, 1150332, OrangeColor16, false, false); // Shop Name:
            AddBackground(190, 280, 285, 22, 9350);
            AddTextEntry(191, 280, 285, 20, LabelHueBlue, 0, m_Broker.Plot.ShopName == null ? "" : m_Broker.Plot.ShopName);
            AddButton(480, 280, 4014, 4016, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(83, 305, 150, 18, 1150195, OrangeColor16, false, false); // Withdraw Funds
            AddBackground(190, 305, 285, 22, 9350);
            AddTextEntry(191, 305, 285, 20, LabelHueBlue, 1, "");
            AddButton(480, 305, 4014, 4016, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(95, 330, 150, 18, 1150196, OrangeColor16, false, false); // Deposit Funds
            AddBackground(190, 330, 285, 22, 9350);
            AddTextEntry(191, 330, 285, 20, LabelHueBlue, 2, "");
            AddButton(480, 330, 4014, 4016, 4, GumpButtonType.Reply, 0);

            AddButton(150, 365, 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddHtmlLocalized(190, 365, 350, 18, 1150276, OrangeColor16, false, false); // TRANSFER STABLED PETS TO MERCHANT

            AddButton(150, 390, 4005, 4007, 6, GumpButtonType.Reply, 0);
            AddHtmlLocalized(190, 390, 350, 18, 1150277, OrangeColor16, false, false); // TRANSFER MERCHANT INVENTORY TO STABLE

            AddButton(150, 415, 4005, 4007, 7, GumpButtonType.Reply, 0);
            AddHtmlLocalized(190, 415, 350, 18, 1150334, OrangeColor16, false, false); // VIEW INVENTORY / EDIT PRICES
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
                    from.SendGump(new PetBrokerGump(m_Broker, from));
                    from.SendGump(new BazaarInformationGump(1150311, 1150614));
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
                    catch (Exception e)
                    {
                        Diagnostics.ExceptionLogging.LogException(e);
                    }

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
                case 5: // TRANSFER STABLED PET TO MERCHANT
                    if (from.Stabled.Count > 0)
                        from.SendGump(new SelectPetsGump(m_Broker, from));
                    else
                        from.SendLocalizedMessage(1150335); // You currently have no pets in your stables that can be traded via an animal broker.
                    return;
                case 6: // TRANSFER MERCHANT INVENTORY TO STABLE
                    m_Broker.CheckInventory();
                    if (m_Broker.BrokerEntries.Count > 0)
                    {
                        if (m_Broker.BankBalance < 0)
                        {
                            from.SendGump(new BazaarInformationGump(1150623, 1150615));
                            return;
                        }
                        from.SendGump(new RemovePetsGump(m_Broker, from));
                    }
                    else
                        from.SendLocalizedMessage(1150336); // The animal broker has no pets in its inventory.
                    return;
                case 7: // VIEW INVENTORY / EDIT PRICES
                    m_Broker.CheckInventory();
                    if (m_Broker.BrokerEntries.Count > 0)
                    {
                        if (m_Broker.BankBalance < 0)
                        {
                            from.SendGump(new BazaarInformationGump(1150623, 1150615));
                            return;
                        }
                        else
                            from.SendGump(new SetPetPricesGump(m_Broker));
                    }
                    else
                        from.SendLocalizedMessage(1150336); // The animal broker has no pets in its inventory.
                    return;
            }

            from.SendGump(new PetBrokerGump(m_Broker, from));
        }
    }

    public class SelectPetsGump : BaseBazaarGump
    {
        private readonly PetBroker m_Broker;
        private readonly int m_Index;
        private readonly List<BaseCreature> m_List;

        public SelectPetsGump(PetBroker broker, Mobile from) : this(broker, from, -1)
        {
        }

        public SelectPetsGump(PetBroker broker, Mobile from, int index)
        {
            m_Broker = broker;
            m_Index = index;

            AddHtmlLocalized(215, 10, 200, 18, 1150311, RedColor16, false, false); // Animal Broker
            AddHtmlLocalized(145, 50, 250, 18, 1150337, RedColor16, false, false); // ADD PET TO BROKER INVENTORY
            AddHtmlLocalized(10, 100, 500, 40, 1150338, GreenColor16, false, false); // Click the button next to a pet to select it. Enter the price you wish to charge into the box below the pet list, then click the "ADD PET" button. 

            m_List = GetList(from);

            int y = 150;
            for (int i = 0; i < m_List.Count; i++)
            {
                int col = index == i ? YellowColor16 : OrangeColor16;
                BaseCreature bc = m_List[i];

                if (bc == null)
                    continue;

                AddButton(10, y, 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(60, y, 200, 18, 1150340, string.Format("{0}\t{1}", bc.Name, PetBrokerEntry.GetOriginalName(bc)), col, false, false); // ~1_NAME~ (~2_type~)
                y += 22;
            }

            AddHtmlLocalized(215, 380, 100, 18, 1150339, OrangeColor16, false, false); // ADD PET
            AddButton(175, 405, 4005, 4007, 501, GumpButtonType.Reply, 0);
            AddBackground(215, 405, 295, 22, 9350);
            AddTextEntry(216, 405, 294, 20, 0, 0, "");

            AddButton(10, 490, 4014, 4016, 500, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 100, 18, 1149777, BlueColor16, false, false); // MAIN MENU
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (info.ButtonID == 0)
                return;

            if (info.ButtonID == 500) // MAIN MENU
            {
                from.SendGump(new PetBrokerGump(m_Broker, from));
                return;
            }
            else if (info.ButtonID == 501) // ADD PET
            {
                if (m_Index >= 0 && m_Index < m_List.Count)
                {
                    if (m_Broker.BankBalance < 0)
                    {
                        from.SendGump(new BazaarInformationGump(1150623, 1150615));
                        return;
                    }

                    TextRelay relay = info.TextEntries[0];
                    int cost = PetBrokerEntry.DefaultPrice;

                    try
                    {
                        cost = Convert.ToInt32(relay.Text);
                    }
                    catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }

                    if (cost > 0)
                    {
                        BaseCreature bc = m_List[m_Index];

                        if (m_Broker.TryAddEntry(bc, from, cost))
                        {
                            from.Stabled.Remove(bc);
                            from.SendGump(new SetPetPricesGump(m_Broker));
                            from.SendLocalizedMessage(1150345, string.Format("{0}\t{1}\t{2}\t{3}", PetBrokerEntry.GetOriginalName(bc), bc.Name, m_Broker.Name, cost)); // Your pet ~1_TYPE~ named ~2_NAME~ has been transferred to the inventory of your animal broker named ~3_SHOP~ with an asking price of ~4_PRICE~.
                        }
                    }
                    else
                        from.SendLocalizedMessage(1150343); // You have entered an invalid price.

                }
                else
                    from.SendLocalizedMessage(1150341); // You did not select a pet.	
            }
            else
                from.SendGump(new SelectPetsGump(m_Broker, from, info.ButtonID - 1));
        }

        public List<BaseCreature> GetList(Mobile from)
        {
            List<BaseCreature> list = new List<BaseCreature>();

            for (int i = 0; i < from.Stabled.Count; ++i)
            {
                BaseCreature pet = from.Stabled[i] as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    if (pet != null)
                        pet.IsStabled = false;

                    from.Stabled.RemoveAt(i);
                    --i;
                    continue;
                }

                list.Add(pet);
            }

            return list;
        }
    }

    public class RemovePetsGump : BaseBazaarGump
    {
        private readonly PetBroker m_Broker;
        private readonly int m_Index;

        public RemovePetsGump(PetBroker broker, Mobile from) : this(broker, from, -1)
        {
        }

        public RemovePetsGump(PetBroker broker, Mobile from, int index)
        {
            m_Broker = broker;
            m_Index = index;

            AddHtmlLocalized(215, 10, 200, 18, 1150311, RedColor16, false, false); // Animal Broker
            AddHtmlLocalized(145, 50, 250, 18, 1150337, RedColor16, false, false); // ADD PET TO BROKER INVENTORY
            AddHtmlLocalized(10, 80, 500, 40, 1150633, GreenColor16, false, false); // Click the button next to a pet to select it, then click the REMOVE PET button below to transfer that pet to your stables.

            m_Broker.CheckInventory();

            int y = 130;
            for (int i = 0; i < broker.BrokerEntries.Count; i++)
            {
                BaseCreature bc = broker.BrokerEntries[i].Pet;
                int col = index == i ? YellowColor16 : OrangeColor16;
                AddButton(10, y, 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, y, 200, 18, 1150340, string.Format("{0}\t{1}", bc.Name, PetBrokerEntry.GetOriginalName(bc)), col, false, false); // ~1_NAME~ (~2_type~)

                y += 20;
            }

            AddHtmlLocalized(215, 405, 150, 18, 1150632, OrangeColor16, false, false); // REMOVE PET
            AddButton(175, 405, 4014, 4016, 501, GumpButtonType.Reply, 0);

            AddButton(10, 490, 4014, 4016, 500, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 100, 18, 1149777, BlueColor16, false, false); // MAIN MENU
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (info.ButtonID == 0)
                return;

            if (info.ButtonID == 500) // MAIN MENU
            {
                from.SendGump(new PetBrokerGump(m_Broker, from));
                return;
            }
            else if (info.ButtonID == 501) // REMOVE PET
            {
                if (m_Index >= 0 && m_Index < m_Broker.BrokerEntries.Count)
                {
                    PetBrokerEntry entry = m_Broker.BrokerEntries[m_Index];

                    if (from.Stabled.Count >= AnimalTrainer.GetMaxStabled(from) || entry.Pet == null)
                        from.SendLocalizedMessage(1150634); // Failed to transfer the selected pet to your stables. Either the pet is no longer in the broker's inventory, or you do not have any available stable slots.
                    else
                    {
                        BaseCreature bc = entry.Pet;
                        m_Broker.RemoveEntry(entry);

                        PetBroker.SendToStables(from, bc);

                        from.SendLocalizedMessage(1150635, string.Format("{0}\t{1}", entry.TypeName, bc.Name)); // Your pet ~1_TYPE~ named ~2_NAME~ has been transferred to the stables.
                        from.SendGump(new PetBrokerGump(m_Broker, from));
                        return;
                    }
                }
                else
                    from.SendLocalizedMessage(1150341); // You did not select a pet.

                from.SendGump(new RemovePetsGump(m_Broker, from, m_Index));
            }
            else
                from.SendGump(new RemovePetsGump(m_Broker, from, info.ButtonID - 1));
        }
    }

    public class SetPetPricesGump : BaseBazaarGump
    {
        private readonly PetBroker m_Broker;
        private int m_Index;

        public SetPetPricesGump(PetBroker broker) : this(broker, -1)
        {
        }

        public SetPetPricesGump(PetBroker broker, int index)
        {
            m_Broker = broker;
            m_Index = index;

            AddHtmlLocalized(215, 10, 200, 18, 1150311, RedColor16, false, false); // Animal Broker

            AddHtmlLocalized(60, 90, 100, 18, 1150347, OrangeColor16, false, false); // NAME
            AddHtmlLocalized(220, 90, 100, 18, 1150348, OrangeColor16, false, false); // TYPE
            AddHtmlLocalized(400, 90, 100, 18, 1150349, OrangeColor16, false, false); // PRICE

            m_Broker.CheckInventory();

            int y = 130;
            for (int i = 0; i < broker.BrokerEntries.Count; i++)
            {
                int col = index == i ? YellowColor : OrangeColor;

                PetBrokerEntry entry = broker.BrokerEntries[i];

                AddHtml(60, y, 200, 18, Color(entry.Pet.Name != null ? entry.Pet.Name : "Unknown", col), false, false);
                AddHtml(220, y, 200, 18, Color(entry.TypeName, col), false, false);
                AddHtml(400, y, 200, 18, Color(FormatAmt(entry.SalePrice), col), false, false);
                AddButton(10, y, 4005, 4007, i + 3, GumpButtonType.Reply, 0);

                y += 22;
            }

            int price = index >= 0 && index < broker.BrokerEntries.Count ? broker.BrokerEntries[index].SalePrice : 0;

            AddHtmlLocalized(215, 380, 150, 18, 1150627, BlueColor16, false, false); // SET PRICE
            AddBackground(215, 405, 295, 22, 9350);
            AddTextEntry(216, 405, 294, 20, 0, 0, FormatAmt(price));
            AddButton(175, 405, 4005, 4007, 500, GumpButtonType.Reply, 0);

            AddButton(10, 490, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 100, 18, 1149777, BlueColor16, false, false); // MAIN MENU
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0: break;
                case 1: // MAIN MENU
                    from.SendGump(new PetBrokerGump(m_Broker, from));
                    break;
                case 500: // SET PRICE
                    {
                        if (m_Index >= 0 && m_Index < m_Broker.BrokerEntries.Count)
                        {
                            PetBrokerEntry entry = m_Broker.BrokerEntries[m_Index];
                            int amount = 0;
                            TextRelay relay = info.TextEntries[0];

                            try
                            {
                                amount = Convert.ToInt32(relay.Text);
                            }
                            catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }

                            if (amount > 0)
                                entry.SalePrice = amount;
                            else
                                from.SendLocalizedMessage(1150343); // You have entered an invalid price.
                        }
                        else
                            from.SendLocalizedMessage(1150341); // You did not select a pet.
                    }
                    from.SendGump(new SetPetPricesGump(m_Broker, -1));
                    break;
                default:
                    int idx = info.ButtonID - 3;
                    if (idx >= 0 && idx < m_Broker.BrokerEntries.Count)
                        m_Index = idx;
                    from.SendGump(new SetPetPricesGump(m_Broker, m_Index));
                    break;
            }
        }
    }

    public class PetInventoryGump : BaseBazaarGump
    {
        private readonly PetBroker m_Broker;
        private readonly List<PetBrokerEntry> m_Entries;

        public PetInventoryGump(PetBroker broker, Mobile from)
        {
            m_Broker = broker;
            m_Entries = broker.BrokerEntries;

            AddHtmlLocalized(10, 10, 500, 18, 1114513, "#1150311", RedColor16, false, false);  // Animal Broker

            if (m_Broker.Plot.ShopName != null && m_Broker.Plot.ShopName.Length > 0)
                AddHtml(10, 37, 500, 18, Color(FormatStallName(m_Broker.Plot.ShopName), BlueColor), false, false);
            else
            {
                AddHtmlLocalized(10, 37, 500, 18, 1114513, "#1150314", BlueColor16, false, false); // This Shop Has No Name
            }

            AddHtmlLocalized(10, 55, 240, 18, 1114514, "#1150313", BlueColor16, false, false); // Proprietor:
            AddHtml(260, 55, 250, 18, Color(string.Format("{0}", broker.Name), BlueColor), false, false);

            if (m_Entries.Count != 0)
            {
                AddHtmlLocalized(10, 91, 500, 18, 1114513, "#1150346", GreenColor16, false, false); // PETS FOR SALE
                AddHtmlLocalized(10, 118, 500, 72, 1114513, "#1150352", GreenColor16, false, false); // LORE: See the animal's

                AddHtmlLocalized(10, 199, 52, 18, 1150351, OrangeColor16, false, false); // LORE
                AddHtmlLocalized(68, 199, 52, 18, 1150353, OrangeColor16, false, false); // VIEW
                AddHtmlLocalized(126, 199, 104, 18, 1150347, OrangeColor16, false, false); // NAME
                AddHtmlLocalized(236, 199, 104, 18, 1150348, OrangeColor16, false, false); // TYPE
                AddHtmlLocalized(346, 199, 104, 18, 1114514, "#1150349", OrangeColor16, false, false); // PRICE
                AddHtmlLocalized(456, 199, 52, 18, 1150350, OrangeColor16, false, false); // BUY

                int y = 219;
                for (int i = 0; i < m_Entries.Count; i++)
                {
                    PetBrokerEntry entry = m_Entries[i];

                    if (entry == null || entry.Pet == null)
                    {
                        continue;
                    }

                    AddButton(10, y + (i * 20), 4011, 4013, 100 + i, GumpButtonType.Reply, 0);
                    AddButton(68, y + (i * 20), 4008, 4010, 200 + i, GumpButtonType.Reply, 0);

                    AddHtml(126, y + (i * 20), 104, 14, Color(entry.Pet.Name, BlueColor), false, false);
                    AddHtml(236, y + (i * 20), 104, 20, Color(entry.TypeName, BlueColor), false, false);
                    AddHtml(346, y + (i * 20), 104, 20, Color(AlignRight(FormatAmt(entry.SalePrice)), GreenColor), false, false);

                    AddButton(456, y + (i * 20), 4014, 4016, 300 + i, GumpButtonType.Reply, 0);
                }
            }
            else
            {
                AddHtmlLocalized(10, 127, 500, 534, 1114513, "#1150336", OrangeColor16, false, false); // The animal broker has no pets in its inventory.

                AddButton(10, 490, 0xFAE, 0xFAF, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 490, 210, 20, 1149777, BlueColor16, false, false); // MAIN MENU
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (info.ButtonID == 1) // Main Menu
            {
                from.SendGump(new PetInventoryGump(m_Broker, from));
            }
            else if (info.ButtonID < 200) // LORE
            {
                int id = info.ButtonID - 100;

                if (id >= 0 && id < m_Entries.Count)
                {
                    PetBrokerEntry entry = m_Entries[id];

                    if (entry != null && entry.Pet != null && m_Broker.BrokerEntries.Contains(entry))
                    {
                        from.SendGump(new PetInventoryGump(m_Broker, from));

                        if (from is PlayerMobile)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                            {
                                BaseGump.SendGump(new NewAnimalLoreGump((PlayerMobile)from, entry.Pet));
                            });
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1150368); // The selected animal is not available.
                    }
                }
            }
            else if (info.ButtonID < 300) // VIEW
            {
                int id = info.ButtonID - 200;

                if (id >= 0 && id < m_Entries.Count)
                {
                    PetBrokerEntry entry = m_Entries[id];

                    if (entry != null && entry.Pet != null && m_Broker.BrokerEntries.Contains(entry) && entry.Pet.IsStabled)
                    {
                        BaseCreature pet = entry.Pet;

                        pet.Blessed = true;
                        pet.SetControlMaster(m_Broker);
                        pet.ControlTarget = m_Broker;
                        pet.ControlOrder = OrderType.None;
                        pet.MoveToWorld(m_Broker.Location, m_Broker.Map);
                        pet.IsStabled = false;
                        pet.Home = pet.Location;
                        pet.RangeHome = 2;
                        pet.Loyalty = BaseCreature.MaxLoyalty;

                        PetBroker.AddToViewTimer(pet);
                        from.SendLocalizedMessage(1150369, string.Format("{0}\t{1}", entry.TypeName, pet.Name)); // The ~1_TYPE~ named "~2_NAME~" is now in the animal broker's pen for inspection.
                    }
                    else
                        from.SendLocalizedMessage(1150368); // The selected animal is not available.
                }

                from.SendGump(new PetInventoryGump(m_Broker, from));
            }
            else // BUY
            {
                int id = info.ButtonID - 300;

                if (id >= 0 && id < m_Entries.Count)
                {
                    PetBrokerEntry entry = m_Entries[id];

                    if (entry != null && entry.Pet != null && m_Broker.BrokerEntries.Contains(entry))
                        from.SendGump(new ConfirmBuyPetGump(m_Broker, entry));
                }
            }
        }
    }

    public class ConfirmBuyPetGump : BaseBazaarGump
    {
        private readonly PetBroker m_Broker;
        private readonly PetBrokerEntry m_Entry;

        public ConfirmBuyPetGump(PetBroker broker, PetBrokerEntry entry)
        {
            m_Broker = broker;
            m_Entry = entry;

            AddHtmlLocalized(10, 10, 500, 18, 1114513, "#1150311", RedColor16, false, false);  // Animal Broker

            if (m_Broker.Plot.ShopName != null && m_Broker.Plot.ShopName.Length > 0)
                AddHtml(10, 37, 500, 18, Color(FormatStallName(m_Broker.Plot.ShopName), BlueColor), false, false);
            else
            {
                AddHtmlLocalized(10, 37, 500, 18, 1114513, "#1150314", BlueColor16, false, false); // This Shop Has No Name
            }

            AddHtmlLocalized(10, 55, 240, 18, 1114514, "#1150313", BlueColor16, false, false); // Proprietor:
            AddHtml(260, 55, 250, 18, Color(string.Format("{0}", broker.Name), BlueColor), false, false);

            AddHtmlLocalized(10, 91, 500, 18, 1114513, "#1150375", GreenColor16, false, false); // PURCHASE PET
            AddHtmlLocalized(10, 118, 500, 72, 1114513, "#1150370", GreenColor16, false, false); // Please confirm your purchase order below, and click "ACCEPT" if you wish to purchase this animal.

            AddHtmlLocalized(10, 235, 245, 18, 1114514, "#1150372", OrangeColor16, false, false); // Animal Name:
            AddHtmlLocalized(10, 255, 245, 18, 1114514, "#1150371", OrangeColor16, false, false); // Animal Type:            
            AddHtmlLocalized(10, 275, 245, 18, 1114514, "#1150373", OrangeColor16, false, false); // Sale Price:

            AddHtml(265, 235, 245, 18, Color(entry.Pet.Name, BlueColor), false, false);
            AddHtml(265, 255, 245, 18, Color(entry.TypeName, BlueColor), false, false);
            AddHtml(265, 275, 245, 18, Color(FormatAmt(entry.SalePrice), BlueColor), false, false);

            /*int itemID = ShrinkTable.Lookup(entry.Pet);
            //if (entry.Pet is WildTiger)
            //    itemID = 0x9844;

			AddItem(240, 250, itemID);*/

            AddHtmlLocalized(265, 295, 245, 22, 1150374, OrangeColor16, false, false); // ACCEPT
            AddButton(225, 295, 4005, 4007, 1, GumpButtonType.Reply, 0);

            AddButton(10, 490, 0xFAE, 0xFAF, 2, GumpButtonType.Reply, 0);
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
                    {
                        int cliloc = m_Broker.TryBuyPet(from, m_Entry);

                        if (cliloc != 0)
                        {
                            from.SendGump(new PurchasePetGump(m_Broker, cliloc));
                        }

                        break;
                    }
                case 2: //MAIN MENU
                    from.SendGump(new PetInventoryGump(m_Broker, from));
                    break;
            }
        }
    }

    public class PurchasePetGump : BaseBazaarGump
    {
        private readonly PetBroker m_Broker;

        public PurchasePetGump(PetBroker broker, int cliloc)
        {
            m_Broker = broker;

            AddHtmlLocalized(10, 10, 500, 18, 1114513, "#1150311", RedColor16, false, false);  // Animal Broker

            if (m_Broker.Plot.ShopName != null && m_Broker.Plot.ShopName.Length > 0)
            {
                AddHtml(10, 37, 500, 18, Color(FormatStallName(m_Broker.Plot.ShopName), BlueColor), false, false);
            }
            else
            {
                AddHtmlLocalized(10, 37, 500, 18, 1114513, "#1150314", BlueColor16, false, false); // This Shop Has No Name
            }

            AddHtmlLocalized(10, 55, 240, 18, 1114514, "#1150313", BlueColor16, false, false); // Proprietor:
            AddHtml(260, 55, 250, 18, Color(string.Format("{0}", broker.Name), BlueColor), false, false);

            AddHtmlLocalized(10, 127, 500, 534, 1114513, string.Format("#{0}", cliloc), OrangeColor16, false, false);

            AddButton(10, 490, 0xFAE, 0xFAF, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 210, 20, 1149777, BlueColor16, false, false); // MAIN MENU
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                default:
                case 0: break;
                case 1: //MAIN MENU
                    from.SendGump(new PetInventoryGump(m_Broker, from));
                    break;
            }
        }
    }
}
