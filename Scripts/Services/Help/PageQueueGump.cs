using System;
using System.Collections;
using System.IO;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.Help
{
    public class MessageSentGump : Gump
    {
        private readonly string m_Name;
        private readonly string m_Text;
        private readonly Mobile m_Mobile;
        public MessageSentGump(Mobile mobile, string name, string text)
            : base(30, 30)
        {
            this.m_Name = name;
            this.m_Text = text;
            this.m_Mobile = mobile;

            this.Closable = false;

            this.AddPage(0);

            this.AddBackground(0, 0, 92, 75, 0xA3C);

            this.AddImageTiled(5, 7, 82, 61, 0xA40);
            this.AddAlphaRegion(5, 7, 82, 61);

            this.AddImageTiled(9, 11, 21, 53, 0xBBC);

            this.AddButton(10, 12, 0x7D2, 0x7D2, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(34, 28, 65, 24, 3001002, 0xFFFFFF, false, false); // Message
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            this.m_Mobile.SendGump(new PageResponseGump(this.m_Mobile, this.m_Name, this.m_Text));
            //m_Mobile.SendMessage( 0x482, "{0} tells you:", m_Name );
            //m_Mobile.SendMessage( 0x482, m_Text );
        }
    }

    public class PageQueueGump : Gump
    {
        private readonly PageEntry[] m_List;
        public PageQueueGump()
            : base(30, 30)
        {
            this.Add(new GumpPage(0));
            //Add( new GumpBackground( 0, 0, 410, 448, 9200 ) );
            this.Add(new GumpImageTiled(0, 0, 410, 448, 0xA40));
            this.Add(new GumpAlphaRegion(1, 1, 408, 446));

            this.Add(new GumpLabel(180, 12, 2100, "Page Queue"));

            ArrayList list = PageQueue.List;

            for (int i = 0; i < list.Count;)
            {
                PageEntry e = (PageEntry)list[i];

                if (e.Sender.Deleted)
                {
                    e.AddResponse(e.Sender, "[Logout]");
                    PageQueue.Remove(e);
                }
                else
                {
                    ++i;
                }
            }

            this.m_List = (PageEntry[])list.ToArray(typeof(PageEntry));

            if (this.m_List.Length > 0)
            {
                this.Add(new GumpPage(1));

                for (int i = 0; i < this.m_List.Length; ++i)
                {
                    PageEntry e = this.m_List[i];

                    if (i >= 5 && (i % 5) == 0)
                    {
                        this.AddButton(368, 12, 0xFA5, 0xFA7, 0, GumpButtonType.Page, (i / 5) + 1);
                        this.Add(new GumpLabel(298, 12, 2100, "Next Page"));
                        this.Add(new GumpPage((i / 5) + 1));
                        this.AddButton(12, 12, 0xFAE, 0xFB0, 0, GumpButtonType.Page, (i / 5));
                        this.Add(new GumpLabel(48, 12, 2100, "Previous Page"));
                    }

                    string typeString = PageQueue.GetPageTypeName(e.Type);

                    string html = String.Format("[{0}] {1} <basefont color=#{2:X6}>[<u>{3}</u>]</basefont>", typeString, e.Message, e.Handler == null ? 0xFF0000 : 0xFF, e.Handler == null ? "Unhandled" : "Handling");

                    this.Add(new GumpHtml(12, 44 + ((i % 5) * 80), 350, 70, html, true, true));
                    this.AddButton(370, 44 + ((i % 5) * 80) + 24, 0xFA5, 0xFA7, i + 1, GumpButtonType.Reply, 0);
                }
            }
            else
            {
                this.Add(new GumpLabel(12, 44, 2100, "The page queue is empty."));
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID >= 1 && info.ButtonID <= this.m_List.Length)
            {
                if (PageQueue.List.IndexOf(this.m_List[info.ButtonID - 1]) >= 0)
                {
                    PageEntryGump g = new PageEntryGump(state.Mobile, this.m_List[info.ButtonID - 1]);

                    g.SendTo(state);
                }
                else
                {
                    state.Mobile.SendGump(new PageQueueGump());
                    state.Mobile.SendMessage("That page has been removed.");
                }
            }
        }
    }

    public class PredefinedResponse
    {
        private static ArrayList m_List;
        private string m_Title;
        private string m_Message;
        public PredefinedResponse(string title, string message)
        {
            this.m_Title = title;
            this.m_Message = message;
        }

        public static ArrayList List
        {
            get
            {
                if (m_List == null)
                    m_List = Load();

                return m_List;
            }
        }
        public string Title
        {
            get
            {
                return this.m_Title;
            }
            set
            {
                this.m_Title = value;
            }
        }
        public string Message
        {
            get
            {
                return this.m_Message;
            }
            set
            {
                this.m_Message = value;
            }
        }
        public static PredefinedResponse Add(string title, string message)
        {
            if (m_List == null)
                m_List = Load();

            PredefinedResponse resp = new PredefinedResponse(title, message);

            m_List.Add(resp);
            Save();

            return resp;
        }

        public static void Save()
        {
            if (m_List == null)
                m_List = Load();

            try
            {
                string path = Path.Combine(Core.BaseDirectory, "Data/pageresponse.cfg");

                using (StreamWriter op = new StreamWriter(path))
                {
                    for (int i = 0; i < m_List.Count; ++i)
                    {
                        PredefinedResponse resp = (PredefinedResponse)m_List[i];

                        op.WriteLine("{0}\t{1}", resp.Title, resp.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static ArrayList Load()
        {
            ArrayList list = new ArrayList();

            string path = Path.Combine(Core.BaseDirectory, "Data/pageresponse.cfg");

            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader ip = new StreamReader(path))
                    {
                        string line;

                        while ((line = ip.ReadLine()) != null)
                        {
                            try
                            {
                                line = line.Trim();

                                if (line.Length == 0 || line.StartsWith("#"))
                                    continue;

                                string[] split = line.Split('\t');

                                if (split.Length == 2)
                                    list.Add(new PredefinedResponse(split[0], split[1]));
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return list;
        }
    }

    public class PredefGump : Gump
    {
        private const int LabelColor32 = 0xFFFFFF;
        private readonly Mobile m_From;
        private readonly PredefinedResponse m_Response;
        public PredefGump(Mobile from, PredefinedResponse response)
            : base(30, 30)
        {
            this.m_From = from;
            this.m_Response = response;

            from.CloseGump(typeof(PredefGump));

            bool canEdit = (from.AccessLevel >= AccessLevel.GameMaster);

            this.AddPage(0);

            if (response == null)
            {
                this.AddImageTiled(0, 0, 410, 448, 0xA40);
                this.AddAlphaRegion(1, 1, 408, 446);

                this.AddHtml(10, 10, 390, 20, this.Color(this.Center("Predefined Responses"), LabelColor32), false, false);

                ArrayList list = PredefinedResponse.List;

                this.AddPage(1);

                int i;

                for (i = 0; i < list.Count; ++i)
                {
                    if (i >= 5 && (i % 5) == 0)
                    {
                        this.AddButton(368, 10, 0xFA5, 0xFA7, 0, GumpButtonType.Page, (i / 5) + 1);
                        this.AddLabel(298, 10, 2100, "Next Page");
                        this.AddPage((i / 5) + 1);
                        this.AddButton(12, 10, 0xFAE, 0xFB0, 0, GumpButtonType.Page, i / 5);
                        this.AddLabel(48, 10, 2100, "Previous Page");
                    }

                    PredefinedResponse resp = (PredefinedResponse)list[i];

                    string html = String.Format("<u>{0}</u><br>{1}", resp.Title, resp.Message);

                    this.AddHtml(12, 44 + ((i % 5) * 80), 350, 70, html, true, true);

                    if (canEdit)
                    {
                        this.AddButton(370, 44 + ((i % 5) * 80) + 24, 0xFA5, 0xFA7, 2 + (i * 3), GumpButtonType.Reply, 0);

                        if (i > 0)
                            this.AddButton(377, 44 + ((i % 5) * 80) + 2, 0x15E0, 0x15E4, 3 + (i * 3), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(377, 44 + ((i % 5) * 80) + 2, 0x25E4);

                        if (i < (list.Count - 1))
                            this.AddButton(377, 44 + ((i % 5) * 80) + 70 - 2 - 16, 0x15E2, 0x15E6, 4 + (i * 3), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(377, 44 + ((i % 5) * 80) + 70 - 2 - 16, 0x25E8);
                    }
                }

                if (canEdit)
                {
                    if (i >= 5 && (i % 5) == 0)
                    {
                        this.AddButton(368, 10, 0xFA5, 0xFA7, 0, GumpButtonType.Page, (i / 5) + 1);
                        this.AddLabel(298, 10, 2100, "Next Page");
                        this.AddPage((i / 5) + 1);
                        this.AddButton(12, 10, 0xFAE, 0xFB0, 0, GumpButtonType.Page, i / 5);
                        this.AddLabel(48, 10, 2100, "Previous Page");
                    }

                    this.AddButton(12, 44 + ((i % 5) * 80), 0xFAB, 0xFAD, 1, GumpButtonType.Reply, 0);
                    this.AddHtml(45, 44 + ((i % 5) * 80), 200, 20, this.Color("New Response", LabelColor32), false, false);
                }
            }
            else if (canEdit)
            {
                this.AddImageTiled(0, 0, 410, 250, 0xA40);
                this.AddAlphaRegion(1, 1, 408, 248);

                this.AddHtml(10, 10, 390, 20, this.Color(this.Center("Predefined Response Editor"), LabelColor32), false, false);

                this.AddButton(10, 40, 0xFB1, 0xFB3, 1, GumpButtonType.Reply, 0);
                this.AddHtml(45, 40, 200, 20, this.Color("Remove", LabelColor32), false, false);

                this.AddButton(10, 70, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
                this.AddHtml(45, 70, 200, 20, this.Color("Title:", LabelColor32), false, false);
                this.AddTextInput(10, 90, 300, 20, 0, response.Title);

                this.AddButton(10, 120, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);
                this.AddHtml(45, 120, 200, 20, this.Color("Message:", LabelColor32), false, false);
                this.AddTextInput(10, 140, 390, 100, 1, response.Message);
            }
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public void AddTextInput(int x, int y, int w, int h, int id, string def)
        {
            this.AddImageTiled(x, y, w, h, 0xA40);
            this.AddImageTiled(x + 1, y + 1, w - 2, h - 2, 0xBBC);
            this.AddTextEntry(x + 3, y + 1, w - 4, h - 2, 0x480, id, def);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_From.AccessLevel < AccessLevel.Administrator)
                return;

            if (this.m_Response == null)
            {
                int index = info.ButtonID - 1;

                if (index == 0)
                {
                    PredefinedResponse resp = new PredefinedResponse("", "");

                    ArrayList list = PredefinedResponse.List;
                    list.Add(resp);

                    this.m_From.SendGump(new PredefGump(this.m_From, resp));
                }
                else
                {
                    --index;

                    int type = index % 3;
                    index /= 3;

                    ArrayList list = PredefinedResponse.List;

                    if (index >= 0 && index < list.Count)
                    {
                        PredefinedResponse resp = (PredefinedResponse)list[index];

                        switch ( type )
                        {
                            case 0: // edit
                                {
                                    this.m_From.SendGump(new PredefGump(this.m_From, resp));
                                    break;
                                }
                            case 1: // move up
                                {
                                    if (index > 0)
                                    {
                                        list.RemoveAt(index);
                                        list.Insert(index - 1, resp);

                                        PredefinedResponse.Save();
                                        this.m_From.SendGump(new PredefGump(this.m_From, null));
                                    }

                                    break;
                                }
                            case 2: // move down
                                {
                                    if (index < (list.Count - 1))
                                    {
                                        list.RemoveAt(index);
                                        list.Insert(index + 1, resp);

                                        PredefinedResponse.Save();
                                        this.m_From.SendGump(new PredefGump(this.m_From, null));
                                    }

                                    break;
                                }
                        }
                    }
                }
            }
            else
            {
                ArrayList list = PredefinedResponse.List;

                switch ( info.ButtonID )
                {
                    case 1:
                        {
                            list.Remove(this.m_Response);

                            PredefinedResponse.Save();
                            this.m_From.SendGump(new PredefGump(this.m_From, null));
                            break;
                        }
                    case 2:
                        {
                            TextRelay te = info.GetTextEntry(0);

                            if (te != null)
                                this.m_Response.Title = te.Text;

                            PredefinedResponse.Save();
                            this.m_From.SendGump(new PredefGump(this.m_From, this.m_Response));

                            break;
                        }
                    case 3:
                        {
                            TextRelay te = info.GetTextEntry(1);

                            if (te != null)
                                this.m_Response.Message = te.Text;

                            PredefinedResponse.Save();
                            this.m_From.SendGump(new PredefGump(this.m_From, this.m_Response));

                            break;
                        }
                }
            }
        }
    }

    public class PageEntryGump : Gump
    {
        private static readonly int[] m_AccessLevelHues = new int[]
        {
                2100, //Player
                2122, //VIP
                2122, //Counselor
                2117, //Decorator
                2117, //Spawner
                2117, //GameMaster
                2129, //Seer
                2415, //Admin
                2415, //Developer
                2415, //CoOwner
                2415  //Owner
        }; 
        
        private readonly PageEntry m_Entry;
        private readonly Mobile m_Mobile;
        public PageEntryGump(Mobile m, PageEntry entry)
            : base(30, 30)
        {
            try
            {
                this.m_Mobile = m;
                this.m_Entry = entry;

                int buttons = 0;

                int bottom = 356;

                this.AddPage(0);

                this.AddImageTiled(0, 0, 410, 456, 0xA40);
                this.AddAlphaRegion(1, 1, 408, 454);

                this.AddPage(1);

                this.AddLabel(18, 18, 2100, "Sent:");
                this.AddLabelCropped(128, 18, 264, 20, 2100, entry.Sent.ToString());

                this.AddLabel(18, 38, 2100, "Sender:");
                this.AddLabelCropped(128, 38, 264, 20, 2100, String.Format("{0} {1} [{2}]", entry.Sender.RawName, entry.Sender.Location, entry.Sender.Map));

                this.AddButton(18, bottom - (buttons * 22), 0xFAB, 0xFAD, 8, GumpButtonType.Reply, 0);
                this.AddImageTiled(52, bottom - (buttons * 22) + 1, 340, 80, 0xA40/*0xBBC*//*0x2458*/);
                this.AddImageTiled(53, bottom - (buttons * 22) + 2, 338, 78, 0xBBC/*0x2426*/);
                this.AddTextEntry(55, bottom - (buttons++ * 22) + 2, 336, 78, 0x480, 0, "");

                this.AddButton(18, bottom - (buttons * 22), 0xFA5, 0xFA7, 0, GumpButtonType.Page, 2);
                this.AddLabel(52, bottom - (buttons++ * 22), 2100, "Predefined Response");

                if (entry.Sender != m)
                {
                    this.AddButton(18, bottom - (buttons * 22), 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                    this.AddLabel(52, bottom - (buttons++ * 22), 2100, "Go to Sender");
                }

                this.AddLabel(18, 58, 2100, "Handler:");

                if (entry.Handler == null)
                {
                    this.AddLabelCropped(128, 58, 264, 20, 2100, "Unhandled");

                    this.AddButton(18, bottom - (buttons * 22), 0xFB1, 0xFB3, 5, GumpButtonType.Reply, 0);
                    this.AddLabel(52, bottom - (buttons++ * 22), 2100, "Delete Page");

                    this.AddButton(18, bottom - (buttons * 22), 0xFB7, 0xFB9, 4, GumpButtonType.Reply, 0);
                    this.AddLabel(52, bottom - (buttons++ * 22), 2100, "Handle Page");
                }
                else
                {
                    this.AddLabelCropped(128, 58, 264, 20, m_AccessLevelHues[(int)entry.Handler.AccessLevel], entry.Handler.Name);

                    if (entry.Handler != m)
                    {
                        this.AddButton(18, bottom - (buttons * 22), 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
                        this.AddLabel(52, bottom - (buttons++ * 22), 2100, "Go to Handler");
                    }
                    else
                    {
                        this.AddButton(18, bottom - (buttons * 22), 0xFA2, 0xFA4, 6, GumpButtonType.Reply, 0);
                        this.AddLabel(52, bottom - (buttons++ * 22), 2100, "Abandon Page");

                        this.AddButton(18, bottom - (buttons * 22), 0xFB7, 0xFB9, 7, GumpButtonType.Reply, 0);
                        this.AddLabel(52, bottom - (buttons++ * 22), 2100, "Page Handled");
                    }
                }

                this.AddLabel(18, 78, 2100, "Page Location:");
                this.AddLabelCropped(128, 78, 264, 20, 2100, String.Format("{0} [{1}]", entry.PageLocation, entry.PageMap));

                this.AddButton(18, bottom - (buttons * 22), 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);
                this.AddLabel(52, bottom - (buttons++ * 22), 2100, "Go to Page Location");

                if (entry.SpeechLog != null)
                {
                    this.AddButton(18, bottom - (buttons * 22), 0xFA5, 0xFA7, 10, GumpButtonType.Reply, 0);
                    this.AddLabel(52, bottom - (buttons++ * 22), 2100, "View Speech Log");
                }

                this.AddLabel(18, 98, 2100, "Page Type:");
                this.AddLabelCropped(128, 98, 264, 20, 2100, PageQueue.GetPageTypeName(entry.Type));

                this.AddLabel(18, 118, 2100, "Message:");
                this.AddHtml(128, 118, 250, 100, entry.Message, true, true);

                this.AddPage(2);

                ArrayList preresp = PredefinedResponse.List;

                this.AddButton(18, 18, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1);
                this.AddButton(410 - 18 - 32, 18, 0xFAB, 0xFAC, 9, GumpButtonType.Reply, 0);

                if (preresp.Count == 0)
                {
                    this.AddLabel(52, 18, 2100, "There are no predefined responses.");
                }
                else
                {
                    this.AddLabel(52, 18, 2100, "Back");

                    for (int i = 0; i < preresp.Count; ++i)
                    {
                        this.AddButton(18, 40 + (i * 22), 0xFA5, 0xFA7, 100 + i, GumpButtonType.Reply, 0);
                        this.AddLabel(52, 40 + (i * 22), 2100, ((PredefinedResponse)preresp[i]).Title);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Resend(NetState state)
        {
            PageEntryGump g = new PageEntryGump(this.m_Mobile, this.m_Entry);

            g.SendTo(state);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID != 0 && PageQueue.List.IndexOf(this.m_Entry) < 0)
            {
                state.Mobile.SendGump(new PageQueueGump());
                state.Mobile.SendMessage("That page has been removed.");
                return;
            }

            switch ( info.ButtonID )
            {
                case 0: // close
                    {
                        if (this.m_Entry.Handler != state.Mobile)
                        {
                            PageQueueGump g = new PageQueueGump();

                            g.SendTo(state);
                        }

                        break;
                    }
                case 1: // go to sender
                    {
                        Mobile m = state.Mobile;

                        if (this.m_Entry.Sender.Deleted)
                        {
                            m.SendMessage("That character no longer exists.");
                        }
                        else if (this.m_Entry.Sender.Map == null || this.m_Entry.Sender.Map == Map.Internal)
                        {
                            m.SendMessage("That character is not in the world.");
                        }
                        else
                        {
                            this.m_Entry.AddResponse(state.Mobile, "[Go Sender]");
                            m.MoveToWorld(this.m_Entry.Sender.Location, this.m_Entry.Sender.Map);

                            m.SendMessage("You have been teleported to that page's sender.");

                            this.Resend(state);
                        }

                        break;
                    }
                case 2: // go to handler
                    {
                        Mobile m = state.Mobile;
                        Mobile h = this.m_Entry.Handler;

                        if (h != null)
                        {
                            if (h.Deleted)
                            {
                                m.SendMessage("That character no longer exists.");
                            }
                            else if (h.Map == null || h.Map == Map.Internal)
                            {
                                m.SendMessage("That character is not in the world.");
                            }
                            else
                            {
                                this.m_Entry.AddResponse(state.Mobile, "[Go Handler]");
                                m.MoveToWorld(h.Location, h.Map);

                                m.SendMessage("You have been teleported to that page's handler.");
                                this.Resend(state);
                            }
                        }
                        else
                        {
                            m.SendMessage("Nobody is handling that page.");
                            this.Resend(state);
                        }

                        break;
                    }
                case 3: // go to page location
                    {
                        Mobile m = state.Mobile;

                        if (this.m_Entry.PageMap == null || this.m_Entry.PageMap == Map.Internal)
                        {
                            m.SendMessage("That location is not in the world.");
                        }
                        else
                        {
                            this.m_Entry.AddResponse(state.Mobile, "[Go PageLoc]");
                            m.MoveToWorld(this.m_Entry.PageLocation, this.m_Entry.PageMap);

                            state.Mobile.SendMessage("You have been teleported to the original page location.");

                            this.Resend(state);
                        }

                        break;
                    }
                case 4: // handle page
                    {
                        if (this.m_Entry.Handler == null)
                        {
                            this.m_Entry.AddResponse(state.Mobile, "[Handling]");
                            this.m_Entry.Handler = state.Mobile;

                            state.Mobile.SendMessage("You are now handling the page.");
                        }
                        else
                        {
                            state.Mobile.SendMessage("Someone is already handling that page.");
                        }

                        this.Resend(state);

                        break;
                    }
                case 5: // delete page
                    {
                        if (this.m_Entry.Handler == null)
                        {
                            this.m_Entry.AddResponse(state.Mobile, "[Deleting]");
                            PageQueue.Remove(this.m_Entry);

                            state.Mobile.SendMessage("You delete the page.");

                            PageQueueGump g = new PageQueueGump();

                            g.SendTo(state);
                        }
                        else
                        {
                            state.Mobile.SendMessage("Someone is handling that page, it can not be deleted.");

                            this.Resend(state);
                        }

                        break;
                    }
                case 6: // abandon page
                    {
                        if (this.m_Entry.Handler == state.Mobile)
                        {
                            this.m_Entry.AddResponse(state.Mobile, "[Abandoning]");
                            state.Mobile.SendMessage("You abandon the page.");

                            this.m_Entry.Handler = null;
                        }
                        else
                        {
                            state.Mobile.SendMessage("You are not handling that page.");
                        }

                        this.Resend(state);

                        break;
                    }
                case 7: // page handled
                    {
                        if (this.m_Entry.Handler == state.Mobile)
                        {
                            this.m_Entry.AddResponse(state.Mobile, "[Handled]");
                            PageQueue.Remove(this.m_Entry);

                            this.m_Entry.Handler = null;

                            state.Mobile.SendMessage("You mark the page as handled, and remove it from the queue.");

                            PageQueueGump g = new PageQueueGump();

                            g.SendTo(state);
                        }
                        else
                        {
                            state.Mobile.SendMessage("You are not handling that page.");

                            this.Resend(state);
                        }

                        break;
                    }
                case 8: // Send message
                    {
                        TextRelay text = info.GetTextEntry(0);

                        if (text != null)
                        {
                            this.m_Entry.AddResponse(state.Mobile, "[Response] " + text.Text);
                            this.m_Entry.Sender.SendGump(new MessageSentGump(this.m_Entry.Sender, state.Mobile.Name, text.Text));
                            //m_Entry.Sender.SendMessage( 0x482, "{0} tells you:", state.Mobile.Name );
                            //m_Entry.Sender.SendMessage( 0x482, text.Text );
                        }

                        this.Resend(state);

                        break;
                    }
                case 9: // predef overview
                    {
                        this.Resend(state);
                        state.Mobile.SendGump(new PredefGump(state.Mobile, null));

                        break;
                    }
                case 10: // View Speech Log
                    {
                        this.Resend(state);

                        if (this.m_Entry.SpeechLog != null)
                        {
                            Gump gump = new SpeechLogGump(this.m_Entry.Sender, this.m_Entry.SpeechLog);
                            state.Mobile.SendGump(gump);
                        }

                        break;
                    }
                default:
                    {
                        int index = info.ButtonID - 100;
                        ArrayList preresp = PredefinedResponse.List;

                        if (index >= 0 && index < preresp.Count)
                        {
                            this.m_Entry.AddResponse(state.Mobile, "[PreDef] " + ((PredefinedResponse)preresp[index]).Title);
                            this.m_Entry.Sender.SendGump(new MessageSentGump(this.m_Entry.Sender, state.Mobile.Name, ((PredefinedResponse)preresp[index]).Message));
                        }

                        this.Resend(state);

                        break;
                    }
            }
        }
    }
}