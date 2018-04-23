using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Targeting;
using Solaris.Extras;

namespace Solaris.ItemStore
{
    //List entry parent class - special kind of entry that contains a list of special item entry blocks.
    public class ListEntry : StoreEntry
    {
        //default itemid: show a south-facing metal box
        const int DEFAULT_ITEMID = 0x9A8;

        //0xFA6 is the right arrow swoop thing
        public override int ButtonID { get { return 0xFA6; } }
        public override int ButtonX { get { return 0; } }
        public override int ButtonY { get { return -3; } }

        //maximum amount to store in a ListEntry object
        public virtual int MaxAmount { get { return 100000; } }

        //the list storing the item entry blocks
        protected List<ItemListEntry> _ItemListEntries;

        public int ListItemID;
        public int ListItemHue;

        //this keeps track of what type of entry list is used for the specified list entry
        protected Type _ItemListEntryType;

        //this is used for sort/filter info
        protected string[] _FilterText;

        public string[] FilterText
        {
            get
            {
                if (_FilterText == null)
                {
                    //limit to 10 columns
                    _FilterText = new string[10];
                }
                return _FilterText;
            }
        }

        public static int debugcount = 0;

        public List<ItemListEntry> ItemListEntries
        {
            get
            {
                if (_ItemListEntries == null)
                {
                    _ItemListEntries = new List<ItemListEntry>();
                }
                return _ItemListEntries;
            }
        }

        public Type ItemListEntryType { get { return _ItemListEntryType; } }

        //the amount property is used to display the amount of items stored within the list
        public override int Amount
        {
            get { return ItemListEntries.Count; }
        }

        //NOTE: the type property here is used differently than the rest of the item list entries.  It is used to define the
        //ItemListEntry object type, which ultimately stores all data in this device

        //default constructor: set up item type, item list entry type, a name, and default artwork, size/offset of artwork
        public ListEntry(Type type,Type itemlistentrytype,string name) : this(type,itemlistentrytype,name,DEFAULT_ITEMID,0)
        {
        }

        //default contstructor: set up type, entry type, name, artwork itemid, artwork hue, and default size/offset of artwork		
        public ListEntry(Type type,Type itemlistentrytype,string name,int itemid,int hue) : this(type,itemlistentrytype,name,itemid,hue,40,0,0)
        {
        }

        //default constructor: set up item type, item list entry type, a name, size/offset of artwork, default artwork, and no item list entry
        public ListEntry(Type type,Type itemlistentrytype,string name,int height,int x,int y) : this(type,itemlistentrytype,name,DEFAULT_ITEMID,0,height,x,y,null)
        {
        }

        //default constructor: set up item type, item list entry type, a name, artwork, size/offset of artwork, and no item list entry
        public ListEntry(Type type,Type itemlistentrytype,string name,int itemid,int hue,int height,int x,int y) : this(type,itemlistentrytype,name,itemid,hue,height,x,y,null)
        {
        }

        //master constructor: set up item type, item list entry type, a name, size/offset of artwork, and item list entry
        public ListEntry(Type type,Type itemlistentrytype,string name,int itemid,int hue,int height,int x,int y,List<ItemListEntry> itemlistentries) : base(type,null,name,0,height,x,y)
        {
            ListItemID = itemid;
            ListItemHue = hue;
            _ItemListEntryType = itemlistentrytype;
            _ItemListEntries = itemlistentries;
        }

        //clone constructor 
        //TODO: clone ItemListEntries?
        public ListEntry(ListEntry entry) : this(entry.Type,entry.ItemListEntryType,entry.Name,entry.ListItemID,entry.ListItemHue,entry.Height,entry.X,entry.Y,entry.ItemListEntries)
        {
        }

        //generic reader deser constructor
        public ListEntry(GenericReader reader) : base(reader)
        {
        }

        public override Item GetModel()
        {
            Item modelitem = new Item(ListItemID);
            modelitem.Hue = ListItemHue;

            return modelitem;
        }

        //adding directly to the item list entry 
        public override bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            if (!Match(item,false))
            {
                return false;
            }

            //try to create the item list entry

            try
            {
                ItemListEntry entry = (ItemListEntry)Activator.CreateInstance(_ItemListEntryType,new object[] { item });

                //check to make sure everything is ok with the item being added.  This checks things like if a treasure map has
                //already been completed, for example.
                if (entry.AllGood(item))
                {
                    ItemListEntries.Add(entry);

                    item.Delete();

                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        //this starts the the addition of an item to the item list entries, by sending a targeting cursor if the player can access this
        public void AddItem(Mobile from)
        {
            if (!CanUse(from))
            {
                return;
            }

            from.Target = new AddItemTarget(this);
        }

        //this performs the actual addition checks
        public void AddItem(Mobile from,object targeted)
        {
            if (!CanUse(from) || !(targeted is Item))
            {
                return;
            }

            if (!Add((Item)targeted))
            {
                return;
            }

            //resend the targeting cursor and refresh the gump 
            if (from != null)
            {
                AddItem(from);

                //resend the store gump after it's all done
                if (!ItemStoreGump.RefreshGump(from))
                {
                    //send a new store gump if there's no existing one up
                    from.SendGump(new ItemStoreGump(from,Store));
                }

                //resend the item list entry gump after it's all done
                if (!ListEntryGump.RefreshGump(from))
                {
                    //send a new list entry gump if there's no existing one up
                    from.SendGump(new ListEntryGump(from,this));
                }
            }
        }

        //the target class used for adding items to a list entry
        class AddItemTarget : Target
        {
            ListEntry _ListEntry;       //reference the list entry

            //constructor
            public AddItemTarget(ListEntry listentry) : base(1,false,TargetFlags.None)
            {
                _ListEntry = listentry;
            }

            protected override void OnTarget(Mobile from,object targeted)
            {
                _ListEntry.AddItem(from,targeted);
            }
        }

        //this performs an automatic sweep of all items in the user's backpack, and fills the the list entry with the items if it is possible
        public void FillFromBackpack(Mobile from)
        {
            if (from == null || from.Backpack == null)
            {
                return;
            }
            //generate a list of all items in the backpack
            List<Item> packitems = ItemStore.RecurseFindItemsInPack(from.Backpack);

            //go through backpack list, and try to add the items
            foreach (Item item in packitems)
            {
                AddItem(null,item);
            }

            //resend the store gump after it's all done
            if (!ItemStoreGump.RefreshGump(from))
            {
                //send a new store gump if there's no existing one up
                from.SendGump(new ItemStoreGump(from,Store));
            }

            //resend the item list entry gump after it's all done
            if (!ListEntryGump.RefreshGump(from))
            {
                //send a new list entry gump if there's no existing one up
                from.SendGump(new ListEntryGump(from,this));
            }
        }

        //special treatment: this is used to override the default behaviour on the main ItemStoreGump, and instead bring up the ListEntryGump
        public override Item Withdraw(ref int amount,bool forcequantity)
        {
            return null;
        }

        public virtual void WithdrawItem(Mobile from,ItemListEntry entry)
        {
            WithdrawItem(from,ItemListEntries.IndexOf(entry));
        }

        //this is the actual withdrawal action.  it withdraws the item from the listentry and puts it in the backpack of the specified person
        public virtual void WithdrawItem(Mobile from,int index)
        {
            if (index < 0 || index >= ItemListEntries.Count)
            {
                from.SendMessage("Invalid withrawal selection");
                return;
            }

            Item item = WithdrawItem(index);

            if (item != null)
            {
                from.AddToBackpack(item);

                //resend the store gump after it's all done
                if (!ItemStoreGump.RefreshGump(from))
                {
                    //send a new store gump if there's no existing one up
                    from.SendGump(new ItemStoreGump(from,Store));
                }

                //resend the item list entry gump after it's all done
                if (!ListEntryGump.RefreshGump(from))
                {
                    //send a new list entry gump if there's no existing one up
                    from.SendGump(new ListEntryGump(from,this));
                }
            }
        }

        //this performs the withdrawal of the item from the specified index
        public Item WithdrawItem(int index)
        {
            try
            {
                Item withdrawitem = ItemListEntries[index].GenerateItem();

                //remove this from the list
                ItemListEntries.RemoveAt(index);

                return withdrawitem;
            }
            catch
            {
            }

            return null;
        }

        //this handles the check to see if the requested item matches this entry.  This is used when an item store is checking thru
        //all its entries to find one that matches
        public override bool Match(Item item,bool checksubtypes)
        {
            //too full
            //TODO: support errormessage system for player feedback
            if (Amount >= MaxAmount)
            {
                return false;
            }

            //incorrect item type
            if (item.GetType() != _Type && !item.GetType().IsSubclassOf(_Type))
            {
                return false;
            }

            return true;
        }

        //this match method is useful for when an abstract collection of parameters are passed.  It is only defined in 
        //child classes
        public override bool Match(int amount,object[] parameters)
        {
            //TODO: see if this is useful, and develop it
            return false;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new ListEntry(this);
        }

        public void CloneItemListEntries(ListEntry sourceentry)
        {
            foreach (ItemListEntry entry in sourceentry.ItemListEntries)
            {
                ItemListEntries.Add(entry.Clone());
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            //version
            writer.Write(0);

            writer.Write(_ItemListEntryType.Name);

            writer.Write(ItemListEntries.Count);

            foreach (ItemListEntry entry in ItemListEntries)
            {
                writer.Write(entry.GetType().Name);

                entry.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            //handle base StoreEntry deserialization first
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                default:
                    {
                        _ItemListEntryType = ScriptCompiler.FindTypeByName(reader.ReadString());

                        int count = reader.ReadInt();

                        for (int i = 0; i < count; i++)
                        {
                            string typestring = reader.ReadString();

                            Type type = ScriptCompiler.FindTypeByName(typestring);

                            ItemListEntry entry = (ItemListEntry)Activator.CreateInstance(type,new object[] { reader });

                            ItemListEntries.Add(entry);
                        }
                        break;
                    }
            }
        }//deserialize
    }//class ListEntry
}