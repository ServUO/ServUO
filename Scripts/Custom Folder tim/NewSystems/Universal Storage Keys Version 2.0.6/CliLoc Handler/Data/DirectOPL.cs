using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Server;

namespace Solaris.CliLocHandler
{
    public class DirectObjectPropertyList : CollectionBase
    {
        public DirectObjectPropertyList(ObjectPropertyList opl)
        {
            //parse the data from the object property list
            //note: this code is adapted from Xanthor's Auction system

            if (opl == null)
            {
                return;
            }

            //since the object property list is based on a packet object, the property info is packed away in a packet format
            byte[] data = opl.UnderlyingStream.UnderlyingStream.ToArray();

            int index = 15; // First localization number index

            while (true)
            {
                //reset the number property
                uint number = 0;

                //if there's not enough room for another record, quit
                if (index + 4 >= data.Length)
                {
                    break;
                }

                //read number property from the packet data
                number = (uint)(data[index++] << 24 | data[index++] << 16 | data[index++] << 8 | data[index++]);

                //reset the length property
                ushort length = 0;

                //if there's not enough room for another record, quit
                if (index + 2 > data.Length)
                {
                    break;
                }

                //read length property from the packet data
                length = (ushort)(data[index++] << 8 | data[index++]);

                //determine the location of the end of the string
                int end = index + length;

                //truncate if necessary
                if (end >= data.Length)
                {
                    end = data.Length - 1;
                }

                //read the string into a StringBuilder object
                StringBuilder s = new StringBuilder();
                while (index + 2 <= end + 1)
                {
                    short next = (short)(data[index++] | data[index++] << 8);

                    if (next == 0)
                        break;

                    s.Append(Encoding.Unicode.GetString(BitConverter.GetBytes(next)));
                }

                //add this data to the list
                Add(new DOPLEntry((int)number,s.ToString()));
            }
        }

        protected override void OnClear()
        {
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

        public virtual void CopyTo(DOPLEntry[] array,int index)
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

        public virtual DOPLEntry this[int index]
        {
            get { return (DOPLEntry)(List[index]); }
        }

        public virtual int Add(DOPLEntry entry)
        {
            int add = List.Add(entry);
            return add;
        }

        public virtual bool Contains(DOPLEntry entry)
        {
            return List.Contains(entry);
        }

        public virtual int IndexOf(DOPLEntry entry)
        {
            return List.IndexOf(entry);
        }

        public virtual void Insert(int index,DOPLEntry entry)
        {
            List.Insert(index,entry);
        }

        public virtual void Remove(DOPLEntry entry)
        {
            List.Remove(entry);
        }
    }

    public class DOPLEntry
    {
        protected int _Index;
        protected string _Arguments;

        public int Index { get { return _Index; } }
        public string Arguments { get { return _Arguments; } }

        public DOPLEntry(int index,string arguments)
        {
            _Index = index;
            _Arguments = arguments;
        }

        //TODO: move compiling thing in here?
    }
}