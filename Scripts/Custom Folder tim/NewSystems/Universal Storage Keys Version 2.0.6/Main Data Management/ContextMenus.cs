using System;
using Server;
using Server.ContextMenus;

namespace Solaris.ItemStore
{
    public class KeyOpenEntry : ContextMenuEntry
    {
        Mobile _From;
        IItemStoreObject _StoreObject;

        //3000362 = "open"
        public KeyOpenEntry(Mobile from,IItemStoreObject storeobject,int index) : base(362,index)
        {
            _From = from;
            _StoreObject = storeobject;
        }

        public override void OnClick()
        {
            if (((Item)_StoreObject).Deleted)
            {
                return;
            }

            _StoreObject.OnDoubleClick(_From);
        }
    }

    //"adding item..." 500927
    //"adding" 1005150
    //"add" 1079279
    //"backpack" 1022482
    //"add" 3000175
    //context menu classes
    public class KeyAddEntry : ContextMenuEntry
    {
        Mobile _From;
        IItemStoreObject _StoreObject;

        //3000175 = "add"
        public KeyAddEntry(Mobile from,IItemStoreObject storeobject,int index) : base(175,index)
        {
            _From = from;
            _StoreObject = storeobject;
        }

        public override void OnClick()
        {
            if (((Item)_StoreObject).Deleted)
            {
                return;
            }

            _StoreObject.Add(_From);
        }
    }

    public class KeyFillEntry : ContextMenuEntry
    {
        Mobile _From;
        IItemStoreObject _StoreObject;

        //141 - "Auto"
        //544 - Smurf it!
        //6230 - "refill from stock"
        public KeyFillEntry(Mobile from,IItemStoreObject storeobject,int index) : base(6230,index)
        {
            _From = from;
            _StoreObject = storeobject;
        }

        public override void OnClick()
        {
            if (((Item)_StoreObject).Deleted)
            {
                return;
            }

            _StoreObject.Fill(_From);
        }
    }
}