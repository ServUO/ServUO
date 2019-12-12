using System;
using System.Collections.Generic;
using System.Linq;

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
        SkillBonusGear = 8,
    }

    public abstract class Property
    {
        public abstract int LabelNumber { get; }
        public abstract Catalog Catalog { get; }
        public virtual int Order { get; } = 1000;
        public virtual int Cap { get; set; } = 0;
        public virtual int Description { get; set; }

        public virtual bool IsBoolen { get { return false; } }
        public virtual bool BoolenValue { get { return false; } }
        public virtual bool AlwaysVisible { get { return false; } }

        public virtual bool IsSpriteGraph { get { return false; } }
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
        public int TitleLabel { get; set; }
        public Catalog Catalog { get; set; }
        public int ColumnLeftCount { get; set; }

        public LabelDefinition(int tl, Catalog ctlg, int cl = 0)
        {
            TitleLabel = tl;
            Catalog = ctlg;
            ColumnLeftCount = cl;
        }
    }
}
