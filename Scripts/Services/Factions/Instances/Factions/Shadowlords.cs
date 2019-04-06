using System;

namespace Server.Factions
{
    public class Shadowlords : Faction
    {
        private static Faction m_Instance;
        public Shadowlords()
        {
            m_Instance = this;

            this.Definition =
                new FactionDefinition(
                    3,
                    1109, // shadow
                    2211, // green
                    1109, // join stone : shadow
                    2211, // broadcast : green
                    0x79, 0x3EB0, // war horse
                    "Shadowlords", "shadow", "SL",
                    new TextDefinition(1011537, "SHADOWLORDS"),
                    new TextDefinition(1060772, "Shadowlords faction"),
                    new TextDefinition(1011424, "<center>SHADES OF DARKNESS</center>"),
                    new TextDefinition(1011451,
                                               "The Shadow Lords are a faction that has sprung up within the ranks of " +
                                               "Minax. Comprised mostly of undead and those who would seek to be " +
                                               "necromancers, they pose a threat to both the sides of good and evil. " +
                                               "Their plans have disrupted the hold Minax has over Felucca, and their " +
                                               "ultimate goal is to destroy all life."),
                    new TextDefinition(1011456, "This city is controlled by the Shadow Lords."),
                    new TextDefinition(1042255, "This sigil has been corrupted by the Shadowlords"),
                    new TextDefinition(1041046, "The faction signup stone for the Shadowlords"),
                    new TextDefinition(1041384, "The Faction Stone of the Shadowlords"),
                    new TextDefinition(1011466, ": Shadowlords"),
                    new TextDefinition(1005184, "Minions of the Shadowlords will now be ignored."),
                    new TextDefinition(1005185, "Minions of the Shadowlords will now be warned of their impending deaths."),
                    new TextDefinition(1005186, "Minions of the Shadowlords will now be attacked at will."),
                    new StrongholdDefinition(
                        new Rectangle2D[]
                        {
                            new Rectangle2D(960, 688, 8, 9),
                            new Rectangle2D(944, 697, 24, 23)
                        },
                        new Point3D(969, 768, 0),
                        new Point3D(947, 713, 0),
                        new Point3D[]
                        {
                            new Point3D(953, 713, 20),
                            new Point3D(953, 709, 20),
                            new Point3D(953, 705, 20),
                            new Point3D(953, 701, 20),
                            new Point3D(957, 713, 20),
                            new Point3D(957, 709, 20),
                            new Point3D(957, 705, 20),
                            new Point3D(957, 701, 20)
                        },
                        new Point3D(948, 713, 0)),
                    new RankDefinition[]
                    {
                        new RankDefinition(10, 991, 8, new TextDefinition(1060799, "Purveyor of Darkness")),
                        new RankDefinition(9, 950, 7, new TextDefinition(1060798, "Agent of Evil")),
                        new RankDefinition(8, 900, 6, new TextDefinition(1060797, "Bringer of Sorrow")),
                        new RankDefinition(7, 800, 6, new TextDefinition(1060797, "Bringer of Sorrow")),
                        new RankDefinition(6, 700, 5, new TextDefinition(1060796, "Keeper of Lies")),
                        new RankDefinition(5, 600, 5, new TextDefinition(1060796, "Keeper of Lies")),
                        new RankDefinition(4, 500, 5, new TextDefinition(1060796, "Keeper of Lies")),
                        new RankDefinition(3, 400, 4, new TextDefinition(1060795, "Servant")),
                        new RankDefinition(2, 200, 4, new TextDefinition(1060795, "Servant")),
                        new RankDefinition(1, 0, 4, new TextDefinition(1060795, "Servant"))
                    },
                    new GuardDefinition[]
                    {
                        new GuardDefinition(typeof(FactionHenchman), 0x1403, 5000, 1000, 10, new TextDefinition(1011526, "HENCHMAN"), new TextDefinition(1011510, "Hire Henchman")),
                        new GuardDefinition(typeof(FactionMercenary),	0x0F62, 6000, 2000, 10, new TextDefinition(1011527, "MERCENARY"), new TextDefinition(1011511, "Hire Mercenary")),
                        new GuardDefinition(typeof(FactionDeathKnight),	0x0F45, 7000, 3000, 10, new TextDefinition(1011512, "DEATH KNIGHT"),	new TextDefinition(1011503, "Hire Death Knight")),
                        new GuardDefinition(typeof(FactionNecromancer),	0x13F8, 8000, 4000, 10, new TextDefinition(1011513, "SHADOW MAGE"),	new TextDefinition(1011504, "Hire Shadow Mage")),
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