using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Factions
{
    public class SheriffGump : FactionGump
    {
        private readonly PlayerMobile m_From;
        private readonly Faction m_Faction;
        private readonly Town m_Town;

        private void CenterItem(int itemID, int x, int y, int w, int h)
        {
            Rectangle2D rc = ItemBounds.Table[itemID];
            this.AddItem(x + ((w - rc.Width) / 2) - rc.X, y + ((h - rc.Height) / 2) - rc.Y, itemID);
        }

        public SheriffGump(PlayerMobile from, Faction faction, Town town)
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

            this.AddHtmlLocalized(20, 30, 260, 25, 1011431, false, false); // Sheriff

            this.AddHtmlLocalized(55, 90, 200, 25, 1011494, false, false); // HIRE GUARDS
            this.AddButton(20, 90, 4005, 4007, 0, GumpButtonType.Page, 3);

            this.AddHtmlLocalized(55, 120, 200, 25, 1011495, false, false); // VIEW FINANCES
            this.AddButton(20, 120, 4005, 4007, 0, GumpButtonType.Page, 2);

            this.AddHtmlLocalized(55, 360, 200, 25, 1011441, false, false); // Exit
            this.AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Reply, 0);
            #endregion

            #region Finances
            this.AddPage(2);

            int financeUpkeep = town.FinanceUpkeep;
            int sheriffUpkeep = town.SheriffUpkeep;
            int dailyIncome = town.DailyIncome;
            int netCashFlow = town.NetCashFlow;

            this.AddHtmlLocalized(20, 30, 300, 25, 1011524, false, false); // FINANCE STATEMENT
			
            this.AddHtmlLocalized(20, 80, 300, 25, 1011538, false, false); // Current total money for town : 
            this.AddLabel(20, 100, 0x44, town.Silver.ToString("N0")); // NOTE: Added 'N0'

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

            #region Hire Guards
            this.AddPage(3);

            this.AddHtmlLocalized(20, 30, 300, 25, 1011494, false, false); // HIRE GUARDS

            List<GuardList> guardLists = town.GuardLists;

            for (int i = 0; i < guardLists.Count; ++i)
            {
                GuardList guardList = guardLists[i];
                int y = 90 + (i * 60);

                this.AddButton(20, y, 4005, 4007, 0, GumpButtonType.Page, 4 + i);
                this.CenterItem(guardList.Definition.ItemID, 50, y - 20, 70, 60);
                this.AddHtmlText(120, y, 200, 25, guardList.Definition.Header, false, false);
            }

            this.AddHtmlLocalized(55, 360, 200, 25, 1011067, false, false); // Previous page
            this.AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Page, 1);
            #endregion

            #region Guard Pages
            for (int i = 0; i < guardLists.Count; ++i)
            {
                GuardList guardList = guardLists[i];

                this.AddPage(4 + i);

                this.AddHtmlText(90, 30, 300, 25, guardList.Definition.Header, false, false);
                this.CenterItem(guardList.Definition.ItemID, 10, 10, 80, 80);

                this.AddHtmlLocalized(20, 90, 200, 25, 1011514, false, false); // You have : 
                this.AddLabel(230, 90, 0x26, guardList.Guards.Count.ToString());

                this.AddHtmlLocalized(20, 120, 200, 25, 1011515, false, false); // Maximum : 
                this.AddLabel(230, 120, 0x12A, guardList.Definition.Maximum.ToString());

                this.AddHtmlLocalized(20, 150, 200, 25, 1011516, false, false); // Cost : 
                this.AddLabel(230, 150, 0x44, guardList.Definition.Price.ToString("N0")); // NOTE: Added 'N0'

                this.AddHtmlLocalized(20, 180, 200, 25, 1011517, false, false); // Daily Pay :
                this.AddLabel(230, 180, 0x37, guardList.Definition.Upkeep.ToString("N0")); // NOTE: Added 'N0'

                this.AddHtmlLocalized(20, 210, 200, 25, 1011518, false, false); // Current Silver : 
                this.AddLabel(230, 210, 0x44, town.Silver.ToString("N0")); // NOTE: Added 'N0'

                this.AddHtmlLocalized(20, 240, 200, 25, 1011519, false, false); // Current Payroll : 
                this.AddLabel(230, 240, 0x44, sheriffUpkeep.ToString("N0")); // NOTE: Added 'N0'

                this.AddHtmlText(55, 300, 200, 25, guardList.Definition.Label, false, false);
                this.AddButton(20, 300, 4005, 4007, 1 + i, GumpButtonType.Reply, 0);

                this.AddHtmlLocalized(55, 360, 200, 25, 1011067, false, false); // Previous page
                this.AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Page, 3);
            }
            #endregion
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (!this.m_Town.IsSheriff(this.m_From) || this.m_Town.Owner != this.m_Faction)
            {
                this.m_From.SendLocalizedMessage(1010339); // You no longer control this city
                return;
            }

            int index = info.ButtonID - 1;

            if (index >= 0 && index < this.m_Town.GuardLists.Count)
            {
                GuardList guardList = this.m_Town.GuardLists[index];
                Town town = Town.FromRegion(this.m_From.Region);

                if (Town.FromRegion(this.m_From.Region) != this.m_Town)
                {
                    this.m_From.SendLocalizedMessage(1010305); // You must be in your controlled city to buy Items
                }
                else if (guardList.Guards.Count >= guardList.Definition.Maximum)
                {
                    this.m_From.SendLocalizedMessage(1010306); // You currently have too many of this enhancement type to place another
                }
                else if (BaseBoat.FindBoatAt(this.m_From.Location, this.m_From.Map) != null)
                {
                    this.m_From.SendMessage("You cannot place a guard here");
                }
                else if (this.m_Town.Silver >= guardList.Definition.Price)
                {
                    BaseFactionGuard guard = guardList.Construct();

                    if (guard != null)
                    {
                        guard.Faction = this.m_Faction;
                        guard.Town = this.m_Town;

                        this.m_Town.Silver -= guardList.Definition.Price;

                        guard.MoveToWorld(this.m_From.Location, this.m_From.Map);
                        guard.Home = guard.Location;
                    }
                }
            }
        }
    }
}