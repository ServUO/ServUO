using Server.Gumps;
using Server.Network;
using System;
using System.Collections;
using System.IO;

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
            m_Name = name;
            m_Text = text;
            m_Mobile = mobile;

            Closable = false;

            AddPage(0);

            AddBackground(0, 0, 92, 75, 0xA3C);

            if (mobile != null && mobile.NetState != null && mobile.NetState.IsEnhancedClient)
                AddBackground(5, 7, 82, 61, 9300);
            else
            {
                AddImageTiled(5, 7, 82, 61, 0xA40);
                AddAlphaRegion(5, 7, 82, 61);
            }

            AddImageTiled(9, 11, 21, 53, 0xBBC);

            AddButton(10, 12, 0x7D2, 0x7D2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(34, 28, 65, 24, 3001002, 0xFFFFFF, false, false); // Message
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            m_Mobile.SendGump(new PageResponseGump(m_Mobile, m_Name, m_Text));
            //m_Mobile.SendMessage( 0x482, "{0} tells you:", m_Name );
            //m_Mobile.SendMessage( 0x482, m_Text );
        }
    }

    public class PageQueueGump : Gump
    {
        private readonly PageEntry[] m_List;
        public PageQueueGump(Mobile m)
            : base(30, 30)
        {
            Add(new GumpPage(0));

            if (m != null && m.NetState != null && m.NetState.IsEnhancedClient)
                AddBackground(1, 1, 408, 446, 9300);
            else
            {
                Add(new GumpImageTiled(0, 0, 410, 448, 0xA40));
                Add(new GumpAlphaRegion(1, 1, 408, 446));
            }

            Add(new GumpLabel(180, 12, 2100, "Page Queue"));

            ArrayList list = PageQueue.List;

            for (int i = 0; i < list.Count;)
            {
                PageEntry e = (PageEntry)list[i];

                if (e.Sender.Deleted)
                {
                    PageQueue.Remove(e);
                }
                else
                {
                    ++i;
                }
            }

            m_List = (PageEntry[])list.ToArray(typeof(PageEntry));

            if (m_List.Length > 0)
            {
                Add(new GumpPage(1));

                for (int i = 0; i < m_List.Length; ++i)
                {
                    PageEntry e = m_List[i];

                    if (i >= 5 && (i % 5) == 0)
                    {
                        AddButton(368, 12, 0xFA5, 0xFA7, 0, GumpButtonType.Page, (i / 5) + 1);
                        Add(new GumpLabel(298, 12, 2100, "Next Page"));
                        Add(new GumpPage((i / 5) + 1));
                        AddButton(12, 12, 0xFAE, 0xFB0, 0, GumpButtonType.Page, (i / 5));
                        Add(new GumpLabel(48, 12, 2100, "Previous Page"));
                    }

                    string typeString = PageQueue.GetPageTypeName(e.Type);

                    string html = string.Format("[{0}] {1} <basefont color=#{2:X6}>[<u>{3}</u>]</basefont>", typeString, e.Message, e.Handler == null ? 0xFF0000 : 0xFF, e.Handler == null ? "Unhandled" : "Handling");

                    Add(new GumpHtml(12, 44 + ((i % 5) * 80), 350, 70, html, true, true));
                    AddButton(370, 44 + ((i % 5) * 80) + 24, 0xFA5, 0xFA7, i + 1, GumpButtonType.Reply, 0);
                }
            }
            else
            {
                Add(new GumpLabel(12, 44, 2100, "The page queue is empty."));
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID >= 1 && info.ButtonID <= m_List.Length)
            {
                if (PageQueue.List.IndexOf(m_List[info.ButtonID - 1]) >= 0)
                {
                    PageEntryGump g = new PageEntryGump(state.Mobile, m_List[info.ButtonID - 1]);

                    g.SendTo(state);
                }
                else
                {
                    state.Mobile.SendGump(new PageQueueGump(state.Mobile));
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
            m_Title = title;
            m_Message = message;
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
                return m_Title;
            }
            set
            {
                m_Title = value;
            }
        }
        public string Message
        {
            get
            {
                return m_Message;
            }
            set
            {
                m_Message = value;
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
                Diagnostics.ExceptionLogging.LogException(e);
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
                            catch (Exception e)
                            {
                                Diagnostics.ExceptionLogging.LogException(e);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
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
            m_From = from;
            m_Response = response;

            from.CloseGump(typeof(PredefGump));

            bool canEdit = (from.AccessLevel >= AccessLevel.GameMaster);

            AddPage(0);

            if (response == null)
            {
                if (from != null && from.NetState != null && from.NetState.IsEnhancedClient)
                    AddBackground(1, 1, 408, 446, 9300);
                else
                {
                    AddImageTiled(0, 0, 410, 448, 0xA40);
                    AddAlphaRegion(1, 1, 408, 446);
                }

                AddHtml(10, 10, 390, 20, Color(Center("Predefined Responses"), LabelColor32), false, false);

                ArrayList list = PredefinedResponse.List;

                AddPage(1);

                int i;

                for (i = 0; i < list.Count; ++i)
                {
                    if (i >= 5 && (i % 5) == 0)
                    {
                        AddButton(368, 10, 0xFA5, 0xFA7, 0, GumpButtonType.Page, (i / 5) + 1);
                        AddLabel(298, 10, 2100, "Next Page");
                        AddPage((i / 5) + 1);
                        AddButton(12, 10, 0xFAE, 0xFB0, 0, GumpButtonType.Page, i / 5);
                        AddLabel(48, 10, 2100, "Previous Page");
                    }

                    PredefinedResponse resp = (PredefinedResponse)list[i];

                    string html = string.Format("<u>{0}</u><br>{1}", resp.Title, resp.Message);

                    AddHtml(12, 44 + ((i % 5) * 80), 350, 70, html, true, true);

                    if (canEdit)
                    {
                        AddButton(370, 44 + ((i % 5) * 80) + 24, 0xFA5, 0xFA7, 2 + (i * 3), GumpButtonType.Reply, 0);

                        if (i > 0)
                            AddButton(377, 44 + ((i % 5) * 80) + 2, 0x15E0, 0x15E4, 3 + (i * 3), GumpButtonType.Reply, 0);
                        else
                            AddImage(377, 44 + ((i % 5) * 80) + 2, 0x25E4);

                        if (i < (list.Count - 1))
                            AddButton(377, 44 + ((i % 5) * 80) + 70 - 2 - 16, 0x15E2, 0x15E6, 4 + (i * 3), GumpButtonType.Reply, 0);
                        else
                            AddImage(377, 44 + ((i % 5) * 80) + 70 - 2 - 16, 0x25E8);
                    }
                }

                if (canEdit)
                {
                    if (i >= 5 && (i % 5) == 0)
                    {
                        AddButton(368, 10, 0xFA5, 0xFA7, 0, GumpButtonType.Page, (i / 5) + 1);
                        AddLabel(298, 10, 2100, "Next Page");
                        AddPage((i / 5) + 1);
                        AddButton(12, 10, 0xFAE, 0xFB0, 0, GumpButtonType.Page, i / 5);
                        AddLabel(48, 10, 2100, "Previous Page");
                    }

                    AddButton(12, 44 + ((i % 5) * 80), 0xFAB, 0xFAD, 1, GumpButtonType.Reply, 0);
                    AddHtml(45, 44 + ((i % 5) * 80), 200, 20, Color("New Response", LabelColor32), false, false);
                }
            }
            else if (canEdit)
            {
                AddImageTiled(0, 0, 410, 250, 0xA40);

                if (from.NetState.IsEnhancedClient)
                    AddBackground(1, 1, 408, 248, 9300);
                else
                    AddAlphaRegion(1, 1, 408, 248);

                AddHtml(10, 10, 390, 20, Color(Center("Predefined Response Editor"), LabelColor32), false, false);

                AddButton(10, 40, 0xFB1, 0xFB3, 1, GumpButtonType.Reply, 0);
                AddHtml(45, 40, 200, 20, Color("Remove", LabelColor32), false, false);

                AddButton(10, 70, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
                AddHtml(45, 70, 200, 20, Color("Title:", LabelColor32), false, false);
                AddTextInput(10, 90, 300, 20, 0, response.Title);

                AddButton(10, 120, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);
                AddHtml(45, 120, 200, 20, Color("Message:", LabelColor32), false, false);
                AddTextInput(10, 140, 390, 100, 1, response.Message);
            }
        }

        public string Center(string text)
        {
            return string.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public void AddTextInput(int x, int y, int w, int h, int id, string def)
        {
            AddImageTiled(x, y, w, h, 0xA40);
            AddImageTiled(x + 1, y + 1, w - 2, h - 2, 0xBBC);
            AddTextEntry(x + 3, y + 1, w - 4, h - 2, 0x480, id, def);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From.AccessLevel < AccessLevel.Administrator)
                return;

            if (m_Response == null)
            {
                int index = info.ButtonID - 1;

                if (index == 0)
                {
                    PredefinedResponse resp = new PredefinedResponse("", "");

                    ArrayList list = PredefinedResponse.List;
                    list.Add(resp);

                    m_From.SendGump(new PredefGump(m_From, resp));
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

                        switch (type)
                        {
                            case 0: // edit
                                {
                                    m_From.SendGump(new PredefGump(m_From, resp));
                                    break;
                                }
                            case 1: // move up
                                {
                                    if (index > 0)
                                    {
                                        list.RemoveAt(index);
                                        list.Insert(index - 1, resp);

                                        PredefinedResponse.Save();
                                        m_From.SendGump(new PredefGump(m_From, null));
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
                                        m_From.SendGump(new PredefGump(m_From, null));
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

                switch (info.ButtonID)
                {
                    case 1:
                        {
                            list.Remove(m_Response);

                            PredefinedResponse.Save();
                            m_From.SendGump(new PredefGump(m_From, null));
                            break;
                        }
                    case 2:
                        {
                            TextRelay te = info.GetTextEntry(0);

                            if (te != null)
                                m_Response.Title = te.Text;

                            PredefinedResponse.Save();
                            m_From.SendGump(new PredefGump(m_From, m_Response));

                            break;
                        }
                    case 3:
                        {
                            TextRelay te = info.GetTextEntry(1);

                            if (te != null)
                                m_Response.Message = te.Text;

                            PredefinedResponse.Save();
                            m_From.SendGump(new PredefGump(m_From, m_Response));

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
                m_Mobile = m;
                m_Entry = entry;

                int buttons = 0;

                int bottom = 356;

                AddPage(0);

                if (m != null && m.NetState != null && m.NetState.IsEnhancedClient)
                    AddBackground(1, 1, 408, 454, 9300);
                else
                {
                    AddImageTiled(0, 0, 410, 456, 0xA40);
                    AddAlphaRegion(1, 1, 408, 454);
                }

                AddPage(1);

                AddLabel(18, 18, 2100, "Sent:");
                AddLabelCropped(128, 18, 264, 20, 2100, entry.Sent.ToString());

                AddLabel(18, 38, 2100, "Sender:");
                AddLabelCropped(128, 38, 264, 20, 2100, string.Format("{0} {1} [{2}]", entry.Sender.RawName, entry.Sender.Location, entry.Sender.Map));

                AddButton(18, bottom - (buttons * 22), 0xFAB, 0xFAD, 8, GumpButtonType.Reply, 0);
                AddImageTiled(52, bottom - (buttons * 22) + 1, 340, 80, 0xA40/*0xBBC*//*0x2458*/);
                AddImageTiled(53, bottom - (buttons * 22) + 2, 338, 78, 0xBBC/*0x2426*/);
                AddTextEntry(55, bottom - (buttons++ * 22) + 2, 336, 78, 0x480, 0, "");

                AddButton(18, bottom - (buttons * 22), 0xFA5, 0xFA7, 0, GumpButtonType.Page, 2);
                AddLabel(52, bottom - (buttons++ * 22), 2100, "Predefined Response");

                if (entry.Sender != m)
                {
                    AddButton(18, bottom - (buttons * 22), 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                    AddLabel(52, bottom - (buttons++ * 22), 2100, "Go to Sender");
                }

                AddLabel(18, 58, 2100, "Handler:");

                if (entry.Handler == null)
                {
                    AddLabelCropped(128, 58, 264, 20, 2100, "Unhandled");

                    AddButton(18, bottom - (buttons * 22), 0xFB1, 0xFB3, 5, GumpButtonType.Reply, 0);
                    AddLabel(52, bottom - (buttons++ * 22), 2100, "Delete Page");

                    AddButton(18, bottom - (buttons * 22), 0xFB7, 0xFB9, 4, GumpButtonType.Reply, 0);
                    AddLabel(52, bottom - (buttons++ * 22), 2100, "Handle Page");
                }
                else
                {
                    AddLabelCropped(128, 58, 264, 20, m_AccessLevelHues[(int)entry.Handler.AccessLevel], entry.Handler.Name);

                    if (entry.Handler != m)
                    {
                        AddButton(18, bottom - (buttons * 22), 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
                        AddLabel(52, bottom - (buttons++ * 22), 2100, "Go to Handler");
                    }
                    else
                    {
                        AddButton(18, bottom - (buttons * 22), 0xFA2, 0xFA4, 6, GumpButtonType.Reply, 0);
                        AddLabel(52, bottom - (buttons++ * 22), 2100, "Abandon Page");

                        AddButton(18, bottom - (buttons * 22), 0xFB7, 0xFB9, 7, GumpButtonType.Reply, 0);
                        AddLabel(52, bottom - (buttons++ * 22), 2100, "Page Handled");
                    }
                }

                AddLabel(18, 78, 2100, "Page Location:");
                AddLabelCropped(128, 78, 264, 20, 2100, string.Format("{0} [{1}]", entry.PageLocation, entry.PageMap));

                AddButton(18, bottom - (buttons * 22), 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);
                AddLabel(52, bottom - (buttons++ * 22), 2100, "Go to Page Location");

                if (entry.SpeechLog != null)
                {
                    AddButton(18, bottom - (buttons * 22), 0xFA5, 0xFA7, 10, GumpButtonType.Reply, 0);
                    AddLabel(52, bottom - (buttons++ * 22), 2100, "View Speech Log");
                }

                AddLabel(18, 98, 2100, "Page Type:");
                AddLabelCropped(128, 98, 264, 20, 2100, PageQueue.GetPageTypeName(entry.Type));

                AddLabel(18, 118, 2100, "Message:");
                AddHtml(128, 118, 250, 100, entry.Message, true, true);

                AddPage(2);

                ArrayList preresp = PredefinedResponse.List;

                AddButton(18, 18, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1);
                AddButton(410 - 18 - 32, 18, 0xFAB, 0xFAC, 9, GumpButtonType.Reply, 0);

                if (preresp.Count == 0)
                {
                    AddLabel(52, 18, 2100, "There are no predefined responses.");
                }
                else
                {
                    AddLabel(52, 18, 2100, "Back");

                    for (int i = 0; i < preresp.Count; ++i)
                    {
                        AddButton(18, 40 + (i * 22), 0xFA5, 0xFA7, 100 + i, GumpButtonType.Reply, 0);
                        AddLabel(52, 40 + (i * 22), 2100, ((PredefinedResponse)preresp[i]).Title);
                    }
                }
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
            }
        }

        public void Resend(NetState state)
        {
            PageEntryGump g = new PageEntryGump(m_Mobile, m_Entry);

            g.SendTo(state);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID != 0 && PageQueue.List.IndexOf(m_Entry) < 0)
            {
                state.Mobile.SendGump(new PageQueueGump(state.Mobile));
                state.Mobile.SendMessage("That page has been removed.");
                return;
            }

            switch (info.ButtonID)
            {
                case 0: // close
                    {
                        if (m_Entry.Handler != state.Mobile)
                        {
                            PageQueueGump g = new PageQueueGump(state.Mobile);

                            g.SendTo(state);
                        }

                        break;
                    }
                case 1: // go to sender
                    {
                        Mobile m = state.Mobile;

                        if (m_Entry.Sender.Deleted)
                        {
                            m.SendMessage("That character no longer exists.");
                        }
                        else if (m_Entry.Sender.Map == null || m_Entry.Sender.Map == Map.Internal)
                        {
                            m.SendMessage("That character is not in the world.");
                        }
                        else
                        {
                            m.MoveToWorld(m_Entry.Sender.Location, m_Entry.Sender.Map);

                            m.SendMessage("You have been teleported to that page's sender.");

                            Resend(state);
                        }

                        break;
                    }
                case 2: // go to handler
                    {
                        Mobile m = state.Mobile;
                        Mobile h = m_Entry.Handler;

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
                                m.MoveToWorld(h.Location, h.Map);

                                m.SendMessage("You have been teleported to that page's handler.");
                                Resend(state);
                            }
                        }
                        else
                        {
                            m.SendMessage("Nobody is handling that page.");
                            Resend(state);
                        }

                        break;
                    }
                case 3: // go to page location
                    {
                        Mobile m = state.Mobile;

                        if (m_Entry.PageMap == null || m_Entry.PageMap == Map.Internal)
                        {
                            m.SendMessage("That location is not in the world.");
                        }
                        else
                        {
                            m.MoveToWorld(m_Entry.PageLocation, m_Entry.PageMap);

                            state.Mobile.SendMessage("You have been teleported to the original page location.");

                            Resend(state);
                        }

                        break;
                    }
                case 4: // handle page
                    {
                        if (m_Entry.Handler == null)
                        {
                            m_Entry.Handler = state.Mobile;

                            state.Mobile.SendMessage("You are now handling the page.");
                        }
                        else
                        {
                            state.Mobile.SendMessage("Someone is already handling that page.");
                        }

                        Resend(state);

                        break;
                    }
                case 5: // delete page
                    {
                        if (m_Entry.Handler == null)
                        {
                            PageQueue.Remove(m_Entry);

                            state.Mobile.SendMessage("You delete the page.");

                            PageQueueGump g = new PageQueueGump(state.Mobile);

                            g.SendTo(state);
                        }
                        else
                        {
                            state.Mobile.SendMessage("Someone is handling that page, it can not be deleted.");

                            Resend(state);
                        }

                        break;
                    }
                case 6: // abandon page
                    {
                        if (m_Entry.Handler == state.Mobile)
                        {
                            state.Mobile.SendMessage("You abandon the page.");

                            m_Entry.Handler = null;
                        }
                        else
                        {
                            state.Mobile.SendMessage("You are not handling that page.");
                        }

                        Resend(state);

                        break;
                    }
                case 7: // page handled
                    {
                        if (m_Entry.Handler == state.Mobile)
                        {
                            PageQueue.Remove(m_Entry);

                            m_Entry.Handler = null;

                            state.Mobile.SendMessage("You mark the page as handled, and remove it from the queue.");

                            PageQueueGump g = new PageQueueGump(state.Mobile);

                            g.SendTo(state);
                        }
                        else
                        {
                            state.Mobile.SendMessage("You are not handling that page.");

                            Resend(state);
                        }

                        break;
                    }
                case 8: // Send message
                    {
                        TextRelay text = info.GetTextEntry(0);

                        if (text != null)
                        {
                            if (m_Entry.Sender.NetState != null)
                            {
                                m_Entry.Sender.SendGump(new MessageSentGump(m_Entry.Sender, state.Mobile.Name, text.Text));
                            }
                            else
                            {
                                ResponseEntry.AddEntry(new ResponseEntry(m_Entry.Sender, state.Mobile, text.Text));
                            }
                        }

                        Resend(state);

                        break;
                    }
                case 9: // predef overview
                    {
                        Resend(state);
                        state.Mobile.SendGump(new PredefGump(state.Mobile, null));

                        break;
                    }
                case 10: // View Speech Log
                    {
                        Resend(state);

                        if (m_Entry.SpeechLog != null)
                        {
                            Gump gump = new SpeechLogGump(m_Entry.Sender, m_Entry.SpeechLog);
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
                            m_Entry.Sender.SendGump(new MessageSentGump(m_Entry.Sender, state.Mobile.Name, ((PredefinedResponse)preresp[index]).Message));
                        }

                        Resend(state);

                        break;
                    }
            }
        }
    }
}
