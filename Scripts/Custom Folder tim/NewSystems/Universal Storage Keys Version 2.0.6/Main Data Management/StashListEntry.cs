using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.CliLocHandler;

namespace Solaris.ItemStore
{
    //this is the base class for item entries stored in an stash list, which allows it to be sorted in the gump
    public class StashListEntry : IComparable
    {
        //arbitrarily chosen spot to dump all the items put in the stash
        const int STORE_POINT_X = 10;
        const int STORE_POINT_Y = 10;
        const int STORE_POINT_Z = 0;

        //reference back to the StashEntry objects which holds this particular entry
        public StashEntry StashEntry;

        //the reference to the item stored in the stash list entry
        protected Item _Item;

        //text entry hue
        protected int _Hue;

        protected string _Name;

        public Item Item { get { return _Item; } }

        public int Hue { get { return _Hue; } }

        public int CompareTo(object obj)
        {
            //can only compare to stash list entries
            if (!(obj is StashListEntry))
            {
                return 0;                                                               //cannot sort
            }

            StashListEntry entry = (StashListEntry)obj;

            //if the sort index is invalid
            if (StashEntry.SortData.Count <= StashEntry.SortIndex || StashEntry.SortIndex == -1)
            {
                return 0;
            }

            string thistext = StashEntry.GetText(this,StashEntry.SortIndex);
            string comparetext = StashEntry.GetText(entry,StashEntry.SortIndex);

            //null text can't be sorted
            if (thistext == null || comparetext == null)
            {
                return 0;
            }

            //test if they're numbers, and sort by numbers
            try
            {
                int thisnum = int.Parse(thistext);
                int comparenum = int.Parse(comparetext);

                return thisnum.CompareTo(comparenum) * StashEntry.SortOrder;
            }
            catch
            {
            }

            return thistext.CompareTo(comparetext) * StashEntry.SortOrder;
        }

        public string Name
        {
            get
            {
                if (_Name == null)
                {
                    GetBestName();
                }

                return _Name;
            }
        }

        //default constructor - no specified hue
        public StashListEntry(Item item) : this(item,1153)
        {
        }

        //master constructor
        public StashListEntry(Item item,int hue)
        {
            _Item = item;

            //move the item to be stored
            _Item.MoveToWorld(new Point3D(STORE_POINT_X,STORE_POINT_Y,STORE_POINT_Z),Map.Internal);

            //the item must be set movable false so it doesn't decay (or does it??)
            _Item.Movable = false;

            _Hue = hue;

            GetBestName();
        }

        //clone constructor
        public StashListEntry(StashListEntry entry)
        {
            //pass over the reference to the item
            _Item = entry.Item;
            _Hue = entry.Hue;
            StashEntry = entry.StashEntry;
        }

        //world load constructor
        public StashListEntry(GenericReader reader)
        {
            Deserialize(reader);
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public virtual Item WithdrawItem()
        {
            if (_Item != null)
            {
                //let the item become movable again
                _Item.Movable = true;
            }

            return _Item;
        }

        protected void GetBestName()
        {
            // Item name

            if (Item.Name != null && Item.Name != "")
            {
                _Name = Item.Name;
            }
            else
            {
                _Name = CliLoc.GetName(Item);
            }
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public virtual bool AllGood(Item item)
        {
            return (item != null);
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public virtual StashListEntry Clone()
        {
            return new StashListEntry((StashListEntry)this);
        }

        public void Dispose()
        {
            if (_Item != null)
            {
                _Item.Delete();
            }
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(_Item);
            writer.Write(_Hue);
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                default:
                    {
                        _Item = reader.ReadItem();

                        _Hue = reader.ReadInt();

                        break;
                    }
            }
        }
    }//class ItemListEntry
}