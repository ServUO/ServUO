using System;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Regions;
using Server.Targeting;

namespace Server.Multis
{
    public abstract class BaseBoatDeed : Item
    {
        private int m_MultiID;
        private Point3D m_Offset;
        public BaseBoatDeed(int id, Point3D offset)
            : base(0x14F2)
        {
            this.Weight = 1.0;

            if (!Core.AOS)
                this.LootType = LootType.Newbied;

            this.m_MultiID = id;
            this.m_Offset = offset;
        }

        public BaseBoatDeed(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MultiID
        {
            get
            {
                return this.m_MultiID;
            }
            set
            {
                this.m_MultiID = value;
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
        public abstract BaseBoat Boat { get; }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_MultiID);
            writer.Write(this.m_Offset);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_MultiID = reader.ReadInt();
                        this.m_Offset = reader.ReadPoint3D();

                        break;
                    }
            }

            if (this.Weight == 0.0)
                this.Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (from.AccessLevel < AccessLevel.GameMaster && (from.Map == Map.Ilshenar || from.Map == Map.Malas))
            {
                from.SendLocalizedMessage(1010567, null, 0x25); // You may not place a boat from this location.
            }
            else
            {
                if (Core.SE)
                    from.SendLocalizedMessage(502482); // Where do you wish to place the ship?
                else
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502482); // Where do you wish to place the ship?

                from.Target = new InternalTarget(this);
            }
        }

        public void OnPlacement(Mobile from, Point3D p)
        {
            if (this.Deleted)
            {
                return;
            }
            else if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                Map map = from.Map;

                if (map == null)
                    return;

                if (from.AccessLevel < AccessLevel.GameMaster && (map == Map.Ilshenar || map == Map.Malas))
                {
                    from.SendLocalizedMessage(1043284); // A ship can not be created here.
                    return;
                }

                if (from.Region.IsPartOf(typeof(HouseRegion)) || BaseBoat.FindBoatAt(from, from.Map) != null)
                {
                    from.SendLocalizedMessage(1010568, null, 0x25); // You may not place a ship while on another ship or inside a house.
                    return;
                }

                BaseBoat boat = this.Boat;

                if (boat == null)
                    return;

                p = new Point3D(p.X - this.m_Offset.X, p.Y - this.m_Offset.Y, p.Z - this.m_Offset.Z);

                if (BaseBoat.IsValidLocation(p, map) && boat.CanFit(p, map, boat.ItemID))
                {
                    this.Delete();

                    boat.Owner = from;
                    boat.Anchored = true;

                    uint keyValue = boat.CreateKeys(from);

                    if (boat.PPlank != null)
                        boat.PPlank.KeyValue = keyValue;

                    if (boat.SPlank != null)
                        boat.SPlank.KeyValue = keyValue;

                    boat.MoveToWorld(p, map);
                }
                else
                {
                    boat.Delete();
                    from.SendLocalizedMessage(1043284); // A ship can not be created here.
                }
            }
        }

        private class InternalTarget : MultiTarget
        {
            private readonly BaseBoatDeed m_Deed;
            public InternalTarget(BaseBoatDeed deed)
                : base(deed.MultiID, deed.Offset)
            {
                this.m_Deed = deed;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D ip = o as IPoint3D;

                if (ip != null)
                {
                    if (ip is Item)
                        ip = ((Item)ip).GetWorldTop();

                    Point3D p = new Point3D(ip);

                    Region region = Region.Find(p, from.Map);

                    if (region.IsPartOf(typeof(DungeonRegion)))
                        from.SendLocalizedMessage(502488); // You can not place a ship inside a dungeon.
                    else if (region.IsPartOf(typeof(HouseRegion)) || region.IsPartOf(typeof(ChampionSpawnRegion)))
                        from.SendLocalizedMessage(1042549); // A boat may not be placed in this area.
                    else
                        this.m_Deed.OnPlacement(from, p);
                }
            }
        }
    }
}