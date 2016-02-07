using System;
using Server.Items;

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
        Barbed
    }

    public enum BulkGenericType
    {
        Iron,
        Cloth,
        Leather
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

            return BulkGenericType.Iron;
        }
    }
}