using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles.MannequinProperty
{
    public abstract class AbsorptionAttr : ValuedProperty
    {
        public abstract SAAbsorptionAttribute Attribute { get; }

        public double GetPropertyValue(Item item)
        {
            if (item is BaseArmor)
            {
                return ((BaseArmor)item).AbsorptionAttributes[Attribute];
            }

            if (item is BaseJewel)
            {
                return ((BaseJewel)item).AbsorptionAttributes[Attribute];
            }


            if (item is BaseWeapon)
            {
                return ((BaseWeapon)item).AbsorptionAttributes[Attribute];
            }

            if (item is BaseClothing)
            {
                return ((BaseClothing)item).SAAbsorptionAttributes[Attribute];
            }

            return 0;
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

    public class DamageEaterProperty : AbsorptionAttr
    {
        public override Catalog Catalog { get { return Catalog.Resistances; } }
        public override int Order { get { return 6; } }
        public override bool AlwaysVisible { get { return true; } }
        public override int LabelNumber { get { return 1154667; } } // Damage Eater
        public override int Description { get { return 1152390; } } // This property converts a small portion of damage dealt to a player back as health.  The damage inflicted must be the same type of damage as the eater for the property to function.  Similar eater properties stack with other but are capped at 30%.  The "damage all" type of eater is capped at 18%.  The property stores up to 20 healing charges and converts charges every three second from the last time damage was received.  The "damage all" type of damage eater does not stack with specific eaters.  This property is generally only found on armor and shields.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.EaterDamage; } }
        public override int Cap { get { return 30; } }
        public override int Hue { get { return 0x42FF; } }
        public override int SpriteW { get { return 270; } }
        public override int SpriteH { get { return 60; } }
    }

    public class KineticEaterProperty : AbsorptionAttr
    {
        public override Catalog Catalog { get { return Catalog.Resistances; } }
        public override int Order { get { return 7; } }
        public override bool AlwaysVisible { get { return true; } }
        public override int LabelNumber { get { return 1154666; } } // Kinetic Eater
        public override int Description { get { return 1152390; } } // This property converts a small portion of damage dealt to a player back as health.  The damage inflicted must be the same type of damage as the eater for the property to function.  Similar eater properties stack with other but are capped at 30%.  The "damage all" type of eater is capped at 18%.  The property stores up to 20 healing charges and converts charges every three second from the last time damage was received.  The "damage all" type of damage eater does not stack with specific eaters.  This property is generally only found on armor and shields.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.EaterKinetic; } }
        public override int Cap { get { return 30; } }
        public override int Hue { get { return 0x42FF; } }
        public override int SpriteW { get { return 0; } }
        public override int SpriteH { get { return 90; } }
    }

    public class FireEaterProperty : AbsorptionAttr
    {
        public override Catalog Catalog { get { return Catalog.Resistances; } }
        public override int Order { get { return 8; } }
        public override bool AlwaysVisible { get { return true; } }
        public override int LabelNumber { get { return 1154662; } } // Fire Eater
        public override int Description { get { return 1152390; } } // This property converts a small portion of damage dealt to a player back as health.  The damage inflicted must be the same type of damage as the eater for the property to function.  Similar eater properties stack with other but are capped at 30%.  The "damage all" type of eater is capped at 18%.  The property stores up to 20 healing charges and converts charges every three second from the last time damage was received.  The "damage all" type of damage eater does not stack with specific eaters.  This property is generally only found on armor and shields.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.EaterFire; } }
        public override int Cap { get { return 30; } }
        public override int Hue { get { return 0x42FF; } }
        public override int SpriteW { get { return 30; } }
        public override int SpriteH { get { return 90; } }
    }

    public class ColdEaterProperty : AbsorptionAttr
    {
        public override Catalog Catalog { get { return Catalog.Resistances; } }
        public override int Order { get { return 9; } }
        public override bool AlwaysVisible { get { return true; } }
        public override int LabelNumber { get { return 1154663; } } // Cold Eater
        public override int Description { get { return 1152390; } } // This property converts a small portion of damage dealt to a player back as health.  The damage inflicted must be the same type of damage as the eater for the property to function.  Similar eater properties stack with other but are capped at 30%.  The "damage all" type of eater is capped at 18%.  The property stores up to 20 healing charges and converts charges every three second from the last time damage was received.  The "damage all" type of damage eater does not stack with specific eaters.  This property is generally only found on armor and shields.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.EaterCold; } }
        public override int Cap { get { return 30; } }
        public override int Hue { get { return 0x42FF; } }
        public override int SpriteW { get { return 60; } }
        public override int SpriteH { get { return 90; } }
    }

    public class PoisonEaterProperty : AbsorptionAttr
    {
        public override Catalog Catalog { get { return Catalog.Resistances; } }
        public override int Order { get { return 10; } }
        public override bool AlwaysVisible { get { return true; } }
        public override int LabelNumber { get { return 1154664; } } // Poison Eater
        public override int Description { get { return 1152390; } } // This property converts a small portion of damage dealt to a player back as health.  The damage inflicted must be the same type of damage as the eater for the property to function.  Similar eater properties stack with other but are capped at 30%.  The "damage all" type of eater is capped at 18%.  The property stores up to 20 healing charges and converts charges every three second from the last time damage was received.  The "damage all" type of damage eater does not stack with specific eaters.  This property is generally only found on armor and shields.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.EaterPoison; } }
        public override int Cap { get { return 30; } }
        public override int Hue { get { return 0x42FF; } }
        public override int SpriteW { get { return 90; } }
        public override int SpriteH { get { return 90; } }
    }

    public class EnergyEaterProperty : AbsorptionAttr
    {
        public override Catalog Catalog { get { return Catalog.Resistances; } }
        public override int Order { get { return 11; } }
        public override bool AlwaysVisible { get { return true; } }
        public override int LabelNumber { get { return 1154665; } } // Energy Eater
        public override int Description { get { return 1152390; } } // This property converts a small portion of damage dealt to a player back as health.  The damage inflicted must be the same type of damage as the eater for the property to function.  Similar eater properties stack with other but are capped at 30%.  The "damage all" type of eater is capped at 18%.  The property stores up to 20 healing charges and converts charges every three second from the last time damage was received.  The "damage all" type of damage eater does not stack with specific eaters.  This property is generally only found on armor and shields.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.EaterEnergy; } }
        public override int Cap { get { return 30; } }
        public override int Hue { get { return 0x42FF; } }
        public override int SpriteW { get { return 120; } }
        public override int SpriteH { get { return 90; } }
    }    

    public class FireResonanceProperty : AbsorptionAttr
    {
        public override Catalog Catalog { get { return Catalog.Casting; } }
        public override int LabelNumber { get { return 1154655; } } // Fire Resonance
        public override int Description { get { return 1152391; } } // This property provides a chance to resist interruptions while casting spells if the damage received is the same damage type as the resonance.  This property can only be found on two handed weapons and shields.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.ResonanceFire; } }
        public override int Cap { get { return 40; } }
        public override int Hue { get { return 0x1FF; } }
        public override int SpriteW { get { return 180; } }
        public override int SpriteH { get { return 90; } }
    }

    public class ColdResonanceProperty : AbsorptionAttr
    {
        public override Catalog Catalog { get { return Catalog.Casting; } }
        public override int LabelNumber { get { return 1154656; } } // Cold Resonance
        public override int Description { get { return 1152391; } } // This property provides a chance to resist interruptions while casting spells if the damage received is the same damage type as the resonance.  This property can only be found on two handed weapons and shields.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.ResonanceCold; } }
        public override int Cap { get { return 40; } }
        public override int Hue { get { return 0x1FF; } }
        public override int SpriteW { get { return 210; } }
        public override int SpriteH { get { return 90; } }
    }

    public class PoisonResonanceProperty : AbsorptionAttr
    {
        public override Catalog Catalog { get { return Catalog.Casting; } }
        public override int LabelNumber { get { return 1154657; } } // Poison Resonance
        public override int Description { get { return 1152391; } } // This property provides a chance to resist interruptions while casting spells if the damage received is the same damage type as the resonance.  This property can only be found on two handed weapons and shields.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.ResonancePoison; } }
        public override int Cap { get { return 40; } }
        public override int Hue { get { return 0x1FF; } }
        public override int SpriteW { get { return 240; } }
        public override int SpriteH { get { return 90; } }
    }

    public class EnergyResonanceProperty : AbsorptionAttr
    {
        public override Catalog Catalog { get { return Catalog.Casting; } }
        public override int LabelNumber { get { return 1154658; } } // Energy Resonance
        public override int Description { get { return 1152391; } } // This property provides a chance to resist interruptions while casting spells if the damage received is the same damage type as the resonance.  This property can only be found on two handed weapons and shields.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.ResonanceEnergy; } }
        public override int Cap { get { return 40; } }
        public override int Hue { get { return 0x1FF; } }
        public override int SpriteW { get { return 270; } }
        public override int SpriteH { get { return 90; } }
    }

    public class KineticResonanceProperty : AbsorptionAttr
    {
        public override Catalog Catalog { get { return Catalog.Casting; } }
        public override int LabelNumber { get { return 1154659; } } // Kinetic Resonance
        public override int Description { get { return 1152391; } } // This property provides a chance to resist interruptions while casting spells if the damage received is the same damage type as the resonance.  This property can only be found on two handed weapons and shields.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.ResonanceKinetic; } }
        public override int Cap { get { return 40; } }
        public override int Hue { get { return 0x1FF; } }
        public override int SpriteW { get { return 150; } }
        public override int SpriteH { get { return 90; } }
    }

    public class CastingFocusProperty : AbsorptionAttr
    {
        // Only Mobile View
        public override Catalog Catalog { get { return Catalog.Casting; } }
        public override int LabelNumber { get { return 1116535; } } // Casting Focus
        public override int Description { get { return 1152389; } } // This property provides a chance to resist any interruptions while casting spells.  It has a cumulative cap of 12%.  The inscription skill can also grant up to a 5% additional bonus which can exceed the item cap.
        public override SAAbsorptionAttribute Attribute { get { return SAAbsorptionAttribute.CastingFocus; } }
        public override int Cap { get { return 12; } }
        public override int Hue { get { return 0x1FF; } }
        public override int SpriteW { get { return 60; } }
        public override int SpriteH { get { return 210; } }
    }
}
