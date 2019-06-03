/*using System;
using Server;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;
using System.IO;

namespace Server.Items
{
    public class NamedPropertyCollection
    {
        public static Dictionary<int, Dictionary<ItemType, NamedPropertyCollection[]>> Table { get; set; } = new Dictionary<int, Dictionary<ItemType, NamedPropertyCollection[]>>();

        public static void Configure()
        {
            Table[0] = null;

            var might = new Dictionary<ItemType, NamedPropertyCollection[]>();
            var mystic = new Dictionary<ItemType, NamedPropertyCollection[]>();
            var animated = new Dictionary<ItemType, NamedPropertyCollection[]>();
            var arcane = new Dictionary<ItemType, NamedPropertyCollection[]>();
            var exquisite = new Dictionary<ItemType, NamedPropertyCollection[]>();
            var vampiric = new Dictionary<ItemType, NamedPropertyCollection[]>();
            var invigorating = new Dictionary<ItemType, NamedPropertyCollection[]>();
            var fortified = new Dictionary<ItemType, NamedPropertyCollection[]>();
            var auspicious = new Dictionary<ItemType, NamedPropertyCollection[]>();
            var charmed = new Dictionary<ItemType, NamedPropertyCollection[]>();
            var vicious = new Dictionary<ItemType, NamedPropertyCollection[]>();
            var towering = new Dictionary<ItemType, NamedPropertyCollection[]>();

            might[ItemType.Melee] = new NamedPropertyCollection[]
            {
                new NamedPropertyCollection(AosWeaponAttribute.HitLeechHits, HitsAndManaLeechTable),
                new NamedPropertyCollection(AosAttribute.BonusHits, WeaponHitsTable),
                new NamedPropertyCollection(AosAttribute.BonusStr, WeaponStrTable),
                new NamedPropertyCollection(AosAttribute.RegenHits, WeaponRegenTable),
            };

            might[ItemType.Ranged] = new NamedPropertyCollection[]
            {
                                        new NamedInfoCol("RandomEater", EaterTable),
                        new NamedInfoCol(AosAttribute.BonusHits, ArmorHitsTable),
                        new NamedInfoCol(AosAttribute.BonusStr, ArmorStrTable),
                        new NamedInfoCol(AosAttribute.RegenHits, ArmorRegenTable),
            };

            might[ItemType.Armor] = new NamedPropertyCollection[]
            {
                new NamedPropertyCollection(AosWeaponAttribute.HitLeechHits, HitsAndManaLeechTable),
                new NamedPropertyCollection(AosAttribute.BonusHits, WeaponHitsTable),
                new NamedPropertyCollection(AosAttribute.BonusStr, WeaponStrTable),
                new NamedPropertyCollection(AosAttribute.RegenHits, WeaponRegenTable),
            };

            might[ItemType.Hat] = might[ItemType.Armor];

            might[ItemType.Shield] = new NamedPropertyCollection[]
            {
                new NamedPropertyCollection(AosWeaponAttribute.HitLeechHits, HitsAndManaLeechTable),
                new NamedPropertyCollection(AosAttribute.BonusHits, WeaponHitsTable),
                new NamedPropertyCollection(AosAttribute.BonusStr, WeaponStrTable),
                new NamedPropertyCollection(AosAttribute.RegenHits, WeaponRegenTable),
            };

            might[ItemType.Jewel] = new NamedPropertyCollection[]
            {
                new NamedPropertyCollection(AosWeaponAttribute.HitLeechHits, HitsAndManaLeechTable),
                new NamedPropertyCollection(AosAttribute.BonusHits, WeaponHitsTable),
                new NamedPropertyCollection(AosAttribute.BonusStr, WeaponStrTable),
                new NamedPropertyCollection(AosAttribute.RegenHits, WeaponRegenTable),
            };
        }

        public int ID { get; set; }
        public ItemType ItemType { get; set; }
        public int[][] ReforgeTable { get; set; }

        public int HardCap { get; set; }

        public NamedPropertyCollection(object attribute, int hardCap)
            : this(ItemPropertyInfo.GetID(attribute), hardCap)
        {
        }

        public NamedPropertyCollection(int id, int hardCap)
        {
            ID = id;
            HardCap = hardCap;
        }

        public NamedPropertyCollection(int id, int[][] table)
        {
        }

        public NamedPropertyCollection(int id, int[][] table)
        {
            ID = id;
            ReforgeTable = table;
        }

        public int ReforgedMin(int resIndex, int preIndex, Item item)
        {
            if (HardCap == 1)
                return 1;

            int max = Max(resIndex, preIndex, item);

            if (resIndex != -1 && preIndex != -1)
            {
                return item is BaseRanged && SecondaryInfo != null ? SecondaryInfo[resIndex][0] : Info[resIndex][0];
            }

            return (int)((double)max * .5);
        }

        public int ReforgedMax(int resIndex, int preIndex, Item item)
        {
            if (resIndex != -1 && preIndex != -1)
            {
                var info = Table;

                if (item is BaseWeapon && this.Attribute is AosWeaponAttribute && ((AosWeaponAttribute)this.Attribute == AosWeaponAttribute.HitLeechHits
                                                        || (AosWeaponAttribute)this.Attribute == AosWeaponAttribute.HitLeechMana))
                {
                    int weight = Table[resIndex][preIndex];
                    return (int)(((BaseWeapon)item).MlSpeed * (weight * 100) / (100 + ((BaseWeapon)item).Attributes.WeaponSpeed));
                }

                if (info != null && resIndex >= 0 && resIndex < info.Length && preIndex >= 0 && preIndex < info[resIndex].Length)
                {
                    return info[resIndex][preIndex];
                }

                return HardCap;
            }

            if (info == null)
            {
                return HardCap;
            }

            return info[Info.Length - 1][Info[Info.Length - 1].Length - 1];
        }
    }
}*/
