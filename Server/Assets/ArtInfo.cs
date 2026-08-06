// **********
// ServUO - ArtInfo.cs
// **********

using System.Drawing;

namespace Server
{
	public class ArtInfo
	{
		public int Width;
		public int Height;
		public Rectangle Bounds;
		
		public ArtInfo(int width, int height, Rectangle bounds)
		{
			Width = width;
			Height = height;
			Bounds = bounds;
		}
	}
}
