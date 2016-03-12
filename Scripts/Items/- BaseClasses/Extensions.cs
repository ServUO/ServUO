using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public static class Extensions
    {
        public static int GetInsuranceCost(this Item item)
        {
            // Previous to SA everything was a flat 600 gold
            if(!Core.SA)
                return 600;
            // All faction items are 800
            if (item.GetType().IsAssignableFrom(typeof(Factions.FactionItem)))
                return 800;
            // Set peices are 600
            BaseArmor armor = item as BaseArmor;
            if (armor != null && armor.IsSetItem)
                return 600;
			// Newbie items cost 10
			if (item.LootType == LootType.Newbied)
				return 10;
            // Magic items cost their imbue weight
            var imbueWeight = SkillHandlers.Imbuing.GetTotalWeight(item);
            if (imbueWeight > 0)
                return Math.Min(800, Math.Max(10, imbueWeight));
            // Non-magic items cost their vendor cost
            if (Mobiles.GenericBuyInfo.BuyPrices.ContainsKey(item.GetType()))
                return Math.Min(800, Math.Max(10, Mobiles.GenericBuyInfo.BuyPrices[item.GetType()]));
            // And if you can't buy it, it costs 600
            return 600;
        }
    }
}
