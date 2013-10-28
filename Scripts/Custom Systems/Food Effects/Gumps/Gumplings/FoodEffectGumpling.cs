using System;

using Server.Gumps;

namespace CustomsFramework.Systems.FoodEffects
{
    public class FoodEffectGumpling : Gumpling
    {
        public FoodEffectGumpling(Int32 x, Int32 y, Int32 width, String name, Int32 textEntryHue, String defaultValue) : base(x, y)
        {
            Add(new GumpLabel(0, 3, 0x0, name));
            Add(new GumpImageTiled(100, 0, width, 23, 0xA40));
            Add(new GumpImageTiled(101, 1, width - 2, 21, 0xBBC));
            Add(new GumpTextEntry(105, 1, width - 5, 21, textEntryHue, defaultValue, name));
        }
    }
}