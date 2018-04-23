using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Targeting;
using Solaris.Extras;

namespace Solaris.ItemStore
{
    //Stash entry parent class - special kind of entry that contains a list of references to actual items, which are stashed in the Internal Map for storage
    public class StashEntry : StoreEntry
    {
        //the default maximum items that can be stored in a stash list
        const int DEFAULT_MAX_ENTRIES = 500;

        public override int ButtonID { get { return 0xFA5; } }
        public override int ButtonX { get { return 0; } }
        public override int ButtonY { get { return -3; } }

        //the list storing the item stash entries within this entry
        protected List<StashListEntry> _StashListEntries;

        //the maximum number of items that can be stored in this stash entry
        protected int _MaxEntries;

        //this contains all display and filter information to be used in the gump
        protected StashSortData _SortData;

        public StashSortData SortData
        {
            get
            {
                if (_SortData == null)
                {
                    _SortData = new StashSortData();
                }
                return _SortData;
            }
        }

        //TODO: find out what this does!!
        public static int debugcount = 0;

        //this is used for the sorting, determine which column in Columns to sort by
        public int SortIndex;

        //set to either -1 or 1
        public int SortOrder;

        //public accessor with auto-generation
        public List<StashListEntry> StashListEntries
        {
            get
            {
                if (_StashListEntries == null)
                {
                    _StashListEntries = new List<StashListEntry>();
                }
                return _StashListEntries;
            }
        }

        public int MaxEntries { get { return _MaxEntries; } }

        //the amount property is used to display the amount of items stored within the list
        public override int Amount
        {
            get { return StashListEntries.Count; }
        }

        //default constructor: set up item type, a name, and default size/offset of artwork
        public StashEntry(Type type,string name) : this(type,name,DEFAULT_MAX_ENTRIES)
        {
        }

        //default constructor:  default sort data
        public StashEntry(Type type,string name,int maxentries) : this(type,name,maxentries,new StashSortData())
        {
        }

        public StashEntry(Type type,string name,int maxentries,StashSortData sortdata) : this(type,name,maxentries,sortdata,40,0,0)
        {
        }

        //default constructor: set up item type, a name, size/offset of artwork, and no existing stash list entry
        public StashEntry(Type type,string name,int maxentries,StashSortData sortdata,int height,int x,int y) : this(type,name,maxentries,sortdata,height,x,y,null)
        {
        }

        //master constructor
        public StashEntry(Type type,string name,int maxentries,StashSortData sortdata,int height,int x,int y,List<StashListEntry> stashlistentries) : base(type,null,name,0,height,x,y)
        {
            _MaxEntries = maxentries;
            _StashListEntries = stashlistentries;
            _SortData = sortdata;
        }

        //clone constructor 
        public StashEntry(StashEntry entry) : this(entry.Type,entry.Name,entry.MaxEntries,entry.SortData,entry.Height,entry.X,entry.Y,entry.StashListEntries)
        {
        }

        //generic reader deser constructor
        public StashEntry(GenericReader reader) : base(reader)
        {
        }

        public void Sort()
        {
            StashListEntries.Sort();
        }

        //these are used to get the text label for a particular column of a particular entry
        public string GetText(StashListEntry entry,int column)
        {
            return GetText(entry,SortData[column].PropertyNames,SortData[column].AltPropertyNames);
        }

        protected string GetText(StashListEntry entry,string[] propertynames,string[] altpropertynames)
        {
            if (entry.Item == null)
            {
                return "NULL!!";
            }

            //special case: best name 
            if (propertynames[0] == "best name")
            {
                return entry.Name;
            }

            //if it's a regular property, find its value through reflection
            string name = GetPropertyName(entry.Item,propertynames);

            //if there is an alternate property, then get that add in that info too
            if (altpropertynames != null)
            {
                name += ":" + GetPropertyName(entry.Item,altpropertynames);
            }

            return name;
        }

        public string GetPropertyName(Item item,string[] propertypath)
        {
            Type itemtype = item.GetType();

            PropertyInfo pi = itemtype.GetProperty(propertypath[0]);

            try
            {
                Object o = (Object)pi.GetValue(item,null);

                //this seeks down into the property to see sub-properties
                for (int i = 1; i < propertypath.Length; i++)
                {
                    itemtype = o.GetType();
                    pi = itemtype.GetProperty(propertypath[i]);
                    o = (Object)pi.GetValue(o,null);
                }

                return o.ToString();
            }
            catch
            {
                return "-none-";
            }
        }

        //this displays the object in the main key gump
        public override Item GetModel()
        {
            //show a metal chest facing south
            return new Item(0x9AB);
        }

        //method to add an item to the stash
        public override bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            //if this item is not compatable with this entry, then abort
            if (!Match(item,false))
            {
                return false;
            }

            //try to create the stash list entry

            try
            {
                //when a stash list entry is created successfully, the item is parked in the internal map
                StashListEntry entry = new StashListEntry(item);
                entry.StashEntry = this;

                //check to make sure everything is ok with the item being added.
                if (entry.AllGood(item))
                {
                    StashListEntries.Add(entry);
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        //this starts the the addition of an item to the stash list entries, by sending a targeting cursor
        public void AddItem(Mobile from)
        {
            if (!CanUse(from))
            {
                //if they can't use this, then abort
                return;
            }

            from.Target = new AddItemTarget(this);
        }

        //this performs the actual addition checks after the user has targeted the desired object
        public void AddItem(Mobile from,object targeted)
        {
            if (!CanUse(from) || !(targeted is Item))
            {
                return;
            }

            //try to add this item
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
                if (!StashEntryGump.RefreshGump(from))
                {
                    //send a new list entry gump if there's no existing one up
                    from.SendGump(new StashEntryGump(from,this));
                }
            }
        }

        //the target class used for adding items to a list entry
        class AddItemTarget : Target
        {
            StashEntry _StashEntry;     //reference the list entry

            //constructor
            public AddItemTarget(StashEntry stashentry) : base(1,false,TargetFlags.None)
            {
                _StashEntry = stashentry;
            }

            protected override void OnTarget(Mobile from,object targeted)
            {
                _StashEntry.AddItem(from,targeted);
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
            if (!StashEntryGump.RefreshGump(from))
            {
                //send a new list entry gump if there's no existing one up
                from.SendGump(new StashEntryGump(from,this));
            }
        }

        //special treatment: this is used to override the default behaviour on the main ItemStoreGump, and instead bring up the StashEntryGump
        public override Item Withdraw(ref int amount,bool forcequantity)
        {
            return null;
        }

        public virtual void WithdrawItem(Mobile from,StashListEntry entry)
        {
            WithdrawItem(from,StashListEntries.IndexOf(entry));
        }

        //this is the actual withdrawal action.  it withdraws the item from the stashentry and puts it in the backpack of the mobile
        public virtual void WithdrawItem(Mobile from,int index)
        {
            if (index < 0 || index >= StashListEntries.Count)
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
                if (!StashEntryGump.RefreshGump(from))
                {
                    //send a new list entry gump if there's no existing one up
                    from.SendGump(new StashEntryGump(from,this));
                }
            }
        }

        //this performs the withdrawal of the item from the specified index
        public Item WithdrawItem(int index)
        {
            try
            {
                Item withdrawitem = StashListEntries[index].WithdrawItem();

                //remove this from the list
                StashListEntries.RemoveAt(index);

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
            if (Amount >= MaxEntries)
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

        //this is used to drive the cloning process - inherited classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new StashEntry(this);
        }

        //this performs a cloning of the stash list entries within the specified stash entry
        public void CloneStashListEntries(StashEntry sourceentry)
        {
            foreach (StashListEntry entry in sourceentry.StashListEntries)
            {
                StashListEntry cloneentry = entry.Clone();
                cloneentry.StashEntry = this;               //need to explicitly pass the reference to this new list into clone
                StashListEntries.Add(cloneentry);
            }
        }

        public override void Dispose()
        {
            foreach (StashListEntry entry in _StashListEntries)
            {
                entry.Dispose();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            //version
            writer.Write(0);

            writer.Write(StashListEntries.Count);

            foreach (StashListEntry entry in StashListEntries)
            {
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
                        int count = reader.ReadInt();

                        for (int i = 0; i < count; i++)
                        {
                            StashListEntry entry = new StashListEntry(reader);

                            if (entry.Item != null)
                            {
                                StashListEntries.Add(entry);
                                entry.StashEntry = this;
                            }
                        }
                        break;
                    }
            }
        }//deserialize
    }//class StashEntry
}