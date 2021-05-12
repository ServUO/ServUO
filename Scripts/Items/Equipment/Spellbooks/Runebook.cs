using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class Runebook : Item, ISecurable, ICraftable
    {
        public override int LabelNumber => 1041267;  // runebook

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
        private Mobile m_Crafter;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextUse { get; set; }

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
        public SecureLevel Level { get; set; }

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
        public int CurCharges { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges { get; set; }

        public List<Mobile> Openers { get; set; } = new List<Mobile>();

        public virtual int MaxEntries => 16;

        [Constructable]
        public Runebook(int maxCharges, int id = 0x22C5)
            : base(id)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = 0x461;

            Layer = Layer.Invalid;

            m_Entries = new List<RunebookEntry>();

            MaxCharges = maxCharges;

            DefaultIndex = -1;

            Level = SecureLevel.CoOwners;
        }

        [Constructable]
        public Runebook()
            : this(12)
        {
        }

        public List<RunebookEntry> Entries => m_Entries;

        public int DefaultIndex { get; set; }

        public RunebookEntry Default
        {
            get
            {
                if (DefaultIndex >= 0 && DefaultIndex < m_Entries.Count)
                    return m_Entries[DefaultIndex];

                return null;
            }
            set
            {
                if (value == null)
                    DefaultIndex = -1;
                else
                    DefaultIndex = m_Entries.IndexOf(value);
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
            writer.Write(3);

            writer.Write((byte)m_Quality);

            writer.Write(m_Crafter);

            writer.Write((int)Level);

            writer.Write(m_Entries.Count);

            for (int i = 0; i < m_Entries.Count; ++i)
                m_Entries[i].Serialize(writer);

            writer.Write(m_Description);
            writer.Write(CurCharges);
            writer.Write(MaxCharges);
            writer.Write(DefaultIndex);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            LootType = LootType.Blessed;

            switch (version)
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
                        Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        int count = reader.ReadInt();

                        m_Entries = new List<RunebookEntry>(count);

                        for (int i = 0; i < count; ++i)
                            m_Entries.Add(new RunebookEntry(reader));

                        m_Description = reader.ReadString();
                        CurCharges = reader.ReadInt();
                        MaxCharges = reader.ReadInt();
                        DefaultIndex = reader.ReadInt();

                        break;
                    }
            }
        }

        public void DropRune(Mobile from, RunebookEntry e, int index)
        {
            if (DefaultIndex > index)
                DefaultIndex -= 1;
            else if (DefaultIndex == index)
                DefaultIndex = -1;

            m_Entries.RemoveAt(index);


            RecallRune rune = new RecallRune();

            if (e.Galleon != null)
            {
                rune.Galleon = e.Galleon;
            }
            else if (e.House != null)
            {
                rune.Target = e.Location;
                rune.TargetMap = e.Map;
                rune.Description = e.Description;
                rune.House = e.House;
            }
            else
            {
                rune.Target = e.Location;
                rune.TargetMap = e.Map;
                rune.Description = e.Description;
            }

            rune.Type = e.Type;
            rune.Marked = true;

            from.AddToBackpack(rune);

            from.SendLocalizedMessage(502421, "", 0x35); // You have removed the rune.
        }

        public bool IsOpen(Mobile toCheck)
        {
            return HasGump(toCheck);
        }

        public virtual bool HasGump(Mobile toCheck)
        {
            RunebookGump bookGump = toCheck.FindGump<RunebookGump>();

            if (bookGump != null && bookGump.Book == this)
            {
                return true;
            }

            return false;
        }

        public virtual void CloseGump(Mobile m)
        {
            m.CloseGump(typeof(RunebookGump));
        }

        public override bool DisplayLootType => true;

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Quality == BookQuality.Exceptional)
                list.Add(1063341); // exceptional

            if (m_Crafter != null)
                list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (!string.IsNullOrEmpty(m_Description))
                list.Add(m_Description);
        }

        public override bool OnDragLift(Mobile from)
        {
            if (HasGump(from))
            {
                from.SendLocalizedMessage(500169); // You cannot pick that up.
                return false;
            }

            foreach (Mobile m in Openers)
            {
                if (IsOpen(m))
                {
                    CloseGump(m);
                }
            }

            Openers.Clear();

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 3) && CheckAccess(from))
            {
                if (RootParent is BaseCreature)
                {
                    from.SendLocalizedMessage(502402); // That is inaccessible.
                    return;
                }

                if (DateTime.UtcNow < NextUse)
                {
                    from.SendLocalizedMessage(502406); // This book needs time to recharge.
                    return;
                }

                from.CloseGump(typeof(RunebookGump));
                from.SendGump(new RunebookGump(from, this));

                Openers.Add(from);
            }
        }

        public virtual void OnTravel()
        {
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

                book.m_Entries.Add(new RunebookEntry(entry.Location, entry.Map, entry.Description, entry.House, entry.Type));
            }

            base.OnAfterDuped(newItem);
        }

        public bool CheckAccess(Mobile m)
        {
            if (!IsLockedDown || m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && (house.Public ? house.IsBanned(m) : !house.HasAccess(m)))
                return false;

            return house != null && house.HasSecureAccess(m, Level);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is RecallRune)
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

                        if (rune.Marked)
                        {
                            if (rune.Type == RecallRuneType.Ship)
                            {
                                RunebookEntry entry = new RunebookEntry(Point3D.Zero, null, null, null, rune.Type, rune.Galleon);
                                m_Entries.Add(entry);

                                dropped.Delete();

                                from.Send(new PlaySound(0x42, GetWorldLocation()));

                                from.SendAsciiMessage(entry.Description);

                                return true;
                            }
                            else if (rune.TargetMap != null)
                            {
                                m_Entries.Add(new RunebookEntry(rune.Target, rune.TargetMap, rune.Description, rune.House, rune.Type));

                                dropped.Delete();

                                from.Send(new PlaySound(0x42, GetWorldLocation()));

                                string desc = rune.Description;

                                if (desc == null || (desc = desc.Trim()).Length == 0)
                                    desc = "(indescript)";

                                from.SendAsciiMessage(desc);

                                return true;
                            }
                        }
                        else
                        {
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
                if (CurCharges < MaxCharges)
                {
                    from.Send(new PlaySound(0x249, GetWorldLocation()));

                    int amount = dropped.Amount;

                    if (amount > (MaxCharges - CurCharges))
                    {
                        dropped.Consume(MaxCharges - CurCharges);
                        CurCharges = MaxCharges;
                    }
                    else
                    {
                        CurCharges += amount;
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

            MaxCharges = charges * 2;

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

        public Point3D Location
        {
            get
            {
                if (Galleon != null && !Galleon.Deleted)
                {
                    return Galleon.GetMarkedLocation();
                }

                return m_Location;
            }
        }

        public Map Map
        {
            get
            {
                if (Galleon != null && !Galleon.Deleted && Galleon.Map != Map.Internal && Galleon.Map != null)
                {
                    return Galleon.Map;
                }

                return m_Map;
            }
        }

        public string Description
        {
            get
            {
                if (Type == RecallRuneType.Ship)
                {
                    string ownername;
                    string shipname;

                    if (Galleon == null)
                    {
                        ownername = "unknown owner";
                        shipname = "unknown ship";
                    }
                    else
                    {
                        if (Galleon.Owner != null)
                        {
                            ownername = Galleon.Owner.Name;
                        }
                        else
                        {
                            ownername = "unknown owner";
                        }

                        if (Galleon.ShipName != null)
                        {
                            shipname = Galleon.ShipName;
                        }
                        else
                        {
                            shipname = "unnamed ship";
                        }
                    }

                    return string.Format("{0}'s ship, the {1}", ownername, shipname);
                }

                return m_Description;
            }
        }

        public BaseHouse House { get; }

        public BaseGalleon Galleon { get; }

        public RecallRuneType Type { get; }

        public RunebookEntry(Point3D loc, Map map, string desc, BaseHouse house, RecallRuneType type = 0, BaseGalleon g = null)
        {
            m_Location = loc;
            m_Map = map;
            m_Description = desc;
            House = house;
            Galleon = g;
            Type = type;
        }

        public RunebookEntry(GenericReader reader)
        {
            int version = reader.ReadByte();

            switch (version)
            {
                case 3:
                    {
                        Type = (RecallRuneType)reader.ReadInt();
                        Galleon = reader.ReadItem() as BaseGalleon;
                        House = reader.ReadItem() as BaseHouse;
                        m_Location = reader.ReadPoint3D();
                        m_Map = reader.ReadMap();
                        m_Description = reader.ReadString();

                        break;
                    }
                case 2:
                    {
                        Galleon = reader.ReadItem() as BaseGalleon;
                        goto case 0;
                    }
                case 1:
                    {
                        House = reader.ReadItem() as BaseHouse;
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

            if (version < 3)
            {
                if (Galleon != null)
                    Type = RecallRuneType.Ship;
                else if (House != null)
                    Type = RecallRuneType.Shop;
                else
                    Type = RecallRuneType.Normal;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((byte)3);

            writer.Write((int)Type);
            writer.Write(Galleon);
            writer.Write(House);
            writer.Write(m_Location);
            writer.Write(m_Map);
            writer.Write(m_Description);
        }
    }
}
