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
        public static readonly TimeSpan UseDelay = TimeSpan.FromSeconds(7.0);

        private BookQuality m_Quality;
		
        [CommandProperty(AccessLevel.GameMaster)]		
        public BookQuality Quality
        {
            get
            {
                return m_Quality;
            }
            set
            {
                m_Quality = value;
                InvalidateProperties();
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
                return m_NextUse;
            }
            set
            {
                m_NextUse = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return m_Crafter;
            }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return m_Level;
            }
            set
            {
                m_Level = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurCharges
        {
            get
            {
                return m_CurCharges;
            }
            set
            {
                m_CurCharges = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges
        {
            get
            {
                return m_MaxCharges;
            }
            set
            {
                m_MaxCharges = value;
            }
        }
		
        public List<Mobile> Openers
        {
            get
            {
                return m_Openers;
            }
            set
            {
                m_Openers = value;
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1041267;
            }
        }// runebook

        public virtual int MaxEntries { get { return 16; } }

        [Constructable]
        public Runebook(int maxCharges, int id = 0x22C5)
            : base(Core.AOS ? id : 0xEFA)
        {
            Weight = (Core.SE ? 1.0 : 3.0);
            LootType = LootType.Blessed;
            Hue = 0x461;

            Layer = (Core.AOS ? Layer.Invalid : Layer.OneHanded);

            m_Entries = new List<RunebookEntry>();

            m_MaxCharges = maxCharges;

            m_DefaultIndex = -1;

            m_Level = SecureLevel.CoOwners;
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
                return m_Entries;
            }
        }

        public int DefaultIndex
        {
            get { return m_DefaultIndex; }
            set { m_DefaultIndex = value; }
        }

        public RunebookEntry Default
        {
            get
            {
                if (m_DefaultIndex >= 0 && m_DefaultIndex < m_Entries.Count)
                    return m_Entries[m_DefaultIndex];

                return null;
            }
            set
            {
                if (value == null)
                    m_DefaultIndex = -1;
                else
                    m_DefaultIndex = m_Entries.IndexOf(value);
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

            writer.Write((byte)m_Quality);	

            writer.Write(m_Crafter);

            writer.Write((int)m_Level);

            writer.Write(m_Entries.Count);

            for (int i = 0; i < m_Entries.Count; ++i)
                m_Entries[i].Serialize(writer);

            writer.Write(m_Description);
            writer.Write(m_CurCharges);
            writer.Write(m_MaxCharges);
            writer.Write(m_DefaultIndex);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            LootType = LootType.Blessed;

            if (Core.SE && Weight == 3.0)
                Weight = 1.0;

            int version = reader.ReadInt();

            switch ( version )
            {
                case 3:
                    {
                        m_Quality = (BookQuality)reader.ReadByte();		
                        goto case 2;
                    }
                case 2:
                    {
                        m_Crafter = reader.ReadMobile();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        int count = reader.ReadInt();

                        m_Entries = new List<RunebookEntry>(count);

                        for (int i = 0; i < count; ++i)
                            m_Entries.Add(new RunebookEntry(reader));

                        m_Description = reader.ReadString();
                        m_CurCharges = reader.ReadInt();
                        m_MaxCharges = reader.ReadInt();
                        m_DefaultIndex = reader.ReadInt();

                        break;
                    }
            }
        }

        public void DropRune(Mobile from, RunebookEntry e, int index)
        {
            if (m_DefaultIndex > index)
                m_DefaultIndex -= 1;
            else if (m_DefaultIndex == index)
                m_DefaultIndex = -1;

            m_Entries.RemoveAt(index);

            if (e.Galleon != null)
            {
                if (e.Galleon.Deleted)
                {
                    from.SendMessage("You discard the rune as the galleon is no longer available.");
                    return;
                }
                else
                {
                    ShipRune rune = new ShipRune(e.Galleon);
                    from.AddToBackpack(rune);
                }
            }
            else
            {
                RecallRune rune = new RecallRune();

                rune.Target = e.Location;
                rune.TargetMap = e.Map;
                rune.Description = e.Description;
                rune.House = e.House;
                rune.Marked = true;
                rune.Hue = RecallRune.CalculateHue(e.Map, e.House, true);

                from.AddToBackpack(rune);
            }

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
		
            if (m_Quality == BookQuality.Exceptional)
                list.Add(1063341); // exceptional

            if (m_Crafter != null)
				list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (m_Description != null && m_Description.Length > 0)
                list.Add(m_Description);
        }
		
        public override bool OnDragLift(Mobile from)
        {
            if (from.HasGump(typeof(RunebookGump)))
            {
                from.SendLocalizedMessage(500169); // You cannot pick that up.
                return false;
            }
			
            foreach (Mobile m in m_Openers)
                if (IsOpen(m))
                    m.CloseGump(typeof(RunebookGump));
				
            m_Openers.Clear();
			
            return true;
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_Description != null && m_Description.Length > 0)
                LabelTo(from, m_Description);

            base.OnSingleClick(from);

            if (m_Crafter != null)
				LabelTo(from, 1050043, m_Crafter.TitleName);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), (Core.ML ? 3 : 1)) && CheckAccess(from))
            {
                if (RootParent is BaseCreature)
                {
                    from.SendLocalizedMessage(502402); // That is inaccessible.
                    return;
                }

                if (DateTime.UtcNow < m_NextUse)
                {
                    from.SendLocalizedMessage(502406); // This book needs time to recharge.
                    return;
                }

                from.CloseGump(typeof(RunebookGump));
                from.SendGump(new RunebookGump(from, this));
				
                m_Openers.Add(from);
            }
        }

        public virtual void OnTravel()
        {
            if (!Core.SA)
                m_NextUse = DateTime.UtcNow + UseDelay;
        }

        public override void OnAfterDuped(Item newItem)
        {
            Runebook book = newItem as Runebook;

            if (book == null)
                return;

            book.m_Entries = new List<RunebookEntry>();

            for (int i = 0; i < m_Entries.Count; i++)
            {
                RunebookEntry entry = m_Entries[i];

                book.m_Entries.Add(new RunebookEntry(entry.Location, entry.Map, entry.Description, entry.House));
            }

            base.OnAfterDuped(newItem);
        }

        public bool CheckAccess(Mobile m)
        {
            if (!IsLockedDown || m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsAosRules && (house.Public ? house.IsBanned(m) : !house.HasAccess(m)))
                return false;

            return (house != null && house.HasSecureAccess(m, m_Level));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is RecallRune || dropped is ShipRune)
            {
                if (IsLockedDown && from.AccessLevel < AccessLevel.GameMaster)
                {
                    from.SendLocalizedMessage(502413, null, 0x35); // That cannot be done while the book is locked down.
                }
                else if (IsOpen(from))
                {
                    from.SendLocalizedMessage(1005571); // You cannot place objects in the book while viewing the contents.
                }
                else if (m_Entries.Count < MaxEntries)
                {
                    if (dropped is RecallRune)
                    {
                        RecallRune rune = (RecallRune)dropped;

                        if (rune.Marked && rune.TargetMap != null)
                        {
                            m_Entries.Add(new RunebookEntry(rune.Target, rune.TargetMap, rune.Description, rune.House));

                            dropped.Delete();

                            from.Send(new PlaySound(0x42, GetWorldLocation()));

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
                    else if(dropped is ShipRune)
                    {
                        ShipRune rune = (ShipRune)dropped;

                        if (rune.Galleon != null && !rune.Galleon.Deleted)
                        {
                            m_Entries.Add(new RunebookEntry(Point3D.Zero, null, rune.Galleon.ShipName, null, rune.Galleon));

                            dropped.Delete();

                            from.Send(new PlaySound(0x42, GetWorldLocation()));

                            string desc = rune.Galleon.ShipName;

                            if (desc == null || (desc = desc.Trim()).Length == 0)
                                desc = "an unnamed ship";

                            from.SendMessage(desc);

                            return true;
                        }
                        else
                        {
                            if (rune.DockedBoat != null)
                                from.SendMessage("You cannot place a rune to a docked boat in the runebook.");
                            else
                                from.SendLocalizedMessage(502409); // This rune does not have a marked location.
                        }
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502401); // This runebook is full.
                }
            }
            else if (dropped is RecallScroll)
            {
                if (m_CurCharges < m_MaxCharges)
                {
                    from.Send(new PlaySound(0x249, GetWorldLocation()));

                    int amount = dropped.Amount;

                    if (amount > (m_MaxCharges - m_CurCharges))
                    {
                        dropped.Consume(m_MaxCharges - m_CurCharges);
                        m_CurCharges = m_MaxCharges;
                    }
                    else
                    {
                        m_CurCharges += amount;
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

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            int charges = 5 + quality + (int)(from.Skills[SkillName.Inscribe].Value / 30);

            if (charges > 10)
                charges = 10;

            MaxCharges = (Core.SE ? charges * 2 : charges);

            if (makersMark)
                Crafter = from;

            m_Quality = (BookQuality)(quality - 1);

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
        private BaseGalleon m_Galleon;

        public Point3D Location
        {
            get
            {
                if (m_Galleon != null && !m_Galleon.Deleted)
                {
                    return m_Galleon.GetMarkedLocation();
                }

                return m_Location;
            }
        }

        public Map Map
        {
            get
            {
                if (m_Galleon != null && !m_Galleon.Deleted && m_Galleon.Map != Map.Internal && m_Galleon.Map != null)
                {
                    return m_Galleon.Map;
                }

                return m_Map;
            }
        }

        public string Description
        {
            get
            {
                return m_Description;
            }
        }

        public BaseHouse House
        {
            get
            {
                return m_House;
            }
        }

        public BaseGalleon Galleon
        {
            get { return m_Galleon; }
        }

        public RunebookEntry(Point3D loc, Map map, string desc, BaseHouse house, BaseGalleon g = null)
        {
            m_Location = loc;
            m_Map = map;
            m_Description = desc;
            m_House = house;
            m_Galleon = g;
        }

        public RunebookEntry(GenericReader reader)
        {
            int version = reader.ReadByte();

            switch ( version )
            {
                case 2:
                    {
                        m_Galleon = reader.ReadItem() as BaseGalleon;
                        goto case 0;
                    }
                case 1:
                    {
                        m_House = reader.ReadItem() as BaseHouse;
                        goto case 0;
                    }
                case 0:
                    {
                        m_Location = reader.ReadPoint3D();
                        m_Map = reader.ReadMap();
                        m_Description = reader.ReadString();

                        break;
                    }
            }
        }

        public void Serialize(GenericWriter writer)
        {
            if (m_Galleon != null && !m_Galleon.Deleted)
            {
                writer.Write((byte)2);

                writer.Write(m_Galleon);
            }
            else if (m_House != null && !m_House.Deleted)
            {
                writer.Write((byte)1); // version

                writer.Write(m_House);
            }
            else
            {
                writer.Write((byte)0); // version
            }

            writer.Write(m_Location);
            writer.Write(m_Map);
            writer.Write(m_Description);
        }
    }
}
