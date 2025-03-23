#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;

using VitaNex;
#endregion

namespace Server
{
	public static class LayerExtUtility
	{
		private const Layer _Face = (Layer)15;
		private const Layer _ShopMax = (Layer)30;

		private const Layer _PlateArms = (Layer)255;
		private const Layer _ChainTunic = (Layer)254;
		private const Layer _LeatherShorts = (Layer)253;

		public static Layer[] LayerOrder =
		{
			Layer.Cloak, Layer.Bracelet, Layer.Ring, Layer.Shirt, Layer.Pants, Layer.InnerLegs, Layer.Shoes, _LeatherShorts,
			Layer.Arms, Layer.InnerTorso, _LeatherShorts, _PlateArms, Layer.MiddleTorso, Layer.OuterLegs, Layer.Neck,
			Layer.Gloves, Layer.OuterTorso, Layer.Waist, Layer.OneHanded, Layer.TwoHanded, _Face, Layer.FacialHair, Layer.Hair,
			Layer.Helm, Layer.Talisman
		};

		public static Layer[] EquipLayers =
		{
			Layer.Arms, Layer.Bracelet, Layer.Cloak, Layer.Earrings, Layer.Gloves, Layer.Helm, Layer.InnerLegs, Layer.InnerTorso,
			Layer.MiddleTorso, Layer.Neck, Layer.OneHanded, Layer.OuterLegs, Layer.OuterTorso, Layer.Pants, Layer.Ring,
			Layer.Shirt, Layer.Shoes, Layer.Talisman, Layer.TwoHanded, Layer.Waist, Layer.Mount
		};

		public static int[] LayerTable { get; private set; }

		static LayerExtUtility()
		{
			LayerTable = new int[256];

			for (var i = 0; i < LayerOrder.Length; ++i)
			{
				LayerTable[(int)LayerOrder[i]] = LayerOrder.Length - i;
			}
		}

		public static int GetOrderIndex(this Layer layer)
		{
			return LayerOrder.IndexOf(layer);
		}

		public static bool IsEquip(this Layer layer)
		{
			return EquipLayers.Contains(layer);
		}

		public static bool IsMount(this Layer layer)
		{
			return layer == Layer.Mount;
		}

		public static bool IsPack(this Layer layer)
		{
			return layer == Layer.Backpack;
		}

		public static bool IsBank(this Layer layer)
		{
			return layer == Layer.Bank;
		}

		public static bool IsPackOrBank(this Layer layer)
		{
			return IsPack(layer) || IsBank(layer);
		}

		public static bool IsFace(this Layer layer)
		{
			return layer == _Face;
		}

		public static bool IsHair(this Layer layer)
		{
			return layer == Layer.Hair;
		}

		public static bool IsFacialHair(this Layer layer)
		{
			return layer == Layer.FacialHair;
		}

		public static bool IsHairOrFacialHair(this Layer layer)
		{
			return IsHair(layer) || IsFacialHair(layer);
		}

		public static bool IsFaceOrHair(this Layer layer)
		{
			return IsFace(layer) || IsHair(layer);
		}

		public static bool IsFaceOrHairOrFacialHair(this Layer layer)
		{
			return IsFace(layer) || IsHair(layer) || IsFacialHair(layer);
		}

		public static bool IsShop(this Layer layer)
		{
			return layer == Layer.ShopBuy || layer == Layer.ShopResale || layer == Layer.ShopSell || layer == _ShopMax;
		}

		public static bool IsInvalid(this Layer layer)
		{
			return layer == Layer.Invalid;
		}

		public static bool IsValid(this Layer layer)
		{
			return IsEquip(layer) || IsPackOrBank(layer) || IsFaceOrHair(layer);
		}

		public static IOrderedEnumerable<Item> OrderByLayer(this IEnumerable<Item> items)
		{
			return items.OrderBy(item => item == null ? 0 : LayerTable[(int)Fix(item.Layer, item.ItemID)]);
		}

		public static IOrderedEnumerable<Item> OrderByLayerDescending(this IEnumerable<Item> items)
		{
			return items.OrderByDescending(item => item == null ? 0 : LayerTable[(int)Fix(item.Layer, item.ItemID)]);
		}

		public static IOrderedEnumerable<Layer> OrderByLayer(this IEnumerable<Layer> layers)
		{
			return layers.OrderBy(layer => LayerTable[(int)layer]);
		}

		public static IOrderedEnumerable<Layer> OrderByLayerDescending(this IEnumerable<Layer> layers)
		{
			return layers.OrderByDescending(layer => LayerTable[(int)layer]);
		}

		public static void SortLayers(this List<Item> items)
		{
			if (items != null && items.Count > 1)
			{
				items.Sort(CompareLayer);
			}
		}

		public static void SortLayers(this List<Layer> layers)
		{
			if (layers != null && layers.Count > 1)
			{
				layers.Sort(CompareLayer);
			}
		}

		public static int CompareLayer(this Item item, Item other)
		{
			var res = 0;

			if (item.CompareNull(other, ref res))
			{
				return res;
			}

			return CompareLayer(Fix(item.Layer, item.ItemID), Fix(other.Layer, other.ItemID));
		}

		public static int CompareLayer(this Layer layer, Layer other)
		{
			return LayerTable[(int)other] - LayerTable[(int)layer];
		}

		public static bool IsOrdered(this Layer layer)
		{
			return LayerTable[(int)layer] > 0;
		}

		public static Layer Fix(this Layer layer, int itemID)
		{
			if (itemID == 0x1410 || itemID == 0x1417) // platemail arms
			{
				return _PlateArms;
			}

			if (itemID == 0x13BF || itemID == 0x13C4) // chainmail tunic
			{
				return _ChainTunic;
			}

			if (itemID == 0x1C08 || itemID == 0x1C09) // leather skirt
			{
				return _LeatherShorts;
			}

			if (itemID == 0x1C00 || itemID == 0x1C01) // leather shorts
			{
				return _LeatherShorts;
			}

			return layer;
		}

		public static PaperdollBounds GetPaperdollBounds(this Layer layer)
		{
			return PaperdollBounds.Find(layer);
		}
	}
}