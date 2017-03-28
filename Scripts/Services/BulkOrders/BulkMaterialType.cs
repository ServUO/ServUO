using System;
using Server.Items;
using Server.Engines.Craft;

namespace Server.Engines.BulkOrders
{
    public enum BulkMaterialType
    {
        None,
        DullCopper,
        ShadowIron,
        Copper,
        Bronze,
        Gold,
        Agapite,
        Verite,
        Valorite,
        Spined,
        Horned,
        Barbed,
        OakWood,
        AshWood,
        YewWood,
        Heartwood,
        Bloodwood,
        Frostwood
    }

    public enum BulkGenericType
    {
        Iron,
        Cloth,
        Leather,
        Wood
    }

    public class BGTClassifier
    {
        public static BulkGenericType Classify(BODType deedType, Type itemType)
        {
            if (deedType == BODType.Tailor)
            {
                if (itemType == null || itemType.IsSubclassOf(typeof(BaseArmor)) || itemType.IsSubclassOf(typeof(BaseShoes)))
                    return BulkGenericType.Leather;

                return BulkGenericType.Cloth;
            }
            else if (deedType == BODType.Tinkering && itemType != null)
            {
                if(itemType == typeof(Clock) || itemType.IsSubclassOf(typeof(Clock)))
                    return BulkGenericType.Wood;

                CraftItem item = DefTinkering.CraftSystem.CraftItems.SearchFor(itemType);

                if (item != null)
                {
                    Type typeRes = item.Resources.GetAt(0).ItemType;

                    if (typeRes == typeof(Board) || typeRes == typeof(Log))
                        return BulkGenericType.Wood;
                }
            }
            else if (deedType == BODType.Fletching || deedType == BODType.Carpentry)
            {
                return BulkGenericType.Wood;
            }

            return BulkGenericType.Iron;
        }
    }
}