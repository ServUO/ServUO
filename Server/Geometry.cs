#region Header
// **********
// ServUO - Geometry.cs
// **********
#endregion

#region References
using System;
#endregion

namespace Server
{
	[Parsable]
	public struct Point2D : IPoint2D, IComparable, IComparable<Point2D>
	{
		internal int m_X;
		internal int m_Y;

		public static readonly Point2D Zero = new Point2D(0, 0);

		public Point2D(int x, int y)
		{
			m_X = x;
			m_Y = y;
		}

		public Point2D(IPoint2D p)
			: this(p.X, p.Y)
		{ }

		[CommandProperty(AccessLevel.Counselor)]
		public int X { get { return m_X; } set { m_X = value; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int Y { get { return m_Y; } set { m_Y = value; } }

		public override string ToString()
		{
			return String.Format("({0}, {1})", m_X, m_Y);
		}

		public static Point2D Parse(string value)
		{
			int start = value.IndexOf('(');
			int end = value.IndexOf(',', start + 1);

			string param1 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(')', start + 1);

			string param2 = value.Substring(start + 1, end - (start + 1)).Trim();

			return new Point2D(Convert.ToInt32(param1), Convert.ToInt32(param2));
		}

		public int CompareTo(Point2D other)
		{
			int v = (m_X.CompareTo(other.m_X));

			if (v == 0)
			{
				v = (m_Y.CompareTo(other.m_Y));
			}

			return v;
		}

		public int CompareTo(object other)
		{
			if (other is Point2D)
			{
				return CompareTo((Point2D)other);
			}
			
			if (other == null)
			{
				return -1;
			}

			throw new ArgumentException();
		}

		public override bool Equals(object o)
		{
			if (!(o is IPoint2D))
			{
				return false;
			}

			IPoint2D p = (IPoint2D)o;

			return m_X == p.X && m_Y == p.Y;
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

		public static bool operator ==(Point2D l, IPoint2D r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.m_X == r.X && l.m_Y == r.Y;
		}

		public static bool operator !=(Point2D l, IPoint2D r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.m_X != r.X || l.m_Y != r.Y;
		}

		public static bool operator >(Point2D l, Point2D r)
		{
			return l.m_X > r.m_X && l.m_Y > r.m_Y;
		}

		public static bool operator >(Point2D l, Point3D r)
		{
			return l.m_X > r.m_X && l.m_Y > r.m_Y;
		}

		public static bool operator >(Point2D l, IPoint2D r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.m_X > r.X && l.m_Y > r.Y;
		}

		public static bool operator <(Point2D l, Point2D r)
		{
			return l.m_X < r.m_X && l.m_Y < r.m_Y;
		}

		public static bool operator <(Point2D l, Point3D r)
		{
			return l.m_X < r.m_X && l.m_Y < r.m_Y;
		}

		public static bool operator <(Point2D l, IPoint2D r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.m_X < r.X && l.m_Y < r.Y;
		}

		public static bool operator >=(Point2D l, Point2D r)
		{
			return l.m_X >= r.m_X && l.m_Y >= r.m_Y;
		}

		public static bool operator >=(Point2D l, Point3D r)
		{
			return l.m_X >= r.m_X && l.m_Y >= r.m_Y;
		}

		public static bool operator >=(Point2D l, IPoint2D r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.m_X >= r.X && l.m_Y >= r.Y;
		}

		public static bool operator <=(Point2D l, Point2D r)
		{
			return l.m_X <= r.m_X && l.m_Y <= r.m_Y;
		}

		public static bool operator <=(Point2D l, Point3D r)
		{
			return l.m_X <= r.m_X && l.m_Y <= r.m_Y;
		}

		public static bool operator <=(Point2D l, IPoint2D r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.m_X <= r.X && l.m_Y <= r.Y;
		}
	}

	[Parsable]
	public struct Point3D : IPoint3D, IComparable, IComparable<Point3D>
	{
		internal int m_X;
		internal int m_Y;
		internal int m_Z;

		public static readonly Point3D Zero = new Point3D(0, 0, 0);

		public Point3D(int x, int y, int z)
		{
			m_X = x;
			m_Y = y;
			m_Z = z;
		}

		public Point3D(IPoint3D p)
			: this(p.X, p.Y, p.Z)
		{ }

		public Point3D(IPoint2D p, int z)
			: this(p.X, p.Y, z)
		{ }

		[CommandProperty(AccessLevel.Counselor)]
		public int X { get { return m_X; } set { m_X = value; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int Y { get { return m_Y; } set { m_Y = value; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int Z { get { return m_Z; } set { m_Z = value; } }

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2})", m_X, m_Y, m_Z);
		}

		public override bool Equals(object o)
		{
			if (!(o is IPoint3D))
			{
				return false;
			}

			IPoint3D p = (IPoint3D)o;

			return m_X == p.X && m_Y == p.Y && m_Z == p.Z;
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

		public static Point3D Parse(string value)
		{
			int start = value.IndexOf('(');
			int end = value.IndexOf(',', start + 1);

			string param1 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(',', start + 1);

			string param2 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(')', start + 1);

			string param3 = value.Substring(start + 1, end - (start + 1)).Trim();

			return new Point3D(Convert.ToInt32(param1), Convert.ToInt32(param2), Convert.ToInt32(param3));
		}

		public static bool operator ==(Point3D l, Point3D r)
		{
			return l.m_X == r.m_X && l.m_Y == r.m_Y && l.m_Z == r.m_Z;
		}

		public static bool operator !=(Point3D l, Point3D r)
		{
			return l.m_X != r.m_X || l.m_Y != r.m_Y || l.m_Z != r.m_Z;
		}

		public static bool operator ==(Point3D l, IPoint3D r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.m_X == r.X && l.m_Y == r.Y && l.m_Z == r.Z;
		}

		public static bool operator !=(Point3D l, IPoint3D r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.m_X != r.X || l.m_Y != r.Y || l.m_Z != r.Z;
		}

		public int CompareTo(Point3D other)
		{
			int v = (m_X.CompareTo(other.m_X));

			if (v == 0)
			{
				v = (m_Y.CompareTo(other.m_Y));

				if (v == 0)
				{
					v = (m_Z.CompareTo(other.m_Z));
				}
			}

			return v;
		}

		public int CompareTo(object other)
		{
			if (other is Point3D)
			{
				return CompareTo((Point3D)other);
			}
			
			if (other == null)
			{
				return -1;
			}

			throw new ArgumentException();
		}
	}

	[NoSort]
	[Parsable]
	[PropertyObject]
	public struct Rectangle2D
	{
		private Point2D m_Start;
		private Point2D m_End;

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

		public void Set(int x, int y, int width, int height)
		{
			m_Start = new Point2D(x, y);
			m_End = new Point2D(x + width, y + height);
		}

		public static Rectangle2D Parse(string value)
		{
			int start = value.IndexOf('(');
			int end = value.IndexOf(',', start + 1);

			string param1 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(',', start + 1);

			string param2 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(',', start + 1);

			string param3 = value.Substring(start + 1, end - (start + 1)).Trim();

			start = end;
			end = value.IndexOf(')', start + 1);

			string param4 = value.Substring(start + 1, end - (start + 1)).Trim();

			return new Rectangle2D(
				Convert.ToInt32(param1), Convert.ToInt32(param2), Convert.ToInt32(param3), Convert.ToInt32(param4));
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Point2D Start { get { return m_Start; } set { m_Start = value; } }

		[CommandProperty(AccessLevel.Counselor)]
		public Point2D End { get { return m_End; } set { m_End = value; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int X { get { return m_Start.m_X; } set { m_Start.m_X = value; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int Y { get { return m_Start.m_Y; } set { m_Start.m_Y = value; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int Width { get { return m_End.m_X - m_Start.m_X; } set { m_End.m_X = m_Start.m_X + value; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int Height { get { return m_End.m_Y - m_Start.m_Y; } set { m_End.m_Y = m_Start.m_Y + value; } }

		public void MakeHold(Rectangle2D r)
		{
			if (r.m_Start.m_X < m_Start.m_X)
			{
				m_Start.m_X = r.m_Start.m_X;
			}

			if (r.m_Start.m_Y < m_Start.m_Y)
			{
				m_Start.m_Y = r.m_Start.m_Y;
			}

			if (r.m_End.m_X > m_End.m_X)
			{
				m_End.m_X = r.m_End.m_X;
			}

			if (r.m_End.m_Y > m_End.m_Y)
			{
				m_End.m_Y = r.m_End.m_Y;
			}
		}

		public bool Contains(Point3D p)
		{
			return (m_Start.m_X <= p.m_X && m_Start.m_Y <= p.m_Y && m_End.m_X > p.m_X && m_End.m_Y > p.m_Y);
			//return ( m_Start <= p && m_End > p );
		}

		public bool Contains(Point2D p)
		{
			return (m_Start.m_X <= p.m_X && m_Start.m_Y <= p.m_Y && m_End.m_X > p.m_X && m_End.m_Y > p.m_Y);
			//return ( m_Start <= p && m_End > p );
		}

		public bool Contains(IPoint2D p)
		{
			return (m_Start <= p && m_End > p);
		}

		public override string ToString()
		{
			return String.Format("({0}, {1})+({2}, {3})", X, Y, Width, Height);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 1 + Math.Abs(Start.X + Start.Y) + Math.Abs(End.X + End.Y);

				hash = (hash * 397) ^ Start.GetHashCode();
				hash = (hash * 397) ^ End.GetHashCode();

				return hash;
			}
		}
	}

	[NoSort]
	[PropertyObject]
	public struct Rectangle3D
	{
		private Point3D m_Start;
		private Point3D m_End;

		public Rectangle3D(Point3D start, Point3D end)
		{
			m_Start = start;
			m_End = end;
		}

		public Rectangle3D(int x, int y, int z, int width, int height, int depth)
		{
			m_Start = new Point3D(x, y, z);
			m_End = new Point3D(x + width, y + height, z + depth);
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Point3D Start { get { return m_Start; } set { m_Start = value; } }

		[CommandProperty(AccessLevel.Counselor)]
		public Point3D End { get { return m_End; } set { m_End = value; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int Width { get { return m_End.X - m_Start.X; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int Height { get { return m_End.Y - m_Start.Y; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int Depth { get { return m_End.Z - m_Start.Z; } }

		public bool Contains(Point3D p)
		{
			return (p.m_X >= m_Start.m_X) && (p.m_X < m_End.m_X) && (p.m_Y >= m_Start.m_Y) && (p.m_Y < m_End.m_Y) &&
				   (p.m_Z >= m_Start.m_Z) && (p.m_Z < m_End.m_Z);
		}

		public bool Contains(IPoint3D p)
		{
			return (p.X >= m_Start.m_X) && (p.X < m_End.m_X) && (p.Y >= m_Start.m_Y) && (p.Y < m_End.m_Y) && (p.Z >= m_Start.m_Z) &&
				   (p.Z < m_End.m_Z);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 1 + Math.Abs(Start.X + Start.Y + Start.Z) + Math.Abs(End.X + End.Y + End.Z);

				hash = (hash * 397) ^ Start.GetHashCode();
				hash = (hash * 397) ^ End.GetHashCode();

				return hash;
			}
		}
	}
}