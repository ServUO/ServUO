using System;
using System.Collections;
using Server;
using Server.Gumps;

namespace Knives.Chat3
{
    public class ListGump20 : GumpPlus
    {
        public enum ListPage { All, Channel, Mail, Friends, Ignores, GIgnores, GListens, Bans, Notifications }

        #region Class Definitions

        private ListPage c_ListPage;
        private Mobile c_Target;
        private int c_Page;
        private bool c_Menu, c_Search;
        private string c_TxtSearch = "";
        private string c_CharSearch = "";

        protected ListPage CurrentPage { get { return c_ListPage; } }
        public Mobile Current { get { return (c_Target == null ? Owner : c_Target); } }

        #endregion

        #region Constructors

        public ListGump20(Mobile m, int page)
            : base(m, 100, 100)
        {
            c_ListPage = (ListPage)page;

            m.CloseGump(typeof(ListGump20));

            Override = true;
        }

        public ListGump20(Mobile m, Mobile targ)
            : base(m, 100, 100)
        {
            c_Target = targ;

            m.CloseGump(typeof(ListGump20));

            Override = true;
        }

        public ListGump20(Mobile m)
            : this(m, null)
        {
            m.CloseGump(typeof(ListGump20));

            Override = true;
        }

        #endregion

        #region Methods

        protected override void BuildGump()
        {
            if (c_ListPage == ListPage.Channel && Data.GetData(Current).CurrentChannel == null)
                c_ListPage = ListPage.All;

            int width = Data.GetData(Current).QuickBar ? 250 : 200;
            int y = 10;
            int perpage = 10;
            int bar = width - 18;

            if (c_ListPage == ListPage.Mail || (c_ListPage == ListPage.Channel && (Data.GetData(Current).CurrentChannel is Guild || Data.GetData(Current).CurrentChannel is Faction)))
                perpage /= 2;

            if (c_Search)
                ShowSearch(width);

            ArrayList list = GetList();
            SearchFilter(list);
            list.Sort(new InternalSort(this));

            if (c_Page != 0)
                AddButton(width / 2 - 10, y - 5, 0x15E0, 0x15E4, "Page Down", new GumpCallback(PageDown));

            if (c_Target != null)
            {
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(224) + " " + c_Target.RawName);
                AddButton(width / 2 - 80, y, 0x5686, "Clear Viewing", new GumpCallback(ClearViewing));
                AddButton(width / 2 + 65, y, 0x5686, "Clear Viewing", new GumpCallback(ClearViewing));
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

                    if (c_ListPage == ListPage.Channel && Data.GetData(Current).CurrentChannel is Guild)
                        AddHtml(35, y += 20, width - 35, ((Mobile)list[i]).GuildTitle);
                    else if (c_ListPage == ListPage.Channel && Data.GetData(Current).CurrentChannel is Faction)
                        AddHtml(35, y += 20, width - 35, General.FactionTitle((Mobile)list[i]));

                    if (list[i] != Current && Data.GetData(Current).QuickBar)
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

                if (i + 1 < list.Count && i + 1 < (c_Page+1)*perpage)
                    AddBackground(50, y + 18, width - 100, 3, 0x13BE);
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

            if (perpage * (c_Page + 1) < list.Count)
                AddButton(width / 2 - 10, y+=25, 0x15E2, 0x15E6, "Page Up", new GumpCallback(PageUp));

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

            if (c_Menu)
                y = ShowFullMenu(y, width);
            else
            {
                AddButton(33+(50/2), (y += 25)+3, 0x29F6, "Full List", new GumpCallback(FullList));
                AddHtml(40, y, 50, "<CENTER>" + GetTitle());
                AddButton(10, y, c_Menu ? 0x15E0 : 0x15E2, c_Menu ? 0x15E4 : 0x15E6, "Menu", new GumpCallback(Menu));
            }

            AddButton(width- 110, y, 0x5689, "Help", new GumpCallback(Help));
            AddButton(width - 95, y, 0x5689, "Profile", new GumpStateCallback(Profile), Current);
            AddLabel(width - 92, y-1, 0x47E, "P");
            AddButton(width - 80, y, 0x5689, "Options", new GumpCallback(Options));
            AddLabel(width - 77, y-1, 0x47E, "O");
            AddButton(width - 65, y, 0x5689, "Status", new GumpCallback(Status));
            AddLabel(width - 62, y-1, Data.GetData(Current).Status == OnlineStatus.Online ? 0x47E : 0x34, "A");
            AddButton(width - 50, y, 0x5689, "QuickBar", new GumpCallback(QuickBar));
            AddLabel(width - 47, y-1, Data.GetData(Current).QuickBar ? 0x34 : 0x47E, "Q");
            AddButton(width - 35, y, 0x5689, "Search", new GumpCallback(Search));
            AddLabel(width - 32, y-1, c_Search ? 0x34 : 0x47E, "S");

            AddBackgroundZero(0, 0, width, y + 20, Data.GetData(Current).DefaultBack);
        }

        private int ShowFullMenu(int y, int width)
        {
            AddButton(width / 2 - 83 + (50/2), (y += 25)+3, 0x29F6, "Channels", new GumpCallback(Channels));
            AddHtml(width / 2 - 75, y, 50, (c_ListPage == ListPage.Channel ? HTML.Green : "") + "<CENTER>" + (c_ListPage == ListPage.Channel ? GetTitle() : General.Local(38)));

            AddButton(width / 2 - 33 + (50 / 2), y + 3, 0x29F6, "Friends", new GumpStateCallback(Page), ListPage.Friends);
            AddHtml(width / 2 - 25, y, 50, (c_ListPage == ListPage.Friends ? HTML.Green : "") + "<CENTER>" + General.Local(203));

            AddButton(width / 2 + 27 + (50 / 2), y + 3, 0x29F6, "Ignores", new GumpStateCallback(Page), ListPage.Ignores);
            AddHtml(width / 2 + 35, y, 50, (c_ListPage == ListPage.Ignores ? HTML.Green : "") + "<CENTER>" + General.Local(51));

            AddButton(width / 2 - 83 + (50 / 2), (y += 15) + 3, 0x29F6, "All", new GumpStateCallback(Page), ListPage.All);
            AddHtml(width / 2 - 75, y, 50, (c_ListPage == ListPage.All ? HTML.Green : "") + "<CENTER>" + General.Local(46));

            AddButton(width / 2 - 33 + (50 / 2), y + 3, 0x29F6, "Mail", new GumpStateCallback(Page), ListPage.Mail);
            AddHtml(width / 2 - 25, y, 50, (c_ListPage == ListPage.Mail ? HTML.Green : "") + "<CENTER>" + General.Local(56));

            AddButton(width / 2 + 27 + (50 / 2), y + 3, 0x29F6, "Views", new GumpCallback(Views));
            AddHtml(width / 2 + 35, y, 50, "<CENTER>" + General.Local(1));

            AddButton(width / 2 - 30, y += 25, 0x8B1, "1.0", new GumpStateCallback(SkinChange), Skin.One);
            AddButton(width / 2 - 10, y, 0x8B2, "2.0", new GumpStateCallback(SkinChange), Skin.Two);
            AddButton(width / 2 + 10, y, 0x8B3, "3.0", new GumpStateCallback(SkinChange), Skin.Three);

            AddButton(10, y += 25, c_Menu ? 0x15E0 : 0x15E2, c_Menu ? 0x15E4 : 0x15E6, "Menu", new GumpCallback(Menu));

            return y;
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
            if (!(o is Mobile))
                return;

            Mobile m = (Mobile)o;

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
            NewGump();
        }

        private void Search()
        {
            c_Search = !c_Search;
            NewGump();
        }

        protected void ViewChannel(object o)
        {
            if (!(o is Channel))
                return;

            Data.GetData(Current).CurrentChannel = (Channel)o;
            c_ListPage = ListPage.Channel;
            NewGump();
        }

        protected void Page(object o)
        {
            if (!(o is ListPage))
                return;

            c_ListPage = (ListPage)o;
            NewGump();
        }

        protected void History()
        {
            NewGump();
            new HistoryGump(Owner, Data.GetData(Current).CurrentChannel);
        }

        protected void GenOpt()
        {
            NewGump();
            new GeneralGump(Owner);
        }

        protected void Logging()
        {
            NewGump();
            new LoggingGump(Owner);
        }

        protected void Filter()
        {
            NewGump();
            new FilterGump(Owner);
        }

        protected void Spam()
        {
            NewGump();
            new SpamGump(Owner);
        }

        protected void Colors()
        {
            NewGump();

            if(c_Target == null )
                new ColorsGump(Owner);
            else
                new ColorsGump(Owner, c_Target);
        }

        protected void Irc()
        {
            NewGump();
            new IrcGump(Owner);
        }

        protected void Multi()
        {
            NewGump();
            new MultiGump(Owner);
        }

        protected void Mail()
        {
            NewGump();

            if (c_Target == null)
                new MailGump(Owner);
            else
                new MailGump(Owner, c_Target);
        }

        protected void GlobalMenu()
        {
            NewGump();
            new GlobalGump(Current);
        }

        protected void Channel()
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
            if (!(obj is Mobile))
                return;

            NewGump();
            Data.GetData(Owner).CheckMsgFrom((Mobile)obj);
        }

        private void FullList()
        {
            new FullGump(Current, this);
        }

        private void Views()
        {
            new ViewsGump(Current, this);
        }

        private void Channels()
        {
            new ChannelSelectGump(Current, this);
        }

        private void Options()
        {
            new OptionsGump(Current, this);
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
            GumpPlus c_Gump;

            public InternalSort(GumpPlus g)
            {
                c_Gump = g;
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

                    return Insensitive.Compare(((Mobile)x).Name, ((Mobile)y).Name);
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

                AddHtml(0, y += 20, width, General.Local(161));
                AddButton(width / 2 - 60, y + 3, 0x2716, "30 minutes", new GumpStateCallback(BanTime), TimeSpan.FromMinutes(30));
                AddButton(width / 2 + 50, y + 3, 0x2716, "30 minutes", new GumpStateCallback(BanTime), TimeSpan.FromMinutes(30));
                AddHtml(0, y += 20, width, General.Local(162));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 hour", new GumpStateCallback(BanTime), TimeSpan.FromHours(1));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 hour", new GumpStateCallback(BanTime), TimeSpan.FromHours(1));
                AddHtml(0, y += 20, width, General.Local(163));
                AddButton(width / 2 - 60, y + 3, 0x2716, "12 hours", new GumpStateCallback(BanTime), TimeSpan.FromHours(12));
                AddButton(width / 2 + 50, y + 3, 0x2716, "12 hours", new GumpStateCallback(BanTime), TimeSpan.FromHours(12));
                AddHtml(0, y += 20, width, General.Local(164));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 day", new GumpStateCallback(BanTime), TimeSpan.FromDays(1));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 day", new GumpStateCallback(BanTime), TimeSpan.FromDays(1));
                AddHtml(0, y += 20, width, General.Local(165));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 week", new GumpStateCallback(BanTime), TimeSpan.FromDays(7));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 week", new GumpStateCallback(BanTime), TimeSpan.FromDays(7));
                AddHtml(0, y += 20, width, General.Local(166));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 month", new GumpStateCallback(BanTime), TimeSpan.FromDays(30));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 month", new GumpStateCallback(BanTime), TimeSpan.FromDays(30));
                AddHtml(0, y += 20, width, General.Local(167));
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

        private class FullGump : GumpPlus
        {
            private ListGump20 c_Gump;
            private Mobile c_Target;

            public FullGump(Mobile m, ListGump20 g)
                : base(g.Owner, 100, 100)
            {
                c_Gump = g;
                c_Target = m;
            }

            protected override void BuildGump()
            {
                int width = 150;
                int y = -10;

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(38));
                AddImage(10, y + 2, 0x39);
                AddImage(width - 40, y + 2, 0x3B);

                y += 5;

                foreach (Chat3.Channel c in Chat3.Channel.Channels)
                {
                    if (!c.CanChat(c_Target, false))
                        continue;

                    AddHtml(40, y += 20, width - 40, (Data.GetData(c_Target).CurrentChannel == c ? HTML.Yellow : HTML.White) + "<CENTER>" + (c.Style == ChatStyle.Regional ? c.Name : c.NameFor(c_Target)));
                    AddButton(20, y, c.IsIn(c_Target) ? 0x2343 : 0x2342, "Join Channel", new GumpStateCallback(JoinChannel), c);

                    if (c.IsIn(c_Target))
                    {
                        AddButton(40 + (width - 40) / 2 - 40, y + 3, 0x2716, "View Channel", new GumpStateCallback(c_Gump.ViewChannel), c);
                        AddButton(40 + (width - 40) / 2 + 30, y + 3, 0x2716, "View Channel", new GumpStateCallback(c_Gump.ViewChannel), c);
                    }
                }

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(1));
                AddImage(0 + 10, y + 2, 0x39);
                AddImage(0 + width - 40, y + 2, 0x3B);

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(46));
                AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.All);
                AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.All);
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(56));
                AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Mail);
                AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Mail);
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(203));
                AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Friends);
                AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Friends);
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(51));
                AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Ignores);
                AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Ignores);

                AddHtml(0, y += 20, width, (c_Gump.CurrentPage == ListPage.Channel ? "" : HTML.Gray) + "<CENTER>" + General.Local(206));
                if (c_Gump.CurrentPage == ListPage.Channel)
                {
                    AddButton(20, y + 3, 0x2716, "History", new GumpCallback(c_Gump.History));
                    AddButton(width - 30, y + 3, 0x2716, "History", new GumpCallback(c_Gump.History));
                }

                if (Data.GetData(c_Target).GlobalAccess)
                {
                    AddHtml(0, y += 20, width, HTML.Red + "<CENTER>" + General.Local(204));
                    AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.GIgnores);
                    AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.GIgnores);
                    AddHtml(0, y += 20, width, HTML.Red + "<CENTER>" + General.Local(205));
                    AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.GListens);
                    AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.GListens);
                }

                if (c_Target.AccessLevel > AccessLevel.GameMaster)
                {
                    AddHtml(0, y += 20, width, HTML.Red + "<CENTER>" + General.Local(54));
                    AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Bans);
                    AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Bans);
                    AddHtml(0, y += 20, width, HTML.Red + "<CENTER>" + General.Local(269));
                    AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Notifications);
                    AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Notifications);
                }

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(40));
                AddImage(10, y + 2, 0x39);
                AddImage(width - 40, y + 2, 0x3B);

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(49));
                AddButton(20, y + 3, 0x2716, "Colors", new GumpCallback(c_Gump.Colors));
                AddButton(width - 30, y + 3, 0x2716, "Colors", new GumpCallback(c_Gump.Colors));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(56));
                AddButton(20, y + 3, 0x2716, "Mail", new GumpCallback(c_Gump.Mail));
                AddButton(width - 30, y + 3, 0x2716, "Mail", new GumpCallback(c_Gump.Mail));

                if (Data.GetData(c_Target).GlobalAccess)
                {
                    AddHtml(0, y += 20, width, HTML.Red + "<CENTER>" + General.Local(43));
                    AddButton(20, y + 3, 0x2716, "Global Menu", new GumpCallback(c_Gump.GlobalMenu));
                    AddButton(width - 30, y + 3, 0x2716, "Global Menu", new GumpCallback(c_Gump.GlobalMenu));
                }

                if (c_Target.AccessLevel >= AccessLevel.Administrator)
                {
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(207));
                    AddButton(20, y + 3, 0x2716, "General", new GumpCallback(c_Gump.GenOpt));
                    AddButton(width - 30, y + 3, 0x2716, "General", new GumpCallback(c_Gump.GenOpt));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(237));
                    AddButton(20, y + 3, 0x2716, "Logging", new GumpCallback(c_Gump.Logging));
                    AddButton(width - 30, y + 3, 0x2716, "Logging", new GumpCallback(c_Gump.Logging));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(208));
                    AddButton(20, y + 3, 0x2716, "Filter", new GumpCallback(c_Gump.Filter));
                    AddButton(width - 30, y + 3, 0x2716, "Filter", new GumpCallback(c_Gump.Filter));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(209));
                    AddButton(20, y + 3, 0x2716, "Spam", new GumpCallback(c_Gump.Spam));
                    AddButton(width - 30, y + 3, 0x2716, "Spam", new GumpCallback(c_Gump.Spam));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(210));
                    AddButton(20, y + 3, 0x2716, "Irc", new GumpCallback(c_Gump.Irc));
                    AddButton(width - 30, y + 3, 0x2716, "Irc", new GumpCallback(c_Gump.Irc));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(287));
                    AddButton(20, y + 3, 0x2716, "Multi", new GumpCallback(c_Gump.Multi));
                    AddButton(width - 30, y + 3, 0x2716, "Multi", new GumpCallback(c_Gump.Multi));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(50));
                    AddButton(20, y + 3, 0x2716, "Channel", new GumpCallback(c_Gump.Channel));
                    AddButton(width - 30, y + 3, 0x2716, "Channel", new GumpCallback(c_Gump.Channel));
                }

                AddBackgroundZero(0, 0, width, y + 40, Data.GetData(c_Target).DefaultBack, false);
            }

            protected void JoinChannel(object o)
            {
                Channel c = o as Channel;

                if (c == null)
                    return;

                if (c.IsIn(c_Target))
                    c.Leave(c_Target);
                else
                    c.Join(c_Target);

                NewGump();
            }

            protected override void OnClose()
            {
                c_Gump.NewGump();
            }
        }

        private class ChannelSelectGump : GumpPlus
        {
            private ListGump20 c_Gump;
            private Mobile c_Target;

            public ChannelSelectGump(Mobile m, ListGump20 g)
                : base(g.Owner, 100, 100)
            {
                c_Gump = g;
                c_Target = m;
            }

            protected override void BuildGump()
            {
                int width = 150;
                int y = -10;

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(38));
                AddImage(10, y + 2, 0x39);
                AddImage(width - 40, y + 2, 0x3B);

                y += 5;

                foreach (Chat3.Channel c in Chat3.Channel.Channels)
                {
                    if (!c.CanChat(c_Target, false))
                        continue;

                    AddHtml(40, y += 20, width - 40, (Data.GetData(c_Target).CurrentChannel == c ? HTML.Yellow : HTML.White) + "<CENTER>" + (c.Style == ChatStyle.Regional ? c.Name : c.NameFor(c_Target)));
                    AddButton(20, y, c.IsIn(c_Target) ? 0x2343 : 0x2342, "Join Channel", new GumpStateCallback(JoinChannel), c);

                    if (c.IsIn(c_Target))
                    {
                        AddButton(40 + (width - 40) / 2 - 40, y + 3, 0x2716, "View Channel", new GumpStateCallback(c_Gump.ViewChannel), c);
                        AddButton(40 + (width - 40) / 2 + 30, y + 3, 0x2716, "View Channel", new GumpStateCallback(c_Gump.ViewChannel), c);
                    }
                }

                AddBackgroundZero(0, 0, width, y + 40, Data.GetData(c_Target).DefaultBack, false);
            }

            private void JoinChannel(object o)
            {
                Channel c = o as Channel;

                if (c == null)
                    return;

                if (c.IsIn(c_Target))
                    c.Leave(c_Target);
                else
                    c.Join(c_Target);

                NewGump();
            }

            protected override void OnClose()
            {
                c_Gump.NewGump();
            }
        }

        private class OptionsGump : GumpPlus
        {
            private ListGump20 c_Gump;
            private Mobile c_Target;

            public OptionsGump(Mobile m, ListGump20 g)
                : base(g.Owner, 100, 100)
            {
                c_Gump = g;
                c_Target = m;
            }

            protected override void BuildGump()
            {
                int width = 150;
                int y = -10;

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(40));
                AddImage(10, y + 2, 0x39);
                AddImage(width - 40, y + 2, 0x3B);

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(49));
                AddButton(20, y + 3, 0x2716, "Colors", new GumpCallback(c_Gump.Colors));
                AddButton(width - 30, y + 3, 0x2716, "Colors", new GumpCallback(c_Gump.Colors));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(56));
                AddButton(20, y + 3, 0x2716, "Mail", new GumpCallback(c_Gump.Mail));
                AddButton(width - 30, y + 3, 0x2716, "Mail", new GumpCallback(c_Gump.Mail));

                if (Data.GetData(c_Target).GlobalAccess)
                {
                    AddHtml(0, y += 20, width, HTML.Red + "<CENTER>" + General.Local(43));
                    AddButton(20, y + 3, 0x2716, "Global Menu", new GumpCallback(c_Gump.GlobalMenu));
                    AddButton(width - 30, y + 3, 0x2716, "Global Menu", new GumpCallback(c_Gump.GlobalMenu));
                }

                if (c_Target.AccessLevel >= AccessLevel.Administrator)
                {
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(207));
                    AddButton(20, y + 3, 0x2716, "General", new GumpCallback(c_Gump.GenOpt));
                    AddButton(width - 30, y + 3, 0x2716, "General", new GumpCallback(c_Gump.GenOpt));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(237));
                    AddButton(20, y + 3, 0x2716, "Logging", new GumpCallback(c_Gump.Logging));
                    AddButton(width - 30, y + 3, 0x2716, "Logging", new GumpCallback(c_Gump.Logging));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(208));
                    AddButton(20, y + 3, 0x2716, "Filter", new GumpCallback(c_Gump.Filter));
                    AddButton(width - 30, y + 3, 0x2716, "Filter", new GumpCallback(c_Gump.Filter));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(209));
                    AddButton(20, y + 3, 0x2716, "Spam", new GumpCallback(c_Gump.Spam));
                    AddButton(width - 30, y + 3, 0x2716, "Spam", new GumpCallback(c_Gump.Spam));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(210));
                    AddButton(20, y + 3, 0x2716, "Irc", new GumpCallback(c_Gump.Irc));
                    AddButton(width - 30, y + 3, 0x2716, "Irc", new GumpCallback(c_Gump.Irc));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(287));
                    AddButton(20, y + 3, 0x2716, "Multi", new GumpCallback(c_Gump.Multi));
                    AddButton(width - 30, y + 3, 0x2716, "Multi", new GumpCallback(c_Gump.Multi));
                    AddHtml(0, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(50));
                    AddButton(20, y + 3, 0x2716, "Channel", new GumpCallback(c_Gump.Channel));
                    AddButton(width - 30, y + 3, 0x2716, "Channel", new GumpCallback(c_Gump.Channel));
                }

                AddBackgroundZero(0, 0, width, y + 40, Data.GetData(c_Target).DefaultBack, false);
            }

            protected override void OnClose()
            {
                c_Gump.NewGump();
            }
        }

        private class ViewsGump : GumpPlus
        {
            private ListGump20 c_Gump;
            private Mobile c_Target;

            public ViewsGump(Mobile m, ListGump20 g)
                : base(g.Owner, 100, 100)
            {
                c_Gump = g;
                c_Target = m;
            }

            protected override void BuildGump()
            {
                int width = 150;
                int y = -10;

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(1));
                AddImage(0 + 10, y + 2, 0x39);
                AddImage(0 + width - 40, y + 2, 0x3B);

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(46));
                AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.All);
                AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.All);
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(56));
                AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Mail);
                AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Mail);
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(203));
                AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Friends);
                AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Friends);
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(51));
                AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Ignores);
                AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Ignores);

                AddHtml(0, y += 20, width, (c_Gump.CurrentPage == ListPage.Channel ? "" : HTML.Gray) + "<CENTER>" + General.Local(206));
                if (c_Gump.CurrentPage == ListPage.Channel)
                {
                    AddButton(20, y + 3, 0x2716, "History", new GumpCallback(c_Gump.History));
                    AddButton(width - 30, y + 3, 0x2716, "History", new GumpCallback(c_Gump.History));
                }

                if (Data.GetData(c_Target).GlobalAccess)
                {
                    AddHtml(0, y += 20, width, HTML.Red + "<CENTER>" + General.Local(204));
                    AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.GIgnores);
                    AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.GIgnores);
                    AddHtml(0, y += 20, width, HTML.Red + "<CENTER>" + General.Local(205));
                    AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.GListens);
                    AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.GListens);
                }

                if (c_Target.AccessLevel > AccessLevel.GameMaster)
                {
                    AddHtml(0, y += 20, width, HTML.Red + "<CENTER>" + General.Local(54));
                    AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Bans);
                    AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Bans);
                    AddHtml(0, y += 20, width, HTML.Red + "<CENTER>" + General.Local(269));
                    AddButton(20, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Notifications);
                    AddButton(width - 30, y + 3, 0x2716, "Page", new GumpStateCallback(c_Gump.Page), ListPage.Notifications);
                }

                AddBackgroundZero(0, 0, width, y + 40, Data.GetData(c_Target).DefaultBack, false);
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