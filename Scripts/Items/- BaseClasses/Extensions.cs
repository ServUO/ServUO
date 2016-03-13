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
            var imbueWeight = SkillHandlers.Imbuing.GetTotalWeight(item);
            int cost = 600; // this handles old items, set items, etc

            if (item.GetType().IsAssignableFrom(typeof(Factions.FactionItem)))
                cost = 800;
            else if (imbueWeight > 0)
                cost = Math.Min(800, Math.Max(10, imbueWeight));
            else if (Mobiles.GenericBuyInfo.BuyPrices.ContainsKey(item.GetType()))
                cost = Math.Min(800, Math.Max(10, Mobiles.GenericBuyInfo.BuyPrices[item.GetType()]));
            else if (item.LootType == LootType.Newbied)
                return 10;

            if ((item is BaseArmor && ((BaseArmor)item).NegativeAttributes.Prized > 0) ||
                (item is BaseWeapon && ((BaseWeapon)item).NegativeAttributes.Prized > 0) ||
                (item is BaseJewel && ((BaseJewel)item).NegativeAttributes.Prized > 0) ||
                (item is BaseClothing && ((BaseClothing)item).NegativeAttributes.Prized > 0))
                cost *= 2;

            return cost;
        }
    }
}
