using Server.Items;

namespace Server.Multis
{
    public class OrcishGalleon : BaseGalleon
    {
        public override int NorthID => 0x18 + (DamageValue * 4);
        public override int EastID => 0x19 + (DamageValue * 4);
        public override int SouthID => 0x1A + (DamageValue * 4);
        public override int WestID => 0x1B + (DamageValue * 4);

        public override int HoldDistance => 9;
        public override int TillerManDistance => 6;
        public override int RuneOffset => -3;

        public override int WheelDistance => 7;
        public override int CaptiveOffset => 4;
        public override int MaxCannons => 7;
        public override double CannonDamageMod => 1.3;

        public override double TurnDelay => 2;
        public override int MaxHits => 100000;

        public override int ZSurface => 14;

        public override BaseDockedBoat DockedBoat => new DockedOrcishGalleon(this);

        [Constructable]
        public OrcishGalleon() : this(Direction.North) { }

        [Constructable]
        public OrcishGalleon(Direction facing) : base(facing)
        {
        }

        public override int[] CannonTileIDs => m_CannonTileIDs;
        private static readonly int[] m_CannonTileIDs = new int[]
        {

            30012, 30037, 30041, 30065, 30069, 30093, 30097, //SOUTH
            30512, 30537, 30541, 30565, 30569, 30593, 30597, //WEST
            31012, 31037, 31041, 31065, 31069, 31093, 31097, //NORTH
            31512, 31537, 31541, 31565, 31569, 31593, 31597, //EAST

            31712, 31737, 31741, 31765, 31769, 31793, 31797, //SOUTH D1
            31913, 31938, 31942, 31966, 31970, 31994, 31998, //WEST  D1
            32112, 32137, 32141, 32165, 32169, 32193, 32197, //NORTH D1
            32312, 32337, 32341, 32365, 32369, 32393, 32397, //EAST  D1

            32512, 32537, 32541, 32565, 32569, 32593, 32597, //NORTH D2
            32712, 32737, 32741, 32765, 32769, 32793, 32797, //WEST  D2
            32912, 32937, 32941, 32965, 32969, 32993, 32997, //NORTH D2
            33112, 33137, 33141, 33165, 33169, 33193, 33197  //NORTH D2   
        };

        public override int[] HoldIDs => m_HoldIDs;
        private static readonly int[] m_HoldIDs = new int[]
        {
            30127, 30124, 30122, 30115, 30120,  //SOUTH
            30627, 30624, 30622, 30615, 30620,  //WEST
            31127, 31124, 31122, 31115, 31120,  //NORTH
            31627, 31624, 31622, 31615, 31620,  //EAST

            31827, 31824, 31822, 31815, 31820,  //SOUTH D1
            32028, 32025, 32023, 32016, 32021,  //WEST D1
            32227, 32224, 32222, 32215, 32220,  //NORTH D1
            32427, 32424, 32422, 32415, 32420,  //EAST D1
             
            32627, 32624, 32622, 32615, 32620,  //SOUTH D2
            32827, 32824, 32822, 32815, 32820,  //WEST D2
            33027, 33024, 33022, 33015, 33020,  //NORTH D2
            33227, 33224, 33222, 33215, 33220   //EAST D1
        };

        public override int[] HoldItemIDs => m_HoldItemIDs;
        private static readonly int[] m_HoldItemIDs = new int[]
        {
            30117, 30617, 31117, 31617,
            31817, 32018, 32217, 32417,
            32617, 32817, 33017, 33217
        };

        public override int[] WheelItemIDs => m_WheelItemIDs;
        private static readonly int[] m_WheelItemIDs = new int[]
        {
            30141, 30641, 31141, 31642,
            31890, 32090, 32290, 32493,
            32692, 32891, 33090, 33290
        };

        public override ShipPosition GetCannonPosition(Point3D pnt)
        {
            int x = pnt.X; int y = pnt.Y;

            switch (Facing)
            {
                default:
                case Direction.North:
                    if (x == X && y < Y)
                        return ShipPosition.Bow;
                    if (x > X && y < Y)
                        return ShipPosition.BowStarboard;
                    if (x < X && y < Y)
                        return ShipPosition.BowPort;
                    if (x > X && y + 2 == Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x < X && y + 2 == Y)
                        return ShipPosition.AmidShipPort;
                    else if (x > X && y > Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.West:
                    if (x < X && y == Y)
                        return ShipPosition.Bow;
                    else if (x < X && y < Y)
                        return ShipPosition.BowStarboard;
                    else if (x < X && y > Y)
                        return ShipPosition.BowPort;
                    else if (x + 2 == X && y < Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x + 2 == X && y > Y)
                        return ShipPosition.AmidShipPort;
                    else if (x > X && y < Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.South:
                    if (x == X && y > Y)
                        return ShipPosition.Bow;
                    else if (x < X && y > Y)
                        return ShipPosition.BowStarboard;
                    else if (x > X && y > Y)
                        return ShipPosition.BowPort;
                    else if (x < X && y - 2 == Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x > X && y - 2 == Y)
                        return ShipPosition.AmidShipPort;
                    else if (x < X && y < Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.East:
                    if (x > X && y == Y)
                        return ShipPosition.Bow;
                    else if (x > X && y > Y)
                        return ShipPosition.BowStarboard;
                    else if (x > X && y < Y)
                        return ShipPosition.BowPort;
                    else if (x - 2 == X && y > Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x - 2 == X && y < Y)
                        return ShipPosition.AmidShipPort;
                    else if (x < X && y > Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
            }
        }

        private readonly int PortAndStarboardOffset = 3;
        private readonly int AftOffset = 12; //TODO: Get this
        private readonly int BowOffset = 9;

        public override bool Contains(int x, int y)
        {
            bool contains = base.Contains(x, y);

            if (contains)
            {
                int boatX = X;
                int boatY = Y;

                switch (Facing)
                {
                    case Direction.North:
                        if (x > boatX + PortAndStarboardOffset || x < boatX - PortAndStarboardOffset)
                            return false;
                        if (y > boatY + AftOffset || y < boatY - BowOffset)
                            return false;
                        return true;
                    case Direction.South:
                        if (x > boatX + PortAndStarboardOffset || x < boatX - PortAndStarboardOffset)
                            return false;
                        if (y > boatY + BowOffset || y < boatY - AftOffset)
                            return false;
                        return true;
                    case Direction.East:
                        if (x > boatX + BowOffset || x < boatX - AftOffset)
                            return false;
                        if (y > boatY + PortAndStarboardOffset || y < boatY - PortAndStarboardOffset)
                            return false;
                        return true;
                    case Direction.West:
                        if (x > boatX + AftOffset || x < boatX - BowOffset)
                            return false;
                        if (y > boatY + PortAndStarboardOffset || y < boatY - PortAndStarboardOffset)
                            return false;
                        return true;
                }
            }

            return contains;
        }

        public override bool ExemptOverheadComponent(Point3D newPnt, int itemID, int x, int y, int height)
        {
            //if(height > this.Z + 16) //TODO: Get height of mast
            //	return false;

            switch (itemID)
            {
                case 0x18: //North/South ID's
                case 0x1A:
                case 0x1C:
                case 0x1E:
                case 0x20:
                case 0x22:
                    if (x > newPnt.X + PortAndStarboardOffset || x < newPnt.X - PortAndStarboardOffset)
                        return true;
                    break;
                case 0x19: //East/West ID's
                case 0x1B:
                case 0x1D:
                case 0x1F:
                case 0x21:
                case 0x23:
                    if (y > newPnt.Y + PortAndStarboardOffset || y < newPnt.Y - PortAndStarboardOffset)
                        return true;
                    break;
            }

            return base.ExemptOverheadComponent(newPnt, itemID, x, y, height);
        }

        public OrcishGalleon(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class OrcishGalleonDeed : BaseBoatDeed
    {
        public override int LabelNumber => 1116738;  // small dragon ship deed
        public override BaseBoat Boat => new OrcishGalleon(BoatDirection);

        [Constructable]
        public OrcishGalleonDeed()
            : base(0x18, Point3D.Zero)
        {
        }

        public OrcishGalleonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }
    }

    public class DockedOrcishGalleon : BaseDockedBoat
    {
        public override int LabelNumber => 1116747;  //Orc Ship
        public override BaseBoat Boat => new OrcishGalleon(BoatDirection);

        public DockedOrcishGalleon(BaseBoat boat)
            : base(0x18, Point3D.Zero, boat)
        {
        }

        public DockedOrcishGalleon(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }
    }
}
