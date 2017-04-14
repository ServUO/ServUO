using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Network;
using Server.Multis;
using Server.Regions;

namespace Server.Gumps
{
    public class BoatPlacementGump : Gump
    {
        private Item m_Item;
        private Mobile m_From;

        public BoatPlacementGump(Item item, Mobile from) : base(0, 0)
        {
            m_From = from;
            m_Item = item;

            AddBackground(0, 0, 220, 180, 9200);
            AddBackground(10, 10, 200, 160, 3000);
            AddBackground(20, 20, 180, 80, 3000);

            AddButton(20, 105, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddButton(115, 105, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddButton(20, 135, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddButton(115, 135, 4005, 4007, 4, GumpButtonType.Reply, 0);

            AddHtmlLocalized(30, 30, 170, 50, 1116329, false, false);

            AddLabel(55, 105, 0, "WEST");
            AddLabel(150, 105, 0, "NORTH");
            AddLabel(55, 135, 0, "SOUTH");
            AddLabel(150, 135, 0, "EAST");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int id = 0;
            Point3D offset = Point3D.Zero;
            Direction direction;

            if(info.ButtonID == 0)
                return;

            switch (info.ButtonID)
            {
                default:
                case 1: //North
                    direction = Direction.West; break;
                case 2: //East
                    direction = Direction.North; break;
                case 3: //South
                    direction = Direction.South; break;
                case 4: //West
                    direction = Direction.East; break;
            }

            if (m_Item is BaseBoatDeed)
            {
                id = BaseBoat.GetID(((BaseBoatDeed)m_Item).MultiID, direction);
                offset = ((BaseBoatDeed)m_Item).Offset;
            }
            else if (m_Item is BaseDockedBoat)
            {
                id = BaseBoat.GetID(((BaseDockedBoat)m_Item).MultiID, direction);
                offset = ((BaseDockedBoat)m_Item).Offset;
            }

            m_From.Target = new InternalTarget(id, offset, m_Item, direction);
        }

        private class InternalTarget : MultiTarget
        {
            private Item m_Item;
            private Direction m_Facing;
            private int m_ItemID;

            public InternalTarget(int itemID, Point3D offset, Item item, Direction facing)
                : base(itemID, offset)
            {
                m_Item = item;
                m_Facing = facing;
                m_ItemID = itemID;
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

                    if (region.IsPartOf<DungeonRegion>())
                        from.SendLocalizedMessage(502488); // You can not place a ship inside a dungeon.
                    else if (region.IsPartOf<HouseRegion>() || region.IsPartOf<Server.Engines.CannedEvil.ChampionSpawnRegion>())
                        from.SendLocalizedMessage(1042549); // A boat may not be placed in this area.
                    else
                    {
                        if (m_Item is BaseBoatDeed)
                            ((BaseBoatDeed)m_Item).OnPlacement(from, p, m_ItemID, m_Facing);
                        else if (m_Item is BaseDockedBoat)
                            ((BaseDockedBoat)m_Item).OnPlacement(from, p, m_ItemID, m_Facing);
                    }
                }
            }

        }
    }
}