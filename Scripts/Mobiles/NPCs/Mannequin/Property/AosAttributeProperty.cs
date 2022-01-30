using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles.MannequinProperty
{
    public abstract class MagicalAttr : ValuedProperty
    {
        public override bool IsMagical => true;
        public abstract AosAttribute Attribute { get; }

        public double GetPropertyValue(Item item)
        {
            if (item is BaseWeapon weapon)
            {
                return weapon.Attributes[Attribute];
            }

            if (item is BaseArmor armor)
            {
                return armor.Attributes[Attribute];
            }

            if (item is BaseClothing clothing)
            {
                return clothing.Attributes[Attribute];
            }

            if (item is BaseJewel jewel)
            {
                return jewel.Attributes[Attribute];
            }

            if (item is BaseTalisman talisman)
            {
                return talisman.Attributes[Attribute];
            }

            if (item is BaseQuiver quiver)
            {
                return quiver.Attributes[Attribute];
            }

            if (item is Spellbook spellbook)
            {
                return spellbook.Attributes[Attribute];
            }

            if (item is FishingPole pole)
            {
                return pole.Attributes[Attribute];
            }

            return 0;
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

    // Combat
    public class DamageIncreaseProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Combat1;
        public override int Order => 1;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1079760;  // Damage Increase
        public override int Description => 1115234;  // This property provides an increase to the damage you inflict on successful melee or ranged attacks.  This property can be found on all armor, shields, accessories or weapons.
        public override AosAttribute Attribute => AosAttribute.WeaponDamage;
        public override int Cap => 100;
        public override int Hue => 0x5E5D;
        public override int SpriteW => 60;
        public override int SpriteH => 0;
    }

    public class DefenseChanceIncreaseProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Combat1;
        public override int Order => 2;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1075620;  // Defense Chance Increase
        public override int Description => 1152403;  // This property enhances your ability to defend yourself from melee or ranged attacks.  It does not affect your chances to evade magical attacks.  Defense chance increase works in addition to your chance to parry or block an attack by using the Parrying skill.  You may bypass the 45% defense chance increase soft cap to offset any sort of defense chance increase debuff, however 45% is the cap for general combat calculations.  This property can be found on nearly all equipment.
        public override AosAttribute Attribute => AosAttribute.DefendChance;
        public override int Cap => 45;
        public override int Hue => 0x5E5D;
        public override int SpriteW => 210;
        public override int SpriteH => 30;
    }

    public class HitChanceIncreaseProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Combat1;
        public override int Order => 3;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1075616;  // Hit Chance Increase
        public override int Description => 1115232;  // This property increases your chance of successfully hitting a target.  This can be offset by your target's defense chance increase value.  This property is generally only found on weapons, shields and accessories.
        public override AosAttribute Attribute => AosAttribute.AttackChance;
        public override int Cap => 45;
        public override int Hue => 0x5E5D;
        public override int SpriteW => 210;
        public override int SpriteH => 0;
    }

    public class SwingSpeedIncreaseProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Combat1;
        public override int Order => 4;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1075629;  // Swing Speed Increase
        public override int Description => 1115233;  // This property increases the rate at which a player attacks with their weapon.  The attack speed of a weapon is calculated based on the weapon speed of the weapon, the player’s current stamina and the amount of swing speed increase a player possesses.  Swing speed can never be lower than 1.25 seconds per attack.  This property is most commonly found on weapons.
        public override AosAttribute Attribute => AosAttribute.WeaponSpeed;
        public override int Cap => 60;
        public override int Hue => 0x5E5D;
        public override int SpriteW => 150;
        public override int SpriteH => 30;
    }

    public class LowerManaCostProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Combat1;
        public override int Order => 5;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1075621;  // Lower Mana Cost
        public override int Description => 1115240;  // This property reduces the mana needed to cast all spells.  The maximum lower mana cost that can be provided from equipment is capped at 40%.  This property is most commonly found on armor and accessories.
        public override AosAttribute Attribute => AosAttribute.LowerManaCost;
        public override int Cap => 41;
        public override int Hue => 0x5E5D;
        public override int SpriteW => 60;
        public override int SpriteH => 60;
    }

    public class ReflectPhysicalDamageProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Combat1;
        public override int Order => 7;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1075626;  // Reflect Physical Damage
        public override int Description => 1152399;  // This property reflects physical damage back onto an attacker.  This property is only triggered by physical damage; any other source of damage (fire, cold, poison, energy) will not trigger the effect.  A percentage of the damage inflicted will be reflected back at the attacker when a player wearing reflect physical damage equipment is struck by any non-magical attack that inflicts physical damage.  The player wearing the reflect physical damage equipment will still receive the initial and the attacker will still be able to negate a portion of reflected damage based on their physical resistance.  This property is capped at a cumulative 250%.  This property is most commonly found on armor and shields.
        public override AosAttribute Attribute => AosAttribute.ReflectPhysical;
        public override int Cap => 250;
        public override int Hue => 0x5E5D;
        public override int SpriteW => 150;
        public override int SpriteH => 60;
    }

    public class EnhancePotionsProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Combat1;
        public override int Order => 8;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1075624;  // Enhance Potions
        public override int Description => 1152405;  // This property increases the effectiveness of potions used.  When a potion is used, its effectiveness will be increased by the percentage of your cumulative enhance potions properties.  Healing potions will heal more points of damage, curing potions will have a better chance to cure poison, etc.  There is a 50% enhance potions cap from items.  If your character has the alchemy skill, the cap is raised 10% for every 33 skill points you have.  At 99 Alchemy the cap is 80%.  Enhance potions is commonly found on jewelry.
        public override AosAttribute Attribute => AosAttribute.EnhancePotions;
        public override int Hue => 0x5E5D;
        public override int Cap => 50;
        public override int SpriteW => 240;
        public override int SpriteH => 30;
    }

    public class LuckProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Combat1;
        public override int Order => 9;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1061153;  // Luck
        public override int Description => 1152415;  // This property affects the quantity and quality of items spawned on a mobs corpse when it is killed.  A player’s luck score is cumulative from all sources and can be viewed on the character status menu.  Luck provides a chance of receiving more pieces of loot, a chance for the loot to have more magical properties and a chance for the magical properties to be of greater intensity.  It should be noted that luck has no direct effect on what specific items will spawn or what specific properties will spawn on the items.  This property can be found on all armor, shields, accessories or weapons.
        public override AosAttribute Attribute => AosAttribute.Luck;
        public override int Hue => 0x5E5D;
        public override int SpriteW => 270;
        public override int SpriteH => 0;
    }

    // Attr

    public class StrengthBonusProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Attributes;
        public override int Order => 1;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1079767;  // Strength Bonus
        public override int Description => 1152432;  // This property grants the specified bonus to your strength stat.  There is a hard cap of 150 on all stats (strength, dexterity and intelligence).  This property can be found on all armor, shields, accessories or weapons.
        public override AosAttribute Attribute => AosAttribute.BonusStr;
        public override int Hue => 0x43FF;
        public override int SpriteW => 120;
        public override int SpriteH => 30;
    }

    public class DexterityBonusProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Attributes;
        public override int Order => 2;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1079732;  // Dexterity Bonus
        public override int Description => 1152434;  // This property grants the specified bonus to your dexterity stat.  There is a hard cap of 150 on all stats (strength, dexterity and intelligence).  This property can be found on all armor, shields, accessories or weapons.
        public override AosAttribute Attribute => AosAttribute.BonusDex;
        public override int Hue => 0x43FF;
        public override int SpriteW => 90;
        public override int SpriteH => 0;
    }

    public class IntelligenceBonusProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Attributes;
        public override int Order => 3;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1079756;  // Intelligence Bonus
        public override int Description => 1152433;  // This property grants the specified bonus to your intelligence stat.  There is a hard cap of 150 on all stats (strength, dexterity and intelligence).  This property can be found on all armor, shields, accessories or weapons.
        public override AosAttribute Attribute => AosAttribute.BonusInt;
        public override int Hue => 0x43FF;
        public override int SpriteW => 240;
        public override int SpriteH => 0;
    }

    public class HitPointsIncreaseProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Attributes;
        public override int Order => 4;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1079404;  // Hit Points Increase
        public override int Description => 1152421;  // This property adds to the user’s maximum hit points.  The hit points are not instantly gained when the item is equipped; they will have to be regenerated or healed to the maximum.  The maximum possible hit points increase from any source is capped at 25.  This property is most commonly found on armor.
        public override AosAttribute Attribute => AosAttribute.BonusHits;
        public override int Hue => 0x43FF;
        public override int Cap => 25;
        public override int SpriteW => 180;
        public override int SpriteH => 0;
    }

    public class StaminaIncreaseProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Attributes;
        public override int Order => 5;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1079405;  // Stamina Increase
        public override int Description => 1152395;  // This property adds to the user’s maximum stamina.  The stamina is not instantly gained when the item is equipped; it will have to be regenerated normally to the maximum.  This property is most commonly found on armor.
        public override AosAttribute Attribute => AosAttribute.BonusStam;
        public override int Hue => 0x43FF;
        public override int Cap => 25;
        public override int SpriteW => 90;
        public override int SpriteH => 30;
    }

    public class ManaIncreaseProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Attributes;
        public override int Order => 6;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1079406;  // Mana Increase
        public override int Description => 1152431;  // This property adds to the user’s maximum mana.  The mana is not instantly gained when the item is equipped; it will have to be regenerated normally to the maximum.  This property is most commonly found on armor.
        public override AosAttribute Attribute => AosAttribute.BonusMana;
        public override int Hue => 0x43FF;
        public override int Cap => 25;
        public override int SpriteW => 0;
        public override int SpriteH => 30;
    }

    public class HitPointRegenerationProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Attributes;
        public override int Order => 7;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1075627;  // Hit Point Regeneration
        public override int Description => 1115241;  // This property increases the rate of hit point recovery.  The maximum hit point regeneration that can be provided from equipment is capped at 18.  This property is most commonly found on armor.
        public override AosAttribute Attribute => AosAttribute.RegenHits;
        public override int Cap => 18;
        public override int Hue => 0x43FF;
        public override int SpriteW => 30;
        public override int SpriteH => 60;
    }

    public class StaminaRegerationProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Attributes;
        public override int Order => 8;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1079411;  // Stamina Regeneration
        public override int Description => 1115242;  // This property increases the rate of stamina recovery.  The maximum stamina regeneration that can be provided from equipment is capped at 24.  This property is most commonly found on armor.
        public override AosAttribute Attribute => AosAttribute.RegenStam;
        public override int Cap => 24;
        public override int Hue => 0x43FF;
        public override int SpriteW => 210;
        public override int SpriteH => 60;
    }

    public class ManaRegenerationProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Attributes;
        public override int Order => 9;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1079410;  // Mana Regeneration
        public override int Description => 1115243;  // This property increases the rate of mana recovery.  Mana regeneration is subject to diminishing returns.  The maximum mana regeneration that can be provided from any source is a capped at 30.  This property is most commonly found on armor.
        public override AosAttribute Attribute => AosAttribute.RegenMana;
        public override int Cap => 30;
        public override int Hue => 0x43FF;
        public override int SpriteW => 120;
        public override int SpriteH => 60;
    }

    public class SpellDamageIncreaseProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Casting;
        public override int Order => 4;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1075628;  // Spell Damage Increase
        public override int Description => 1152397;  // This property increases the damage for most magical attacks that are casted.  While the property is normally capped at 15% in PvP, the cap can be further increased to 30% for focused templates.  Focus templates have no more than 30 modified skill points in more than one primary skill such as magery, necromancy, mysticism, ninjitsu, bushido, animal taming, musicianship, chivalry or spellweaving.  Also no more than 30 modified skill points in the associated secondary skill such as evaluate intelligence, spirit speaking, focus, imbuing.   There is no cap to spell damage increase in PvM.  This property is most commonly found on accessories and spellbooks.
        public override AosAttribute Attribute => AosAttribute.SpellDamage;
        public override int Hue => 0x1FF;
        public override int SpriteW => 180;
        public override int SpriteH => 60;
    }

    public class FasterCastRecoveryProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Casting;
        public override int Order => 2;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1075618;  // Faster Cast Recovery
        public override int Description => 1152406;  // This property reduces the recovery time after casting a spell.  When a spell is cast it can take a short period of time before another can be started.  This period of recovery can be reduced with faster cast recovery.  Casting recovery affects all spell casting and the property is capped at a cumulative total of 6 cast recovery.  This property is generally only found on accessories and spellbooks in values ranging from 1 to 3.
        public override AosAttribute Attribute => AosAttribute.CastRecovery;
        public override int Hue => 0x1FF;
        public override int Cap => 6;
        public override int SpriteW => 0;
        public override int SpriteH => 60;
    }

    public class FasterCastingProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Casting;
        public override int Order => 1;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1075617;  // Faster Casting
        public override int Description => 1152407;  // This property reduces the amount time it takes to cast a spell.  This property is capped based on the source of the spell being cast.  Mages, Mystics and Necromancers have a maximum faster casting of 2 while Chivalry, Ninjitsu, Bushido or Spellweaving can have a maximum Faster Casting of 4.  If a character possesses Magery, Mysticism or Necromancy at 70.0 skill or greater then faster casting is capped at 2 for Chivalry.  Casting time is shortened by 0.25 seconds for every point in faster casting.  This property is generally only found on weapons, shields and accessories.
        public override AosAttribute Attribute => AosAttribute.CastSpeed;
        public override int Hue => 0x1FF;
        public override int Cap => 4;
        public override int SpriteW => 270;
        public override int SpriteH => 30;
    }

    public class LowerReagentCostProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Casting;
        public override int Order => 3;
        public override bool AlwaysVisible => true;
        public override int LabelNumber => 1075625;  // Lower Reagent Cost
        public override int Description => 1115235;  // This property increases the chance to not consume reagents or tithing points when casting spells.  If the cumulative amount of this property is equal or above 100% then there is no reagents or tithing points cost to casting spells.  This property is most commonly found on armor and accessories.
        public override AosAttribute Attribute => AosAttribute.LowerRegCost;
        public override int Cap => 100;
        public override int Hue => 0x1FF;
        public override int SpriteW => 90;
        public override int SpriteH => 60;
    }

    public class SpellChannelingCostProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Casting;
        public override bool IsBoolen => true;
        public override int LabelNumber => 1079766;  // Spell Channeling
        public override int Description => 1152398;  // This property allows all spell casting to occur without the equipped item being unequipped.  It is always accompanied by a faster casting penalty of -1 which does not count as an additional property.  This faster casting penalty can be negated if the item also has a positive faster casting property as well.  This property is only found on weapons and shields.
        public override AosAttribute Attribute => AosAttribute.SpellChanneling;
        public override int Hue => 0x1FF;
        public override int SpriteW => 210;
        public override int SpriteH => 120;
    }

    public class NightSightProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Misc;
        public override bool IsBoolen => true;
        public override int LabelNumber => 1075643;  // Night Sight
        public override int Description => 1152416;  // This property allows the user to ignore the effects of darkness from either night or from being underground in a dungeon.  This property can be found on all armor, shields, accessories or weapons.
        public override AosAttribute Attribute => AosAttribute.NightSight;
        public override int Hue => 0x43FF;
        public override int SpriteW => 30;
        public override int SpriteH => 150;
    }

    public class BrittleProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.None;
        public override int LabelNumber => 1116209;  // Brittle
        public override bool IsSpriteGraph => true;
        public override int SpriteW => 90;
        public override int SpriteH => 240;
        public override AosAttribute Attribute => AosAttribute.Brittle;
    }

    public class LowerAmmoCostProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Misc;
        public override int LabelNumber => 1159315;  // Lower Ammo Cost
        public override int Description => 1152459;  // This property provides a reduction in the amount of arrows or bolts an archer uses.  The reduction is based on a random chance that an arrow or bolt shot from a weapon will not be consumed.
        public override AosAttribute Attribute => AosAttribute.LowerAmmoCost;
        public override int Hue => 0x43FF;
        public override int SpriteW => 150;
        public override int SpriteH => 270;
    }

    public class BalancedWeaponProperty : MagicalAttr
    {
        public override Catalog Catalog => Catalog.Combat1;
        public override bool IsBoolen => true;
        public override int LabelNumber => 1072792;  // Balanced
        public override int Description => 1152384;  // This property is only available on archery weapons.  This property allows a player to use a two handed weapon as if it were a one handed weapon.  You can perform any action that normally requires a free hand, such as drinking a potion or throwing a shuriken.  
        public override AosAttribute Attribute => AosAttribute.BalancedWeapon;
        public override int Hue => 0x5E5D;
        public override int SpriteW => 180;
        public override int SpriteH => 120;
    }
}
