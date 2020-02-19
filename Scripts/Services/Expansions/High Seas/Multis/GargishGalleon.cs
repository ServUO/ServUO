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

        public override double TurnDelay { get { return 1; } }
        public override int MaxHits { get { return 140000; } }

        public override int ZSurface { get { return 16; } }

        public override BaseDockedBoat DockedBoat { get { return new DockedGargishGalleon(this); } }

        [Constructable]
        public GargishGalleon() : this(Direction.North) { }

        [Constructable]
        public GargishGalleon(Direction facing)
            : base(facing)
        {
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
