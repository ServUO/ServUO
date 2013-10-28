using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Server.Accounting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Network;
using Server.Regions;

namespace Server.Items
{
    public enum HouseRaffleState
    {
        Inactive,
        Active,
        Completed
    }

    public enum HouseRaffleExpireAction
    {
        None,
        HideStone,
        DeleteStone
    }

    public class RaffleEntry
    {
        private readonly Mobile m_From;
        private readonly IPAddress m_Address;
        private readonly DateTime m_Date;
        public RaffleEntry(Mobile from)
        {
            this.m_From = from;

            if (this.m_From.NetState != null)
                this.m_Address = this.m_From.NetState.Address;
            else
                this.m_Address = IPAddress.None;

            this.m_Date = DateTime.UtcNow;
        }

        public RaffleEntry(GenericReader reader, int version)
        {
            switch ( version )
            {
                case 3: // HouseRaffleStone version changes
                case 2:
                case 1:
                case 0:
                    {
                        this.m_From = reader.ReadMobile();
                        this.m_Address = Utility.Intern(reader.ReadIPAddress());
                        this.m_Date = reader.ReadDateTime();

                        break;
                    }
            }
        }

        public Mobile From
        {
            get
            {
                return this.m_From;
            }
        }
        public IPAddress Address
        {
            get
            {
                return this.m_Address;
            }
        }
        public DateTime Date
        {
            get
            {
                return this.m_Date;
            }
        }
        public void Serialize(GenericWriter writer)
        {
            writer.Write(this.m_From);
            writer.Write(this.m_Address);
            writer.Write(this.m_Date);
        }
    }

    [FlipableAttribute(0xEDD, 0xEDE)]
    public class HouseRaffleStone : Item
    {
        public static readonly TimeSpan DefaultDuration = TimeSpan.FromDays(7.0);
        public static readonly TimeSpan ExpirationTime = TimeSpan.FromDays(30.0);
        private static readonly List<HouseRaffleStone> m_AllStones = new List<HouseRaffleStone>();
        private const int EntryLimitPerIP = 4;
        private const int DefaultTicketPrice = 5000;
        private const int MessageHue = 1153;
        private HouseRaffleRegion m_Region;
        private Rectangle2D m_Bounds;
        private Map m_Facet;
        private Mobile m_Winner;
        private HouseRaffleDeed m_Deed;
        private HouseRaffleState m_State;
        private DateTime m_Started;
        private TimeSpan m_Duration;
        private HouseRaffleExpireAction m_ExpireAction;
        private int m_TicketPrice;
        private List<RaffleEntry> m_Entries;
        [Constructable]
        public HouseRaffleStone()
            : base(0xEDD)
        {
            this.m_Region = null;
            this.m_Bounds = new Rectangle2D();
            this.m_Facet = null;

            this.m_Winner = null;
            this.m_Deed = null;

            this.m_State = HouseRaffleState.Inactive;
            this.m_Started = DateTime.MinValue;
            this.m_Duration = DefaultDuration;
            this.m_ExpireAction = HouseRaffleExpireAction.None;
            this.m_TicketPrice = DefaultTicketPrice;

            this.m_Entries = new List<RaffleEntry>();

            this.Movable = false;

            m_AllStones.Add(this);
        }

        public HouseRaffleStone(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public HouseRaffleState CurrentState
        {
            get
            {
                return this.m_State;
            }
            set
            {
                if (this.m_State != value)
                {
                    if (value == HouseRaffleState.Active)
                    {
                        this.m_Entries.Clear();
                        this.m_Winner = null;
                        this.m_Deed = null;
                        this.m_Started = DateTime.UtcNow;
                    }

                    this.m_State = value;
                    this.InvalidateProperties();
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public Rectangle2D PlotBounds
        {
            get
            {
                return this.m_Bounds;
            }
            set
            {
                this.m_Bounds = value;

                this.InvalidateRegion();
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public Map PlotFacet
        {
            get
            {
                return this.m_Facet;
            }
            set
            {
                this.m_Facet = value;

                this.InvalidateRegion();
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public Mobile Winner
        {
            get
            {
                return this.m_Winner;
            }
            set
            {
                this.m_Winner = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public HouseRaffleDeed Deed
        {
            get
            {
                return this.m_Deed;
            }
            set
            {
                this.m_Deed = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public DateTime Started
        {
            get
            {
                return this.m_Started;
            }
            set
            {
                this.m_Started = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public TimeSpan Duration
        {
            get
            {
                return this.m_Duration;
            }
            set
            {
                this.m_Duration = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsExpired
        {
            get
            {
                if (this.m_State != HouseRaffleState.Completed)
                    return false;

                return (this.m_Started + this.m_Duration + ExpirationTime <= DateTime.UtcNow);
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public HouseRaffleExpireAction ExpireAction
        {
            get
            {
                return this.m_ExpireAction;
            }
            set
            {
                this.m_ExpireAction = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Seer)]
        public int TicketPrice
        {
            get
            {
                return this.m_TicketPrice;
            }
            set
            {
                this.m_TicketPrice = Math.Max(0, value);
                this.InvalidateProperties();
            }
        }
        public List<RaffleEntry> Entries
        {
            get
            {
                return this.m_Entries;
            }
        }
        public override string DefaultName
        {
            get
            {
                return "a house raffle stone";
            }
        }
        public override bool DisplayWeight
        {
            get
            {
                return false;
            }
        }
        public static void CheckEnd_OnTick()
        {
            for (int i = 0; i < m_AllStones.Count; i++)
                m_AllStones[i].CheckEnd();
        }

        public static void Initialize()
        {
            for (int i = m_AllStones.Count - 1; i >= 0; i--)
            {
                HouseRaffleStone stone = m_AllStones[i];

                if (stone.IsExpired)
                {
                    switch ( stone.ExpireAction )
                    {
                        case HouseRaffleExpireAction.HideStone:
                            {
                                if (stone.Visible)
                                {
                                    stone.Visible = false;
                                    stone.ItemID = 0x1B7B; // Non-blocking ItemID
                                }

                                break;
                            }
                        case HouseRaffleExpireAction.DeleteStone:
                            {
                                stone.Delete();
                                break;
                            }
                    }
                }
            }

            Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0), new TimerCallback(CheckEnd_OnTick));
        }

        public static string FormatLocation(Point3D loc, Map map, bool displayMap)
        {
            StringBuilder result = new StringBuilder();

            int xLong = 0, yLat = 0;
            int xMins = 0, yMins = 0;
            bool xEast = false, ySouth = false;

            if (Sextant.Format(loc, map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                result.AppendFormat("{0}°{1}'{2},{3}°{4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
            else
                result.AppendFormat("{0},{1}", loc.X, loc.Y);

            if (displayMap)
                result.AppendFormat(" ({0})", map);

            return result.ToString();
        }

        public bool ValidLocation()
        {
            return (this.m_Bounds.Start != Point2D.Zero && this.m_Bounds.End != Point2D.Zero && this.m_Facet != null && this.m_Facet != Map.Internal);
        }

        public Point3D GetPlotCenter()
        {
            int x = this.m_Bounds.X + this.m_Bounds.Width / 2;
            int y = this.m_Bounds.Y + this.m_Bounds.Height / 2;
            int z = (this.m_Facet == null) ? 0 : this.m_Facet.GetAverageZ(x, y);

            return new Point3D(x, y, z);
        }

        public string FormatLocation()
        {
            if (!this.ValidLocation())
                return "no location set";

            return FormatLocation(this.GetPlotCenter(), this.m_Facet, true);
        }

        public string FormatPrice()
        {
            if (this.m_TicketPrice == 0)
                return "FREE";
            else
                return String.Format("{0} gold", this.m_TicketPrice);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.ValidLocation())
                list.Add(this.FormatLocation());

            switch (this.m_State)
            {
                case HouseRaffleState.Active:
                    {
                        list.Add(1060658, "ticket price\t{0}", this.FormatPrice()); // ~1_val~: ~2_val~
                        list.Add(1060659, "ends\t{0}", this.m_Started + this.m_Duration); // ~1_val~: ~2_val~
                        break;
                    }
                case HouseRaffleState.Completed:
                    {
                        list.Add(1060658, "winner\t{0}", (this.m_Winner == null) ? "unknown" : this.m_Winner.Name); // ~1_val~: ~2_val~
                        break;
                    }
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            switch (this.m_State)
            {
                case HouseRaffleState.Active:
                    {
                        this.LabelTo(from, 1060658, String.Format("Ends\t{0}", this.m_Started + this.m_Duration)); // ~1_val~: ~2_val~
                        break;
                    }
                case HouseRaffleState.Completed:
                    {
                        this.LabelTo(from, 1060658, String.Format("Winner\t{0}", (this.m_Winner == null) ? "Unknown" : this.m_Winner.Name)); // ~1_val~: ~2_val~
                        break;
                    }
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.AccessLevel >= AccessLevel.Seer)
            {
                list.Add(new EditEntry(from, this));

                if (this.m_State == HouseRaffleState.Inactive)
                    list.Add(new ActivateEntry(from, this));
                else
                    list.Add(new ManagementEntry(from, this));
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_State != HouseRaffleState.Active || !from.CheckAlive())
                return;

            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            if (this.HasEntered(from))
            {
                from.SendMessage(MessageHue, "You have already entered this plot's raffle.");
            }
            else if (this.IsAtIPLimit(from))
            {
                from.SendMessage(MessageHue, "You may not enter this plot's raffle.");
            }
            else
            {
                from.SendGump(new WarningGump(1150470, 0x7F00, String.Format("You are about to purchase a raffle ticket for the house plot located at {0}.  The ticket price is {1}.  Tickets are non-refundable and you can only purchase one ticket per account.  Do you wish to continue?", this.FormatLocation(), this.FormatPrice()), 0xFFFFFF, 420, 280, new WarningGumpCallback(Purchase_Callback), null)); // CONFIRM TICKET PURCHASE
            }
        }

        public void Purchase_Callback(Mobile from, bool okay, object state)
        {
            if (this.Deleted || this.m_State != HouseRaffleState.Active || !from.CheckAlive() || this.HasEntered(from) || this.IsAtIPLimit(from))
                return;

            Account acc = from.Account as Account;

            if (acc == null)
                return;

            if (okay)
            {
                Container bank = from.FindBankNoCreate();

                if (this.m_TicketPrice == 0 || (from.Backpack != null && from.Backpack.ConsumeTotal(typeof(Gold), this.m_TicketPrice)) || (bank != null && bank.ConsumeTotal(typeof(Gold), this.m_TicketPrice)))
                {
                    this.m_Entries.Add(new RaffleEntry(from));

                    from.SendMessage(MessageHue, "You have successfully entered the plot's raffle.");
                }
                else
                {
                    from.SendMessage(MessageHue, "You do not have the {0} required to enter the raffle.", this.FormatPrice());
                }
            }
            else
            {
                from.SendMessage(MessageHue, "You have chosen not to enter the raffle.");
            }
        }

        public void CheckEnd()
        {
            if (this.m_State != HouseRaffleState.Active || this.m_Started + this.m_Duration > DateTime.UtcNow)
                return;

            this.m_State = HouseRaffleState.Completed;

            if (this.m_Region != null && this.m_Entries.Count != 0)
            {
                int winner = Utility.Random(this.m_Entries.Count);

                this.m_Winner = this.m_Entries[winner].From;

                if (this.m_Winner != null)
                {
                    this.m_Deed = new HouseRaffleDeed(this, this.m_Winner);

                    this.m_Winner.SendMessage(MessageHue, "Congratulations, {0}!  You have won the raffle for the plot located at {1}.", this.m_Winner.Name, this.FormatLocation());

                    if (this.m_Winner.AddToBackpack(this.m_Deed))
                    {
                        this.m_Winner.SendMessage(MessageHue, "The writ of lease has been placed in your backpack.");
                    }
                    else
                    {
                        this.m_Winner.BankBox.DropItem(this.m_Deed);
                        this.m_Winner.SendMessage(MessageHue, "As your backpack is full, the writ of lease has been placed in your bank box.");
                    }
                }
            }

            this.InvalidateProperties();
        }

        public override void OnDelete()
        {
            if (this.m_Region != null)
            {
                this.m_Region.Unregister();
                this.m_Region = null;
            }

            m_AllStones.Remove(this);

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3); // version

            writer.WriteEncodedInt((int)this.m_State);
            writer.WriteEncodedInt((int)this.m_ExpireAction);

            writer.Write(this.m_Deed);

            writer.Write(this.m_Bounds);
            writer.Write(this.m_Facet);

            writer.Write(this.m_Winner);

            writer.Write(this.m_TicketPrice);
            writer.Write(this.m_Started);
            writer.Write(this.m_Duration);

            writer.Write(this.m_Entries.Count);

            foreach (RaffleEntry entry in this.m_Entries)
                entry.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 3:
                    {
                        this.m_State = (HouseRaffleState)reader.ReadEncodedInt();

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_ExpireAction = (HouseRaffleExpireAction)reader.ReadEncodedInt();

                        goto case 1;
                    }
                case 1:
                    {
                        this.m_Deed = reader.ReadItem<HouseRaffleDeed>();

                        goto case 0;
                    }
                case 0:
                    {
                        bool oldActive = (version < 3) ? reader.ReadBool() : false;

                        this.m_Bounds = reader.ReadRect2D();
                        this.m_Facet = reader.ReadMap();

                        this.m_Winner = reader.ReadMobile();

                        this.m_TicketPrice = reader.ReadInt();
                        this.m_Started = reader.ReadDateTime();
                        this.m_Duration = reader.ReadTimeSpan();

                        int entryCount = reader.ReadInt();
                        this.m_Entries = new List<RaffleEntry>(entryCount);

                        for (int i = 0; i < entryCount; i++)
                        {
                            RaffleEntry entry = new RaffleEntry(reader, version);

                            if (entry.From == null)
                                continue; // Character was deleted

                            this.m_Entries.Add(entry);
                        }

                        this.InvalidateRegion();

                        m_AllStones.Add(this);

                        if (version < 3)
                        {
                            if (oldActive)
                                this.m_State = HouseRaffleState.Active;
                            else if (this.m_Winner != null)
                                this.m_State = HouseRaffleState.Completed;
                            else
                                this.m_State = HouseRaffleState.Inactive;
                        }

                        break;
                    }
            }
        }

        private void InvalidateRegion()
        {
            if (this.m_Region != null)
            {
                this.m_Region.Unregister();
                this.m_Region = null;
            }

            if (this.ValidLocation())
            {
                this.m_Region = new HouseRaffleRegion(this);
                this.m_Region.Register();
            }
        }

        private bool HasEntered(Mobile from)
        {
            Account acc = from.Account as Account;

            if (acc == null)
                return false;

            foreach (RaffleEntry entry in this.m_Entries)
            {
                if (entry.From != null)
                {
                    Account entryAcc = entry.From.Account as Account;

                    if (entryAcc == acc)
                        return true;
                }
            }

            return false;
        }

        private bool IsAtIPLimit(Mobile from)
        {
            if (from.NetState == null)
                return false;

            IPAddress address = from.NetState.Address;
            int tickets = 0;

            foreach (RaffleEntry entry in this.m_Entries)
            {
                if (Utility.IPMatchClassC(entry.Address, address))
                {
                    if (++tickets >= EntryLimitPerIP)
                        return true;
                }
            }

            return false;
        }

        private class RaffleContextMenuEntry : ContextMenuEntry
        {
            protected readonly Mobile m_From;
            protected readonly HouseRaffleStone m_Stone;
            public RaffleContextMenuEntry(Mobile from, HouseRaffleStone stone, int label)
                : base(label)
            {
                this.m_From = from;
                this.m_Stone = stone;
            }
        }

        private class EditEntry : RaffleContextMenuEntry
        {
            public EditEntry(Mobile from, HouseRaffleStone stone)
                : base(from, stone, 5101)// Edit
            {
            }

            public override void OnClick()
            {
                if (this.m_Stone.Deleted || this.m_From.AccessLevel < AccessLevel.Seer)
                    return;

                this.m_From.SendGump(new PropertiesGump(this.m_From, this.m_Stone));
            }
        }

        private class ActivateEntry : RaffleContextMenuEntry
        {
            public ActivateEntry(Mobile from, HouseRaffleStone stone)
                : base(from, stone, 5113)// Start
            {
                if (!stone.ValidLocation())
                    this.Flags |= Network.CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if (this.m_Stone.Deleted || this.m_From.AccessLevel < AccessLevel.Seer || !this.m_Stone.ValidLocation())
                    return;

                this.m_Stone.CurrentState = HouseRaffleState.Active;
            }
        }

        private class ManagementEntry : RaffleContextMenuEntry
        {
            public ManagementEntry(Mobile from, HouseRaffleStone stone)
                : base(from, stone, 5032)// Game Monitor
            {
            }

            public override void OnClick()
            {
                if (this.m_Stone.Deleted || this.m_From.AccessLevel < AccessLevel.Seer)
                    return;

                this.m_From.SendGump(new HouseRaffleManagementGump(this.m_Stone));
            }
        }
    }
}