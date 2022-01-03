using System.Collections.Generic;
using Server.Items;
using Server.Services.ContainerSort.ItemIdentification.ItemIdentificationRules;

namespace Server.Services.ContainerSort.ItemIdentification
{
	public class ItemIdentificationRulesFactory
	{
		public static bool IsItemOfSortItemType(Item item, ItemSortType sortType)
		{
			if (Map.ContainsKey(sortType))
			{
				return Map[sortType].DoesItemQualifyForSortFilter(item);
			}

			return false;
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
			{ ItemSortType.ArcaneGem, new ObjectTypeIdentificationRule<ArcaneGem>() }
		};
	}
}