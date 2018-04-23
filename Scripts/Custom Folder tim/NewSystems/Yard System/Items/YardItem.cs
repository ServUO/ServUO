using System;
using System.Collections.Generic;
using Server.Items;
using Server.Multis;

namespace Server.ACC.YS
{
    public class YardMultiInfo
    {
        public int ItemID;
        public Point3D Offset;

        public YardMultiInfo(int itemID, Point3D offset)
        {
            ItemID = itemID;
            Offset = offset;
        }
    }

    public class YardItem : YardPiece
    {
        #region Properties
        private Mobile m_Placer;
        public Mobile Placer
        {
            get { return m_Placer; }
            set { m_Placer = value; }
        }

        private int m_Price;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        private BaseHouse m_House;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseHouse House
        {
            get { return m_House; }
            set { m_House = value; }
        }

        private List<YardPiece> m_Pieces;
        public List<YardPiece> Pieces
        {
            get
            {
                if (m_Pieces == null)
                {
                    m_Pieces = new List<YardPiece>();
                }
                return m_Pieces;
            }
            set { m_Pieces = value; }
        }
        #endregion

        #region Constructors
        public YardItem(int itemID, Mobile from, string itemName, Point3D location, int price, BaseHouse house)
            : base(itemID, from.Name + "'s " + itemName)
        {
            Price = price;
            Placer = from;

            Movable = false;
            HasMoved = true;
            MoveToWorld(location, from.Map);

            if (house == null)
            {
                FindHouseOfPlacer();
            }
            else
            {
                House = house;
            }

            Pieces = new List<YardPiece>();
            ParentYardItem = this;
            Pieces.Add(this);

            if (YardRegistry.YardMultiIDs.ContainsKey(ItemID) && YardRegistry.YardMultiIDs[ItemID] != null)
            {
                YardPiece piece;
                foreach (YardMultiInfo info in YardRegistry.YardMultiIDs[ItemID])
                {
                    piece = new YardPiece(info.ItemID, Name, this);
                    piece.HasMoved = true;
                    piece.MoveToWorld(new Point3D(Location.X + info.Offset.X,
                                                  Location.Y + info.Offset.Y,
                                                  Location.Z + info.Offset.Z),
                                                  from.Map);
                    Pieces.Add(piece);
                }
            }

            for (int i = 0; i < Pieces.Count; i++)
            {
                Pieces[i].HasMoved = false;
            }
        }

        public YardItem(Serial serial)
            : base(serial)
        {
        }
        #endregion

        #region Overrides
        public override void OnAfterDelete()
        {
            for (int i = 0; i < Pieces.Count; ++i)
            {
                Pieces[i].Delete();
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 10))
            {
                if (Placer == null || from == Placer || from.AccessLevel >= AccessLevel.GameMaster)
                {
                    Refund();
                }
                else
                {
                    from.SendMessage("Stay out of my yard!");
                }
            }
            else
            {
                from.SendMessage("The item is too far away");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            //Version 1
            if (House == null || House.Deleted)
            {
                writer.Write(false);
                YardSystem.AddOrphanedItem(this);
            }
            else
            {
                writer.Write(true);
                writer.Write(House);
            }

            //Version 0
            writer.WriteMobile(Placer);
            writer.Write(Price);
            writer.WriteItemList(Pieces);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        if (reader.ReadBool())
                        {
                            House = reader.ReadItem() as BaseHouse;
                        }
                        goto case 0;
                    }
                case 0:
                    {
                        Placer = reader.ReadMobile();
                        Price = reader.ReadInt();

                        Pieces = new List<YardPiece>();
                        foreach (YardPiece item in reader.ReadItemList())
                        {
                            Pieces.Add(item);
                        }
                        break;
                    }
            }

            if (House == null)
            {
                FindHouseOfPlacer();
                if (House == null)
                {
                    Refund();                    
                }
            }
        }
        #endregion

        #region Methods
        public void Refund()
        {
            Gold toGive = new Gold(Price);
            if (Placer.BankBox.TryDropItem(Placer, toGive, false))
            {
                Delete();
                Placer.SendLocalizedMessage(1060397, toGive.Amount.ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.
            }
            else
            {
                toGive.Delete();
                Placer.SendMessage("Your bankbox is full!");
            }
        }

        public void FindHouseOfPlacer()
        {
            if (Placer == null || House != null)
            {
                return;
            }

            IPooledEnumerable eable = Map.GetItemsInRange(Location, 20);
            foreach (Item item in eable)
            {
                if (item is BaseHouse)
                {
                    BaseHouse house = (BaseHouse)item;
                    if (house.Owner == Placer)
                    {
                        House = house;
                        return;
                    }
                }
            }
        }
        #endregion
    }

    public class YardPiece : Item
    {
        private YardItem m_ParentYardItem;
        public YardItem ParentYardItem
        {
            get { return m_ParentYardItem; }
            set { m_ParentYardItem = value; }
        }

        private bool m_HasMoved;
        public bool HasMoved
        {
            get { return m_HasMoved; }
            set { m_HasMoved = value; }
        }

        public YardPiece(int itemID, string name)
            : this(itemID, name, null)
        {
        }

        public YardPiece(int itemID, string name, YardItem multiParent)
            : base(itemID)
        {
            Movable = false;
            Name = name;
            ItemID = itemID;
            Light = LightType.Circle150;

            if (multiParent != null)
            {
                ParentYardItem = multiParent;
            }
        }

        public YardPiece(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (ParentYardItem != null)
            {
                ParentYardItem.OnAfterDelete();
            }
            else
            {
                base.OnAfterDelete();
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (ParentYardItem != null)
            {
                ParentYardItem.OnDoubleClick(from);
            }
            else
            {
                base.OnDoubleClick(from);
            }
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (HasMoved)
            {
                base.OnLocationChange(oldLocation);
                return;
            }

            int xOff = 0, yOff = 0, zOff = 0;

            xOff = Location.X - oldLocation.X;
            yOff = Location.Y - oldLocation.Y;
            zOff = Location.Z - oldLocation.Z;

            if (ParentYardItem != null && ParentYardItem.Pieces != null)
            {
                HasMoved = true;

                for (int i = 0; i < ParentYardItem.Pieces.Count; i++)
                {
                    if (!ParentYardItem.Pieces[i].HasMoved)
                    {
                        ParentYardItem.Pieces[i].HasMoved = true;
                        ParentYardItem.Pieces[i].MoveToWorld(new Point3D(ParentYardItem.Pieces[i].Location.X + xOff,
                                                                      ParentYardItem.Pieces[i].Location.Y + yOff,
                                                                      ParentYardItem.Pieces[i].Location.Z + zOff),
                                                                      Map);
                    }
                }

                for (int i = 0; i < ParentYardItem.Pieces.Count; i++)
                {
                    ParentYardItem.Pieces[i].HasMoved = false;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(ParentYardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        ParentYardItem = reader.ReadItem() as YardItem;
                        break;
                    }
            }
        }
    }
}