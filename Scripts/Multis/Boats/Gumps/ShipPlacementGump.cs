using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.Targeting;

namespace Server.Gumps
{
    public class BoatPlacementGump : Gump
    {
        private readonly Item m_Item;
        private readonly Mobile m_From;

        public BoatPlacementGump(Item item, Mobile from)
            : base(0, 0)
        {
            m_From = from;
            m_Item = item;

            AddPage(0);

            AddBackground(0, 0, 220, 170, 0x13BE);
            AddBackground(10, 10, 200, 150, 0xBB8);

            AddHtmlLocalized(20, 20, 180, 70, 1116329, true, false); // Select the ship direction for placement.

            AddHtmlLocalized(55, 100, 50, 25, 1116330, false, false); // WEST
            AddButton(20, 100, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);

            AddHtmlLocalized(150, 100, 50, 25, 1116331, false, false); // NORTH
            AddButton(115, 100, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(55, 125, 50, 25, 1116332, false, false); // SOUTH
            AddButton(20, 125, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(150, 125, 50, 25, 1116333, false, false); // EAST
            AddButton(115, 125, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int id = 0;
            Point3D offset = Point3D.Zero;
            Direction direction;

            if (info.ButtonID == 0)
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
            private readonly Item m_Item;
            private readonly Direction m_Facing;
            private readonly int m_ItemID;

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
                    else if (region.IsPartOf<HouseRegion>() || region.IsPartOf<Engines.CannedEvil.ChampionSpawnRegion>())
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
