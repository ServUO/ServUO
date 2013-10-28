using System;
using System.Collections;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

/*
** XmlEditNPC
** Version 1.00
** updated 5/24/05
** ArteGordon
**
**
*/
namespace Server.Engines.XmlSpawner2
{
    public class XmlEditDialogGump : Gump
    {
        public const int MaxLabelLength = 200;
        private const int MaxEntries = 8;
        private const int MaxEntriesPerPage = 8;
        private readonly Mobile m_From;
        private readonly string Name;
        private readonly bool[] m_SelectionList;
        private readonly XmlDialog m_Dialog;
        private readonly ArrayList m_SearchList;
        private int Selected;
        private int DisplayFrom;
        private string SaveFilename;
        private bool SelectAll = false;
        public XmlEditDialogGump(Mobile from, bool firststart,
            XmlDialog dialog, int selected, int displayfrom, string savefilename,
            bool selectall, bool[] selectionlist, int X, int Y)
            : base(X,Y)
        {
            if (from == null || dialog == null)
                return;

            this.m_Dialog = dialog;
            this.m_From = from;

            this.m_SelectionList = selectionlist;
            if (this.m_SelectionList == null)
            {
                this.m_SelectionList = new bool[MaxEntries];
            }
            this.SaveFilename = savefilename;

            this.DisplayFrom = displayfrom;
            this.Selected = selected;

            // fill the list with the XmlDialog entries
            this.m_SearchList = dialog.SpeechEntries;
			
            // prepare the page
            int height = 500;
			
            this.AddPage(0);

            this.AddBackground(0, 0, 755, height, 5054);
            this.AddAlphaRegion(0, 0, 755, height);

            // add the separators
            this.AddImageTiled(10, 80, 735, 4, 0xBB9);
            this.AddImageTiled(10, height - 212, 735, 4, 0xBB9);
            this.AddImageTiled(10, height - 60, 735, 4, 0xBB9);

            // add the xmldialog properties
            int y = 5;
            int w = 140;
            int x = 5;
            int lw = 0;
            // the dialog name
            this.AddImageTiled(x, y, w, 21, 0x23F4);
            // get the name of the object this is attached to
			
            if (this.m_Dialog.AttachedTo is Item)
            {
                this.Name = ((Item)this.m_Dialog.AttachedTo).Name;
            }
            else if (this.m_Dialog.AttachedTo is Mobile)
            {
                this.Name = ((Mobile)this.m_Dialog.AttachedTo).Name;
            }
            if (this.Name == null && this.m_Dialog.AttachedTo != null)
            {
                this.Name = this.m_Dialog.AttachedTo.GetType().Name;
            }
            this.AddLabelCropped(x, y, w, 21, 0, this.Name);

            x += w + lw + 20;
            w = 40;
            lw = 90;
            // add the proximity range
            this.AddLabel(x, y, 0x384, "ProximityRange");
            this.AddImageTiled(x + lw, y, w, 21, 0xBBC);
            this.AddTextEntry(x + lw, y, w, 21, 0, 140, this.m_Dialog.ProximityRange.ToString());

            x += w + lw + 20;
            w = 100;
            lw = 60;
            // reset time
            this.AddLabel(x, y, 0x384, "ResetTime");
            this.AddImageTiled(x + lw, y, w, 21, 0xBBC);
            this.AddTextEntry(x + lw, y, w, 21, 0, 141, this.m_Dialog.ResetTime.ToString());

            x += w + lw + 20;
            w = 40;
            lw = 65;
            // speech pace
            this.AddLabel(x, y, 0x384, "SpeechPace");
            this.AddImageTiled(x + lw, y, w, 21, 0xBBC);
            this.AddTextEntry(x + lw, y, w, 21, 0, 142, this.m_Dialog.SpeechPace.ToString());

            x += w + lw + 20;
            w = 30;
            lw = 55;
            // allow ghost triggering
            this.AddLabel(x, y, 0x384, "GhostTrig");
            this.AddCheck(x + lw, y, 0xD2, 0xD3, this.m_Dialog.AllowGhostTrig, 260);

            // add the triggeroncarried
            y += 27;
            w = 600;
            x = 10;
            lw = 100;
            this.AddLabel(10, y, 0x384, "TrigOnCarried");
            this.AddImageTiled(x + lw, y, w, 21, 0xBBC);
            this.AddTextEntry(x + lw, y, w, 21, 0, 150, this.TruncateLabel(this.m_Dialog.TriggerOnCarried));
            this.AddButton(720, y, 0xFAB, 0xFAD, 5005, GumpButtonType.Reply, 0);

            // add the notriggeroncarried
            y += 22;
            w = 600;
            x = 10;
            lw = 100;
            this.AddLabel(x, y, 0x384, "NoTrigOnCarried");
            this.AddImageTiled(x + lw, y, w, 21, 0xBBC);
            this.AddTextEntry(x + lw, y, w, 21, 0, 151, this.TruncateLabel(this.m_Dialog.NoTriggerOnCarried));
            this.AddButton(720, y, 0xFAB, 0xFAD, 5006, GumpButtonType.Reply, 0);

            y = 88;
            // column labels
            this.AddLabel(10, y, 0x384, "Edit");
            this.AddLabel(45, y, 0x384, "#");
            this.AddLabel(95, y, 0x384, "ID");
            this.AddLabel(145, y, 0x384, "Depends");
            this.AddLabel(195, y, 0x384, "Keywords");
            this.AddLabel(295, y, 0x384, "Text");
            this.AddLabel(540, y, 0x384, "Action");
            this.AddLabel(602, y, 0x384, "Condition");
            this.AddLabel(664, y, 0x384, "Gump");

            // display the select-all-displayed toggle
            this.AddButton(730, y, 0xD2, 0xD3, 3999, GumpButtonType.Reply, 0);

            y -= 10;
            for (int i = 0; i < MaxEntries; i++)
            {
                int index = i + this.DisplayFrom;
                if (this.m_SearchList == null || index >= this.m_SearchList.Count)
                    break;
                int page = (int)(i / MaxEntriesPerPage);
                if (i % MaxEntriesPerPage == 0)
                {
                    this.AddPage(page + 1);
                    // add highlighted page button
                    //AddImageTiled( 235+page*25, 448, 25, 25, 0xBBC );
                    //AddImage( 238+page*25, 450, 0x8B1+page );
                }

                // background for search results area
                this.AddImageTiled(235, y + 22 * (i % MaxEntriesPerPage) + 30, 386, 23, 0x52);
                this.AddImageTiled(236, y + 22 * (i % MaxEntriesPerPage) + 31, 384, 21, 0xBBC);

                XmlDialog.SpeechEntry s = (XmlDialog.SpeechEntry)this.m_SearchList[index];

                if (s == null)
                    continue;

                int texthue = 0;

                bool sel = false;

                if (this.m_SelectionList != null && i < this.m_SelectionList.Length)
                {
                    sel = this.m_SelectionList[i];
                }

                // entries with the selection box checked are highlighted in red
                if (sel)
                    texthue = 33;

                // the selected entry is highlighted in green
                if (i == this.Selected)
                    texthue = 68;

                x = 10;
                w = 35;
                // add the Edit button for each entry
                this.AddButton(10, y + 22 * (i % MaxEntriesPerPage) + 30, 0xFAE, 0xFAF, 1000 + i, GumpButtonType.Reply, 0);

                x += w;
                w = 50;
                // display the entry number
                this.AddImageTiled(x, y + 22 * (i % MaxEntriesPerPage) + 31, w, 21, 0xBBC);
                this.AddLabel(x, y + 22 * (i % MaxEntriesPerPage) + 31, texthue, s.EntryNumber.ToString());
				
                x += w;
                w = 50;
                // display the entry ID
                this.AddImageTiled(x, y + 22 * (i % MaxEntriesPerPage) + 31, w, 21, 0x23F4);
                this.AddLabel(x, y + 22 * (i % MaxEntriesPerPage) + 31, texthue, s.ID.ToString());

                x += w;
                w = 50;
                // display the entry dependson
                this.AddImageTiled(x, y + 22 * (i % MaxEntriesPerPage) + 31, w, 21, 0xBBC);
                this.AddLabel(x, y + 22 * (i % MaxEntriesPerPage) + 31, texthue, s.DependsOn);

                x += w;
                w = 100;				
                // display the entry keywords
                this.AddImageTiled(x, y + 22 * (i % MaxEntriesPerPage) + 31, w, 21, 0x23F4);
                this.AddLabelCropped(x, y + 22 * (i % MaxEntriesPerPage) + 31, w - 5, 21, texthue, this.TruncateLabel(s.Keywords));

                x += w;
                w = 245;
                // display the entry text
                this.AddImageTiled(x, y + 22 * (i % MaxEntriesPerPage) + 31, w, 21, 0xBBC);
                this.AddLabelCropped(x, y + 22 * (i % MaxEntriesPerPage) + 31, w - 5, 21, texthue, this.TruncateLabel(s.Text));

                x += w;
                w = 62;
                // display the action text
                this.AddImageTiled(x, y + 22 * (i % MaxEntriesPerPage) + 31, w, 21, 0x23F4);
                this.AddLabelCropped(x, y + 22 * (i % MaxEntriesPerPage) + 31, w - 5, 21, texthue, this.TruncateLabel(s.Action));

                x += w;
                w = 62;
                // display the condition text
                this.AddImageTiled(x, y + 22 * (i % MaxEntriesPerPage) + 31, w, 21, 0xBBC);
                this.AddLabelCropped(x, y + 22 * (i % MaxEntriesPerPage) + 31, w - 5, 21, texthue, this.TruncateLabel(s.Condition));

                x += w;
                w = 62;
                // display the gump text
                this.AddImageTiled(x, y + 22 * (i % MaxEntriesPerPage) + 31, w, 21, 0x23F4);
                this.AddLabelCropped(x, y + 22 * (i % MaxEntriesPerPage) + 31, w - 5, 21, texthue, this.TruncateLabel(s.Gump));

                // display the selection button
                this.AddButton(730, y + 22 * (i % MaxEntriesPerPage) + 32, (sel ? 0xD3 : 0xD2), (sel ? 0xD2 : 0xD3), 4000 + i, GumpButtonType.Reply, 0);
            }

            // display the selected entry information for editing
            XmlDialog.SpeechEntry sentry = null;
            if (this.Selected >= 0 && this.Selected + this.DisplayFrom >= 0 && this.Selected + this.DisplayFrom < this.m_SearchList.Count)
            {
                sentry = (XmlDialog.SpeechEntry)this.m_SearchList[this.Selected + this.DisplayFrom];
            }

            if (sentry != null)
            {
                y = height - 200;

                // add the entry parameters
                lw = 15;
                w = 40;
                x = 10;
                int spacing = 11;

                // entry number
                this.AddLabel(x, y, 0x384, "#");
                this.AddImageTiled(x + lw, y, w, 21, 0xBBC);
                this.AddTextEntry(x + lw, y, w, 21, 0, 200, sentry.EntryNumber.ToString());

                x += w + lw + spacing;
                w = 40;
                lw = 17;
                // ID number
                this.AddLabel(x, y, 0x384, "ID");
                this.AddImageTiled(x + lw, y, w, 21, 0xBBC);
                this.AddTextEntry(x + lw, y, w, 21, 0, 201, sentry.ID.ToString());

                x += w + lw + spacing;
                w = 40;
                lw = 65;
                // depends on 
                this.AddLabel(x, y, 0x384, "DependsOn");
                this.AddImageTiled(x + lw, y, w, 21, 0xBBC);
                this.AddTextEntry(x + lw, y, w, 21, 0, 202, sentry.DependsOn);

                x += w + lw + spacing;
                w = 35;
                lw = 57;
                // prepause 
                this.AddLabel(x, y, 0x384, "PrePause");
                this.AddImageTiled(x + lw, y, w, 21, 0xBBC);
                this.AddTextEntry(x + lw, y, w, 21, 0, 203, sentry.PrePause.ToString());

                x += w + lw + spacing;
                w = 35;
                lw = 37;
                // pause
                this.AddLabel(x, y, 0x384, "Pause");
                this.AddImageTiled(x + lw, y, w, 21, 0xBBC);
                this.AddTextEntry(x + lw, y, w, 21, 0, 204, sentry.Pause.ToString());

                x += w + lw + spacing;
                w = 37;
                lw = 26;
                // speech hue
                this.AddLabel(x, y, 0x384, "Hue");
                this.AddImageTiled(x + lw, y, w, 21, 0xBBC);
                this.AddTextEntry(x + lw, y, w, 21, 0, 205, sentry.SpeechHue.ToString());

                x += w + lw + spacing;
                w = 20;
                lw = 52;
                // lock conversation
                this.AddLabel(x, y, 0x384, "IgnoreCar");
                this.AddCheck(x + lw, y, 0xD2, 0xD3, sentry.IgnoreCarried, 252);

                x += w + lw + spacing;
                w = 20;
                lw = 42;
                // lock conversation
                this.AddLabel(x, y, 0x384, "LockOn");
                this.AddCheck(x + lw, y, 0xD2, 0xD3, sentry.LockConversation, 250);

                x += w + lw + spacing;
                w = 20;
                lw = 54;
                // npctrigger
                this.AddLabel(x, y, 0x384, "AllowNPC");
                this.AddCheck(x + lw, y, 0xD2, 0xD3, sentry.AllowNPCTrigger, 251);

                w = 650;
                x = 70;

                y += 27;
                // add the keyword entry
                this.AddLabel(10, y, 0x384, "Keywords");
                this.AddImageTiled(x, y, w, 21, 0xBBC);
                this.AddTextEntry(x + 1, y, w, 21, 0, 101, sentry.Keywords);
                this.AddButton(720, y, 0xFAB, 0xFAD, 5001, GumpButtonType.Reply, 0);

                y += 22;
                // add the text entry
                this.AddLabel(10, y, 0x384, "Text");
                this.AddImageTiled(x, y, w, 21, 0xBBC);
                this.AddTextEntry(x + 1, y, w, 21, 0, 100, sentry.Text);
                this.AddButton(720, y, 0xFAB, 0xFAD, 5000, GumpButtonType.Reply, 0);

                y += 22;
                // add the condition string entry
                this.AddLabel(10, y, 0x384, "Condition");
                this.AddImageTiled(x, y, w, 21, 0xBBC);
                this.AddTextEntry(x + 1, y, w, 21, 0, 102, sentry.Condition);
                this.AddButton(720, y, 0xFAB, 0xFAD, 5002, GumpButtonType.Reply, 0);

                y += 22;
                // add the action string entry
                this.AddLabel(10, y, 0x384, "Action");
                this.AddImageTiled(x, y, w, 21, 0xBBC);
                this.AddTextEntry(x + 1, y, w, 21, 0, 103, sentry.Action);
                this.AddButton(720, y, 0xFAB, 0xFAD, 5003, GumpButtonType.Reply, 0);

                y += 22;
                // add the gump string entry
                this.AddLabel(10, y, 0x384, "Gump");
                this.AddImageTiled(x, y, w, 21, 0xBBC);
                this.AddTextEntry(x + 1, y, w, 21, 0, 104, sentry.Gump);
                this.AddButton(720, y, 0xFAB, 0xFAD, 5004, GumpButtonType.Reply, 0);
            }

            y = height - 50;

            this.AddLabel(10, y, 0x384, "Config:");
            this.AddImageTiled(50, y, 120, 19, 0x23F4);
            this.AddLabel(50, y, 0, this.m_Dialog.ConfigFile);

            if (from.AccessLevel >= XmlSpawner.DiskAccessLevel)
            {
                // add the save entry
                this.AddButton(185, y, 0xFA8, 0xFAA, 159, GumpButtonType.Reply, 0);
                this.AddLabel(218, y, 0x384, "Save to file:");
                this.AddImageTiled(300, y, 180, 19, 0xBBC);
                this.AddTextEntry(300, y, 180, 19, 0, 300, this.SaveFilename);
            }

            // display the item list
            if (this.m_SearchList != null)
            {
                this.AddLabel(495, y, 68, String.Format("{0} Entries", this.m_SearchList.Count));
                int last = this.DisplayFrom + MaxEntries < this.m_SearchList.Count ? this.DisplayFrom + MaxEntries : this.m_SearchList.Count;
                if (last > 0) 
                    this.AddLabel(595, y, 68, String.Format("Displaying {0}-{1}", this.DisplayFrom, last - 1));
            }

            y = height - 25;

            // add run status display
            if (this.m_Dialog.Running)
            {
                this.AddButton(10, y - 5, 0x2A4E, 0x2A3A, 100, GumpButtonType.Reply, 0);
                this.AddLabel(43, y, 0x384, "On");
            }
            else 
            {
                this.AddButton(10, y - 5, 0x2A62, 0x2A3A, 100, GumpButtonType.Reply, 0);
                this.AddLabel(43, y, 0x384, "Off");
            }

            // add the Refresh/Sort button
            this.AddButton(80, y, 0xFAB, 0xFAD, 700, GumpButtonType.Reply, 0);
            this.AddLabel(113, y, 0x384, "Refresh");

            // add the Add button
            this.AddButton(185, y, 0xFAB, 0xFAD, 155, GumpButtonType.Reply, 0);
            this.AddLabel(218, y, 0x384, "Add");

            // add the Delete button
            this.AddButton(255, y, 0xFB1, 0xFB3, 156, GumpButtonType.Reply, 0);
            this.AddLabel(283, y, 0x384, "Delete");

            // add the page buttons
            for (int i = 0; i < (int)(MaxEntries / MaxEntriesPerPage); i++)
            {
                this.AddButton(513 + i * 25, y, 0x8B1 + i, 0x8B1 + i, 0, GumpButtonType.Page, 1 + i);
            }

            // add the advance pageblock buttons
            this.AddButton(510 + 25 * (int)(MaxEntries / MaxEntriesPerPage), y, 0x15E1, 0x15E5, 201, GumpButtonType.Reply, 0); // block forward
            this.AddButton(490, y, 0x15E3, 0x15E7, 202, GumpButtonType.Reply, 0); // block backward

            // add the displayfrom entry
            this.AddLabel(555, y, 0x384, "Display");
            this.AddImageTiled(595, y, 60, 21, 0xBBC);
            this.AddTextEntry(595, y, 60, 21, 0, 400, this.DisplayFrom.ToString());
            this.AddButton(655, y, 0xFAB, 0xFAD, 9998, GumpButtonType.Reply, 0);
            //AddLabel( 610, y, 0x384, "Select All" );
            // display the select-all toggle
            //AddButton( 670, y, (SelectAll? 0xD3:0xD2), (SelectAll? 0xD2:0xD3), 3998, GumpButtonType.Reply, 0 );
        }

        public static void Initialize()
        {
            CommandSystem.Register("XmlEdit", AccessLevel.GameMaster, new CommandEventHandler(XmlEditDialog_OnCommand));
        }

        [Usage("XmlEdit")]
        [Description("Edits XmlDialog entries on an object")]
        public static void XmlEditDialog_OnCommand(CommandEventArgs e)
        {
            if (e == null || e.Mobile == null)
                return;

            // target an object with the xmldialog attachment
            e.Mobile.Target = new EditDialogTarget();
        }

        public static void ProcessXmlEditBookEntry(Mobile from, object[] args, string text)
        {
            if (from == null || args == null || args.Length < 6)
                return;

            XmlDialog dialog = (XmlDialog)args[0];
            XmlDialog.SpeechEntry entry = (XmlDialog.SpeechEntry)args[1];
            int textid = (int)args[2];
            int selected = (int)args[3];
            int displayfrom = (int)args[4];
            string savefile = (string)args[5];

            // place the book text into the entry by type
            switch(textid)
            {
                case 0: // text
                    if (entry != null)
                        entry.Text = text;
                    break;
                case 1: // keywords
                    if (entry != null)
                        entry.Keywords = text;
                    break;
                case 2: // condition
                    if (entry != null)
                        entry.Condition = text;
                    break;
                case 3: // action
                    if (entry != null)
                        entry.Action = text;
                    break;
                case 4: // gump
                    if (entry != null)
                        entry.Gump = text;
                    break;
                case 5: // trigoncarried
                    if (dialog != null)
                        dialog.TriggerOnCarried = text;
                    break;
                case 6: // notrigoncarried
                    if (dialog != null)
                        dialog.NoTriggerOnCarried = text;
                    break;
            }

            from.CloseGump(typeof(XmlEditDialogGump));

            //from.SendGump( new XmlEditDialogGump(from, false, m_Dialog, selected, displayfrom, savefilename, false, null, X, Y) );
            from.SendGump(new XmlEditDialogGump(from, true, dialog, selected, displayfrom, savefile, false, null, 0, 0));
        }

        public string TruncateLabel(string s)
        {
            if (s == null || s.Length < MaxLabelLength)
                return s;

            return s.Substring(0, MaxLabelLength);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info == null || state == null || state.Mobile == null || this.m_Dialog == null)
                return;
            
            int radiostate = -1;
            if (info.Switches.Length > 0)
            {
                radiostate = info.Switches[0];
            }

            TextRelay tr = info.GetTextEntry(400);        // displayfrom info
            try
            {
                this.DisplayFrom = int.Parse(tr.Text);
            }
            catch
            {
            }

            tr = info.GetTextEntry(300);        // savefilename info
            if (tr != null)
                this.SaveFilename = tr.Text;

            if (this.m_Dialog != null)
            {
                tr = info.GetTextEntry(140);        // proximity range
                if (tr != null)
                {
                    try
                    {
                        this.m_Dialog.ProximityRange = int.Parse(tr.Text);
                    }
                    catch
                    {
                    }
                }
                tr = info.GetTextEntry(141);        // reset time
                if (tr != null)
                {
                    try
                    {
                        this.m_Dialog.ResetTime = TimeSpan.Parse(tr.Text);
                    }
                    catch
                    {
                    }
                }
                tr = info.GetTextEntry(142);        // speech pace
                if (tr != null)
                {
                    try
                    {
                        this.m_Dialog.SpeechPace = int.Parse(tr.Text);
                    }
                    catch
                    {
                    }
                }

                tr = info.GetTextEntry(150);        // trig on carried
                if (tr != null && (this.m_Dialog.TriggerOnCarried == null || this.m_Dialog.TriggerOnCarried.Length < 230))
                {
                    if (tr.Text != null && tr.Text.Trim().Length > 0)
                    {
                        this.m_Dialog.TriggerOnCarried = tr.Text;
                    }
                    else
                    {
                        this.m_Dialog.TriggerOnCarried = null;
                    }
                }

                tr = info.GetTextEntry(151);        // notrig on carried
                if (tr != null && (this.m_Dialog.NoTriggerOnCarried == null || this.m_Dialog.NoTriggerOnCarried.Length < 230))
                {
                    if (tr.Text != null && tr.Text.Trim().Length > 0)
                    {
                        this.m_Dialog.NoTriggerOnCarried = tr.Text;
                    }
                    else
                    {
                        this.m_Dialog.NoTriggerOnCarried = null;
                    }
                }

                this.m_Dialog.AllowGhostTrig = info.IsSwitched(260);	// allow ghost triggering
            }

            if (this.m_SearchList != null && this.Selected >= 0 && this.Selected + this.DisplayFrom >= 0 && this.Selected + this.DisplayFrom < this.m_SearchList.Count)
            {
                // entry information
                XmlDialog.SpeechEntry entry = (XmlDialog.SpeechEntry)this.m_SearchList[this.Selected + this.DisplayFrom]; 

                tr = info.GetTextEntry(200);        // entry number
                if (tr != null)
                {
                    try
                    {
                        entry.EntryNumber = int.Parse(tr.Text);
                    }
                    catch
                    {
                    }
                }

                tr = info.GetTextEntry(201);        // entry id
                if (tr != null)
                {
                    try
                    {
                        entry.ID = int.Parse(tr.Text);
                    }
                    catch
                    {
                    }
                }

                tr = info.GetTextEntry(202);        // depends on
                if (tr != null)
                {
                    try
                    {
                        entry.DependsOn = tr.Text;
                    }
                    catch
                    {
                    }
                }

                tr = info.GetTextEntry(203);        // prepause
                if (tr != null)
                {
                    try
                    {
                        entry.PrePause = int.Parse(tr.Text);
                    }
                    catch
                    {
                    }
                }

                tr = info.GetTextEntry(204);        // pause
                if (tr != null)
                {
                    try
                    {
                        entry.Pause = int.Parse(tr.Text);
                    }
                    catch
                    {
                    }
                }

                tr = info.GetTextEntry(205);        // hue
                if (tr != null)
                {
                    try
                    {
                        entry.SpeechHue = int.Parse(tr.Text);
                    }
                    catch
                    {
                    }
                }

                tr = info.GetTextEntry(101);        // keywords
                if (tr != null && (entry.Keywords == null || entry.Keywords.Length < 230))
                {
                    if (tr.Text != null && tr.Text.Trim().Length > 0)
                    {
                        entry.Keywords = tr.Text;
                    }
                    else
                    {
                        entry.Keywords = null;
                    }
                }

                tr = info.GetTextEntry(100);        // text
                if (tr != null && (entry.Text == null || entry.Text.Length < 230))
                {
                    if (tr.Text != null && tr.Text.Trim().Length > 0)
                    {
                        entry.Text = tr.Text;
                    }
                    else
                    {
                        entry.Text = null;
                    }
                }

                tr = info.GetTextEntry(102);        // condition
                if (tr != null && (entry.Condition == null || entry.Condition.Length < 230))
                {
                    if (tr.Text != null && tr.Text.Trim().Length > 0)
                    {
                        entry.Condition = tr.Text;
                    }
                    else
                    {
                        entry.Condition = null;
                    }
                }

                tr = info.GetTextEntry(103);        // action
                if (tr != null && (entry.Action == null || entry.Action.Length < 230))
                {
                    if (tr.Text != null && tr.Text.Trim().Length > 0)
                    {
                        entry.Action = tr.Text;
                    }
                    else
                    {
                        entry.Action = null;
                    }
                }

                tr = info.GetTextEntry(104);        // gump
                if (tr != null && (entry.Gump == null || entry.Gump.Length < 230))
                {
                    if (tr.Text != null && tr.Text.Trim().Length > 0)
                    {
                        entry.Gump = tr.Text;
                    }
                    else
                    {
                        entry.Gump = null;
                    }
                }

                entry.LockConversation = info.IsSwitched(250);	// lock conversation
                entry.AllowNPCTrigger = info.IsSwitched(251);	// allow npc
                entry.IgnoreCarried = info.IsSwitched(252);	// ignorecarried
            }

            switch ( info.ButtonID )
            {
                case 0: // Close
                    {
                        this.m_Dialog.DeleteTextEntryBook();

                        return;
                    }
                case 100: // toggle running status
                    {
                        this.m_Dialog.Running = !this.m_Dialog.Running;

                        break;
                    }
                case 155: // add new entry
                    {
                        if (this.m_SearchList != null)
                        {
                            // find the last entry
                            int lastentry = 0;
                            foreach (XmlDialog.SpeechEntry e in this.m_SearchList)
                            {
                                if (e.EntryNumber > lastentry)
                                    lastentry = e.EntryNumber;
                            }
                            lastentry += 10;
                            XmlDialog.SpeechEntry se = new XmlDialog.SpeechEntry();
                            se.EntryNumber = lastentry;
                            se.ID = lastentry;
                            this.m_SearchList.Add(se);
                            this.Selected = this.m_SearchList.Count - 1;
                        }
                        break;
                    }

                case 156: // Delete selected entries
                    {
                        XmlEditDialogGump g = this.Refresh(state);
                        int allcount = 0;
                        if (this.m_SearchList != null)
                            allcount = this.m_SearchList.Count;
                        state.Mobile.SendGump(new XmlConfirmDeleteGump(state.Mobile, g, this.m_SearchList, this.m_SelectionList, this.DisplayFrom, this.SelectAll, allcount));
                        return;
                    }

                case 159: // save to a .npc file
                    { 
                        // Create a new gump
                        this.Refresh(state);
                        // try to save
                        this.m_Dialog.DoSaveNPC(state.Mobile, this.SaveFilename, true);

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

                        this.SortFindList();
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
                        if (info.ButtonID >= 1000 && info.ButtonID < 1000 + MaxEntries)
                        {
                            // flag the entry selected
                            this.Selected = info.ButtonID - 1000;
                        }
                        else if (info.ButtonID == 3998)
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
                        else if (info.ButtonID == 3999)
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
                        else if (info.ButtonID >= 4000 && info.ButtonID < 4000 + MaxEntries)
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
                        else if (info.ButtonID >= 5000 && info.ButtonID < 5100)
                        {
                            // text entry book buttons
                            int textid = info.ButtonID - 5000;

                            // entry information
                            XmlDialog.SpeechEntry entry = null;

                            if (this.m_SearchList != null && this.Selected >= 0 && this.Selected + this.DisplayFrom >= 0 && this.Selected + this.DisplayFrom < this.m_SearchList.Count)
                            {
                                entry = (XmlDialog.SpeechEntry)this.m_SearchList[this.Selected + this.DisplayFrom]; 
                            }

                            string text = String.Empty;
                            string title = String.Empty;
                            switch(textid)
                            {
                                case 0: // text
                                    if (entry != null)
                                        text = entry.Text;
                                    title = "Text";
                                    break;
                                case 1: // keywords
                                    if (entry != null)
                                        text = entry.Keywords;
                                    title = "Keywords";
                                    break;
                                case 2: // condition
                                    if (entry != null)
                                        text = entry.Condition;
                                    title = "Condition";
                                    break;
                                case 3: // action
                                    if (entry != null)
                                        text = entry.Action;
                                    title = "Action";
                                    break;
                                case 4: // gump
                                    if (entry != null)
                                        text = entry.Gump;
                                    title = "Gump";
                                    break;
                                case 5: // trigoncarried
                                    text = this.m_Dialog.TriggerOnCarried;
                                    title = "TrigOnCarried";
                                    break;
                                case 6: // notrigoncarried
                                    text = this.m_Dialog.NoTriggerOnCarried;
                                    title = "NoTrigOnCarried";
                                    break;
                            }

                            object[] args = new object[6];

                            args[0] = this.m_Dialog;
                            args[1] = entry;
                            args[2] = textid;
                            args[3] = this.Selected;
                            args[4] = this.DisplayFrom;
                            args[5] = this.SaveFilename;

                            XmlTextEntryBook book = new XmlTextEntryBook(0, String.Empty, this.m_Dialog.Name, 20, true, new XmlTextEntryBookCallback(ProcessXmlEditBookEntry), args);
						
                            if (this.m_Dialog.m_TextEntryBook == null)
                            {
                                this.m_Dialog.m_TextEntryBook = new ArrayList();
                            }
                            this.m_Dialog.m_TextEntryBook.Add(book);

                            book.Title = title;
                            book.Author = this.Name;

                            // fill the contents of the book with the current text entry data
                            book.FillTextEntryBook(text);

                            // put the book at the location of the player so that it can be opened, but drop it below visible range
                            book.Visible = false;
                            book.Movable = false;
                            book.MoveToWorld(new Point3D(state.Mobile.Location.X,state.Mobile.Location.Y,state.Mobile.Location.Z - 100), state.Mobile.Map);

                            // Create a new gump
                            this.Refresh(state);

                            // and open it
                            book.OnDoubleClick(state.Mobile);

                            return;
                        }
                        break;
                    }
            }
            // Create a new gump
            this.Refresh(state);
        }

        private void SortFindList()
        {
            if (this.m_SearchList != null && this.m_SearchList.Count > 0)
            {
                this.m_SearchList.Sort(new ListSorter(false));
            }
        }

        private void SaveList(Mobile from, string filename)
        {
            if (this.m_SearchList == null || this.m_SelectionList == null)
                return;
		  
            string dirname;
            if (System.IO.Directory.Exists(XmlDialog.DefsDir) && filename != null && !filename.StartsWith("/") && !filename.StartsWith("\\"))
            {
                // put it in the defaults directory if it exists
                dirname = String.Format("{0}/{1}", XmlDialog.DefsDir, filename);
            }
            else 
            {
                // otherwise just put it in the main installation dir
                dirname = filename;
            }
            // save it to the file
        }

        private XmlEditDialogGump Refresh(NetState state)
        {
            XmlEditDialogGump g = new XmlEditDialogGump(this.m_From, false, this.m_Dialog, this.Selected,
                this.DisplayFrom, this.SaveFilename, this.SelectAll, this.m_SelectionList, this.X, this.Y);
            state.Mobile.SendGump(g);
            return g;
        }

        public class XmlConfirmAddGump : Gump
        {
            private readonly Mobile m_From;
            private readonly object m_Targeted;
            public XmlConfirmAddGump(Mobile from, object targeted)
                : base(0, 0)
            {
                if (from == null || targeted == null)
                    return;

                this.m_Targeted = targeted;
                this.m_From = from;

                this.Closable = false;
                this.Dragable = true;
                this.AddPage(0);
                this.AddBackground(10, 200, 200, 130, 5054);

                this.AddLabel(20, 210, 68, String.Format("Add an XmlDialog to target?"));

                string name = null;
                if (targeted is Item)
                {
                    name = ((Item)targeted).Name;
                }
                else if (targeted is Mobile)
                {
                    name = ((Mobile)targeted).Name;
                }

                if (name == null)
                {
                    name = targeted.GetType().Name;
                }
                this.AddLabel(20, 230, 0, String.Format("{0}", name));

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
                            if (radiostate == 1)
                            { // accept
                                // add the attachment
                                XmlDialog xd = new XmlDialog();
                                XmlAttach.AttachTo(state.Mobile, this.m_Targeted, xd);

                                // open the editing gump
                                state.Mobile.SendGump(new XmlEditDialogGump(state.Mobile, true, xd, -1, 0, null, false, null, 0, 0));
                            }
                            break;
                        }
                }
            }
        }

        public class XmlConfirmDeleteGump : Gump
        {
            private readonly ArrayList SearchList;
            private readonly bool[] SelectedList;
            private readonly Mobile From;
            private readonly int DisplayFrom;
            private readonly bool selectAll;
            readonly XmlEditDialogGump m_Gump;
            public XmlConfirmDeleteGump(Mobile from, XmlEditDialogGump gump, ArrayList searchlist, bool[] selectedlist, int displayfrom, bool selectall, int allcount)
                : base(0, 0)
            {
                this.SearchList = searchlist;
                this.SelectedList = selectedlist;
                this.DisplayFrom = displayfrom;
                this.selectAll = selectall;
                this.m_Gump = gump;
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

                this.AddLabel(20, 225, 33, String.Format("Delete {0} entries?", count));
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
                                ArrayList dlist = new ArrayList();
                                for (int i = 0; i < this.SearchList.Count; i++)
                                {
                                    int index = i - this.DisplayFrom;
                                    if ((index >= 0 && index < this.SelectedList.Length && this.SelectedList[index] == true) || this.selectAll)
                                    {
                                        object o = this.SearchList[i];
                                        // delete the entry;
                                        dlist.Add(o);
                                    }
                                }

                                foreach (object o in dlist)
                                {
                                    this.SearchList.Remove(o);
                                }

                                // clear the selections
                                Array.Clear(this.SelectedList, 0, this.SelectedList.Length);

                                if (this.m_Gump != null)
                                {
                                    state.Mobile.CloseGump(typeof(XmlEditDialogGump));
                                    this.m_Gump.Refresh(state);
                                }
                            }
                            break;
                        }
                }
            }
        }

        private class EditDialogTarget : Target
        {
            public EditDialogTarget()
                : base(30, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null)
                    return;

                // does it have an xmldialog attachment?
                XmlDialog xd = XmlAttach.FindAttachment(targeted, typeof(XmlDialog)) as XmlDialog;

                if (xd == null)
                {
                    from.SendMessage("Target has no XmlDialog attachment");

                    // TODO: ask whether they would like to add one
                    from.SendGump(new XmlConfirmAddGump(from, targeted));

                    return;
                }

                from.SendGump(new XmlEditDialogGump(from, true, xd, -1, 0, null, false, null, 0, 0));
            }
        }

        private class ListSorter : IComparer
        {
            private readonly bool Dsort;
            public ListSorter(bool descend)
                : base()
            {
                this.Dsort = descend;
            }

            public int Compare(object x, object y)
            {
                int xn = 0;
                int yn = 0;

                xn = ((XmlDialog.SpeechEntry)x).EntryNumber;

                yn = ((XmlDialog.SpeechEntry)y).EntryNumber;

                if (this.Dsort)
                    return yn - xn;
                else
                    return xn - yn;
            }
        }
    }
}