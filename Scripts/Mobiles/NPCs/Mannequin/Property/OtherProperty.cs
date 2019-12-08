using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles.MannequinProperty
{
    public class DurabilityProperty : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.None; } }
        public override int LabelNumber { get { return 1017323; } } // Durability
        public override int Description { get { return 1152404; } } // This property indicates how in need of repairs an item is or how close an item is to breaking.  It is comprised of two parts: the current durability and the maximum durability.  Both of which are values in the range of 0 - 255.  If both of these values are allowed to reach 0 on an item, the item is destroyed.  This property can be found on all armor, shields, accessories or weapons.
        public override int Hue { get { return 0x1F0; } }

        public override bool Matches(Item item)
        {
            int prop, prop2;

            if (item is BaseArmor)
            {
                if ((prop = ((BaseArmor)item).HitPoints) >= 0 && (prop2 = ((BaseArmor)item).MaxHitPoints) > 0)
                {
                    Value = prop;
                    Cap = prop2;
                    return true;
                }
            }

            if (item is BaseJewel)
            {
                if ((prop = ((BaseJewel)item).HitPoints) >= 0 && (prop2 = ((BaseJewel)item).MaxHitPoints) > 0)
                {
                    Value = prop;
                    Cap = prop2;
                    return true;
                }
            }


            if (item is BaseWeapon)
            {
                if ((prop = ((BaseWeapon)item).HitPoints) >= 0 && (prop2 = ((BaseWeapon)item).MaxHitPoints) > 0)
                {
                    Value = prop;
                    Cap = prop2;
                    return true;
                }
            }

            if (item is BaseClothing)
            {
                if ((prop = ((BaseClothing)item).HitPoints) >= 0 && (prop2 = ((BaseClothing)item).MaxHitPoints) > 0)
                {
                    Value = prop;
                    Cap = prop2;
                    return true;
                }
            }

            if (item is BaseTalisman)
            {
                if ((prop = ((BaseTalisman)item).HitPoints) >= 0 && (prop2 = ((BaseTalisman)item).MaxHitPoints) > 0)
                {
                    Value = prop;
                    Cap = prop2;
                    return true;
                }
            }

            if (item is Spellbook)
            {
                if ((prop = ((Spellbook)item).HitPoints) >= 0 && (prop2 = ((Spellbook)item).MaxHitPoints) > 0)
                {
                    Value = prop;
                    Cap = prop2;
                    return true;
                }
            }

            return false;
        }
    }

    public class BlessedProperty : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.None; } }
        public override int LabelNumber { get { return 1038021; } } // Blessed
        public override bool IsSpriteGraph { get { return true; } }
        public override int SpriteW { get { return 150; } }
        public override int SpriteH { get { return 210; } }

        public override bool Matches(Item item)
        {
            return item.LootType == LootType.Blessed;
        }
    }

    public class CursedProperty : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.None; } }
        public override int LabelNumber { get { return 1049643; } } // cursed
        public override bool IsSpriteGraph { get { return true; } }
        public override int SpriteW { get { return 150; } }
        public override int SpriteH { get { return 240; } }

        public override bool Matches(Item item)
        {
            return item.LootType == LootType.Cursed;
        }
    }

    public class MedableArmorProperty : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.Combat1; } }
        public override int Order { get { return 6; } }
        public override bool AlwaysVisible { get { return true; } }
        public override bool IsBoolen { get { return true; } }
        public override bool BoolenValue { get { return true; } }
        public override int LabelNumber { get { return 1159280; } } // Medable Armor
        public override int SpriteW { get { return 0; } }
        public override int SpriteH { get { return 150; } }

        public double GetPropertyValue(Item item)
        {
            return item is BaseArmor ? Misc.RegenRates.GetArmorMeditationValue((BaseArmor)item) : 0;
        }

        public override bool Matches(Item item)
        {
            double total = GetPropertyValue(item);

            if (total != 0)
                return true;

            return false;
        }

        public override bool Matches(List<Item> items)
        {
            double total = 0;

            items.ForEach(x => total += GetPropertyValue(x));

            if (total != 0)
            {
                return true;
            }

            return false;
        }
    }

    public class GargoyleProperty : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.None; } }
        public override int LabelNumber { get { return 1111709; } } // Gargoyles Only
        public override bool IsSpriteGraph { get { return true; } }
        public override int SpriteW { get { return 30; } }
        public override int SpriteH { get { return 270; } }

        public override bool Matches(Item item)
        {
            if (item is BaseArmor && ((BaseArmor)item).CanBeWornByGargoyles)
                return true;

            if (item is BaseJewel && ((BaseJewel)item).CanBeWornByGargoyles)
                return true;

            if (item is BaseWeapon && ((BaseWeapon)item).CanBeWornByGargoyles)
                return true;

            if (item is BaseClothing && ((BaseClothing)item).CanBeWornByGargoyles)
                return true;

            return false;
        }
    }

    public class ElfProperty : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.None; } }
        public override int LabelNumber { get { return 1075086; } } // Elves Only
        public override bool IsSpriteGraph { get { return true; } }
        public override int SpriteW { get { return 210; } }
        public override int SpriteH { get { return 240; } }

        public override bool Matches(Item item)
        {
            if (item is BaseArmor && ((BaseArmor)item).RequiredRace == Race.Elf)
                return true;

            if (item is BaseJewel && ((BaseJewel)item).RequiredRace == Race.Elf)
                return true;

            if (item is BaseWeapon && ((BaseWeapon)item).RequiredRace == Race.Elf)
                return true;

            if (item is BaseClothing && ((BaseClothing)item).RequiredRace == Race.Elf)
                return true;

            return false;
        }
    }

    public class DamageModifierProperty : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.Misc; } }
        public override int LabelNumber { get { return 1159401; } } // Damage Modifier
        public override int Description { get { return 1152402; } } // This property provides an increase to the damage you inflict on a successful ranged weapon attack.  The bonus damage is applied to the net damage inflicted (after all damage and resistance calculations are made).  This property can only be found on certain quivers.
        public override int Hue { get { return 0x43FF; } }
        public override int SpriteW { get { return 180; } }
        public override int SpriteH { get { return 270; } }

        public double GetPropertyValue(Item item)
        {
            return item is BaseQuiver ? ((BaseQuiver)item).DamageIncrease : 0;
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

    public class ManaPhaseProperty : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.Combat1; } }
        public override int Order { get { return 10; } }
        public override bool IsBoolen { get { return true; } }
        public override int LabelNumber { get { return 1116158; } } // Mana Phase
        public override int Description { get { return 1152417; } } // This property reduces mana cost to 0.  Mana phasing is a property that consumes charges.  <br>When the effect is activated the next two mana checks are made at a cost of zero mana but one charge is consumed.  The first mana check occurs when the mana consuming activity (starting to cast a spell, triggering a special move, etc.) is started and the second mana check occurs when the mana consuming activity is finalized (final targeting with the spell, the special move occurring, etc.).  There is a 30 second cool down between uses.  This property is currently only available on specific talisman.
        public override int SpriteW { get { return 240; } }
        public override int SpriteH { get { return 270; } }

        public bool GetPropertyValue(Item item)
        {
            return item is ManaPhasingOrb;
        }

        public override bool Matches(Item item)
        {
            return GetPropertyValue(item);
        }

        public override bool Matches(List<Item> items)
        {
            foreach (Item i in items)
            {
                if (GetPropertyValue(i))
                    return true;
            }

            return false;
        }
    }

    public class SearingWeaponProperty : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.HitEffects; } }
        public override bool IsBoolen { get { return true; } }
        public override int LabelNumber { get { return 1151183; } } // Searing Weapon
        public override int Description { get { return 1152471; } } // This property provides a 20% chance (10% for ranged weapons) to deal additional fire damage to a target while inflicting 4 points of direct damage to the wielder.  This property also induces a hit point regeneration penalty on the target that lasts for four seconds.  The penalty is -20 hit point regeneration for players and -60 hit point regeneration for monsters and NPCs.  Each attack with a weapon with this property consumes one mana.  This property is only found on weapons.
        public override int SpriteW { get { return 270; } }
        public override int SpriteH { get { return 270; } }

        public bool GetPropertyValue(Item item)
        {
            return item is BaseWeapon && ((BaseWeapon)item).SearingWeapon;
        }

        public override bool Matches(Item item)
        {
            return GetPropertyValue(item);
        }

        public override bool Matches(List<Item> items)
        {
            foreach (Item i in items)
            {
                if (GetPropertyValue(i))
                    return true;
            }

            return false;
        }
    }
}
