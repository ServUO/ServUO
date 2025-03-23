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
#endregion

namespace Server
{
	[Parsable, PropertyObject]
	public struct Angle
		: IEquatable<Angle>, IEquatable<int>, IEquatable<double>, IComparable<Angle>, IComparable<int>, IComparable<double>
	{
		public const double D2R = Math.PI / 180.0;
		public const double R2D = 180.0 / Math.PI;

		public static readonly Angle Zero = 0;

		public static readonly Angle MinValue = -360;
		public static readonly Angle MaxValue = 360;

		public static Angle FromDirection(Direction dir)
		{
			int x = 0, y = 0;

			Movement.Movement.Offset(dir & Direction.Mask, ref x, ref y);

			return FromPoints(0, 0, x, y);
		}

		public static Angle FromPoints(IPoint2D p1, IPoint2D p2)
		{
			return FromPoints(p1.X, p1.Y, p2.X, p2.Y);
		}

		public static Angle FromPoints(IPoint2D p1, IPoint2D p2, IPoint2D p3)
		{
			return FromPoints(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);
		}

		public static Angle FromPoints(int x1, int y1, int x2, int y2)
		{
			return Math.Atan2(y2, x2) - Math.Atan2(y1, x1);
		}

		public static Angle FromPoints(int x1, int y1, int x2, int y2, int x3, int y3)
		{
			return FromPoints(x2, y2, x1, y1) - FromPoints(x2, y2, x3, y3);
		}

		public static Angle FromDegrees(int degrees)
		{
			return degrees;
		}

		public static Angle FromRadians(double radians)
		{
			return radians;
		}

		public static Angle GetPitch(IPoint3D p1, IPoint3D p2)
		{
			int x = p2.X - p1.X, y = p2.Y - p1.Y, z = p2.Z - p1.Z;

			return -Math.Atan2(z, Math.Sqrt((x * x) + (y * y)));
		}

		public static Angle GetYaw(IPoint2D p, IPoint2D left, IPoint2D right)
		{
			return Math.Abs(FromPoints(p, left) - FromPoints(p, right));
		}

		public static bool InLOS(IPoint2D p1, IPoint2D target, Direction d, Angle yaw, double distance)
		{
			return GetLOS(p1, d, yaw, distance).Contains(target);
		}

		public static Triangle2D GetLOS(IPoint2D p, Direction d, Angle yaw, double distance)
		{
			var a = FromDirection(d);

			var p2 = GetPoint2D(p.X, p.Y, a - yaw, distance);
			var p3 = GetPoint2D(p.X, p.Y, a + yaw, distance);

			return new Triangle2D(p, p2, p3);
		}

		public static IEnumerable<Point2D> PlotLOS(IPoint2D p, Direction d, Angle yaw, double distance)
		{
			var a = FromDirection(d);

			var p2 = GetPoint2D(p.X, p.Y, a - yaw, distance);
			var p3 = GetPoint2D(p.X, p.Y, a + yaw, distance);

			return Triangle2D.Plot(p, p2, p3);
		}

		public static void Transform(ref Point3D p, Angle angle, double offset)
		{
			int x = p.X, y = p.Y, z = p.Z;

			Transform(ref x, ref y, angle, offset);

			p = new Point3D(x, y, z);
		}

		public static void Transform(ref Point2D p, Angle angle, double offset)
		{
			int x = p.X, y = p.Y;

			Transform(ref x, ref y, angle, offset);

			p = new Point2D(x, y);
		}

		public static void Transform(ref int x, ref int y, Angle angle, double offset)
		{
			x += (int)(offset * Math.Cos(angle._Radians));
			y += (int)(offset * Math.Sin(angle._Radians));
		}

		public static Point2D GetPoint2D(int x, int y, Angle angle, double distance)
		{
			return new Point2D(x + (int)(distance * Math.Cos(angle._Radians)), y + (int)(distance * Math.Sin(angle._Radians)));
		}

		public static Point3D GetPoint3D(int x, int y, int z, Angle angle, double distance)
		{
			return new Point3D(
				x + (int)(distance * Math.Cos(angle._Radians)),
				y + (int)(distance * Math.Sin(angle._Radians)),
				z);
		}

		public static bool TryParse(string value, out Angle angle)
		{
			try
			{
				angle = Parse(value);
				return true;
			}
			catch
			{
				angle = Zero;
				return false;
			}
		}

		public static Angle Parse(string value)
		{
			value = value ?? String.Empty;
			value = value.Trim();

			int d;
			double r;

			if (!value.Contains(","))
			{
				if (Int32.TryParse(value, out d))
				{
					return d;
				}

				if (Double.TryParse(value, out r))
				{
					return r;
				}
			}
			else
			{
				value = value.Trim('(', ')', ' ');

				var i = value.IndexOf(',');

				if (Int32.TryParse(value.Substring(0, i).Trim(), out d))
				{
					return d;
				}

				if (Double.TryParse(value.Substring(i + 1).Trim(), out r))
				{
					return r;
				}
			}

			throw new FormatException(
				"The specified angle must be represented by Int32 (Degrees) or Double (Radians) using the format " + //
				"'###', '#.##', or '(###, #.##)'");
		}

		private readonly int _Degrees;
		private readonly double _Radians;

		[CommandProperty(AccessLevel.Counselor)]
		public int Degrees => _Degrees;

		[CommandProperty(AccessLevel.Counselor)]
		public double Radians => _Radians;

		public Angle(Angle angle)
		{
			_Degrees = angle._Degrees;
			_Radians = angle._Radians;
		}

		public Angle(int degrees)
		{
			_Degrees = degrees;
			_Radians = _Degrees * D2R;
		}

		public Angle(double radians)
			: this((int)(radians * R2D))
		{ }

		public Angle(int x1, int y1, int x2, int y2)
			: this(Math.Atan2(y2, x2) - Math.Atan2(y1, x1))
		{ }

		public Angle(IPoint2D p1, IPoint2D p2)
			: this(p1.X, p1.Y, p2.X, p2.Y)
		{ }

		public void Transform(ref Point3D p, double offset)
		{
			Transform(ref p, this, offset);
		}

		public void Transform(ref Point2D p, double offset)
		{
			Transform(ref p, this, offset);
		}

		public void Transform(ref int x, ref int y, double offset)
		{
			Transform(ref x, ref y, this, offset);
		}

		public Point2D GetPoint2D(int x, int y, double distance)
		{
			return GetPoint2D(x, y, this, distance);
		}

		public Point3D GetPoint3D(int x, int y, int z, double distance)
		{
			return GetPoint3D(x, y, z, this, distance);
		}

		public override int GetHashCode()
		{
			return _Degrees;
		}

		public override bool Equals(object obj)
		{
			return (obj is Angle && Equals((Angle)obj)) || (obj is int && Equals((int)obj)) ||
				   (obj is double && Equals((double)obj));
		}

		public bool Equals(Angle angle)
		{
			return _Degrees == angle._Degrees;
		}

		public bool Equals(int degrees)
		{
			return _Degrees == degrees;
		}

		public bool Equals(double radians)
		{
			return _Radians == radians;
		}

		public int CompareTo(Angle angle)
		{
			return _Degrees.CompareTo(angle._Degrees);
		}

		public int CompareTo(int degrees)
		{
			return _Degrees.CompareTo(degrees);
		}

		public int CompareTo(double radians)
		{
			return _Radians.CompareTo(radians);
		}

		public override string ToString()
		{
			return String.Format("({0}, {1})", _Degrees, _Radians);
		}

		public Angle Normalize()
		{
			return _Degrees % 360;
		}

		#region Operators
		public static Angle operator --(Angle a)
		{
			return a._Degrees - 1;
		}

		public static Angle operator ++(Angle a)
		{
			return a._Degrees + 1;
		}

		public static implicit operator int(Angle a)
		{
			return a._Degrees;
		}

		public static implicit operator double(Angle a)
		{
			return a._Radians;
		}

		public static implicit operator Angle(int d)
		{
			return new Angle(d);
		}

		public static implicit operator Angle(double r)
		{
			return new Angle(r);
		}
		#endregion Operators
	}
}