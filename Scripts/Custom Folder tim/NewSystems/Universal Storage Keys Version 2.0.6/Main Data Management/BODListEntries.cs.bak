using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Engines.Plants;
using Server.Engines.BulkOrders;
using Solaris.CliLocHandler;

namespace Solaris.ItemStore
{
    //the base class for storing bulk order deeds
    public class SmallBODListEntry : ItemListEntry
    {
        public override int GumpWidth { get { return 650; } }

        //generic bod-specific data
        protected int _AmountCur, _AmountMax;
        protected Type _BODFillType;                //the item to go into the BOD, different from the BOD type
        protected int _Number;
        protected int _Graphic;
        protected bool _RequireExceptional;
        protected BulkMaterialType _Material;

        public override List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    //NOTE: the default name is overridden here completely
                    _Columns = new List<ItemListEntryColumn>();

                    //add in specifics for this list type here
                    _Columns.Add(new ItemListEntryColumn(0,"Item Type",CliLoc.GetName(_BODFillType)));
                    _Columns.Add(new ItemListEntryColumn(150,"Amount",_AmountCur.ToString() + "/" + _AmountMax.ToString()));
                    _Columns.Add(new ItemListEntryColumn(250,"Exceptional?",(_RequireExceptional ? "yes" : "no")));
                    _Columns.Add(new ItemListEntryColumn(350,"Resource",Enum.GetName(typeof(BulkMaterialType),(int)_Material)));
                }

                return _Columns;
            }
        }

        //public accessors
        public int AmountCur { get { return _AmountCur; } }
        public int AmountMax { get { return _AmountMax; } }
        public Type BODFillType { get { return _BODFillType; } }
        public int Number { get { return _Number; } }
        public int Graphic { get { return _Graphic; } }
        public bool RequireExceptional { get { return _RequireExceptional; } }
        public BulkMaterialType Material { get { return _Material; } }

        //master constructor
        public SmallBODListEntry(Item item) : base(item)
        {
            SmallBOD bod = (SmallBOD)item;

            _AmountCur = bod.AmountCur;
            _AmountMax = bod.AmountMax;
            _BODFillType = bod.Type;
            _Number = bod.Number;
            _Graphic = bod.Graphic;
            _RequireExceptional = bod.RequireExceptional;
            _Material = bod.Material;
        }

        //world load constructor
        public SmallBODListEntry(GenericReader reader) : base(reader)
        {
        }

        //clone constructor
        public SmallBODListEntry(SmallBODListEntry entry) : base(entry)
        {
            _AmountCur = entry.AmountCur;
            _AmountMax = entry.AmountMax;
            _BODFillType = entry.BODFillType;
            _Number = entry.Number;
            _Graphic = entry.Graphic;
            _RequireExceptional = entry.RequireExceptional;
            _Material = entry.Material;
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public override Item GenerateItem()
        {
            SmallBOD bod = (SmallBOD)Activator.CreateInstance(_Type,new object[] { _AmountCur,_AmountMax,_BODFillType,_Number,_Graphic,_RequireExceptional,_Material });

            return bod;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public override bool AllGood(Item item)
        {
            if (!base.AllGood(item))
            {
                return false;
            }

            if (!(item is SmallBOD))
            {
                return false;
            }

            SmallBOD bod = (SmallBOD)item;

            //TODO: test for bod stuff here?

            return true;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override ItemListEntry Clone()
        {
            return new SmallBODListEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(_AmountCur);
            writer.Write(_AmountMax);
            writer.Write(_BODFillType.Name);
            writer.Write(_Number);
            writer.Write(_Graphic);
            writer.Write(_RequireExceptional);
            writer.Write((int)_Material);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _AmountCur = reader.ReadInt();
            _AmountMax = reader.ReadInt();
            _BODFillType = ScriptCompiler.FindTypeByName(reader.ReadString());
            _Number = reader.ReadInt();
            _Graphic = reader.ReadInt();
            _RequireExceptional = reader.ReadBool();
            _Material = (BulkMaterialType)reader.ReadInt();
        }
    }

    /*
	//elron's small pet bod entry
	public class SmallBODMobileListEntry : ItemListEntry
	{
		public override int GumpWidth { get { return 550; } }

		//generic bod-specific data
		protected int _AmountCur, _AmountMax;
		protected Type _BODFillType;				//the item to go into the BOD, different from the BOD type
		protected int _Number;
		protected int _Graphic;
		protected string _AnimalName;

		public override List<ItemListEntryColumn> Columns
		{
			get
			{
				if (_Columns == null)
				{
					//NOTE: the default name is overridden here completely
					_Columns = new List<ItemListEntryColumn>();

					//add in specifics for this list type here
					_Columns.Add(new ItemListEntryColumn(0, "Item Type", CliLoc.GetName(_BODFillType)));
					_Columns.Add(new ItemListEntryColumn(150, "Amount", _AmountCur.ToString() + "/" + _AmountMax.ToString()));
				}

				return _Columns;
			}
		}

		//public accessors
		public int AmountCur { get { return _AmountCur; } }
		public int AmountMax { get { return _AmountMax; } }
		public Type BODFillType { get { return _BODFillType; } }
		public string AnimalName { get { return _AnimalName; } }
		public int Graphic { get { return _Graphic; } }

		//master constructor
		public SmallBODMobileListEntry(Item item) : base(item)
		{
			SmallTamingBOD bod = (SmallTamingBOD)item;

			_AmountCur = bod.AmountCur;
			_AmountMax = bod.AmountMax;
			_BODFillType = bod.Type;
			_Graphic = bod.Graphic;
			_AnimalName = bod.AnimalName;
		}
		
		//world load constructor
		public SmallBODMobileListEntry(GenericReader reader) : base(reader)
		{
		}

		//clone constructor
		public SmallBODMobileListEntry(SmallBODMobileListEntry entry) : base(entry)
		{
			_AmountCur = entry.AmountCur;
			_AmountMax = entry.AmountMax;
			_BODFillType = entry.BODFillType;
			_Graphic = entry.Graphic;
			_AnimalName = entry.AnimalName;   
		}

		//this generates an item from what is stored in the entry.  Note no exception handling
		public override Item GenerateItem()
		{
			//TODO: have this do nothing, and have child constructors do the work?
			SmallTamingBOD bod = (SmallTamingBOD)Activator.CreateInstance(_Type, new object[] { _AmountCur, _AmountMax, _BODFillType, _AnimalName, _Graphic });
			return bod;
		}

		//this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
		public override bool AllGood(Item item)
		{
			if (!base.AllGood(item))
			{
				return false;
			}

			if (!(item is SmallTamingBOD))
			{
				return false;
			}

			SmallTamingBOD bod = (SmallTamingBOD)item;

			if( bod.Type == null )
			{
				return false;
			}

			//TODO: test for bod stuff here?

			return true;
		}

		//this is used to drive the cloning process - derived classes fire their associated clone constructor
		public override ItemListEntry Clone()
		{
			return new SmallBODMobileListEntry(this);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);

			writer.Write(_AmountCur);
			writer.Write(_AmountMax);

			if( _BODFillType != null )
			{
				writer.Write(_BODFillType.Name);
			}
			else
			{
				writer.Write( "-NULL-" );
			}
			writer.Write(_Graphic);
			writer.Write(_AnimalName);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			_AmountCur = reader.ReadInt();
			_AmountMax = reader.ReadInt();
			_BODFillType = ScriptCompiler.FindTypeByName(reader.ReadString());
			if( _BODFillType == null )
			{
				Console.WriteLine( "Bad Bod!!" );
			}
			_Graphic = reader.ReadInt();
			_AnimalName = reader.ReadString();
		}
	}
	*/

    /*
	//Elron's large pet bod entry
	public class LargeBODMobileListEntry : ItemListEntry
	{
		public override int GumpWidth { get { return 700; } }

		//large bod-specific data

		protected int _AmountMax;
		protected string _Contents;				//the name is a composite of the types within the bod
		protected LargeMobileBulkEntry[] _Entries;

		public override List<ItemListEntryColumn> Columns
		{
			get
			{
				if (_Columns == null)
				{
					//NOTE: the default name is overridden here completely
					_Columns = new List<ItemListEntryColumn>();

					//add in specifics for this list type here
					_Columns.Add(new ItemListEntryColumn(0, "Contents", _Contents));
					_Columns.Add(new ItemListEntryColumn(550, "Amount", _AmountMax.ToString()));
				}

				return _Columns;
			}
		}

		//public accessors
		public int AmountMax { get { return _AmountMax; } }
		public string Contents { get { return _Contents; } }
		public LargeMobileBulkEntry[] Entries { get { return _Entries; } }

		//master constructor
		public LargeBODMobileListEntry(Item item) : base(item)
		{
			LargeTamingBOD bod = (LargeTamingBOD)item;

			_AmountMax = bod.AmountMax;

			//proper cloning is required

			_Entries = new LargeMobileBulkEntry[bod.Entries.Length];
			for (int i = 0; i < bod.Entries.Length; i++)
			{
				_Entries[i] = new LargeMobileBulkEntry(null, bod.Entries[i].Details);
				_Entries[i].Amount = bod.Entries[i].Amount;
			}

			//this produces the name for the bod based on the bod name definitions
			GenerateContentsName();
		}

		//world load constructor
		public LargeBODMobileListEntry(GenericReader reader) : base(reader)
		{
		}

		//clone constructor
		public LargeBODMobileListEntry(LargeBODMobileListEntry entry) : base(entry)
		{
			_AmountMax = entry.AmountMax;

			_Entries = new LargeMobileBulkEntry[entry.Entries.Length];

			//proper cloning is required
			for (int i = 0; i < entry.Entries.Length; i++)
			{
				_Entries[i] = new LargeMobileBulkEntry(null, entry.Entries[i].Details);
				_Entries[i].Amount = entry.Entries[i].Amount;
			}

			GenerateContentsName();
		}

		public void GenerateContentsName()
		{
			_Contents = "";

			for (int i = 0; i < _Entries.Length; i++)
			{
				_Contents += CliLoc.GetName(_Entries[i].Details.Type);

				if (i < _Entries.Length - 1)
				{
					_Contents += ",";
				}
			}
		}

		//this generates an item from what is stored in the entry.  Note no exception handling
		public override Item GenerateItem()
		{
			LargeTamingBOD bod = (LargeTamingBOD)Activator.CreateInstance(_Type, new object[] { _AmountMax, _Entries });

			//attach the large bod to the entries
			for (int i = 0; i < bod.Entries.Length; i++)
			{
				bod.Entries[i].Owner = bod;
			}

			return bod;
		}

		//this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
		public override bool AllGood(Item item)
		{
			if (!base.AllGood(item))
			{
				return false;
			}

			if (!(item is LargeTamingBOD))
			{
				return false;
			}

			LargeTamingBOD bod = (LargeTamingBOD)item;

			//TODO: test for bod stuff here?

			return true;
		}

		//this is used to drive the cloning process - derived classes fire their associated clone constructor
		public override ItemListEntry Clone()
		{
			return new LargeBODMobileListEntry(this);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);

			writer.Write(_AmountMax);

			writer.Write(_Entries.Length);

			for (int i = 0; i < _Entries.Length; i++)
			{
				_Entries[i].Serialize(writer);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			_AmountMax = reader.ReadInt();

			int count = reader.ReadInt();

			_Entries = new LargeMobileBulkEntry[count];
			for (int i = 0; i < count; i++)
			{
				_Entries[i] = new LargeMobileBulkEntry(null, reader);
			}

			GenerateContentsName();
		}
	}*/

    public class LargeBODListEntry : ItemListEntry
    {
        public override int GumpWidth { get { return 700; } }

        //large bod-specific data

        protected int _AmountMax;
        protected string _Contents;             //the name is a composite of the types within the bod
        protected bool _RequireExceptional;
        protected BulkMaterialType _Material;
        protected LargeBulkEntry[] _Entries;

        public override List<ItemListEntryColumn> Columns
        {
            get
            {
                if (_Columns == null)
                {
                    //NOTE: the default name is overridden here completely
                    _Columns = new List<ItemListEntryColumn>();

                    //add in specifics for this list type here
                    _Columns.Add(new ItemListEntryColumn(0,"Contents",_Contents));
                    _Columns.Add(new ItemListEntryColumn(350,"Amount",_AmountMax.ToString()));
                    _Columns.Add(new ItemListEntryColumn(450,"Exceptional?",(_RequireExceptional ? "yes" : "no")));
                    _Columns.Add(new ItemListEntryColumn(550,"Resource",Enum.GetName(typeof(BulkMaterialType),(int)_Material)));
                }

                return _Columns;
            }
        }

        //public accessors
        public int AmountMax { get { return _AmountMax; } }
        public string Contents { get { return _Contents; } }
        public bool RequireExceptional { get { return _RequireExceptional; } }
        public BulkMaterialType Material { get { return _Material; } }
        public LargeBulkEntry[] Entries { get { return _Entries; } }

        //master constructor
        public LargeBODListEntry(Item item) : base(item)
        {
            LargeBOD bod = (LargeBOD)item;

            _AmountMax = bod.AmountMax;
            _RequireExceptional = bod.RequireExceptional;
            _Material = bod.Material;

            //proper cloning is required

            _Entries = new LargeBulkEntry[bod.Entries.Length];
            for (int i = 0; i < bod.Entries.Length; i++)
            {
                _Entries[i] = new LargeBulkEntry(null,bod.Entries[i].Details);
                _Entries[i].Amount = bod.Entries[i].Amount;
            }

            //this produces the name for the bod based on the bod name definitions
            GenerateContentsName();
        }

        //world load constructor
        public LargeBODListEntry(GenericReader reader) : base(reader)
        {
        }

        //clone constructor
        public LargeBODListEntry(LargeBODListEntry entry) : base(entry)
        {
            _AmountMax = entry.AmountMax;
            _RequireExceptional = entry.RequireExceptional;
            _Material = entry.Material;

            _Entries = new LargeBulkEntry[entry.Entries.Length];

            //proper cloning is required
            for (int i = 0; i < entry.Entries.Length; i++)
            {
                _Entries[i] = new LargeBulkEntry(null,entry.Entries[i].Details);
                _Entries[i].Amount = entry.Entries[i].Amount;
            }

            GenerateContentsName();
        }

        public void GenerateContentsName()
        {
            _Contents = "";

            for (int i = 0; i < _Entries.Length; i++)
            {
                _Contents += CliLoc.GetName(_Entries[i].Details.Type);

                if (i < _Entries.Length - 1)
                {
                    _Contents += ", ";
                }
            }
        }

        //this generates an item from what is stored in the entry.  Note no exception handling
        public override Item GenerateItem()
        {
            LargeBOD bod = (LargeBOD)Activator.CreateInstance(_Type,new object[] { _AmountMax,_RequireExceptional,_Material,_Entries });

            //attach the large bod to the entries
            for (int i = 0; i < bod.Entries.Length; i++)
            {
                bod.Entries[i].Owner = bod;
            }

            return bod;
        }

        //this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
        public override bool AllGood(Item item)
        {
            if (!base.AllGood(item))
            {
                return false;
            }

            if (!(item is LargeBOD))
            {
                return false;
            }

            LargeBOD bod = (LargeBOD)item;

            //TODO: test for bod stuff here?

            return true;
        }

        //this is used to drive the cloning process - derived classes fire their associated clone constructor
        public override ItemListEntry Clone()
        {
            return new LargeBODListEntry(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(_AmountMax);
            writer.Write(_RequireExceptional);
            writer.Write((int)_Material);

            writer.Write(_Entries.Length);

            for (int i = 0; i < _Entries.Length; i++)
            {
                _Entries[i].Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _AmountMax = reader.ReadInt();
            _RequireExceptional = reader.ReadBool();
            _Material = (BulkMaterialType)reader.ReadInt();

            int count = reader.ReadInt();

            _Entries = new LargeBulkEntry[count];
            for (int i = 0; i < count; i++)
            {
                _Entries[i] = new LargeBulkEntry(null,reader);
            }

            GenerateContentsName();
        }
    }
}