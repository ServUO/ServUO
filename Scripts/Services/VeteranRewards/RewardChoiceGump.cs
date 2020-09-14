using Server.Gumps;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Engines.VeteranRewards
{
    public class RewardChoiceGump : Gump
    {
        private readonly Mobile m_From;
        public RewardChoiceGump(Mobile from)
            : base(0, 0)
        {
            m_From = from;

            from.CloseGump(typeof(RewardChoiceGump));

            RenderBackground();
            RenderCategories();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int buttonID = info.ButtonID - 1;

            if (buttonID == 0)
            {
                int cur, max;

                RewardSystem.ComputeRewardInfo(m_From, out cur, out max);

                if (cur < max)
                    m_From.SendGump(new RewardNoticeGump(m_From));
            }
            else
            {
                --buttonID;

                int type = (buttonID % 20);
                int index = (buttonID / 20);

                RewardCategory[] categories = RewardSystem.Categories;

                if (type >= 0 && type < categories.Length)
                {
                    RewardCategory category = categories[type];

                    if (index >= 0 && index < category.Entries.Count)
                    {
                        RewardEntry entry = category.Entries[index];

                        if (!RewardSystem.HasAccess(m_From, entry))
                            return;

                        m_From.SendGump(new RewardConfirmGump(m_From, entry));
                    }
                }
            }
        }

        private void RenderBackground()
        {
            AddPage(0);

            AddBackground(10, 10, 600, 450, 2600);

            AddButton(530, 415, 4017, 4019, 0, GumpButtonType.Reply, 0);

            AddButton(60, 415, 4014, 4016, 0, GumpButtonType.Page, 1);
            AddHtmlLocalized(95, 415, 200, 20, 1049755, false, false); // Main Menu
        }

        private void RenderCategories()
        {
            TimeSpan rewardInterval = RewardSystem.RewardInterval;

            string intervalAsString;

            if (rewardInterval == TimeSpan.FromDays(30.0))
                intervalAsString = "month";
            else if (rewardInterval == TimeSpan.FromDays(60.0))
                intervalAsString = "two months";
            else if (rewardInterval == TimeSpan.FromDays(90.0))
                intervalAsString = "three months";
            else if (rewardInterval == TimeSpan.FromDays(365.0))
                intervalAsString = "year";
            else
                intervalAsString = string.Format("{0} day{1}", rewardInterval.TotalDays, rewardInterval.TotalDays == 1 ? "" : "s");

            AddPage(1);

            AddHtml(60, 35, 500, 70, "<B>Ultima Online Rewards Program</B><BR>" +
                                          "Thank you for being a part of the Ultima Online community for a full " + intervalAsString + ".  " +
                                          "As a token of our appreciation,  you may select from the following in-game reward items listed below.  " +
                                          "The gift items will be attributed to the character you have logged-in with on the shard you are on when you chose the item(s).  " +
                                          "The number of rewards you are entitled to are listed below and are for your entire account.  " +
                                          "To read more about these rewards before making a selection, feel free to visit the uo.com site at " +
                                          "<A HREF=\"http://www.uo.com/rewards\">http://www.uo.com/rewards</A>.", true, true);

            int cur, max;

            RewardSystem.ComputeRewardInfo(m_From, out cur, out max);

            AddHtmlLocalized(60, 105, 300, 35, 1006006, false, false); // Your current total of rewards to choose:
            AddLabel(370, 107, 50, (max - cur).ToString());

            AddHtmlLocalized(60, 140, 300, 35, 1006007, false, false); // You have already chosen:
            AddLabel(370, 142, 50, cur.ToString());

            RewardCategory[] categories = RewardSystem.Categories;

            int page = 2;

            for (int i = 0; i < categories.Length; ++i)
            {
                if (!RewardSystem.HasAccess(m_From, categories[i]))
                {
                    page += 1;
                    continue;
                }

                AddButton(100, 180 + (i * 40), 4005, 4005, 0, GumpButtonType.Page, page);

                page += PagesPerCategory(categories[i]);

                if (categories[i].NameString != null)
                    AddHtml(135, 180 + (i * 40), 300, 20, categories[i].NameString, false, false);
                else
                    AddHtmlLocalized(135, 180 + (i * 40), 300, 20, categories[i].Name, false, false);
            }

            page = 2;

            for (int i = 0; i < categories.Length; ++i)
                RenderCategory(categories[i], i, ref page);
        }

        private int PagesPerCategory(RewardCategory category)
        {
            List<RewardEntry> entries = category.Entries;
            int j = 0, i = 0;

            for (j = 0; j < entries.Count; j++)
            {
                if (RewardSystem.HasAccess(m_From, entries[j]))
                    i++;
            }

            return (int)Math.Ceiling(i / 24.0);
        }

        private int GetButtonID(int type, int index)
        {
            return 2 + (index * 20) + type;
        }

        private void RenderCategory(RewardCategory category, int index, ref int page)
        {
            AddPage(page);

            List<RewardEntry> entries = category.Entries;

            int i = 0;

            for (int j = 0; j < entries.Count; ++j)
            {
                RewardEntry entry = entries[j];

                if (!RewardSystem.HasAccess(m_From, entry))
                    continue;

                if (i == 24)
                {
                    AddButton(305, 415, 0xFA5, 0xFA7, 0, GumpButtonType.Page, ++page);
                    AddHtmlLocalized(340, 415, 200, 20, 1011066, false, false); // Next page

                    AddPage(page);

                    AddButton(270, 415, 0xFAE, 0xFB0, 0, GumpButtonType.Page, page - 1);
                    AddHtmlLocalized(185, 415, 200, 20, 1011067, false, false); // Previous page

                    i = 0;
                }

                AddButton(55 + ((i / 12) * 250), 80 + ((i % 12) * 25), 5540, 5541, GetButtonID(index, j), GumpButtonType.Reply, 0);

                if (entry.NameString != null)
                    AddHtml(80 + ((i / 12) * 250), 80 + ((i % 12) * 25), 250, 20, entry.NameString, false, false);
                else
                    AddHtmlLocalized(80 + ((i / 12) * 250), 80 + ((i % 12) * 25), 250, 20, entry.Name, false, false);
                ++i;
            }

            page += 1;
        }
    }
}