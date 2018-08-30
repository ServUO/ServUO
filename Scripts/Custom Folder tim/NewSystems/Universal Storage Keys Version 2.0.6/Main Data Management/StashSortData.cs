using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Solaris.ItemStore
{
    //the StashSortData class contains all information for displaying, sorting, and filtering 
    //properties of items stored in a stash entry
    public class StashSortData : CollectionBase, IDisposable
    {
        //main properties:

        //this stores columns that are not on display, but are available to be displayed by the user if desired
        protected List<StashSortEntry> _AvailableColumns;

        //total gump width
        public int Width
        {
            get
            {
                return this[this.Count - 1].X + this[this.Count - 1].Width;
            }
        }

        public List<StashSortEntry> AvailableColumns { get { return _AvailableColumns; } }

        protected bool _disposing;

        //default constructor - no property listings specified
        public StashSortData() : this(null,null)
        {
        }

        //default constructor: all displayed property listings, no extra available ones
        public StashSortData(StashSortEntry[] displayentries) : this(displayentries,null)
        {
        }

        //master constructor, where all property listings, both displayed and available, are specified
        public StashSortData(StashSortEntry[] displayentries,StashSortEntry[] availableentries)
        {
            if (availableentries != null)
            {
                _AvailableColumns = new List<StashSortEntry>(availableentries);
            }
            else
            {
                _AvailableColumns = new List<StashSortEntry>();
            }

            //special case: the first column in all Stash Sort listings shows the item graphic and the "best name" for the item
            this.Add(new StashSortEntry("Item","best name",0,100));

            if (displayentries != null)
            {
                //add in all user-specified columns
                foreach (StashSortEntry entry in displayentries)
                {
                    entry.X = Width;
                    this.Add(entry);
                }
            }
        }

        //used for reordering sort data columns - shift column left
        public void ShiftColumnLeft(int index)
        {
            if (index > 1)
            {
                StashSortEntry entry = this[index];

                Remove(entry);

                Insert(index - 1,entry);

                RefreshPositions();
            }
        }

        //used for reordering sort data columns - shift column right
        public void ShiftColumnRight(int index)
        {
            if (index < Count - 1 && index > 0)
            {
                StashSortEntry entry = this[index];

                Remove(entry);

                Insert(index + 1,entry);

                RefreshPositions();
            }
        }

        public void AddColumn(int index)
        {
            if (index < 0 || index >= _AvailableColumns.Count)
            {
                return;
            }

            Add(_AvailableColumns[index]);
            _AvailableColumns.RemoveAt(index);

            RefreshPositions();
        }

        public void RemoveColumn(int index)
        {
            _AvailableColumns.Add(this[index]);

            Remove(this[index]);

            RefreshPositions();
        }

        //this updates the position of all the entries when things are modified
        protected void RefreshPositions()
        {
            this[0].X = 0;

            for (int i = 1; i < Count; i++)
            {
                this[i].X = this[i - 1].X + this[i - 1].Width;
            }
        }

        protected override void OnClear()
        {
            Dispose();

            base.OnClear();
        }

        //ICollection members
        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        public virtual object SyncRoot
        {
            get { return List.SyncRoot; }
        }

        public virtual void CopyTo(StashSortEntry[] array,int index)
        {
            List.CopyTo(array,index);
        }

        //IList members
        public virtual bool IsFixedSize
        {
            get { return false; }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual StashSortEntry this[int index]
        {
            get { return (StashSortEntry)(List[index]); }
        }

        public virtual int Add(StashSortEntry entry)
        {
            int add = List.Add(entry);

            //this forces a refresh
            return add;
        }

        public virtual bool Contains(StashSortEntry entry)
        {
            return List.Contains(entry);
        }

        public virtual int IndexOf(StashSortEntry entry)
        {
            return List.IndexOf(entry);
        }

        public virtual void Insert(int index,StashSortEntry entry)
        {
            List.Insert(index,entry);
        }

        public virtual void Remove(StashSortEntry entry)
        {
            List.Remove(entry);
        }

        //IDisposable members
        public void Dispose()
        {
            if (!_disposing)
            {
                _disposing = true;
                foreach (StashSortEntry entry in this)
                {
                    entry.Dispose();
                }

                Clear();
            }
        }
    }

    //the column entry class
    public class StashSortEntry : IDisposable
    {
        //properties for each column

        //*property label header for the column
        public string Header;

        //*property type identifiers.  The array is used to specify nested properties
        public string[] PropertyNames;

        //in some cases (eg. skill bonuses) you need more than one property to fully express the data.  This holds the info for the alternate property to be displayed
        public string[] AltPropertyNames;

        //*column x-position
        public int X;

        //*column width
        public int Width;

        //*sort order (1 or -1 to sort ascending/decending, or 0 to ignore sorting this column)
        public int SortOrder;

        //*property text filter
        public string Filter;

        //default constructor: default x position and width
        public StashSortEntry(string header,string propertyname) : this(header,new string[] { propertyname },null)
        {
        }

        public StashSortEntry(string header,string[] propertynames) : this(header,propertynames,null)
        {
        }

        public StashSortEntry(string header,string propertyname,string altpropertyname) : this(header,new string[] { propertyname },new string[] { altpropertyname })
        {
        }

        public StashSortEntry(string header,string[] propertynames,string[] altpropertynames) : this(header,propertynames,altpropertynames,0,100)
        {
        }

        //default constructor: default sort order and filter
        public StashSortEntry(string header,string propertyname,int x,int width) : this(header,new string[] { propertyname },null,x,width)
        {
        }

        public StashSortEntry(string header,string[] propertynames,int x,int width) : this(header,propertynames,null,x,width,0,null)
        {
        }

        public StashSortEntry(string header,string propertyname,string altpropertyname,int x,int width) : this(header,new string[] { propertyname },new string[] { altpropertyname },x,width,0,null)
        {
        }

        public StashSortEntry(string header,string[] propertynames,string[] altpropertynames,int x,int width) : this(header,propertynames,altpropertynames,x,width,0,null)
        {
        }

        public StashSortEntry(string header,string propertyname,int x,int width,int sortorder,string filter) : this(header,new string[] { propertyname },null,x,width,sortorder,filter)
        {
        }

        public StashSortEntry(string header,string[] propertynames,int x,int width,int sortorder,string filter) : this(header,propertynames,null,x,width,sortorder,filter)
        {
        }

        public StashSortEntry(string header,string propertyname,string altpropertyname,int x,int width,int sortorder,string filter) : this(header,new string[] { propertyname },new string[] { altpropertyname },x,width,sortorder,filter)
        {
        }

        //master constructor
        public StashSortEntry(string header,string[] propertynames,string[] altpropertynames,int x,int width,int sortorder,string filter)
        {
            Header = header;
            PropertyNames = propertynames;
            AltPropertyNames = altpropertynames;
            X = x;
            Width = width;
            SortOrder = sortorder;
            Filter = filter;
        }

        public void Dispose()
        {
        }
    }
}