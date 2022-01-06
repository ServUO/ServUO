using System.Collections.Generic;

namespace Server.Services.ContainerSort
{
	public class SortItemGroups
	{
		public static Dictionary<ItemSortType, List<ItemSortType>> Groups = new Dictionary<ItemSortType, List<ItemSortType>>
		{
			{
				ItemSortType.AllOre,
				new List<ItemSortType> { ItemSortType.IronOre, ItemSortType.DullCopperOre, ItemSortType.ShadowIronOre,
										 ItemSortType.CopperOre, ItemSortType.BronzeOre, ItemSortType.GoldOre,
										 ItemSortType.AgapiteOre, ItemSortType.VeriteOre, ItemSortType.ValoriteOre
									   }
			},
			{
				ItemSortType.AllIgnots,
				new List<ItemSortType> { ItemSortType.IronIngot, ItemSortType.DullCopperIngot, ItemSortType.ShadowIronIngot,
										 ItemSortType.CopperIngot, ItemSortType.BronzeIngot, ItemSortType.GoldIngot,
										 ItemSortType.AgapiteIngot, ItemSortType.VeriteIngot, ItemSortType.ValoriteIngot
									   }
			},
			{
				ItemSortType.AllGems,
				new List<ItemSortType> { ItemSortType.Amber, ItemSortType.Citrine, ItemSortType.Ruby,
										 ItemSortType.Tourmaline, ItemSortType.Amethyst, ItemSortType.Emerald,
										 ItemSortType.Sapphire, ItemSortType.StarSapphire, ItemSortType.Diamond,
										 ItemSortType.PrismaticAmber, ItemSortType.BrilliantAmber, ItemSortType.BlueDiamond,
										 ItemSortType.DarkSapphire, ItemSortType.EcruCitrine, ItemSortType.FireRuby,
										 ItemSortType.PerfectEmerald, ItemSortType.Turquoise, ItemSortType.WhitePearl,
										 ItemSortType.ArcaneGem
									   }
			},
			{
				ItemSortType.AllMageryReagents,
				new List<ItemSortType> { ItemSortType.BlackPearl, ItemSortType.BloodMoss, ItemSortType.Garlic,
										 ItemSortType.Ginseng, ItemSortType.MandrakeRoot, ItemSortType.Nightshade,
										 ItemSortType.SpidersSilk, ItemSortType.SulfurousAsh
									   }
			},
			{
				ItemSortType.AllNecromancyReagents,
				new List<ItemSortType> { ItemSortType.GraveDust, ItemSortType.NoxCrystal, ItemSortType.DaemonBlood,
										 ItemSortType.Batwing, ItemSortType.PigIron
									   }
			},
			{
				ItemSortType.AllMysticismReagents,
				new List<ItemSortType> { ItemSortType.Bone, ItemSortType.DragonsBlood, ItemSortType.DaemonBone,
										 ItemSortType.FertileDirt
									   }
			},
			{
				ItemSortType.AllWeapons,
				new List<ItemSortType> { ItemSortType.WeaponSkillSwords, ItemSortType.WeaponSkillMaces, ItemSortType.WeaponSkillFencing,
										 ItemSortType.WeaponSkillRanged, ItemSortType.WeaponSkillThrowing
									   }
			},
			{
				ItemSortType.AllArmorMaterials,
				new List<ItemSortType> { ItemSortType.ClothArmor, ItemSortType.LeatherArmor, ItemSortType.StuddedArmor,
										 ItemSortType.BoneArmor, ItemSortType.SpinedArmor, ItemSortType.HornedArmor,
										 ItemSortType.BarbedArmor, ItemSortType.RingmailArmor, ItemSortType.ChainmailArmor,
										 ItemSortType.PlateArmor, ItemSortType.DragonArmor, ItemSortType.WoodArmor,
										 ItemSortType.StoneArmor
									   }
			},
			{
				ItemSortType.AllArmorSlots,
				new List<ItemSortType> { ItemSortType.EaringArmorPosition, ItemSortType.FingerArmorPosition, ItemSortType.WristArmorPosition,
										 ItemSortType.NeckArmorPosition, ItemSortType.HeadArmorPosition, ItemSortType.RobeArmorPosition,
										 ItemSortType.ChestArmorPosition, ItemSortType.ArmsArmorPosition, ItemSortType.HandsArmorPosition,
										 ItemSortType.LegsArmorPosition, ItemSortType.FeetArmorPosition, ItemSortType.BackArmorPosition,
										 ItemSortType.ShieldArmorPosition
									   }
			},
			{
				ItemSortType.AllWoodTypes,
				new List<ItemSortType> { ItemSortType.RegularWood, ItemSortType.OakWood, ItemSortType.AshWood,
										 ItemSortType.YewWood, ItemSortType.Heartwood, ItemSortType.Bloodwood,
										 ItemSortType.Frostwood
									   }
			},
			{
				ItemSortType.AllBoardTypes,
				new List<ItemSortType> { ItemSortType.RegularBoard, ItemSortType.OakBoard, ItemSortType.AshBoard,
										 ItemSortType.YewBoard, ItemSortType.HeartwoodBoard, ItemSortType.BloodwoodBoard,
										 ItemSortType.FrostwoodBoard,
									   }
			},
			{
				ItemSortType.AllImbuingIngredients,
				new List<ItemSortType> { ItemSortType.ArcanicRuneStone, ItemSortType.BottleOfIchor, ItemSortType.BouraPelt,
										 ItemSortType.ChagaMushroom, ItemSortType.CrushedGlass, ItemSortType.CrystallineBlackrock,
										 ItemSortType.CrystalShards, ItemSortType.DaemonClaw, ItemSortType.DelicateScales,
										 ItemSortType.ElvenFletching, ItemSortType.EnchantedEssence, ItemSortType.FaeryDust,
										 ItemSortType.FeyWings, ItemSortType.GoblinBlood, ItemSortType.LavaSerpentCrust,
										 ItemSortType.LuminescentFungi, ItemSortType.MagicalResidue, ItemSortType.ParasiticPlant,
										 ItemSortType.PowderedIron, ItemSortType.RaptorTeeth, ItemSortType.ReflectiveWolfEye,
										 ItemSortType.RelicFragment, ItemSortType.SeedOfRenewal, ItemSortType.SilverSnakeSkin,
										 ItemSortType.SlithTongue, ItemSortType.SpiderCarapace, ItemSortType.UndyingFlesh,
										 ItemSortType.VialOfVitriol, ItemSortType.VoidOrb, ItemSortType.EssenceOfAchievement,
										 ItemSortType.EssenceOfBalance, ItemSortType.EssenceOfControl, ItemSortType.EssenceOfDiligence,
										 ItemSortType.EssenceOfDirection, ItemSortType.EssenceOfFeeling, ItemSortType.EssenceOfOrder,
										 ItemSortType.EssenceOfPassion, ItemSortType.EssenceOfPrecision, ItemSortType.EssenceOfSingularity
									   }
			},
			{
				ItemSortType.AllHides,
				new List<ItemSortType> { ItemSortType.LeatherHide, ItemSortType.BarbedLeatherHide, ItemSortType.HornedLeatherHide,
										 ItemSortType.SpinedLeatherHide
									   }
			},
			{
				ItemSortType.AllCutLeather,
				new List<ItemSortType> { ItemSortType.CutLeather, ItemSortType.CutBarbedLeather, ItemSortType.CutHornedLeather,
										 ItemSortType.CutSpinedLeather
									   }
			},
			{
				ItemSortType.AllScales,
				new List<ItemSortType> { ItemSortType.BlackDragonScale, ItemSortType.GreenDragonScale, ItemSortType.RedDragonScale,
										 ItemSortType.WhiteDragonScale, ItemSortType.YellowDragonScale, ItemSortType.SeaSerpentScale
									   }
			},
			{
				ItemSortType.AllHealingPotions,
				new List<ItemSortType> { ItemSortType.Refresh, ItemSortType.GreaterRefreshment, ItemSortType.LesserHeal,
										 ItemSortType.Heal, ItemSortType.GreaterHeal, ItemSortType.LesserCure,
										 ItemSortType.Cure, ItemSortType.GreaterCure, ItemSortType.ElixirOfRebirth,
										 ItemSortType.BarrabHemolymphConcentrate
									   }
			},
			{
				ItemSortType.AllEnchancingPotions,
				new List<ItemSortType> { ItemSortType.Agility, ItemSortType.GreaterAgility, ItemSortType.NightSight,
										 ItemSortType.Strength, ItemSortType.GreaterStrength, ItemSortType.Invisiblity,
										 ItemSortType.JukariBurnPoultice, ItemSortType.KurakAmbushersEssence, ItemSortType.BarakoDraftOfMight,
										 ItemSortType.UraliTranceTonic, ItemSortType.SakkhraProphylaxis
									   }
			},
			{
				ItemSortType.AllToxicPotions,
				new List<ItemSortType> { ItemSortType.LesserPoison, ItemSortType.Poison, ItemSortType.GreaterPoison,
										 ItemSortType.DeadlyPoison, ItemSortType.Parasitic, ItemSortType.Darkglow,
										 ItemSortType.ScouringToxin
									   }
			},
			{
				ItemSortType.AllExplosives,
				new List<ItemSortType> { ItemSortType.LesserExplosion, ItemSortType.Explosion, ItemSortType.GreaterExplosion,
										 ItemSortType.Conflagration, ItemSortType.GreaterConflageration, ItemSortType.ConfusionBlast,
										 ItemSortType.GreaterConfusionBlast, ItemSortType.BlackPower, ItemSortType.FuseCord
									   }
			},
			{
				ItemSortType.AllAlchemyIngredients,
				new List<ItemSortType> { ItemSortType.PlantPigment, ItemSortType.ColorFixative, ItemSortType.CrystalGranules,
										 ItemSortType.CrystalDust, ItemSortType.SoftenedReeds, ItemSortType.Potash,
										 ItemSortType.GoldDust, ItemSortType.PlantClippings, ItemSortType.MyrmidexEggsac,
										 ItemSortType.PerfectBanana, ItemSortType.Charcoal, ItemSortType.Saltpeter,
										 ItemSortType.SilverSerpentVenom, ItemSortType.BrokenCrystal, ItemSortType.ShimmeringCrystal,
										 ItemSortType.MedusaBlood, ItemSortType.CapturedEssence, ItemSortType.LavaBerry,
										 ItemSortType.TigerPelt, ItemSortType.BlueCorn, ItemSortType.ToxicVenomSac,
										 ItemSortType.RiverMoss
									   }
			},
			{
				ItemSortType.AllFirstCircleScrolls,
				new List<ItemSortType> { ItemSortType.ClumsyScroll, ItemSortType.CreateFoodScroll, ItemSortType.FeeblemindScroll,
										 ItemSortType.HealScroll, ItemSortType.MagicArrowScroll, ItemSortType.NightSightScroll,
										 ItemSortType.ReactiveArmorScroll, ItemSortType.WeakenScroll
									   }
			},
			{
				ItemSortType.AllSecondCircleScrolls,
				new List<ItemSortType> { ItemSortType.AgilityScroll, ItemSortType.CunningScroll, ItemSortType.CureScroll, 
										 ItemSortType.HarmScroll, ItemSortType.MagicTrapScroll, ItemSortType.MagicUntrapScroll, 
										 ItemSortType.ProtectionScroll, ItemSortType.StrengthScroll
									   }
			},
			{
				ItemSortType.AllThirdCircleScrolls,
				new List<ItemSortType> { ItemSortType.BlessScroll, ItemSortType.FireballScroll, ItemSortType.MagicLockScroll, 
										 ItemSortType.PoisonScroll, ItemSortType.TelekinesisScroll, ItemSortType.TeleportScroll, 
										 ItemSortType.UnlockScroll, ItemSortType.WallofStoneScroll
									   }
			},
			{
				ItemSortType.AllForthCircleScrolls,
				new List<ItemSortType> { ItemSortType.ArchCureScroll, ItemSortType.ArchProtectionScroll, ItemSortType.CurseScroll, 
										 ItemSortType.FireFieldScroll, ItemSortType.GreaterHealScroll, ItemSortType.LightningScroll, 
										 ItemSortType.ManaDrainScroll, ItemSortType.RecallScroll
									   }
			},
			{
				ItemSortType.AllFifthCircleScrolls,
				new List<ItemSortType> { ItemSortType.BladeSpiritsScroll, ItemSortType.DispelFieldScroll, ItemSortType.IncognitoScroll, 
										 ItemSortType.MagicReflectionScroll, ItemSortType.MindBlastScroll, ItemSortType.ParalyzeScroll, 
										 ItemSortType.PoisonFieldScroll, ItemSortType.SummonCreatureScroll
									   } 
			},
			{
				ItemSortType.AllSixthCircleScrolls,
				new List<ItemSortType> { ItemSortType.DispelScroll, ItemSortType.EnergyBoltScroll, ItemSortType.ExplosionScroll, 
										 ItemSortType.InvisibilityScroll, ItemSortType.MarkScroll, ItemSortType.MassCurseScroll, 
										 ItemSortType.ParalyzeFieldScroll, ItemSortType.RevealScroll
									   }
			},
			{
				ItemSortType.AllSeventhCircleScrolls,
				new List<ItemSortType> { ItemSortType.ChainLightningScroll, ItemSortType.EnergyFieldScroll, ItemSortType.FlamestrikeScroll, 
										 ItemSortType.GateTravelScroll, ItemSortType.ManaVampireScroll, ItemSortType.MassDispelScroll, 
										 ItemSortType.MeteorSwarmScroll, ItemSortType.PolymorphScroll
									   }
			},
			{
				ItemSortType.AllEighthCircleScrolls,
				new List<ItemSortType> { ItemSortType.EarthquakeScroll, ItemSortType.EnergyVortexScroll, ItemSortType.ResurrectionScroll, 
										 ItemSortType.SummonAirElementalScroll, ItemSortType.SummonDaemonScroll, ItemSortType.SummonEarthElementalScroll, 
										 ItemSortType.SummonFireElementalScroll, ItemSortType.SummonWaterElementalScroll
									   }
			},
			{
				ItemSortType.AllMysticismScrolls,
				new List<ItemSortType> { ItemSortType.AnimatedWeaponScroll, ItemSortType.BombardScroll, ItemSortType.CleansingWindsScroll, 
										 ItemSortType.EagleStrikeScroll, ItemSortType.EnchantScroll, ItemSortType.HailStormScroll, 
										 ItemSortType.HealingStoneScroll, ItemSortType.MassSleepScroll, ItemSortType.NetherBoltScroll, 
										 ItemSortType.NetherCycloneScroll, ItemSortType.PurgeMagicScroll, ItemSortType.RisingColossusScroll, 
										 ItemSortType.SleepScroll, ItemSortType.SpellPlagueScroll, ItemSortType.SpellTriggerScroll, 
										 ItemSortType.StoneFormScroll
									   }
			},
			{
				ItemSortType.AllNecromancyScrolls,
				new List<ItemSortType> { ItemSortType.AnimateDeadScroll, ItemSortType.BloodOathScroll, ItemSortType.CorpseSkinScroll, 
										 ItemSortType.CurseWeaponScroll, ItemSortType.EvilOmenScroll, ItemSortType.HorrificBeastScroll, 
										 ItemSortType.LichFormScroll, ItemSortType.MindRotScroll, ItemSortType.PainSpikeScroll, 
										 ItemSortType.PoisonStrikeScroll, ItemSortType.StrangleScroll, ItemSortType.SummonFamiliarScroll, 
										 ItemSortType.VampiricEmbraceScroll, ItemSortType.VengefulSpiritScroll, ItemSortType.WitherScroll, 
										 ItemSortType.WraithFormScroll, ItemSortType.ExorcismScroll
									   }
			},
			{
				ItemSortType.AllSpellweavingScrolls,
				new List<ItemSortType> { ItemSortType.ArcaneCircleScroll, ItemSortType.GiftofRenewalScroll, ItemSortType.ImmolatingWeaponScroll, 
										 ItemSortType.AttunementScroll, ItemSortType.ThunderstormScroll, ItemSortType.NaturesFuryScroll, 
										 ItemSortType.SummonFeyScroll, ItemSortType.SummonFiendScroll, ItemSortType.ReaperFormScroll, 
										 ItemSortType.WildfireScroll, ItemSortType.EssenceofWindScroll, ItemSortType.DryadAllureScroll, 
										 ItemSortType.EtherealVoyageScroll, ItemSortType.WordofDeathScroll, ItemSortType.GiftofLifeScroll, 
										 ItemSortType.ArcaneEmpowermentScroll
									   }
			}
		};
	}
}