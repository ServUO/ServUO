using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;

namespace Server.Multis
{
    public class BritannianShip : BaseGalleon
    {
        public override int NorthID { get { return 0x40 + (DamageValue * 4); } }
        public override int EastID { get { return  0x41 + (DamageValue * 4); } }
        public override int SouthID { get { return 0x42 + (DamageValue * 4); } }
        public override int WestID { get { return  0x43 + (DamageValue * 4); } }

        public override int HoldDistance { get { return 9; } }
        public override int TillerManDistance { get { return 7; } }
        public override int RuneOffset { get { return -5; } }
        public override int MaxAddons { get { return 2; } }

        public override int WheelDistance { get { return 3; } }
        public override int CaptiveOffset { get { return 5; } }
        public override int MaxCannons { get { return 9; } }
        public override int MaxHits { get { return 200000; } }

        public override double TurnDelay { get { return 3; } }
        public override TimeSpan BoatDecayDelay { get { return TimeSpan.FromDays(30); } }
        public override int ZSurface { get { return 18; } }

        public override BaseDockedBoat DockedBoat { get { return new DockedBritannianShip(this); } }

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

        public override int[][] CannonTileIDs { get { return m_CannonTileIDs; } }
        private int[][] m_CannonTileIDs = new int[][]
        {  
            new int[] { 23612, 23631, 23630, 23632, 23634, 23639, 23637, 23643, 23641 }, //SOUTH
            new int[] { 23666, 23684, 23685, 23686, 23688, 23691, 23693, 23695, 23697 }, //WEST
            new int[] { 23558, 23576, 23577, 23580, 23578, 23583, 23585, 23587, 23589 }, //NORTH 
            new int[] { 23504, 23522, 23523, 23526, 23524, 23529, 23531, 23533, 23535 }, //EAST 

            new int[] { 23612, 23631, 23630, 23632, 23634, 23639, 23637, 23643, 23641 }, //SOUTH
            new int[] { 23666, 23684, 23685, 23686, 23688, 23691, 23693, 23695, 23697 }, //WEST
            new int[] { 23558, 23576, 23577, 23580, 23578, 23583, 23585, 23587, 23589 }, //NORTH 
            new int[] { 23504, 23522, 23523, 23526, 23524, 23529, 23531, 23533, 23535 }, //EAST 
        };

        public override int[][] FillerIDs { get { return m_FillerIDs; } }
        private int[][] m_FillerIDs = new int[][]
        {                 
            //          fsail  faddn  baddn
            new int[] { 23833, 23664, 23665 },   //SOUTH
            new int[] { 23887, 23718, 23719 },  //WEST
            new int[] { 23779, 23610, 23611 },  //NORTH
            new int[] { 23725, 23556, 23557 },  //EAST

            new int[] { 23833, 23664, 23665 },   //SOUTH
            new int[] { 23887, 23718, 23719 },  //WEST
            new int[] { 23779, 23610, 23611 },  //NORTH
            new int[] { 23725, 23556, 23557 },  //EAST
        };

        public override int[][] HoldIDs { get { return m_HoldIDs; } }
        private int[][] m_HoldIDs = new int[][]
        {
            new int[] { 23649, 23650, 23652, 23653, 23654, 23655, 23656, 23651},  //SOUTH
            new int[] { 23703, 23704, 23706, 23707, 23708, 23709, 23710, 23705},  //WEST
            new int[] { 23595, 23596, 23598, 23599, 23600, 23601, 23602, 23597},  //NORTH
            new int[] { 23541, 23542, 23544, 23545, 23546, 23547, 23548, 23543},  //EAST

            new int[] { 23649, 23650, 23652, 23653, 23654, 23655, 23656, 23651},  //SOUTH
            new int[] { 23703, 23704, 23706, 23707, 23708, 23709, 23710, 23705},  //WEST
            new int[] { 23595, 23596, 23598, 23599, 23600, 23601, 23602, 23597},  //NORTH
            new int[] { 23541, 23542, 23544, 23545, 23546, 23547, 23548, 23543},  //EAST
        };

        public override int[][] HoldItemIDs { get { return m_HoldItemIDs; } }
        private int[][] m_HoldItemIDs = new int[][]
        {
            new int[] { 23648 },
            new int[] { 23702 },
            new int[] { 23594 },
            new int[] { 23540 },

            new int[] { 23648 },
            new int[] { 23702 },
            new int[] { 23594 },
            new int[] { 23540 },
        };

        public override int[][] WheelItemIDs { get { return m_WheelItemIDs; } }
        private int[][] m_WheelItemIDs = new int[][]
        {
            
            new int[] { 23618 },
            new int[] { 23618 },
            new int[] { 23618 },
            new int[] { 23618 },
        };

        public override ShipPosition GetCannonPosition(Point3D pnt)
        {
            int x = pnt.X; int y = pnt.Y;

            switch (Facing)
            {
                default:
                case Direction.North:
                    if (x == this.X && y < this.Y)
                        return ShipPosition.Bow;
                    else if (x > this.X && y + 5 == this.Y)
                        return ShipPosition.BowStarboard;
                    else if (x < this.X && y + 5 == this.Y)
                        return ShipPosition.BowPort;
                    else if (x > this.X && y + 1 == this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x < this.X && y + 1 == this.Y)
                        return ShipPosition.AmidShipPort;
                    else if (x > this.X && y - 3 == this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x < this.X && y - 3 == this.Y)
                        return ShipPosition.AmidShipPort;
                    else if (x > this.X && y - 3 > this.Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.West:
                    if (x < this.X && y == this.Y)
                        return ShipPosition.Bow;
                    else if (x + 5 == this.X && y < this.Y)
                        return ShipPosition.BowStarboard;
                    else if (x + 5 == this.X && y > this.Y)
                        return ShipPosition.BowPort;
                    else if (x + 1 == this.X && y < this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x + 1 == this.X && y > this.Y)
                        return ShipPosition.AmidShipPort;
                    else if (x - 3 == this.X && y < this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x - 3 == this.X && y > this.Y)
                        return ShipPosition.AmidShipPort;
                    else if (x - 3 > this.X && y < this.Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.South:
                    if (x == this.X && y > this.Y)
                        return ShipPosition.Bow;
                    else if (x < this.X && y - 5 == this.Y)
                        return ShipPosition.BowStarboard;
                    else if (x > this.X && y - 5 == this.Y)
                        return ShipPosition.BowPort;
                    else if (x < this.X && y - 1 == this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x > this.X && y - 1 == this.Y)
                        return ShipPosition.AmidShipPort;
                    else if (x < this.X && y + 3 == this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x > this.X && y + 3 == this.Y)
                        return ShipPosition.AmidShipPort;
                    else if (x < this.X && y + 3 < this.Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.East:
                    if (x > this.X && y == this.Y)
                        return ShipPosition.Bow;
                    else if (x - 5 == this.X && y > this.Y)
                        return ShipPosition.BowStarboard;
                    else if (x - 5 == this.X && y < this.Y)
                        return ShipPosition.BowPort;
                    else if (x - 1 == this.X && y > this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x - 1 == this.X && y < this.Y)
                        return ShipPosition.AmidShipPort;
                    else if (x + 3 == this.X && y > this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x + 3 == this.X && y < this.Y)
                        return ShipPosition.AmidShipPort;
                    else if (x + 3 < this.X && y > this.Y)
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
                int boatX = this.X;
                int boatY = this.Y;

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BritannianShipDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1150017; } } 
        public override BaseBoat Boat { get { return new BritannianShip(this.BoatDirection); } }

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

            writer.Write((int)0);
        }
    }

    public class DockedBritannianShip : BaseDockedBoat
    {
        public override int LabelNumber { get { return 1150100; } } //Britannian Ship
        public override BaseBoat Boat { get { return new BritannianShip(this.BoatDirection); } }

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

            writer.Write((int)0);
        }
    }
}
