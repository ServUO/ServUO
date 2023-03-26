namespace Server.Targeting
{
	public class StaticTarget : IPoint3D
	{
		public StaticTarget(Point3D location, int itemID)
			: this(location, itemID, 0)
		{ }

		public StaticTarget(Point3D location, int itemID, int hue)
		{
			ItemID = itemID & TileData.MaxItemValue;
			Hue = hue & 0x3FFF;

			location.Z += TileData.ItemTable[ItemID].CalcHeight;

			Location = location;
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Point3D Location { get; }

		[CommandProperty(AccessLevel.Counselor)]
		public int X => Location.X;

		[CommandProperty(AccessLevel.Counselor)]
		public int Y => Location.Y;

		[CommandProperty(AccessLevel.Counselor)]
		public int Z => Location.Z;

		[CommandProperty(AccessLevel.Counselor)]
		public int ItemID { get; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Hue { get; }

		[CommandProperty(AccessLevel.Counselor)]
		public string Name => TileData.ItemTable[ItemID].Name;

		[CommandProperty(AccessLevel.Counselor)]
		public TileFlag Flags => TileData.ItemTable[ItemID].Flags;
	}
}