using Server.Items;

namespace Server.Multis
{
    public class TokunoGalleon : BaseGalleon
    {
        public override int NorthID => 0x30 + (DamageValue * 4);
        public override int EastID => 0x31 + (DamageValue * 4);
        public override int SouthID => 0x32 + (DamageValue * 4);
        public override int WestID => 0x33 + (DamageValue * 4);

        public override int HoldDistance => -5;
        public override int CaptiveOffset => 2;
        public override int TillerManDistance => -1;
        public override int RuneOffset => -3;

        public override int WheelDistance => 7;
        public override int MaxCannons => 5;

        public override double TurnDelay => 3;
        public override int MaxHits => 100000;

        public override int ZSurface => 7;

        public override BaseDockedBoat DockedBoat => new DockedTokunoGalleon(this);

        [Constructable]
        public TokunoGalleon() : this(Direction.North) { }

        [Constructable]
        public TokunoGalleon(Direction facing)
            : base(facing)
        {
        }

        public override int[] CannonTileIDs => m_CannonTileIDs;
        private static readonly int[] m_CannonTileIDs = new int[]
        {
            37054, 37020, 37016, 36985, 36981, //SOUTH
            37189, 37155, 37151, 37120, 37116, //WEST
            37324, 37290, 37286, 37255, 37251, //NORTH
            37459, 37425, 37421, 37390, 37386, //EAST

            37786, 37752, 37748, 37717, 37713, //SOUTH D1
            37921, 37887, 37883, 37852, 37848, //WEST  D1
            38056, 38022, 38018, 37987, 37983, //NORTH D1
            38191, 38157, 38153, 38122, 38118,  //EAST  D1

            38794, 38760, 38756, 38725, 38721, //SOUTH D2
            38934, 38900, 38896, 38865, 38861, //WEST  D2
            38654, 38620, 38616, 38585, 38581, //NORTH D2
            38519, 38485, 38481, 38450, 38446, //EAST  D2   
        };

        public override int[] HoldIDs => m_HoldIDs;
        private static readonly int[] m_HoldIDs = new int[]
        {
            36963, 36970, 36968, 36961, 37964,  //SOUTH
            37098, 37105, 37103, 37096, 37097,  //WEST
            37233, 37240, 37238, 37231, 37232,  //NORTH
            37368, 37375, 37373, 37366, 37367,  //EAST
           
            37695, 37702, 37700, 37693, 37964,  //SOUTH D1
            37830, 37837, 37835, 37828, 37097,  //WEST D1
            37965, 37972, 37970, 37963, 37232,  //NORTH D1
            38100, 38107, 38105, 38098, 37367,  //EAST D1
             
            38703, 38710, 38708, 38701, 37964,  //SOUTH D2
            38843, 38850, 38848, 38841, 37097,  //WEST D2
            38563, 38570, 38568, 38561, 37232,  //NORTH D2
            38428, 38435, 38433, 38426, 37367   //EAST D1
        };

        public override int[] HoldItemIDs => m_HoldItemIDs;
        private static readonly int[] m_HoldItemIDs = new int[]
        {
            36969, 37104, 37239, 37374,
            37701, 37836, 37971, 38106,
            38709, 38849, 38569, 38434
        };

        public override int[] WheelItemIDs => m_WheelItemIDs;
        private static readonly int[] m_WheelItemIDs = new int[]
        {
            37650, 37652, 37654, 37656,
            38382, 39152, 38386, 38388,
            39150, 39152, 39148, 39145
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
                    else if (x < X && y > Y)
                        return ShipPosition.AftStarboard;
                    else
                        return ShipPosition.AftPort;
            }
        }

        public TokunoGalleon(Serial serial)
            : base(serial)
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

    public class TokunoGalleonDeed : BaseBoatDeed
    {
        public override int LabelNumber => 1116740;
        public override BaseBoat Boat => new TokunoGalleon(BoatDirection);

        [Constructable]
        public TokunoGalleonDeed()
            : base(0x30, Point3D.Zero)
        {
        }

        public TokunoGalleonDeed(Serial serial)
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

    public class DockedTokunoGalleon : BaseDockedBoat
    {
        public override int LabelNumber => 1116749;  //Tokuno Ship
        public override BaseBoat Boat => new TokunoGalleon(BoatDirection);

        public DockedTokunoGalleon(BaseBoat boat)
            : base(0x30, Point3D.Zero, boat)
        {
        }

        public DockedTokunoGalleon(Serial serial)
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
