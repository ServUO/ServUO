using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles.MannequinProperty
{
    public abstract class AosArmorAttr : ValuedProperty
    {
        public abstract AosArmorAttribute Attribute { get; }

        public double GetPropertyValue(Item item)
        {
            if (item is BaseArmor)
            {
                return ((BaseArmor)item).ArmorAttributes[Attribute];
            }

            if (item is BaseClothing)
            {
                return ((BaseClothing)item).ClothingAttributes[Attribute];
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

    public class ArmorSelfRepairProperty : AosArmorAttr
    {
        public override Catalog Catalog { get { return Catalog.None; } }
        public override int LabelNumber { get { return 1079709; } } // Self Repair
        public override bool IsSpriteGraph { get { return true; } }
        public override int SpriteW { get { return 0; } }
        public override int SpriteH { get { return 300; } }        
        public override AosArmorAttribute Attribute { get { return AosArmorAttribute.SelfRepair; } }
    }

    public class ArmorReactiveParalyzeProperty : AosArmorAttr
    {
        public override Catalog Catalog { get { return Catalog.Combat1; } }
        public override bool IsBoolen { get { return true; } }
        public override int LabelNumber { get { return 1154660; } } // Reactive Paralyze
        public override int Description { get { return 1152400; } } // This property provides a chance to cast a paralyze spell on an opponent.  The paralyze effect is similar to the magery fifth circle spell paralyze.  When the wielder effectively parries an attacker’s blow there is a chance that the attacker will be affected by the paralyze effect.   This property can be found on two handed weapons and shields.
        public override AosArmorAttribute Attribute { get { return AosArmorAttribute.ReactiveParalyze; } }
        public override int Hue { get { return 0x5E5D; } }
        public override int SpriteW { get { return 270; } }
        public override int SpriteH { get { return 120; } }
    }

    public class SoulChargeProperty : AosArmorAttr
    {
        public override Catalog Catalog { get { return Catalog.Casting; } }
        public override int LabelNumber { get { return 1116536; } } // Soul Charge
        public override int Description { get { return 1152382; } } // This property converts a percentage of the damage inflicted to a player into mana replenishment.  This effect can only be triggered once every 40 seconds.  This property is only found on shields.
        public override AosArmorAttribute Attribute { get { return AosArmorAttribute.SoulCharge; } }
        public override int Hue { get { return 0x1FF; } }
        public override int SpriteW { get { return 240; } }
        public override int SpriteH { get { return 60; } }
    }
}
