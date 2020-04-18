using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.MiniChamps
{
    public enum MiniChampType
    {
        CrimsonVeins,
        FairyDragonLair,
        AbyssalLair,
        DiscardedCavernClanRibbon,
        DiscardedCavernClanScratch,
        DiscardedCavernClanChitter,
        PassageofTears,
        LandsoftheLich,
        SecretGarden,
        FireTemple,
        EnslavedGoblins,
        SkeletalDragon,
        LavaCaldera,
        MeraktusTheTormented
    }

    public class MiniChampTypeInfo
    {
        public int Required { get; set; }
        public Type SpawnType { get; set; }

        public MiniChampTypeInfo(int required, Type spawnType)
        {
            Required = required;
            SpawnType = spawnType;
        }
    }

    public class MiniChampLevelInfo
    {
        public MiniChampTypeInfo[] Types { get; set; }

        public MiniChampLevelInfo(params MiniChampTypeInfo[] types)
        {
            Types = types;
        }
    }

    public class MiniChampInfo
    {
        public MiniChampLevelInfo[] Levels { get; set; }
        public Type EssenceType { get; set; }

        public int MaxLevel => Levels == null ? 0 : Levels.Length;

        public MiniChampInfo(Type essenceType, params MiniChampLevelInfo[] levels)
        {
            Levels = levels;
            EssenceType = essenceType;
        }

        public MiniChampLevelInfo GetLevelInfo(int level)
        {
            level--;

            if (level >= 0 && level < Levels.Length)
                return Levels[level];

            return null;
        }

        public static MiniChampInfo[] Table => m_Table;

        private static readonly MiniChampInfo[] m_Table = new MiniChampInfo[]
        {
            new MiniChampInfo // Crimson Veins
            (
                typeof(EssencePrecision),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(20, typeof(FireAnt)),
                    new MiniChampTypeInfo(10, typeof(LavaSnake)),
                    new MiniChampTypeInfo(10, typeof(LavaLizard))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(5, typeof(Efreet)),
                    new MiniChampTypeInfo(5, typeof(FireGargoyle))
                ),
                new MiniChampLevelInfo // Level 3
                (
                    new MiniChampTypeInfo(10, typeof(LavaElemental)),
                    new MiniChampTypeInfo(5, typeof(FireDaemon))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(FireElementalRenowned))
                )
            ),
            new MiniChampInfo // Fairy Dragon Lair
            (
                typeof(EssenceDiligence),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(25, typeof(FairyDragon))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(10, typeof(Wyvern))
                ),
                new MiniChampLevelInfo // Level 3
                (
                    new MiniChampTypeInfo(10, typeof(ForgottenServant))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(WyvernRenowned))
                )
            ),
            new MiniChampInfo // Abyssal Lair
            (
                typeof(EssenceAchievement),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(20, typeof(GreaterMongbat)),
                    new MiniChampTypeInfo(20, typeof(Imp))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(10, typeof(Daemon))
                ),
                new MiniChampLevelInfo // Level 3
                (
                    new MiniChampTypeInfo(5, typeof(PitFiend))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(DevourerRenowned))
                )
            ),
            new MiniChampInfo // Discarded Cavern Clan Ribbon
            (
                typeof(EssenceBalance),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(10, typeof(ClanRibbonPlagueRat)),
                    new MiniChampTypeInfo(10, typeof(ClanRS))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(10, typeof(ClanRibbonPlagueRat)),
                    new MiniChampTypeInfo(10, typeof(ClanRC))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(VitaviRenowned))
                )
            ),
            new MiniChampInfo // Discarded Cavern Clan Scratch
            (
                typeof(EssenceBalance),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(10, typeof(ClanSSW)),
                    new MiniChampTypeInfo(10, typeof(ClanSS))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(10, typeof(ClanSSW)),
                    new MiniChampTypeInfo(10, typeof(ClanSH))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(TikitaviRenowned))
                )
            ),
            new MiniChampInfo // Discarded Cavern Clan Chitter
            (
                typeof(EssenceBalance),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(10, typeof(ClockworkScorpion)),
                    new MiniChampTypeInfo(10, typeof(ClanCA))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(10, typeof(ClockworkScorpion)),
                    new MiniChampTypeInfo(10, typeof(ClanCT))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(RakktaviRenowned))
                )
            ),
            new MiniChampInfo // Passage of Tears
            (
                typeof(EssenceSingularity),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(10, typeof(AcidSlug)),
                    new MiniChampTypeInfo(20, typeof(CorrosiveSlime))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(10, typeof(AcidElemental))
                ),
                new MiniChampLevelInfo // Level 3
                (
                    new MiniChampTypeInfo(3, typeof(InterredGrizzle))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(AcidElementalRenowned))
                )
            ),
            new MiniChampInfo // Lands of the Lich
            (
                typeof(EssenceDirection),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(5, typeof(Wraith)),
                    new MiniChampTypeInfo(10, typeof(Spectre)),
                    new MiniChampTypeInfo(5, typeof(Shade)),
                    new MiniChampTypeInfo(30, typeof(Skeleton)),
                    new MiniChampTypeInfo(20, typeof(Zombie))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(5, typeof(BoneMagi)),
                    new MiniChampTypeInfo(10, typeof(SkeletalMage)),
                    new MiniChampTypeInfo(10, typeof(BoneKnight)),
                    new MiniChampTypeInfo(10, typeof(SkeletalKnight)),
                    new MiniChampTypeInfo(10, typeof(WailingBanshee))
                ),
                new MiniChampLevelInfo // Level 3
                (
                    new MiniChampTypeInfo(5, typeof(SkeletalLich)),
                    new MiniChampTypeInfo(20, typeof(RottingCorpse))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(AncientLichRenowned))
                )
            ),
            new MiniChampInfo // Secret Garden
            (
                typeof(EssenceFeeling),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(20, typeof(Pixie))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(15, typeof(Wisp))
                ),
                new MiniChampLevelInfo // Level 3
                (
                    new MiniChampTypeInfo(10, typeof(DarkWisp))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(PixieRenowned))
                )
            ),
            new MiniChampInfo // Fire Temple Ruins
            (
                typeof(EssenceOrder),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(20, typeof(LavaSnake)),
                    new MiniChampTypeInfo(10, typeof(LavaLizard)),
                    new MiniChampTypeInfo(10, typeof(FireAnt))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(10, typeof(LavaSerpent)),
                    new MiniChampTypeInfo(10, typeof(HellCat)),
                    new MiniChampTypeInfo(10, typeof(HellHound))
                ),
                new MiniChampLevelInfo // Level 3
                (
                    new MiniChampTypeInfo(5, typeof(FireDaemon)),
                    new MiniChampTypeInfo(10, typeof(LavaElemental))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(FireDaemonRenowned))
                )
            ),
            new MiniChampInfo // Enslaved Goblins
            (
                typeof(EssenceControl),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(10, typeof(EnslavedGrayGoblin)),
                    new MiniChampTypeInfo(15, typeof(EnslavedGreenGoblin))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(10, typeof(EnslavedGoblinScout)),
                    new MiniChampTypeInfo(10, typeof(EnslavedGoblinKeeper))
                ),
                new MiniChampLevelInfo // Level 3
                (
                    new MiniChampTypeInfo(5, typeof(EnslavedGoblinMage)),
                    new MiniChampTypeInfo(5, typeof(EnslavedGreenGoblinAlchemist))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(GrayGoblinMageRenowned)),
                    new MiniChampTypeInfo(1, typeof(GreenGoblinAlchemistRenowned))
                )
            ),
            new MiniChampInfo // Skeletal Dragon
            (
                typeof(EssencePersistence),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(5, typeof(PatchworkSkeleton)),
                    new MiniChampTypeInfo(15, typeof(Skeleton))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(5, typeof(BoneKnight)),
                    new MiniChampTypeInfo(5, typeof(SkeletalKnight))
                ),
                new MiniChampLevelInfo // Level 3
                (
                    new MiniChampTypeInfo(5, typeof(BoneMagi)),
                    new MiniChampTypeInfo(2, typeof(SkeletalMage))
                ),
                new MiniChampLevelInfo // Level 4
                (
                    new MiniChampTypeInfo(2, typeof(SkeletalLich))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(SkeletalDragonRenowned))
                )
            ),
            new MiniChampInfo // Lava Caldera
            (
                typeof(EssencePassion),
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(10, typeof(LavaSnake)),
                    new MiniChampTypeInfo(10, typeof(LavaLizard)),
                    new MiniChampTypeInfo(20, typeof(FireAnt))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(10, typeof(LavaSerpent)),
                    new MiniChampTypeInfo(10, typeof(HellCat)),
                    new MiniChampTypeInfo(10, typeof(HellHound))
                ),
                new MiniChampLevelInfo // Level 3
                (
                    new MiniChampTypeInfo(5, typeof(FireDaemon)),
                    new MiniChampTypeInfo(10, typeof(LavaElemental))
                ),
                new MiniChampLevelInfo // Renowned
                (
                    new MiniChampTypeInfo(1, typeof(FireDaemonRenowned))
                )
            ),
            new MiniChampInfo // Meraktus the Tormented
            (
                null,
                new MiniChampLevelInfo // Level 1
                (
                    new MiniChampTypeInfo(20, typeof(Minotaur))
                ),
                new MiniChampLevelInfo // Level 2
                (
                    new MiniChampTypeInfo(20, typeof(MinotaurScout))
                ),
                new MiniChampLevelInfo // Level 3
                (
                    new MiniChampTypeInfo(15, typeof(MinotaurCaptain))
                ),
                new MiniChampLevelInfo // Level 4
                (
                    new MiniChampTypeInfo(15, typeof(MinotaurCaptain))
                ),
                new MiniChampLevelInfo // Champion
                (
                    new MiniChampTypeInfo(1, typeof(Meraktus))
                )
            ),
        };

        public static MiniChampInfo GetInfo(MiniChampType type)
        {
            int v = (int)type;

            if (v < 0 || v >= m_Table.Length)
                v = 0;

            return m_Table[v];
        }
    }
}