using System.Collections.Generic;

namespace Server.Services.ContainerSort
{
	public class SortItemMap
	{

		//gold = 0xEEF
		public static Dictionary<SortCategoryEntry, List<SortItemEntry>> Map = new Dictionary<SortCategoryEntry, List<SortItemEntry>>
		{
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Mining, DisplayName = "Mining" },
				new List<SortItemEntry>
				{
					new SortItemEntry { SortType = ItemSortType.AllOre, DisplayNames = new List<string> { "All", "Ore" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.AllIgnots, DisplayNames = new List<string> { "All", "Ignots" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.IronOre, DisplayNames = new List<string> { "Iron" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, HorizontalOffset = 15, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.DullCopperOre, DisplayNames = new List<string> { "Dull Copper" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x973, HorizontalOffset = 15, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.ShadowIronOre, DisplayNames = new List<string> { "Shadow Iron" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x966, HorizontalOffset = 15, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.CopperOre, DisplayNames = new List<string> { "Copper" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x96D, HorizontalOffset = 15, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.BronzeOre, DisplayNames = new List<string> { "Bronze" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x972, HorizontalOffset = 15, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.GoldOre, DisplayNames = new List<string> { "Gold" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x8A5, HorizontalOffset = 15, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.AgapiteOre, DisplayNames = new List<string> { "Agapite" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x979, HorizontalOffset = 15, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.VeriteOre, DisplayNames = new List<string> { "Verite" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x89F, HorizontalOffset = 15, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.ValoriteOre, DisplayNames = new List<string> { "Valorite" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x8AB, HorizontalOffset = 15, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.IronIngot, DisplayNames = new List<string> { "Iron" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BF2, HorizontalOffset = 24, VerticalOffset = 19 } },
					new SortItemEntry { SortType = ItemSortType.DullCopperIngot, DisplayNames = new List<string> { "Dull Copper" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BF2, Hue = 0x973, HorizontalOffset = 24, VerticalOffset = 19 } },
					new SortItemEntry { SortType = ItemSortType.ShadowIronIngot, DisplayNames = new List<string> { "Shadow Iron" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BF2, Hue = 0x966, HorizontalOffset = 24, VerticalOffset = 19 } },
					new SortItemEntry { SortType = ItemSortType.CopperIngot, DisplayNames = new List<string> { "Copper" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BF2, Hue = 0x96D, HorizontalOffset = 24, VerticalOffset = 19 } },
					new SortItemEntry { SortType = ItemSortType.BronzeIngot, DisplayNames = new List<string> { "Bronze" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BF2, Hue = 0x972, HorizontalOffset = 24, VerticalOffset = 19 } },
					new SortItemEntry { SortType = ItemSortType.GoldIngot, DisplayNames = new List<string> { "Gold" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BF2, Hue = 0x8A5, HorizontalOffset = 24, VerticalOffset = 19 } },
					new SortItemEntry { SortType = ItemSortType.AgapiteIngot, DisplayNames = new List<string> { "Agapite" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BF2, Hue = 0x979, HorizontalOffset = 24, VerticalOffset = 19 } },
					new SortItemEntry { SortType = ItemSortType.VeriteIngot, DisplayNames = new List<string> { "Verite" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BF2, Hue = 0x89F, HorizontalOffset = 24, VerticalOffset = 19 } },
					new SortItemEntry { SortType = ItemSortType.ValoriteIngot, DisplayNames = new List<string> { "Valorite" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BF2, Hue = 0x8AB, HorizontalOffset = 24, VerticalOffset = 19 } }
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Gems, DisplayName = "Gems" },
				new List<SortItemEntry>
				{
					new SortItemEntry { SortType = ItemSortType.AllGems, DisplayNames = new List<string> { "All", "Gems" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.Amber, DisplayNames = new List<string> { "Amber" }, SortIconInfo = new SortIconInfo { ArtId = 0xF25, HorizontalOffset = 9, VerticalOffset = 15 } },
					new SortItemEntry { SortType = ItemSortType.Citrine, DisplayNames = new List<string> { "Citrine" }, SortIconInfo = new SortIconInfo { ArtId = 0xF15, HorizontalOffset = 4, VerticalOffset = 15 } },
					new SortItemEntry { SortType = ItemSortType.Ruby, DisplayNames = new List<string> { "Ruby" }, SortIconInfo = new SortIconInfo { ArtId = 0xF13, HorizontalOffset = 26, VerticalOffset = 15 } },
					new SortItemEntry { SortType = ItemSortType.Tourmaline, DisplayNames = new List<string> { "Tourmaline" }, SortIconInfo = new SortIconInfo { ArtId = 0xF18, HorizontalOffset = 20, VerticalOffset = 15 } },
					new SortItemEntry { SortType = ItemSortType.Amethyst, DisplayNames = new List<string> { "Amethyst" }, SortIconInfo = new SortIconInfo { ArtId = 0xF16, HorizontalOffset = 9, VerticalOffset = 15 } },
					new SortItemEntry { SortType = ItemSortType.Emerald, DisplayNames = new List<string> { "Emerald" }, SortIconInfo = new SortIconInfo { ArtId = 0xF10, HorizontalOffset = 28, VerticalOffset = 15 } },
					new SortItemEntry { SortType = ItemSortType.Sapphire, DisplayNames = new List<string> { "Sapphire" }, SortIconInfo = new SortIconInfo { ArtId = 0xF11, HorizontalOffset = 17, VerticalOffset = 15 } },
					new SortItemEntry { SortType = ItemSortType.StarSapphire, DisplayNames = new List<string> { "Star", "Sapphire" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF0F, HorizontalOffset = 14, VerticalOffset = 15 } },
					new SortItemEntry { SortType = ItemSortType.Diamond, DisplayNames = new List<string> { "Diamond" }, SortIconInfo = new SortIconInfo { ArtId = 0xF26, HorizontalOffset = 14, VerticalOffset = 15 } },
					new SortItemEntry { SortType = ItemSortType.PrismaticAmber, DisplayNames = new List<string> { "Prismatic", "Amber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF25, HorizontalOffset = 9, VerticalOffset = 15 } },
					new SortItemEntry { SortType = ItemSortType.BrilliantAmber, DisplayNames = new List<string> { "Brilliant", "Amber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x3199, HorizontalOffset = 17, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.BlueDiamond, DisplayNames = new List<string> { "Blue", "Diamond" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x3198, HorizontalOffset = 17, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.DarkSapphire, DisplayNames = new List<string> { "Dark", "Sapphire" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x3192, HorizontalOffset = 17, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.EcruCitrine, DisplayNames = new List<string> { "Ecru Citrine" }, SortIconInfo = new SortIconInfo { ArtId = 0x3195, HorizontalOffset = 17, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.FireRuby, DisplayNames = new List<string> { "Fire Ruby" }, SortIconInfo = new SortIconInfo { ArtId = 0x3197, HorizontalOffset = 17, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.PerfectEmerald, DisplayNames = new List<string> { "Perfect", "Emerald" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x3194, HorizontalOffset = 17, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.Turquoise, DisplayNames = new List<string> { "Turquoise" }, SortIconInfo = new SortIconInfo { ArtId = 0x3193, HorizontalOffset = 17, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.WhitePearl, DisplayNames = new List<string> { "White Pearl" }, SortIconInfo = new SortIconInfo { ArtId = 0x3196, HorizontalOffset = 17, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.ArcaneGem, DisplayNames = new List<string> { "Arcane Gem" }, SortIconInfo = new SortIconInfo { ArtId = 0x1EA7, HorizontalOffset = 17, VerticalOffset = 13 } }
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Reagents, DisplayName = "Reagents" },
				new List<SortItemEntry>
				{
					new SortItemEntry { SortType = ItemSortType.AllMageryReagents, DisplayNames = new List<string> { "All", "Magery", "Reagents" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.AllNecromancyReagents, DisplayNames = new List<string> { "All", "Necromancy", "Reagents" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.AllMysticismReagents, DisplayNames = new List<string> { "All", "Mysticism", "Reagents" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.BlackPearl, DisplayNames = new List<string> { "Black Pearl" }, SortIconInfo = new SortIconInfo { ArtId = 0xF7A, HorizontalOffset = 16, VerticalOffset = 15 } },
					new SortItemEntry { SortType = ItemSortType.BloodMoss, DisplayNames = new List<string> { "Blood Moss" }, SortIconInfo = new SortIconInfo { ArtId = 0xF7B, HorizontalOffset = 15, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.Garlic, DisplayNames = new List<string> { "Garlic" }, SortIconInfo = new SortIconInfo { ArtId = 0xF84, HorizontalOffset = 18, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.Ginseng, DisplayNames = new List<string> { "Ginseng" }, SortIconInfo = new SortIconInfo { ArtId = 0xF85, HorizontalOffset = 15, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.MandrakeRoot, DisplayNames = new List<string> { "Mandrake", "Root" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF86, HorizontalOffset = 15, VerticalOffset = 9 } },
					new SortItemEntry { SortType = ItemSortType.Nightshade, DisplayNames = new List<string> { "Nightshade" }, SortIconInfo = new SortIconInfo { ArtId = 0xF88, HorizontalOffset = 17, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.SpidersSilk, DisplayNames = new List<string> { "Spider's Silk" }, SortIconInfo = new SortIconInfo { ArtId = 0xF8D, HorizontalOffset = 20, VerticalOffset = 10 } },
					new SortItemEntry { SortType = ItemSortType.SulfurousAsh, DisplayNames = new List<string> { "Sulfurous", "Ash" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF8C, HorizontalOffset = 15, VerticalOffset = 9 } },
					new SortItemEntry { SortType = ItemSortType.GraveDust, DisplayNames = new List<string> { "Grave Dust" }, SortIconInfo = new SortIconInfo { ArtId = 0xF8F, HorizontalOffset = 14, VerticalOffset = 12 } },
					new SortItemEntry { SortType = ItemSortType.NoxCrystal, DisplayNames = new List<string> { "Nox Crystal" }, SortIconInfo = new SortIconInfo { ArtId = 0xF8E, HorizontalOffset = 26, VerticalOffset = 10 } },
					new SortItemEntry { SortType = ItemSortType.DaemonBlood, DisplayNames = new List<string> { "Daemon", "Blood" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF7D, HorizontalOffset = 18, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.Batwing, DisplayNames = new List<string> { "Batwing" }, SortIconInfo = new SortIconInfo { ArtId = 0xF78, HorizontalOffset = 16, VerticalOffset = 10 } },
					new SortItemEntry { SortType = ItemSortType.PigIron, DisplayNames = new List<string> { "Pig Iron" }, SortIconInfo = new SortIconInfo { ArtId = 0xF8A, HorizontalOffset = 15, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.Bone, DisplayNames = new List<string> { "Bone" }, SortIconInfo = new SortIconInfo { ArtId = 0xF7E, HorizontalOffset = 10, VerticalOffset = 10 } },
					new SortItemEntry { SortType = ItemSortType.DragonsBlood, DisplayNames = new List<string> { "Dragon's", "Blood" }, TextVerticalOffset = 18, SortIconInfo = new SortIconInfo { ArtId = 0x4077, HorizontalOffset = 21, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.DaemonBone, DisplayNames = new List<string> { "Daemon Bone" }, SortIconInfo = new SortIconInfo { ArtId = 0xF80, HorizontalOffset = 20, VerticalOffset = 10 } },
					new SortItemEntry { SortType = ItemSortType.FertileDirt, DisplayNames = new List<string> { "Fertile Dirt" }, SortIconInfo = new SortIconInfo { ArtId = 0xF81, HorizontalOffset = 24, VerticalOffset = 10 } },
					new SortItemEntry { SortType = ItemSortType.Blackrock, DisplayNames = new List<string> { "Blackrock" }, SortIconInfo = new SortIconInfo { ArtId = 0x136C, Hue = 1954, HorizontalOffset = 19, VerticalOffset = 9 } }
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.MagicLevel, DisplayName = "Magic Level" },
				new List<SortItemEntry>
				{
					new SortItemEntry { SortType = ItemSortType.ItemPowerNone, DisplayNames = new List<string> { "None" }, TextVerticalOffset = 20, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerMinor, DisplayNames = new List<string> { "Minor", "Magical" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerLesser, DisplayNames = new List<string> { "Lesser", "Magical" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerGreater, DisplayNames = new List<string> { "Greater", "Magical" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerMajor, DisplayNames = new List<string> { "Major", "Magical" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerLesserArtifact, DisplayNames = new List<string> { "Lesser", "Artifact" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerGreaterArtifact, DisplayNames = new List<string> { "Greater", "Artifact" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerMajorArtifact, DisplayNames = new List<string> { "Major", "Artifact" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerLegendaryArtifact, DisplayNames = new List<string> { "Legendary", "Artifact" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerReforgedMinor, DisplayNames = new List<string> { "Reforged", "Minor" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerReforgedLesser, DisplayNames = new List<string> { "Reforged", "Lesser" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerReforgedGreater, DisplayNames = new List<string> { "Reforged", "Greater" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerReforgedMajor, DisplayNames = new List<string> { "Reforged", "Major" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ItemPowerReforgedLegendary, DisplayNames = new List<string> { "Reforged", "Legendary" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } }
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Weapons, DisplayName = "Weapons" },
				new List<SortItemEntry>
				{
					new SortItemEntry { SortType = ItemSortType.AllWeapons, DisplayNames = new List<string> { "All", "Weapons" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.WeaponSkillSwords, DisplayNames = new List<string> { "Swords" }, SortIconInfo = new SortIconInfo { ArtId = 0x13B6, HorizontalOffset = 12, VerticalOffset = 5 } },
					new SortItemEntry { SortType = ItemSortType.WeaponSkillMaces, DisplayNames = new List<string> { "Maces" }, SortIconInfo = new SortIconInfo { ArtId = 0xF5C, HorizontalOffset = 14, VerticalOffset = 8 } },
					new SortItemEntry { SortType = ItemSortType.WeaponSkillFencing, DisplayNames = new List<string> { "Fencing" }, SortIconInfo = new SortIconInfo { ArtId = 0x26C0, HorizontalOffset = 14, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.WeaponSkillRanged, DisplayNames = new List<string> { "Ranged" }, SortIconInfo = new SortIconInfo { ArtId = 0x26C3, HorizontalOffset = 22, VerticalOffset = 5 } },
					new SortItemEntry { SortType = ItemSortType.WeaponSkillThrowing, DisplayNames = new List<string> { "Throwing" }, SortIconInfo = new SortIconInfo { ArtId = 0x90A, HorizontalOffset = 19, VerticalOffset = 8 } }
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Armor, DisplayName = "Armor" },
				new List<SortItemEntry>
				{
					new SortItemEntry { SortType = ItemSortType.AllArmorMaterials, DisplayNames = new List<string> { "All", "Armor", "Materials" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.AllArmorSlots, DisplayNames = new List<string> { "All", "Armor", "Slots" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.ClothArmor, DisplayNames = new List<string> { "Cloth" }, SortIconInfo = new SortIconInfo { ArtId = 0x404, HorizontalOffset = 19, VerticalOffset = 9 } },
					new SortItemEntry { SortType = ItemSortType.LeatherArmor, DisplayNames = new List<string> { "Leather" }, SortIconInfo = new SortIconInfo { ArtId = 0x13CC, HorizontalOffset = 14, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.StuddedArmor, DisplayNames = new List<string> { "Studded" }, SortIconInfo = new SortIconInfo { ArtId = 0x13DB, HorizontalOffset = 14, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.BoneArmor, DisplayNames = new List<string> { "Bone" }, SortIconInfo = new SortIconInfo { ArtId = 0x144F, HorizontalOffset = 14, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.SpinedArmor, DisplayNames = new List<string> { "Spined" }, SortIconInfo = new SortIconInfo { ArtId = 0x13CC, HorizontalOffset = 14, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.HornedArmor, DisplayNames = new List<string> { "Horned" }, SortIconInfo = new SortIconInfo { ArtId = 0x13CC, HorizontalOffset = 14, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.BarbedArmor, DisplayNames = new List<string> { "Barbed" }, SortIconInfo = new SortIconInfo { ArtId = 0x13CC, HorizontalOffset = 14, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.RingmailArmor, DisplayNames = new List<string> { "Ringmail" }, SortIconInfo = new SortIconInfo { ArtId = 0x13EC, HorizontalOffset = 14, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.ChainmailArmor, DisplayNames = new List<string> { "Chainmail" }, SortIconInfo = new SortIconInfo { ArtId = 0x13BF, HorizontalOffset = 14, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.PlateArmor, DisplayNames = new List<string> { "Plate" }, SortIconInfo = new SortIconInfo { ArtId = 0x1415, HorizontalOffset = 17, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.DragonArmor, DisplayNames = new List<string> { "Dragon" }, SortIconInfo = new SortIconInfo { ArtId = 0x2641, HorizontalOffset = 14, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.WoodArmor, DisplayNames = new List<string> { "Wood" }, SortIconInfo = new SortIconInfo { ArtId = 0x2B67, HorizontalOffset = 19, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.StoneArmor, DisplayNames = new List<string> { "Stone" }, SortIconInfo = new SortIconInfo { ArtId = 0x286, HorizontalOffset = 19, VerticalOffset = 9 } },
					new SortItemEntry { SortType = ItemSortType.EaringArmorPosition, DisplayNames = new List<string> { "Earing" }, SortIconInfo = new SortIconInfo { ArtId = 0x1087, HorizontalOffset = 17, VerticalOffset = 19 } },
					new SortItemEntry { SortType = ItemSortType.FingerArmorPosition, DisplayNames = new List<string> { "Finger" }, SortIconInfo = new SortIconInfo { ArtId = 0x108A, HorizontalOffset = 14, VerticalOffset = 19 } },
					new SortItemEntry { SortType = ItemSortType.WristArmorPosition, DisplayNames = new List<string> { "Wrist" }, SortIconInfo = new SortIconInfo { ArtId = 0x1086, HorizontalOffset = 18, VerticalOffset = 17 } },
					new SortItemEntry { SortType = ItemSortType.NeckArmorPosition, DisplayNames = new List<string> { "Neck" }, SortIconInfo = new SortIconInfo { ArtId = 0x1413, HorizontalOffset = 23, VerticalOffset = 16 } },
					new SortItemEntry { SortType = ItemSortType.HeadArmorPosition, DisplayNames = new List<string> { "Head" }, SortIconInfo = new SortIconInfo { ArtId = 0x1412, HorizontalOffset = 21, VerticalOffset = 9 } },
					new SortItemEntry { SortType = ItemSortType.RobeArmorPosition, DisplayNames = new List<string> { "Robe" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F03, HorizontalOffset = 14, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.ChestArmorPosition, DisplayNames = new List<string> { "Chest" }, SortIconInfo = new SortIconInfo { ArtId = 0x1415, HorizontalOffset = 17, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.ArmsArmorPosition, DisplayNames = new List<string> { "Arms" }, SortIconInfo = new SortIconInfo { ArtId = 0x1410, HorizontalOffset = 17, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.HandsArmorPosition, DisplayNames = new List<string> { "Hands" }, SortIconInfo = new SortIconInfo { ArtId = 0x1414, HorizontalOffset = 19, VerticalOffset = 9 } },
					new SortItemEntry { SortType = ItemSortType.LegsArmorPosition, DisplayNames = new List<string> { "Legs" }, SortIconInfo = new SortIconInfo { ArtId = 0x1411, HorizontalOffset = 14, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.FeetArmorPosition, DisplayNames = new List<string> { "Feet" }, SortIconInfo = new SortIconInfo { ArtId = 0x170F, HorizontalOffset = 19, VerticalOffset = 9 } },
					new SortItemEntry { SortType = ItemSortType.BackArmorPosition, DisplayNames = new List<string> { "Back" }, SortIconInfo = new SortIconInfo { ArtId = 0x1515, HorizontalOffset = 12, VerticalOffset = 4 } },
					new SortItemEntry { SortType = ItemSortType.ShieldArmorPosition, DisplayNames = new List<string> { "Shield" }, SortIconInfo = new SortIconInfo { ArtId = 0x1B7B, HorizontalOffset = 17, VerticalOffset = 9 } }
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.LumberAndBoards, DisplayName = "Lumber And Boards" },
				new List<SortItemEntry>
				{
					new SortItemEntry { SortType = ItemSortType.AllWoodTypes, DisplayNames = new List<string> { "All", "Lumber" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.AllBoardTypes, DisplayNames = new List<string> { "All", "Boards" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
					new SortItemEntry { SortType = ItemSortType.RegularWood, DisplayNames = new List<string> { "Regular", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, HorizontalOffset = 15, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.OakWood, DisplayNames = new List<string> { "Oak", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x7DA, HorizontalOffset = 15, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.AshWood, DisplayNames = new List<string> { "Ash", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x4A7, HorizontalOffset = 15, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.YewWood, DisplayNames = new List<string> { "Yew", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x4A8, HorizontalOffset = 15, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.Heartwood, DisplayNames = new List<string> { "Heardwood", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x4A9, HorizontalOffset = 15, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.Bloodwood, DisplayNames = new List<string> { "Bloodwood", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x4AA, HorizontalOffset = 15, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.Frostwood, DisplayNames = new List<string> { "Frostwood", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x47F, HorizontalOffset = 15, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.RegularBoard, DisplayNames = new List<string> { "Regular", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, HorizontalOffset = 22, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.OakBoard, DisplayNames = new List<string> { "Oak", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x7DA, HorizontalOffset = 22, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.AshBoard, DisplayNames = new List<string> { "Ash", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x4A7, HorizontalOffset = 22, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.YewBoard, DisplayNames = new List<string> { "Yew", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x4A8, HorizontalOffset = 22, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.HeartwoodBoard, DisplayNames = new List<string> { "Heardwood", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x4A9, HorizontalOffset = 22, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.BloodwoodBoard, DisplayNames = new List<string> { "Bloodwood", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x4AA, HorizontalOffset = 22, VerticalOffset = 7 } },
					new SortItemEntry { SortType = ItemSortType.FrostwoodBoard, DisplayNames = new List<string> { "Frostwood", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x47F, HorizontalOffset = 22, VerticalOffset = 7 } },
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.ImbuingIngredients, DisplayName = "Imbuing Ingredients" },
				new List<SortItemEntry>
				{

				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.LeatherAndScales, DisplayName = "Leather And Scales" },
				new List<SortItemEntry>
				{

				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Potions, DisplayName = "Potions" },
				new List<SortItemEntry>
				{

				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.FirstAndSecondCircleScrolls, DisplayName = "1st & 2nd Circle Scrolls" },
				new List<SortItemEntry>
				{

				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.ThirdAndForthCircleScrolls, DisplayName = "3rd & 4th Circle Scrolls" },
				new List<SortItemEntry>
				{

				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.FifthAndSixthCircleScrolls, DisplayName = "5th & 6th Circle Scrolls" },
				new List<SortItemEntry>
				{

				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.SeventhAndEighthCircleScrolls, DisplayName = "7th & 8th Circle Scrolls" },
				new List<SortItemEntry>
				{

				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.MysticismScrolls, DisplayName = "Mysticism Scrolls" },
				new List<SortItemEntry>
				{

				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.NecromancyScrolls, DisplayName = "Necromancy Scrolls" },
				new List<SortItemEntry>
				{

				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Miscellaneous, DisplayName = "Miscellaneous" },
				new List<SortItemEntry>
				{
					/*
					 * gold
					 * lucky coin
					 * cloth
					 * bandage
					 * arrow
					 * bolt
					 * shaft
					 * feathers
					 */
				}
			}
		};
	}
}