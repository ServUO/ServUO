using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public enum AttributeCategory
	{
		Misc = 0x00000001,
		Melee = 0x00000002,
		Magic = 0x00000004,
		Stats = 0x00000008,
		Resists = 0x00000010,
		Hits = 0x00000020
	}

	public class LevelAttributes
	{
		public static AttributeInfo[] m_Attributes = new AttributeInfo[]
		{
			new AttributeInfo( AosAttribute.RegenHits, "Regen Hits", AttributeCategory.Stats, 5, 4 ),
			new AttributeInfo( AosAttribute.RegenStam, "Regen Stamina", AttributeCategory.Stats, 5, 4 ),
			new AttributeInfo( AosAttribute.RegenMana, "Regen Mana", AttributeCategory.Stats, 5, 4 ),
			new AttributeInfo( AosAttribute.DefendChance, "Defence Chance Increase", AttributeCategory.Melee, 8, 15 ),
			new AttributeInfo( AosAttribute.AttackChance, "Hit Chance Increase", AttributeCategory.Melee, 10, 15 ),
			new AttributeInfo( AosAttribute.BonusStr, "Bonus Strength", AttributeCategory.Stats, 10, 8 ),
			new AttributeInfo( AosAttribute.BonusDex, "Bonus Dex", AttributeCategory.Stats, 10, 8 ),
			new AttributeInfo( AosAttribute.BonusInt, "Bonus Int", AttributeCategory.Stats, 10, 8 ),
			new AttributeInfo( AosAttribute.BonusHits, "Bonus Hits", AttributeCategory.Stats, 5, 8 ),
			new AttributeInfo( AosAttribute.BonusStam, "Bonus Stamina", AttributeCategory.Stats, 5, 8 ),
			new AttributeInfo( AosAttribute.BonusMana, "Bonus Mana", AttributeCategory.Stats, 5, 8 ),
			new AttributeInfo( AosAttribute.WeaponDamage, "Damage Increase", AttributeCategory.Melee, 5, 50 ),
			new AttributeInfo( AosAttribute.WeaponSpeed, "Swing Speed Increase", AttributeCategory.Melee, 6, 40 ),
			new AttributeInfo( AosAttribute.SpellDamage, "Spell Damage", AttributeCategory.Magic, 4, 20 ),
			new AttributeInfo( AosAttribute.CastRecovery, "Faster Cast Recovery", AttributeCategory.Magic, 20, 4 ),
			new AttributeInfo( AosAttribute.CastSpeed, "Faster Casting", AttributeCategory.Magic, 20, 4 ),
			new AttributeInfo( AosAttribute.LowerManaCost, "Lower Mana Cost", AttributeCategory.Magic, 5, 10 ),
			new AttributeInfo( AosAttribute.LowerRegCost, "Lower Reagent Cost", AttributeCategory.Magic, 5, 20 ),
			new AttributeInfo( AosAttribute.ReflectPhysical, "Reflect Physical Damage", AttributeCategory.Melee, 2, 15 ),
			new AttributeInfo( AosAttribute.EnhancePotions, "Enhance Potions", AttributeCategory.Magic, 2, 25 ),
			new AttributeInfo( AosAttribute.Luck, "Luck", AttributeCategory.Misc, 2, 100 ),
			new AttributeInfo( AosAttribute.SpellChanneling, "Spell Channeling", AttributeCategory.Magic, 15, 1 ),
			new AttributeInfo( AosAttribute.NightSight, "Nightsight", AttributeCategory.Misc, 6, 1 )
		};

        //Weapon Specific
		public static WeaponAttributeInfo[] m_WeaponAttributes = new WeaponAttributeInfo[]
		{
			new WeaponAttributeInfo( AosWeaponAttribute.LowerStatReq, "Lower Stat Requirement", AttributeCategory.Stats, 2, 100 ),
			new WeaponAttributeInfo( AosWeaponAttribute.SelfRepair, "Self Repair", AttributeCategory.Misc, 2, 10 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitLeechHits, "Hit Life Leech", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitLeechStam, "Hit Stamina Leech", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitLeechMana, "Hit Mana Leech", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitLowerAttack, "Hit Lower Attack", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitLowerDefend, "Hit Lower Defence", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitMagicArrow, "Hit Magic Arrow", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitHarm, "Hit Harm", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitFireball, "Hit Fireball", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitLightning, "Hit Lightning", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitDispel, "Hit Dispel", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitColdArea, "Hit Cold Area", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitFireArea, "Hit Fire Area", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitPoisonArea, "Hit Poison Area", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitEnergyArea, "Hit Energy Area", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.HitPhysicalArea, "Hit Physical Area", AttributeCategory.Hits, 3, 50 ),
			new WeaponAttributeInfo( AosWeaponAttribute.ResistPhysicalBonus, "Resist Physical Bonus", AttributeCategory.Resists, 40, 20 ),
			new WeaponAttributeInfo( AosWeaponAttribute.ResistFireBonus, "Resist Fire Bonus", AttributeCategory.Resists, 5, 20 ),
			new WeaponAttributeInfo( AosWeaponAttribute.ResistColdBonus, "Resist Cold Bonus", AttributeCategory.Resists, 5, 20 ),
			new WeaponAttributeInfo( AosWeaponAttribute.ResistPoisonBonus, "Resist Poison Bonus", AttributeCategory.Resists, 5, 20 ),
			new WeaponAttributeInfo( AosWeaponAttribute.ResistEnergyBonus, "Resist Energy Bonus", AttributeCategory.Resists, 5, 20 ),
			new WeaponAttributeInfo( AosWeaponAttribute.UseBestSkill, "Use Best Weapon Skill", AttributeCategory.Misc, 10, 1 ),
			new WeaponAttributeInfo( AosWeaponAttribute.MageWeapon, "Mage Weapon", AttributeCategory.Magic, 5, 25 ),
            new WeaponAttributeInfo( AosWeaponAttribute.DurabilityBonus, "Durability Bonus", AttributeCategory.Misc, 1, 255 )
		};

		//Armor specific attributes
        public static ArmorAttributeInfo[] m_ArmorAttributes = new ArmorAttributeInfo[]
		{
            new ArmorAttributeInfo( AosArmorAttribute.LowerStatReq, "Lower Stat Requirement", AttributeCategory.Stats, 2, 100 ),
            new ArmorAttributeInfo( AosArmorAttribute.SelfRepair, "Self Repair", AttributeCategory.Misc, 2, 5 ),
            new ArmorAttributeInfo( AosArmorAttribute.MageArmor, "Mage Armor", AttributeCategory.Magic, 5, 1 ),
            new ArmorAttributeInfo( AosArmorAttribute.DurabilityBonus, "Durability Bonus", AttributeCategory.Misc, 1, 255 )
        };

        //Armor specific
        public static ResistanceTypeInfo[] m_ResistanceTypes = new ResistanceTypeInfo[]
		{
            new ResistanceTypeInfo( ResistanceType.Physical, "Physical Resistance", AttributeCategory.Resists, 2, 20 ),
            new ResistanceTypeInfo( ResistanceType.Fire, "Fire Resistance", AttributeCategory.Resists, 2, 20 ),
            new ResistanceTypeInfo( ResistanceType.Cold, "Cold Resistance", AttributeCategory.Resists, 2, 20 ),
            new ResistanceTypeInfo( ResistanceType.Poison, "Poison Resistance", AttributeCategory.Resists, 2, 20 ),
            new ResistanceTypeInfo( ResistanceType.Energy, "Energy Resistance", AttributeCategory.Resists, 2, 20 )
        };

        //Jewel & Clothing Specific Resists
        public static ElementAttributeInfo[] m_ElementAttributes = new ElementAttributeInfo[]
		{
            new ElementAttributeInfo( AosElementAttribute.Physical, "Physical Resistance", AttributeCategory.Resists, 2, 20 ),
            new ElementAttributeInfo( AosElementAttribute.Fire, "Fire Resistance", AttributeCategory.Resists, 2, 20 ),
            new ElementAttributeInfo( AosElementAttribute.Cold, "Cold Resistance", AttributeCategory.Resists, 2, 20 ),
            new ElementAttributeInfo( AosElementAttribute.Poison, "Poison Resistance", AttributeCategory.Resists, 2, 20 ),
            new ElementAttributeInfo( AosElementAttribute.Energy, "Energy Resistance", AttributeCategory.Resists, 2, 20 )
        };
	}

	#region " Info Classes "

	public class AttributeInfo
	{
		public AosAttribute m_Attribute;
		public string m_Name;
		public AttributeCategory m_Category;
		public int m_XP;
		public int m_MaxValue;

		public AttributeInfo( AosAttribute attribute, string name, AttributeCategory category, int xp, int maxvalue )
		{
			m_Attribute = attribute;
			m_Name = name;
			m_Category = category;
			m_XP = xp;
			m_MaxValue = maxvalue;
		}
	}

	public class WeaponAttributeInfo
	{
		public AosWeaponAttribute m_Attribute;
		public string m_Name;
		public AttributeCategory m_Category;
		public int m_XP;
		public int m_MaxValue;

		public WeaponAttributeInfo( AosWeaponAttribute attribute, string name, AttributeCategory category, int xp, int maxvalue )
		{
			m_Attribute = attribute;
			m_Name = name;
			m_Category = category;
			m_XP = xp;
			m_MaxValue = maxvalue;
		}
	}

    public class ArmorAttributeInfo
    {
        public AosArmorAttribute m_Attribute;
        public string m_Name;
        public AttributeCategory m_Category;
        public int m_XP;
        public int m_MaxValue;

        public ArmorAttributeInfo(AosArmorAttribute attribute, string name, AttributeCategory category, int xp, int maxvalue)
        {
            m_Attribute = attribute;
            m_Name = name;
            m_Category = category;
            m_XP = xp;
            m_MaxValue = maxvalue;
        }
    }

    public class ResistanceTypeInfo
    {
        public ResistanceType m_Attribute;
        public string m_Name;
        public AttributeCategory m_Category;
        public int m_XP;
        public int m_MaxValue;

        public ResistanceTypeInfo(ResistanceType attribute, string name, AttributeCategory category, int xp, int maxvalue)
        {
            m_Attribute = attribute;
            m_Name = name;
            m_Category = category;
            m_XP = xp;
            m_MaxValue = maxvalue;
        }
    }

    public class ElementAttributeInfo
    {
        public AosElementAttribute m_Attribute;
        public string m_Name;
        public AttributeCategory m_Category;
        public int m_XP;
        public int m_MaxValue;

        public ElementAttributeInfo(AosElementAttribute attribute, string name, AttributeCategory category, int xp, int maxvalue)
        {
            m_Attribute = attribute;
            m_Name = name;
            m_Category = category;
            m_XP = xp;
            m_MaxValue = maxvalue;
        }
    }
	#endregion
}