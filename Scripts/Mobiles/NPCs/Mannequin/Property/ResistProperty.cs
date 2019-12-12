using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles.MannequinProperty
{
    public abstract class ResistAttr : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.Resistances; } }        
        public override bool AlwaysVisible { get { return true; } }
        public abstract ResistanceType Resist { get; }
        public override int Hue { get { return 0x42FF; } }
        public override int Cap { get { return 70; } }

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
        public override int Order { get { return 1; } }
        public override int LabelNumber { get { return 1079764; } } // Physical Resist
        public override ResistanceType Resist { get { return ResistanceType.Physical; } }
        public override int Description { get { return 1115226; } } // This property reduces the damage taken from attacks that cause physical damage.  This property can be found on all armor, shields, accessories or weapons.
        public override int SpriteW { get { return 30; } }
        public override int SpriteH { get { return 30; } }
    }

    public class FireResistProperty : ResistAttr
    {
        public override int Order { get { return 2; } }
        public override int LabelNumber { get { return 1079763; } } // Fire Resist
        public override ResistanceType Resist { get { return ResistanceType.Fire; } }
        public override int Description { get { return 1115227; } } // This property reduces the damage taken from attacks that cause fire damage.  This property can be found on all armor, shields, accessories or weapons.
        public override int SpriteW { get { return 150; } }
        public override int SpriteH { get { return 0; } }
    }

    public class ColdResistProperty : ResistAttr
    {
        public override int Order { get { return 3; } }
        public override int LabelNumber { get { return 1079761; } } // Cold Resist
        public override ResistanceType Resist { get { return ResistanceType.Cold; } }
        public override int Description { get { return 1115228; } } // This property reduces the damage taken from attacks that cause cold damage.  This property can be found on all armor, shields, accessories or weapons.
        public override int SpriteW { get { return 0; } }
        public override int SpriteH { get { return 0; } }
    }

    public class PoisonResistProperty : ResistAttr
    {
        public override int Order { get { return 4; } }
        public override int LabelNumber { get { return 1079765; } } // Poison Resist
        public override ResistanceType Resist { get { return ResistanceType.Poison; } }
        public override int Description { get { return 1115229; } } // This property reduces the damage taken from attacks that cause poison damage.  This property can be found on all armor, shields, accessories or weapons.
        public override int SpriteW { get { return 60; } }
        public override int SpriteH { get { return 30; } }
    }

    public class EnergyResistProperty : ResistAttr
    {
        public override int Order { get { return 5; } }
        public override int LabelNumber { get { return 1079762; } } // Energy Resist
        public override ResistanceType Resist { get { return ResistanceType.Energy; } }
        public override int Description { get { return 1115230; } } // This property reduces the damage taken from attacks that cause energy damage.  This property can be found on all armor, shields, accessories or weapons.
        public override int SpriteW { get { return 120; } }
        public override int SpriteH { get { return 0; } }
    }
}
