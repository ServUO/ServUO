using System;
using Server;
using Server.Mobiles;
using Server.Misc;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using System.Linq;
using Server.Guilds;
using Server.ContextMenus;
using Server.Engines.CityLoyalty;

namespace Server.Gumps
{
    public class TitlesMenuEntry : ContextMenuEntry
    {
        private PlayerMobile _From;

        public TitlesMenuEntry(PlayerMobile from)
            : base(1115022, -1) // Open Titles Menu
        {
            _From = from;
        }

        public override void OnClick()
        {
            _From.CloseGump(typeof(TitlesGump));
            _From.SendGump(new TitlesGump(_From));
        }
    }

    public enum  TitleType
    {
        None,
        PaperdollPrefix,
        PaperdollSuffix,
        OverheadName,
        SubTitles
    }

    public enum TitleCategory
    {
        None,
        FameKarma,
        Skills,
        Guild,
        Champion,
        RewardTitles,
        Veteran
    }

    public class TitlesGump : Gump
    {
        public TitleType TitleType { get; set; }
        public TitleCategory Category { get; set; }
        public bool ShowingDescription { get; set; }
        public bool TitleCleared { get; set; }
        public bool TitleClearing { get; set; }
        public int TitleSelected { get; set; }

        public PlayerMobile User { get; set; }

        public Dictionary<GumpButton, Action<GumpButton>> ButtonCallbacks { get; set; }

        public TitlesGump(PlayerMobile pm, TitleType type = TitleType.None, TitleCategory cat = TitleCategory.None, bool description = false)
            : base(50, 50)
        {
            User = pm;
            TitleType = type;
            Category = cat;

            AddPage(0);

            ButtonCallbacks = new Dictionary<GumpButton, Action<GumpButton>>();

            AddGumpLayout();
        }

        public void AddCallbackButton(int x, int y, int buttonIDnormal, int buttonIDpushed, int id, GumpButtonType type, int page, Action<GumpButton> callback)
        {
            GumpButton b = new GumpButton(x, y, buttonIDnormal, buttonIDpushed, id, type, page);
            ButtonCallbacks[b] = callback;
            Add(b);
        }

        protected void AddGumpLayout()
        {
            AddBackground(0, 0, 540, 350, 9200);

            AddImageTiled(10, 10, 520, 30, 2624);
            AddImageTiled(10, 45, 200, 115, 2624);
            AddImageTiled(10, 165, 200, 175, 2624);
            AddImageTiled(215, 45, 315, 260, 2624);
            AddImageTiled(215, 310, 315, 30, 2624);

            AddAlphaRegion(10, 10, 520, 30);
            AddAlphaRegion(10, 45, 200, 115);
            AddAlphaRegion(10, 165, 200, 175);
            AddAlphaRegion(215, 45, 315, 260);
            AddAlphaRegion(215, 310, 290, 30);

            AddHtmlLocalized(0, 15, 540, 16, 1115023, 0xFFFF, false, false); // <CENTER>TITLES MENU</CENTER>
            AddHtmlLocalized(10, 50, 220, 16, 1115024, 0xFFFF, false, false); // <CENTER>TYPES</CENTER>
            AddHtmlLocalized(10, 170, 220, 16, 1044010, 0xFFFF, false, false); // <CENTER>CATEGORIES</CENTER>

            AddHtmlLocalized(480, 315, 80, 16, 1060675, 0xFFFF, false, false); // CLOSE
            AddCallbackButton(445, 315, 4005, 4007, 0, GumpButtonType.Reply, 0, b => { });

            AddHtmlLocalized(55, 70, 160, 16, 1115026, 0xFFFF, false, false); // Paperdoll Name (Prefix)
            AddCallbackButton(20, 70, 4005, 4007, 10001, GumpButtonType.Reply, 0, b =>
                {
                    TitleType = TitleType.PaperdollPrefix;
                    Reset();
                    Category = TitleCategory.None;
                    Refresh();
                });

            AddHtmlLocalized(55, 92, 160, 16, 1115027, 0xFFFF, false, false); // Paperdoll Name (Suffix)
            AddCallbackButton(20, 92, 4005, 4007, 10002, GumpButtonType.Reply, 0, b =>
                {
                    TitleType = TitleType.PaperdollSuffix;
                    Reset();
                    Category = TitleCategory.None;
                    Refresh();
                });

            AddHtmlLocalized(55, 114, 160, 16, 1115028, 0xFFFF, false, false); // Overhead Name
            AddCallbackButton(20, 114, 4005, 4007, 10003, GumpButtonType.Reply, 0, b =>
                {
                    TitleType = TitleType.OverheadName;
                    Reset();
                    Category = TitleCategory.None;
                    Refresh();
                });

            AddHtmlLocalized(55, 136, 160, 16, 1115029, 0xFFFF, false, false); // Subtitle
            AddCallbackButton(20, 136, 4005, 4007, 10004, GumpButtonType.Reply, 0, b =>
                {
                    TitleType = TitleType.SubTitles;
                    Reset();
                    Category = TitleCategory.None;
                    Refresh();
                });

            if (TitleType > TitleType.PaperdollPrefix && HasTitle())
            {
                if (!TitleClearing)
                {
                    AddHtmlLocalized(55, 315, 80, 16, 1062141, 0xFFFF, false, false); // CLEAR
                    AddCallbackButton(20, 315, 4005, 4007, 10005, GumpButtonType.Reply, 0, b =>
                    {
                        TitleClearing = true;
                        Category = TitleCategory.None;
                        Refresh();

                    });
                }
                else
                {
                    AddHtmlLocalized(225, 275, 200, 16, 1115038, 0xFFFF, false, false); // Do you wish to clear your title?

                    AddHtmlLocalized(480, 275, 80, 16, 1062141, 0xFFFF, false, false); // CLEAR
                    AddCallbackButton(445, 275, 4005, 4007, 10006, GumpButtonType.Reply, 0, but =>
                    {
                        if (TitleType > TitleType.PaperdollPrefix && ClearTitle())
                        {
                            TitleClearing = false;
                            TitleCleared = true;
                            Refresh();
                        }
                    });
                }
            }
            
            if (Category != TitleCategory.None)
            {
                if(ShowingDescription)
                    AddHtmlLocalized(215, 50, 315, 16, 1115025, 0xFFFF, false, false); // <CENTER>DESCRIPTION</CENTER>
                else
                    AddHtmlLocalized(215, 50, 315, 16, 1044011, 0xFFFF, false, false); // <CENTER>SELECTIONS</CENTER>
            }

            if (TitleCleared)
            {
                AddHtmlLocalized(225, 315, 200, 16, 1115037, 0xFFFF, false, false); // TITLE CLEARED
                TitleCleared = false;
            }

            switch (TitleType)
            {
                case TitleType.None: break;
                case TitleType.PaperdollPrefix: BuildPaperdollPrefix(); break;
                case TitleType.PaperdollSuffix: BuildPaperdollSuffix(); break;
                case TitleType.OverheadName: BuildOverheadName(); break;
                case TitleType.SubTitles: BuildSubtitle(); break;
            }
        }

        private void Reset()
        {
            ShowingDescription = false;
            TitleSelected = -1;
            TitleClearing = false;
            TitleCleared = false;
        }

        private void BuildPaperdollPrefix()
        {
            List<string> fameKarma = Titles.GetFameKarmaEntries(User);

            AddHtmlLocalized(55, 190, 160, 16, 1115031, 0xFFFF, false, false); // Fame/Karma
            AddCallbackButton(20, 190, 4005, 4007, 1, GumpButtonType.Reply, 0, b =>
                {
                    Category = TitleCategory.FameKarma;
                    Reset();
                    Refresh();
                });

            if (Category == TitleCategory.FameKarma)
            {
                if (!ShowingDescription)
                {
                    int index = 0;
                    int page = 1;

                    AddPage(page);

                    AddHtmlLocalized(260, 70, 160, 16, 1154764, 0xFFFF, false, false); // (DEFAULT)
                    AddCallbackButton(225, 70, 4005, 4007, 2, GumpButtonType.Reply, 0, b =>
                    {
                        ShowingDescription = true;
                        TitleSelected = 1500;
                        Refresh();
                    });

                    for (int i = 0; i < fameKarma.Count; i++)
                    {
                        AddHtml(260, 92 + (index * 22), 245, 16, Color("#FFFFFF", fameKarma[i]), false, false);
                        AddCallbackButton(225, 92 + (index * 22), 4005, 4007, 3 + i, GumpButtonType.Reply, 0, b =>
                        {
                            TitleSelected = b.ButtonID - 3;
                            ShowingDescription = true;
                            Refresh();
                        });

                        index++;
                        CheckPage(ref index, ref page, 8);
                    }
                }
                else
                {
                    string title = null;

                    if (fameKarma.Count == 0 || TitleSelected == 1500)
                        AddHtmlLocalized(275, 240, 160, 32, 1154764, 0xFFFF, false, false); // (DEFAULT)
                    else
                    {
                        if (TitleSelected >= 0 && TitleSelected < fameKarma.Count)
                        {
                            title = fameKarma[TitleSelected];
                            AddHtml(275, 240, 245, 16, Color("#FFFFFF", title), false, false);
                        }
                    }

                    AddHtmlLocalized(225, 70, 270, 200, 1115128, 0xFFFF, false, false); // This option will update your Fame/Karma title automatically during progression.
                    AddHtmlLocalized(225, 220, 160, 16, 1115026, 0xFFFF, false, false); // Paperdoll Name (Prefix)
                    AddHtmlLocalized(225, 275, 200, 16, 1115035, 0xFFFF, false, false); // Do you wish to apply this title?

                    AddHtmlLocalized(480, 275, 80, 16, 1011046, 0xFFFF, false, false); // APPLY
                    AddCallbackButton(445, 275, 4005, 4007, 99, GumpButtonType.Reply, 0, b =>
                        {
                            if (TitleSelected >= 0 && TitleSelected < fameKarma.Count)
                                title = fameKarma[TitleSelected];

                            if (title != null)
                                User.FameKarmaTitle = title;
                            else
                                User.FameKarmaTitle = null;

                            AddHtmlLocalized(225, 315, 200, 16, 1115036, 0xFFFF, false, false); // TITLE APPLIED
                            Refresh(false);
                        });
                }
            }
        }

        private void BuildPaperdollSuffix()
        {
            AddHtmlLocalized(55, 190, 160, 16, 1115030, 0xFFFF, false, false); // Skills
            AddCallbackButton(20, 190, 4005, 4007, 100, GumpButtonType.Reply, 0, b =>
            {
                Category = TitleCategory.Skills;
                Reset();
                Refresh();
            });

            PlayerMobile.ChampionTitleInfo info = User.ChampionTitles;

            if (info != null && info.HasChampionTitle(User))
            {
                AddHtmlLocalized(55, 212, 160, 16, 1115032, 0xFFFF, false, false); // Monster
                AddCallbackButton(20, 212, 4005, 4007, 101, GumpButtonType.Reply, 0, b =>
                {
                    Category = TitleCategory.Champion;
                    Reset();
                    Refresh();
                });
            }

            if (Category == TitleCategory.Skills)
            {
                if (!ShowingDescription || TitleSelected == -1)
                {
                    int index = 0;
                    int page = 1;

                    AddPage(page);

                    foreach (Skill sk in User.Skills)
                    {
                        if (sk.Base < 30)
                            continue;

                        string title = Titles.GetSkillTitle(User, sk);

                        if (title != null)
                        {
                            AddHtml(260, 70 + (index * 22), 245, 16, Color("#FFFFFF", title), false, false);
                        }

                        AddCallbackButton(225, 70 + (index * 22), 4005, 4007, sk.Info.SkillID + 102, GumpButtonType.Reply, 0, b =>
                            {
                                TitleSelected = b.ButtonID - 102;
                                ShowingDescription = true;
                                Refresh();
                            });

                        index++;

                        CheckPage(ref index, ref page);
                    }
                }
                else
                {
                    string str = Titles.GetSkillTitle(User, User.Skills[(SkillName)TitleSelected]);

                    AddHtmlLocalized(225, 70, 270, 200, 1115056 + TitleSelected, 0xFFFF, false, false);
                    AddHtmlLocalized(225, 220, 160, 16, 1115027, 0xFFFF, false, false); // Paperdoll Name (Suffix)

                    if (str != null)
                    {
                        AddHtml(275, 240, 245, 16, Color("#FFFFFF", str), false, false);
                    }

                    AddHtmlLocalized(225, 275, 200, 16, 1115035, 0xFFFF, false, false); // Do you wish to apply this title?

                    AddHtmlLocalized(480, 275, 80, 16, 1011046, 0xFFFF, false, false); // APPLY
                    AddCallbackButton(445, 275, 4005, 4007, 199, GumpButtonType.Reply, 0, b =>
                    {
                        AddHtmlLocalized(225, 315, 200, 16, 1115036, 0xFFFF, false, false); // TITLE APPLIED
                        str = Titles.GetSkillTitle(User, User.Skills[(SkillName)TitleSelected]);
                        Refresh(false);

                        User.PaperdollSkillTitle = str;
                    });
                }
            }
            else if (Category == TitleCategory.Champion && info != null)
            {
                if (!ShowingDescription || TitleSelected == -1)
                {
                    int y = 70;
                    int page = 1;
                    int index = 0;

                    AddPage(page);

                    if (info.Harrower > 0)
                    {
                        AddHtml(260, y, 245, 16, Color("#FFFFFF", String.Format(": {0} of Evil", Titles.HarrowerTitles[Math.Min(Titles.HarrowerTitles.Length, info.Harrower) - 1])), false, false);
                        AddCallbackButton(225, y, 4005, 4007, 295, GumpButtonType.Reply, 0, b =>
                        {
                            TitleSelected = 295;
                            ShowingDescription = true;
                            Refresh();
                        });

                        index++;
                    }

                    for (int i = 0; i < ChampionSpawnInfo.Table.Length; i++)
                    {
                        int v = info.GetValue(i);

                        if (v == 0)
                            continue;

                        int offset = 0;
                        if (v > 800)
                            offset = 3;
                        else if (v > 300)
                            offset = (int)(v / 300);

                        if (offset <= 0)
                            continue;

                        ChampionSpawnInfo champInfo = ChampionSpawnInfo.GetInfo((ChampionSpawnType)i);

                        AddHtml(260, y + (index * 22), 245, 16, Color("#FFFFFF", String.Format(": {0} of the {1}", champInfo.LevelNames[Math.Min(offset, champInfo.LevelNames.Length) - 1], champInfo.Name)), false, false);
                        AddCallbackButton(225, y + (index * 22), 4005, 4007, i + 251, GumpButtonType.Reply, 0, b =>
                        {
                            TitleSelected = b.ButtonID - 251;
                            ShowingDescription = true;
                            Refresh();
                        });

                        index++;
                        CheckPage(ref index, ref page);
                    }
                }
                else
                {
                    string str = GetChampionTitle();
                    object description = GetChampInfo();

                    if (description is int)
                        AddHtmlLocalized(225, 70, 270, 140, (int)description, 0xFFFF, false, false);
                    else if (description is string)
                        AddHtml(250, 70, 270, 140, Color("#FFFFFF", (string)description), false, false);

                    AddHtmlLocalized(225, 220, 160, 16, 1115027, 0xFFFF, false, false); // Paperdoll Name (Suffix)
                    AddHtml(275, 240, 245, 16, Color("#FFFFFF", str), false, false);

                    AddHtmlLocalized(225, 275, 200, 16, 1115035, 0xFFFF, false, false); // Do you wish to apply this title?

                    AddHtmlLocalized(480, 275, 80, 16, 1011046, 0xFFFF, false, false); // APPLY
                    AddCallbackButton(445, 275, 4005, 4007, 299, GumpButtonType.Reply, 0, b =>
                    {
                        AddHtmlLocalized(225, 315, 200, 16, 1115036, 0xFFFF, false, false); // TITLE APPLIED
                        str = GetChampionTitle();
                        User.DisplayChampionTitle = true;
                        User.CurrentChampTitle = str;

                        Refresh(false);
                    });
                }
            }
        }

        private void BuildOverheadName()
        {
            int y = 190;
            AddHtmlLocalized(55, y, 160, 16, 1115030, 0xFFFF, false, false); // Skills
            AddCallbackButton(20, y, 4005, 4007, 300, GumpButtonType.Reply, 0, b =>
            {
                Category = TitleCategory.Skills;
                ShowingDescription = false;
                TitleClearing = false;
                Refresh();
            });

            y += 22;

            List<int> rewards = GetCityTitles();

            if (rewards != null && rewards.Count > 0)
            {
                AddHtmlLocalized(55, y, 160, 16, 1115034, 0xFFFF, false, false); // Rewards
                AddCallbackButton(20, y, 4005, 4007, 5435, GumpButtonType.Reply, 0, b =>
                {
                    Category = TitleCategory.RewardTitles;
                    ShowingDescription = false;
                    TitleClearing = false;
                    Refresh();
                });

                y += 22;
            }

            Guild g = User.Guild as Guild;

            if (g != null)
            {
                AddHtmlLocalized(55, y, 160, 16, 1115033, 0xFFFF, false, false); // GUILD
                AddCallbackButton(20, y, 4005, 4007, 301, GumpButtonType.Reply, 0, b =>
                {
                    Category = TitleCategory.Guild;
                    ShowingDescription = false;
                    TitleClearing = false;
                    Refresh();
                });
            }

            if (Category == TitleCategory.Skills)
            {
                if (!ShowingDescription || TitleSelected == -1)
                {
                    int index = 0;
                    int page = 1;

                    AddPage(page);

                    foreach (Skill sk in User.Skills)
                    {
                        if (sk.Base < 30)
                            continue;

                        AddHtml(260, 70 + (index * 22), 245, 16, Color("#FFFFFF", "the " + sk.Info.Title), false, false);
                        AddCallbackButton(225, 70 + (index * 22), 4005, 4007, sk.Info.SkillID + 302, GumpButtonType.Reply, 0, b =>
                        {
                            TitleSelected = b.ButtonID - 302;
                            ShowingDescription = true;
                            Refresh();
                        });

                        index++;
                        CheckPage(ref index, ref page);

                        /*if ((SkillName)sk.Info.SkillID == User.Skills.CurrentMastery && User.Skills.CurrentMastery != SkillName.Alchemy)
                        {
                            AddHtml(260, 70 + (index * 22), 160, 16, Color("#FFFFFF", MasteryInfo.GetTitle(User)), false, false);
                            AddCallbackButton(225, 70 + (index * 22), 4005, 4007, 350, GumpButtonType.Reply, 0, b =>
                            {
                                TitleSelected = 350;
                                ShowingDescription = true;
                                Refresh();
                            });

                            index++;
                        }

                        CheckPage(ref index, ref page);*/
                    }
                }
                else
                {
                    string title;
                    int desc;

                    /*if (TitleSelected == 350)
                    {
                        title = MasteryInfo.GetTitle(User);
                        desc = MasteryInfo.GetDescription(User);
                    }
                    else
                    {
                        title = "the " + User.Skills[(SkillName)TitleSelected].Info.Title;
                        desc = 1115056 + TitleSelected;
                    }*/
                    title = "the " + User.Skills[(SkillName)TitleSelected].Info.Title;
                    desc = 1115056 + TitleSelected;

                    AddHtmlLocalized(225, 70, 270, 140, desc, 0xFFFF, false, false);
                    AddHtmlLocalized(225, 220, 160, 16, 1115028, 0xFFFF, false, false); // Overhead Name

                    AddHtml(275, 240, 245, 16, Color("#FFFFFF", title), false, false);
                    AddHtmlLocalized(225, 275, 200, 16, 1115035, 0xFFFF, false, false); // Do you wish to apply this title?

                    AddHtmlLocalized(480, 275, 80, 16, 1011046, 0xFFFF, false, false); // APPLY
                    AddCallbackButton(445, 275, 4005, 4007, 399, GumpButtonType.Reply, 0, b =>
                    {
                        AddHtmlLocalized(225, 315, 200, 16, 1115036, 0xFFFF, false, false); // TITLE APPLIED
                        title = "the " + User.Skills[(SkillName)TitleSelected].Info.Title;
                        User.OverheadTitle = title;

                        Refresh(false);
                    });
                }
            }
            else if (Category == TitleCategory.Guild && g != null)
            {
                if (!ShowingDescription || TitleSelected == -1)
                {
                    AddHtml(260, 70, 160, 16, Color("#FFFFFF", String.Format("[{0}]", g.Abbreviation)), false, false);
                    AddCallbackButton(225, 70, 4005, 4007, 397, GumpButtonType.Reply, 0, b =>
                        {
                            ShowingDescription = true;
                            TitleSelected = 351;
                            Refresh();
                        });
                }
                else
                {
                    AddHtmlLocalized(225, 65, 270, 140, 1115040, 0xFFFF, false, false); // This is your guild's abbreviation.
                    AddHtmlLocalized(225, 220, 160, 16, 1115028, 0xFFFF, false, false); // Overhead Name

                    AddHtml(275, 240, 245, 16, Color("#FFFFFF", String.Format("[{0}]", g.Abbreviation)), false, false);
                    AddHtmlLocalized(225, 275, 200, 16, 1115035, 0xFFFF, false, false); // Do you wish to apply this title?

                    AddHtmlLocalized(480, 275, 80, 16, 1011046, 0xFFFF, false, false); // APPLY
                    AddCallbackButton(445, 275, 4005, 4007, 398, GumpButtonType.Reply, 0, b =>
                        {
                            AddHtmlLocalized(225, 315, 200, 16, 1115036, 0xFFFF, false, false); // TITLE APPLIED
                            Refresh(false);

                            User.OverheadTitle = null;
                            User.ShowGuildAbbreviation = true;
                        });
                }
            }
            else if (Category == TitleCategory.RewardTitles && rewards != null && rewards.Count > 0)
            {
                if (!ShowingDescription || TitleSelected == -1)
                {
                    int index = 0;
                    int page = 1;

                    AddPage(page);

                    for (int i = 0; i < rewards.Count; i++)
                    {

                        int title = rewards[i];

                        if (title == 1154017)
                            continue;

                        AddHtmlLocalized(260, 70 + (index * 22), 245, 16, (int)title, 0xFFFF, false, false);

                        AddCallbackButton(225, 70 + (index * 22), 4005, 4007, i + 2550, GumpButtonType.Reply, 0, b =>
                        {
                            TitleSelected = b.ButtonID - 2550;
                            ShowingDescription = true;
                            Refresh();
                        });

                        index++;
                        CheckPage(ref index, ref page);
                    }
                }
                else if (TitleSelected >= 0 && TitleSelected < rewards.Count)
                {
                    int title = rewards[TitleSelected];
                    object description = GetRewardTitleInfo(title);

                    if (description is int)
                        AddHtmlLocalized(225, 70, 270, 140, (int)description, 0xFFFF, false, false);
                    else if (description is string)
                        AddHtml(225, 70, 270, 140, Color("#FFFFFF", (string)description), false, false);

                    AddHtmlLocalized(225, 220, 160, 16, 1115029, 0xFFFF, false, false); // Subtitle

                    string cust = null;
                    if (title == 1154017 && CityLoyaltySystem.HasCustomTitle(User, out cust))
                    {
                        AddHtmlLocalized(275, 240, 245, 16, 1154017, cust, 0xFFFF, false, false);
                    }
                    else
                        AddHtmlLocalized(275, 240, 160, 32, (int)title, 0xFFFF, false, false);

                    AddHtmlLocalized(225, 275, 200, 16, 1115035, 0xFFFF, false, false); // Do you wish to apply this title?

                    AddHtmlLocalized(480, 275, 80, 16, 1011046, 0xFFFF, false, false); // APPLY
                    AddCallbackButton(445, 275, 4005, 4007, 5789, GumpButtonType.Reply, 0, b =>
                    {
                        AddHtmlLocalized(225, 315, 200, 16, 1115036, 0xFFFF, false, false); // TITLE APPLIED
                        Refresh(false);
                        title = rewards[TitleSelected];
                        User.OverheadTitle = title.ToString();
                    });
                }
            }
        }

        private void BuildSubtitle()
        {
            int y = 190;

            Guild guild = User.Guild as Guild;

            AddHtmlLocalized(55, y, 160, 16, 1115030, 0xFFFF, false, false); // Skills
            AddCallbackButton(20, y, 4005, 4007, 400, GumpButtonType.Reply, 0, b =>
            {
                Category = TitleCategory.Skills;
                ShowingDescription = false;
                TitleClearing = false;
                Refresh();
            });

            y += 22;

            if (guild != null && User.GuildTitle != null)
            {
                AddHtmlLocalized(55, y, 160, 16, 1115033, 0xFFFF, false, false); // GUILD
                AddCallbackButton(20, y, 4005, 4007, 401, GumpButtonType.Reply, 0, b =>
                {
                    Category = TitleCategory.Guild;
                    ShowingDescription = false;
                    TitleClearing = false;
                    Refresh();
                });

                y += 22;
            }

            if (User.RewardTitles != null && User.RewardTitles.Count > 0)
            {
                AddHtmlLocalized(55, y, 160, 16, 1115034, 0xFFFF, false, false); // Rewards
                AddCallbackButton(20, y, 4005, 4007, 402, GumpButtonType.Reply, 0, b =>
                {
                    Category = TitleCategory.RewardTitles;
                    ShowingDescription = false;
                    TitleClearing = false;
                    Refresh();
                });

                y += 22;
            }

            List<VeteranTitle> vetTitles = Titles.GetVeteranTitles(User);
            
            if (vetTitles != null && vetTitles.Count > 0)
            {
                AddHtml(55, y, 160, 16, Color("#FFFFFF", "Veterans"), false, false); // Rewards
                AddCallbackButton(20, y, 4005, 4007, 403, GumpButtonType.Reply, 0, b =>
                {
                    Category = TitleCategory.Veteran;
                    ShowingDescription = false;
                    TitleClearing = false;
                    Refresh();
                });
            }

            if (Category == TitleCategory.Skills)
            {
                if (!ShowingDescription || TitleSelected == -1)
                {
                    int index = 0;
                    int page = 1;

                    AddPage(page);

                    foreach (Skill sk in User.Skills)
                    {
                        if (sk.Base < 30)
                            continue;

                        string title = Titles.GetSkillTitle(User, sk);

                        if (title != null)
                        {
                            AddHtml(260, 70 + (index * 22), 245, 16, Color("#FFFFFF", title), false, false);
                        }

                        AddCallbackButton(225, 70 + (index * 22), 4005, 4007, sk.Info.SkillID + 404, GumpButtonType.Reply, 0, b =>
                        {
                            TitleSelected = b.ButtonID - 404;
                            ShowingDescription = true;
                            Refresh();
                        });

                        index++;
                        CheckPage(ref index, ref page);
                    }
                }
                else
                {
                    string title = Titles.GetSkillTitle(User, User.Skills[(SkillName)TitleSelected]);

                    AddHtmlLocalized(225, 70, 270, 140, 1115056 + TitleSelected, 0xFFFF, false, false);
                    AddHtmlLocalized(225, 220, 160, 16, 1115029, 0xFFFF, false, false); // Subtitle
                    AddHtml(275, 240, 245, 16, Color("#FFFFFF", title), false, false);

                    AddHtmlLocalized(225, 275, 200, 16, 1115035, 0xFFFF, false, false); // Do you wish to apply this title?

                    AddHtmlLocalized(480, 275, 80, 16, 1011046, 0xFFFF, false, false); // APPLY
                    AddCallbackButton(445, 275, 4005, 4007, 102, GumpButtonType.Reply, 0, b =>
                    {
                        AddHtmlLocalized(225, 315, 200, 16, 1115036, 0xFFFF, false, false); // TITLE APPLIED
                        title = Titles.GetSkillTitle(User, User.Skills[(SkillName)TitleSelected]);

                        if (title != null)
                        {
                            User.SubtitleSkillTitle = title;

                            User.SelectRewardTitle(-1, true);
                            User.DisplayGuildTitle = false;
                        }

                        Refresh(false);
                    });
                }
            }
            else if (Category == TitleCategory.Guild && guild != null && User.GuildTitle != null)
            {
                if (!ShowingDescription || TitleSelected == -1)
                {
                    AddHtml(260, 70, 245, 16, Color("#FFFFFF", String.Format("{0}, {1}", Utility.FixHtml(User.GuildTitle), Utility.FixHtml(guild.Name))), false, false);
                    AddCallbackButton(225, 70, 4005, 4007, 500, GumpButtonType.Reply, 0, b =>
                    {
                        TitleSelected = 1;
                        ShowingDescription = true;
                        Refresh();
                    });
                }
                else
                {
                    AddHtmlLocalized(225, 70, 270, 140, 1115039, 0xFFFF, false, false); // This is a custom guild title assigned by your guild leader.
                    AddHtmlLocalized(225, 220, 160, 16, 1115029, 0xFFFF, false, false); // Subtitle
                    AddHtml(275, 240, 245, 16, Color("#FFFFFF", String.Format("{0}, {1}", Utility.FixHtml(User.GuildTitle), Utility.FixHtml(guild.Name))), false, false);

                    AddHtmlLocalized(225, 275, 200, 16, 1115035, 0xFFFF, false, false); // Do you wish to apply this title?

                    AddHtmlLocalized(480, 275, 80, 16, 1011046, 0xFFFF, false, false); // APPLY
                    AddCallbackButton(445, 275, 4005, 4007, 599, GumpButtonType.Reply, 0, b =>
                    {
                        AddHtmlLocalized(225, 315, 200, 16, 1115036, 0xFFFF, false, false); // TITLE APPLIED
                        User.DisplayGuildTitle = true;

                        if (User.SubtitleSkillTitle != null)
                            User.SubtitleSkillTitle = null;

                        User.SelectRewardTitle(-1, true);

                        Refresh(false);
                    });
                }
            }  
            else if (Category == TitleCategory.RewardTitles && User.RewardTitles != null && User.RewardTitles.Count > 0)
            {
                if (!ShowingDescription || TitleSelected == -1)
                {
                    int index = 0;
                    int page = 1;

                    AddPage(page);

                    for (int i = 0; i < User.RewardTitles.Count; i++)
                    {
                        object title = User.RewardTitles[i];

                        if (title is int)
                        {
                            string cust = null;

                            if ((int)title == 1154017 && CityLoyaltySystem.HasCustomTitle(User, out cust))
                            {
                                AddHtmlLocalized(260, 70 + (index * 22), 245, 16, 1154017, cust, 0xFFFF, false, false);
                            }
                            else
                                AddHtmlLocalized(260, 70 + (index * 22), 245, 16, (int)title, 0xFFFF, false, false);
                        }
                        else if (title is string)
                            AddHtml(260, 70 + (index * 22), 245, 16, Color("#FFFFFF", (string)title), false, false);

                        AddCallbackButton(225, 70 + (index * 22), 4005, 4007, i + 600, GumpButtonType.Reply, 0, b =>
                        {
                            TitleSelected = b.ButtonID - 600;
                            ShowingDescription = true;
                            Refresh();
                        });

                        index++;
                        CheckPage(ref index, ref page);
                    }
                }
                else if (TitleSelected >= 0 && User.RewardTitles != null && TitleSelected < User.RewardTitles.Count)
                {
                    object title = User.RewardTitles[TitleSelected];
                    object description = GetRewardTitleInfo(title);

                    if (description is int)
                        AddHtmlLocalized(225, 70, 270, 140, (int)description, 0xFFFF, false, false);
                    else if (description is string)
                        AddHtml(225, 70, 270, 140, Color("#FFFFFF", (string)description), false, false);

                    AddHtmlLocalized(225, 220, 160, 16, 1115029, 0xFFFF, false, false); // Subtitle

                    if (title is int)
                    {
                        string cust = null;

                        if ((int)title == 1154017 && CityLoyaltySystem.HasCustomTitle(User, out cust))
                        {
                            AddHtmlLocalized(275, 240, 245, 16, 1154017, cust, 0xFFFF, false, false);
                        } 
                        else
                            AddHtmlLocalized(275, 240, 160, 32, (int)title, 0xFFFF, false, false);
                    }
                    else
                        AddHtml(275, 240, 245, 16, Color("#FFFFFF", (string)title), false, false);

                    AddHtmlLocalized(225, 275, 200, 16, 1115035, 0xFFFF, false, false); // Do you wish to apply this title?

                    AddHtmlLocalized(480, 275, 80, 16, 1011046, 0xFFFF, false, false); // APPLY
                    AddCallbackButton(445, 275, 4005, 4007, 699, GumpButtonType.Reply, 0, b =>
                    {
                        AddHtmlLocalized(225, 315, 200, 16, 1115036, 0xFFFF, false, false); // TITLE APPLIED
                        Refresh(false);

                        User.SelectRewardTitle(TitleSelected, true);

                        if (User.SubtitleSkillTitle != null)
                            User.SubtitleSkillTitle = null;

                        User.DisplayGuildTitle = false;
                    });
                }
            }
            else if (Category == TitleCategory.Veteran && vetTitles != null && vetTitles.Count > 0)
            {
                if (!ShowingDescription || TitleSelected == -1)
                {
                    int index = 0;
                    int page = 1;

                    AddPage(page);

                    for (int i = 0; i < vetTitles.Count; i++)
                    {
                        AddHtmlLocalized(260, 70 + (index * 22), 245, 16, vetTitles[i].Title, 0xFFFF, false, false);
                        AddCallbackButton(225, 70 + (index * 22), 4005, 4007, i + 700, GumpButtonType.Reply, 0, b =>
                            {
                                TitleSelected = b.ButtonID - 700;
                                ShowingDescription = true;
                                Refresh();
                            });

                        index++;
                        CheckPage(ref index, ref page);
                    }
                }
                else if(TitleSelected >= 0 && TitleSelected < vetTitles.Count)
                {
                    VeteranTitle title = vetTitles[TitleSelected];

                    AddHtmlLocalized(225, 70, 270, 200, title.Title + 410, 0xFFFF, false, false);
                    AddHtmlLocalized(225, 220, 160, 16, 1115029, 0xFFFF, false, false); // Subtitle

                    AddHtmlLocalized(275, 240, 160, 32, title.Title, 0xFFFF, false, false);

                    AddHtmlLocalized(225, 275, 200, 16, 1115035, 0xFFFF, false, false); // Do you wish to apply this title?

                    AddHtmlLocalized(480, 275, 80, 16, 1011046, 0xFFFF, false, false); // APPLY
                    AddCallbackButton(445, 275, 4005, 4007, 799, GumpButtonType.Reply, 0, b =>
                    {
                        AddHtmlLocalized(225, 315, 200, 16, 1115036, 0xFFFF, false, false); // TITLE APPLIED
                        title = vetTitles[TitleSelected];
                        User.CurrentVeteranTitle = title.Title;

                        Refresh(false);
                    });
                }
            }
        }

        public override void OnResponse(Server.Network.NetState state, RelayInfo info)
        {
            GumpButton button = ButtonCallbacks.Keys.FirstOrDefault(b => b.ButtonID == info.ButtonID);

            if (ButtonCallbacks.ContainsKey(button) && ButtonCallbacks[button] != null)
                ButtonCallbacks[button](button);
        }

        public void CheckPage(ref int index, ref int page, int perpage = 9)
        {
            if (index >= perpage)
            {
                AddHtmlLocalized(415, 275, 100, 16, 1044045, 0xFFFF, false, false); // NEXT PAGE
                AddCallbackButton(380, 275, 4005, 4007, 0, GumpButtonType.Page, page + 1, b => { });

                page++;
                index = 0;
                AddPage(page);

                AddHtmlLocalized(265, 275, 100, 16, 1044044, 0xFFFF, false, false); // PREV PAGE
                AddCallbackButton(225, 275, 4014, 4016, 0, GumpButtonType.Page, page - 1, b => { });
            }
        }

        private bool HasTitle()
        {
            switch (TitleType)
            {
                case TitleType.PaperdollSuffix:
                    return User.PaperdollSkillTitle != null || User.CurrentChampTitle != null;
                case TitleType.OverheadName:
                    return User.OverheadTitle != null || User.ShowGuildAbbreviation;
                case TitleType.SubTitles:
                    return User.SubtitleSkillTitle != null || User.SelectedTitle > -1 || User.CurrentVeteranTitle > 0 || User.DisplayGuildTitle;
            }

            return false;
        }

        private bool ClearTitle()
        {
            switch (TitleType)
            {
                case TitleType.PaperdollSuffix:
                    User.PaperdollSkillTitle = null;
                    User.CurrentChampTitle = null;
                    break;
                case TitleType.OverheadName:
                    User.OverheadTitle = null;
                    User.DisplayGuildTitle = false;
                    User.ShowGuildAbbreviation = false;
                    break;
                case TitleType.SubTitles:
                    User.SubtitleSkillTitle = null;
                    User.SelectRewardTitle(-1, true);
                    User.CurrentVeteranTitle = -1;
                    User.DisplayGuildTitle = false;
                    break;
            }

            return true;
        }

        private string GetChampionTitle()
        {
            PlayerMobile.ChampionTitleInfo info = User.ChampionTitles;

            int v;
            string str = null;

            if (TitleSelected == 295)
            {
                v = info.Harrower;

                if (v > 0)
                    str = String.Format(": {0} of Evil", Titles.HarrowerTitles[Math.Min(Titles.HarrowerTitles.Length, info.Harrower) - 1]);
            }
            else
            {
                v = info.GetValue(TitleSelected);

                ChampionSpawnInfo champInfo = ChampionSpawnInfo.GetInfo((ChampionSpawnType)TitleSelected);

                int offset = 0;
                if (v > 800)
                    offset = 3;
                else if (v > 300)
                    offset = (int)(v / 300);

                if (offset > 0)
                {
                    str = String.Format(": {0} of the {1}", champInfo.LevelNames[Math.Min(offset, champInfo.LevelNames.Length) - 1], champInfo.Name);
                }
            }

            return str;
        }

        private object GetChampInfo()
        {
            int index = TitleSelected;

            switch (index)
            {
                default: return "Unknown Champion";
                case 0: return 1115047; // This title is gained from battling against the minions of Semidar.
                case 1: return 1115046; // This title is gained from battling against the minions of Mephitis.
                case 2: return 1115044; // This title is gained from battling against the minions of Rikktor.
                case 3: return 1115045; // This title is gained from battling against the minions of Lord Oaks.
                case 4: return 1115048; // This title is gained from battling against the minions of Barracoon the Piper.
                case 5: return 1115049; // This title is gained from battling against the minions of Neira the Necromancer.
                case 6: return 1115050; // This title is gained from battling against the minions of Serado the Awakened.
                case 7: return 1115052; // This title is gained from battling against the minions of Twaulo of the Glade.
                case 8: return 1115051; // This title is gained from battling against the minions of Ilhenir the Stained.
                case 12: return 1115054; // This title is gained from battling against the minions of the Abyssal Inferno.
                case 13: return 1115053; // This title is gained from battling against the minions of the Primeval Lich.
                case 14: return "This title is gained from battling against the minions of the Dragon Turtle."; // Dragon Turtle, no cliloc???
                case 295: return 1115055; // This title is gained from battling against the Harrower and the Champions.
            }
        }

        private object GetRewardTitleInfo(object title)
        {
            if (!(title is int))
            {
                if (title is string)
                    return title;

                return null;
            }

            if (title is int)
            {
                int id = (int)title;

                if ((id >= 1152739 && id <= 1152893) || id == 1154017)
                    return 1152893; // This title is gained through city loyalty.

                if ((id >= 1151739 && id <= 1151747) || id == 1155481 || id == 1154505)
                    return id + 1;

                if (id >= 1157181 && id <= 1157203)
                    return 1157180; // This title is obtained from turning in Bulk Order Deeds.

                if (id >= 1156985 && id <= 1156987)
                    return 1156984; // This title is obtained from the Halloween Treasures of the Kotl City Event.	

                if (id >= 1152068 && id <= 1152073)
                    return 1152075; // This is a reward title given for your valorous fights in arenas.

                switch (id)
                {
                    default: return "This reward title has no desciption.";
                    case 1073235:
                    case 1073236:
                    case 1073237:
                    case 1073238:
                    case 1073239: return 1115115; // This is a reward title given for contributing to the Vesper Museum.
                    case 1073201:
                    case 1073202:
                    case 1073203:
                    case 1073204:
                    case 1073205:
                    case 1073206: return 1115116; // This is a reward title given for contributing to the Britannia Royal Zoo.
                    case 1073341:
                    case 1073342:
                    case 1073343:
                    case 1073344:
                    case 1073345: return 1115117; // This is a reward title given for contributing to the Britain Public Library.
                    case 1156477: return 1156476; // This title is obtained by completing the Valley of One Quest in the Valley of Eodon.
                    case 1155727: return 1155728; // This title is obtained from the Huntmaster's Challenge at the Ranger's Guild in Skara Brae.
                    case 1156318: return 1156319; // This is a reward title given for completing Shadowguard.
                    case 1157594: return 1157595; // This title is obtained by completing the "Discovering Animal Training" quest.
                    case 1158140: return 1158141; // This title is obtained by completing the Huntmaster's Challenge Quest.
                    case 1158090: return 1158087; // This title is obtained by completing the Paladins of Trinsic Quest.
                    case 1158161: return 1158162; // This title is obtained by completing the Righting Wrong Quest.	
                    case 1158389: return 1158248; // This title is obtained by completing the Treasure Chase Quest.
                    case 1158278: return 1158279; // This title is obtained by completing the "A Forced Sacrifice" quest.
                    case 1158303: return 1158324; // This title is obtained by completing the "Whispering with Wisps" quest.
                    case 1154505: return 1154506; // This is a reward title given for completing the Exploring the Deep Quest.
                }
            }

            //Future: 1156717 This title is obtained by completing the Myrmidex Threat quest in the Valley of Eodon.
            return "This reward title has no desciption.";
        }

        private bool IsCityTitle(int id)
        {
            return (id >= 1152739 && id <= 1152893) || (id >= 1151739 && id <= 1151747) || id == 1155481 || id == 1154017;
        }

        public List<int> GetCityTitles()
        {
            IEnumerable<int> list = User.RewardTitles.OfType<int>().Where(i => IsCityTitle(i));

            return list.ToList();
        }

        public void Refresh(bool recompile = true)
        {
            if (recompile)
            {
                Entries.Clear();
                Entries.TrimExcess();
                AddGumpLayout();
            }

            User.CloseGump(this.GetType());
            User.SendGump(this, false);
        }

        private string Color(string color, string str)
        {
            return String.Format("<basefont color={0}>{1}", color, str);
        }
    }
}