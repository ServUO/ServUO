using System;
using Server;
using System.Collections.Generic;
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

        public override int WheelDistance { get { return 3; } }
        public override int CaptiveOffset { get { return 5; } }
        public override int MaxCannons { get { return 9; } }
        public override double CannonDamageMod { get { return 1.0; } }
        public override int MaxHits { get { return 150; } }

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

        public override void AddMooringLines(Direction direction)
        {
            Item line1 = AddMooringLine(new MooringLine(this));
            Item line2 = AddMooringLine(new MooringLine(this));
            Item line3 = AddMooringLine(new MooringLine(this));
            Item line4 = AddMooringLine(new MooringLine(this));
            Item line5 = AddMooringLine(new MooringLine(this));
            Item line6 = AddMooringLine(new MooringLine(this));
            Item line7 = AddMooringLine(new MooringLine(this));
            Item line8 = AddMooringLine(new MooringLine(this));

            switch (direction)
            {
                default:
                case Direction.North:
                    {
                        line1.Location = new Point3D(X + 3, Y - 6, Z + ZSurface);
                        line2.Location = new Point3D(X - 3, Y - 6, Z + ZSurface);
                        line3.Location = new Point3D(X + 3, Y - 2, Z + ZSurface);
                        line4.Location = new Point3D(X - 3, Y - 2, Z + ZSurface);
                        line5.Location = new Point3D(X + 3, Y + 2, Z + ZSurface);
                        line6.Location = new Point3D(X - 3, Y + 2, Z + ZSurface);
                        line7.Location = new Point3D(X + 3, Y + 6, Z + ZSurface);
                        line8.Location = new Point3D(X - 3, Y + 6, Z + ZSurface);
                        break;
                    }
                case Direction.South:
                    {
                        line1.Location = new Point3D(X + 3, Y + 6, Z + ZSurface);
                        line2.Location = new Point3D(X - 3, Y + 6, Z + ZSurface);
                        line3.Location = new Point3D(X + 3, Y + 2, Z + ZSurface);
                        line4.Location = new Point3D(X - 3, Y + 2, Z + ZSurface);
                        line5.Location = new Point3D(X + 3, Y - 2, Z + ZSurface);
                        line6.Location = new Point3D(X - 3, Y - 2, Z + ZSurface);
                        line7.Location = new Point3D(X + 3, Y - 6, Z + ZSurface);
                        line8.Location = new Point3D(X - 3, Y - 6, Z + ZSurface);
                        break;
                    }
                case Direction.East:
                    {
                        line1.Location = new Point3D(X + 6, Y - 3, Z + ZSurface);
                        line2.Location = new Point3D(X + 6, Y + 3, Z + ZSurface);
                        line3.Location = new Point3D(X + 2, Y - 3, Z + ZSurface);
                        line4.Location = new Point3D(X + 2, Y + 3, Z + ZSurface);
                        line5.Location = new Point3D(X - 2, Y - 3, Z + ZSurface);
                        line6.Location = new Point3D(X - 2, Y + 3, Z + ZSurface);
                        line7.Location = new Point3D(X - 6, Y - 3, Z + ZSurface);
                        line8.Location = new Point3D(X - 6, Y + 3, Z + ZSurface);
                        break;
                    }
                case Direction.West:
                    {
                        line1.Location = new Point3D(X - 6, Y - 3, Z + ZSurface);
                        line2.Location = new Point3D(X - 6, Y + 3, Z + ZSurface);
                        line3.Location = new Point3D(X - 2, Y - 3, Z + ZSurface);
                        line4.Location = new Point3D(X - 2, Y + 3, Z + ZSurface);
                        line5.Location = new Point3D(X + 2, Y - 3, Z + ZSurface);
                        line6.Location = new Point3D(X + 2, Y + 3, Z + ZSurface);
                        line7.Location = new Point3D(X + 6, Y - 3, Z + ZSurface);
                        line8.Location = new Point3D(X + 6, Y + 3, Z + ZSurface);
                        break;
                    }
            }
        }

        public override void AddCannonTiles(Direction direction)
        {
            int dir = GetValueForDirection(direction);

            Static tile1 = AddCannonTile(new Static(m_CannonTileIDs[dir][0]));
            Static tile2 = AddCannonTile(new Static(m_CannonTileIDs[dir][1]));
            Static tile3 = AddCannonTile(new Static(m_CannonTileIDs[dir][2]));
            Static tile4 = AddCannonTile(new Static(m_CannonTileIDs[dir][3]));
            Static tile5 = AddCannonTile(new Static(m_CannonTileIDs[dir][4]));
            Static tile6 = AddCannonTile(new Static(m_CannonTileIDs[dir][5]));
            Static tile7 = AddCannonTile(new Static(m_CannonTileIDs[dir][6]));
            Static tile8 = AddCannonTile(new Static(m_CannonTileIDs[dir][7]));
            Static tile9 = AddCannonTile(new Static(m_CannonTileIDs[dir][8]));

            switch (direction)
            {
                default:
                case Direction.North:
                    {
                        tile1.Location = new Point3D(X, Y - 9, Z);       
                        tile2.Location = new Point3D(X + 3, Y - 5, Z);   
                        tile3.Location = new Point3D(X - 3, Y - 5, Z);   
                        tile4.Location = new Point3D(X + 3, Y - 1, Z);  
                        tile5.Location = new Point3D(X - 3, Y - 1, Z);   
                        tile6.Location = new Point3D(X + 3, Y + 3, Z);   
                        tile7.Location = new Point3D(X - 3, Y + 3, Z);
                        tile8.Location = new Point3D(X + 3, Y + 7, Z);
                        tile9.Location = new Point3D(X - 3, Y + 7, Z); 
                        break;
                    }
                case Direction.South:
                    {
                        tile1.Location = new Point3D(X, Y + 9, Z);       
                        tile2.Location = new Point3D(X + 3, Y + 5, Z);  
                        tile3.Location = new Point3D(X - 3, Y + 5, Z);   
                        tile4.Location = new Point3D(X + 3, Y + 1, Z);   
                        tile5.Location = new Point3D(X - 3, Y + 1, Z);   
                        tile6.Location = new Point3D(X + 3, Y - 3, Z);  
                        tile7.Location = new Point3D(X - 3, Y - 3, Z);
                        tile8.Location = new Point3D(X + 3, Y - 7, Z);
                        tile9.Location = new Point3D(X - 3, Y - 7, Z); 
                        break;
                    }
                case Direction.East:
                    {
                        tile1.Location = new Point3D(X + 9, Y, Z);       
                        tile2.Location = new Point3D(X + 5, Y + 3, Z);   
                        tile3.Location = new Point3D(X + 5, Y - 3, Z);   
                        tile4.Location = new Point3D(X + 1, Y + 3, Z);   
                        tile5.Location = new Point3D(X + 1, Y - 3, Z);   
                        tile6.Location = new Point3D(X - 3, Y + 3, Z);  
                        tile7.Location = new Point3D(X - 3, Y - 3, Z);
                        tile8.Location = new Point3D(X - 7, Y + 3, Z);
                        tile9.Location = new Point3D(X - 7, Y - 3, Z); 
                        break;
                    }
                case Direction.West:
                    {
                        tile1.Location = new Point3D(X - 9, Y,     Z);      
                        tile2.Location = new Point3D(X - 5, Y - 3, Z);   
                        tile3.Location = new Point3D(X - 5, Y + 3, Z);  
                        tile4.Location = new Point3D(X - 1, Y - 3, Z);   
                        tile5.Location = new Point3D(X - 1, Y + 3, Z);  
                        tile6.Location = new Point3D(X + 3, Y - 3, Z);  
                        tile7.Location = new Point3D(X + 3, Y + 3, Z);
                        tile8.Location = new Point3D(X + 7, Y - 3, Z);
                        tile9.Location = new Point3D(X + 7, Y + 3, Z);  
                        break;
                    }
            }
        }

        public override void AddHoldTiles(Direction direction)
        {
            int dir = GetValueForDirection(direction);

            GalleonHold gHold = AddGalleonHold(new GalleonHold(this, m_HoldItemIDs[dir][0]));

            HoldItem hold1 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][0]));
            HoldItem hold2 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][1]));
            HoldItem hold3 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][2]));
            HoldItem hold4 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][3]));
            HoldItem hold5 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][4]));
            HoldItem hold6 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][5]));
            HoldItem hold7 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][6]));
            HoldItem hold8 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][7]));

            Static fill1 = AddFillerTile(new Static(m_FillerIDs[dir][0]));
            Static fill2 = AddFillerTile(new Static(m_FillerIDs[dir][1]));
            Static fill3 = AddFillerTile(new Static(m_FillerIDs[dir][2]));
            AddAddonTile(fill2);
            AddAddonTile(fill3);

            switch (direction)
            {
                case Direction.North:
                    gHold.Location = new Point3D(X,     Y + 8, Z);
                    hold1.Location = new Point3D(X + 1, Y + 8, Z);
                    hold2.Location = new Point3D(X - 1, Y + 8, Z);
                    hold3.Location = new Point3D(X + 1, Y + 9, Z);
                    hold4.Location = new Point3D(X - 1, Y + 9, Z);
                    hold5.Location = new Point3D(X,     Y + 10, Z);
                    hold6.Location = new Point3D(X + 1, Y + 10, Z);
                    hold7.Location = new Point3D(X - 1, Y + 10, Z);
                    hold8.Location = new Point3D(X,    Y + 9, Z);

                    fill1.Location = new Point3D(X, Y - 3, Z);
                    fill2.Location = new Point3D(X, Y + 13, Z);
                    fill3.Location = new Point3D(X, Y + 16, Z);
                    break;
                case Direction.South:
                    gHold.Location = new Point3D(X, Y - 8, Z);
                    hold1.Location = new Point3D(X - 1, Y - 8, Z);
                    hold2.Location = new Point3D(X + 1, Y - 8, Z);
                    hold3.Location = new Point3D(X - 1, Y - 9, Z);
                    hold4.Location = new Point3D(X + 1, Y - 9, Z);
                    hold5.Location = new Point3D(X,     Y - 10, Z);
                    hold6.Location = new Point3D(X - 1, Y - 10, Z);
                    hold7.Location = new Point3D(X + 1, Y - 10, Z);
                    hold8.Location = new Point3D(X,    Y - 9, Z);

                    fill1.Location = new Point3D(X, Y + 3, Z);
                    fill2.Location = new Point3D(X, Y - 13, Z);
                    fill3.Location = new Point3D(X, Y - 16, Z);
                    break;
                case Direction.East:
                    gHold.Location = new Point3D(X - 8, Y, Z);
                    hold1.Location = new Point3D(X - 8,  Y + 1, Z);
                    hold2.Location = new Point3D(X - 8,  Y - 1, Z);
                    hold3.Location = new Point3D(X - 9,  Y + 1, Z);
                    hold4.Location = new Point3D(X - 9,  Y - 1, Z);
                    hold5.Location = new Point3D(X - 10, Y, Z);
                    hold6.Location = new Point3D(X - 10, Y + 1, Z);
                    hold7.Location = new Point3D(X - 10, Y - 1, Z);
                    hold8.Location = new Point3D(X - 9,Y,     Z);

                    fill1.Location = new Point3D(X + 3, Y, Z);
                    fill2.Location = new Point3D(X - 13, Y, Z);
                    fill3.Location = new Point3D(X - 16, Y, Z);
                    break;
                case Direction.West:
                    gHold.Location = new Point3D(X + 8, Y, Z);
                    hold1.Location = new Point3D(X + 8,  Y - 1, Z);
                    hold2.Location = new Point3D(X + 8,  Y + 1, Z);
                    hold3.Location = new Point3D(X + 9,  Y - 1, Z);
                    hold4.Location = new Point3D(X + 9,  Y + 1, Z);
                    hold5.Location = new Point3D(X + 10, Y, Z);
                    hold6.Location = new Point3D(X + 10, Y - 1, Z);
                    hold7.Location = new Point3D(X + 10, Y + 1, Z);
                    hold8.Location = new Point3D(X + 9,Y,     Z);

                    fill1.Location = new Point3D(X - 3, Y, Z);
                    fill2.Location = new Point3D(X + 13, Y, Z);
                    fill3.Location = new Point3D(X + 16, Y, Z);
                    break;
            }
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