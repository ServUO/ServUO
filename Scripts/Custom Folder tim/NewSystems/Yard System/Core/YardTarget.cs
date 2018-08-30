using System;
using Server;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using System.Collections;
using System.Text;
using Server.Targeting;
using Server.Misc;
using Server.Multis;
using Server.Gumps;

namespace Server.ACC.YS
{
    public class YardTarget : Target
    {
        private Mobile m_From;
        private int m_SelectedID;
        private int m_Price;
        private YardShovel m_Shovel;
        private BaseHouse m_House;
        private string m_Category;
        private int m_Page;

        public YardTarget(YardShovel shovel, Mobile from, int itemID, int price, string category, int page)
            : base(-1, true, TargetFlags.None)
        {
            m_Shovel = shovel;
            m_From = from;
            m_SelectedID = itemID;
            m_Price = price;
            m_Category = category;
            m_Page = page;
            CheckLOS = false;
            m_Shovel.Category = category;
            m_Shovel.Page = page;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            IPoint3D t = targeted as IPoint3D;
            if (t == null)
                return;
            
            Point3D loc = new Point3D(t);
            if (t is StaticTarget)
                loc.Z -= TileData.ItemTable[((StaticTarget)t).ItemID & 0x3FFF].CalcHeight;

            if (!YardSettings.AllowOtherHouses && m_From.AccessLevel == AccessLevel.Player)
            {
                BaseHouse house =  BaseHouse.FindHouseAt(loc, from.Map, 20);
                if (house != null && house.Owner != m_From)
                {
                    m_From.SendMessage("You cannot place a yard item in someone else's house.");
                    GumpUp();
                    return;
                }
            }

            if (ValidatePlacement(loc))
                EndPlace(loc);
            else
                GumpUp();
        }

        public bool ValidatePlacement(Point3D loc)
        {
            Map map = m_From.Map;
            if (map == null)
                return false;
            
            m_House = BaseHouse.FindHouseAt(m_From.Location, map, 20);
            if (m_House == null || !m_House.IsOwner(m_From))
            {
                m_From.SendMessage("You must be standing in your house to place this");
                return false;
            }

            if (loc.Y > m_From.Location.Y + YardSettings.Front || loc.Y < m_From.Location.Y - YardSettings.Back)
            {
                m_From.SendMessage("This is outside of your yard. Please re-try the placement");
                return false;
            }

            if (loc.X > m_From.Location.X + YardSettings.Right || loc.X < m_From.Location.X - YardSettings.Left)
            {
                m_From.SendMessage("This is outside of your yard. Please re-try the placement");
                return false;
            }
            return true;
        }

        public void EndPlace(Point3D loc)
        {
            bool Paid = false;
            if (m_From.Backpack.ConsumeTotal(typeof(Gold), m_Price))
            {
                Paid = true;
            }
            else if (m_From.BankBox.ConsumeTotal(typeof(Gold), m_Price))
            {
                Paid = true;
            }

            if (Paid)
            {
                switch (m_SelectedID)
                {
                    //Tall Iron
                    case 2084: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.WestCW); break; }
                    case 2086: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.EastCCW); break; }
                    case 2088: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.WestCCW); break; }
                    case 2090: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.EastCW); break; }
                    case 2092: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.SouthCW); break; }
                    case 2094: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.NorthCCW); break; }
                    case 2096: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.SouthCCW); break; }
                    case 2098: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.NorthCW); break; }
                    //Short Iron
                    case 2124: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.WestCW); break; }
                    case 2126: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.EastCCW); break; }
                    case 2128: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.WestCCW); break; }
                    case 2130: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.EastCW); break; }
                    case 2132: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.SouthCW); break; }
                    case 2134: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.NorthCCW); break; }
                    case 2136: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.SouthCCW); break; }
                    case 2138: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.NorthCW); break; }
                    //Light Wood
                    case 2105: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.WestCW); break; }
                    case 2107: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.EastCCW); break; }
                    case 2109: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.WestCCW); break; }
                    case 2111: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.EastCW); break; }
                    case 2113: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.SouthCW); break; }
                    case 2115: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.NorthCCW); break; }
                    case 2117: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.SouthCCW); break; }
                    case 2119: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.NorthCW); break; }
                    //Dark Wood
                    case 2150: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.WestCW); break; }
                    case 2152: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.EastCCW); break; }
                    case 2154: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.WestCCW); break; }
                    case 2156: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.EastCW); break; }
                    case 2158: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.SouthCW); break; }
                    case 2160: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.NorthCCW); break; }
                    case 2162: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.SouthCCW); break; }
                    case 2164: { new YardGate(m_SelectedID, m_From, m_Price, m_House, loc, DoorFacing.NorthCW); break; }

                    case 5952: { new YardItem(5946, m_From, "Fountain", loc, m_Price, m_House); break; }
                    case 6610: { new YardItem(6604, m_From, "Fountain", loc, m_Price, m_House); break; }

                    default:
                        {
                            if (YardRegistry.YardStairIDGroups.ContainsKey(m_SelectedID))
                            {
                                new YardStair(m_From, m_SelectedID, loc, m_Price, m_House);
                            }
                            else
                            {
                                new YardItem(m_SelectedID, m_From, "Yard", loc, m_Price, m_House);
                            }
                            break;
                        }
                }

                GumpUp();
            }
            else
            {
                m_From.SendMessage("You do not have enough gold for that");
                GumpUp();
            }
        }

        public void GumpUp()
        {
            m_From.SendGump(new YardGump(m_From, m_Shovel, m_Category, m_Page, m_SelectedID, m_Price));
        }
    }
}