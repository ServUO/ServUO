using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Solaris.CliLocHandler;

namespace Server.Gumps
{
    //objectproperylist gump - displays an object property list for an item similar to how the client displays the info
    public class ObjectPropertyListGump : Gump
    {
        //size to display artwork
        const int ARTWORK_WIDTH = 50;
        const int ARTWORK_HEIGHT = 50;

        Mobile _Owner;

        object _Object;

        List<string> _PropsList;

        //used for seeking around on the page
        int _Y = 25;
        int _X = 0;

        //page size
        protected int _Height;
        protected int _Width;

        //line spacing between entries
        public int EntryLineSpacing { get { return 20; } }

        //public accessors for gump refreshing
        public Mobile Owner { get { return _Owner; } }
        public object Object { get { return _Object; } }

        //static refresh method, used when withdrawing/adding
        public static bool RefreshGump(Mobile player)
        {
            return RefreshGump(player,null);
        }

        public static bool RefreshGump(Mobile player,object o)
        {
            if (player.HasGump(typeof(ObjectPropertyListGump)))
            {
                ObjectPropertyListGump gump = (ObjectPropertyListGump)player.FindGump(typeof(ObjectPropertyListGump));

                //if this gump that's up is showing this list entry, or if none was specified, then refresh
                if (o == null || gump.Object == o)
                {
                    //then, resend this gump!
                    player.SendGump(new ObjectPropertyListGump(gump));
                    return true;
                }
            }

            return false;
        }

        //gump refresh constructor
        public ObjectPropertyListGump(ObjectPropertyListGump oldgump) : this(oldgump.Owner,oldgump.Object)
        {
        }

        //default first page constructor
        public ObjectPropertyListGump(Mobile owner,Object o) : base(10,10)
        {
            _Owner = owner;
            _Object = o;

            //clear old gumps that are up
            _Owner.CloseGump(typeof(ObjectPropertyListGump));

            //set up the page
            AddPage(0);

            //determine page layout, sizes, and what gets displayed where
            DeterminePageLayout();

            //add the background			            
            AddBackground(0,0,_Width,_Height,9270);
            AddImageTiled(11,10,_Width - 23,_Height - 20,2624);
            AddAlphaRegion(11,10,_Width - 22,_Height - 20);

            AddTitle();

            AddArtwork();

            //if there was a problem when adding the property listing
            if (!AddPropsListing())
            {
                //clear old gumps that are up
                _Owner.CloseGump(typeof(ObjectPropertyListGump));
                return;
            }

            AddControlButtons();
        }

        //this calculates all stuff needed to display the gump properly
        protected void DeterminePageLayout()
        {
            _PropsList = CliLoc.GetPropertiesList(_Object);

            if (_PropsList == null)
            {
                _Width = 100;
                _Height = 100;
                return;
            }

            //determine the required width of the gump based on the object property list contents
            _Width = Math.Max(ARTWORK_WIDTH + 40,CliLoc.PixelsPerCharacter * CliLoc.GetMaxLength(_PropsList) + 50);

            _Height = ARTWORK_HEIGHT + _PropsList.Count * EntryLineSpacing + 60;
        }

        //this adds the title stuff for the gump
        protected void AddTitle()
        {
        }

        protected void AddArtwork()
        {
            int x = (_Width - ARTWORK_HEIGHT) / 2;
            int y = 10;

            AddBackground(x,y,ARTWORK_WIDTH,ARTWORK_HEIGHT,9270);
            AddImageTiled(x + 1,y + 1,ARTWORK_WIDTH - 2,ARTWORK_HEIGHT - 2,2624);

            if (_Object is Item)
            {
                AddItem(x,y,((Item)_Object).ItemID,((Item)_Object).Hue);
            }

            _Y += ARTWORK_HEIGHT;
        }

        //this adds the listing of all item stores
        protected bool AddPropsListing()
        {
            foreach (string property in _PropsList)
            {
                AddHtml(_X,_Y,_Width,20,"<BASEFONT COLOR=#FFFFFF><CENTER>" + property,false,false);
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
            AddAlphaRegion(x - 2,y - 2,width + 4,height + 4);
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

            _Owner.SendGump(new ObjectPropertyListGump(this));
        }
    }
}