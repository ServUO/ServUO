using System.Collections.Generic;

namespace Server.Services.ContainerSort
{
	public class SortItemMap
	{

		//gold = 0xEEF
		public static Dictionary<SortCategoryEntry, List<SortItemEntry>> Map = new Dictionary<SortCategoryEntry, List<SortItemEntry>>
		{
			{
				new SortCategoryEntry { CategoryType =  ItemSortCategoryType.Ore, DisplayName = "Mining" },
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
			}
		};
	}
}