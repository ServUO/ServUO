using System;
using Server.ContextMenus;
using Server.Items;
using Server.Multis;

namespace Server.ACC.YS
{
    public class YardStair : YardItem
    {
        #region Properties
        private int m_DefaultID;
        public int DefaultID
        {
            get { return m_DefaultID; }
            set { m_DefaultID = value; }
        }

        //private Mobile m_Placer;
        //public Mobile Placer
        //{
        //    get { return m_Placer; }
        //    set { m_Placer = value; }
        //}

        //private int m_Price;
        //public int Price
        //{
        //    get { return m_Price; }
        //    set { m_Price = value; }
        //}

        //private BaseHouse m_House;
        //public BaseHouse House
        //{
        //    get { return m_House; }
        //    set { m_House = value; }
        //}
        #endregion

        #region Constructors
        [Constructable]
        public YardStair(Mobile placer, int defaultID, Point3D loc, int price, BaseHouse house)
            : base(defaultID, placer, "Stairs", loc, price, house)
        {
            DefaultID = defaultID;
            //Placer = placer;
            //Name = placer.Name + "'s Yard";

            //Light = LightType.Circle150;

            //Movable = false;
            //MoveToWorld(loc, placer.Map);
            //if (house == null)
            //{
            //    FindHouseOfPlacer();
            //}
            //else
            //{
            //    House = house;
            //}
        }

        public YardStair(Serial serial)
            : base(serial)
        {
        }
        #endregion

        #region Overrides
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write((int)DefaultID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            DefaultID = reader.ReadInt();
        }

        public override void GetContextMenuEntries(Mobile from, System.Collections.Generic.List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            if (Placer == null || from == Placer || from.AccessLevel >= AccessLevel.GameMaster)
            {
                list.Add(new StairRefundEntry(from, this, Price));
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 10))
            {
                if (Placer == null || from == Placer || from.AccessLevel >= AccessLevel.GameMaster)
                {
                    if (YardRegistry.YardStairIDGroups.ContainsKey(DefaultID) && YardRegistry.YardStairIDGroups[DefaultID] != null && YardRegistry.YardStairIDGroups[DefaultID].Length > 0)
                    {
                        int index;
                        for (index = 0; index < YardRegistry.YardStairIDGroups[DefaultID].Length; index++)
                        {
                            if (YardRegistry.YardStairIDGroups[DefaultID][index] == ItemID)
                            {
                                break;
                            }
                        }
                        ItemID = (index == YardRegistry.YardStairIDGroups[DefaultID].Length - 1 ? YardRegistry.YardStairIDGroups[DefaultID][0] : YardRegistry.YardStairIDGroups[DefaultID][index+1]);
                    }
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
        #endregion
    }
}