using Server.Items;
using System;
using System.Linq;

namespace Server.Multis
{
    public class BritannianShip : BaseGalleon
    {
        public override int NorthID => 0x40 + (DamageValue * 4);
        public override int EastID => 0x41 + (DamageValue * 4);
        public override int SouthID => 0x42 + (DamageValue * 4);
        public override int WestID => 0x43 + (DamageValue * 4);

        public override int HoldDistance => 9;
        public override int TillerManDistance => 7;
        public override int RuneOffset => -5;
        public override int MaxAddons => 2;

        public override int WheelDistance => 3;
        public override int CaptiveOffset => 5;
        public override int MaxCannons => 9;
        public override int MaxHits => 200000;

        public override double TurnDelay => 3;
        public override TimeSpan BoatDecayDelay => TimeSpan.FromDays(30);
        public override int ZSurface => 18;

        public override BaseDockedBoat DockedBoat => new DockedBritannianShip(this);

        public override int DamageValue
        {
            get
            {
                switch (DamageTaken)
                {
                    default:
                    case DamageLevel.Pristine:
                    case DamageLevel.Slightly:
                    case DamageLevel.Moderately: return 0;
                    case DamageLevel.Heavily:
                    case DamageLevel.Severely: return 1;
                }
            }
        }

        [Constructable]
        public BritannianShip() : this(Direction.North) { }

        [Constructable]
        public BritannianShip(Direction facing)
            : base(facing)
        {
        }

        public override int[] CannonTileIDs => m_CannonTileIDs;
        private static readonly int[] m_CannonTileIDs = new int[]
        {
            23612, 23631, 23630, 23632, 23634, 23639, 23637, 23643, 23641, //SOUTH
            23666, 23684, 23685, 23686, 23688, 23691, 23693, 23695, 23697, //WEST
            23558, 23576, 23577, 23580, 23578, 23583, 23585, 23587, 23589, //NORTH 
            23504, 23522, 23523, 23526, 23524, 23529, 23531, 23533, 23535, //EAST 

            23612, 23631, 23630, 23632, 23634, 23639, 23637, 23643, 23641, //SOUTH
            23666, 23684, 23685, 23686, 23688, 23691, 23693, 23695, 23697, //WEST
            23558, 23576, 23577, 23580, 23578, 23583, 23585, 23587, 23589, //NORTH 
            23504, 23522, 23523, 23526, 23524, 23529, 23531, 23533, 23535  //EAST 
        };

        public override int[] HoldIDs => m_HoldIDs;
        private static readonly int[] m_HoldIDs = new int[]
        {
            23649, 23650, 23652, 23653, 23654, 23655, 23656, 23651,  //SOUTH
            23703, 23704, 23706, 23707, 23708, 23709, 23710, 23705,  //WEST
            23595, 23596, 23598, 23599, 23600, 23601, 23602, 23597,  //NORTH
            23541, 23542, 23544, 23545, 23546, 23547, 23548, 23543,  //EAST

            23649, 23650, 23652, 23653, 23654, 23655, 23656, 23651,  //SOUTH
            23703, 23704, 23706, 23707, 23708, 23709, 23710, 23705,  //WEST
            23595, 23596, 23598, 23599, 23600, 23601, 23602, 23597,  //NORTH
            23541, 23542, 23544, 23545, 23546, 23547, 23548, 23543   //EAST
        };

        public override int[] HoldItemIDs => m_HoldItemIDs;
        private static readonly int[] m_HoldItemIDs = new int[]
        {
            23648, 23702, 23594, 23540,
            23648, 23702, 23594, 23540,
        };

        public override int[] WheelItemIDs => m_WheelItemIDs;
        private static readonly int[] m_WheelItemIDs = new int[]
        {

            23618, 23672, 23564, 23510,
            25154, 25208, 25100, 25046
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
                    else if (x > X && y + 5 == Y)
                        return ShipPosition.BowStarboard;
                    else if (x < X && y + 5 == Y)
                        return ShipPosition.BowPort;
                    else if (x > X && y + 1 == Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x < X && y + 1 == Y)
                        return ShipPosition.AmidShipPort;
                    else if (x > X && y - 3 == Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x < X && y - 3 == Y)
                        return ShipPosition.AmidShipPort;
                    else if (x > X && y - 3 > Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.West:
                    if (x < X && y == Y)
                        return ShipPosition.Bow;
                    else if (x + 5 == X && y < Y)
                        return ShipPosition.BowStarboard;
                    else if (x + 5 == X && y > Y)
                        return ShipPosition.BowPort;
                    else if (x + 1 == X && y < Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x + 1 == X && y > Y)
                        return ShipPosition.AmidShipPort;
                    else if (x - 3 == X && y < Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x - 3 == X && y > Y)
                        return ShipPosition.AmidShipPort;
                    else if (x - 3 > X && y < Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.South:
                    if (x == X && y > Y)
                        return ShipPosition.Bow;
                    else if (x < X && y - 5 == Y)
                        return ShipPosition.BowStarboard;
                    else if (x > X && y - 5 == Y)
                        return ShipPosition.BowPort;
                    else if (x < X && y - 1 == Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x > X && y - 1 == Y)
                        return ShipPosition.AmidShipPort;
                    else if (x < X && y + 3 == Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x > X && y + 3 == Y)
                        return ShipPosition.AmidShipPort;
                    else if (x < X && y + 3 < Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.East:
                    if (x > X && y == Y)
                        return ShipPosition.Bow;
                    else if (x - 5 == X && y > Y)
                        return ShipPosition.BowStarboard;
                    else if (x - 5 == X && y < Y)
                        return ShipPosition.BowPort;
                    else if (x - 1 == X && y > Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x - 1 == X && y < Y)
                        return ShipPosition.AmidShipPort;
                    else if (x + 3 == X && y > Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x + 3 == X && y < Y)
                        return ShipPosition.AmidShipPort;
                    else if (x + 3 < X && y > Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
            }
        }

        private readonly int PortAndStarboardOffset = 4;
        private readonly int AftOffset = 18;
        private readonly int BowOffset = 17;

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
            //if (height > this.Z + 16) //TODO: Get height of mast
            //    return false;

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

        public BritannianShip(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                Timer.DelayCall(() =>
                {
                    var deckItem = Fixtures.FirstOrDefault(f => m_WheelItemIDs.Any(listID => listID == f.ItemID));

                    if (deckItem != null)
                    {
                        ShipWheel wheel = new ShipWheel(this, deckItem.ItemID);
                        AddFixture(wheel);

                        wheel.MoveToWorld(new Point3D(deckItem.X, deckItem.Y, deckItem.Z), deckItem.Map);

                        deckItem.Delete();
                        RemoveFixture(deckItem);
                    }
                });
            }
        }
    }

    public class BritannianShipDeed : BaseBoatDeed
    {
        public override int LabelNumber => 1150017;
        public override BaseBoat Boat => new BritannianShip(BoatDirection);

        [Constructable]
        public BritannianShipDeed()
            : base(0x40, Point3D.Zero)
        {
        }

        public BritannianShipDeed(Serial serial)
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

    public class DockedBritannianShip : BaseDockedBoat
    {
        public override int LabelNumber => 1150100;  //Britannian Ship
        public override BaseBoat Boat => new BritannianShip(BoatDirection);

        public DockedBritannianShip(BaseBoat boat)
            : base(0x40, Point3D.Zero, boat)
        {
        }

        public DockedBritannianShip(Serial serial)
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
