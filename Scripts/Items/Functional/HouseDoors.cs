using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;

namespace Server.Items
{
    public class MetalHouseDoor : BaseHouseDoor
    {
        [Constructable]
        public MetalHouseDoor(DoorFacing facing)
            : base(facing, 0x675 + (2 * (int)facing), 0x676 + (2 * (int)facing), 0xEC, 0xF3, BaseDoor.GetOffset(facing))
        {
        }

        public MetalHouseDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DarkWoodHouseDoor : BaseHouseDoor
    {
        [Constructable]
        public DarkWoodHouseDoor(DoorFacing facing)
            : base(facing, 0x6A5 + (2 * (int)facing), 0x6A6 + (2 * (int)facing), 0xEA, 0xF1, BaseDoor.GetOffset(facing))
        {
        }

        public DarkWoodHouseDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GenericHouseDoor : BaseHouseDoor
    {
        [Constructable]
        public GenericHouseDoor(DoorFacing facing, int baseItemID, int openedSound, int closedSound)
            : this(facing, baseItemID, openedSound, closedSound, true)
        {
        }

        [Constructable]
        public GenericHouseDoor(DoorFacing facing, int baseItemID, int openedSound, int closedSound, bool autoAdjust)
            : base(facing, baseItemID + (autoAdjust ? (2 * (int)facing) : 0), baseItemID + 1 + (autoAdjust ? (2 * (int)facing) : 0), openedSound, closedSound, BaseDoor.GetOffset(facing))
        {
        }

        public GenericHouseDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public abstract class BaseHouseDoor : BaseDoor, ISecurable
    {
        private DoorFacing m_Facing;
        private SecureLevel m_Level;
        public BaseHouseDoor(DoorFacing facing, int closedID, int openedID, int openedSound, int closedSound, Point3D offset)
            : base(closedID, openedID, openedSound, closedSound, offset)
        {
            m_Facing = facing;
            m_Level = SecureLevel.Anyone;
        }

        public BaseHouseDoor(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DoorFacing Facing
        {
            get
            {
                return m_Facing;
            }
            set
            {
                m_Facing = value;
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
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public BaseHouse FindHouse()
        {
            Point3D loc;

            if (Open)
                loc = new Point3D(X - Offset.X, Y - Offset.Y, Z - Offset.Z);
            else
                loc = Location;

            return BaseHouse.FindHouseAt(loc, Map, 20);
        }

        public bool CheckAccess(Mobile m)
        {
            BaseHouse house = FindHouse();

            if (house == null)
                return false;

            if (!house.IsAosRules)
                return true;

            if (house.Public ? house.IsBanned(m) : !house.HasAccess(m))
                return false;

            return house.HasSecureAccess(m, m_Level);
        }

        public override void OnOpened(Mobile from)
        {
            BaseHouse house = FindHouse();

            if (house != null && house.IsFriend(from) && from.IsPlayer() && house.RefreshDecay())
                from.SendLocalizedMessage(1043293); // Your house's age and contents have been refreshed.

            if (!Core.AOS && house != null && house.Public && !house.IsFriend(from))
                house.AddVisit(from);
        }

        public override bool UseLocks()
        {
            BaseHouse house = FindHouse();

            return (house == null || !house.IsAosRules);
        }

        public override void Use(Mobile from)
        {
            if (!CheckAccess(from))
                from.SendLocalizedMessage(1061637); // You are not allowed to access 
            else
                base.Use(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)m_Level);

            writer.Write((int)m_Facing);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        m_Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 1)
                            m_Level = SecureLevel.Anyone;

                        m_Facing = (DoorFacing)reader.ReadInt();
                        break;
                    }
            }
        }

        public override bool IsInside(Mobile from)
        {
            int x,y,w,h;

            const int r = 2;
            const int bs = r * 2 + 1;
            const int ss = r + 1;

            switch ( m_Facing )
            {
                case DoorFacing.WestCW:
                case DoorFacing.EastCCW:
                    x = -r;
                    y = -r;
                    w = bs;
                    h = ss;
                    break;
                case DoorFacing.EastCW: 
                case DoorFacing.WestCCW:
                    x = -r;
                    y = 0;
                    w = bs;
                    h = ss;
                    break;
                case DoorFacing.SouthCW:
                case DoorFacing.NorthCCW:
                    x = -r;
                    y = -r;
                    w = ss;
                    h = bs;
                    break;
                case DoorFacing.NorthCW:
                case DoorFacing.SouthCCW:
                    x = 0;
                    y = -r;
                    w = ss;
                    h = bs;
                    break;
                    //No way to test the 'insideness' of SE Sliding doors on OSI, so leaving them default to false until furthur information gained

                default:
                    return false;
            }

            int rx = from.X - X;
            int ry = from.Y - Y;
            int az = Math.Abs(from.Z - Z);

            return (rx >= x && rx < (x + w) && ry >= y && ry < (y + h) && az <= 4);
        }
    }
}