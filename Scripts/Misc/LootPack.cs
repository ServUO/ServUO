#region References
using Server.Items;
using Server.Mobiles;
using System;
#endregion

namespace Server
{
    public class LootPack
    {
        public static int GetLuckChance(Mobile killer, Mobile victim)
        {
            int luck = killer is PlayerMobile ? ((PlayerMobile)killer).RealLuck : killer.Luck;

            PlayerMobile pmKiller = killer as PlayerMobile;

            if (pmKiller != null && pmKiller.SentHonorContext != null && pmKiller.SentHonorContext.Target == victim)
            {
                luck += pmKiller.SentHonorContext.PerfectionLuckBonus;
            }

            if (luck < 0)
            {
                return 0;
            }

            return GetLuckChance(luck);
        }

        public static int GetLuckChance(int luck)
        {
            return (int)(Math.Pow(luck, 1 / 1.8) * 100);
        }

        public static int GetLuckChanceForKiller(Mobile m)
        {
            BaseCreature dead = m as BaseCreature;

            if (dead == null)
                return 240;

            System.Collections.Generic.List<DamageStore> list = dead.GetLootingRights();

            DamageStore highest = null;

            for (int i = 0; i < list.Count; ++i)
            {
                DamageStore ds = list[i];

                if (ds.m_HasRight && (highest == null || ds.m_Damage > highest.m_Damage))
                {
                    highest = ds;
                }
            }

            if (highest == null)
            {
                return 0;
            }

            return GetLuckChance(highest.m_Mobile, dead);
        }

        public static bool CheckLuck(int chance)
        {
            return (chance > Utility.Random(10000));
        }

        private readonly LootPackEntry[] m_Entries;

        public LootPack(LootPackEntry[] entries)
        {
            m_Entries = entries;
        }

        public void Generate(IEntity e)
        {
            BaseContainer cont = null;
            LootStage stage = LootStage.Death;
            int luckChance = 0;
            bool hasBeenStolenFrom = false;
            Mobile from = e as Mobile;

            if (e is BaseCreature)
            {
                var bc = (BaseCreature)e;

                cont = bc.Backpack as BaseContainer;
                stage = bc.LootStage;
                luckChance = bc.KillersLuck;
                hasBeenStolenFrom = bc.StealPackGenerated;
            }
            else
            {
                cont = e as BaseContainer;
            }

            if (cont != null)
            {
                Generate(from, cont, stage, luckChance, hasBeenStolenFrom);
            }
        }

        public void Generate(Mobile from, Container cont, bool spawning, int luckChance)
        {
            Generate(from, cont as BaseContainer, spawning ? LootStage.Spawning : LootStage.Death, luckChance, false);
        }

        public void Generate(IEntity from, BaseContainer cont, LootStage stage, int luckChance, bool hasBeenStolenFrom)
        {
            if (cont == null)
            {
                return;
            }

            bool checkLuck = true;

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                LootPackEntry entry = m_Entries[i];

                if (!entry.CanGenerate(stage, hasBeenStolenFrom))
                    continue;

                bool shouldAdd = (entry.Chance > Utility.Random(10000));

                if (!shouldAdd && checkLuck)
                {
                    checkLuck = false;

                    if (CheckLuck(luckChance))
                    {
                        shouldAdd = (entry.Chance > Utility.Random(10000));
                    }
                }

                if (!shouldAdd)
                {
                    continue;
                }

                Item item = entry.Construct(from, luckChance, stage, hasBeenStolenFrom);

                if (item != null)
                {
                    if (from is BaseCreature && item.LootType == LootType.Blessed)
                    {
                        Timer.DelayCall(TimeSpan.FromMilliseconds(25), () =>
                        {
                            var corpse = ((BaseCreature)from).Corpse;

                            if (corpse != null)
                            {
                                if (!corpse.TryDropItem((BaseCreature)from, item, false))
                                {
                                    corpse.DropItem(item);
                                }
                            }
                            else
                            {
                                item.Delete();
                            }
                        });
                    }
                    else if (item.Stackable)
                    {
                        cont.DropItemStacked(item);
                    }
                    else
                    {
                        cont.DropItem(item);
                    }
                }
            }
        }

        public static readonly LootPackItem[] Gold = new[] { new LootPackItem(typeof(Gold), 1) };

        public static readonly LootPackItem[] Instruments = new[] { new LootPackItem(typeof(BaseInstrument), 1) };

        // Circles 1 - 3
        public static readonly LootPackItem[] LowScrollItems = new[]
        {
            new LootPackItem(typeof(ReactiveArmorScroll), 1),
            new LootPackItem(typeof(ClumsyScroll), 1),
            new LootPackItem(typeof(CreateFoodScroll), 1),
            new LootPackItem(typeof(FeeblemindScroll), 1),
            new LootPackItem(typeof(HealScroll), 1),
            new LootPackItem(typeof(MagicArrowScroll), 1),
            new LootPackItem(typeof(NightSightScroll), 1),
            new LootPackItem(typeof(WeakenScroll), 1),
            new LootPackItem(typeof(AgilityScroll), 1),
            new LootPackItem(typeof(CunningScroll), 1),
            new LootPackItem(typeof(CureScroll), 1),
            new LootPackItem(typeof(HarmScroll), 1),
            new LootPackItem(typeof(MagicTrapScroll), 1),
            new LootPackItem(typeof(MagicUnTrapScroll), 1),
            new LootPackItem(typeof(ProtectionScroll), 1),
            new LootPackItem(typeof(StrengthScroll), 1),
            new LootPackItem(typeof(BlessScroll), 1),
            new LootPackItem(typeof(FireballScroll), 1),
            new LootPackItem(typeof(MagicLockScroll), 1),
            new LootPackItem(typeof(PoisonScroll), 1),
            new LootPackItem(typeof(TelekinisisScroll), 1),
            new LootPackItem(typeof(TeleportScroll), 1),
            new LootPackItem(typeof(UnlockScroll), 1),
            new LootPackItem(typeof(WallOfStoneScroll), 1)
        };

        // Circles 4 - 6
        public static readonly LootPackItem[] MedScrollItems = new[]
        {
            new LootPackItem(typeof(ArchCureScroll), 1),
            new LootPackItem(typeof(ArchProtectionScroll), 1),
            new LootPackItem(typeof(CurseScroll), 1),
            new LootPackItem(typeof(FireFieldScroll), 1),
            new LootPackItem(typeof(GreaterHealScroll), 1),
            new LootPackItem(typeof(LightningScroll), 1),
            new LootPackItem(typeof(ManaDrainScroll), 1),
            new LootPackItem(typeof(RecallScroll), 1),
            new LootPackItem(typeof(BladeSpiritsScroll), 1),
            new LootPackItem(typeof(DispelFieldScroll), 1),
            new LootPackItem(typeof(IncognitoScroll), 1),
            new LootPackItem(typeof(MagicReflectScroll), 1),
            new LootPackItem(typeof(MindBlastScroll), 1),
            new LootPackItem(typeof(ParalyzeScroll), 1),
            new LootPackItem(typeof(PoisonFieldScroll), 1),
            new LootPackItem(typeof(SummonCreatureScroll), 1),
            new LootPackItem(typeof(DispelScroll), 1),
            new LootPackItem(typeof(EnergyBoltScroll), 1),
            new LootPackItem(typeof(ExplosionScroll), 1),
            new LootPackItem(typeof(InvisibilityScroll), 1),
            new LootPackItem(typeof(MarkScroll), 1),
            new LootPackItem(typeof(MassCurseScroll), 1),
            new LootPackItem(typeof(ParalyzeFieldScroll), 1),
            new LootPackItem(typeof(RevealScroll), 1)
        };

        // Circles 7 - 8
        public static readonly LootPackItem[] HighScrollItems = new[]
        {
            new LootPackItem(typeof(ChainLightningScroll), 1),
            new LootPackItem(typeof(EnergyFieldScroll), 1),
            new LootPackItem(typeof(FlamestrikeScroll), 1),
            new LootPackItem(typeof(GateTravelScroll), 1),
            new LootPackItem(typeof(ManaVampireScroll), 1),
            new LootPackItem(typeof(MassDispelScroll), 1),
            new LootPackItem(typeof(MeteorSwarmScroll), 1),
            new LootPackItem(typeof(PolymorphScroll), 1),
            new LootPackItem(typeof(EarthquakeScroll), 1),
            new LootPackItem(typeof(EnergyVortexScroll), 1),
            new LootPackItem(typeof(ResurrectionScroll), 1),
            new LootPackItem(typeof(SummonAirElementalScroll), 1),
            new LootPackItem(typeof(SummonDaemonScroll), 1),
            new LootPackItem(typeof(SummonEarthElementalScroll), 1),
            new LootPackItem(typeof(SummonFireElementalScroll), 1),
            new LootPackItem(typeof(SummonWaterElementalScroll), 1)
        };

        public static readonly LootPackItem[] MageryScrollItems = new[]
        {
            new LootPackItem(typeof(ReactiveArmorScroll), 1), new LootPackItem(typeof(ClumsyScroll), 1), new LootPackItem(typeof(CreateFoodScroll), 1), new LootPackItem(typeof(FeeblemindScroll), 1),
            new LootPackItem(typeof(HealScroll), 1), new LootPackItem(typeof(MagicArrowScroll), 1), new LootPackItem(typeof(NightSightScroll), 1), new LootPackItem(typeof(WeakenScroll), 1), new LootPackItem(typeof(AgilityScroll), 1),
            new LootPackItem(typeof(CunningScroll), 1), new LootPackItem(typeof(CureScroll), 1), new LootPackItem(typeof(HarmScroll), 1), new LootPackItem(typeof(MagicTrapScroll), 1), new LootPackItem(typeof(MagicUnTrapScroll), 1),
            new LootPackItem(typeof(ProtectionScroll), 1), new LootPackItem(typeof(StrengthScroll), 1), new LootPackItem(typeof(BlessScroll), 1), new LootPackItem(typeof(FireballScroll), 1),
            new LootPackItem(typeof(MagicLockScroll), 1), new LootPackItem(typeof(PoisonScroll), 1), new LootPackItem(typeof(TelekinisisScroll), 1), new LootPackItem(typeof(TeleportScroll), 1),
            new LootPackItem(typeof(UnlockScroll), 1), new LootPackItem(typeof(WallOfStoneScroll), 1), new LootPackItem(typeof(ArchCureScroll), 1), new LootPackItem(typeof(ArchProtectionScroll), 1),
            new LootPackItem(typeof(CurseScroll), 1), new LootPackItem(typeof(FireFieldScroll), 1), new LootPackItem(typeof(GreaterHealScroll), 1), new LootPackItem(typeof(LightningScroll), 1),
            new LootPackItem(typeof(ManaDrainScroll), 1), new LootPackItem(typeof(RecallScroll), 1), new LootPackItem(typeof(BladeSpiritsScroll), 1), new LootPackItem(typeof(DispelFieldScroll), 1),
            new LootPackItem(typeof(IncognitoScroll), 1), new LootPackItem(typeof(MagicReflectScroll), 1), new LootPackItem(typeof(MindBlastScroll), 1), new LootPackItem(typeof(ParalyzeScroll), 1),
            new LootPackItem(typeof(PoisonFieldScroll), 1), new LootPackItem(typeof(SummonCreatureScroll), 1), new LootPackItem(typeof(DispelScroll), 1), new LootPackItem(typeof(EnergyBoltScroll), 1),
            new LootPackItem(typeof(ExplosionScroll), 1), new LootPackItem(typeof(InvisibilityScroll), 1), new LootPackItem(typeof(MarkScroll), 1), new LootPackItem(typeof(MassCurseScroll), 1),
            new LootPackItem(typeof(ParalyzeFieldScroll), 1), new LootPackItem(typeof(RevealScroll), 1), new LootPackItem(typeof(ChainLightningScroll), 1), new LootPackItem(typeof(EnergyFieldScroll), 1),
            new LootPackItem(typeof(FlamestrikeScroll), 1), new LootPackItem(typeof(GateTravelScroll), 1), new LootPackItem(typeof(ManaVampireScroll), 1), new LootPackItem(typeof(MassDispelScroll), 1),
            new LootPackItem(typeof(MeteorSwarmScroll), 1), new LootPackItem(typeof(PolymorphScroll), 1), new LootPackItem(typeof(EarthquakeScroll), 1), new LootPackItem(typeof(EnergyVortexScroll), 1),
            new LootPackItem(typeof(ResurrectionScroll), 1), new LootPackItem(typeof(SummonAirElementalScroll), 1), new LootPackItem(typeof(SummonDaemonScroll), 1),
            new LootPackItem(typeof(SummonEarthElementalScroll), 1), new LootPackItem(typeof(SummonFireElementalScroll), 1), new LootPackItem(typeof(SummonWaterElementalScroll), 1 )
        };

        public static readonly LootPackItem[] NecroScrollItems = new[]
        {
            new LootPackItem(typeof(AnimateDeadScroll), 1),
            new LootPackItem(typeof(BloodOathScroll), 1),
            new LootPackItem(typeof(CorpseSkinScroll), 1),
            new LootPackItem(typeof(CurseWeaponScroll), 1),
            new LootPackItem(typeof(EvilOmenScroll), 1),
            new LootPackItem(typeof(HorrificBeastScroll), 1),
            new LootPackItem(typeof(MindRotScroll), 1),
            new LootPackItem(typeof(PainSpikeScroll), 1),
            new LootPackItem(typeof(SummonFamiliarScroll), 1),
            new LootPackItem(typeof(WraithFormScroll), 1),
            new LootPackItem(typeof(LichFormScroll), 1),
            new LootPackItem(typeof(PoisonStrikeScroll), 1),
            new LootPackItem(typeof(StrangleScroll), 1),
            new LootPackItem(typeof(WitherScroll), 1),
            new LootPackItem(typeof(VengefulSpiritScroll), 1),
            new LootPackItem(typeof(VampiricEmbraceScroll), 1),
            new LootPackItem(typeof(ExorcismScroll), 1)
        };

        public static readonly LootPackItem[] ArcanistScrollItems = new[]
        {
            new LootPackItem(typeof(ArcaneCircleScroll), 1),
            new LootPackItem(typeof(GiftOfRenewalScroll), 1),
            new LootPackItem(typeof(ImmolatingWeaponScroll), 1),
            new LootPackItem(typeof(AttuneWeaponScroll), 1),
            new LootPackItem(typeof(ThunderstormScroll), 1),
            new LootPackItem(typeof(NatureFuryScroll), 1),
            new LootPackItem(typeof(ReaperFormScroll), 1),
            new LootPackItem(typeof(WildfireScroll), 1),
            new LootPackItem(typeof(EssenceOfWindScroll), 1),
            new LootPackItem(typeof(DryadAllureScroll), 1),
            new LootPackItem(typeof(EtherealVoyageScroll), 1),
            new LootPackItem(typeof(WordOfDeathScroll), 1),
            new LootPackItem(typeof(GiftOfLifeScroll), 1),
            new LootPackItem(typeof(ArcaneEmpowermentScroll), 1)
        };

        public static readonly LootPackItem[] MysticScrollItems = new[]
        {
            new LootPackItem(typeof(NetherBoltScroll), 1),
            new LootPackItem(typeof(HealingStoneScroll), 1),
            new LootPackItem(typeof(PurgeMagicScroll), 1),
            new LootPackItem(typeof(EnchantScroll), 1),
            new LootPackItem(typeof(SleepScroll), 1),
            new LootPackItem(typeof(EagleStrikeScroll), 1),
            new LootPackItem(typeof(AnimatedWeaponScroll), 1),
            new LootPackItem(typeof(StoneFormScroll), 1),
            new LootPackItem(typeof(SpellTriggerScroll), 1),
            new LootPackItem(typeof(MassSleepScroll), 1),
            new LootPackItem(typeof(CleansingWindsScroll), 1),
            new LootPackItem(typeof(BombardScroll), 1),
            new LootPackItem(typeof(SpellPlagueScroll), 1),
            new LootPackItem(typeof(HailStormScroll), 1),
            new LootPackItem(typeof(NetherCycloneScroll), 1),
            new LootPackItem(typeof(RisingColossusScroll), 1)
        };


        public static readonly LootPackItem[] GemItems = new[] { new LootPackItem(typeof(Amber), 1) };
        public static readonly LootPackItem[] RareGemItems = new[] { new LootPackItem(typeof(BlueDiamond), 1) };


        public static readonly LootPackItem[] MageryRegItems = new[]
        {
            new LootPackItem(typeof(BlackPearl), 1),
            new LootPackItem(typeof(Bloodmoss), 1),
            new LootPackItem(typeof(Garlic), 1),
            new LootPackItem(typeof(Ginseng), 1),
            new LootPackItem(typeof(MandrakeRoot), 1),
            new LootPackItem(typeof(Nightshade), 1),
            new LootPackItem(typeof(SulfurousAsh), 1),
            new LootPackItem(typeof(SpidersSilk), 1)
        };

        public static readonly LootPackItem[] NecroRegItems = new[]
        {
            new LootPackItem(typeof(BatWing), 1),
            new LootPackItem(typeof(GraveDust), 1),
            new LootPackItem(typeof(DaemonBlood), 1),
            new LootPackItem(typeof(NoxCrystal), 1),
            new LootPackItem(typeof(PigIron), 1)
        };

        public static readonly LootPackItem[] MysticRegItems = new[]
        {
            new LootPackItem(typeof(Bone), 1),
            new LootPackItem(typeof(DragonBlood), 1),
            new LootPackItem(typeof(FertileDirt), 1),
            new LootPackItem(typeof(DaemonBone), 1)
        };

        public static readonly LootPackItem[] PeerlessResourceItems = new[]
{
            new LootPackItem(typeof(Blight), 1),
            new LootPackItem(typeof(Scourge), 1),
            new LootPackItem(typeof(Taint), 1),
            new LootPackItem(typeof(Putrefaction), 1),
            new LootPackItem(typeof(Corruption), 1),
            new LootPackItem(typeof(Muculent), 1)
        };

        public static readonly LootPackItem[] PotionItems = new[]
        {
            new LootPackItem(typeof(AgilityPotion), 1), new LootPackItem(typeof(StrengthPotion), 1),
            new LootPackItem(typeof(RefreshPotion), 1), new LootPackItem(typeof(LesserCurePotion), 1),
            new LootPackItem(typeof(LesserHealPotion), 1), new LootPackItem(typeof(LesserPoisonPotion), 1)
        };

        public static readonly LootPackItem[] LootBodyParts = new[]
        {
            new LootPackItem(typeof(LeftArm), 1), new LootPackItem(typeof(RightArm), 1),
            new LootPackItem(typeof(Torso), 1), new LootPackItem(typeof(RightLeg), 1),
            new LootPackItem(typeof(LeftLeg), 1)
        };

        public static readonly LootPackItem[] LootBones = new[]
        {
            new LootPackItem(typeof(Bone), 1), new LootPackItem(typeof(RibCage), 2),
            new LootPackItem(typeof(BonePile), 3)
        };

        public static readonly LootPackItem[] LootBodyPartsAndBones = new[]
        {
            new LootPackItem(typeof(LeftArm), 1), new LootPackItem(typeof(RightArm), 1),
            new LootPackItem(typeof(Torso), 1), new LootPackItem(typeof(RightLeg), 1),
            new LootPackItem(typeof(LeftLeg), 1), new LootPackItem(typeof(Bone), 1),
            new LootPackItem(typeof(RibCage), 1), new LootPackItem(typeof(BonePile), 1)
        };


        public static readonly LootPackItem[] StatueItems = new[]
        {
            new LootPackItem(typeof(StatueSouth), 1), new LootPackItem(typeof(StatueSouth2), 1),
            new LootPackItem(typeof(StatueNorth), 1), new LootPackItem(typeof(StatueWest), 1),
            new LootPackItem(typeof(StatueEast), 1), new LootPackItem(typeof(StatueEast2), 1),
            new LootPackItem(typeof(StatueSouthEast), 1), new LootPackItem(typeof(BustSouth), 1),
            new LootPackItem(typeof(BustEast), 1)
        };

        #region Magic Items
        public static readonly LootPackItem[] MagicItemsPoor = new[]
        {
            new LootPackItem(typeof(BaseWeapon), 3), new LootPackItem(typeof(BaseRanged), 1),
            new LootPackItem(typeof(BaseArmor), 4), new LootPackItem(typeof(BaseShield), 1),
            new LootPackItem(typeof(BaseJewel), 2)
        };

        public static readonly LootPackItem[] MagicItemsMeagerType1 = new[]
        {
            new LootPackItem(typeof(BaseWeapon), 56), new LootPackItem(typeof(BaseRanged), 14),
            new LootPackItem(typeof(BaseArmor), 81), new LootPackItem(typeof(BaseShield), 11),
            new LootPackItem(typeof(BaseJewel), 42)
        };

        public static readonly LootPackItem[] MagicItemsMeagerType2 = new[]
        {
            new LootPackItem(typeof(BaseWeapon), 28), new LootPackItem(typeof(BaseRanged), 7),
            new LootPackItem(typeof(BaseArmor), 40), new LootPackItem(typeof(BaseShield), 5),
            new LootPackItem(typeof(BaseJewel), 21)
        };

        public static readonly LootPackItem[] MagicItemsAverageType1 = new[]
        {
            new LootPackItem(typeof(BaseWeapon), 90), new LootPackItem(typeof(BaseRanged), 23),
            new LootPackItem(typeof(BaseArmor), 130), new LootPackItem(typeof(BaseShield), 17),
            new LootPackItem(typeof(BaseJewel), 68)
        };

        public static readonly LootPackItem[] MagicItemsAverageType2 = new[]
        {
            new LootPackItem(typeof(BaseWeapon), 54), new LootPackItem(typeof(BaseRanged), 13),
            new LootPackItem(typeof(BaseArmor), 77), new LootPackItem(typeof(BaseShield), 10),
            new LootPackItem(typeof(BaseJewel), 40)
        };

        public static readonly LootPackItem[] MagicItemsRichType1 = new[]
        {
            new LootPackItem(typeof(BaseWeapon), 211), new LootPackItem(typeof(BaseRanged), 53),
            new LootPackItem(typeof(BaseArmor), 303), new LootPackItem(typeof(BaseShield), 39),
            new LootPackItem(typeof(BaseJewel), 158)
        };

        public static readonly LootPackItem[] MagicItemsRichType2 = new[]
        {
            new LootPackItem(typeof(BaseWeapon), 170), new LootPackItem(typeof(BaseRanged), 43),
            new LootPackItem(typeof(BaseArmor), 245), new LootPackItem(typeof(BaseShield), 32),
            new LootPackItem(typeof(BaseJewel), 128)
        };

        public static readonly LootPackItem[] MagicItemsFilthyRichType1 = new[]
        {
            new LootPackItem(typeof(BaseWeapon), 219), new LootPackItem(typeof(BaseRanged), 55),
            new LootPackItem(typeof(BaseArmor), 315), new LootPackItem(typeof(BaseShield), 41),
            new LootPackItem(typeof(BaseJewel), 164)
        };

        public static readonly LootPackItem[] MagicItemsFilthyRichType2 = new[]
        {
            new LootPackItem(typeof(BaseWeapon), 239), new LootPackItem(typeof(BaseRanged), 60),
            new LootPackItem(typeof(BaseArmor), 343), new LootPackItem(typeof(BaseShield), 90),
            new LootPackItem(typeof(BaseJewel), 45)
        };

        public static readonly LootPackItem[] MagicItemsUltraRich = new[]
        {
            new LootPackItem(typeof(BaseWeapon), 276), new LootPackItem(typeof(BaseRanged), 69),
            new LootPackItem(typeof(BaseArmor), 397), new LootPackItem(typeof(BaseShield), 52),
            new LootPackItem(typeof(BaseJewel), 207)
        };
        #endregion

        #region Level definitions
        public static readonly LootPack LootPoor =
            new LootPack(
                new[]
                {
                    new LootPackEntry(false, true, Gold, 100.00, "2d10+20"),
                    new LootPackEntry(false, false, MagicItemsPoor, 1.00, 1, 5, 0, 100, true),
                    new LootPackEntry(false, false, Instruments, 0.02, 1, true)
                });

        public static readonly LootPack LootMeager =
            new LootPack(
                new[]
                {
                    new LootPackEntry(false, true, Gold, 100.00, "4d10+40"),
                    new LootPackEntry(false, false, MagicItemsMeagerType1, 20.40, 1, 2, 0, 50, true),
                    new LootPackEntry(false, false, MagicItemsMeagerType2, 10.20, 1, 5, 0, 100, true),
                    new LootPackEntry(false, false, Instruments, 0.10, 1)
                });

        public static readonly LootPack LootAverage =
            new LootPack(
                new[]
                {
                    new LootPackEntry(false, true, Gold, 100.00, "8d10+100"),
                    new LootPackEntry(false, false, MagicItemsAverageType1, 32.80, 1, 3, 0, 50, true),
                    new LootPackEntry(false, false, MagicItemsAverageType1, 32.80, 1, 4, 0, 75, true),
                    new LootPackEntry(false, false, MagicItemsAverageType2, 19.50, 1, 5, 0, 100, true),
                    new LootPackEntry(false, false, Instruments, 0.40, 1)
                });

        public static readonly LootPack LootRich =
            new LootPack(
                new[]
                {
                    new LootPackEntry(false, true, Gold, 100.00, "15d10+225"),
                    new LootPackEntry(false, false, MagicItemsRichType1, 76.30, 1, 4, 0, 75, true),
                    new LootPackEntry(false, false, MagicItemsRichType1, 76.30, 1, 4, 0, 75, true),
                    new LootPackEntry(false, false, MagicItemsRichType2, 61.70, 1, 5, 0, 100, true),
                    new LootPackEntry(false, false, Instruments, 1.00, 1)
                });

        public static readonly LootPack LootFilthyRich =
            new LootPack(
                new[]
                {
                    new LootPackEntry(false, true, Gold, 100.00, "3d100+400"),
                    new LootPackEntry(false, false, MagicItemsFilthyRichType1, 79.50, 1, 5, 0, 100, true),
                    new LootPackEntry(false, false, MagicItemsFilthyRichType1, 79.50, 1, 5, 0, 100, true),
                    new LootPackEntry(false, false, MagicItemsFilthyRichType2, 77.60, 1, 5, 25, 100, true),
                    new LootPackEntry(false, false, Instruments, 2.00, 1)
                });

        public static readonly LootPack LootUltraRich =
            new LootPack(
                new[]
                {
                    new LootPackEntry(false, true, Gold, 100.00, "6d100+600"),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 25, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 25, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 25, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 25, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 25, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 33, 100, true),
                    new LootPackEntry(false, false, Instruments, 2.00, 1)
                });

        public static readonly LootPack LootSuperBoss =
            new LootPack(
                new[]
                {
                    new LootPackEntry(false, true, Gold, 100.00, "10d100+800"),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 25, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 25, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 25, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 25, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 33, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 33, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 33, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 33, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 50, 100, true),
                    new LootPackEntry(false, false, MagicItemsUltraRich, 100.00, 1, 5, 50, 100, true),
                    new LootPackEntry(false, false, Instruments, 2.00, 1)
                });
        #endregion

        #region Generic accessors
        public static LootPack Poor => LootPoor;
        public static LootPack Meager => LootMeager;
        public static LootPack Average => LootAverage;
        public static LootPack Rich => LootRich;
        public static LootPack FilthyRich => LootFilthyRich;
        public static LootPack UltraRich => LootUltraRich;
        public static LootPack SuperBoss => LootSuperBoss;
        #endregion

        public static readonly LootPack LowScrolls = new LootPack(new[] { new LootPackEntry(false, true, LowScrollItems, 100.00, 1) });
        public static readonly LootPack MedScrolls = new LootPack(new[] { new LootPackEntry(false, true, MedScrollItems, 100.00, 1) });
        public static readonly LootPack HighScrolls = new LootPack(new[] { new LootPackEntry(false, true, HighScrollItems, 100.00, 1) });
        public static readonly LootPack MageryScrolls = new LootPack(new[] { new LootPackEntry(false, true, MageryScrollItems, 100.00, 1) });
        public static readonly LootPack NecroScrolls = new LootPack(new[] { new LootPackEntry(false, true, NecroScrollItems, 100.00, 1) });
        public static readonly LootPack ArcanistScrolls = new LootPack(new[] { new LootPackEntry(false, true, ArcanistScrollItems, 100.00, 1) });
        public static readonly LootPack MysticScrolls = new LootPack(new[] { new LootPackEntry(false, true, MysticScrollItems, 100.00, 1) });

        public static readonly LootPack MageryRegs = new LootPack(new[] { new LootPackEntry(false, true, MageryRegItems, 100.00, 1) });
        public static readonly LootPack NecroRegs = new LootPack(new[] { new LootPackEntry(false, true, NecroRegItems, 100.00, 1) });
        public static readonly LootPack MysticRegs = new LootPack(new[] { new LootPackEntry(false, true, MysticRegItems, 100.00, 1) });
        public static readonly LootPack PeerlessResource = new LootPack(new[] { new LootPackEntry(false, true, PeerlessResourceItems, 100.00, 1) });

        public static readonly LootPack Gems = new LootPack(new[] { new LootPackEntry(false, true, GemItems, 100.00, 1) });
        public static readonly LootPack RareGems = new LootPack(new[] { new LootPackEntry(false, true, RareGemItems, 100.00, 1) });

        public static readonly LootPack Potions = new LootPack(new[] { new LootPackEntry(false, true, PotionItems, 100.00, 1) });
        public static readonly LootPack BodyParts = new LootPack(new[] { new LootPackEntry(false, true, LootBodyParts, 100.00, 1) });
        public static readonly LootPack Bones = new LootPack(new[] { new LootPackEntry(false, true, LootBones, 100.00, 1) });
        public static readonly LootPack BodyPartsAndBones = new LootPack(new[] { new LootPackEntry(false, true, LootBodyPartsAndBones, 100.00, 1) });
        public static readonly LootPack Statue = new LootPack(new[] { new LootPackEntry(false, true, StatueItems, 100.00, 1) });

        public static readonly LootPack Parrot = new LootPack(new[] { new LootPackEntry(false, false, new LootPackItem[] { new LootPackItem(typeof(ParrotItem), 1) }, 10.00, 1) });
        public static readonly LootPack Talisman = new LootPack(new[] { new LootPackEntry(false, false, new LootPackItem[] { new LootPackItem(typeof(RandomTalisman), 1) }, 100.00, 1) });

        public static readonly LootPack PeculiarSeed1 = new LootPack(new[] { new LootPackEntry(false, true, new LootPackItem[] { new LootPackItem(e => Engines.Plants.Seed.RandomPeculiarSeed(1), 1) }, 33.3, 1) });
        public static readonly LootPack PeculiarSeed2 = new LootPack(new[] { new LootPackEntry(false, true, new LootPackItem[] { new LootPackItem(e => Engines.Plants.Seed.RandomPeculiarSeed(2), 1) }, 33.3, 1) });
        public static readonly LootPack PeculiarSeed3 = new LootPack(new[] { new LootPackEntry(false, true, new LootPackItem[] { new LootPackItem(e => Engines.Plants.Seed.RandomPeculiarSeed(3), 1)}, 33.3, 1) });
        public static readonly LootPack PeculiarSeed4 = new LootPack(new[] { new LootPackEntry(false, true, new LootPackItem[] { new LootPackItem(e => Engines.Plants.Seed.RandomPeculiarSeed(4), 1) }, 33.3, 1) });
        public static readonly LootPack BonsaiSeed = new LootPack(new[] { new LootPackEntry(false, true, new LootPackItem[] { new LootPackItem(e => Engines.Plants.Seed.RandomBonsaiSeed(), 1) }, 25.0, 1) });

        public static LootPack LootItems(LootPackItem[] items)
        {
            return new LootPack(new[] { new LootPackEntry(false, false, items, 100.0, 1) });
        }

        public static LootPack LootItems(LootPackItem[] items, int amount)
        {
            return new LootPack(new[] { new LootPackEntry(false, false, items, 100.0, amount) });
        }

        public static LootPack LootItems(LootPackItem[] items, double chance)
        {
            return new LootPack(new[] { new LootPackEntry(false, false, items, chance, 1) });
        }

        public static LootPack LootItems(LootPackItem[] items, double chance, int amount)
        {
            return new LootPack(new[] { new LootPackEntry(false, false, items, chance, amount) });
        }

        public static LootPack LootItems(LootPackItem[] items, double chance, int amount, bool resource)
        {
            return new LootPack(new[] { new LootPackEntry(false, resource, items, chance, amount) });
        }

        public static LootPack LootItems(LootPackItem[] items, double chance, int amount, bool spawn, bool steal)
        {
            return new LootPack(new[] { new LootPackEntry(spawn, steal, items, chance, amount) });
        }

        public static LootPack LootItem<T>() where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(false, false, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, 100.0, 1) });
        }

        public static LootPack LootItem<T>(bool resource) where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(false, resource, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, 100.0, 1) });
        }

        public static LootPack LootItem<T>(double chance) where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(false, false, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, chance, 1) });
        }

        public static LootPack LootItem<T>(double chance, bool resource) where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(false, resource, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, chance, 1) });
        }

        public static LootPack LootItem<T>(bool onSpawn, bool onSteal) where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(onSpawn, onSteal, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, 100.0, 1) });
        }

        public static LootPack LootItem<T>(int amount) where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(false, false, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, 100.0, amount) });
        }

        public static LootPack LootItem<T>(int min, int max) where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(false, false, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, 100.0, Utility.RandomMinMax(min, max)) });
        }

        public static LootPack LootItem<T>(int min, int max, bool resource) where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(false, resource, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, 100.0, Utility.RandomMinMax(min, max)) });
        }

        public static LootPack LootItem<T>(int min, int max, bool spawnTime, bool onSteal) where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(spawnTime, onSteal, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, 100.0, Utility.RandomMinMax(min, max)) });
        }

        public static LootPack LootItem<T>(int amount, bool resource) where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(false, resource, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, 100.0, amount) });
        }

        public static LootPack LootItem<T>(double chance, int amount) where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(false, false, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, chance, amount) });
        }

        public static LootPack LootItem<T>(double chance, int amount, bool spawnTime, bool onSteal) where T : Item
        {
            return new LootPack(new[] { new LootPackEntry(spawnTime, onSteal, new LootPackItem[] { new LootPackItem(typeof(T), 1) }, chance, amount) });
        }

        public static LootPack RandomLootItem(Type[] types)
        {
            return RandomLootItem(types, 100.0, 1, false, false);
        }

        public static LootPack RandomLootItem(Type[] types, bool onSpawn, bool onSteal)
        {
            return RandomLootItem(types, 100.0, 1, onSpawn, onSteal);
        }

        public static LootPack RandomLootItem(Type[] types, double chance, int amount)
        {
            return RandomLootItem(types, chance, amount, false, false);
        }

        public static LootPack RandomLootItem(Type[] types, double chance, int amount, bool onSpawn, bool onSteal)
        {
            var items = new LootPackItem[types.Length];

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new LootPackItem(types[i], 1);
            }

            return new LootPack(new[] { new LootPackEntry(onSpawn, onSteal, items, chance, amount) });
        }

        public static LootPack LootItemCallback(Func<IEntity, Item> callback)
        {
            return new LootPack(new[] { new LootPackEntry(false, false, new LootPackItem[] { new LootPackItem(callback, 1) }, 100.0, 1) });
        }

        public static LootPack LootItemCallback(Func<IEntity, Item> callback, double chance, int amount, bool onSpawn, bool onSteal)
        {
            return new LootPack(new[] { new LootPackEntry(onSpawn, onSteal, new LootPackItem[] { new LootPackItem(callback, 1) }, chance, amount) });
        }

        public static LootPack LootGold(int amount)
        {
            return new LootPack(new[] { new LootPackEntry(false, true, new LootPackItem[] { new LootPackItem(typeof(Gold), 1) }, 100.0, amount) });
        }

        public static LootPack LootGold(int min, int max)
        {
            if (min > max)
                min = max;

            if (min > 0)
            {
                return LootGold(Utility.RandomMinMax(min, max));
            }

            return null;
        }
    }

    public class LootPackEntry
    {
        public int Chance { get; set; }

        public LootPackDice Quantity { get; set; }

        public bool AtSpawnTime { get; set; }
        public bool OnStolen { get; set; }
        public int MaxProps { get; set; }
        public int MinIntensity { get; set; }
        public int MaxIntensity { get; set; }

        public LootPackItem[] Items { get; set; }

        public bool StandardLootItem { get; set; }

        public static bool IsInTokuno(IEntity e)
        {
            if (e == null)
            {
                return false;
            }

            Region r = Region.Find(e.Location, e.Map);

            if (r.IsPartOf("Fan Dancer's Dojo"))
            {
                return true;
            }

            if (r.IsPartOf("Yomotsu Mines"))
            {
                return true;
            }

            return e.Map == Map.Tokuno;
        }

        public static bool IsMondain(IEntity e)
        {
            if (e == null)
                return false;

            return MondainsLegacy.IsMLRegion(Region.Find(e.Location, e.Map));
        }

        public static bool IsStygian(IEntity e)
        {
            if (e == null)
                return false;

            return e.Map == Map.TerMur || (!IsInTokuno(e) && !IsMondain(e) && Utility.RandomBool());
        }

        public bool CanGenerate(LootStage stage, bool hasBeenStolenFrom)
        {
            switch (stage)
            {
                case LootStage.Spawning:
                    if (!AtSpawnTime)
                        return false;
                    break;
                case LootStage.Stolen:
                    if (!OnStolen)
                        return false;
                    break;
                case LootStage.Death:
                    if (OnStolen && hasBeenStolenFrom)
                        return false;
                    break;
            }

            return true;
        }

        public Item Construct(IEntity from, int luckChance, LootStage stage, bool hasBeenStolenFrom)
        {
            int totalChance = 0;

            for (int i = 0; i < Items.Length; ++i)
            {
                totalChance += Items[i].Chance;
            }

            int rnd = Utility.Random(totalChance);

            for (int i = 0; i < Items.Length; ++i)
            {
                LootPackItem item = Items[i];

                if (rnd < item.Chance)
                {
                    Item loot = null;

                    if(item.ConstructCallback != null)
                    {
                        loot = item.ConstructCallback(from);
                    }
                    else
                    {
                        loot = item.Construct(IsInTokuno(from), IsMondain(from), IsStygian(from));
                    }

                    if (loot != null)
                    {
                        return Mutate(from, luckChance, loot);
                    }
                }

                rnd -= item.Chance;
            }

            return null;
        }

        public Item Mutate(IEntity from, int luckChance, Item item)
        {
            if (item != null)
            {
                if (item is BaseWeapon && 1 > Utility.Random(100))
                {
                    item.Delete();
                    item = new FireHorn();
                    return item;
                }

                if (StandardLootItem && (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat))
                {
                    // Try to generate a new random item based on the creature killed
                    if (RandomItemGenerator.Enabled && from is BaseCreature)
                    {
                        if (RandomItemGenerator.GenerateRandomItem(item, ((BaseCreature)from).LastKiller, (BaseCreature)from))
                        {
                            return item;
                        }
                    }

                    int bonusProps = GetBonusProperties();

                    if (bonusProps < MaxProps && LootPack.CheckLuck(luckChance))
                    {
                        ++bonusProps;
                    }

                    int props = 1 + bonusProps;

                    // Make sure we're not spawning items with 6 properties.
                    if (props > MaxProps)
                    {
                        props = MaxProps;
                    }

                    // Use the older style random generation
                    if (item is BaseWeapon)
                    {
                        BaseRunicTool.ApplyAttributesTo((BaseWeapon)item, false, luckChance, props, MinIntensity, MaxIntensity);
                    }
                    else if (item is BaseArmor)
                    {
                        BaseRunicTool.ApplyAttributesTo((BaseArmor)item, false, luckChance, props, MinIntensity, MaxIntensity);
                    }
                    else if (item is BaseJewel)
                    {
                        BaseRunicTool.ApplyAttributesTo((BaseJewel)item, false, luckChance, props, MinIntensity, MaxIntensity);
                    }
                    else if (item is BaseHat)
                    {
                        BaseRunicTool.ApplyAttributesTo((BaseHat)item, false, luckChance, props, MinIntensity, MaxIntensity);
                    }
                }
                else if (item is BaseInstrument)
                {
                    SlayerName slayer = SlayerName.None;

                    slayer = BaseRunicTool.GetRandomSlayer();

                    if (slayer == SlayerName.None)
                    {
                        item.Delete();
                        return null;
                    }

                    BaseInstrument instr = (BaseInstrument)item;

                    instr.Quality = ItemQuality.Normal;
                    instr.Slayer = slayer;
                }

                if (item.Stackable)
                {
                    item.Amount = Quantity.Roll();
                }
            }

            return item;
        }

        public LootPackEntry(bool atSpawnTime, bool onStolen, LootPackItem[] items, double chance, string quantity)
            : this(atSpawnTime, onStolen, items, chance, new LootPackDice(quantity), 0, 0, 0, false)
        { }

        public LootPackEntry(bool atSpawnTime, bool onStolen, LootPackItem[] items, double chance, string quantity, bool standardLoot)
            : this(atSpawnTime, onStolen, items, chance, new LootPackDice(quantity), 0, 0, 0, standardLoot)
        { }

        public LootPackEntry(bool atSpawnTime, bool onStolen, LootPackItem[] items, double chance, int quantity)
            : this(atSpawnTime, onStolen, items, chance, new LootPackDice(0, 0, quantity), 0, 0, 0, false)
        { }

        public LootPackEntry(bool atSpawnTime, bool onStolen, LootPackItem[] items, double chance, int quantity, bool standardLoot)
            : this(atSpawnTime, onStolen, items, chance, new LootPackDice(0, 0, quantity), 0, 0, 0, standardLoot)
        { }

        public LootPackEntry(
            bool atSpawnTime,
            bool onStolen,
            LootPackItem[] items,
            double chance,
            string quantity,
            int maxProps,
            int minIntensity,
            int maxIntensity)
            : this(atSpawnTime, onStolen, items, chance, new LootPackDice(quantity), maxProps, minIntensity, maxIntensity, false)
        { }

        public LootPackEntry(
            bool atSpawnTime,
            bool onStolen,
            LootPackItem[] items,
            double chance,
            string quantity,
            int maxProps,
            int minIntensity,
            int maxIntensity,
            bool standardLoot)
            : this(atSpawnTime, onStolen, items, chance, new LootPackDice(quantity), maxProps, minIntensity, maxIntensity, standardLoot)
                { }

        public LootPackEntry( bool atSpawnTime, bool onStolen, LootPackItem[] items, double chance, int quantity, int maxProps, int minIntensity, int maxIntensity)
            : this(atSpawnTime, onStolen, items, chance, new LootPackDice(0, 0, quantity), maxProps, minIntensity, maxIntensity, false)
        { }

        public LootPackEntry(bool atSpawnTime, bool onStolen, LootPackItem[] items, double chance, int quantity, int maxProps, int minIntensity, int maxIntensity, bool standardLoot)
            : this(atSpawnTime, onStolen, items, chance, new LootPackDice(0, 0, quantity), maxProps, minIntensity, maxIntensity, standardLoot)
                { }

        public LootPackEntry(
            bool atSpawnTime,
            bool onStolen,
            LootPackItem[] items,
            double chance,
            LootPackDice quantity,
            int maxProps,
            int minIntensity,
            int maxIntensity,
            bool standardLootItem)
        {
            AtSpawnTime = atSpawnTime;
            OnStolen = onStolen;
            Items = items;
            Chance = (int)(100 * chance);
            Quantity = quantity;
            MaxProps = maxProps;
            MinIntensity = minIntensity;
            MaxIntensity = maxIntensity;
            StandardLootItem = standardLootItem;
        }

        public int GetBonusProperties()
        {
            int p0 = 0, p1 = 0, p2 = 0, p3 = 0, p4 = 0, p5 = 0;

            switch (MaxProps)
            {
                case 1:
                    p0 = 3;
                    p1 = 1;
                    break;
                case 2:
                    p0 = 6;
                    p1 = 3;
                    p2 = 1;
                    break;
                case 3:
                    p0 = 10;
                    p1 = 6;
                    p2 = 3;
                    p3 = 1;
                    break;
                case 4:
                    p0 = 16;
                    p1 = 12;
                    p2 = 6;
                    p3 = 5;
                    p4 = 1;
                    break;
                case 5:
                    p0 = 30;
                    p1 = 25;
                    p2 = 20;
                    p3 = 15;
                    p4 = 9;
                    p5 = 1;
                    break;
            }

            int pc = p0 + p1 + p2 + p3 + p4 + p5;

            int rnd = Utility.Random(pc);

            if (rnd < p5)
            {
                return 5;
            }
            else
            {
                rnd -= p5;
            }

            if (rnd < p4)
            {
                return 4;
            }
            else
            {
                rnd -= p4;
            }

            if (rnd < p3)
            {
                return 3;
            }
            else
            {
                rnd -= p3;
            }

            if (rnd < p2)
            {
                return 2;
            }
            else
            {
                rnd -= p2;
            }

            if (rnd < p1)
            {
                return 1;
            }

            return 0;
        }
    }

    public class LootPackItem
    {
        public Type Type { get; set; }
        public int Chance { get; set; }

        public Func<IEntity, Item> ConstructCallback { get; set; }

        public Item Construct(bool inTokuno, bool isMondain, bool isStygian)
        {
            try
            {
                Item item;

                if (Type == typeof(BaseRanged))
                {
                    item = Loot.RandomRangedWeapon(inTokuno, isMondain, isStygian);
                }
                else if (Type == typeof(BaseWeapon))
                {
                    item = Loot.RandomWeapon(inTokuno, isMondain, isStygian);
                }
                else if (Type == typeof(BaseArmor))
                {
                    item = Loot.RandomArmorOrHat(inTokuno, isMondain, isStygian);
                }
                else if (Type == typeof(BaseShield))
                {
                    item = Loot.RandomShield(isStygian);
                }
                else if (Type == typeof(BaseJewel))
                {
                    item = Loot.RandomJewelry(isStygian);
                }
                else if (Type == typeof(BaseInstrument))
                {
                    item = Loot.RandomInstrument();
                }
                else if (Type == typeof(Amber)) // gem
                {
                    item = Loot.RandomGem();
                }
                else if (Type == typeof(BlueDiamond)) // rare gem
                {
                    item = Loot.RandomRareGem();
                }
                else
                {
                    item = Activator.CreateInstance(Type) as Item;
                }

                return item;
            }
            catch (Exception e)
            {
                Server.Diagnostics.ExceptionLogging.LogException(e);
            }

            return null;
        }

        public LootPackItem(Func<IEntity, Item> callback, int chance)
        {
            ConstructCallback = callback;
            Chance = chance;
        }

        public LootPackItem(Type type, int chance)
        {
            Type = type;
            Chance = chance;
        }
    }

    public class LootPackDice
    {
        private int m_Count, m_Sides, m_Bonus;

        public int Count { get { return m_Count; } set { m_Count = value; } }

        public int Sides { get { return m_Sides; } set { m_Sides = value; } }

        public int Bonus { get { return m_Bonus; } set { m_Bonus = value; } }

        public int Roll()
        {
            int v = m_Bonus;

            for (int i = 0; i < m_Count; ++i)
            {
                v += Utility.Random(1, m_Sides);
            }

            return v;
        }

        public LootPackDice(string str)
        {
            int start = 0;
            int index = str.IndexOf('d', start);

            if (index < start)
            {
                return;
            }

            m_Count = Utility.ToInt32(str.Substring(start, index - start));

            bool negative;

            start = index + 1;
            index = str.IndexOf('+', start);

            if (negative = (index < start))
            {
                index = str.IndexOf('-', start);
            }

            if (index < start)
            {
                index = str.Length;
            }

            m_Sides = Utility.ToInt32(str.Substring(start, index - start));

            if (index == str.Length)
            {
                return;
            }

            start = index + 1;
            index = str.Length;

            m_Bonus = Utility.ToInt32(str.Substring(start, index - start));

            if (negative)
            {
                m_Bonus *= -1;
            }
        }

        public LootPackDice(int count, int sides, int bonus)
        {
            m_Count = count;
            m_Sides = sides;
            m_Bonus = bonus;
        }
    }
}
