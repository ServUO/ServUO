using System.Collections.Generic;

namespace Server.Mobiles
{
    public enum Catalog
    {
        None = 0,
        Attributes = 1,
        Resistances = 2,
        Combat1 = 3,
        Combat2 = 4,
        Casting = 5,
        Misc = 6,
        HitEffects = 7,
        SkillBonusGear = 8
    }

    public abstract class Property
    {
        public abstract int LabelNumber { get; }
        public abstract Catalog Catalog { get; }
        public virtual int Order { get; } = 1000;
        public virtual int Cap { get; set; }
        public virtual int Description { get; set; }

        public virtual bool IsBoolen => false;
        public virtual bool IsMagical => false;
        public virtual bool AlwaysVisible => false;

        public virtual bool IsSpriteGraph => false;
        public virtual int SpriteH { get; set; }
        public virtual int SpriteW { get; set; }

        public abstract bool Matches(Item item);
        public abstract bool Matches(List<Item> items);
    }

    public abstract class ValuedProperty : Property
    {
        public virtual int Hue { get; }
        public double Value { get; set; }

        public override bool Matches(List<Item> items) { return false; }
    }

    public class LabelDefinition
    {
        public int TitleLabel { get; }
        public Catalog Catalog { get; }
        public int ColumnLeftCount { get; }

        public LabelDefinition(int tl, Catalog ctlg, int cl = 0)
        {
            TitleLabel = tl;
            Catalog = ctlg;
            ColumnLeftCount = cl;
        }
    }
}
