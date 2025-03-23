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

using Server.Items;
#endregion

namespace Server
{
	public interface ICoords : IPoint2D
	{
		int Long { get; }
		int Lat { get; }

		bool East { get; }
		bool South { get; }
	}

	[PropertyObject]
	public sealed class Coords : ICoords, IComparable, IComparable<Coords>, IEquatable<Coords>
	{
		public static Coords Zero => new Coords(Map.Internal, 0, 0);

		private bool _East;
		private int _Lat;
		private int _LatMins;
		private int _Long;
		private int _LongMins;

		private Map _Map;
		private Point2D _Point;
		private bool _South;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Map Map
		{
			get => _Map ?? (_Map = Map.Internal);
			set
			{
				_Map = value ?? Map.Internal;
				Compute();
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int LongMins => _LongMins;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int LatMins => _LatMins;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int X
		{
			get => _Point.X;
			set
			{
				_Point.X = value;
				Compute();
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Y
		{
			get => _Point.Y;
			set
			{
				_Point.Y = value;
				Compute();
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Long => _Long;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Lat => _Lat;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool East => _East;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool South => _South;

		public Coords(Map map, IPoint2D p)
			: this(map, p.X, p.Y)
		{ }

		public Coords(Map map, int x, int y)
		{
			_Map = map;
			_Point = new Point2D(x, y);

			Compute();
		}

		public Coords(GenericReader reader)
		{
			Deserialize(reader);
		}

		public void Compute()
		{
			if (!Sextant.Format(
				_Point.ToPoint3D(),
				_Map,
				ref _Long,
				ref _Lat,
				ref _LongMins,
				ref _LatMins,
				ref _East,
				ref _South))
			{
				Clear();
			}
		}

		public void Clear()
		{
			_Map = Map.Internal;
			_Point = Point2D.Zero;
			_Long = _LongMins = _Lat = _LatMins = 0;
			_East = _South = false;
		}

		public Point2D ToPoint2D()
		{
			return new Point2D(_Point);
		}

		public Point3D ToPoint3D()
		{
			return new Point3D(_Point, (_Map != null && _Map != Map.Internal) ? _Map.GetAverageZ(_Point.X, _Point.Y) : 0);
		}

		public override string ToString()
		{
			return String.Format(
				"{0}° {1}'{2}, {3}° {4}'{5}",
				_Lat,
				_LatMins,
				_South ? "S" : "N",
				_Long,
				_LongMins,
				_East ? "E" : "W");
		}

		public int CompareTo(object other)
		{
			return other == null || !(other is Coords) ? 0 : CompareTo((Coords)other);
		}

		public int CompareTo(Coords other)
		{
			return _Point.CompareTo(other._Point);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_Point.GetHashCode() * 397) ^ _Map.MapIndex;
			}
		}

		public bool Equals(Coords other)
		{
			return _Point == other._Point && _Map == other._Map;
		}

		public bool Equals(ICoords other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			return _Long == other.Long && _Lat == other.Lat && _South == other.South && _East == other.East;
		}

		public bool Equals(Map other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			return _Map == other;
		}

		public bool Equals(IPoint3D other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			return _Point.X == other.X && _Point.Y == other.Y;
		}

		public bool Equals(IPoint2D other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			return _Point.X == other.X && _Point.Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (obj is Coords)
			{
				return Equals((Coords)obj);
			}

			if (obj is Map)
			{
				return Equals((Map)obj);
			}

			if (obj is ICoords)
			{
				return Equals((ICoords)obj);
			}

			if (obj is IPoint3D)
			{
				return Equals((IPoint3D)obj);
			}

			if (obj is IPoint2D)
			{
				return Equals((IPoint2D)obj);
			}

			return false;
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(_Map);
			writer.Write(_Point);
		}

		public void Deserialize(GenericReader reader)
		{
			_Map = reader.ReadMap();
			_Point = reader.ReadPoint2D();

			Compute();
		}

#region Conversion Operators
		public static implicit operator Coords(Mobile m)
		{
			return m == null ? Zero : new Coords(m.Map, m.Location);
		}

		public static implicit operator Coords(Item i)
		{
			return i == null ? Zero : new Coords(i.Map, i.Location);
		}

		public static implicit operator Coords(Entity e)
		{
			return e == null ? Zero : new Coords(e.Map, e.Location);
		}

		public static implicit operator Point2D(Coords c)
		{
			return ReferenceEquals(c, null) ? Point2D.Zero : new Point2D(c.X, c.Y);
		}

		public static implicit operator Point3D(Coords c)
		{
			return ReferenceEquals(c, null) ? Point3D.Zero : new Point3D(c.X, c.Y, 0);
		}

		public static implicit operator Map(Coords c)
		{
			return ReferenceEquals(c, null) ? Map.Internal : c.Map;
		}
#endregion Conversion Operators

#region Point2D Operators
		public static bool operator ==(Coords l, Point2D r)
		{
			if (ReferenceEquals(l, null))
			{
				return false;
			}

			return l.Equals(r);
		}

		public static bool operator !=(Coords l, Point2D r)
		{
			if (ReferenceEquals(l, null))
			{
				return true;
			}

			return !l.Equals(r);
		}

		public static bool operator >(Coords l, Point2D r)
		{
			if (ReferenceEquals(l, null))
			{
				return false;
			}

			return l._Point > r;
		}

		public static bool operator <(Coords l, Point2D r)
		{
			if (ReferenceEquals(l, null))
			{
				return false;
			}

			return l._Point < r;
		}

		public static bool operator >=(Coords l, Point2D r)
		{
			if (ReferenceEquals(l, null))
			{
				return false;
			}

			return l._Point >= r;
		}

		public static bool operator <=(Coords l, Point2D r)
		{
			if (ReferenceEquals(l, null))
			{
				return false;
			}

			return l._Point <= r;
		}

		public static bool operator ==(Point2D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return r.Equals(l);
		}

		public static bool operator !=(Point2D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return !r.Equals(l);
		}

		public static bool operator >(Point2D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l > r._Point;
		}

		public static bool operator <(Point2D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l < r._Point;
		}

		public static bool operator >=(Point2D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l >= r._Point;
		}

		public static bool operator <=(Point2D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l <= r._Point;
		}
#endregion Point2D Operators

#region Point3D Operators
		public static bool operator ==(Coords l, Point3D r)
		{
			if (ReferenceEquals(l, null))
			{
				return false;
			}

			return l.Equals(r);
		}

		public static bool operator !=(Coords l, Point3D r)
		{
			if (ReferenceEquals(l, null))
			{
				return true;
			}

			return !l.Equals(r);
		}

		public static bool operator >(Coords l, Point3D r)
		{
			if (ReferenceEquals(l, null))
			{
				return false;
			}

			return l._Point > r;
		}

		public static bool operator <(Coords l, Point3D r)
		{
			if (ReferenceEquals(l, null))
			{
				return false;
			}

			return l._Point < r;
		}

		public static bool operator >=(Coords l, Point3D r)
		{
			if (ReferenceEquals(l, null))
			{
				return false;
			}

			return l._Point >= r;
		}

		public static bool operator <=(Coords l, Point3D r)
		{
			if (ReferenceEquals(l, null))
			{
				return false;
			}

			return l._Point <= r;
		}

		public static bool operator ==(Point3D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return r.Equals(l);
		}

		public static bool operator !=(Point3D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return !r.Equals(l);
		}

		public static bool operator >(Point3D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return r._Point <= l;
		}

		public static bool operator <(Point3D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return r._Point >= l;
		}

		public static bool operator >=(Point3D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return r._Point < l;
		}

		public static bool operator <=(Point3D l, Coords r)
		{
			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return r._Point > l;
		}
#endregion Point3D Operators

#region Coords Operators
		public static bool operator ==(Coords l, Coords r)
		{
			if (ReferenceEquals(l, null))
			{
				return ReferenceEquals(r, null);
			}

			return l.Equals(r);
		}

		public static bool operator !=(Coords l, Coords r)
		{
			if (ReferenceEquals(l, null))
			{
				return !ReferenceEquals(r, null);
			}

			return l.Equals(r);
		}

		public static bool operator >(Coords l, Coords r)
		{
			if (ReferenceEquals(l, null) || ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X > r.X && l.Y > r.Y;
		}

		public static bool operator <(Coords l, Coords r)
		{
			if (ReferenceEquals(l, null) || ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X < r.X && l.Y < r.Y;
		}

		public static bool operator >=(Coords l, Coords r)
		{
			if (ReferenceEquals(l, null))
			{
				return ReferenceEquals(r, null);
			}

			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X >= r.X && l.Y >= r.Y;
		}

		public static bool operator <=(Coords l, Coords r)
		{
			if (ReferenceEquals(l, null))
			{
				return ReferenceEquals(r, null);
			}

			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X <= r.X && l.Y <= r.Y;
		}
#endregion Coords Operators
	}
}
