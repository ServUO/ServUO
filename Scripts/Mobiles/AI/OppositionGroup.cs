using System;
using Server.Mobiles;

namespace Server
{
    public class OppositionGroup
    {
        private static readonly OppositionGroup m_TerathansAndOphidians = new OppositionGroup(new Type[][]
        {
            new Type[]
            {
                typeof(TerathanAvenger),
                typeof(TerathanDrone),
                typeof(TerathanMatriarch),
                typeof(TerathanWarrior)
            },
            new Type[]
            {
                typeof(OphidianArchmage),
                typeof(OphidianKnight),
                typeof(OphidianMage),
                typeof(OphidianMatriarch),
                typeof(OphidianWarrior)
            }
        });
        private static readonly OppositionGroup m_SavagesAndOrcs = new OppositionGroup(new Type[][]
        {
            new Type[]
            {
                typeof(Orc),
                typeof(OrcBomber),
                typeof(OrcBrute),
                typeof(OrcCaptain),
                typeof(OrcChopper),
                typeof(OrcishLord),
                typeof(OrcishMage),
                typeof(OrcScout),
                typeof(SpawnedOrcishLord)
            },
            new Type[]
            {
                typeof(Savage),
                typeof(SavageRider),
                typeof(SavageRidgeback),
                typeof(SavageShaman)
            }
        });
        private static readonly OppositionGroup m_FeyAndUndead = new OppositionGroup(new Type[][]
        {
            new Type[]
            {
                typeof(Centaur),
                typeof(EtherealWarrior),
                typeof(Kirin),
                typeof(LordOaks),
                typeof(Pixie),
                typeof(Silvani),
                typeof(Unicorn),
                typeof(Wisp),
                typeof(Treefellow),
                typeof(MLDryad),
                typeof(Satyr)
            },
            new Type[]
            {
                typeof(AncientLich),
                typeof(Bogle),
                typeof(BoneKnight),
                typeof(BoneMagi),
                typeof(DarknightCreeper),
                typeof(Ghoul),
                typeof(LadyOfTheSnow),
                typeof(Lich),
                typeof(LichLord),
                typeof(Mummy),
                typeof(RevenantLion),
                typeof(RottingCorpse),
                typeof(Shade),
                typeof(ShadowKnight),
                typeof(SkeletalDragon),
                typeof(SkeletalDrake),
                typeof(SkeletalKnight),
                typeof(SkeletalMage),
                typeof(Skeleton),
                typeof(Spectre),
                typeof(Wraith),
                typeof(Zombie)
            }
        });
        private readonly Type[][] m_Types;
        public OppositionGroup(Type[][] types)
        {
            this.m_Types = types;
        }

        public static OppositionGroup TerathansAndOphidians
        {
            get
            {
                return m_TerathansAndOphidians;
            }
        }
        public static OppositionGroup SavagesAndOrcs
        {
            get
            {
                return m_SavagesAndOrcs;
            }
        }
        public static OppositionGroup FeyAndUndead
        {
            get
            {
                return m_FeyAndUndead;
            }
        }
        public bool IsEnemy(object from, object target)
        {
            int fromGroup = this.IndexOf(from);
            int targGroup = this.IndexOf(target);

            return (fromGroup != -1 && targGroup != -1 && fromGroup != targGroup);
        }

        public int IndexOf(object obj)
        {
            if (obj == null)
                return -1;

            Type type = obj.GetType();

            for (int i = 0; i < this.m_Types.Length; ++i)
            {
                Type[] group = this.m_Types[i];

                bool contains = false;

                for (int j = 0; !contains && j < group.Length; ++j)
                    contains = group[j].IsAssignableFrom(type);

                if (contains)
                    return i;
            }

            return -1;
        }
    }
}
