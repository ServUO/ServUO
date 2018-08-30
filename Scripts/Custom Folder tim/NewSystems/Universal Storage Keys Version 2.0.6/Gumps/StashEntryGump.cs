using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Solaris.ItemStore;

namespace Server.Gumps
{
    //stash entry gump for displaying the contents of a stash entry object
    public class StashEntryGump : Gump
    {
        PlayerMobile _Owner;
        StashEntry _StashEntry;

        //used for seeking around on the page
        int _Y = 25;

        //page size
        protected int _Height;
        protected int _Width;

        //maximum entry listing height, for multi-page calculation
        public int MaxEntryDisplayHeight { get { return 300; } }

        //line spacing between entries
        public int EntryLineSpacing { get { return 20; } }

        //page number that this gump is displaying
        protected int _Page;

        //this is determined based on the number of entries and the maximum number to display per page
        protected int _MaxPages;

        //these are used to truncate the store entry listing to fit only a subset on the page
        protected int _ListingHeight;
        protected int _ListingStart;
        protected int _ListingEnd;

        //this value is used to separate the button id's for the sort/filter controls, the withdraw/info controls
        protected int _FilterButtonIDOffset;

        //used when calculating the filter button ID offset.  This is how many buttons belong to a particular column
        protected int _FilterButtonsPerColumn = 6;

        //this value is used to separate the button id's for the general controls and the sort/filter controls
        protected int _ControlButtonIDOffset = 10;

        //a filtered stash entry list
        protected List<StashListEntry> _FilteredEntries;

        //public accessors for gump refreshing
        public Mobile Owner { get { return _Owner; } }
        public int Page { get { return _Page; } }
        //public accessor to the list entry
        public StashEntry StashEntry { get { return _StashEntry; } }

        //static refresh method, used when withdrawing/adding
        public static bool RefreshGump(Mobile player)
        {
            return RefreshGump(player,null);
        }

        public static bool RefreshGump(Mobile player,StashEntry stashentry)
        {
            //if this mobile has a stash entry gump up
            if (player.HasGump(typeof(StashEntryGump)))
            {
                StashEntryGump gump = (StashEntryGump)player.FindGump(typeof(StashEntryGump));

                //if this gump that's up is showing this list entry, or if none was specified, then refresh
                if (stashentry == null || gump.StashEntry == stashentry)
                {
                    //then, resend this gump!
                    player.SendGump(new StashEntryGump(gump));
                    return true;
                }
            }

            return false;
        }

        //gump refresh constructor
        public StashEntryGump(StashEntryGump oldgump) : this(oldgump.Owner,oldgump.StashEntry,oldgump.Page)
        {
        }

        //default first page constructor
        public StashEntryGump(Mobile owner,StashEntry stashentry) : this(owner,stashentry,0)
        {
        }

        //master constructor, with page number specified
        public StashEntryGump(Mobile owner,StashEntry stashentry,int page) : base(50,350)
        {
            if (!(owner is PlayerMobile))
            {
                return;
            }

            _Owner = (PlayerMobile)owner;
            _StashEntry = stashentry;

            _FilterButtonIDOffset = _FilterButtonsPerColumn * _StashEntry.SortData.Count;

            //clear old gumps that are up
            _Owner.CloseGump(typeof(StashEntryGump));

            //set up the page
            AddPage(0);

            _Page = page;

            ApplyFilters();

            //determine page layout, sizes, and what gets displayed where
            DeterminePageLayout();

            //add the background			            
            AddBackground(0,0,_Width,_Height,9270);
            AddImageTiled(11,10,_Width - 23,_Height - 20,3604/*2624*/);
            //AddAlphaRegion(11, 10, _Width - 22, _Height - 20);

            AddTitle();

            //if there was a problem when adding the entries
            if (!AddStashEntryListing())
            {
                //clear old gumps that are up
                _Owner.CloseGump(typeof(StashEntryGump));
                return;
            }
            if (_MaxPages > 1)
            {
                AddPageButtons();
            }

            AddControlButtons();
        }

        protected void ApplyFilters()
        {
            _FilteredEntries = new List<StashListEntry>();

            foreach (StashListEntry entry in _StashEntry.StashListEntries)
            {
                //default assume we're adding the entry
                bool addentry = true;

                //check all entries
                for (int i = 0; i < _StashEntry.SortData.Count; i++)
                {
                    string filter = _StashEntry.SortData[i].Filter;
                    //if there is a filter applied
                    if (filter != null && filter != "")
                    {
                        string text = _StashEntry.GetText(entry,i);

                        //if this doesn't have text, or there is a filter applied
                        if (text == null || text.ToLower().IndexOf(filter.ToLower()) == -1)
                        {
                            addentry = false;
                            break;
                        }
                    }
                }

                //if it's not filtered, then add to the list to be displayed
                if (addentry)
                {
                    _FilteredEntries.Add(entry);
                }
            }
        }

        //this calculates all stuff needed to display the gump properly
        protected void DeterminePageLayout()
        {
            if (_StashEntry == null || _StashEntry.StashListEntries.Count == 0)
            {
                _Height = 200;
                _Width = 400;
                return;
            }

            _Width = Math.Max(_StashEntry.SortData.Width + 100,400);

            //page size
            if (_FilteredEntries == null || _FilteredEntries.Count == 0)
            {
                _Height = 200;

                _MaxPages = 1;
                _Page = 0;
            }
            else
            {
                //minimum spacing 20, maximum entry display height
                _ListingHeight = Math.Max(20,Math.Min(MaxEntryDisplayHeight,_FilteredEntries.Count * EntryLineSpacing));

                //determine how many entries can fit on a given page
                int entriesperpage = MaxEntryDisplayHeight / EntryLineSpacing;

                //calculate max # of pages
                _MaxPages = _FilteredEntries.Count / entriesperpage + 1;

                _Page = Math.Min(_MaxPages - 1,_Page);

                _ListingStart = _Page * entriesperpage;
                _ListingEnd = (_Page + 1) * entriesperpage;

                _Height = 200 + (_MaxPages > 1 ? 30 : 0) + _ListingHeight;
            }
        }

        //this adds the title stuff for the gump
        protected void AddTitle()
        {
            if (_StashEntry == null)
            {
                return;
            }

            AddLabel(20,_Y,88,_StashEntry.Name);
            AddLabel(120,_Y,88,"Contents: " + _StashEntry.Amount.ToString() + "/" + _StashEntry.MaxEntries.ToString());

            _Y += 25;
        }

        //this adds the listing of all item stores
        protected bool AddStashEntryListing()
        {
            if (_StashEntry == null || _StashEntry.StashListEntries.Count == 0)
            {
                return true;
            }

            for (int j = 0; j < _StashEntry.SortData.Count; j++)
            {
                AddLabel(60 + _StashEntry.SortData[j].X,_Y,(_StashEntry.SortData[j].Filter == null || _StashEntry.SortData[j].Filter == "" ? 1153 : 78),_StashEntry.SortData[j].Header);
                AddSortFilterControls(60 + _StashEntry.SortData[j].X,_Y + 20,j,_StashEntry.SortData.Count,_StashEntry.SortData[j].Filter);
            }

            //column addition button
            AddLabel(_Width - 130,_Y - 25,1153,"Add Column...");
            AddButton(_Width - 50,_Y - 25,0x15A5,0x15A6,9,GumpButtonType.Reply,0);

            _Y += 40;

            //list off the items that can be displayed
            for (int i = _ListingStart; i < _ListingEnd && i < _FilteredEntries.Count; i++)
            {
                StashListEntry entry = _FilteredEntries[i];

                //add withdrawal button - put buttonid offset of 100 to allow for control/sort/filter button id's uninterrupted
                AddButton(20,_Y + 3,0x4B9,0x4B9,_ControlButtonIDOffset + _FilterButtonIDOffset + i,GumpButtonType.Reply,0);

                //add detailed info button - put buttonid offest of 100 + maxcontents
                AddButton(40,_Y + 3,0xFC1,0xFC1,_ControlButtonIDOffset + _FilterButtonIDOffset + _StashEntry.MaxEntries + i,GumpButtonType.Reply,0);

                //Add the details about this entry
                for (int j = 0; j < _StashEntry.SortData.Count; j++)
                {
                    AddLabel(60 + _StashEntry.SortData[j].X,_Y,(entry.Hue > 1 ? entry.Hue : 1153),_StashEntry.GetText(entry,j));
                }

                _Y += EntryLineSpacing;
            }

            return true;
        }

        //this looks for a property named in propertyname within the item in entry, and returns the string version of it

        protected void AddPageButtons()
        {
            //page buttons
            _Y = _Height - 90;

            if (_Page > 0)
            {
                AddButton(20,_Y,0x15E3,0x15E7,4,GumpButtonType.Reply,0);
            }
            else
            {
                AddImage(20,_Y,0x25EA);
            }
            AddLabel(40,_Y,88,"Previous Page");

            if (_Page < _MaxPages - 1)
            {
                AddButton(_Width - 40,_Y,0x15E1,0x15E5,5,GumpButtonType.Reply,0);
            }
            else
            {
                AddImage(_Width - 40,_Y,0x25E6);
            }
            AddLabel(_Width - 120,_Y,88,"Next Page");

            AddLabel(_Width / 2 - 10,_Y,88,String.Format("({0}/{1})",_Page + 1,_MaxPages));
        }

        protected void AddControlButtons()
        {
            _Y = _Height - 60;

            AddLabel(_Width / 2 + 70,_Y,1153,"Add");
            AddButton(_Width / 2 + 50,_Y + 5,0x4B9,0x4BA,1,GumpButtonType.Reply,0);

            _Y += 30;
            AddLabel(_Width / 2 + 70,_Y,1153,"Fill from backpack");
            AddButton(_Width / 2 + 50,_Y + 5,0x4B9,0x4BA,2,GumpButtonType.Reply,0);
        }

        //gump utilities

        public void AddTextField(int x,int y,int width,int height,int index,string text)
        {
            AddImageTiled(x - 2,y - 2,width + 4,height + 4,0xA2C);
            //AddAlphaRegion( x -2, y - 2, width + 4, height + 4 );
            AddTextEntry(x + 2,y + 2,width - 4,height - 4,1153,index,text);
        }

        public string GetTextField(RelayInfo info,int index)
        {
            TextRelay relay = info.GetTextEntry(index);
            return (relay == null ? null : relay.Text.Trim());
        }

        //this adds sort/filter components at the specified column location and specified column index
        public void AddSortFilterControls(int x,int y,int index,int maxindex,string filtertext)
        {
            //sort buttons
            AddButton(x,y,0x15E0,0x15E4,_ControlButtonIDOffset + _FilterButtonsPerColumn * index,GumpButtonType.Reply,0);  //Ascending
            AddButton(x + 15,y,0x15E2,0x15E6,_ControlButtonIDOffset + _FilterButtonsPerColumn * index + 1,GumpButtonType.Reply,0);  //Decending

            //column reordering buttons - shift left
            if (index > 1)
            {
                AddButton(x + 30,y,0x15E3,0x15E7,_ControlButtonIDOffset + _FilterButtonsPerColumn * index + 3,GumpButtonType.Reply,0); //shift left
            }

            //column reordering buttons - shift left
            if (index < maxindex - 1 && index != 0)
            {
                AddButton(x + 45,y,0x15E1,0x15E5,_ControlButtonIDOffset + _FilterButtonsPerColumn * index + 4,GumpButtonType.Reply,0);  //shift right
            }

            //column removal button
            if (index > 0)
            {
                AddButton(x + 65,y,0x3,0x4,_ControlButtonIDOffset + _FilterButtonsPerColumn * index + 5,GumpButtonType.Reply,0); //remove column
            }

            //seek down to the bottom of the gump
            y = _Height - 90;

            if (_MaxPages > 1)
            {
                y -= 30;
            }

            //filter text field
            AddTextField(x,y,50,20,index,filtertext);

            //update filter button
            AddButton(x + 55,y,0x15E1,0x15E5,_ControlButtonIDOffset + _FilterButtonsPerColumn * index + 2,GumpButtonType.Reply,0);
        }

        public override void OnResponse(NetState sender,RelayInfo info)
        {
            if (_StashEntry == null || !_StashEntry.CanUse(_Owner))
            {
                return;
            }

            //store flags
            int buttonid = info.ButtonID;

            //right click
            if (buttonid == 0)
            {
                return;
            }

            //first look for control buttons

            //add button
            if (buttonid == 1)
            {

                _StashEntry.AddItem(_Owner);

                //refresh the gump
                _Owner.SendGump(new StashEntryGump(this));
                return;
            }

            //fill from backpack button
            if (buttonid == 2)
            {
                _StashEntry.FillFromBackpack(_Owner);

                _Owner.SendGump(new StashEntryGump(this));
                return;
            }

            //previous page button
            if (buttonid == 4)
            {
                if (_Page > 0)
                {
                    _Owner.SendGump(new StashEntryGump(_Owner,_StashEntry,_Page - 1));
                }
                return;
            }

            //next page button
            if (buttonid == 5)
            {
                if (_Page < _MaxPages - 1)
                {
                    _Owner.SendGump(new StashEntryGump(_Owner,_StashEntry,_Page + 1));
                }
                return;
            }

            //add column button
            if (buttonid == 9)
            {
                _Owner.SendGump(new AddStashColumnGump(_Owner,_StashEntry));

                //refresh this gump
                _Owner.SendGump(new StashEntryGump(this));

                return;
            }

            //sort/filter buttons

            if (buttonid >= _ControlButtonIDOffset && buttonid < _ControlButtonIDOffset + _FilterButtonIDOffset)
            {
                int columnnum = (buttonid - (_ControlButtonIDOffset)) / _FilterButtonsPerColumn;
                int buttontype = (buttonid - (_ControlButtonIDOffset)) % _FilterButtonsPerColumn;

                //if it's a sort button
                if (buttontype < 2)
                {
                    _StashEntry.SortIndex = columnnum;
                    _StashEntry.SortOrder = (buttontype == 0 ? -1 : 1);

                    _StashEntry.Sort();
                }
                else if (buttontype == 2)       //apply filter button
                {
                    //apply filters
                    for (int i = 0; i < _StashEntry.SortData.Count; i++)
                    {
                        _StashEntry.SortData[i].Filter = GetTextField(info,i);
                    }
                }
                else if (buttontype == 3)   //column shift left button
                {
                    _StashEntry.SortData.ShiftColumnLeft(columnnum);
                }
                else if (buttontype == 4)  //column shift right button
                {
                    _StashEntry.SortData.ShiftColumnRight(columnnum);
                }
                else if (buttontype == 5 && columnnum > 0) //remove button
                {
                    _StashEntry.SortData.RemoveColumn(columnnum);
                }

                _Owner.SendGump(new StashEntryGump(this));
                return;
            }

            //any button that is left is a withdraw or item details button

            buttonid -= (_ControlButtonIDOffset + _FilterButtonIDOffset);

            if (buttonid >= 0 && buttonid < _FilteredEntries.Count)
            {
                _StashEntry.WithdrawItem(_Owner,_FilteredEntries[buttonid]);
            }
            else
            {
                buttonid -= _StashEntry.MaxEntries;

                _Owner.SendGump(new ObjectPropertyListGump(_Owner,_FilteredEntries[buttonid].Item));
            }

            _Owner.SendGump(new StashEntryGump(this));
        }
    }
}