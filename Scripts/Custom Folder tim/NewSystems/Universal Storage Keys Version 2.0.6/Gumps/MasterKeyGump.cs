using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Solaris.ItemStore;

namespace Server.Gumps
{
    //Master key gump - shows a list of all keys loaded into a master key
    public class MasterItemStoreKeyGump : Gump
    {
        PlayerMobile _Owner;
        MasterItemStoreKey _Key;

        public int Height
        {
            get
            {
                if (_Key == null)
                {
                    return 100;
                }
                return 130 + _Key.Stores.Count * 20;            //page size
            }
        }

        public static int Width = 350;

        //used for seeking around on the page
        int _Y = 15;

        //used for formatting the gump columns
        static int _ArtworkColumn = 5;
        static int _AccessColumn = _ArtworkColumn + 50;
        static int _KeyNameColumn = _AccessColumn + 50;

        static int _WithdrawColumn = Width - 80;

        public MasterItemStoreKeyGump(Mobile owner,MasterItemStoreKey key) : base(450,50)
        {
            if (!(owner is PlayerMobile))
            {
                return;
            }

            _Owner = (PlayerMobile)owner;
            _Key = key;

            //clear old gumps that are up
            _Owner.CloseGump(typeof(MasterItemStoreKeyGump));

            //set up the page
            AddPage(0);

            AddBackground(0,0,Width,Height,9270);
            AddImageTiled(11,10,Width - 22,Height - 20,3604/*2624*/);
            //AddAlphaRegion(11, 10, Width - 22, Height - 20);

            AddTitle();

            if (!AddKeyListing())
            {
                //clear old gumps that are up
                _Owner.CloseGump(typeof(MasterItemStoreKeyGump));
                return;
            }

            AddControlButtons();
        }

        //this adds the title stuff for the gump
        protected void AddTitle()
        {
            AddLabel(Width / 2 - 50,_Y,88,"Master Keys");

            _Y += 25;
        }

        //this adds the listing of all item stores
        protected bool AddKeyListing()
        {
            if (_Key == null || _Key.Stores.Count == 0)
            {
                return true;
            }

            AddLabel(_AccessColumn,_Y,1153,"Access");
            AddLabel(_WithdrawColumn,_Y,1153,"Withdraw");
            AddLabel(_KeyNameColumn,_Y,1153,"Type");

            _Y += 25;

            int counter = 0;

            foreach (Type type in _Key.KeyTypes)
            {
                Item item = null;
                String errormessage = "";

                //draw the artwork
                try
                {
                    Object o = Activator.CreateInstance(type);

                    item = (Item)o;
                }
                catch (Exception e)
                {
                    errormessage = e.Message;
                }

                //if there was a problem obtaining the graphic
                if (item == null)
                {
                    _Owner.SendMessage("Error: " + errormessage);
                    return false;
                }

                //add a graphic of the item 
                AddItem(_ArtworkColumn,_Y,item.ItemID,item.Hue);

                //add the name
                AddLabel(_KeyNameColumn,_Y,0x486,item.Name);

                //Access button
                AddButton(_AccessColumn,_Y + 3,0x4B9,0x4BA,counter + 1,GumpButtonType.Reply,0);

                //withdraw button
                AddButton(_WithdrawColumn,_Y + 3,0x4B9,0x4BA,_Key.MaxEntries + counter + 1,GumpButtonType.Reply,0);

                _Y += 20;

                //clean up the instanced item so it doesn't populate the shard with bogus items
                item.Delete();

                //increment the counter
                counter++;
            }

            return true;
        }

        protected void AddControlButtons()
        {
            _Y = Height - 60;

            AddLabel(Width / 2 + 70,_Y,1153,"Add Keys");
            AddButton(Width / 2 + 50,_Y + 5,0x4B9,0x4BA,_Key.MaxEntries * 2 + 1,GumpButtonType.Reply,0);

            AddLabel(10,_Y,1153,"Storage: ");
            AddLabel(100,_Y,1152,String.Format("({0}/{1})",_Key.KeyTypes.Count,_Key.MaxEntries.ToString()));

            _Y += 30;
            AddLabel(Width / 2 - 20,_Y,1153,"Fill entries from backpack");
            AddButton(Width / 2 - 40,_Y + 5,0x4B9,0x4BA,_Key.MaxEntries * 2 + 2,GumpButtonType.Reply,0);
        }

        public override void OnResponse(NetState sender,RelayInfo info)
        {
            if (!_Key.CanUse(_Owner))
            {
                return;
            }

            //flags
            int buttonid = info.ButtonID;

            //right click
            if (buttonid == 0)
            {
                return;
            }

            //add button
            if (buttonid == _Key.MaxEntries * 2 + 1)
            {
                _Key.Add(_Owner);

                _Owner.SendGump(new MasterItemStoreKeyGump(_Owner,_Key));
                return;
            }

            //fill from backpack button
            if (buttonid == _Key.MaxEntries * 2 + 2)
            {
                _Key.FillEntriesFromBackpack(_Owner);
                _Owner.SendGump(new MasterItemStoreKeyGump(_Owner,_Key));
                return;
            }

            //flag if a withdraw was pressed
            if (buttonid > _Key.MaxEntries)
            {
                buttonid -= _Key.MaxEntries;

                if (buttonid < 1 || buttonid > _Key.Stores.Count)
                {
                    _Owner.SendMessage("Invalid button press");
                    return;
                }
                Item key = _Key.RemoveKey(buttonid - 1);

                if (key != null)
                {
                    _Owner.AddToBackpack(key);
                }
                _Owner.CloseGump(typeof(ItemStoreGump));
                _Owner.CloseGump(typeof(ListEntryGump));

                _Owner.SendGump(new MasterItemStoreKeyGump(_Owner,_Key));
                return;
            }

            if (buttonid < 1 || buttonid > _Key.Stores.Count)
            {
                _Owner.SendMessage("Invalid option.  Please try again.");
                return;
            }

            //view key contents
            _Key.Stores[buttonid - 1].DoubleClick(_Owner);

            _Owner.SendGump(new MasterItemStoreKeyGump(_Owner,_Key));
        }
    }
}