using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles.MannequinProperty
{
    public abstract class WeaponAttr : ValuedProperty
    {
        public override bool IsMagical => true;
        public abstract AosWeaponAttribute Attribute { get; }

        public double GetPropertyValue(Item item)
        {
            return item is BaseWeapon weapon ? weapon.WeaponAttributes[Attribute] : 0;
        }

        public override bool Matches(Item item)
        {
            double total = GetPropertyValue(item);

            if (!IsBoolen)
                Value = GetPropertyValue(item);

            if (total != 0)
                return true;

            return false;
        }

        public override bool Matches(List<Item> items)
        {
            double total = 0;

            items.ForEach(x => total += GetPropertyValue(x));

            if (!IsBoolen)
                Value = total;

            if (total != 0)
            {
                return true;
            }

            return false;
        }
    }

    public class WeaponSelfRepairProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.None;
        public override int LabelNumber => 1079709;  // Self Repair
        public override bool IsSpriteGraph => true;
        public override int SpriteW => 0;
        public override int SpriteH => 300;
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.SelfRepair;
    }

    public class HitLeechHitsProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079698;  // Hit Life Leech
        public override int Description => 1152425;  // This property provides a chance to leech hit points from the target.  The amount of hit points leeched from the target is up to 30% of the damage inflicted.  This property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLeechHits;
        public override int Hue => 0x43FF;
        public override int SpriteW => 90;
        public override int SpriteH => 180;
    }

    public class HitLeechStamProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079707;  // Hit Stamina Leech
        public override int Description => 1152423;  // This property provides a chance to leech stamina from the target.  The amount of stamina leeched from the target is 100% of the damage inflicted.  This property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLeechStam;
        public override int Hue => 0x43FF;
        public override int SpriteW => 150;
        public override int SpriteH => 180;
    }

    public class HitLeechManaProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079701;  // Hit Mana Leech
        public override int Description => 1152424;  // This property provides a chance to leech mana from the target.  The amount of mana leeched from the target is up to 40% of the damage inflicted.  This property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLeechMana;
        public override int Hue => 0x43FF;
        public override int SpriteW => 120;
        public override int SpriteH => 180;
    }

    public class HitLowerAttackProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079699;  // Hit Lower Attack
        public override int Description => 1152420;  // This property provides a chance to lower the target’s hit chance increase.  The effect reduces the target’s hit chance increase by 25% (40% hit chance increase is reduced to 15% hit chance increase) for 10 seconds if inflicted by melee weapons and 7 seconds for ranged weapons.  This property does not stack from different sources and each successful hit while the target is under the effect will reset the duration timer.  This property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLowerAttack;
        public override int Hue => 0x43FF;
        public override int SpriteW => 0;
        public override int SpriteH => 120;
    }

    public class HitLowerDefendProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079700;  // Hit Lower Defense
        public override int Description => 1152419;  // This property provides a chance to lower the target’s defense chance increase.  The effect reduces the target’s defense chance increase by 25% (40% defense chance increase is reduced to 15% defense chance increase) for 8 seconds if inflicted by melee weapons and 5 seconds for ranged weapons.  This property does not stack from different sources and each successful hit while the target is under the effect will reset the duration timer.  This property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLowerDefend;
        public override int Hue => 0x43FF;
        public override int SpriteW => 30;
        public override int SpriteH => 120;
    }

    public class HitMagicArrowProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079706;  // Hit Magic Arrow
        public override int Description => 1152427;  // This property provides a chance to cast a magic arrow spell on the target.  The magic arrow effect is similar to the magery first circle spell magic arrow.  The property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitMagicArrow;
        public override int Hue => 0x43FF;
        public override int SpriteW => 270;
        public override int SpriteH => 150;
    }

    public class HitHarmProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079704;  // Hit Harm
        public override int Description => 1152413;  // This property provides a chance to cast a harm spell on the target.  The harm effect is similar to the magery second circle spell harm.  This property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitHarm;
        public override int Hue => 0x43FF;
        public override int SpriteW => 0;
        public override int SpriteH => 180;
    }

    public class HitFireballProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079703;  // Hit Fireball
        public override int Description => 1152412;  // This property provides a chance to cast a fireball spell on the target.  The fireball effect is similar to the magery third circle spell fireball.  This property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitFireball;
        public override int Hue => 0x43FF;
        public override int SpriteW => 210;
        public override int SpriteH => 150;
    }

    public class HitLightningProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079705;  // Hit Lightning
        public override int Description => 1152426;  // This property provides a chance to cast a lightning spell on the target.  The lightning effect is similar to the magery fourth circle spell lightning.  The property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitLightning;
        public override int Hue => 0x43FF;
        public override int SpriteW => 240;
        public override int SpriteH => 150;
    }

    public class HitDispelProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079702;  // Hit Dispel
        public override int Description => 1152409;  // This property provides a chance to dispel a summoned creature.  The chance to dispel the target is based on the player’s tactics skill level.  This property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitDispel;
        public override int Hue => 0x43FF;
        public override int SpriteW => 30;
        public override int SpriteH => 180;
    }

    public class HitColdAreaProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079693;  // Hit Cold Area
        public override int Description => 1152422;  // This property provides a chance to deal additional splash damage.  The hit area property provides a chance to deal additional (physical, fire, cold, poison or energy) damage based on half of the weapon damage inflicted to the primary target.  The splash damage is not inflicted to the original target, but is inflicted to legally attackable targets near the original target.  This property can be found on all weapons. 
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitColdArea;
        public override int Hue => 0x43FF;
        public override int SpriteW => 120;
        public override int SpriteH => 150;
    }

    public class HitFireAreaProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079695;  // Hit Fire Area
        public override int Description => 1152422;  // This property provides a chance to deal additional splash damage.  The hit area property provides a chance to deal additional (physical, fire, cold, poison or energy) damage based on half of the weapon damage inflicted to the primary target.  The splash damage is not inflicted to the original target, but is inflicted to legally attackable targets near the original target.  This property can be found on all weapons. 
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitFireArea;
        public override int Hue => 0x43FF;
        public override int SpriteW => 90;
        public override int SpriteH => 150;
    }

    public class HitPoisonAreaProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079697;  // Hit Poison Area
        public override int Description => 1152422;  // This property provides a chance to deal additional splash damage.  The hit area property provides a chance to deal additional (physical, fire, cold, poison or energy) damage based on half of the weapon damage inflicted to the primary target.  The splash damage is not inflicted to the original target, but is inflicted to legally attackable targets near the original target.  This property can be found on all weapons. s
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitPoisonArea;
        public override int Hue => 0x43FF;
        public override int SpriteW => 150;
        public override int SpriteH => 150;
    }

    public class HitEnergyAreaProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079694;  // Hit Energy Area
        public override int Description => 1152422;  // This property provides a chance to deal additional splash damage.  The hit area property provides a chance to deal additional (physical, fire, cold, poison or energy) damage based on half of the weapon damage inflicted to the primary target.  The splash damage is not inflicted to the original target, but is inflicted to legally attackable targets near the original target.  This property can be found on all weapons. 
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitEnergyArea;
        public override int Hue => 0x43FF;
        public override int SpriteW => 180;
        public override int SpriteH => 150;
    }

    public class HitPhysicalAreaProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1079696;  // Hit Physical Area
        public override int Description => 1152422;  // This property provides a chance to deal additional splash damage.  The hit area property provides a chance to deal additional (physical, fire, cold, poison or energy) damage based on half of the weapon damage inflicted to the primary target.  The splash damage is not inflicted to the original target, but is inflicted to legally attackable targets near the original target.  This property can be found on all weapons. 
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitPhysicalArea;
        public override int Hue => 0x43FF;
        public override int SpriteW => 60;
        public override int SpriteH => 150;
    }

    public class UseBestWeaponSkillProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.Combat1;
        public override bool IsBoolen => true;
        public override int LabelNumber => 1079592;  // Use Best Weapon Skill
        public override int Description => 1152393;  // This property allows a player to use a weapon as long as they have a weapon skill (fencing, mace fighting or swordsmanship).  The weapon functions as if the player’s highest weapon skill is the appropriate skill.  The wrestling, archery and throwing skills are excluded from this property.  This property is only found on melee weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.UseBestSkill;
        public override int Hue => 0x5E5D;
        public override int SpriteW => 30;
        public override int SpriteH => 330;
    }

    public class MageWeaponProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.Casting;
        public override int LabelNumber => 1079759;  // Mage Weapon
        public override int Description => 1152428;  // This property modifies a weapon so that it uses a player’s magery skill as the weapons required weapon skill instead of the weapons default required weapon skill (archery, fencing, throwing, etc.).  The wielder will also be subject to any listed penalty to their magery skill.  This property can only be found on weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.MageWeapon;
        public override int Hue => 0x1FF;
        public override int SpriteW => 0;
        public override int SpriteH => 150;
    }

    public class BloodDrinkerProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override bool IsBoolen => true;
        public override int LabelNumber => 1113591;  // Blood Drinker
        public override int Description => 1152387;  // This property will allow an attacker to gain hit points when using the bleed attack special move.  All of the damage inflicted through the bleed attack is directly transferred back to the attacker’s health.  This property is only found on weapons that allow use of the bleed attack special move.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.BloodDrinker;
        public override int Hue => 0x43FF;
        public override int SpriteW => 120;
        public override int SpriteH => 120;
    }

    public class BattleLustProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override bool IsBoolen => true;
        public override int LabelNumber => 1113710;  // Battle Lust
        public override int Description => 1152385;  // This property initiates a damage increase that is modified by several factors.  Significant damage received from opponents will be added to the attacker’s Battle Lust, causing them to do more damage to all opponents the attacker engages.  This damage bonus is further modified by how many opponents the attacker is aggressed against.  The damage bonus is 15% per opponent, with a cap of 45% in PvP and 90% in PvM.  Battle Lust is gained every two seconds and decays at a rate of one point every six seconds.  This property can be found on most weapon types.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.BattleLust;
        public override int Hue => 0x43FF;
        public override int SpriteW => 150;
        public override int SpriteH => 120;
    }

    public class HitCurseProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1154673;  // Hit Curse
        public override int Description => 1152438;  // This property provides a chance to curse the target.  The curse effect is similar to the magery third circle spell curse.  This effect can only fire once every 30 seconds.  This property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitCurse;
        public override int Hue => 0x43FF;
        public override int SpriteW => 60;
        public override int SpriteH => 180;
    }

    public class HitFatigueProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1154668;  // Hit Fatigue
        public override int Description => 1152437;  // This property provides a chance to reduce the target’s current stamina by 20% of the damage inflicted by the attack.  This property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitFatigue;
        public override int Hue => 0x43FF;
        public override int SpriteW => 240;
        public override int SpriteH => 180;
    }

    public class HitManaDrainProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1154669;  // Hit Mana Drain
        public override int Description => 1152436;  // This property provides a chance to reduce the target’s current mana by 20% of the damage inflicted by the attack.  This property can be found on all weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.HitManaDrain;
        public override int Hue => 0x43FF;
        public override int SpriteW => 210;
        public override int SpriteH => 180;
    }

    public class SplinteringWeaponProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1154670;  // Splintering Weapon
        public override int Description => 1152396;  // This property provides a chance to apply a bleed effect and a four second duration forced walk to the target.  Each application of the splintering weapons reduces the durability of the weapon.  This property can be found on melee weapons.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.SplinteringWeapon;
        public override int Hue => 0x43FF;
        public override int SpriteW => 60;
        public override int SpriteH => 120;
    }

    public class WeaponReactiveParalyzeProperty : WeaponAttr
    {
        public override Catalog Catalog => Catalog.Combat1;
        public override bool IsBoolen => true;
        public override int LabelNumber => 1154660;  // Reactive Paralyze
        public override int Description => 1152400;  // This property provides a chance to cast a paralyze spell on an opponent.  The paralyze effect is similar to the magery fifth circle spell paralyze.  When the wielder effectively parries an attacker’s blow there is a chance that the attacker will be affected by the paralyze effect.   This property can be found on two handed weapons and shields.
        public override AosWeaponAttribute Attribute => AosWeaponAttribute.ReactiveParalyze;
        public override int Hue => 0x5E5D;
        public override int SpriteW => 270;
        public override int SpriteH => 120;
    }

    public abstract class ElementalDamageAttr : ValuedProperty
    {
        public override bool IsMagical => true;
        public override Catalog Catalog => Catalog.Combat2;
        public override bool AlwaysVisible => true;
        public abstract AosElementAttribute Element { get; }
        public override int Hue => 0x5E5D;

        public double GetPropertyValue(Item item)
        {
            return item is BaseWeapon weapon ? GetElementAttributeValue(weapon, Element) : 0;
        }

        public override bool Matches(Item item)
        {
            Value = GetPropertyValue(item);

            if (Value != 0)
            {
                return true;
            }

            return false;
        }

        public override bool Matches(List<Item> items)
        {
            double total = 0;

            items.ForEach(x => total += GetPropertyValue(x));

            Value = total;

            if (Value != 0)
            {
                return true;
            }

            return false;
        }

        private static int GetElementAttributeValue(BaseWeapon weapon, AosElementAttribute element)
        {
            weapon.GetDamageTypes(null, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct);

            switch (element)
            {
                case AosElementAttribute.Physical:
                    return phys;
                case AosElementAttribute.Fire:
                    return fire;
                case AosElementAttribute.Cold:
                    return cold;
                case AosElementAttribute.Poison:
                    return pois;
                case AosElementAttribute.Energy:
                    return nrgy;
                case AosElementAttribute.Chaos:
                    return chaos;
                case AosElementAttribute.Direct:
                    return direct;
            }

            return -1;
        }
    }

    public class PhysicalDamageProperty : ElementalDamageAttr
    {
        public override int LabelNumber => 1151800;  // Physical Damage
        public override int Description => 1152461;  // This property indicates that a percentage of the damage inflicted by the weapon (or spell) will be physical damage.  Physical damage is applied against an opponent’s physical resistance.
        public override AosElementAttribute Element => AosElementAttribute.Physical;
        public override int SpriteW => 270;
        public override int SpriteH => 210;
    }

    public class ColdDamageProperty : ElementalDamageAttr
    {
        public override int LabelNumber => 1151802;  // Cold Damage
        public override int Description => 1152462;  // This property indicates that a percentage of the damage inflicted by the weapon (or spell) will be cold damage.  Cold damage is applied against an opponent’s cold resistance.
        public override AosElementAttribute Element => AosElementAttribute.Cold;
        public override int SpriteW => 210;
        public override int SpriteH => 210;
    }

    public class FireDamageProperty : ElementalDamageAttr
    {
        public override int LabelNumber => 1151801;  // Fire Damage
        public override int Description => 1152472;  // This property indicates that a percentage of the damage inflicted by the weapon (or spell) will be fire damage.  Fire damage is applied against an opponent’s fire resistance.
        public override AosElementAttribute Element => AosElementAttribute.Fire;
        public override int SpriteW => 180;
        public override int SpriteH => 210;
    }

    public class PoisonDamageProperty : ElementalDamageAttr
    {
        public override int LabelNumber => 1151803;  // Poison Damage
        public override int Description => 1152473;  // This property indicates that a percentage of the damage inflicted by the weapon (or spell) will be poison damage.  Poison damage is applied against an opponent’s poison resistance.
        public override AosElementAttribute Element => AosElementAttribute.Poison;
        public override int SpriteW => 0;
        public override int SpriteH => 240;
    }

    public class EnergyDamageProperty : ElementalDamageAttr
    {
        public override int LabelNumber => 1151804;  // Energy Damage
        public override int Description => 1152474;  // This property indicates that a percentage of the damage inflicted by the weapon (or spell) will be energy damage.  Energy damage is applied against an opponent’s energy resistance.
        public override AosElementAttribute Element => AosElementAttribute.Energy;
        public override int SpriteW => 240;
        public override int SpriteH => 210;
    }

    public class ChaosDamageProperty : ElementalDamageAttr
    {
        public override int LabelNumber => 1151805;  // Chaos Damage
        public override int Description => 1152475;  // This property indicates that a percentage of the damage inflicted by the weapon (or spell) will be of a random type of damage (physical, fire, cold, poison or energy).
        public override AosElementAttribute Element => AosElementAttribute.Chaos;
        public override int SpriteW => 30;
        public override int SpriteH => 240;
    }

    public class VelocityProperty : ValuedProperty
    {
        public override bool IsMagical => true;
        public override Catalog Catalog => Catalog.HitEffects;
        public override int LabelNumber => 1080416;  // Velocity
        public override int Description => 1152392;  // This property provides a chance for a ranged weapon to inflict additional damage.  The additional damage is increased by 3 points for each tile that the attacker is away from the target up to a maximum of 30 points of damage.  The damage inflicted is physical damage that can be lessened by a targets physical resistance.  This property can only be found on ranged weapons.
        public override int Hue => 0x5E5D;
        public override int SpriteW => 90;
        public override int SpriteH => 120;

        public double GetPropertyValue(Item item)
        {
            return item is BaseRanged ranged ? ranged.Velocity : 0;
        }

        public override bool Matches(Item item)
        {
            Value = GetPropertyValue(item);

            if (Value != 0)
            {
                return true;
            }

            return false;
        }

        public override bool Matches(List<Item> items)
        {
            double total = 0;

            items.ForEach(x => total += GetPropertyValue(x));

            Value = total;

            if (Value != 0)
            {
                return true;
            }

            return false;
        }
    }
}
