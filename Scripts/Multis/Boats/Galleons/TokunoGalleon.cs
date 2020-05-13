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

        public override int[][] CannonTileIDs => m_CannonTileIDs;
        private readonly int[][] m_CannonTileIDs = new int[][]
        {
            //                 -34    -4     -31    -4
            new int[] { 37054, 37020, 37016, 36985, 36981 }, //SOUTH
            new int[] { 37189, 37155, 37151, 37120, 37116 }, //WEST
            new int[] { 37324, 37290, 37286, 37255, 37251 }, //NORTH
            new int[] { 37459, 37425, 37421, 37390, 37386 }, //EAST

            new int[] { 37786, 37752, 37748, 37717, 37713 }, //SOUTH D1
            new int[] { 37921, 37887, 37883, 37852, 37848 }, //WEST  D1
            new int[] { 38056, 38022, 38018, 37987, 37983 }, //NORTH D1
            new int[] { 38191, 38157, 38153, 38122, 38118},  //EAST  D1

            new int[] { 38794, 38760, 38756, 38725, 38721 }, //SOUTH D2
            new int[] { 38934, 38900, 38896, 38865, 38861 }, //WEST  D2
            new int[] { 38654, 38620, 38616, 38585, 38581 }, //NORTH D2
            new int[] { 38519, 38485, 38481, 38450, 38446 }, //EAST  D2   
        };

        public override int[][] FillerIDs => m_FillerIDs;
        private readonly int[][] m_FillerIDs = new int[][]
        {
            /*//          BL     ML1    ML2    FL     F1     F2     F3     FR     MR2    MR1    BR
            new int[] { 33410, 33403, 33396, 33389, 33387, 33388, 33393, 33391, 33398, 33405, 33412},   //SOUTH
            new int[] { 33715, 33708, 33701, 33694, 33692, 33693, 33698, 33696, 33703, 33710, 33717 },  //WEST
            new int[] { 34017, 34010, 34003, 33996, 33994, 33995, 34000, 33998, 34005, 34012, 34019 },  //NORTH
            new int[] { 34320, 34313, 34306, 34299, 34297, 34298, 34303, 34301, 34308, 34315, 34322 },  //EAST

            //          110    103    96     89     87     88     93     91     98     105    112
            //35419
            new int[] { 19026, 19019, 19012, 19005, 19003, 19004, 19009, 19007, 19014, 19021, 19028 },  //SOUTH D1
            new int[] { 35529, 35522, 35515, 35508, 35506, 35507, 35512, 35510, 35517, 35524, 35531 },  //WEST  D1
            new int[] { 34925, 34918, 34911, 34904, 34902, 34903, 34908, 34906, 34913, 34920, 34927 },  //NORTH D1
            new int[] { 34621, 34614, 34607, 34600, 34598, 34599, 34604, 34602, 34609, 34616, 34623 },  //EAST  D1

            //          +86   -7     -7     -7     -2     +1     +5     -2     +7     +7     +7
            //          110    103    96     89     87     88     93     91     98     105    112
            new int[] { 36409, 36402, 36395, 36388, 36386, 36387, 36392, 36390, 36397, 36404, 36411 },  //SOUTH D2
            new int[] { 36711, 36704, 36697, 36690, 36688, 36689, 36694, 36692, 36699, 36706, 36713 },  //WEST  D2
            new int[] { 36107, 36100, 36093, 36086, 36084, 36085, 36090, 36088, 36095, 36102, 36109 },  //NORTH D2
            new int[] { 35123, 35116, 35109, 35102, 35100, 35101, 35106, 35104, 35111, 35118, 35125 },  //EAST  D2*/
        };

        public override int[][] HoldIDs => m_HoldIDs;
        private readonly int[][] m_HoldIDs = new int[][]
        {
            //          BL +7  FL -1  FM -1  FR -7  BR  +1
            new int[] { 36963, 36970, 36968, 36961, 37964 },  //SOUTH
            new int[] { 37098, 37105, 37103, 37096, 37097 },  //WEST
            new int[] { 37233, 37240, 37238, 37231, 37232 },  //NORTH
            new int[] { 37368, 37375, 37373, 37366, 37367 },  //EAST
           
            new int[] { 37695, 37702, 37700, 37693, 37964 },  //SOUTH D1
            new int[] { 37830, 37837, 37835, 37828, 37097 },  //WEST D1
            new int[] { 37965, 37972, 37970, 37963, 37232 },  //NORTH D1
            new int[] { 38100, 38107, 38105, 38098, 37367 },  //EAST D1
             
            new int[] { 38703, 38710, 38708, 38701, 37964 },  //SOUTH D2
            new int[] { 38843, 38850, 38848, 38841, 37097 },  //WEST D2
            new int[] { 38563, 38570, 38568, 38561, 37232 },  //NORTH D2
            new int[] { 38428, 38435, 38433, 38426, 37367 },  //EAST D1
        };

        public override int[][] HoldItemIDs => m_HoldItemIDs;
        private readonly int[][] m_HoldItemIDs = new int[][]
        {
            new int[] { 36969 },
            new int[] { 37104 },
            new int[] { 37239 },
            new int[] { 37374 },

            new int[] { 37701 },
            new int[] { 37836 },
            new int[] { 37971 },
            new int[] { 38106 },

            new int[] { 38709 },
            new int[] { 38849 },
            new int[] { 38569 },
            new int[] { 38434 },
        };

        public override int[][] WheelItemIDs => m_WheelItemIDs;
        private readonly int[][] m_WheelItemIDs = new int[][]
        {

            new int[] { 37650 },
            new int[] { 37652 },
            new int[] { 37654 },
            new int[] { 37656 },
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
