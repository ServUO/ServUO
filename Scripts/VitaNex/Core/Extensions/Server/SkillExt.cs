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
#endregion

namespace Server
{
	public enum SkillCategory
	{
		None = 0,
		Combat,
		Healing,
		Magic,
		Bardic,
		Rogue,
		Knowledge,
		Craft,
		Harvest
	}

	public enum SkillIcon
	{
		None = 1033,
		Combat = 1008,
		Healing = 1037,
		Magic = 1006,
		Bardic = 1031,
		Rogue = 1002,
		Knowledge = 1034,
		Craft = 1030,
		Harvest = 1036
	}

	public static class SkillExtUtility
	{
		public static readonly SkillName[] EmptySkills = new SkillName[0];

		public static readonly SkillName[] AllSkills = default(SkillName).GetValues<SkillName>();

		public static readonly SkillName[] CombatSkills =
		{
			SkillName.Archery, SkillName.Fencing, SkillName.Focus, SkillName.Macing, SkillName.Parry, SkillName.Swords,
			SkillName.Tactics, SkillName.Wrestling, SkillName.Bushido
		};

		public static readonly SkillName[] HealingSkills =
		{
			SkillName.Healing, SkillName.Veterinary
		};

		public static readonly SkillName[] MagicSkills =
		{
			SkillName.EvalInt, SkillName.Inscribe, SkillName.Magery, SkillName.Meditation, SkillName.Chivalry,
			SkillName.Necromancy, SkillName.MagicResist, SkillName.Spellweaving, SkillName.SpiritSpeak
		};

		public static readonly SkillName[] BardicSkills =
			{SkillName.Discordance, SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation};

		public static readonly SkillName[] RogueSkills =
		{
			SkillName.Begging, SkillName.DetectHidden, SkillName.Hiding, SkillName.Lockpicking, SkillName.Poisoning,
			SkillName.RemoveTrap, SkillName.Snooping, SkillName.Stealing, SkillName.Stealth, SkillName.Ninjitsu
		};

		public static readonly SkillName[] KnowledgeSkills =
		{
			SkillName.Anatomy, SkillName.AnimalLore, SkillName.AnimalTaming, SkillName.ArmsLore, SkillName.Camping,
			SkillName.Forensics, SkillName.Herding, SkillName.ItemID, SkillName.TasteID, SkillName.Tracking
		};

		public static readonly SkillName[] CraftSkills =
		{
			SkillName.Alchemy, SkillName.Blacksmith, SkillName.Fletching, SkillName.Carpentry, SkillName.Cooking,
			SkillName.Cartography, SkillName.Tailoring, SkillName.Tinkering, SkillName.Imbuing
		};

		public static readonly SkillName[] HarvestSkills =
		{
			SkillName.Fishing, SkillName.Mining, SkillName.Lumberjacking
		};

		public static readonly SkillCategory[] Categories =
		{
			SkillCategory.Combat, SkillCategory.Healing, SkillCategory.Magic, SkillCategory.Bardic, SkillCategory.Rogue,
			SkillCategory.Knowledge, SkillCategory.Craft, SkillCategory.Harvest
		};

		public static readonly SkillIcon[] Icons =
		{
			SkillIcon.None, SkillIcon.Combat, SkillIcon.Healing, SkillIcon.Magic, SkillIcon.Bardic, SkillIcon.Rogue,
			SkillIcon.Knowledge, SkillIcon.Craft, SkillIcon.Harvest
		};

		public static IEnumerable<Skill> OfExpansion(this Skills skills)
		{
			return OfExpansion(skills.Select(sk => sk.SkillName)).Select(sk => skills[sk]);
		}

		public static IEnumerable<Skill> OfExpansion(this Skills skills, Expansion ex)
		{
			return OfExpansion(skills.Select(sk => sk.SkillName), ex).Select(sk => skills[sk]);
		}

		public static IEnumerable<SkillName> OfExpansion(this IEnumerable<SkillName> skills)
		{
			return OfExpansion(skills, Core.Expansion);
		}

		public static IEnumerable<SkillName> OfExpansion(this IEnumerable<SkillName> skills, Expansion ex)
		{
			foreach (var sk in skills)
			{
				switch (sk)
				{
					case SkillName.Chivalry:
					case SkillName.Focus:
					case SkillName.Necromancy:
					case SkillName.SpiritSpeak:
					{
						if (ex >= Expansion.AOS)
						{
							yield return sk;
						}
					}
					break;
					case SkillName.Bushido:
					case SkillName.Ninjitsu:
					{
						if (ex >= Expansion.SE)
						{
							yield return sk;
						}
					}
					break;
					case SkillName.Spellweaving:
					{
						if (ex >= Expansion.ML)
						{
							yield return sk;
						}
					}
					break;
					case SkillName.Imbuing:
					case SkillName.Throwing:
					case SkillName.Mysticism:
					{
						if (ex >= Expansion.SA)
						{
							yield return sk;
						}
					}
					break;
					default:
						yield return sk;
						break;
				}
			}
		}

		public static SkillName[] GetSkills(this SkillCategory cat)
		{
			switch (cat)
			{
				case SkillCategory.Combat:
					return CombatSkills;
				case SkillCategory.Healing:
					return HealingSkills;
				case SkillCategory.Magic:
					return MagicSkills;
				case SkillCategory.Bardic:
					return BardicSkills;
				case SkillCategory.Rogue:
					return RogueSkills;
				case SkillCategory.Knowledge:
					return KnowledgeSkills;
				case SkillCategory.Craft:
					return CraftSkills;
				case SkillCategory.Harvest:
					return HarvestSkills;
			}

			return EmptySkills;
		}

		public static SkillIcon GetIcon(this SkillCategory cat)
		{
			return Icons[(int)cat];
		}

		public static SkillIcon GetIcon(this SkillInfo info)
		{
			return GetIcon((SkillName)info.SkillID);
		}

		public static SkillIcon GetIcon(this SkillName skill)
		{
			return GetIcon(GetCategory(skill));
		}

		public static int GetIconID(this SkillInfo info)
		{
			return (int)GetIcon(info);
		}

		public static int GetIconID(this SkillCategory cat)
		{
			return (int)GetIcon(cat);
		}

		public static int GetIconID(this SkillName skill)
		{
			return (int)GetIcon(skill);
		}

		public static bool IsLocked(this Skill skill, SkillLock locked)
		{
			return skill.Lock == locked;
		}

		public static bool IsCapped(this Skill skill)
		{
			return skill.Base >= skill.Cap;
		}

		public static bool IsZero(this Skill skill)
		{
			return skill.Base <= 0;
		}

		public static bool IsZeroOrCapped(this Skill skill)
		{
			return IsZero(skill) || IsCapped(skill);
		}

		public static bool WillCap(this Skill skill, double value, bool isEqual = true)
		{
			return isEqual ? (skill.Base + value >= skill.Cap) : (skill.Base + value > skill.Cap);
		}

		public static bool WillZero(this Skill skill, double value, bool isEqual = true)
		{
			return isEqual ? (skill.Base - value <= 0) : (skill.Base - value < 0);
		}

		public static bool DecreaseBase(this Skill skill, double value, bool ignoreZero = false, bool trim = true)
		{
			if (trim)
			{
				value = Math.Min(skill.Base, value);
			}

			if (ignoreZero || (!IsZero(skill) && !WillZero(skill, value, false)))
			{
				skill.Base -= value;
				return true;
			}

			return false;
		}

		public static bool IncreaseBase(this Skill skill, double value, bool ignoreCap = false, bool trim = true)
		{
			if (trim)
			{
				value = Math.Min(skill.Cap - skill.Base, value);
			}

			if (ignoreCap || (!IsCapped(skill) && !WillCap(skill, value, false)))
			{
				skill.Base += value;
				return true;
			}

			return false;
		}

		public static bool SetBase(this Skill skill, double value, bool ignoreLimits = false, bool trim = true)
		{
			if (trim)
			{
				value = Math.Max(0, Math.Min(skill.Cap, value));
			}

			if (ignoreLimits || (value < skill.Base && !IsZero(skill) && !WillZero(skill, skill.Base - value, false)) ||
				(value > skill.Base && !IsCapped(skill) && !WillCap(skill, value - skill.Base, false)))
			{
				skill.Base = value;
				return true;
			}

			return false;
		}

		public static void DecreaseCap(this Skill skill, double value)
		{
			SetCap(skill, skill.Cap - value);
		}

		public static void IncreaseCap(this Skill skill, double value)
		{
			SetCap(skill, skill.Cap + value);
		}

		public static void SetCap(this Skill skill, double value)
		{
			skill.Cap = Math.Max(0, value);
			Normalize(skill);
		}

		public static void Normalize(this Skill skill)
		{
			if (IsCapped(skill))
			{
				skill.BaseFixedPoint = skill.CapFixedPoint;
			}

			if (IsZero(skill))
			{
				skill.BaseFixedPoint = 0;
			}
		}

		public static string GetName(this SkillName skill)
		{
			return SkillInfo.Table[(int)skill].Name;
		}

		public static bool CheckCategory(this SkillName skill, SkillCategory category)
		{
			return GetCategory(skill) == category;
		}

		public static SkillCategory GetCategory(this SkillName skill)
		{
			if (IsCombat(skill))
			{
				return SkillCategory.Combat;
			}

			if (IsHealing(skill))
			{
				return SkillCategory.Healing;
			}

			if (IsMagic(skill))
			{
				return SkillCategory.Magic;
			}

			if (IsBardic(skill))
			{
				return SkillCategory.Bardic;
			}

			if (IsRogue(skill))
			{
				return SkillCategory.Rogue;
			}

			if (IsKnowledge(skill))
			{
				return SkillCategory.Knowledge;
			}

			if (IsCraft(skill))
			{
				return SkillCategory.Craft;
			}

			if (IsHarvest(skill))
			{
				return SkillCategory.Harvest;
			}

			return SkillCategory.None;
		}

		public static bool IsCombat(this SkillName skill)
		{
			return CombatSkills.Contains(skill);
		}

		public static bool IsHealing(this SkillName skill)
		{
			return HealingSkills.Contains(skill);
		}

		public static bool IsMagic(this SkillName skill)
		{
			return MagicSkills.Contains(skill);
		}

		public static bool IsBardic(this SkillName skill)
		{
			return BardicSkills.Contains(skill);
		}

		public static bool IsRogue(this SkillName skill)
		{
			return RogueSkills.Contains(skill);
		}

		public static bool IsKnowledge(this SkillName skill)
		{
			return KnowledgeSkills.Contains(skill);
		}

		public static bool IsCraft(this SkillName skill)
		{
			return CraftSkills.Contains(skill);
		}

		public static bool IsHarvest(this SkillName skill)
		{
			return HarvestSkills.Contains(skill);
		}
	}
}