using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.Craft;
using Server.SkillHandlers;
using Server.Misc;
using Server.Gumps;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public enum ReforgedPrefix
    {
        None,
        Might,
        Mystic,
        Animated,
        Arcane,
        Exquisite,
        Vampiric,
        Invigorating,
        Fortified,
        Auspicious,
        Charmed,
        Vicious,
        Towering
    }

    public enum ReforgedSuffix
    {
        None,
        Vitality,
        Sorcery,
        Haste,
        Wizadry,
        Quality,
        Vampire,
        Restoration,
        Defense, 
        Fortune, 
        Alchemy,
        Slaughter,
        Aegis,
        Blackthorn,
        Minax,
        Kotl
    }

    public enum ItemPower
    {
        None,
        Minor,
        Lesser,
        Greater, 
        Major,
        LesserArtifact,
        GreaterArtifact,
        MajorArtifact, 
        LegendaryArtifact,
        ReforgedMinor,
        ReforgedLesser,
        ReforgedGreater,
        ReforgedMajor,
        ReforgedLegendary
    }

    public static class RunicReforging
    {
        public static bool CanReforge(Mobile from, Item item, CraftSystem crsystem)
        {
            CraftItem crItem = null;
            bool allowableSpecial = m_AllowableTable.ContainsKey(item.GetType());

            if (!allowableSpecial)
            {
                foreach (CraftSystem system in CraftSystem.Systems)
                {
                    if (system == crsystem && system != null && system.CraftItems != null)
                        crItem = system.CraftItems.SearchFor(item.GetType());

                    if (crItem != null)
                        break;

                }
            }

            if (crItem == null && !allowableSpecial)
            {
                from.SendLocalizedMessage(1152279); // You cannot re-forge that item with this tool.
                return false;
            }

            bool goodtogo = true;
            int mods = GetTotalMods(item);
            int maxmods = item is JukaBow || 
                (item is BaseWeapon && !((BaseWeapon)item).DImodded) || 
                (item is BaseArmor && ((BaseArmor)item).ArmorAttributes.MageArmor > 0 && BaseArmor.IsMageArmorType((BaseArmor)item)) ? 1 : 0;

            if (mods > maxmods)
                goodtogo = false;
            else if(m_AllowableTable.ContainsKey(item.GetType()) && m_AllowableTable[item.GetType()] != crsystem)
                goodtogo = false;
            else if (item is IResource && !CraftResources.IsStandard(((IResource)item).Resource))
                goodtogo = false;
            else if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied)
                goodtogo = false;
            else if (item is BaseWeapon && Server.Spells.Mysticism.EnchantSpell.IsUnderSpellEffects(from, (BaseWeapon)item))
                goodtogo = false;
            else if (item is BaseWeapon && ((BaseWeapon)item).FocusWeilder != null)
                goodtogo = false;
            else if (!allowableSpecial && ((item is BaseWeapon && !((BaseWeapon)item).PlayerConstructed) || (item is BaseArmor && !((BaseArmor)item).PlayerConstructed)))
                goodtogo = false;
            else if (!allowableSpecial && item is BaseClothing && !(item is BaseHat))
                goodtogo = false;
            else if (Imbuing.IsInNonImbueList(item.GetType()))
                goodtogo = false;

            if (!goodtogo)
                from.SendLocalizedMessage(1152113); // You cannot reforge that item.

            return goodtogo;
        }

        public static void ApplyReforgedProperties(Item item, ReforgedPrefix prefix, ReforgedSuffix suffix, bool playermade, int budget, int perclow, int perchigh, int maxmods)
        {
            ApplyReforgedProperties(item, prefix, suffix, playermade, budget, perclow, perchigh, maxmods, 0);
        }

        public static void ApplyReforgedProperties(Item item, ReforgedPrefix prefix, ReforgedSuffix suffix, bool playermade, int budget, int perclow, int perchigh, int maxmods, int luckchance, BaseRunicTool tool = null, ReforgingOption option = ReforgingOption.None)
        {
            if (prefix == ReforgedPrefix.None && (suffix == ReforgedSuffix.None || suffix > ReforgedSuffix.Aegis))
            {
                for (int i = 0; i < maxmods; i++)
                    ApplyRunicAttributes(item, perclow, perchigh, ref budget, i, luckchance, tool != null);

                if (suffix != ReforgedSuffix.None)
                    ApplySuffixName(item, suffix);
            }
            else
            {
                int prefixID = (int)prefix;
                int suffixID = (int)suffix;

                int index = GetCollectionIndex(item);
                int resIndex = -1;
                int preIndex = -1;

                if (tool != null)
                {
                    resIndex = GetResourceIndex(tool.Resource);
                    preIndex = GetPrerequisiteIndex(option);
                }

                if (index == -1)
                    return;

                List<NamedInfoCol> prefixCol = null;
                List<NamedInfoCol> suffixCol = null;

                if (prefix != ReforgedPrefix.None)
                {
                    try
                    {
                        prefixCol = new List<NamedInfoCol>();
                        prefixCol.AddRange(m_PrefixSuffixInfo[prefixID][index]);
                    }
                    catch
                    {
                        Console.WriteLine("Error: Prefix not in collection: {0}", prefixID);
                    }
                }

                if (suffix != ReforgedSuffix.None)
                {
                    if (suffixCol == null)
                        suffixCol = new List<NamedInfoCol>();

                    try
                    {
                        suffixCol.AddRange(m_PrefixSuffixInfo[suffixID][index]);
                    }
                    catch
                    {
                        Console.WriteLine("Error: Suffix not in collection: {0}", suffixID);
                    }
                }

                //Removes things like blood drinking/balanced/splintering
                CheckAttributes(item, prefixCol, playermade);
                CheckAttributes(item, suffixCol, playermade);

                int i = 0;
                int mods = 0;

                if (prefix != ReforgedPrefix.None && suffix == ReforgedSuffix.None && prefixCol != null)
                {
                    int specialAdd = 0;
                    int nothing = 0;
                    GetNamedModCount(index, prefixID, 0, maxmods, prefixCol.Count, 0, ref specialAdd, ref nothing);

                    while (budget > 25 && mods < maxmods && i < 25)
                    {
                        if (prefixCol.Count > 0 && specialAdd > 0) 
                        {
                            int random = Utility.Random(prefixCol.Count);
                            if (ApplyPrefixSuffixAttribute(item, prefixCol[random].Attribute, prefixCol[random].Min(resIndex, preIndex, item), prefixCol[random].Max(resIndex, preIndex, item), perclow, perchigh, ref budget, luckchance, playermade))
                            {
                                specialAdd--;
                                mods++;
                            }

                            prefixCol.RemoveAt(random);
                        }
                        else if (((playermade || Utility.RandomBool()) && ApplyNewAttributes(item, prefixID, suffixID, index, perclow, perchigh, resIndex, preIndex, luckchance, playermade, ref budget)) ||
                            ApplyRunicAttributes(item, perclow, perchigh, ref budget, i, luckchance, playermade))
                        {
                            mods++;
                        }

                        i++;
                    }

                    if (prefix != ReforgedPrefix.None)
                        ApplyPrefixName(item, prefix);
                }
                else if (prefix == ReforgedPrefix.None && suffix != ReforgedSuffix.None && suffixCol != null)
                {
                    int specialAdd = 0;
                    int nothing = 0;
                    GetNamedModCount(index, 0, suffixID, maxmods, 0, suffixCol.Count, ref nothing, ref specialAdd);

                    while (budget > 25 && mods < maxmods && i < 25)
                    {
                        if (suffixCol.Count > 0 && specialAdd > 0)
                        {
                            int random = Utility.Random(suffixCol.Count);
                            if (ApplyPrefixSuffixAttribute(item, suffixCol[random].Attribute, suffixCol[random].Min(resIndex, preIndex, item), suffixCol[random].Max(resIndex, preIndex, item), perclow, perchigh, ref budget, luckchance, playermade))
                            {
                                specialAdd--;
                                mods++;
                            }

                            suffixCol.RemoveAt(random);
                        }
                        else if (((playermade || Utility.RandomBool()) && ApplyNewAttributes(item, prefixID, suffixID, index, perclow, perchigh, resIndex, preIndex, luckchance, playermade, ref budget)) ||
                            ApplyRunicAttributes(item, perclow, perchigh, ref budget, i, luckchance, playermade))
                        {
                            mods++;
                        }

                        i++;
                    }

                    if (suffix != ReforgedSuffix.None)
                        ApplySuffixName(item, suffix);
                }
                else if (prefix != ReforgedPrefix.None && suffix != ReforgedSuffix.None && prefixCol != null && suffixCol != null)
                {
                    int specialAddPrefix = 0;
                    int specialAddSuffix = 0;

                    GetNamedModCount(index, prefixID, suffixID, maxmods, prefixCol.Count, suffixCol.Count, ref specialAddPrefix, ref specialAddSuffix);

                    while (budget > 25 && mods < maxmods && i < 25)
                    {
                        if (prefixCol.Count > 0 && specialAddPrefix > 0)
                        {
                            int random = Utility.Random(prefixCol.Count);
                            if (ApplyPrefixSuffixAttribute(item, prefixCol[random].Attribute, prefixCol[random].Min(resIndex, preIndex, item), prefixCol[random].Max(resIndex, preIndex, item), perclow, perchigh, ref budget, luckchance, playermade))
                            {
                                specialAddPrefix--;
                                mods++;
                            }

                            prefixCol.RemoveAt(random);
                        }
                        else if (suffixCol.Count > 0 && specialAddSuffix > 0)
                        {
                            int random = Utility.Random(suffixCol.Count);
                            if (ApplyPrefixSuffixAttribute(item, suffixCol[random].Attribute, suffixCol[random].Min(resIndex, preIndex, item), suffixCol[random].Max(resIndex, preIndex, item), perclow, perchigh, ref budget, luckchance, playermade))
                            {
                                specialAddSuffix--;
                                mods++;
                            }
                            suffixCol.RemoveAt(random);
                        }
                        else if (((playermade || Utility.RandomBool()) && ApplyNewAttributes(item, prefixID, suffixID, index, perclow, perchigh, resIndex, preIndex, luckchance, playermade, ref budget)) ||
                            ApplyRunicAttributes(item, perclow, perchigh, ref budget, i, luckchance, playermade))
                        {
                            mods++;
                        }

                        i++;
                    }

                    if (prefix != ReforgedPrefix.None)
                        ApplyPrefixName(item, prefix);

                    if (suffix != ReforgedSuffix.None)
                        ApplySuffixName(item, suffix);
                }

                if (_Elements.ContainsKey(item))
                    _Elements.Remove(item);
            }
        }

        public static bool HasSelection(int index, Item toreforge, BaseRunicTool tool, ReforgingOption options, int prefix, int suffix)
        {
            // No Vampire prefix/suffix for non-weapons
            if (index == 6 && !(toreforge is BaseWeapon))
                return false;

            // Cannot choose same suffix/prefix
            if (index != 0 && (index == prefix || index == suffix))
                return false;

            switch (tool.Resource)
            {
                default:
                case CraftResource.DullCopper:
                    {
                        if ((index == 10 || index == 11) && (options & ReforgingOption.Powerful) != 0 &&
                                                            (options & ReforgingOption.Fundamental) != 0)
                            return false;
                    }
                    break;
                case CraftResource.ShadowIron:
                case CraftResource.SpinedLeather:
                case CraftResource.OakWood:
                    {
                        if ((index == 10 || index == 11) && ((options & ReforgingOption.Structural) != 0 ||
                                                             (options & ReforgingOption.Fundamental) != 0))
                            return false;

                        if (index == 5 && (options & ReforgingOption.Powerful) != 0 &&
                                          (options & ReforgingOption.Fundamental) != 0)
                            return false;

                        return true;
                    }
                case CraftResource.Copper:
                case CraftResource.HornedLeather:
                case CraftResource.AshWood:
                    {
                        if (index == 10 || index == 11)
                            return false;

                        if (index == 5 && ((options & ReforgingOption.Structural) != 0 ||
                                           (options & ReforgingOption.Fundamental) != 0))
                            return false;

                        if (index == 9 && (options & ReforgingOption.Fundamental) != 0)
                            return false;

                        if (index == 12 && tool.Resource == CraftResource.Copper && (options & ReforgingOption.Structural) != 0 &&
                                                                                    (options & ReforgingOption.Fundamental) != 0)
                            return false;

                        if (index == 12 && (options & ReforgingOption.Structural) != 0 &&
                                           (options & ReforgingOption.Fundamental) != 0)
                            return false;
                    }
                    break;
                case CraftResource.Bronze:
                case CraftResource.Gold:
                    {
                        if (index == 10 || index == 11)
                            return false;

                        if ((index == 5 || index == 9) && (options & ReforgingOption.Powerful) != 0 &&
                                                          (options & ReforgingOption.Structural) != 0)
                            return false;

                        if (index == 5 && (options & ReforgingOption.Structural) != 0)
                            return false;

                        if (index == 12 &&  (options & ReforgingOption.Structural) != 0 &&
                                            (options & ReforgingOption.Fundamental) != 0)
                            return false;
                    }
                    break;
                case CraftResource.Agapite:
                case CraftResource.YewWood:
                    {
                        if (index == 9 || index == 10 || index == 11)
                            return false;

                        if (index == 12 && (options & ReforgingOption.Powerful) != 0)
                            return false;

                        if ((index == 5 || index == 12) && (options & ReforgingOption.Structural) != 0)
                            return false;
                    }
                    break;
                case CraftResource.Heartwood:
                case CraftResource.Verite:
                case CraftResource.BarbedLeather:
                case CraftResource.Valorite:
                    {
                        if (index == 9 || index == 10 || index == 11 || index == 12)
                            return false;

                        if (index == 5 && (options & ReforgingOption.Structural) != 0)
                            return false;
                    }
                    break;
            }

            return true;
        }

        private static void CheckAttributes(Item item, List<NamedInfoCol> list, bool playermade)
        {
            if (list == null || list.Count == 0)
                return;

            List<NamedInfoCol> copy = new List<NamedInfoCol>(list);
            for (int c = 0; c < copy.Count; c++)
            {
                NamedInfoCol col = copy[c];

                if (col == null) continue;

                if (list.Contains(col) && col.Attribute is AosWeaponAttribute && (AosWeaponAttribute)col.Attribute == AosWeaponAttribute.BloodDrinker)
                {
                    if (!(item is BaseWeapon) || (((BaseWeapon)item).PrimaryAbility != WeaponAbility.BleedAttack && ((BaseWeapon)item).SecondaryAbility != WeaponAbility.BleedAttack))
                        list.Remove(col);
                }
                else if (list.Contains(col) && col.Attribute is AosWeaponAttribute && (AosWeaponAttribute)col.Attribute == AosWeaponAttribute.SplinteringWeapon)
                {
                    if (playermade || item is BaseRanged)
                        list.Remove(col);
                }
                else if (list.Contains(col) && col.Attribute is AosWeaponAttribute && (AosWeaponAttribute)col.Attribute == AosWeaponAttribute.ReactiveParalyze)
                {
                    if (!(item is BaseWeapon && item is BaseShield) && item.Layer != Layer.TwoHanded)
                        list.Remove(col);
                }
                else if (list.Contains(col) && col.Attribute is AosArmorAttribute && (AosArmorAttribute)col.Attribute == AosArmorAttribute.ReactiveParalyze)
                {
                    if (!(item is BaseWeapon && item is BaseShield) && item.Layer != Layer.TwoHanded)
                        list.Remove(col);
                }
                else if (list.Contains(col) && col.Attribute is AosAttribute && (AosAttribute)col.Attribute == AosAttribute.BalancedWeapon && (!(item is BaseWeapon) || item.Layer != Layer.TwoHanded))
                {
                    list.Remove(col);
                }
            }
        }

        private static void GetNamedModCount(int itemIndex, int prefixID, int suffixID, int maxmods, int precolcount, int suffixcolcount, ref int prefixCount, ref int suffixCount)
        {
            if (prefixID > 0 && suffixID > 0)
            {
                if (0.5 > Utility.RandomDouble())
                {
                    // Even Split
                    if (0.5 > Utility.RandomDouble())
                    {
                        prefixCount = maxmods / 2;
                        suffixCount = maxmods - prefixCount;
                    }
                    else
                    {
                        suffixCount = maxmods / 2;
                        prefixCount = maxmods - suffixCount;
                    }
                }
                else if (0.5 > Utility.RandomDouble())
                {
                    prefixCount = (maxmods / 2) - 1;
                    suffixCount = maxmods - prefixCount;
                }
                else
                {
                    suffixCount = (maxmods / 2) - 1;
                    prefixCount = maxmods - suffixCount;
                }
            }
            else
            {
                int mods = 0;

                switch (maxmods)
                {
                    default:
                    case 8:
                    case 7: mods = maxmods / 2; break;
                    case 6:
                    case 5:
                    case 4: mods = Utility.RandomBool() ? 2 : 3; break;
                    case 3: mods = Utility.RandomBool() ? 1 : 2; break;
                    case 2:
                    case 1: mods = 1; break;
                }

                if (prefixID > 0)
                    prefixCount = mods;
                else
                    suffixCount = mods;
            }

            if (prefixCount > precolcount)
                prefixCount = precolcount;

            if (suffixCount > suffixcolcount)
                suffixCount = suffixcolcount;
        }

        private static bool ApplyPrefixSuffixAttribute(Item item, object attribute, int min, int max, int percLow, int percHigh, ref int budget, int luckchance, bool playerMade, bool named = true)
		{
            int start = budget;

            if (CheckConflictingNegative(item, attribute))
                return false;

            if(playerMade)
            {
                percLow = 100;
                percHigh = 100;

                min = Utility.RandomMinMax(min, max);
                max = min;
            }

			if(attribute is string)
			{
				string str = attribute as string;
                if (str == "RandomEater" && !HasEater(item) && (item is BaseArmor || item is BaseJewel || item is BaseWeapon))
				{
                    budget -= ApplyRandomEater(item, min, max, percLow, percHigh, budget, luckchance, playerMade);
				}
				else if (str == "HitSpell" && item is BaseWeapon && !HasHitSpell((BaseWeapon)item))
				{
                    budget -= ApplyRandomHitSpell((BaseWeapon)item, min, max, percLow, percHigh, budget, luckchance, playerMade);
				}
                else if (str == "HitArea" && item is BaseWeapon && !HasHitArea((BaseWeapon)item))
				{
                    budget -= ApplyRandomHitArea((BaseWeapon)item, min, max, percLow, percHigh, budget, luckchance, playerMade);
				}
                else if (str == "Slayer" && item is BaseWeapon && ((BaseWeapon)item).Slayer == SlayerName.None)
                {
                    SlayerName name = BaseRunicTool.GetRandomSlayer();
                    int weight = Imbuing.GetIntensityForAttribute(item, name, -1, 1);

                    if (weight <= budget)
                    {
                        ((BaseWeapon)item).Slayer = name;
                        budget -= weight;
                    }
                }
                else if (str == "WeaponVelocity" && item is BaseRanged && ((BaseRanged)item).Velocity == 0)
                {
                    int value = CalculateValue(attribute, min, max, percLow, percHigh, ref budget, luckchance, playerMade);

                    ((BaseRanged)item).Velocity = value;
                    budget -= Imbuing.GetIntensityForAttribute(item, str, -1, value);
                }
			}
			else if (attribute is AosAttribute)
			{
                int value = CalculateValue(attribute, min, max, percLow, percHigh, ref budget, luckchance, playerMade);
                AosAttributes attrs = GetAosAttributes(item);

                if ((AosAttribute)attribute == AosAttribute.BalancedWeapon && (!(item is BaseWeapon) || item.Layer != Layer.TwoHanded))
                {
                    return false;
                }

                bool hasValue;

                if ((AosAttribute)attribute == AosAttribute.CastSpeed && attrs.SpellChanneling > 0)
                    hasValue = attrs.CastSpeed > -1;
                else
                    hasValue = attrs[(AosAttribute)attribute] > 0;

                if (!hasValue)
				{
					attrs[(AosAttribute)attribute] += value;
                    budget -= Imbuing.GetIntensityForAttribute(item, (AosAttribute)attribute, -1, value);

                    if ((AosAttribute)attribute == AosAttribute.SpellChanneling && attrs[AosAttribute.CastSpeed] > -1)
                        attrs[AosAttribute.CastSpeed]--;
				}
			}
			else if (attribute is AosWeaponAttribute)
			{
                AosWeaponAttribute wepattr = (AosWeaponAttribute)attribute;

                if (item is BaseWeapon)
                {
                    if (wepattr == AosWeaponAttribute.HitLeechHits || wepattr == AosWeaponAttribute.HitLeechMana)
                    {
                        max = (int)((double)Imbuing.GetPropRange((BaseWeapon)item, wepattr)[1] * 1.4);
                    }
                    else
                    {
                        if (CheckHitSpell((BaseWeapon)item, wepattr))
                            return false;

                        if (CheckHitArea((BaseWeapon)item, wepattr))
                            return false;
                    }
                }


                int value = CalculateValue(attribute, min, max, percLow, percHigh, ref budget, luckchance, playerMade);
                AosWeaponAttributes attrs = GetAosWeaponAttributes(item);
				
				if(attrs != null && value > 0 && attrs[wepattr] == 0)
				{
					attrs[wepattr] = value;
                    budget -= Imbuing.GetIntensityForAttribute(item, wepattr, -1, value);
				}
			}
            else if (attribute is AosArmorAttribute)
            {
                int value = CalculateValue(attribute, min, max, percLow, percHigh, ref budget, luckchance, playerMade);
                AosArmorAttributes attrs = GetAosArmorAttributes(item);

                if (attrs != null && value > 0 && attrs[(AosArmorAttribute)attribute] == 0)
                {
                    attrs[(AosArmorAttribute)attribute] = value;
                    budget -= Imbuing.GetIntensityForAttribute(item, (AosArmorAttribute)attribute, -1, value);
                }
            }
            else if (attribute is SAAbsorptionAttribute)
            {
                int value = CalculateValue(attribute, min, max, percLow, percHigh, ref budget, luckchance, playerMade);
                SAAbsorptionAttributes attrs = GetSAAbsorptionAttributes(item);

                if (attrs != null && value > 0 && attrs[(SAAbsorptionAttribute)attribute] == 0)
                {
                    attrs[(SAAbsorptionAttribute)attribute] = value;
                    budget -= Imbuing.GetIntensityForAttribute(item, (SAAbsorptionAttribute)attribute, -1, value);
                }
            }
            else if (attribute is AosElementAttribute)
            {
                int value = CalculateValue(attribute, min, max, percLow, percHigh, ref budget, luckchance, playerMade);

                if (value > 0)
                {
                    if (ApplyResistance(item, value, (AosElementAttribute)attribute))
                    {
                        budget -= Imbuing.GetIntensityForAttribute(item, (AosElementAttribute)attribute, -1, value);
                    }
                }
            }

			return start != budget;
		}

        private static Dictionary<Item, int[]> _Elements = new Dictionary<Item, int[]>();

        public static bool ApplyResistance(Item item, int value, AosElementAttribute attribute)
        {
            var resists = GetElementalAttributes(item);

            if (!_Elements.ContainsKey(item))
            {
                if (item is BaseArmor)
                {
                    _Elements[item] = new int[] { ((BaseArmor)item).PhysicalBonus, ((BaseArmor)item).FireBonus, ((BaseArmor)item).ColdBonus, ((BaseArmor)item).PoisonBonus, ((BaseArmor)item).EnergyBonus };
                }
                else if (item is BaseWeapon)
                {
                    _Elements[item] = new int[] { ((BaseWeapon)item).WeaponAttributes.ResistPhysicalBonus, ((BaseWeapon)item).WeaponAttributes.ResistFireBonus,
                            ((BaseWeapon)item).WeaponAttributes.ResistColdBonus, ((BaseWeapon)item).WeaponAttributes.ResistPoisonBonus, ((BaseWeapon)item).WeaponAttributes.ResistEnergyBonus };
                }
                else if (resists != null)
                {
                    _Elements[item] = new int[] { resists[AosElementAttribute.Physical], resists[AosElementAttribute.Fire], resists[AosElementAttribute.Cold], 
                        resists[AosElementAttribute.Poison], resists[AosElementAttribute.Energy] };
                }
                else
                {
                    return false;
                }
            }

            switch (attribute)
            {
                default:
                case AosElementAttribute.Physical:
                    if (item is BaseArmor && (!_Elements.ContainsKey(item) || ((BaseArmor)item).PhysicalBonus == _Elements[item][0]))
                    {
                        ((BaseArmor)item).PhysicalBonus = value;
                        return true;
                    }
                    else if (item is BaseWeapon && (!_Elements.ContainsKey(item) || ((BaseWeapon)item).WeaponAttributes.ResistPhysicalBonus == _Elements[item][0]))
                    {
                        ((BaseWeapon)item).WeaponAttributes.ResistPhysicalBonus = value;
                        return true;
                    }
                    else if (resists != null && (!_Elements.ContainsKey(item) || resists[attribute] == _Elements[item][0]))
                    {
                        resists[attribute] = value;
                        return true;
                    }
                    break;
                case AosElementAttribute.Fire:
                    if (item is BaseArmor && (!_Elements.ContainsKey(item) || ((BaseArmor)item).FireBonus == _Elements[item][1]))
                    {
                        ((BaseArmor)item).FireBonus = value;
                        return true;
                    }
                    else if (item is BaseWeapon && (!_Elements.ContainsKey(item) || ((BaseWeapon)item).WeaponAttributes.ResistFireBonus == _Elements[item][1]))
                    {
                        ((BaseWeapon)item).WeaponAttributes.ResistFireBonus = value;
                        return true;
                    }
                    else if (resists != null && (!_Elements.ContainsKey(item) || resists[attribute] == _Elements[item][1]))
                    {
                        resists[attribute] = value;
                        return true;
                    }
                    break;
                case AosElementAttribute.Cold:
                    if (item is BaseArmor && (!_Elements.ContainsKey(item) || ((BaseArmor)item).ColdBonus == _Elements[item][2]))
                    {
                        ((BaseArmor)item).ColdBonus = value;
                        return true;
                    }
                    else if (item is BaseWeapon && (!_Elements.ContainsKey(item) || ((BaseWeapon)item).WeaponAttributes.ResistColdBonus == _Elements[item][2]))
                    {
                        ((BaseWeapon)item).WeaponAttributes.ResistColdBonus = value;
                        return true;
                    }
                    else if (resists != null && (!_Elements.ContainsKey(item) || resists[attribute] == _Elements[item][2]))
                    {
                        resists[attribute] = value;
                        return true;
                    }
                    break;
                case AosElementAttribute.Poison:
                    if (item is BaseArmor && (!_Elements.ContainsKey(item) || ((BaseArmor)item).PoisonBonus == _Elements[item][3]))
                    {
                        ((BaseArmor)item).PoisonBonus = value;
                        return true;
                    }
                    else if (item is BaseWeapon && (!_Elements.ContainsKey(item) || ((BaseWeapon)item).WeaponAttributes.ResistPoisonBonus == _Elements[item][3]))
                    {
                        ((BaseWeapon)item).WeaponAttributes.ResistPoisonBonus = value;
                        return true;
                    }
                    else if (resists != null && (!_Elements.ContainsKey(item) || resists[attribute] == _Elements[item][3]))
                    {
                        resists[attribute] = value;
                        return true;
                    }
                    break;
                case AosElementAttribute.Energy:
                    if (item is BaseArmor && (!_Elements.ContainsKey(item) || ((BaseArmor)item).EnergyBonus == _Elements[item][4]))
                    {
                        ((BaseArmor)item).EnergyBonus = value;
                        return true;
                    }
                    else if (item is BaseWeapon && (!_Elements.ContainsKey(item) || ((BaseWeapon)item).WeaponAttributes.ResistEnergyBonus == _Elements[item][4]))
                    {
                        ((BaseWeapon)item).WeaponAttributes.ResistEnergyBonus = value;
                        return true;
                    }
                    else if (resists != null && (!_Elements.ContainsKey(item) || resists[attribute] == _Elements[item][4]))
                    {
                        resists[attribute] = value;
                        return true;
                    }
                    break;
            }

            return false;
        }

        public static int Scale(int min, int max, int perclow, int perchigh, int luckchance, bool playerMade)
        {
            int percent;

            if (playerMade)
            {
                percent = Utility.RandomMinMax(perclow, perchigh);
            }
            else
            {
                percent = Utility.RandomMinMax(0, perchigh);
            }

            if (LootPack.CheckLuck(luckchance))
                percent += 10;

            if (percent < perclow) percent = perclow;
            if (percent > perchigh) percent = perchigh;

            int scaledBy = Math.Abs(min - max) + 1;

            if (scaledBy != 0)
                scaledBy = 10000 / scaledBy;

            percent *= (10000 + scaledBy);

            return min + (((max - min) * percent) / 1000001);
        }

        private static int CalculateValue(object attribute, int min, int max, int perclow, int perchigh, ref int budget, int luckchance)
        {
            return CalculateValue(attribute, min, max, perclow, perchigh, ref budget, luckchance, false);
        }

        private static int CalculateValue(object attribute, int min, int max, int perclow, int perchigh, ref int budget, int luckchance, bool playerMade)
		{
            int scale = ScaleAttribute(attribute);
            int value = Scale(min / scale, max / scale, perclow, perchigh, luckchance, playerMade) * scale;
            int totalweight = GetTotalWeight(attribute, value);

            if (value > max) value = max;
            if (value < min) value = min;

			while(budget <= totalweight)
			{
				value -= scale;

                if (value <= 0)
                {
                    if(GetTotalWeight(attribute, 3) > budget)
                        budget = 0;

                    return 0;
                }

                totalweight = GetTotalWeight(attribute, value);
			}
			
			return value;
		}
		
		private static int GetWeight(object attr)
		{
			ImbuingDefinition def = GetImbuingDef(attr);
			
			if(def == null)
				return 0;
				
			return def.Weight;
		}

        private static int GetTotalWeight(object attr, int value)
        {
            ImbuingDefinition def = GetImbuingDef(attr);

            if (def == null)
                return 0;

            return (int)(((double)def.Weight / (double)def.MaxIntensity) * (double)value);
        }

        private static int GetTotalMods(Item item)
        {
            return Imbuing.GetTotalMods(item);
        }

		private static ImbuingDefinition GetImbuingDef(object attr)
		{
			int mod = -1;

            if (attr is AosAttribute)
                mod = Imbuing.GetModForAttribute((AosAttribute)attr);

            else if (attr is AosWeaponAttribute)
                mod = Imbuing.GetModForAttribute((AosWeaponAttribute)attr);

            else if (attr is SkillName)
                mod = Imbuing.GetModForAttribute((SkillName)attr);

            else if (attr is SlayerName)
                mod = Imbuing.GetModForAttribute((SlayerName)attr);

            else if (attr is SAAbsorptionAttribute)
                mod = Imbuing.GetModForAttribute((SAAbsorptionAttribute)attr);

            else if (attr is AosArmorAttribute)
                mod = Imbuing.GetModForAttribute((AosArmorAttribute)attr);

            else if (attr is AosElementAttribute)
                mod = Imbuing.GetModForAttribute((AosElementAttribute)attr);
				
			if(Imbuing.Table.ContainsKey(mod))
				return Imbuing.Table[mod];
				
			return null;
		}
		
		private static int GetCollectionIndex(Item item)
		{
			if(item is BaseWeapon)
				return 0;
			if(item is BaseShield)
				return 2;
			if(item is BaseArmor || item is BaseClothing)
				return 1;
			if(item is BaseJewel)
				return 3;
				
			return -1;
		}

        private static int GetResourceIndex(CraftResource resource)
        {
           // RunicIndex 0 - dullcopper; 1 - shadow; 2 - copper; 3 - spined; 4 - Oak; 5 - ash
            switch (resource)
            {
                default:
                case CraftResource.DullCopper: return 0;
                case CraftResource.ShadowIron: return 1;
                case CraftResource.Bronze:
                case CraftResource.Gold:
                case CraftResource.Agapite:
                case CraftResource.Verite:
                case CraftResource.Valorite:
                case CraftResource.Copper: return 2;
                case CraftResource.HornedLeather:
                case CraftResource.BarbedLeather:
                case CraftResource.SpinedLeather: return 3;
                case CraftResource.OakWood: return 4;
                case CraftResource.YewWood:
                case CraftResource.Heartwood:
                case CraftResource.Bloodwood:
                case CraftResource.Frostwood:
                case CraftResource.AshWood: return 5;
            }
        }

        private static int GetPrerequisiteIndex(ReforgingOption option)
        {
            if ((option & ReforgingOption.Powerful) != 0 &&
                (option & ReforgingOption.Structural) != 0 &&
                (option & ReforgingOption.Fundamental) != 0)
                return 6;

            if ((option & ReforgingOption.Structural) != 0 &&
                (option & ReforgingOption.Fundamental) != 0)
                return 5;

            if ((option & ReforgingOption.Powerful) != 0 &&
                (option & ReforgingOption.Structural) != 0)
                return 4;

            if ((option & ReforgingOption.Fundamental) != 0)
                return 3;

            if ((option & ReforgingOption.Structural) != 0)
                return 2;

            if ((option & ReforgingOption.Powerful) != 0)
                return 1;

            return 0;
        }

        private static int CalculateMinIntensity(int perclow, int perchi, ReforgingOption option)
        {
            if (option == ReforgingOption.None)
                return perclow;

            return perclow + (int)((double)(perchi - perclow) * ((double)(GetPrerequisiteIndex(option) * 5.0) / 100.0));
        }
		
		private static Dictionary<Type, CraftSystem> m_AllowableTable = new Dictionary<Type, CraftSystem>();
		private static Dictionary<int, NamedInfoCol[][]> m_PrefixSuffixInfo = new Dictionary<int, NamedInfoCol[][]>();

        public static void Initialize()
        {
            m_AllowableTable[typeof(BaseGlovesOfMining)] = DefTailoring.CraftSystem;
            m_AllowableTable[typeof(RingmailGlovesOfMining)] = DefTailoring.CraftSystem;
            m_AllowableTable[typeof(StuddedGlovesOfMining)] = DefTailoring.CraftSystem;
            m_AllowableTable[typeof(JukaBow)] = DefBowFletching.CraftSystem;
            m_AllowableTable[typeof(TribalSpear)] = DefBlacksmithy.CraftSystem;
            m_AllowableTable[typeof(Pickaxe)] = DefBlacksmithy.CraftSystem;
            m_AllowableTable[typeof(Cleaver)] = DefBlacksmithy.CraftSystem;
            m_AllowableTable[typeof(SkinningKnife)] = DefBlacksmithy.CraftSystem;
            m_AllowableTable[typeof(ButcherKnife)] = DefBlacksmithy.CraftSystem;
            m_AllowableTable[typeof(GargishNecklace)] = DefBlacksmithy.CraftSystem;
            m_AllowableTable[typeof(GargishEarrings)] = DefBlacksmithy.CraftSystem;

            m_AllowableTable[typeof(GargishAmulet)] = DefBlacksmithy.CraftSystem;
            m_AllowableTable[typeof(GargishStoneAmulet)] = DefMasonry.CraftSystem;
        }

        public static void Configure()
        {
            Server.Commands.CommandSystem.Register("GetCreatureScore", AccessLevel.GameMaster, e =>
                {
                    e.Mobile.BeginTarget(12, false, Server.Targeting.TargetFlags.None, (from, targeted) =>
                        {
                            if (targeted is BaseCreature)
                            {
                                ((BaseCreature)targeted).PrivateOverheadMessage(Server.Network.MessageType.Regular, 0x25, false, GetDifficultyFor((BaseCreature)targeted).ToString(), e.Mobile.NetState);
                            }
                        });
                });

            m_MeleeWeaponList = new List<object>();
            m_RangedWeaponList = new List<object>();
            m_ArmorList = new List<object>();
            m_JewelList = new List<object>();
            m_ShieldList = new List<object>();

            m_MeleeWeaponList.AddRange(m_WeaponBasic);
            m_MeleeWeaponList.AddRange(m_MeleeStandard);

            m_RangedWeaponList.AddRange(m_WeaponBasic);
            m_RangedWeaponList.AddRange(m_RangedStandard);

            m_ArmorList.AddRange(m_ArmorStandard);
            m_JewelList.AddRange(m_JewelStandard);
            m_ShieldList.AddRange(m_ShieldStandard);

			// TypeIndex 0 - Weapon; 1 - Armor; 2 - Shield; 3 - Jewels
            // RunicIndex 0 - dullcopper; 1 - shadow; 2 - copper; 3 - spined; 4 - Oak; 5 - ash

			m_PrefixSuffixInfo[0] = null;
			m_PrefixSuffixInfo[1] = new NamedInfoCol[][] 	//Might
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLeechHits, HitsAndManaLeechTable),
                        new NamedInfoCol(AosAttribute.BonusHits, WeaponHitsTable),
                        new NamedInfoCol(AosAttribute.BonusStr, WeaponStrTable),
                        new NamedInfoCol(AosAttribute.RegenHits, WeaponRegenTable),
                    },

                    new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol("RandomEater", EaterTable),
                        new NamedInfoCol(AosAttribute.BonusHits, ArmorHitsTable),
                        new NamedInfoCol(AosAttribute.BonusStr, ArmorStrTable),
                        new NamedInfoCol(AosAttribute.RegenHits, ArmorRegenTable),
                    },
					
                    new NamedInfoCol[] // shield
                    {
                        new NamedInfoCol("RandomEater", EaterTable),
                        new NamedInfoCol(AosAttribute.BonusHits, ArmorHitsTable),
                        new NamedInfoCol(AosAttribute.BonusStr, ArmorStrTable),
                        new NamedInfoCol(AosAttribute.RegenHits, ArmorRegenTable),
                    },

                    new NamedInfoCol[] // jewels
                    {
                        new NamedInfoCol(AosAttribute.BonusHits, ArmorHitsTable),
                        new NamedInfoCol(AosAttribute.BonusStr, ArmorStrTable),
                    }
				};
				
			m_PrefixSuffixInfo[2] = new NamedInfoCol[][] 	//Mystic
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLeechMana, HitsAndManaLeechTable),
                        new NamedInfoCol(AosAttribute.BonusMana, WeaponStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                        new NamedInfoCol(AosAttribute.LowerManaCost, WeaponStamManaLMCTable),
                        /*new NamedInfoCol(AosAttribute.LowerRegCost, LowerRegTable), */
                    },
                    new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol(AosAttribute.LowerRegCost, LowerRegTable),
                        new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                        new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                        new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.LowerRegCost, LowerRegTable),
                    },
				};
				
			m_PrefixSuffixInfo[3] = new NamedInfoCol[][]	// Animated
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLeechStam, HitStamLeechTable),
                        new NamedInfoCol(AosAttribute.BonusStam, WeaponStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.BonusDex, DexIntTable),
                        new NamedInfoCol(AosAttribute.RegenStam, WeaponRegenTable),
                        new NamedInfoCol(AosAttribute.WeaponSpeed, WeaponWeaponSpeedTable),
                    },
                    new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol(AosAttribute.BonusStam, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.BonusDex, DexIntTable),
                        new NamedInfoCol(AosAttribute.RegenStam, ArmorRegenTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusStam, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.BonusDex, DexIntTable),
                        new NamedInfoCol(AosAttribute.RegenStam, ArmorRegenTable),
                        new NamedInfoCol(AosAttribute.WeaponSpeed, ShieldWeaponSpeedTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusStam, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.BonusDex, DexIntTable),
                        new NamedInfoCol(AosAttribute.WeaponSpeed, ShieldWeaponSpeedTable),
                    },
				};
			m_PrefixSuffixInfo[4] = new NamedInfoCol[][]	//Arcane
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLeechMana, HitsAndManaLeechTable),
                        new NamedInfoCol(AosWeaponAttribute.HitManaDrain, HitWeaponTable2),
                        new NamedInfoCol(AosAttribute.BonusMana, WeaponStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                        new NamedInfoCol(AosAttribute.LowerManaCost, WeaponStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.CastSpeed, 1),
                        new NamedInfoCol(AosAttribute.SpellChanneling, 1),
                        new NamedInfoCol(AosWeaponAttribute.MageWeapon, MageWeaponTable),
                        new NamedInfoCol(AosAttribute.RegenMana, WeaponRegenTable),
                    },
                    new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                        new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
                        new NamedInfoCol(AosAttribute.LowerRegCost, LowerRegTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                        new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusMana, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
                        new NamedInfoCol(AosAttribute.LowerManaCost, ArmorStamManaLMCTable),
                        new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
                        new NamedInfoCol(AosAttribute.LowerRegCost, LowerRegTable),
                        new NamedInfoCol(AosAttribute.CastSpeed, 1),
                        new NamedInfoCol(AosAttribute.CastRecovery, 4),
                        new NamedInfoCol(AosAttribute.SpellDamage, 15),
                    },
				};
			m_PrefixSuffixInfo[5] = new NamedInfoCol[][]	// Exquisite
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosWeaponAttribute.SelfRepair, SelfRepairTable),
                        new NamedInfoCol(AosWeaponAttribute.DurabilityBonus, DurabilityTable),
                        new NamedInfoCol(AosWeaponAttribute.HitLowerDefend, HitWeaponTable2),
                        new NamedInfoCol(AosWeaponAttribute.LowerStatReq, LowerStatReqTable),
                        new NamedInfoCol("Slayer", 1),
                        new NamedInfoCol(AosWeaponAttribute.MageWeapon, MageWeaponTable),
                        new NamedInfoCol(AosAttribute.SpellChanneling, 1),
                        new NamedInfoCol(AosAttribute.BalancedWeapon, 1),
                        new NamedInfoCol("WeaponVelocity", WeaponVelocityTable),
                    },
                    new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol(AosArmorAttribute.SelfRepair, SelfRepairTable),
                        new NamedInfoCol(AosArmorAttribute.DurabilityBonus, DurabilityTable),
                        new NamedInfoCol(AosArmorAttribute.LowerStatReq, LowerStatReqTable),
                    },
                    new NamedInfoCol[] // shield
                    {
                        new NamedInfoCol(AosArmorAttribute.SelfRepair, SelfRepairTable),
                        new NamedInfoCol(AosArmorAttribute.DurabilityBonus, DurabilityTable),
                        new NamedInfoCol(AosArmorAttribute.LowerStatReq, LowerStatReqTable),
                    },
                    new NamedInfoCol[]
                    {
                    },
				};
			m_PrefixSuffixInfo[6] = new NamedInfoCol[][]	//Vampiric
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLeechHits, HitsAndManaLeechTable),
                        new NamedInfoCol(AosWeaponAttribute.HitLeechStam, HitStamLeechTable),
                        new NamedInfoCol(AosWeaponAttribute.HitLeechMana, HitsAndManaLeechTable),
                        new NamedInfoCol(AosWeaponAttribute.HitManaDrain, HitWeaponTable2),
                        new NamedInfoCol(AosWeaponAttribute.HitFatigue, HitWeaponTable2),
                        new NamedInfoCol(AosWeaponAttribute.BloodDrinker, 1),
                    },
                    new NamedInfoCol[] // armor
                    {
                    },
                    new NamedInfoCol[]
                    {
                    },
                    new NamedInfoCol[]
                    {
                    },
				};
			m_PrefixSuffixInfo[7] = new NamedInfoCol[][]	// Invigorating
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosAttribute.RegenHits, WeaponRegenTable),
                        new NamedInfoCol(AosAttribute.RegenStam, WeaponRegenTable),
                        new NamedInfoCol(AosAttribute.RegenMana, WeaponRegenTable),
                    },
                    new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol(AosAttribute.RegenHits, ArmorRegenTable),
                        new NamedInfoCol(AosAttribute.RegenStam, ArmorRegenTable),
                        new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),

                        new NamedInfoCol("RandomEater",  EaterTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.RegenHits, ArmorRegenTable),
                        new NamedInfoCol(AosAttribute.RegenStam, ArmorRegenTable),
                        new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
                        new NamedInfoCol(AosArmorAttribute.SoulCharge, ShieldSoulChargeTable),
                        new NamedInfoCol("RandomEater", EaterTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.RegenHits, ArmorRegenTable),
                        new NamedInfoCol(AosAttribute.RegenStam, ArmorRegenTable),
                        new NamedInfoCol(AosAttribute.RegenMana, ArmorRegenTable),
                    },
				};
			m_PrefixSuffixInfo[8] = new NamedInfoCol[][]	// Fortified
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosWeaponAttribute.ResistPhysicalBonus, ResistTable),
                        new NamedInfoCol(AosWeaponAttribute.ResistFireBonus, ResistTable),
                        new NamedInfoCol(AosWeaponAttribute.ResistColdBonus, ResistTable),
                        new NamedInfoCol(AosWeaponAttribute.ResistPoisonBonus, ResistTable),
                        new NamedInfoCol(AosWeaponAttribute.ResistEnergyBonus, ResistTable),
                    },
                    new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol(AosElementAttribute.Physical, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Fire, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Cold, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Poison, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Energy, ResistTable),
                        new NamedInfoCol("RandomEater", EaterTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosElementAttribute.Physical, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Fire, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Cold, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Poison, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Energy, ResistTable),
                        new NamedInfoCol("RandomEater", EaterTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosElementAttribute.Physical, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Fire, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Cold, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Poison, ResistTable),
                        new NamedInfoCol(AosElementAttribute.Energy, ResistTable),
                    },
				};
			m_PrefixSuffixInfo[9] = new NamedInfoCol[][]	// Auspicious
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosAttribute.Luck, LuckTable, RangedLuckTable),
                    },
					new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol(AosAttribute.Luck, LuckTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.Luck, LuckTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.Luck, LuckTable),
                    },
				};
			m_PrefixSuffixInfo[10] = new NamedInfoCol[][]	// Charmed
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosAttribute.EnhancePotions, WeaponEnhancePots),
                        new NamedInfoCol(AosAttribute.BalancedWeapon, 1),
                    },
                    new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol(AosAttribute.EnhancePotions, ArmorEnhancePotsTable),
                    },
                    new NamedInfoCol[]
                    {
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.EnhancePotions, ArmorEnhancePotsTable),
                    },
				};
			m_PrefixSuffixInfo[11] = new NamedInfoCol[][]	//Vicious
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol("HitSpell", HitWeaponTable1),
                        new NamedInfoCol("HitArea", HitWeaponTable1),
                        new NamedInfoCol(AosAttribute.AttackChance, WeaponHCITable, RangedHCITable),
                        new NamedInfoCol(AosAttribute.WeaponDamage, WeaponDamageTable),
                        new NamedInfoCol(AosWeaponAttribute.BattleLust, 1),
                        new NamedInfoCol(AosWeaponAttribute.SplinteringWeapon, 30),
                        new NamedInfoCol("Slayer", 1),
                    },
                    new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol(AosAttribute.AttackChance, ArmorHCIDCITable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.AttackChance, WeaponHCITable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.AttackChance, WeaponHCITable),
                        new NamedInfoCol(AosAttribute.SpellDamage, 15),
                    }, 
				};
			m_PrefixSuffixInfo[12] = new NamedInfoCol[][]	// Towering
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLowerAttack, HitWeaponTable1),
                        new NamedInfoCol(AosWeaponAttribute.ReactiveParalyze, 1),
                        new NamedInfoCol(AosAttribute.DefendChance, WeaponDCITable, RangedDCITable),
                    },
                    new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol(AosAttribute.DefendChance, ArmorHCIDCITable),
                        new NamedInfoCol(SAAbsorptionAttribute.CastingFocus, ArmorCastingFocusTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.DefendChance, WeaponDCITable),
                        new NamedInfoCol(AosArmorAttribute.ReactiveParalyze, 1),
                        new NamedInfoCol(AosArmorAttribute.SoulCharge, ShieldSoulChargeTable),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.DefendChance, ArmorHCIDCITable),
                        //new NamedInfoCol(SAAbsorptionAttribute.CastingFocus, ArmorCastingFocusTable),
                    },
				};
        }

        public class NamedInfoCol
        {
            public object Attribute { get; private set; }
            public int[][] Info { get; private set; }
            public int[][] SecondaryInfo { get; private set; }

            public int HardCap { get; set; }

            public NamedInfoCol(object attr, int[][] info, int[][] secondary = null)
            {
                this.Attribute = attr;
                Info = info;
                SecondaryInfo = secondary;
            }

            public NamedInfoCol(object attr, int hardcap)
            {
                this.Attribute = attr;
                HardCap = hardcap;
            }

            public int Min(int resIndex, int preIndex, Item item, bool random = false)
            {
                if (HardCap == 1)
                    return 1;

                int max = Max(resIndex, preIndex, item);

                if (resIndex != -1 && preIndex != -1)
                {
                    double mod = random ? .66 : .8;

                    return (int)((double)max * mod);
                }

                return (int)((double)max * .5);
            }

            public int Max(int resIndex, int preIndex, Item item)
            {
                int[][] info = item is BaseRanged && SecondaryInfo != null ? SecondaryInfo : Info;

                if (resIndex != -1 && preIndex != -1)
                {
                    if (item is BaseWeapon && this.Attribute is AosWeaponAttribute && ((AosWeaponAttribute)this.Attribute == AosWeaponAttribute.HitLeechHits
                                                            || (AosWeaponAttribute)this.Attribute == AosWeaponAttribute.HitLeechMana))
                    {
                        int weight = Info[resIndex][preIndex];
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

        private static int ApplyRandomHitSpell(BaseWeapon weapon, int min, int max, int perclow, int perchigh, int budget, int luckchance, bool playerMade)
        {
            object attr;

            switch (Utility.Random(5))
            {
                default:
                case 0: attr = AosWeaponAttribute.HitMagicArrow; break;
                case 1: attr = AosWeaponAttribute.HitFireball; break;
                case 2: attr = AosWeaponAttribute.HitHarm; break;
                case 3: attr = AosWeaponAttribute.HitLightning; break;
                case 4: attr = AosWeaponAttribute.HitCurse; break;
            }

            int value = CalculateValue(attr, min, max, perclow, perchigh, ref budget, luckchance, playerMade);
            weapon.WeaponAttributes[(AosWeaponAttribute)attr] = value;

			return (140 / 50) * value;
        }

        private static int ApplyRandomHitArea(BaseWeapon weapon, int min, int max, int perclow, int perchigh, int budget, int luckchance, bool playerMade)
        {
            object attr;

            switch (Utility.Random(5))
            {
                default:
                case 0: attr = AosWeaponAttribute.HitPhysicalArea; break;
                case 1: attr = AosWeaponAttribute.HitFireArea; break;
                case 2: attr = AosWeaponAttribute.HitColdArea; break;
                case 3: attr = AosWeaponAttribute.HitPoisonArea; break;
                case 4: attr = AosWeaponAttribute.HitEnergyArea; break;
            }

            int value = CalculateValue(attr, min, max, perclow, perchigh, ref budget, luckchance, playerMade);
            weapon.WeaponAttributes[(AosWeaponAttribute)attr] = value;

            return (100 / 50) * value;
        }

        private static int ApplyRandomEater(Item item, int min, int max, int perclow, int perchigh, int budget, int luckchance, bool playerMade)
        {
            object attr;

            switch (Utility.Random(5))
            {
                default:
                case 0: attr = SAAbsorptionAttribute.EaterKinetic; break;
                case 1: attr = SAAbsorptionAttribute.EaterFire; break;
                case 2: attr = SAAbsorptionAttribute.EaterCold; break;
                case 3: attr = SAAbsorptionAttribute.EaterPoison; break;
                case 4: attr = SAAbsorptionAttribute.EaterEnergy; break;
            }

            int value = CalculateValue(attr, min, max, perclow, perchigh, ref budget, luckchance, playerMade);

            if(item is BaseWeapon)
                ((BaseWeapon)item).AbsorptionAttributes[(SAAbsorptionAttribute)attr] = value;
            else if (item is BaseArmor)
                ((BaseArmor)item).AbsorptionAttributes[(SAAbsorptionAttribute)attr] = value;
            else if (item is BaseJewel)
                ((BaseJewel)item).AbsorptionAttributes[(SAAbsorptionAttribute)attr] = value;

            return (140 / 12) * value;
        }

        private static bool HasHitSpell(BaseWeapon wep)
        {
            return wep.WeaponAttributes.HitFireball > 0 || wep.WeaponAttributes.HitLightning > 0 || wep.WeaponAttributes.HitMagicArrow > 0
                || wep.WeaponAttributes.HitCurse > 0 || wep.WeaponAttributes.HitHarm > 0;
        }

        private static bool HasHitArea(BaseWeapon wep)
        {
            return wep.WeaponAttributes.HitPhysicalArea > 0 || wep.WeaponAttributes.HitFireArea > 0 || wep.WeaponAttributes.HitColdArea > 0
                || wep.WeaponAttributes.HitPoisonArea > 0 || wep.WeaponAttributes.HitEnergyArea> 0;
        }

        private static bool HasEater(Item item)
        {
            SAAbsorptionAttributes attr = GetSAAbsorptionAttributes(item);

            return attr != null && (attr.EaterKinetic > 0 || attr.EaterFire > 0 || attr.EaterCold > 0 || attr.EaterPoison > 0 || attr.EaterEnergy > 0 || attr.EaterDamage > 0);
        }

        private static bool CheckHitSpell(BaseWeapon wep, object attr)
        {
            return HasHitSpell(wep) && IsHitSpell(attr);
        }

        private static bool CheckHitArea(BaseWeapon wep, object attr)
        {
            return HasHitArea(wep) && IsHitArea(attr);
        }

        private static bool CheckEater(Item item, object attr)
        {
            return HasEater(item) && IsEater(attr);
        }

        private static bool IsHitSpell(object attr)
        {
            return attr is AosWeaponAttribute && ((AosWeaponAttribute)attr == AosWeaponAttribute.HitFireball || (AosWeaponAttribute)attr == AosWeaponAttribute.HitLightning || (AosWeaponAttribute)attr == AosWeaponAttribute.HitMagicArrow
               || (AosWeaponAttribute)attr == AosWeaponAttribute.HitHarm || (AosWeaponAttribute)attr == AosWeaponAttribute.HitHarm);
        }

        private static bool IsHitArea(object attr)
        {
            return attr is AosWeaponAttribute && ((AosWeaponAttribute)attr == AosWeaponAttribute.HitPhysicalArea || (AosWeaponAttribute)attr == AosWeaponAttribute.HitFireArea || (AosWeaponAttribute)attr == AosWeaponAttribute.HitColdArea
                || (AosWeaponAttribute)attr == AosWeaponAttribute.HitPoisonArea || (AosWeaponAttribute)attr == AosWeaponAttribute.HitEnergyArea);
        }

        private static bool IsEater(object attr)
        {
            return attr is SAAbsorptionAttribute && ((SAAbsorptionAttribute)attr == SAAbsorptionAttribute.EaterKinetic || (SAAbsorptionAttribute)attr == SAAbsorptionAttribute.EaterFire || (SAAbsorptionAttribute)attr == SAAbsorptionAttribute.EaterCold
                || (SAAbsorptionAttribute)attr == SAAbsorptionAttribute.EaterPoison || (SAAbsorptionAttribute)attr == SAAbsorptionAttribute.EaterEnergy || (SAAbsorptionAttribute)attr == SAAbsorptionAttribute.EaterDamage);
        }

        public static int GetName(int value)
        {
            switch (value)
            {
                default:
                case 0: return 1062648;
                case 1: return 1151717;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12: return 1151706 + (value - 2);
            }
        }

        public static void ApplyPrefixName(Item item, ReforgedPrefix prefix)
        {
            if (item is BaseWeapon)
                ((BaseWeapon)item).ReforgedPrefix = prefix;

            else if (item is BaseShield)
                ((BaseShield)item).ReforgedPrefix = prefix;

            else if (item is BaseArmor)
                ((BaseArmor)item).ReforgedPrefix = prefix;

            else if (item is BaseJewel)
                ((BaseJewel)item).ReforgedPrefix = prefix;

            else if (item is BaseClothing)
                ((BaseClothing)item).ReforgedPrefix = prefix;
        }

        public static void ApplySuffixName(Item item, ReforgedSuffix suffix)
        {
            if (item is BaseWeapon)
                ((BaseWeapon)item).ReforgedSuffix = suffix;

            else if (item is BaseShield)
                ((BaseShield)item).ReforgedSuffix = suffix;

            else if (item is BaseArmor)
                ((BaseArmor)item).ReforgedSuffix = suffix;
				
            else if (item is BaseJewel)
                ((BaseJewel)item).ReforgedSuffix = suffix;

            else if (item is BaseClothing)
                ((BaseClothing)item).ReforgedSuffix = suffix;
        }

        public static int GetPrefixName(ReforgedPrefix prefix)
        {
            return NameTable[(int)prefix - 1][0];
        }

        public static int GetSuffixName(ReforgedSuffix suffix)
        {
            return NameTable[(int)suffix - 1][1];
        }

        private static int[][] NameTable = new int[][]
        {
            new int[] { 1151682, 1151683 },
            new int[] { 1151684, 1151685 },
            new int[] { 1151686, 1151687 },
            new int[] { 1151688, 1151689 },
            new int[] { 1151690, 1151691 },
            new int[] { 1151692, 1151693 },
            new int[] { 1151694, 1151695 },
            new int[] { 1151696, 1151697 },
            new int[] { 1151698, 1151699 },
            new int[] { 1151700, 1151701 },
            new int[] { 1151702, 1151703 },
            new int[] { 1151704, 1151705 },
        };

        public static void AddSuffixName(ObjectPropertyList list, ReforgedSuffix suffix, string name)
        {
            switch (suffix)
            {
                case ReforgedSuffix.Minax:
                    list.Add(1154507, name); break; // ~1_ITEM~ bearing the crest of Minax
                case ReforgedSuffix.Blackthorn:
                    list.Add(1154548, name); break;// ~1_TYPE~ bearing the crest of Blackthorn
                case ReforgedSuffix.Kotl:
                    list.Add(1156900, name); break;// ~1_ITEM~ of the Kotl
                default:
                    list.Add(1151758, String.Format("{0}\t#{1}", name, GetSuffixName(suffix))); break;// ~1_ITEM~ of ~2_SUFFIX~
            }
        }

        private static readonly SkillName[] m_Skills = new SkillName[]
        {
            SkillName.Swords,
            SkillName.Fencing,
            SkillName.Macing,
            SkillName.Archery,
            SkillName.Wrestling,
            SkillName.Parry,
            SkillName.Tactics,
            SkillName.Anatomy,
            SkillName.Healing,
            SkillName.Magery,
            SkillName.Meditation,
            SkillName.EvalInt,
            SkillName.MagicResist,
            SkillName.AnimalTaming,
            SkillName.AnimalLore,
            SkillName.Veterinary,
            SkillName.Musicianship,
            SkillName.Provocation,
            SkillName.Discordance,
            SkillName.Peacemaking,
            SkillName.Chivalry,
            SkillName.Focus,
            SkillName.Necromancy,
            SkillName.Stealing,
            SkillName.Stealth,
            SkillName.SpiritSpeak,
            SkillName.Bushido,
            SkillName.Ninjitsu
        };

        #region Random Item Generation
        public static Item GenerateRandomItem(IEntity e)
        {
            Item item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(LootPackEntry.IsInTokuno(e), LootPackEntry.IsMondain(e), LootPackEntry.IsStygian(e));

            if (item != null)
                GenerateRandomItem(item, null, Utility.RandomMinMax(100, 700), 0, ReforgedPrefix.None, ReforgedSuffix.None);

            return item;
        }

        /// <summary>
        /// This can be called from lootpack once loot pack conversions are implemented (if need be)
        /// </summary>
        /// <param name="item">item to mutate</param>
        /// <param name="luck">adjust luck chance</param>
        /// <param name="minBudget"></param>
        /// <param name="maxBudget"></param>
        /// <returns></returns>
        public static bool GenerateRandomItem(Item item, int luck, int minBudget, int maxBudget)
        {
            if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
            {
                int budget = Utility.RandomMinMax(minBudget, maxBudget);
                GenerateRandomItem(item, null, budget, luck, ReforgedPrefix.None, ReforgedSuffix.None);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called in DemonKnight.cs for forcing rad items
        /// </summary>
        /// <param name="item"></param>
        /// <param name="luck">raw luck</param>
        /// <param name="artifact"></param>
        /// <returns></returns>
        public static bool GenerateRandomArtifactItem(Item item, int luck, int budget, ReforgedPrefix prefix = ReforgedPrefix.None, ReforgedSuffix suffix = ReforgedSuffix.None)
        {
            if (prefix == ReforgedPrefix.None)
                prefix = ChooseRandomPrefix(item);

            if (suffix == ReforgedSuffix.None)
                suffix = ChooseRandomSuffix(item);

            if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
            {
                GenerateRandomItem(item, null, budget, LootPack.GetLuckChance(luck), prefix, suffix, artifact: true);
                return true;
            }
            return false;
        }

        public static Item GenerateRandomItem(Mobile killer, BaseCreature creature)
        {
            Item item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(LootPackEntry.IsInTokuno(killer), LootPackEntry.IsMondain(killer), LootPackEntry.IsStygian(killer));

            if (item != null)
                GenerateRandomItem(item, killer, Math.Max(100, GetDifficultyFor(creature)), LootPack.GetLuckChance(GetLuckForKiller(creature)), ReforgedPrefix.None, ReforgedSuffix.None);

            return item;
        }

        /// <summary>
        /// Called in LootPack.cs
        /// </summary>
        public static bool GenerateRandomItem(Item item, Mobile killer, BaseCreature creature)
        {
            if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
            {
                GenerateRandomItem(item, killer, Math.Max(100, GetDifficultyFor(creature)), LootPack.GetLuckChance(GetLuckForKiller(creature)), ReforgedPrefix.None, ReforgedSuffix.None);
                return true;
            }

            return false;
        }

        public static bool GenerateRandomItem(Item item, Mobile killer, BaseCreature creature, ReforgedPrefix prefix, ReforgedSuffix suffix)
        {
            if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
            {
                GenerateRandomItem(item, killer, Math.Max(100, GetDifficultyFor(creature)), LootPack.GetLuckChance(GetLuckForKiller(creature)), prefix, suffix);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called from TreasureMapChest.cs
        /// </summary>
        /// <param name="item">item to mutate</param>
        /// <param name="luck">raw luck</param>
        /// <param name="minBudget"></param>
        /// <param name="maxBudget"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool GenerateRandomItem(Item item, int luck, int minBudget, int maxBudget, Map map)
        {
            if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
            {
                int budget = Utility.RandomMinMax(minBudget, maxBudget);
                GenerateRandomItem(item, null, budget, LootPack.GetLuckChance(luck), ReforgedPrefix.None, ReforgedSuffix.None, map);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">item to mutate</param>
        /// <param name="killer">who killed the monster, if applicable</param>
        /// <param name="basebudget">where to we start, regarding the difficulty of the monster we killed</param>
        /// <param name="luckchance">adjusted luck</param>
        /// <param name="forcedprefix"></param>
        /// <param name="forcedsuffix"></param>
        /// <param name="map"></param>
        public static void GenerateRandomItem(Item item, Mobile killer, int basebudget, int luckchance, ReforgedPrefix forcedprefix, ReforgedSuffix forcedsuffix, Map map = null, bool artifact = false)
        {
            if (map == null && killer != null)
                map = killer.Map;

            if (item != null)
            {
                int budget = basebudget;
                int rawLuck = killer != null ? killer is PlayerMobile ? ((PlayerMobile)killer).RealLuck : killer.Luck : 0;

                int mods = 0;
                int perclow = 0;
                int perchigh = 0;

                bool powerful = budget >= 550;

                ReforgedPrefix prefix = forcedprefix;
                ReforgedSuffix suffix = forcedsuffix;

                if (artifact)
                {
                    ChooseArtifactMods(item, budget, out mods, out perclow, out perchigh);
                }
                else
                {
                    int budgetBonus = 0;

                    if (killer != null)
                    {
                        if (map != null && map.Rules == MapRules.FeluccaRules)
                        {
                            budgetBonus = RandomItemGenerator.FeluccaBudgetBonus;
                        }
                    }

                    int divisor = GetDivisor(basebudget);

                    double perc = 0.0;
                    double highest = 0.0;
                    
                    for (int i = 0; i < 1 + (rawLuck / 600); i++)
                    {
                        perc = (100.0 - Math.Sqrt(Utility.RandomMinMax(0, 10000))) / 100.0;

                        if (perc > highest)
                            highest = perc;
                    }

                    perc = highest;

                    if (perc > 1.0) perc = 1.0;
                    int toAdd = Math.Min(500, RandomItemGenerator.MaxAdjustedBudget - basebudget);

                    budget = Utility.RandomMinMax(basebudget - (basebudget / divisor), (int)(basebudget + (toAdd * perc))) + budgetBonus;

                    // Gives a rare chance for a high end item to drop on a low budgeted monster
                    if (rawLuck > 0 && budget <= 550 && LootPack.CheckLuck(luckchance / 6))
                    {
                        budget = Utility.RandomMinMax(600, 1150);
                    }

                    budget = Math.Min(RandomItemGenerator.MaxAdjustedBudget, budget);
                    budget = Math.Max(RandomItemGenerator.MinAdjustedBudget, budget);

                    if (!(item is BaseWeapon) && prefix == ReforgedPrefix.Vampiric)
                        prefix = ReforgedPrefix.None;

                    if (!(item is BaseWeapon) && suffix == ReforgedSuffix.Vampire)
                        suffix = ReforgedSuffix.None;

                    if (forcedprefix == ReforgedPrefix.None && budget >= Utility.Random(2700) && suffix != ReforgedSuffix.Minax && suffix != ReforgedSuffix.Kotl)
                        prefix = ChooseRandomPrefix(item);

                    if (forcedsuffix == ReforgedSuffix.None && budget >= Utility.Random(2700))
                        suffix = ChooseRandomSuffix(item, prefix);

                    if (suffix == ReforgedSuffix.Minax)
                        item.Hue = 1157;

                    if (!powerful)
                    {
                        mods = Math.Max(1, GetProperties(5));

                        perchigh = Math.Max(50, Math.Min(500, budget) / mods);
                        perclow = Math.Max(20, perchigh / 3);
                    }
                    else
                    {
                        int maxmods = Math.Max(5, Math.Min(RandomItemGenerator.MaxProps - 1, (int)Math.Ceiling((double)budget / (double)Utility.RandomMinMax(100, 140))));
                        int minmods = Math.Max(4, maxmods - 4);

                        mods = Math.Max(minmods, GetProperties(maxmods));

                        perchigh = 100;
                        perclow = Utility.RandomMinMax(50, 70);
                    }

                    if (perchigh > 100) perchigh = 100;
                    if (perclow < 10) perclow = 10;
                    if (perclow > 80) perclow = 80;
                }

                if (mods < RandomItemGenerator.MaxProps - 1 && LootPack.CheckLuck(luckchance))
                    mods++;

                ApplyReforgedProperties(item, prefix, suffix, false, budget, perclow, perchigh, mods, luckchance);

                int addonbudget = TryApplyRandomDisadvantage(item);

                if (addonbudget > 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        ApplyRunicAttributes(item, perclow, perchigh, ref addonbudget, i, luckchance, false);

                        if (addonbudget <= 0 || mods + (i + 1) >= RandomItemGenerator.MaxProps)
                            break;
                    }
                }

                NegativeAttributes neg = GetNegativeAttributes(item);

                if (neg != null && item is IDurability && (neg.Antique == 1 || neg.Brittle == 1 || item is BaseJewel))
                {
                    ((IDurability)item).MaxHitPoints = 255;
                    ((IDurability)item).HitPoints = 255;
                }

                ItemPower power = ApplyItemPower(item, false);

                if (artifact && power < ItemPower.LesserArtifact)
                {
                    int extra = 5000;
                    do
                    {
                        ApplyRunicAttributes(item, perclow, perchigh, ref extra, 0, luckchance, false);
                    }
                    while (ApplyItemPower(item, false) < ItemPower.LesserArtifact);
                }

                if (power == ItemPower.LegendaryArtifact && (item is BaseArmor || item is BaseClothing))
                {
                    item.Hue = 2500;
                }
            }
        }

        public static int GetProperties(int max)
        {
            if (max > RandomItemGenerator.MaxProps - 1)
                max = RandomItemGenerator.MaxProps - 1;

            int p0 = 0, p1 = 0, p2 = 0, p3 = 0, p4 = 0, p5 = 0, p6 = 0, p7 = 0, p8 = 0, p9 = 0, p10 = 0, p11 = 0;

            switch (max)
            {
                case 1: p0 = 3; p1 = 1; break;
                case 2: p0 = 6; p1 = 3; p2 = 1; break;
                case 3: p0 = 10; p1 = 6; p2 = 3; p3 = 1; break;
                case 4: p0 = 16; p1 = 12; p2 = 6; p3 = 5; p4 = 1; break;
                case 5: p0 = 30; p1 = 25; p2 = 20; p3 = 15; p4 = 9; p5 = 1; break;
                case 6: p0 = 35; p1 = 30; p2 = 25; p3 = 20; p4 = 15; p5 = 9; p6 = 1; break;
                case 7: p0 = 40; p1 = 35; p2 = 30; p3 = 25; p4 = 20; p5 = 15; p6 = 9; p7 = 1; break;
                case 8: p0 = 50; p1 = 40; p2 = 35; p3 = 30; p4 = 25; p5 = 20; p6 = 15; p7 = 9; p8 = 1; break;
                case 9: p0 = 70; p1 = 55; p2 = 45; p3 = 35; p4 = 30; p5 = 25; p6 = 20; p7 = 15; p8 = 9; p9 = 1; break;
                case 10: p0 = 90; p1 = 70; p2 = 55; p3 = 45; p4 = 40; p5 = 35; p6 = 25; p7 = 15; p8 = 10; p9 = 5; p10 = 1; break;
            }

            int pc = p0 + p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11;

            int rnd = Utility.Random(pc);

            if (rnd < p10)
                return 10;
            else
                rnd -= p10;

            if (rnd < p9)
                return 9;
            else
                rnd -= p9;

            if (rnd < p8)
                return 8;
            else
                rnd -= p8;

            if (rnd < p7)
                return 7;
            else
                rnd -= p7;

            if (rnd < p6)
                return 6;
            else
                rnd -= p6;

            if (rnd < p5)
                return 5;
            else
                rnd -= p5;

            if (rnd < p4)
                return 4;
            else
                rnd -= p4;

            if (rnd < p3)
                return 3;
            else
                rnd -= p3;

            if (rnd < p2)
                return 2;
            else
                rnd -= p2;

            if (rnd < p1)
                return 1;

            return 0;
        }

        private static void ChooseArtifactMods(Item item, int budget, out int mods, out int perclow, out int perchigh)
        {
            int maxmods = Math.Min(10, budget / 120);
            mods = Utility.RandomMinMax(6, maxmods);

            perchigh = 100;
            perclow = item is BaseShield ? 100 : 80;
        }

        private static int GetDivisor(int basebudget)
        {
            if (basebudget < 400)
                return 5;

            if (basebudget < 550)
                return 4;

            if (basebudget < 650)
                return 3;

            return 2;
        }

        public static int GetLuckForKiller(BaseCreature dead)
        {
            Mobile highest = dead.GetHighestDamager();

            if (highest != null)
            {
                return highest is PlayerMobile ? ((PlayerMobile)highest).RealLuck : highest.Luck;
            }

            return 0;
        }

        private static int[] m_Standard = new int[] { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12 };
        private static int[] m_Weapon = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        public static ReforgedPrefix ChooseRandomPrefix(Item item)
        {
            return ChooseRandomSuffix(item, ReforgedSuffix.None);
        }

        public static ReforgedPrefix ChooseRandomSuffix(Item item, ReforgedSuffix suffix)
        {
            int random = item is BaseWeapon ? m_Weapon[Utility.Random(m_Weapon.Length)] : m_Standard[Utility.Random(m_Standard.Length)];

            while ((int)suffix != 0 && random == (int)suffix)
                random = item is BaseWeapon ? m_Weapon[Utility.Random(m_Weapon.Length)] : m_Standard[Utility.Random(m_Standard.Length)];

            return (ReforgedPrefix)random;

        }

        public static ReforgedSuffix ChooseRandomSuffix(Item item)
        {
            return ChooseRandomSuffix(item, ReforgedPrefix.None);
        }

        public static ReforgedSuffix ChooseRandomSuffix(Item item, ReforgedPrefix prefix)
        {
            int random = item is BaseWeapon ? m_Weapon[Utility.Random(m_Weapon.Length)] : m_Standard[Utility.Random(m_Standard.Length)];

            while ((int)prefix != 0 && random == (int)prefix)
                random = item is BaseWeapon ? m_Weapon[Utility.Random(m_Weapon.Length)] : m_Standard[Utility.Random(m_Standard.Length)];

            return (ReforgedSuffix)random;

        }

        public static int GetDifficultyFor(BaseCreature bc)
        {
            return RandomItemGenerator.GetDifficultyFor(bc);
        }

        private static int TryApplyRandomDisadvantage(Item item)
        {
            AosAttributes attrs = GetAosAttributes(item);
            NegativeAttributes neg = GetNegativeAttributes(item);

            if (attrs == null || neg == null)
                return 0;

            int max = Imbuing.GetMaxWeight(item);
            ItemPower power = GetItemPower(item, Imbuing.GetTotalWeight(item), Imbuing.GetTotalMods(item), false);
            double chance = Utility.RandomDouble();

            if (item is BaseJewel && power >= ItemPower.MajorArtifact)
            {
                if (chance > .25)
                    neg.Antique = 1;
                else
                    item.LootType = LootType.Cursed;
                return 100;
            }

            switch (power)
            {
                default:
                    return 0;
                case ItemPower.Lesser: // lesser magic
                    {
                        if (.95 >= chance)
                            return 0;

                        switch (Utility.Random(item is BaseJewel ? 8 : 10))
                        {
                            case 0: neg.Prized = 1; break;
                            case 1: neg.Antique = 1; break;
                            case 2:
                            case 3: neg.Unwieldly = 1; break;
                            case 4:
                            case 5: attrs.Luck = -100; break;
                            case 6:
                            case 7: item.LootType = LootType.Cursed; break;
                            case 8:
                            case 9: neg.Massive = 1; break;
                        }

                        return 100;
                    }
                case ItemPower.Greater:// greater magic
                    {
                        if (.75 >= chance)
                            return 0;

                        chance = Utility.RandomDouble();

                        if (.75 > chance)
                        {
                            switch (Utility.Random(item is BaseJewel ? 6 : 8))
                            {
                                case 0: neg.Prized = 1; break;
                                case 1: neg.Antique = 1; break;
                                case 2:
                                case 3: neg.Unwieldly = 1; break;
                                case 4:
                                case 5: attrs.Luck = -100; break;
                                case 6:
                                case 7: neg.Massive = 1; break;
                            }

                            return 100;
                        }
                        else if (.5 > chance)
                        {
                            if (Utility.RandomBool())
                                neg.Prized = 1;
                            else
                                attrs.Luck = -100;

                            return 100;
                        }
                        else if (.85 > chance)
                        {
                            if (Utility.RandomBool() || item is BaseJewel)
                                neg.Antique = 1;
                            else
                                neg.Brittle = 1;

                            return 150;
                        }
                        else
                        {
                            item.LootType = LootType.Cursed;
                            return 100;
                        }
                    }
                case ItemPower.Major: // major magic
                    {
                        if (.50 >= chance)
                            return 0;

                        chance = Utility.RandomDouble();

                        if (.4 > chance)
                        {
                            neg.Prized = 1;
                            return 100;
                        }
                        else if (.6 > chance)
                        {
                            switch (Utility.Random(item is BaseJewel ? 8 : 10))
                            {
                                case 0: neg.Prized = 1; break;
                                case 1: neg.Antique = 1; break;
                                case 2:
                                case 3: neg.Unwieldly = 1; break;
                                case 4:
                                case 5: attrs.Luck = -100; break;
                                case 6:
                                case 7: item.LootType = LootType.Cursed; break;
                                case 8:
                                case 9: neg.Massive = 1; break;
                            }

                            return 100;
                        }
                        else if (.9 > chance || item is BaseJewel)
                        {
                            neg.Antique = 1;
                            return 150;
                        }
                        else
                        {
                            neg.Brittle = 1;
                            return 150;
                        }
                    }
                case ItemPower.LesserArtifact: // lesser arty
                case ItemPower.GreaterArtifact: // greater arty
                    {
                        if (0.001 > chance)
                            return 0;

                        chance = Utility.RandomDouble();

                        if (0.33 > chance && !(item is BaseJewel))
                        {
                            neg.Brittle = 1;
                            return 150;
                        }
                        else if (0.66 > chance)
                        {
                            item.LootType = LootType.Cursed;
                            return 150;
                        }
                        else if (0.85 > chance)
                        {
                            neg.Antique = 1;
                            return 150;
                        }
                        else
                        {
                            neg.Prized = 1;
                            return 100;
                        }
                    }
                case ItemPower.MajorArtifact:
                case ItemPower.LegendaryArtifact:
                    {
                        if (0.0001 > Utility.RandomDouble())
                            return 0;

                        if (0.85 > chance)
                        {
                            neg.Antique = 1;
                            return 100;
                        }
                        else if (.95 > chance)
                        {
                            item.LootType = LootType.Cursed;
                            return 100;
                        }
                        else
                        {
                            neg.Brittle = 1;
                            return 100;
                        }
                    }
            }
        }

        public static ItemPower ApplyItemPower(Item item, bool playermade)
        {
            ItemPower ip = GetItemPower(item, Imbuing.GetTotalWeight(item), Imbuing.GetTotalMods(item), playermade);

            if (item is BaseWeapon)
                ((BaseWeapon)item).ItemPower = ip;
            else if (item is BaseArmor)
                ((BaseArmor)item).ItemPower = ip;
            else if (item is BaseJewel)
                ((BaseJewel)item).ItemPower = ip;
            else if (item is BaseClothing)
                ((BaseClothing)item).ItemPower = ip;

            return ip;
        }

        public static ItemPower GetItemPower(Item item, int weight, int totalMods, bool playermade)
        {
            // pre-arty uses max imbuing weight + 100
            // arty ranges from pre-arty to a flat 1200
            double preArty = Imbuing.GetMaxWeight(item) + 100;
            double arty = 1200 - preArty;

            if (totalMods == 0)
                return ItemPower.None;

            if (weight < preArty * .4)
                return playermade ? ItemPower.ReforgedMinor : ItemPower.Minor;

            if (weight < preArty * .6)
                return playermade ? ItemPower.ReforgedLesser : ItemPower.Lesser;

            if (weight < preArty * .8)
                return playermade ? ItemPower.ReforgedGreater : ItemPower.Greater;

            if (weight <= preArty)
                return playermade ? ItemPower.ReforgedGreater : ItemPower.Major;

            if (weight < preArty + (arty * .2))
                return playermade ? ItemPower.ReforgedMajor : ItemPower.LesserArtifact;

            if (weight < preArty + (arty * .4))
                return playermade ? ItemPower.ReforgedMajor : ItemPower.GreaterArtifact;

            if (weight < preArty + (arty * .7) || totalMods <= 5)
                return ItemPower.MajorArtifact;

            return playermade ? ItemPower.ReforgedLegendary : ItemPower.LegendaryArtifact;
        }

        private static bool ApplyNewAttributes(Item item, int prefixID, int suffixID, int colIndex, int percLow, int percHigh, int resIndex, int preIndex, int luckchance, bool playermade, ref int budget)
        {
            int randomCol = item is BaseWeapon ? m_Weapon[Utility.Random(m_Weapon.Length)] : m_Standard[Utility.Random(m_Standard.Length)];

            while (prefixID != 0 && randomCol == prefixID && suffixID != 0 && randomCol == suffixID)
                randomCol = item is BaseWeapon ? m_Weapon[Utility.Random(m_Weapon.Length)] : m_Standard[Utility.Random(m_Standard.Length)];

            ReforgedPrefix prefix = (ReforgedPrefix)randomCol;
            var collection = new List<NamedInfoCol>(m_PrefixSuffixInfo[randomCol][colIndex]);

            if (collection == null || collection.Count == 0)
            {
                return false;
            }

            CheckAttributes(item, collection, playermade);
            int random = Utility.Random(collection.Count);

            return ApplyPrefixSuffixAttribute(item, 
                collection[random].Attribute, 
                collection[random].Min(resIndex, preIndex, item), 
                collection[random].Max(resIndex, preIndex, item), percLow, percHigh, ref budget, luckchance, playermade);
        }

        private static bool ApplyRunicAttributes(Item item, int perclow, int perchigh, ref int budget, int idx, int luckchance, bool playerMade)
        {
            List<object> attrList = null;
            AosWeaponAttributes wepattrs = GetAosWeaponAttributes(item);
            AosAttributes aosattrs = GetAosAttributes(item);
            AosArmorAttributes armorattrs = GetAosArmorAttributes(item);
            AosSkillBonuses skillbonuses = GetAosSkillBonuses(item);
            AosElementAttributes resistattrs = GetElementalAttributes(item);

            if (item is BaseWeapon)
            {
                if (item is BaseRanged)
                    attrList = new List<object>(m_RangedWeaponList);
                else
                    attrList = new List<object>(m_MeleeWeaponList);
            }
            else if (item is BaseShield)
            {
                attrList = new List<object>(m_ShieldList);
            }
            else if (item is BaseArmor)
            {
                attrList = new List<object>(m_ArmorList);
            }
            else if (item is BaseClothing)
            {
                attrList = new List<object>(m_ArmorList);
            }
            else if (item is BaseJewel)
            {
                attrList = new List<object>(m_JewelList);
            }
            else
                return false;

            int random = 0;
            int start = budget;

            while (start == budget && budget > 0 && idx < 25)
            {
                if (attrList.Count == 0)
                    return false;

                random = Utility.Random(attrList.Count);
                object attr = attrList[random];
                int[] minmax = new int[] { 1, 1 };
                int value = 1;

                if (CheckAttribute(item, attr))
                {
                    attrList.RemoveAt(random);
                    continue;
                }

                if (wepattrs != null && attr is AosWeaponAttribute[])
                {
                    int ran = Utility.Random(((AosWeaponAttribute[])attr).Length);

                    while (wepattrs[(AosWeaponAttribute)ran] != 0)
                        ran = Utility.Random(((AosWeaponAttribute[])attr).Length);

                    AosWeaponAttribute[] list = attr as AosWeaponAttribute[];

                    attr = list[ran];
                }

                if (aosattrs != null && attr is AosAttribute)
                {
                    minmax = Imbuing.GetPropRange((AosAttribute)attr);

                    int min = minmax[0];
                    int max = minmax[1];

                    if (item is BaseJewel && (AosAttribute)attr == AosAttribute.WeaponDamage)
                        max = 25;

                    value = CalculateValue(attr, min, max, perclow, perchigh, ref budget, luckchance, playerMade);

                    bool hasValue;

                    if ((AosAttribute)attr == AosAttribute.CastSpeed && aosattrs.SpellChanneling > 0)
                        hasValue = aosattrs.CastSpeed > -1;
                    else
                        hasValue = aosattrs[(AosAttribute)attr] > 0;

                    if (!hasValue)
                    {
                        aosattrs[(AosAttribute)attr] += value;
                        budget -= Imbuing.GetIntensityForAttribute(item, (AosAttribute)attr, -1, value);

                        if ((AosAttribute)attr == AosAttribute.SpellChanneling && aosattrs[AosAttribute.CastSpeed] > -1)
                            aosattrs[AosAttribute.CastSpeed]--;
                    }
                }
                else if (wepattrs != null && attr is AosWeaponAttribute)
                {
                    minmax = Imbuing.GetPropRange(item, (AosWeaponAttribute)attr);
                    value = CalculateValue(attr, minmax[0], minmax[1], perclow, perchigh, ref budget, luckchance, playerMade);

                    if (wepattrs[(AosWeaponAttribute)attr] == 0)
                    {
                        wepattrs[(AosWeaponAttribute)attr] = value;
                        budget -= Imbuing.GetIntensityForAttribute(item, (AosWeaponAttribute)attr, -1, value);
                    }
                }
                else if (armorattrs != null && attr is AosArmorAttribute)
                {
                    minmax = Imbuing.GetPropRange((AosArmorAttribute)attr);
                    value = CalculateValue(attr, minmax[0], minmax[1], perclow, perchigh, ref budget, luckchance, playerMade);

                    if (armorattrs[(AosArmorAttribute)attr] == 0)
                    {
                        armorattrs[(AosArmorAttribute)attr] = value;
                        budget -= Imbuing.GetIntensityForAttribute(item, (AosArmorAttribute)attr, -1, value);
                    }
                }
                else if (attr is AosElementAttribute)
                {
                    value = CalculateValue(attr, 1, 15, perclow, perchigh, ref budget, luckchance, playerMade);

                    if (value > 0)
                    {
                        if (ApplyResistance(item, value, (AosElementAttribute)attr))
                        {
                            budget -= Imbuing.GetIntensityForAttribute(item, (AosElementAttribute)attr, -1, value);
                        }
                    }
                }
                else if (attr is string)
                {
                    string str = attr as string;

                    if (item is BaseWeapon)
                    {
                        if (str == "Slayer" && ((BaseWeapon)item).Slayer == SlayerName.None)
                        {
                            SlayerName name = BaseRunicTool.GetRandomSlayer();
                            int weight = Imbuing.GetIntensityForAttribute(item, name, -1, value);

                            if (weight <= budget)
                            {
                                ((BaseWeapon)item).Slayer = name;
                                budget -= weight;
                            }
                        }
                        else if (str == "ElementalDamage")
                        {
                            BaseRunicTool.ApplyElementalDamage((BaseWeapon)item, perclow, perchigh);
                        }
                        else if (item is BaseRanged && str == "WeaponVelocity")
                        {
                            value = CalculateValue(attr, 2, 50, perclow, perchigh, ref budget, luckchance, playerMade);

                            if (((BaseRanged)item).Velocity == 0)
                            {
                                ((BaseRanged)item).Velocity = value;
                                budget -= Imbuing.GetIntensityForAttribute(item, attr, -1, value);
                            }
                        }
                    }
                    else if (skillbonuses != null && str == "SkillBonus")
                    {
                        SkillName[] possibleSkills = m_Skills;
                        SkillName sk, check;
                        int skillIdx = idx - 1;
                        double bonus;
                        bool found;

                        do
                        {
                            found = false;
                            sk = possibleSkills[Utility.Random(possibleSkills.Length)];

                            if ((item is GargishRing || item is GargishBracelet) && sk == SkillName.Archery)
                                sk = SkillName.Throwing;

                            for (int i = 0; !found && i < 5; ++i)
                                found = (skillbonuses.GetValues(i, out check, out bonus) && check == sk);
                        } while (found);

                        value = CalculateValue(sk, 1, 15, perclow, perchigh, ref budget, luckchance, playerMade);
                        skillbonuses.SetValues(skillIdx, sk, value);
                        budget -= Imbuing.GetIntensityForAttribute(item, sk, -1, value);
                    }
                }

                attrList.RemoveAt(random);
                idx++;
            }

            if (attrList != null)
                attrList.Clear();

            return true;
        }

        public static bool HasAosAttributesValue(AosAttributes attrs, AosAttribute attr)
        {
            if (attr == AosAttribute.CastSpeed && attrs.SpellChanneling > 0)
            {
                return attrs.CastSpeed >= 0;
            }

            return attrs[attr] > 0;
        }

        public static bool CheckAttribute(Item item, object attr)
        {
            if (CheckEater(item, attr))
                return true;

            if (item is BaseArmor && !(item is BaseShield) && attr is AosArmorAttribute && (AosArmorAttribute)attr == AosArmorAttribute.MageArmor && ((BaseArmor)item).MeditationAllowance == ArmorMeditationAllowance.All)
                return true;

            if (item is BaseClothing && attr is AosArmorAttribute && (AosArmorAttribute)attr == AosArmorAttribute.MageArmor)
                return true;

            if (item is BaseWeapon && attr is AosWeaponAttribute[])
            {
                AosWeaponAttribute[] attrs = attr as AosWeaponAttribute[];

                if (CheckHitSpell((BaseWeapon)item, attrs[0]) || CheckHitArea((BaseWeapon)item, attrs[0]))
                    return true;
            }

            if (CheckConflictingNegative(item, attr))
                return true;

            return false;
        }

        public static bool CheckConflictingNegative(Item item, object attr)
        {
            NegativeAttributes neg = GetNegativeAttributes(item);
            AosAttributes aosattr = GetAosAttributes(item);

            if (neg == null)
                return false;

            if (neg.Brittle > 0 || neg.Antique > 0 || neg.NoRepair > 0 || (aosattr != null && aosattr.Brittle > 0))
            {
                if (attr is AosWeaponAttribute && (AosWeaponAttribute)attr == AosWeaponAttribute.SelfRepair)
                    return true;

                if (attr is AosArmorAttribute && (AosArmorAttribute)attr == AosArmorAttribute.SelfRepair)
                    return true;
            }

            return false;
        }

        public static AosAttributes GetAosAttributes(Item item)
        {
            if (item is BaseWeapon)
                return ((BaseWeapon)item).Attributes;

            if (item is BaseArmor)
                return ((BaseArmor)item).Attributes;

            if (item is BaseClothing)
                return ((BaseClothing)item).Attributes;

            if (item is BaseJewel)
                return ((BaseJewel)item).Attributes;

            if (item is BaseTalisman)
                return ((BaseTalisman)item).Attributes;

            if (item is BaseQuiver)
                return ((BaseQuiver)item).Attributes;

            if (item is Spellbook)
                return ((Spellbook)item).Attributes;

            if(item is FishingPole)
                return ((FishingPole)item).Attributes;

            return null;
        }

        public static AosArmorAttributes GetAosArmorAttributes(Item item)
        {
            if (item is BaseArmor)
                return ((BaseArmor)item).ArmorAttributes;

            if (item is BaseClothing)
                return ((BaseClothing)item).ClothingAttributes;

            return null;
        }

        public static AosWeaponAttributes GetAosWeaponAttributes(Item item)
        {
            if (item is BaseWeapon)
                return ((BaseWeapon)item).WeaponAttributes;

            if (item is Glasses)
                return ((Glasses)item).WeaponAttributes;

            if (item is GargishGlasses)
                return ((GargishGlasses)item).WeaponAttributes;

            if(item is ElvenGlasses)
                return ((ElvenGlasses)item).WeaponAttributes;
            return null;
        }

        public static ExtendedWeaponAttributes GetExtendedWeaponAttributes(Item item)
        {
            if (item is BaseWeapon)
                return ((BaseWeapon)item).ExtendedWeaponAttributes;

            return null;
        }

        public static AosElementAttributes GetElementalAttributes(Item item)
        {
            if (item is BaseClothing)
                return ((BaseClothing)item).Resistances;

            else if (item is BaseJewel)
                return ((BaseJewel)item).Resistances;

            else if (item is BaseWeapon)
                return ((BaseWeapon)item).AosElementDamages;

            else if (item is BaseQuiver)
                return ((BaseQuiver)item).Resistances;

            return null;
        }

        public static SAAbsorptionAttributes GetSAAbsorptionAttributes(Item item)
        {
            if (item is BaseArmor)
                return ((BaseArmor)item).AbsorptionAttributes;

            else if (item is BaseJewel)
                return ((BaseJewel)item).AbsorptionAttributes;

            else if (item is BaseWeapon)
                return ((BaseWeapon)item).AbsorptionAttributes;

            else if (item is BaseClothing)
                return ((BaseClothing)item).SAAbsorptionAttributes;

            return null;
        }

        public static AosSkillBonuses GetAosSkillBonuses(Item item)
        {
            if (item is BaseJewel)
                return ((BaseJewel)item).SkillBonuses;

            else if (item is BaseWeapon)
                return ((BaseWeapon)item).SkillBonuses;

            else if (item is BaseArmor)
                return ((BaseArmor)item).SkillBonuses;

            else if (item is BaseTalisman)
                return ((BaseTalisman)item).SkillBonuses;

            else if (item is Spellbook)
                return ((Spellbook)item).SkillBonuses;

            else if (item is BaseQuiver)
                return ((BaseQuiver)item).SkillBonuses;

            else if (item is BaseClothing)
                return ((BaseClothing)item).SkillBonuses;

            return null;
        }

        public static NegativeAttributes GetNegativeAttributes(Item item)
        {
            if (item is BaseWeapon)
                return ((BaseWeapon)item).NegativeAttributes;

            if (item is BaseArmor)
                return ((BaseArmor)item).NegativeAttributes;

            if (item is BaseClothing)
                return ((BaseClothing)item).NegativeAttributes;

            if (item is BaseJewel)
                return ((BaseJewel)item).NegativeAttributes;

            if (item is BaseTalisman)
                return ((BaseTalisman)item).NegativeAttributes;

            if (item is Spellbook)
                return ((Spellbook)item).NegativeAttributes;

            return null;
        }

        public static int GetArtifactRarity(Item item)
        {
            if (item is BaseWeapon)
                return ((BaseWeapon)item).ArtifactRarity;

            if (item is BaseArmor)
                return ((BaseArmor)item).ArtifactRarity;

            if (item is BaseJewel)
                return ((BaseJewel)item).ArtifactRarity;

            if (item is BaseClothing)
                return ((BaseClothing)item).ArtifactRarity;

            return 0;
        }

        private static int ScaleAttribute(object o)
        {
            if (o is AosAttribute)
            {
                AosAttribute attr = (AosAttribute)o;

                if (attr == AosAttribute.Luck)
                    return 10;

                if (attr == AosAttribute.WeaponSpeed || attr == AosAttribute.EnhancePotions)
                    return 5;
            }
            else if (o is AosArmorAttribute)
            {
                AosArmorAttribute attr = (AosArmorAttribute)o;

                if (attr == AosArmorAttribute.LowerStatReq)
                    return 10;

                if (attr == AosArmorAttribute.DurabilityBonus)
                    return 10;
            }
            else if (o is AosWeaponAttribute)
            {
                AosWeaponAttribute attr = (AosWeaponAttribute)o;

                if (attr == AosWeaponAttribute.LowerStatReq)
                    return 10;

                if (attr == AosWeaponAttribute.DurabilityBonus)
                    return 10;

                if (attr == AosWeaponAttribute.SplinteringWeapon)
                    return 5;
            }
            else if (o is SkillName)
                return 5;

            return 1;
        }

        public static List<object> m_MeleeWeaponList;
        public static List<object> m_RangedWeaponList;
        public static List<object> m_ArmorList;
        public static List<object> m_JewelList;
        public static List<object> m_ShieldList;

        private static object[] m_WeaponBasic = new object[]
		{
			new AosWeaponAttribute[] { AosWeaponAttribute.HitMagicArrow, AosWeaponAttribute.HitHarm, AosWeaponAttribute.HitFireball, AosWeaponAttribute.HitLightning },// Hit spell
			new AosWeaponAttribute[] { AosWeaponAttribute.HitPhysicalArea, AosWeaponAttribute.HitFireArea, AosWeaponAttribute.HitColdArea, AosWeaponAttribute.HitPoisonArea, AosWeaponAttribute.HitEnergyArea, },// hit area
			"Slayer",
            "ElementalDamage",
			AosAttribute.WeaponDamage,
			AosAttribute.DefendChance,
			AosAttribute.CastSpeed,
			AosAttribute.AttackChance,
            AosAttribute.Luck,
			AosAttribute.WeaponSpeed,
			AosAttribute.SpellChanneling,
			AosWeaponAttribute.HitDispel,
			AosWeaponAttribute.HitLeechHits,
			AosWeaponAttribute.HitLeechStam,
			AosWeaponAttribute.HitLeechMana,
			AosWeaponAttribute.HitLowerAttack,
			AosWeaponAttribute.HitLowerDefend,
			AosWeaponAttribute.ResistPhysicalBonus,
			AosWeaponAttribute.ResistFireBonus,
			AosWeaponAttribute.ResistColdBonus,
			AosWeaponAttribute.ResistPoisonBonus,
			AosWeaponAttribute.ResistEnergyBonus,
			AosWeaponAttribute.HitFatigue,
			AosWeaponAttribute.HitManaDrain,
		};

        private static object[] m_RangedStandard = new object[]
        {
            AosAttribute.BalancedWeapon,
            "WeaponVelocity"
        };

        private static object[] m_MeleeStandard = new object[]
        {
            AosWeaponAttribute.UseBestSkill,
            AosWeaponAttribute.MageWeapon
        };

        private static object[] m_ArmorStandard = new object[]
		{
			AosAttribute.RegenHits,
			AosAttribute.RegenStam,
			AosAttribute.RegenMana,
			AosAttribute.NightSight,
			AosAttribute.BonusHits,
			AosAttribute.BonusStam,
			AosAttribute.BonusMana,
			AosAttribute.LowerManaCost,
			AosAttribute.LowerRegCost,
			AosAttribute.Luck,
			AosAttribute.ReflectPhysical,
			AosElementAttribute.Physical,
			AosElementAttribute.Fire,
			AosElementAttribute.Cold,
			AosElementAttribute.Poison,
			AosElementAttribute.Energy,
            AosArmorAttribute.MageArmor
		};

        private static object[] m_HatStandard = new object[]
		{
			AosAttribute.RegenHits,
			AosAttribute.RegenStam,
			AosAttribute.RegenMana,
			AosAttribute.NightSight,
			AosAttribute.BonusHits,
			AosAttribute.BonusStam,
			AosAttribute.BonusMana,
			AosAttribute.LowerManaCost,
			AosAttribute.LowerRegCost,
			AosAttribute.Luck,
			AosAttribute.ReflectPhysical,
			AosElementAttribute.Physical,
			AosElementAttribute.Fire,
			AosElementAttribute.Cold,
			AosElementAttribute.Poison,
			AosElementAttribute.Energy
		};

        private static object[] m_ShieldStandard = new object[]
		{
			AosAttribute.SpellChanneling,
			AosAttribute.DefendChance,
			//AosAttribute.AttackChance,
			AosAttribute.CastSpeed,
			AosAttribute.ReflectPhysical,
			AosArmorAttribute.LowerStatReq,
		};

        private static object[] m_JewelStandard = new object[]
		{
			AosElementAttribute.Physical,
			AosElementAttribute.Fire,
			AosElementAttribute.Cold,
			AosElementAttribute.Poison,
			AosElementAttribute.Energy,
			AosAttribute.DefendChance,
			AosAttribute.AttackChance,
			AosAttribute.WeaponDamage,
			AosAttribute.BonusStr,
			AosAttribute.BonusDex,
			AosAttribute.BonusInt,
			AosAttribute.EnhancePotions,
			AosAttribute.CastSpeed,
			AosAttribute.CastRecovery,
			AosAttribute.LowerManaCost,
			AosAttribute.LowerRegCost,
			AosAttribute.Luck,
			AosAttribute.SpellDamage,
			AosAttribute.NightSight,
			"SkillBonus",
			"SkillBonus",
			"SkillBonus",
			"SkillBonus",
			"SkillBonus",
		};

        #endregion

        #region Tables
        #region All
        public static int[][] DexIntTable = new int[][]
        {
            new int[] { 3, 4, 4, 4, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
        };

        public static int[][] LowerStatReqTable = new int[][]
        {
            new int[] { 60, 70, 80, 100, 100, 100, 100 },
            new int[] { 80, 100, 100, 100, 100, 100, 100 },
            new int[] { 100, 100, 100, 100, 100, 100, 100 },
            new int[] { 70, 100, 100, 100, 100, 100, 100 },
            new int[] { 80, 100, 100, 100, 100, 100, 100 },
            new int[] { 100, 100, 100, 100, 100, 100, 100 },
        };

        public static int[][] SelfRepairTable = new int[][]
        {
            new int[] { 2, 4, 0, 0, 0, 0, 0 },
            new int[] { 5, 5, 0, 0, 0, 0, 0 },
            new int[] { 6, 7, 0, 0, 0, 0, 0 },
            new int[] { 5, 5, 0, 0, 0, 0, 0 },
            new int[] { 5, 5, 0, 0, 0, 0, 0 },
            new int[] { 7, 7, 0, 0, 0, 0, 0 },
        };

        public static int[][] DurabilityTable = new int[][]
        {
            new int[] { 90, 100, 0, 0, 0, 0, 0 },
            new int[] { 110, 140, 0, 0, 0, 0, 0 },
            new int[] { 150, 150, 0, 0, 0, 0, 0 },
            new int[] { 100, 140, 0, 0, 0, 0, 0 },
            new int[] { 110, 140, 0, 0, 0, 0, 0 },
            new int[] { 150, 150, 0, 0, 0, 0, 0 },
        };

        public static int[][] ResistTable = new int[][]
        {
            new int[] { 10, 15, 15, 15, 20, 20, 20 },
            new int[] { 15, 15, 15, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
            new int[] { 15, 15, 20, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
        };

        public static int[][] EaterTable = new int[][]
        {
            new int[] { 9, 12, 12, 15, 15, 15, 15 },
            new int[] { 12, 15, 15, 15, 15, 15, 15 },
            new int[] { 15, 15, 15, 15, 15, 15, 15 },
            new int[] { 12, 15, 15, 15, 15, 15, 15 },
            new int[] { 12, 15, 15, 15, 15, 15, 15 },
            new int[] { 15, 15, 15, 15, 15, 15, 15 },
        };
        #endregion

        #region Weapon Tables
        // Hit magic, area, HLA
        public static int[][] HitWeaponTable1 = new int[][]
        {
            new int[] { 30, 50, 50, 60, 70, 70, 70 },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
            new int[] {  },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
        };

        // hit fatigue, mana drain, HLD
        public static int[][] HitWeaponTable2 = new int[][]
        {
            new int[] { 30, 40, 50, 50, 60, 70, 70 },
            new int[] { 50, 50, 50, 60, 70, 70, 70 },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] {  },
            new int[] { 50, 50, 50, 60, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
        };

        public static int[][] WeaponVelocityTable = new int[][]
        {
            new int[] { 25, 35, 40, 40, 40, 45, 50 },
            new int[] { 40, 40, 40, 45, 50, 50, 50 },
            new int[] { 40, 45, 50, 50, 50, 50, 50 },
            new int[] {  },
            new int[] { 40, 40, 40, 45, 50, 50, 50 },
            new int[] { 45, 50, 50, 50, 50, 50, 50 },
        };

        public static int[][] HitsAndManaLeechTable = new int[][]
        {
            new int[] { 15, 25, 25, 30, 35, 35, 35 },
            new int[] { 25, 25, 30, 35, 35, 35, 35 },
            new int[] { 30, 35, 35, 35, 35, 35, 35 },
            new int[] {  },
            new int[] { 25, 25, 30, 35, 35, 35, 35 },
            new int[] { 35, 35, 35, 35, 35, 35, 35 },
        };

        public static int[][] HitStamLeechTable = new int[][]
        {
            new int[] { 30, 50, 50, 60, 70, 70, 70 },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
            new int[] {  },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
        };

        public static int[][] LuckTable = new int[][]
        {
            new int[] { 80, 100, 100, 120, 140, 150, 150 },
            new int[] { 100, 120, 140, 150, 150, 150, 150 },
            new int[] { 130, 150, 150, 150, 150, 150, 150 },
            new int[] { 100, 120, 140, 150, 150, 150, 150 },
            new int[] { 100, 120, 140, 150, 150, 150, 150 },
            new int[] { 150, 150, 150, 150, 150, 150, 150 },
        };

        public static int[][] MageWeaponTable = new int[][]
        {
            new int[] { 25, 20, 20, 20, 20, 15, 15 },
            new int[] { 20, 20, 20, 15, 15, 15, 15 },
            new int[] { 20, 15, 15, 15, 15, 15, 15 },
            new int[] {  },
            new int[] { 20, 20, 20, 15, 15, 15, 15 },
            new int[] { 15, 15, 15, 15, 15, 15, 15 },
        };

        public static int[][] WeaponRegenTable = new int[][]
        {
            new int[] { 2, 3, 6, 6, 6, 6, 6 },
            new int[] { 3, 6, 6, 6, 6, 6, 6 },
            new int[] { 6, 6, 6, 6, 6, 9, 9 },
            new int[] {  },
            new int[] { 3, 6, 6, 6, 6, 6, 9 },
            new int[] { 6, 9, 9, 9, 9, 9, 9 },
        };

        public static int[][] WeaponHitsTable = new int[][]
        {
            new int[] { 2, 3, 3, 3, 4, 4, 4 },
            new int[] { 3, 3, 4, 4, 4, 4, 4 },
            new int[] { 4, 4, 4, 4, 4, 4, 4 },
            new int[] { },
            new int[] { 3, 3, 4, 4, 4, 4, 4 },
            new int[] { 4, 4, 4, 4, 4, 4, 4 },
        };

        public static int[][] WeaponStamManaLMCTable = new int[][]
        {
            new int[] { 2, 4, 4, 4, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
        };

        public static int[][] WeaponStrTable = new int[][]
        {
            new int[] { 2, 4, 4, 4, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] {  },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
        };

        public static int[][] WeaponHCITable = new int[][]
        {
            new int[] { 5, 10, 15, 15, 15, 20, 20 },
            new int[] { 15, 15, 15, 20, 20, 20, 20 },
            new int[] { 15, 20, 20, 20, 20, 20, 20 },
            new int[] {  },
            new int[] { 15, 15, 20, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
        };

        public static int[][] WeaponDCITable = new int[][]
        {
            new int[] { 10, 15, 15, 15, 20, 20, 20 },
            new int[] { 15, 15, 20, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
            new int[] {  },
            new int[] { 15, 15, 20, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
        };

        public static int[][] WeaponDamageTable = new int[][]
        {
            new int[] { 30, 50, 50, 60, 70, 70, 70 },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
            new int[] {  },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
        };

        public static int[][] WeaponEnhancePots = new int[][]
        {
            new int[] { 5, 10, 10, 10, 10, 15, 15 },
            new int[] { 10, 10, 10, 15, 15, 15, 15 },
            new int[] { 10, 15, 15, 15, 15, 15, 15 },
            new int[] {  },
            new int[] { 10, 10, 10, 15, 15, 15, 15 },
            new int[] { 15, 15, 15, 15, 15, 15, 15 },
        };

        public static int[][] WeaponWeaponSpeedTable = new int[][]
        {
            new int[] { 20, 30, 30, 35, 40, 40, 40 },
            new int[] { 30, 35, 40, 40, 40, 40, 40 },
            new int[] { 35, 40, 40, 40, 40, 40, 40 },
            new int[] {  },
            new int[] { 30, 35, 40, 40, 40, 40, 40 },
            new int[] { 40, 40, 40, 40, 40, 40, 40 },
        };
        #endregion

        #region Ranged Weapons
        public static int[][] RangedLuckTable = new int[][]
        {
            new int[] { 90, 120, 120, 140, 170, 170, 170 },
            new int[] { 120, 140, 160, 170, 170, 170, 170 },
            new int[] { 160, 170, 170, 170, 170, 170, 170 },
            new int[] {  },
            new int[] { 120, 140, 160, 170, 170, 170, 170 },
            new int[] { 170, 170, 170, 170, 170, 170, 170 },
        };

        public static int[][] RangedHCITable = new int[][]
        {
            new int[] { 15, 25, 25, 30, 35, 35, 35 },
            new int[] { 25, 30, 35, 35, 35, 35, 35 },
            new int[] { 35, 35, 35, 35, 35, 35, 35 },
            new int[] {  },
            new int[] { 25, 25, 30, 35, 35, 35, 35 },
            new int[] { 35, 35, 35, 35, 35, 35, 35 },
        };

        public static int[][] RangedDCITable = new int[][]
        {
            new int[] {  },
            new int[] {  },
            new int[] {  },
            new int[] {  },
            new int[] { 25, 25, 30, 35, 35, 35, 35 },
            new int[] { 35, 35, 35, 35, 35, 35, 35 },
        };
        #endregion

        #region Armor Tables
        public static int[][] LowerRegTable = new int[][]
        {
            new int[] { 10, 20, 20, 20, 25, 25, 25 },
            new int[] { 20, 20, 25, 25, 25, 25, 25 },
            new int[] { 25, 25, 25, 25, 25, 25, 25 },
            new int[] { 20, 20, 25, 25, 25, 25, 25 },
            new int[] { 20, 20, 25, 25, 25, 25, 25 },
            new int[] { 25, 25, 25, 25, 25, 25, 25 },
        };

        public static int[][] ArmorHitsTable = new int[][]
        {
            new int[] { 3, 5, 5, 6, 7, 7, 7 },
            new int[] { 5, 6, 7, 7, 7, 7, 7 },
            new int[] { 7, 7, 7, 7, 7, 7, 7 },
            new int[] { 5, 5, 6, 7, 7, 7, 7 },
            new int[] { 5, 6, 7, 7, 7, 7, 7 },
            new int[] { 7, 7, 7, 7, 7, 7, 7 },
        };

        public static int[][] ArmorStrTable = new int[][]
        {
            new int[] { 3, 4, 4, 4, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
        };

        public static int[][] ArmorRegenTable = new int[][]
        {
            new int[] { 2, 3, 3, 3, 4, 4, 4 },
            new int[] { 3, 3, 4, 4, 4, 4, 4 },
            new int[] { 4, 4, 4, 4, 4, 4, 4 },
            new int[] { 3, 3, 4, 4, 4, 4, 4 },
            new int[] { 3, 3, 4, 4, 4, 4, 4 },
            new int[] { 4, 4, 4, 4, 4, 4, 4 },
        };

        public static int[][] ArmorStamManaLMCTable = new int[][]
        {
            new int[] { 4, 8, 8, 8, 10, 10, 10 },
            new int[] { 8, 8, 10, 10, 10, 10, 10 },
            new int[] { 10, 10, 10, 10, 10, 10, 10 },
            new int[] { 8, 8, 10, 10, 10, 10, 10 },
            new int[] { 8, 8, 10, 10, 10, 10, 10 },
            new int[] { 10, 10, 10, 10, 10, 10, 10 },
        };

        public static int[][] ArmorEnhancePotsTable = new int[][]
        {
            new int[] { 2, 2, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
        };

        public static int[][] ArmorHCIDCITable = new int[][]
        {
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
        };

        public static int[][] ArmorCastingFocusTable = new int[][]
        {
            new int[] { 1, 2, 2, 2, 3, 3, 3 },
            new int[] { 2, 2, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
            new int[] { 2, 2, 3, 3, 3, 3, 3 },
            new int[] { 2, 2, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
        };

        public static int[][] ShieldWeaponSpeedTable = new int[][]
        {
            new int[] { 5, 5, 5, 5, 10, 10, 10 },
            new int[] { 5, 5, 10, 10, 10, 10, 10 },
            new int[] { 10, 10, 10, 10, 10, 10, 10 },
            new int[] {  },
            new int[] { 5, 5, 10, 10, 10, 10, 10 },
            new int[] { 10, 10, 10, 10, 10, 10, 10 },
        };

        public static int[][] ShieldSoulChargeTable = new int[][]
        {
            new int[] { 15, 20, 20, 20, 25, 25, 25 },
            new int[] { 20, 20, 25, 30, 30, 30, 30 },
            new int[] { 25, 30, 30, 30, 30, 30, 30 },
            new int[] {  },
            new int[] { 20, 20, 25, 30, 30, 30, 30 },
            new int[] { 25, 30, 30, 30, 30, 30, 30 },
        };
        #endregion
        #endregion

        #region Updates
        public static void ItemNerfVersion6()
        {
            int fc2 = 0;
            int eater = 0;
            int focus = 0;
            int brittle = 0;

            foreach (var jewel in World.Items.Values.OfType<BaseJewel>().Where(j => j.ItemPower > ItemPower.None))
            {
                if (jewel.Attributes.CastSpeed > 1)
                {
                    jewel.Attributes.CastSpeed = 1;
                    fc2++;
                }

                SAAbsorptionAttributes attr = GetSAAbsorptionAttributes(jewel);
                NegativeAttributes neg = GetNegativeAttributes(jewel);

                if (HasEater(jewel) && attr != null)
                {
                    if (attr != null)
                    {
                        if (attr.EaterKinetic > 0)
                            attr.EaterKinetic = 0;

                        if (attr.EaterFire > 0)
                            attr.EaterFire = 0;

                        if (attr.EaterCold > 0)
                            attr.EaterCold = 0;

                        if (attr.EaterPoison > 0)
                            attr.EaterPoison = 0;

                        if (attr.EaterEnergy > 0)
                            attr.EaterEnergy = 0;

                        if (attr.EaterDamage > 0)
                            attr.EaterDamage = 0;

                        eater++;
                    }
                }

                if (attr != null && attr.CastingFocus > 0)
                {
                    attr.CastingFocus = 0;
                    focus++;
                }

                if (neg != null && neg.Brittle > 0)
                {
                    neg.Brittle = 0;
                    neg.Antique = 1;
                    brittle++;
                }
            }

            SpawnerPersistence.ToConsole(String.Format("Cleauned up {0} items: {1} fc2, {2} non-Armor eater, {3} non armor casting focus, {4} brittle jewels converted to Antique.", fc2 + eater + focus + brittle, fc2, eater, focus, brittle));
        }
        #endregion
    }

    public class RunicReforgingTarget : Target
    {
        private BaseRunicTool m_Tool;

        public RunicReforgingTarget(BaseRunicTool tool)
            : base(-1, false, TargetFlags.None)
        {
            m_Tool = tool;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item && m_Tool != null)
            {
                Item item = targeted as Item;

                if (item is BaseRunicTool)
                {
                    if (item.IsChildOf(from.Backpack))
                    {
                        if (item == m_Tool)
                            from.SendLocalizedMessage(1010087); // You cannot use that!
                        else if (item.GetType() != m_Tool.GetType())
                            from.SendLocalizedMessage(1152274); // You may only combine runic tools of the same type.
                        else if (((BaseRunicTool)item).Resource != m_Tool.Resource)
                            from.SendLocalizedMessage(1152275); // You may only combine runic tools of the same material.
                        else if (m_Tool.UsesRemaining + ((BaseRunicTool)item).UsesRemaining > 100)
                            from.SendLocalizedMessage(1152276); // The combined charges of the two tools cannot exceed 100.
                        else
                        {
                            m_Tool.UsesRemaining += ((BaseRunicTool)item).UsesRemaining;
                            item.Delete();

                            from.SendLocalizedMessage(1152278); // You combine the runic tools, consolidating their Uses Remaining.
                        }
                    }
                    else
                        from.SendLocalizedMessage(1152277); // Both tools must be in your backpack in order to combine them.
                }
                else
                {
                    if (item.IsChildOf(from.Backpack))
                    {
                        if (RunicReforging.CanReforge(from, item, m_Tool.CraftSystem))
                            from.SendGump(new RunicReforgingGump(from, item, m_Tool));
                    }
                    else
                        from.SendLocalizedMessage(1152271); // The item must be in your backpack to re-forge it.
                }
            }
        }
    }
}
