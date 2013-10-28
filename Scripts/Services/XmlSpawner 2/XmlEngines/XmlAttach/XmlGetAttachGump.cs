using System;
using System.Collections;
using Server.Commands;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

/*
** XmlGetAtt
** Version 1.00
** updated 10/24/04
** ArteGordon
**
*/
namespace Server.Mobiles
{
    public class XmlGetAttGump : Gump
    {
        private const int MaxEntries = 18;
        private const int MaxEntriesPerPage = 18;
        private readonly object m_TargetObject;
        private readonly bool Dosearchtype;
        private readonly bool Dosearchname;
        private readonly bool Dosearchage;
        private readonly bool Searchagedirection;
        private readonly string Searchtype;
        private readonly string Searchname;
        private readonly Mobile m_From;
        private readonly bool[] m_SelectionList;
        private readonly ArrayList m_SearchList;
        private double Searchage;
        private bool Sorttype;
        private bool Sortname;
        private bool Descendingsort;
        private int Selected;
        private int DisplayFrom;
        private bool SelectAll = false;
        public XmlGetAttGump(Mobile from, object targeted, int x, int y)
            : this(from, targeted, true, false,
            false, false, false,
            null,null, false, 0,
            null, -1, 0,
            false,false,
            false, null, x, y)
        {
        }

        public XmlGetAttGump(Mobile from, object targeted, bool firststart, bool descend,
            bool dosearchtype, bool dosearchname, bool dosearchage,
            string searchtype, string searchname, bool searchagedirection, double searchage,
            ArrayList searchlist, int selected, int displayfrom,
            bool sorttype, bool sortname,
            bool selectall, bool[] selectionlist, int X, int Y)
            : base(X,Y)
        {
            this.m_TargetObject = targeted;
            this.m_From = from;
            this.m_SelectionList = selectionlist;
            if (this.m_SelectionList == null)
            {
                this.m_SelectionList = new bool[MaxEntries];
            }
            this.SelectAll = selectall;
            this.Sorttype = sorttype;
            this.Sortname = sortname;

            this.DisplayFrom = displayfrom;
            this.Selected = selected;

            this.Descendingsort = descend;
            this.Dosearchtype = dosearchtype;
            this.Dosearchname = dosearchname;
            this.Dosearchage = dosearchage;

            this.Searchagedirection = searchagedirection;

            this.Searchage = searchage;
            this.Searchtype = searchtype;
            this.Searchname = searchname;

            this.m_SearchList = searchlist;

            if (firststart)
            {
                string status_str;
                this.m_SearchList = this.Search(this.m_TargetObject, out status_str);
            }

            // prepare the page

            this.AddPage(0);

            this.AddBackground(0, 0, 640, 474, 5054);
            this.AddAlphaRegion(0, 0, 640, 474);

            string tnamestr = null;
            if (targeted is Item)
            {
                tnamestr = ((Item)targeted).Name;
            }
            else if (targeted is Mobile)
            {
                tnamestr = ((Mobile)targeted).Name;
            }
            this.AddLabel(2, 0, 0x33, String.Format("Attachments on {0} : {1}", targeted.GetType().Name, tnamestr));

            // add the Sort button
            this.AddButton(5, 450, 0xFAB, 0xFAD, 700, GumpButtonType.Reply, 0);
            this.AddLabel(38, 450, 0x384, "Sort");

            // add the sort direction button
            if (this.Descendingsort)
            {
                this.AddButton(75, 453, 0x15E2, 0x15E6, 701, GumpButtonType.Reply, 0);
                this.AddLabel(100, 450, 0x384, "descend");
            }
            else
            {
                this.AddButton(75, 453, 0x15E0, 0x15E4, 701, GumpButtonType.Reply, 0);
                this.AddLabel(100, 450, 0x384, "ascend");
            }

            // add the Sort on type toggle
            this.AddRadio(155, 450, 0xD2, 0xD3, this.Sorttype, 0);
            this.AddLabel(155, 425, 0x384, "type");

            // add the Sort on name toggle
            this.AddRadio(200, 450, 0xD2, 0xD3, this.Sortname, 1);
            this.AddLabel(200, 425, 0x384, "name");

            this.AddLabel(42, 13, 0x384, "Name");
            this.AddLabel(145, 13, 0x384, "Type");
            this.AddLabel(285, 13, 0x384, "Created");
            this.AddLabel(425, 13, 0x384, "Expires In");
            this.AddLabel(505, 13, 0x384, "Attached By");

            // add the Delete button
            this.AddButton(250, 450, 0xFB1, 0xFB3, 156, GumpButtonType.Reply, 0);
            this.AddLabel(283, 450, 0x384, "Delete");

            // add the page buttons
            for (int i = 0; i < (int)(MaxEntries / MaxEntriesPerPage); i++)
            {
                //AddButton( 38+i*30, 365, 2206, 2206, 0, GumpButtonType.Page, 1+i );
                this.AddButton(418 + i * 25, 450, 0x8B1 + i, 0x8B1 + i, 0, GumpButtonType.Page, 1 + i);
            }

            // add the advance pageblock buttons
            this.AddButton(415 + 25 * (int)(MaxEntries / MaxEntriesPerPage), 450, 0x15E1, 0x15E5, 201, GumpButtonType.Reply, 0); // block forward
            this.AddButton(395, 450, 0x15E3, 0x15E7, 202, GumpButtonType.Reply, 0); // block backward

            // add the displayfrom entry
            this.AddLabel(460, 450, 0x384, "Display");
            this.AddImageTiled(500, 450, 60, 21, 0xBBC);
            this.AddTextEntry(501, 450, 60, 21, 0, 400, this.DisplayFrom.ToString());
            this.AddButton(560, 450, 0xFAB, 0xFAD, 9998, GumpButtonType.Reply, 0);

            // display the item list
            if (this.m_SearchList != null)
            {
                this.AddLabel(320, 425, 68, String.Format("Found {0} attachments", this.m_SearchList.Count));
                this.AddLabel(500, 425, 68, String.Format("Displaying {0}-{1}", this.DisplayFrom,
                    (this.DisplayFrom + MaxEntries < this.m_SearchList.Count ? this.DisplayFrom + MaxEntries : this.m_SearchList.Count)));
            }

            // display the select-all-displayed toggle
            this.AddButton(620, 5, 0xD2, 0xD3, 3999, GumpButtonType.Reply, 0);

            // display the select-all toggle
            this.AddButton(600, 5, (this.SelectAll ? 0xD3 : 0xD2), (this.SelectAll ? 0xD2 : 0xD3), 3998, GumpButtonType.Reply, 0);

            for (int i = 0; i < MaxEntries; i++)
            {
                int index = i + this.DisplayFrom;
                if (this.m_SearchList == null || index >= this.m_SearchList.Count)
                    break;
                int page = (int)(i / MaxEntriesPerPage);
                if (i % MaxEntriesPerPage == 0)
                {
                    this.AddPage(page + 1);
                }

                // background for search results area
                //AddImageTiled( 235, 22 * (i%MaxEntriesPerPage)  + 30, 386, 23, 0x52 );
                //AddImageTiled( 236, 22 * (i%MaxEntriesPerPage) + 31, 384, 21, 0xBBC );

                // add the Props button for each entry
                this.AddButton(5, 22 * (i % MaxEntriesPerPage) + 30, 0xFAB, 0xFAD, 3000 + i, GumpButtonType.Reply, 0);

                string namestr = null;
                string typestr = null;
                string expirestr = null;
                //string description = null;
                string attachedby = null;
                string created = null;

                int texthue = 0;

                object o = (object)this.m_SearchList[index];

                if (o is XmlAttachment)
                {
                    XmlAttachment a = (XmlAttachment)this.m_SearchList[index];

                    namestr = a.Name;
                    typestr = a.GetType().Name;
                    expirestr = a.Expiration.ToString();
                    //description = a.OnIdentify(m_From);
                    created = a.CreationTime.ToString();
                    attachedby = a.AttachedBy;
                }

                bool sel = false;
                if (this.m_SelectionList != null && i < this.m_SelectionList.Length)
                {
                    sel = this.m_SelectionList[i];
                }
                if (sel)
                    texthue = 33;

                if (i == this.Selected)
                    texthue = 68;

                // display the name
                this.AddImageTiled(36, 22 * (i % MaxEntriesPerPage) + 31, 102, 21, 0xBBC);
                this.AddLabelCropped(38, 22 * (i % MaxEntriesPerPage) + 31, 100, 21, texthue, namestr);

                // display the type
                this.AddImageTiled(140, 22 * (i % MaxEntriesPerPage) + 31, 133, 21, 0xBBC);
                this.AddLabelCropped(140, 22 * (i % MaxEntriesPerPage) + 31, 133, 21, texthue, typestr);

                // display the creation time
                this.AddImageTiled(275, 22 * (i % MaxEntriesPerPage) + 31, 138, 21, 0xBBC);
                this.AddLabelCropped(275, 22 * (i % MaxEntriesPerPage) + 31, 138, 21, texthue, created);

                // display the expiration
                this.AddImageTiled(415, 22 * (i % MaxEntriesPerPage) + 31, 78, 21, 0xBBC);
                this.AddLabelCropped(415, 22 * (i % MaxEntriesPerPage) + 31, 78, 21, texthue, expirestr);

                // display the attachedby
                this.AddImageTiled(495, 22 * (i % MaxEntriesPerPage) + 31, 125, 21, 0xBBC);
                this.AddLabelCropped(495, 22 * (i % MaxEntriesPerPage) + 31, 105, 21, texthue, attachedby);
                
                // display the descriptio button
                this.AddButton(600, 22 * (i % MaxEntriesPerPage) + 32, 0x5689, 0x568A, 5000 + i, GumpButtonType.Reply, 0);

                // display the selection button
                this.AddButton(620, 22 * (i % MaxEntriesPerPage) + 32, (sel ? 0xD3 : 0xD2), (sel ? 0xD2 : 0xD3), 4000 + i, GumpButtonType.Reply, 0);
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register("XmlGetAtt", AccessLevel.GameMaster, new CommandEventHandler(XmlGetAtt_OnCommand));
        }

        [Usage("XmlGetAtt")]
        [Description("Gets attachments on an object")]
        public static void XmlGetAtt_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new GetAttachTarget(e);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info == null || state == null || state.Mobile == null)
                return;
            
            int radiostate = -1;
            if (info.Switches.Length > 0)
            {
                radiostate = info.Switches[0];
            }

            // read the text entries for the search criteria

            this.Searchage = 0;

            TextRelay tr = info.GetTextEntry(400);        // displayfrom info
            try
            {
                this.DisplayFrom = int.Parse(tr.Text);
            }
            catch
            {
            }

            switch ( info.ButtonID )
            {
                case 0: // Close
                    {
                        return;
                    }

                case 156: // Delete selected items
                    {
                        this.Refresh(state);
                        int allcount = 0;
                        if (this.m_SearchList != null)
                            allcount = this.m_SearchList.Count;
                        state.Mobile.SendGump(new XmlConfirmDeleteGump(state.Mobile, this.m_TargetObject, this.m_SearchList, this.m_SelectionList, this.DisplayFrom, this.SelectAll, allcount));
                        return;
                    }

                case 201: // forward block
                    {
                        // clear the selections
                        if (this.m_SelectionList != null && !this.SelectAll)
                            Array.Clear(this.m_SelectionList, 0, this.m_SelectionList.Length);
                        if (this.m_SearchList != null && this.DisplayFrom + MaxEntries < this.m_SearchList.Count)
                        {
                            this.DisplayFrom += MaxEntries;
                            // clear any selection
                            this.Selected = -1;
                        }
                        break;
                    }
                case 202: // backward block
                    {
                        // clear the selections
                        if (this.m_SelectionList != null && !this.SelectAll)
                            Array.Clear(this.m_SelectionList, 0, this.m_SelectionList.Length);
                        this.DisplayFrom -= MaxEntries;
                        if (this.DisplayFrom < 0)
                            this.DisplayFrom = 0;
                        // clear any selection
                        this.Selected = -1;
                        break;
                    }

                case 700: // Sort
                    {
                        // clear any selection
                        this.Selected = -1;
                        // clear the selections
                        if (this.m_SelectionList != null && !this.SelectAll)
                            Array.Clear(this.m_SelectionList, 0, this.m_SelectionList.Length);
                        this.Sorttype = false;
                        this.Sortname = false;

                        // read the toggle switches that determine the sort
                        if (radiostate == 0) // sort by type
                        {
                            this.Sorttype = true;
                        }
                        if (radiostate == 1) // sort by name
                        {
                            this.Sortname = true;
                        }
                        this.SortFindList();
                        break;
                    }
                case 701: // descending sort
                    {
                        this.Descendingsort = !this.Descendingsort;
                        break;
                    }
                case 9998:  // refresh the gump
                    {
                        // clear any selection
                        this.Selected = -1;
                        break;
                    }
                default:
                    {
                        if (info.ButtonID >= 3000 && info.ButtonID < 3000 + MaxEntries)
                        {
                            this.Selected = info.ButtonID - 3000;
                            // Show the props window
                            this.Refresh(state);

                            this.DoShowProps(info.ButtonID - 3000 + this.DisplayFrom);
                            return;
                        }

                        if (info.ButtonID == 3998)
                        {
                            this.SelectAll = !this.SelectAll;

                            // dont allow individual selection with the selectall button selected
                            if (this.m_SelectionList != null)
                            {
                                for (int i = 0; i < MaxEntries; i++)
                                {
                                    if (i < this.m_SelectionList.Length)
                                    {
                                        // only toggle the selection list entries for things that actually have entries
                                        this.m_SelectionList[i] = this.SelectAll;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        if (info.ButtonID == 3999)
                        {
                            // dont allow individual selection with the selectall button selected
                            if (this.m_SelectionList != null && this.m_SearchList != null && !this.SelectAll)
                            {
                                for (int i = 0; i < MaxEntries; i++)
                                {
                                    if (i < this.m_SelectionList.Length)
                                    {
                                        // only toggle the selection list entries for things that actually have entries
                                        if ((this.m_SearchList.Count - this.DisplayFrom > i))
                                        {
                                            this.m_SelectionList[i] = !this.m_SelectionList[i];
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        if (info.ButtonID >= 4000 && info.ButtonID < 4000 + MaxEntries)
                        {
                            int i = info.ButtonID - 4000;
                            // dont allow individual selection with the selectall button selected
                            if (this.m_SelectionList != null && i >= 0 && i < this.m_SelectionList.Length && !this.SelectAll)
                            {
                                // only toggle the selection list entries for things that actually have entries
                                if (this.m_SearchList != null && (this.m_SearchList.Count - this.DisplayFrom > i))
                                {
                                    this.m_SelectionList[i] = !this.m_SelectionList[i];
                                }
                            }
                        }
                        if (info.ButtonID >= 5000 && info.ButtonID < 5000 + MaxEntries)
                        {
                            int i = info.ButtonID - 5000;
                            // dont allow individual selection with the selectall button selected
                            if (this.m_SelectionList != null && i >= 0 && i < this.m_SelectionList.Length && !this.SelectAll)
                            {
                                // only toggle the selection list entries for things that actually have entries
                                if (this.m_SearchList != null && (this.m_SearchList.Count - this.DisplayFrom > i))
                                {
                                    XmlAttachment a = this.m_SearchList[i + this.DisplayFrom] as XmlAttachment;
                                    if (a != null)
                                    {
                                        state.Mobile.SendMessage(a.OnIdentify(state.Mobile));
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
            // Create a new gump
            //m_Spawner.OnDoubleClick( state.Mobile);
            this.Refresh(state);
        }

        private bool TestAge(object o)
        {
            if (this.Searchage <= 0)
                return true;

            if (o is XmlAttachment)
            {
                XmlAttachment a = (XmlAttachment)o;

                if (this.Searchagedirection)
                {
                    // true means allow only mobs greater than the age
                    if ((DateTime.UtcNow - a.CreationTime) > TimeSpan.FromHours(this.Searchage))
                        return true;
                }
                else
                {
                    // false means allow only mobs less than the age
                    if ((DateTime.UtcNow - a.CreationTime) < TimeSpan.FromHours(this.Searchage))
                        return true;
                }
            }
            return false;
        }

        private ArrayList Search(object target, out string status_str)
        {
            status_str = null;
            ArrayList newarray = new ArrayList();
            Type targetType = null;
            // if the type is specified then get the search type
            if (this.Dosearchtype && this.Searchtype != null)
            {
                targetType = SpawnerType.GetType(this.Searchtype);
                if (targetType == null)
                {
                    status_str = "Invalid type: " + this.Searchtype;
                    return newarray;
                }
            }

            ArrayList attachments = XmlAttach.FindAttachments(target);

            // do the search through attachments
            if (attachments != null)
                foreach (XmlAttachment i in attachments)
                {
                    bool hastype = false;
                    bool hasname = false;

                    if (i == null || i.Deleted)
                        continue;

                    // check for type
                    if (this.Dosearchtype && (i.GetType().IsSubclassOf(targetType) || i.GetType().Equals(targetType)))
                    {
                        hastype = true;
                    }
                    if (this.Dosearchtype && !hastype)
                        continue;

                    // check for name
                    if (this.Dosearchname && (i.Name != null) && (this.Searchname != null) && (i.Name.ToLower().IndexOf(this.Searchname.ToLower()) >= 0))
                    {
                        hasname = true;
                    }
                    if (this.Dosearchname && !hasname)
                        continue;

                    // satisfied all conditions so add it
                    newarray.Add(i);
                }

            return newarray;
        }

        private void DoShowProps(int index)
        {
            if (this.m_From == null || this.m_From.Deleted)
                return;

            if (index < this.m_SearchList.Count)
            {
                object o = this.m_SearchList[index];
                if (o is XmlAttachment)
                {
                    XmlAttachment x = (XmlAttachment)o;
                    if (x == null || x.Deleted)
                        return;

                    this.m_From.SendGump(new PropertiesGump(this.m_From, o));
                }
            }
        }

        private void SortFindList()
        {
            if (this.m_SearchList != null && this.m_SearchList.Count > 0)
            {
                if (this.Sorttype)
                {
                    this.m_SearchList.Sort(new ListTypeSorter(this.Descendingsort));
                }
                else if (this.Sortname)
                {
                    this.m_SearchList.Sort(new ListNameSorter(this.Descendingsort));
                }
            }
        }

        private void Refresh(NetState state)
        {
            state.Mobile.SendGump(new XmlGetAttGump(this.m_From, this.m_TargetObject, false, this.Descendingsort,
                this.Dosearchtype, this.Dosearchname, this.Dosearchage,
                this.Searchtype, this.Searchname, this.Searchagedirection, this.Searchage,
                this.m_SearchList, this.Selected, this.DisplayFrom,
                this.Sorttype, this.Sortname,
                this.SelectAll, this.m_SelectionList, this.X, this.Y));
        }

        public class XmlConfirmDeleteGump : Gump
        {
            private readonly ArrayList SearchList;
            private readonly bool[] SelectedList;
            private readonly Mobile From;
            private readonly int DisplayFrom;
            private readonly bool selectAll;
            private readonly object m_target;
            public XmlConfirmDeleteGump(Mobile from, object target, ArrayList searchlist, bool[] selectedlist, int displayfrom, bool selectall, int allcount)
                : base(0, 0)
            {
                this.SearchList = searchlist;
                this.SelectedList = selectedlist;
                this.DisplayFrom = displayfrom;
                this.selectAll = selectall;
                this.m_target = target;
                this.From = from;
                this.Closable = false;
                this.Dragable = true;
                this.AddPage(0);
                this.AddBackground(10, 200, 200, 130, 5054);
                int count = 0;
                if (selectall)
                {
                    count = allcount;
                }
                else
                {
                    for (int i = 0; i < this.SelectedList.Length; i++)
                    {
                        if (this.SelectedList[i])
                            count++;
                    }
                }

                this.AddLabel(20, 225, 33, String.Format("Delete {0} attachments?", count));
                this.AddRadio(35, 255, 9721, 9724, false, 1); // accept/yes radio
                this.AddRadio(135, 255, 9721, 9724, true, 2); // decline/no radio
                this.AddHtmlLocalized(72, 255, 200, 30, 1049016, 0x7fff, false, false); // Yes
                this.AddHtmlLocalized(172, 255, 200, 30, 1049017, 0x7fff, false, false); // No
                this.AddButton(80, 289, 2130, 2129, 3, GumpButtonType.Reply, 0); // Okay button
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (info == null || state == null || state.Mobile == null)
                    return;

                int radiostate = -1;
                if (info.Switches.Length > 0)
                {
                    radiostate = info.Switches[0];
                }
                switch(info.ButtonID)
                {
                    default:
                        {
                            if (radiostate == 1 && this.SearchList != null && this.SelectedList != null)
                            { // accept
                                for (int i = 0; i < this.SearchList.Count; i++)
                                {
                                    int index = i - this.DisplayFrom;
                                    if ((index >= 0 && index < this.SelectedList.Length && this.SelectedList[index] == true) || this.selectAll)
                                    {
                                        object o = this.SearchList[i];
                                        if (o is XmlAttachment)
                                        {
                                            // some objects may not delete gracefully (null map items are particularly error prone) so trap them
                                            try
                                            {
                                                ((XmlAttachment)o).Delete();
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                                // refresh the gump
                                state.Mobile.CloseGump(typeof(XmlGetAttGump));
                                state.Mobile.SendGump(new XmlGetAttGump(state.Mobile,this.m_target,0,0));
                            }
                            break;
                        }
                }
            }
        }

        private class GetAttachTarget : Target
        {
            private readonly CommandEventArgs m_e;
            public GetAttachTarget(CommandEventArgs e)
                : base(30, false, TargetFlags.None)
            {
                this.m_e = e;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || targeted == null)
                    return;

                from.SendGump(new XmlGetAttGump(from, targeted, 0,0));
            }
        }

        private class ListTypeSorter : IComparer
        {
            private readonly bool Dsort;
            public ListTypeSorter(bool descend)
                : base()
            {
                this.Dsort = descend;
            }

            public int Compare(object x, object y)
            {
                string xstr = null;
                string ystr = null;
                string str = null;
                if (x is XmlAttachment)
                {
                    str = ((XmlAttachment)x).GetType().ToString();
                }
                if (str != null)
                {
                    string[] arglist = str.Split('.');
                    xstr = arglist[arglist.Length - 1];
                }

                str = null;
                if (y is XmlAttachment)
                {
                    str = ((XmlAttachment)y).GetType().ToString();
                }
                if (str != null)
                {
                    string[] arglist = str.Split('.');
                    ystr = arglist[arglist.Length - 1];
                }
                if (this.Dsort)
                    return String.Compare(ystr, xstr, true);
                else
                    return String.Compare(xstr, ystr, true);
            }
        }

        private class ListNameSorter : IComparer
        {
            private readonly bool Dsort;
            public ListNameSorter(bool descend)
                : base()
            {
                this.Dsort = descend;
            }

            public int Compare(object x, object y)
            {
                string xstr = null;
                string ystr = null;

                if (x is XmlAttachment)
                {
                    xstr = ((XmlAttachment)x).Name;
                }

                if (y is XmlAttachment)
                {
                    ystr = ((XmlAttachment)y).Name;
                }
                if (this.Dsort)
                    return String.Compare(ystr, xstr, true);
                else
                    return String.Compare(xstr, ystr, true);
            }
        }
    }
}