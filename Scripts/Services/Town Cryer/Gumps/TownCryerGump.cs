using Server;
using System;
using System.Linq;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Engines.CityLoyalty;
using Server.Gumps;
using Server.Guilds;
using Server.Engines.Quests;

namespace Server.Services.TownCryer
{
    public class TownCryerGump : BaseTownCryerGump
    {
        public enum GumpCategory
        {
            News = 1,
            EventModerator,
            City,
            Guild
        }

        private City _City;

        public GumpCategory Category { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }

        public City City
        {
            get { return _City; }
            set
            {
                var city = _City;

                if (city != value)
                {
                    _City = value;
                    SetDefaultCity();
                }
            }
        }

        public static Dictionary<PlayerMobile, City> LastCity { get; private set; }

        public TownCryerGump(PlayerMobile pm, TownCrier tc, int page = 0, GumpCategory Cartegory = GumpCategory.News)
            : base(pm, tc)
        {
            Page = page;
            Category = GumpCategory.News;

            SetDefaultCity();
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            switch (Category)
            {
                case GumpCategory.News:
                    BuildNewsPage();
                    break;
                case GumpCategory.EventModerator:
                    BuildEMPage();
                    break;
                case GumpCategory.City:
                    BuildCityPage();
                    break;
                case GumpCategory.Guild:
                    BuildGuildPage();
                    break;
            }

            AddButton(275, 598, Category == GumpCategory.News ? 0x5F6 : 0x5F5, 0x5F6, 1, GumpButtonType.Reply, 0);
            AddButton(355, 598, Category == GumpCategory.EventModerator ? 0x5F4 : 0x5F3, 0x5F4, 2, GumpButtonType.Reply, 0);
            AddButton(435, 598, Category == GumpCategory.City ? 0x5F8 : 0x5F7, 0x5F8, 3, GumpButtonType.Reply, 0);
            AddButton(515, 598, Category == GumpCategory.Guild ? 0x5F2 : 0x5F1, 0x5F2, 4, GumpButtonType.Reply, 0);
        }

        private void BuildNewsPage()
        {
            int perPage = 20;

            int y = 170;
            int start = Page * perPage;

            Pages = (int)Math.Ceiling((double)TownCryerSystem.NewsEntries.Count / (double)perPage);

            for (int i = start; i < TownCryerSystem.NewsEntries.Count && i < perPage; i++)
            {
                var entry = TownCryerSystem.NewsEntries[i];

                AddButton(50, y, 0x5FB, 0x5FC, 100 + i, GumpButtonType.Reply, 0);
                bool doneQuest = entry.QuestType != null && QuestHelper.CheckDoneOnce(User, entry.QuestType, Cryer, false);

                if (entry.Title.Number > 0)
                {
                    AddHtmlLocalized(87, y, 700, 20, entry.Title.Number, doneQuest ? C32216(0x696969) : 0, false, false);
                }
                else
                {
                    AddLabelCropped(87, y, 700, 20, doneQuest ? 0x3B2 : 0, entry.Title);
                }

                y += 23;
            }

            if (TownCryerSystem.NewsEntries.Count > perPage)
            {
                AddButton(350, 570, 0x605, 0x606, 5, GumpButtonType.Reply, 0);
                AddButton(380, 570, 0x609, 0x60A, 6, GumpButtonType.Reply, 0);
                AddButton(430, 570, 0x607, 0x608, 7, GumpButtonType.Reply, 0);
                AddButton(455, 570, 0x603, 0x604, 8, GumpButtonType.Reply, 0);

                AddHtml(395, 570, 35, 20, Center(String.Format("{0}/{1}", (Page + 1).ToString(), (Pages + 1).ToString())), false, false);
            }
        }

        private void BuildEMPage()
        {
            int perPage = 15;
            AddPage(1);
            int y = 170;

            for (int i = 0; i < TownCryerSystem.ModeratorEntries.Count && i < perPage; i++)
            {
                var entry = TownCryerSystem.ModeratorEntries[i];

                AddButton(50, y, 0x5FB, 0x5FC, 200 + i, GumpButtonType.Reply, 0);
                AddLabelCropped(87, y, 631, 20, 0, entry.Title);

                if (User.AccessLevel > AccessLevel.Player) // Couselors+ can moderate events
                {
                    AddButton(735, y, 0x5FD, 0x5FE, 2000 + i, GumpButtonType.Reply, 0);
                    AddButton(760, y, 0x5FF, 0x600, 2500 + i, GumpButtonType.Reply, 0);
                }

                y += 23;
            }

            AddButton(320, 525, 0x627, 0x628, 9, GumpButtonType.Reply, 0);
        }

        private void BuildCityPage()
        {
            AddButton(233, 150, City == City.Britain ? 0x5E5 : 0x5E4, City == City.Britain ? 0x5E5 : 0x5E4, 10, GumpButtonType.Reply, 0);
            AddTooltip(CityLoyaltySystem.GetCityLocalization(City.Britain));
            
            AddButton(280, 150, City == City.Jhelom ? 0x5E7 : 0x5E6, City == City.Jhelom ? 0x5E7 : 0x5E6, 11, GumpButtonType.Reply, 0);
            AddTooltip(CityLoyaltySystem.GetCityLocalization(City.Jhelom));
            
            AddButton(327, 150, City == City.Minoc ? 0x5E5 : 0x5E4, City == City.Minoc ? 0x5E5 : 0x5E4, 12, GumpButtonType.Reply, 0);
            AddTooltip(CityLoyaltySystem.GetCityLocalization(City.Minoc));

            AddButton(374, 150, City == City.Moonglow ? 0x5E3 : 0x5E2, City == City.Moonglow ? 0x5E3 : 0x5E2, 13, GumpButtonType.Reply, 0);
            AddTooltip(CityLoyaltySystem.GetCityLocalization(City.Moonglow));

            AddButton(418, 150, City == City.NewMagincia ? 0x5DD : 0x5DC, City == City.NewMagincia ? 0x5DD : 0x5DC, 14, GumpButtonType.Reply, 0);
            AddTooltip(CityLoyaltySystem.GetCityLocalization(City.NewMagincia));

            AddButton(463, 150, City == City.SkaraBrae ? 0x5DF : 0x5DE, City == City.SkaraBrae ? 0x5DF : 0x5DE, 15, GumpButtonType.Reply, 0);
            AddTooltip(CityLoyaltySystem.GetCityLocalization(City.SkaraBrae));

            AddButton(509, 150, City == City.Trinsic ? 0x5E1 : 0x5E0, City == City.Trinsic ? 0x5E1 : 0x5E0, 16, GumpButtonType.Reply, 0);
            AddTooltip(CityLoyaltySystem.GetCityLocalization(City.Trinsic));

            AddButton(555, 150, City == City.Vesper ? 0x5ED : 0x5ED, City == City.Vesper ? 0x5ED : 0x5EC, 17, GumpButtonType.Reply, 0);
            AddTooltip(CityLoyaltySystem.GetCityLocalization(City.Vesper));

            AddButton(601, 150, City == City.Yew ? 0x5E9 : 0x5E8, City == City.Yew ? 0x5E9 : 0x5E8, 18, GumpButtonType.Reply, 0);
            AddTooltip(CityLoyaltySystem.GetCityLocalization(City.Yew));

            AddHtmlLocalized(0, 260, 854, 20, CenterLoc, String.Format("#{0}", TownCryerSystem.GetCityLoc(this.City)), 0, false, false); // The Latest News from the City of ~1_CITY~

            int y = 300;

            for (int i = 0; i < TownCryerSystem.CityEntries.Count && i < TownCryerSystem.MaxPerCityGoverrnorEntries; i++)
            {
                var entry = TownCryerSystem.CityEntries[i];

                if (entry.City != this.City)
                    continue;

                AddButton(50, y, 0x5FB, 0x5FC, 300 + i, GumpButtonType.Reply, 0);
                AddLabelCropped(87, y, 700, 20, 0, entry.Title);

                var city = CityLoyaltySystem.GetCitizenship(User, false);

                if ((city != null && city.Governor == User) || User.AccessLevel >= AccessLevel.GameMaster) // Only Governors
                {
                    AddButton(735, y, 0x5FD, 0x5FE, 3000 + i, GumpButtonType.Reply, 0);
                    AddButton(760, y, 0x5FF, 0x600, 3500 + i, GumpButtonType.Reply, 0);
                }

                y += 23;
            }

            AddImage(230, 460, 0x5F0);
        }

        private void BuildGuildPage()
        {
            var list = TownCryerSystem.GuildEntries.OrderBy(e => e.EventTime).ToList();

            int perPage = 20;
            int y = 170;

            int start = Page * perPage;
            var guild = User.Guild as Guild;

            Pages = (int)Math.Ceiling((double)list.Count / (double)perPage);

            for (int i = start; i < list.Count && i < perPage; i++)
            {
                var entry = TownCryerSystem.GuildEntries[i];

                AddButton(50, y, 0x5FB, 0x5FC, 400 + TownCryerSystem.GuildEntries.IndexOf(entry), GumpButtonType.Reply, 0);

                AddLabelCropped(87, y, 700, 20, 0, entry.FullTitle);

                if ((guild == entry.Guild && User.GuildRank != null && User.GuildRank.Rank >= 3) || User.AccessLevel >= AccessLevel.GameMaster) // Only warlords+
                {
                    int index = TownCryerSystem.GuildEntries.IndexOf(entry);
                    AddButton(735, y, 0x5FD, 0x5FE, 4000 + index, GumpButtonType.Reply, 0);
                    AddButton(760, y, 0x5FF, 0x600, 4500 + index, GumpButtonType.Reply, 0);
                }

                y += 23;
            }

            AddButton(350, 570, 0x605, 0x606, 5, GumpButtonType.Reply, 0);
            AddButton(380, 570, 0x609, 0x60A, 6, GumpButtonType.Reply, 0);
            AddButton(430, 570, 0x607, 0x608, 7, GumpButtonType.Reply, 0);
            AddButton(455, 570, 0x603, 0x604, 8, GumpButtonType.Reply, 0);

            AddHtml(395, 570, 35, 20, Center(String.Format("{0}/{1}", (Page + 1).ToString(), (Pages + 1).ToString())), false, false);
        }

        private void SetDefaultCity()
        {
            if (LastCity == null || !LastCity.ContainsKey(User))
            {
                LastCity = new Dictionary<PlayerMobile, City>();

                var system = CityLoyaltySystem.GetCitizenship(User, false);

                if (system != null)
                {
                    LastCity[User] = system.City;
                }
            }
            else
            {
                LastCity[User] = _City;
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            int id = info.ButtonID;

            switch (id)
            {
                case 0: break;
                case 1:
                case 2:
                case 3:
                case 4:
                    {
                        Category = (GumpCategory)id;
                        Refresh();
                        break;
                    }
                case 5: // <<
                    {
                        Page = 0;
                        Refresh();
                        break;
                    }
                case 6: // <
                    {
                        Page = Math.Max(0, Page - 1);
                        Refresh();
                        break;
                    }
                case 7: // >
                    {
                        Page = Math.Min(Pages, Page + 1);
                        Refresh();
                        break;
                    }
                case 8: // >>
                    {
                        Page = Pages;
                        Refresh();
                        break;
                    }
                case 9: // Learn More - EM Page
                    {
                        User.LaunchBrowser(TownCryerSystem.EMEventsPage);
                        Refresh();
                        break;
                    }
                case 10:
                    {
                        City = City.Britain;
                        Refresh();
                        break;
                    }
                case 11:
                    {
                        City = City.Jhelom;
                        Refresh();
                        break;
                    }
                case 12:
                    {
                        City = City.Minoc;
                        Refresh();
                        break;
                    }
                case 13:
                    {
                        City = City.Moonglow;
                        Refresh();
                        break;
                    }
                case 14:
                    {
                        City = City.NewMagincia;
                        Refresh();
                        break;
                    }
                case 15:
                    {
                        City = City.SkaraBrae;
                        Refresh();
                        break;
                    }
                case 16:
                    {
                        City = City.Trinsic;
                        Refresh();
                        break;
                    }
                case 17:
                    {
                        City = City.Vesper;
                        Refresh();
                        break;
                    }
                case 18:
                    {
                        City = City.Yew;
                        Refresh();
                        break;
                    }
                default:
                    {
                        if (id < 200)
                        {
                            id -= 100;

                            if (id >= 0 && id < TownCryerSystem.NewsEntries.Count)
                            {
                                BaseGump.SendGump(new TownCryerNewsGump(User, Cryer, TownCryerSystem.NewsEntries[id]));
                            }
                        }
                        else if (id < 300)
                        {
                            id -= 200;

                            if (id >= 0 && id < TownCryerSystem.ModeratorEntries.Count)
                            {
                                BaseGump.SendGump(new TownCryerEventModeratorGump(User, Cryer, TownCryerSystem.ModeratorEntries[id]));
                            }
                        }
                        else if (id < 400)
                        {
                            id -= 300;

                            if (id >= 0 && id < TownCryerSystem.CityEntries.Count)
                            {
                                BaseGump.SendGump(new TownCryerCityGump(User, Cryer, TownCryerSystem.CityEntries[id]));
                            }
                        }
                        else if (id < 600)
                        {
                            id -= 400;

                            if (id >= 0 && id < TownCryerSystem.GuildEntries.Count)
                            {
                                BaseGump.SendGump(new TownCryerGuildGump(User, Cryer, TownCryerSystem.GuildEntries[id]));
                            }
                        }
                        else if (id < 3000)
                        {
                            if (id < 2500)
                            {
                                id -= 2000;

                                if (id >= 0 && id < TownCryerSystem.ModeratorEntries.Count)
                                {
                                    BaseGump.SendGump(new CreateEMEntryGump(User, Cryer, TownCryerSystem.ModeratorEntries[id]));
                                }
                            }
                            else
                            {
                                id -= 2500;

                                if (id >= 0 && id < TownCryerSystem.ModeratorEntries.Count)
                                {
                                    TownCryerSystem.ModeratorEntries.RemoveAt(id);
                                }

                                Refresh();
                            }
                        }
                        else if (id < 4000)
                        {
                            var city = CityLoyaltySystem.GetCitizenship(User, false);

                            if ((city != null && city.Governor == User) || User.AccessLevel >= AccessLevel.GameMaster) // Only Governors
                            {
                                if (id < 3500)
                                {
                                    id -= 3000;

                                    if (id >= 0 && id < TownCryerSystem.CityEntries.Count)
                                    {
                                        BaseGump.SendGump(new CreateCityEntryGump(User, Cryer, City, TownCryerSystem.CityEntries[id]));
                                    }
                                }
                                else
                                {
                                    id -= 3500;

                                    if (id >= 0 && id < TownCryerSystem.CityEntries.Count)
                                    {
                                        TownCryerSystem.CityEntries.RemoveAt(id);
                                    }
                                }
                            }

                            Refresh();
                        }
                        else if (id < 5000)
                        {
                            if (id < 4500)
                            {
                                id -= 4000;

                                if (id >= 0 && id < TownCryerSystem.GuildEntries.Count)
                                {
                                    BaseGump.SendGump(new CreateGuildEntryGump(User, Cryer, TownCryerSystem.GuildEntries[id]));
                                }
                            }
                            else
                            {
                                id -= 4500;

                                if (id >= 0 && id < TownCryerSystem.GuildEntries.Count)
                                {
                                    TownCryerSystem.GuildEntries.RemoveAt(id);
                                }

                                Refresh();
                            }
                        }
                    }

                    break;
            }
        }
    }
}