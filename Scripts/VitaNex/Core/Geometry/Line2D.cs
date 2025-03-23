#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server
{
	[NoSort, Parsable, PropertyObject]
	public struct Line2D : IPoint2D
	{
		public static readonly Line2D Empty = new Line2D(0, 0, 0, 0);

		private static void Swap<T>(ref T a, ref T b)
		{
			var t = a;
			a = b;
			b = t;
		}

		private static IEnumerable<Point2D> Plot(int x1, int y1, int x2, int y2)
		{
			var delta = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);

			if (delta)
			{
				Swap(ref x1, ref y1);
				Swap(ref x2, ref y2);
			}

			if (x1 > x2)
			{
				Swap(ref x1, ref x2);
				Swap(ref y1, ref y2);
			}

			var dX = x2 - x1;
			var dY = Math.Abs(y2 - y1);
			var eX = dX / 2;
			var sY = y1 < y2 ? 1 : -1;

			var y = y1;

			for (var x = x1; x <= x2; x++)
			{
				if (delta)
				{
					yield return new Point2D(y, x);
				}
				else
				{
					yield return new Point2D(x, y);
				}

				eX -= dY;

				if (eX < 0)
				{
					y += sY;
					eX += dX;
				}
			}
		}

		public static IEnumerable<Point2D> Plot(IPoint2D start, IPoint2D end)
		{
			return Plot(start.X, start.Y, end.X, end.Y).OrderBy(p => GetLength(start, p));
		}

		public static double GetLength(IPoint2D start, IPoint2D end)
		{
			return Math.Abs(Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2)));
		}

		public static double GetLength(Line2D line)
		{
			return GetLength(line.Start, line.End);
		}

		public static IEnumerable<Point2D> Intersect(Line2D[] lines1, Line2D[] lines2)
		{
			return lines1.Ensure().SelectMany(l => Intersect(l, lines2));
		}

		public static IEnumerable<Point2D> Intersect(Line2D line, Line2D[] lines)
		{
			return lines.Ensure().Select(l => Intersect(line, l)).Where(p => p != null).Select(p => p.Value);
		}

		public static Point2D? Intersect(IPoint2D a1, IPoint2D b1, IPoint2D a2, IPoint2D b2)
		{
			if ((a1.X == a2.X && a1.Y == a2.Y) || (a1.X == b2.X && a1.Y == b2.Y))
			{
				return new Point2D(a1.X, a1.Y);
			}

			if ((b1.X == b2.X && b1.Y == b2.Y) || (b1.X == a2.X && b1.Y == a2.Y))
			{
				return new Point2D(b1.X, b1.Y);
			}

			var da1 = b1.Y - a1.Y;
			var da2 = a1.X - b1.X;
			var da3 = da1 * a1.X + da2 * a1.Y;

			var db1 = b2.Y - a2.Y;
			var db2 = a2.X - b2.X;
			var db3 = db1 * a2.X + db2 * a2.Y;

			var delta = da1 * db2 - db1 * da2;

			if (delta != 0)
			{
				return new Point2D((db2 * da3 - da2 * db3) / delta, (da1 * db3 - db1 * da3) / delta);
			}

			return null;
		}

		public static Point2D? Intersect(Line2D line, IPoint2D a, IPoint2D b)
		{
			return Intersect(line._Start, line._End, a, b);
		}

		public static Point2D? Intersect(Line2D l1, Line2D l2)
		{
			return Intersect(l1._Start, l1._End, l2._Start, l2._End);
		}

		public static bool Intersects(Line2D[] lines1, Line2D[] lines2)
		{
			return Intersect(lines1, lines2).Any();
		}

		public static bool Intersects(Line2D line, Line2D[] lines)
		{
			return Intersect(line, lines).Any();
		}

		public static bool Intersects(Line2D line, IPoint2D a, IPoint2D b)
		{
			return Intersect(line, a, b) != null;
		}

		public static bool Intersects(IPoint2D a1, IPoint2D b1, IPoint2D a2, IPoint2D b2)
		{
			return Intersect(a1, b1, a2, b2) != null;
		}

		public static bool Intersects(Line2D l1, Line2D l2)
		{
			return Intersect(l1, l2) != null;
		}

		public static bool TryParse(string value, out Line2D l)
		{
			try
			{
				l = Parse(value);
				return true;
			}
			catch
			{
				l = Empty;
				return false;
			}
		}

		public static Line2D Parse(string value)
		{
			var param = value.Split('+');

			if (param.Length >= 2 && param.All(p => p.Contains(',')))
			{
				return new Line2D(Point2D.Parse(param[0]), Point2D.Parse(param[1]));
			}

			throw new FormatException(
				"The specified line must be represented by two Point2D coords using the format " + //
				"'(x1,y1)+(x2,y2)'");
		}

		private Point2D _Start, _End;

		private Angle _Rotation;
		private double _Length;

		[CommandProperty(AccessLevel.Counselor)]
		public int X
		{
			get => _Start.X;
			set
			{
				_Start.X = value;

				_Rotation = Angle.FromPoints(_Start, _End);
				_Length = _Start.GetDistance(_End);
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public int Y
		{
			get => _Start.Y;
			set
			{
				_Start.Y = value;

				_Rotation = Angle.FromPoints(_Start, _End);
				_Length = _Start.GetDistance(_End);
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Point2D Start
		{
			get => _Start;
			set
			{
				_Start = value;

				_Rotation = Angle.FromPoints(_Start, _End);
				_Length = _Start.GetDistance(_End);
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Point2D End
		{
			get => _End;
			set
			{
				_End = value;

				_Rotation = Angle.FromPoints(_Start, _End);
				_Length = _Start.GetDistance(_End);
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Angle Rotation
		{
			get => _Rotation;
			set
			{
				_Rotation = value;
				_End = _Rotation.GetPoint2D(_Start.X, _Start.Y, _Length);
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public double Length
		{
			get => _Length;
			set
			{
				_Length = value;
				_End = _Rotation.GetPoint2D(_Start.X, _Start.Y, _Length);
			}
		}

		public Line2D(IPoint2D start, IPoint2D end)
		{
			_Start = new Point2D(start);
			_End = new Point2D(end);

			_Rotation = Angle.FromPoints(_Start, _End);
			_Length = _Start.GetDistance(_End);
		}

		public Line2D(int xStart, int yStart, int xEnd, int yEnd)
		{
			_Start = new Point2D(xStart, yStart);
			_End = new Point2D(xEnd, yEnd);

			_Rotation = Angle.FromPoints(_Start, _End);
			_Length = _Start.GetDistance(_End);
		}

		public Line2D(IPoint2D start, Angle angle, double length)
		{
			_Rotation = angle;
			_Length = length;

			_Start = new Point2D(start.X, start.Y);
			_End = _Rotation.GetPoint2D(start.X, start.Y, _Length);
		}

		public Line2D(int xStart, int yStart, Angle angle, double length)
		{
			_Rotation = angle;
			_Length = length;

			_Start = new Point2D(xStart, yStart);
			_End = _Rotation.GetPoint2D(xStart, yStart, _Length);
		}

		public IEnumerable<Point2D> Intersect(Line2D[] lines)
		{
			return Intersect(this, lines);
		}

		public Point2D? Intersect(Line2D line)
		{
			return Intersect(this, line);
		}

		public bool Intersects(Line2D line)
		{
			return Intersects(this, line);
		}

		public bool Intersects(Line2D[] lines)
		{
			return Intersects(this, lines);
		}

		public bool Intersects(IPoint2D a2, IPoint2D b2)
		{
			return Intersects(this, a2, b2);
		}

		public override string ToString()
		{
			return String.Format("{0}+{1}", _Start, _End);
		}
	}
}