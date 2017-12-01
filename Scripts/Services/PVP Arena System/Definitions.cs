using Server;
using System;

namespace Server.Engines.ArenaSystem
{
    [PropertyObject]
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
        public Rectangle2D[] GuardBounds { get; private set; }
        public Rectangle2D[] StartLocations { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D StartLocation1 { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D StartLocation2 { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D EjectLocation { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D DeadEjectLocation { get; private set; }

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
            Rectangle2D[] startLocs,
            Point3D gateLoc,
            Rectangle2D[] bounds,
            Rectangle2D[] guardbounds,
            Rectangle2D eject,
            Rectangle2D deadEject)
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
            StartLocations = startLocs;
            StartLocation1 = startLocs[0];
            StartLocation2 = startLocs[1];
            GateLocation = gateLoc;
            RegionBounds = bounds;
            GuardBounds = guardbounds;
            EjectLocation = eject;
            DeadEjectLocation = deadEject;
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
                    new Rectangle2D(6072, 3718, 24, 1),
                    new Rectangle2D(6072, 3723, 24, 1),
                    new Rectangle2D(6075, 3714, 1, 14),
                    new Rectangle2D(6083, 3713, 1, 16),
                    new Rectangle2D(6091, 3714, 1, 14),
                },
                new Rectangle2D[]
                {
                    new Rectangle2D(6071, 3719, 4, 3),
                    new Rectangle2D(6092, 3719, 4, 3),
                    new Rectangle2D(6077, 3713, 5, 4),
                    new Rectangle2D(6084, 3713, 5, 4),
                    new Rectangle2D(6076, 3724, 5, 4),
                    new Rectangle2D(6084, 3724, 5, 4),
                    new Rectangle2D(6073, 3716, 1, 1),
                    new Rectangle2D(6073, 3724, 1, 1),
                    new Rectangle2D(6091, 3714, 2, 2),
                    new Rectangle2D(6091, 3724, 2, 2),
                },
                new Point3D(6100, 3721, 25),
                new Rectangle2D[]
                {
                    new Rectangle2D(6070, 3713, 27, 16)
                },
                new Rectangle2D[]
                {
                    new Rectangle2D(6059, 3697, 53, 56)
                },
                new Rectangle2D(6099, 3718, 5, 7),
                new Rectangle2D(6097, 3729, 2, 2));

            LostLandsFelucca = new ArenaDefinition("Lostland (F)", 0, 
                new Point3D(6102, 3721, 25),
                new Point3D(6097, 3730, 20),
                new Point3D(6081, 3713, 26),
                new Point3D(6087, 3713, 26),
                17101,
                17099,
                new Rectangle2D[]
                {
                    new Rectangle2D(6072, 3718, 24, 1),
                    new Rectangle2D(6072, 3723, 24, 1),
                    new Rectangle2D(6075, 3714, 1, 14),
                    new Rectangle2D(6083, 3713, 1, 16),
                    new Rectangle2D(6090, 3713, 1, 16),
                },
                new Rectangle2D[]
                {
                    new Rectangle2D(6071, 3719, 4, 3),
                    new Rectangle2D(6092, 3719, 4, 3),
                    new Rectangle2D(6077, 3713, 5, 4),
                    new Rectangle2D(6084, 3713, 5, 4),
                    new Rectangle2D(6076, 3724, 5, 4),
                    new Rectangle2D(6084, 3724, 5, 4),
                    new Rectangle2D(6073, 3716, 1, 1),
                    new Rectangle2D(6073, 3724, 1, 1),
                    new Rectangle2D(6091, 3714, 2, 2),
                    new Rectangle2D(6091, 3724, 2, 2),
                },
                new Point3D(6100, 3721, 25),
                new Rectangle2D[]
                {
                    new Rectangle2D(6070, 3713, 27, 16)
                },
                new Rectangle2D[]
                {
                    new Rectangle2D(6059, 3697, 53, 56)
                },
                new Rectangle2D(6099, 3718, 5, 7),
                new Rectangle2D(6097, 3729, 2, 2));

            HavenTrammel = new ArenaDefinition("New Haven", 1, 
                new Point3D(3793, 2770, 6), 
                new Point3D(3790, 2783, 6),
                new Point3D(3760, 2769, 12),
                new Point3D(3783, 2761, 10),
                17102,
                17099,
                new Rectangle2D[]
                {
                    new Rectangle2D(3760, 2766, 25, 1),
                    new Rectangle2D(3760, 2772, 25, 1),
                    new Rectangle2D(3765, 2761, 1, 16),
                    new Rectangle2D(3772, 2761, 1, 16),
                    new Rectangle2D(3780, 2761, 1, 16),
                },
                new Rectangle2D[]
                {
                    new Rectangle2D(3760, 2767, 4, 4),
                    new Rectangle2D(3781, 2767, 3, 4),
                    new Rectangle2D(3766, 2761, 4, 4),
                    new Rectangle2D(3773, 2761, 4, 4),
                    new Rectangle2D(3766, 2773, 4, 4),
                    new Rectangle2D(3773, 2773, 4, 4),
                    new Rectangle2D(3760, 2761, 3, 3),
                    new Rectangle2D(3781, 2761, 3, 3),
                    new Rectangle2D(3781, 2773, 3, 3),
                    new Rectangle2D(3760, 2773, 3, 3),
                },
                new Point3D(3792, 2768, 6),
                new Rectangle2D[]
                {
                    new Rectangle2D(3760, 2761, 25, 16)
                },
                new Rectangle2D[]
                {
                    new Rectangle2D(3740, 2747, 63, 51)
                },
                new Rectangle2D(3791, 2766, 4, 9),
                new Rectangle2D(3790, 2781, 2, 5));

            HavenFelucca = new ArenaDefinition("New Haven", 0,
                new Point3D(3782, 2766, 5), 
                new Point3D(3779, 2778, 5),
                new Point3D(3749, 2765, 12),
                new Point3D(3772, 2757, 10),
                17102,
                17099,
                new Rectangle2D[]
                {
                    new Rectangle2D(3749, 2762, 25, 1),
                    new Rectangle2D(3749, 2768, 25, 1),
                    new Rectangle2D(3754, 2757, 1, 16),
                    new Rectangle2D(3761, 2757, 1, 16),
                    new Rectangle2D(3769, 2757, 1, 16),
                },
                new Rectangle2D[]
                {
                    new Rectangle2D(3749, 2763, 4, 4),
                    new Rectangle2D(3770, 2763, 3, 4),
                    new Rectangle2D(3755, 2757, 4, 4),
                    new Rectangle2D(3762, 2757, 4, 4),
                    new Rectangle2D(3755, 2769, 4, 4),
                    new Rectangle2D(3762, 2769, 4, 4),
                    new Rectangle2D(3749, 2757, 3, 3),
                    new Rectangle2D(3770, 2757, 3, 3),
                    new Rectangle2D(3770, 2769, 3, 3),
                    new Rectangle2D(3759, 2769, 3, 3),
                },
                new Point3D(3781, 2764, 5),
                new Rectangle2D[]
                {
                    new Rectangle2D(3749, 2757, 25, 16)
                },
                new Rectangle2D[]
                {
                    new Rectangle2D(3735, 2747, 68, 51)
                },
                new Rectangle2D(3780, 2763, 4, 9),
                new Rectangle2D(3779, 2776, 2, 5));
        }
    }
}