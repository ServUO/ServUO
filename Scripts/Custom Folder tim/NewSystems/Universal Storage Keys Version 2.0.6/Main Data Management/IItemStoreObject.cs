using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Solaris.ItemStore
{
    //this interface defines objects that can hold a item store.  It contains methods used to check accessibility and such
    public interface IItemStoreObject
    {
        //checks if the specified mobile can use this object
        bool CanUse(Mobile from);

        //access
        void OnDoubleClick(Mobile from);

        //looks for a collection of entries that contains the desired types and amounts, and tallies up successful finds in the foundentries array so there is a one-to-one correspondance between what was looked for and what was found
        List<StoreEntry> FindConsumableEntries(Type[] types,int[] amounts,ref bool[] foundentries);

        //looks for an entry that contains any of the desired types and amount
        StoreEntry FindConsumableEntry(Type[] types,int amount);

        //looks for an entry that is of specified type, and satisfies the conditions in parameters
        StoreEntry FindEntryByEntryType(Type entrytype,int amount,object[] parameters);

        //activate the add functionality from context menu's
        void Add(Mobile from);

        //activate the fill functionality from context menu's
        void Fill(Mobile from);
    }
}