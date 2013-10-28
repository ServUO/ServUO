#region Header
// **********
// ServUO - Movement.cs
// **********
#endregion

namespace Server.Movement
{
	public static class Movement
	{
		private static IMovementImpl m_Impl;

		public static IMovementImpl Impl { get { return m_Impl; } set { m_Impl = value; } }

		public static bool CheckMovement(Mobile m, Direction d, out int newZ)
		{
			if (m_Impl != null)
			{
				return m_Impl.CheckMovement(m, d, out newZ);
			}

			newZ = m.Z;
			return false;
		}

		public static bool CheckMovement(Mobile m, Map map, Point3D loc, Direction d, out int newZ)
		{
			if (m_Impl != null)
			{
				return m_Impl.CheckMovement(m, map, loc, d, out newZ);
			}

			newZ = m.Z;
			return false;
		}

		public static void Offset(Direction d, ref int x, ref int y)
		{
			switch (d & Direction.Mask)
			{
				case Direction.North:
					--y;
					break;
				case Direction.South:
					++y;
					break;
				case Direction.West:
					--x;
					break;
				case Direction.East:
					++x;
					break;
				case Direction.Right:
					++x;
					--y;
					break;
				case Direction.Left:
					--x;
					++y;
					break;
				case Direction.Down:
					++x;
					++y;
					break;
				case Direction.Up:
					--x;
					--y;
					break;
			}
		}
	}

	public interface IMovementImpl
	{
		bool CheckMovement(Mobile m, Direction d, out int newZ);
		bool CheckMovement(Mobile m, Map map, Point3D loc, Direction d, out int newZ);
	}
}