using System;
using Server;
using System.Collections.Generic;
using Server.Items;

namespace Server.Multis
{
    public class OrcishGalleon : BaseGalleon
    {
        public override int NorthID { get { return 0x18 + (DamageValue * 4); } }
        public override int EastID { get { return  0x19 + (DamageValue * 4); } }
        public override int SouthID { get { return 0x1A + (DamageValue * 4); } }
        public override int WestID { get { return  0x1B + (DamageValue * 4); } }

        public override int HoldDistance { get { return 9; } }
        public override int TillerManDistance { get { return 6; } }
        public override int RuneOffset { get { return -3; } }

        public override int WheelDistance { get { return 7; } }
        public override int CaptiveOffset { get { return 4; } }
        public override int MaxCannons { get { return 7; } }
        public override double CannonDamageMod { get { return 1.5; } }

        public override double TurnDelay { get { return 2; } }
        public override int MaxHits { get { return 110; } }

        public override int ZSurface { get { return 14; } }

        public override BaseDockedBoat DockedBoat { get { return new DockedOrcishGalleon(this); } }

        [Constructable]
        public OrcishGalleon() : this(Direction.North) { }

        [Constructable]
        public OrcishGalleon(Direction facing) : base(facing)
        {
        }

        public override void AddMooringLines(Direction direction)
        {
            MooringLine line1 = AddMooringLine(new MooringLine(this));
            MooringLine line2 = AddMooringLine(new MooringLine(this));
            MooringLine line3 = AddMooringLine(new MooringLine(this));
            MooringLine line4 = AddMooringLine(new MooringLine(this));
            MooringLine line5 = AddMooringLine(new MooringLine(this));
            MooringLine line6 = AddMooringLine(new MooringLine(this));

            switch (direction)
            {
                default:
                case Direction.North:
                    {
                        line1.Location = new Point3D(X + 2, Y - 3, Z + ZSurface);
                        line2.Location = new Point3D(X - 2, Y - 3, Z + ZSurface);
                        line3.Location = new Point3D(X + 2, Y + 1, Z + ZSurface);
                        line4.Location = new Point3D(X - 2, Y + 1, Z + ZSurface);
                        line5.Location = new Point3D(X + 2, Y + 5, Z + ZSurface);
                        line6.Location = new Point3D(X - 2, Y + 5, Z + ZSurface);
                        break;
                    }
                case Direction.South:
                    {
                        line1.Location = new Point3D(X + 2, Y + 3, Z + ZSurface);
                        line2.Location = new Point3D(X - 2, Y + 3, Z + ZSurface);
                        line3.Location = new Point3D(X + 2, Y - 1, Z + ZSurface);
                        line4.Location = new Point3D(X - 2, Y - 1, Z + ZSurface);
                        line5.Location = new Point3D(X + 2, Y - 5, Z + ZSurface);
                        line6.Location = new Point3D(X - 2, Y - 5, Z + ZSurface);
                        break;
                    }
                case Direction.East:
                    {
                        line1.Location = new Point3D(X - 1, Y - 2, Z + ZSurface);
                        line2.Location = new Point3D(X - 1, Y + 2, Z + ZSurface);
                        line3.Location = new Point3D(X - 5, Y - 2, Z + ZSurface);
                        line4.Location = new Point3D(X - 5, Y + 2, Z + ZSurface);
                        line5.Location = new Point3D(X + 3, Y - 2, Z + ZSurface);
                        line6.Location = new Point3D(X + 3, Y + 2, Z + ZSurface);
                        break;
                    }
                case Direction.West:
                    {
                        line1.Location = new Point3D(X + 1, Y - 2, Z + ZSurface);
                        line2.Location = new Point3D(X + 1, Y + 2, Z + ZSurface);
                        line3.Location = new Point3D(X + 5, Y - 2, Z + ZSurface);
                        line4.Location = new Point3D(X + 5, Y + 2, Z + ZSurface);
                        line5.Location = new Point3D(X - 3, Y - 2, Z + ZSurface);
                        line6.Location = new Point3D(X - 3, Y + 2, Z + ZSurface);
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

            Static fill1 = AddFillerTile(new Static(m_FillerIDs[dir][0]));
            Static fill2 = AddFillerTile(new Static(m_FillerIDs[dir][1]));
            Static fill3 = AddFillerTile(new Static(m_FillerIDs[dir][2]));
            Static fill4 = AddFillerTile(new Static(m_FillerIDs[dir][3]));
            Static fill5 = AddFillerTile(new Static(m_FillerIDs[dir][4]));
            Static fill6 = AddFillerTile(new Static(m_FillerIDs[dir][5]));

            switch (direction)
            {
                default:
                case Direction.North:
                    {
                        tile1.Location = new Point3D(X,     Y - 6, Z);   //Center Front
                        tile2.Location = new Point3D(X - 2, Y - 2, Z);   //Left Front
                        tile3.Location = new Point3D(X + 2, Y - 2, Z);   //Right Front
                        tile4.Location = new Point3D(X - 2, Y + 2, Z);   //Left Center
                        tile5.Location = new Point3D(X + 2, Y + 2, Z);   //Right Center
                        tile6.Location = new Point3D(X - 2, Y + 6, Z);   //Left Rear
                        tile7.Location = new Point3D(X + 2, Y + 6, Z);   //Right Rear

                        fill1.Location = new Point3D(X - 2, Y - 3, Z);   //Left Front1
                        fill2.Location = new Point3D(X + 2, Y - 3, Z);   //Right Front1
                        fill3.Location = new Point3D(X + 2, Y - 1, Z);   //Right Front2
                        fill4.Location = new Point3D(X - 2, Y - 1, Z);   //Left Front2
                        fill5.Location = new Point3D(X + 2, Y + 5, Z);   //Left Back
                        fill6.Location = new Point3D(X - 2, Y + 5, Z);   //Right Back
                        break;
                    }
                case Direction.South:
                    {
                        tile1.Location = new Point3D(X,     Y + 6, Z);   //Center Front
                        tile2.Location = new Point3D(X + 2, Y + 2, Z);   //Left Front
                        tile3.Location = new Point3D(X - 2, Y + 2, Z);   //Right Front
                        tile4.Location = new Point3D(X + 2, Y - 2, Z);   //Left Center
                        tile5.Location = new Point3D(X - 2, Y - 2, Z);   //Right Center
                        tile6.Location = new Point3D(X + 2, Y - 6, Z);   //Left Rear
                        tile7.Location = new Point3D(X - 2, Y - 6, Z);   //Right Rear

                        fill1.Location = new Point3D(X + 2, Y + 3, Z);   //Left Front1
                        fill2.Location = new Point3D(X - 2, Y + 3, Z);   //Right Front1
                        fill3.Location = new Point3D(X - 2, Y + 1, Z);   //Right Front2
                        fill4.Location = new Point3D(X + 2, Y + 1, Z);   //Left Front2
                        fill5.Location = new Point3D(X - 2, Y - 5, Z);   //Left Back
                        fill6.Location = new Point3D(X + 2, Y - 5, Z);   //Right Back
                        break;
                    }
                case Direction.East:
                    {
                        tile1.Location = new Point3D(X + 6, Y,     Z);   //Center Front
                        tile2.Location = new Point3D(X + 2, Y - 2, Z);   //Left Front
                        tile3.Location = new Point3D(X + 2, Y + 2, Z);   //Right Front
                        tile4.Location = new Point3D(X - 2, Y - 2, Z);   //Left Center
                        tile5.Location = new Point3D(X - 2, Y + 2, Z);   //Right Center
                        tile6.Location = new Point3D(X - 6, Y - 2, Z);   //Left Rear
                        tile7.Location = new Point3D(X - 6, Y + 2, Z);   //Right Rear

                        fill1.Location = new Point3D(X + 3, Y - 2, Z);
                        fill2.Location = new Point3D(X + 3, Y + 2, Z);
                        fill3.Location = new Point3D(X + 1, Y + 2, Z);
                        fill4.Location = new Point3D(X + 1, Y - 2, Z);
                        fill5.Location = new Point3D(X - 5, Y + 2, Z);
                        fill6.Location = new Point3D(X - 5, Y - 2, Z);
                        break;
                    }
                case Direction.West:
                    {
                        tile1.Location = new Point3D(X - 6, Y,     Z);   //Center Front
                        tile2.Location = new Point3D(X - 2, Y + 2, Z);   //Left Front
                        tile3.Location = new Point3D(X - 2, Y - 2, Z);   //Right Front
                        tile4.Location = new Point3D(X + 2, Y + 2, Z);   //Left Center
                        tile5.Location = new Point3D(X + 2, Y - 2, Z);   //Right Center
                        tile6.Location = new Point3D(X + 6, Y + 2, Z);   //Left Rear
                        tile7.Location = new Point3D(X + 6, Y - 2, Z);   //Right Rear

                        fill1.Location = new Point3D(X - 3, Y + 2, Z);
                        fill2.Location = new Point3D(X - 3, Y - 2, Z);
                        fill3.Location = new Point3D(X - 1, Y - 2, Z);
                        fill4.Location = new Point3D(X - 1, Y + 2, Z);
                        fill5.Location = new Point3D(X + 5, Y - 2, Z);
                        fill6.Location = new Point3D(X + 5, Y + 2, Z);
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

            switch (direction)
            {
                case Direction.North:
                    hold1.Location = new Point3D(X + 1, Y + 10, Z);
                    hold2.Location = new Point3D(X,     Y + 10, Z);
                    hold3.Location = new Point3D(X - 1, Y + 10, Z);
                    hold4.Location = new Point3D(X - 1, Y + 9,  Z);
                    hold5.Location = new Point3D(X + 1, Y + 9,  Z);
                    gHold.Location = new Point3D(X,     Y + 9,  Z);
                    break;
                case Direction.South:
                    hold1.Location = new Point3D(X - 1, Y - 10, Z);
                    hold2.Location = new Point3D(X, Y - 10, Z);
                    hold3.Location = new Point3D(X + 1, Y - 10, Z);
                    hold4.Location = new Point3D(X + 1, Y - 9, Z);
                    hold5.Location = new Point3D(X - 1, Y - 9, Z);
                    gHold.Location = new Point3D(X,     Y - 9,  Z);
                    break;
                case Direction.East:
                    hold1.Location = new Point3D(X - 10, Y + 1, Z);
                    hold2.Location = new Point3D(X - 10, Y,     Z);
                    hold3.Location = new Point3D(X - 10, Y - 1, Z);
                    hold4.Location = new Point3D(X - 9,  Y - 1, Z);
                    hold5.Location = new Point3D(X - 9,  Y + 1, Z);
                    gHold.Location = new Point3D(X - 9,  Y,     Z);
                    break;
                case Direction.West:
                    hold1.Location = new Point3D(X + 10, Y - 1, Z);
                    hold2.Location = new Point3D(X + 10, Y,     Z);
                    hold3.Location = new Point3D(X + 10, Y + 1, Z);
                    hold4.Location = new Point3D(X + 9,  Y + 1, Z);
                    hold5.Location = new Point3D(X + 9,  Y - 1, Z);
                    gHold.Location = new Point3D(X + 9,  Y,     Z);
                    break;
            }
        }

        public override int[][] CannonTileIDs { get { return m_CannonTileIDs; } }
        private int[][] m_CannonTileIDs = new int[][]
        {

            new int[] { 30012, 30037, 30041, 30065, 30069, 30093, 30097 }, //SOUTH
            new int[] { 30512, 30537, 30541, 30565, 30569, 30593, 30597 }, //WEST
            new int[] { 31012, 31037, 31041, 31065, 31069, 31093, 31097 }, //NORTH
            new int[] { 31512, 31537, 31541, 31565, 31569, 31593, 31597 }, //EAST

            new int[] { 31712, 31737, 31741, 31765, 31769, 31793, 31797 }, //SOUTH D1
            new int[] { 31913, 31938, 31942, 31966, 31970, 31994, 31998 }, //WEST  D1
            new int[] { 32112, 32137, 32141, 32165, 32169, 32193, 32197 }, //NORTH D1
            new int[] { 32312, 32337, 32341, 32365, 32369, 32393, 32397 }, //EAST  D1

            new int[] { 32512, 32537, 32541, 32565, 32569, 32593, 32597 }, //NORTH D2
            new int[] { 32712, 32737, 32741, 32765, 32769, 32793, 32797 }, //WEST  D2
            new int[] { 32912, 32937, 32941, 32965, 32969, 32993, 32997 }, //NORTH D2
            new int[] { 33112, 33137, 33141, 33165, 33169, 33193, 33197 }, //NORTH D2   
        };

        public override int[][] FillerIDs { get { return m_FillerIDs; } }
        private int[][] m_FillerIDs = new int[][]
        {
            new int[] { 30030, 30034, 30048, 30044, 30090, 30086 },  //SOUTH
            new int[] { 30530, 30534, 30548, 30544, 30590, 30586 },  //WEST
            new int[] { 31030, 31034, 31048, 31044, 31090, 31086 },  //NORTH
            new int[] { 31530, 31534, 31548, 31544, 31590, 31586 },  //EAST

            new int[] { 31730, 31734, 31748, 31744, 31790, 31786 },  //SOUTH D1
            new int[] { 31931, 31935, 31949, 31945, 31991, 31987 },  //WEST  D1
            new int[] { 32130, 32134, 32148, 32144, 32190, 32186 },  //NORTH D1
            new int[] { 32330, 32334, 32348, 32344, 32390, 32386 },  //EAST  D1

            new int[] { 32530, 32534, 32548, 32544, 32590, 32586 },  //SOUTH D2
            new int[] { 32730, 32734, 32748, 32744, 32790, 32786 },  //WEST  D2
            new int[] { 32930, 32934, 32948, 32944, 32990, 32986 },  //NORTH D2
            new int[] { 33130, 33134, 33148, 33144, 33190, 33186 },  //EAST  D2
        };

        public override int[][] HoldIDs { get { return m_HoldIDs; } }
        private int[][] m_HoldIDs = new int[][]
        {
            new int[] { 30127, 30124, 30122, 30115, 30120 },  //SOUTH
            new int[] { 30627, 30624, 30622, 30615, 30620 },  //WEST
            new int[] { 31127, 31124, 31122, 31115, 31120 },  //NORTH
            new int[] { 31627, 31624, 31622, 31615, 31620 },  //EAST

            new int[] { 31827, 31824, 31822, 31815, 31820 },  //SOUTH D1
            new int[] { 32028, 32025, 32023, 32016, 32021 },  //WEST D1
            new int[] { 32227, 32224, 32222, 32215, 32220 },  //NORTH D1
            new int[] { 32427, 32424, 32422, 32415, 32420 },  //EAST D1
             
            new int[] { 32627, 32624, 32622, 32615, 32620 },  //SOUTH D2
            new int[] { 32827, 32824, 32822, 32815, 32820 },  //WEST D2
            new int[] { 33027, 33024, 33022, 33015, 33020 },  //NORTH D2
            new int[] { 33227, 33224, 33222, 33215, 33220 },  //EAST D1
        };

        public override int[][] HoldItemIDs { get { return m_HoldItemIDs; } }
        private int[][] m_HoldItemIDs = new int[][]
        {
            new int[] { 30117 },
            new int[] { 30617 },
            new int[] { 31117 },
            new int[] { 31617 },

            new int[] { 31817 },
            new int[] { 32018 },
            new int[] { 32217 },
            new int[] { 32417 },

            new int[] { 32617 },
            new int[] { 32817 },
            new int[] { 33017 },
            new int[] { 33217 },
        };

        public override int[][] WheelItemIDs { get { return m_WheelItemIDs; } }
        private int[][] m_WheelItemIDs = new int[][]
        {            
            new int[] { 30141 },
            new int[] { 30641 },
            new int[] { 31141 },
            new int[] { 31642 },
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
                    if (x > this.X && y < this.Y)
                        return ShipPosition.BowStarboard;
                    if (x < this.X && y < this.Y)
                        return ShipPosition.BowPort;
                    if (x > this.X && y + 2 == this.Y)
                        return ShipPosition.AmidShipStarboard; 
                    else if(x <  this.X && y + 2 == this.Y)
                        return ShipPosition.AmidShipPort;
                    else if (x > this.X && y > this.Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.West:
                    if (x < this.X && y == this.Y)
                        return ShipPosition.Bow;
                    else if (x < this.X && y < this.Y)
                        return ShipPosition.BowStarboard;
                    else if (x < this.X && y > this.Y)
                        return ShipPosition.BowPort;
                    else if(x + 2 == this.X && y < this.Y)
                        return ShipPosition.AmidShipStarboard; 
                    else if (x + 2 == this.X && y > this.Y)
                        return ShipPosition.AmidShipPort; 
                    else if (x > this.X && y < this.Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.South:
                    if (x == this.X && y > this.Y)
                        return ShipPosition.Bow;
                    else if (x < this.X && y > this.Y)
                        return ShipPosition.BowStarboard;
                    else if (x > this.X && y > this.Y)
                        return ShipPosition.BowPort;
                    else if (x < this.X && y - 2 == this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x > this.X && y - 2 == this.Y)
                        return ShipPosition.AmidShipPort;
                    else if (x < this.X && y < this.Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
                case Direction.East:
                    if (x > this.X && y == this.Y)
                        return ShipPosition.Bow;
                    else if (x > this.X && y > this.Y)
                        return ShipPosition.BowStarboard;
                    else if (x > this.X && y < this.Y)
                        return ShipPosition.BowPort;
                    else if(x - 2 == this.X && y > this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x - 2 == this.X && y < this.Y)
                        return ShipPosition.AmidShipPort; 
                    else if (x < this.X && y > this.Y)
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

		public override bool ExemptOverheadComponent(Point3D newPnt, int itemID, int x , int y, int height)
		{
			//if(height > this.Z + 16) //TODO: Get height of mast
			//	return false;
		
			switch(itemID)
			{
				case 0x18: //North/South ID's
				case 0x1A:
				case 0x1C:
				case 0x1E:
				case 0x20:
				case 0x22:
					if(x > newPnt.X + PortAndStarboardOffset || x < newPnt.X - PortAndStarboardOffset)
						return true;
					break;
				case 0x19: //East/West ID's
				case 0x1B:
				case 0x1D:
				case 0x1F:
				case 0x21:
				case 0x23:
					if(y > newPnt.Y + PortAndStarboardOffset || y < newPnt.Y - PortAndStarboardOffset)
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class OrcishGalleonDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1116738; } } // small dragon ship deed
        public override BaseBoat Boat { get { return new OrcishGalleon(this.BoatDirection); } }

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

            writer.Write((int)0);
        }
    }

    public class DockedOrcishGalleon : BaseDockedBoat
    {
        public override int LabelNumber { get { return 1116747; } } //Orc Ship
        public override BaseBoat Boat { get { return new OrcishGalleon(this.BoatDirection); } }

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

            writer.Write((int)0);
        }
    }
}