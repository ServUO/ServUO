#region References
using System;
#endregion

namespace Server
{
	[Parsable, PropertyObject]
	public struct Point2D : IPoint2D, IComparable<Point2D>, IEquatable<Point2D>
	{
		public static readonly Point2D Zero = new Point2D(0, 0);

		public static bool TryParse(string value, out Point2D point)
		{
			try
			{
				point = Parse(value);
				return true;
			}
			catch
			{
				point = Zero;
				return false;
			}
		}

		public static Point2D Parse(string value)
		{
			var start = value.IndexOf('(');
			var end = value.IndexOf(',', start + 1);

			var param1 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(')', start + 1);

			var param2 = value.Substring(start + 1, end - (start + 1)).Trim();

			return new Point2D(Convert.ToInt32(param1), Convert.ToInt32(param2));
		}

		internal int m_X;
		internal int m_Y;

		[CommandProperty(AccessLevel.Counselor)]
		public int X { get => m_X; set => m_X = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Y { get => m_Y; set => m_Y = value; }

		public Point2D(Point2D p)
		{
			m_X = p.m_X;
			m_Y = p.m_Y;
		}

		public Point2D(IPoint2D p)
		{
			m_X = p.X;
			m_Y = p.Y;
		}

		public Point2D(Point3D p)
		{
			m_X = p.m_X;
			m_Y = p.m_Y;
		}

		public Point2D(IPoint3D p)
		{
			m_X = p.X;
			m_Y = p.Y;
		}

		public Point2D(int x, int y)
		{
			m_X = x;
			m_Y = y;
		}

		public override string ToString()
		{
			return String.Format("({0}, {1})", m_X, m_Y);
		}

		public int CompareTo(Point2D other)
		{
			var v = m_X.CompareTo(other.m_X);

			if (v == 0)
			{
				v = m_Y.CompareTo(other.m_Y);
			}

			return v;
		}

		public bool Equals(Point2D p)
		{
			return m_X == p.X && m_Y == p.Y;
		}

		public override bool Equals(object obj)
		{
			return obj is Point2D p && Equals(p);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 1 + Math.Abs(X) + Math.Abs(Y);

				hash = (hash * 397) ^ X;
				hash = (hash * 397) ^ Y;

				return hash;
			}
		}

		public static bool operator ==(Point2D l, Point2D r)
		{
			return l.m_X == r.m_X && l.m_Y == r.m_Y;
		}

		public static bool operator !=(Point2D l, Point2D r)
		{
			return l.m_X != r.m_X || l.m_Y != r.m_Y;
		}

		public static bool operator >(Point2D l, Point2D r)
		{
			return l.m_X > r.m_X && l.m_Y > r.m_Y;
		}

		public static bool operator >(Point2D l, Point3D r)
		{
			return l.m_X > r.m_X && l.m_Y > r.m_Y;
		}

		public static bool operator <(Point2D l, Point2D r)
		{
			return l.m_X < r.m_X && l.m_Y < r.m_Y;
		}

		public static bool operator <(Point2D l, Point3D r)
		{
			return l.m_X < r.m_X && l.m_Y < r.m_Y;
		}

		public static bool operator >=(Point2D l, Point2D r)
		{
			return l.m_X >= r.m_X && l.m_Y >= r.m_Y;
		}

		public static bool operator >=(Point2D l, Point3D r)
		{
			return l.m_X >= r.m_X && l.m_Y >= r.m_Y;
		}

		public static bool operator <=(Point2D l, Point2D r)
		{
			return l.m_X <= r.m_X && l.m_Y <= r.m_Y;
		}

		public static bool operator <=(Point2D l, Point3D r)
		{
			return l.m_X <= r.m_X && l.m_Y <= r.m_Y;
		}
	}

	[Parsable, PropertyObject]
	public struct Point3D : IPoint3D, IComparable<Point3D>, IEquatable<Point3D>
	{
		public static readonly Point3D Zero = new Point3D(0, 0, 0);

		public static bool TryParse(string value, out Point3D point)
		{
			try
			{
				point = Parse(value);
				return true;
			}
			catch
			{
				point = Zero;
				return false;
			}
		}

		public static Point3D Parse(string value)
		{
			var start = value.IndexOf('(');
			var end = value.IndexOf(',', start + 1);

			var param1 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(',', start + 1);

			var param2 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(')', start + 1);

			var param3 = value.Substring(start + 1, end - (start + 1)).Trim();

			return new Point3D(Convert.ToInt32(param1), Convert.ToInt32(param2), Convert.ToInt32(param3));
		}

		internal int m_X;
		internal int m_Y;
		internal int m_Z;

		[CommandProperty(AccessLevel.Counselor)]
		public int X { get => m_X; set => m_X = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Y { get => m_Y; set => m_Y = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Z { get => m_Z; set => m_Z = value; }

		public Point3D(Point3D p)
		{
			m_X = p.m_X;
			m_Y = p.m_Y;
			m_Z = p.m_Z;
		}

		public Point3D(IPoint3D p)
		{
			m_X = p.X;
			m_Y = p.Y;
			m_Z = p.Z;
		}

		public Point3D(Point2D p, int z)
		{
			m_X = p.m_X;
			m_Y = p.m_Y;
			m_Z = z;
		}

		public Point3D(IPoint2D p, int z)
		{
			m_X = p.X;
			m_Y = p.Y;
			m_Z = z;
		}

		public Point3D(int x, int y, int z)
		{
			m_X = x;
			m_Y = y;
			m_Z = z;
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2})", m_X, m_Y, m_Z);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 1 + Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

				hash = (hash * 397) ^ X;
				hash = (hash * 397) ^ Y;
				hash = (hash * 397) ^ Z;

				return hash;
			}
		}

		public int CompareTo(Point3D other)
		{
			var v = m_X.CompareTo(other.m_X);

			if (v == 0)
			{
				v = m_Y.CompareTo(other.m_Y);

				if (v == 0)
				{
					v = m_Z.CompareTo(other.m_Z);
				}
			}

			return v;
		}

		public bool Equals(Point3D p)
		{
			return m_X == p.X && m_Y == p.Y && m_Z == p.Z;
		}

		public override bool Equals(object obj)
		{
			return obj is Point3D p && Equals(p);
		}

		public static bool operator ==(Point3D l, Point3D r)
		{
			return l.m_X == r.m_X && l.m_Y == r.m_Y && l.m_Z == r.m_Z;
		}

		public static bool operator !=(Point3D l, Point3D r)
		{
			return l.m_X != r.m_X || l.m_Y != r.m_Y || l.m_Z != r.m_Z;
		}

		public static bool operator >(Point3D l, Point3D r)
		{
			return l.m_X > r.m_X && l.m_Y > r.m_Y;
		}

		public static bool operator >(Point3D l, Point2D r)
		{
			return l.m_X > r.m_X && l.m_Y > r.m_Y;
		}

		public static bool operator <(Point3D l, Point3D r)
		{
			return l.m_X < r.m_X && l.m_Y < r.m_Y;
		}

		public static bool operator <(Point3D l, Point2D r)
		{
			return l.m_X < r.m_X && l.m_Y < r.m_Y;
		}

		public static bool operator >=(Point3D l, Point3D r)
		{
			return l.m_X >= r.m_X && l.m_Y >= r.m_Y;
		}

		public static bool operator >=(Point3D l, Point2D r)
		{
			return l.m_X >= r.m_X && l.m_Y >= r.m_Y;
		}

		public static bool operator <=(Point3D l, Point3D r)
		{
			return l.m_X <= r.m_X && l.m_Y <= r.m_Y;
		}

		public static bool operator <=(Point3D l, Point2D r)
		{
			return l.m_X <= r.m_X && l.m_Y <= r.m_Y;
		}
	}

	[NoSort, Parsable, PropertyObject]
	public struct Rectangle2D : IEquatable<Rectangle2D>
	{
		public static readonly Rectangle2D Empty = new Rectangle2D(0, 0, 0, 0);

		public static bool TryParse(string value, out Rectangle2D rect)
		{
			try
			{
				rect = Parse(value);
				return true;
			}
			catch
			{
				rect = Empty;
				return false;
			}
		}

		public static Rectangle2D Parse(string value)
		{
			var start = value.IndexOf('(');
			var end = value.IndexOf(',', start + 1);

			var param1 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(',', start + 1);

			var param2 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(',', start + 1);

			var param3 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(')', start + 1);

			var param4 = value.Substring(start + 1, end - (start + 1)).Trim();

			return new Rectangle2D(
				Convert.ToInt32(param1),
				Convert.ToInt32(param2),
				Convert.ToInt32(param3),
				Convert.ToInt32(param4));
		}

		private Point2D m_Start;
		private Point2D m_End;

		[CommandProperty(AccessLevel.Counselor)]
		public Point2D Start { get => m_Start; set => m_Start = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public Point2D End { get => m_End; set => m_End = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int X { get => m_Start.m_X; set => m_Start.m_X = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Y { get => m_Start.m_Y; set => m_Start.m_Y = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Width { get => m_End.m_X - m_Start.m_X; set => m_End.m_X = m_Start.m_X + value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Height { get => m_End.m_Y - m_Start.m_Y; set => m_End.m_Y = m_Start.m_Y + value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Left { get => m_Start.m_X; set => m_Start.m_X = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Right { get => m_End.m_X; set => m_End.m_X = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Top { get => m_Start.m_Y; set => m_Start.m_Y = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Bottom { get => m_End.m_Y; set => m_End.m_Y = value; }

		public Rectangle2D(Point2D start, Point2D end)
		{
			m_Start = start;
			m_End = end;
		}

		public Rectangle2D(IPoint2D start, IPoint2D end)
		{
			m_Start = new Point2D(start);
			m_End = new Point2D(end);
		}

		public Rectangle2D(int x, int y, int width, int height)
		{
			m_Start = new Point2D(x, y);
			m_End = new Point2D(x + width, y + height);
		}

		public override string ToString()
		{
			return String.Format("({0}, {1})+({2}, {3})", X, Y, Width, Height);
		}

		public void Set(int x, int y, int width, int height)
		{
			m_Start.m_X = x;
			m_Start.m_Y = y;

			m_End.m_X = x + width;
			m_End.m_Y = y + height;
		}

		public bool Contains(Point3D p)
		{
			return Contains(p, false);
		}

		public bool Contains(Point3D p, bool inclusive)
		{
			return Contains(p.m_X, p.m_Y, inclusive);
		}

		public bool Contains(IPoint3D p)
		{
			return Contains(p, false);
		}

		public bool Contains(IPoint3D p, bool inclusive)
		{
			return Contains(p.X, p.Y, inclusive);
		}

		public bool Contains(Point2D p)
		{
			return Contains(p, false);
		}

		public bool Contains(Point2D p, bool inclusive)
		{
			return Contains(p.m_X, p.m_Y, inclusive);
		}

		public bool Contains(IPoint2D p)
		{
			return Contains(p, false);
		}

		public bool Contains(IPoint2D p, bool inclusive)
		{
			return Contains(p.X, p.Y, inclusive);
		}

		public bool Contains(int x, int y)
		{
			return Contains(x, y, false);
		}

		public bool Contains(int x, int y, bool inclusive)
		{
			if (inclusive)
			{
				return x >= m_Start.m_X && x <= m_End.m_X && y >= m_Start.m_Y && y <= m_End.m_Y;
			}

			return x >= m_Start.m_X && x < m_End.m_X && y >= m_Start.m_Y && y < m_End.m_Y;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 1 + Math.Abs(m_Start.m_X + m_Start.m_Y) + Math.Abs(m_End.m_X + m_End.m_Y);

				hash = (hash * 397) ^ m_Start.GetHashCode();
				hash = (hash * 397) ^ m_End.GetHashCode();

				return hash;
			}
		}

		public bool Equals(Rectangle2D rect)
		{
			return m_Start == rect.m_Start && m_End == rect.m_End;
		}

		public override bool Equals(object obj)
		{
			return obj is Rectangle2D r && Equals(r);
		}

		public static bool operator ==(Rectangle2D a, Rectangle2D b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Rectangle2D a, Rectangle2D b)
		{
			return !a.Equals(b);
		}
	}

	[NoSort, Parsable, PropertyObject]
	public struct Rectangle3D : IEquatable<Rectangle3D>
	{
		public static readonly Rectangle3D Empty = new Rectangle3D(0, 0, 0, 0, 0, 0);

		public static bool TryParse(string value, out Rectangle3D rect)
		{
			try
			{
				rect = Parse(value);
				return true;
			}
			catch
			{
				rect = Empty;
				return false;
			}
		}

		public static Rectangle3D Parse(string value)
		{
			var start = value.IndexOf('(');
			var end = value.IndexOf(',', start + 1);

			var param1 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(',', start + 1);

			var param2 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(',', start + 1);

			var param3 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(',', start + 1);

			var param4 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(',', start + 1);

			var param5 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(')', start + 1);

			var param6 = value.Substring(start + 1, end - (start + 1)).Trim();

			return new Rectangle3D(
				Convert.ToInt32(param1),
				Convert.ToInt32(param2),
				Convert.ToInt32(param3),
				Convert.ToInt32(param4),
				Convert.ToInt32(param5),
				Convert.ToInt32(param6));
		}

		private Point3D m_Start;
		private Point3D m_End;

		[CommandProperty(AccessLevel.Counselor)]
		public Point3D Start { get => m_Start; set => m_Start = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public Point3D End { get => m_End; set => m_End = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int X { get => m_Start.m_X; set => m_Start.m_X = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Y { get => m_Start.m_Y; set => m_Start.m_Y = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Z { get => m_Start.m_Z; set => m_Start.m_Z = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Width { get => m_End.m_X - m_Start.m_X; set => m_End.m_X = m_Start.m_X + value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Height { get => m_End.m_Y - m_Start.m_Y; set => m_End.m_Y = m_Start.m_Y + value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Depth { get => m_End.m_Z - m_Start.m_Z; set => m_End.m_Z = m_Start.m_Z + value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Left { get => m_Start.m_X; set => m_Start.m_X = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Right { get => m_End.m_X; set => m_End.m_X = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Top { get => m_Start.m_Y; set => m_Start.m_Y = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public int Bottom { get => m_End.m_Y; set => m_End.m_Y = value; }

		public Rectangle3D(Point3D start, Point3D end)
		{
			m_Start = start;
			m_End = end;
		}

		public Rectangle3D(IPoint3D start, IPoint3D end)
		{
			m_Start = new Point3D(start);
			m_End = new Point3D(end);
		}

		public Rectangle3D(int x, int y, int z, int width, int height, int depth)
		{
			m_Start = new Point3D(x, y, z);
			m_End = new Point3D(x + width, y + height, z + depth);
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2})+({3}, {4}, {5})", X, Y, Z, Width, Height, Depth);
		}

		public void Set(int x, int y, int z, int width, int height, int depth)
		{
			m_Start.m_X = x;
			m_Start.m_Y = y;
			m_Start.m_Z = z;

			m_End.m_X = x + width;
			m_End.m_Y = y + height;
			m_End.m_Z = z + depth;
		}

		public bool Contains(Point2D p)
		{
			return Contains(p, false);
		}

		public bool Contains(Point2D p, bool inclusive)
		{
			return Contains(p.m_X, p.m_Y, inclusive);
		}

		public bool Contains(IPoint2D p)
		{
			return Contains(p, false);
		}

		public bool Contains(IPoint2D p, bool inclusive)
		{
			return Contains(p.X, p.Y, inclusive);
		}

		public bool Contains(Point3D p)
		{
			return Contains(p, false);
		}

		public bool Contains(Point3D p, bool inclusive)
		{
			return Contains(p.m_X, p.m_Y, p.m_Z, inclusive);
		}

		public bool Contains(IPoint3D p)
		{
			return Contains(p, false);
		}

		public bool Contains(IPoint3D p, bool inclusive)
		{
			return Contains(p.X, p.Y, p.Z, inclusive);
		}

		public bool Contains(int x, int y)
		{
			return Contains(x, y, false);
		}

		public bool Contains(int x, int y, bool inclusive)
		{
			if (inclusive)
			{
				return x >= m_Start.m_X && x <= m_End.m_X && y >= m_Start.m_Y && y <= m_End.m_Y;
			}

			return x >= m_Start.m_X && x < m_End.m_X && y >= m_Start.m_Y && y < m_End.m_Y;
		}

		public bool Contains(int x, int y, int z)
		{
			return Contains(x, y, z, false);
		}

		public bool Contains(int x, int y, int z, bool inclusive)
		{
			if (inclusive)
			{
				return Contains(x, y, true) && z >= m_Start.m_Z && z <= m_End.m_Z;
			}

			return Contains(x, y, false) && z >= m_Start.m_Z && z < m_End.m_Z;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 1 + Math.Abs(m_Start.m_X + m_Start.m_Y + m_Start.m_Z) + Math.Abs(m_End.m_X + m_End.m_Y + m_End.m_Z);

				hash = (hash * 397) ^ m_Start.GetHashCode();
				hash = (hash * 397) ^ m_End.GetHashCode();

				return hash;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Rectangle3D r && Equals(r);
		}

		public bool Equals(Rectangle3D rect)
		{
			return m_Start == rect.m_Start && m_End == rect.m_End;
		}

		public static bool operator ==(Rectangle3D a, Rectangle3D b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Rectangle3D a, Rectangle3D b)
		{
			return !a.Equals(b);
		}
	}
}
