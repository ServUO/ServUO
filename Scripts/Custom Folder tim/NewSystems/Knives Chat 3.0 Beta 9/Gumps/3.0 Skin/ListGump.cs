using System;
using System.Collections;
using Server;
using Server.Gumps;

namespace Knives.Chat3
{
    public class ListGump : GumpPlus
    {
        public enum ListPage { All, Channel, Mail, Friends, Ignores, GIgnores, GListens, Bans, Notifications }

        #region Class Definitions

        private ListPage c_ListPage;
        private Mobile c_Target;
        private int c_Page;
        private bool c_Menu, c_Search;
        private string c_TxtSearch = "";
        private string c_CharSearch = "";

        public Mobile Current { get { return (c_Target == null ? Owner : c_Target); } }

        #endregion

        #region Constructors

        public ListGump(Mobile m, int page)
            : base(m, 100, 100)
        {
            c_ListPage = (ListPage)page;

            m.CloseGump(typeof(ListGump));

            Override = true;
        }

        public ListGump(Mobile m, Mobile targ)
            : base(m, 100, 100)
        {
            c_Target = targ;

            m.CloseGump(typeof(ListGump));

            Override = true;
        }

        public ListGump(Mobile m)
            : this(m, 0)
        {
        }

        #endregion

        #region Methods

        protected override void BuildGump()
        {
            if (c_ListPage == ListPage.Channel && Data.GetData(Current).CurrentChannel == null)
                c_ListPage = ListPage.All;

            int width = Data.GetData(Current).QuickBar ? 250 : 200;
            int y = 10;
            int perpage = Data.GetData(Current).PerPage;
            int bar = width - 18;

            if (c_ListPage == ListPage.Mail)
                perpage /= 2;

            if (c_Menu)
                ShowMenu(width);
            if (c_Search)
                ShowSearch(width);

            AddButton(5, y-5, 0x5689, "Help", new GumpCallback(Help));
            AddButton(30, y - 1, 0x983, "PerPage Down", new GumpCallback(PerPageDown));
            AddButton(40, y-1, 0x985, "PerPage Up", new GumpCallback(PerPageUp));
            AddButton(width - 80, y - 5, 0x768, "Profile", new GumpStateCallback(Profile), Current);
            AddLabel(width - 75, y - 5, 0x47E, "P");
            AddButton(width - 65, y - 5, 0x768, "QuickBar", new GumpCallback(QuickBar));
            AddLabel(width - 60, y - 5, Data.GetData(Current).QuickBar ? 0x34 : 0x47E, "Q");
            AddButton(width - 50, y - 5, 0x768, "Menu", new GumpCallback(Menu));
            AddLabel(width - 46, y-5, c_Menu ? 0x34 : 0x47E, "M");
            AddButton(width - 35, y - 5, 0x768, "Search", new GumpCallback(Search));
            AddLabel(width - 30, y - 5, c_Search ? 0x34 : 0x47E, "S");

            ArrayList list = GetList();
            SearchFilter(list);
            list.Sort(new InternalSort(this));

            if (list.Count < c_Page * perpage)
                c_Page = 0;

            if (c_Page != 0)
                AddButton(width / 2 - 20, y-3, 0x25E4, 0x25E5, "Page Down", new GumpCallback(PageDown));
            if (perpage * (c_Page + 1) < list.Count)
                AddButton(width / 2, y-3, 0x25E8, 0x25E9, "Page Up", new GumpCallback(PageUp));

            if (c_Target != null)
            {
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(224) + " " + c_Target.RawName);
                AddButton(width / 2 - 80, y, 0x5686, "Clear Viewing", new GumpCallback(ClearViewing));
                AddButton(width / 2 + 65, y, 0x5686, "Clear Viewing", new GumpCallback(ClearViewing));
            }

            AddImage(width / 2 - 72, (y += 25) - 1, 0x9C5);
            AddHtml(0, y, width, "<CENTER>" + GetTitle());

            if (c_ListPage == ListPage.Mail)
                AddHtml(0, y += 20, width, "<CENTER>" + Data.GetData(Owner).Messages.Count + " / " + Data.MaxMsgs);
            else if(c_ListPage != ListPage.Notifications)
            {
                ArrayList states = new ArrayList(Server.Network.NetState.Instances);
                foreach (Server.Network.NetState state in Server.Network.NetState.Instances)
                    if (state.Mobile != null && state.Mobile.AccessLevel != AccessLevel.Player)
                        states.Remove(state);

                AddHtml(0, y += 20, width, "<CENTER>" + states.Count + " " + General.Local(19));
            }

            y += 5;

            for (int i = c_Page * perpage; i < (c_Page + 1) * perpage && i < list.Count; ++i)
            {
                if (list[i] is string)
                {
                    if (list[i].ToString().IndexOf("@") == 0)
                        list[i] = list[i].ToString().Substring(1, list[i].ToString().Length - 1);

                    AddHtml(35, y += 20, width - 35, list[i].ToString());
                    AddButton(width - 40, y, Data.GetData(Current).IrcIgnores.Contains(list[i].ToString()) ? 0x5687 : 0x5686, "Ignore IRC", new GumpStateCallback(IgnoreIrc), list[i]);
                }
                else if(list[i] is Mobile)
                {
                    AddHtml(35, y += 20, width - 35, ColorFor((Mobile)list[i]) + ((Mobile)list[i]).RawName + StatusFor((Mobile)list[i]));
                    if(Current == Owner && Data.GetData(Owner).NewMsgFrom((Mobile)list[i]))
                        AddButton(20, y, 0x1523, "Check Msg", new GumpStateCallback(CheckMsg), (Mobile)list[i]);
                    else
                        AddButton(20, y + 3, 0x2716, "Profile", new GumpStateCallback(Profile), (Mobile)list[i]);

                    if (list[i] == Current)
                        continue;

                    if (Data.GetData(Current).QuickBar)
                    {
                        bar = width - 18;

                        if (Current.AccessLevel > ((Mobile)list[i]).AccessLevel)
                        {
                            if (Data.GetData(Current).GlobalAccess)
                            {
                                if (Data.GetData(Current).Global)
                                {
                                    AddButton(bar -= 12, y + 3, 0x13A8, "Mini Ignore", new GumpStateCallback(GIgnore), list[i]);
                                    AddLabel(bar + 4, y, Data.GetData(Current).GIgnores.Contains(list[i]) ? 0x44 : 0x26, "I");
                                }
                                else
                                {
                                    AddButton(bar -= 12, y + 3, 0x13A8, "Mini Listen", new GumpStateCallback(GListen), list[i]);
                                    AddLabel(bar + 4, y, Data.GetData(Current).GListens.Contains(list[i]) ? 0x44 : 0x26, "L");
                                }
                            }

                            AddButton(bar -= 12, y + 3, 0x13A8, "Mini Ban", new GumpStateCallback(Ban), list[i]);
                            AddLabel(bar + 4, y, Data.GetData((Mobile)list[i]).Banned ? 0x44 : 0x26, "b");
                        }

                        if (Current.AccessLevel >= AccessLevel.GameMaster && ((Mobile)list[i]).NetState != null)
                        {
                            AddButton(bar -= 12, y + 3, 0x13A8, "Mini Goto", new GumpStateCallback(Goto), list[i]);
                            AddLabel(bar + 3, y - 2, 0x47E, "g");

                            AddButton(bar -= 12, y + 3, 0x13A8, "Mini Client", new GumpStateCallback(Client), list[i]);
                            AddLabel(bar + 3, y - 2, 0x47E, "c");
                        }

                        if (Chat3.Message.CanMessage(Current, (Mobile)list[i]))
                        {
                            AddButton(bar -= 12, y + 3, 0x13A8, "Mini Message", new GumpStateCallback(Message), list[i]);
                            AddLabel(bar + 3, y - 2, 0x47E, "m");
                        }

                        AddButton(bar -= 12, y + 3, 0x13A8, "Mini Ignore", new GumpStateCallback(Ignore), list[i]);
                        AddLabel(bar + 5, y - 1, Data.GetData(Current).Ignores.Contains(list[i]) ? 0x44 : 0x26, "i");

                        AddButton(bar -= 12, y + 3, 0x13A8, "Mini Friend", new GumpStateCallback(Friend), list[i]);
                        AddLabel(bar + 3, y, Data.GetData(Current).Friends.Contains(list[i]) ? 0x44 : 0x26, "f");
                    }
                }
                else if (list[i] is Message)
                {
                    Message msg = (Message)list[i];

                    AddHtml(45, y += 20, width-85, ColorFor(msg) + (msg.Read ? "" : "<B>") + msg.Subject, false);
                    AddHtml(45, y += 16, width-85, General.Local(60) + " " + msg.From.RawName);

                    AddButton(20, y - 10, 0x2716, "Open", new GumpStateCallback(Open), (Message)list[i]);
                    AddButton(width - 40, y - 10, 0x5686, 0x5687, "Delete", new GumpStateCallback(Delete), (Message)list[i]);
                }
                else if (list[i] is Notification)
                {
                    Notification not = (Notification)list[i];

                    AddHtml(45, y += 20, width-85, ColorFor(not) + not.Name);

                    AddButton(20, y + 3, 0x2716, "Edit Notif", new GumpStateCallback(EditNotif), (Notification)list[i]);
                    AddButton(width - 40, y + 3, 0x5686, 0x5687, "Delete", new GumpStateCallback(Delete), (Notification)list[i]);
                }
            }

            if (c_ListPage == ListPage.Mail && Current.AccessLevel >= AccessLevel.GameMaster)
            {
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(95), false);
                AddButton(width / 2 - 50, y + 3, 0x2716, "Broadcast", new GumpCallback(Broadcast));
                AddButton(width / 2 + 40, y + 3, 0x2716, "Broadcast", new GumpCallback(Broadcast));

                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(257), false);
                AddButton(width / 2 - 50, y + 3, 0x2716, "Staff", new GumpCallback(BroadcastStaff));
                AddButton(width / 2 + 40, y + 3, 0x2716, "Staff", new GumpCallback(BroadcastStaff));
            }
            else if (c_ListPage == ListPage.Notifications)
            {
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(270), false);
                AddButton(width / 2 - 70, y + 3, 0x2716, "New Notif", new GumpCallback(NewNotif));
                AddButton(width / 2 + 60, y + 3, 0x2716, "New Notif", new GumpCallback(NewNotif));
            }

            AddImage(width / 2 - 32, (y+=30) - 2, 0x98C);
            AddHtml(0, y, width, "<CENTER>" + General.Local(19 + (int)Data.GetData(Current).Status));
            AddButton(width / 2 - 50, y + 3, 0x2716, "Status", new GumpCallback(Status));
            AddButton(width / 2 + 40, y + 3, 0x2716, "Status", new GumpCallback(Status));

            AddBackgroundZero(0, 0, width, y+50, Data.GetData(Current).DefaultBack);
        }

        private void ShowMenu(int x)
        {
            int width = 150;
            int y = -10;

            AddImage(x-8, 10, 0x100);
            AddHtml(x, y += 25, width, "<CENTER>" + General.Local(38));
            AddImage(x + 10, y + 2, 0x39);
            AddImage(x + width - 40, y + 2, 0x3B);

            y += 5;

            foreach (Chat3.Channel c in Chat3.Channel.Channels)
            {
                if (!c.CanChat(Current, false))
                    continue;

                AddHtml(x+40, y+=20, width-40, (Data.GetData(Current).CurrentChannel == c ? HTML.Yellow : HTML.White) + "<CENTER>" + (c.Style == ChatStyle.Regional ? c.Name : c.NameFor(Current)));
                AddButton(x+20, y, c.IsIn(Current) ? 0x2343 : 0x2342, "Join Channel", new GumpStateCallback(JoinChannel), c);

                if (c.IsIn(Current))
                {
                    AddButton(x + 40 + (width-40)/2-40, y + 3, 0x2716, "View Channel", new GumpStateCallback(ViewChannel), c);
                    AddButton(x + 40 + (width-40)/2+30, y + 3, 0x2716, "View Channel", new GumpStateCallback(ViewChannel), c);
                }
            }

            AddHtml(x, y += 25, width, "<CENTER>" + General.Local(1));
            AddImage(x + 10, y + 2, 0x39);
            AddImage(x + width - 40, y + 2, 0x3B);

            AddHtml(x, y += 25, width, "<CENTER>" + General.Local(46));
            AddButton(x + 20, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.All);
            AddButton(x + width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.All);
            AddHtml(x, y += 20, width, "<CENTER>" + General.Local(56));
            AddButton(x + 20, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.Mail);
            AddButton(x + width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.Mail);
            AddHtml(x, y += 20, width, "<CENTER>" + General.Local(203));
            AddButton(x + 20, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.Friends);
            AddButton(x + width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.Friends);
            AddHtml(x, y += 20, width, "<CENTER>" + General.Local(51));
            AddButton(x + 20, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.Ignores);
            AddButton(x + width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.Ignores);

            AddHtml(x, y += 20, width, (c_ListPage == ListPage.Channel ? "" : HTML.Gray) + "<CENTER>" + General.Local(206));
            if (c_ListPage == ListPage.Channel)
            {
                AddButton(x + 20, y + 3, 0x2716, "History", new GumpCallback(History));
                AddButton(x + width - 30, y + 3, 0x2716, "History", new GumpCallback(History));
            }

            if (Data.GetData(Current).GlobalAccess)
            {
                AddHtml(x, y += 20, width, HTML.Red + "<CENTER>" + General.Local(204));
                AddButton(x + 20, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.GIgnores);
                AddButton(x + width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.GIgnores);
                AddHtml(x, y += 20, width, HTML.Red + "<CENTER>" + General.Local(205));
                AddButton(x + 20, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.GListens);
                AddButton(x + width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.GListens);
            }

            if (Current.AccessLevel > AccessLevel.GameMaster)
            {
                AddHtml(x, y += 20, width, HTML.Red + "<CENTER>" + General.Local(54));
                AddButton(x + 20, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.Bans);
                AddButton(x + width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.Bans);
                AddHtml(x, y += 20, width, HTML.Red + "<CENTER>" + General.Local(269));
                AddButton(x + 20, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.Notifications);
                AddButton(x + width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(Page), ListPage.Notifications);
            }

            AddHtml(x, y+=25, width, "<CENTER>" + General.Local(40));
            AddImage(x + 10, y + 2, 0x39);
            AddImage(x + width - 40, y + 2, 0x3B);

            AddHtml(x, y += 25, width, "<CENTER>" + General.Local(49));
            AddButton(x + 20, y + 3, 0x2716, "Colors", new GumpCallback(Colors));
            AddButton(x + width - 30, y + 3, 0x2716, "Colors", new GumpCallback(Colors));
            AddHtml(x, y += 20, width, "<CENTER>" + General.Local(56));
            AddButton(x + 20, y + 3, 0x2716, "Mail", new GumpCallback(Mail));
            AddButton(x + width - 30, y + 3, 0x2716, "Mail", new GumpCallback(Mail));

            if (Data.GetData(Current).GlobalAccess)
            {
                AddHtml(x, y += 20, width, HTML.Red + "<CENTER>" + General.Local(43));
                AddButton(x + 20, y + 3, 0x2716, "Global Menu", new GumpCallback(GlobalMenu));
                AddButton(x + width - 30, y + 3, 0x2716, "Global Menu", new GumpCallback(GlobalMenu));
            }

            if (Current.AccessLevel >= AccessLevel.Administrator)
            {
                AddHtml(x, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(207));
                AddButton(x + 20, y + 3, 0x2716, "General", new GumpCallback(GenOpt));
                AddButton(x + width - 30, y + 3, 0x2716, "General", new GumpCallback(GenOpt));
                AddHtml(x, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(237));
                AddButton(x + 20, y + 3, 0x2716, "Logging", new GumpCallback(Logging));
                AddButton(x + width - 30, y + 3, 0x2716, "Logging", new GumpCallback(Logging));
                AddHtml(x, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(208));
                AddButton(x + 20, y + 3, 0x2716, "Filter", new GumpCallback(Filter));
                AddButton(x + width - 30, y + 3, 0x2716, "Filter", new GumpCallback(Filter));
                AddHtml(x, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(209));
                AddButton(x + 20, y + 3, 0x2716, "Spam", new GumpCallback(Spam));
                AddButton(x + width - 30, y + 3, 0x2716, "Spam", new GumpCallback(Spam));
                AddHtml(x, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(210));
                AddButton(x + 20, y + 3, 0x2716, "Irc", new GumpCallback(Irc));
                AddButton(x + width - 30, y + 3, 0x2716, "Irc", new GumpCallback(Irc));
                AddHtml(x, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(287));
                AddButton(x + 20, y + 3, 0x2716, "Multi", new GumpCallback(Multi));
                AddButton(x + width - 30, y + 3, 0x2716, "Multi", new GumpCallback(Multi));
                AddHtml(x, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(50));
                AddButton(x + 20, y + 3, 0x2716, "Channel", new GumpCallback(Channel));
                AddButton(x + width - 30, y + 3, 0x2716, "Channel", new GumpCallback(Channel));
            }

            AddButton(x + width / 2 - 30, y += 25, 0x8B1, "1.0", new GumpStateCallback(SkinChange), Skin.One);
            AddButton(x + width / 2 - 10, y, 0x8B2, "2.0", new GumpStateCallback(SkinChange), Skin.Two);
            AddButton(x + width / 2 + 10, y, 0x8B3, "3.0", new GumpStateCallback(SkinChange), Skin.Three);

            AddBackgroundZero(x, 0, width, y + 40, Data.GetData(Current).DefaultBack, false);
        }

        private void ShowSearch(int x)
        {
            int width = 130;

            AddBackground(x, 0, width, 50, Data.GetData(Current).DefaultBack, false);

            AddImage(x - 8, 10, 0x100);
            AddTextField(x+15, 15, 90, 21, 0x480, 0xBBC, "Search", c_TxtSearch);
            AddButton(x + width-17, 19, 0x2716, "Text Search", new GumpCallback(TxtSearch));

            char[] chars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            int difx = x-13;
            int y = 61;

            foreach (char c in chars)
            {
                AddButton(difx += 20, y, 0x2344, "Char Search", new GumpStateCallback(CharSearch), c.ToString());
                AddHtml(difx + 6, y, 20, (c_CharSearch == c.ToString() ? HTML.Green : "") + c, false);

                if (difx >= x + 102)
                {
                    difx = x-13;
                    y += 20;
                }
            }
        }

        private string GetTitle()
        {
            switch (c_ListPage)
            {
                case ListPage.All: return General.Local(46);
                case ListPage.Channel: return Data.GetData(Current).CurrentChannel.NameFor(Current);
                case ListPage.Mail: return General.Local(56);
                case ListPage.Friends: return General.Local(203);
                case ListPage.Ignores: return General.Local(51);
                case ListPage.GIgnores: return General.Local(204);
                case ListPage.GListens: return General.Local(205);
                case ListPage.Bans: return General.Local(54);
                case ListPage.Notifications: return General.Local(269);
            }

            return "";
        }

        private ArrayList GetList()
        {
            ArrayList list;

            switch (c_ListPage)
            {
                case ListPage.All:
                    list = new ArrayList();
                    foreach (Data data in Data.Datas.Values)
                        if (Current.AccessLevel >= data.Mobile.AccessLevel)
                            list.Add(data.Mobile);
                    return list;
                case ListPage.Channel: return new ArrayList(Data.GetData(Current).CurrentChannel.BuildList(Current));
                case ListPage.Mail: return new ArrayList(Data.GetData(Current).Messages);
                case ListPage.Friends: return new ArrayList(Data.GetData(Current).Friends);
                case ListPage.Ignores: return new ArrayList(Data.GetData(Current).Ignores);
                case ListPage.GIgnores: return new ArrayList(Data.GetData(Current).GIgnores);
                case ListPage.GListens: return new ArrayList(Data.GetData(Current).GListens);
                case ListPage.Bans:
                    list = new ArrayList();
                    foreach (Data data in Data.Datas.Values)
                        if (data.Banned)
                            list.Add(data.Mobile);
                    return list;
                case ListPage.Notifications: return new ArrayList(Data.Notifications);
            }

            return new ArrayList();
        }

        private void SearchFilter(ArrayList list)
        {
            string txt = "";
            foreach (object obj in new ArrayList(list))
            {
                if (obj is Mobile)
                    txt = ((Mobile)obj).RawName;
                else if (obj is Message)
                    txt = ((Message)obj).From.RawName;
                else
                    txt = obj.ToString();

                if (c_CharSearch.ToLower() != "" && txt.ToLower().IndexOf(c_CharSearch) != 0)
                    list.Remove(obj);
                else if (c_TxtSearch.ToLower() != "" && txt.ToLower().IndexOf(c_TxtSearch) == -1)
                    list.Remove(obj);
            }
        }

        private string ColorFor(Message msg)
        {
            switch (msg.Type)
            {
                case MsgType.Normal: return HTML.White;
                case MsgType.Invite: return HTML.Yellow;
                case MsgType.System: return HTML.Red;
                case MsgType.Staff: return HTML.Purple;
                default: return HTML.White;
            }
        }

        private string ColorFor(Notification not)
        {
            return HTML.White;
        }

        private string ColorFor(Mobile m)
        {
            if (Current == m)
                return HTML.Yellow;
            if (Data.GetData(m).Banned)
                return HTML.Red;
            if (Data.GetData(Current).Ignores.Contains(m))
                return HTML.AshRed;
            if (Data.GetData(Current).Global && Data.GetData(Current).GIgnores.Contains(m))
                return HTML.AshRed;
            if (!Data.GetData(Current).Global && Data.GetData(Current).GListens.Contains(m))
                return HTML.Blue;
            if (m.NetState == null || Data.GetData(m).Status == OnlineStatus.Hidden)
                return HTML.DarkGray;
            if (Data.GetData(m).Status == OnlineStatus.Away || Data.GetData(m).Status == OnlineStatus.Busy)
                return HTML.Gray;
            if (m.AccessLevel > AccessLevel.Player)
                return HTML.LightPurple;
            if (m.Guild != null && (m.Guild == Current.Guild))
                return HTML.Green;

            return HTML.White;
        }

        private string StatusFor(Mobile m)
        {
            if (Data.GetData(m).Status == OnlineStatus.Away)
                return " (Away)";
            else if (Data.GetData(m).Status == OnlineStatus.Busy)
                return " (Busy)";
            else if (Data.GetData(m).Status == OnlineStatus.Hidden)
                return " (Hidden)";

            return "";
        }

        #endregion

        #region Responses

        private void PageUp()
        {
            c_Page++;
            NewGump();
        }

        private void PageDown()
        {
            c_Page--;
            NewGump();
        }

        private void Help()
        {
            NewGump();
            new HelpContentsGump(Owner);
        }

        private void PerPageUp()
        {
            Data.GetData(Current).PerPage++;
            NewGump();
        }

        private void PerPageDown()
        {
            Data.GetData(Current).PerPage--;
            NewGump();
        }

        private void IgnoreIrc(object o)
        {
            if (!(o is string))
                return;

            if (Data.GetData(Current).IrcIgnores.Contains(o.ToString()))
                Data.GetData(Current).RemoveIrcIgnore(o.ToString());
            else
                Data.GetData(Current).AddIrcIgnore(o.ToString());

            NewGump();
        }

        private void Profile(object o)
        {
            if (!(o is Mobile))
                return;

            NewGump();
            new ProfileGump(Owner, (Mobile)o);
        }

        private void Open(object o)
        {
            Message m = o as Message;

            if (m == null)
                return;

            if (Data.GetData(Owner).Messages.Contains(m))
                m.Read = true;

            NewGump();

            if (m.Read && Data.GetData(m.From).ReadReceipt && m.From.AccessLevel >= Owner.AccessLevel)
                m.From.SendMessage(Data.GetData(m.From).SystemC, Owner.RawName + " " + General.Local(197) + " " + m.Subject);

            new MessageGump(Owner, m);
        }

        private void EditNotif(object o)
        {
            if (!(o is Notification))
                return;

            NewGump();
            new EditNotGump(Owner, (Notification)o);
        }

        private void Delete(object o)
        {
            if (o is Message)
                Data.GetData(Current).DeleteMessage((Message)o);
            else if (o is Notification)
                Data.Notifications.Remove(o);

            NewGump();
        }

        private void Friend(object o)
        {
            Mobile m = o as Mobile;

            if (m == null)
                return;

            if (Data.GetData(m).ByRequest && !Data.GetData(Current).Friends.Contains(m))
            {
                if (!TrackSpam.LogSpam(Current, "Request " + m.RawName, TimeSpan.FromHours(Data.RequestSpam)))
                {
                    TimeSpan ts = TrackSpam.NextAllowedIn(Current, "Request " + m.RawName, TimeSpan.FromHours(Data.RequestSpam));
                    string txt = (ts.Days != 0 ? ts.Days + " " + General.Local(170) + " " : "") + (ts.Hours != 0 ? ts.Hours + " " + General.Local(171) + " " : "") + (ts.Minutes != 0 ? ts.Minutes + " " + General.Local(172) + " " : "");

                    Owner.SendMessage(Data.GetData(Current).SystemC, General.Local(96) + " " + txt);
                    NewGump();
                    return;
                }

                Data.GetData(m).AddMessage(new Message(Current, General.Local(84), General.Local(85), MsgType.Invite));

                Owner.SendMessage(Data.GetData(Current).SystemC, General.Local(86) + " " + m.RawName);

                NewGump();
                return;
            }

            if (Data.GetData(Current).Friends.Contains(m))
                Data.GetData(Current).RemoveFriend(m);
            else
                Data.GetData(Current).AddFriend(m);

            NewGump();
        }

        private void Ignore(object o)
        {
            if (!(o is Mobile))
                return;

            if (Data.GetData(Current).Ignores.Contains(o))
                Data.GetData(Current).RemoveIgnore((Mobile)o);
            else
                Data.GetData(Current).AddIgnore((Mobile)o);

            NewGump();
        }

        private void Message(object o)
        {
            if (!(o is Mobile))
                return;

            NewGump();

            if (Current != Owner)
                Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(225));
            else if (Chat3.Message.CanMessage(Owner, (Mobile)o))
                new SendMessageGump(Owner, (Mobile)o, "", null, MsgType.Normal);
        }

        private void Ban(object o)
        {
            Mobile m = o as Mobile;

            if (m == null)
                return;

            if (Data.GetData(m).Banned)
            {
                Data.GetData(m).RemoveBan();
                Owner.SendMessage(Data.GetData(Current).SystemC, General.Local(78) + " " + m.RawName);
                NewGump();
            }
            else
                new BanGump(m, this);
        }

        private void GIgnore(object o)
        {
            if (!(o is Mobile))
                return;

            if (Data.GetData(Current).GIgnores.Contains(o))
                Data.GetData(Current).RemoveGIgnore((Mobile)o);
            else
                Data.GetData(Current).AddGIgnore((Mobile)o);

            NewGump();
        }

        private void GListen(object o)
        {
            if (!(o is Mobile))
                return;

            if (Data.GetData(Current).GListens.Contains(o))
                Data.GetData(Current).RemoveGListen((Mobile)o);
            else
                Data.GetData(Current).AddGListen((Mobile)o);

            NewGump();
        }

        private void Client(object o)
        {
            Mobile m = o as Mobile;

            if (m == null)
                return;

            NewGump();

            if (m.NetState == null)
                Owner.SendMessage(Data.GetData(Current).SystemC, m.RawName + " " + General.Local(83));
            else
                Owner.SendGump(new ClientGump(Owner, m.NetState));
        }

        private void Goto(object o)
        {
            Mobile m = o as Mobile;

            if (m == null)
                return;

            if (m.NetState == null)
                Owner.SendMessage(Data.GetData(Current).SystemC, m.RawName + " " + General.Local(83));
            else
            {
                Owner.Location = m.Location;
                Owner.Map = m.Map;
            }

            NewGump();
        }

        private void QuickBar()
        {
            Data.GetData(Current).QuickBar = !Data.GetData(Current).QuickBar;
            NewGump();
        }

        private void Menu()
        {
            c_Menu = !c_Menu;

            if (c_Menu)
                c_Search = false;

            NewGump();
        }

        private void Search()
        {
            c_Search = !c_Search;

            if (c_Search)
                c_Menu = false;

            NewGump();
        }

        private void JoinChannel(object o)
        {
            Channel c = o as Channel;

            if (c == null)
                return;

            if (c.IsIn(Current))
                c.Leave(Current);
            else
                c.Join(Current);

            NewGump();
        }

        private void ViewChannel(object o)
        {
            if (!(o is Channel))
                return;

            Data.GetData(Current).CurrentChannel = (Channel)o;
            c_ListPage = ListPage.Channel;
            NewGump();
        }

        private void Page(object o)
        {
            if (!(o is ListPage))
                return;

            c_ListPage = (ListPage)o;
            NewGump();
        }

        private void History()
        {
            NewGump();
            new HistoryGump(Owner, Data.GetData(Current).CurrentChannel);
        }

        private void GenOpt()
        {
            NewGump();
            new GeneralGump(Owner);
        }

        private void Logging()
        {
            NewGump();
            new LoggingGump(Owner);
        }

        private void Filter()
        {
            NewGump();
            new FilterGump(Owner);
        }

        private void Spam()
        {
            NewGump();
            new SpamGump(Owner);
        }

        private void Colors()
        {
            NewGump();

            if(c_Target == null )
                new ColorsGump(Owner);
            else
                new ColorsGump(Owner, c_Target);
        }

        private void Irc()
        {
            NewGump();
            new IrcGump(Owner);
        }

        private void Multi()
        {
            NewGump();
            new MultiGump(Owner);
        }

        private void Mail()
        {
            NewGump();

            if (c_Target == null)
                new MailGump(Owner);
            else
                new MailGump(Owner, c_Target);
        }

        private void GlobalMenu()
        {
            NewGump();
            new GlobalGump(Current);
        }

        private void Channel()
        {
            NewGump();
            new ChannelGump(Owner);
        }

        private void TxtSearch()
        {
            c_TxtSearch = GetTextField("Search");
            c_CharSearch = "";
            NewGump();
        }

        private void CharSearch(object o)
        {
            if (!(o is string))
                return;

            if (c_CharSearch == o.ToString())
                c_CharSearch = "";
            else
                c_CharSearch = o.ToString();

            c_TxtSearch = "";

            NewGump();
        }

        private void Status()
        {
            new StatusGump(Owner, this);
        }

        private void Broadcast()
        {
            NewGump();
            new SendMessageGump(Owner, null, "", null, MsgType.System);
        }

        private void BroadcastStaff()
        {
            NewGump();
            new SendMessageGump(Owner, null, "", null, MsgType.Staff);
        }

        private void NewNotif()
        {
            new Notification();
            NewGump();
        }

        private void ClearViewing()
        {
            c_Target = null;
            NewGump();
        }

        private void CheckMsg(object obj)
        {
            Mobile m = obj as Mobile;

            if (m == null)
                return;

            Message msg = Data.GetData(Owner).GetNewMsgFrom(m);
            if (msg == null)
                NewGump();
            else
                Open(msg);
        }

        private void SkinChange(object obj)
        {
            if (!(obj is Skin))
                return;

            Data.GetData(Owner).MenuSkin = (Skin)obj;
            General.List(Owner, (int)c_ListPage);
        }

        #endregion

        #region Internal Classes

        private class InternalSort : IComparer
        {
            private GumpPlus c_Gump;

            public InternalSort(GumpPlus gump)
            {
                c_Gump = gump;
            }

            public int Compare(object x, object y)
            {
                if (x is Mobile && y is Mobile)
                {
                    if (((Mobile)x).NetState == null && ((Mobile)y).NetState != null)
                        return 1;
                    if (((Mobile)x).NetState != null && ((Mobile)y).NetState == null)
                        return -1;

                    if (!Data.GetData(c_Gump.Owner).NewMsgFrom(((Mobile)x)) && Data.GetData(c_Gump.Owner).NewMsgFrom(((Mobile)y)))
                        return 1;
                    if (Data.GetData(c_Gump.Owner).NewMsgFrom(((Mobile)x)) && !Data.GetData(c_Gump.Owner).NewMsgFrom(((Mobile)y)))
                        return -1;

                    if (((Mobile)x).AccessLevel < ((Mobile)y).AccessLevel)
                        return 1;
                    if (((Mobile)x).AccessLevel > ((Mobile)y).AccessLevel)
                        return -1;

                    return Insensitive.Compare(((Mobile)x).RawName, ((Mobile)y).RawName);
                }
                else if (x is string && y is string)
                    return Insensitive.Compare(x.ToString(), y.ToString());
                else if (x is string)
                    return 1;
                else if (y is string)
                    return -1;
                else if (x is Message && y is Message)
                {
                    if (((Message)x).Received > ((Message)y).Received)
                        return -1;
                    if (((Message)x).Received < ((Message)y).Received)
                        return 1;
                }

                return Insensitive.Compare(x.ToString(), y.ToString());
            }
        }

        private class BanGump : GumpPlus
        {
            private GumpPlus c_Gump;
            private Mobile c_Target;

            public BanGump(Mobile m, GumpPlus g)
                : base(g.Owner, 100, 100)
            {
                c_Gump = g;
                c_Target = m;
            }

            protected override void BuildGump()
            {
                int width = 200;
                int y = 10;

                AddHtml(0, y, width, "<CENTER>" + General.Local(160));
                AddImage(width / 2 - 70, y + 2, 0x39);
                AddImage(width / 2 + 40, y + 2, 0x3B);

                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(161));
                AddButton(width / 2 - 60, y + 3, 0x2716, "30 minutes", new GumpStateCallback(BanTime), TimeSpan.FromMinutes(30));
                AddButton(width / 2 + 50, y + 3, 0x2716, "30 minutes", new GumpStateCallback(BanTime), TimeSpan.FromMinutes(30));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(162));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 hour", new GumpStateCallback(BanTime), TimeSpan.FromHours(1));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 hour", new GumpStateCallback(BanTime), TimeSpan.FromHours(1));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(163));
                AddButton(width / 2 - 60, y + 3, 0x2716, "12 hours", new GumpStateCallback(BanTime), TimeSpan.FromHours(12));
                AddButton(width / 2 + 50, y + 3, 0x2716, "12 hours", new GumpStateCallback(BanTime), TimeSpan.FromHours(12));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(164));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 day", new GumpStateCallback(BanTime), TimeSpan.FromDays(1));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 day", new GumpStateCallback(BanTime), TimeSpan.FromDays(1));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(165));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 week", new GumpStateCallback(BanTime), TimeSpan.FromDays(7));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 week", new GumpStateCallback(BanTime), TimeSpan.FromDays(7));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(166));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 month", new GumpStateCallback(BanTime), TimeSpan.FromDays(30));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 month", new GumpStateCallback(BanTime), TimeSpan.FromDays(30));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(167));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 year", new GumpStateCallback(BanTime), TimeSpan.FromDays(365));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 year", new GumpStateCallback(BanTime), TimeSpan.FromDays(365));

                AddBackgroundZero(0, 0, width, y + 40, Data.GetData(c_Target).DefaultBack);
            }

            private void BanTime(object o)
            {
                if (!(o is TimeSpan))
                    return;

                Data.GetData(c_Target).Ban((TimeSpan)o);
                Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(77) + " " + c_Target.RawName);

                c_Gump.NewGump();
            }

            protected override void OnClose()
            {
                c_Gump.NewGump();
            }
        }

        private class StatusGump : GumpPlus
        {
            private GumpPlus c_Gump;

            public StatusGump(Mobile m, GumpPlus g)
                : base(m, 100, 100)
            {
                m.CloseGump(typeof(StatusGump));

                c_Gump = g;
            }

            protected override void BuildGump()
            {
                int width = 100;
                int y = 20;

                AddHtml(0, y, width, "<CENTER>" + General.Local(19));
                AddButton(width / 2 - 50, y + 3, 0x2716, "Online", new GumpStateCallback(Status), OnlineStatus.Online);
                AddButton(width / 2 + 40, y + 3, 0x2716, "Online", new GumpStateCallback(Status), OnlineStatus.Online);
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(20));
                AddButton(width / 2 - 50, y + 3, 0x2716, "Away", new GumpStateCallback(Status), OnlineStatus.Away);
                AddButton(width / 2 + 40, y + 3, 0x2716, "Away", new GumpStateCallback(Status), OnlineStatus.Away);
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(21));
                AddButton(width / 2 - 50, y + 3, 0x2716, "Busy", new GumpStateCallback(Status), OnlineStatus.Busy);
                AddButton(width / 2 + 40, y + 3, 0x2716, "Busy", new GumpStateCallback(Status), OnlineStatus.Busy);
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(22));
                AddButton(width / 2 - 50, y + 3, 0x2716, "Hidden", new GumpStateCallback(Status), OnlineStatus.Hidden);
                AddButton(width / 2 + 40, y + 3, 0x2716, "Hidden", new GumpStateCallback(Status), OnlineStatus.Hidden);

                AddBackgroundZero(0, 0, width, y + 40, Data.GetData(Owner).DefaultBack);
            }

            private void Status(object o)
            {
                if (!(o is OnlineStatus))
                    return;

                Data.GetData(Owner).Status = (OnlineStatus)o;

                if ((OnlineStatus)o == OnlineStatus.Away || (OnlineStatus)o == OnlineStatus.Busy)
                    new AwayGump(Owner, c_Gump);
                else
                    c_Gump.NewGump();
            }

            protected override void OnClose()
            {
                c_Gump.NewGump();
            }
        }


        private class AwayGump : GumpPlus
        {
            private GumpPlus c_Gump;

            public AwayGump(Mobile m, GumpPlus g)
                : base(m, 100, 100)
            {
                m.CloseGump(typeof(AwayGump));

                c_Gump = g;
            }

            protected override void BuildGump()
            {
                AddBackground(0, 0, 200, 200, Data.GetData(Owner).DefaultBack);

                AddHtml(0, 10, 200, "<CENTER>" + General.Local(12));
                AddTextField(10, 30, 180, 120, 0x480, 0xBBC, "Away", Data.GetData(Owner).AwayMsg);
                AddButton(60, 160, 0xFB1, 0xFB3, "Clear", new GumpCallback(ClearMsg));
                AddButton(120, 160, 0xFB7, 0xFB9, "Submit", new GumpCallback(Submit));
            }

            private void ClearMsg()
            {
                Data.GetData(Owner).AwayMsg = "";
                NewGump();
            }

            private void Submit()
            {
                Data.GetData(Owner).AwayMsg = GetTextField("Away");
                c_Gump.NewGump();
            }

            protected override void OnClose()
            {
                c_Gump.NewGump();
            }
        }

        #endregion
    }
}