using System.Collections.Generic;
using Server.Items;
using Server.Services.ContainerSort.ItemIdentification.ItemIdentificationRules;

namespace Server.Services.ContainerSort.ItemIdentification
{
	//Not actually a factory pattern but it is kinda close and naming things is hard
	public class ItemIdentificationRulesFactory
	{
		public const string ITEM_SORT_TYPES_LOOKUP_KEY = "SortTypesItemIs";
		public static bool IsItemOfSortItemType(Item item, ItemSortType sortType)
		{
			//If the item instance has the list of sort items it qualifes for then use it to help pref
			if (item.PropertyBag.ContainsKey(ITEM_SORT_TYPES_LOOKUP_KEY) &&
				item.PropertyBag[ITEM_SORT_TYPES_LOOKUP_KEY] is List<ItemSortType> itemTypes)
			{
				return itemTypes.Contains(sortType);
			}

			//If this list of sort items itsnt populated then populated it and store it for further use.
			var sortTypes = new List<ItemSortType>();

			foreach (var kvp in Map)
			{
				if (kvp.Value.DoesItemQualifyForSortFilter(item))
				{
					sortTypes.Add(kvp.Key);
				}
			}

			item.PropertyBag[ITEM_SORT_TYPES_LOOKUP_KEY] = sortTypes;

			return sortTypes.Contains(sortType); ;
		}

		public static bool IsItemOfSortItemTypes(Item item, List<ItemSortType> sortTypes)
		{
			foreach(var sortType in sortTypes)
			{
				if(IsItemOfSortItemType(item, sortType))
				{
					return true;
				}
			}

			return false;
		}

		private static Dictionary<ItemSortType, IItemIdentificationRule> Map = new Dictionary<ItemSortType, IItemIdentificationRule>
		{
			//Ore
			{ ItemSortType.IronOre, new ObjectTypeIdentificationRule<IronOre>() },
			{ ItemSortType.ShadowIronOre, new ObjectTypeIdentificationRule<ShadowIronOre>() },
			{ ItemSortType.DullCopperOre, new ObjectTypeIdentificationRule<DullCopperOre>() },
			{ ItemSortType.CopperOre, new ObjectTypeIdentificationRule<CopperOre>() },
			{ ItemSortType.BronzeOre, new ObjectTypeIdentificationRule<BronzeOre>() },
			{ ItemSortType.GoldOre, new ObjectTypeIdentificationRule<GoldOre>() },
			{ ItemSortType.AgapiteOre, new ObjectTypeIdentificationRule<AgapiteOre>() },
			{ ItemSortType.VeriteOre, new ObjectTypeIdentificationRule<VeriteOre>() },
			{ ItemSortType.ValoriteOre, new ObjectTypeIdentificationRule<ValoriteOre>() },

			//Ingots
			{ ItemSortType.IronIngot, new ObjectTypeIdentificationRule<IronIngot>() },
			{ ItemSortType.ShadowIronIngot, new ObjectTypeIdentificationRule<ShadowIronIngot>() },
			{ ItemSortType.DullCopperIngot, new ObjectTypeIdentificationRule<DullCopperIngot>() },
			{ ItemSortType.CopperIngot, new ObjectTypeIdentificationRule<CopperIngot>() },
			{ ItemSortType.BronzeIngot, new ObjectTypeIdentificationRule<BronzeIngot>() },
			{ ItemSortType.GoldIngot, new ObjectTypeIdentificationRule<GoldIngot>() },
			{ ItemSortType.AgapiteIngot, new ObjectTypeIdentificationRule<AgapiteIngot>() },
			{ ItemSortType.VeriteIngot, new ObjectTypeIdentificationRule<VeriteIngot>() },
			{ ItemSortType.ValoriteIngot, new ObjectTypeIdentificationRule<ValoriteIngot>() },

			//Gems
			{ ItemSortType.Amber, new ObjectTypeIdentificationRule<Amber>() },
			{ ItemSortType.Citrine, new ObjectTypeIdentificationRule<Citrine>() },
			{ ItemSortType.Ruby, new ObjectTypeIdentificationRule<Ruby>() },
			{ ItemSortType.Tourmaline, new ObjectTypeIdentificationRule<Tourmaline>() },
			{ ItemSortType.Amethyst, new ObjectTypeIdentificationRule<Amethyst>() },
			{ ItemSortType.Emerald, new ObjectTypeIdentificationRule<Emerald>() },
			{ ItemSortType.Sapphire, new ObjectTypeIdentificationRule<Sapphire>() },
			{ ItemSortType.StarSapphire, new ObjectTypeIdentificationRule<StarSapphire>() },
			{ ItemSortType.Diamond, new ObjectTypeIdentificationRule<Diamond>() },
			{ ItemSortType.PrismaticAmber, new ObjectTypeIdentificationRule<PrismaticAmber>() },
			{ ItemSortType.BrilliantAmber, new ObjectTypeIdentificationRule<BrilliantAmber>() },
			{ ItemSortType.BlueDiamond, new ObjectTypeIdentificationRule<BlueDiamond>() },
			{ ItemSortType.DarkSapphire, new ObjectTypeIdentificationRule<DarkSapphire>() },
			{ ItemSortType.EcruCitrine, new ObjectTypeIdentificationRule<EcruCitrine>() },
			{ ItemSortType.FireRuby, new ObjectTypeIdentificationRule<FireRuby>() },
			{ ItemSortType.PerfectEmerald, new ObjectTypeIdentificationRule<PerfectEmerald>() },
			{ ItemSortType.Turquoise, new ObjectTypeIdentificationRule<Turquoise>() },
			{ ItemSortType.WhitePearl, new ObjectTypeIdentificationRule<WhitePearl>() },
			{ ItemSortType.ArcaneGem, new ObjectTypeIdentificationRule<ArcaneGem>() },

			//Reagents
			{ ItemSortType.BlackPearl, new ObjectTypeIdentificationRule<BlackPearl>() },
			{ ItemSortType.BloodMoss, new ObjectTypeIdentificationRule<Bloodmoss>() },
			{ ItemSortType.Garlic, new ObjectTypeIdentificationRule<Garlic>() },
			{ ItemSortType.Ginseng, new ObjectTypeIdentificationRule<Ginseng>() },
			{ ItemSortType.MandrakeRoot, new ObjectTypeIdentificationRule<MandrakeRoot>() },
			{ ItemSortType.Nightshade, new ObjectTypeIdentificationRule<Nightshade>() },
			{ ItemSortType.SpidersSilk, new ObjectTypeIdentificationRule<SpidersSilk>() },
			{ ItemSortType.SulfurousAsh, new ObjectTypeIdentificationRule<SulfurousAsh>() },
			{ ItemSortType.GraveDust, new ObjectTypeIdentificationRule<GraveDust>() },
			{ ItemSortType.NoxCrystal, new ObjectTypeIdentificationRule<NoxCrystal>() },
			{ ItemSortType.DaemonBlood, new ObjectTypeIdentificationRule<DaemonBlood>() },
			{ ItemSortType.Batwing, new ObjectTypeIdentificationRule<BatWing>() },
			{ ItemSortType.PigIron, new ObjectTypeIdentificationRule<PigIron>() },
			{ ItemSortType.Bone, new ObjectTypeIdentificationRule<Bone>() },
			{ ItemSortType.DragonsBlood, new ObjectTypeIdentificationRule<DragonBlood>() },
			{ ItemSortType.DaemonBone, new ObjectTypeIdentificationRule<DaemonBone>() },
			{ ItemSortType.FertileDirt, new ObjectTypeIdentificationRule<FertileDirt>() },
			{ ItemSortType.Blackrock, new ObjectTypeIdentificationRule<Blackrock>() },

			//Power Level
			{ ItemSortType.ItemPowerNone, new ItemPowerIdentificationRule(ItemPower.None) },
			{ ItemSortType.ItemPowerMinor, new ItemPowerIdentificationRule(ItemPower.Minor) },
			{ ItemSortType.ItemPowerLesser, new ItemPowerIdentificationRule(ItemPower.Lesser) },
			{ ItemSortType.ItemPowerGreater, new ItemPowerIdentificationRule(ItemPower.Greater) },
			{ ItemSortType.ItemPowerMajor, new ItemPowerIdentificationRule(ItemPower.Major) },
			{ ItemSortType.ItemPowerLesserArtifact, new ItemPowerIdentificationRule(ItemPower.LesserArtifact) },
			{ ItemSortType.ItemPowerGreaterArtifact, new ItemPowerIdentificationRule(ItemPower.GreaterArtifact) },
			{ ItemSortType.ItemPowerMajorArtifact, new ItemPowerIdentificationRule(ItemPower.MajorArtifact) },
			{ ItemSortType.ItemPowerLegendaryArtifact, new ItemPowerIdentificationRule(ItemPower.LegendaryArtifact) },
			{ ItemSortType.ItemPowerReforgedMinor, new ItemPowerIdentificationRule(ItemPower.ReforgedMinor) },
			{ ItemSortType.ItemPowerReforgedLesser, new ItemPowerIdentificationRule(ItemPower.ReforgedLesser) },
			{ ItemSortType.ItemPowerReforgedGreater, new ItemPowerIdentificationRule(ItemPower.ReforgedGreater) },
			{ ItemSortType.ItemPowerReforgedMajor, new ItemPowerIdentificationRule(ItemPower.ReforgedMajor) },
			{ ItemSortType.ItemPowerReforgedLegendary, new ItemPowerIdentificationRule(ItemPower.ReforgedLegendary) },

			//Weapon Skill
			{ ItemSortType.WeaponSkillSwords, new WeaponSkillIdentificaionRule(SkillName.Swords) },
			{ ItemSortType.WeaponSkillMaces, new WeaponSkillIdentificaionRule(SkillName.Macing) },
			{ ItemSortType.WeaponSkillFencing, new WeaponSkillIdentificaionRule(SkillName.Fencing) },
			{ ItemSortType.WeaponSkillRanged, new WeaponSkillIdentificaionRule(SkillName.Archery) },
			{ ItemSortType.WeaponSkillThrowing, new WeaponSkillIdentificaionRule(SkillName.Throwing) },

			//ArmorMaterials
			{ ItemSortType.ClothArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Cloth) },
			{ ItemSortType.LeatherArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Leather) },
			{ ItemSortType.StuddedArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Studded) },
			{ ItemSortType.BoneArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Bone) },
			{ ItemSortType.SpinedArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Spined) },
			{ ItemSortType.HornedArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Horned) },
			{ ItemSortType.BarbedArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Barbed) },
			{ ItemSortType.RingmailArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Ringmail) },
			{ ItemSortType.ChainmailArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Chainmail) },
			{ ItemSortType.PlateArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Plate) },
			{ ItemSortType.DragonArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Dragon) },
			{ ItemSortType.WoodArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Wood) },
			{ ItemSortType.StoneArmor, new ArmorMaterialIdentificationRule(ArmorMaterialType.Stone) },

			//Armor Slots
			{ ItemSortType.EaringArmorPosition, new ArmorPositionIdentificationRule(Layer.Earrings) },
			{ ItemSortType.FingerArmorPosition, new ArmorPositionIdentificationRule(Layer.Ring) },
			{ ItemSortType.WristArmorPosition, new ArmorPositionIdentificationRule(Layer.Bracelet) },
			{ ItemSortType.NeckArmorPosition, new ArmorPositionIdentificationRule(Layer.Neck) },
			{ ItemSortType.HeadArmorPosition, new ArmorPositionIdentificationRule(Layer.Helm) },
			{ ItemSortType.RobeArmorPosition, new ArmorPositionIdentificationRule(Layer.OuterTorso) },
			{ ItemSortType.ChestArmorPosition, new ArmorPositionIdentificationRule(Layer.InnerTorso) },
			{ ItemSortType.ArmsArmorPosition, new ArmorPositionIdentificationRule(Layer.Arms) },
			{ ItemSortType.HandsArmorPosition, new ArmorPositionIdentificationRule(Layer.Gloves) },
			{ ItemSortType.LegsArmorPosition, new ArmorPositionIdentificationRule(Layer.Pants) },
			{ ItemSortType.FeetArmorPosition, new ArmorPositionIdentificationRule(Layer.Shoes) },
			{ ItemSortType.BackArmorPosition, new ArmorPositionIdentificationRule(Layer.Cloak) },
			{ ItemSortType.ShieldArmorPosition, new ArmorPositionIdentificationRule(Layer.TwoHanded) },

			//Wood
			{ ItemSortType.RegularWood, new ObjectTypeIdentificationRule<Log>() },
			{ ItemSortType.OakWood, new ObjectTypeIdentificationRule<OakLog>() },
			{ ItemSortType.AshWood, new ObjectTypeIdentificationRule<AshLog>() },
			{ ItemSortType.YewWood, new ObjectTypeIdentificationRule<YewLog>() },
			{ ItemSortType.Heartwood, new ObjectTypeIdentificationRule<HeartwoodLog>() },
			{ ItemSortType.Bloodwood, new ObjectTypeIdentificationRule<BloodwoodLog>() },
			{ ItemSortType.Frostwood, new ObjectTypeIdentificationRule<FrostwoodLog>() },

			//Boards
			{ ItemSortType.RegularBoard, new ObjectTypeIdentificationRule<Board>() },
			{ ItemSortType.OakBoard, new ObjectTypeIdentificationRule<OakBoard>() },
			{ ItemSortType.AshBoard, new ObjectTypeIdentificationRule<AshBoard>() },
			{ ItemSortType.YewBoard, new ObjectTypeIdentificationRule<YewBoard>() },
			{ ItemSortType.HeartwoodBoard, new ObjectTypeIdentificationRule<HeartwoodBoard>() },
			{ ItemSortType.BloodwoodBoard, new ObjectTypeIdentificationRule<BloodwoodBoard>() },
			{ ItemSortType.FrostwoodBoard, new ObjectTypeIdentificationRule<FrostwoodBoard>() },

			//Imbuing			
			{ ItemSortType.ArcanicRuneStone, new ObjectTypeIdentificationRule<ArcanicRuneStone>() },
			{ ItemSortType.BottleOfIchor, new ObjectTypeIdentificationRule<BottleIchor>() },
			{ ItemSortType.BouraPelt, new ObjectTypeIdentificationRule<BouraPelt>() },
			{ ItemSortType.ChagaMushroom, new ObjectTypeIdentificationRule<ChagaMushroom>() },
			{ ItemSortType.CrushedGlass, new ObjectTypeIdentificationRule<CrushedGlass>() },
			{ ItemSortType.CrystallineBlackrock, new ObjectTypeIdentificationRule<CrystallineBlackrock>() },
			{ ItemSortType.CrystalShards, new ObjectTypeIdentificationRule<CrystalShards>() },
			{ ItemSortType.DaemonClaw, new ObjectTypeIdentificationRule<DaemonClaw>() },
			{ ItemSortType.DelicateScales, new ObjectTypeIdentificationRule<DelicateScales>() },
			{ ItemSortType.ElvenFletching, new ObjectTypeIdentificationRule<ElvenFletching>() },
			{ ItemSortType.EnchantedEssence, new ObjectTypeIdentificationRule<EnchantedEssence>() },
			{ ItemSortType.FaeryDust, new ObjectTypeIdentificationRule<FaeryDust>() },
			{ ItemSortType.FeyWings, new ObjectTypeIdentificationRule<FeyWings>() },
			{ ItemSortType.GoblinBlood, new ObjectTypeIdentificationRule<GoblinBlood>() },
			{ ItemSortType.LavaSerpentCrust, new ObjectTypeIdentificationRule<LavaSerpentCrust>() },
			{ ItemSortType.LuminescentFungi, new ObjectTypeIdentificationRule<LuminescentFungi>() },
			{ ItemSortType.MagicalResidue, new ObjectTypeIdentificationRule<MagicalResidue>() },
			{ ItemSortType.ParasiticPlant, new ObjectTypeIdentificationRule<ParasiticPlant>() },
			{ ItemSortType.PowderedIron, new ObjectTypeIdentificationRule<PowderedIron>() },
			{ ItemSortType.RaptorTeeth, new ObjectTypeIdentificationRule<RaptorTeeth>() },
			{ ItemSortType.ReflectiveWolfEye, new ObjectTypeIdentificationRule<ReflectiveWolfEye>() },
			{ ItemSortType.RelicFragment, new ObjectTypeIdentificationRule<RelicFragment>() },
			{ ItemSortType.SeedOfRenewal, new ObjectTypeIdentificationRule<SeedOfRenewal>() },
			{ ItemSortType.SilverSnakeSkin, new ObjectTypeIdentificationRule<SilverSnakeSkin>() },
			{ ItemSortType.SlithTongue, new ObjectTypeIdentificationRule<SlithTongue>() },
			{ ItemSortType.SpiderCarapace, new ObjectTypeIdentificationRule<SpiderCarapace>() },
			{ ItemSortType.UndyingFlesh, new ObjectTypeIdentificationRule<UndyingFlesh>() },
			{ ItemSortType.VialOfVitriol, new ObjectTypeIdentificationRule<VialOfVitriol>() },
			{ ItemSortType.VoidOrb, new ObjectTypeIdentificationRule<VoidOrb>() },
			{ ItemSortType.EssenceOfAchievement, new ObjectTypeIdentificationRule<EssenceAchievement>() },
			{ ItemSortType.EssenceOfBalance, new ObjectTypeIdentificationRule<EssenceBalance>() },
			{ ItemSortType.EssenceOfControl, new ObjectTypeIdentificationRule<EssenceControl>() },
			{ ItemSortType.EssenceOfDiligence, new ObjectTypeIdentificationRule<EssenceDiligence>() },
			{ ItemSortType.EssenceOfDirection, new ObjectTypeIdentificationRule<EssenceDirection>() },
			{ ItemSortType.EssenceOfFeeling, new ObjectTypeIdentificationRule<EssenceFeeling>() },
			{ ItemSortType.EssenceOfOrder, new ObjectTypeIdentificationRule<EssenceOrder>() },
			{ ItemSortType.EssenceOfPassion, new ObjectTypeIdentificationRule<EssencePassion>() },
			{ ItemSortType.EssenceOfPrecision, new ObjectTypeIdentificationRule<EssencePrecision>() },
			{ ItemSortType.EssenceOfSingularity, new ObjectTypeIdentificationRule<EssenceSingularity>() },

			//Hides
			{ ItemSortType.LeatherHide, new ObjectTypeIdentificationRule<Hides>() },
			{ ItemSortType.BarbedLeatherHide, new ObjectTypeIdentificationRule<BarbedHides>() },
			{ ItemSortType.HornedLeatherHide, new ObjectTypeIdentificationRule<HornedHides>() },
			{ ItemSortType.SpinedLeatherHide, new ObjectTypeIdentificationRule<SpinedHides>() },

			//Cut Leather
			{ ItemSortType.CutLeather, new ObjectTypeIdentificationRule<Leather>() },
			{ ItemSortType.CutBarbedLeather, new ObjectTypeIdentificationRule<BarbedLeather>() },
			{ ItemSortType.CutHornedLeather, new ObjectTypeIdentificationRule<HornedLeather>() },
			{ ItemSortType.CutSpinedLeather, new ObjectTypeIdentificationRule<SpinedLeather>() },

			//Scales
			{ ItemSortType.BlackDragonScale, new ObjectTypeIdentificationRule<BlackScales>() },
			{ ItemSortType.GreenDragonScale, new ObjectTypeIdentificationRule<GreenScales>() },
			{ ItemSortType.RedDragonScale, new ObjectTypeIdentificationRule<RedScales>() },
			{ ItemSortType.WhiteDragonScale, new ObjectTypeIdentificationRule<WhiteScales>() },
			{ ItemSortType.YellowDragonScale, new ObjectTypeIdentificationRule<YellowScales>() },
			{ ItemSortType.SeaSerpentScale, new ObjectTypeIdentificationRule<BlueScales>() },

			//HealingPotions
			{ ItemSortType.Refresh, new ObjectTypeIdentificationRule<RefreshPotion>() },
			{ ItemSortType.GreaterRefreshment, new ObjectTypeIdentificationRule<TotalRefreshPotion>() },
			{ ItemSortType.LesserHeal, new ObjectTypeIdentificationRule<LesserHealPotion>() },
			{ ItemSortType.Heal, new ObjectTypeIdentificationRule<HealPotion>() },
			{ ItemSortType.GreaterHeal, new ObjectTypeIdentificationRule<GreaterHealPotion>() },
			{ ItemSortType.LesserCure, new ObjectTypeIdentificationRule<LesserCurePotion>() },
			{ ItemSortType.Cure, new ObjectTypeIdentificationRule<CurePotion>() },
			{ ItemSortType.GreaterCure, new ObjectTypeIdentificationRule<GreaterCurePotion>() },
			{ ItemSortType.ElixirOfRebirth, new ObjectTypeIdentificationRule<ElixirOfRebirth>() },
			{ ItemSortType.BarrabHemolymphConcentrate, new ObjectTypeIdentificationRule<BarrabHemolymphConcentrate>() },

			//EnchancingPotions
			{ ItemSortType.Agility, new ObjectTypeIdentificationRule<AgilityPotion>() },
			{ ItemSortType.GreaterAgility, new ObjectTypeIdentificationRule<GreaterAgilityPotion>() },
			{ ItemSortType.NightSight, new ObjectTypeIdentificationRule<NightSightPotion>() },
			{ ItemSortType.Strength, new ObjectTypeIdentificationRule<StrengthPotion>() },
			{ ItemSortType.GreaterStrength, new ObjectTypeIdentificationRule<GreaterStrengthPotion>() },
			{ ItemSortType.Invisiblity, new ObjectTypeIdentificationRule<InvisibilityPotion>() },
			{ ItemSortType.JukariBurnPoultice, new ObjectTypeIdentificationRule<JukariBurnPoiltice>() },
			{ ItemSortType.KurakAmbushersEssence, new ObjectTypeIdentificationRule<KurakAmbushersEssence>() },
			{ ItemSortType.BarakoDraftOfMight, new ObjectTypeIdentificationRule<BarakoDraftOfMight>() },
			{ ItemSortType.UraliTranceTonic, new ObjectTypeIdentificationRule<UraliTranceTonic>() },
			{ ItemSortType.SakkhraProphylaxis, new ObjectTypeIdentificationRule<SakkhraProphylaxisPotion>() },

			//ToxicPotions
			{ ItemSortType.LesserPoison, new ObjectTypeIdentificationRule<LesserPoisonPotion>() },
			{ ItemSortType.Poison, new ObjectTypeIdentificationRule<PoisonPotion>() },
			{ ItemSortType.GreaterPoison, new ObjectTypeIdentificationRule<GreaterPoisonPotion>() },
			{ ItemSortType.DeadlyPoison, new ObjectTypeIdentificationRule<DeadlyPoisonPotion>() },
			{ ItemSortType.Parasitic, new ObjectTypeIdentificationRule<ParasiticPotion>() },
			{ ItemSortType.Darkglow, new ObjectTypeIdentificationRule<DarkglowPotion>() },
			{ ItemSortType.ScouringToxin, new ObjectTypeIdentificationRule<ScouringToxin>() },

			//Explosives
			{ ItemSortType.LesserExplosion, new ObjectTypeIdentificationRule<LesserExplosionPotion>() },
			{ ItemSortType.Explosion, new ObjectTypeIdentificationRule<ExplosionPotion>() },
			{ ItemSortType.GreaterExplosion, new ObjectTypeIdentificationRule<GreaterExplosionPotion>() },
			{ ItemSortType.Conflagration, new ObjectTypeIdentificationRule<ConflagrationPotion>() },
			{ ItemSortType.GreaterConflageration, new ObjectTypeIdentificationRule<GreaterConflagrationPotion>() },
			{ ItemSortType.ConfusionBlast, new ObjectTypeIdentificationRule<ConfusionBlastPotion>() },
			{ ItemSortType.GreaterConfusionBlast, new ObjectTypeIdentificationRule<GreaterConfusionBlastPotion>() },
			{ ItemSortType.BlackPower, new ObjectTypeIdentificationRule<BlackPowder>() },
			{ ItemSortType.FuseCord, new ObjectTypeIdentificationRule<FuseCord>() },

			//Ingredients
			{ ItemSortType.PlantPigment, new ObjectTypeIdentificationRule<PlantPigment>() },
			{ ItemSortType.ColorFixative, new ObjectTypeIdentificationRule<ColorFixative>() },
			{ ItemSortType.CrystalGranules, new ObjectTypeIdentificationRule<CrystalGranules>() },
			{ ItemSortType.CrystalDust, new ObjectTypeIdentificationRule<CrystalDust>() },
			{ ItemSortType.SoftenedReeds, new ObjectTypeIdentificationRule<SoftenedReeds>() },
			{ ItemSortType.Potash, new ObjectTypeIdentificationRule<Potash>() },
			{ ItemSortType.GoldDust, new ObjectTypeIdentificationRule<GoldDust>() },
			{ ItemSortType.PlantClippings, new ObjectTypeIdentificationRule<PlantClippings>() },
			{ ItemSortType.MyrmidexEggsac, new ObjectTypeIdentificationRule<MyrmidexEggsac>() },
			{ ItemSortType.PerfectBanana, new ObjectTypeIdentificationRule<PerfectBanana>() },
			{ ItemSortType.Charcoal, new ObjectTypeIdentificationRule<Charcoal>() },
			{ ItemSortType.Saltpeter, new ObjectTypeIdentificationRule<Saltpeter>() },
			{ ItemSortType.SilverSerpentVenom, new ObjectTypeIdentificationRule<SilverSerpentVenom>() },
			{ ItemSortType.BrokenCrystal, new ObjectTypeIdentificationRule<BrokenCrystals>() },
			{ ItemSortType.ShimmeringCrystal, new ObjectTypeIdentificationRule<ShimmeringCrystals>() },
			{ ItemSortType.MedusaBlood, new ObjectTypeIdentificationRule<MedusaBlood>() },
			{ ItemSortType.CapturedEssence, new ObjectTypeIdentificationRule<CapturedEssence>() },
			{ ItemSortType.LavaBerry, new ObjectTypeIdentificationRule<LavaBerry>() },
			{ ItemSortType.TigerPelt, new ObjectTypeIdentificationRule<TigerPelt>() },
			{ ItemSortType.BlueCorn, new ObjectTypeIdentificationRule<BlueCorn>() },
			{ ItemSortType.ToxicVenomSac, new ObjectTypeIdentificationRule<ToxicVenomSac>() },
			{ ItemSortType.RiverMoss, new ObjectTypeIdentificationRule<RiverMoss>() },

			//Other
			{ ItemSortType.SmokeBomb, new ObjectTypeIdentificationRule<SmokeBomb>() },
			{ ItemSortType.HoveringWisp, new ObjectTypeIdentificationRule<HoveringWisp>() },
			{ ItemSortType.NaturalDye, new ObjectTypeIdentificationRule<NaturalDye>() },
			{ ItemSortType.NexusCore, new ObjectTypeIdentificationRule<NexusCore>() },

			//Scrolls
			{ ItemSortType.ClumsyScroll, new ObjectTypeIdentificationRule<ClumsyScroll>() },
			{ ItemSortType.CreateFoodScroll, new ObjectTypeIdentificationRule<CreateFoodScroll>() },
			{ ItemSortType.FeeblemindScroll, new ObjectTypeIdentificationRule<FeeblemindScroll>() },
			{ ItemSortType.HealScroll, new ObjectTypeIdentificationRule<HealScroll>() },
			{ ItemSortType.MagicArrowScroll, new ObjectTypeIdentificationRule<MagicArrowScroll>() },
			{ ItemSortType.NightSightScroll, new ObjectTypeIdentificationRule<NightSightScroll>() },
			{ ItemSortType.ReactiveArmorScroll, new ObjectTypeIdentificationRule<ReactiveArmorScroll>() },
			{ ItemSortType.WeakenScroll, new ObjectTypeIdentificationRule<WeakenScroll>() },
			{ ItemSortType.AgilityScroll, new ObjectTypeIdentificationRule<AgilityScroll>() },
			{ ItemSortType.CunningScroll, new ObjectTypeIdentificationRule<CunningScroll>() },
			{ ItemSortType.CureScroll, new ObjectTypeIdentificationRule<CureScroll>() },
			{ ItemSortType.HarmScroll, new ObjectTypeIdentificationRule<HarmScroll>() },
			{ ItemSortType.MagicTrapScroll, new ObjectTypeIdentificationRule<MagicTrapScroll>() },
			{ ItemSortType.MagicUntrapScroll, new ObjectTypeIdentificationRule<MagicUnTrapScroll>() },
			{ ItemSortType.ProtectionScroll, new ObjectTypeIdentificationRule<ProtectionScroll>() },
			{ ItemSortType.StrengthScroll, new ObjectTypeIdentificationRule<StrengthScroll>() },
			{ ItemSortType.BlessScroll, new ObjectTypeIdentificationRule< BlessScroll>() },
			{ ItemSortType.FireballScroll, new ObjectTypeIdentificationRule<FireballScroll>() },
			{ ItemSortType.MagicLockScroll, new ObjectTypeIdentificationRule<MagicLockScroll>() },
			{ ItemSortType.PoisonScroll, new ObjectTypeIdentificationRule<PoisonScroll>() },
			{ ItemSortType.TelekinesisScroll, new ObjectTypeIdentificationRule<TelekinisisScroll>() },
			{ ItemSortType.TeleportScroll, new ObjectTypeIdentificationRule<TeleportScroll>() },
			{ ItemSortType.UnlockScroll, new ObjectTypeIdentificationRule<UnlockScroll>() },
			{ ItemSortType.WallofStoneScroll, new ObjectTypeIdentificationRule<WallOfStoneScroll>() },
			{ ItemSortType.ArchCureScroll, new ObjectTypeIdentificationRule<ArchCureScroll>() },
			{ ItemSortType.ArchProtectionScroll, new ObjectTypeIdentificationRule<ArchProtectionScroll>() },
			{ ItemSortType.CurseScroll, new ObjectTypeIdentificationRule<CurseScroll>() },
			{ ItemSortType.FireFieldScroll, new ObjectTypeIdentificationRule<FireFieldScroll>() },
			{ ItemSortType.GreaterHealScroll, new ObjectTypeIdentificationRule<GreaterHealScroll>() },
			{ ItemSortType.LightningScroll, new ObjectTypeIdentificationRule<LightningScroll>() },
			{ ItemSortType.ManaDrainScroll, new ObjectTypeIdentificationRule<ManaDrainScroll>() },
			{ ItemSortType.RecallScroll, new ObjectTypeIdentificationRule<RecallScroll>() },
			{ ItemSortType.BladeSpiritsScroll, new ObjectTypeIdentificationRule<BladeSpiritsScroll>() },
			{ ItemSortType.DispelFieldScroll, new ObjectTypeIdentificationRule<DispelFieldScroll>() },
			{ ItemSortType.IncognitoScroll, new ObjectTypeIdentificationRule<IncognitoScroll>() },
			{ ItemSortType.MagicReflectionScroll, new ObjectTypeIdentificationRule<MagicReflectScroll>() },
			{ ItemSortType.MindBlastScroll, new ObjectTypeIdentificationRule<MindBlastScroll>() },
			{ ItemSortType.ParalyzeScroll, new ObjectTypeIdentificationRule<ParalyzeScroll>() },
			{ ItemSortType.PoisonFieldScroll, new ObjectTypeIdentificationRule<PoisonFieldScroll>() },
			{ ItemSortType.SummonCreatureScroll, new ObjectTypeIdentificationRule<SummonCreatureScroll>() },
			{ ItemSortType.DispelScroll, new ObjectTypeIdentificationRule<DispelScroll>() },
			{ ItemSortType.EnergyBoltScroll, new ObjectTypeIdentificationRule<EnergyBoltScroll>() },
			{ ItemSortType.ExplosionScroll, new ObjectTypeIdentificationRule<ExplosionScroll>() },
			{ ItemSortType.InvisibilityScroll, new ObjectTypeIdentificationRule<InvisibilityScroll>() },
			{ ItemSortType.MarkScroll, new ObjectTypeIdentificationRule<MarkScroll>() },
			{ ItemSortType.MassCurseScroll, new ObjectTypeIdentificationRule<MassCurseScroll>() },
			{ ItemSortType.ParalyzeFieldScroll, new ObjectTypeIdentificationRule<ParalyzeFieldScroll>() },
			{ ItemSortType.RevealScroll, new ObjectTypeIdentificationRule<RevealScroll>() },
			{ ItemSortType.ChainLightningScroll, new ObjectTypeIdentificationRule<ChainLightningScroll>() },
			{ ItemSortType.EnergyFieldScroll, new ObjectTypeIdentificationRule<EnergyFieldScroll>() },
			{ ItemSortType.FlamestrikeScroll, new ObjectTypeIdentificationRule<FlamestrikeScroll>() },
			{ ItemSortType.GateTravelScroll, new ObjectTypeIdentificationRule<GateTravelScroll>() },
			{ ItemSortType.ManaVampireScroll, new ObjectTypeIdentificationRule<ManaVampireScroll>() },
			{ ItemSortType.MassDispelScroll, new ObjectTypeIdentificationRule<MassDispelScroll>() },
			{ ItemSortType.MeteorSwarmScroll, new ObjectTypeIdentificationRule<MeteorSwarmScroll>() },
			{ ItemSortType.PolymorphScroll, new ObjectTypeIdentificationRule<PolymorphScroll>() },
			{ ItemSortType.EarthquakeScroll, new ObjectTypeIdentificationRule<EarthquakeScroll>() },
			{ ItemSortType.EnergyVortexScroll, new ObjectTypeIdentificationRule<EnergyVortexScroll>() },
			{ ItemSortType.ResurrectionScroll, new ObjectTypeIdentificationRule<ResurrectionScroll>() },
			{ ItemSortType.SummonAirElementalScroll, new ObjectTypeIdentificationRule<SummonAirElementalScroll>() },
			{ ItemSortType.SummonDaemonScroll, new ObjectTypeIdentificationRule<SummonDaemonScroll>() },
			{ ItemSortType.SummonEarthElementalScroll, new ObjectTypeIdentificationRule<SummonEarthElementalScroll>() },
			{ ItemSortType.SummonFireElementalScroll, new ObjectTypeIdentificationRule<SummonFireElementalScroll>() },
			{ ItemSortType.SummonWaterElementalScroll, new ObjectTypeIdentificationRule<SummonWaterElementalScroll>() },
			{ ItemSortType.AnimatedWeaponScroll, new ObjectTypeIdentificationRule<AnimatedWeaponScroll>() },
			{ ItemSortType.BombardScroll, new ObjectTypeIdentificationRule<BombardScroll>() },
			{ ItemSortType.CleansingWindsScroll, new ObjectTypeIdentificationRule<CleansingWindsScroll>() },
			{ ItemSortType.EagleStrikeScroll, new ObjectTypeIdentificationRule<EagleStrikeScroll>() },
			{ ItemSortType.EnchantScroll, new ObjectTypeIdentificationRule<EnchantScroll>() },
			{ ItemSortType.HailStormScroll, new ObjectTypeIdentificationRule<HailStormScroll>() },
			{ ItemSortType.HealingStoneScroll, new ObjectTypeIdentificationRule<HealingStoneScroll>() },
			{ ItemSortType.MassSleepScroll, new ObjectTypeIdentificationRule<MassSleepScroll>() },
			{ ItemSortType.NetherBoltScroll, new ObjectTypeIdentificationRule<NetherBoltScroll>() },
			{ ItemSortType.NetherCycloneScroll, new ObjectTypeIdentificationRule<NetherCycloneScroll>() },
			{ ItemSortType.PurgeMagicScroll, new ObjectTypeIdentificationRule<PurgeMagicScroll>() },
			{ ItemSortType.RisingColossusScroll, new ObjectTypeIdentificationRule<RisingColossusScroll>() },
			{ ItemSortType.SleepScroll, new ObjectTypeIdentificationRule<SleepScroll>() },
			{ ItemSortType.SpellPlagueScroll, new ObjectTypeIdentificationRule<SpellPlagueScroll>() },
			{ ItemSortType.SpellTriggerScroll, new ObjectTypeIdentificationRule<SpellTriggerScroll>() },
			{ ItemSortType.StoneFormScroll, new ObjectTypeIdentificationRule<StoneFormScroll>() },
			{ ItemSortType.AnimateDeadScroll, new ObjectTypeIdentificationRule<AnimateDeadScroll>() },
			{ ItemSortType.BloodOathScroll, new ObjectTypeIdentificationRule<BloodOathScroll>() },
			{ ItemSortType.CorpseSkinScroll, new ObjectTypeIdentificationRule<CorpseSkinScroll>() },
			{ ItemSortType.CurseWeaponScroll, new ObjectTypeIdentificationRule<CurseWeaponScroll>() },
			{ ItemSortType.EvilOmenScroll, new ObjectTypeIdentificationRule<EvilOmenScroll>() },
			{ ItemSortType.HorrificBeastScroll, new ObjectTypeIdentificationRule<HorrificBeastScroll>() },
			{ ItemSortType.LichFormScroll, new ObjectTypeIdentificationRule<LichFormScroll>() },
			{ ItemSortType.MindRotScroll, new ObjectTypeIdentificationRule<MindRotScroll>() },
			{ ItemSortType.PainSpikeScroll, new ObjectTypeIdentificationRule<PainSpikeScroll>() },
			{ ItemSortType.PoisonStrikeScroll, new ObjectTypeIdentificationRule<PoisonStrikeScroll>() },
			{ ItemSortType.StrangleScroll, new ObjectTypeIdentificationRule<StrangleScroll>() },
			{ ItemSortType.SummonFamiliarScroll, new ObjectTypeIdentificationRule<SummonFamiliarScroll>() },
			{ ItemSortType.VampiricEmbraceScroll, new ObjectTypeIdentificationRule<VampiricEmbraceScroll>() },
			{ ItemSortType.VengefulSpiritScroll, new ObjectTypeIdentificationRule<VengefulSpiritScroll>() },
			{ ItemSortType.WitherScroll, new ObjectTypeIdentificationRule<WitherScroll>() },
			{ ItemSortType.WraithFormScroll, new ObjectTypeIdentificationRule<WraithFormScroll>() },
			{ ItemSortType.ExorcismScroll, new ObjectTypeIdentificationRule<ExorcismScroll>() },
			{ ItemSortType.ArcaneCircleScroll, new ObjectTypeIdentificationRule<ArcaneCircleScroll>() },
			{ ItemSortType.GiftofRenewalScroll, new ObjectTypeIdentificationRule<GiftOfRenewalScroll>() },
			{ ItemSortType.ImmolatingWeaponScroll, new ObjectTypeIdentificationRule<ImmolatingWeaponScroll>() },
			{ ItemSortType.AttunementScroll, new ObjectTypeIdentificationRule<AttuneWeaponScroll>() },
			{ ItemSortType.ThunderstormScroll, new ObjectTypeIdentificationRule<ThunderstormScroll>() },
			{ ItemSortType.NaturesFuryScroll, new ObjectTypeIdentificationRule<NatureFuryScroll>() },
			{ ItemSortType.SummonFeyScroll, new ObjectTypeIdentificationRule<SummonFeyScroll>() },
			{ ItemSortType.SummonFiendScroll, new ObjectTypeIdentificationRule<SummonFiendScroll>() },
			{ ItemSortType.ReaperFormScroll, new ObjectTypeIdentificationRule<ReaperFormScroll>() },
			{ ItemSortType.WildfireScroll, new ObjectTypeIdentificationRule<WildfireScroll>() },
			{ ItemSortType.EssenceofWindScroll, new ObjectTypeIdentificationRule<EssenceOfWindScroll>() },
			{ ItemSortType.DryadAllureScroll, new ObjectTypeIdentificationRule<DryadAllureScroll>() },
			{ ItemSortType.EtherealVoyageScroll, new ObjectTypeIdentificationRule<EtherealVoyageScroll>() },
			{ ItemSortType.WordofDeathScroll, new ObjectTypeIdentificationRule<WordOfDeathScroll>() },
			{ ItemSortType.GiftofLifeScroll, new ObjectTypeIdentificationRule<GiftOfLifeScroll>() },
			{ ItemSortType.ArcaneEmpowermentScroll, new ObjectTypeIdentificationRule<ArcaneEmpowermentScroll>() },

			//Miscellaneous
			{ ItemSortType.Gold, new ObjectTypeIdentificationRule<Gold>() },
			{ ItemSortType.BlankScroll, new ObjectTypeIdentificationRule<BlankScroll>() },
			{ ItemSortType.LuckyCoin, new ObjectTypeIdentificationRule<LuckyCoin>() },
			{ ItemSortType.Cloth, new ObjectTypeIdentificationRule<Cloth>() },
			{ ItemSortType.Bandage, new ObjectTypeIdentificationRule<Bandage>() },
			{ ItemSortType.Arrow, new ObjectTypeIdentificationRule<Arrow>() },
			{ ItemSortType.Bolt, new ObjectTypeIdentificationRule<Bolt>() },
			{ ItemSortType.Shaft, new ObjectTypeIdentificationRule<Shaft>() },
			{ ItemSortType.Feather, new ObjectTypeIdentificationRule<Feather>() }
		};
	}
}