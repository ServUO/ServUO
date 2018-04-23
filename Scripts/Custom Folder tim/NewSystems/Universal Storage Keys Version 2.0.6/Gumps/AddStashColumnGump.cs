using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Solaris.ItemStore;

namespace Server.Gumps
{
    //objectproperylist gump - displays an object property list for an item similar to how the client displays the info
    public class AddStashColumnGump : Gump
    {
        Mobile _Owner;

        const int MAXROWS = 16;
        const int COLUMNWIDTH = 160;

        //used for seeking around on the page
        int _Y = 25;
        int _X = 10;

        //page size
        protected int _Height;
        protected int _Width;

        protected StashEntry _StashEntry;

        //line spacing between entries
        public int EntryLineSpacing { get { return 20; } }

        //constructor
        public AddStashColumnGump(Mobile owner,StashEntry stashentry) : base(500,20)
        {
            _StashEntry = stashentry;

            _Owner = owner;

            //clear old gumps that are up
            _Owner.CloseGump(typeof(AddStashColumnGump));

            //set up the page
            AddPage(0);

            //determine page layout, sizes, and what gets displayed where
            DeterminePageLayout();

            //add the background
            AddBackground(0,0,_Width,_Height,9270);
            AddImageTiled(11,10,_Width - 23,_Height - 20,3604/*2624*/);
            //AddAlphaRegion(11, 10, _Width - 22, _Height - 20);

            AddTitle();

            //if there was a problem when adding the property listing
            if (!AddColumnEntries())
            {
                //clear old gumps that are up
                _Owner.CloseGump(typeof(AddStashColumnGump));
                return;
            }
        }

        //this calculates all stuff needed to display the gump properly
        protected void DeterminePageLayout()
        {
            if (_StashEntry == null)
            {
                _Width = 100;
                _Height = 100;
                return;
            }

            _Width = 20 + (_StashEntry.SortData.AvailableColumns.Count / MAXROWS + 1) * COLUMNWIDTH;
            _Height = Math.Min(_StashEntry.SortData.AvailableColumns.Count,MAXROWS) * EntryLineSpacing + 60;
        }

        //this adds the title stuff for the gump
        protected void AddTitle()
        {
            AddLabel(20,_Y,88,"Add Column:");

            _Y += 25;
        }

        //this adds the listing of all item stores
        protected bool AddColumnEntries()
        {
            int starty = _Y;

            for (int i = 0; i < _StashEntry.SortData.AvailableColumns.Count; i++)
            {
                if (i % MAXROWS == 0 && i > 0)
                {
                    _Y = starty;
                    _X += COLUMNWIDTH;
                }

                StashSortEntry entry = _StashEntry.SortData.AvailableColumns[i];

                AddButton(_X + 3,_Y,0x4B9,0x4BA,i + 1,GumpButtonType.Reply,0);
                AddLabel(_X + 20,_Y,1153,entry.Header);

                _Y += EntryLineSpacing;
            }

            return true;
        }

        protected void AddControlButtons()
        {
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

        public override void OnResponse(NetState sender,RelayInfo info)
        {
            //store flags
            int buttonid = info.ButtonID;

            //right click
            if (buttonid == 0)
            {
                return;
            }

            int index = buttonid - 1;

            _StashEntry.SortData.AddColumn(index);

            if (!StashEntryGump.RefreshGump(_Owner,_StashEntry))
            {
                _Owner.SendGump(new StashEntryGump(_Owner,_StashEntry));
            }
        }
    }
}