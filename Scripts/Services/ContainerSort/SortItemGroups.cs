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
										 ItemSortType.AgapiteOre, ItemSortType.VeriteOre, ItemSortType.ValoriteOre } 
			},
			{
				ItemSortType.AllIgnots,
				new List<ItemSortType> { ItemSortType.IronIngot, ItemSortType.DullCopperIngot, ItemSortType.ShadowIronIngot,
										 ItemSortType.CopperIngot, ItemSortType.BronzeIngot, ItemSortType.GoldIngot,
										 ItemSortType.AgapiteIngot, ItemSortType.VeriteIngot, ItemSortType.ValoriteIngot }
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
			}
		};
	}
}