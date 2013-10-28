using System;
using System.Collections;
using Server.Engines.XmlSpawner2;
using Server.Items;

//
// XmlLogGump
// modified from RC0 BOBGump.cs
//
namespace Server.Gumps
{
    public class QuestLogGump : Gump
    {
        private const int LabelColor = 0x7FFF;
        private readonly Mobile m_From;
        private readonly ArrayList m_List;
        private readonly int m_Page;
        public QuestLogGump(Mobile from)
            : this(from, 0, null)
        {
        }

        public QuestLogGump(Mobile from, int page, ArrayList list)
            : base(12, 24)
        {
            if (from == null)
                return;

            from.CloseGump(typeof(QuestLogGump));

            XmlQuestPoints p = (XmlQuestPoints)XmlAttach.FindAttachment(from, typeof(XmlQuestPoints));

            this.m_From = from;
            this.m_Page = page;

            if (list == null)
            {
                // make a new list based on the number of items in the book
                int nquests = 0;
                list = new ArrayList();

                // find all quest items in the players pack
                if (from.Backpack != null)
                {
                    Item[] packquestitems = from.Backpack.FindItemsByType(typeof(IXmlQuest));

                    if (packquestitems != null)
                    {
                        nquests += packquestitems.Length;
                        for (int i = 0; i < packquestitems.Length; ++i)
                        {
                            if (packquestitems[i] != null && !packquestitems[i].Deleted && !(packquestitems[i].Parent is XmlQuestBook))
                                list.Add(packquestitems[i]);
                        }
                    }

                    // find any questbooks they might have
                    Item[] questbookitems = from.Backpack.FindItemsByType(typeof(XmlQuestBook));

                    if (questbookitems != null)
                    {
                        for (int j = 0; j < questbookitems.Length; ++j)
                        {
                            Item[] questitems = ((XmlQuestBook)questbookitems[j]).FindItemsByType(typeof(IXmlQuest));

                            if (questitems != null)
                            {
                                nquests += questitems.Length;
				
                                for (int i = 0; i < questitems.Length; ++i)
                                {
                                    list.Add(questitems[i]);
                                }
                            }
                        }
                    }

                    // find any completed quests on the XmlQuestPoints attachment

                    if (p != null && p.QuestList != null)
                    {
                        // add all completed quests
                        foreach (XmlQuestPoints.QuestEntry q in p.QuestList)
                        {
                            list.Add(q);
                        }
                    }
                }
            }

            this.m_List = list;

            int index = this.GetIndexForPage(page);
            int count = this.GetCountForIndex(index);

            int tableIndex = 0;

            int width = 600;

            width = 766;

            this.X = (824 - width) / 2;
			
            int xoffset = 20;

            this.AddPage(0);

            this.AddBackground(10, 10, width, 439, 5054);
            this.AddImageTiled(18, 20, width - 17, 420, 2624);

            this.AddImageTiled(58 - xoffset, 64, 36, 352, 200); // open
            this.AddImageTiled(96 - xoffset, 64, 163, 352, 1416);  // name
            this.AddImageTiled(261 - xoffset, 64, 55, 352, 200); // type
            this.AddImageTiled(308 - xoffset, 64, 85, 352, 1416);  // status
            this.AddImageTiled(395 - xoffset, 64, 116, 352, 200);  // expires

            this.AddImageTiled(511 - xoffset, 64, 42, 352, 1416);  // points
            this.AddImageTiled(555 - xoffset, 64, 175, 352, 200);  // completed
            this.AddImageTiled(734 - xoffset, 64, 42, 352, 1416);  // repeated

            for (int i = index; i < (index + count) && i >= 0 && i < list.Count; ++i)
            {
                object obj = list[i];

                this.AddImageTiled(24, 94 + (tableIndex * 32), 489, 2, 2624);

                ++tableIndex;
            }

            this.AddAlphaRegion(18, 20, width - 17, 420);
            this.AddImage(5, 5, 10460);
            this.AddImage(width - 15, 5, 10460);
            this.AddImage(5, 424, 10460);
            this.AddImage(width - 15, 424, 10460);

            this.AddHtmlLocalized(375, 25, 200, 30, 1046026, LabelColor, false, false); // Quest Log

            this.AddHtmlLocalized(63 - xoffset, 45, 200, 32, 1072837, LabelColor, false, false); // Current Points: 
			
            this.AddHtml(243 - xoffset, 45, 200, 32, XmlSimpleGump.Color("Available Credits:", "FFFFFF"), false, false); // Your Reward Points: 

            this.AddHtml(453 - xoffset, 45, 200, 32, XmlSimpleGump.Color("Rank:", "FFFFFF"), false, false);  // Rank

            this.AddHtml(600 - xoffset, 45, 200, 32, XmlSimpleGump.Color("Quests Completed:", "FFFFFF"), false, false); // Quests completed

            if (p != null)
            {
                int pcolor = 53;
                this.AddLabel(170 - xoffset, 45, pcolor, p.Points.ToString());
                this.AddLabel(350 - xoffset, 45, pcolor, p.Credits.ToString());
                this.AddLabel(500 - xoffset, 45, pcolor, p.Rank.ToString());
                this.AddLabel(720 - xoffset, 45, pcolor, p.QuestsCompleted.ToString());
            }

            this.AddHtmlLocalized(63 - xoffset, 64, 200, 32, 3000362, LabelColor, false, false); // Open
            this.AddHtmlLocalized(147 - xoffset, 64, 200, 32, 3005104, LabelColor, false, false); // Name
            this.AddHtmlLocalized(270 - xoffset, 64, 200, 32, 1062213, LabelColor, false, false); // Type
            this.AddHtmlLocalized(326 - xoffset, 64, 200, 32, 3000132, LabelColor, false, false); // Status
            this.AddHtmlLocalized(429 - xoffset, 64, 200, 32, 1062465, LabelColor, false, false); // Expires

            this.AddHtml(514 - xoffset, 64, 200, 32, XmlSimpleGump.Color("Points", "FFFFFF"), false, false); // Points
            this.AddHtml(610 - xoffset, 64, 200, 32, XmlSimpleGump.Color("Next Available", "FFFFFF"), false, false); // Next Available
            //AddHtmlLocalized( 610 - xoffset, 64, 200, 32,  1046033, LabelColor, false, false ); // Completed
            this.AddHtmlLocalized(738 - xoffset, 64, 200, 32, 3005020, LabelColor, false, false); // Repeat

            this.AddButton(675 - xoffset, 416, 4017, 4018, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(710 - xoffset, 416, 120, 20, 1011441, LabelColor, false, false); // EXIT

            this.AddButton(113 - xoffset, 416, 0xFA8, 0xFAA, 10, GumpButtonType.Reply, 0);
            this.AddHtml(150 - xoffset, 416, 200, 32, XmlSimpleGump.Color("Top Players", "FFFFFF"), false, false); // Top players gump
			
            tableIndex = 0;

            if (page > 0)
            {
                this.AddButton(225, 416, 4014, 4016, 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(260, 416, 150, 20, 1011067, LabelColor, false, false); // Previous page
            }

            if (this.GetIndexForPage(page + 1) < list.Count)
            {
                this.AddButton(375, 416, 4005, 4007, 3, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(410, 416, 150, 20, 1011066, LabelColor, false, false); // Next page
            }

            for (int i = index; i < (index + count) && i >= 0 && i < list.Count; ++i)
            {
                object obj = list[i];

                if (obj is IXmlQuest)
                {
                    IXmlQuest e = (IXmlQuest)obj;

                    int y = 96 + (tableIndex++ * 32);

                    this.AddButton(60 - xoffset, y + 2, 0xFAB, 0xFAD, 2000 + i, GumpButtonType.Reply, 0); // open gump

                    int color;

                    if (!e.IsValid)
                    {
                        color = 33;
                    }
                    else if (e.IsCompleted)
                    {
                        color = 67;
                    }
                    else
                    {
                        color = 5;
                    }

                    this.AddLabel(100 - xoffset, y, color, (string)e.Name);

                    //AddHtmlLocalized( 315, y, 200, 32, e.IsCompleted ? 1049071 : 1049072, htmlcolor, false, false ); // Completed/Incomplete
                    this.AddLabel(315 - xoffset, y, color, e.IsCompleted ? "Completed" : "In Progress");

                    // indicate the expiration time
                    if (e.IsValid)
                    {
                        // do a little parsing of the expiration string to fit it in the space
                        string substring = e.ExpirationString;
                        if (e.ExpirationString.IndexOf("Expires in") >= 0)
                        {
                            substring = e.ExpirationString.Substring(11);
                        }
                        this.AddLabel(400 - xoffset, y, color, (string)substring);
                    }
                    else
                    {
                        this.AddLabel(400 - xoffset, y, color, "No longer valid");
                    }

                    if (e.PartyEnabled)
                    {
                        this.AddLabel(270 - xoffset, y, color, "Party");
                        //AddHtmlLocalized( 250, y, 200, 32, 3000332, htmlcolor, false, false ); // Party
                    }
                    else 
                    {
                        this.AddLabel(270 - xoffset, y, color, "Solo");
                    }

                    this.AddLabel(515 - xoffset, y, color, e.Difficulty.ToString());
                }
                else if (obj is XmlQuestPoints.QuestEntry)
                {
                    XmlQuestPoints.QuestEntry e = (XmlQuestPoints.QuestEntry)obj;

                    int y = 96 + (tableIndex++ * 32);
                    int color = 67;

                    this.AddLabel(100 - xoffset, y, color, (string)e.Name);

                    this.AddLabel(315 - xoffset, y, color, "Completed");

                    if (e.PartyEnabled)
                    {
                        this.AddLabel(270 - xoffset, y, color, "Party");
                        //AddHtmlLocalized( 250, y, 200, 32, 3000332, htmlcolor, false, false ); // Party
                    }
                    else 
                    {
                        this.AddLabel(270 - xoffset, y, color, "Solo");
                    }

                    this.AddLabel(515 - xoffset, y, color, e.Difficulty.ToString());

                    //AddLabel( 560 - xoffset, y, color, e.WhenCompleted.ToString() );
                    // determine when the quest can be done again by looking for an xmlquestattachment with the same name
                    XmlQuestAttachment qa = (XmlQuestAttachment)XmlAttach.FindAttachment(from, typeof(XmlQuestAttachment), e.Name);
                    if (qa != null)
                    {
                        if (qa.Expiration == TimeSpan.Zero)
                        {
                            this.AddLabel(560 - xoffset, y, color, "Not Repeatable");
                        }
                        else
                        {
                            DateTime nexttime = DateTime.UtcNow + qa.Expiration;
                            this.AddLabel(560 - xoffset, y, color, nexttime.ToString());
                        }
                    }
                    else
                    {
                        // didnt find one so it can be done again
                        this.AddLabel(560 - xoffset, y, color, "Available Now");
                    }

                    this.AddLabel(741 - xoffset, y, color, e.TimesCompleted.ToString());
                }
            }
        }

        public int GetIndexForPage(int page)
        {
            int index = 0;

            while (page-- > 0)
                index += this.GetCountForIndex(index);

            return index;
        }

        public int GetCountForIndex(int index)
        {
            int slots = 0;
            int count = 0;

            ArrayList list = this.m_List;

            for (int i = index; i >= 0 && i < list.Count; ++i)
            {
                object obj = list[i];

                int add;

                add = 1;

                if ((slots + add) > 10)
                    break;

                slots += add;

                ++count;
            }

            return count;
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (info == null || this.m_From == null)
                return;

            switch ( info.ButtonID )
            {
                case 0: // EXIT
                    {
                        break;
                    }

                case 2: // Previous page
                    {
                        if (this.m_Page > 0)
                            this.m_From.SendGump(new QuestLogGump(this.m_From, this.m_Page - 1, this.m_List));

                        return;
                    }
                case 3: // Next page
                    {
                        if (this.GetIndexForPage(this.m_Page + 1) < this.m_List.Count)
                            this.m_From.SendGump(new QuestLogGump(this.m_From, this.m_Page + 1, this.m_List));

                        break;
                    }
                case 10: // Top players
                    {
                        // if this player has an XmlQuestPoints attachment, find it
                        XmlQuestPoints p = (XmlQuestPoints)XmlAttach.FindAttachment(this.m_From, typeof(XmlQuestPoints));

                        this.m_From.CloseGump(typeof(XmlQuestLeaders.TopQuestPlayersGump));
                        this.m_From.SendGump(new XmlQuestLeaders.TopQuestPlayersGump(p));

                        break;
                    }

                default:
                    {
                        if (info.ButtonID >= 2000)
                        {
                            int index = info.ButtonID - 2000;

                            if (index < 0 || index >= this.m_List.Count)
                                break;

                            if (this.m_List[index] is IXmlQuest)
                            {
                                IXmlQuest o = this.m_List[index] as IXmlQuest;
    
                                if (o != null && !o.Deleted)
                                {
                                    this.m_From.SendGump(new QuestLogGump(this.m_From, this.m_Page, null));
                                    this.m_From.CloseGump(typeof(XmlQuestStatusGump));
                                    this.m_From.SendGump(new XmlQuestStatusGump(o, o.TitleString, 320, 0, true));
                                }
                            }
                        }

                        break;
                    }
            }
        }
    }
}