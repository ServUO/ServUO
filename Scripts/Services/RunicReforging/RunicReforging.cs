using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.Craft;
using Server.SkillHandlers;
using Server.Misc;
using Server.Gumps;
using System.Collections.Generic;

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
        Aegis
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
            int maxmods = item is JukaBow ? 1 : 0;

            if(m_AllowableTable.ContainsKey(item.GetType()) && m_AllowableTable[item.GetType()] != crsystem)
                goodtogo = false;
            else if (mods > maxmods)
                goodtogo = false;
            else if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied)
                goodtogo = false;
            else if (item is BaseWeapon && Server.Spells.Mystic.EnchantSpell.IsUnderSpellEffects(from, (BaseWeapon)item))
                goodtogo = false;
            else if (item is BaseWeapon && ((BaseWeapon)item).FocusWeilder != null)
                goodtogo = false;
            else if (!allowableSpecial && ((item is BaseWeapon && !((BaseWeapon)item).PlayerConstructed) || (item is BaseArmor && !((BaseArmor)item).PlayerConstructed)))
                goodtogo = false;

            if (!goodtogo)
                from.SendLocalizedMessage(1152113); // You cannot reforge that item.

            return goodtogo;
        }

        public static void ApplyReforgedProperties(Item item, ReforgedPrefix prefix, ReforgedSuffix suffix, bool playermade, int budget, int perclow, int perchigh, int maxmods)
        {
            ApplyReforgedProperties(item, prefix, suffix, playermade, budget, perclow, perchigh, maxmods, 0, 0);
        }

        public static void ApplyReforgedProperties(Item item, ReforgedPrefix prefix, ReforgedSuffix suffix, bool playermade, int budget, int perclow, int perchigh, int maxmods, int luckchance)
        {
            ApplyReforgedProperties(item, prefix, suffix, playermade, budget, perclow, perchigh, maxmods, 0, luckchance);
        }

        public static void ApplyReforgedProperties(Item item, ReforgedPrefix prefix, ReforgedSuffix suffix, bool playermade, int budget, int perclow, int perchigh, int maxmods, int powermod, int luckchance)
		{
			if(prefix == ReforgedPrefix.None && suffix == ReforgedSuffix.None)
			{
                for(int i = 0; i < maxmods; i++)
                    ApplyRunicAttributes(item, perclow, perchigh, ref budget, i, luckchance);

                ApplyItemPower(item, playermade);
			}
			else
			{
				int prefixID = (int)prefix;
				int suffixID = (int)suffix;
				int index = GetCollectionIndex(item);
				
				if(index == -1)
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
                bool addedprefix = false;
                bool addedsuffix = false;

                int moddedPercLow = perclow - (int)((double)powermod / 2.0);
                int moddedPercHigh = perchigh - powermod;

                if (moddedPercLow < 0) moddedPercLow = 0;
                if (moddedPercHigh > 100) moddedPercHigh = 100;

				if(prefix != ReforgedPrefix.None && suffix == ReforgedSuffix.None && prefixCol != null)
				{
                    int specialAdd = GetModsPer(index, prefixID, maxmods, false);

					while(budget > 0 && mods < maxmods && i < 25)
					{
                        if (prefixCol.Count > 0 && specialAdd > 0)
						{
                            int random = Utility.Random(prefixCol.Count);

                            if (ApplyAttribute(item, prefixCol[random].Attribute, moddedPercLow, moddedPercHigh, prefixCol[random].Min, prefixCol[random].Max, ref budget, luckchance))
                            {
                                addedprefix = true;
                                specialAdd--;
                                mods++;
                            }

                            prefixCol.RemoveAt(random);
						}
                        else if (ApplyRunicAttributes(item, perclow, perchigh, ref budget, i, luckchance))
                            mods++;

                        i++;
					}

                    if(addedprefix)
					    ApplyPrefixName(item, prefix);
				}
                else if (prefix == ReforgedPrefix.None && suffix != ReforgedSuffix.None && suffixCol != null)
				{
                    int specialAdd = GetModsPer(index, suffixID, maxmods, false);

                    while (budget > 0 && mods < maxmods && i < 25)
					{
						if(suffixCol.Count > 0 && specialAdd > 0)
						{
                            int random = Utility.Random(suffixCol.Count);

                            if (ApplyAttribute(item, suffixCol[random].Attribute, moddedPercLow, moddedPercHigh, suffixCol[random].Min, suffixCol[random].Max, ref budget, luckchance))
                            {
                                addedsuffix = true;
                                specialAdd--;
                                mods++;
                            }

                            suffixCol.RemoveAt(random);
						}
                        else if (ApplyRunicAttributes(item, perclow, perchigh, ref budget, i, luckchance))
                            mods++;

                        i++;
					}

                    if(addedsuffix)
					    ApplySuffixName(item, suffix);
				}
                else if (prefix != ReforgedPrefix.None && suffix != ReforgedSuffix.None && prefixCol != null && suffixCol != null)
				{
                    int specialAddPrefix = GetModsPer(index, prefixID, maxmods, true);
                    int specialAddSuffix = GetModsPer(index, suffixID, maxmods, true);

                    while (budget > 0 && mods < maxmods && i < 25)
					{
                        if (prefixCol.Count > 0 && specialAddPrefix > 0)
						{
							int random = Utility.Random(prefixCol.Count);

                            if (ApplyAttribute(item, prefixCol[random].Attribute, moddedPercLow, moddedPercHigh, prefixCol[random].Min, prefixCol[random].Max, ref budget, luckchance))
                            {
                                addedprefix = true;
                                specialAddPrefix--;
                                mods++;
                            }

                            prefixCol.RemoveAt(random);
						}
                        else if (suffixCol.Count > 0 && specialAddSuffix > 0)
						{
                            int random = Utility.Random(suffixCol.Count);

                            if (ApplyAttribute(item, suffixCol[random].Attribute, moddedPercLow, moddedPercHigh, suffixCol[random].Min, suffixCol[random].Max, ref budget, luckchance))
                            {
                                addedsuffix = true;
                                specialAddSuffix--;
                                mods++;
                            }

                            suffixCol.RemoveAt(random);
						}
                        else if (ApplyRunicAttributes(item, perclow, perchigh, ref budget, i, luckchance))
                            mods++;

                        i++;
					}

                    if(addedprefix)
					    ApplyPrefixName(item, prefix);

                    if(addedsuffix)
					    ApplySuffixName(item, suffix);
				}

                ApplyItemPower(item, playermade);

                if (prefixCol != null)
                    prefixCol.Clear();

                if (suffixCol != null)
                    suffixCol.Clear();
			}

            //item.Hue = 25;
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
                else if (list.Contains(col) && col.Attribute is string && (string)col.Attribute == "BalancedWeapon")
                {
                    if (!(item is BaseRanged))
                        list.Remove(col);
                }
                else if (list.Contains(col) && col.Attribute is AosWeaponAttribute && (AosWeaponAttribute)col.Attribute == AosWeaponAttribute.SplinteringWeapon)
                {
                    if (playermade || item is BaseRanged)
                        list.Remove(col);
                }
                else if (list.Contains(col) && col.Attribute is AosWeaponAttributes && (AosWeaponAttribute)col.Attribute == AosWeaponAttribute.ReactiveParalyze)
                {
                    if ((item is BaseWeapon && item.Layer != Layer.TwoHanded) || (item is BaseShield && item.Layer != Layer.TwoHanded))
                        list.Remove(col);
                }
                else if (list.Contains(col) && col.Attribute is AosArmorAttribute && (AosArmorAttribute)col.Attribute == AosArmorAttribute.ReactiveParalyze)
                {
                    if ((item is BaseWeapon && item.Layer != Layer.TwoHanded) || (item is BaseShield && item.Layer != Layer.TwoHanded))
                        list.Remove(col);
                }
            }
        }

        private static int GetModsPer(int itemIndex, int prefixsuffixid, int maxmods, bool prefixandsuffix)
        {
            //Shilds with fortified/of defense
            if (itemIndex == 3 && prefixsuffixid == 8)
                return 1;

            switch (maxmods)
            {
                default:
                case 6:
                case 5: return prefixandsuffix ? 2 : 3;
                case 4: return prefixandsuffix ? Utility.RandomDouble() > .5 ? 1 : 2 : 3;
                case 3: return prefixandsuffix ? 1 : 2;
                case 2:
                case 1: return 1;
            }
        }

        private static bool ApplyAttribute(Item item, object attribute, int perclow, int perchigh, int min, int max, ref int budget, int luckchance)
		{
            int start = budget;

            if (CheckConflictingNegative(item, attribute))
                return false;

			if(attribute is string)
			{
				string str = attribute as string;
                if (str == "RandomEater" && !HasEater(item) && (item is BaseArmor || item is BaseJewel || item is BaseWeapon))
				{
				    budget -= ApplyRandomEater(item, perclow, perchigh, budget, luckchance);
				}
				else if (str == "HitSpell" && item is BaseWeapon && !HasHitSpell((BaseWeapon)item))
				{
                    budget -= ApplyRandomHitSpell((BaseWeapon)item, perclow, perchigh, budget, luckchance);
				}
                else if (str == "HitArea" && item is BaseWeapon && !HasHitArea((BaseWeapon)item))
				{
                    budget -= ApplyRandomHitArea((BaseWeapon)item, perclow, perchigh, budget, luckchance);
				}
                else if (str == "Slayer" && item is BaseWeapon && ((BaseWeapon)item).Slayer == SlayerName.None)
                {
                    SlayerName name = BaseRunicTool.GetRandomSlayer();
                    int weight = Imbuing.GetIntensityForAttribute(name, -1, 1);

                    if (weight <= budget)
                    {
                        ((BaseWeapon)item).Slayer = name;
                        budget -= weight;
                    }
                }
                else if (str == "BalancedWeapon" && item is BaseRanged)
                {
                    ((BaseRanged)item).Balanced = true;
                    budget -= 100;
                }
                else if (str == "WeaponVelocity" && item is BaseRanged)
                {
                    int value = CalculateValue(attribute, min, max, perclow, perchigh, ref budget, luckchance, true);

                    ((BaseRanged)item).Velocity = value;
                    budget -= 100;
                }
			}
			else if (attribute is AosAttribute)
			{
                //if ((AosAttribute)attribute == AosAttribute.Luck && item is BaseRanged)
                //    max = 180;

                int value = CalculateValue(attribute, min, max, perclow, perchigh, ref budget, luckchance, true);
                AosAttributes attrs = GetAosAttributes(item);

				if(attrs != null && value > 0 && attrs[(AosAttribute)attribute] == 0)
				{
					attrs[(AosAttribute)attribute] = value;
                    budget -= Imbuing.GetIntensityForAttribute((AosAttribute)attribute, -1, value);

                    if ((AosAttribute)attribute == AosAttribute.SpellChanneling && attrs[AosAttribute.CastSpeed] > -1)
                        attrs[AosAttribute.CastSpeed]--;
				}
			}
			else if (attribute is AosWeaponAttribute)
			{
                AosWeaponAttribute wepattr = (AosWeaponAttribute)attribute;
                int value = CalculateValue(attribute, min, max, perclow, perchigh, ref budget, luckchance, true);
                AosWeaponAttributes attrs = GetAosWeaponAttributes(item);
				
				if(attrs != null && value > 0 && attrs[wepattr] == 0)
				{
					attrs[wepattr] = value;
                    budget -= Imbuing.GetIntensityForAttribute(wepattr, -1, value);
				}
			}
            else if (attribute is AosArmorAttribute)
            {
                int value = CalculateValue(attribute, min, max, perclow, perchigh, ref budget, luckchance, true);
                AosArmorAttributes attrs = GetAosArmorAttributes(item);

                if (item is BaseArmor) attrs = ((BaseArmor)item).ArmorAttributes;
                else if (item is BaseClothing) attrs = ((BaseClothing)item).ClothingAttributes;

                if (attrs != null && value > 0 && attrs[(AosArmorAttribute)attribute] == 0)
                {
                    attrs[(AosArmorAttribute)attribute] = value;
                    budget -= Imbuing.GetIntensityForAttribute((AosArmorAttribute)attribute, -1, value);
                }
            }
            else if (attribute is SAAbsorptionAttribute)
            {
                int value = CalculateValue(attribute, min, max, perclow, perchigh, ref budget, luckchance, true);
                SAAbsorptionAttributes attrs = GetSAAbsorptionAttributes(item);

                if (attrs != null && value > 0 && attrs[(SAAbsorptionAttribute)attribute] == 0)
                {
                    attrs[(SAAbsorptionAttribute)attribute] = value;
                    budget -= Imbuing.GetIntensityForAttribute((SAAbsorptionAttribute)attribute, -1, value);
                }
            }
            else if (attribute is AosElementAttribute)
            {
                int value = CalculateValue(attribute, min, max, perclow, perchigh, ref budget, luckchance, true);

                if (value > 0)
                {
                    ApplyResistance(item, value, (AosElementAttribute)attribute);
                    budget -= Imbuing.GetIntensityForAttribute((AosElementAttribute)attribute, -1, value);
                }
            }

			return start != budget;
		}

        public static void ApplyResistance(Item item, int value, AosElementAttribute attribute)
        {
            if(item is BaseJewel)
                ((BaseJewel)item).Resistances[attribute] = value;
            else if (item is BaseClothing)
                ((BaseClothing)item).Resistances[attribute] = value;
            else
            {
                switch (attribute)
                {
                    default:
                    case AosElementAttribute.Physical:
                        if (item is BaseArmor) ((BaseArmor)item).PhysicalBonus = value;
                        else if (item is BaseWeapon) ((BaseWeapon)item).WeaponAttributes.ResistPhysicalBonus = value;
                        break;
                    case AosElementAttribute.Fire:
                        if (item is BaseArmor) ((BaseArmor)item).FireBonus = value;
                        else if (item is BaseWeapon) ((BaseWeapon)item).WeaponAttributes.ResistFireBonus = value;
                        break;
                    case AosElementAttribute.Cold:
                        if (item is BaseArmor) ((BaseArmor)item).ColdBonus = value;
                        else if (item is BaseWeapon) ((BaseWeapon)item).WeaponAttributes.ResistColdBonus = value; 
                        break;
                    case AosElementAttribute.Poison:
                        if (item is BaseArmor) ((BaseArmor)item).PoisonBonus = value;
                        else if (item is BaseWeapon) ((BaseWeapon)item).WeaponAttributes.ResistPoisonBonus = value; 
                        break;
                    case AosElementAttribute.Energy:
                        if (item is BaseArmor) ((BaseArmor)item).EnergyBonus = value;
                        else if (item is BaseWeapon) ((BaseWeapon)item).WeaponAttributes.ResistEnergyBonus = value; 
                        break;
                }
            }
        }

        public static int Scale(int min, int max, int perclow, int perchigh, int luckchance, bool usesqrt)
        {
            int percent = Utility.RandomMinMax(0, perchigh * 100);

            // this takes off the curve of generating better items.  Its the downfall of the old lootsystem, 
            // so lets just take it out and use a linear system like runic crafting
            usesqrt = false; 

            if (usesqrt)
                percent = (int)Math.Sqrt(percent);
            else
                percent /= 100;

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

        private static int CalculateValue(object attribute, int min, int max, int perclow, int perchigh, ref int budget, int luckchance, bool usesqrt)
		{
            int scale = ScaleAttribute(attribute);
            int value = Scale(min / scale, max / scale, perclow, perchigh, luckchance, usesqrt) * scale;
            int totalweight = GetTotalWeight(attribute, value);
				
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
		
		private static Dictionary<Type, CraftSystem> m_AllowableTable = new Dictionary<Type, CraftSystem>();
		private static Dictionary<int, NamedInfoCol[][]> m_PrefixSuffixInfo = new Dictionary<int, NamedInfoCol[][]>();
		
        public static void Initialize()
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
            //m_AllowableTable[typeof(GargishAmulet)] = DefBlacksmithy.CraftSystem;
            //m_AllowableTable[typeof(GargishStoneAmulet)] = DefMasonry.CraftSystem;

			// Index 0 - Weapon; 1 - Armor; 2 - Shield; 3 - Jewels
			m_PrefixSuffixInfo[0] = null;
			m_PrefixSuffixInfo[1] = new NamedInfoCol[][] 	//Might
				{
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLeechHits, 2, 70),
                        new NamedInfoCol(AosAttribute.BonusHits, 1, 7),
                        new NamedInfoCol(AosAttribute.BonusStr, 1, 10),
                        new NamedInfoCol(AosAttribute.RegenHits, 1, 9)
                    },

                    new NamedInfoCol[]
                    {
                        new NamedInfoCol("RandomEater", 1, 15),
                        new NamedInfoCol(AosAttribute.BonusHits, 1, 7),
                        new NamedInfoCol(AosAttribute.BonusStr, 1, 10),
                        new NamedInfoCol(AosAttribute.RegenHits, 1, 4)
                    },
					
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol("RandomEater", 1, 15),
                        new NamedInfoCol(AosAttribute.BonusHits, 1, 7),
                        new NamedInfoCol(AosAttribute.BonusStr, 1, 10),
                        new NamedInfoCol(AosAttribute.RegenHits, 1, 4)
                    },

                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusHits, 1, 7),
                        new NamedInfoCol(AosAttribute.BonusStr, 1, 10)
                    }
				};
				
			m_PrefixSuffixInfo[2] = new NamedInfoCol[][] 	//Mystic
				{
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLeechMana, 2, 70),
                        new NamedInfoCol(AosAttribute.BonusMana, 1, 10),
                        new NamedInfoCol(AosAttribute.BonusInt, 1, 10),
                        new NamedInfoCol(AosAttribute.LowerManaCost, 1, 10),
                        new NamedInfoCol(AosAttribute.LowerRegCost, 1, 9)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.LowerRegCost, 2, 25),
                        new NamedInfoCol(AosAttribute.BonusMana, 1, 10),
                        new NamedInfoCol(AosAttribute.LowerManaCost, 1, 10),
                        new NamedInfoCol(AosAttribute.RegenMana, 1, 4)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusMana, 1, 10),
                        new NamedInfoCol(AosAttribute.BonusInt, 1, 10),
                        new NamedInfoCol(AosAttribute.LowerManaCost, 1, 10),
                        new NamedInfoCol(AosAttribute.RegenMana, 1, 4)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusMana, 1, 10),
                        new NamedInfoCol(AosAttribute.BonusInt, 1, 10),
                        new NamedInfoCol(AosAttribute.LowerManaCost, 1, 10),
                        new NamedInfoCol(AosAttribute.LowerRegCost, 1, 25)
                    },
				};
				
			m_PrefixSuffixInfo[3] = new NamedInfoCol[][]	// Animated
				{
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLeechStam, 2, 70),
                        new NamedInfoCol(AosAttribute.BonusStam, 1, 10),
                        new NamedInfoCol(AosAttribute.BonusDex, 1, 10),
                        new NamedInfoCol(AosAttribute.RegenStam, 1, 9),
                        new NamedInfoCol(AosAttribute.WeaponSpeed, 10, 40)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusStam, 1, 10),
                        new NamedInfoCol(AosAttribute.BonusDex, 1, 10),
                        new NamedInfoCol(AosAttribute.RegenStam, 1, 4)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusStam, 1, 10),
                        new NamedInfoCol(AosAttribute.BonusDex, 1, 10),
                        new NamedInfoCol(AosAttribute.RegenStam, 1, 4),
                        new NamedInfoCol(AosAttribute.WeaponSpeed, 1, 10)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusStam, 1, 10),
                        new NamedInfoCol(AosAttribute.BonusDex, 1, 10),
                        new NamedInfoCol(AosAttribute.WeaponSpeed, 1, 10)
                    },
				};
			m_PrefixSuffixInfo[4] = new NamedInfoCol[][]	//Arcane
				{
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLeechMana, 2, 70),
                        new NamedInfoCol(AosWeaponAttribute.HitManaDrain, 2, 70),
                        new NamedInfoCol(AosAttribute.BonusMana, 1, 10),
                        new NamedInfoCol(AosAttribute.BonusInt, 1, 10),
                        new NamedInfoCol(AosAttribute.LowerManaCost, 1, 10),
                        new NamedInfoCol(AosAttribute.CastSpeed, 1, 1),
                        new NamedInfoCol(AosAttribute.SpellChanneling, 1, 1),
                        new NamedInfoCol(AosWeaponAttribute.MageWeapon, 1, 15),
                        new NamedInfoCol(AosAttribute.RegenMana, 1, 9)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusMana, 1, 10),
                        new NamedInfoCol(AosAttribute.BonusInt, 1, 10),
                        new NamedInfoCol(AosAttribute.LowerManaCost,1, 10),
                        new NamedInfoCol(AosAttribute.RegenMana, 1, 4),
                        new NamedInfoCol(AosAttribute.LowerRegCost, 1, 25)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusMana, 1, 10),
                        new NamedInfoCol(AosAttribute.BonusInt, 1, 10),
                        new NamedInfoCol(AosAttribute.LowerManaCost,1, 10),
                        new NamedInfoCol(AosAttribute.RegenMana, 1, 4),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.BonusMana, 1, 10),
                        new NamedInfoCol(AosAttribute.BonusInt, 1, 10),
                        new NamedInfoCol(AosAttribute.LowerManaCost,1, 10),
                        new NamedInfoCol(AosAttribute.RegenMana, 1, 4),
                        new NamedInfoCol(AosAttribute.LowerRegCost, 1, 25),
                        new NamedInfoCol(AosAttribute.CastSpeed, 1, 2),
                        new NamedInfoCol(AosAttribute.CastRecovery, 1, 4),
                        new NamedInfoCol(AosAttribute.SpellDamage, 1, 15),
                    },
				};
			m_PrefixSuffixInfo[5] = new NamedInfoCol[][]	// Exquisite
				{
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosWeaponAttribute.SelfRepair, 1, 7),
                        new NamedInfoCol(AosWeaponAttribute.DurabilityBonus, 10, 100),
                        new NamedInfoCol(AosWeaponAttribute.HitLowerDefend, 1, 70),
                        new NamedInfoCol(AosWeaponAttribute.LowerStatReq, 10, 100),
                        new NamedInfoCol("Slayer", 1, 1),
                        new NamedInfoCol(AosWeaponAttribute.MageWeapon, 1, 15),
                        new NamedInfoCol(AosAttribute.SpellChanneling, 1, 1),
                        new NamedInfoCol("BalancedWeapon", 1, 1),
                        new NamedInfoCol("WeaponVelocity", 1, 70)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosArmorAttribute.SelfRepair, 1, 7),
                        new NamedInfoCol(AosArmorAttribute.DurabilityBonus, 10, 100),
                        new NamedInfoCol(AosArmorAttribute.LowerStatReq, 10, 100),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosArmorAttribute.SelfRepair, 1, 7),
                        new NamedInfoCol(AosArmorAttribute.DurabilityBonus, 10, 100),
                        new NamedInfoCol(AosArmorAttribute.LowerStatReq, 10, 100),
                    },
                    new NamedInfoCol[]
                    {
                    },
				};
			m_PrefixSuffixInfo[6] = new NamedInfoCol[][]	//Vampiric
				{
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLeechHits, 2, 70),
                        new NamedInfoCol(AosWeaponAttribute.HitLeechStam, 2, 70),
                        new NamedInfoCol(AosWeaponAttribute.HitLeechMana, 2, 70),
                        new NamedInfoCol(AosWeaponAttribute.HitManaDrain, 2, 70),
                        new NamedInfoCol(AosWeaponAttribute.HitFatigue, 2, 70),
                        new NamedInfoCol(AosWeaponAttribute.BloodDrinker, 1, 1),
                    },
                    new NamedInfoCol[]
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
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.RegenHits, 1, 9),
                        new NamedInfoCol(AosAttribute.RegenStam, 1, 9),
                        new NamedInfoCol(AosAttribute.RegenMana, 1, 9),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.RegenHits, 1, 4),
                        new NamedInfoCol(AosAttribute.RegenStam, 1, 4),
                        new NamedInfoCol(AosAttribute.RegenMana, 1, 4),
                        new NamedInfoCol("RandomEater", 1, 15)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.RegenHits, 1, 4),
                        new NamedInfoCol(AosAttribute.RegenStam, 1, 4),
                        new NamedInfoCol(AosAttribute.RegenMana, 1, 4),
                        new NamedInfoCol(AosArmorAttribute.SoulCharge, 1, 30),
                        new NamedInfoCol("RandomEater", 1, 15)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.RegenHits, 1, 4),
                        new NamedInfoCol(AosAttribute.RegenStam, 1, 4),
                        new NamedInfoCol(AosAttribute.RegenMana, 1, 4),
                        new NamedInfoCol("RandomEater", 1, 15)
                    },
				};
			m_PrefixSuffixInfo[8] = new NamedInfoCol[][]	// Fortified
				{
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosWeaponAttribute.ResistPhysicalBonus, 1, 20),
                        new NamedInfoCol(AosWeaponAttribute.ResistFireBonus, 1, 20),
                        new NamedInfoCol(AosWeaponAttribute.ResistColdBonus, 1, 20),
                        new NamedInfoCol(AosWeaponAttribute.ResistPoisonBonus, 1, 20),
                        new NamedInfoCol(AosWeaponAttribute.ResistEnergyBonus, 1, 20),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosElementAttribute.Physical, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Fire, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Cold, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Poison, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Energy, 1, 20),
                        new NamedInfoCol("RandomEater", 1, 15)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosElementAttribute.Physical, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Fire, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Cold, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Poison, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Energy, 1, 20),
                        new NamedInfoCol("RandomEater", 1, 15)
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosElementAttribute.Physical, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Fire, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Cold, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Poison, 1, 20),
                        new NamedInfoCol(AosElementAttribute.Energy, 1, 20),
                    },
				};
			m_PrefixSuffixInfo[9] = new NamedInfoCol[][]	// Auspicious
				{
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.Luck, 10, 150),
                    },
					new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.Luck, 10, 150),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.Luck, 10, 150),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.Luck, 10, 150),
                    },
				};
			m_PrefixSuffixInfo[10] = new NamedInfoCol[][]	// Charmed
				{
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.EnhancePotions, 1, 15),
                        new NamedInfoCol("BalancedWeapon", 1, 1),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.EnhancePotions, 1, 7),
                    },
                    new NamedInfoCol[]
                    {
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.EnhancePotions, 1, 7),
                    },
				};
			m_PrefixSuffixInfo[11] = new NamedInfoCol[][]	//Vicious
				{
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol("HitSpell", 2, 70),
                        new NamedInfoCol("HitArea", 2, 70),
                        new NamedInfoCol(AosAttribute.AttackChance, 1, 25),
                        new NamedInfoCol(AosAttribute.WeaponDamage, 1, 70),
                        new NamedInfoCol(AosWeaponAttribute.BattleLust, 1, 1),
                        new NamedInfoCol(AosWeaponAttribute.SplinteringWeapon, 5, 30),
                        new NamedInfoCol("Slayer", 1, 1),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.AttackChance, 1, 5),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.AttackChance, 1, 20),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.AttackChance, 1, 20),
                        new NamedInfoCol(AosAttribute.SpellDamage, 1, 15),
                    }, 
				};
			m_PrefixSuffixInfo[12] = new NamedInfoCol[][]	// Towering
				{
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosWeaponAttribute.HitLowerAttack, 1, 70),
                        new NamedInfoCol(AosWeaponAttribute.ReactiveParalyze, 1, 1),
                        new NamedInfoCol(AosAttribute.DefendChance, 1, 25),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.DefendChance, 1, 5),
                        new NamedInfoCol(SAAbsorptionAttribute.CastingFocus, 1, 3),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.DefendChance, 1, 20),
                        new NamedInfoCol(AosArmorAttribute.ReactiveParalyze, 1, 1),
                        new NamedInfoCol(AosArmorAttribute.SoulCharge, 1, 30),
                    },
                    new NamedInfoCol[]
                    {
                        new NamedInfoCol(AosAttribute.DefendChance, 1, 20),
                        new NamedInfoCol(SAAbsorptionAttribute.CastingFocus, 1, 3),
                    },
				};
        }
		
		public class NamedInfoCol
		{
			private object m_Attribute;
			private int m_Min;
			private int m_Max;
			
			public object Attribute { get { return m_Attribute; } }
			public int Min { get { return m_Min; } }
			public int Max { get { return m_Max; } }
			
			public NamedInfoCol(object attr, int min, int max)
			{
				m_Attribute = attr;
				m_Min = min;
				m_Max = max;
			}
		}

        private static int ApplyRandomHitSpell(BaseWeapon weapon, int perclow, int perchigh, int budget, int luckchance)
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

            int value = CalculateValue(attr, 2, 70, perclow, perchigh, ref budget, luckchance, true);
            weapon.WeaponAttributes[(AosWeaponAttribute)attr] = value;

			return (140 / 50) * value;
        }

        private static int ApplyRandomHitArea(BaseWeapon weapon, int perclow, int perchigh, int budget, int luckchance)
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

            int value = CalculateValue(attr, 2, 70, perclow, perchigh, ref budget, luckchance, true);
            weapon.WeaponAttributes[(AosWeaponAttribute)attr] = value;

            return (100 / 50) * value;
        }

        private static int ApplyRandomEater(Item item, int perclow, int perchigh, int budget, int luckchance)
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

            int value = CalculateValue(attr, 1, 15, perclow, perchigh, ref budget, luckchance, true);

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
        /// <param name="killer">who killed them. Their luck is taken into account</param>
        /// <param name="minBudget"></param>
        /// <param name="maxBudget"></param>
        /// <returns></returns>
        public static bool GenerateRandomItem(Item item, int luckChance, int minBudget, int maxBudget)
        {
            if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
            {
                int budget = Utility.RandomMinMax(minBudget, maxBudget);
                //Utility.WriteLine(ConsoleColor.Magenta, String.Format("Creating Random Item: {0}, BUDGET: {1}", item.GetType().Name, budget));
                GenerateRandomItem(item, null, budget, luckChance, ReforgedPrefix.None, ReforgedSuffix.None);
                //Utility.WriteLine(ConsoleColor.Green, String.Format("Total Weight: {0}", Imbuing.GetTotalWeight(item)));
                return true;
            }
            return false;
        }

        public static Item GenerateRandomItem(Mobile killer, BaseCreature creature)
        {
			Item item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(LootPackEntry.IsInTokuno(killer), LootPackEntry.IsMondain(killer), LootPackEntry.IsStygian(killer));

            if(item != null)
                GenerateRandomItem(item, killer, Math.Max(100, GetDifficultyFor(creature)), LootPack.GetLuckChanceForKiller(creature), ReforgedPrefix.None, ReforgedSuffix.None);

            return item;
        }

        /// <summary>
        /// Called in LootPack.cs
        /// </summary>
        public static bool GenerateRandomItem(Item item, Mobile killer, BaseCreature creature)
        {
            if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
            {
                GenerateRandomItem(item, killer, Math.Max(100, GetDifficultyFor(creature)), LootPack.GetLuckChanceForKiller(creature), ReforgedPrefix.None, ReforgedSuffix.None);
                return true;
            }

            return false;
        }

        public static bool GenerateRandomItem(Item item, Mobile killer, BaseCreature creature, ReforgedPrefix prefix, ReforgedSuffix suffix)
        {
            if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
            {
                GenerateRandomItem(item, killer, Math.Max(100, GetDifficultyFor(creature)), LootPack.GetLuckChanceForKiller(creature), prefix, suffix);
                return true;
            }
            return false;
        }

        public static void GenerateRandomItem(Item item, Mobile killer, int basebudget, int luckchance, ReforgedPrefix forcedprefix, ReforgedSuffix forcedsuffix)
        {
            int luckbonus = 0;
            int budgetBonus = 0;

            if (killer != null)
            {
                luckbonus = killer.Luck;
                if (killer.Map == Map.Felucca)
                {
                    luckbonus += RandomItemGenerator.FeluccaLuckBonus;
                    budgetBonus += RandomItemGenerator.FeluccaBudgetBonus;
                }
            }
            else
            {
                luckbonus = 240;
            }

            if (item != null)
			{
                // base budget range
                int budget = Utility.RandomMinMax(basebudget - (basebudget / 4), basebudget);

                // Gives a rare chance for a high end item to drop on a low budgeted monster
				if(Utility.RandomDouble() < (double)luckbonus / 2000.0)
					budget += Utility.RandomMinMax(150, Math.Max(160, 400 - basebudget));

                budget += budgetBonus;

                TryApplyRandomDisadvantage(item, ref budget);

                if (budget > 900)
                    budget = 900;

                bool powerful = budget >= 550;

				ReforgedPrefix prefix = forcedprefix;
				ReforgedSuffix suffix = forcedsuffix;

                if (!(item is BaseWeapon) && prefix == ReforgedPrefix.Vampiric)
                    prefix = ReforgedPrefix.None;

                if (!(item is BaseWeapon) && suffix == ReforgedSuffix.Vampire)
                    suffix = ReforgedSuffix.None;

				if(forcedprefix == ReforgedPrefix.None && budget >= Utility.Random(2100))
					prefix = ChooseRandomPrefix(item);

                if (forcedsuffix == ReforgedSuffix.None && budget >= Utility.Random(2100))
					suffix = ChooseRandomSuffix(item, prefix);

                int mods;
                int perclow;
                int perchigh;

                if (!powerful)
                {
                    if (prefix == ReforgedPrefix.None && suffix == ReforgedSuffix.None)
                        mods = Utility.RandomMinMax(4, 5);
                    else
                        mods = Utility.RandomMinMax(2, 4);

                    perchigh = Math.Max(20, Math.Min(500, budget) / mods);
                    perclow = Math.Max(10, perchigh / 3);
                }
                else
                {
                    int maxmods = Math.Min(9, budget / Utility.RandomMinMax(100, 140));
                    int minmods = Math.Max(5, maxmods - 1);
                    mods = Utility.RandomMinMax(minmods, maxmods);

                    perchigh = 100;
                    perclow = Math.Max(50, (budget / 2) / mods);
                }

                if (perchigh > 100)
                    perchigh = 100;

                if (perclow < 10)
                    perclow = 10;

                ApplyReforgedProperties(item, prefix, suffix, false, budget, perclow, perchigh, mods, luckchance);
			}
        }

        private static int[] m_Standard = new int[] { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12 };
        private static int[] m_Weapon = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
		
		public static ReforgedPrefix ChooseRandomPrefix(Item item)
		{
            if (item is BaseWeapon)
                return (ReforgedPrefix)m_Weapon[Utility.Random(m_Weapon.Length)];

            return (ReforgedPrefix)m_Standard[Utility.Random(m_Standard.Length)];
		}

        public static ReforgedSuffix ChooseRandomSuffix(Item item)
        {
            return ChooseRandomSuffix(item, ReforgedPrefix.None);
        }

		public static ReforgedSuffix ChooseRandomSuffix(Item item, ReforgedPrefix prefix)
		{
            int random = item is BaseWeapon ? m_Weapon[Utility.Random(m_Weapon.Length)] : m_Standard[Utility.Random(m_Standard.Length)];

            while((int)prefix != 0 && random == (int)prefix)
                random = item is BaseWeapon ? m_Weapon[Utility.Random(m_Weapon.Length)] : m_Standard[Utility.Random(m_Standard.Length)];

            return (ReforgedSuffix)random;

		}
		
        public static int GetDifficultyFor(BaseCreature bc)
		{
            if (bc == null)
                return 100;

			double val = (bc.HitsMax * 1.6) + bc.StamMax + bc.ManaMax;

			val += bc.SkillsTotal / 10;

			if ( val > 600 )
				val = 600 + (int)((val - 600) * (3.0 / 11));

			if ( IsSpellCastingCreature(bc) )
				val += 100;

            if ( bc.HasBreath )
				val += 100;

            if (bc.PoisonImmune != null)
				val += 100;

			val += BaseInstrument.GetPoisonLevel( bc ) * 20;

			val /= 5;

			if (bc.IsParagon )
				val += 50.0;

			if(val > 400) val = 400;

			val *= 2;

			return (int)val;
		}

        private static bool IsSpellCastingCreature(BaseCreature bc)
        {
            AIType type = bc.AI;

            switch (type) {
                default: return false;
                case AIType.AI_Mage: return bc.Skills[SkillName.Magery].Value >= 10;
                //case AIType.AI_Necro: return bc.Skills[SkillName.Necromancy].Value >= 10;
                case AIType.AI_NecroMage: return bc.Skills[SkillName.Necromancy].Value >= 10;
                case AIType.AI_Spellweaving: return bc.Skills[SkillName.Spellweaving].Value >= 10;
                //case AIType.AI_Paladin: return bc.Skills[SkillName.Chivalry].Value >= 10;
                case AIType.AI_Mystic: return bc.Skills[SkillName.Mysticism].Value >= 10;
                case AIType.AI_Spellbinder: return bc.Skills[SkillName.Magery].Value >= 10;
                case AIType.AI_Ninja: return bc.Skills[SkillName.Ninjitsu].Value >= 10;
                case AIType.AI_Samurai: return bc.Skills[SkillName.Bushido].Value >= 10;
            }
        }

        /* LegendaryArtifact:
         * None: NEVER
         * Antique: Common
         * Brittle: Less Common
         * 
         * GreaterArtifact:
         * None: Never
         * Antique/Prized: Common
         * Brittle: Uncommon
         * 
         * LesserArtifact:
         * None: Very Uncommon (-.1%)
         * Prized: Common
         * Antique: Uncommon
         * Brittle: Rare
         * 
         * MajorMagicItem: 
         * None: semi common
         * prized: semi common
         * Antique: uncommon
         * Unlucky: uncommon
         * Brittle: Rare
         * 
         * GreaterMagicItem:
         * None: common (60%)
         * Prized: uncommon
         * Unluck: uncommon
         * Brittle/Antique: rare
         * 
         * LesserMagicItem:
         * None: very common(75%)
         * Prized/Antique/Unlucky: Uncommon
         * 
         * Minor Magic Item
         * None: most common: (95%)
         * Prized/Antique/Unlucky: Uncommon
         */ 

        private static void TryApplyRandomDisadvantage(Item item, ref int budget)
        {
            AosAttributes attrs = null;
            NegativeAttributes neg = null;

            if (item is BaseWeapon)
            {
                attrs = ((BaseWeapon)item).Attributes;
                neg = ((BaseWeapon)item).NegativeAttributes;
            }
            else if (item is BaseArmor)
            {
                attrs = ((BaseArmor)item).Attributes;
                neg = ((BaseArmor)item).NegativeAttributes;
            }
            else if (item is BaseJewel)
            {
                attrs = ((BaseJewel)item).Attributes;
                neg = ((BaseJewel)item).NegativeAttributes;
            }
            else if (item is BaseClothing)
            {
                attrs = ((BaseClothing)item).Attributes;
                neg = ((BaseClothing)item).NegativeAttributes;
            }

            if (budget < 150) // minor magic
            {
                if (.99 > Utility.RandomDouble())
                    return;

                switch (Utility.Random(3))
                {
                    case 0: neg.Prized = 1; break;
                    case 1: neg.Antique = 1; break;
                    case 2: attrs.Luck = -100; break;
                }

                budget += 100;
            }
            else if (budget < 225) // lesser magic
            {
                if (.80 > Utility.RandomDouble())
                    return;

                switch (Utility.Random(3))
                {
                    case 0: neg.Prized = 1; break;
                    case 1: neg.Antique = 1; break;
                    case 2: attrs.Luck = -100; break;
                }

                budget += 100;
            }
            else if (budget < 350) // greater magic
            {
                if (.60 > Utility.RandomDouble())
                    return;

                if (.85 > Utility.RandomDouble())
                {
                    if (Utility.RandomBool())
                        neg.Prized = 1;
                    else
                        attrs.Luck = -100;

                    budget += 100;
                }
                else
                {
                    if (Utility.RandomBool())
                        neg.Antique = 1;
                    else
                        neg.Brittle = 1;

                    budget += 150;
                }

            }
            else if (budget < 500) // major magic
            {
                if (.33 > Utility.RandomDouble())
                    return;

                if (.5 > Utility.RandomDouble())
                {
                    neg.Prized = 1;
                    budget += 100;
                }
                else if (.9 > Utility.RandomDouble())
                {
                    if (Utility.RandomBool())
                    {
                        neg.Antique = 1;
                        budget += 150;
                    }
                    else
                    {
                        attrs.Luck = -100;
                        budget += 100;
                    }
                }
                else
                {
                    neg.Brittle = 1;
                    budget += 150;
                }
            }
            else if (budget < 650) // lesser arty
            {
                if (0.01 > Utility.RandomDouble())
                    return;

                if (0.5 > Utility.RandomDouble())
                {
                    neg.Prized = 1;
                    budget += 100;
                }
                else if (0.9 > Utility.RandomDouble())
                {
                    neg.Antique = 1;
                    budget += 150;
                }
                else
                {
                    neg.Brittle = 1;
                    budget += 150;
                }
            }
            else if (budget < 800) // greater arty
            {
                if (0.85 > Utility.RandomDouble())
                {
                    if (Utility.RandomBool())
                        neg.Antique = 1;
                    else
                        neg.Prized = 1;

                    budget += 100;
                }
                else
                {
                    neg.Brittle = 1;
                    budget += 150;
                }
            }
            else
            {
                if (0.85 > Utility.RandomDouble())
                {
                    neg.Antique = 1;
                    budget += 100;
                }
                else
                {
                    neg.Brittle = 1;
                    budget += 150;
                }
            }

            //TODO: Verify this
            if (item is IDurability && (neg.Antique == 1 || neg.Brittle == 1 || item is BaseJewel))
            {
                ((IDurability)item).MaxHitPoints = 255;
                ((IDurability)item).HitPoints = 255;
            }
        }

        public static void SetBlockRepair(Item item)
        {
            if (item is BaseWeapon)
                ((BaseWeapon)item).BlockRepair = true;
            else if (item is BaseArmor)
                ((BaseArmor)item).BlockRepair = true;
            else if (item is BaseJewel)
                ((BaseJewel)item).BlockRepair = true;
            else if (item is BaseClothing)
                ((BaseClothing)item).BlockRepair = true;
        }

        private static void ApplyItemPower(Item item, bool playermade)
        {
            if (item is BaseWeapon)
                ((BaseWeapon)item).ItemPower = GetItemPower(item, playermade);
            else if (item is BaseArmor)
                ((BaseArmor)item).ItemPower = GetItemPower(item, playermade);
            else if (item is BaseJewel)
                ((BaseJewel)item).ItemPower = GetItemPower(item, playermade);
            else if (item is BaseClothing)
                ((BaseClothing)item).ItemPower = GetItemPower(item, playermade);
        }

        public static ItemPower GetItemPower(Item item, bool playermade)
        {
            int max = Imbuing.GetMaxWeight(item);
            int weight = Imbuing.GetTotalWeight(item);
            int totalMods = Imbuing.GetTotalMods(item);

            if (totalMods == 0)
                return ItemPower.None;

            if (weight < 250 && totalMods <= 4)
                return playermade ? ItemPower.ReforgedMinor : ItemPower.Minor;

            if (weight < 250)
                return playermade ? ItemPower.ReforgedLesser : ItemPower.Lesser;

            if (weight < max && totalMods <= 4)
                return playermade ? ItemPower.ReforgedGreater : ItemPower.Greater;

            if (weight < max)
                return playermade ? ItemPower.ReforgedGreater : ItemPower.Major;

            if (weight >= max && weight < max + 100)
                return playermade ? ItemPower.ReforgedMajor : ItemPower.LesserArtifact;

            if (weight < 700)
                return playermade ? ItemPower.ReforgedMajor : ItemPower.GreaterArtifact;

            if (totalMods < 6 && !playermade)
                return ItemPower.MajorArtifact;

            return playermade ? ItemPower.ReforgedLegendary : ItemPower.LegendaryArtifact;
        }
		
		private static bool ApplyRunicAttributes(Item item, int perclow, int perchigh, ref int budget, int idx, int luckchance)
		{
            List<object> attrList = null;
            AosWeaponAttributes wepattrs = GetAosWeaponAttributes(item);
            AosAttributes aosattrs = GetAosAttributes(item);
            AosArmorAttributes armorattrs = GetAosArmorAttributes(item);
            AosSkillBonuses skillbonuses = GetAosSkillBonuses(item);
            AosElementAttributes resistattrs = GetElementalAttributes(item);

			if(item is BaseWeapon)
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

			while(start == budget && budget > 0 && idx < 25)
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

				if(wepattrs != null && attr is AosWeaponAttribute[])
				{
					int ran = Utility.Random(((AosWeaponAttribute[])attr).Length);
					
					while(wepattrs[(AosWeaponAttribute)ran] != 0)
						ran = Utility.Random(((AosWeaponAttribute[])attr).Length);

                    AosWeaponAttribute[] list = attr as AosWeaponAttribute[];

					attr = list[ran];
				}
				
				if(aosattrs != null && attr is AosAttribute)
				{
					minmax = Imbuing.GetPropRange((AosAttribute)attr);

                    int min = minmax[0];
                    int max = minmax[1];

                    if (item is BaseJewel && (AosAttribute)attr == AosAttribute.WeaponDamage)
                        max = 25;

                    value = CalculateValue(attr, min, max, perclow, perchigh, ref budget, luckchance);

                    if (aosattrs[(AosAttribute)attr] == 0)
                    {
                        aosattrs[(AosAttribute)attr] = value;
                        budget -= Imbuing.GetIntensityForAttribute((AosAttribute)attr, -1, value);

                        if ((AosAttribute)attr == AosAttribute.SpellChanneling && aosattrs[AosAttribute.CastSpeed] > -1)
                            aosattrs[AosAttribute.CastSpeed]--;
                    }
				}
				else if (wepattrs != null && attr is AosWeaponAttribute)
				{
                    minmax = Imbuing.GetPropRange((AosWeaponAttribute)attr);
                    value = CalculateValue(attr, minmax[0], minmax[1], perclow, perchigh, ref budget, luckchance);

                    if (wepattrs[(AosWeaponAttribute)attr] == 0)
                    {
                        wepattrs[(AosWeaponAttribute)attr] = value;
                        budget -= Imbuing.GetIntensityForAttribute((AosWeaponAttribute)attr, -1, value);
                    }
				}
				else if (armorattrs != null && attr is AosArmorAttribute)
				{
                    minmax = Imbuing.GetPropRange((AosArmorAttribute)attr);
                    value = CalculateValue(attr, minmax[0], minmax[1], perclow, perchigh, ref budget, luckchance);

                    if (armorattrs[(AosArmorAttribute)attr] == 0)
                    {
                        armorattrs[(AosArmorAttribute)attr] = value;
                        budget -= Imbuing.GetIntensityForAttribute((AosArmorAttribute)attr, -1, value);
                    }
				}
                else if (attr is AosElementAttribute && (resistattrs != null || item is BaseArmor))
                {
                    minmax = Imbuing.GetPropRange((AosElementAttribute)attr);
                    value = CalculateValue(attr, minmax[0], minmax[1], perclow, perchigh, ref budget, luckchance);

                    if (resistattrs != null && resistattrs[(AosElementAttribute)attr] == 0)
                    {
                        resistattrs[(AosElementAttribute)attr] = value;
                        budget -= Imbuing.GetIntensityForAttribute((AosElementAttribute)attr, -1, value);
                    }
                    else if (item is BaseArmor)
                    {
                        BaseArmor armor = item as BaseArmor;

                        switch ((AosElementAttribute)attr)
                        {
                            case AosElementAttribute.Physical: armor.PhysicalBonus = value; break;
                            case AosElementAttribute.Fire: armor.FireBonus = value; break;
                            case AosElementAttribute.Cold: armor.ColdBonus = value; break;
                            case AosElementAttribute.Poison: armor.PoisonBonus = value; break;
                            case AosElementAttribute.Energy: armor.EnergyBonus = value; break;
                        }

                        budget -= Imbuing.GetIntensityForAttribute((AosElementAttribute)attr, -1, value);
                    }
                }
                else if (attr is string)
                {
                    string str = attr as string;

                    if (item is BaseWeapon && str == "Slayer" && ((BaseWeapon)item).Slayer == SlayerName.None)
                    {
                        SlayerName name = BaseRunicTool.GetRandomSlayer();
                        int weight = Imbuing.GetIntensityForAttribute(name, -1, value);

                        if (weight <= budget)
                        {
                            ((BaseWeapon)item).Slayer = name;
                            budget -= weight;
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

                        value = CalculateValue(sk, 1, 15, perclow, perchigh, ref budget, luckchance);
                        skillbonuses.SetValues(skillIdx, sk, value);
                        budget -= Imbuing.GetIntensityForAttribute(sk, -1, value);
                    }
                }

                attrList.RemoveAt(random);
                idx++;
			}

            if(attrList != null)
                attrList.Clear();

			return true;
		}

        public static bool CheckAttribute(Item item, object attr)
        {
            if (CheckEater(item, attr))
                return true;

            if (item is BaseArmor && !(item is BaseShield) && attr is AosArmorAttribute && (AosArmorAttribute)attr == AosArmorAttribute.MageArmor && ((BaseArmor)item).MeditationAllowance == ArmorMeditationAllowance.All)
                return true;

            if (item is BaseClothing && attr is AosArmorAttribute && (AosArmorAttribute)attr == AosArmorAttribute.MageArmor)
                return true;

            if (item is BaseWeapon && attr is AosWeaponAttribute[] && (CheckHitSpell((BaseWeapon)item, attr) || (CheckHitArea((BaseWeapon)item, attr))))
                return true;

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

            return null;
        }

        public static AosArmorAttributes GetAosArmorAttributes(Item item)
        {
            if (item is BaseArmor)
                return ((BaseArmor)item).ArmorAttributes;

            if (item is BaseClothing)
                return ((BaseClothing)item).ClothingAttributes;

            //if (item is BaseTalisman)
            //    return ((BaseTalisman)item).ArmorAttributes;

            //if (item is BaseJewel)
            //    return ((BaseJewel)item).ArmorAttributes;

            return null;
        }

        public static AosWeaponAttributes GetAosWeaponAttributes(Item item)
        {
            if (item is BaseWeapon)
                return ((BaseWeapon)item).WeaponAttributes;

            if (item is Glasses)
                return ((Glasses)item).WeaponAttributes;

            return null;
        }

        public static AosElementAttributes GetElementalAttributes(Item item)
        {
            if (item is BaseClothing)
                return ((BaseClothing)item).Resistances;

            if (item is BaseJewel)
                return ((BaseJewel)item).Resistances;

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

            return null;
        }

        public static AosSkillBonuses GetAosSkillBonuses(Item item)
        {
            if (item is BaseJewel)
                return ((BaseJewel)item).SkillBonuses;

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

            return null;
        }
		
		private static int ScaleAttribute(object o)
		{
			if(o is AosAttribute)
			{
				AosAttribute attr = (AosAttribute)o;
				
				if(attr == AosAttribute.Luck)
					return 10;
					
				if(attr == AosAttribute.WeaponSpeed)
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
            //AosWeaponAttribute.BalancedWeapon,
            //AosWeaponAttribute.WeaponVelocity
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
			AosAttribute.AttackChance,
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
                if (((Item)targeted).IsChildOf(from.Backpack))
                {
                    Item item = targeted as Item;

                    if (RunicReforging.CanReforge(from, item, m_Tool.CraftSystem))
                    {
                        from.SendGump(new RunicReforgingGump(from, item, m_Tool));
                    }
                }
                else
                    from.SendLocalizedMessage(1152271); // That must be in your backpack for you to use it.
            }
        }
    }
}