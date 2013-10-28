using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public abstract class BaseDoor : Item, ILockable, ITelekinesisable
    {
        private static readonly Point3D[] m_Offsets = new Point3D[]
        {
            new Point3D(-1, 1, 0),
            new Point3D(1, 1, 0),
            new Point3D(-1, 0, 0),
            new Point3D(1,-1, 0),
            new Point3D(1, 1, 0),
            new Point3D(1,-1, 0),
            new Point3D(0, 0, 0),
            new Point3D(0,-1, 0),
            new Point3D(0, 0, 0),
            new Point3D(0, 0, 0),
            new Point3D(0, 0, 0),
            new Point3D(0, 0, 0)
        };
        private bool m_Open, m_Locked;
        private int m_OpenedID, m_OpenedSound;
        private int m_ClosedID, m_ClosedSound;
        private Point3D m_Offset;
        private BaseDoor m_Link;
        private uint m_KeyValue;
        private Timer m_Timer;
        public BaseDoor(int closedID, int openedID, int openedSound, int closedSound, Point3D offset)
            : base(closedID)
        {
            this.m_OpenedID = openedID;
            this.m_ClosedID = closedID;
            this.m_OpenedSound = openedSound;
            this.m_ClosedSound = closedSound;
            this.m_Offset = offset;

            this.m_Timer = new InternalTimer(this);

            this.Movable = false;
        }

        public BaseDoor(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Locked
        {
            get
            {
                return this.m_Locked;
            }
            set
            {
                this.m_Locked = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public uint KeyValue
        {
            get
            {
                return this.m_KeyValue;
            }
            set
            {
                this.m_KeyValue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Open
        {
            get
            {
                return this.m_Open;
            }
            set
            {
                if (this.m_Open != value)
                {
                    this.m_Open = value;

                    this.ItemID = this.m_Open ? this.m_OpenedID : this.m_ClosedID;

                    if (this.m_Open)
                        this.Location = new Point3D(this.X + this.m_Offset.X, this.Y + this.m_Offset.Y, this.Z + this.m_Offset.Z);
                    else
                        this.Location = new Point3D(this.X - this.m_Offset.X, this.Y - this.m_Offset.Y, this.Z - this.m_Offset.Z);

                    Effects.PlaySound(this, this.Map, this.m_Open ? this.m_OpenedSound : this.m_ClosedSound);

                    if (this.m_Open)
                        this.m_Timer.Start();
                    else
                        this.m_Timer.Stop();
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int OpenedID
        {
            get
            {
                return this.m_OpenedID;
            }
            set
            {
                this.m_OpenedID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ClosedID
        {
            get
            {
                return this.m_ClosedID;
            }
            set
            {
                this.m_ClosedID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int OpenedSound
        {
            get
            {
                return this.m_OpenedSound;
            }
            set
            {
                this.m_OpenedSound = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ClosedSound
        {
            get
            {
                return this.m_ClosedSound;
            }
            set
            {
                this.m_ClosedSound = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset
        {
            get
            {
                return this.m_Offset;
            }
            set
            {
                this.m_Offset = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseDoor Link
        {
            get
            {
                if (this.m_Link != null && this.m_Link.Deleted)
                    this.m_Link = null;

                return this.m_Link;
            }
            set
            {
                this.m_Link = value;
            }
        }
        public virtual bool UseChainedFunctionality
        {
            get
            {
                return false;
            }
        }
        // Called by RunUO
        public static void Initialize()
        {
            EventSink.OpenDoorMacroUsed += new OpenDoorMacroEventHandler(EventSink_OpenDoorMacroUsed);

            CommandSystem.Register("Link", AccessLevel.GameMaster, new CommandEventHandler(Link_OnCommand));
            CommandSystem.Register("ChainLink", AccessLevel.GameMaster, new CommandEventHandler(ChainLink_OnCommand));
        }

        public static Point3D GetOffset(DoorFacing facing)
        {
            return m_Offsets[(int)facing];
        }

        public bool CanClose()
        {
            if (!this.m_Open)
                return true;

            Map map = this.Map;

            if (map == null)
                return false;

            Point3D p = new Point3D(this.X - this.m_Offset.X, this.Y - this.m_Offset.Y, this.Z - this.m_Offset.Z);

            return this.CheckFit(map, p, 16);
        }

        public List<BaseDoor> GetChain()
        {
            List<BaseDoor> list = new List<BaseDoor>();
            BaseDoor c = this;

            do
            {
                list.Add(c);
                c = c.Link;
            }
            while (c != null && !list.Contains(c));

            return list;
        }

        public bool IsFreeToClose()
        {
            if (!this.UseChainedFunctionality)
                return this.CanClose();

            List<BaseDoor> list = this.GetChain();

            bool freeToClose = true;

            for (int i = 0; freeToClose && i < list.Count; ++i)
                freeToClose = list[i].CanClose();

            return freeToClose;
        }

        public void OnTelekinesis(Mobile from)
        {
            Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
            Effects.PlaySound(this.Location, this.Map, 0x1F5);

            this.Use(from);
        }

        public virtual bool IsInside(Mobile from)
        {
            return false;
        }

        public virtual bool UseLocks()
        {
            return true;
        }

        public virtual void Use(Mobile from)
        {
            if (this.m_Locked && !this.m_Open && this.UseLocks())
            {
                if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502502); // That is locked, but you open it with your godly powers.
                    //from.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x3B2, 3, 502502, "", "" ) ); // That is locked, but you open it with your godly powers.
                }
                else if (Key.ContainsKey(from.Backpack, this.KeyValue))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501282); // You quickly unlock, open, and relock the door
                }
                else if (this.IsInside(from))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501280); // That is locked, but is usable from the inside.
                }
                else
                {
                    if (this.Hue == 0x44E && this.Map == Map.Malas) // doom door into healer room in doom
                        this.SendLocalizedMessageTo(from, 1060014); // Only the dead may pass.
                    else
                        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502503); // That is locked.

                    return;
                }
            }

            if (this.m_Open && !this.IsFreeToClose())
                return;

            if (this.m_Open)
                this.OnClosed(from);
            else
                this.OnOpened(from);

            if (this.UseChainedFunctionality)
            {
                bool open = !this.m_Open;

                List<BaseDoor> list = this.GetChain();

                for (int i = 0; i < list.Count; ++i)
                    list[i].Open = open;
            }
            else
            {
                this.Open = !this.m_Open;

                BaseDoor link = this.Link;

                if (this.m_Open && link != null && !link.Open)
                    link.Open = true;
            }
        }

        public virtual void OnOpened(Mobile from)
        {
        }

        public virtual void OnClosed(Mobile from)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.IsPlayer() && (/*!from.InLOS( this ) || */!from.InRange(this.GetWorldLocation(), 2)))
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else
                this.Use(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_KeyValue);

            writer.Write(this.m_Open);
            writer.Write(this.m_Locked);
            writer.Write(this.m_OpenedID);
            writer.Write(this.m_ClosedID);
            writer.Write(this.m_OpenedSound);
            writer.Write(this.m_ClosedSound);
            writer.Write(this.m_Offset);
            writer.Write(this.m_Link);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_KeyValue = reader.ReadUInt();
                        this.m_Open = reader.ReadBool();
                        this.m_Locked = reader.ReadBool();
                        this.m_OpenedID = reader.ReadInt();
                        this.m_ClosedID = reader.ReadInt();
                        this.m_OpenedSound = reader.ReadInt();
                        this.m_ClosedSound = reader.ReadInt();
                        this.m_Offset = reader.ReadPoint3D();
                        this.m_Link = reader.ReadItem() as BaseDoor;

                        this.m_Timer = new InternalTimer(this);

                        if (this.m_Open)
                            this.m_Timer.Start();

                        break;
                    }
            }
        }

        [Usage("Link")]
        [Description("Links two targeted doors together.")]
        private static void Link_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(Link_OnFirstTarget));
            e.Mobile.SendMessage("Target the first door to link.");
        }

        private static void Link_OnFirstTarget(Mobile from, object targeted)
        {
            BaseDoor door = targeted as BaseDoor;

            if (door == null)
            {
                from.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(Link_OnFirstTarget));
                from.SendMessage("That is not a door. Try again.");
            }
            else
            {
                from.BeginTarget(-1, false, TargetFlags.None, new TargetStateCallback(Link_OnSecondTarget), door);
                from.SendMessage("Target the second door to link.");
            }
        }

        private static void Link_OnSecondTarget(Mobile from, object targeted, object state)
        {
            BaseDoor first = (BaseDoor)state;
            BaseDoor second = targeted as BaseDoor;

            if (second == null)
            {
                from.BeginTarget(-1, false, TargetFlags.None, new TargetStateCallback(Link_OnSecondTarget), first);
                from.SendMessage("That is not a door. Try again.");
            }
            else
            {
                first.Link = second;
                second.Link = first;
                from.SendMessage("The doors have been linked.");
            }
        }

        [Usage("ChainLink")]
        [Description("Chain-links two or more targeted doors together.")]
        private static void ChainLink_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetStateCallback(ChainLink_OnTarget), new List<BaseDoor>());
            e.Mobile.SendMessage("Target the first of a sequence of doors to link.");
        }

        private static void ChainLink_OnTarget(Mobile from, object targeted, object state)
        {
            BaseDoor door = targeted as BaseDoor;

            if (door == null)
            {
                from.BeginTarget(-1, false, TargetFlags.None, new TargetStateCallback(ChainLink_OnTarget), state);
                from.SendMessage("That is not a door. Try again.");
            }
            else
            {
                List<BaseDoor> list = (List<BaseDoor>)state;

                if (list.Count > 0 && list[0] == door)
                {
                    if (list.Count >= 2)
                    {
                        for (int i = 0; i < list.Count; ++i)
                            list[i].Link = list[(i + 1) % list.Count];

                        from.SendMessage("The chain of doors have been linked.");
                    }
                    else
                    {
                        from.BeginTarget(-1, false, TargetFlags.None, new TargetStateCallback(ChainLink_OnTarget), state);
                        from.SendMessage("You have not yet targeted two unique doors. Target the second door to link.");
                    }
                }
                else if (list.Contains(door))
                {
                    from.BeginTarget(-1, false, TargetFlags.None, new TargetStateCallback(ChainLink_OnTarget), state);
                    from.SendMessage("You have already targeted that door. Target another door, or retarget the first door to complete the chain.");
                }
                else
                {
                    list.Add(door);

                    from.BeginTarget(-1, false, TargetFlags.None, new TargetStateCallback(ChainLink_OnTarget), state);

                    if (list.Count == 1)
                        from.SendMessage("Target the second door to link.");
                    else
                        from.SendMessage("Target another door to link. To complete the chain, retarget the first door.");
                }
            }
        }

        private static void EventSink_OpenDoorMacroUsed(OpenDoorMacroEventArgs args)
        {
            Mobile m = args.Mobile;

            if (m.Map != null)
            {
                int x = m.X, y = m.Y;

                switch ( m.Direction & Direction.Mask )
                {
                    case Direction.North:
                        --y;
                        break;
                    case Direction.Right:
                        ++x;
                        --y;
                        break;
                    case Direction.East:
                        ++x;
                        break;
                    case Direction.Down:
                        ++x;
                        ++y;
                        break;
                    case Direction.South:
                        ++y;
                        break;
                    case Direction.Left:
                        --x;
                        ++y;
                        break;
                    case Direction.West:
                        --x;
                        break;
                    case Direction.Up:
                        --x;
                        --y;
                        break;
                }

                Sector sector = m.Map.GetSector(x, y);

                foreach (Item item in sector.Items)
                {
                    if (item.Location.X == x && item.Location.Y == y && (item.Z + item.ItemData.Height) > m.Z && (m.Z + 16) > item.Z && item is BaseDoor && m.CanSee(item) && m.InLOS(item))
                    {
                        if (m.CheckAlive())
                        {
                            m.SendLocalizedMessage(500024); // Opening door...
                            item.OnDoubleClick(m);
                        }

                        break;
                    }
                }
            }
        }

        private bool CheckFit(Map map, Point3D p, int height)
        {
            if (map == Map.Internal)
                return false;

            int x = p.X;
            int y = p.Y;
            int z = p.Z;

            Sector sector = map.GetSector(x, y);
            List<Item> items = sector.Items;
            List<Mobile> mobs = sector.Mobiles;

            for (int i = 0; i < items.Count; ++i)
            {
                Item item = items[i];

                if (!(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint(x, y) && !(item is BaseDoor))
                {
                    ItemData id = item.ItemData;
                    bool surface = id.Surface;
                    bool impassable = id.Impassable;

                    if ((surface || impassable) && (item.Z + id.CalcHeight) > z && (z + height) > item.Z)
                        return false;
                }
            }

            for (int i = 0; i < mobs.Count; ++i)
            {
                Mobile m = mobs[i];

                if (m.Location.X == x && m.Location.Y == y)
                {
                    if (m.Hidden && m.IsPlayer())
                        continue;

                    if (!m.Alive)
                        continue;

                    if ((m.Z + 16) > z && (z + height) > m.Z)
                        return false;
                }
            }

            return true;
        }

        private class InternalTimer : Timer
        {
            private readonly BaseDoor m_Door;
            public InternalTimer(BaseDoor door)
                : base(TimeSpan.FromSeconds(20.0), TimeSpan.FromSeconds(10.0))
            {
                this.Priority = TimerPriority.OneSecond;
                this.m_Door = door;
            }

            protected override void OnTick()
            {
                if (this.m_Door.Open && this.m_Door.IsFreeToClose())
                    this.m_Door.Open = false;
            }
        }
    }
}