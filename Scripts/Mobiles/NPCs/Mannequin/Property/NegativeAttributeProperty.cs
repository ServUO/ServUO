using System;
using Server;
using Server.Items;

namespace Server.Mobiles.MannequinProperty
{
    public abstract class NegativeAttr : ValuedProperty
    {
        public abstract NegativeAttribute Attribute { get; }

        public double GetPropertyValue(Item item)
        {
            return item is BaseWeapon ? ((BaseWeapon)item).NegativeAttributes[Attribute] : 0;
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
    }

    public class PrizedProperty : NegativeAttr
    {
        public override Catalog Catalog { get { return Catalog.None; } }
        public override int LabelNumber { get { return 1154910; } } // Prized
        public override bool IsSpriteGraph { get { return true; } }
        public override int SpriteW { get { return 30; } }
        public override int SpriteH { get { return 210; } }
        public override NegativeAttribute Attribute { get { return NegativeAttribute.Prized; } }
    }

    public class AntiqueProperty : NegativeAttr
    {
        public override Catalog Catalog { get { return Catalog.None; } }
        public override int LabelNumber { get { return 1152714; } } // Antique
        public override bool IsSpriteGraph { get { return true; } }
        public override int SpriteW { get { return 60; } }
        public override int SpriteH { get { return 240; } }
        public override NegativeAttribute Attribute { get { return NegativeAttribute.Antique; } }
    }

    public class NegativeBrittleProperty : NegativeAttr
    {
        public override Catalog Catalog { get { return Catalog.None; } }
        public override int LabelNumber { get { return 1116209; } } // Brittle
        public override bool IsSpriteGraph { get { return true; } }
        public override int SpriteW { get { return 90; } }
        public override int SpriteH { get { return 240; } }
        public override NegativeAttribute Attribute { get { return NegativeAttribute.Brittle; } }
    }

    public class NotCannotBeRepairedProperty : NegativeAttr
    {
        public override Catalog Catalog { get { return Catalog.None; } }
        public override int LabelNumber { get { return 1151782; } } // cannot be repaired
        public override bool IsSpriteGraph { get { return true; } }
        public override int SpriteW { get { return 120; } }
        public override int SpriteH { get { return 240; } }
        public override NegativeAttribute Attribute { get { return NegativeAttribute.NoRepair; } }
    }
}
