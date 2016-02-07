using System;

namespace Server.Factions
{
    public class TrueBritannians : Faction
    {
        private static Faction m_Instance;
        public TrueBritannians()
        {
            m_Instance = this;

            this.Definition =
                new FactionDefinition(
                    2,
                    1254, // dark purple
                    2125, // gold
                    2214, // join stone : gold
                    2125, // broadcast : gold
                    0x76, 0x3EB2, // war horse
                    "True Britannians", "true", "TB",
                    new TextDefinition(1011536, "LORD BRITISH"),
                    new TextDefinition(1060771, "True Britannians faction"),
                    new TextDefinition(1011423, "<center>TRUE BRITANNIANS</center>"),
                    new TextDefinition(1011450,
                                               "True Britannians are loyal to the throne of Lord British. They refuse " +
                                               "to give up their homelands to the vile Minax, and detest the Shadowlords " +
                                               "for their evil ways. In addition, the Council of Mages threatens the " +
                                               "existence of their ruler, and as such they have armed themselves, and " +
                                               "prepare for war with all."),
                    new TextDefinition(1011454, "This city is controlled by Lord British."),
                    new TextDefinition(1042254, "This sigil has been corrupted by the True Britannians"),
                    new TextDefinition(1041045, "The faction signup stone for the True Britannians"),
                    new TextDefinition(1041383, "The Faction Stone of the True Britannians"),
                    new TextDefinition(1011465, ": True Britannians"),
                    new TextDefinition(1005181, "Followers of Lord British will now be ignored."),
                    new TextDefinition(1005182, "Followers of Lord British will now be warned of their impending doom."),
                    new TextDefinition(1005183, "Followers of Lord British will now be attacked on sight."),
                    new StrongholdDefinition(
                        new Rectangle2D[]
                        {
                            new Rectangle2D(1292, 1556, 25, 25),
                            new Rectangle2D(1292, 1676, 120, 25),
                            new Rectangle2D(1388, 1556, 25, 25),
                            new Rectangle2D(1317, 1563, 71, 18),
                            new Rectangle2D(1300, 1581, 105, 95),
                            new Rectangle2D(1405, 1612, 12, 21),
                            new Rectangle2D(1405, 1633, 11, 5)
                        },
                        new Point3D(1419, 1622, 20),
                        new Point3D(1330, 1621, 50),
                        new Point3D[]
                        {
                            new Point3D(1328, 1627, 50),
                            new Point3D(1328, 1621, 50),
                            new Point3D(1334, 1627, 50),
                            new Point3D(1334, 1621, 50),
                            new Point3D(1340, 1627, 50),
                            new Point3D(1340, 1621, 50),
                            new Point3D(1345, 1621, 50),
                            new Point3D(1345, 1627, 50)
                        }),
                    new RankDefinition[]
                    {
                        new RankDefinition(10, 991, 8, new TextDefinition(1060794, "Knight of the Codex")),
                        new RankDefinition(9, 950, 7, new TextDefinition(1060793, "Knight of Virtue")),
                        new RankDefinition(8, 900, 6, new TextDefinition(1060792, "Crusader")),
                        new RankDefinition(7, 800, 6, new TextDefinition(1060792, "Crusader")),
                        new RankDefinition(6, 700, 5, new TextDefinition(1060791, "Sentinel")),
                        new RankDefinition(5, 600, 5, new TextDefinition(1060791, "Sentinel")),
                        new RankDefinition(4, 500, 5, new TextDefinition(1060791, "Sentinel")),
                        new RankDefinition(3, 400, 4, new TextDefinition(1060790, "Defender")),
                        new RankDefinition(2, 200, 4, new TextDefinition(1060790, "Defender")),
                        new RankDefinition(1, 0, 4, new TextDefinition(1060790, "Defender"))
                    },
                    new GuardDefinition[]
                    {
                        new GuardDefinition(typeof(FactionHenchman), 0x1403, 5000, 1000, 10, new TextDefinition(1011526, "HENCHMAN"), new TextDefinition(1011510, "Hire Henchman")),
                        new GuardDefinition(typeof(FactionMercenary),	0x0F62, 6000, 2000, 10, new TextDefinition(1011527, "MERCENARY"), new TextDefinition(1011511, "Hire Mercenary")),
                        new GuardDefinition(typeof(FactionKnight), 0x0F4D, 7000, 3000, 10, new TextDefinition(1011528, "KNIGHT"), new TextDefinition(1011497, "Hire Knight")),
                        new GuardDefinition(typeof(FactionPaladin), 0x143F, 8000, 4000, 10, new TextDefinition(1011529, "PALADIN"), new TextDefinition(1011498, "Hire Paladin")),
                    });
        }

        public static Faction Instance
        {
            get
            {
                return m_Instance;
            }
        }
    }
}