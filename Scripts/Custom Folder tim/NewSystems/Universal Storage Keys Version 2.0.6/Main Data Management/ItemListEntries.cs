using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Engines.Plants;
using Solaris.CliLocHandler;

namespace Solaris.ItemStore
{
    //this is the base class for item entries stored in an item list
    public class ItemListEntry : IComparable
    {
        //reference back to the ListEntry objects which holds this particular entry
        protected ListEntry _ListEntry;

        public virtual int GumpWidth { get { return 400; } }

        //basic storage stuff needed to store items within this item list entry
        protected Type _Type;

        //insured/loot properties of item
        protected LootType _LootType;
        protected bool _Insured;

        protected string _Name;

        //used for display purposes, to help differentiate between special entries
        protected int _Hue;

        //gump specific positioning of data is stored in this fancy ItemListEntryColumn for easy access from above
        protected List<ItemListEntryColumn> _Columns;

        public Type Type { get { return _Type; } }
        public string Name { get { return _Name; } }
        public int Hue { get { return _Hue; } }
        public LootType LootType { get { return _LootType; } }
        public bool Insured { get { return _Insured; } }

        //this is the (overridable) public accessor and generator for the column formatting for the gump
        public virtual List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    _Columns = new List<ItemListEntryColumn>();

                    //here's where the columns are defined
                    _Columns.Add(new ItemListEntryColumn(0,"Name",_Name));
                }

                return _Columns;
            }
        }

        //this is used for the sorting, determine which column in Columns to sort by
        public static int SortIndex;

        //set to either -1 or 1
        public static int SortOrder;

        public int CompareTo(object obj)
        {
            if (!(obj is ItemListEntry))
            {
                return 0;   //cannot sort
            }

            ItemListEntry entry = (ItemListEntry)obj;

            //if the sort index is invalid
            if (Columns.Count <= SortIndex || SortIndex == -1)
            {
                return 0;
            }

            //TODO: special case:  sort by hue if SortIndex = -1

            //null text can't be sorted
            if (this.Columns[SortIndex].Text == null || entry.Columns[SortIndex].Text == null)
            {
                return 0;
            }

            return this.Columns[SortIndex].Text.CompareTo(entry.Columns[SortIndex].Text) * SortOrder;
        }

        public ItemListEntry(Item item) : this(item,(item.Name == null ? item.GetType().Name : item.Name))
        {
        }

        //constructor with default listing hue
        public ItemListEntry(Item item,string name) : this(item,name,1153)
        {
        }

        //master constructor
        public ItemListEntry(Item item,string name,int hue)
        {
            _Type = item.GetType();
            _Name = name;
            _Hue = hue;
            _LootType = item.LootType;
            _Insured = item.Insured;
        }

        //clone constructor
        public ItemListEntry(ItemListEntry entry)
        {
            _Type = entry.Type;
            _Name = entry.Name;
            _Hue = entry.Hue;
            _LootType = entry.LootType;
            _Insured = entry.Insured;
        }

        //world load constructor
        public ItemListEntry(GenericReader reader)
        {
            Deserialize(reader);
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public virtual Item GenerateItem()
        {
            Item item = (Item)Activator.CreateInstance(_Type);

            item.Name = _Name;
            item.LootType = _LootType;
            item.Insured = _Insured;

            return item;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public virtual bool AllGood(Item item)
        {
            return (item != null);
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public virtual ItemListEntry Clone()
        {
            return new ItemListEntry(this);
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(2);

            writer.Write((int)_LootType);
            writer.Write(_Insured);

            writer.Write(_Hue);

            writer.Write(_Type.Name);
            writer.Write(_Name);
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        _LootType = (LootType)reader.ReadInt();
                        _Insured = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        _Hue = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                default:
                    {
                        _Type = ScriptCompiler.FindTypeByName(reader.ReadString());
                        _Name = reader.ReadString();
                        break;
                    }
            }

            if (version == 0)
            {
                //default hue from previous version;
                _Hue = 1153;
            }
        }

        //this is used to remove namespace text from a type name
        public static string TrimNamespace(string typename)
        {
            string trimname = typename;

            while (trimname.IndexOf(".") > -1)
            {
                trimname = trimname.Substring(trimname.IndexOf("."),trimname.Length - trimname.IndexOf("."));
            }
            return trimname;
        }
    }//class ItemListEntry

    public class TreasureMapListEntry : ItemListEntry
    {
        public override int GumpWidth { get { return 400; } }

        //treasure map specific data
        protected Point2D _ChestLocation;
        protected Map _ChestMap;
        protected Mobile _Decoder;
        protected int _Level;
        protected Rectangle2D _Bounds;

        public override List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    List<ItemListEntryColumn> columns = base.Columns;

                    //add in specifics for this list type here
                    columns.Add(new ItemListEntryColumn(150,"Map",_ChestMap.ToString()));
                    columns.Add(new ItemListEntryColumn(240,"Decoded?",(_Decoder == null ? "no" : "yes")));

                    return columns;
                }

                return _Columns;
            }
        }

        //public accessors
        public Point2D ChestLocation { get { return _ChestLocation; } }
        public Map ChestMap { get { return _ChestMap; } }
        public Mobile Decoder { get { return _Decoder; } }
        public int Level { get { return _Level; } }
        public Rectangle2D Bounds { get { return _Bounds; } }

        //master constructor
        public TreasureMapListEntry(Item item) : base(item)
        {
            TreasureMap map = (TreasureMap)item;

            //fill in the name based on the level
            //TODO: find a better way to do this
            switch (map.Level)
            {
                case 0:
                    {
                        _Name = "Youthful";
                        break;
                    }
                case 1:
                    {
                        _Name = "Plainly";
                        break;
                    }
                case 2:
                    {
                        _Name = "Expertly";
                        break;
                    }
                case 3:
                    {
                        _Name = "Adeptly";
                        break;
                    }
                case 4:
                    {
                        _Name = "Cleverly";
                        break;
                    }
                case 5:
                    {
                        _Name = "Deviously";
                        break;
                    }
                case 6:
                    {
                        _Name = "Ingeniously";
                        break;
                    }
            }

            _ChestLocation = map.ChestLocation;
            //_ChestMap = map.ChestMap;
            _Decoder = map.Decoder;
            _Level = map.Level;
            _Bounds = map.Bounds;
        }

        //world load constructor
        public TreasureMapListEntry(GenericReader reader) : base(reader)
        {
        }

        //clone constructor
        public TreasureMapListEntry(TreasureMapListEntry entry) : base(entry)
        {
            _ChestLocation = entry.ChestLocation;
            _ChestMap = entry.ChestMap;
            _Decoder = entry.Decoder;
            _Level = entry.Level;
            _Bounds = entry.Bounds;
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public override Item GenerateItem()
        {
            TreasureMap map = (TreasureMap)Activator.CreateInstance(_Type,new object[] { _Level,_ChestMap });

            map.ChestLocation = _ChestLocation;

            //this is done to ensure the map gump displays properly
            map.Bounds = _Bounds;

            map.Decoder = _Decoder;

            //the map will have some bogus pin on it when it's first withdrawn.  First, remove the bogus pin
            map.ClearPins();

            //then add the proper pin
            map.AddWorldPin(_ChestLocation.X,_ChestLocation.Y);

            map.LootType = _LootType;
            map.Insured = _Insured;

            return map;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public override bool AllGood(Item item)
        {
            if (!base.AllGood(item))
            {
                return false;
            }

            if (!(item is TreasureMap))
            {
                return false;
            }

            TreasureMap map = (TreasureMap)item;

            //can't add completed maps
            return (map.CompletedBy == null);
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override ItemListEntry Clone()
        {
            return new TreasureMapListEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(_ChestLocation);
            writer.Write(_ChestMap);
            writer.Write(_Decoder);
            writer.Write(_Level);
            writer.Write(_Bounds);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _ChestLocation = reader.ReadPoint2D();
            _ChestMap = reader.ReadMap();
            _Decoder = reader.ReadMobile();
            _Level = reader.ReadInt();
            _Bounds = reader.ReadRect2D();
        }
    }

    public class SOSListEntry : ItemListEntry
    {
        public override int GumpWidth { get { return 450; } }

        //SOS-specific data

        protected int _Level;
        protected int _MessageIndex;
        protected Map _TargetMap;
        protected Point3D _TargetLocation;

        public override List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    List<ItemListEntryColumn> columns = base.Columns;

                    //add in specifics for this list type here
                    columns.Add(new ItemListEntryColumn(150,"Map",_TargetMap.ToString()));
                    columns.Add(new ItemListEntryColumn(240,"Location",GetCoordsText(_TargetLocation,_TargetMap)));

                    return columns;
                }

                return _Columns;
            }
        }

        //public accessors
        public int Level { get { return _Level; } }
        public int MessageIndex { get { return _MessageIndex; } }
        public Map TargetMap { get { return _TargetMap; } }
        public Point3D TargetLocation { get { return _TargetLocation; } }

        //this is copied right from the SOS gump code, used to display the location in sextant formatted coordinates
        public static string GetCoordsText(Point3D point,Map map)
        {
            int xLong = 0, yLat = 0;
            int xMins = 0, yMins = 0;
            bool xEast = false, ySouth = false;
            string fmt;

            if (Sextant.Format(point,map,ref xLong,ref yLat,ref xMins,ref yMins,ref xEast,ref ySouth))
                fmt = String.Format("{0}°{1}'{2},{3}°{4}'{5}",yLat,yMins,ySouth ? "S" : "N",xLong,xMins,xEast ? "E" : "W");
            else
                fmt = "?????";

            return fmt;
        }

        //master constructor
        public SOSListEntry(Item item) : base(item)
        {
            SOS sos = (SOS)item;

            if (sos.IsAncient)
            {
                _Name = "Ancient SOS";
            }
            else
            {
                _Name = "SOS";
            }

            _Level = sos.Level;
            _MessageIndex = sos.MessageIndex;
            _TargetMap = sos.TargetMap;
            _TargetLocation = sos.TargetLocation;
        }

        //world load constructor
        public SOSListEntry(GenericReader reader) : base(reader)
        {
        }

        //clone constructor
        public SOSListEntry(SOSListEntry entry) : base(entry)
        {
            _Level = entry.Level;
            _MessageIndex = entry.MessageIndex;
            _TargetMap = entry.TargetMap;
            _TargetLocation = entry.TargetLocation;
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public override Item GenerateItem()
        {
            SOS sos = new SOS(_TargetMap,_Level);

            sos.TargetLocation = _TargetLocation;
            sos.MessageIndex = _MessageIndex;

            sos.LootType = _LootType;
            sos.Insured = _Insured;

            return sos;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public override bool AllGood(Item item)
        {
            if (!base.AllGood(item))
            {
                return false;
            }

            if (!(item is SOS))
            {
                return false;
            }

            return (true);
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override ItemListEntry Clone()
        {
            return new SOSListEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(_Level);
            writer.Write(_MessageIndex);
            writer.Write(_TargetMap);
            writer.Write(_TargetLocation);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Level = reader.ReadInt();
            _MessageIndex = reader.ReadInt();
            _TargetMap = reader.ReadMap();
            _TargetLocation = reader.ReadPoint3D();
        }
    }

    public class SpecialFishingNetListEntry : ItemListEntry
    {
        public override int GumpWidth { get { return 400; } }

        public override List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    List<ItemListEntryColumn> columns = base.Columns;

                    //add in specifics for this list type here
                    //no special listing for fishing nets

                    return columns;
                }

                return _Columns;
            }
        }

        //master constructor, use the net hue to hue the entry
        public SpecialFishingNetListEntry(Item item) : base(item,item.GetType().Name,item.Hue - 1)
        {
        }

        //world load constructor
        public SpecialFishingNetListEntry(GenericReader reader) : base(reader)
        {
        }

        //clone constructor
        public SpecialFishingNetListEntry(SpecialFishingNetListEntry entry) : base(entry)
        {
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public override Item GenerateItem()
        {
            //this allows for derived classes of fishing net to fit into this entry.
            SpecialFishingNet net = (SpecialFishingNet)Activator.CreateInstance(_Type);

            //note: gump hue display is shifted by 1 for some crazy reason
            net.Hue = _Hue + 1;

            net.LootType = _LootType;
            net.Insured = _Insured;

            return net;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public override bool AllGood(Item item)
        {
            if (!base.AllGood(item))
            {
                return false;
            }

            //TODO: move this to base class, since the _Type is specified in ListEntry?
            if (!(item is SpecialFishingNet))
            {
                return false;
            }

            return true;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override ItemListEntry Clone()
        {
            return new SpecialFishingNetListEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class InstrumentListEntry : ItemListEntry
    {
        public override int GumpWidth { get { return 750; } }

        protected ItemQuality _Quality;
        protected int _UsesRemaining;
        protected SlayerName _Slayer;
        protected SlayerName _Slayer2;
        protected Mobile _Crafter;

        public override List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    List<ItemListEntryColumn> columns = base.Columns;

                    //add in specifics for this list type here
                    _Columns.Add(new ItemListEntryColumn(100,"Quality",_Quality.ToString()));
                    _Columns.Add(new ItemListEntryColumn(200,"Uses",_UsesRemaining.ToString()));
                    _Columns.Add(new ItemListEntryColumn(300,"Slayer 1",_Slayer.ToString()));
                    _Columns.Add(new ItemListEntryColumn(450,"Slayer 2",_Slayer2.ToString()));
                    _Columns.Add(new ItemListEntryColumn(600,"Crafter",_Crafter != null ? _Crafter.Name : "-none-"));

                    return columns;
                }

                return _Columns;
            }
        }

        //public accessors
        public ItemQuality Quality { get { return _Quality; } }
        public int UsesRemaining { get { return _UsesRemaining; } }
        public SlayerName Slayer { get { return _Slayer; } }
        public SlayerName Slayer2 { get { return _Slayer2; } }
        public Mobile Crafter { get { return _Crafter; } }

        //master constructor, use the net hue to hue the entry
        public InstrumentListEntry(Item item) : base(item)
        {
            _Name = TrimNamespace(item.GetType().Name);

            BaseInstrument instrument = (BaseInstrument)item;

            _Quality = instrument.Quality;
            _UsesRemaining = instrument.UsesRemaining;
            _Slayer = instrument.Slayer;
            _Slayer2 = instrument.Slayer2;
            _Crafter = instrument.Crafter;
        }

        //world load constructor
        public InstrumentListEntry(GenericReader reader) : base(reader)
        {
        }

        //clone constructor
        public InstrumentListEntry(InstrumentListEntry entry) : base(entry)
        {
            _Quality = entry.Quality;
            _UsesRemaining = entry.UsesRemaining;
            _Slayer = entry.Slayer;
            _Slayer2 = entry.Slayer2;
            _Crafter = entry.Crafter;
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public override Item GenerateItem()
        {
            BaseInstrument instrument = (BaseInstrument)Activator.CreateInstance(_Type);

            instrument.Quality = _Quality;
            instrument.UsesRemaining = _UsesRemaining;
            instrument.Slayer = _Slayer;
            instrument.Slayer2 = _Slayer2;
            instrument.Crafter = _Crafter;

            instrument.LootType = _LootType;
            instrument.Insured = _Insured;

            return instrument;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public override bool AllGood(Item item)
        {
            if (!base.AllGood(item))
            {
                return false;
            }

            //TODO: move this to base class, since the _Type is specified in ListEntry?
            if (!(item is BaseInstrument))
            {
                return false;
            }

            return true;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override ItemListEntry Clone()
        {
            return new InstrumentListEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write((int)_Quality);
            writer.Write(_UsesRemaining);
            writer.Write((int)_Slayer);
            writer.Write((int)_Slayer2);
            writer.Write(_Crafter);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Quality = (ItemQuality)reader.ReadInt();
            _UsesRemaining = reader.ReadInt();
            _Slayer = (SlayerName)reader.ReadInt();
            _Slayer2 = (SlayerName)reader.ReadInt();
            _Crafter = reader.ReadMobile();
        }
    }

    public class SeedListEntry : ItemListEntry
    {
        public override int GumpWidth { get { return 500; } }

        protected PlantType _PlantType;
        protected PlantHue _PlantHue;
        protected bool _ShowType;

        public override List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    List<ItemListEntryColumn> columns = base.Columns;

                    //add in specifics for this list type here

                    //TODO: find some way of using the PlantHueInfo cliloc values
                    _Columns.Add(new ItemListEntryColumn(250,"Color",_PlantHue.ToString()));

                    PlantHueInfo hueinfo = PlantHueInfo.GetInfo(_PlantHue);
                    _Columns.Add(new ItemListEntryColumn(350,"Bright?",hueinfo.IsBright() ? "yes" : "no"));

                    return columns;
                }

                return _Columns;
            }
        }

        //public accessors
        public PlantType PlantType { get { return _PlantType; } }
        public PlantHue PlantHue { get { return _PlantHue; } }
        public bool ShowType { get { return _ShowType; } }

        //master constructor
        public SeedListEntry(Item item) : base(item)
        {
            Seed seed = (Seed)item;

            _PlantType = seed.PlantType;
            _PlantHue = seed.PlantHue;
            _ShowType = seed.ShowType;

            //TODO: find a way to using the PlantTypeInfo cliloc values
            PlantHueInfo hueInfo = PlantHueInfo.GetInfo(seed.PlantHue);
            PlantTypeInfo typeInfo = PlantTypeInfo.GetInfo(seed.PlantType);

            _Name = CliLoc.LocToString(hueInfo.IsBright() ? 1061918 : 1061917,String.Concat("#",hueInfo.Name.ToString(),"\t#",typeInfo.Name.ToString())); // [bright] ~1_COLOR~ ~2_TYPE~ seed
                                                                                                                                                          //_PlantType.ToString();
        }

        //world load constructor
        public SeedListEntry(GenericReader reader) : base(reader)
        {
        }

        //clone constructor
        public SeedListEntry(SeedListEntry entry) : base(entry)
        {
            _PlantType = entry.PlantType;
            _PlantHue = entry.PlantHue;
            _ShowType = entry.ShowType;
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public override Item GenerateItem()
        {
            //awesome constructor!
            Seed seed = new Seed(_PlantType,_PlantHue,_ShowType);

            seed.LootType = _LootType;
            seed.Insured = _Insured;

            return seed;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public override bool AllGood(Item item)
        {
            if (!base.AllGood(item))
            {
                return false;
            }

            //TODO: move this to base class, since the _Type is specified in ListEntry?
            if (!(item is Seed))
            {
                return false;
            }

            return true;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override ItemListEntry Clone()
        {
            return new SeedListEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write((int)_PlantType);
            writer.Write((int)_PlantHue);
            writer.Write(_ShowType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _PlantType = (PlantType)reader.ReadInt();
            _PlantHue = (PlantHue)reader.ReadInt();
            _ShowType = reader.ReadBool();
        }
    }

    public class RepairDeedListEntry : ItemListEntry
    {
        protected RepairDeed.RepairSkillType _RepairSkillType;
        protected double _Level;
        protected Mobile _Crafter;

        public override List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    List<ItemListEntryColumn> columns = base.Columns;

                    //add in specifics for this list type here

                    _Columns.Add(new ItemListEntryColumn(150,"Level",_Level.ToString()));

                    _Columns.Add(new ItemListEntryColumn(250,"Crafter",_Crafter == null ? "-null-" : _Crafter.Name));

                    return columns;
                }

                return _Columns;
            }
        }

        //public accessors
        public RepairDeed.RepairSkillType RepairSkillType { get { return _RepairSkillType; } }
        public double Level { get { return _Level; } }
        public Mobile Crafter { get { return _Crafter; } }

        //master constructor
        public RepairDeedListEntry(Item item) : base(item)
        {
            RepairDeed deed = (RepairDeed)item;

            _Name = deed.RepairSkill.ToString();
            _RepairSkillType = deed.RepairSkill;
            _Level = deed.SkillLevel;
            _Crafter = deed.Crafter;
        }

        //world load constructor
        public RepairDeedListEntry(GenericReader reader) : base(reader)
        {
        }

        //clone constructor
        public RepairDeedListEntry(RepairDeedListEntry entry) : base(entry)
        {
            _RepairSkillType = entry.RepairSkillType;
            _Level = entry.Level;
            _Crafter = entry.Crafter;
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public override Item GenerateItem()
        {
            //awesome constructor!
            RepairDeed deed = new RepairDeed(_RepairSkillType,_Level,_Crafter);

            deed.LootType = _LootType;
            deed.Insured = _Insured;

            return deed;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public override bool AllGood(Item item)
        {
            if (!base.AllGood(item))
            {
                return false;
            }

            //TODO: move this to base class, since the _Type is specified in ListEntry?
            if (!(item is RepairDeed))
            {
                return false;
            }

            return true;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override ItemListEntry Clone()
        {
            return new RepairDeedListEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write((int)_RepairSkillType);
            writer.Write(_Level);
            writer.Write(_Crafter);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _RepairSkillType = (RepairDeed.RepairSkillType)reader.ReadInt();
            _Level = reader.ReadDouble();
            _Crafter = reader.ReadMobile();
        }
    }

    //stores addon deeds.   WARNING: what about addon deeds that need some constructor data?
    public class BaseAddonDeedListEntry : ItemListEntry
    {
        public override int GumpWidth { get { return 400; } }

        public override List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    List<ItemListEntryColumn> columns = base.Columns;

                    //add in specifics for this list type here

                    return columns;
                }

                return _Columns;
            }
        }

        //master constructor, use the addon deed class name, and hue
        public BaseAddonDeedListEntry(Item item) : base(item,item.GetType().Name,item.Hue - 1)
        {
        }

        //world load constructor
        public BaseAddonDeedListEntry(GenericReader reader) : base(reader)
        {
        }

        //clone constructor
        public BaseAddonDeedListEntry(BaseAddonDeedListEntry entry) : base(entry)
        {
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public override Item GenerateItem()
        {
            //this allows for inherited classes of addon deeds to fit into this entry.
            //WARNING:  what if the addon deed carries more info, or has a constructor that requires parameters?
            BaseAddonDeed deed = (BaseAddonDeed)Activator.CreateInstance(_Type);

            //note: gump hue display is shifted by 1 for some crazy reason
            deed.Hue = _Hue + 1;

            deed.LootType = _LootType;
            deed.Insured = _Insured;

            return deed;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public override bool AllGood(Item item)
        {
            if (!base.AllGood(item))
            {
                return false;
            }

            //TODO: move this to base class, since the _Type is specified in ListEntry?
            if (!(item is BaseAddonDeed))
            {
                return false;
            }

            //test if the deed being added can be constructed with a zero parameter constructor
            try
            {
                BaseAddonDeed deed = (BaseAddonDeed)Activator.CreateInstance(item.GetType());

                //if it constructs fine, then delete it
                deed.Delete();
            }
            catch
            {
                //this catch hits if the deed does not have a zero parameter constructor, and would have otherwise crashed the shard
                return false;
            }

            return true;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override ItemListEntry Clone()
        {
            return new BaseAddonDeedListEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    //stores power scrolls
    public class PowerScrollListEntry : ItemListEntry
    {
        public override int GumpWidth { get { return 400; } }

        protected SkillName _Skill;
        protected double _Value;

        public SkillName Skill { get { return _Skill; } }
        public double Value { get { return _Value; } }

        public override List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    List<ItemListEntryColumn> columns = base.Columns;

                    _Columns.Add(new ItemListEntryColumn(250,"Value",_Value.ToString()));

                    return columns;
                }

                return _Columns;
            }
        }

        //master constructor, use the addon deed class name, and hue
        public PowerScrollListEntry(Item item) : base(item,((PowerScroll)item).Skill.ToString(),item.Hue - 1)
        {
            PowerScroll scroll = (PowerScroll)item;

            _Skill = scroll.Skill;
            _Value = scroll.Value;
        }

        //world load constructor
        public PowerScrollListEntry(GenericReader reader) : base(reader)
        {
        }

        //clone constructor
        public PowerScrollListEntry(PowerScrollListEntry entry) : base(entry)
        {
            _Skill = entry.Skill;
            _Value = entry.Value;
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public override Item GenerateItem()
        {
            //this allows for inherited classes of addon deeds to fit into this entry.
            PowerScroll scroll = (PowerScroll)Activator.CreateInstance(_Type,new object[] { _Skill,_Value });

            return scroll;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public override bool AllGood(Item item)
        {
            if (!base.AllGood(item))
            {
                return false;
            }

            //TODO: move this to base class, since the _Type is specified in ListEntry?
            if (!(item is PowerScroll))
            {
                return false;
            }

            return true;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override ItemListEntry Clone()
        {
            return new PowerScrollListEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write((int)_Skill);
            writer.Write(_Value);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Skill = (SkillName)reader.ReadInt();
            _Value = reader.ReadDouble();
        }
    }

    //stores stat scrolls
    public class StatCapScrollListEntry : ItemListEntry
    {
        public override int GumpWidth { get { return 400; } }

        protected double _Value;
        public double Value { get { return _Value; } }

        public override List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    List<ItemListEntryColumn> columns = base.Columns;

                    _Columns.Add(new ItemListEntryColumn(250,"Value",_Value.ToString()));

                    return columns;
                }

                return _Columns;
            }
        }

        //master constructor, use the addon deed class name, and hue
        public StatCapScrollListEntry(Item item) : base(item,"Scroll of Power",item.Hue - 1)
        {
            _Value = ((StatCapScroll)item).Value;
        }

        //world load constructor
        public StatCapScrollListEntry(GenericReader reader) : base(reader)
        {
        }

        //clone constructor
        public StatCapScrollListEntry(StatCapScrollListEntry entry) : base(entry)
        {
            _Value = entry.Value;
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public override Item GenerateItem()
        {
            //this allows for inherited classes of addon deeds to fit into this entry.
            StatCapScroll scroll = (StatCapScroll)Activator.CreateInstance(_Type,new object[] { _Value });

            return scroll;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public override bool AllGood(Item item)
        {
            if (!base.AllGood(item))
            {
                return false;
            }

            //TODO: move this to base class, since the _Type is specified in ListEntry?
            if (!(item is StatCapScroll))
            {
                return false;
            }

            return true;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override ItemListEntry Clone()
        {
            return new StatCapScrollListEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(_Value);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Value = reader.ReadDouble();
        }
    }

    //this data structure is useful when arranging gump display columns
    public class ItemListEntryColumn
    {
        int _X;
        string _Header;     //header listed at the top of the column
        string _Text;       //the text for this particular entry

        public int X { get { return _X; } }
        public int Width;

        public string Header { get { return _Header; } }
        public string Text { get { return _Text; } }

        public ItemListEntryColumn(int x,string header,string text)
        {
            _X = x;
            _Header = header;
            _Text = text;
        }
    }
}