using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public enum TalismanSlayerName
    {
        None,
        Bear,
        Vermin,
        Bat,
        Mage,
        Beetle,
        Bird,
        Ice,
        Flame,
        Bovine,
        Wolf,
        Undead,
        Goblin
    }

    public static class TalismanSlayer
    {
        private static readonly Dictionary<TalismanSlayerName, Type[]> m_Table = new Dictionary<TalismanSlayerName, Type[]>();
        public static void Initialize()
        {
            m_Table[TalismanSlayerName.Undead] = new Type[] 
            {
                typeof(AncientLich), typeof(Bogle), typeof(BoneKnight),
                typeof(BoneMagi), /* typeof( DarkGuardian ), */typeof(DarknightCreeper),
                typeof(FleshGolem), typeof(Ghoul), typeof(GoreFiend), typeof(HellSteed),
                typeof(LadyOfTheSnow), typeof(Lich), typeof(LichLord), typeof(Mummy),
                typeof(Revenant), typeof(RevenantLion), typeof(RottingCorpse), typeof(Shade),
                typeof(ShadowKnight), typeof(SkeletalKnight), typeof(SkeletalMage), typeof(SkeletalMount),
                typeof(Skeleton), typeof(Spectre), typeof(Wraith), typeof(Zombie),
                /* Mondain's Legacy */ typeof(UnfrozenMummy), typeof(RedDeath),
                typeof(SirPatrick), typeof(LadyJennifyr), typeof(MasterMikael),
                typeof(MasterJonath), typeof(LadyMarai) /* End */, typeof (PestilentBandage), typeof (PrimevalLich),
                typeof (DreamWraith), typeof (UndeadGargoyle), typeof (UndeadGuardian), typeof (PutridUndeadGuardian)
            };
            m_Table[TalismanSlayerName.Bear] = new Type[]
            {
                typeof(GrizzlyBear), typeof(BlackBear), typeof(BrownBear), typeof(PolarBear), typeof(Grobu), typeof(GrizzlyBear), typeof(RagingGrizzlyBear)
            };

            m_Table[TalismanSlayerName.Vermin] = new Type[]
            {
                typeof(RatmanMage), typeof(RatmanMage), typeof(RatmanArcher), //typeof(Barracoon),
                typeof(Ratman), typeof(Sewerrat), typeof(Rat), typeof(GiantRat) //, typeof( Chiikkaha )
            };

            m_Table[TalismanSlayerName.Bat] = new Type[]
            {
                typeof(Mongbat), typeof(StrongMongbat), typeof(VampireBat)
            };
			
            m_Table[TalismanSlayerName.Mage] = new Type[]
            {
                typeof(EvilMage), typeof(EvilMageLord), typeof(AncientLich), typeof(Lich), typeof(LichLord),
                typeof(SkeletalMage), typeof(BoneMagi), typeof(OrcishMage), typeof(KhaldunZealot), typeof(JukaMage),
            };

            m_Table[TalismanSlayerName.Beetle] = new Type[]
            {
                typeof(Beetle), typeof(RuneBeetle), typeof(FireBeetle), typeof(DeathwatchBeetle),
                typeof(DeathwatchBeetleHatchling)
            };

            m_Table[TalismanSlayerName.Bird] = new Type[]
            {
                typeof(Bird), typeof(TropicalBird), typeof(Chicken), typeof(Crane),
                typeof(DesertOstard), typeof(Eagle), typeof(ForestOstard), typeof(FrenziedOstard),
                typeof(Phoenix), typeof(Pyre), typeof(Swoop), typeof(Saliva), typeof(Harpy),
                typeof(StoneHarpy) // ?????
            };

            m_Table[TalismanSlayerName.Ice] = new Type[]
            {
                typeof(ArcticOgreLord), typeof(IceElemental), typeof(SnowElemental), typeof(FrostOoze),
                typeof(IceFiend), typeof(UnfrozenMummy), typeof(FrostSpider), typeof(LadyOfTheSnow),
                typeof(FrostTroll),

                // TODO WinterReaper, check
                typeof(IceSnake), typeof(SnowLeopard), typeof(PolarBear), typeof(IceSerpent), typeof(GiantIceWorm),
                typeof(IceHound), typeof(Wight)
            };
			
            m_Table[TalismanSlayerName.Flame] = new Type[]
            {
                typeof(FireBeetle), typeof(HellHound), typeof(LavaSerpent), typeof(FireElemental),
                typeof(PredatorHellCat), typeof(Phoenix), typeof(FireGargoyle), typeof(HellCat),
                typeof(Pyre), typeof(FireSteed), typeof(LavaLizard),

                // TODO check
                typeof(LavaSnake),
            };

            m_Table[TalismanSlayerName.Bovine] = new Type[]
            {
                typeof(Cow), typeof(Bull), typeof(Gaman), typeof(MinotaurCaptain),
                typeof(MinotaurScout), typeof(Minotaur), typeof(TormentedMinotaur), typeof(LowlandBoura), typeof(RuddyBoura)
            /*, typeof( HighPlanesBoura ) */ };

            m_Table[TalismanSlayerName.Wolf] = new Type[]
            {
                typeof(CuSidhe), typeof(Gnaw), typeof(TimberWolf), typeof(DireWolf),
                typeof(GreyWolf), typeof(TsukiWolf), typeof(Dog), typeof(HellHound),
                typeof(IceHound), typeof(WhiteWolf)
            };
			
            m_Table[TalismanSlayerName.Goblin] = new Type[]
            {
                typeof(EnslavedGoblinScout), typeof(EnslavedGoblinKeeper), typeof(EnslavedGreenGoblin),
                typeof(EnslavedGreenGoblinAlchemist), typeof(EnslavedGoblinMage), typeof(EnslavedGreenGoblinAlchemist),
                typeof(EnslavedGrayGoblin), typeof(GreenGoblinScout), typeof(GreenGoblinAlchemist),
                typeof(GreenGoblin), typeof(GrayGoblinMage), typeof(GrayGoblinKeeper), typeof(GrayGoblin),
                typeof(GreenGoblinAlchemistRenowned), typeof(GrayGoblinMageRenowned)
            };
        }

        public static bool Slays(TalismanSlayerName name, Mobile m)
        {

            if (m.SpecialSlayerMechanics)
            {
                if (m.SlayerVulnerabilities.Contains(name.ToString()))
                    return true;
                else
                    return false;
                
            }

            if (!m_Table.ContainsKey(name))
                return false;

            Type[] types = m_Table[name];
			
            if (types == null || m == null)
                return false;

            Type type = m.GetType();

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsAssignableFrom(type))
                    return true;
            }

            return false;
        }
    }
}