using System;
using Server;
using System.Collections.Generic;
using Server.Items;

namespace Server.Multis
{
    public class GargishGalleon : BaseGalleon
    {
        public override int NorthID { get { return 0x24 + (DamageValue * 4); } }
        public override int EastID { get { return  0x25 + (DamageValue * 4); } }
        public override int SouthID { get { return 0x26 + (DamageValue * 4); } }
        public override int WestID { get { return  0x27 + (DamageValue * 4); } }

        public override int HoldDistance { get { return 6; } }
        public override int TillerManDistance { get { return 1; } }
        public override int RuneOffset { get { return 3; } }

        public override int WheelDistance { get { return 2; } }
        public override int CaptiveOffset { get { return 3; } }
        public override int MaxCannons { get { return 7; } }
        public override double CannonDamageMod { get { return 1.0; } }

        public override double TurnDelay { get { return 1; } }
        public override int MaxHits { get { return 140; } }

        public override int ZSurface { get { return 16; } }

        public override BaseDockedBoat DockedBoat { get { return new DockedGargishGalleon(this); } }

        [Constructable]
        public GargishGalleon() : this(Direction.North) { }

        [Constructable]
        public GargishGalleon(Direction facing)
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

            switch (direction)
            {
                default:
                case Direction.North:
                    {
                        line1.Location = new Point3D(X + 2, Y + 3, Z + ZSurface);
                        line2.Location = new Point3D(X - 2, Y + 3, Z + ZSurface);
                        line3.Location = new Point3D(X + 2, Y - 1, Z + ZSurface);
                        line4.Location = new Point3D(X - 2, Y - 1, Z + ZSurface);
                        line5.Location = new Point3D(X + 2, Y - 6, Z + ZSurface);
                        line6.Location = new Point3D(X - 2, Y - 6, Z + ZSurface);
                        break;
                    }
                case Direction.South:
                    {
                        line1.Location = new Point3D(X + 2, Y - 3, Z + ZSurface);
                        line2.Location = new Point3D(X - 2, Y - 3, Z + ZSurface);
                        line3.Location = new Point3D(X + 2, Y + 1, Z + ZSurface);
                        line4.Location = new Point3D(X - 2, Y + 1, Z + ZSurface);
                        line5.Location = new Point3D(X + 2, Y + 6, Z + ZSurface);
                        line6.Location = new Point3D(X - 2, Y + 6, Z + ZSurface);
                        break;
                    }
                case Direction.East:
                    {
                        line1.Location = new Point3D(X - 3, Y - 2, Z + ZSurface);
                        line2.Location = new Point3D(X - 3, Y + 2, Z + ZSurface);
                        line3.Location = new Point3D(X + 1, Y - 2, Z + ZSurface);
                        line4.Location = new Point3D(X + 1, Y + 2, Z + ZSurface);
                        line5.Location = new Point3D(X + 6, Y - 2, Z + ZSurface);
                        line6.Location = new Point3D(X + 6, Y + 2, Z + ZSurface);
                        break;
                    }
                case Direction.West:
                    {
                        line1.Location = new Point3D(X + 3, Y - 2, Z + ZSurface);
                        line2.Location = new Point3D(X + 3, Y + 2, Z + ZSurface);
                        line3.Location = new Point3D(X - 1, Y - 2, Z + ZSurface);
                        line4.Location = new Point3D(X - 1, Y + 2, Z + ZSurface);
                        line5.Location = new Point3D(X - 6, Y - 2, Z + ZSurface);
                        line6.Location = new Point3D(X - 6, Y + 2, Z + ZSurface);
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

            switch (direction)
            {
                default:
                case Direction.North:
                    {
                        tile1.Location = new Point3D(X,     Y - 8, Z);   //Center Front
                        tile2.Location = new Point3D(X + 2, Y - 5, Z);   //Left Front
                        tile3.Location = new Point3D(X - 2, Y - 5, Z);   //Right Front
                        tile4.Location = new Point3D(X + 2, Y - 2, Z);   //Left Center
                        tile5.Location = new Point3D(X - 2, Y - 2, Z);   //Right Center
                        tile6.Location = new Point3D(X + 2, Y + 1, Z);   //Left Rear
                        tile7.Location = new Point3D(X - 2, Y + 1, Z);   //Right Rear
                        break;
                    }
                case Direction.South:
                    {
                        tile1.Location = new Point3D(X,     Y + 8, Z);   //Center Front
                        tile2.Location = new Point3D(X - 2, Y + 5, Z);   //Left Front
                        tile3.Location = new Point3D(X + 2, Y + 5, Z);   //Right Front
                        tile4.Location = new Point3D(X - 2, Y + 2, Z);   //Left Center
                        tile5.Location = new Point3D(X + 2, Y + 2, Z);   //Right Center
                        tile6.Location = new Point3D(X - 2, Y - 1, Z);   //Left Rear
                        tile7.Location = new Point3D(X + 2, Y - 1, Z);   //Right Rear
                        break;
                    }
                case Direction.East:
                    {
                        tile1.Location = new Point3D(X + 8, Y,     Z);   //Center Front
                        tile2.Location = new Point3D(X + 5, Y + 2, Z);   //Left Front
                        tile3.Location = new Point3D(X + 5, Y - 2, Z);   //Right Front
                        tile4.Location = new Point3D(X + 2, Y + 2, Z);   //Left Center
                        tile5.Location = new Point3D(X + 2, Y - 2, Z);   //Right Center
                        tile6.Location = new Point3D(X - 1, Y + 2, Z);   //Left Rear
                        tile7.Location = new Point3D(X - 1, Y - 2, Z);   //Right Rear
                        break;
                    }
                case Direction.West:
                    {
                        tile1.Location = new Point3D(X - 8, Y,     Z);   //Center Front
                        tile2.Location = new Point3D(X - 5, Y - 2, Z);   //Left Front
                        tile3.Location = new Point3D(X - 5, Y + 2, Z);   //Right Front
                        tile4.Location = new Point3D(X - 2, Y - 2, Z);   //Left Center
                        tile5.Location = new Point3D(X - 2, Y + 2, Z);   //Right Center
                        tile6.Location = new Point3D(X + 1, Y - 2, Z);   //Left Rear
                        tile7.Location = new Point3D(X + 1, Y + 2, Z);   //Right Rear
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
            HoldItem hold9 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][8]));

            HoldItem fill1 = AddHoldTile(new HoldItem(GalleonHold, m_FillerIDs[dir][0]));
            HoldItem fill2 = AddHoldTile(new HoldItem(GalleonHold, m_FillerIDs[dir][1]));
            HoldItem fill3 = AddHoldTile(new HoldItem(GalleonHold, m_FillerIDs[dir][2]));
            HoldItem fill4 = AddHoldTile(new HoldItem(GalleonHold, m_FillerIDs[dir][3]));
            HoldItem fill6 = AddHoldTile(new HoldItem(GalleonHold, m_FillerIDs[dir][4]));
            HoldItem fill7 = AddHoldTile(new HoldItem(GalleonHold, m_FillerIDs[dir][5]));
            HoldItem fill8 = AddHoldTile(new HoldItem(GalleonHold, m_FillerIDs[dir][6]));
            HoldItem fill9 = AddHoldTile(new HoldItem(GalleonHold, m_FillerIDs[dir][7]));
            HoldItem fill10 = AddHoldTile(new HoldItem(GalleonHold, m_FillerIDs[dir][8]));
            HoldItem fill11 = AddHoldTile(new HoldItem(GalleonHold, m_FillerIDs[dir][9]));

            switch (direction)
            {
                case Direction.North:
                    hold1.Location = new Point3D(X - 1, Y + 8, Z);
                    hold2.Location = new Point3D(X,     Y + 8, Z);
                    hold3.Location = new Point3D(X + 1, Y + 8, Z); 
                    hold4.Location = new Point3D(X + 1, Y + 7, Z);
                    hold5.Location = new Point3D(X    , Y + 7, Z);
                    hold6.Location = new Point3D(X - 1, Y + 7, Z);
                    hold7.Location = new Point3D(X + 1, Y + 6, Z);
                    hold8.Location = new Point3D(X - 1, Y + 6, Z);
                    hold9.Location = new Point3D(X,     Y + 6, Z);

                    fill1.Location = new Point3D(X - 2, Y +  8, Z);
                    fill2.Location = new Point3D(X - 2, Y +  7, Z);
                    fill3.Location = new Point3D(X - 2, Y +  6, Z);
                    fill4.Location = new Point3D(X - 2, Y +  5, Z);

                    gHold.Location = new Point3D(X,     Y +  5, Z);
                    
                    fill6.Location = new Point3D(X - 1, Y +  5, Z);
                    fill7.Location = new Point3D(X + 1, Y +  5, Z);
                    fill8.Location = new Point3D(X + 2, Y +  5, Z);
                    fill9.Location = new Point3D(X + 2, Y +  6, Z);
                    fill10.Location = new Point3D(X + 2,Y +  7, Z);
                    fill11.Location = new Point3D(X + 2, Y + 8, Z);
                    break;
                case Direction.South:
                    hold1.Location = new Point3D(X + 1, Y - 8, Z);
                    hold2.Location = new Point3D(X,     Y - 8, Z);
                    hold3.Location = new Point3D(X - 1, Y - 8, Z);
                    hold4.Location = new Point3D(X - 1, Y - 7, Z);
                    hold5.Location = new Point3D(X,     Y - 7, Z);
                    hold6.Location = new Point3D(X + 1, Y - 7, Z);
                    hold7.Location = new Point3D(X - 1, Y - 6, Z);
                    hold8.Location = new Point3D(X + 1, Y - 6, Z);
                    hold9.Location = new Point3D(X,     Y - 6, Z);

                    fill1.Location = new Point3D(X + 2, Y - 8, Z);
                    fill2.Location = new Point3D(X + 2, Y - 7, Z);
                    fill3.Location = new Point3D(X + 2, Y - 6, Z);
                    fill4.Location = new Point3D(X + 2, Y - 5, Z);

                    gHold.Location = new Point3D(X,     Y - 5, Z);

                    fill6.Location = new Point3D(X + 1, Y - 5, Z);
                    fill7.Location = new Point3D(X - 1, Y - 5, Z);
                    fill8.Location = new Point3D(X - 2, Y - 5, Z);
                    fill9.Location = new Point3D(X - 2, Y - 6, Z);
                    fill10.Location = new Point3D(X - 2, Y - 7, Z);
                    fill11.Location = new Point3D(X - 2, Y - 8, Z);
                    break;
                case Direction.East:
                    hold1.Location = new Point3D(X - 8, Y - 1, Z);
                    hold2.Location = new Point3D(X - 8, Y    , Z);
                    hold3.Location = new Point3D(X - 8, Y + 1, Z);
                    hold4.Location = new Point3D(X - 7, Y - 1, Z);
                    hold5.Location = new Point3D(X - 7, Y    , Z);
                    hold6.Location = new Point3D(X - 7, Y + 1, Z);
                    hold7.Location = new Point3D(X - 6, Y + 1, Z);
                    hold8.Location = new Point3D(X - 6, Y - 1, Z);
                    hold9.Location = new Point3D(X - 6, Y,     Z);

                    fill1.Location = new Point3D(X - 8, Y - 2, Z);
                    fill2.Location = new Point3D(X - 7, Y - 2, Z);
                    fill3.Location = new Point3D(X - 6, Y - 2, Z);
                    fill4.Location = new Point3D(X - 5, Y - 2, Z);

                    gHold.Location = new Point3D(X - 5, Y,     Z);

                    fill6.Location = new Point3D(X - 5, Y - 1, Z);
                    fill7.Location = new Point3D(X - 5, Y + 1, Z);
                    fill8.Location = new Point3D(X - 5, Y + 2, Z);
                    fill9.Location = new Point3D(X - 6, Y + 2, Z);
                    fill10.Location = new Point3D(X - 7, Y + 2, Z);
                    fill11.Location = new Point3D(X - 8, Y + 2, Z);
                    break;
                case Direction.West:
                    hold1.Location = new Point3D(X + 8, Y + 1, Z);
                    hold2.Location = new Point3D(X + 8, Y, Z);
                    hold3.Location = new Point3D(X + 8, Y - 1, Z);
                    hold4.Location = new Point3D(X + 7, Y + 1, Z);
                    hold5.Location = new Point3D(X + 7, Y, Z);
                    hold6.Location = new Point3D(X + 7, Y - 1, Z);
                    hold7.Location = new Point3D(X + 6, Y - 1, Z);
                    hold8.Location = new Point3D(X + 6, Y + 1, Z);
                    hold9.Location = new Point3D(X + 6, Y,     Z);

                    fill1.Location = new Point3D(X + 8, Y + 2, Z);
                    fill2.Location = new Point3D(X + 7, Y + 2, Z);
                    fill3.Location = new Point3D(X + 6, Y + 2, Z);
                    fill4.Location = new Point3D(X + 5, Y + 2, Z);

                    gHold.Location = new Point3D(X + 5, Y,     Z);

                    fill6.Location = new Point3D(X + 5, Y + 1, Z);
                    fill7.Location = new Point3D(X + 5, Y - 1, Z);
                    fill8.Location = new Point3D(X + 5, Y - 2, Z);
                    fill9.Location = new Point3D(X + 6, Y - 2, Z);
                    fill10.Location = new Point3D(X + 7, Y - 2, Z);
                    fill11.Location = new Point3D(X + 8, Y - 2, Z);
                    break;
            }
        }

        public override int[][] CannonTileIDs { get { return m_CannonTileIDs; } }
        private int[][] m_CannonTileIDs = new int[][]
        {
            new int[] { 33463, 33440, 33438, 33327, 33322, 33357, 33355 }, //SOUTH
            new int[] { 33768, 33745, 33743, 33632, 33627, 33662, 33660 }, //WEST
            new int[] { 34070, 34047, 34045, 33934, 33929, 33964, 33962 }, //NORTH
            new int[] { 34373, 34350, 34348, 34237, 34232, 34267, 34265 }, //EAST

            new int[] { 19059, 19042, 19040, 18958, 18956, 18979, 18977 }, //SOUTH D1
            new int[] { 35582, 35559, 35557, 35446, 35441, 35476, 35474 }, //WEST  D1
            new int[] { 34978, 34955, 34953, 34842, 34837, 34872, 34870 }, //NORTH D1
            new int[] { 34674, 34651, 34649, 34538, 34533, 34568, 34566 }, //EAST  D1

            new int[] { 36442, 36425, 36423, 36341, 36339, 36362, 36360 }, //SOUTH D2
            new int[] { 36744, 36727, 36725, 36733, 36641, 36664, 36662 }, //WEST  D2
            new int[] { 36140, 36123, 36121, 36039, 36037, 36060, 36058 }, //NORTH D2
            new int[] { 35156, 35139, 35137, 35055, 35053, 35076, 35074 }, //EAST  D2   
        };

        public override int[][] FillerIDs { get { return m_FillerIDs; } }
        private int[][] m_FillerIDs = new int[][]
        {
            //          110    103    96     89     87     88     93     91     98     105    112

            //          BL     ML1    ML2    FL     F1     F2     F3     FR     MR2    MR1    BR
            new int[] { 33410, 33403, 33396, 33389, 33388, 33393, 33391, 33398, 33405, 33412},   //SOUTH
            new int[] { 33715, 33708, 33701, 33694, 33693, 33698, 33696, 33703, 33710, 33717 },  //WEST
            new int[] { 34017, 34010, 34003, 33996, 33995, 34000, 33998, 34005, 34012, 34019 },  //NORTH
            new int[] { 34320, 34313, 34306, 34299, 34298, 34303, 34301, 34308, 34315, 34322 },  //EAST

            //          110    103    96     89     87     88     93     91     98     105    112
            //35419
            new int[] { 19026, 19019, 19012, 19005, 19004, 19009, 19007, 19014, 19021, 19028 },  //SOUTH D1
            new int[] { 35529, 35522, 35515, 35508, 35507, 35512, 35510, 35517, 35524, 35531 },  //WEST  D1
            new int[] { 34925, 34918, 34911, 34904, 34903, 34908, 34906, 34913, 34920, 34927 },  //NORTH D1
            new int[] { 34621, 34614, 34607, 34600, 34599, 34604, 34602, 34609, 34616, 34623 },  //EAST  D1

            //          +86   -7     -7     -7     -2     +1     +5     -2     +7     +7     +7
            //          110    103    96     89     87     88     93     91     98     105    112
            new int[] { 36409, 36402, 36395, 36388, 36387, 36392, 36390, 36397, 36404, 36411 },  //SOUTH D2
            new int[] { 36711, 36704, 36697, 36690, 36689, 36694, 36692, 36699, 36706, 36713 },  //WEST  D2
            new int[] { 36107, 36100, 36093, 36086, 36085, 36090, 36088, 36095, 36102, 36109 },  //NORTH D2
            new int[] { 35123, 35116, 35109, 35102, 35101, 35106, 35104, 35111, 35118, 35125 },  //EAST  D2
        };

        public override int[][] HoldIDs { get { return m_HoldIDs; } }
        private int[][] m_HoldIDs = new int[][]
        {
            //          108    109    114    94     95     100    101    102
            //          BL     BM     BR     MR     MM     ML     FL     FR
            new int[] { 33408, 33409, 33414, 33394, 33395, 33400, 33401, 33402, 33407,
                        33410, 33403, 33396, 33389, 33388, 33393, 33391, 33398, 33405, 33412 },  //SOUTH
            new int[] { 33713, 33714, 33719, 33699, 33700, 33705, 33706, 33707, 33712,
                        33715, 33708, 33701, 33694, 33693, 33698, 33696, 33703, 33710, 33717},  //WEST
            new int[] { 34015, 34016, 34021, 34001, 34002, 34007, 34008, 34009, 34007,
                        34017, 34010, 34003, 33996, 33995, 34000, 33998, 34005, 34012, 34019 },  //NORTH
            new int[] { 34318, 34319, 34324, 34304, 34305, 34310, 34311, 34312, 34317,
                        34320, 34313, 34306, 34299, 34298, 34303, 34301, 34308, 34315, 34322 },  //EAST
            //18940
            //          +85    +1     +1      -15   +1     +1     +1     +1
            new int[] { 19025, 19024, 19025, 19010, 19011, 19016, 19017, 19018, 33407,
                        19026, 19019, 19012, 19005, 19004, 19009, 19007, 19014, 19021, 19028 },  //SOUTH D1
            new int[] { 35506, 35507, 35512, 35513, 35514, 35519, 35520, 35521, 33712,
                        35529, 35522, 35515, 35508, 35507, 35512, 35510, 35517, 35524, 35531 },  //WEST D1
            new int[] { 34902, 34903, 34908, 34909, 34910, 34915, 34916, 34917, 34007,
                        34925, 34918, 34911, 34904, 34903, 34908, 34906, 34913, 34920, 34927 },  //NORTH D1
            new int[] { 34598, 34599, 34604, 34605, 34606, 34611, 34612, 34613, 34317,
                        34621, 34614, 34607, 34600, 34599, 34604, 34602, 34609, 34616, 34623 },  //EAST D1
             
            new int[] { 36386, 36387, 36392, 36393, 36394, 36399, 36400, 36401, 33407,
                        36409, 36402, 36395, 36388, 36387, 36392, 36390, 36397, 36404, 36411 },  //SOUTH D2
            new int[] { 36688, 36689, 36694, 36695, 36696, 36701, 36702, 36703, 33712,
                        36711, 36704, 36697, 36690, 36689, 36694, 36692, 36699, 36706, 36713 },  //WEST D2
            new int[] { 36084, 36085, 36090, 36091, 36092, 36097, 36098, 36099, 34007,
                        36107, 36100, 36093, 36086, 36085, 36090, 36088, 36095, 36102, 36109 },  //NORTH D2
            new int[] { 35121, 35122, 35127, 35107, 35108, 35113, 35114, 35115, 34317,
                        35123, 35116, 35109, 35102, 35101, 35106, 35104, 35111, 35118, 35125},  //EAST D1
        };

        public override int[][] HoldItemIDs { get { return m_HoldItemIDs; } }
        private int[][] m_HoldItemIDs = new int[][]
        {
            new int[] { 33387 },
            new int[] { 33692 },
            new int[] { 33994 },
            new int[] { 34297 },

            new int[] { 19003 },
            new int[] { 35506 },
            new int[] { 34902 },
            new int[] { 34598 },

            new int[] { 36386 },
            new int[] { 36688 },
            new int[] { 36084 },
            new int[] { 35100 },
        };

        public override int[][] WheelItemIDs { get { return m_WheelItemIDs; } }
        private int[][] m_WheelItemIDs = new int[][]
        {
            
            new int[] { 33604 },
            new int[] { 33906 },
            new int[] { 34208 },
            new int[] { 34510 },
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
                    if (x > this.X && y - 2 == this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x < this.X && y - 2 == this.Y)
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
                    else if (x - 2 == this.X && y < this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x - 2 == this.X && y > this.Y)
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
                    else if (x < this.X && y + 2 == this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x > this.X && y + 2 == this.Y)
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
                    else if (x + 2 == this.X && y > this.Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x + 2 == this.X && y < this.Y)
                        return ShipPosition.AmidShipPort;
                    else if (x < this.X && y > this.Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
            }
        }

        public GargishGalleon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        foreach (var tile in FillerTiles)
                        {
                            var holdItem = new HoldItem(GalleonHold, tile.ItemID);
                            holdItem.MoveToWorld(tile.Location, tile.Map);
                            AddHoldTile(holdItem);

                            tile.Delete();
                        }

                        FillerTiles.Clear();
                    });
            }
        }
    }

    public class GargishGalleonDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1116739; } }
        public override BaseBoat Boat { get { return new GargishGalleon(this.BoatDirection); } }

        [Constructable]
        public GargishGalleonDeed()
            : base(0x24, Point3D.Zero)
        {
        }

        public GargishGalleonDeed(Serial serial)
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

    public class DockedGargishGalleon : BaseDockedBoat
    {
        public override int LabelNumber { get { return 1116748; } } //Gargoyle Ship
        public override BaseBoat Boat { get { return new GargishGalleon(this.BoatDirection); } }

        public DockedGargishGalleon(BaseBoat boat)
            : base(0x24, Point3D.Zero, boat)
        {
        }

        public DockedGargishGalleon(Serial serial)
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