using Server;
using System;

namespace Server.Engines.ArenaSystem
{
    public class ArenaDefinition
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public TextDefinition Name { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D StoneLocation { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D ManagerLocation { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D BannerLocation1 { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D BannerLocation2 { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D GateLocation { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BannerID1 { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BannerID2 { get; private set; }

        public Rectangle2D[] EffectAreas { get; private set; }
        public Rectangle2D[] RegionBounds { get; private set; }

        public Rectangle2D StartLocation1 { get; private set; }

        public Rectangle2D StartLocation2 { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MapIndex { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map Map { get { return Map.Maps[MapIndex]; } }

        public ArenaDefinition(
            TextDefinition name, 
            int mapIndex, 
            Point3D stoneLoc, 
            Point3D manLoc, 
            Point3D banloc1,
            Point3D banloc2, 
            int id1, 
            int id2, 
            Rectangle2D[] effectAreas, 
            Rectangle2D start1,
            Rectangle2D start2,
            Point3D gateLoc,
            Rectangle2D[] bounds)
        {
            Name = name;
            MapIndex = mapIndex;
            StoneLocation = stoneLoc;
            ManagerLocation = manLoc;
            BannerLocation1 = banloc1;
            BannerLocation2 = banloc2;
            BannerID1 = id1;
            BannerID2 = id2;
            EffectAreas = effectAreas;
            StartLocation1 = start1;
            StartLocation2 = start2;
            GateLocation = gateLoc;
            RegionBounds = bounds;
        }

        public static ArenaDefinition LostLandsTrammel { get; set; }
        public static ArenaDefinition LostLandsFelucca { get; set; }
        public static ArenaDefinition HavenTrammel { get; set; }
        public static ArenaDefinition HavenFelucca { get; set; }

        static ArenaDefinition()
        {
            LostLandsTrammel = new ArenaDefinition("Lostland (T)", 1, 
                new Point3D(6102, 3721, 25), 
                new Point3D(6097, 3730, 20),
                new Point3D(6081, 3713, 26),
                new Point3D(6087, 3713, 26), 
                17101,
                17099,
                new Rectangle2D[]
                {
                    new Rectangle2D(6072, 3718, 23, 1),
                    new Rectangle2D(6072, 3723, 23, 1),
                    new Rectangle2D(6075, 3714, 1, 13),
                    new Rectangle2D(6083, 3713, 1, 15),
                    new Rectangle2D(6090, 3713, 1, 13),
                },
                new Rectangle2D(6076, 3718, 2, 3),
                new Rectangle2D(6089, 3718, 2, 3),
                new Point3D(6100, 3721, 25),
                new Rectangle2D[]
                {
                    new Rectangle2D(6070, 3713, 27, 16)
                });

            LostLandsFelucca = new ArenaDefinition("Lostland (F)", 0, 
                new Point3D(6102, 3721, 25),
                new Point3D(6097, 3730, 20),
                new Point3D(6081, 3713, 26),
                new Point3D(6087, 3713, 26),
                17101,
                17099,
                new Rectangle2D[]
                {
                    new Rectangle2D(6072, 3718, 23, 1),
                    new Rectangle2D(6072, 3723, 23, 1),
                    new Rectangle2D(6075, 3714, 1, 13),
                    new Rectangle2D(6083, 3713, 1, 15),
                    new Rectangle2D(6090, 3713, 1, 13),
                },
                new Rectangle2D(6076, 3718, 2, 3),
                new Rectangle2D(6089, 3718, 2, 3),
                new Point3D(6100, 3721, 25),
                new Rectangle2D[]
                {
                    new Rectangle2D(6070, 3713, 27, 16)
                });

            HavenTrammel = new ArenaDefinition("New Haven", 1, 
                new Point3D(3793, 2770, 6), 
                new Point3D(3790, 2783, 6),
                new Point3D(3760, 2769, 12),
                new Point3D(3783, 2761, 10),
                17102,
                17099,
                new Rectangle2D[]
                {
                    new Rectangle2D(3760, 2765, 24, 1),
                    new Rectangle2D(3760, 2772, 24, 1),
                    new Rectangle2D(3766, 2761, 1, 15),
                    new Rectangle2D(3772, 2761, 1, 15),
                    new Rectangle2D(3778, 2761, 1, 15),
                },
                new Rectangle2D(3763, 2767, 2, 5),
                new Rectangle2D(3781, 2767, 2, 5),
                new Point3D(3792, 2768, 6),
                new Rectangle2D[]
                {
                    new Rectangle2D(3760, 2761, 26, 16)
                });

            HavenFelucca = new ArenaDefinition("New Haven", 0,
                new Point3D(3782, 2766, 6), 
                new Point3D(3779, 2778, 5),
                new Point3D(3749, 2765, 12),
                new Point3D(3772, 2757, 10),
                17102,
                17099,
                new Rectangle2D[]
                {
                    new Rectangle2D(3749, 2761, 24, 1),
                    new Rectangle2D(3749, 2769, 24, 1),
                    new Rectangle2D(3756, 2757, 1, 15),
                    new Rectangle2D(3761, 2757, 1, 15),
                    new Rectangle2D(3766, 2757, 1, 15),
                },
                new Rectangle2D(3752, 2763, 2, 5),
                new Rectangle2D(3770, 2763, 2, 5),
                new Point3D(3781, 2764, 5),
                new Rectangle2D[]
                {
                    new Rectangle2D(3749, 2757, 25, 16)
                });
        }
    }
}