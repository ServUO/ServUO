namespace Ultima
{
	public sealed class LocationPointer
	{
		public int PointerX { get; set; }
		public int PointerY { get; set; }
		public int PointerZ { get; set; }
		public int PointerF { get; set; }
		public int SizeX { get; set; }
		public int SizeY { get; set; }
		public int SizeZ { get; set; }
		public int SizeF { get; set; }

		public LocationPointer(int ptrX, int ptrY, int ptrZ, int ptrF, int sizeX, int sizeY, int sizeZ, int sizeF)
		{
			PointerX = ptrX;
			PointerY = ptrY;
			PointerZ = ptrZ;
			PointerF = ptrF;
			SizeX = sizeX;
			SizeY = sizeY;
			SizeZ = sizeZ;
			SizeF = sizeF;
		}
	}
}