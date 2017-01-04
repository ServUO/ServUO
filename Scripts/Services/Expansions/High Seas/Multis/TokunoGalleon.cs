using System;
using Server;
using System.Collections.Generic;
using Server.Items;

namespace Server.Multis
{
    public class TokunoGalleon : BaseGalleon
    {
        public override int NorthID { get { return 0x30 + (DamageValue * 4); } }
        public override int EastID { get { return  0x31 + (DamageValue * 4); } }
        public override int SouthID { get { return 0x32 + (DamageValue * 4); } }
        public override int WestID { get { return  0x33 + (DamageValue * 4); } }

        public override int HoldDistance { get { return -5; } }
        public override int CaptiveOffset { get { return 2; } }
        public override int TillerManDistance { get { return -1; } }
        public override int RuneOffset { get { return -3; } }

        public override int WheelDistance { get { return 7; } }
        public override int MaxCannons { get { return 5; } }

        public override double TurnDelay { get { return 3; } }
        public override int MaxHits { get { return 100; } }

        public override int ZSurface { get { return 7; } }

        public override BaseDockedBoat DockedBoat { get { return new DockedTokunoGalleon(this); } }

        [Constructable]
        public TokunoGalleon() : this(Direction.North) { }

        [Constructable]
        public TokunoGalleon(Direction facing)
            : base(facing)
        {
        }

        public override void AddMooringLines(Direction direction)
        {
            Item line1 = AddMooringLine(new MooringLine(this));
            Item line2 = AddMooringLine(new MooringLine(this));
            Item line3 = AddMooringLine(new MooringLine(this));
            Item line4 = AddMooringLine(new MooringLine(this));

            switch (direction)
            {
                default:
                case Direction.North:
                    {
                        line1.Location = new Point3D(X + 2, Y - 2, Z + ZSurface);
                        line2.Location = new Point3D(X - 2, Y - 2, Z + ZSurface);
                        line3.Location = new Point3D(X + 2, Y + 3, Z + ZSurface);
                        line4.Location = new Point3D(X - 2, Y + 3, Z + ZSurface);
                        break;
                    }
                case Direction.South:
                    {
                        line1.Location = new Point3D(X + 2, Y + 2, Z + ZSurface);
                        line2.Location = new Point3D(X - 2, Y + 2, Z + ZSurface);
                        line3.Location = new Point3D(X + 2, Y - 3, Z + ZSurface);
                        line4.Location = new Point3D(X - 2, Y - 3, Z + ZSurface);
                        break;
                    }
                case Direction.East:
                    {
                        line1.Location = new Point3D(X - 3, Y - 2, Z + ZSurface);
                        line2.Location = new Point3D(X - 3, Y + 2, Z + ZSurface);
                        line3.Location = new Point3D(X + 2, Y - 2, Z + ZSurface);
                        line4.Location = new Point3D(X + 2, Y + 2, Z + ZSurface);
                        break;
                    }
                case Direction.West:
                    {
                        line1.Location = new Point3D(X + 3, Y - 2, Z + ZSurface);
                        line2.Location = new Point3D(X + 3, Y + 2, Z + ZSurface);
                        line3.Location = new Point3D(X - 2, Y - 2, Z + ZSurface);
                        line4.Location = new Point3D(X - 2, Y + 2, Z + ZSurface);
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

            switch (direction)
            {
                default:
                case Direction.North:
                    {
                        tile1.Location = new Point3D(X,     Y - 9, Z);   //Center Front
                        tile2.Location = new Point3D(X - 2, Y - 3, Z);   //Left Front
                        tile3.Location = new Point3D(X + 2, Y - 3, Z);   //Right Front
                        tile4.Location = new Point3D(X - 2, Y + 2, Z);   //Left Center
                        tile5.Location = new Point3D(X + 2, Y + 2, Z);   //Right Center
                        break;
                    }
                case Direction.South:
                    {
                        tile1.Location = new Point3D(X,     Y + 9, Z);   //Center Front
                        tile2.Location = new Point3D(X + 2, Y + 3, Z);   //Left Front
                        tile3.Location = new Point3D(X - 2, Y + 3, Z);   //Right Front
                        tile4.Location = new Point3D(X + 2, Y - 2, Z);   //Left Center
                        tile5.Location = new Point3D(X - 2, Y - 2, Z);   //Right Center
                        break;
                    }
                case Direction.East:
                    {
                        tile1.Location = new Point3D(X + 9, Y,     Z);   //Center Front
                        tile2.Location = new Point3D(X + 3, Y - 2, Z);   //Left Front
                        tile3.Location = new Point3D(X + 3, Y + 2, Z);   //Right Front
                        tile4.Location = new Point3D(X - 2, Y - 2, Z);   //Left Center
                        tile5.Location = new Point3D(X - 2, Y + 2, Z);   //Right Center
                        break;
                    }
                case Direction.West:
                    {
                        tile1.Location = new Point3D(X - 9, Y,     Z);   //Center Front
                        tile2.Location = new Point3D(X - 3, Y + 2, Z);   //Left Front
                        tile3.Location = new Point3D(X - 3, Y - 2, Z);   //Right Front
                        tile4.Location = new Point3D(X + 2, Y + 2, Z);   //Left Center
                        tile5.Location = new Point3D(X + 2, Y - 2, Z);   //Right Center
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
            HoldItem hold4 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][2]));
            HoldItem hold5 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][3]));
            HoldItem hold6 = AddHoldTile(new HoldItem(GalleonHold, m_HoldIDs[dir][4]));

            switch (direction)
            {
                case Direction.North:
                    hold1.Location = new Point3D(X - 1, Y + 5, Z);
                    hold2.Location = new Point3D(X - 1, Y + 4, Z);

                    gHold.Location = new Point3D(X,     Y + 4, Z);

                    hold4.Location = new Point3D(X + 1, Y + 4, Z);
                    hold5.Location = new Point3D(X + 1, Y + 5, Z);
                    hold6.Location = new Point3D(X,     Y + 5, Z);
                    break;
                case Direction.South:
                    hold1.Location = new Point3D(X + 1, Y - 5, Z);
                    hold2.Location = new Point3D(X + 1, Y - 4, Z);

                    gHold.Location = new Point3D(X,     Y - 4, Z);

                    hold4.Location = new Point3D(X - 1, Y - 4, Z);
                    hold5.Location = new Point3D(X - 1, Y - 5, Z);
                    hold6.Location = new Point3D(X,     Y - 5, Z);
                    break;
                case Direction.East:
                    hold1.Location = new Point3D(X - 5, Y - 1, Z);
                    hold2.Location = new Point3D(X - 4, Y - 1, Z);

                    gHold.Location = new Point3D(X - 4, Y,     Z);

                    hold4.Location = new Point3D(X - 4, Y + 1, Z);
                    hold5.Location = new Point3D(X - 5, Y + 1, Z);
                    hold6.Location = new Point3D(X - 5, Y,     Z);
                    break;
                case Direction.West:
                    hold1.Location = new Point3D(X + 5, Y + 1, Z);
                    hold2.Location = new Point3D(X + 4, Y + 1, Z);

                    gHold.Location = new Point3D(X + 4, Y,     Z);

                    hold4.Location = new Point3D(X + 4, Y - 1, Z);
                    hold5.Location = new Point3D(X + 5, Y - 1, Z);
                    hold6.Location = new Point3D(X + 5, Y,     Z);
                    break;
            }
        }

        public override int[][] CannonTileIDs { get { return m_CannonTileIDs; } }
        private int[][] m_CannonTileIDs = new int[][]
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

        public override int[][] FillerIDs { get { return m_FillerIDs; } }
        private int[][] m_FillerIDs = new int[][]
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

        public override int[][] HoldIDs { get { return m_HoldIDs; } }
        private int[][] m_HoldIDs = new int[][]
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

        public override int[][] HoldItemIDs { get { return m_HoldItemIDs; } }
        private int[][] m_HoldItemIDs = new int[][]
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

        public override int[][] WheelItemIDs { get { return m_WheelItemIDs; } }
        private int[][] m_WheelItemIDs = new int[][]
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
                    if (x == this.X && y < this.Y)
                        return ShipPosition.Bow;
                    if (x > this.X && y < this.Y)
                        return ShipPosition.BowStarboard;
                    if (x < this.X && y < this.Y)
                        return ShipPosition.BowPort;
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
                    else if (x < this.X && y > this.Y)
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class TokunoGalleonDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1116740; } }
        public override BaseBoat Boat { get { return new TokunoGalleon(this.BoatDirection); } }

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

            writer.Write((int)0);
        }
    }

    public class DockedTokunoGalleon : BaseDockedBoat
    {
        public override int LabelNumber { get { return 1116749; } } //Tokuno Ship
        public override BaseBoat Boat { get { return new TokunoGalleon(this.BoatDirection); } }

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

            writer.Write((int)0);
        }
    }
}