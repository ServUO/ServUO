using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles.MannequinProperty
{
    public abstract class ResistAttr : ValuedProperty
    {
        public override Catalog Catalog => Catalog.Resistances;
        public override bool AlwaysVisible => true;
        public abstract ResistanceType Resist { get; }
        public override int Hue => 0x42FF;
        public override int Cap => 70;

        public double GetPropertyValue(Item item)
        {
            return GetBaseResistBonus(item, Resist);
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

        public int GetBaseResistBonus(Item item, ResistanceType resist)
        {
            switch (resist)
            {
                case ResistanceType.Physical:
                    {
                        if (item is BaseWeapon)
                            return ((BaseWeapon)item).PhysicalResistance;

                        if (item is BaseArmor)
                            return ((BaseArmor)item).PhysicalResistance;

                        if (item is BaseClothing)
                            return ((BaseClothing)item).PhysicalResistance;

                        break;
                    }
                case ResistanceType.Fire:
                    {
                        if (item is BaseWeapon)
                            return ((BaseWeapon)item).FireResistance;

                        if (item is BaseArmor)
                            return ((BaseArmor)item).FireResistance;

                        if (item is BaseClothing)
                            return ((BaseClothing)item).FireResistance;

                        break;
                    }
                case ResistanceType.Cold:
                    {
                        if (item is BaseWeapon)
                            return ((BaseWeapon)item).ColdResistance;

                        if (item is BaseArmor)
                            return ((BaseArmor)item).ColdResistance;

                        if (item is BaseClothing)
                            return ((BaseClothing)item).ColdResistance;

                        break;
                    }
                case ResistanceType.Poison:
                    {
                        if (item is BaseWeapon)
                            return ((BaseWeapon)item).PoisonResistance;

                        if (item is BaseArmor)
                            return ((BaseArmor)item).PoisonResistance;

                        if (item is BaseClothing)
                            return ((BaseClothing)item).PoisonResistance;

                        break;
                    }
                case ResistanceType.Energy:
                    {
                        if (item is BaseWeapon)
                            return ((BaseWeapon)item).EnergyResistance;

                        if (item is BaseArmor)
                            return ((BaseArmor)item).EnergyResistance;

                        if (item is BaseClothing)
                            return ((BaseClothing)item).EnergyResistance;

                        break;
                    }
            }

            return 0;
        }
    }

    public class PhysicalResistProperty : ResistAttr
    {
        public override int Order => 1;
        public override int LabelNumber => 1079764;  // Physical Resist
        public override ResistanceType Resist => ResistanceType.Physical;
        public override int Description => 1115226;  // This property reduces the damage taken from attacks that cause physical damage.  This property can be found on all armor, shields, accessories or weapons.
        public override int SpriteW => 30;
        public override int SpriteH => 30;
    }

    public class FireResistProperty : ResistAttr
    {
        public override int Order => 2;
        public override int LabelNumber => 1079763;  // Fire Resist
        public override ResistanceType Resist => ResistanceType.Fire;
        public override int Description => 1115227;  // This property reduces the damage taken from attacks that cause fire damage.  This property can be found on all armor, shields, accessories or weapons.
        public override int SpriteW => 150;
        public override int SpriteH => 0;
    }

    public class ColdResistProperty : ResistAttr
    {
        public override int Order => 3;
        public override int LabelNumber => 1079761;  // Cold Resist
        public override ResistanceType Resist => ResistanceType.Cold;
        public override int Description => 1115228;  // This property reduces the damage taken from attacks that cause cold damage.  This property can be found on all armor, shields, accessories or weapons.
        public override int SpriteW => 0;
        public override int SpriteH => 0;
    }

    public class PoisonResistProperty : ResistAttr
    {
        public override int Order => 4;
        public override int LabelNumber => 1079765;  // Poison Resist
        public override ResistanceType Resist => ResistanceType.Poison;
        public override int Description => 1115229;  // This property reduces the damage taken from attacks that cause poison damage.  This property can be found on all armor, shields, accessories or weapons.
        public override int SpriteW => 60;
        public override int SpriteH => 30;
    }

    public class EnergyResistProperty : ResistAttr
    {
        public override int Order => 5;
        public override int LabelNumber => 1079762;  // Energy Resist
        public override ResistanceType Resist => ResistanceType.Energy;
        public override int Description => 1115230;  // This property reduces the damage taken from attacks that cause energy damage.  This property can be found on all armor, shields, accessories or weapons.
        public override int SpriteW => 120;
        public override int SpriteH => 0;
    }
}
