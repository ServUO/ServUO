using System;
using Server.Engines.CannedEvil;
using Server.Regions;
using Server.Targeting;

namespace Server.Multis
{
    public abstract class BaseDockedBoat : Item
    {
        private int m_MultiID;
        private Point3D m_Offset;
        private string m_ShipName;
        public BaseDockedBoat(int id, Point3D offset, BaseBoat boat)
            : base(0x14F4)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;

            this.m_MultiID = id;
            this.m_Offset = offset;

            this.m_ShipName = boat.ShipName;
        }

        public BaseDockedBoat(Serial serial)
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
        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipName
        {
            get
            {
                return this.m_ShipName;
            }
            set
            {
                this.m_ShipName = value;
                this.InvalidateProperties();
            }
        }
        public abstract BaseBoat Boat { get; }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(this.m_MultiID);
            writer.Write(this.m_Offset);
            writer.Write(this.m_ShipName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        this.m_MultiID = reader.ReadInt();
                        this.m_Offset = reader.ReadPoint3D();
                        this.m_ShipName = reader.ReadString();

                        if (version == 0)
                            reader.ReadUInt();

                        break;
                    }
            }

            if (this.LootType == LootType.Newbied)
                this.LootType = LootType.Blessed;

            if (this.Weight == 0.0)
                this.Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                from.SendLocalizedMessage(502482); // Where do you wish to place the ship?

                from.Target = new InternalTarget(this);
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (this.m_ShipName != null)
                list.Add(this.m_ShipName);
            else
                base.AddNameProperty(list);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.m_ShipName != null)
                this.LabelTo(from, this.m_ShipName);
            else
                base.OnSingleClick(from);
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

                BaseBoat boat = this.Boat;

                if (boat == null)
                    return;

                p = new Point3D(p.X - this.m_Offset.X, p.Y - this.m_Offset.Y, p.Z - this.m_Offset.Z);

                if (BaseBoat.IsValidLocation(p, map) && boat.CanFit(p, map, boat.ItemID) && map != Map.Ilshenar && map != Map.Malas)
                {
                    this.Delete();

                    boat.Owner = from;
                    boat.Anchored = true;
                    boat.ShipName = this.m_ShipName;

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
            private readonly BaseDockedBoat m_Model;
            public InternalTarget(BaseDockedBoat model)
                : base(model.MultiID, model.Offset)
            {
                this.m_Model = model;
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
                        this.m_Model.OnPlacement(from, p);
                }
            }
        }
    }
}