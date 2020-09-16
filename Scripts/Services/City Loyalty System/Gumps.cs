using Server.Engines.Points;
using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.CityLoyalty
{
    public class BaseCityGump : BaseGump
    {
        public CityLoyaltySystem Citizenship { get; set; }

        public BaseCityGump(PlayerMobile pm) : base(pm, 120, 120)
        {
            Citizenship = CityLoyaltySystem.GetCitizenship(User, false);

            pm.CloseGump(typeof(BaseCityGump));
        }

        public override void AddGumpLayout()
        {
            AddHtmlLocalized(0, 7, 345, 20, 1154645, "#1152190", 0, false, false); // City Loyalty

            AddImage(0, 0, 8000);
            AddImage(20, 37, 8001);
            AddImage(20, 107, 8002);
            AddImage(20, 177, 8001);
            AddImage(20, 247, 8002);
            AddImage(20, 317, 8001);
            AddImage(20, 387, 8003);

            AddHtmlLocalized(65, 395, 200, 16, 1152189, false, false); // Loyalty Ratings
            AddButton(50, 400, 2103, 2104, 500, GumpButtonType.Reply, 0);

            AddHtmlLocalized(175, 395, 200, 16, 1157159, false, false); // Toggle Gain Message
            AddButton(160, 400, 2103, 2104, 501, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 500)
                User.SendGump(new LoyaltyRatingGump(User));

            if (info.ButtonID == 501)
            {
                if (Citizenship != null)
                {
                    CityLoyaltyEntry entry = Citizenship.GetPlayerEntry<CityLoyaltyEntry>(User);

                    if (entry != null)
                    {
                        if (entry.ShowGainMessage)
                        {
                            entry.ShowGainMessage = false;
                            User.SendLocalizedMessage(1157160); //You will no longer receive City Loyalty gain messages.
                        }
                        else
                        {
                            entry.ShowGainMessage = true;
                            User.SendLocalizedMessage(1157161); //You will now receive City Loyalty gain messages.
                        }
                    }
                }

                Refresh();
            }
        }
    }

    public class CityLoyaltyGump : BaseCityGump
    {
        public CityLoyaltyGump(PlayerMobile pm) : base(pm)
        {
        }

        public override void AddGumpLayout()
        {
            if (!CityLoyaltySystem.Enabled || CityLoyaltySystem.Cities == null)
                return;

            base.AddGumpLayout();
            int y = 40;

            for (int i = 0; i < CityLoyaltySystem.Cities.Count; i++)
            {
                CityLoyaltySystem city = CityLoyaltySystem.Cities[i];

                if (city.CanUtilize && Citizenship == null && CityLoyaltySystem.CanAddCitizen(User))
                    AddButton(30, y + 3, 2103, 2104, 100 + i, GumpButtonType.Reply, 0);

                AddHtmlLocalized(50, y, 200, 16, CityLoyaltySystem.CityLocalization(city.City), false, false);
                AddHtmlLocalized(200, y, 200, 16, CityLoyaltySystem.RatingLocalization(city.GetLoyaltyRating(User)), false, false);

                y += 20;
            }

            y += 20;

            AddHtmlLocalized(70, y, 250, 16, 1152883, false, false); // Citizenship:
            AddHtmlLocalized(200, y, 100, 16, Citizenship != null ? CityLoyaltySystem.CityLocalization(Citizenship.City) : 1152884, false, false);

            y += 40;

            if (!CityLoyaltySystem.IsSetup())
            {
                AddHtml(70, y, 250, 60, "City Loyalty System has not been enabled by your server owner yet.", false, false);
            }
            else if (Citizenship != null)
            {
                AddHtmlLocalized(115, y - 5, 150, 16, 1152890, false, false); // Citizenship Titles
                AddButton(100, y, 2103, 2104, 1, GumpButtonType.Reply, 0);

                y += 20;

                AddHtmlLocalized(115, y - 5, 150, 16, 1152888, false, false); // Renounce Citizenship
                AddButton(100, y, 2103, 2104, 2, GumpButtonType.Reply, 0);
            }
            else
            {
                if (CityLoyaltySystem.CanAddCitizen(User))
                {
                    AddHtmlLocalized(30, y, 280, 90, 1152885, false, false);
                    /*Click the gem next to the name of a city to declare your 
                     * citizenship. You may renounce citizenship, but afterwards
                     * you may not declare new citizenship for one week.*/
                }
                else
                {
                    AddHtmlLocalized(30, y, 285, 80, 1152886, CityLoyaltySystem.NextJoinCity(User).ToString(), 0, false, false);
                    /*You recently renounced citizenship, so you must wait ~1_COUNT~ 
                     * more days before you may declare citizenship again.*/
                }
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            base.OnResponse(info);

            if (!CityLoyaltySystem.IsSetup())
                return;

            switch (info.ButtonID)
            {
                case 0: break;
                case 1:
                    SendGump(new CityTitlesGump(User));
                    break;
                case 2:
                    SendGump(new RenounceCitizenshipGump(User));
                    break;
                case 3:
                default:
                    int id = info.ButtonID - 100;
                    if (id >= 0 && id < CityLoyaltySystem.Cities.Count)
                    {
                        if (Citizenship == null)
                            SendGump(new DeclareCitizenshipGump(CityLoyaltySystem.Cities[id], User));
                    }
                    break;
            }
        }
    }

    public class DeclareCitizenshipGump : BaseCityGump
    {
        public CityLoyaltySystem City { get; set; }

        public DeclareCitizenshipGump(CityLoyaltySystem toDeclare, PlayerMobile pm) : base(pm)
        {
            City = toDeclare;
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            if (City == null)
                return;

            AddHtmlLocalized(30, 40, 285, 200, 1152891, string.Format("#{0}", CityLoyaltySystem.GetCityLocalization(City.City).ToString()), 1, false, true);
            /*If you choose to declare citizenship with ~1_CITY~, you will be granted the "Citizen" title.
             * If your loyalty rating to ~1_CITY~ is high enough, you will be able to receive titles with
             * that city.<br><br>You may only have citizenship in one city at a time. You may renounce 
             * citizenship at any time. After renouncing citizenship, you must wait one week before declaring 
             * citizenship again.<br><br>Note that gaining city titles requires positive loyalty with your city,
             * and you will be required to donate funds to the city's coffers.<br><br>Are you sure you wish to
             * declare citizenship with ~1_CITY~?*/

            AddHtmlLocalized(115, 280, 150, 16, 1152892, false, false); // Declare Citizenship
            AddButton(100, 285, 2103, 2104, 1, GumpButtonType.Reply, 0);

            AddHtmlLocalized(115, 300, 150, 16, 1152889, false, false); // Cancel
            AddButton(100, 305, 2103, 2104, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            base.OnResponse(info);

            if (info.ButtonID == 1 && City != null)
            {
                if (CityLoyaltySystem.CanAddCitizen(User))
                {
                    City.DeclareCitizen(User);
                }
            }
            else if (info.ButtonID == 2)
                SendGump(new CityLoyaltyGump(User));
        }
    }

    public class RenounceCitizenshipGump : BaseCityGump
    {
        public RenounceCitizenshipGump(PlayerMobile pm)
            : base(pm)
        {
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            if (Citizenship == null)
                return;

            AddHtmlLocalized(45, 40, 250, 16, 1152883, false, false); // Citizenship:
            AddHtmlLocalized(200, 40, 100, 16, CityLoyaltySystem.CityLocalization(Citizenship.City), false, false);

            AddHtmlLocalized(30, 70, 280, 200, 1152887, string.Format("#{0}", CityLoyaltySystem.GetCityLocalization(Citizenship.City).ToString()), 1, false, true);
            /*If you renounce your citizenship, you will be stripped of all titles gained with your current
             * city, and you must wait 7 days before declaring citizenship again.<br><br>Are you sure you wish
             * to renounce your citizenship with ~1_CITY~?*/

            AddHtmlLocalized(115, 280, 150, 16, 1152888, false, false); // Renounce Citizenship
            AddButton(100, 285, 2103, 2104, 1, GumpButtonType.Reply, 0);

            AddHtmlLocalized(115, 300, 150, 16, 1152889, false, false); // Cancel
            AddButton(100, 305, 2103, 2104, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            base.OnResponse(info);

            if (info.ButtonID == 1 && Citizenship != null)
            {
                if (CityLoyaltySystem.CanAddCitizen(User))
                {
                    Citizenship.RenounceCitizenship(User);
                    User.SendMessage("You renounce your citizenship to {0}!", Citizenship.Definition.Name);
                }
            }
            else if (info.ButtonID == 2)
                SendGump(new CityLoyaltyGump(User));
        }
    }

    public class CityTitlesGump : BaseCityGump
    {
        public CityTitlesGump(PlayerMobile pm) : base(pm)
        {
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            AddHtmlLocalized(60, 40, 200, 16, 1152894, false, false); // Your Titles
            int y = 60;

            foreach (int i in Enum.GetValues(typeof(CityTitle)))
            {
                CityTitle title = (CityTitle)i;

                if (title == CityTitle.None)
                    continue;

                if (Citizenship.HasTitle(User, title))
                {
                    AddHtmlLocalized(65, y, 300, 16, CityLoyaltySystem.GetTitleLocalization(User, title, Citizenship.City), false, false);
                    y += 20;
                }
            }

            y += 20;
            AddHtmlLocalized(60, y, 250, 16, 1152895, false, false); //Available Titles:
            y += 20;

            foreach (int i in Enum.GetValues(typeof(CityTitle)))
            {
                CityTitle title = (CityTitle)i;

                if (title == CityTitle.None)
                    continue;

                if (!Citizenship.HasTitle(User, title))
                {
                    AddButton(60, y + 5, 2103, 2104, i + 1, GumpButtonType.Reply, 0);

                    AddHtmlLocalized(75, y, 300, 16, CityLoyaltySystem.GetTitleLocalization(User, title, Citizenship.City), false, false);
                    y += 20;
                }
            }

            y += 40;
            AddHtmlLocalized(30, y, 285, 60, 1152896, false, false);
            /*Click the gem next to an available title for more information about obtaining that title.*/
        }

        public override void OnResponse(RelayInfo info)
        {
            base.OnResponse(info);

            if (info.ButtonID > 0 && info.ButtonID < 500)
            {
                CityTitle t = (CityTitle)info.ButtonID - 1;

                if (!Citizenship.HasTitle(User, t))
                    SendGump(new CityTitlesInfoGump(User, t));
            }
        }
    }

    public class CityTitlesInfoGump : BaseCityGump
    {
        public CityTitle Title { get; private set; }

        public CityTitlesInfoGump(PlayerMobile pm, CityTitle title) : base(pm)
        {
            Title = title;
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            int gold = Citizenship.GetTitleCost(Title);
            LoyaltyRating rating = Citizenship.GetMinimumRating(Title);

            AddHtmlLocalized(60, 40, 300, 20, 1152901, false, false); // Obtain Title
            AddHtmlLocalized(75, 60, 200, 16, CityLoyaltySystem.GetTitleLocalization(User, Title, Citizenship.City), false, false);

            AddHtmlLocalized(60, 120, 200, 16, 1152899, false, false); // Loyalty Required:
            AddHtmlLocalized(60, 140, 200, 16, 1152900, false, false); // Donation Required:

            AddHtmlLocalized(200, 120, 150, 16, CityLoyaltySystem.RatingLocalization(Citizenship.GetMinimumRating(Title)), false, false);
            AddHtml(200, 140, 150, 16, Citizenship.GetTitleCost(Title).ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US")), false, false);

            if (gold > Banker.GetBalance(User))
            {
                AddHtmlLocalized(30, 180, 280, 80, 1152902, false, false);
                // You do not have enough funds in your bank box to donate for this title.
            }
            else if (rating > Citizenship.GetLoyaltyRating(User))
            {
                AddHtmlLocalized(30, 180, 280, 80, 1152903, false, false);
                // You do not have the required minimum loyalty rating to receive this title.
            }
            else
            {
                AddHtmlLocalized(60, 345, 150, 16, 1152904, false, false); // Gain Title
                AddButton(40, 350, 2103, 2104, 1, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(60, 365, 150, 16, 1152889, false, false); // Cancel
            AddButton(40, 370, 2103, 2104, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            base.OnResponse(info);

            if (info.ButtonID == 1)
            {
                int cost = Citizenship.GetTitleCost(Title);

                if (Banker.Withdraw(User, cost, true))
                {
                    Citizenship.AddTitle(User, Title);
                    Citizenship.AddToTreasury(User, cost);
                }
            }
            else if (info.ButtonID == 2)
            {
                SendGump(new CityTitlesGump(User));
            }
        }
    }

    public class CityStoneGump : BaseGump
    {
        public CityLoyaltySystem City { get; set; }

        public CityStoneGump(PlayerMobile pm, CityLoyaltySystem city)
            : base(pm, 50, 50)
        {
            City = city;

            pm.CloseGump(typeof(CityStoneGump));
        }

        public override void AddGumpLayout()
        {
            if (City.Election == null || !City.Election.Ongoing)
            {
                AddBackground(0, 0, 400, 150, 5054);
                AddHtmlLocalized(0, 12, 400, 20, CenterLoc, "#1155758", 0xFFFF, false, false); // City Stone

                DateTime next = City.Election.NextElection();

                AddHtmlLocalized(20, 45, 360, 60, 1153898, string.Format("{0}\t{1}\t{2}",
                    City.Definition.Name,
                    City.Governor == null ? "Vacant" : City.Governor.Name,
                    next.ToString()), 0xFFFF, false, false); // The current Governor of ~1_CITY~ is ~2_PLAYER~.  The next election cycle begins after ~3_DAYS~. 
            }
            else
            {
                AddBackground(0, 0, 500, 182, 5054);
                AddHtmlLocalized(10, 12, 150, 20, 1155758, 0xFFFF, false, false); // City Stone

                if (City.Election.CanNominate())
                {
                    AddButton(10, 40, 4002, 4004, 1, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(45, 40, 250, 20, 1153901, 0xFFFF, false, false); // Nominate Yourself for the Ballot 
                }

                AddButton(10, 80, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 80, 250, 20, 1153900, 0xFFFF, false, false); // View List of Current Nominees

                AddButton(10, 110, 4005, 4007, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 110, 250, 20, 1153899, 0xFFFF, false, false); // View List of Current Candidates 

                AddButton(10, 150, 4005, 4007, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 150, 250, 20, 1060675, 0xFFFF, false, false); // CLOSE
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: break;
                case 1: City.Election.TryNominate(User); break;
                case 2: SendGump(new NomineesGump(User, City)); break;
                case 3: SendGump(new CandidatesGump(User, City)); break;
            }
        }
    }

    public class NomineesGump : BaseGump
    {
        public CityLoyaltySystem City { get; set; }

        public NomineesGump(PlayerMobile pm, CityLoyaltySystem city)
            : base(pm, 50, 50)
        {
            City = city;

            pm.CloseGump(typeof(NomineesGump));
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 600, 400, 5054);
            AddHtmlLocalized(10, 12, 200, 20, 1153906, 0xFFFF, false, false); // Current Nominees

            int page = 0;
            AddPage(page);

            AddHtmlLocalized(10, 40, 120, 20, 1153907, 0xFFFF, false, false); // Endorse
            AddHtmlLocalized(130, 40, 170, 20, 1153908, 0xFFFF, false, false); // Name
            AddHtmlLocalized(300, 40, 170, 20, 1153909, 0xFFFF, false, false); // Guild

            AddButton(10, 368, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 368, 150, 16, 1149777, 0xFFFF, false, false); // MAIN MENU

            AddButton(540, 368, 4017, 4019, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(485, 368, 150, 16, 1153910, 0xFFFF, false, false); // Withdraw

            page++;
            AddPage(page);
            int pageIndex = 0;

            for (int i = 0; i < City.Election.Candidates.Count; i++)
            {
                BallotEntry entry = City.Election.Candidates[i];

                Guild g = entry.Player == null ? null : entry.Player.Guild as Guild;

                AddButton(10, 70 + (pageIndex * 25), 4002, 4004, i + 100, GumpButtonType.Reply, 0);
                AddHtml(130, 70 + (pageIndex * 25), 200, 20, string.Format("<basefont color=#EEE8AA>{0}", entry.Player == null ? "Unknown" : entry.Player.Name), false, false);
                AddHtml(300, 70 + (pageIndex * 25), 200, 20, string.Format("<basefont color=#EEE8AA>{0}", g != null ? g.Name : ""), false, false);
                pageIndex++;

                if (pageIndex >= 10 && i < City.Election.Candidates.Count - 1)
                {
                    pageIndex = 0;
                    page++;

                    AddHtml(535, 346, 100, 16, "<basefont color=#FFFFFF>NEXT", false, false);
                    AddButton(574, 348, 5601, 5605, 0, GumpButtonType.Page, page);

                    AddPage(page);
                }

                if (i > 10)
                {
                    AddHtml(32, 346, 100, 16, "<basefont color=#FFFFFF>BACK", false, false);
                    AddButton(10, 348, 5603, 5607, 0, GumpButtonType.Page, page - 1);
                }
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1)
                SendGump(new CityStoneGump(User, City));
            else if (info.ButtonID == 2)
                City.Election.TryWithdraw(User);
            else if (info.ButtonID >= 100)
            {
                int id = info.ButtonID - 100;

                if (id >= 0 && id < City.Election.Candidates.Count)
                {
                    BallotEntry entry = City.Election.Candidates[id];

                    if (entry.Player != null)
                    {
                        City.Election.TryEndorse(User, entry.Player);
                    }
                }
            }
        }
    }

    public class CandidatesGump : BaseGump
    {
        public CityLoyaltySystem City { get; set; }

        List<BallotEntry> Candidates { get; set; }

        public CandidatesGump(PlayerMobile pm, CityLoyaltySystem city)
            : base(pm, 50, 50)
        {
            City = city;

            pm.CloseGump(typeof(CandidatesGump));
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 600, 400, 5054);
            AddHtmlLocalized(10, 12, 200, 20, 1153914, 0xFFFF, false, false); // Candidate List

            int page = 0;
            AddPage(page);

            AddHtmlLocalized(10, 40, 120, 20, 1153915, 0xFFFF, false, false); // Cast Vote
            AddHtmlLocalized(130, 40, 170, 20, 1153908, 0xFFFF, false, false); // Name
            AddHtmlLocalized(300, 40, 170, 20, 1153909, 0xFFFF, false, false); // Guild
            AddHtmlLocalized(470, 40, 80, 20, 1153916, 0xFFFF, false, false); // Standing

            AddButton(10, 368, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 368, 150, 16, 1149777, 0xFFFF, false, false); // MAIN MENU

            AddButton(540, 368, 4017, 4019, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(485, 368, 150, 16, 1153910, 0xFFFF, false, false); // Withdraw

            page++;
            AddPage(page);
            int pageIndex = 0;

            Candidates = new List<BallotEntry>(City.Election.Candidates.Where(e => e.Endorsements.Count > 0));

            for (int i = 0; i < Candidates.Count; i++)
            {
                BallotEntry entry = Candidates[i];

                Guild g = entry.Player == null ? null : entry.Player.Guild as Guild;

                AddButton(10, 70 + (pageIndex * 25), 4002, 4004, i + 100, GumpButtonType.Reply, 0);
                AddHtml(130, 70 + (pageIndex * 25), 200, 20, string.Format("<basefont color=#EEE8AA>{0}", entry.Player == null ? "Unknown" : entry.Player.Name), false, false);
                AddHtml(300, 70 + (pageIndex * 25), 200, 20, string.Format("<basefont color=#EEE8AA>{0}", g != null ? g.Name : ""), false, false);
                AddHtml(470, 70 + (pageIndex * 25), 200, 20, string.Format("<basefont color=#EEE8AA>{0}%", City.Election.GetStanding(entry).ToString("F1", System.Globalization.CultureInfo.InvariantCulture)), false, false);

                pageIndex++;

                if (pageIndex >= 10 && i < Candidates.Count - 1)
                {
                    pageIndex = 0;
                    page++;

                    AddHtml(535, 346, 100, 16, "<basefont color=#FFFFFF>NEXT", false, false);
                    AddButton(574, 348, 5601, 5605, 0, GumpButtonType.Page, page);

                    AddPage(page);
                }

                if (i > 10)
                {
                    AddHtml(32, 346, 100, 16, "<basefont color=#FFFFFF>BACK", false, false);
                    AddButton(10, 348, 5603, 5607, 0, GumpButtonType.Page, page - 1);
                }
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1)
                SendGump(new CityStoneGump(User, City));
            else if (info.ButtonID == 2)
                City.Election.TryWithdraw(User);
            else if (info.ButtonID >= 100)
            {
                int id = info.ButtonID - 100;

                if (id >= 0 && id < Candidates.Count)
                {
                    BallotEntry entry = Candidates[id];

                    if (entry.Player != null)
                    {
                        City.Election.TryVote(User, entry.Player);
                    }
                }
            }
        }
    }

    public class ChooseTradeDealGump : BaseGump
    {
        public CityLoyaltySystem City { get; set; }

        public ChooseTradeDealGump(PlayerMobile pm, CityLoyaltySystem city)
            : base(pm, 75, 75)
        {
            City = city;
        }

        public override void AddGumpLayout()
        {
            AddImage(0, 0, 8000);
            AddImage(20, 37, 8001);
            AddImage(20, 107, 8002);
            AddImage(20, 177, 8001);
            AddImage(20, 247, 8002);
            AddImage(20, 317, 8001);
            AddImage(20, 387, 8003);

            AddHtmlLocalized(0, 7, 345, 20, 1154645, "#1154103", 0, false, false); // City Loyalty Trade Deal

            AddHtmlLocalized(30, 40, 280, 100, 1154030, false, false);
            /*Which Trade Guild do you wish to make a deal with?  A new trade deal costs 
             * 2,000,000gp and is valid for one week, at which time you may opt to renew 
             * the deal or choose a different deal.*/

            int y = 150;

            for (int i = 0; i < _Deals.Length; i++)
            {
                AddButton(30, (y + 5), 2103, 2104, i + 100, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, y, 200, 20, (int)_Deals[i] - 12, false, false);

                y += 20;
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            int id = info.ButtonID - 100;

            if (id >= 0 && id < _Deals.Length)
            {
                if (City.Treasury < CityLoyaltySystem.TradeDealCost)
                {
                    City.HeraldMessage(User, 1154057); // Begging thy pardon but the City Treasury doth nay have the funds available to make such a deal!
                }
                else if (User.AccessLevel == AccessLevel.Player && City.TradeDealStart != DateTime.MinValue && City.TradeDealStart + TimeSpan.FromDays(CityLoyaltySystem.TradeDealCooldown) > DateTime.UtcNow)
                {
                    City.HeraldMessage(User, 1154056); // You may only make a trade deal once per real world week!
                }
                else
                {
                    City.Treasury -= CityLoyaltySystem.TradeDealCost;

                    City.OnNewTradeDeal(_Deals[id]);
                    City.HeraldMessage(1154058, string.Format("{0}\t#{1}", City.Definition.Name, (int)_Deals[id] - 12));
                }
            }
        }

        private readonly TradeDeal[] _Deals =
        {
            TradeDeal.GuildOfArcaneArts,
            TradeDeal.SocietyOfClothiers,
            TradeDeal.BardicCollegium,
            TradeDeal.OrderOfEngineers,
            TradeDeal.GuildOfHealers,
            TradeDeal.MaritimeGuild,
            TradeDeal.MerchantsAssociation,
            TradeDeal.MiningCooperative,
            TradeDeal.LeagueOfRangers,
            TradeDeal.GuildOfAssassins,
            TradeDeal.WarriorsGuild,
        };
    }

    public class PlayerTitleGump : BaseGump
    {
        public PlayerMobile Citizen { get; set; }
        public CityLoyaltySystem City { get; set; }

        public PlayerTitleGump(PlayerMobile pm, PlayerMobile citizen, CityLoyaltySystem sys)
            : base(pm, 50, 50)
        {
            Citizen = citizen;
            City = sys;

            pm.CloseGump(typeof(PlayerTitleGump));
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 400, 400, 5054);
            AddHtmlLocalized(0, 15, 400, 20, 1154015, false, false); // <CENTER>City Title</CENTER>

            AddHtmlLocalized(15, 45, 370, 80, 1154016, false, false); // What title doth thou wish to bestow upon this citizen?<BR><BR>Note: All City Titles are automatically followed by "of (City Name)" where (City Name) is the name of the city. 

            AddImageTiled(50, 145, 250, 20, 3504);
            AddTextEntry(51, 145, 249, 20, 0, 1, "");
            AddButton(15, 145, 4023, 4024, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            TextRelay relay = info.GetTextEntry(1);
            CityLoyaltyEntry entry = City.GetPlayerEntry<CityLoyaltyEntry>(Citizen);

            if (entry == null)
                return;

            if (relay == null || string.IsNullOrEmpty(relay.Text))
            {
                if (entry != null)
                {
                    entry.CustomTitle = null;
                    Citizen.RemoveRewardTitle(1154017, true);

                    if (User != Citizen)
                    {
                        User.SendMessage("You have removed their title.");
                        Citizen.SendMessage("{0} has removed your city title.", User.Name);
                    }
                    else
                    {
                        User.SendMessage("You have removed your title.");
                    }
                }
            }
            else
            {
                string text = Utility.FixHtml(relay.Text);

                if (BaseGuildGump.CheckProfanity(text) && text.Trim().Length > 3)
                {
                    if (entry != null && entry.IsCitizen)
                    {
                        Citizen.AddRewardTitle(1154017); // ~1_TITLE~ of ~2_CITY~
                        entry.CustomTitle = text.Trim();

                        if (User != Citizen)
                            User.SendMessage("You have bestowed {0} the title: {1} of {2}.", Citizen.Name, text, City.Definition.Name);

                        Citizen.SendLocalizedMessage(1155605, string.Format("{0}\t{1}", text, City.Definition.Name)); // Thou hath been bestowed the title ~1_TITLE~! - ~1_TITLE~ of ~2_CITY~
                    }
                }
                else
                    User.SendLocalizedMessage(501179); // That title is disallowed.
            }
        }
    }

    public class AcceptOfficeGump : BaseGump
    {
        public CityLoyaltySystem City { get; set; }

        public AcceptOfficeGump(PlayerMobile pm, CityLoyaltySystem city) : base(pm, 50, 50)
        {
            City = city;

            pm.CloseGump(typeof(AcceptOfficeGump));
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 600, 600, 9380);

            AddHtmlLocalized(50, 50, 500, 400, 1154069, City.Definition.Name, 0, false, false);
            /*You are about to accept the office of Governor of ~1_CITY~. You will hold this office for 90 days. During your time in office you 
             * will be expected to:<BR><BR>1. Attend meetings with His Royal Majesty King Blackthorn.<BR>2. Encourage citizens to raise revenue 
             * for the City Treasury.<BR>3. Address the needs and concerns of your citizens and bring their voice to The King.<BR>4. Negotiate
             * Trade Deals with various Professional Guilds.<BR>5. Grant titles to those citizens you deem worthy. Any title you bestow that is 
             * deemed inappropriate will be grounds for removal from office and action being taken against your account.<BR><BR>At any time 
             * during your administration should you have a negative loyalty with the City, renounce your citizenship, or transfer off this shard 
             * you will automatically resign your office.<BR><BR>If at any time should you require to take a leave of absence from Britannia or 
             * otherwise cannot fulfill the duties of your office it is your responsibility to communicate to King Blackthorn so that a replacement 
             * can be found or he shall do so at his discretion.<BR><BR>Congratulations, Governor.*/

            AddButton(50, 548, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(90, 548, 150, 20, 3000372, false, false); // Accept

            AddButton(510, 548, 4017, 4019, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(450, 548, 150, 20, 1115372, false, false); // Decline
        }

        public override void OnResponse(RelayInfo info)
        {
            PlayerMobile pm = User as PlayerMobile;

            if (pm == null)
                return;

            if (info.ButtonID == 1)
            {
                City.Governor = pm;
            }
            else if (info.ButtonID == 2)
            {
                //<CENTER>Are you sure you wish to reject the Office of Governor? This is permanent and cannot be undone.<CENTER>
                pm.SendGump(new ConfirmCallbackGump(pm, null, 1154092, null, null, (m, o) =>
                    {
                        City.PendingGovernor = true;
                        City.Election.OnRejectOffice(User);
                    }));
            }
        }
    }

    public class ElectionStartTimeGump : BaseGump
    {
        public ElectionStartTimeGump(PlayerMobile pm) : base(pm, 50, 50)
        {
            pm.CloseGump(typeof(ElectionStartTimeGump));
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 400, 400, 5054);
            AddHtml(0, 15, 400, 20, "<center>Election Start Times</center>", false, false);

            if (CityLoyaltySystem.Britain.Election != null && CityLoyaltySystem.Britain.Election.Ongoing)
            {
                AddHtml(15, 35, 370, 40, "You will have to wait until the current election is over before you can start another one.", false, true);
            }
            else
            {
                AddHtml(15, 35, 370, 120, "You can have up to 4 election times.  Per EA, default are March, June, September and December, " +
                                         "or 3, 6, 9, 12 as the system uses the numeric value for the month. Each election time must be at " +
                                         "least 1 month apart, and each will begin on the first day of that month.", false, false);


                AddLabel(15, 160, 0, "Start Time");
                AddLabel(150, 160, 0, "Month (numeric)");

                for (int i = 0; i < 4; i++)
                {
                    string start = "";

                    if (CityLoyaltySystem.Britain.Election != null && CityLoyaltySystem.Britain.Election.StartTimes.Length >= i && CityLoyaltySystem.Britain.Election.StartTimes[i] != DateTime.MinValue)
                        start = CityLoyaltySystem.Britain.Election.StartTimes[i].Month.ToString();

                    AddLabel(15, 180 + (i * 25), 0, (i + 1).ToString() + ".");
                    AddImageTiled(150, 180 + (i * 25), 50, 20, 5058);
                    AddAlphaRegion(150, 180 + (i * 25), 50, 20);
                    AddTextEntry(151, 180 + (i * 25), 49, 20, 0, i + 1, start);
                }

                AddHtml(55, 346, 150, 20, "DEFAULTS", false, false);
                AddButton(15, 346, 4005, 4007, 2, GumpButtonType.Reply, 0);

                AddHtml(55, 368, 150, 20, "SELECT", false, false);
                AddButton(15, 368, 4005, 4007, 1, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID < 1 || (CityLoyaltySystem.Britain.Election != null && CityLoyaltySystem.Britain.Election.Ongoing))
                return;

            if (info.ButtonID == 2)
            {
                foreach (CityLoyaltySystem sys in CityLoyaltySystem.Cities.Where(s => s.Election != null))
                {
                    sys.Election.GetDefaultStartTimes();
                }

                User.SendMessage("Election start times reset to default.");
                return;
            }

            TextRelay relay1 = info.GetTextEntry(1);
            TextRelay relay2 = info.GetTextEntry(2);
            TextRelay relay3 = info.GetTextEntry(3);
            TextRelay relay4 = info.GetTextEntry(4);

            List<int> times = new List<int>();

            if (relay1 != null)
            {
                int time = Utility.ToInt32(relay1.Text);

                if (time > 0 && time < 13)
                    times.Add(time);
            }

            if (relay2 != null)
            {
                int time = Utility.ToInt32(relay2.Text);

                if (time > 0 && time < 13)
                    times.Add(time);
            }

            if (relay3 != null)
            {
                int time = Utility.ToInt32(relay3.Text);

                if (time > 0 && time < 13)
                    times.Add(time);
            }

            if (relay4 != null)
            {
                int time = Utility.ToInt32(relay4.Text);

                if (time > 0 && time < 13)
                    times.Add(time);
            }

            if (times.Count > 0)
            {
                DateTime[] starttimes = CityElection.ValidateStartTimes(User, times.ToArray());

                if (starttimes != null)
                {
                    foreach (CityLoyaltySystem sys in CityLoyaltySystem.Cities.Where(s => s.Election != null))
                    {
                        sys.Election.StartTimes = starttimes;
                    }

                    User.SendMessage("Election start times reset.");
                }
            }
        }
    }

    /// <summary>
    /// I cannot find ANYWHERE what "Open Inventory" context menu entry does on EA.
    /// Its apparent it is a Governor only function, however no docs can be found on it.
    /// </summary>
    public class OpenInventoryGump : BaseGump
    {
        public CityLoyaltySystem City { get; set; }

        public OpenInventoryGump(PlayerMobile pm, CityLoyaltySystem city)
            : base(pm, 50, 50)
        {
            City = city;
        }

        public override void AddGumpLayout()
        {
            if (City == null || City.Stone == null || City.Stone.Boxes == null)
                return;

            AddBackground(0, 0, 250, (City.Stone.Boxes.Count * 25) + 90, 5054);
            AddHtml(0, 15, 250, 20, string.Format("<center>Inventory - {0}</center>", City.Definition.Name), false, false); // Inventory

            AddLabel(10, 40, 0, "Location");
            AddLabel(150, 40, 0, "View");
            AddLabel(200, 40, 0, "Remove");

            City.Stone.Boxes.ForEach(b =>
            {
                if (b.Deleted)
                    City.Stone.Boxes.Remove(b);
            });

            for (int i = 0; i < City.Stone.Boxes.Count; i++)
            {
                BallotBox box = City.Stone.Boxes[i];

                AddLabel(10, 70 + (i * 25), 0, box == null || box.Deleted ? "Deleted" : box.Location.ToString());
                AddButton(150, 70 + (i * 25), 4011, 4013, i + 100, GumpButtonType.Reply, 0);
                AddButton(200, 70 + (i * 25), 4017, 4019, i + 200, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            Mobile m = User;

            if (info.ButtonID == 0)
                return;

            int id;

            if (info.ButtonID < 200)
            {
                id = info.ButtonID - 100;

                if (id >= 0 && id < City.Stone.Boxes.Count)
                {
                    BallotBox box = City.Stone.Boxes[id];
                    box.SendGumpTo(m);
                }
            }
            else
            {
                id = info.ButtonID - 200;

                if (id >= 0 && id < City.Stone.Boxes.Count)
                {
                    BallotBox box = City.Stone.Boxes[id];

                    m.SendGump(new WarningGump(1111873, 30720, "This will permanently delete the ballot box. Proceed?", 32512, 300, 200, (mob, ok, obj) =>
                        {
                            if (ok)
                            {
                                box.Delete();
                                City.Stone.Boxes.Remove(box);

                                m.SendMessage("Ballot box deleted.");
                            }

                            m.CloseGump(typeof(OpenInventoryGump));
                            SendGump(new OpenInventoryGump(User, City));
                        }, box, true));
                }
            }
        }
    }

    public class CityMessageBoardGump : BaseGump
    {
        private readonly int _Red = 0x8B0000;

        public CityMessageBoardGump(PlayerMobile pm)
            : base(pm, 100, 100)
        {
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 554, 350, 9380);

            AddHtmlLocalized(0, 55, 554, 20, 1154645, "#1154911", Quests.BaseQuestGump.C32216(_Red), false, false);

            for (int i = 0; i < CityLoyaltySystem.Cities.Count; i++)
            {
                CityLoyaltySystem city = CityLoyaltySystem.Cities[i];

                AddButton(25, 78 + (i * 20), 2103, 2104, i + 100, GumpButtonType.Reply, 0);
                AddHtml(70, 75 + (i * 20), 150, 20, string.Format("<basefont color=#8B0000>{0}", city.Definition.Name), false, false);
                AddLabelCropped(170, 75 + (i * 20), 295, 20, 0, city.Headline);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            int id = info.ButtonID - 100;

            if (id >= 0 && id < CityLoyaltySystem.Cities.Count)
                SendGump(new CityMessageGump(User, CityLoyaltySystem.Cities[id]));
        }
    }

    public class CityMessageGump : BaseGump
    {
        public CityLoyaltySystem City { get; set; }

        public CityMessageGump(PlayerMobile m, CityLoyaltySystem city)
            : base(m, 100, 100)
        {
            City = city;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 554, 350, 9380);
            AddHtmlLocalized(25, 55, 500, 20, 1154915, City.Definition.Name, 0, false, false); // The Latest News from the City of ~1_CITY~

            if (City.PostedOn != DateTime.MinValue)
                AddHtmlLocalized(25, 85, 500, 20, 1154916, string.Format("{0}\t{1}", City.Governor != null ? City.Governor.Name : "somebody", City.PostedOn.ToShortDateString()), 0, false, false); // Posted by ~1_NAME~ on ~2_date~

            AddHtml(25, 115, 500, 20, City.Headline, false, false);
            AddHtml(25, 195, 500, 150, City.Body, false, false);
        }
    }

    public class SystemInfoGump : BaseGump
    {
        public SystemInfoGump(PlayerMobile pm)
            : base(pm, 50, 50)
        {
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 400, (CityLoyaltySystem.Cities.Count * 25) + 100, 5054);

            AddHtml(0, 15, 400, 20, "<center>City Loyalty System Info</center>", false, false);

            AddLabel(10, 35, 0, "City");
            AddLabel(150, 35, 0, "Citizens");
            AddLabel(250, 35, 0, "Treasury");
            AddLabel(350, 35, 0, "View");

            for (int i = 0; i < CityLoyaltySystem.Cities.Count; i++)
            {
                CityLoyaltySystem city = CityLoyaltySystem.Cities[i];

                AddHtml(10, 60 + (i * 25), 190, 20, city.Definition.Name, false, false);
                AddHtml(150, 60 + (i * 25), 100, 20, city.GetCitizenCount().ToString(), false, false);
                AddHtml(250, 60 + (i * 25), 100, 20, city.Treasury.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US")), false, false);
                AddButton(350, 60 + (i * 25), 4005, 4007, i + 100, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            int id = info.ButtonID - 100;

            if (id >= 0 && id < CityLoyaltySystem.Cities.Count)
            {
                SendGump(new CityInfoGump(User, CityLoyaltySystem.Cities[id]));
            }
        }
    }

    public class CityInfoGump : BaseGump
    {
        public CityLoyaltySystem City { get; set; }

        public CityInfoGump(PlayerMobile pm, CityLoyaltySystem city)
            : base(pm, 50, 50)
        {
            City = city;
        }

        public override void AddGumpLayout()
        {
            int page = 0;
            AddPage(page);

            AddBackground(0, 0, 500, 400, 5054);
            AddHtml(0, 15, 400, 20, string.Format("<center>City Info - {0}</center>", City.Definition.Name), false, false);

            AddLabel(10, 35, 0, "Player");
            AddLabel(150, 35, 0, "Love");
            AddLabel(225, 35, 0, "Hate");
            AddLabel(300, 35, 0, "Title");
            AddLabel(450, 35, 0, "Info");

            int pageIndex = 0;
            page++;
            AddPage(page);

            AddButton(10, 370, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddLabel(45, 370, 0, "BACK");

            for (int i = 0; i < City.PlayerTable.Count; i++)
            {
                CityLoyaltyEntry entry = City.PlayerTable[i] as CityLoyaltyEntry;

                if (entry == null)
                    continue;

                pageIndex++;
                AddHtml(10, 60 + (pageIndex * 25), 140, 20, string.Format("{0}{1}", entry.Player != null ? entry.Player.Name : "Unknown", entry.IsCitizen ? "(Citizen)" : ""), false, false);
                AddHtml(150, 60 + (pageIndex * 25), 75, 20, entry.Love.ToString(), false, false);
                AddHtml(225, 60 + (pageIndex * 25), 75, 20, entry.Hate.ToString(), false, false);
                AddHtml(300, 60 + (pageIndex * 25), 150, 20, string.IsNullOrEmpty(entry.CustomTitle) ? "" : entry.CustomTitle, false, false);
                AddButton(450, 60 + (pageIndex * 25), 4005, 4007, i + 100, GumpButtonType.Reply, 0);

                if (pageIndex >= 11 && i < City.PlayerTable.Count - 1)
                {
                    pageIndex = 0;

                    page++;
                    AddButton(252, 370, 4005, 4007, 0, GumpButtonType.Page, page);
                    AddPage(page);
                }

                if (i > 12)
                {
                    AddButton(228, 370, 4014, 4016, 0, GumpButtonType.Page, page - 1);
                }
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            if (info.ButtonID == 1)
            {
                SendGump(new SystemInfoGump(User));
                return;
            }

            int id = info.ButtonID - 100;

            if (id >= 0 && id < City.PlayerTable.Count)
            {
                Refresh();
                User.SendGump(new PropertiesGump(User, City.PlayerTable[id]));
            }
        }
    }
}
