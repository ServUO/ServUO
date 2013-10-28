#region Header
// **********
// ServUO - ItemBounds.cs
// **********
#endregion

#region References
using System;
using System.IO;
#endregion

namespace Server
{
	public static class ItemBounds
	{
		private static readonly Rectangle2D[] m_Bounds;

		public static Rectangle2D[] Table { get { return m_Bounds; } }

		static ItemBounds()
		{
			m_Bounds = new Rectangle2D[TileData.ItemTable.Length];

			if (File.Exists("Data/Binary/Bounds.bin"))
			{
				using (FileStream fs = new FileStream("Data/Binary/Bounds.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					BinaryReader bin = new BinaryReader(fs);

					int count = Math.Min(m_Bounds.Length, (int)(fs.Length / 8));

					for (int i = 0; i < count; ++i)
					{
						int xMin = bin.ReadInt16();
						int yMin = bin.ReadInt16();
						int xMax = bin.ReadInt16();
						int yMax = bin.ReadInt16();

						m_Bounds[i].Set(xMin, yMin, (xMax - xMin) + 1, (yMax - yMin) + 1);
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