using System;
using System.Drawing;

namespace Server.Gumps
{
    public class CenteredLabel : Gumpling
    {
        public CenteredLabel(Int32 x, Int32 y, Int32 width, Int32 hue, String label) : base(x, y)
        {
			Bitmap text = Ultima.ASCIIText.DrawText(1, label);

            Add(new GumpLabel((width - text.Width) / 2, 0, hue, label));
        }
    }
}