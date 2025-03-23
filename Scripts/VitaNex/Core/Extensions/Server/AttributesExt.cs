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
using System.Drawing;
using System.Linq;
using System.Reflection;

using Server.Items;

using VitaNex;
#endregion

namespace Server
{
	public static class AttributesExtUtility
	{
		#region Defaults
		public static List<SlayerName> SuperSlayers = new List<SlayerName>
		{
			SlayerName.Repond,
			SlayerName.ReptilianDeath,
			SlayerName.DaemonDismissal,
			SlayerName.ElementalBan,
			SlayerName.Exorcism,
			SlayerName.ArachnidDoom,
			SlayerName.Fey
		};

		public static List<TalismanSlayerName> SuperTalismanSlayers = new List<TalismanSlayerName>
		{
			TalismanSlayerName.Bat,
			TalismanSlayerName.Bear,
			TalismanSlayerName.Beetle,
			TalismanSlayerName.Bird,
			TalismanSlayerName.Bovine,
			TalismanSlayerName.Flame,
			TalismanSlayerName.Ice,
			TalismanSlayerName.Mage,
			TalismanSlayerName.Vermin
		};
		#endregion Defaults

		#region Fields & Properties
		/// <summary>
		///     (InstanceOf:BaseAttributes).GetValue multiple parameter support.
		///     Some instances of RunUO may have extended attribute enum flags beyond the Int32 capacity of 31 entries.
		/// </summary>
		private static readonly MethodInfo _GetValueImpl;

		private static readonly byte _GetValueSupport;

		public static readonly Type TypeOfBaseAttributes = typeof(BaseAttributes);
		public static readonly Type TypeOfAosAttribute = typeof(AosAttribute);
		public static readonly Type TypeOfAosArmorAttribute = typeof(AosArmorAttribute);
		public static readonly Type TypeOfAosWeaponAttribute = typeof(AosWeaponAttribute);
		public static readonly Type TypeOfAosElementAttribute = typeof(AosElementAttribute);
		public static readonly Type TypeOfSkillName = typeof(SkillName);
		public static readonly Type TypeOfSlayerName = typeof(SlayerName);
		public static readonly Type TypeOfTalismanSlayerName = typeof(TalismanSlayerName);
		public static readonly Type TypeOfSAAbsorptionAttribute = typeof(SAAbsorptionAttribute);

		public static Dictionary<AosAttribute, AttributeDefinition> AttrFactors { get; private set; }
		public static Dictionary<AosArmorAttribute, AttributeDefinition> ArmorAttrFactors { get; private set; }
		public static Dictionary<AosWeaponAttribute, AttributeDefinition> WeaponAttrFactors { get; private set; }
		public static Dictionary<AosElementAttribute, AttributeDefinition> ElementAttrFactors { get; private set; }
		public static Dictionary<SkillName, AttributeDefinition> SkillBonusAttrFactors { get; private set; }
		public static Dictionary<SlayerName, AttributeDefinition> SlayerAttrFactors { get; private set; }
		public static Dictionary<TalismanSlayerName, AttributeDefinition> TalismanSlayerAttrFactors { get; private set; }
		public static Dictionary<SAAbsorptionAttribute, AttributeDefinition> AbsorptionAttrFactors { get; private set; }

		#endregion Fields & Properties

		#region Initialization
		static AttributesExtUtility()
		{
			_GetValueImpl = typeof(BaseAttributes).GetMethod("GetValue", BindingFlags.Instance | BindingFlags.Public);
			_GetValueSupport = 0x0;

			if (_GetValueImpl != null)
			{
				var p = _GetValueImpl.GetParameters();

				if (p.Length == 1)
				{
					if (p[0].ParameterType.IsEqual(typeof(uint)))
					{
						_GetValueSupport = 0x1;
					}
					else if (p[0].ParameterType.IsEqual(typeof(long)))
					{
						_GetValueSupport = 0x2;
					}
					else if (p[0].ParameterType.IsEqual(typeof(ulong)))
					{
						_GetValueSupport = 0x3;
					}
				}
			}

			AttrFactors = new Dictionary<AosAttribute, AttributeDefinition>
			{
				{AosAttribute.AttackChance, new AttributeDefinition("Hit Chance Increase", 1.3, 0, 15, 3, true)},
				{AosAttribute.BonusDex, new AttributeDefinition("Bonus Dexterity", 1.1, 0, 8)},
				{AosAttribute.BonusHits, new AttributeDefinition("Bonus Health", 1.1, 0, 5)},
				{AosAttribute.BonusInt, new AttributeDefinition("Bonus Intelligence", 1.1, 0, 8)},
				{AosAttribute.BonusMana, new AttributeDefinition("Bonus Mana", 1.1, 0, 8)},
				{AosAttribute.BonusStam, new AttributeDefinition("Bonus Stamina", 1.1, 0, 8)},
				{AosAttribute.BonusStr, new AttributeDefinition("Bonus Strength", 1.1, 0, 8)},
				{AosAttribute.CastRecovery, new AttributeDefinition("Faster Cast Recovery", 1.2, 0, 3)},
				{AosAttribute.CastSpeed, new AttributeDefinition("Faster Casting", 1.4)},
				{AosAttribute.DefendChance, new AttributeDefinition("Defend Chance Increase", 1.1, 0, 15, 3, true)},
				{AosAttribute.EnhancePotions, new AttributeDefinition("Enhance Potions", 1.0, 0, 25, 5, true)},
				{AosAttribute.IncreasedKarmaLoss, new AttributeDefinition("Increased Karma Loss", 1.0, 0, 1, 1, true)},
				{AosAttribute.LowerManaCost, new AttributeDefinition("Lower Mana Cost", 1.1, 0, 15, 3, true)},
				{AosAttribute.LowerRegCost, new AttributeDefinition("Lower Reagent Cost", 1.0, 0, 20, 2, true)},
				{AosAttribute.Luck, new AttributeDefinition("Luck", 1.0, 0, 100, 10)},
				{AosAttribute.NightSight, new AttributeDefinition("Night Sight")},
				{AosAttribute.ReflectPhysical, new AttributeDefinition("Reflect Physical Damage", 1.0, 0, 15, 3, true)},
				{AosAttribute.RegenHits, new AttributeDefinition("Health Regeneration", 1.0, 0, 2)},
				{AosAttribute.RegenMana, new AttributeDefinition("Mana Regeneration", 1.0, 0, 2)},
				{AosAttribute.RegenStam, new AttributeDefinition("Stamina Regeneration", 1.0, 0, 3)},
				{AosAttribute.SpellChanneling, new AttributeDefinition("Spell Channeling")},
				{AosAttribute.SpellDamage, new AttributeDefinition("Spell Damage Increase", 1.0, 0, 12, 2, true)},
				{AosAttribute.WeaponDamage, new AttributeDefinition("Weapon Damage Increase", 1.0, 0, 50, 5, true)},
				{AosAttribute.WeaponSpeed, new AttributeDefinition("Weapon Speed Increase", 1.1, 0, 30, 5, true)}
			};

			ArmorAttrFactors = new Dictionary<AosArmorAttribute, AttributeDefinition>
			{
				{AosArmorAttribute.DurabilityBonus, new AttributeDefinition("Durability Bonus", 1.0, 0, 250, 25, true)},
				{AosArmorAttribute.LowerStatReq, new AttributeDefinition("Lower Stat Requirement", 1.0, 0, 100, 10)},
				{AosArmorAttribute.MageArmor, new AttributeDefinition("Mage Armor")},
				{AosArmorAttribute.SelfRepair, new AttributeDefinition("Armor Self Repair", 1.0, 0, 5)}
			};

			WeaponAttrFactors = new Dictionary<AosWeaponAttribute, AttributeDefinition>
			{
				{AosWeaponAttribute.DurabilityBonus, new AttributeDefinition("Durability Bonus", 1.0, 0, 250, 25, true)},
				{AosWeaponAttribute.HitColdArea, new AttributeDefinition("Hit Cold Area", 1.0, 0, 50, 5, true)},
				{AosWeaponAttribute.HitDispel, new AttributeDefinition("Hit Dispel", 1.0, 0, 50, 5, true)},
				{AosWeaponAttribute.HitEnergyArea, new AttributeDefinition("Hit Energy Area", 1.0, 0, 50, 5, true)},
				{AosWeaponAttribute.HitFireArea, new AttributeDefinition("Hit Fire Area", 1.0, 0, 50, 5, true)},
				{AosWeaponAttribute.HitFireball, new AttributeDefinition("Hit Fireball", 1.4, 0, 50, 5, true)},
				{AosWeaponAttribute.HitHarm, new AttributeDefinition("Hit Harm", 1.1, 0, 50, 5, true)},
				{AosWeaponAttribute.HitLeechHits, new AttributeDefinition("Hit Leech Health", 1.1, 0, 50, 10, true)},
				{AosWeaponAttribute.HitLeechMana, new AttributeDefinition("Hit Leech Mana", 1.1, 0, 50, 10, true)},
				{AosWeaponAttribute.HitLeechStam, new AttributeDefinition("Hit Leech Stamina", 1.0, 0, 50, 5, true)},
				{AosWeaponAttribute.HitLightning, new AttributeDefinition("Hit Lightning", 1.4, 0, 50, 5, true)},
				{AosWeaponAttribute.HitLowerAttack, new AttributeDefinition("Hit Lower Attack", 1.1, 0, 50, 5, true)},
				{AosWeaponAttribute.HitLowerDefend, new AttributeDefinition("Hit Lower Defense", 1.3, 0, 50, 5, true)},
				{AosWeaponAttribute.HitMagicArrow, new AttributeDefinition("Hit Magic Arrow", 1.0, 0, 50, 5, true)},
				{AosWeaponAttribute.HitPhysicalArea, new AttributeDefinition("Hit Physical Area", 1.0, 0, 50, 5, true)},
				{AosWeaponAttribute.HitPoisonArea, new AttributeDefinition("Hit Poison Area", 1.0, 0, 50, 5, true)},
				{AosWeaponAttribute.LowerStatReq, new AttributeDefinition("Lower Stat Requirement", 1.0, 0, 100, 5)},
				{AosWeaponAttribute.MageWeapon, new AttributeDefinition("Mage Weapon", 1.0, 0, 30, 3)},
				{AosWeaponAttribute.ResistColdBonus, new AttributeDefinition("Resist Cold Bonus", 1.0, 0, 15, 3, true)},
				{AosWeaponAttribute.ResistEnergyBonus, new AttributeDefinition("Resist Energy Bonus", 1.0, 0, 15, 3, true)},
				{AosWeaponAttribute.ResistFireBonus, new AttributeDefinition("Resist Fire Bonus", 1.0, 0, 15, 3, true)},
				{AosWeaponAttribute.ResistPhysicalBonus, new AttributeDefinition("Resist Physical Bonus", 1.0, 0, 15, 3, true)},
				{AosWeaponAttribute.ResistPoisonBonus, new AttributeDefinition("Resist Poison Bonus", 1.0, 0, 15, 3, true)},
				{AosWeaponAttribute.SelfRepair, new AttributeDefinition("Weapon Self Repair", 1.0, 0, 5)},
				{AosWeaponAttribute.UseBestSkill, new AttributeDefinition("Use Best Weapon Skill", 1.4)}
			};

			ElementAttrFactors = new Dictionary<AosElementAttribute, AttributeDefinition>
			{
				{AosElementAttribute.Physical, new AttributeDefinition("Physical", 1.0, 0, 15, 3, true)},
				{AosElementAttribute.Fire, new AttributeDefinition("Fire", 1.0, 0, 15, 3, true)},
				{AosElementAttribute.Cold, new AttributeDefinition("Cold", 1.0, 0, 15, 3, true)},
				{AosElementAttribute.Poison, new AttributeDefinition("Poison", 1.0, 0, 15, 3, true)},
				{AosElementAttribute.Energy, new AttributeDefinition("Energy", 1.0, 0, 15, 3, true)},
				{AosElementAttribute.Chaos, new AttributeDefinition("Chaos", 1.0, 0, 15, 3, true)},
				{AosElementAttribute.Direct, new AttributeDefinition("Direct", 1.0, 0, 15, 3, true)}
			};

			SkillBonusAttrFactors = new Dictionary<SkillName, AttributeDefinition>();
			SlayerAttrFactors = new Dictionary<SlayerName, AttributeDefinition>();
			TalismanSlayerAttrFactors = new Dictionary<TalismanSlayerName, AttributeDefinition>();

			// All skill bonuses have a weight of 1.4 and intensity cap of 15%
			foreach (SkillName attr in Enum.GetValues(TypeOfSkillName))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), 1.4, 0, 15, 3);
			}

			// All standard slayers have a weight of 1.1 and intensity cap of 1
			// If a standard slayer is considered a super slayer, its weight is 1.3
			foreach (var attr in Enum.GetValues(TypeOfSlayerName).Cast<SlayerName>().Where(a => a != SlayerName.None))
			{
				var se = SlayerGroup.GetEntryByName(attr);

				TextDefinition name;

				if (se != null && se.Title > 0)
				{
					name = new TextDefinition(se.Title, Clilocs.GetString(se.Title));
				}
				else
				{
					name = attr.ToString(true);
				}

				SlayerAttrFactors[attr] = GetDefaultDefinition(name, IsSuper(attr) ? 1.3 : 1.1);
			}

			// All talisman slayers have a weight of 1.1 and intensity cap of 1
			// If a talisman slayer is considered a super slayer, its weight is 1.3
			foreach (var attr in Enum.GetValues(TypeOfTalismanSlayerName)
									 .Cast<TalismanSlayerName>()
									 .Where(a => a != TalismanSlayerName.None))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			AbsorptionAttrFactors = new Dictionary<SAAbsorptionAttribute, AttributeDefinition>
			{
				{SAAbsorptionAttribute.CastingFocus, new AttributeDefinition("Casting Focus", 1.3, 0, 5, 1, true)},
				{SAAbsorptionAttribute.EaterKinetic, new AttributeDefinition("Kinetic Eater", 1.2, 0, 5, 1, true)},
				{SAAbsorptionAttribute.EaterFire, new AttributeDefinition("Fire Eater", 1.2, 0, 5, 1, true)},
				{SAAbsorptionAttribute.EaterCold, new AttributeDefinition("Cold Eater", 1.2, 0, 5, 1, true)},
				{SAAbsorptionAttribute.EaterPoison, new AttributeDefinition("Poison Eater", 1.2, 0, 5, 1, true)},
				{SAAbsorptionAttribute.EaterEnergy, new AttributeDefinition("Energy Eater", 1.2, 0, 5, 1, true)},
				{SAAbsorptionAttribute.ResonanceKinetic, new AttributeDefinition("Kinetic Resonance", 1.3, 0, 5, 1, true)},
				{SAAbsorptionAttribute.ResonanceFire, new AttributeDefinition("Fire Resonance", 1.3, 0, 5, 1, true)},
				{SAAbsorptionAttribute.ResonanceCold, new AttributeDefinition("Cold Resonance", 1.3, 0, 5, 1, true)},
				{SAAbsorptionAttribute.ResonancePoison, new AttributeDefinition("Poison Resonance", 1.3, 0, 5, 1, true)},
				{SAAbsorptionAttribute.ResonanceEnergy, new AttributeDefinition("Energy Resonance", 1.3, 0, 5, 1, true)},
				{SAAbsorptionAttribute.SoulChargeKinetic, new AttributeDefinition("Kinetic Soul Charge", 1.4, 0, 5, 1, true)},
				{SAAbsorptionAttribute.SoulChargeFire, new AttributeDefinition("Fire Soul Charge", 1.4, 0, 5, 1, true)},
				{SAAbsorptionAttribute.SoulChargeCold, new AttributeDefinition("Cold Soul Charge", 1.4, 0, 5, 1, true)},
				{SAAbsorptionAttribute.SoulChargePoison, new AttributeDefinition("Poison Soul Charge", 1.4, 0, 5, 1, true)},
				{SAAbsorptionAttribute.SoulChargeEnergy, new AttributeDefinition("Energy Soul Charge", 1.4, 0, 5, 1, true)}
			};
		}
		#endregion Initialization

		#region Helper Methods
		public static bool IsHitSpell(this AosWeaponAttribute attr)
		{
			switch (attr)
			{
				case AosWeaponAttribute.HitMagicArrow:
				case AosWeaponAttribute.HitHarm:
				case AosWeaponAttribute.HitFireball:
				case AosWeaponAttribute.HitLightning:
				case AosWeaponAttribute.HitDispel:
				case AosWeaponAttribute.HitCurse:
					return true;
			}

			return false;
		}

		public static bool IsHitAttack(this AosWeaponAttribute attr)
		{
			switch (attr)
			{
				case AosWeaponAttribute.HitLeechHits:
				case AosWeaponAttribute.HitLeechStam:
				case AosWeaponAttribute.HitLeechMana:
				case AosWeaponAttribute.HitLowerAttack:
				case AosWeaponAttribute.HitLowerDefend:
				case AosWeaponAttribute.HitFatigue:
				case AosWeaponAttribute.HitManaDrain:
					return true;
			}

			return false;
		}

		public static bool IsHitArea(this AosWeaponAttribute attr)
		{
			switch (attr)
			{
				case AosWeaponAttribute.HitColdArea:
				case AosWeaponAttribute.HitFireArea:
				case AosWeaponAttribute.HitPoisonArea:
				case AosWeaponAttribute.HitEnergyArea:
				case AosWeaponAttribute.HitPhysicalArea:
					return true;
			}

			return false;
		}

		public static AttributeDefinition GetInfo(Enum attr)
		{
			if (attr is AosAttribute)
			{
				return AttrFactors.GetValue((AosAttribute)attr);
			}

			if (attr is AosArmorAttribute)
			{
				return ArmorAttrFactors.GetValue((AosArmorAttribute)attr);
			}

			if (attr is AosWeaponAttribute)
			{
				return WeaponAttrFactors.GetValue((AosWeaponAttribute)attr);
			}

			if (attr is AosElementAttribute)
			{
				return ElementAttrFactors.GetValue((AosElementAttribute)attr);
			}

			if (attr is SkillName)
			{
				return SkillBonusAttrFactors.GetValue((SkillName)attr);
			}

			if (attr is SlayerName)
			{
				return SlayerAttrFactors.GetValue((SlayerName)attr);
			}

			if (attr is TalismanSlayerName)
			{
				return TalismanSlayerAttrFactors.GetValue((TalismanSlayerName)attr);
			}

			if (attr is SAAbsorptionAttribute)
			{
				return AbsorptionAttrFactors.GetValue((SAAbsorptionAttribute)attr);
			}

			return null;
		}

		private static bool SupportsAttributes(Item item, string name, out BaseAttributes attrs)
		{
			var pi = item.GetType().GetProperty(name);

			if (pi == null)
			{
				attrs = null;
				return false;
			}

			return (attrs = pi.GetValue(item, null) as BaseAttributes) != null;
		}

		private static bool HasAttribute(Item item, string name, ulong attr, out int value)
		{
			value = 0;

			var pi = item.GetType().GetProperty(name);

			if (pi == null)
			{
				return false;
			}

			var attrs = pi.GetValue(item, null) as BaseAttributes;

			if (attrs == null)
			{
				return false;
			}

			if (attrs is AosSkillBonuses)
			{
				var sb = (AosSkillBonuses)attrs;
				var sk = (SkillName)attr;

				for (var i = 0; i < 5; i++)
				{
					if (sb.GetSkill(i) == sk)
					{
						value += (int)(sb.GetBonus(i) * 10);
					}
				}

				return value != 0;
			}

			if (_GetValueImpl != null)
			{
				switch (_GetValueSupport)
				{
					case 0x0:
						return (value = (int)_GetValueImpl.Invoke(attrs, new object[] { (int)attr })) != 0;
					case 0x1:
						return (value = (int)_GetValueImpl.Invoke(attrs, new object[] { (uint)attr })) != 0;
					case 0x2:
						return (value = (int)_GetValueImpl.Invoke(attrs, new object[] { (long)attr })) != 0;
					case 0x3:
						return (value = (int)_GetValueImpl.Invoke(attrs, new object[] { attr })) != 0;
				}
			}

			return false;
		}

		public static ulong[] AttrMasks = ((AosAttribute)0).GetValues<ulong>();
		public static ulong[] ArmorAttrMasks = ((AosArmorAttribute)0).GetValues<ulong>();
		public static ulong[] WeaponAttrMasks = ((AosWeaponAttribute)0).GetValues<ulong>();
		public static ulong[] ElementAttrMasks = ((AosElementAttribute)0).GetValues<ulong>();
		public static ulong[] SkillBonusMasks = ((SkillName)0).GetValues<ulong>();
		public static ulong[] SlayerAttrMasks = ((SlayerName)0).GetValues<ulong>();
		public static ulong[] TalismanSlayerAttrMasks = ((TalismanSlayerName)0).GetValues<ulong>();
		public static ulong[] AbsorptionAttrMasks = ((SAAbsorptionAttribute)0).GetValues<ulong>();

		public static ulong[][] AttributeMasks =
		{
			AttrMasks, 
			ArmorAttrMasks, 
			WeaponAttrMasks, 
			ElementAttrMasks, 
			SkillBonusMasks, 
			SlayerAttrMasks,
			TalismanSlayerAttrMasks, 
			AbsorptionAttrMasks,
		};

		public static int GetAttributeCount(
			this Item item,
			bool normal = true,
			bool armor = true,
			bool weapon = true,
			bool element = true,
			bool skills = true,
			bool slayers = true,
			bool absorb = true
		)
		{
			int total = 0, value;

			if (normal)
			{
				total += AttributeMasks[0].Count(a => item.HasAttribute((AosAttribute)a, out value));
			}

			if (armor)
			{
				total += AttributeMasks[1].Count(a => item.HasAttribute((AosArmorAttribute)a, out value));
			}

			if (weapon)
			{
				total += AttributeMasks[2].Count(a => item.HasAttribute((AosWeaponAttribute)a, out value));
			}

			if (element)
			{
				total += AttributeMasks[3].Count(a => item.HasAttribute((AosElementAttribute)a, out value));
			}

			if (skills)
			{
				total += AttributeMasks[4].Count(a => item.HasSkillBonus((SkillName)a, out value));
			}

			if (slayers)
			{
				total += AttributeMasks[5].Count(a => item.HasSlayer((SlayerName)a));
				total += AttributeMasks[6].Count(a => item.HasSlayer((TalismanSlayerName)a));
			}

			if (absorb)
			{
				total += AttributeMasks[7].Count(a => item.HasAttribute((SAAbsorptionAttribute)a, out value));
			}

			return total;
		}

		public static string GetPropertyString(double value, bool html = true)
		{
			var s = value.ToString("#,0");

			if (html)
			{
				s = s.WrapUOHtmlColor(value > 0 ? Color.LimeGreen : value < 0 ? Color.Red : Color.Yellow);
			}

			return s;
		}

		private static AttributeDefinition GetDefaultDefinition(
			TextDefinition name = default,
			double weight = 1.0,
			int min = 0,
			int max = 1,
			int inc = 1,
			bool percentage = false)
		{
			return new AttributeDefinition(name, weight, min, max, inc, percentage);
		}
		#endregion

		#region Base Attributes
		public static string ToString(this BaseAttributes attr, double val, bool html = true)
		{
			return GetPropertyString(val, html);
		}
		#endregion

		#region Standard Attributes
		public static bool HasAttribute(this Item item, AosAttribute attr, out int value)
		{
			return HasAttribute(item, "Attributes", (ulong)attr, out value);
		}

		public static bool SupportsAttribute(this Item item, out AosAttributes attrs)
		{

			if (SupportsAttributes(item, "Attributes", out var a))
			{
				attrs = (AosAttributes)a;
				return true;
			}

			attrs = null;
			return false;
		}

		public static bool IsPercentage(this AosAttribute attr)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AttrFactors[attr].Percentage;
		}

		public static void SetPercentage(this AosAttribute attr, bool value)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), percentage: value);
			}
			else
			{
				AttrFactors[attr].Percentage = value;
			}
		}

		public static TextDefinition GetAttributeName(this AosAttribute attr)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AttrFactors[attr].Name;
		}

		public static void SetAttributeName(this AosAttribute attr, TextDefinition name)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(name);
			}
			else
			{
				AttrFactors[attr].Name = name;
			}
		}

		public static double GetIntensity(this AosAttribute attr, int value)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AttrFactors[attr].GetIntensity(value);
		}

		public static double GetWeight(this AosAttribute attr)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AttrFactors[attr].Weight;
		}

		public static double GetWeight(this AosAttribute attr, int value)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AttrFactors[attr].GetWeight(value);
		}

		public static void SetWeight(this AosAttribute attr, double weight)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), weight);
			}
			else
			{
				AttrFactors[attr].Weight = weight;
			}
		}

		public static int GetMin(this AosAttribute attr)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AttrFactors[attr].Min;
		}

		public static void SetMin(this AosAttribute attr, int min)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), min: min);
			}
			else
			{
				AttrFactors[attr].Min = min;
			}
		}

		public static int GetMax(this AosAttribute attr)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AttrFactors[attr].Max;
		}

		public static void SetMax(this AosAttribute attr, int max)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), max: max);
			}
			else
			{
				AttrFactors[attr].Max = max;
			}
		}

		public static int GetInc(this AosAttribute attr)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AttrFactors[attr].Inc;
		}

		public static void SetInc(this AosAttribute attr, int inc)
		{
			if (!AttrFactors.ContainsKey(attr))
			{
				AttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), inc: inc);
			}
			else
			{
				AttrFactors[attr].Inc = inc;
			}
		}

		public static string ToString(this AosAttribute attr, double val, bool html = true)
		{
			return GetPropertyString(val, html);
		}
		#endregion

		#region Armor Attributes
		public static bool HasAttribute(this Item item, AosArmorAttribute attr, out int value)
		{
			return (HasAttribute(item, "ArmorAttributes", (ulong)attr, out value) ||
					HasAttribute(item, "ClothingAttributes", (ulong)attr, out value) ||
					HasAttribute(item, "JewelAttributes", (ulong)attr, out value));
		}

		public static bool SupportsAttribute(this Item item, out AosArmorAttributes attrs)
		{

			if (SupportsAttributes(item, "ArmorAttributes", out var a) || SupportsAttributes(item, "ClothingAttributes", out a) ||
				SupportsAttributes(item, "JewelAttributes", out a))
			{
				attrs = (AosArmorAttributes)a;
				return true;
			}

			attrs = null;
			return false;
		}

		public static bool IsPercentage(this AosArmorAttribute attr)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ArmorAttrFactors[attr].Percentage;
		}

		public static void SetPercentage(this AosArmorAttribute attr, bool value)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), percentage: value);
			}
			else
			{
				ArmorAttrFactors[attr].Percentage = value;
			}
		}

		public static TextDefinition GetAttributeName(this AosArmorAttribute attr)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ArmorAttrFactors[attr].Name;
		}

		public static void SetAttributeName(this AosArmorAttribute attr, TextDefinition name)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(name);
			}
			else
			{
				ArmorAttrFactors[attr].Name = name;
			}
		}

		public static double GetIntensity(this AosArmorAttribute attr, int value)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ArmorAttrFactors[attr].GetIntensity(value);
		}

		public static double GetWeight(this AosArmorAttribute attr)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ArmorAttrFactors[attr].Weight;
		}

		public static double GetWeight(this AosArmorAttribute attr, int value)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ArmorAttrFactors[attr].GetWeight(value);
		}

		public static void SetWeight(this AosArmorAttribute attr, double weight)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), weight);
			}
			else
			{
				ArmorAttrFactors[attr].Weight = weight;
			}
		}

		public static int GetMin(this AosArmorAttribute attr)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ArmorAttrFactors[attr].Min;
		}

		public static void SetMin(this AosArmorAttribute attr, int min)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), min: min);
			}
			else
			{
				ArmorAttrFactors[attr].Min = min;
			}
		}

		public static int GetMax(this AosArmorAttribute attr)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ArmorAttrFactors[attr].Max;
		}

		public static void SetMax(this AosArmorAttribute attr, int max)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), max: max);
			}
			else
			{
				ArmorAttrFactors[attr].Max = max;
			}
		}

		public static int GetInc(this AosArmorAttribute attr)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ArmorAttrFactors[attr].Inc;
		}

		public static void SetInc(this AosArmorAttribute attr, int inc)
		{
			if (!ArmorAttrFactors.ContainsKey(attr))
			{
				ArmorAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), inc: inc);
			}
			else
			{
				ArmorAttrFactors[attr].Inc = inc;
			}
		}

		public static string ToString(this AosArmorAttribute attr, double val, bool html = true)
		{
			return GetPropertyString(val, html);
		}
		#endregion

		#region Weapon Attributes
		public static bool HasAttribute(this Item item, AosWeaponAttribute attr, out int value)
		{
			return HasAttribute(item, "WeaponAttributes", (ulong)attr, out value);
		}

		public static bool SupportsAttribute(this Item item, out AosWeaponAttributes attrs)
		{

			if (SupportsAttributes(item, "WeaponAttributes", out var a))
			{
				attrs = (AosWeaponAttributes)a;
				return true;
			}

			attrs = null;
			return false;
		}

		public static bool IsPercentage(this AosWeaponAttribute attr)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return WeaponAttrFactors[attr].Percentage;
		}

		public static void SetPercentage(this AosWeaponAttribute attr, bool value)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), percentage: value);
			}
			else
			{
				WeaponAttrFactors[attr].Percentage = value;
			}
		}

		public static TextDefinition GetAttributeName(this AosWeaponAttribute attr)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return WeaponAttrFactors[attr].Name;
		}

		public static void SetAttributeName(this AosWeaponAttribute attr, TextDefinition name)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(name);
			}
			else
			{
				WeaponAttrFactors[attr].Name = name;
			}
		}

		public static double GetIntensity(this AosWeaponAttribute attr, int value)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return WeaponAttrFactors[attr].GetIntensity(value);
		}

		public static double GetWeight(this AosWeaponAttribute attr)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return WeaponAttrFactors[attr].Weight;
		}

		public static double GetWeight(this AosWeaponAttribute attr, int value)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return WeaponAttrFactors[attr].GetWeight(value);
		}

		public static void SetWeight(this AosWeaponAttribute attr, double weight)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), weight);
			}
			else
			{
				WeaponAttrFactors[attr].Weight = weight;
			}
		}

		public static int GetMin(this AosWeaponAttribute attr)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return WeaponAttrFactors[attr].Min;
		}

		public static void SetMin(this AosWeaponAttribute attr, int min)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), min: min);
			}
			else
			{
				WeaponAttrFactors[attr].Min = min;
			}
		}

		public static int GetMax(this AosWeaponAttribute attr)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return WeaponAttrFactors[attr].Max;
		}

		public static void SetMax(this AosWeaponAttribute attr, int max)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), max: max);
			}
			else
			{
				WeaponAttrFactors[attr].Max = max;
			}
		}

		public static int GetInc(this AosWeaponAttribute attr)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return WeaponAttrFactors[attr].Inc;
		}

		public static void SetInc(this AosWeaponAttribute attr, int inc)
		{
			if (!WeaponAttrFactors.ContainsKey(attr))
			{
				WeaponAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), inc: inc);
			}
			else
			{
				WeaponAttrFactors[attr].Inc = inc;
			}
		}

		public static string ToString(this AosWeaponAttribute attr, double val, bool html = true)
		{
			return GetPropertyString(val, html);
		}
		#endregion

		#region Element Attributes
		public static bool HasAttribute(this Item item, AosElementAttribute attr, out int value)
		{
			return HasAttribute(item, "Resistances", (ulong)attr, out value);
		}

		public static bool SupportsAttribute(this Item item, out AosElementAttributes attrs)
		{

			if (SupportsAttributes(item, "Resistances", out var a))
			{
				attrs = (AosElementAttributes)a;
				return true;
			}

			attrs = null;
			return false;
		}

		public static bool IsPercentage(this AosElementAttribute attr)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ElementAttrFactors[attr].Percentage;
		}

		public static void SetPercentage(this AosElementAttribute attr, bool value)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), percentage: value);
			}
			else
			{
				ElementAttrFactors[attr].Percentage = value;
			}
		}

		public static TextDefinition GetAttributeName(this AosElementAttribute attr)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ElementAttrFactors[attr].Name;
		}

		public static void SetAttributeName(this AosElementAttribute attr, TextDefinition name)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(name);
			}
			else
			{
				ElementAttrFactors[attr].Name = name;
			}
		}

		public static double GetIntensity(this AosElementAttribute attr, int value)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ElementAttrFactors[attr].GetIntensity(value);
		}

		public static double GetWeight(this AosElementAttribute attr)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ElementAttrFactors[attr].Weight;
		}

		public static double GetWeight(this AosElementAttribute attr, int value)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ElementAttrFactors[attr].GetWeight(value);
		}

		public static void SetWeight(this AosElementAttribute attr, double weight)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), weight);
			}
			else
			{
				ElementAttrFactors[attr].Weight = weight;
			}
		}

		public static int GetMin(this AosElementAttribute attr)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ElementAttrFactors[attr].Min;
		}

		public static void SetMin(this AosElementAttribute attr, int min)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), min: min);
			}
			else
			{
				ElementAttrFactors[attr].Min = min;
			}
		}

		public static int GetMax(this AosElementAttribute attr)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ElementAttrFactors[attr].Max;
		}

		public static void SetMax(this AosElementAttribute attr, int max)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), max: max);
			}
			else
			{
				ElementAttrFactors[attr].Max = max;
			}
		}

		public static int GetInc(this AosElementAttribute attr)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return ElementAttrFactors[attr].Inc;
		}

		public static void SetInc(this AosElementAttribute attr, int inc)
		{
			if (!ElementAttrFactors.ContainsKey(attr))
			{
				ElementAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), inc: inc);
			}
			else
			{
				ElementAttrFactors[attr].Inc = inc;
			}
		}

		public static string ToString(this AosElementAttribute attr, double val, bool html = true)
		{
			return GetPropertyString(val, html);
		}
		#endregion

		#region Skill Bonus Attributes
		public static bool HasSkillBonus(this Item item, SkillName attr, out int value)
		{
			return HasAttribute(item, "SkillBonuses", (ulong)attr, out value);
		}

		public static bool SupportsSkillBonus(this Item item, out AosSkillBonuses attrs)
		{

			if (SupportsAttributes(item, "SkillBonuses", out var a))
			{
				attrs = (AosSkillBonuses)a;
				return true;
			}

			attrs = null;
			return false;
		}

		public static TextDefinition GetAttributeName(this SkillName attr)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), 1.4, 0, 15, 3);
			}

			return SkillBonusAttrFactors[attr].Name;
		}

		public static void SetAttributeName(this SkillName attr, TextDefinition name)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(name, 1.4, 0, 15, 3);
			}
			else
			{
				SkillBonusAttrFactors[attr].Name = name;
			}
		}

		public static double GetBonusIntensity(this SkillName attr, int value)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), 1.4, 0, 15, 3);
			}

			return SkillBonusAttrFactors[attr].GetIntensity(value);
		}

		public static double GetBonusWeight(this SkillName attr)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), 1.4, 0, 15, 3);
			}

			return SkillBonusAttrFactors[attr].Weight;
		}

		public static double GetBonusWeight(this SkillName attr, int value)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), 1.4, 0, 15, 3);
			}

			return SkillBonusAttrFactors[attr].GetWeight(value);
		}

		public static void SetBonusWeight(this SkillName attr, double weight)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), weight, 0, 15, 3);
			}
			else
			{
				SkillBonusAttrFactors[attr].Weight = weight;
			}
		}

		public static int GetBonusMin(this SkillName attr)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), 1.4, 0, 15, 3);
			}

			return SkillBonusAttrFactors[attr].Min;
		}

		public static void SetBonusMin(this SkillName attr, int min)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), 1.4, min, 15, 3);
			}
			else
			{
				SkillBonusAttrFactors[attr].Min = min;
			}
		}

		public static int GetBonusMax(this SkillName attr)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), 1.4, 0, 15, 3);
			}

			return SkillBonusAttrFactors[attr].Max;
		}

		public static void SetBonusMax(this SkillName attr, int max)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), 1.4, 0, max, 3);
			}
			else
			{
				SkillBonusAttrFactors[attr].Max = max;
			}
		}

		public static int GetBonusInc(this SkillName attr)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), 1.4, 0, 15, 3);
			}

			return SkillBonusAttrFactors[attr].Inc;
		}

		public static void SetBonusInc(this SkillName attr, int inc)
		{
			if (!SkillBonusAttrFactors.ContainsKey(attr))
			{
				SkillBonusAttrFactors[attr] = GetDefaultDefinition(attr.GetName(), 1.4, 0, 15, inc);
			}
			else
			{
				SkillBonusAttrFactors[attr].Inc = inc;
			}
		}

		public static string ToString(this SkillName attr, double val, bool html = true)
		{
			return GetPropertyString(val, html);
		}
		#endregion

		#region Slayer Attributes
		private static bool SupportsSlayers(Item item, string name, out PropertyInfo prop)
		{
			prop = item.GetType().GetProperty(name, TypeOfSlayerName);

			return prop != null;
		}

		public static bool SupportsSlayers(this Item item)
		{

			return SupportsSlayer(item, out var props);
		}

		public static bool SupportsSlayer(this Item item, out PropertyInfo[] props)
		{
			var p = new List<PropertyInfo>();

			if (SupportsSlayers(item, "Slayer", out var pi))
			{
				p.Add(pi);
			}

			if (SupportsSlayers(item, "Slayer1", out pi))
			{
				p.Add(pi);
			}

			if (SupportsSlayers(item, "Slayer2", out pi))
			{
				p.Add(pi);
			}

			if (SupportsSlayers(item, "Slayer3", out pi))
			{
				p.Add(pi);
			}

			if (SupportsSlayers(item, "Slayer4", out pi))
			{
				p.Add(pi);
			}

			props = p.ToArray();
			return props.Length > 0;
		}

		private static bool HasSlayer(Item item, string name, SlayerName attr)
		{
			var pi = item.GetType().GetProperty(name, TypeOfSlayerName);

			if (pi == null)
			{
				return false;
			}

			return ((SlayerName)pi.GetValue(item, null) == attr);
		}

		public static bool HasSlayer(this Item item, SlayerName attr)
		{
			return (HasSlayer(item, "Slayer", attr) || HasSlayer(item, "Slayer1", attr) || HasSlayer(item, "Slayer2", attr) ||
					HasSlayer(item, "Slayer3", attr) || HasSlayer(item, "Slayer4", attr));
		}

		public static TextDefinition GetAttributeName(this SlayerName attr)
		{
			if (attr == SlayerName.None)
			{
				return "Unknown";
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return SlayerAttrFactors[attr].Name;
		}

		public static void SetAttributeName(this SlayerName attr, TextDefinition name)
		{
			if (attr == SlayerName.None)
			{
				return;
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(name, IsSuper(attr) ? 1.3 : 1.1);
			}
			else
			{
				SlayerAttrFactors[attr].Name = name;
			}
		}

		public static double GetIntensity(this SlayerName attr, int value)
		{
			if (attr == SlayerName.None)
			{
				return 0;
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return SlayerAttrFactors[attr].GetIntensity(value);
		}

		public static double GetWeight(this SlayerName attr)
		{
			if (attr == SlayerName.None)
			{
				return 0;
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return SlayerAttrFactors[attr].Weight;
		}

		public static double GetWeight(this SlayerName attr, int value)
		{
			if (attr == SlayerName.None)
			{
				return 0;
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return SlayerAttrFactors[attr].GetWeight(value);
		}

		public static void SetWeight(this SlayerName attr, double weight)
		{
			if (attr == SlayerName.None)
			{
				return;
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), weight);
			}
			else
			{
				SlayerAttrFactors[attr].Weight = weight;
			}
		}

		public static int GetMin(this SlayerName attr)
		{
			if (attr == SlayerName.None)
			{
				return 0;
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return SlayerAttrFactors[attr].Min;
		}

		public static void SetMin(this SlayerName attr, int min)
		{
			if (attr == SlayerName.None)
			{
				return;
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1, min);
			}
			else
			{
				SlayerAttrFactors[attr].Min = min;
			}
		}

		public static int GetMax(this SlayerName attr)
		{
			if (attr == SlayerName.None)
			{
				return 0;
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return SlayerAttrFactors[attr].Max;
		}

		public static void SetMax(this SlayerName attr, int max)
		{
			if (attr == SlayerName.None)
			{
				return;
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1, max: max);
			}
			else
			{
				SlayerAttrFactors[attr].Max = max;
			}
		}

		public static int GetInc(this SlayerName attr)
		{
			if (attr == SlayerName.None)
			{
				return 0;
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return SlayerAttrFactors[attr].Inc;
		}

		public static void SetInc(this SlayerName attr, int inc)
		{
			if (attr == SlayerName.None)
			{
				return;
			}

			if (!SlayerAttrFactors.ContainsKey(attr))
			{
				SlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1, inc: inc);
			}
			else
			{
				SlayerAttrFactors[attr].Inc = inc;
			}
		}

		public static bool IsSuper(this SlayerName attr)
		{
			return SuperSlayers.Contains(attr);
		}

		public static string ToString(this SlayerName attr, double val, bool html = true)
		{
			return GetPropertyString(val, html);
		}
		#endregion

		#region Talisman Slayer Attributes
		private static bool SupportsTalismanSlayers(Item item, string name, out PropertyInfo prop)
		{
			prop = item.GetType().GetProperty(name, TypeOfTalismanSlayerName);

			return prop != null;
		}

		public static bool SupportsTalismanSlayers(this Item item)
		{

			return SupportsTalismanSlayer(item, out var props);
		}

		public static bool SupportsTalismanSlayer(this Item item, out PropertyInfo[] props)
		{
			var p = new List<PropertyInfo>();


			if (SupportsTalismanSlayers(item, "Slayer", out var pi))
			{
				p.Add(pi);
			}

			if (SupportsTalismanSlayers(item, "Slayer1", out pi))
			{
				p.Add(pi);
			}

			if (SupportsTalismanSlayers(item, "Slayer2", out pi))
			{
				p.Add(pi);
			}

			if (SupportsTalismanSlayers(item, "Slayer3", out pi))
			{
				p.Add(pi);
			}

			if (SupportsTalismanSlayers(item, "Slayer4", out pi))
			{
				p.Add(pi);
			}

			props = p.ToArray();
			return props.Length > 0;
		}

		private static bool HasTalismanSlayer(Item item, string name, TalismanSlayerName attr)
		{
			var pi = item.GetType().GetProperty(name, TypeOfTalismanSlayerName);

			if (pi == null)
			{
				return false;
			}

			return ((TalismanSlayerName)pi.GetValue(item, null) == attr);
		}

		public static bool HasSlayer(this Item item, TalismanSlayerName attr)
		{
			return (HasTalismanSlayer(item, "Slayer", attr) || HasTalismanSlayer(item, "Slayer1", attr) ||
					HasTalismanSlayer(item, "Slayer2", attr) || HasTalismanSlayer(item, "Slayer3", attr) ||
					HasTalismanSlayer(item, "Slayer4", attr));
		}

		public static TextDefinition GetAttributeName(this TalismanSlayerName attr)
		{
			if (attr == TalismanSlayerName.None)
			{
				return "None";
			}

			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return TalismanSlayerAttrFactors[attr].Name;
		}

		public static void SetAttributeName(this TalismanSlayerName attr, TextDefinition name)
		{
			if (attr == TalismanSlayerName.None)
			{
				return;
			}

			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(name, IsSuper(attr) ? 1.3 : 1.1);
			}
			else
			{
				TalismanSlayerAttrFactors[attr].Name = name;
			}
		}

		public static double GetIntensity(this TalismanSlayerName attr, int value)
		{
			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return TalismanSlayerAttrFactors[attr].GetIntensity(value);
		}

		public static double GetWeight(this TalismanSlayerName attr)
		{
			if (attr == TalismanSlayerName.None)
			{
				return 0;
			}

			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return TalismanSlayerAttrFactors[attr].Weight;
		}

		public static double GetWeight(this TalismanSlayerName attr, int value)
		{
			if (attr == TalismanSlayerName.None)
			{
				return 0;
			}

			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return TalismanSlayerAttrFactors[attr].GetWeight(value);
		}

		public static void SetWeight(this TalismanSlayerName attr, double weight)
		{
			if (attr == TalismanSlayerName.None)
			{
				return;
			}

			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), weight);
			}
			else
			{
				TalismanSlayerAttrFactors[attr].Weight = weight;
			}
		}

		public static int GetMin(this TalismanSlayerName attr)
		{
			if (attr == TalismanSlayerName.None)
			{
				return 0;
			}

			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return TalismanSlayerAttrFactors[attr].Min;
		}

		public static void SetMin(this TalismanSlayerName attr, int min)
		{
			if (attr == TalismanSlayerName.None)
			{
				return;
			}

			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1, min);
			}
			else
			{
				TalismanSlayerAttrFactors[attr].Min = min;
			}
		}

		public static int GetMax(this TalismanSlayerName attr)
		{
			if (attr == TalismanSlayerName.None)
			{
				return 0;
			}

			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return TalismanSlayerAttrFactors[attr].Max;
		}

		public static void SetMax(this TalismanSlayerName attr, int max)
		{
			if (attr == TalismanSlayerName.None)
			{
				return;
			}

			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1, max: max);
			}
			else
			{
				TalismanSlayerAttrFactors[attr].Max = max;
			}
		}

		public static int GetInc(this TalismanSlayerName attr)
		{
			if (attr == TalismanSlayerName.None)
			{
				return 0;
			}

			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1);
			}

			return TalismanSlayerAttrFactors[attr].Inc;
		}

		public static void SetInc(this TalismanSlayerName attr, int inc)
		{
			if (attr == TalismanSlayerName.None)
			{
				return;
			}

			if (!TalismanSlayerAttrFactors.ContainsKey(attr))
			{
				TalismanSlayerAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), IsSuper(attr) ? 1.3 : 1.1, inc: inc);
			}
			else
			{
				TalismanSlayerAttrFactors[attr].Inc = inc;
			}
		}

		public static bool IsSuper(this TalismanSlayerName attr)
		{
			return SuperTalismanSlayers.Contains(attr);
		}

		public static string ToString(this TalismanSlayerName attr, double val, bool html = true)
		{
			return GetPropertyString(val, html);
		}
		#endregion

		#region Absorption Attributes
		public static bool HasAttribute(this Item item, SAAbsorptionAttribute attr, out int value)
		{
			return HasAttribute(item, "AbsorptionAttributes", (ulong)attr, out value);
		}

		public static bool SupportsAttribute(this Item item, out SAAbsorptionAttributes attrs)
		{

			if (SupportsAttributes(item, "AbsorptionAttributes", out var a))
			{
				attrs = (SAAbsorptionAttributes)a;
				return true;
			}

			attrs = null;
			return false;
		}

		public static bool IsPercentage(this SAAbsorptionAttribute attr)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AbsorptionAttrFactors[attr].Percentage;
		}

		public static void SetPercentage(this SAAbsorptionAttribute attr, bool value)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), percentage: value);
			}
			else
			{
				AbsorptionAttrFactors[attr].Percentage = value;
			}
		}

		public static TextDefinition GetAttributeName(this SAAbsorptionAttribute attr)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AbsorptionAttrFactors[attr].Name;
		}

		public static void SetAttributeName(this SAAbsorptionAttribute attr, TextDefinition name)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(name);
			}
			else
			{
				AbsorptionAttrFactors[attr].Name = name;
			}
		}

		public static double GetIntensity(this SAAbsorptionAttribute attr, int value)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AbsorptionAttrFactors[attr].GetIntensity(value);
		}

		public static double GetWeight(this SAAbsorptionAttribute attr)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AbsorptionAttrFactors[attr].Weight;
		}

		public static double GetWeight(this SAAbsorptionAttribute attr, int value)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AbsorptionAttrFactors[attr].GetWeight(value);
		}

		public static void SetWeight(this SAAbsorptionAttribute attr, double weight)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), weight);
			}
			else
			{
				AbsorptionAttrFactors[attr].Weight = weight;
			}
		}

		public static int GetMin(this SAAbsorptionAttribute attr)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AbsorptionAttrFactors[attr].Min;
		}

		public static void SetMin(this SAAbsorptionAttribute attr, int min)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), min: min);
			}
			else
			{
				AbsorptionAttrFactors[attr].Min = min;
			}
		}

		public static int GetMax(this SAAbsorptionAttribute attr)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AbsorptionAttrFactors[attr].Max;
		}

		public static void SetMax(this SAAbsorptionAttribute attr, int max)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), max: max);
			}
			else
			{
				AbsorptionAttrFactors[attr].Max = max;
			}
		}

		public static int GetInc(this SAAbsorptionAttribute attr)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true));
			}

			return AbsorptionAttrFactors[attr].Inc;
		}

		public static void SetInc(this SAAbsorptionAttribute attr, int inc)
		{
			if (!AbsorptionAttrFactors.ContainsKey(attr))
			{
				AbsorptionAttrFactors[attr] = GetDefaultDefinition(attr.ToString(true), inc: inc);
			}
			else
			{
				AbsorptionAttrFactors[attr].Inc = inc;
			}
		}

		public static string ToString(this SAAbsorptionAttribute attr, double val, bool html = true)
		{
			return GetPropertyString(val, html);
		}
		#endregion
	}
}
