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
#endregion

namespace Server
{
	public interface IMapPoint : IPoint3D
	{
		bool Internal { get; }
		Map Map { get; set; }
		Point3D Location { get; set; }
	}

	[PropertyObject]
	public sealed class MapPoint
		: IMapPoint, IEquatable<MapPoint>, IEquatable<IMapPoint>, IEquatable<Map>, IEquatable<IPoint3D>, IEquatable<IPoint2D>
	{
		public static MapPoint Empty => new MapPoint(Map.Internal, Point3D.Zero);

		private Map _Map = Map.Internal;
		private Point3D _Location = Point3D.Zero;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Map Map { get => _Map ?? (_Map = Map.Internal); set => _Map = value ?? Map.Internal; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Point3D Location { get => _Location; set => _Location = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int X { get => _Location.X; set => _Location = new Point3D(Math.Max(0, value), Y, Z); }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Y { get => _Location.Y; set => _Location = new Point3D(X, Math.Max(0, value), Z); }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Z
		{
			get => _Location.Z;
			set => _Location = new Point3D(X, Y, Math.Max(-128, Math.Min(128, value)));
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool Internal => _Map == null || _Map == Map.Internal;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool Zero => _Location == Point3D.Zero;

		public bool InternalOrZero => Internal || Zero;

		public MapPoint(Map map, IPoint2D p, int z)
			: this(map, p.X, p.Y, z)
		{ }

		public MapPoint(Map map, IPoint3D p)
			: this(map, p.X, p.Y, p.Z)
		{ }

		public MapPoint(Map map, int x, int y, int z)
		{
			_Map = map ?? Map.Internal;
			_Location = new Point3D(x, y, z);
		}

		public MapPoint(GenericReader reader)
		{
			Deserialize(reader);
		}

		public bool MoveToWorld(Mobile m)
		{
			if (InternalOrZero)
			{
				return false;
			}

			m.MoveToWorld(this, Map);
			return true;
		}

		public bool MoveToWorld(Item i)
		{
			if (InternalOrZero)
			{
				return false;
			}

			i.MoveToWorld(this, Map);
			return true;
		}

		public override string ToString()
		{
			return String.Format("{0} ({1}, {2}, {3})", Map.Name, X, Y, Z);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = Map == null ? -1 : Map.MapID ^ Map.MapIndex;

				hash = (hash * 397) ^ X;
				hash = (hash * 397) ^ Y;
				hash = (hash * 397) ^ Z;

				return hash;
			}
		}

		public bool Equals(MapPoint other)
		{
			return !ReferenceEquals(other, null) && Map == other.Map && Location == other.Location;
		}

		public bool Equals(IMapPoint other)
		{
			return !ReferenceEquals(other, null) && Map == other.Map && Location == other.Location;
		}

		public bool Equals(Map other)
		{
			return !ReferenceEquals(other, null) && Map == other;
		}

		public bool Equals(IPoint3D other)
		{
			return !ReferenceEquals(other, null) && X == other.X && Y == other.Y && Z == other.Z;
		}

		public bool Equals(IPoint2D other)
		{
			return !ReferenceEquals(other, null) && X == other.X && Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
			{
				return false;
			}

			if (obj is MapPoint)
			{
				return Equals((MapPoint)obj);
			}

			if (obj is Map)
			{
				return Equals((Map)obj);
			}

			if (obj is IMapPoint)
			{
				return Equals((IMapPoint)obj);
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
			writer.Write(Location);
			writer.Write(Map);
		}

		public void Deserialize(GenericReader reader)
		{
			Location = reader.ReadPoint3D();
			Map = reader.ReadMap();
		}

		#region Conversion Operators
		public static implicit operator MapPoint(Mobile m)
		{
			return m == null ? Empty : new MapPoint(m.Map, m.Location);
		}

		public static implicit operator MapPoint(Item i)
		{
			return i == null ? Empty : new MapPoint(i.Map, i.Location);
		}

		public static implicit operator MapPoint(Entity e)
		{
			return e == null ? Empty : new MapPoint(e.Map, e.Location);
		}

		public static implicit operator Point3D(MapPoint m)
		{
			return ReferenceEquals(m, null) ? Point3D.Zero : m.Location;
		}

		public static implicit operator Map(MapPoint m)
		{
			return ReferenceEquals(m, null) ? Map.Internal : m.Map;
		}
		#endregion Conversion Operators

		#region MapPoint Operators
		public static bool operator ==(MapPoint l, MapPoint r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(MapPoint l, MapPoint r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}

		public static bool operator ==(MapPoint l, IMapPoint r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(MapPoint l, IMapPoint r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}

		public static bool operator ==(IMapPoint l, MapPoint r)
		{
			return ReferenceEquals(r, null) ? ReferenceEquals(l, null) : r.Equals(l);
		}

		public static bool operator !=(IMapPoint l, MapPoint r)
		{
			return ReferenceEquals(r, null) ? !ReferenceEquals(l, null) : !r.Equals(l);
		}

		/*
		public static bool operator >(MapPoint l, MapPoint r)
		{
			if (ReferenceEquals(l, null) || ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X > r.X && l.Y > r.Y && l.Z > r.Z;
		}

		public static bool operator >(MapPoint l, IMapPoint r)
		{
			if (ReferenceEquals(l, null) || ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X > r.X && l.Y > r.Y && l.Z > r.Z;
		}

		public static bool operator <(MapPoint l, MapPoint r)
		{
			if (ReferenceEquals(l, null) || ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X < r.X && l.Y < r.Y && l.Z < r.Z;
		}

		public static bool operator <(MapPoint l, IMapPoint r)
		{
			if (ReferenceEquals(l, null) || ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X < r.X && l.Y < r.Y && l.Z < r.Z;
		}

		public static bool operator >=(MapPoint l, MapPoint r)
		{
			if (ReferenceEquals(l, null))
			{
				return ReferenceEquals(r, null);
			}

			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X >= r.X && l.Y >= r.Y && l.Z >= r.Z;
		}

		public static bool operator >=(MapPoint l, IMapPoint r)
		{
			if (ReferenceEquals(l, null))
			{
				return ReferenceEquals(r, null);
			}

			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X >= r.X && l.Y >= r.Y && l.Z >= r.Z;
		}

		public static bool operator <=(MapPoint l, MapPoint r)
		{
			if (ReferenceEquals(l, null))
			{
				return ReferenceEquals(r, null);
			}

			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X <= r.X && l.Y <= r.Y && l.Z <= r.Z;
		}

		public static bool operator <=(MapPoint l, IMapPoint r)
		{
			if (ReferenceEquals(l, null))
			{
				return ReferenceEquals(r, null);
			}

			if (ReferenceEquals(r, null))
			{
				return false;
			}

			return l.X <= r.X && l.Y <= r.Y && l.Z <= r.Z;
		}
		*/
		#endregion MapPoint Operators
	}
}