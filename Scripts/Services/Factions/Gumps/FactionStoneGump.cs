using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Factions
{
    public class FactionStoneGump : FactionGump
    {
        private readonly PlayerMobile m_From;
        private readonly Faction m_Faction;

        public override int ButtonTypes
        {
            get
            {
                return 4;
            }
        }

        public FactionStoneGump(PlayerMobile from, Faction faction)
            : base(20, 30)
        {
            m_From = from;
            m_Faction = faction;

            AddPage(0);

            AddBackground(0, 0, 550, 440, 5054);
            AddBackground(10, 10, 530, 420, 3000);

            #region General
            AddPage(1);

            AddHtmlText(20, 30, 510, 20, faction.Definition.Header, false, false);

            AddHtmlLocalized(20, 60, 100, 20, 1011429, false, false); // Led By : 
            AddHtml(125, 60, 200, 20, faction.Commander != null ? faction.Commander.Name : "Nobody", false, false);

            AddHtmlLocalized(20, 80, 100, 20, 1011457, false, false); // Tithe rate : 
            if (faction.Tithe >= 0 && faction.Tithe <= 100 && (faction.Tithe % 10) == 0)
                AddHtmlLocalized(125, 80, 350, 20, 1011480 + (faction.Tithe / 10), false, false);
            else
                AddHtml(125, 80, 350, 20, faction.Tithe + "%", false, false);

            AddHtmlLocalized(20, 100, 100, 20, 1011458, false, false); // Traps placed : 
            AddHtml(125, 100, 50, 20, faction.Traps.Count.ToString(), false, false);

            AddHtmlLocalized(55, 225, 200, 20, 1011428, false, false); // VOTE FOR LEADERSHIP

            if (m_Faction.Election != null)
            {
                AddButton(20, 225, 4005, 4007, ToButtonID(0, 0), GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(55, 150, 100, 20, 1011430, false, false); // CITY STATUS
            AddButton(20, 150, 4005, 4007, 0, GumpButtonType.Page, 2);

            AddHtmlLocalized(55, 175, 100, 20, 1011444, false, false); // STATISTICS
            AddButton(20, 175, 4005, 4007, 0, GumpButtonType.Page, 4);

            bool isMerchantQualified = MerchantTitles.HasMerchantQualifications(from);

            PlayerState pl = PlayerState.Find(from);

            if (pl != null && pl.MerchantTitle != MerchantTitle.None)
            {
                AddHtmlLocalized(55, 200, 250, 20, 1011460, false, false); // UNDECLARE FACTION MERCHANT
                AddButton(20, 200, 4005, 4007, ToButtonID(1, 0), GumpButtonType.Reply, 0);
            }
            else if (isMerchantQualified)
            {
                AddHtmlLocalized(55, 200, 250, 20, 1011459, false, false); // DECLARE FACTION MERCHANT
                AddButton(20, 200, 4005, 4007, 0, GumpButtonType.Page, 5);
            }
            else
            {
                AddHtmlLocalized(55, 200, 250, 20, 1011467, false, false); // MERCHANT OPTIONS
                AddImage(20, 200, 4020);
            }

            AddHtmlLocalized(55, 250, 300, 20, 1011461, false, false); // COMMANDER OPTIONS
            if (faction.IsCommander(from))
                AddButton(20, 250, 4005, 4007, 0, GumpButtonType.Page, 6);
            else
                AddImage(20, 250, 4020);

            AddHtmlLocalized(55, 275, 300, 20, 1011426, false, false); // LEAVE THIS FACTION
            AddButton(20, 275, 4005, 4007, ToButtonID(0, 1), GumpButtonType.Reply, 0);

            AddHtmlLocalized(55, 300, 200, 20, 1011441, false, false); // EXIT
            AddButton(20, 300, 4005, 4007, 0, GumpButtonType.Reply, 0);
            #endregion

            #region City Status
            AddPage(2);

            AddHtmlLocalized(20, 30, 250, 20, 1011430, false, false); // CITY STATUS

            List<Town> towns = Town.Towns;

            for (int i = 0; i < towns.Count; ++i)
            {
                Town town = towns[i];

                AddHtmlText(40, 55 + (i * 30), 150, 20, town.Definition.TownName, false, false);

                if (town.Owner == null)
                {
                    AddHtmlLocalized(200, 55 + (i * 30), 150, 20, 1011462, false, false); // : Neutral
                }
                else
                {
                    AddHtmlLocalized(200, 55 + (i * 30), 150, 20, town.Owner.Definition.OwnerLabel, false, false);

                    BaseMonolith monolith = town.Monolith;

                    AddImage(20, 60 + (i * 30), (monolith != null && monolith.Sigil != null && monolith.Sigil.IsPurifying) ? 0x938 : 0x939);
                }
            }

            AddImage(20, 300, 2361);
            AddHtmlLocalized(45, 295, 300, 20, 1011491, false, false); // sigil may be recaptured

            AddImage(20, 320, 2360);
            AddHtmlLocalized(45, 315, 300, 20, 1011492, false, false); // sigil may not be recaptured

            AddHtmlLocalized(55, 350, 100, 20, 1011447, false, false); // BACK
            AddButton(20, 350, 4005, 4007, 0, GumpButtonType.Page, 1);
            #endregion

            #region Statistics
            AddPage(4);

            AddHtmlLocalized(20, 30, 150, 20, 1011444, false, false); // STATISTICS

            AddHtmlLocalized(20, 100, 100, 20, 1011445, false, false); // Name : 
            AddHtml(120, 100, 150, 20, from.Name, false, false);

            AddHtmlLocalized(20, 130, 100, 20, 1018064, false, false); // score : 
            AddHtml(120, 130, 100, 20, (pl != null ? pl.KillPoints : 0).ToString(), false, false);

            AddHtmlLocalized(20, 160, 100, 20, 1011446, false, false); // Rank : 
            AddHtml(120, 160, 100, 20, (pl != null ? pl.Rank.Rank : 0).ToString(), false, false);

            AddHtmlLocalized(55, 250, 100, 20, 1011447, false, false); // BACK
            AddButton(20, 250, 4005, 4007, 0, GumpButtonType.Page, 1);
            #endregion

            #region Merchant Options
            if ((pl == null || pl.MerchantTitle == MerchantTitle.None) && isMerchantQualified)
            {
                AddPage(5);

                AddHtmlLocalized(20, 30, 250, 20, 1011467, false, false); // MERCHANT OPTIONS

                AddHtmlLocalized(20, 80, 300, 20, 1011473, false, false); // Select the title you wish to display

                MerchantTitleInfo[] infos = MerchantTitles.Info;

                for (int i = 0; i < infos.Length; ++i)
                {
                    MerchantTitleInfo info = infos[i];

                    if (MerchantTitles.IsQualified(from, info))
                        AddButton(20, 100 + (i * 30), 4005, 4007, ToButtonID(1, i + 1), GumpButtonType.Reply, 0);
                    else
                        AddImage(20, 100 + (i * 30), 4020);

                    AddHtmlText(55, 100 + (i * 30), 200, 20, info.Label, false, false);
                }

                AddHtmlLocalized(55, 340, 100, 20, 1011447, false, false); // BACK
                AddButton(20, 340, 4005, 4007, 0, GumpButtonType.Page, 1);
            }
            #endregion

            #region Commander Options
            if (faction.IsCommander(from))
            {
                #region General
                AddPage(6);

                AddHtmlLocalized(20, 30, 200, 20, 1011461, false, false); // COMMANDER OPTIONS

                AddHtmlLocalized(20, 70, 120, 20, 1011457, false, false); // Tithe rate : 
                if (faction.Tithe >= 0 && faction.Tithe <= 100 && (faction.Tithe % 10) == 0)
                    AddHtmlLocalized(140, 70, 250, 20, 1011480 + (faction.Tithe / 10), false, false);
                else
                    AddHtml(140, 70, 250, 20, faction.Tithe + "%", false, false);

                AddHtmlLocalized(20, 100, 120, 20, 1011474, false, false); // Silver available : 
                AddHtml(140, 100, 50, 20, faction.Silver.ToString("N0"), false, false); // NOTE: Added 'N0' formatting

                AddHtmlLocalized(55, 130, 200, 20, 1011478, false, false); // CHANGE TITHE RATE
                AddButton(20, 130, 4005, 4007, 0, GumpButtonType.Page, 8);

                AddHtmlLocalized(55, 160, 200, 20, 1018301, false, false); // TRANSFER SILVER
                if (faction.Silver >= 10000)
                    AddButton(20, 160, 4005, 4007, 0, GumpButtonType.Page, 7);
                else
                    AddImage(20, 160, 4020);

                AddHtmlLocalized(55, 310, 100, 20, 1011447, false, false); // BACK
                AddButton(20, 310, 4005, 4007, 0, GumpButtonType.Page, 1);
                #endregion

                #region Town Finance
                if (faction.Silver >= 10000)
                {
                    AddPage(7);

                    AddHtmlLocalized(20, 30, 250, 20, 1011476, false, false); // TOWN FINANCE

                    AddHtmlLocalized(20, 50, 400, 20, 1011477, false, false); // Select a town to transfer 10000 silver to

                    for (int i = 0; i < towns.Count; ++i)
                    {
                        Town town = towns[i];

                        AddHtmlText(55, 75 + (i * 30), 200, 20, town.Definition.TownName, false, false);

                        if (town.Owner == faction)
                            AddButton(20, 75 + (i * 30), 4005, 4007, ToButtonID(2, i), GumpButtonType.Reply, 0);
                        else
                            AddImage(20, 75 + (i * 30), 4020);
                    }

                    AddHtmlLocalized(55, 310, 100, 20, 1011447, false, false); // BACK
                    AddButton(20, 310, 4005, 4007, 0, GumpButtonType.Page, 1);
                }
                #endregion

                #region Change Tithe Rate
                AddPage(8);

                AddHtmlLocalized(20, 30, 400, 20, 1011479, false, false); // Select the % for the new tithe rate

                int y = 55;

                for (int i = 0; i <= 10; ++i)
                {
                    if (i == 5)
                        y += 5;

                    AddHtmlLocalized(55, y, 300, 20, 1011480 + i, false, false);
                    AddButton(20, y, 4005, 4007, ToButtonID(3, i), GumpButtonType.Reply, 0);

                    y += 20;

                    if (i == 5)
                        y += 5;
                }

                AddHtmlLocalized(55, 310, 300, 20, 1011447, false, false); // BACK
                AddButton(20, 310, 4005, 4007, 0, GumpButtonType.Page, 1);
                #endregion
            }
            #endregion
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int type, index;

            if (!FromButtonID(info.ButtonID, out type, out index))
                return;

            switch ( type )
            {
                case 0: // general
                    {
                        switch ( index )
                        {
                            case 0: // vote
                                {
                                    m_From.SendGump(new ElectionGump(m_From, m_Faction.Election));
                                    break;
                                }
                            case 1: // leave
                                {
                                    m_From.SendGump(new LeaveFactionGump(m_From, m_Faction));
                                    break;
                                }
                        }

                        break;
                    }
                case 1: // merchant title
                    {
                        if (index >= 0 && index <= MerchantTitles.Info.Length)
                        {
                            PlayerState pl = PlayerState.Find(m_From);

                            MerchantTitle newTitle = (MerchantTitle)index;
                            MerchantTitleInfo mti = MerchantTitles.GetInfo(newTitle);

                            if (mti == null)
                            {
                                m_From.SendLocalizedMessage(1010120); // Your merchant title has been removed

                                if (pl != null)
                                    pl.MerchantTitle = newTitle;
                            }
                            else if (MerchantTitles.IsQualified(m_From, mti))
                            {
                                m_From.SendLocalizedMessage(mti.Assigned);

                                if (pl != null)
                                    pl.MerchantTitle = newTitle;
                            }
                        }

                        break;
                    }
                case 2: // transfer silver
                    {
                        if (!m_Faction.IsCommander(m_From))
                            return;

                        List<Town> towns = Town.Towns;

                        if (index >= 0 && index < towns.Count)
                        {
                            Town town = towns[index];

                            if (town.Owner == m_Faction)
                            {
                                if (m_Faction.Silver >= 10000)
                                {
                                    m_Faction.Silver -= 10000;
                                    town.Silver += 10000;

                                    // 10k in silver has been received by:
                                    m_From.SendLocalizedMessage(1042726, true, " " + town.Definition.FriendlyName);
                                }
                            }
                        }

                        break;
                    }
                case 3: // change tithe
                    {
                        if (!m_Faction.IsCommander(m_From))
                            return;

                        if (index >= 0 && index <= 10)
                            m_Faction.Tithe = index * 10;

                        break;
                    }
            }
        }
    }
}