using Server.Engines.Craft;
using Server.Gumps;
using Server.Mobiles;
using Server.SkillHandlers;
using Server.Targeting;
using System;
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
        Kotl,
        Khaldun,
        Doom,
        EnchantedOrigin,
        Fellowship
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
            bool allowableSpecial = m_AllowableTable.ContainsKey(item.GetType());
            CraftSystem system = null;

            if (!allowableSpecial)
            {
                system = CraftSystem.GetSystem(item.GetType());                
            }
            else
            {
                system = m_AllowableTable[item.GetType()];
            }

            bool goodtogo = true;

            if (system == null)
            {
                from.SendLocalizedMessage(1152113); // You cannot reforge that item.
                goodtogo = false;
            }
            else if (system != crsystem)
            {
                from.SendLocalizedMessage(1152279); // You cannot re-forge that item with this tool.
                goodtogo = false;
            }
            else
            {
                int mods = GetTotalMods(item);
                int maxmods = item is JukaBow ||
                    (item is BaseWeapon && !((BaseWeapon)item).DImodded) ||
                    (item is BaseArmor && ((BaseArmor)item).ArmorAttributes.MageArmor > 0 && BaseArmor.IsMageArmorType((BaseArmor)item)) ? 1 : 0;

                if (item is BaseWeapon &&
                    (((BaseWeapon)item).AosElementDamages[AosElementAttribute.Fire] > 0 ||
                    ((BaseWeapon)item).AosElementDamages[AosElementAttribute.Cold] > 0 ||
                    ((BaseWeapon)item).AosElementDamages[AosElementAttribute.Poison] > 0 ||
                    ((BaseWeapon)item).AosElementDamages[AosElementAttribute.Energy] > 0))
                {
                    mods++;
                }

                if (mods > maxmods)
                    goodtogo = false;
                else if (item is IResource && !CraftResources.IsStandard(((IResource)item).Resource))
                    goodtogo = false;
                else if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied)
                    goodtogo = false;
                else if (item is BaseWeapon && Spells.Mysticism.EnchantSpell.IsUnderSpellEffects(from, (BaseWeapon)item))
                    goodtogo = false;
                else if (item is BaseWeapon && ((BaseWeapon)item).FocusWeilder != null)
                    goodtogo = false;
                else if (!allowableSpecial && (item is IQuality && !((IQuality)item).PlayerConstructed))
                    goodtogo = false;
                else if (!allowableSpecial && item is BaseClothing && !(item is BaseHat))
                    goodtogo = false;
                else if (!allowableSpecial && item is BaseJewel)
                    goodtogo = false;
                else if (Imbuing.IsInNonImbueList(item.GetType()))
                    goodtogo = false;

                if (!goodtogo)
                {
                    from.SendLocalizedMessage(1152113); // You cannot reforge that item.
                }
            }

            return goodtogo;
        }

        public static void ApplyReforgedProperties(Item item, List<int> props, ReforgedPrefix prefix, ReforgedSuffix suffix, int budget, int perclow, int perchigh, int maxmods, int luckChance)
        {
            ApplyReforgedProperties(item, props, prefix, suffix, budget, perclow, perchigh, maxmods, luckChance, null, ReforgingOption.None);
        }

        public static void ApplyReforgedProperties(Item item, ReforgedPrefix prefix, ReforgedSuffix suffix, int budget, int perclow, int perchigh, int maxmods, int luckchance, BaseRunicTool tool, ReforgingOption option)
        {
            var props = new List<int>(ItemPropertyInfo.LookupLootTable(item));

            if (props.Count > 0)
            {
                ApplyReforgedProperties(item, props, prefix, suffix, budget, perclow, perchigh, maxmods, luckchance, tool, option);
            }

            ColUtility.Free(props);
        }

        public static void ApplyReforgedProperties(Item item, List<int> props, ReforgedPrefix prefix, ReforgedSuffix suffix, int budget, int perclow, int perchigh, int maxmods, int luckchance, BaseRunicTool tool, ReforgingOption option)
        {
            bool reforged = tool != null;
            bool powerful = reforged ? (option & ReforgingOption.Powerful) != 0 : IsPowerful(budget);

            if (prefix == ReforgedPrefix.None && (suffix == ReforgedSuffix.None || suffix > ReforgedSuffix.Aegis))
            {
                for (int i = 0; i < maxmods; i++)
                {
                    ApplyRandomProperty(item, props, perclow, perchigh, ref budget, luckchance, reforged, powerful);
                }

                if (suffix != ReforgedSuffix.None)
                {
                    ApplySuffixName(item, suffix);
                }
            }
            else
            {
                int prefixID = (int)prefix;
                int suffixID = (int)suffix;

                int index = GetCollectionIndex(item);
                int resIndex = -1;
                int preIndex = -1;
                // resIndex & preIndex = -1 indicates is not reforged

                if (reforged)
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
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: Prefix not in collection: {0}", prefixID);
                        Diagnostics.ExceptionLogging.LogException(e);
                    }
                }

                if (suffix != ReforgedSuffix.None)
                {
                    suffixCol = new List<NamedInfoCol>();

                    try
                    {
                        suffixCol.AddRange(m_PrefixSuffixInfo[suffixID][index]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: Suffix not in collection: {0}", suffixID);
                        Diagnostics.ExceptionLogging.LogException(e);
                    }
                }

                //Removes things like blood drinking/balanced/splintering
                ValidateAttributes(item, prefixCol, reforged);
                ValidateAttributes(item, suffixCol, reforged);

                int i = 0;
                int mods = 0;

                if (prefix != ReforgedPrefix.None && suffix == ReforgedSuffix.None && prefixCol != null)
                {
                    int specialAdd = 0;
                    int nothing = 0;
                    GetNamedModCount(prefixID, 0, maxmods, prefixCol.Count, 0, ref specialAdd, ref nothing);

                    while (budget > 25 && mods < maxmods && i < 25)
                    {
                        if (prefixCol.Count > 0 && specialAdd > 0)
                        {
                            int random = Utility.Random(prefixCol.Count);

                            if (ApplyPrefixSuffixAttribute(item, prefixCol[random], resIndex, preIndex, perclow, perchigh, ref budget, luckchance, reforged, powerful))
                            {
                                specialAdd--;
                                mods++;
                            }

                            prefixCol.RemoveAt(random);
                        }
                        else if (ApplyRandomProperty(item, props, perclow, perchigh, ref budget, luckchance, reforged, powerful))
                        {
                            mods++;
                        }

                        i++;
                    }

                    ApplyPrefixName(item, prefix);
                }
                else if (prefix == ReforgedPrefix.None && suffix != ReforgedSuffix.None && suffixCol != null)
                {
                    int specialAdd = 0;
                    int nothing = 0;
                    GetNamedModCount(0, suffixID, maxmods, 0, suffixCol.Count, ref nothing, ref specialAdd);

                    while (budget > 25 && mods < maxmods && i < 25)
                    {
                        if (suffixCol.Count > 0 && specialAdd > 0)
                        {
                            int random = Utility.Random(suffixCol.Count);

                            if (ApplyPrefixSuffixAttribute(item, suffixCol[random], resIndex, preIndex, perclow, perchigh, ref budget, luckchance, reforged, powerful))
                            {
                                specialAdd--;
                                mods++;
                            }

                            suffixCol.RemoveAt(random);
                        }
                        else if (ApplyRandomProperty(item, props, perclow, perchigh, ref budget, luckchance, reforged, powerful))
                        {
                            mods++;
                        }

                        i++;
                    }

                    ApplySuffixName(item, suffix);
                }
                else if (prefix != ReforgedPrefix.None && suffix != ReforgedSuffix.None && prefixCol != null && suffixCol != null)
                {
                    int specialAddPrefix = 0;
                    int specialAddSuffix = 0;

                    GetNamedModCount(prefixID, suffixID, maxmods, prefixCol.Count, suffixCol.Count, ref specialAddPrefix, ref specialAddSuffix);

                    while (budget > 25 && mods < maxmods && i < 25)
                    {
                        if (prefixCol.Count > 0 && specialAddPrefix > 0)
                        {
                            int random = Utility.Random(prefixCol.Count);

                            if (ApplyPrefixSuffixAttribute(item, prefixCol[random], resIndex, preIndex, perclow, perchigh, ref budget, luckchance, reforged, powerful))
                            {
                                specialAddPrefix--;
                                mods++;
                            }

                            prefixCol.RemoveAt(random);
                        }
                        else if (suffixCol.Count > 0 && specialAddSuffix > 0)
                        {
                            int random = Utility.Random(suffixCol.Count);

                            if (ApplyPrefixSuffixAttribute(item, suffixCol[random], resIndex, preIndex, perclow, perchigh, ref budget, luckchance, reforged, powerful))
                            {
                                specialAddSuffix--;
                                mods++;
                            }

                            suffixCol.RemoveAt(random);
                        }
                        else if (ApplyRandomProperty(item, props, perclow, perchigh, ref budget, luckchance, reforged, powerful))
                        {
                            mods++;
                        }

                        i++;
                    }

                    ApplyPrefixName(item, prefix);

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
            //if (index != 0 && (index == prefix || index == suffix))
            //    return false;HasOption(options, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulAndFundamental)

            ItemType type = ItemPropertyInfo.GetItemType(toreforge);

            if (type == ItemType.Melee)
            {
                switch (tool.Resource)
                {
                    case CraftResource.DullCopper:
                        if (index == 8 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.ShadowIron:
                        if (index == 8 && HasOption(options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental))
                            return false;
                        else if (index >= 8 && index <= 10 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Copper:
                        if (index == 8 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental))
                            return false;
                        if (index >= 8 && index <= 10 && HasOption(options, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Bronze:
                        if (index == 8)
                            return false;
                        if (index == 9 && HasOption(options, ReforgingOption.Powerful))
                            return false;
                        if (index >= 8 && index <= 10 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Gold:
                        if (index == 8)
                            return false;
                        if (index >= 8 && index <= 10 && HasOption(options, ReforgingOption.Powerful, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Agapite:
                    case CraftResource.Verite:
                        if (index >= 8 && index <= 10)
                            return false;
                        if (index == 12 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Valorite:
                        if (index >= 8 && index <= 10)
                            return false;
                        if (index == 12 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;

                    case CraftResource.OakWood:
                        if (index == 8 && HasOption(options, ReforgingOption.StructuralAndFundamental))
                            return false;
                        if (index >= 8 && index <= 10 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.AshWood:
                        if (index == 8)
                            return false;
                        if (index >= 8 && index <= 10 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.YewWood:
                        if (index >= 8 && index <= 10)
                            return false;
                        if (index == 12 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Heartwood:
                        if (index >= 8 && index <= 10)
                            return false;
                        if (index == 12 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                }
            }
            else if (type == ItemType.Ranged)
            {
                switch (tool.Resource)
                {
                    case CraftResource.OakWood:
                        if (index == 10 && HasOption(options, ReforgingOption.PowerfulAndStructural))
                            return false;
                        if ((index == 8 || index == 10) && HasOption(options, ReforgingOption.PowerfulAndFundamental))
                            return false;
                        if (index >= 8 && index <= 10 && (HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental) || HasOption(options, ReforgingOption.StructuralAndFundamental)))
                            return false;
                        break;
                    case CraftResource.AshWood:
                        if (index == 8 || index == 10)
                            return false;
                        if (index >= 8 && index <= 10 && HasOption(options, ReforgingOption.PowerfulAndStructural))
                            return false;
                        if (index >= 8 && index <= 11 && HasOption(options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.YewWood:
                        if (index >= 8 && index <= 11)
                            return false;
                        break;
                    case CraftResource.Heartwood:
                        if (index >= 8 && index <= 11)
                            return false;
                        if (index == 12 && HasOption(options, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                }
            }
            else if (type == ItemType.Shield)
            {
                if (index == 10)
                    return false;

                switch (tool.Resource)
                {
                    case CraftResource.DullCopper:
                        if (index == 8 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.ShadowIron:
                        if (index == 8 && HasOption(options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental))
                            return false;
                        if ((index == 8 || index == 9) && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Copper:
                        if (index == 8 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental))
                            return false;
                        else if ((index == 8 || index == 9) && HasOption(options, ReforgingOption.StructuralAndFundamental))
                            return false;
                        else if ((index == 5 || index == 8 || index == 9) && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Bronze:
                        if (index == 8)
                            return false;
                        else if (index == 9 && HasOption(options, ReforgingOption.PowerfulAndStructural))
                            return false;
                        else if ((index == 9 || index == 5) && HasOption(options, ReforgingOption.PowerfulAndFundamental))
                            return false;
                        else if ((index == 9 || index == 5 || index == 11) && HasOption(options, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Gold:
                        if (index == 8)
                            return false;
                        else if (index == 9 && HasOption(options, ReforgingOption.Powerful))
                            return false;
                        else if ((index == 5 || index == 9) && HasOption(options, ReforgingOption.PowerfulAndStructural))
                            return false;
                        else if ((index == 9 || index == 5 || index == 11) && HasOption(options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Agapite:
                        if (index == 8 || index == 9)
                            return false;
                        else if ((index == 5 || index == 11) && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Verite:
                    case CraftResource.Valorite:
                        if (index == 8 || index == 9 || index == 11)
                            return false;
                        else if (index == 5 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;

                    case CraftResource.OakWood:
                        if (index == 8 && HasOption(options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental))
                            return false;
                        if ((index == 8 || index == 9) && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.AshWood:
                        if (index == 8)
                            return false;
                        else if (index == 9 && HasOption(options, ReforgingOption.PowerfulAndStructural))
                            return false;
                        else if ((index == 9 || index == 5) && HasOption(options, ReforgingOption.PowerfulAndFundamental))
                            return false;
                        else if ((index == 9 || index == 5 || index == 11) && HasOption(options, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.YewWood:
                        if (index == 8 || index == 9)
                            return false;
                        else if ((index == 5 || index == 11) && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Heartwood:
                        if (index == 8 || index == 9 || index == 11)
                            return false;
                        else if (index == 5 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                }
            }
            else if (type == ItemType.Armor)
            {
                switch (tool.Resource)
                {
                    case CraftResource.DullCopper:
                        if ((index == 10 || index == 11) && HasOption(options, ReforgingOption.PowerfulAndFundamental))
                            return false;
                        else if ((index == 5 || index == 10 || index == 11) && HasOption(options, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.ShadowIron:
                        if ((index == 10 || index == 11) && HasOption(options, ReforgingOption.PowerfulAndStructural))
                            return false;
                        else if ((index == 5 || index == 10 || index == 11) && HasOption(options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental))
                            return false;
                        else if ((index == 5 || index == 9 || index == 10 || index == 11) && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Copper:
                        if (index == 10 || index == 11)
                            return false;
                        else if (index == 5 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental))
                            return false;
                        else if ((index == 5 || index == 9) && HasOption(options, ReforgingOption.StructuralAndFundamental))
                            return false;
                        else if ((index == 5 || index == 9 || index == 5) && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Bronze:
                        if (index == 10 || index == 11)
                            return false;
                        else if ((index == 5 || index == 9) && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental))
                            return false;
                        else if ((index == 5 || index == 9 || index == 12) && HasOption(options, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Gold:
                        if (index == 10 || index == 11)
                            return false;
                        else if (index == 9 && HasOption(options, ReforgingOption.Powerful))
                            return false;
                        else if ((index == 5 || index == 9 || index == 12) && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Agapite:
                        if (index >= 9 && index <= 11)
                            return false;
                        else if (index == 12 && HasOption(options, ReforgingOption.Powerful))
                            return false;
                        else if ((index == 12 || index == 5) && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Verite:
                    case CraftResource.Valorite:
                        if (index >= 9 && index <= 12)
                            return false;
                        else if (index == 5 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;

                    case CraftResource.SpinedLeather:
                        if ((index == 10 || index == 11) && HasOption(options, ReforgingOption.PowerfulAndStructural))
                            return false;
                        else if ((index == 10 || index == 11 || index == 5) && HasOption(options, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental))
                            return false;
                        else if ((index == 9 || index == 10 || index == 11 || index == 5) && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.HornedLeather:
                        if (index == 10 || index == 11)
                            return false;
                        else if (index == 9 && HasOption(options, ReforgingOption.Powerful))
                            return false;
                        else if ((index == 5 || index == 9 || index == 12) && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.BarbedLeather:
                        if (index >= 9 && index <= 12)
                            return false;
                        else if (index == 5 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;

                    case CraftResource.OakWood:
                        if ((index == 10 || index == 11) && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental))
                            return false;
                        else if (index == 9 && HasOption(options, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.AshWood:
                        if (index == 10 || index == 11)
                            return false;
                        else if ((index == 5 || index == 9) && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental))
                            return false;
                        else if ((index == 5 || index == 9 || index == 12) && HasOption(options, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.YewWood:
                        if (index == 9 || index == 10 || index == 11)
                            return false;
                        else if (index == 12 && HasOption(options, ReforgingOption.Powerful))
                            return false;
                        else if ((index == 5 || index == 12) && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                    case CraftResource.Heartwood:
                        if (index >= 9 && index <= 12)
                            return false;
                        else if (index == 5 && HasOption(options, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndStructural, ReforgingOption.PowerfulAndFundamental, ReforgingOption.StructuralAndFundamental, ReforgingOption.PowerfulStructuralAndFundamental))
                            return false;
                        break;
                }
            }

            return true;
        }

        public static bool HasOption(ReforgingOption options, params ReforgingOption[] optionArray)
        {
            foreach (ReforgingOption option in optionArray)
            {
                if ((options & option) == option)
                {
                    return true;
                }
            }

            return false;
        }

        private static void ValidateAttributes(Item item, List<NamedInfoCol> list, bool reforged)
        {
            if (list == null || list.Count == 0)
                return;

            list.IterateReverse(col =>
            {
                if (col != null && list.Contains(col) && !ItemPropertyInfo.ValidateProperty(item, col.Attribute, reforged))
                {
                    list.Remove(col);
                }
            });
        }

        private static void GetNamedModCount(int prefixID, int suffixID, int maxmods, int precolcount, int suffixcolcount, ref int prefixCount, ref int suffixCount)
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

        public static int GetPropertyCount(BaseRunicTool tool)
        {
            switch (tool.Resource)
            {
                case CraftResource.DullCopper:
                case CraftResource.ShadowIron: return Utility.RandomMinMax(1, 2);
                case CraftResource.Copper: return Utility.RandomMinMax(2, 3);
                case CraftResource.Bronze:
                case CraftResource.Gold: return 3;
                case CraftResource.Agapite:
                case CraftResource.Verite: return Utility.RandomMinMax(3, 4);
                case CraftResource.Valorite: return 5;

                case CraftResource.SpinedLeather: return Utility.RandomMinMax(1, 2);
                case CraftResource.HornedLeather: return 3;
                case CraftResource.BarbedLeather: return 5;

                case CraftResource.OakWood: return Utility.RandomMinMax(1, 2);
                case CraftResource.AshWood: return 2;
                case CraftResource.YewWood: return 3;
                case CraftResource.Heartwood: return 5;
            }

            return 1;
        }

        private static bool ApplyPrefixSuffixAttribute(Item item, NamedInfoCol col, int resIndex, int preIndex, int percLow, int percHigh, ref int budget, int luckchance, bool reforged, bool powerful)
        {
            int start = budget;
            object attribute = col.Attribute;

            // Converts Collection entry into actual attribute
            if (attribute is string)
            {
                switch ((string)attribute)
                {
                    case "RandomEater": attribute = GetRandomEater(); break;
                    case "HitSpell": attribute = GetRandomHitSpell(); break;
                    case "HitArea": attribute = GetRandomHitArea(); break;
                    case "Slayer": attribute = BaseRunicTool.GetRandomSlayer(); break;
                    case "WeaponVelocity": break;
                    case "ElementalDamage": attribute = GetRandomElemental(); break;
                }
            }

            int id = ItemPropertyInfo.GetID(attribute);

            // prop is invalid, or the item already has a value for this prop
            if (id == -1 || Imbuing.GetValueForID(item, id) > 0 || !ItemPropertyInfo.ValidateProperty(item, id, reforged))
            {
                return false;
            }

            if (reforged)
            {
                ApplyReforgedNameProperty(item, id, col, resIndex, preIndex, 0, 100, ref budget, luckchance, reforged, powerful);
            }
            else
            {
                ApplyProperty(item, id, percLow, percHigh, ref budget, luckchance, reforged, powerful); // TODO: powerful
            }

            return start != budget;
        }

        private static readonly Dictionary<Item, int[]> _Elements = new Dictionary<Item, int[]>();

        public static bool ApplyResistance(Item item, int value, AosElementAttribute attribute)
        {
            AosElementAttributes resists = GetElementalAttributes(item);

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

        public static int Scale(int min, int max, int perclow, int perchigh, int luckchance, bool reforged)
        {
            int percent;

            if (reforged)
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

        private static int CalculateValue(Item item, object attribute, int min, int max, int perclow, int perchigh, ref int budget, int luckchance)
        {
            return CalculateValue(item, attribute, min, max, perclow, perchigh, ref budget, luckchance, false);
        }

        private static int CalculateValue(Item item, object attribute, int min, int max, int perclow, int perchigh, ref int budget, int luckchance, bool reforged)
        {
            int scale = Math.Max(1, ItemPropertyInfo.GetScale(item, attribute, true));

            if (scale > 0 && min < scale)
            {
                min = scale;
            }

            int value = Scale(min, max, perclow, perchigh, luckchance, reforged);

            if (scale > 1 && value > scale)
            {
                value = (value / scale) * scale;
            }

            int totalweight = ItemPropertyInfo.GetTotalWeight(item, attribute, value);

            while (budget <= totalweight)
            {
                value -= scale;

                if (value <= 0)
                {
                    if (ItemPropertyInfo.GetTotalWeight(item, attribute, 3) > budget)
                        budget = 0;

                    return 0;
                }

                totalweight = ItemPropertyInfo.GetTotalWeight(item, attribute, value);
            }

            return value;
        }

        private static int GetTotalMods(Item item)
        {
            return Imbuing.GetTotalMods(item);
        }

        private static ItemPropertyInfo GetItemProps(object attr)
        {
            int id = -1;

            if (attr is AosAttribute)
                id = ItemPropertyInfo.GetIDForAttribute((AosAttribute)attr);

            else if (attr is AosWeaponAttribute)
                id = ItemPropertyInfo.GetIDForAttribute((AosWeaponAttribute)attr);

            else if (attr is SkillName)
                id = ItemPropertyInfo.GetIDForAttribute((SkillName)attr);

            else if (attr is SlayerName)
                id = ItemPropertyInfo.GetIDForAttribute((SlayerName)attr);

            else if (attr is SAAbsorptionAttribute)
                id = ItemPropertyInfo.GetIDForAttribute((SAAbsorptionAttribute)attr);

            else if (attr is AosArmorAttribute)
                id = ItemPropertyInfo.GetIDForAttribute((AosArmorAttribute)attr);

            else if (attr is AosElementAttribute)
                id = ItemPropertyInfo.GetIDForAttribute((AosElementAttribute)attr);

            if (ItemPropertyInfo.Table.ContainsKey(id))
                return ItemPropertyInfo.Table[id];

            return null;
        }

        private static int GetCollectionIndex(IEntity item)
        {
            if (item is BaseWeapon)
                return 0;
            if (item is BaseShield)
                return 2;
            if (item is BaseArmor || item is BaseClothing)
                return 1;
            if (item is BaseJewel)
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
                case CraftResource.SpinedLeather: return 3;
                case CraftResource.OakWood: return 4;
                case CraftResource.YewWood:
                case CraftResource.Heartwood:
                case CraftResource.Bloodwood:
                case CraftResource.Frostwood:
                case CraftResource.HornedLeather:
                case CraftResource.BarbedLeather:
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

            return perclow + (int)((perchi - perclow) * (GetPrerequisiteIndex(option) * 5.0 / 100.0));
        }

        private static readonly Dictionary<Type, CraftSystem> m_AllowableTable = new Dictionary<Type, CraftSystem>();
        private static readonly Dictionary<int, NamedInfoCol[][]> m_PrefixSuffixInfo = new Dictionary<int, NamedInfoCol[][]>();

        public static Dictionary<int, NamedInfoCol[][]> PrefixSuffixInfo => m_PrefixSuffixInfo;

        public static void Initialize()
        {
            m_AllowableTable[typeof(LeatherGlovesOfMining)] = DefTailoring.CraftSystem;
            m_AllowableTable[typeof(RingmailGlovesOfMining)] = DefBlacksmithy.CraftSystem;
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
            m_AllowableTable[typeof(BarbedWhip)] = DefTailoring.CraftSystem;
            m_AllowableTable[typeof(SpikedWhip)] = DefTailoring.CraftSystem;
            m_AllowableTable[typeof(BladedWhip)] = DefTailoring.CraftSystem;
        }

        public static void Configure()
        {
            Commands.CommandSystem.Register("GetCreatureScore", AccessLevel.GameMaster, e =>
                {
                    e.Mobile.BeginTarget(12, false, TargetFlags.None, (from, targeted) =>
                        {
                            if (targeted is BaseCreature)
                            {
                                ((BaseCreature)targeted).PrivateOverheadMessage(Network.MessageType.Regular, 0x25, false, GetDifficultyFor((BaseCreature)targeted).ToString(), e.Mobile.NetState);
                            }
                        });
                });

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
                        new NamedInfoCol(AosAttribute.RegenMana, WeaponRegenTable),
                        /*new NamedInfoCol(AosAttribute.LowerRegCost, LowerRegTable), */
                    },
                    new NamedInfoCol[] // armor
                    {
                        new NamedInfoCol(AosAttribute.BonusInt, DexIntTable),
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
                        new NamedInfoCol(AosAttribute.SpellDamage, 18),
                    },
                };
            m_PrefixSuffixInfo[5] = new NamedInfoCol[][]	// Exquisite
				{
                    new NamedInfoCol[] // Weapon
                    {
                        new NamedInfoCol(AosWeaponAttribute.SelfRepair, SelfRepairTable), //
                        new NamedInfoCol(AosWeaponAttribute.DurabilityBonus, DurabilityTable), //
                        new NamedInfoCol(AosWeaponAttribute.LowerStatReq, LowerStatReqTable), //
                        new NamedInfoCol("Slayer", 1), //
                        new NamedInfoCol(AosWeaponAttribute.MageWeapon, MageWeaponTable), // 
                        new NamedInfoCol(AosAttribute.SpellChanneling, 1), //
                        new NamedInfoCol(AosAttribute.BalancedWeapon, 1), //
                        new NamedInfoCol("WeaponVelocity", WeaponVelocityTable), // 
                        new NamedInfoCol("ElementalDamage", ElementalDamageTable), //
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
                        new NamedInfoCol(AosAttribute.SpellDamage, 18),
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
                Attribute = attr;
                Info = info;
                SecondaryInfo = secondary;
            }

            public NamedInfoCol(object attr, int hardcap)
            {
                Attribute = attr;
                HardCap = hardcap;
            }

            public int RandomRangedIntensity(Item item, int id, int resIndex, int preIndex)
            {
                if (Info == null || HardCap == 1)
                    return HardCap;

                int[] range = item is BaseRanged && SecondaryInfo != null ? SecondaryInfo[resIndex] : Info[resIndex];

                int max = range[preIndex];
                int min = Math.Max(ItemPropertyInfo.GetMinIntensity(item, id), (int)(range[0] * .75));
                int value;

                if (Utility.RandomBool())
                {
                    value = Utility.RandomBool() ? min : max;
                }
                else
                {
                    value = Utility.RandomMinMax(min, max);
                }

                int scale = ItemPropertyInfo.GetScale(item, id, true);

                if (scale > 1 && value > scale)
                {
                    value = (value / scale) * scale;
                }

                return value;
            }
        }

        public static object GetRandomHitSpell()
        {
            switch (Utility.Random(4))
            {
                default:
                case 0: return AosWeaponAttribute.HitMagicArrow;
                case 1: return AosWeaponAttribute.HitFireball;
                case 2: return AosWeaponAttribute.HitHarm;
                case 3: return AosWeaponAttribute.HitLightning;
                    //case 4: return AosWeaponAttribute.HitCurse;
            }
        }

        private static object GetRandomHitArea()
        {
            switch (Utility.Random(5))
            {
                default:
                case 0: return AosWeaponAttribute.HitPhysicalArea;
                case 1: return AosWeaponAttribute.HitFireArea;
                case 2: return AosWeaponAttribute.HitColdArea;
                case 3: return AosWeaponAttribute.HitPoisonArea;
                case 4: return AosWeaponAttribute.HitEnergyArea;
            }
        }

        private static object GetRandomEater()
        {
            switch (Utility.Random(6))
            {
                default:
                case 0: return SAAbsorptionAttribute.EaterKinetic;
                case 1: return SAAbsorptionAttribute.EaterFire;
                case 2: return SAAbsorptionAttribute.EaterCold;
                case 3: return SAAbsorptionAttribute.EaterPoison;
                case 4: return SAAbsorptionAttribute.EaterEnergy;
                case 5: return SAAbsorptionAttribute.EaterDamage;
            }
        }

        private static AosElementAttribute GetRandomElemental()
        {
            switch (Utility.Random(5))
            {
                default:
                case 0: return AosElementAttribute.Fire;
                case 1: return AosElementAttribute.Cold;
                case 2: return AosElementAttribute.Poison;
                case 3: return AosElementAttribute.Energy;
                case 4: return AosElementAttribute.Chaos;
            }
        }

        private static SkillName GetRandomSkill(Item item)
        {
            AosSkillBonuses skillbonuses = GetAosSkillBonuses(item);

            if (skillbonuses == null)
            {
                return SkillName.Alchemy;
            }

            SkillName[] possibleSkills = m_Skills;
            SkillName sk, check;

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

            return sk;
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

        public static int[][] NameTable => _NameTable;
        private static readonly int[][] _NameTable = {
            new int[] { 1151682, 1151683 }, // Might
            new int[] { 1151684, 1151685 }, // Mystic
            new int[] { 1151686, 1151687 }, // Animated
            new int[] { 1151688, 1151689 }, // Arcane
            new int[] { 1151690, 1151691 }, // Exquisite
            new int[] { 1151692, 1151693 }, // Vampiric
            new int[] { 1151694, 1151695 }, // Invigorating
            new int[] { 1151696, 1151697 }, // Fortified
            new int[] { 1151698, 1151699 }, // Auspicious
            new int[] { 1151700, 1151701 }, // Charmed
            new int[] { 1151702, 1151703 }, // Vicious
            new int[] { 1151704, 1151705 }, // Towering
            new int[] {       0, 1154548 }, // Blackthorn
            new int[] {       0, 1154507 }, // Minax
            new int[] {       0, 1156900 }, // Kotl
            new int[] {       0, 1158672 }, // Khaldun
            new int[] {       0, 1155589 }, // Doom
            new int[] {       0, 1157614 }, // Sorcerers Dungeon
            new int[] {       0, 1159317 }, // Fellowship
        };

        public static void AddSuffixName(ObjectPropertyList list, ReforgedSuffix suffix, string name)
        {
            if (suffix >= ReforgedSuffix.Blackthorn)
            {
                list.Add(GetSuffixName(suffix), name);
            }
            else
            {
                list.Add(1151758, string.Format("{0}\t#{1}", name, GetSuffixName(suffix)));// ~1_ITEM~ of ~2_SUFFIX~
            }
        }

        private static readonly SkillName[] m_Skills = {
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
        /// <param name="budget"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static bool GenerateRandomArtifactItem(Item item, int luck, int budget, ReforgedPrefix prefix = ReforgedPrefix.None, ReforgedSuffix suffix = ReforgedSuffix.None)
        {
            if (prefix == ReforgedPrefix.None)
                prefix = ChooseRandomPrefix(item, budget);

            if (suffix == ReforgedSuffix.None)
                suffix = ChooseRandomSuffix(item, budget);

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
        /// <param name="artifact"></param>
        public static void GenerateRandomItem(Item item, Mobile killer, int basebudget, int luckchance, ReforgedPrefix forcedprefix, ReforgedSuffix forcedsuffix, Map map = null, bool artifact = false)
        {
            if (map == null && killer != null)
            {
                map = killer.Map;
            }

            if (item != null)
            {
                int budget = basebudget;

                if (killer is BaseCreature bc && bc.Controlled)
                {
                    killer = bc.ControlMaster;
                }

                int rawLuck = killer != null ? killer is PlayerMobile ? ((PlayerMobile)killer).RealLuck : killer.Luck : 0;

                int mods = 0;
                int perclow = 0;
                int perchigh = 0;

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
                    if (rawLuck > 0 && !IsPowerful(budget) && LootPack.CheckLuck(luckchance / 6))
                    {
                        budget = Utility.RandomMinMax(600, 1150);
                    }

                    budget = Math.Min(RandomItemGenerator.MaxAdjustedBudget, budget);
                    budget = Math.Max(RandomItemGenerator.MinAdjustedBudget, budget);

                    if (!(item is BaseWeapon) && prefix == ReforgedPrefix.Vampiric)
                        prefix = ReforgedPrefix.None;

                    if (!(item is BaseWeapon) && suffix == ReforgedSuffix.Vampire)
                        suffix = ReforgedSuffix.None;

                    if (forcedprefix == ReforgedPrefix.None && budget >= Utility.Random(2700) && suffix < ReforgedSuffix.Minax)
                        prefix = ChooseRandomPrefix(item, budget);

                    if (forcedsuffix == ReforgedSuffix.None && budget >= Utility.Random(2700))
                        suffix = ChooseRandomSuffix(item, budget, prefix);

                    if (!IsPowerful(budget))
                    {
                        mods = Math.Max(1, GetProperties(5));

                        perchigh = Math.Max(50, Math.Min(500, budget) / mods);
                        perclow = Math.Max(20, perchigh / 3);
                    }
                    else
                    {
                        int maxmods = Math.Max(5, Math.Min(RandomItemGenerator.MaxProps - 1, (int)Math.Ceiling(budget / (double)Utility.RandomMinMax(100, 140))));
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

                var props = new List<int>(ItemPropertyInfo.LookupLootTable(item));
                bool powerful = IsPowerful(budget);

                ApplyReforgedProperties(item, props, prefix, suffix, budget, perclow, perchigh, mods, luckchance);

                int addonbudget = 0;

                if (!artifact)
                {
                    addonbudget = TryApplyRandomDisadvantage(item);
                }

                if (addonbudget > 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        ApplyRandomProperty(item, props, perclow, perchigh, ref addonbudget, luckchance, false, powerful);

                        if (addonbudget <= 0 || mods + (i + 1) >= RandomItemGenerator.MaxProps)
                            break;
                    }
                }

                NegativeAttributes neg = GetNegativeAttributes(item);

                if (neg != null)
                {
                    if (item is IDurability && (neg.Antique == 1 || neg.Brittle == 1 || item is BaseJewel))
                    {
                        ((IDurability)item).MaxHitPoints = 255;
                        ((IDurability)item).HitPoints = 255;
                    }

                    AosWeaponAttributes wepAttrs = GetAosWeaponAttributes(item);

                    if (wepAttrs != null && wepAttrs[AosWeaponAttribute.SelfRepair] > 0)
                    {
                        wepAttrs[AosWeaponAttribute.SelfRepair] = 0;
                    }

                    AosArmorAttributes armAttrs = GetAosArmorAttributes(item);

                    if (armAttrs != null && armAttrs[AosArmorAttribute.SelfRepair] > 0)
                    {
                        armAttrs[AosArmorAttribute.SelfRepair] = 0;
                    }
                }

                ItemPower power = ApplyItemPower(item, false);

                if (artifact && power < ItemPower.LesserArtifact)
                {
                    int extra = 5000;

                    do
                    {
                        ApplyRandomProperty(item, props, perclow, perchigh, ref extra, luckchance, false, powerful);
                    }
                    while (ApplyItemPower(item, false) < ItemPower.LesserArtifact);
                }

                // hues
                if (power == ItemPower.LegendaryArtifact && (item is BaseArmor || item is BaseClothing))
                {
                    item.Hue = 2500;
                }

                switch (suffix)
                {
                    case ReforgedSuffix.Minax: item.Hue = 1157; break;
                    case ReforgedSuffix.Khaldun: item.Hue = 2745; break;
                    case ReforgedSuffix.Kotl: item.Hue = 2591; break;
                    case ReforgedSuffix.EnchantedOrigin: item.Hue = 1171; break;
                    case ReforgedSuffix.Doom: item.Hue = 2301; break;
                    case ReforgedSuffix.Fellowship: item.Hue = 2751; break;
                }

                ColUtility.Free(props);
            }
        }

        private static bool IsPowerful(int budget)
        {
            return budget >= 550;
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

        private static void ChooseArtifactMods(IEntity item, int budget, out int mods, out int perclow, out int perchigh)
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

        private static readonly Dictionary<int, int> _Standard = new Dictionary<int, int>
        {
            { 1,  10 },
            { 2,  10 },
            { 3,  10 },
            { 4,  10 },
            { 5,  10 },
            { 7,  10 },
            { 8,  10 },
            { 9,  2 },
            { 10, 2 },
            { 11, 5 },
            { 12, 5 },
        };

        private static readonly Dictionary<int, int> _StandardPowerful = new Dictionary<int, int>
        {
            { 1,  10 },
            { 2,  10 },
            { 3,  10 },
            { 4,  10 },
            { 5,  10 },
            { 7,  10 },
            { 8,  10 },
            { 9,  0 },
            { 10, 0 },
            { 11, 2 },
            { 12, 2 },
        };

        private static readonly Dictionary<int, int> _Weapon = new Dictionary<int, int>
        {
            { 1,  10 },
            { 2,  10 },
            { 3,  10 },
            { 4,  10 },
            { 5,  10 },
            { 6,  10 },
            { 7,  10 },
            { 8,  10 },
            { 9,  2 },
            { 10, 2 },
            { 11, 5 },
            { 12, 5 },
        };

        private static readonly Dictionary<int, int> _WeaponPowerful = new Dictionary<int, int>
        {
            { 1,  10 },
            { 2,  10 },
            { 3,  10 },
            { 4,  10 },
            { 5,  10 },
            { 6,  10 },
            { 7,  10 },
            { 8,  10 },
            { 9,  0 },
            { 10, 0 },
            { 11, 2 },
            { 12, 2 },
        };

        public static ReforgedPrefix ChooseRandomPrefix(Item item, int budget)
        {
            return ChooseRandomPrefix(item, budget, ReforgedSuffix.None);
        }

        public static ReforgedPrefix ChooseRandomPrefix(Item item, int budget, ReforgedSuffix suffix)
        {
            Dictionary<int, int> table;
            bool powerful = budget > 600;

            if (item is BaseWeapon)
            {
                table = powerful ? _WeaponPowerful : _Weapon;
            }
            else
            {
                table = powerful ? _StandardPowerful : _Standard;
            }

            int random = GetRandomName(table);

            while (suffix != 0 && random == (int)suffix)
                random = GetRandomName(table);

            return (ReforgedPrefix)random;
        }

        public static ReforgedSuffix ChooseRandomSuffix(Item item, int budget)
        {
            return ChooseRandomSuffix(item, budget, ReforgedPrefix.None);
        }

        public static ReforgedSuffix ChooseRandomSuffix(Item item, int budget, ReforgedPrefix prefix)
        {
            //int random = item is BaseWeapon ? m_Weapon[Utility.Random(m_Weapon.Length)] : m_Standard[Utility.Random(m_Standard.Length)];
            Dictionary<int, int> table;
            bool powerful = budget > 600;

            if (item is BaseWeapon)
            {
                table = powerful ? _WeaponPowerful : _Weapon;
            }
            else
            {
                table = powerful ? _StandardPowerful : _Standard;
            }

            int random = GetRandomName(table);

            while (prefix != 0 && random == (int)prefix)
                random = GetRandomName(table);

            return (ReforgedSuffix)random;
        }

        private static int GetRandomName(Dictionary<int, int> table)
        {
            int total = 0;

            foreach (KeyValuePair<int, int> kvp in table)
            {
                total += kvp.Value;
            }

            int random = Utility.RandomMinMax(1, total);
            total = 0;

            foreach (KeyValuePair<int, int> kvp in table)
            {
                total += kvp.Value;

                if (total >= random)
                {
                    return kvp.Key;
                }
            }

            return 0;
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
            ItemPower power = GetItemPower(item, Imbuing.GetTotalWeight(item, -1, false, false), Imbuing.GetTotalMods(item), false);
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

                        switch (Utility.Random(item is BaseJewel ? 6 : 8))
                        {
                            case 0: neg.Prized = 1; break;
                            case 1: neg.Antique = 1; break;
                            case 2:
                            case 3: neg.Unwieldly = 1; break;
                            case 4:
                            case 5: item.LootType = LootType.Cursed; break;
                            case 6:
                            case 7: neg.Massive = 1; break;
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
                            switch (Utility.Random(item is BaseJewel ? 4 : 6))
                            {
                                case 0: neg.Prized = 1; break;
                                case 1: neg.Antique = 1; break;
                                case 2:
                                case 3: neg.Unwieldly = 1; break;
                                case 4:
                                case 5: neg.Massive = 1; break;
                            }

                            return 100;
                        }
                        else if (.5 > chance)
                        {
                            neg.Prized = 1;
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
                            switch (Utility.Random(item is BaseJewel ? 6 : 8))
                            {
                                case 0: neg.Prized = 1; break;
                                case 1: neg.Antique = 1; break;
                                case 2:
                                case 3: neg.Unwieldly = 1; break;
                                case 4:
                                case 5: item.LootType = LootType.Cursed; break;
                                case 6:
                                case 7: neg.Massive = 1; break;
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

        public static ItemPower ApplyItemPower(Item item, bool reforged)
        {
            ItemPower ip = GetItemPower(item, Imbuing.GetTotalWeight(item, -1, false, false), Imbuing.GetTotalMods(item), reforged);

            if (item is ICombatEquipment)
            {
                ((ICombatEquipment)item).ItemPower = ip;
            }

            return ip;
        }

        public static ItemPower GetItemPower(Item item, int weight, int totalMods, bool reforged)
        {
            // pre-arty uses max imbuing weight + 100
            // arty ranges from pre-arty to a flat 1200
            double preArty = Imbuing.GetMaxWeight(item) + 100;
            double arty = 1200 - preArty;

            if (totalMods == 0)
                return ItemPower.None;

            if (weight < preArty * .4)
                return reforged ? ItemPower.ReforgedMinor : ItemPower.Minor;

            if (weight < preArty * .6)
                return reforged ? ItemPower.ReforgedLesser : ItemPower.Lesser;

            if (weight < preArty * .8)
                return reforged ? ItemPower.ReforgedGreater : ItemPower.Greater;

            if (weight <= preArty)
                return reforged ? ItemPower.ReforgedGreater : ItemPower.Major;

            if (weight < preArty + (arty * .2))
                return reforged ? ItemPower.ReforgedMajor : ItemPower.LesserArtifact;

            if (weight < preArty + (arty * .4))
                return reforged ? ItemPower.ReforgedMajor : ItemPower.GreaterArtifact;

            if (weight < preArty + (arty * .7) || totalMods <= 5)
                return ItemPower.MajorArtifact;

            return reforged ? ItemPower.ReforgedLegendary : ItemPower.LegendaryArtifact;
        }

        private static bool ApplyRandomProperty(Item item, IList<int> props, int perclow, int perchigh, ref int budget, int luckchance, bool reforged, bool powerful)
        {
            if (props == null || props.Count == 0)
            {
                return false;
            }

            int id = -1;

            while (true)
            {
                int random = props[Utility.Random(props.Count)];

                if (random == 1000)
                {
                    random = ItemPropertyInfo.GetID(BaseRunicTool.GetRandomSlayer());
                }
                else if (random >= 1001 && id <= 1005)
                {
                    random = ItemPropertyInfo.GetID(GetRandomSkill(item));
                }

                if (Imbuing.GetValueForID(item, random) == 0 && ItemPropertyInfo.ValidateProperty(item, random, reforged))
                {
                    id = random;
                    break;
                }

                props.Remove(random);

                if (props.Count == 0)
                {
                    break;
                }
            }

            if (id == -1)
            {
                return false;
            }

            return ApplyProperty(item, id, perclow, perchigh, ref budget, luckchance, reforged, powerful);
        }

        /// <summary>
        /// unsafe applies property. Checks need to be made prior to calling this, see Imbuing.GetValueForID and ItemPropertyInfo.ValidateProperty
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <param name="perclow"></param>
        /// <param name="perchigh"></param>
        /// <param name="budget"></param>
        /// <param name="luckchance"></param>
        /// <param name="reforged"></param>
        /// <param name="powerful"></param>
        /// <returns></returns>
        public static bool ApplyProperty(Item item, int id, int perclow, int perchigh, ref int budget, int luckchance, bool reforged, bool powerful)
        {
            int min = ItemPropertyInfo.GetMinIntensity(item, id);
            int naturalMax = ItemPropertyInfo.GetMaxIntensity(item, id, false, true);
            int max = naturalMax;
            int[] overcap = null;

            if (powerful)
            {
                overcap = ItemPropertyInfo.GetMaxOvercappedRange(item, id);

                if (overcap != null)
                {
                    max = overcap[overcap.Length - 1];
                }
            }

            int value = CalculateValue(item, ItemPropertyInfo.GetAttribute(id), min, max, perclow, perchigh, ref budget, luckchance, reforged);

            // We're using overcap, so the value must have gone over the natural max, but under the overrcap max
            if (overcap != null && overcap.Length > 0 && value > naturalMax && value < max)
            {
                if (overcap.Length > 1)
                {
                    value = AdjustOvercap(overcap, value);
                }
                else
                {
                    value = naturalMax;
                }
            }

            Imbuing.SetProperty(item, id, value);
            budget -= Imbuing.GetIntensityForID(item, id, -1, value);

            return true;
        }

        public static bool ApplyReforgedNameProperty(Item item, int id, NamedInfoCol info, int resIndex, int preIndex, int perclow, int perchigh, ref int budget, int luckchance, bool reforged, bool powerful)
        {
            int value = info.RandomRangedIntensity(item, id, resIndex, preIndex);

            Imbuing.SetProperty(item, id, value);

            budget -= Imbuing.GetIntensityForID(item, id, -1, value);

            return true;
        }

        private static int AdjustOvercap(IReadOnlyList<int> overcap, int value)
        {
            for (int i = overcap.Count - 1; i >= 0; i--)
            {
                if (value >= overcap[i])
                {
                    return overcap[i];
                }
            }

            return overcap[0];
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

            if (item is FishingPole)
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

            if (item is ElvenGlasses)
                return ((ElvenGlasses)item).WeaponAttributes;

            if (item is BaseArmor)
                return ((BaseArmor)item).WeaponAttributes;

            if(item is BaseClothing)
                return ((BaseClothing)item).WeaponAttributes;

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
            if (item is IArtifact)
            {
                return ((IArtifact)item).ArtifactRarity;
            }

            return 0;
        }
        /* Reforging Test:
         * Powerful/Structural - Luck [30]
         * 150: 14
         * 140: 4
         * 120: 2
         * 100: 10
         *
        */
        #endregion

        #region Tables
        #region All
        public static readonly int[][] DexIntTable =
        {
            new int[] { 3, 4, 4, 4, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
        };

        public static readonly int[][] LowerStatReqTable =
        {
            new int[] { 60, 70, 80, 100, 100, 100, 100 },
            new int[] { 80, 100, 100, 100, 100, 100, 100 },
            new int[] { 100, 100, 100, 100, 100, 100, 100 },
            new int[] { 70, 100, 100, 100, 100, 100, 100 },
            new int[] { 80, 100, 100, 100, 100, 100, 100 },
            new int[] { 100, 100, 100, 100, 100, 100, 100 },
        };

        public static readonly int[][] SelfRepairTable =
        {
            new int[] { 2, 4, 0, 0, 0, 0, 0 },
            new int[] { 5, 5, 0, 0, 0, 0, 0 },
            new int[] { 6, 7, 0, 0, 0, 0, 0 },
            new int[] { 5, 5, 0, 0, 0, 0, 0 },
            new int[] { 5, 5, 0, 0, 0, 0, 0 },
            new int[] { 7, 7, 0, 0, 0, 0, 0 },
        };

        public static readonly int[][] DurabilityTable =
        {
            new int[] { 90, 100, 0, 0, 0, 0, 0 },
            new int[] { 110, 140, 0, 0, 0, 0, 0 },
            new int[] { 150, 150, 0, 0, 0, 0, 0 },
            new int[] { 100, 140, 0, 0, 0, 0, 0 },
            new int[] { 110, 140, 0, 0, 0, 0, 0 },
            new int[] { 150, 150, 0, 0, 0, 0, 0 },
        };

        public static readonly int[][] ResistTable =
        {
            new int[] { 10, 15, 15, 15, 20, 20, 20 },
            new int[] { 15, 15, 15, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
            new int[] { 15, 15, 20, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
        };

        public static readonly int[][] EaterTable =
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
        public static readonly int[][] ElementalDamageTable =
        {
            new int[] { 60, 70, 80, 100, 100, 100, 100 },
            new int[] { 80, 100, 100, 100, 100, 100, 100 },
            new int[] { 100, 100, 100, 100, 100, 100, 100 },
            new int[] {  },
            new int[] { 100, 100, 100, 100, 100, 100, 100 },
            new int[] { 100, 100, 100, 100, 100, 100, 100 },
        };

        // Hit magic, area, HLA
        public static readonly int[][] HitWeaponTable1 =
        {
            new int[] { 30, 50, 50, 60, 70, 70, 70 },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
            new int[] {  },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
        };

        // hit fatigue, mana drain, HLD
        public static readonly int[][] HitWeaponTable2 =
        {
            new int[] { 30, 40, 50, 50, 60, 70, 70 },
            new int[] { 50, 50, 50, 60, 70, 70, 70 },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] {  },
            new int[] { 50, 50, 50, 60, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
        };

        public static readonly int[][] WeaponVelocityTable =
        {
            new int[] { 25, 35, 40, 40, 40, 45, 50 },
            new int[] { 40, 40, 40, 45, 50, 50, 50 },
            new int[] { 40, 45, 50, 50, 50, 50, 50 },
            new int[] {  },
            new int[] { 40, 40, 40, 45, 50, 50, 50 },
            new int[] { 45, 50, 50, 50, 50, 50, 50 },
        };

        public static readonly int[][] HitsAndManaLeechTable =
        {
            new int[] { 15, 25, 25, 30, 35, 35, 35 },
            new int[] { 25, 25, 30, 35, 35, 35, 35 },
            new int[] { 30, 35, 35, 35, 35, 35, 35 },
            new int[] {  },
            new int[] { 25, 25, 30, 35, 35, 35, 35 },
            new int[] { 35, 35, 35, 35, 35, 35, 35 },
        };

        public static readonly int[][] HitStamLeechTable =
        {
            new int[] { 30, 50, 50, 60, 70, 70, 70 },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
            new int[] {  },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
        };

        public static readonly int[][] LuckTable =
        {
            new int[] { 80, 100, 100, 120, 140, 150, 150 },
            new int[] { 100, 120, 140, 150, 150, 150, 150 },
            new int[] { 130, 150, 150, 150, 150, 150, 150 },
            new int[] { 100, 120, 140, 150, 150, 150, 150 },
            new int[] { 100, 120, 140, 150, 150, 150, 150 },
            new int[] { 150, 150, 150, 150, 150, 150, 150 },
        };

        public static readonly int[][] MageWeaponTable =
        {
            new int[] { 25, 20, 20, 20, 20, 15, 15 },
            new int[] { 20, 20, 20, 15, 15, 15, 15 },
            new int[] { 20, 15, 15, 15, 15, 15, 15 },
            new int[] {  },
            new int[] { 20, 20, 20, 15, 15, 15, 15 },
            new int[] { 15, 15, 15, 15, 15, 15, 15 },
        };

        public static readonly int[][] WeaponRegenTable =
        {
            new int[] { 2, 3, 6, 6, 6, 6, 6 },
            new int[] { 3, 6, 6, 6, 6, 6, 6 },
            new int[] { 6, 6, 6, 6, 6, 9, 9 },
            new int[] {  },
            new int[] { 3, 6, 6, 6, 6, 6, 9 },
            new int[] { 6, 9, 9, 9, 9, 9, 9 },
        };

        public static readonly int[][] WeaponHitsTable =
        {
            new int[] { 2, 3, 3, 3, 4, 4, 4 },
            new int[] { 3, 3, 4, 4, 4, 4, 4 },
            new int[] { 4, 4, 4, 4, 4, 4, 4 },
            new int[] { },
            new int[] { 3, 3, 4, 4, 4, 4, 4 },
            new int[] { 4, 4, 4, 4, 4, 4, 4 },
        };

        public static readonly int[][] WeaponStamManaLMCTable =
        {
            new int[] { 2, 4, 4, 4, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
        };

        public static readonly int[][] WeaponStrTable =
        {
            new int[] { 2, 4, 4, 4, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] {  },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
        };

        public static readonly int[][] WeaponHCITable =
        {
            new int[] { 5, 10, 15, 15, 15, 20, 20 },
            new int[] { 15, 15, 15, 20, 20, 20, 20 },
            new int[] { 15, 20, 20, 20, 20, 20, 20 },
            new int[] {  },
            new int[] { 15, 15, 20, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
        };

        public static readonly int[][] WeaponDCITable =
        {
            new int[] { 10, 15, 15, 15, 20, 20, 20 },
            new int[] { 15, 15, 20, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
            new int[] {  },
            new int[] { 15, 15, 20, 20, 20, 20, 20 },
            new int[] { 20, 20, 20, 20, 20, 20, 20 },
        };

        public static readonly int[][] WeaponDamageTable =
        {
            new int[] { 30, 50, 50, 60, 70, 70, 70 },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
            new int[] {  },
            new int[] { 50, 60, 70, 70, 70, 70, 70 },
            new int[] { 70, 70, 70, 70, 70, 70, 70 },
        };

        public static readonly int[][] WeaponEnhancePots =
        {
            new int[] { 5, 10, 10, 10, 10, 15, 15 },
            new int[] { 10, 10, 10, 15, 15, 15, 15 },
            new int[] { 10, 15, 15, 15, 15, 15, 15 },
            new int[] {  },
            new int[] { 10, 10, 10, 15, 15, 15, 15 },
            new int[] { 15, 15, 15, 15, 15, 15, 15 },
        };

        public static readonly int[][] WeaponWeaponSpeedTable =
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
        public static readonly int[][] RangedLuckTable =
        {
            new int[] { 90, 120, 120, 140, 170, 170, 170 },
            new int[] { 120, 140, 160, 170, 170, 170, 170 },
            new int[] { 160, 170, 170, 170, 170, 170, 170 },
            new int[] {  },
            new int[] { 120, 140, 160, 170, 170, 170, 170 },
            new int[] { 170, 170, 170, 170, 170, 170, 170 },
        };

        public static readonly int[][] RangedHCITable =
        {
            new int[] { 15, 25, 25, 30, 35, 35, 35 },
            new int[] { 25, 30, 35, 35, 35, 35, 35 },
            new int[] { 35, 35, 35, 35, 35, 35, 35 },
            new int[] {  },
            new int[] { 25, 25, 30, 35, 35, 35, 35 },
            new int[] { 35, 35, 35, 35, 35, 35, 35 },
        };

        public static readonly int[][] RangedDCITable =
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
        public static readonly int[][] LowerRegTable =
        {
            new int[] { 10, 20, 20, 20, 25, 25, 25 },
            new int[] { 20, 20, 25, 25, 25, 25, 25 },
            new int[] { 25, 25, 25, 25, 25, 25, 25 },
            new int[] { 20, 20, 25, 25, 25, 25, 25 },
            new int[] { 20, 20, 25, 25, 25, 25, 25 },
            new int[] { 25, 25, 25, 25, 25, 25, 25 },
        };

        public static readonly int[][] ArmorHitsTable =
        {
            new int[] { 3, 5, 5, 6, 7, 7, 7 },
            new int[] { 5, 6, 7, 7, 7, 7, 7 },
            new int[] { 7, 7, 7, 7, 7, 7, 7 },
            new int[] { 5, 5, 6, 7, 7, 7, 7 },
            new int[] { 5, 6, 7, 7, 7, 7, 7 },
            new int[] { 7, 7, 7, 7, 7, 7, 7 },
        };

        public static readonly int[][] ArmorStrTable =
        {
            new int[] { 3, 4, 4, 4, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
        };

        public static readonly int[][] ArmorRegenTable =
        {
            new int[] { 2, 3, 3, 3, 4, 4, 4 },
            new int[] { 3, 3, 4, 4, 4, 4, 4 },
            new int[] { 4, 4, 4, 4, 4, 4, 4 },
            new int[] { 3, 3, 4, 4, 4, 4, 4 },
            new int[] { 3, 3, 4, 4, 4, 4, 4 },
            new int[] { 4, 4, 4, 4, 4, 4, 4 },
        };

        public static readonly int[][] ArmorStamManaLMCTable =
        {
            new int[] { 4, 8, 8, 8, 10, 10, 10 },
            new int[] { 8, 8, 10, 10, 10, 10, 10 },
            new int[] { 10, 10, 10, 10, 10, 10, 10 },
            new int[] { 8, 8, 10, 10, 10, 10, 10 },
            new int[] { 8, 8, 10, 10, 10, 10, 10 },
            new int[] { 10, 10, 10, 10, 10, 10, 10 },
        };

        public static readonly int[][] ArmorEnhancePotsTable =
        {
            new int[] { 2, 2, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
        };

        public static readonly int[][] ArmorHCIDCITable =
        {
            new int[] { 4, 4, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
            new int[] { 5, 5, 5, 5, 5, 5, 5 },
        };

        public static readonly int[][] ArmorCastingFocusTable =
        {
            new int[] { 1, 2, 2, 2, 3, 3, 3 },
            new int[] { 2, 2, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
            new int[] { 2, 2, 3, 3, 3, 3, 3 },
            new int[] { 2, 2, 3, 3, 3, 3, 3 },
            new int[] { 3, 3, 3, 3, 3, 3, 3 },
        };

        public static readonly int[][] ShieldWeaponSpeedTable =
        {
            new int[] { 5, 5, 5, 5, 10, 10, 10 },
            new int[] { 5, 5, 10, 10, 10, 10, 10 },
            new int[] { 10, 10, 10, 10, 10, 10, 10 },
            new int[] {  },
            new int[] { 5, 5, 10, 10, 10, 10, 10 },
            new int[] { 10, 10, 10, 10, 10, 10, 10 },
        };

        public static readonly int[][] ShieldSoulChargeTable =
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
        public static void LootNerf2()
        {
            int fix = 0;

            foreach (Item item in World.Items.Values)
            {
                NegativeAttributes neg = GetNegativeAttributes(item);

                if (neg != null && (neg.Brittle > 0 || neg.Antique > 0 || neg.NoRepair > 0))
                {
                    AosWeaponAttributes wep = GetAosWeaponAttributes(item);
                    AosArmorAttributes armor = GetAosArmorAttributes(item);

                    if (wep != null && wep.SelfRepair > 0)
                    {
                        wep.SelfRepair = 0;
                        fix++;
                    }

                    if (armor != null && armor.SelfRepair > 0)
                    {
                        armor.SelfRepair = 0;
                        fix++;
                    }
                }
            }

            SpawnerPersistence.ToConsole(string.Format("Removed Self Repair from {0} items.", fix));
        }

        public static void ItemNerfVersion6()
        {
            int fc2 = 0;
            int eater = 0;
            int focus = 0;
            int brittle = 0;

            foreach (BaseJewel jewel in World.Items.Values.OfType<BaseJewel>().Where(j => j.ItemPower > ItemPower.None))
            {
                if (jewel.Attributes.CastSpeed > 1)
                {
                    jewel.Attributes.CastSpeed = 1;
                    fc2++;
                }

                SAAbsorptionAttributes attr = GetSAAbsorptionAttributes(jewel);
                NegativeAttributes neg = GetNegativeAttributes(jewel);

                if (ItemPropertyInfo.HasEater(jewel) && attr != null)
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

            SpawnerPersistence.ToConsole(string.Format("Cleauned up {0} items: {1} fc2, {2} non-Armor eater, {3} non armor casting focus, {4} brittle jewels converted to Antique.", fc2 + eater + focus + brittle, fc2, eater, focus, brittle));
        }
        #endregion
    }

    public class RunicReforgingTarget : Target
    {
        private readonly BaseRunicTool m_Tool;

        public RunicReforgingTarget(BaseRunicTool tool)
            : base(-1, false, TargetFlags.None)
        {
            m_Tool = tool;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item && BaseTool.CheckAccessible(m_Tool, from, true))
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
                    {
                        from.SendLocalizedMessage(1152277); // Both tools must be in your backpack in order to combine them.
                    }
                }
                else if (item is ICombatEquipment)
                {
                    if (item.IsChildOf(from.Backpack))
                    {
                        if (RunicReforging.CanReforge(from, item, m_Tool.CraftSystem))
                        {
                            from.SendGump(new RunicReforgingGump(from, item, m_Tool));
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1152271); // The item must be in your backpack to re-forge it.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1152113); // You cannot reforge that item.
                }
            }
        }
    }
}
