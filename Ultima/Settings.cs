#region References
using System.Drawing.Imaging;
#endregion

namespace Ultima
{
	public static class Settings
	{
#if MONO
		public const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format16bppRgb555;
#else
		public const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format16bppArgb1555;
#endif
	}
}