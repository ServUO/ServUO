using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Targeting;

namespace Solaris.ItemStore
{
    //the ItemStore object, contains a sortable list of items
    public class ItemStore : IDisposable
    {
        //the maximum any item store entry can hold
        public static int MaxAmount { get { return 999999999; } }

        //this is the list of actively used item store entries.  these are fully functional and accessible to the owner
        protected List<StoreEntry> _StoreEntries;

        //this is used to keep track of how big the gump will need to be to display all item entries
        int _EntryHeight = 0;

        //this provides a calculation of the total height of all entries.  useful when rendering the gump
        public int EntryHeight
        {
            get
            {
                if (_EntryHeight == 0)
                {
                    RefreshEntryHeight();
                }
                return _EntryHeight;
            }
        }

        public List<StoreEntry> StoreEntries
        {
            get
            {
                if (_StoreEntries == null)
                {
                    _StoreEntries = new List<StoreEntry>();
                }

                return _StoreEntries;
            }
        }

        //this is a list of items that are to be expelled due to a change in the structure of the device implementing
        //this item store.  An example is if a item key script is modified to no longer contain a particular item.
        //upon world load, any item entries that are no longer specified by the containing owner device will be moved into
        //this list, ready to be extracted by the player using the device
        protected List<StoreEntry> _ExpelStoreEntries;

        public List<StoreEntry> ExpelStoreEntries
        {
            get
            {
                if (_ExpelStoreEntries == null)
                {
                    _ExpelStoreEntries = new List<StoreEntry>();
                }
                return _ExpelStoreEntries;
            }
            //set accessor used for cloning
            set
            {
                _ExpelStoreEntries = value;
            }
        }

        public int Count
        {
            get { return StoreEntries.Count; }
        }

        //this is used to display a name for the item store on the gump
        public string Label;

        //this flag denotes if the item store listing can be dynamically assigned by the owner
        public bool Dynamic;

        //this flag enables the ability to withdraw into commodity deeds
        public bool OfferDeeds;

        //the amount to withdraw when the player selects the item on the gump
        public int WithdrawAmount;

        //thie minimum amount to withdraw
        public int MinWithdrawAmount;

        //TODO: define this better
        //this is a reference to whatever is implementing or containing this item store

        public object Owner;

        //stores the loottype of the object implementing this.  This is important when keys are added, then removed, from the master keys
        public LootType LootType;

        //stores the insured status of the object implementing this.
        public bool Insured;

        //number of columns to display on the gump.  Default is 2.  This is set by whatever object implements it, and it persists here to allow storage in master keys to persist
        public int DisplayColumns = 2;

        public bool LockWithdrawalAmount;

        //constructors
        public ItemStore() : this((List<StoreEntry>)null)
        {
        }

        public ItemStore(List<StoreEntry> StoreEntries)
        {
            _StoreEntries = StoreEntries;

            foreach (StoreEntry entry in _StoreEntries)
            {
                entry.Store = this;
            }

            WithdrawAmount = 100;

            RefreshEntryHeight();
        }

        //deserialization constructor
        public ItemStore(GenericReader reader)
        {
            Deserialize(reader);
        }

        public void RefreshEntryHeight()
        {
            _EntryHeight = StoreEntry.GetTotalHeight(StoreEntries);
        }

        //check if a mobile can use it
        public bool CanUse(Mobile from)
        {
            //for automatic filling using fill from pack
            if (from == null)
            {
                return true;
            }

            //check what object contains this store, and return an error if it's not set up right
            if (Owner == null || !(Owner is IItemStoreObject))
            {
                from.SendMessage("Item store improperly initialized.  Call a GM!");
                return false;
            }

            //store use is containing object-specific
            return ((IItemStoreObject)Owner).CanUse(from);
        }

        //withdraw action, default withdraw amount
        public void WithdrawItem(Mobile from,int entryindex,bool makedeed)
        {
            WithdrawItem(from,WithdrawAmount,entryindex,makedeed);
        }

        //withdraw action, specified withdraw amount
        public void WithdrawItem(Mobile from,int amount,int entryindex,bool makedeed)
        {
            WithdrawItem(from,amount,entryindex,makedeed,true);
        }

        //withdraw action by type - this one will be used from the craft system
        public void WithdrawItem(Mobile from,int amount,Type type)
        {
            int entryindex = StoreEntry.IndexOfType(_StoreEntries,type);

            if (entryindex > 0)
            {
                WithdrawItem(from,amount,entryindex,false,false);
            }
        }

        //the master withdraw method
        public void WithdrawItem(Mobile from,int amount,int entryindex,bool makedeed,bool resend)
        {
            StoreEntry entry = null;
            try
            {
                entry = _StoreEntries[entryindex];
            }
            catch
            {
                from.SendMessage("invalid selection pressed.");
                return;
            }

            //make sure making deeds is being offered with this bitmask
            makedeed &= OfferDeeds;

            //special handling - for a ListEntry object, hitting withdraw is not intended to remove the object, but rather
            //bring up a gump showing the list contained within the ListEntry
            if (entry is ListEntry)
            {
                from.SendGump(new ListEntryGump(from,(ListEntry)entry));
                return;
            }

            //special handling - for a StashEntry object, hitting withdraw will bring up a gump showing the stash contained
            //within the StashEntry
            if (entry is StashEntry)
            {
                from.SendGump(new StashEntryGump(from,(StashEntry)entry));
                return;
            }

            if (entry.Amount > 0)
            {
                //check if they can afford a commodity deed
                if (makedeed && resend && !Banker.Withdraw(from,5))
                {
                    from.SendMessage("you must have at least 5 gold in your bank to extract the resource into a commodity deed");
                    return;
                }

                if (BaseStoreKey.EmptyContents && entry.RequiresRecipient)
                {
                    Item recipient = entry.FindRecipient(from);

                    if (recipient != null)
                    {
                        if (entry.WithdrawTo(ref WithdrawAmount,recipient) == null)
                        {
                            from.SendMessage("Cannot withdraw that for some reason!");
                            return;
                        }
                    }
                    else
                    {
                        from.SendMessage("You do not have a container to store that.");
                        return;
                    }
                }
                else
                {
                    Item item = entry.Withdraw(ref WithdrawAmount,makedeed);

                    if (item == null)
                    {
                        from.SendMessage("there was an error creating that item.  Please page a GM!");
                        return;
                    }

                    from.AddToBackpack(item);
                }
            }
            else
            {
                from.SendMessage("you don't have any of that");
            }
        }

        //code to begin adding a item, accessed by the gump
        public void AddItem(Mobile from)
        {
            if (!CanUse(from))
            {
                return;
            }

            from.Target = new AddItemTarget(this);
        }

        //code to finish adding an item - accessed by the add item target
        public void AddItem(Mobile from,object targeted)
        {
            if (from != null && !CanUse(from))
            {
                from.SendMessage("You no longer can use that");
                return;
            }

            //keep track of the item
            Item item = null;

            //keep track of the deed if it's a commodity deeds
            Item deed = null;

            int entryindex;

            if (!(targeted is Item))
            {
                if (from != null)
                {
                    from.SendMessage("this only works on items.");
                }
                return;
            }

            item = (Item)targeted;

            if (from != null && !item.IsChildOf(from.Backpack))
            {
                BankBox box = from.FindBankNoCreate();

                if (box == null || !item.IsChildOf(box))
                {
                    from.SendMessage("you can only add items from your backpack or bank box");
                    return;
                }
            }

            //Handle commodity deed insertion
            if (item is CommodityDeed)
            {
                if (((CommodityDeed)item).Commodity == null)
                {
                    if (from != null)
                    {
                        from.SendMessage("there is nothing to add in that commodity deed.");
                    }
                    return;
                }
                //store the deed reference
                deed = item;

                //reference the commodity within the deed
                item = ((CommodityDeed)item).Commodity;
            }

            //this uses the overloadable comparison for the appropriate store entries, so that custom store entries
            //can provide custom comparisons with items through their Match() methods
            entryindex = StoreEntry.IndexOfItem(_StoreEntries,item,true);

            if (entryindex == -1)
            {
                if (from != null)
                {
                    from.SendMessage("that cannot be stored in this.");
                }
                return;
            }

            //reference the item store entry
            StoreEntry entry = _StoreEntries[entryindex];

            if (!entry.Add(item))
            {
                if (from != null)
                {
                    from.SendMessage("that quantity cannot fit in this.");
                }
                return;
            }

            //don't delete items that are stuck in a stash list
            if (!(entry is StashEntry))
            {
                //delete the item after
                if (deed != null)
                {
                    deed.Delete();
                }
                else
                {
                    entry.AbsorbItem(item);
                }
            }

            //start next add and give another cursor
            if (from != null)
            {
                AddItem(from);

                //resend the gump after it's all done
                if (!ItemStoreGump.RefreshGump(from))
                {
                    //send a new gump if there's no existing one up
                    from.SendGump(new ItemStoreGump(from,this));
                }
            }
        }

        //the target class used for adding items
        class AddItemTarget : Target
        {
            ItemStore _Store;       //reference the store

            //constructor
            public AddItemTarget(ItemStore store) : base(1,false,TargetFlags.None)
            {
                _Store = store;
            }

            protected override void OnTarget(Mobile from,object targeted)
            {
                _Store.AddItem(from,targeted);
            }
        }

        //this performs an automatic sweep of all items in the user's backpack, and fills the keys with the items if it is possible
        public void FillFromBackpack(Mobile from)
        {
            FillFromBackpack(from,true);
        }

        public void FillFromBackpack(Mobile from,bool resendgump)
        {
            if (from == null || from.Backpack == null)
            {
                return;
            }

            FillFromContainer(from.Backpack);

            //resend the gump after it's all done.  Note that if the gump is already up, it will refresh
            if (!ItemStoreGump.RefreshGump(from) && resendgump)
            {
                //send a new gump if there's no existing one up and resendgump was set true
                from.SendGump(new ItemStoreGump(from,this));
            }

            //refresh the item list entry gump if one is up
            ListEntryGump.RefreshGump(from);
        }

        public void FillFromContainer(Container container)
        {
            //generate a list of all items in the backpack
            List<Item> packitems = RecurseFindItemsInPack(container);

            //go through backpack list, and try to add the items
            foreach (Item item in packitems)
            {
                AddItem(null,item);
            }
        }

        //this checks for any expelled entries due to changes in item structure of any implementing device (eg. keys)
        protected void CheckExpelledEntries(Mobile from)
        {
            if (ExpelStoreEntries.Count > 0)
            {
                foreach (StoreEntry entry in ExpelStoreEntries)
                {
                    from.SendMessage("This no longer stores " + entry.Name);

                    //special handling:  if this entry is a list entry, then tell the list entry to dump its contents
                    if (entry is ListEntry)
                    {
                        ListEntry listentry = (ListEntry)entry;

                        while (listentry.ItemListEntries.Count > 0)
                        {
                            Item item = listentry.WithdrawItem(0);

                            if (item != null)
                            {
                                from.SendMessage("Adding " + entry.Name + " to your backpack.");
                                from.Backpack.DropItem(item);
                            }
                        }
                    }
                    else
                    {
                        while (entry.Amount > 0)
                        {
                            int towithdraw = entry.Amount;
                            Item item = entry.Withdraw(ref towithdraw,true);

                            if (item != null)
                            {
                                from.Backpack.DropItem(item);
                            }
                        }
                        from.SendMessage("Adding " + entry.Name + " to your backpack.");
                    }
                }

                //wipe the list
                _ExpelStoreEntries = new List<StoreEntry>();
            }
        }

        //this checks if the person using this store has a gump open for it.
        public void RefreshParentGump()
        {
            //if the object containing this store is an item
            if (Owner != null && Owner is Item)
            {
                Item item = (Item)Owner;

                //if the item is being held by a mobile
                if (item.RootParent is Mobile)
                {
                    Mobile player = (Mobile)item.RootParent;

                    //perform a refresh operation on their gump if this store is being displayed 
                    ItemStoreGump.RefreshGump(player,this);
                }
            }
        }

        //this assigns a reference to this store to all entries contained within it
        public void RegisterEntries()
        {
            foreach (StoreEntry entry in StoreEntries)
            {
                entry.Store = this;
            }
        }

        //event control methods

        //used when a player initially accesses the store, through doubleclicking keys, for example
        public void DoubleClick(Mobile from)
        {
            //every time the store is accessed, check for any stuff to expel from previous synchronization
            CheckExpelledEntries(from);

            from.SendGump(new ItemStoreGump(from,this));
        }

        //this synchronizes the item store with a specified list of item entries.  this is done to allow a scripter to on-the-fly 
        //modify the contents of any object containing an item store without having to manually reorganize the data entries of all 
        //instanced objects in the world save data
        public void SynchronizeStore(List<StoreEntry> synchentries)
        {
            //Idea: stick current active store entry list in a temporary location, and rebuild the list using the
            //synchentries list data.  Pull amount info from the temporary list (if it exists there) and remove that entry
            //from the temporary list.  Finally, put any leftover entries in the temporary list into the expel list, to 
            //be claimed externally the next time the device implementing this list is used.

            //store the current world loaded list into a temporary list
            List<StoreEntry> templist = StoreEntry.CloneList(_StoreEntries);

            //clear the current list so it's ready to be written to
            _StoreEntries = new List<StoreEntry>();

            //begin generating new list based on synch entries parameter
            foreach (StoreEntry entry in synchentries)
            {
                //use clone constructor
                StoreEntry newentry = entry.Clone();

                //find a matching item entry in the temporary list
                int matchingindex = StoreEntry.IndexOfType(templist,entry.Type);

                if (matchingindex > -1)
                {
                    //special treatment: if the entry is a list entry, then transfer the contained list too
                    if (entry is ListEntry && templist[matchingindex] is ListEntry)
                    {
                        //transfer over a clone copy of all item list entries
                        ((ListEntry)entry).CloneItemListEntries((ListEntry)templist[matchingindex]);
                    }
                    //special treatment: if the entry is a stash entry, then transfer the contained list too
                    else if (entry is StashEntry && templist[matchingindex] is StashEntry)
                    {
                        //transfer over a clone copy of all the item stash entries
                        ((StashEntry)entry).CloneStashListEntries((StashEntry)templist[matchingindex]);
                    }
                    else
                    {
                        //transfer over the amount into the new listing
                        entry.Amount = templist[matchingindex].Amount;
                    }

                    templist.RemoveAt(matchingindex);
                }
                else
                {
                }

                //add this to the finished product list
                _StoreEntries.Add(entry);
                //register entry with this store for refresh purposes
                entry.Store = this;
            }

            //finally, store the leftovers in the expel list
            foreach (StoreEntry entry in templist)
            {
                if (entry.Amount > 0)       //note, this will automatically ignore all column separators
                {
                    int matchingindex = StoreEntry.IndexOfType(ExpelStoreEntries,entry.Type);
                    if (matchingindex > -1)
                    {
                        //append amount to existing entry in expel list
                        ExpelStoreEntries[matchingindex].Amount += entry.Amount;
                    }
                    else
                    {
                        //add new entry to the expel list
                        ExpelStoreEntries.Add(entry);
                    }
                }
            }

            RefreshEntryHeight();
        }

        public void Dispose()
        {
            foreach (StoreEntry entry in _StoreEntries)
            {
                entry.Dispose();
            }
        }

        //persistance methods

        public void Serialize(GenericWriter writer)
        {
            //write version
            writer.Write(3);

            //version 3 stuff: lock/unlock property in withdrawal amount
            writer.Write(LockWithdrawalAmount);

            //version 2 stuff: insured stored from parent object
            writer.Write(Insured);

            //version 1 stuff:  loottype stored from parent object
            writer.Write((int)LootType);
            writer.Write(DisplayColumns);

            //version 0 stuff: original serial data
            writer.Write(Label);
            writer.Write(Dynamic);
            writer.Write(OfferDeeds);

            writer.Write(WithdrawAmount);
            writer.Write(MinWithdrawAmount);

            //write # of entries in _StoreEntries
            writer.Write(StoreEntries.Count);

            foreach (StoreEntry entry in StoreEntries)
            {
                //write the type so we know what kind to rebuild next time
                writer.Write(entry.GetType().Name);

                //perform type-specific serialization
                entry.Serialize(writer);
            }

            //keep track of the expel list so the items don't vanish
            writer.Write(ExpelStoreEntries.Count);

            foreach (StoreEntry entry in ExpelStoreEntries)
            {
                writer.Write(entry.GetType().Name);

                entry.Serialize(writer);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    {
                        LockWithdrawalAmount = reader.ReadBool();
                        goto case 2;
                    }

                case 2:
                    {
                        Insured = reader.ReadBool();
                        goto case 1;
                    }

                //case 1: added reference for loottype of parent object, as well as columns to display on gump
                case 1:
                    {
                        LootType = (LootType)reader.ReadInt();
                        DisplayColumns = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                default:
                    {
                        Label = reader.ReadString();
                        Dynamic = reader.ReadBool();
                        OfferDeeds = reader.ReadBool();

                        WithdrawAmount = reader.ReadInt();
                        MinWithdrawAmount = reader.ReadInt();

                        int entrycount = reader.ReadInt();

                        //read in the active items
                        if (entrycount > 0)
                        {
                            for (int i = 0; i < entrycount; i++)
                            {
                                //dynamically create store entries, ore derived ones, based on the type name stored in the serial data, then add it to the serial data reader

                                //WARNING... this is very delicate!!  if an improper type name was saved to the serial data (eg. if a tool or resource used to belong to a tool, and was removed from the shard) then an exception will be thrown here.
                                //be sure to remove any and all tool types from keys, and cycle a world load/save before taking that class out

                                StoreEntry entry = (StoreEntry)Activator.CreateInstance(ScriptCompiler.FindTypeByName(reader.ReadString()),new object[] { reader });

                                //register this store with the entry for refresh purposes
                                entry.Store = this;
                                StoreEntries.Add(entry);
                            }
                        }

                        //read in the expelled items

                        entrycount = reader.ReadInt();

                        if (entrycount > 0)
                        {
                            for (int i = 0; i < entrycount; i++)
                            {
                                StoreEntry entry = (StoreEntry)Activator.CreateInstance(ScriptCompiler.FindTypeByName(reader.ReadString()),new object[] { reader });
                                //register this store with the entry for refresh purposes
                                entry.Store = this;

                                ExpelStoreEntries.Add(entry);
                            }
                        }
                        break;
                    }
            }
        }//deserialize

        //this recursively checks a container for items, and tries to add them to the keys.  It will ignore
        //locked or trapped containers
        public static List<Item> RecurseFindItemsInPack(Container c)
        {
            List<Item> items = new List<Item>();

            foreach (Item item in c.Items)
            {
                //if the item is a container
                if (item is Container)
                {
                    //check if it can be trapped
                    if (item is TrapableContainer)
                    {
                        //if there's a trap on the container, ignore the container and move on
                        if (((TrapableContainer)item).TrapType != TrapType.None)
                        {
                            continue;
                        }

                        //if it's not trapped but is also lockable and is locked, ignore the container and move on
                        if (item is LockableContainer && ((LockableContainer)item).Locked)
                        {
                            continue;
                        }
                    }
                    //otherwise, recursively find items from this container
                    items.AddRange(RecurseFindItemsInPack((Container)item));
                }
                else    //it's not a container, so try to add to keys
                {
                    items.Add(item);
                }
            }
            return items;
        }

        //this produces a clone of the specified ItemStore
        public static ItemStore Clone(ItemStore store)
        {
            //create a new itemstore and populate its store entry list with a clone of the store entry list belonging to the source store
            ItemStore newstore = new ItemStore(StoreEntry.CloneList(store.StoreEntries));

            newstore.ExpelStoreEntries = StoreEntry.CloneList(store.ExpelStoreEntries);

            newstore.Label = store.Label;
            newstore.Dynamic = store.Dynamic;
            newstore.OfferDeeds = store.OfferDeeds;
            newstore.WithdrawAmount = store.WithdrawAmount;
            newstore.MinWithdrawAmount = store.MinWithdrawAmount;
            newstore.LootType = store.LootType;
            newstore.Insured = store.Insured;
            newstore.LockWithdrawalAmount = store.LockWithdrawalAmount;

            newstore.DisplayColumns = store.DisplayColumns;

            //this is reset to the new containing object after the cloning process is complete.
            newstore.Owner = store.Owner;

            foreach (StoreEntry entry in store.StoreEntries)
            {
                entry.Store = newstore;
            }

            return newstore;
        }
    }//class ItemStore
}