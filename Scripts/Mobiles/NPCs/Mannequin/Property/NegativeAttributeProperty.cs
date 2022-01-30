using Server.Items;

namespace Server.Mobiles.MannequinProperty
{
    public abstract class NegativeAttr : ValuedProperty
    {
        public override bool IsMagical => true;
        public abstract NegativeAttribute Attribute { get; }

        public double GetPropertyValue(Item item)
        {
            return item is BaseWeapon weapon ? weapon.NegativeAttributes[Attribute] : 0;
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
        public override Catalog Catalog => Catalog.None;
        public override int LabelNumber => 1154910;  // Prized
        public override bool IsSpriteGraph => true;
        public override int SpriteW => 30;
        public override int SpriteH => 210;
        public override NegativeAttribute Attribute => NegativeAttribute.Prized;
    }

    public class AntiqueProperty : NegativeAttr
    {
        public override Catalog Catalog => Catalog.None;
        public override int LabelNumber => 1152714;  // Antique
        public override bool IsSpriteGraph => true;
        public override int SpriteW => 60;
        public override int SpriteH => 240;
        public override NegativeAttribute Attribute => NegativeAttribute.Antique;
    }

    public class NegativeBrittleProperty : NegativeAttr
    {
        public override Catalog Catalog => Catalog.None;
        public override int LabelNumber => 1116209;  // Brittle
        public override bool IsSpriteGraph => true;
        public override int SpriteW => 90;
        public override int SpriteH => 240;
        public override NegativeAttribute Attribute => NegativeAttribute.Brittle;
    }

    public class NotCannotBeRepairedProperty : NegativeAttr
    {
        public override Catalog Catalog => Catalog.None;
        public override int LabelNumber => 1151782;  // cannot be repaired
        public override bool IsSpriteGraph => true;
        public override int SpriteW => 120;
        public override int SpriteH => 240;
        public override NegativeAttribute Attribute => NegativeAttribute.NoRepair;
    }
}
