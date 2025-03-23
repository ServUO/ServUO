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
	public struct Triangle2D : IPoint2D
	{
		public static readonly Triangle2D Empty = new Triangle2D(0, 0, 0, 0, 0, 0);

		public static IEnumerable<Point2D> Plot(IPoint2D a, IPoint2D b, IPoint2D c)
		{
			foreach (var p in Line2D.Plot(a, b).Skip(1))
			{
				yield return p;
			}

			foreach (var p in Line2D.Plot(b, c).Skip(1))
			{
				yield return p;
			}

			foreach (var p in Line2D.Plot(c, a).Skip(1))
			{
				yield return p;
			}
		}

		public static bool Contains(IPoint2D p, IPoint2D a, IPoint2D b, IPoint2D c)
		{
			var x = p.X - a.X;
			var y = p.Y - a.Y;

			var delta = (b.X - a.X) * y - (b.Y - a.Y) * x > 0;

			if ((c.X - a.X) * y - (c.Y - a.Y) * x > 0 == delta)
			{
				return false;
			}

			if ((c.X - b.X) * (p.Y - b.Y) - (c.Y - b.Y) * (p.X - b.X) > 0 != delta)
			{
				return false;
			}

			return true;
		}

		public static bool Contains(int x, int y, IPoint2D a, IPoint2D b, IPoint2D c)
		{
			return Contains(new Point2D(x, y), a, b, c);
		}

		public static IEnumerable<Point2D> Intersect(Triangle2D t1, Triangle2D t2)
		{
			return Line2D.Intersect(new[] { t1._AB, t1._BC, t1._CA }, new[] { t2._AB, t2._BC, t2._CA });
		}

		public static IEnumerable<Point2D> Intersect(Triangle2D t, Rectangle2D r)
		{
			return Line2D.Intersect(
				new[] { t._AB, t._BC, t._CA },
				new[]
				{
					new Line2D(r.X, r.Y, r.X + r.Width, r.Y), new Line2D(r.X + r.Width, r.Y, r.X + r.Width, r.Y + r.Height),
					new Line2D(r.X, r.Y, r.X, r.Y + r.Height), new Line2D(r.X, r.Y + r.Height, r.X + r.Width, r.Y + r.Height)
				});
		}

		public static bool Intersects(Triangle2D t1, Triangle2D t2)
		{
			return Intersect(t1, t2).Any();
		}

		public static bool Intersects(Triangle2D t, Rectangle2D r)
		{
			return Intersect(t, r).Any();
		}

		public static bool TryParse(string value, out Triangle2D t)
		{
			try
			{
				t = Parse(value);
				return true;
			}
			catch
			{
				t = Empty;
				return false;
			}
		}

		public static Triangle2D Parse(string value)
		{
			var param = value.Split('+');

			if (param.Length >= 3 && param.All(p => p.Contains(',')))
			{
				return new Triangle2D(Point2D.Parse(param[0]), Point2D.Parse(param[1]), Point2D.Parse(param[2]));
			}

			throw new FormatException(
				"The specified triangle must be represented by three Point2D coords using the format " + //
				"'(x1,y1)+(x2,y2)+(x3,y3)'");
		}

		private Point2D _A, _B, _C;

		private Line2D _AB, _BC, _CA;
		private Line2D _AC, _CB, _BA;

		[CommandProperty(AccessLevel.Counselor)]
		public int X
		{
			get => _A.X;
			set
			{
				_A.X = value;
				_AB.Start = _BA.End = _AC.Start = _CA.End = _A;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public int Y
		{
			get => _A.Y;
			set
			{
				_A.Y = value;
				_AB.Start = _BA.End = _AC.Start = _CA.End = _A;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Point2D A { get => _A; set => _A = _AB.Start = _BA.End = _AC.Start = _CA.End = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public Point2D B { get => _B; set => _B = _BC.Start = _CB.End = _BA.Start = _AB.End = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public Point2D C { get => _C; set => _C = _CA.Start = _AC.End = _CB.Start = _BC.End = value; }

		[CommandProperty(AccessLevel.Counselor)]
		public Line2D AB
		{
			get => _AB;
			set
			{
				_AB = value;

				_A = _BA.End = _AB.Start;
				_B = _BA.Start = _AB.End;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Line2D BC
		{
			get => _BC;
			set
			{
				_BC = value;

				_B = _CB.End = _BC.Start;
				_C = _CB.Start = _BC.End;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Line2D CA
		{
			get => _CA;
			set
			{
				_CA = value;

				_C = _AC.End = _CA.Start;
				_A = _AC.Start = _CA.End;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Line2D AC
		{
			get => _AC;
			set
			{
				_AC = value;

				_A = _CA.End = _AC.Start;
				_C = _CA.Start = _AC.End;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Line2D CB
		{
			get => _CB;
			set
			{
				_CB = value;

				_C = _BC.End = _CB.Start;
				_B = _BC.Start = _CB.End;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Line2D BA
		{
			get => _BA;
			set
			{
				_BA = value;

				_B = _AB.End = _BA.Start;
				_A = _AB.Start = _BA.End;
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Angle ABC => Angle.FromPoints(_A, _B, _C);

		[CommandProperty(AccessLevel.Counselor)]
		public Angle BCA => Angle.FromPoints(_B, _C, _A);

		[CommandProperty(AccessLevel.Counselor)]
		public Angle CAB => Angle.FromPoints(_C, _A, _B);

		public Triangle2D(IPoint2D a, IPoint2D b, IPoint2D c)
		{
			_A = new Point2D(a);
			_B = new Point2D(b);
			_C = new Point2D(c);

			_AB = new Line2D(_A, _B);
			_BC = new Line2D(_B, _C);
			_CA = new Line2D(_C, _A);

			_AC = new Line2D(_A, _C);
			_CB = new Line2D(_C, _B);
			_BA = new Line2D(_B, _A);
		}

		public Triangle2D(int x1, int y1, int x2, int y2, int x3, int y3)
		{
			_A = new Point2D(x1, y1);
			_B = new Point2D(x2, y2);
			_C = new Point2D(x3, y3);

			_AB = new Line2D(_A, _B);
			_BC = new Line2D(_B, _C);
			_CA = new Line2D(_C, _A);

			_AC = new Line2D(_A, _C);
			_CB = new Line2D(_C, _B);
			_BA = new Line2D(_B, _A);
		}

		public bool Contains(IPoint2D p)
		{
			return Contains(p, _A, _B, _C);
		}

		public bool Contains(int x, int y)
		{
			return Contains(x, y, _A, _B, _C);
		}

		public IEnumerable<Point2D> Intersect(Triangle2D t)
		{
			return Intersect(this, t);
		}

		public IEnumerable<Point2D> Intersect(Rectangle2D r)
		{
			return Intersect(this, r);
		}

		public bool Intersects(Triangle2D t)
		{
			return Intersects(this, t);
		}

		public bool Intersects(Rectangle2D r)
		{
			return Intersects(this, r);
		}

		public IEnumerable<Point2D> Plot()
		{
			return Plot(_A, _B, _C);
		}

		public override string ToString()
		{
			return String.Format("{0}+{1}+{2}", _A, _B, _C);
		}
	}
}