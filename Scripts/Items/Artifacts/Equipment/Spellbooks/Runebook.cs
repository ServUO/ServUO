using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class Runebook : Item, ISecurable, ICraftable
	{
		public override bool IsArtifact { get { return true; } }
        public static readonly TimeSpan UseDelay = TimeSpan.FromSeconds(7.0);

        private BookQuality m_Quality;
		
        [CommandProperty(AccessLevel.GameMaster)]		
        public BookQuality Quality
        {
            get
            {
                return this.m_Quality;
            }
            set
            {
                this.m_Quality = value;
                this.InvalidateProperties();
            }
        }

        private List<RunebookEntry> m_Entries;
        private string m_Description;
        private int m_CurCharges, m_MaxCharges;
        private int m_DefaultIndex;
        private SecureLevel m_Level;
        private Mobile m_Crafter;
		
        private DateTime m_NextUse;
		
        private List<Mobile> m_Openers = new List<Mobile>();

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextUse
        {
            get
            {
                return this.m_NextUse;
            }
            set
            {
                this.m_NextUse = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return this.m_Crafter;
            }
            set
            {
                this.m_Crafter = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Description
        {
            get
            {
                return this.m_Description;
            }
            set
            {
                this.m_Description = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurCharges
        {
            get
            {
                return this.m_CurCharges;
            }
            set
            {
                this.m_CurCharges = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges
        {
            get
            {
                return this.m_MaxCharges;
            }
            set
            {
                this.m_MaxCharges = value;
            }
        }
		
        public List<Mobile> Openers
        {
            get
            {
                return this.m_Openers;
            }
            set
            {
                this.m_Openers = value;
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1041267;
            }
        }// runebook

        [Constructable]
        public Runebook(int maxCharges)
            : base(Core.AOS ? 0x22C5 : 0xEFA)
        {
            this.Weight = (Core.SE ? 1.0 : 3.0);
            this.LootType = LootType.Blessed;
            this.Hue = 0x461;

            this.Layer = (Core.AOS ? Layer.Invalid : Layer.OneHanded);

            this.m_Entries = new List<RunebookEntry>();

            this.m_MaxCharges = maxCharges;

            this.m_DefaultIndex = -1;

            this.m_Level = SecureLevel.CoOwners;
        }

        [Constructable]
        public Runebook()
            : this(Core.SE ? 12 : 6)
        {
        }

        public List<RunebookEntry> Entries
        {
            get
            {
                return this.m_Entries;
            }
        }

        public RunebookEntry Default
        {
            get
            {
                if (this.m_DefaultIndex >= 0 && this.m_DefaultIndex < this.m_Entries.Count)
                    return this.m_Entries[this.m_DefaultIndex];

                return null;
            }
            set
            {
                if (value == null)
                    this.m_DefaultIndex = -1;
                else
                    this.m_DefaultIndex = this.m_Entries.IndexOf(value);
            }
        }

        public Runebook(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            return true;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);

            writer.Write((byte)this.m_Quality);	

            writer.Write(this.m_Crafter);

            writer.Write((int)this.m_Level);

            writer.Write(this.m_Entries.Count);

            for (int i = 0; i < this.m_Entries.Count; ++i)
                this.m_Entries[i].Serialize(writer);

            writer.Write(this.m_Description);
            writer.Write(this.m_CurCharges);
            writer.Write(this.m_MaxCharges);
            writer.Write(this.m_DefaultIndex);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            this.LootType = LootType.Blessed;

            if (Core.SE && this.Weight == 3.0)
                this.Weight = 1.0;

            int version = reader.ReadInt();

            switch ( version )
            {
                case 3:
                    {
                        this.m_Quality = (BookQuality)reader.ReadByte();		
                        goto case 2;
                    }
                case 2:
                    {
                        this.m_Crafter = reader.ReadMobile();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        int count = reader.ReadInt();

                        this.m_Entries = new List<RunebookEntry>(count);

                        for (int i = 0; i < count; ++i)
                            this.m_Entries.Add(new RunebookEntry(reader));

                        this.m_Description = reader.ReadString();
                        this.m_CurCharges = reader.ReadInt();
                        this.m_MaxCharges = reader.ReadInt();
                        this.m_DefaultIndex = reader.ReadInt();

                        break;
                    }
            }
        }

        public void DropRune(Mobile from, RunebookEntry e, int index)
        {
            if (this.m_DefaultIndex > index)
                this.m_DefaultIndex -= 1;
            else if (this.m_DefaultIndex == index)
                this.m_DefaultIndex = -1;

            this.m_Entries.RemoveAt(index);

            RecallRune rune = new RecallRune();

            rune.Target = e.Location;
            rune.TargetMap = e.Map;
            rune.Description = e.Description;
            rune.House = e.House;
            rune.Marked = true;

            from.AddToBackpack(rune);

            from.SendLocalizedMessage(502421); // You have removed the rune.
        }

        public bool IsOpen(Mobile toCheck)
        {
            NetState ns = toCheck.NetState;

            if (ns != null)
            {
                foreach (Gump gump in ns.Gumps)
                {
                    RunebookGump bookGump = gump as RunebookGump;

                    if (bookGump != null && bookGump.Book == this)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool DisplayLootType
        {
            get
            {
                return Core.AOS;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
		
            if (this.m_Quality == BookQuality.Exceptional)
                list.Add(1063341); // exceptional

            if (this.m_Crafter != null)
				list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (this.m_Description != null && this.m_Description.Length > 0)
                list.Add(this.m_Description);
        }
		
        public override bool OnDragLift(Mobile from)
        {
            if (from.HasGump(typeof(RunebookGump)))
            {
                from.SendLocalizedMessage(500169); // You cannot pick that up.
                return false;
            }
			
            foreach (Mobile m in this.m_Openers)
                if (this.IsOpen(m))
                    m.CloseGump(typeof(RunebookGump));
				
            this.m_Openers.Clear();
			
            return true;
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.m_Description != null && this.m_Description.Length > 0)
                this.LabelTo(from, this.m_Description);

            base.OnSingleClick(from);

            if (this.m_Crafter != null)
				this.LabelTo(from, 1050043, m_Crafter.TitleName);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), (Core.ML ? 3 : 1)) && this.CheckAccess(from))
            {
                if (this.RootParent is BaseCreature)
                {
                    from.SendLocalizedMessage(502402); // That is inaccessible.
                    return;
                }

                if (DateTime.UtcNow < this.m_NextUse)
                {
                    from.SendLocalizedMessage(502406); // This book needs time to recharge.
                    return;
                }

                from.CloseGump(typeof(RunebookGump));
                from.SendGump(new RunebookGump(from, this));
				
                this.m_Openers.Add(from);
            }
        }

        public virtual void OnTravel()
        {
            if (!Core.SA)
                this.m_NextUse = DateTime.UtcNow + UseDelay;
        }

        public override void OnAfterDuped(Item newItem)
        {
            Runebook book = newItem as Runebook;

            if (book == null)
                return;

            book.m_Entries = new List<RunebookEntry>();

            for (int i = 0; i < this.m_Entries.Count; i++)
            {
                RunebookEntry entry = this.m_Entries[i];

                book.m_Entries.Add(new RunebookEntry(entry.Location, entry.Map, entry.Description, entry.House));
            }
        }

        public bool CheckAccess(Mobile m)
        {
            if (!this.IsLockedDown || m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsAosRules && (house.Public ? house.IsBanned(m) : !house.HasAccess(m)))
                return false;

            return (house != null && house.HasSecureAccess(m, this.m_Level));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is RecallRune)
            {
                if (this.IsLockedDown && from.AccessLevel < AccessLevel.GameMaster)
                {
                    from.SendLocalizedMessage(502413, null, 0x35); // That cannot be done while the book is locked down.
                }
                else if (this.IsOpen(from))
                {
                    from.SendLocalizedMessage(1005571); // You cannot place objects in the book while viewing the contents.
                }
                else if (this.m_Entries.Count < 16)
                {
                    RecallRune rune = (RecallRune)dropped;

                    if (rune.Marked && rune.TargetMap != null)
                    {
                        this.m_Entries.Add(new RunebookEntry(rune.Target, rune.TargetMap, rune.Description, rune.House));

                        dropped.Delete();

                        from.Send(new PlaySound(0x42, this.GetWorldLocation()));

                        string desc = rune.Description;

                        if (desc == null || (desc = desc.Trim()).Length == 0)
                            desc = "(indescript)";

                        from.SendMessage(desc);

                        return true;
                    }
                    else
                    {
                        from.SendLocalizedMessage(502409); // This rune does not have a marked location.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502401); // This runebook is full.
                }
            }
            else if (dropped is RecallScroll)
            {
                if (this.m_CurCharges < this.m_MaxCharges)
                {
                    from.Send(new PlaySound(0x249, this.GetWorldLocation()));

                    int amount = dropped.Amount;

                    if (amount > (this.m_MaxCharges - this.m_CurCharges))
                    {
                        dropped.Consume(this.m_MaxCharges - this.m_CurCharges);
                        this.m_CurCharges = this.m_MaxCharges;
                    }
                    else
                    {
                        this.m_CurCharges += amount;
                        dropped.Delete();

                        return true;
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502410); // This book already has the maximum amount of charges.
                }
            }

            return false;
        }

        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            int charges = 5 + quality + (int)(from.Skills[SkillName.Inscribe].Value / 30);

            if (charges > 10)
                charges = 10;

            this.MaxCharges = (Core.SE ? charges * 2 : charges);

            if (makersMark)
                this.Crafter = from;

            this.m_Quality = (BookQuality)(quality - 1);

            return quality;
        }
        #endregion
    }

    public class RunebookEntry
    {
        private readonly Point3D m_Location;
        private readonly Map m_Map;
        private readonly string m_Description;
        private readonly BaseHouse m_House;

        public Point3D Location
        {
            get
            {
                return this.m_Location;
            }
        }

        public Map Map
        {
            get
            {
                return this.m_Map;
            }
        }

        public string Description
        {
            get
            {
                return this.m_Description;
            }
        }

        public BaseHouse House
        {
            get
            {
                return this.m_House;
            }
        }

        public RunebookEntry(Point3D loc, Map map, string desc, BaseHouse house)
        {
            this.m_Location = loc;
            this.m_Map = map;
            this.m_Description = desc;
            this.m_House = house;
        }

        public RunebookEntry(GenericReader reader)
        {
            int version = reader.ReadByte();

            switch ( version )
            {
                case 1:
                    {
                        this.m_House = reader.ReadItem() as BaseHouse;
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Location = reader.ReadPoint3D();
                        this.m_Map = reader.ReadMap();
                        this.m_Description = reader.ReadString();

                        break;
                    }
            }
        }

        public void Serialize(GenericWriter writer)
        {
            if (this.m_House != null && !this.m_House.Deleted)
            {
                writer.Write((byte)1); // version

                writer.Write(this.m_House);
            }
            else
            {
                writer.Write((byte)0); // version
            }

            writer.Write(this.m_Location);
            writer.Write(this.m_Map);
            writer.Write(this.m_Description);
        }
    }
}