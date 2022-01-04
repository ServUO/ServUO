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
			{ ItemSortType.FrostwoodBoard, new ObjectTypeIdentificationRule<FrostwoodBoard>() }
		};
	}
}