using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Commands;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Targeting;
using Solaris.Extras;
using Solaris.CliLocHandler;

namespace Solaris.ItemStore
{
    //resource entry class - contains data needed to store a resource into a resource store
    public class StoreEntry
    {
        //this is used for gump button graphic control and relative placement
        public virtual int ButtonID { get { return 0x4B9; } }
        public virtual int ButtonX { get { return 0; } }
        public virtual int ButtonY { get { return 5; } }

        public virtual bool RequiresRecipient { get { return false; } }

        //the main type accepted and provided by the entry
        protected Type _Type;

        //a list of alternate types that can be added and give the same result
        protected Type[] _AlternateTypes;

        //label for the gump display
        protected string _Name;

        //amount stored in this entry
        protected int _Amount;

        //used for displaying the entry on the gump
        protected int _Height;          //how high the line needs to be to properly display artwork
        protected int _X;               //x-y offset of artwork
        protected int _Y;

        //public accessors
        public virtual int Amount
        {
            get { return _Amount; }
            set
            {
                _Amount = value;
            }
        }

        public Type Type { get { return _Type; } }
        public string Name { get { return _Name; } }
        public int Height { get { return _Height; } }
        public int X { get { return _X; } }
        public int Y { get { return _Y; } }
        public Type[] AlternateTypes { get { return _AlternateTypes; } }

        //reference to item store, used for refreshing displays when resources are auto-withdrawn
        public ItemStore Store;

        //used for reagent/resource consumption directly from keys
        public int ToConsume;

        //used for debugging if there is a problem, passed on to user who accesses a bugged entry
        public string ErrorMessage;

        //default constructor: set up one type, a name, and default 0 quantity
        public StoreEntry(Type type,string name) : this(type,null,name,0)
        {
        }

        //default constructor: set up several types, a name, and default 0 quantity
        public StoreEntry(Type type,Type[] subtypes,string name) : this(type,subtypes,name,0)
        {
        }

        //default constructor: set up one type, a name, an amount, and default size/offset of artwork
        public StoreEntry(Type type,string name,int amount) : this(type,null,name,amount,25,0,0)
        {
        }

        //default constructor: set up several types, a name, an amount, and default size/offset of artwork
        public StoreEntry(Type type,Type[] subtypes,string name,int amount) : this(type,subtypes,name,amount,25,0,0)
        {
        }

        //default constructor: set up one type, a name, an amount, and size/offset of artwork
        public StoreEntry(Type type,string name,int amount,int height,int x,int y) : this(type,null,name,amount,height,x,y)
        {
        }

        //master constructor: set up several types, a name, an amount, and size/offset of artwork
        public StoreEntry(Type type,Type[] subtypes,string name,int amount,int height,int x,int y)
        {
            _Type = type;

            if (subtypes != null)
            {
                _AlternateTypes = new Type[subtypes.Length];

                for (int i = 0; i < subtypes.Length; i++)
                {
                    _AlternateTypes[i] = subtypes[i];
                }
            }

            _Name = name;

            _Amount = Math.Max(amount,0);
            _Height = height;
            _X = x;
            _Y = y;
        }

        //clone constructor
        public StoreEntry(StoreEntry entry) : this(entry.Type,entry.AlternateTypes,entry.Name,entry.Amount,entry.Height,entry.X,entry.Y)
        {
        }

        //generic reader deser constructor
        public StoreEntry(GenericReader reader)
        {
            Deserialize(reader);
        }

        //methods

        //check if this store entry is accessible to the specified player
        public virtual bool CanUse(Mobile from)
        {
            if (Store == null)
            {
                return false;
            }

            return Store.CanUse(from);
        }

        //this method performs the addition of a resource to the entry
        public virtual bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            //store the amount in a local variable for now
            int amount = item.Amount;

            //if the item has some quantity within it, like flour...
            if (item is IHasQuantity && !(item is BaseBeverage))
            {
                amount = ((IHasQuantity)item).Quantity;
            }

            //can't fit test
            if (Amount + amount > ItemStore.MaxAmount)
            {
                return false;
            }
            Amount += amount;

            return true;
        }

        public void AbsorbItem(Item item)
        {
            if (BaseStoreKey.EmptyContents)
            {
                Empty(item);
            }
            else
            {
                item.Delete();
            }
        }

        public virtual void Empty(Item item)
        {
            item.Delete();
        }

        public virtual Item FindRecipient(Mobile from)
        {
            return from.Backpack;
        }

        public Item Withdraw(ref int amount)
        {
            return Withdraw(ref amount,false);
        }

        //this handles the check to see if the requested item matches this entry.  This is used when an item store is checking thru
        //all its entries to find one that matches
        public bool Match(Item item)
        {
            return Match(item,false);
        }

        //check if the specified item matches any items in this entry.  This is overloaded for things like runic tools since
        //more than just the type needs to be tested
        public virtual bool Match(Item item,bool checksubtypes)
        {
            return Match(item.GetType(),checksubtypes);
        }

        public bool Match(Type checktype,bool checksubtypes)
        {
            return Match(new Type[] { checktype },checksubtypes);
        }

        public bool Match(Type[] checktypes,bool checksubtypes)
        {
            foreach (Type checktype in checktypes)
            {
                if (checktype == Type)
                {
                    return true;
                }
            }

            if (!checksubtypes || AlternateTypes == null)
            {
                return false;
            }

            foreach (Type type in AlternateTypes)
            {
                foreach (Type checktype in checktypes)
                {
                    if (checktype == type)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //this match method is useful for when an abstract collection of parameters are passed.  It is only defined in 
        //child classes
        public virtual bool Match(int amount,object[] parameters)
        {
            return false;
        }

        //this method performs the withdrawal of a resource from an entry
        public virtual Item Withdraw(ref int amount,bool deedable)
        {
            //default item definition does not allow for deedable items - need a ResourceEntry for that
            if (deedable)
            {
                return null;
            }

            //limit the amount to withdraw by the amount stored in the entry
            //the most Amount you can properly have a stacked item is 60k units.  Otherwise, drag/drop gets messed up
            int towithdraw = Math.Min(Math.Min(amount,_Amount),60000);

            try
            {
                Object o = Activator.CreateInstance(_Type);

                Item resource = (Item)o;

                if (resource is IHasQuantity && !(resource is BaseBeverage))
                {
                    ((IHasQuantity)resource).Quantity = towithdraw;

                    //cap the amount withdrawn based on the object's quantity resitrictions
                    towithdraw = ((IHasQuantity)resource).Quantity;
                }
                //only withdraw one if it's not stackable
                else if (!resource.Stackable)
                {
                    towithdraw = 1;
                }
                else
                {
                    //set the amount on the stackable object
                    resource.Amount = towithdraw;
                }

                //pull that amount out of the store
                _Amount -= towithdraw;
                amount = towithdraw;        //update the byref value "amount" so the withdrawal system knows how much was actually taken out
                return resource;
            }
            catch
            {
                return null;
            }
        }

        public virtual Item WithdrawTo(ref int amount,Item destination)
        {
            return null;
        }

        //this generates an item based on the entry to be used to display artwork on the gump
        //TODO: change this to extract the itemid and hue, so it's not so messy?
        public virtual Item GetModel()
        {
            try
            {
                Object o = Activator.CreateInstance(Type);

                return (Item)o;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public virtual StoreEntry Clone()
        {
            return new StoreEntry(this);
        }

        //this deducts from the amount by the value specfified in ToConsume integer.  This is used for direct access in
        //casting or crafting engines
        public void Consume()
        {
            _Amount -= ToConsume;

            ToConsume = 0;
        }

        public void RefreshParentGump()
        {
            if (Store != null)
            {
                Store.RefreshParentGump();
            }
        }

        public virtual void Dispose()
        {
        }

        public virtual void Serialize(GenericWriter writer)
        {
            //version
            writer.Write(0);

            //store type by string, and recover it by reflection
            if (_Type != null)
            {
                writer.Write(_Type.Name);
            }
            else
            {
                writer.Write((string)null);
            }

            writer.Write(_Name);

            writer.Write(_Amount);

            writer.Write(_Height);
            writer.Write(_X);
            writer.Write(_Y);

            if (_AlternateTypes != null)
            {
                writer.Write(_AlternateTypes.Length);

                for (int i = 0; i < _AlternateTypes.Length; i++)
                {
                    writer.Write(AlternateTypes[i].Name);
                }
            }
            else
            {
                writer.Write((int)0);
            }
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            //persists because it is useful when doing error checking.
            string typename;

            switch (version)
            {
                case 0:
                default:
                    {
                        typename = reader.ReadString();
                        _Name = reader.ReadString();
                        _Amount = reader.ReadInt();

                        _Height = reader.ReadInt();
                        _X = reader.ReadInt();
                        _Y = reader.ReadInt();

                        int alternatetypecount = reader.ReadInt();

                        if (alternatetypecount > 0)
                        {
                            _AlternateTypes = new Type[alternatetypecount];

                            for (int i = 0; i < alternatetypecount; i++)
                            {
                                string alttypename = reader.ReadString();
                                try
                                {
                                    AlternateTypes[i] = ScriptCompiler.FindTypeByName(alttypename);
                                }
                                catch
                                {
                                }
                            }
                        }

                        break;
                    }
            }

            try
            {
                _Type = ScriptCompiler.FindTypeByName(typename);
            }
            catch
            {
            }

            //null type handler
            if (_Type == null)
            {
                //TODO: define this better
            }
        }//deserialize

        //static, data management methods

        //this is used to generate a new, unlinked clone of a list of entries
        public static List<StoreEntry> CloneList(List<StoreEntry> sourcelist)
        {
            List<StoreEntry> destlist = new List<StoreEntry>();

            if (sourcelist != null)
            {
                //transfer contents to temp list
                foreach (StoreEntry entry in sourcelist)
                {
                    //use clone constructor
                    //Clone() method is overridden in the derived classes to allow proper clone constructor access for the right class
                    StoreEntry newentry = entry.Clone();
                    newentry.Store = entry.Store;
                    destlist.Add(newentry);
                }
            }

            return destlist;
        }

        public static int IndexOfItem(List<StoreEntry> sourcelist,Item item)
        {
            return IndexOfItem(sourcelist,item,false);
        }

        public static int IndexOfItem(List<StoreEntry> sourcelist,Item item,bool checkalltypes)
        {
            int index = 0;

            foreach (StoreEntry entry in sourcelist)
            {
                if (entry.Match(item,checkalltypes))
                {
                    return index;
                }
                index++;
            }

            //not found
            return -1;
        }

        //scan a list of store entries for a particular type
        public static int IndexOfType(List<StoreEntry> sourcelist,Type type)
        {
            return IndexOfType(sourcelist,new Type[] { type });
        }

        public static int IndexOfType(List<StoreEntry> sourcelist,Type type,bool checkalltypes)
        {
            return IndexOfType(sourcelist,new Type[] { type },checkalltypes);
        }

        public static int IndexOfType(List<StoreEntry> sourcelist,Type[] types)
        {
            //default: ignore subtypes
            return IndexOfType(sourcelist,types,false);
        }

        public static int IndexOfType(List<StoreEntry> sourcelist,Type[] types,bool checkalltypes)
        {
            int index = 0;

            foreach (StoreEntry entry in sourcelist)
            {
                if (entry.Match(types,checkalltypes))
                {
                    return index;
                }
                index++;
            }

            //not found
            return -1;
        }

        //this calculates the total height required to display a list of entries on a gump
        public static int GetTotalHeight(List<StoreEntry> sourcelist)
        {
            int totalheight = 0;

            foreach (StoreEntry entry in sourcelist)
            {
                totalheight += entry.Height;
            }

            return totalheight;
        }
    }//class StoreEntry

    //column separation entry entry - used to tell the gump to step over to new column when rendering contents
    //this is just a simple placeholder that doesn't have any functionality, but the gump uses to calculate
    //page sizes, and control position
    public class ColumnSeparationEntry : StoreEntry
    {
        public ColumnSeparationEntry() : base(null,null,null,0,0,0,0)
        {
        }

        //clone constructor
        public ColumnSeparationEntry(ColumnSeparationEntry entry) : this()
        {
        }

        //deserialization constructor
        public ColumnSeparationEntry(GenericReader reader) : base(reader)
        {
        }

        //this method performs the addition of a resource to the entry
        public override bool Add(Item item)
        {
            return false;
        }

        public override Item Withdraw(ref int amount,bool makedeed)
        {
            return null;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new ColumnSeparationEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            //version
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            //handle base StoreEntry deserialization first
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }//deserialize	
    }

    //resource entry class - special store entry that allows for deeding possibility
    public class ResourceEntry : StoreEntry
    {
        bool _Deedable;                 //whether the resource can be put into or pulled from a commodity deed

        public bool Deedable { get { return _Deedable; } }

        //default constructor: set up one type, a name, and default 0 quantity
        public ResourceEntry(Type type,string name) : this(type,null,name,0)
        {
        }

        //default constructor: set up several types, a name, and default 0 quantity
        public ResourceEntry(Type type,Type[] subtypes,string name) : this(type,subtypes,name,0)
        {
        }

        //default constructor: set up one type, a name, an amount, and default size/offset of artwork
        public ResourceEntry(Type type,string name,int amount) : this(type,null,name,amount,25,0,0)
        {
        }

        //default constructor: set up several types, a name, an amount, and default size/offset of artwork
        public ResourceEntry(Type type,Type[] subtypes,string name,int amount) : this(type,subtypes,name,amount,25,0,0)
        {
        }

        //default constructor: set up one type, a name, an amount, and size/offset of artwork
        public ResourceEntry(Type type,string name,int amount,int height,int x,int y) : this(type,null,name,amount,height,x,y)
        {
        }

        //master constructor: set up several types, a name, an amount, and size/offset of artwork
        public ResourceEntry(Type type,Type[] subtypes,string name,int amount,int height,int x,int y) : base(type,subtypes,name,amount,height,x,y)
        {
            //build a copy of the object to test if it's deedable
            try
            {
                Object o = Activator.CreateInstance(_Type);

                Item resource = (Item)o;

                _Deedable = (resource is ICommodity);

                //clean up after itself
                resource.Delete();
            }
            catch
            {
            }
        }

        //clone constructor
        public ResourceEntry(ResourceEntry entry) : this(entry.Type,entry.AlternateTypes,entry.Name,entry.Amount,entry.Height,entry.X,entry.Y)
        {
        }

        //generic reader deser constructor
        public ResourceEntry(GenericReader reader) : base(reader)
        {
        }

        //this method performs the addition of a resource to the entry
        public override bool Add(Item item)
        {
            //don't allow non-empty potion kegs to go in
            if (item is PotionKeg && ((PotionKeg)item).Held > 0)
            {
                return false;
            }

            //don't allow non-emtpy base beverages
            if (item is BaseBeverage && ((BaseBeverage)item).Quantity > 0)
            {
                return false;
            }

            //don't allow marked runes to go in
            if (item is RecallRune && ((RecallRune)item).Marked)
            {
                return false;
            }

            //otherwise no special care needed for resources
            return base.Add(item);
        }

        //this handles the check to see if the requested item matches this entry.  This is used when an item store is checking thru
        //all its entries to find one that matches
        public override bool Match(Item item,bool checksubtypes)
        {
            if (!base.Match(item,checksubtypes))
            {
                return false;
            }

            //don't allow non-emtpy base beverages
            if (item is BaseBeverage && ((BaseBeverage)item).Quantity > 0)
            {
                return false;
            }

            //don't allow non-empty potion kegs to go in
            if (item is PotionKeg && ((PotionKeg)item).Held > 0)
            {
                return false;
            }

            return true;
        }

        public override Item Withdraw(ref int amount,bool makedeed)
        {
            Item resource = base.Withdraw(ref amount,false);

            if (resource != null)
            {
                //if they want you to make a deed and it's a valid deed item
                if (makedeed && (resource is ICommodity) && resource.Stackable)
                {
                    CommodityDeed deed = new CommodityDeed(resource);
                    return deed;
                }

                return resource;
            }
            return null;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new ResourceEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            //version
            writer.Write(0);

            //store type by string, and recover it by reflection
            writer.Write(_Deedable);
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
                        _Deedable = reader.ReadBool();
                        break;
                    }
            }
        }//deserialize

        //static, data management methods

    }//class ResourceEntry

    //Tool entry class - handles objects that support the interface IUsesRemaining for tools, etc
    public class ToolEntry : StoreEntry
    {
        public override int ButtonID { get { return 0x9A8; } }
        public override int ButtonX { get { return 5; } }
        public override int ButtonY { get { return -3; } }

        //default constructor: set up one type, a name, and default 0 quantity
        public ToolEntry(Type type,string name) : this(type,null,name,0)
        {
        }

        //default constructor: set up several types, a name, and default 0 quantity
        public ToolEntry(Type type,Type[] subtypes,string name) : this(type,subtypes,name,0)
        {
        }

        //default constructor: set up one type, a name, an amount, and default size/offset of artwork
        public ToolEntry(Type type,string name,int amount) : this(type,null,name,amount,25,0,0)
        {
        }

        //default constructor: set up several types, a name, an amount, and default size/offset of artwork
        public ToolEntry(Type type,Type[] subtypes,string name,int amount) : this(type,subtypes,name,amount,25,0,0)
        {
        }

        //default constructor: set up one type, a name, an amount, and size/offset of artwork
        public ToolEntry(Type type,string name,int amount,int height,int x,int y) : this(type,null,name,amount,height,x,y)
        {
        }

        //master constructor: set up several types, a name, an amount, and size/offset of artwork
        public ToolEntry(Type type,Type[] subtypes,string name,int amount,int height,int x,int y) : base(type,subtypes,name,amount,height,x,y)
        {
        }

        //clone constructor
        public ToolEntry(ToolEntry entry) : this(entry.Type,entry.AlternateTypes,entry.Name,entry.Amount,entry.Height,entry.X,entry.Y)
        {
        }

        //generic reader deser constructor
        public ToolEntry(GenericReader reader) : base(reader)
        {
        }

        //this method performs the addition of a tool to the entry
        public override bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            //if it's not a tool, ignore it
            if (!(item is IUsesRemaining))
            {
                return false;
            }

            //don't add insured/blessed tools
            if (item.LootType != LootType.Regular || item.Insured)
            {
                return false;
            }

            IUsesRemaining tool = (IUsesRemaining)item;

            if (Amount + tool.UsesRemaining > ItemStore.MaxAmount)
            {
                return false;
            }
            Amount += tool.UsesRemaining;

            return true;
        }

        public override Item Withdraw(ref int amount,bool makedeed)
        {
            //force it so you cannot withdraw less than 50 uses on a tool
            //this removes the ability to exploit selling 1-use tools to vendors for gold farming
            int towithdraw = Math.Min(Math.Max(50,amount),_Amount);

            //TODO: find a way to access this number from item store..  50 is hardcoded here, but should be set by the implementing object
            if (_Amount - towithdraw < 50)
            {
                //if the amount to be left behind is less than 50 uses remaining, then force the keys to withdraw all that's left
                towithdraw = _Amount;
            }

            try
            {
                Object o = Activator.CreateInstance(_Type);

                if (!(o is IUsesRemaining))
                {
                    return null;
                }

                IUsesRemaining tool = (IUsesRemaining)o;

                tool.UsesRemaining = towithdraw;

                //pull that amount out of the store
                _Amount -= towithdraw;

                return (Item)tool;
            }
            catch
            {
                return null;
            }
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new ToolEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            //handle base StoreEntry deserialization first
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }//deserialize

        //static, data management methods

    }//class ToolEntry

    //a more general entry type that uses reflection to access the property of interest
    public class GenericEntry : StoreEntry
    {
        public override int ButtonID { get { return 0x9A8; } }
        public override int ButtonX { get { return 5; } }
        public override int ButtonY { get { return -3; } }

        public string PropertyName;

        //default constructor: set up type, and default 0 quantity
        public GenericEntry(Type type,string propertyname) : this(type,propertyname,CliLoc.GetName(type))
        {
        }

        public GenericEntry(Type type,string propertyname,string name) : this(type,propertyname,name,0)
        {
        }

        //default constructor: set up name, an amount, and default size/offset of artwork
        public GenericEntry(Type type,string propertyname,string name,int amount) : this(type,propertyname,name,amount,25,0,0)
        {
        }

        //master constructor: set up name, an amount, and size/offset of artwork
        public GenericEntry(Type type,string propertyname,string name,int amount,int height,int x,int y) : base(type,name,amount,height,x,y)
        {
            PropertyName = propertyname;
        }

        //clone constructor
        public GenericEntry(GenericEntry entry) : this(entry.Type,entry.PropertyName,entry.Name,entry.Amount,entry.Height,entry.X,entry.Y)
        {
        }

        //generic reader deser constructor
        public GenericEntry(GenericReader reader) : base(reader)
        {
        }

        //this method performs the addition of a tool to the entry
        public override bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            //don't add insured/blessed tools
            if (item.LootType != LootType.Regular || item.Insured)
            {
                return false;
            }

            //if the item type does not match
            if (!(item.GetType().IsSubclassOf(_Type)) && !(item.GetType() == _Type))
            {
                return false;
            }

            //here's where it gets fancy, due to the nature of the dip tubs.  
            int amounttoadd = 0;

            Type itemtype = item.GetType();

            PropertyInfo pi = itemtype.GetProperty(PropertyName);

            try
            {
                Object o = (Object)pi.GetValue(item,null);

                amounttoadd = (int)o;
            }
            catch
            {
                return false;
            }

            if (Amount + amounttoadd > ItemStore.MaxAmount)
            {
                return false;
            }

            Amount += amounttoadd;

            return true;
        }

        public override Item Withdraw(ref int amount,bool makedeed)
        {
            int towithdraw = Math.Min(amount,_Amount);

            Item item = (Item)Activator.CreateInstance(_Type,new object[] { });

            Type itemtype = item.GetType();

            PropertyInfo pi = itemtype.GetProperty(PropertyName);

            pi.SetValue(item,towithdraw,null);

            _Amount -= towithdraw;

            return item;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new GenericEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(PropertyName);
        }

        public override void Deserialize(GenericReader reader)
        {
            //handle base StoreEntry deserialization first
            base.Deserialize(reader);
            int version = reader.ReadInt();

            PropertyName = reader.ReadString();
        }//deserialize
    }

    //a more general entry type with a specified distinguishable property that uses reflection to access the property of interest
    public class GenericDistinguishableEntry : StoreEntry
    {
        public override int ButtonID { get { return 0x9A8; } }
        public override int ButtonX { get { return 5; } }
        public override int ButtonY { get { return -3; } }

        public string PropertyName;
        public string DistinguishProp;          //property name which distinguishes this from others of the same type
        public string DistinguishValue;         //property value which distinguihses this from others of the same type

        //default constructor: set up type, and property that distinguishes it from others, 
        public GenericDistinguishableEntry(Type type,string distinguishprop,string distinguishvalue) : this(type,distinguishprop,distinguishvalue,"Amount")
        {
        }

        //default: use the distinguishvalue as the name for the listing
        public GenericDistinguishableEntry(Type type,string distinguishprop,string distinguishvalue,string propertyname) : this(type,distinguishprop,distinguishvalue,propertyname,distinguishvalue)
        {
        }

        public GenericDistinguishableEntry(Type type,string distinguishprop,string distinguishvalue,string propertyname,string name) : this(type,distinguishprop,distinguishvalue,propertyname,name,0)
        {
        }

        //default constructor: set up name, an amount, and default size/offset of artwork
        public GenericDistinguishableEntry(Type type,string distinguishprop,string distinguishvalue,string propertyname,string name,int amount) : this(type,distinguishprop,distinguishvalue,propertyname,name,amount,25,0,0)
        {
        }

        //master constructor: set up name, an amount, and size/offset of artwork
        public GenericDistinguishableEntry(Type type,string distinguishprop,string distinguishvalue,string propertyname,string name,int amount,int height,int x,int y) : base(type,name,amount,height,x,y)
        {
            PropertyName = propertyname;
            DistinguishProp = distinguishprop;
            DistinguishValue = distinguishvalue;
        }

        //clone constructor
        public GenericDistinguishableEntry(GenericDistinguishableEntry entry) : this(entry.Type,entry.DistinguishProp,entry.DistinguishValue,entry.PropertyName,entry.Name,entry.Amount,entry.Height,entry.X,entry.Y)
        {
        }

        //generic reader deser constructor
        public GenericDistinguishableEntry(GenericReader reader) : base(reader)
        {
        }

        //this method performs the addition of a tool to the entry
        public override bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            //don't add insured/blessed items
            if (item.LootType != LootType.Regular || item.Insured)
            {
                return false;
            }

            //if the item type does not match
            if (!(item.GetType().IsSubclassOf(_Type)) && !(item.GetType() == _Type))
            {
                return false;
            }

            Type itemtype = item.GetType();

            PropertyInfo pi = itemtype.GetProperty(DistinguishProp);

            Object o = (Object)pi.GetValue(item,null);

            if (o.ToString() != DistinguishValue)
            {
                return false;
            }

            //here's where it gets fancy, due to the nature of the dip tubs.  
            int amounttoadd = 0;

            pi = itemtype.GetProperty(PropertyName);

            try
            {
                o = (Object)pi.GetValue(item,null);

                amounttoadd = (int)o;
            }
            catch
            {
                return false;
            }

            if (Amount + amounttoadd > ItemStore.MaxAmount)
            {
                return false;
            }

            Amount += amounttoadd;

            return true;
        }

        public override Item Withdraw(ref int amount,bool makedeed)
        {
            int towithdraw = Math.Min(amount,_Amount);

            //This only works with zero-parameter constructors.  Do not try to put items without zero-parameter constructors in this
            //TODO: develop a constructor parameter handler like in the [add command structure
            Item item = (Item)Activator.CreateInstance(_Type,new object[] { });

            Type itemtype = item.GetType();

            PropertyInfo pi = itemtype.GetProperty(PropertyName);

            //make sure you don't stack nonstackable items
            if (PropertyName == "Amount" && !item.Stackable)
            {
                towithdraw = 1;
            }

            pi.SetValue(item,towithdraw,null);

            pi = itemtype.GetProperty(DistinguishProp);

            object toset = null;
            if (Properties.ConstructFromString(pi.PropertyType,item,DistinguishValue,ref toset) != null)
            {
                return null;
            }

            pi.SetValue(item,toset,null);

            _Amount -= towithdraw;

            return item;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new GenericDistinguishableEntry(this);
        }

        //this handles the check to see if the requested item matches this entry.  This is used when an item store is checking thru
        //all its entries to find one that matches
        public override bool Match(Item item,bool checksubtypes)
        {
            if (!base.Match(item,checksubtypes))
            {
                return false;
            }

            Type itemtype = item.GetType();

            PropertyInfo pi = itemtype.GetProperty(DistinguishProp);

            Object o = (Object)pi.GetValue(item,null);

            return o.ToString() == DistinguishValue;
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(PropertyName);
            writer.Write(DistinguishProp);
            writer.Write(DistinguishValue);
        }

        public override void Deserialize(GenericReader reader)
        {
            //handle base StoreEntry deserialization first
            base.Deserialize(reader);
            int version = reader.ReadInt();

            PropertyName = reader.ReadString();
            DistinguishProp = reader.ReadString();
            DistinguishValue = reader.ReadString();
        }//deserialize
    }

    //Runic Tool entry class - handles runic tools
    public class RunicToolEntry : StoreEntry
    {
        public override int ButtonID { get { return 0x9A8; } }
        public override int ButtonX { get { return 5; } }
        public override int ButtonY { get { return -3; } }

        //reference to the resource type
        protected CraftResource _Resource;

        public CraftResource Resource { get { return _Resource; } }

        //default constructor: set up one type, a name, and default 0 quantity
        public RunicToolEntry(Type type,CraftResource resource,string name) : this(type,null,resource,name,0)
        {
        }

        //default constructor: set up several types, a name, and default 0 quantity
        public RunicToolEntry(Type type,Type[] subtypes,CraftResource resource,string name) : this(type,subtypes,resource,name,0)
        {
        }

        //default constructor: set up one type, a name, an amount, and default size/offset of artwork
        public RunicToolEntry(Type type,CraftResource resource,string name,int amount) : this(type,null,resource,name,amount,25,0,0)
        {
        }

        //default constructor: set up several types, a name, an amount, and default size/offset of artwork
        public RunicToolEntry(Type type,Type[] subtypes,CraftResource resource,string name,int amount) : this(type,subtypes,resource,name,amount,25,0,0)
        {
        }

        //default constructor: set up one type, a name, an amount, and size/offset of artwork
        public RunicToolEntry(Type type,CraftResource resource,string name,int amount,int height,int x,int y) : this(type,null,resource,name,amount,height,x,y)
        {
        }

        //master constructor: set up several types, a name, an amount, and size/offset of artwork
        public RunicToolEntry(Type type,Type[] subtypes,CraftResource resource,string name,int amount,int height,int x,int y) : base(type,subtypes,name,amount,height,x,y)
        {
            _Resource = resource;
        }

        //clone constructor
        public RunicToolEntry(RunicToolEntry entry) : this(entry.Type,entry.AlternateTypes,entry.Resource,entry.Name,entry.Amount,entry.Height,entry.X,entry.Y)
        {
        }

        //generic reader deser constructor
        public RunicToolEntry(GenericReader reader) : base(reader)
        {
        }

        //this method performs the addition of a tool to the entry
        public override bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            //if it's not a tool, ignore it
            if (!(item is BaseRunicTool))
            {
                return false;
            }

            BaseRunicTool tool = (BaseRunicTool)item;

            if (Amount + tool.UsesRemaining > ItemStore.MaxAmount)
            {
                return false;
            }
            Amount += tool.UsesRemaining;

            return true;
        }

        public override Item Withdraw(ref int amount,bool makedeed)
        {
            //force it so you cannot withdraw less than 50 uses on a tool
            //this removes the ability to exploit selling 1-use tools to vendors for gold farming
            int towithdraw = Math.Min(Math.Max(50,amount),_Amount);

            //this removes the ability to exploit selling 1-use tools to vendors for gold farming

            try
            {
                //constructor for all derived classes of BaseRunicTool require the craftresource property specified
                Object o = Activator.CreateInstance(_Type,new object[] { _Resource });

                if (!(o is BaseRunicTool))
                {
                    return null;
                }

                BaseRunicTool tool = (BaseRunicTool)o;

                tool.UsesRemaining = towithdraw;

                //pull that amount out of the store
                _Amount -= towithdraw;

                return (Item)tool;
            }
            catch
            {
                return null;
            }
        }

        //this generates an item based on the entry to be used to display artwork on the gump
        //TODO: change this to extract the itemid and hue, so it's not so messy?
        public override Item GetModel()
        {
            try
            {
                //runic tools need a resource specification in their constructor
                Object o = Activator.CreateInstance(Type,new object[] { _Resource });

                return (Item)o;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
        }

        //this handles the check to see if the requested item matches this entry.  This is used when an item store is checking thru
        //all its entries to find one that matches
        public override bool Match(Item item,bool checksubtypes)
        {
            if (!base.Match(item,checksubtypes))
            {
                return false;
            }

            return (((BaseRunicTool)item).Resource == _Resource);
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new RunicToolEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            writer.Write(0);
            writer.Write((int)_Resource);
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
                        _Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }
        }//deserialize

        //static, data management methods

    }//class RunicToolEntry

    //Champion Skull entry class
    public class ChampionSkullEntry : StoreEntry
    {
        protected ChampionSkullType _SkullType;

        public ChampionSkullType SkullType { get { return _SkullType; } }

        //default constructor: set up one type, a name, and default 0 quantity
        public ChampionSkullEntry(ChampionSkullType type) : this(type,type.ToString())
        {
        }

        public ChampionSkullEntry(ChampionSkullType type,string name) : this(type,name,0)
        {
        }

        public ChampionSkullEntry(ChampionSkullType type,string name,int amount) : this(type,name,amount,25,0,0)
        {
        }

        //master constructor: set up several types, a name, an amount, and size/offset of artwork
        public ChampionSkullEntry(ChampionSkullType type,string name,int amount,int height,int x,int y) : base(typeof(ChampionSkull),null,name,amount,height,x,y)
        {
            _SkullType = type;
        }

        //clone constructor
        public ChampionSkullEntry(ChampionSkullEntry entry) : this(entry.SkullType,entry.Name,entry.Amount,entry.Height,entry.X,entry.Y)
        {
        }

        //generic reader deser constructor
        public ChampionSkullEntry(GenericReader reader) : base(reader)
        {
        }

        //this method performs the addition of a tool to the entry
        public override bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            if (!(item is ChampionSkull))
            {
                return false;
            }

            ChampionSkull skull = (ChampionSkull)item;

            if (Amount + 1 > ItemStore.MaxAmount)
            {
                return false;
            }
            Amount += 1;

            return true;
        }

        public override Item Withdraw(ref int amount,bool makedeed)
        {
            //force it so you can only withdraw 1 at a time
            amount = 1;

            try
            {
                //constructor for all derived classes of BaseRunicTool require the craftresource property specified
                ChampionSkull newskull = (ChampionSkull)Activator.CreateInstance(typeof(ChampionSkull),new object[] { _SkullType });

                //pull that amount out of the store
                _Amount -= amount;

                return (Item)newskull;
            }
            catch
            {
                return null;
            }
        }

        //this generates an item based on the entry to be used to display artwork on the gump
        //TODO: change this to extract the itemid and hue, so it's not so messy?
        public override Item GetModel()
        {
            try
            {
                Object o = Activator.CreateInstance(typeof(ChampionSkull),new object[] { _SkullType });

                return (Item)o;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
        }

        //this handles the check to see if the requested item matches this entry.  This is used when an item store is checking thru
        //all its entries to find one that matches
        public override bool Match(Item item,bool checksubtypes)
        {
            if (!base.Match(item,checksubtypes))
            {
                return false;
            }

            return (((ChampionSkull)item).Type == _SkullType);
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new ChampionSkullEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            writer.Write(0);
            writer.Write((int)_SkullType);
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
                        _SkullType = (ChampionSkullType)reader.ReadInt();
                        break;
                    }
            }
        }//deserialize

        //static, data management methods

    }//class ChampionSkullEntry

    //Ancient Smithy Hammer Tool entry class - handles runic tools
    public class AncientSmithyHammerToolEntry : StoreEntry
    {
        public override int ButtonID { get { return 0x9A8; } }
        public override int ButtonX { get { return 5; } }
        public override int ButtonY { get { return -3; } }

        //reference to the skill bonus
        protected int _Bonus;

        public int Bonus { get { return _Bonus; } }

        //default constructor: set up the bonus, a name, and default 0 quantity
        public AncientSmithyHammerToolEntry(int bonus,string name) : this(bonus,name,0)
        {
        }

        //default constructor: set up the bonus, a name, an amount, and default size/offset of artwork
        public AncientSmithyHammerToolEntry(int bonus,string name,int amount) : this(bonus,name,amount,25,0,0)
        {
        }

        //master constructor: set the bonus, a name, an amount, and size/offset of artwork
        public AncientSmithyHammerToolEntry(int bonus,string name,int amount,int height,int x,int y) : base(typeof(AncientSmithyHammer),null,name,amount,height,x,y)
        {
            _Bonus = bonus;
        }

        //clone constructor
        public AncientSmithyHammerToolEntry(AncientSmithyHammerToolEntry entry) : this(entry.Bonus,entry.Name,entry.Amount,entry.Height,entry.X,entry.Y)
        {
        }

        //generic reader deser constructor
        public AncientSmithyHammerToolEntry(GenericReader reader) : base(reader)
        {
        }

        //this method performs the addition of a tool to the entry
        public override bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            //if it's not an ASH, ignore it
            if (!(item is AncientSmithyHammer))
            {
                return false;
            }

            AncientSmithyHammer tool = (AncientSmithyHammer)item;

            if (Amount + tool.UsesRemaining > ItemStore.MaxAmount)
            {
                return false;
            }
            Amount += tool.UsesRemaining;

            return true;
        }

        public override Item Withdraw(ref int amount,bool makedeed)
        {
            //force it so you cannot withdraw less than 50 uses on a tool
            //this removes the ability to exploit selling 1-use tools to vendors for gold farming
            int towithdraw = Math.Min(Math.Max(50,amount),_Amount);

            //this removes the ability to exploit selling 1-use tools to vendors for gold farming

            try
            {
                //constructor for all derived classes of AncientSmithyHammer require the bonus value
                AncientSmithyHammer tool = (AncientSmithyHammer)Activator.CreateInstance(_Type,new object[] { _Bonus });

                tool.UsesRemaining = towithdraw;

                //pull that amount out of the store
                _Amount -= towithdraw;

                return (Item)tool;
            }
            catch
            {
                return null;
            }
        }

        //this generates an item based on the entry to be used to display artwork on the gump
        //TODO: change this to extract the itemid and hue, so it's not so messy?
        public override Item GetModel()
        {
            try
            {
                //ASH's need the skill bonus specification in their constructor
                Object o = Activator.CreateInstance(Type,new object[] { _Bonus });

                return (Item)o;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
        }

        //this handles the check to see if the requested item matches this entry.  This is used when an item store is checking thru
        //all its entries to find one that matches
        public override bool Match(Item item,bool checksubtypes)
        {
            if (!(item is AncientSmithyHammer))
            {
                return false;
            }

            return (((AncientSmithyHammer)item).Bonus == _Bonus);
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new AncientSmithyHammerToolEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(_Bonus);
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
                        _Bonus = reader.ReadInt();
                        break;
                    }
            }
        }//deserialize
    }//class AncientSmithyHammerToolEntry

    //potion entry class - special store entry that holds potion charges.  Note that 
    public class PotionEntry : StoreEntry
    {
        //store the potion type
        PotionEffect _Effect;

        public override bool RequiresRecipient { get { return true; } }

        public PotionEffect Effect { get { return _Effect; } }

        //default constructor: set up one type, a name, and default 0 quantity
        public PotionEntry(Type type,string name) : this(type,name,0)
        {
        }

        //default constructor: set up one type, a name, an amount, and default size/offset of artwork
        public PotionEntry(Type type,string name,int amount) : this(type,name,amount,25,0,0)
        {
        }

        //master constructor: set up type, a name, an amount, and size/offset of artwork
        public PotionEntry(Type type,string name,int amount,int height,int x,int y) : base(type,new Type[] { typeof(PotionKeg) },name,amount,height,x,y)
        {
            try
            {
                BasePotion potion = (BasePotion)Activator.CreateInstance(type);

                _Effect = potion.PotionEffect;

                potion.Delete();
            }
            catch
            {
            }
        }

        //clone constructor
        public PotionEntry(PotionEntry entry) : this(entry.Type,entry.Name,entry.Amount,entry.Height,entry.X,entry.Y)
        {
        }

        //generic reader deser constructor
        public PotionEntry(GenericReader reader) : base(reader)
        {
        }

        //this method performs the addition of a resource to the entry
        public override bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            int toadd = 0;

            if (item is PotionKeg)
            {
                if (((PotionKeg)item).Type != Effect)
                {
                    return false;
                }

                toadd = ((PotionKeg)item).Held;
            }
            else if (item is BasePotion)
            {
                if (((BasePotion)item).PotionEffect != Effect)
                {
                    return false;
                }

                toadd = item.Amount;
            }
            else
            {
                return false;
            }

            if (Amount + toadd > ItemStore.MaxAmount)
            {
                return false;
            }
            Amount += toadd;

            return true;
        }

        public override void Empty(Item item)
        {
            if (item is BasePotion)
            {
                Bottle bottle = new Bottle();

                BaseStoreKey.StoreAt(item,bottle);

                item.Delete();
            }
            else if (item is PotionKeg)
            {
                ((PotionKeg)item).Held = 0;
            }
        }

        public override Item FindRecipient(Mobile from)
        {
            if (from.Backpack == null)
            {
                return null;
            }

            Item[] kegrecipients = from.Backpack.FindItemsByType(typeof(PotionKeg));

            foreach (PotionKeg recipient in kegrecipients)
            {
                if (recipient.Held == 0 || recipient.Type == _Effect && recipient.Held < 100)
                {
                    return recipient;
                }
            }

            Item[] bottlerecipients = from.Backpack.FindItemsByType(typeof(Bottle));

            if (bottlerecipients.Length > 0)
            {
                return bottlerecipients[0];
            }

            return null;
        }

        //in overridden types, the boolean value passed in tells the withdraw method to force the quantity.  Used for 
        //direct resource withdrawal
        public override Item Withdraw(ref int amount,bool forcequantity)
        {
            if (_Amount == 0)
            {
                return null;
            }

            //cannot spit out more than 100, or more than what's in there already
            int towithdraw = Math.Min(100,Math.Min(_Amount,amount));

            if (towithdraw > 1 && towithdraw < 100)
            {
                //force give them a potion unless they can fill a keg			
                towithdraw = 1;
            }

            //force the quantity back, if the flag was set
            if (forcequantity)
            {
                towithdraw = amount;
            }

            //if you want more than one, then create a potion keg
            if (towithdraw > 1)
            {
                try
                {
                    PotionKeg keg = new PotionKeg();

                    keg.Type = _Effect;
                    keg.Held = towithdraw;

                    _Amount -= towithdraw;

                    return keg;
                }
                catch
                {
                }
            }
            else        //just one potion, so withdraw a bottle
            {
                Item item = null;

                try
                {
                    item = (Item)Activator.CreateInstance(_Type);
                }
                catch
                {
                }

                //pull that amount out of the store
                _Amount -= towithdraw;

                return item;
            }

            return null; ;
        }

        public override Item WithdrawTo(ref int amount,Item destination)
        {
            if (_Amount == 0 || destination == null)
            {
                return null;
            }

            if (destination is Bottle)
            {
                amount = 1;
                Item newpotion = Withdraw(ref amount,false);

                if (newpotion == null)
                {
                    return null;
                }

                BaseStoreKey.StoreAt(destination,newpotion);

                ((Bottle)destination).Consume();

                return newpotion;
            }

            if (destination is PotionKeg)
            {
                PotionKeg keg = (PotionKeg)destination;

                keg.Type = _Effect;

                amount = Math.Min(Math.Min(amount,_Amount),100 - keg.Held);

                keg.Held += amount;
                _Amount -= amount;

                return keg;
            }

            return null;
        }

        //this handles the check to see if the requested item matches this entry.  This is used when an item store is checking thru
        //all its entries to find one that matches
        public override bool Match(Item item,bool checksubtypes)
        {
            if (item is BasePotion)
            {
                if (((BasePotion)item).PotionEffect == _Effect)
                {
                    return true;
                }
            }
            else if (item is PotionKeg)
            {
                if (((PotionKeg)item).Type == _Effect)
                {
                    return true;
                }
            }
            return false;
        }

        //this match method is useful for when an abstract collection of parameters are passed.  It is only defined in 
        //child classes
        public override bool Match(int amount,object[] parameters)
        {
            //if there's nothing in this entry
            if (_Amount < amount)
            {
                return false;
            }

            //if the specified parameter search is invalid, or does not match this potion entry's effect
            if (parameters.Length == 0)
            {
                return false;
            }

            if (!(parameters[0] is PotionEffect[]))
            {
                return false;
            }

            //look down the list of effects provided
            foreach (PotionEffect effect in ((PotionEffect[])parameters[0]))
            {
                //check if there is a match
                if (effect == Effect)
                {
                    return true;
                }
            }

            //otherwise, return false for nothing found
            return false;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new PotionEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            //version
            writer.Write(0);

            //store type by string, and recover it by reflection
            writer.Write((int)_Effect);
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
                        _Effect = (PotionEffect)reader.ReadInt();
                        break;
                    }
            }
        }//deserialize
    }//class PotionEntry

    //Beverage entry class - special store entry that holds drinks like pitchers of water, etc
    public class BeverageEntry : StoreEntry
    {
        public override bool RequiresRecipient { get { return true; } }

        //store the beverage type
        BeverageType _BeverageType;

        public BeverageType BeverageType { get { return _BeverageType; } }

        //default constructor: set up one type, a name, and default 0 quantity
        public BeverageEntry(Type type,BeverageType beveragetype,string name) : this(type,beveragetype,name,0)
        {
        }

        //default constructor: set up one type, a name, an amount, and default size/offset of artwork
        public BeverageEntry(Type type,BeverageType beveragetype,string name,int amount) : this(type,beveragetype,name,amount,25,0,0)
        {
        }

        //master constructor: set up type, a name, an amount, and size/offset of artwork
        public BeverageEntry(Type type,BeverageType beveragetype,string name,int amount,int height,int x,int y) : base(type,null,name,amount,height,x,y)
        {
            _BeverageType = beveragetype;
        }

        //clone constructor
        public BeverageEntry(BeverageEntry entry) : this(entry.Type,entry.BeverageType,entry.Name,entry.Amount,entry.Height,entry.X,entry.Y)
        {
        }

        //generic reader deser constructor
        public BeverageEntry(GenericReader reader) : base(reader)
        {
        }

        //this method performs the addition of a resource to the entry
        public override bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            if (!(item is BaseBeverage))
            {
                return false;
            }

            BaseBeverage beverage = (BaseBeverage)item;

            //Don't add poisoned food.  This blocks the exploit of being able to unpoison a drink by adding/removing from keys
            if (beverage.Poison != null)
            {
                //TODO: create errormessage system to pass back to keys
                return false;
            }

            if (beverage.Content != _BeverageType)
            {
                return false;
            }

            int toadd = beverage.Quantity;

            if (toadd == 0 || Amount + toadd > ItemStore.MaxAmount)
            {
                return false;
            }
            Amount += toadd;
            beverage.Quantity -= toadd;

            return true;
        }

        public override void Empty(Item item)
        {
            if (item is BaseBeverage)
            {
                ((BaseBeverage)item).Quantity = 0;
            }
            else
            {
                item.Delete();
            }
        }

        public override Item FindRecipient(Mobile from)
        {
            if (from.Backpack == null)
            {
                return null;
            }

            Item[] recipients = from.Backpack.FindItemsByType(typeof(BaseBeverage));

            foreach (BaseBeverage recipient in recipients)
            {
                if (recipient.Quantity == 0 || recipient.Content == _BeverageType && !recipient.IsFull && recipient.Poison == null)
                {
                    return recipient;
                }
            }

            return null;
        }

        //in overridden types, the boolean value passed in tells the withdraw method to force the quantity.  Used for 
        //direct resource withdrawal
        public override Item Withdraw(ref int amount,bool forcequantity)
        {
            if (_Amount == 0 || amount == 0)
            {
                return null;
            }

            try
            {
                BaseBeverage beverage = (BaseBeverage)Activator.CreateInstance(_Type);

                beverage.Content = _BeverageType;

                //force them to withdraw as much as possible, so they can't exploit mass prduction of items				
                int towithdraw = Math.Min(_Amount,beverage.MaxQuantity);

                if (forcequantity)
                {
                    //this is important for gardening, as we only need to withdraw 1 unit
                    towithdraw = amount;
                }

                beverage.Quantity = towithdraw;

                _Amount -= towithdraw;

                return beverage;
            }
            catch
            {
            }

            return null; ;
        }

        public override Item WithdrawTo(ref int amount,Item destination)
        {
            if (_Amount == 0 || destination == null)
            {
                return null;
            }

            if (destination is BaseBeverage)
            {
                BaseBeverage beverage = (BaseBeverage)destination;

                beverage.Content = _BeverageType;

                amount = Math.Min(Math.Min(amount,_Amount),beverage.MaxQuantity - beverage.Quantity);

                beverage.Quantity += amount;
                _Amount -= amount;

                return beverage;
            }

            return null;
        }

        //this handles the check to see if the requested item matches this entry.  This is used when an item store is checking thru
        //all its entries to find one that matches
        public override bool Match(Item item,bool checksubtypes)
        {
            if (item is BaseBeverage && ((BaseBeverage)item).Content == _BeverageType)
            {
                return true;
            }

            return false;
        }

        //this match method is useful for when an abstract collection of parameters are passed.  It is only defined in 
        //child classes
        public override bool Match(int amount,object[] parameters)
        {
            //if there's nothing in this entry
            if (_Amount < amount)
            {
                return false;
            }

            //if the specified parameter search is invalid, or does not match this potion entry's effect
            if (parameters.Length == 0)
            {
                return false;
            }

            if (!(parameters[0] is BeverageType))
            {
                return false;
            }

            //check if there is a match
            if (((BeverageType)parameters[0]) == BeverageType)
            {
                return true;
            }

            //otherwise, return false for nothing found
            return false;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new BeverageEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            //version
            writer.Write(0);

            //store type by string, and recover it by reflection
            writer.Write((int)_BeverageType);
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
                        _BeverageType = (BeverageType)reader.ReadInt();
                        break;
                    }
            }
        }//deserialize
    }//class BeverageEntry

    //Instrument entry class - special store entry that holds instruments of defined type(s), and only considers the instrument quality
    // and the number of uses remaining.  This entry will reject any instruments with slayer properties.  To store instruments with
    // slayer properties, look into the InstrumentListEntry
    public class InstrumentEntry : StoreEntry
    {
        //instrument quality
        protected ItemQuality _Quality;

        public ItemQuality Quality { get { return _Quality; } }

        //default constructor: set up one type, a quality, a name, and default 0 quantity
        public InstrumentEntry(Type type, ItemQuality quality, string name)
            : this(type, null, quality, name, 0)
        {
        }

        //default constructor: set up several types, a quality, a name, and default 0 quantity
        public InstrumentEntry(Type type, Type[] types, ItemQuality quality, string name)
            : this(type, types, quality, name, 0)
        {
        }

        //default constructor: set up one type, a quality, a name, an amount, and default size/offset of artwork
        public InstrumentEntry(Type type, ItemQuality quality, string name, int amount)
            : this(type, null, quality, name, amount, 25, 0, 0)
        {
        }

        //default constructor: set up types, a quality, a name, an amount, and default size/offset of artwork
        public InstrumentEntry(Type type, Type[] types, ItemQuality quality, string name, int amount)
            : this(type, types, quality, name, amount, 25, 0, 0)
        {
        }

        //default constructor, set up a type, quality, name, amount, size/offset of artwork
        public InstrumentEntry(Type type, ItemQuality quality, string name, int amount, int height, int x, int y)
            : this(type, null, quality, name, amount, height, x, y)
        {
        }

        //master constructor: set up types, quality, a name, an amount, and size/offset of artwork
        public InstrumentEntry(Type type, Type[] types, ItemQuality quality, string name, int amount, int height, int x, int y)
            : base(type, types, name, amount, height, x, y)
        {
            _Quality = quality;
        }

        //clone constructor
        public InstrumentEntry(InstrumentEntry entry) : this(entry.Type,entry.AlternateTypes,entry.Quality,entry.Name,entry.Amount,entry.Height,entry.X,entry.Y)
        {
        }

        //generic reader deser constructor
        public InstrumentEntry(GenericReader reader) : base(reader)
        {
        }

        //this method performs the addition of a resource to the entry
        public override bool Add(Item item)
        {
            //check configuration-specific conditions for adding the item
            if (!CheckExtras.Add(item))
            {
                return false;
            }

            if (!(item is BaseInstrument))
            {
                return false;
            }

            BaseInstrument instrument = (BaseInstrument)item;

            //only add the same quality as specified
            if (instrument.Quality != _Quality)
            {
                return false;
            }

            //don't add insured/blessed instruments
            if (instrument.LootType != LootType.Regular || instrument.Insured)
            {
                return false;
            }

            //dont' add slayer instruments to this
            if (instrument.Slayer != SlayerName.None || instrument.Slayer2 != SlayerName.None)
            {
                return false;
            }

            int toadd = instrument.UsesRemaining;

            if (toadd == 0 || Amount + toadd > ItemStore.MaxAmount)
            {
                return false;
            }
            Amount += toadd;

            return true;
        }

        //in overridden types, the boolean value passed in tells the withdraw method to force the quantity.  Used for 
        //direct resource withdrawal
        public override Item Withdraw(ref int amount,bool forcequantity)
        {
            if (_Amount == 0 || amount == 0)
            {
                return null;
            }

            try
            {
                BaseInstrument instrument = (BaseInstrument)Activator.CreateInstance(_Type);

                instrument.Quality = _Quality;

                //force them to withdraw no less than 350 uses
                int towithdraw = Math.Min(_Amount,Math.Max(amount,350));

                if (forcequantity)
                {
                    //this may not be needed
                    towithdraw = amount;
                }

                instrument.UsesRemaining = towithdraw;

                _Amount -= towithdraw;

                return instrument;
            }
            catch
            {
            }

            return null; ;
        }

        //this handles the check to see if the requested item matches this entry.  This is used when an item store is checking thru
        //all its entries to find one that matches
        public override bool Match(Item item,bool checksubtypes)
        {
            if (base.Match(item,checksubtypes) && item is BaseInstrument && ((BaseInstrument)item).Quality == _Quality)
            {
                return true;
            }

            return false;
        }

        //this match method is useful for when an abstract collection of parameters are passed.  It is only defined in 
        //child classes
        public override bool Match(int amount,object[] parameters)
        {
            //this is unused for this type.

            return false;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override StoreEntry Clone()
        {
            return new InstrumentEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            //handle base StoreEntry serialization first
            base.Serialize(writer);

            //version
            writer.Write(0);

            //store type by string, and recover it by reflection
            writer.Write((int)_Quality);
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
                        _Quality = (ItemQuality)reader.ReadInt();
                        break;
                    }
            }
        }//deserialize
    }//class InstrumentEntry
}