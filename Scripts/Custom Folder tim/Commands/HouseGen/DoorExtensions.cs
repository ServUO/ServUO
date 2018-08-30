using Server.Items;

namespace Server.Multis
{
    public static class DoorExtensions
    {
        internal enum Doorswing
        {
            MetalSouthWest = 1653,
            MetalSouthEast = 1655,
            MetalEastSouth = 1661,
            MetalEastNorth = 1663,

            WeaveSouthWest = 1685,
            WeaveSouthEast = 1687,
            WeaveEastSouth = 1693,
            WeaveEastNorth = 1695,

            DarkSouthWest = 1701,
            DarkSouthEast = 1703,
            DarkEastSouth = 1709,
            DarkEastNorth = 1711,

            LightSouthWest = 1749,
            LightSouthEast = 1751,
            LightEastSouth = 1757,
            LightEastNorth = 1759,

            WMSouthWest = 1765,
            WMSouthEast = 1767,
            WMEastSouth = 1773,
            WMEastNorth = 1775
        }

        public static void AddDoorEx(this BaseHouse house, int doorType, int x, int y, int z, uint keyValue)
        {
            GenericHouseDoor ghd = null;

            var ds = (Doorswing)doorType;
            switch (ds)
            {
                case Doorswing.MetalSouthWest:
                    ghd = new GenericHouseDoor(DoorFacing.WestCW, 1653, 0xEC, 0xF3);
                    break;
                case Doorswing.MetalSouthEast:
                    ghd = new GenericHouseDoor(DoorFacing.EastCCW, 1653, 0xEC, 0xF3);
                    break;
                case Doorswing.MetalEastSouth:
                    ghd = new GenericHouseDoor(DoorFacing.SouthCW, 1653, 0xEC, 0xF3);
                    break;
                case Doorswing.MetalEastNorth:
                    ghd = new GenericHouseDoor(DoorFacing.NorthCCW, 1653, 0xEC, 0xF3);
                    break;
                //--
                case Doorswing.WeaveSouthWest:
                    ghd = new GenericHouseDoor(DoorFacing.WestCW, 1685, 0x69E, 0xF2);
                    break;
                case Doorswing.WeaveSouthEast:
                    ghd = new GenericHouseDoor(DoorFacing.EastCCW, 1685, 0x69E, 0xF2);
                    break;
                case Doorswing.WeaveEastSouth:
                    ghd = new GenericHouseDoor(DoorFacing.SouthCW, 1685, 0x69E, 0xF2);
                    break;
                case Doorswing.WeaveEastNorth:
                    ghd = new GenericHouseDoor(DoorFacing.NorthCCW, 1685, 0x69E, 0xF2);
                    break;
                //--
                case Doorswing.DarkSouthWest:
                    ghd = new GenericHouseDoor(DoorFacing.WestCW, 1701, 0xEA, 0xF1);
                    break;
                case Doorswing.DarkSouthEast:
                    ghd = new GenericHouseDoor(DoorFacing.EastCCW, 1701, 0xEA, 0xF1);
                    break;
                case Doorswing.DarkEastSouth:
                    ghd = new GenericHouseDoor(DoorFacing.SouthCW, 1701, 0xEA, 0xF1);
                    break;
                case Doorswing.DarkEastNorth:
                    ghd = new GenericHouseDoor(DoorFacing.NorthCCW, 1701, 0xEA, 0xF1);
                    break;
                //--
                case Doorswing.LightSouthWest:
                    ghd = new GenericHouseDoor(DoorFacing.WestCW, 1749, 0xEA, 0xF1);
                    break;
                case Doorswing.LightSouthEast:
                    ghd = new GenericHouseDoor(DoorFacing.EastCCW, 1749, 0xEA, 0xF1);
                    break;
                case Doorswing.LightEastSouth:
                    ghd = new GenericHouseDoor(DoorFacing.SouthCW, 1749, 0xEA, 0xF1);
                    break;
                case Doorswing.LightEastNorth:
                    ghd = new GenericHouseDoor(DoorFacing.NorthCCW, 1749, 0xEA, 0xF1);
                    break;
                //--
                case Doorswing.WMSouthWest:
                    ghd = new GenericHouseDoor(DoorFacing.WestCW, 1765, 0xEA, 0xF1);
                    break;
                case Doorswing.WMSouthEast:
                    ghd = new GenericHouseDoor(DoorFacing.EastCCW, 1765, 0xEA, 0xF1);
                    break;
                case Doorswing.WMEastSouth:
                    ghd = new GenericHouseDoor(DoorFacing.SouthCW, 1765, 0xEA, 0xF1);
                    break;
                case Doorswing.WMEastNorth:
                    ghd = new GenericHouseDoor(DoorFacing.NorthCCW, 1765, 0xEA, 0xF1);
                    break;
            }

            ghd.KeyValue = keyValue;

            house.AddDoor(ghd, x, y, z);
        }
        public static void AddDoorsEx(this BaseHouse house, int doorType, int x, int y, int z, uint keyValue)
        {
            GenericHouseDoor ghd = null;
            GenericHouseDoor ghd2 = null;
            bool east = false;
            bool rev = false;
            var ds = (Doorswing)doorType;

            switch (ds)
            {
                case Doorswing.MetalSouthWest:
                    ghd = new GenericHouseDoor(DoorFacing.WestCW, 1653, 0xEC, 0xF3);
                    ghd2 = new GenericHouseDoor(DoorFacing.EastCCW, 1653, 0xEC, 0xF3);
                    break;
                case Doorswing.MetalEastSouth:
                    ghd = new GenericHouseDoor(DoorFacing.SouthCW, 1653, 0xEC, 0xF3);
                    ghd2 = new GenericHouseDoor(DoorFacing.NorthCCW, 1653, 0xEC, 0xF3);
                    east = true;
                    rev = true;
                    break;
                //--
                case Doorswing.WeaveSouthWest:
                    ghd = new GenericHouseDoor(DoorFacing.WestCW, 1685, 0x69E, 0xF2);
                    ghd2 = new GenericHouseDoor(DoorFacing.EastCCW, 1685, 0x69E, 0xF2);
                    break;
                case Doorswing.WeaveEastSouth:
                    ghd = new GenericHouseDoor(DoorFacing.SouthCW, 1685, 0x69E, 0xF2);
                    ghd2 = new GenericHouseDoor(DoorFacing.NorthCCW, 1685, 0x69E, 0xF2);
                    east = true;
                    rev = true;
                    break;
                //--
                case Doorswing.DarkSouthWest:
                    ghd = new GenericHouseDoor(DoorFacing.WestCW, 1701, 0xEA, 0xF1);
                    ghd2 = new GenericHouseDoor(DoorFacing.EastCCW, 1701, 0xEA, 0xF1);
                    break;
                case Doorswing.DarkEastSouth:
                    ghd = new GenericHouseDoor(DoorFacing.SouthCW, 1701, 0xEA, 0xF1);
                    ghd2 = new GenericHouseDoor(DoorFacing.NorthCCW, 1701, 0xEA, 0xF1);
                    east = true;
                    rev = true;
                    break;
                //--
                case Doorswing.LightSouthWest:
                    ghd = new GenericHouseDoor(DoorFacing.WestCW, 1749, 0xEA, 0xF1);
                    ghd2 = new GenericHouseDoor(DoorFacing.EastCCW, 1749, 0xEA, 0xF1);
                    break;
                case Doorswing.LightEastSouth:
                    ghd = new GenericHouseDoor(DoorFacing.SouthCW, 1749, 0xEA, 0xF1);
                    ghd2 = new GenericHouseDoor(DoorFacing.NorthCCW, 1749, 0xEA, 0xF1);
                    east = true;
                    rev = true;
                    break;
                //--
                case Doorswing.WMSouthWest:
                    ghd = new GenericHouseDoor(DoorFacing.WestCW, 1765, 0xEA, 0xF1);
                    ghd2 = new GenericHouseDoor(DoorFacing.EastCCW, 1765, 0xEA, 0xF1);
                    break;
                case Doorswing.WMEastSouth:
                    ghd = new GenericHouseDoor(DoorFacing.SouthCW, 1765, 0xEA, 0xF1);
                    ghd2 = new GenericHouseDoor(DoorFacing.NorthCCW, 1765, 0xEA, 0xF1);
                    east = true;
                    rev = true;
                    break;
                case Doorswing.MetalSouthEast:
                    ghd2 = new GenericHouseDoor(DoorFacing.WestCW, 1653, 0xEC, 0xF3);
                    ghd = new GenericHouseDoor(DoorFacing.EastCCW, 1653, 0xEC, 0xF3);
                    rev = true;
                    break;
                case Doorswing.MetalEastNorth:
                    ghd2 = new GenericHouseDoor(DoorFacing.SouthCW, 1653, 0xEC, 0xF3);
                    ghd = new GenericHouseDoor(DoorFacing.NorthCCW, 1653, 0xEC, 0xF3);
                    east = true;
                    break;
                case Doorswing.WeaveSouthEast:
                    ghd2 = new GenericHouseDoor(DoorFacing.WestCW, 1685, 0x69E, 0xF2);
                    ghd = new GenericHouseDoor(DoorFacing.EastCCW, 1685, 0x69E, 0xF2);
                    rev = true;
                    break;
                case Doorswing.WeaveEastNorth:
                    ghd2 = new GenericHouseDoor(DoorFacing.SouthCW, 1685, 0x69E, 0xF2);
                    ghd = new GenericHouseDoor(DoorFacing.NorthCCW, 1685, 0x69E, 0xF2);
                    east = true;
                    break;
                case Doorswing.DarkSouthEast:
                    ghd2 = new GenericHouseDoor(DoorFacing.WestCW, 1701, 0xEA, 0xF1);
                    ghd = new GenericHouseDoor(DoorFacing.EastCCW, 1701, 0xEA, 0xF1);
                    rev = true;
                    break;
                case Doorswing.DarkEastNorth:
                    ghd2 = new GenericHouseDoor(DoorFacing.SouthCW, 1701, 0xEA, 0xF1);
                    ghd = new GenericHouseDoor(DoorFacing.NorthCCW, 1701, 0xEA, 0xF1);
                    east = true;
                    break;
                case Doorswing.LightSouthEast:
                    ghd2 = new GenericHouseDoor(DoorFacing.WestCW, 1749, 0xEA, 0xF1);
                    ghd = new GenericHouseDoor(DoorFacing.EastCCW, 1749, 0xEA, 0xF1);
                    rev = true;
                    break;
                case Doorswing.LightEastNorth:
                    ghd2 = new GenericHouseDoor(DoorFacing.SouthCW, 1749, 0xEA, 0xF1);
                    ghd = new GenericHouseDoor(DoorFacing.NorthCCW, 1749, 0xEA, 0xF1);
                    east = true;
                    break;
                case Doorswing.WMSouthEast:
                    ghd2 = new GenericHouseDoor(DoorFacing.WestCW, 1765, 0xEA, 0xF1);
                    ghd = new GenericHouseDoor(DoorFacing.EastCCW, 1765, 0xEA, 0xF1);
                    rev = true;
                    break;
                case Doorswing.WMEastNorth:
                    ghd2 = new GenericHouseDoor(DoorFacing.SouthCW, 1765, 0xEA, 0xF1);
                    ghd = new GenericHouseDoor(DoorFacing.NorthCCW, 1765, 0xEA, 0xF1);
                    east = true;
                    break;
            }

            ghd.Link = ghd2;
            ghd2.Link = ghd;

            ghd.KeyValue = keyValue;
            ghd2.KeyValue = keyValue;

            house.AddDoor(ghd, x, y, z);
            var ox = x + (!east ? (rev ? -1 : 1) : 0);
            var oy = y + (east ? (rev ? -1 : 1) : 0);

            house.AddDoor(ghd2, ox, oy, z);
        }
    }
}
