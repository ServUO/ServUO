using System;
using Server.Factions;
using Server.Multis;

namespace Server.Items
{
    public enum PlankSide
    {
        Port,
        Starboard
    }

    public class Plank : Item, ILockable
    {
        private BaseBoat m_Boat;
        private PlankSide m_Side;
        private bool m_Locked;
        private uint m_KeyValue;
        private Timer m_CloseTimer;
        public Plank(BaseBoat boat, PlankSide side, uint keyValue)
            : base(0x3EB1 + (int)side)
        {
            this.m_Boat = boat;
            this.m_Side = side;
            this.m_KeyValue = keyValue;
            this.m_Locked = true;

            this.Movable = false;
        }

        public Plank(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat Boat
        {
            get
            {
                return this.m_Boat;
            }
            set
            {
                this.m_Boat = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public PlankSide Side
        {
            get
            {
                return this.m_Side;
            }
            set
            {
                this.m_Side = value;
            }
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
        public bool IsOpen
        {
            get
            {
                return (this.ItemID == 0x3ED5 || this.ItemID == 0x3ED4 || this.ItemID == 0x3E84 || this.ItemID == 0x3E89);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Starboard
        {
            get
            {
                return (this.m_Side == PlankSide.Starboard);
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version

            writer.Write(this.m_Boat);
            writer.Write((int)this.m_Side);
            writer.Write(this.m_Locked);
            writer.Write(this.m_KeyValue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Boat = reader.ReadItem() as BaseBoat;
                        this.m_Side = (PlankSide)reader.ReadInt();
                        this.m_Locked = reader.ReadBool();
                        this.m_KeyValue = reader.ReadUInt();

                        if (this.m_Boat == null)
                            this.Delete();

                        break;
                    }
            }

            if (this.IsOpen)
            {
                this.m_CloseTimer = new CloseTimer(this);
                this.m_CloseTimer.Start();
            }
        }

        public void SetFacing(Direction dir)
        {
            if (this.IsOpen)
            {
                switch ( dir )
                {
                    case Direction.North:
                        this.ItemID = this.Starboard ? 0x3ED4 : 0x3ED5;
                        break;
                    case Direction.East:
                        this.ItemID = this.Starboard ? 0x3E84 : 0x3E89;
                        break;
                    case Direction.South:
                        this.ItemID = this.Starboard ? 0x3ED5 : 0x3ED4;
                        break;
                    case Direction.West:
                        this.ItemID = this.Starboard ? 0x3E89 : 0x3E84;
                        break;
                }
            }
            else
            {
                switch ( dir )
                {
                    case Direction.North:
                        this.ItemID = this.Starboard ? 0x3EB2 : 0x3EB1;
                        break;
                    case Direction.East:
                        this.ItemID = this.Starboard ? 0x3E85 : 0x3E8A;
                        break;
                    case Direction.South:
                        this.ItemID = this.Starboard ? 0x3EB1 : 0x3EB2;
                        break;
                    case Direction.West:
                        this.ItemID = this.Starboard ? 0x3E8A : 0x3E85;
                        break;
                }
            }
        }

        public void Open()
        {
            if (this.IsOpen || this.Deleted)
                return;

            if (this.m_CloseTimer != null)
                this.m_CloseTimer.Stop();

            this.m_CloseTimer = new CloseTimer(this);
            this.m_CloseTimer.Start();

            switch ( this.ItemID )
            {
                case 0x3EB1:
                    this.ItemID = 0x3ED5;
                    break;
                case 0x3E8A:
                    this.ItemID = 0x3E89;
                    break;
                case 0x3EB2:
                    this.ItemID = 0x3ED4;
                    break;
                case 0x3E85:
                    this.ItemID = 0x3E84;
                    break;
            }

            if (this.m_Boat != null)
                this.m_Boat.Refresh();
        }

        public override bool OnMoveOver(Mobile from)
        {
            if (this.IsOpen)
            {
                if (from is BaseFactionGuard)
                    return false;

                if ((from.Direction & Direction.Running) != 0 || (this.m_Boat != null && !this.m_Boat.Contains(from)))
                    return true;

                Map map = this.Map;

                if (map == null)
                    return false;

                int rx = 0, ry = 0;

                if (this.ItemID == 0x3ED4)
                    rx = 1;
                else if (this.ItemID == 0x3ED5)
                    rx = -1;
                else if (this.ItemID == 0x3E84)
                    ry = 1;
                else if (this.ItemID == 0x3E89)
                    ry = -1;

                for (int i = 1; i <= 6; ++i)
                {
                    int x = this.X + (i * rx);
                    int y = this.Y + (i * ry);
                    int z;

                    for (int j = -8; j <= 8; ++j)
                    {
                        z = from.Z + j;

                        if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                        {
                            if (i == 1 && j >= -2 && j <= 2)
                                return true;

                            from.Location = new Point3D(x, y, z);
                            return false;
                        }
                    }

                    z = map.GetAverageZ(x, y);

                    if (map.CanFit(x, y, z, 16, false, false) && !Server.Spells.SpellHelper.CheckMulti(new Point3D(x, y, z), map) && !Region.Find(new Point3D(x, y, z), map).IsPartOf(typeof(Factions.StrongholdRegion)))
                    {
                        if (i == 1)
                            return true;

                        from.Location = new Point3D(x, y, z);
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanClose()
        {
            Map map = this.Map;

            if (map == null || this.Deleted)
                return false;

            foreach (object o in this.GetObjectsInRange(0))
            {
                if (o != this)
                    return false;
            }

            return true;
        }

        public void Close()
        {
            if (!this.IsOpen || !this.CanClose() || this.Deleted)
                return;

            if (this.m_CloseTimer != null)
                this.m_CloseTimer.Stop();

            this.m_CloseTimer = null;

            switch ( this.ItemID )
            {
                case 0x3ED5:
                    this.ItemID = 0x3EB1;
                    break;
                case 0x3E89:
                    this.ItemID = 0x3E8A;
                    break;
                case 0x3ED4:
                    this.ItemID = 0x3EB2;
                    break;
                case 0x3E84:
                    this.ItemID = 0x3E85;
                    break;
            }

            if (this.m_Boat != null)
                this.m_Boat.Refresh();
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            this.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Boat == null)
                return;

            if (from.InRange(this.GetWorldLocation(), 8))
            {
                if (this.m_Boat.Contains(from))
                {
                    if (this.IsOpen)
                        this.Close();
                    else
                        this.Open();
                }
                else
                {
                    if (!this.IsOpen)
                    {
                        if (!this.Locked)
                        {
                            this.Open();
                        }
                        else if (from.AccessLevel >= AccessLevel.GameMaster)
                        {
                            from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502502); // That is locked but your godly powers allow access
                            this.Open();
                        }
                        else
                        {
                            from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502503); // That is locked.
                        }
                    }
                    else if (!this.Locked)
                    {
                        from.Location = new Point3D(this.X, this.Y, this.Z + 3);
                    }
                    else if (from.AccessLevel >= AccessLevel.GameMaster)
                    {
                        from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502502); // That is locked but your godly powers allow access
                        from.Location = new Point3D(this.X, this.Y, this.Z + 3);
                    }
                    else
                    {
                        from.LocalOverheadMessage(Network.MessageType.Regular, 0x00, 502503); // That is locked.
                    }
                }
            }
        }

        private class CloseTimer : Timer
        {
            private readonly Plank m_Plank;
            public CloseTimer(Plank plank)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                this.m_Plank = plank;
                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                this.m_Plank.Close();
            }
        }
    }
}