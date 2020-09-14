using Server.Items;

namespace Server.Multis
{
    public class GargishGalleon : BaseGalleon
    {
        public override int NorthID => 0x24 + (DamageValue * 4);
        public override int EastID => 0x25 + (DamageValue * 4);
        public override int SouthID => 0x26 + (DamageValue * 4);
        public override int WestID => 0x27 + (DamageValue * 4);

        public override int HoldDistance => 6;
        public override int TillerManDistance => 1;
        public override int RuneOffset => 3;

        public override int WheelDistance => 2;
        public override int CaptiveOffset => 3;
        public override int MaxCannons => 7;

        public override double TurnDelay => 1;
        public override int MaxHits => 140000;

        public override int ZSurface => 16;

        public override BaseDockedBoat DockedBoat => new DockedGargishGalleon(this);

        [Constructable]
        public GargishGalleon() : this(Direction.North) { }

        [Constructable]
        public GargishGalleon(Direction facing)
            : base(facing)
        {
        }

        public override int[] CannonTileIDs => m_CannonTileIDs;
        private static readonly int[] m_CannonTileIDs = new int[]
        {
            33463, 33440, 33438, 33327, 33322, 33357, 33355, //SOUTH
            33768, 33745, 33743, 33632, 33627, 33662, 33660, //WEST
            34070, 34047, 34045, 33934, 33929, 33964, 33962, //NORTH
            34373, 34350, 34348, 34237, 34232, 34267, 34265, //EAST

            19059, 19042, 19040, 18958, 18956, 18979, 18977, //SOUTH D1
            35582, 35559, 35557, 35446, 35441, 35476, 35474, //WEST  D1
            34978, 34955, 34953, 34842, 34837, 34872, 34870, //NORTH D1
            34674, 34651, 34649, 34538, 34533, 34568, 34566, //EAST  D1

            36442, 36425, 36423, 36341, 36339, 36362, 36360, //SOUTH D2
            36744, 36727, 36725, 36733, 36641, 36664, 36662, //WEST  D2
            36140, 36123, 36121, 36039, 36037, 36060, 36058, //NORTH D2
            35156, 35139, 35137, 35055, 35053, 35076, 35074  //EAST  D2   
        };

        public override int[] HoldIDs => m_HoldIDs;
        private static readonly int[] m_HoldIDs = new int[]
        {
            33408, 33409, 33414, 33394, 33395, 33400, 33401, 33402, 33407, 33410, 33403, 33396, 33389, 33388, 33393, 33391, 33398, 33405, 33412,  //SOUTH
            33713, 33714, 33719, 33699, 33700, 33705, 33706, 33707, 33712, 33715, 33708, 33701, 33694, 33693, 33698, 33696, 33703, 33710, 33717,  //WEST
            34015, 34016, 34021, 34001, 34002, 34007, 34008, 34009, 34007, 34017, 34010, 34003, 33996, 33995, 34000, 33998, 34005, 34012, 34019,  //NORTH
            34318, 34319, 34324, 34304, 34305, 34310, 34311, 34312, 34317, 34320, 34313, 34306, 34299, 34298, 34303, 34301, 34308, 34315, 34322,  //EAST

            19025, 19024, 19025, 19010, 19011, 19016, 19017, 19018, 33407, 19026, 19019, 19012, 19005, 19004, 19009, 19007, 19014, 19021, 19028,  //SOUTH D1
            35506, 35507, 35512, 35513, 35514, 35519, 35520, 35521, 33712, 35529, 35522, 35515, 35508, 35507, 35512, 35510, 35517, 35524, 35531,  //WEST D1
            34902, 34903, 34908, 34909, 34910, 34915, 34916, 34917, 34007, 34925, 34918, 34911, 34904, 34903, 34908, 34906, 34913, 34920, 34927,  //NORTH D1
            34598, 34599, 34604, 34605, 34606, 34611, 34612, 34613, 34317, 34621, 34614, 34607, 34600, 34599, 34604, 34602, 34609, 34616, 34623,  //EAST D1
             
            36386, 36387, 36392, 36393, 36394, 36399, 36400, 36401, 33407, 36409, 36402, 36395, 36388, 36387, 36392, 36390, 36397, 36404, 36411,  //SOUTH D2
            36688, 36689, 36694, 36695, 36696, 36701, 36702, 36703, 33712, 36711, 36704, 36697, 36690, 36689, 36694, 36692, 36699, 36706, 36713,  //WEST D2
            36084, 36085, 36090, 36091, 36092, 36097, 36098, 36099, 34007, 36107, 36100, 36093, 36086, 36085, 36090, 36088, 36095, 36102, 36109,  //NORTH D2
            35121, 35122, 35127, 35107, 35108, 35113, 35114, 35115, 34317, 35123, 35116, 35109, 35102, 35101, 35106, 35104, 35111, 35118, 35125  //EAST D1
        };

        public override int[] HoldItemIDs => m_HoldItemIDs;
        private static readonly int[] m_HoldItemIDs = new int[]
        {
            33387, 33692, 33994, 34297,
            19003, 35506, 34902, 34598,
            36386, 36688, 36084, 35100
        };

        public override int[] WheelItemIDs => m_WheelItemIDs;
        private static readonly int[] m_WheelItemIDs = new int[]
        {
            33604, 33906, 34208, 34510,
            19188, 35718, 39212, 34812,
            36623,  36923,  36322, 36020,
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
                    if (x > X && y - 2 == Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x < X && y - 2 == Y)
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
                    else if (x - 2 == X && y < Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x - 2 == X && y > Y)
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
                    else if (x < X && y + 2 == Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x > X && y + 2 == Y)
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
                    else if (x + 2 == X && y > Y)
                        return ShipPosition.AmidShipStarboard;
                    else if (x + 2 == X && y < Y)
                        return ShipPosition.AmidShipPort;
                    else if (x < X && y > Y)
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
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GargishGalleonDeed : BaseBoatDeed
    {
        public override int LabelNumber => 1116739;
        public override BaseBoat Boat => new GargishGalleon(BoatDirection);

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

            writer.Write(0);
        }
    }

    public class DockedGargishGalleon : BaseDockedBoat
    {
        public override int LabelNumber => 1116748;  //Gargoyle Ship
        public override BaseBoat Boat => new GargishGalleon(BoatDirection);

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

            writer.Write(0);
        }
    }
}
