#region References
using System;
using System.IO;
#endregion

namespace Server
{
	public static class ItemBounds
	{
		private static readonly Rectangle2D[] m_Bounds;

		public static Rectangle2D[] Table => m_Bounds;

		static ItemBounds()
		{
			m_Bounds = new Rectangle2D[TileData.ItemTable.Length];

			if (File.Exists("Data/Binary/Bounds.bin"))
			{
				using (var fs = new FileStream("Data/Binary/Bounds.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var bin = new BinaryReader(fs);

					var count = Math.Min(m_Bounds.Length, (int)(fs.Length / 8));

					for (var i = 0; i < count; ++i)
					{
						int xMin = bin.ReadInt16();
						int yMin = bin.ReadInt16();
						int xMax = bin.ReadInt16();
						int yMax = bin.ReadInt16();

						m_Bounds[i].Set(xMin, yMin, xMax - xMin + 1, yMax - yMin + 1);
					}

					bin.Close();
				}
			}
			else
			{
				Console.WriteLine("Warning: Data/Binary/Bounds.bin does not exist");
			}
		}
	}
}