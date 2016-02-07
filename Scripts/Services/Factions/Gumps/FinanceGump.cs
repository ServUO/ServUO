using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Factions
{
    public class FinanceGump : FactionGump
    {
        private readonly PlayerMobile m_From;
        private readonly Faction m_Faction;
        private readonly Town m_Town;

        private static readonly int[] m_PriceOffsets = new int[]
        {
            -30, -25, -20, -15, -10, -5,
            +50, +100, +150, +200, +250, +300
        };

        public override int ButtonTypes
        {
            get
            {
                return 2;
            }
        }

        public FinanceGump(PlayerMobile from, Faction faction, Town town)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Faction = faction;
            this.m_Town = town;

            this.AddPage(0);

            this.AddBackground(0, 0, 320, 410, 5054);
            this.AddBackground(10, 10, 300, 390, 3000);

            #region General
            this.AddPage(1);

            this.AddHtmlLocalized(20, 30, 260, 25, 1011541, false, false); // FINANCE MINISTER

            this.AddHtmlLocalized(55, 90, 200, 25, 1011539, false, false); // CHANGE PRICES
            this.AddButton(20, 90, 4005, 4007, 0, GumpButtonType.Page, 2);

            this.AddHtmlLocalized(55, 120, 200, 25, 1011540, false, false); // BUY SHOPKEEPERS	
            this.AddButton(20, 120, 4005, 4007, 0, GumpButtonType.Page, 3);

            this.AddHtmlLocalized(55, 150, 200, 25, 1011495, false, false); // VIEW FINANCES
            this.AddButton(20, 150, 4005, 4007, 0, GumpButtonType.Page, 4);

            this.AddHtmlLocalized(55, 360, 200, 25, 1011441, false, false); // EXIT
            this.AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Reply, 0);
            #endregion

            #region Change Prices
            this.AddPage(2);

            this.AddHtmlLocalized(20, 30, 200, 25, 1011539, false, false); // CHANGE PRICES

            for (int i = 0; i < m_PriceOffsets.Length; ++i)
            {
                int ofs = m_PriceOffsets[i];

                int x = 20 + ((i / 6) * 150);
                int y = 90 + ((i % 6) * 30);

                this.AddRadio(x, y, 208, 209, (town.Tax == ofs), i + 1);

                if (ofs < 0)
                    this.AddLabel(x + 35, y, 0x26, String.Concat("- ", -ofs, "%"));
                else
                    this.AddLabel(x + 35, y, 0x12A, String.Concat("+ ", ofs, "%"));
            }

            this.AddRadio(20, 270, 208, 209, (town.Tax == 0), 0);
            this.AddHtmlLocalized(55, 270, 90, 25, 1011542, false, false); // normal

            this.AddHtmlLocalized(55, 330, 200, 25, 1011509, false, false); // Set Prices
            this.AddButton(20, 330, 4005, 4007, this.ToButtonID(0, 0), GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(55, 360, 200, 25, 1011067, false, false); // Previous page
            this.AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Page, 1);
            #endregion

            #region Buy Shopkeepers
            this.AddPage(3);

            this.AddHtmlLocalized(20, 30, 200, 25, 1011540, false, false); // BUY SHOPKEEPERS

            List<VendorList> vendorLists = town.VendorLists;

            for (int i = 0; i < vendorLists.Count; ++i)
            {
                VendorList list = vendorLists[i];

                this.AddButton(20, 90 + (i * 40), 4005, 4007, 0, GumpButtonType.Page, 5 + i);
                this.AddItem(55, 90 + (i * 40), list.Definition.ItemID);
                this.AddHtmlText(100, 90 + (i * 40), 200, 25, list.Definition.Label, false, false);
            }

            this.AddHtmlLocalized(55, 360, 200, 25, 1011067, false, false);	//	Previous page
            this.AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Page, 1);
            #endregion

            #region View Finances
            this.AddPage(4);

            int financeUpkeep = town.FinanceUpkeep;
            int sheriffUpkeep = town.SheriffUpkeep;
            int dailyIncome = town.DailyIncome;
            int netCashFlow = town.NetCashFlow;

            this.AddHtmlLocalized(20, 30, 300, 25, 1011524, false, false); // FINANCE STATEMENT
	
            this.AddHtmlLocalized(20, 80, 300, 25, 1011538, false, false); // Current total money for town : 
            this.AddLabel(20, 100, 0x44, town.Silver.ToString());

            this.AddHtmlLocalized(20, 130, 300, 25, 1011520, false, false); // Finance Minister Upkeep : 
            this.AddLabel(20, 150, 0x44, financeUpkeep.ToString("N0")); // NOTE: Added 'N0'

            this.AddHtmlLocalized(20, 180, 300, 25, 1011521, false, false); // Sheriff Upkeep : 
            this.AddLabel(20, 200, 0x44, sheriffUpkeep.ToString("N0")); // NOTE: Added 'N0'

            this.AddHtmlLocalized(20, 230, 300, 25, 1011522, false, false); // Town Income : 
            this.AddLabel(20, 250, 0x44, dailyIncome.ToString("N0")); // NOTE: Added 'N0'

            this.AddHtmlLocalized(20, 280, 300, 25, 1011523, false, false); // Net Cash flow per day : 
            this.AddLabel(20, 300, 0x44, netCashFlow.ToString("N0")); // NOTE: Added 'N0'

            this.AddHtmlLocalized(55, 360, 200, 25, 1011067, false, false); // Previous page
            this.AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Page, 1);
            #endregion

            #region Shopkeeper Pages
            for (int i = 0; i < vendorLists.Count; ++i)
            {
                VendorList vendorList = vendorLists[i];

                this.AddPage(5 + i);

                this.AddHtmlText(60, 30, 300, 25, vendorList.Definition.Header, false, false);
                this.AddItem(20, 30, vendorList.Definition.ItemID);

                this.AddHtmlLocalized(20, 90, 200, 25, 1011514, false, false); // You have : 
                this.AddLabel(230, 90, 0x26, vendorList.Vendors.Count.ToString());

                this.AddHtmlLocalized(20, 120, 200, 25, 1011515, false, false); // Maximum : 
                this.AddLabel(230, 120, 0x256, vendorList.Definition.Maximum.ToString());

                this.AddHtmlLocalized(20, 150, 200, 25, 1011516, false, false); // Cost :
                this.AddLabel(230, 150, 0x44, vendorList.Definition.Price.ToString("N0")); // NOTE: Added 'N0'

                this.AddHtmlLocalized(20, 180, 200, 25, 1011517, false, false); // Daily Pay :
                this.AddLabel(230, 180, 0x37, vendorList.Definition.Upkeep.ToString("N0")); // NOTE: Added 'N0'

                this.AddHtmlLocalized(20, 210, 200, 25, 1011518, false, false); // Current Silver :
                this.AddLabel(230, 210, 0x44, town.Silver.ToString("N0")); // NOTE: Added 'N0'

                this.AddHtmlLocalized(20, 240, 200, 25, 1011519, false, false); // Current Payroll :
                this.AddLabel(230, 240, 0x44, financeUpkeep.ToString("N0")); // NOTE: Added 'N0'

                this.AddHtmlText(55, 300, 200, 25, vendorList.Definition.Label, false, false);
                if (town.Silver >= vendorList.Definition.Price)
                    this.AddButton(20, 300, 4005, 4007, this.ToButtonID(1, i), GumpButtonType.Reply, 0);
                else
                    this.AddImage(20, 300, 4020);

                this.AddHtmlLocalized(55, 360, 200, 25, 1011067, false, false); // Previous page
                this.AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Page, 3);
            }
            #endregion
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (!this.m_Town.IsFinance(this.m_From) || this.m_Town.Owner != this.m_Faction)
            {
                this.m_From.SendLocalizedMessage(1010339); // You no longer control this city
                return;
            }

            int type, index;

            if (!this.FromButtonID(info.ButtonID, out type, out index))
                return;

            switch ( type )
            {
                case 0: // general
                    {
                        switch ( index )
                        {
                            case 0: // set price
                                {
                                    int[] switches = info.Switches;

                                    if (switches.Length == 0)
                                        break;

                                    int opt = switches[0];
                                    int newTax = 0;

                                    if (opt >= 1 && opt <= m_PriceOffsets.Length)
                                        newTax = m_PriceOffsets[opt - 1];

                                    if (this.m_Town.Tax == newTax)
                                        break;

                                    if (this.m_From.IsPlayer() && !this.m_Town.TaxChangeReady)
                                    {
                                        TimeSpan remaining = DateTime.UtcNow - (this.m_Town.LastTaxChange + Town.TaxChangePeriod);

                                        if (remaining.TotalMinutes < 4)
                                            this.m_From.SendLocalizedMessage(1042165); // You must wait a short while before changing prices again.
                                        else if (remaining.TotalMinutes < 10)
                                            this.m_From.SendLocalizedMessage(1042166); // You must wait several minutes before changing prices again.
                                        else if (remaining.TotalHours < 1)
                                            this.m_From.SendLocalizedMessage(1042167); // You must wait up to an hour before changing prices again.
                                        else if (remaining.TotalHours < 4)
                                            this.m_From.SendLocalizedMessage(1042168); // You must wait a few hours before changing prices again.
                                        else 
                                            this.m_From.SendLocalizedMessage(1042169); // You must wait several hours before changing prices again.
                                    }
                                    else
                                    {
                                        this.m_Town.Tax = newTax;

                                        if (this.m_From.IsPlayer())
                                            this.m_Town.LastTaxChange = DateTime.UtcNow;
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case 1: // make vendor
                    {
                        List<VendorList> vendorLists = this.m_Town.VendorLists;

                        if (index >= 0 && index < vendorLists.Count)
                        {
                            VendorList vendorList = vendorLists[index];

                            Town town = Town.FromRegion(this.m_From.Region);

                            if (Town.FromRegion(this.m_From.Region) != this.m_Town)
                            {
                                this.m_From.SendLocalizedMessage(1010305); // You must be in your controlled city to buy Items
                            }
                            else if (vendorList.Vendors.Count >= vendorList.Definition.Maximum)
                            {
                                this.m_From.SendLocalizedMessage(1010306); // You currently have too many of this enhancement type to place another
                            }
                            else if (BaseBoat.FindBoatAt(this.m_From.Location, this.m_From.Map) != null)
                            {
                                this.m_From.SendMessage("You cannot place a vendor here");
                            }
                            else if (this.m_Town.Silver >= vendorList.Definition.Price)
                            {
                                BaseFactionVendor vendor = vendorList.Construct(this.m_Town, this.m_Faction);

                                if (vendor != null)
                                {
                                    this.m_Town.Silver -= vendorList.Definition.Price;

                                    vendor.MoveToWorld(this.m_From.Location, this.m_From.Map);
                                    vendor.Home = vendor.Location;
                                }
                            }
                        }

                        break;
                    }
            }
        }
    }
}