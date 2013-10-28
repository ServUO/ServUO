using System;
using System.Drawing;

namespace Server.Gumps
{
    public class CustomServiceGumpling : Gumpling
    {
        public CustomServiceGumpling(Int32 width, Int32 height, Int32 coreLabelHue, String coreLabel, String version, GumpResponse saveButtonPressed) : base(0, 0)
        {
            Bitmap text = OpenUOSDK.AsciiFontFactory.GetText<Bitmap>(1, coreLabel, 0);

            Add(new StoneyBackground(width, height));
            Add(new GumpLabel((width - text.Width) / 2, 4, coreLabelHue, coreLabel));
            Add(new GumpImageTiled(2, 30, width - 4, 4, 0x13ED));
            Add(new GumpLabel(10, height - 25, 0x4B, String.Format("v{0}", version)));
            Add(new SaveCancelGumpling(width - 170, height - 30, saveButtonPressed));
        }
    }
}
