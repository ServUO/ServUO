using System.Collections.Generic;

namespace Server.Services.ContainerSort
{
	public class SortItemMap
	{
		public static Dictionary<SortCategoryEntry, List<SortPageEntry>> Map = new Dictionary<SortCategoryEntry, List<SortPageEntry>>
		{
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Mining, DisplayName = "Mining" },
				new List<SortPageEntry>
				{
					new SortPageEntry
					{
						DisplayName = "Ore",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllOre, DisplayNames = new List<string> { "All", "Ore" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.IronOre, DisplayNames = new List<string> { "Iron" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, HorizontalOffset = 15, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.DullCopperOre, DisplayNames = new List<string> { "Dull Copper" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x973, HorizontalOffset = 15, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.ShadowIronOre, DisplayNames = new List<string> { "Shadow Iron" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x966, HorizontalOffset = 15, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.CopperOre, DisplayNames = new List<string> { "Copper" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x96D, HorizontalOffset = 15, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.BronzeOre, DisplayNames = new List<string> { "Bronze" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x972, HorizontalOffset = 15, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.GoldOre, DisplayNames = new List<string> { "Gold" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x8A5, HorizontalOffset = 15, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.AgapiteOre, DisplayNames = new List<string> { "Agapite" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x979, HorizontalOffset = 15, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.VeriteOre, DisplayNames = new List<string> { "Verite" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x89F, HorizontalOffset = 15, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.ValoriteOre, DisplayNames = new List<string> { "Valorite" }, SortIconInfo = new SortIconInfo { ArtId = 0x19B9, Hue = 0x8AB, HorizontalOffset = 15, VerticalOffset = 4 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Ingots",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllIgnots, DisplayNames = new List<string> { "All", "Ignots" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
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
					}
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Gems, DisplayName = "Gems" },
				new List<SortPageEntry>
				{
					new SortPageEntry
					{
						DisplayName = "Gems",
						Items = new List<SortItemEntry>
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
					}
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Reagents, DisplayName = "Reagents" },
				new List<SortPageEntry>
				{
					new SortPageEntry
					{
						DisplayName = "Magery Reagents",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllMageryReagents, DisplayNames = new List<string> { "All", "Magery", "Reagents" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.BlackPearl, DisplayNames = new List<string> { "Black Pearl" }, SortIconInfo = new SortIconInfo { ArtId = 0xF7A, HorizontalOffset = 16, VerticalOffset = 15 } },
							new SortItemEntry { SortType = ItemSortType.BloodMoss, DisplayNames = new List<string> { "Blood Moss" }, SortIconInfo = new SortIconInfo { ArtId = 0xF7B, HorizontalOffset = 15, VerticalOffset = 12 } },
							new SortItemEntry { SortType = ItemSortType.Garlic, DisplayNames = new List<string> { "Garlic" }, SortIconInfo = new SortIconInfo { ArtId = 0xF84, HorizontalOffset = 18, VerticalOffset = 12 } },
							new SortItemEntry { SortType = ItemSortType.Ginseng, DisplayNames = new List<string> { "Ginseng" }, SortIconInfo = new SortIconInfo { ArtId = 0xF85, HorizontalOffset = 15, VerticalOffset = 12 } },
							new SortItemEntry { SortType = ItemSortType.MandrakeRoot, DisplayNames = new List<string> { "Mandrake", "Root" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF86, HorizontalOffset = 15, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.Nightshade, DisplayNames = new List<string> { "Nightshade" }, SortIconInfo = new SortIconInfo { ArtId = 0xF88, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SpidersSilk, DisplayNames = new List<string> { "Spider's Silk" }, SortIconInfo = new SortIconInfo { ArtId = 0xF8D, HorizontalOffset = 20, VerticalOffset = 10 } },
							new SortItemEntry { SortType = ItemSortType.SulfurousAsh, DisplayNames = new List<string> { "Sulfurous", "Ash" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF8C, HorizontalOffset = 15, VerticalOffset = 9 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Necromancy Reagents",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllNecromancyReagents, DisplayNames = new List<string> { "All", "Necromancy", "Reagents" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.GraveDust, DisplayNames = new List<string> { "Grave Dust" }, SortIconInfo = new SortIconInfo { ArtId = 0xF8F, HorizontalOffset = 14, VerticalOffset = 12 } },
							new SortItemEntry { SortType = ItemSortType.NoxCrystal, DisplayNames = new List<string> { "Nox Crystal" }, SortIconInfo = new SortIconInfo { ArtId = 0xF8E, HorizontalOffset = 26, VerticalOffset = 10 } },
							new SortItemEntry { SortType = ItemSortType.DaemonBlood, DisplayNames = new List<string> { "Daemon", "Blood" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF7D, HorizontalOffset = 18, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.Batwing, DisplayNames = new List<string> { "Batwing" }, SortIconInfo = new SortIconInfo { ArtId = 0xF78, HorizontalOffset = 16, VerticalOffset = 10 } },
							new SortItemEntry { SortType = ItemSortType.PigIron, DisplayNames = new List<string> { "Pig Iron" }, SortIconInfo = new SortIconInfo { ArtId = 0xF8A, HorizontalOffset = 15, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Mysticism Reagents",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllMysticismReagents, DisplayNames = new List<string> { "All", "Mysticism", "Reagents" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.Bone, DisplayNames = new List<string> { "Bone" }, SortIconInfo = new SortIconInfo { ArtId = 0xF7E, HorizontalOffset = 10, VerticalOffset = 10 } },
							new SortItemEntry { SortType = ItemSortType.DragonsBlood, DisplayNames = new List<string> { "Dragon's", "Blood" }, TextVerticalOffset = 18, SortIconInfo = new SortIconInfo { ArtId = 0x4077, HorizontalOffset = 21, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.DaemonBone, DisplayNames = new List<string> { "Daemon Bone" }, SortIconInfo = new SortIconInfo { ArtId = 0xF80, HorizontalOffset = 20, VerticalOffset = 10 } },
							new SortItemEntry { SortType = ItemSortType.FertileDirt, DisplayNames = new List<string> { "Fertile Dirt" }, SortIconInfo = new SortIconInfo { ArtId = 0xF81, HorizontalOffset = 24, VerticalOffset = 10 } },
							new SortItemEntry { SortType = ItemSortType.Blackrock, DisplayNames = new List<string> { "Blackrock" }, SortIconInfo = new SortIconInfo { ArtId = 0x136C, Hue = 1954, HorizontalOffset = 19, VerticalOffset = 9 } }
						}
					}
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.MagicLevel, DisplayName = "Magic Level" },
				new List<SortPageEntry>
				{
					new SortPageEntry
					{
						DisplayName = "Magic Level",
						Items = new List<SortItemEntry>
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
					}
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.WeaponsAndArmor, DisplayName = "Weapons And Armor" },
				new List<SortPageEntry>
				{
					new SortPageEntry
					{
						DisplayName = "Weapons",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllWeapons, DisplayNames = new List<string> { "All", "Weapons" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.WeaponSkillSwords, DisplayNames = new List<string> { "Swords" }, SortIconInfo = new SortIconInfo { ArtId = 0x13B6, HorizontalOffset = 12, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.WeaponSkillMaces, DisplayNames = new List<string> { "Maces" }, SortIconInfo = new SortIconInfo { ArtId = 0xF5C, HorizontalOffset = 14, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.WeaponSkillFencing, DisplayNames = new List<string> { "Fencing" }, SortIconInfo = new SortIconInfo { ArtId = 0x26C0, HorizontalOffset = 14, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.WeaponSkillRanged, DisplayNames = new List<string> { "Ranged" }, SortIconInfo = new SortIconInfo { ArtId = 0x26C3, HorizontalOffset = 22, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.WeaponSkillThrowing, DisplayNames = new List<string> { "Throwing" }, SortIconInfo = new SortIconInfo { ArtId = 0x90A, HorizontalOffset = 19, VerticalOffset = 8 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Armor Materials",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllArmorMaterials, DisplayNames = new List<string> { "All", "Armor", "Materials" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
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
							new SortItemEntry { SortType = ItemSortType.StoneArmor, DisplayNames = new List<string> { "Stone" }, SortIconInfo = new SortIconInfo { ArtId = 0x286, HorizontalOffset = 19, VerticalOffset = 9 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Armor Slots",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllArmorSlots, DisplayNames = new List<string> { "All", "Armor", "Slots" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
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
					}
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.LumberAndBoards, DisplayName = "Lumber And Boards" },
				new List<SortPageEntry>
				{
					new SortPageEntry
					{
						DisplayName = "Lumber",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllWoodTypes, DisplayNames = new List<string> { "All", "Lumber" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.RegularWood, DisplayNames = new List<string> { "Regular", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, HorizontalOffset = 15, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.OakWood, DisplayNames = new List<string> { "Oak", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x7DA, HorizontalOffset = 15, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.AshWood, DisplayNames = new List<string> { "Ash", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x4A7, HorizontalOffset = 15, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.YewWood, DisplayNames = new List<string> { "Yew", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x4A8, HorizontalOffset = 15, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.Heartwood, DisplayNames = new List<string> { "Heardwood", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x4A9, HorizontalOffset = 15, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.Bloodwood, DisplayNames = new List<string> { "Bloodwood", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x4AA, HorizontalOffset = 15, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.Frostwood, DisplayNames = new List<string> { "Frostwood", "Lumber" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BDD, Hue = 0x47F, HorizontalOffset = 15, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Boards",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllBoardTypes, DisplayNames = new List<string> { "All", "Boards" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.RegularBoard, DisplayNames = new List<string> { "Regular", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.OakBoard, DisplayNames = new List<string> { "Oak", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x7DA, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.AshBoard, DisplayNames = new List<string> { "Ash", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x4A7, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.YewBoard, DisplayNames = new List<string> { "Yew", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x4A8, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.HeartwoodBoard, DisplayNames = new List<string> { "Heardwood", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x4A9, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.BloodwoodBoard, DisplayNames = new List<string> { "Bloodwood", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x4AA, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.FrostwoodBoard, DisplayNames = new List<string> { "Frostwood", "Board" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1BD7, Hue = 0x47F, HorizontalOffset = 22, VerticalOffset = 7 } }
						}
					}
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.ImbuingIngredients, DisplayName = "Imbuing Ingredients" },
				new List<SortPageEntry>
				{
					new SortPageEntry
					{
						DisplayName = "Imbuing Ingredients",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllImbuingIngredients, DisplayNames = new List<string> { "All", "Imbuing", "Ingredients" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.ArcanicRuneStone, DisplayNames = new List<string> { "Arcanic", "Rune Stone" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x573C, HorizontalOffset = 18, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.BottleOfIchor, DisplayNames = new List<string> { "Bottle", "Of Ichor" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5748, HorizontalOffset = 22, VerticalOffset = 11 } },
							new SortItemEntry { SortType = ItemSortType.BouraPelt, DisplayNames = new List<string> { "Boura Pelt" }, SortIconInfo = new SortIconInfo { ArtId = 0x5742, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ChagaMushroom, DisplayNames = new List<string> { "Chaga", "Mushroom" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5743, HorizontalOffset = 18, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.CrushedGlass, DisplayNames = new List<string> { "Crushed", "Glass" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x573B, HorizontalOffset = 18, VerticalOffset = 12 } },
							new SortItemEntry { SortType = ItemSortType.CrystallineBlackrock, DisplayNames = new List<string> { "Crystalline", "Blackrock" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5732, HorizontalOffset = 26, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.CrystalShards, DisplayNames = new List<string> { "Crystal", "Shards" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5738, HorizontalOffset = 18, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.DaemonClaw, DisplayNames = new List<string> { "Daemon Claw" }, SortIconInfo = new SortIconInfo { ArtId = 0x5721, HorizontalOffset = 18, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.DelicateScales, DisplayNames = new List<string> { "Delicate", "Scales" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x573A, HorizontalOffset = 21, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.ElvenFletching, DisplayNames = new List<string> { "Elven", "Fletching" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5737, HorizontalOffset = 18, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.EnchantedEssence, DisplayNames = new List<string> { "Enchanted", "Essence" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2DB2, HorizontalOffset = 20, VerticalOffset = 10 } },
							new SortItemEntry { SortType = ItemSortType.FaeryDust, DisplayNames = new List<string> { "Faery Dust" }, SortIconInfo = new SortIconInfo { ArtId = 0x5745, HorizontalOffset = 18, VerticalOffset = 14 } },
							new SortItemEntry { SortType = ItemSortType.FeyWings, DisplayNames = new List<string> { "Fey Wings" }, SortIconInfo = new SortIconInfo { ArtId = 0x5726, HorizontalOffset = 24, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.GoblinBlood, DisplayNames = new List<string> { "Goblin", "Blood" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x572C, HorizontalOffset = 25, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.LavaSerpentCrust, DisplayNames = new List<string> { "Lava Serpent", "Crust" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x572D, HorizontalOffset = 18, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.LuminescentFungi, DisplayNames = new List<string> { "Luminescent", "Fungi" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x3191, HorizontalOffset = 18, VerticalOffset = 12 } },
							new SortItemEntry { SortType = ItemSortType.MagicalResidue, DisplayNames = new List<string> { "Magical", "Residue" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2DB1, HorizontalOffset = 20, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.ParasiticPlant, DisplayNames = new List<string> { "Parasitic", "Plant" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x3190, HorizontalOffset = 18, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.PowderedIron, DisplayNames = new List<string> { "Powdered", "Iron" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x573D, HorizontalOffset = 16, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.RaptorTeeth, DisplayNames = new List<string> { "Raptor", "Teeth" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5747, HorizontalOffset = 16, VerticalOffset = 10 } },
							new SortItemEntry { SortType = ItemSortType.ReflectiveWolfEye, DisplayNames = new List<string> { "Reflective", "Wolf Eye" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5749, HorizontalOffset = 18, VerticalOffset = 12 } },
							new SortItemEntry { SortType = ItemSortType.RelicFragment, DisplayNames = new List<string> { "Relic", "Fragment" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2DB3, HorizontalOffset = 18, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.SeedOfRenewal, DisplayNames = new List<string> { "Seed Of", "Renewal" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5736, HorizontalOffset = 24, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.SilverSnakeSkin, DisplayNames = new List<string> { "Silver", "Snake Skin" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5744, HorizontalOffset = 30, VerticalOffset = 6 } },
							new SortItemEntry { SortType = ItemSortType.SlithTongue, DisplayNames = new List<string> { "Slith", "Tongue" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5746, HorizontalOffset = 20, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.SpiderCarapace, DisplayNames = new List<string> { "Spider", "Carapace" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5720, HorizontalOffset = 0, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.UndyingFlesh, DisplayNames = new List<string> { "Undying", "Flesh" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5731, HorizontalOffset = 4, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.VialOfVitriol, DisplayNames = new List<string> { "Vial Of", "Vitriol" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x5722, HorizontalOffset = 25, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.VoidOrb, DisplayNames = new List<string> { "Void Orb" }, SortIconInfo = new SortIconInfo { ArtId = 0x573E, HorizontalOffset = 19, VerticalOffset = 11 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Essences",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.EssenceOfAchievement, DisplayNames = new List<string> { "Essence Of", "Achievement" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x571C, Hue = 1724, HorizontalOffset = 23, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.EssenceOfBalance, DisplayNames = new List<string> { "Essence Of", "Balance" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x571C, Hue = 1268, HorizontalOffset = 23, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.EssenceOfControl, DisplayNames = new List<string> { "Essence Of", "Control" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x571C, Hue = 1165, HorizontalOffset = 23, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.EssenceOfDiligence, DisplayNames = new List<string> { "Essence Of", "Diligence" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x571C, Hue = 1166, HorizontalOffset = 23, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.EssenceOfDirection, DisplayNames = new List<string> { "Essence Of", "Direction" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x571C, Hue = 1156, HorizontalOffset = 23, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.EssenceOfFeeling, DisplayNames = new List<string> { "Essence Of", "Feeling" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x571C, Hue = 455, HorizontalOffset = 23, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.EssenceOfOrder, DisplayNames = new List<string> { "Essence Of", "Order" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x571C, Hue = 1153, HorizontalOffset = 23, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.EssenceOfPassion, DisplayNames = new List<string> { "Essence Of", "Passion" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x571C, Hue = 1161, HorizontalOffset = 23, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.EssenceOfPrecision, DisplayNames = new List<string> { "Essence Of", "Precision" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x571C, Hue = 1158, HorizontalOffset = 23, VerticalOffset = 4 } },
							new SortItemEntry { SortType = ItemSortType.EssenceOfSingularity, DisplayNames = new List<string> { "Essence Of", "Singularity" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x571C, Hue = 1109, HorizontalOffset = 23, VerticalOffset = 4 } }
						}
					}
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.LeatherAndScales, DisplayName = "Leather And Scales" },
				new List<SortPageEntry>
				{
					new SortPageEntry
					{
						DisplayName = "Hides",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllHides, DisplayNames = new List<string> { "All", "Hides" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.LeatherHide, DisplayNames = new List<string> { "Leather Hide" }, SortIconInfo = new SortIconInfo { ArtId = 0x1079, HorizontalOffset = 18, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.BarbedLeatherHide, DisplayNames = new List<string> { "Barbed Hide" }, SortIconInfo = new SortIconInfo { ArtId = 0x1079, Hue = 0x851, HorizontalOffset = 18, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.HornedLeatherHide, DisplayNames = new List<string> { "Horned Hide" }, SortIconInfo = new SortIconInfo { ArtId = 0x1079, Hue = 0x845, HorizontalOffset = 18, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.SpinedLeatherHide, DisplayNames = new List<string> { "Spined Hide" }, SortIconInfo = new SortIconInfo { ArtId = 0x1079, Hue = 0x8AC, HorizontalOffset = 18, VerticalOffset = 5 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Cut Leather",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllCutLeather, DisplayNames = new List<string> { "All", "Cut", "Leather" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.CutLeather, DisplayNames = new List<string> { "Cut Leather" }, SortIconInfo = new SortIconInfo { ArtId = 0x1081, HorizontalOffset = 18, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.CutBarbedLeather, DisplayNames = new List<string> { "Cut Barbed", "Leather" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1081, Hue = 0x851, HorizontalOffset = 18, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.CutHornedLeather, DisplayNames = new List<string> { "Cut Horned", "Leather" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1081, Hue = 0x845, HorizontalOffset = 18, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.CutSpinedLeather, DisplayNames = new List<string> { "Cut Spined", "Leather" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1081, Hue = 0x8AC, HorizontalOffset = 18, VerticalOffset = 5 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Scales",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllScales, DisplayNames = new List<string> { "All", "Scales" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.BlackDragonScale, DisplayNames = new List<string> { "Black", "Dragon Scale" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x26B4, Hue = 0x455, HorizontalOffset = 17, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.GreenDragonScale, DisplayNames = new List<string> { "Green", "Dragon Scale" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x26B4, Hue = 0x851, HorizontalOffset = 17, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.RedDragonScale, DisplayNames = new List<string> { "Red", "Dragon Scale" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x26B4, Hue = 0x66D, HorizontalOffset = 17, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.WhiteDragonScale, DisplayNames = new List<string> { "White", "Dragon Scale" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x26B4, Hue = 0x8FD, HorizontalOffset = 17, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.YellowDragonScale, DisplayNames = new List<string> { "Yellow", "Dragon Scale" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x26B4, Hue = 0x8A8, HorizontalOffset = 17, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.SeaSerpentScale, DisplayNames = new List<string> { "Sea Serpent", "Scale" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x26B4, Hue = 0x8B0, HorizontalOffset = 17, VerticalOffset = 9 } }
						}
					}
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Potions, DisplayName = "Alchemy" },
				new List<SortPageEntry>
				{
					new SortPageEntry
					{
						DisplayName = "Healing Potions",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllHealingPotions, DisplayNames = new List<string> { "All", "Healing", "Potions" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.Refresh, DisplayNames = new List<string> { "Refresh" }, SortIconInfo = new SortIconInfo { ArtId = 0xF0B, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.GreaterRefreshment, DisplayNames = new List<string> { "Greater", "Refreshment" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF0B, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.LesserHeal, DisplayNames = new List<string> { "Lesser", "Heal" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF0C, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.Heal, DisplayNames = new List<string> { "Heal" }, SortIconInfo = new SortIconInfo { ArtId = 0xF0C, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.GreaterHeal, DisplayNames = new List<string> { "Greater", "Heal" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF0C, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.LesserCure, DisplayNames = new List<string> { "Lesser", "Cure" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF07, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.Cure, DisplayNames = new List<string> { "Cure" }, SortIconInfo = new SortIconInfo { ArtId = 0xF07, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.GreaterCure, DisplayNames = new List<string> { "Greater", "Cure" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF07, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.ElixirOfRebirth, DisplayNames = new List<string> { "Elixir", "Of Rebirth" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x24E2, HorizontalOffset = 18, Hue = 0x48E, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.BarrabHemolymphConcentrate, DisplayNames = new List<string> { "Barrab", "Hemolymph" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 3846, Hue = 1272, HorizontalOffset = 22, VerticalOffset = 8 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Enchancing Potions",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllEnchancingPotions, DisplayNames = new List<string> { "All", "Enchancing", "Potions" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.Agility, DisplayNames = new List<string> { "Agility" }, SortIconInfo = new SortIconInfo { ArtId = 0xF08, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.GreaterAgility, DisplayNames = new List<string> { "Greater", "Agility" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF08, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.NightSight, DisplayNames = new List<string> { "Night Sight" }, SortIconInfo = new SortIconInfo { ArtId = 0xF06, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.Strength, DisplayNames = new List<string> { "Strength" }, SortIconInfo = new SortIconInfo { ArtId = 0xF09, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.GreaterStrength, DisplayNames = new List<string> { "Greater", "Strength" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF09, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.Invisiblity, DisplayNames = new List<string> { "Invisiblity" }, SortIconInfo = new SortIconInfo { ArtId = 0xF0A, Hue = 0x48D, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.JukariBurnPoultice, DisplayNames = new List<string> { "Jukari Burn", "Poultice" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 3846, Hue = 2727, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.KurakAmbushersEssence, DisplayNames = new List<string> { "Kurak Ambushers", "Essence" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 3846, Hue = 1260, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.BarakoDraftOfMight, DisplayNames = new List<string> { "Barako Draft", "Of Might" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 3846, Hue = 1072, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.UraliTranceTonic, DisplayNames = new List<string> { "Urali", "Trance Tonic" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 3846, Hue = 1098, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.SakkhraProphylaxis, DisplayNames = new List<string> { "Sakkhra", "Prophylaxis" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 3846, Hue = 2531, HorizontalOffset = 22, VerticalOffset = 8 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Toxic Potions",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllToxicPotions, DisplayNames = new List<string> { "All", "Toxic", "Potions" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.LesserPoison, DisplayNames = new List<string> { "Lesser", "Poison" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF0A, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.Poison, DisplayNames = new List<string> { "Poison" }, SortIconInfo = new SortIconInfo { ArtId = 0xF0A, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.GreaterPoison, DisplayNames = new List<string> { "Greater", "Poison" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF0A, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.DeadlyPoison, DisplayNames = new List<string> { "Deadly", "Poison" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF0A, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.Parasitic, DisplayNames = new List<string> { "Parasitic" }, SortIconInfo = new SortIconInfo { ArtId = 0xF0A, Hue = 0x17C, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.Darkglow, DisplayNames = new List<string> { "Darkglow" }, SortIconInfo = new SortIconInfo { ArtId = 0xF0A, Hue = 0x96, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.ScouringToxin, DisplayNames = new List<string> { "Scouring", "Toxin" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1848, HorizontalOffset = 19, VerticalOffset = 8 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Explosives",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllExplosives, DisplayNames = new List<string> { "All", "Explosives" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.LesserExplosion, DisplayNames = new List<string> { "Lesser", "Explosion" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF0D, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.Explosion, DisplayNames = new List<string> { "Explosion" }, SortIconInfo = new SortIconInfo { ArtId = 0xF0D, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.GreaterExplosion, DisplayNames = new List<string> { "Greater", "Explosion" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF0D, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.Conflagration, DisplayNames = new List<string> { "Conflagration" }, SortIconInfo = new SortIconInfo { ArtId = 0xF06, Hue = 0x489, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.GreaterConflageration, DisplayNames = new List<string> { "Greater", "Conflagration" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF06, Hue = 0x489, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.ConfusionBlast, DisplayNames = new List<string> { "Confusion", "Blast" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF06, Hue = 0x48D, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.GreaterConfusionBlast, DisplayNames = new List<string> { "Greater", "Confusion Blast" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xF06, Hue = 0x48D, HorizontalOffset = 22, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.BlackPower, DisplayNames = new List<string> { "Black Power" }, SortIconInfo = new SortIconInfo { ArtId = 0x423A, Hue = 1109, HorizontalOffset = 16, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.FuseCord, DisplayNames = new List<string> { "Fuse Cord" }, SortIconInfo = new SortIconInfo { ArtId = 0x1420, Hue = 1164, HorizontalOffset = 21, VerticalOffset = 12 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Ingredients",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllAlchemyIngredients, DisplayNames = new List<string> { "All", "Ingredients" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.PlantPigment, DisplayNames = new List<string> { "Plant", "Pigment" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x0F02, Hue = 0x1, HorizontalOffset = 10, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.ColorFixative, DisplayNames = new List<string> { "Color", "Fixative" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x182D, Hue = 473, HorizontalOffset = 20, VerticalOffset = 3 } },
							new SortItemEntry { SortType = ItemSortType.CrystalGranules, DisplayNames = new List<string> { "Crystal", "Granules" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 16392, Hue = 2625, HorizontalOffset = 2, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.CrystalDust, DisplayNames = new List<string> { "Crystal", "Dust" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 16393, Hue = 2103, HorizontalOffset = 19, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SoftenedReeds, DisplayNames = new List<string> { "Softened", "Reeds" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x4006, HorizontalOffset = 20, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.Potash, DisplayNames = new List<string> { "Potash" }, SortIconInfo = new SortIconInfo { ArtId = 0x423A, Hue = 1102, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.GoldDust, DisplayNames = new List<string> { "Gold Dust" }, SortIconInfo = new SortIconInfo { ArtId = 0x4C09, Hue = 1177, HorizontalOffset = 19, VerticalOffset = 10 } },
							new SortItemEntry { SortType = ItemSortType.PlantClippings, DisplayNames = new List<string> { "Plant", "Clippings" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x4022, HorizontalOffset = 18, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.MyrmidexEggsac, DisplayNames = new List<string> { "Myrmidex", "Eggsac" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 10248, Hue = 1272, HorizontalOffset = 17, VerticalOffset = 10 } },
							new SortItemEntry { SortType = ItemSortType.PerfectBanana, DisplayNames = new List<string> { "Perfect", "Banana" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 5922, Hue = 1119, HorizontalOffset = 14, VerticalOffset = 6 } },
							new SortItemEntry { SortType = ItemSortType.Charcoal, DisplayNames = new List<string> { "Charcoal" }, SortIconInfo = new SortIconInfo { ArtId = 0x423A, Hue = 1190, HorizontalOffset = 16, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.Saltpeter, DisplayNames = new List<string> { "Saltpeter" }, SortIconInfo = new SortIconInfo { ArtId = 0x423A, Hue = 1150, HorizontalOffset = 16, VerticalOffset = 8 } },
							new SortItemEntry { SortType = ItemSortType.SilverSerpentVenom, DisplayNames = new List<string> { "Silver", "Serpent Venom" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0xE24, Hue = 1155, HorizontalOffset = 17, VerticalOffset = 6 } },
							new SortItemEntry { SortType = ItemSortType.BrokenCrystal, DisplayNames = new List<string> { "Broken", "Crystal" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2247, Hue = 0x2B2, HorizontalOffset = 17, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.ShimmeringCrystal, DisplayNames = new List<string> { "Shimmering", "Crystal" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x222C, HorizontalOffset = 5, VerticalOffset = 0 } },
							new SortItemEntry { SortType = ItemSortType.MedusaBlood, DisplayNames = new List<string> { "Medusa", "Blood" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2DB6, HorizontalOffset = 19, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.CapturedEssence, DisplayNames = new List<string> { "Captured", "Essence" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x318E, HorizontalOffset = 21, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.LavaBerry, DisplayNames = new List<string> { "Lava Berry" }, SortIconInfo = new SortIconInfo { ArtId = 22326, Hue = 1955, HorizontalOffset = 23, VerticalOffset = 10 } },
							new SortItemEntry { SortType = ItemSortType.TigerPelt, DisplayNames = new List<string> { "Tiger Pelt" }, SortIconInfo = new SortIconInfo { ArtId = 0x9BCC, Hue = 243, HorizontalOffset = 3, VerticalOffset = 0 } },
							new SortItemEntry { SortType = ItemSortType.BlueCorn, DisplayNames = new List<string> { "Blue Corn" }, SortIconInfo = new SortIconInfo { ArtId = 0xC81, Hue = 1284, HorizontalOffset = 12, VerticalOffset = 12 } },
							new SortItemEntry { SortType = ItemSortType.ToxicVenomSac, DisplayNames = new List<string> { "Toxic", "Venom Sac" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x4005, HorizontalOffset = 19, VerticalOffset = 11 } },
							new SortItemEntry { SortType = ItemSortType.RiverMoss, DisplayNames = new List<string> { "River Moss" }, SortIconInfo = new SortIconInfo { ArtId = 22333, Hue = 1272, HorizontalOffset = 16, VerticalOffset = 9 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Other",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.SmokeBomb, DisplayNames = new List<string> { "Smoke Bomb" }, SortIconInfo = new SortIconInfo { ArtId = 0x2808, HorizontalOffset = 17, VerticalOffset = 11 } },
							new SortItemEntry { SortType = ItemSortType.HoveringWisp, DisplayNames = new List<string> { "Hovering", "Wisp" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2100, HorizontalOffset = 17, VerticalOffset = 3 } },
							new SortItemEntry { SortType = ItemSortType.NaturalDye, DisplayNames = new List<string> { "Natural Dye" }, SortIconInfo = new SortIconInfo { ArtId = 0x182B, Hue = 2101, HorizontalOffset = 19, VerticalOffset = 5 } },
							new SortItemEntry { SortType = ItemSortType.NexusCore, DisplayNames = new List<string> { "Nexus Core" }, SortIconInfo = new SortIconInfo { ArtId = 0x4B82, HorizontalOffset = 18, VerticalOffset = 4 } }
						}
					}
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Scrolls, DisplayName = "Scrolls" },
				new List<SortPageEntry>
				{
					new SortPageEntry
					{
						DisplayName = "First Circle",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllFirstCircleScrolls, DisplayNames = new List<string> { "All", "First", "Circle" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.ClumsyScroll, DisplayNames = new List<string> { "Clumsy" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F2E, HorizontalOffset = 15, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.CreateFoodScroll, DisplayNames = new List<string> { "Create", "Food" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F2F, HorizontalOffset = 18, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.FeeblemindScroll, DisplayNames = new List<string> { "Feeblemind" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F30, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.HealScroll, DisplayNames = new List<string> { "Heal" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F31, HorizontalOffset = 23, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MagicArrowScroll, DisplayNames = new List<string> { "Magic", "Arrow" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F32, HorizontalOffset = 15, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.NightSightScroll, DisplayNames = new List<string> { "Night", "Sight" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F33, HorizontalOffset = 18, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ReactiveArmorScroll, DisplayNames = new List<string> { "Reactive", "Armor" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F2D, HorizontalOffset = 23, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.WeakenScroll, DisplayNames = new List<string> { "Weaken" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F34, HorizontalOffset = 20, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Second Circle",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllSecondCircleScrolls, DisplayNames = new List<string> { "All", "Second", "Circle" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.AgilityScroll, DisplayNames = new List<string> { "Agility" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F35, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.CunningScroll, DisplayNames = new List<string> { "Cunning" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F36, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.CureScroll, DisplayNames = new List<string> { "Cure" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F37, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.HarmScroll, DisplayNames = new List<string> { "Harm" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F38, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MagicTrapScroll, DisplayNames = new List<string> { "Magic Trap" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F39, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MagicUntrapScroll, DisplayNames = new List<string> { "Magic Untrap" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F3A, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ProtectionScroll, DisplayNames = new List<string> { "Protection" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F3B, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.StrengthScroll, DisplayNames = new List<string> { "Strength" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F3C, HorizontalOffset = 20, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Third Circle",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllThirdCircleScrolls, DisplayNames = new List<string> { "All", "Third", "Circle" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.BlessScroll, DisplayNames = new List<string> { "Bless" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F3D, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.FireballScroll, DisplayNames = new List<string> { "Fireball" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F3E, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MagicLockScroll, DisplayNames = new List<string> { "Magic Lock" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F3F, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.PoisonScroll, DisplayNames = new List<string> { "Poison" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F40, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.TelekinesisScroll, DisplayNames = new List<string> { "Telekinesis" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F41, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.TeleportScroll, DisplayNames = new List<string> { "Teleport" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F42, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.UnlockScroll, DisplayNames = new List<string> { "Unlock" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F43, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.WallofStoneScroll, DisplayNames = new List<string> { "Wall Of", "Stone" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F44, HorizontalOffset = 20, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Forth Circle",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllForthCircleScrolls, DisplayNames = new List<string> { "All", "Forth", "Circle" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.ArchCureScroll, DisplayNames = new List<string> { "Arch Cure" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F45, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ArchProtectionScroll, DisplayNames = new List<string> { "Arch", "Protection" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F46, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.CurseScroll, DisplayNames = new List<string> { "Curse" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F47, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.FireFieldScroll, DisplayNames = new List<string> { "Fire Field" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F48, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.GreaterHealScroll, DisplayNames = new List<string> { "Greater", "Heal" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F49, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.LightningScroll, DisplayNames = new List<string> { "Lightning" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F4A, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ManaDrainScroll, DisplayNames = new List<string> { "Mana Drain" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F4B, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.RecallScroll, DisplayNames = new List<string> { "Recall" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F4C, HorizontalOffset = 20, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Fifth Circle",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllFifthCircleScrolls, DisplayNames = new List<string> { "All", "Fifth", "Circle" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.BladeSpiritsScroll, DisplayNames = new List<string> { "Blade", "Spirits" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F4D, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.DispelFieldScroll, DisplayNames = new List<string> { "Dispel", "Field" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F4E, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.IncognitoScroll, DisplayNames = new List<string> { "Incognito" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F4F, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MagicReflectionScroll, DisplayNames = new List<string> { "Magic", "Reflection" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F50, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MindBlastScroll, DisplayNames = new List<string> { "Mind Blast" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F51, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ParalyzeScroll, DisplayNames = new List<string> { "Paralyze" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F52, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.PoisonFieldScroll, DisplayNames = new List<string> { "Poison", "Field" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F53, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SummonCreatureScroll, DisplayNames = new List<string> { "Summon", "Creature" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F54, HorizontalOffset = 20, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Sixth Circle",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllSixthCircleScrolls, DisplayNames = new List<string> { "All", "Sixth", "Circle" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.DispelScroll, DisplayNames = new List<string> { "Dispel" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F55, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.EnergyBoltScroll, DisplayNames = new List<string> { "Energy Bolt" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F56, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ExplosionScroll, DisplayNames = new List<string> { "Explosion" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F57, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.InvisibilityScroll, DisplayNames = new List<string> { "Invisibility" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F58, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MarkScroll, DisplayNames = new List<string> { "Mark" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F59, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MassCurseScroll, DisplayNames = new List<string> { "Mass Curse" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F5A, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ParalyzeFieldScroll, DisplayNames = new List<string> { "Paralyze", "Field" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F5B, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.RevealScroll, DisplayNames = new List<string> { "Reveal" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F5C, HorizontalOffset = 20, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Seventh Circle",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllSeventhCircleScrolls, DisplayNames = new List<string> { "All", "Seventh", "Circle" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.ChainLightningScroll, DisplayNames = new List<string> { "Chain", "Lightning" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F5D, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.EnergyFieldScroll, DisplayNames = new List<string> { "Energy", "Field" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F5E, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.FlamestrikeScroll, DisplayNames = new List<string> { "Flamestrike" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F5F, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.GateTravelScroll, DisplayNames = new List<string> { "Gate Travel" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F60, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ManaVampireScroll, DisplayNames = new List<string> { "Mana", "Vampire" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F61, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MassDispelScroll, DisplayNames = new List<string> { "Mass Dispel" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F62, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MeteorSwarmScroll, DisplayNames = new List<string> { "Meteor", "Swarm" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F63, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.PolymorphScroll, DisplayNames = new List<string> { "Polymorph" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F64, HorizontalOffset = 20, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Eighth Circle",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllEighthCircleScrolls, DisplayNames = new List<string> { "All", "Eighth", "Circle" }, TextVerticalOffset = 32, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.EarthquakeScroll, DisplayNames = new List<string> { "Earthquake" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F65, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.EnergyVortexScroll, DisplayNames = new List<string> { "Energy", "Vortex" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F66, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ResurrectionScroll, DisplayNames = new List<string> { "Resurrection" }, SortIconInfo = new SortIconInfo { ArtId = 0x1F67, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SummonAirElementalScroll, DisplayNames = new List<string> { "Summon Air", "Elemental" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F68, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SummonDaemonScroll, DisplayNames = new List<string> { "Summon", "Daemon" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F69, HorizontalOffset = 22, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SummonEarthElementalScroll, DisplayNames = new List<string> { "Summon Elemental", "Arrow" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F6A, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SummonFireElementalScroll, DisplayNames = new List<string> { "Summon Fire", "Elemental" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F6B, HorizontalOffset = 17, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SummonWaterElementalScroll, DisplayNames = new List<string> { "Summon Water", "Elemental" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x1F6C, HorizontalOffset = 20, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Mysticism",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllMysticismScrolls, DisplayNames = new List<string> { "All", "Mysticism" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.AnimatedWeaponScroll, DisplayNames = new List<string> { "Animated", "Weapon" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2DA4, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.BombardScroll, DisplayNames = new List<string> { "Bombard" }, SortIconInfo = new SortIconInfo { ArtId = 0x2DA9, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.CleansingWindsScroll, DisplayNames = new List<string> { "Cleansing", "Winds" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2DA8, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.EagleStrikeScroll, DisplayNames = new List<string> { "Eagle Strike" }, SortIconInfo = new SortIconInfo { ArtId = 0x2DA3, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.EnchantScroll, DisplayNames = new List<string> { "Enchant" }, SortIconInfo = new SortIconInfo { ArtId = 0x2DA1, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.HailStormScroll, DisplayNames = new List<string> { "Hail Storm" }, SortIconInfo = new SortIconInfo { ArtId = 0x2DAB, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.HealingStoneScroll, DisplayNames = new List<string> { "Healing", "Stone" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D9F, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MassSleepScroll, DisplayNames = new List<string> { "Mass Sleep" }, SortIconInfo = new SortIconInfo { ArtId = 0x2DA7, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.NetherBoltScroll, DisplayNames = new List<string> { "Nether Bolt" }, SortIconInfo = new SortIconInfo { ArtId = 0x2D9E, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.NetherCycloneScroll, DisplayNames = new List<string> { "Nether", "Cyclone" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2DAC, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.PurgeMagicScroll, DisplayNames = new List<string> { "Purge Magic" }, SortIconInfo = new SortIconInfo { ArtId = 0x2DA0, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.RisingColossusScroll, DisplayNames = new List<string> { "Rising", "Colossus" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2DAD, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SleepScroll, DisplayNames = new List<string> { "Sleep" }, SortIconInfo = new SortIconInfo { ArtId = 0x2DA2, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SpellPlagueScroll, DisplayNames = new List<string> { "Spell", "Plague" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2DAA, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SpellTriggerScroll, DisplayNames = new List<string> { "Spell", "Trigger" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2DA6, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.StoneFormScroll, DisplayNames = new List<string> { "Stone Form" }, SortIconInfo = new SortIconInfo { ArtId = 0x2DA5, HorizontalOffset = 20, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Necromancy",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllNecromancyScrolls, DisplayNames = new List<string> { "All", "Necromancy" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.AnimateDeadScroll, DisplayNames = new List<string> { "Animate", "Dead" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2260, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.BloodOathScroll, DisplayNames = new List<string> { "Blood Oath" }, SortIconInfo = new SortIconInfo { ArtId = 0x2261, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.CorpseSkinScroll, DisplayNames = new List<string> { "Corpse Skin" }, SortIconInfo = new SortIconInfo { ArtId = 0x2262, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.CurseWeaponScroll, DisplayNames = new List<string> { "Curse", "Weapon" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2263, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.EvilOmenScroll, DisplayNames = new List<string> { "Evil Omen" }, SortIconInfo = new SortIconInfo { ArtId = 0x2264, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.HorrificBeastScroll, DisplayNames = new List<string> { "Horrific", "Beast" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2265, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.LichFormScroll, DisplayNames = new List<string> { "Lich Form" }, SortIconInfo = new SortIconInfo { ArtId = 0x2266, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.MindRotScroll, DisplayNames = new List<string> { "Mind Rot" }, SortIconInfo = new SortIconInfo { ArtId = 0x2267, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.PainSpikeScroll, DisplayNames = new List<string> { "Pain Spike" }, SortIconInfo = new SortIconInfo { ArtId = 0x2268, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.PoisonStrikeScroll, DisplayNames = new List<string> { "Poison", "Strike" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2269, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.StrangleScroll, DisplayNames = new List<string> { "Strangle" }, SortIconInfo = new SortIconInfo { ArtId = 0x226A, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SummonFamiliarScroll, DisplayNames = new List<string> { "Summon", "Familiar" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x226B, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.VampiricEmbraceScroll, DisplayNames = new List<string> { "Vampiric", "Embrace" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x226C, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.VengefulSpiritScroll, DisplayNames = new List<string> { "Vengeful", "Spirit" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x226D, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.WitherScroll, DisplayNames = new List<string> { "Wither" }, SortIconInfo = new SortIconInfo { ArtId = 0x226E, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.WraithFormScroll, DisplayNames = new List<string> { "Wraith", "Form" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x226F, HorizontalOffset = 20, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ExorcismScroll, DisplayNames = new List<string> { "Exorcism" }, SortIconInfo = new SortIconInfo { ArtId = 0x2270, HorizontalOffset = 20, VerticalOffset = 7 } }
						}
					},
					new SortPageEntry
					{
						DisplayName = "Spellweaving",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.AllSpellweavingScrolls, DisplayNames = new List<string> { "All", "Spellweaving" }, TextVerticalOffset = 24, SortIconInfo = new SortIconInfo { HorizontalOffset = 300 } },
							new SortItemEntry { SortType = ItemSortType.ArcaneCircleScroll, DisplayNames = new List<string> { "Arcane", "Circle" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D51, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.GiftofRenewalScroll, DisplayNames = new List<string> { "Gift Of", "Renewal" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D52, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ImmolatingWeaponScroll, DisplayNames = new List<string> { "Immolating", "Weapon" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D53, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.AttunementScroll, DisplayNames = new List<string> { "Attunement" }, SortIconInfo = new SortIconInfo { ArtId = 0x2D54, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ThunderstormScroll, DisplayNames = new List<string> { "Thunderstorm" }, SortIconInfo = new SortIconInfo { ArtId = 0x2D55, Hue = 0x8FD, HorizontalOffset = 16, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.NaturesFuryScroll, DisplayNames = new List<string> { "Natures", "Fury" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D56, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SummonFeyScroll, DisplayNames = new List<string> { "Summon", "Fey" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D57, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.SummonFiendScroll, DisplayNames = new List<string> { "Summon", "Fiend" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D58, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ReaperFormScroll, DisplayNames = new List<string> { "Reaper", "Form" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D59, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.WildfireScroll, DisplayNames = new List<string> { "Wildfire" }, SortIconInfo = new SortIconInfo { ArtId = 0x2D5A, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.EssenceofWindScroll, DisplayNames = new List<string> { "Essence", "Of Wind" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D5B, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.DryadAllureScroll, DisplayNames = new List<string> { "Dryad", "Allure" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D5C, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.EtherealVoyageScroll, DisplayNames = new List<string> { "Ethereal", "Voyage" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D5D, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.WordofDeathScroll, DisplayNames = new List<string> { "Word Of", "Death" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D5E, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.GiftofLifeScroll, DisplayNames = new List<string> { "Gift", "Of Life" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D5F, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.ArcaneEmpowermentScroll, DisplayNames = new List<string> { "Arcane", "Empowerment" }, TextVerticalOffset = 15, SortIconInfo = new SortIconInfo { ArtId = 0x2D60, Hue = 0x8FD, HorizontalOffset = 14, VerticalOffset = 7 } }
						}
					}
				}
			},
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Miscellaneous, DisplayName = "Miscellaneous" },
				new List<SortPageEntry>
				{
					new SortPageEntry
					{
						DisplayName = "Miscellaneous",
						Items = new List<SortItemEntry>
						{
							new SortItemEntry { SortType = ItemSortType.Gold, DisplayNames = new List<string> { "Gold" }, SortIconInfo = new SortIconInfo { ArtId = 0xEEF, HorizontalOffset = 18, VerticalOffset = 7 } },
							new SortItemEntry { SortType = ItemSortType.BlankScroll, DisplayNames = new List<string> { "Blank Scroll" }, SortIconInfo = new SortIconInfo { ArtId = 0xEF3, HorizontalOffset = 18, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.LuckyCoin, DisplayNames = new List<string> { "Lucky Coin" }, SortIconInfo = new SortIconInfo { ArtId = 0xF87, Hue = 1174, HorizontalOffset = 18, VerticalOffset = 15 } },
							new SortItemEntry { SortType = ItemSortType.Cloth, DisplayNames = new List<string> { "Cloth" }, SortIconInfo = new SortIconInfo { ArtId = 0x1766, HorizontalOffset = 16, VerticalOffset = 3 } },
							new SortItemEntry { SortType = ItemSortType.Bandage, DisplayNames = new List<string> { "Bandage" }, SortIconInfo = new SortIconInfo { ArtId = 0xE21, HorizontalOffset = 16, VerticalOffset = 12 } },
							new SortItemEntry { SortType = ItemSortType.Arrow, DisplayNames = new List<string> { "Arrow" }, SortIconInfo = new SortIconInfo { ArtId = 0xF3F, HorizontalOffset = 18, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.Bolt, DisplayNames = new List<string> { "Bolt" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BFB, HorizontalOffset = 18, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.Shaft, DisplayNames = new List<string> { "Shaft" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BD4, HorizontalOffset = 18, VerticalOffset = 9 } },
							new SortItemEntry { SortType = ItemSortType.Feather, DisplayNames = new List<string> { "Feather" }, SortIconInfo = new SortIconInfo { ArtId = 0x1BD1, HorizontalOffset = 17, VerticalOffset = 14 } }
						}
					}
				}
			}
		};
	}
}