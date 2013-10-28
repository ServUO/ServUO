using System;
using Server.Mobiles;

namespace Server.Engines.CannedEvil
{
    public enum ChampionSpawnType
    {
        Abyss,
        Arachnid,
        ColdBlood,
        ForestLord,
        VerminHorde,
        UnholyTerror,
        SleepingDragon,
        Glade,
        Corrupt,
        #region Stygian Abyss
        Terror,
        Infuse
        #endregion
    }

    public class ChampionSpawnInfo
    {
        private readonly string m_Name;
        private readonly Type m_Champion;
        private readonly Type[][] m_SpawnTypes;
        private readonly string[] m_LevelNames;

        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public Type Champion
        {
            get
            {
                return this.m_Champion;
            }
        }
        public Type[][] SpawnTypes
        {
            get
            {
                return this.m_SpawnTypes;
            }
        }
        public string[] LevelNames
        {
            get
            {
                return this.m_LevelNames;
            }
        }

        public ChampionSpawnInfo(string name, Type champion, string[] levelNames, Type[][] spawnTypes)
        {
            this.m_Name = name;
            this.m_Champion = champion;
            this.m_LevelNames = levelNames;
            this.m_SpawnTypes = spawnTypes;
        }

        public static ChampionSpawnInfo[] Table
        {
            get
            {
                return m_Table;
            }
        }

        private static readonly ChampionSpawnInfo[] m_Table = new ChampionSpawnInfo[]
        {
            new ChampionSpawnInfo("Abyss", typeof(Semidar), new string[] { "Foe", "Assassin", "Conqueror" }, new Type[][]	// Abyss
            { // Abyss
                new Type[] { typeof(GreaterMongbat), typeof(Imp) }, // Level 1
                new Type[] { typeof(Gargoyle), typeof(Harpy) }, // Level 2
                new Type[] { typeof(FireGargoyle), typeof(StoneGargoyle) }, // Level 3
                new Type[] { typeof(Daemon), typeof(Succubus) }// Level 4
            }),
            new ChampionSpawnInfo("Arachnid", typeof(Mephitis), new string[] { "Bane", "Killer", "Vanquisher" }, new Type[][]	// Arachnid
            { // Arachnid
                new Type[] { typeof(Scorpion), typeof(GiantSpider) }, // Level 1
                new Type[] { typeof(TerathanDrone), typeof(TerathanWarrior) }, // Level 2
                new Type[] { typeof(DreadSpider), typeof(TerathanMatriarch) }, // Level 3
                new Type[] { typeof(PoisonElemental), typeof(TerathanAvenger) }// Level 4
            }),
            new ChampionSpawnInfo("Cold Blood", typeof(Rikktor), new string[] { "Blight", "Slayer", "Destroyer" }, new Type[][]	// Cold Blood
            { // Cold Blood
                new Type[] { typeof(Lizardman), typeof(Snake) }, // Level 1
                new Type[] { typeof(LavaLizard), typeof(OphidianWarrior) }, // Level 2
                new Type[] { typeof(Drake), typeof(OphidianArchmage) }, // Level 3
                new Type[] { typeof(Dragon), typeof(OphidianKnight) }// Level 4
            }),
            new ChampionSpawnInfo("Forest Lord", typeof(LordOaks), new string[] { "Enemy", "Curse", "Slaughterer" }, new Type[][]	// Forest Lord
            { // Forest Lord
                new Type[] { typeof(Pixie), typeof(ShadowWisp) }, // Level 1
                new Type[] { typeof(Kirin), typeof(Wisp) }, // Level 2
                new Type[] { typeof(Centaur), typeof(Unicorn) }, // Level 3
                new Type[] { typeof(EtherealWarrior), typeof(SerpentineDragon) }// Level 4
            }),
            new ChampionSpawnInfo("Vermin Horde", typeof(Barracoon), new string[] { "Adversary", "Subjugator", "Eradicator" }, new Type[][]	// Vermin Horde
            { // Vermin Horde
                new Type[] { typeof(GiantRat), typeof(Slime) }, // Level 1
                new Type[] { typeof(DireWolf), typeof(Ratman) }, // Level 2
                new Type[] { typeof(HellHound), typeof(RatmanMage) }, // Level 3
                new Type[] { typeof(RatmanArcher), typeof(SilverSerpent) }// Level 4
            }),
            new ChampionSpawnInfo("Unholy Terror", typeof(Neira), new string[] { "Scourge", "Punisher", "Nemesis" }, new Type[][]	// Unholy Terror
            { // Unholy Terror
                (Core.AOS ? new Type[] { typeof(Bogle), typeof(Ghoul), typeof(Shade), typeof(Spectre), typeof(Wraith) }// Level 1 (Pre-AoS)
                 : new Type[] { typeof(Ghoul), typeof(Shade), typeof(Spectre), typeof(Wraith) }), // Level 1

                new Type[] { typeof(BoneMagi), typeof(Mummy), typeof(SkeletalMage) }, // Level 2
                new Type[] { typeof(BoneKnight), typeof(Lich), typeof(SkeletalKnight) }, // Level 3
                new Type[] { typeof(LichLord), typeof(RottingCorpse) }// Level 4
            }),
            new ChampionSpawnInfo("Sleeping Dragon", typeof(Serado), new string[] { "Rival", "Challenger", "Antagonist" }, new Type[][]
            { // Unholy Terror
                new Type[] { typeof(DeathwatchBeetleHatchling), typeof(Lizardman) },
                new Type[] { typeof(DeathwatchBeetle), typeof(Kappa) },
                new Type[] { typeof(LesserHiryu), typeof(RevenantLion) },
                new Type[] { typeof(Hiryu), typeof(Oni) }
            }),
            new ChampionSpawnInfo("Glade", typeof(Twaulo), new string[] { "Banisher", "Enforcer", "Eradicator" }, new Type[][]
            { // Glade
                new Type[] { typeof(Pixie), typeof(ShadowWisp) },
                new Type[] { typeof(Centaur), typeof(MLDryad) },
                new Type[] { typeof(Satyr), typeof(CuSidhe) },
                new Type[] { typeof(FerelTreefellow), typeof(RagingGrizzlyBear) }
            }),
            new ChampionSpawnInfo("Corrupt", typeof(Ilhenir), new string[] { "Cleanser", "Expunger", "Depurator" }, new Type[][]
            { // Corrupt
                new Type[] { typeof(PlagueSpawn), typeof(Bogling) },
                new Type[] { typeof(PlagueBeast), typeof(BogThing) },
                new Type[] { typeof(PlagueBeastLord), typeof(InterredGrizzle) },
                new Type[] { typeof(FetidEssence), typeof(PestilentBandage) }
            }),
            #region SA
            new ChampionSpawnInfo("Terror", typeof(AbyssalInfernal), new string[] { "Banisher", "Enforcer", "Eradicator" }, new Type[][]
            { // Terror
                new Type[] { typeof(HordeMinion), typeof(ChaosDaemon) }, // Level 1
                new Type[] { typeof(StoneHarpy), typeof(ArcaneDaemon) }, // Level 2
                new Type[] { typeof(PitFiend), typeof(Moloch) }, // Level 3
                new Type[] { typeof(ArchDaemon), typeof(AbyssalAbomination) }// Level 4
            }),
            new ChampionSpawnInfo("Infuse", typeof(PrimevalLich), new string[] { "Cleanser", "Expunger", "Depurator" }, new Type[][]
            { // Infused
                new Type[] { typeof(GoreFiend), typeof(VampireBat) }, // Level 1
                new Type[] { typeof(FleshGolem), typeof(DarkWisp) }, // Level 2
                new Type[] { typeof(UndeadGargoyle), typeof(Wight) }, // Level 3
                new Type[] { typeof(SkeletalDrake), typeof(DreamWraith) }// Level 4
            })
            #endregion
        };

        public static ChampionSpawnInfo GetInfo(ChampionSpawnType type)
        {
            int v = (int)type;

            if (v < 0 || v >= m_Table.Length)
                v = 0;

            return m_Table[v];
        }
    }
}