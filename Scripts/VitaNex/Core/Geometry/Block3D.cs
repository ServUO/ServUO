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
using System.Collections;
using System.Collections.Generic;

using Server.Targeting;
#endregion

namespace Server
{
	public interface IBlock3D : IPoint3D, IEquatable<IBlock3D>, IEnumerable<IPoint3D>
	{
		int H { get; set; }

		bool Intersects(Item e);
		bool Intersects(Mobile e);
		bool Intersects(IEntity e);
		bool Intersects(IPoint3D p);
		bool Intersects(IBlock3D b);
		bool Intersects(int z);
		bool Intersects(int x, int y, int z);
		bool Intersects(int x, int y, int z, int h);

		IEnumerable<IPoint3D> Scan();
	}

	public struct Block3D : IBlock3D
	{
		public static readonly Block3D Empty = new Block3D(0, 0, 0, 0);

		public static bool Intersects(IPoint3D a, IPoint3D b)
		{
			return Create(a).Intersects(Create(b));
		}

		public static Block3D Create(IPoint3D o)
		{
			if (o is Mobile)
			{
				return new Block3D(o, 18);
			}

			if (o is Item)
			{
				return new Block3D(o, Math.Max(1, ((Item)o).ItemData.CalcHeight));
			}

			if (o is LandTarget)
			{
				return new Block3D(o, 1);
			}

			if (o is StaticTarget)
			{
				return new Block3D(o, TileData.ItemTable[((StaticTarget)o).ItemID].CalcHeight);
			}

			return new Block3D(o, 5);
		}

		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		public int H { get; set; }

		public Block3D(IBlock3D b)
			: this(b.X, b.Y, b.Z, b.H)
		{ }

		public Block3D(IPoint3D p, int h)
			: this(p.X, p.Y, p.Z, h)
		{ }

		public Block3D(int x, int y, int z, int h)
			: this()
		{
			X = x;
			Y = y;
			Z = z;
			H = h;
		}

		public bool Intersects(IEntity e)
		{
			if (e is Item)
			{
				return Intersects((Item)e);
			}

			if (e is Mobile)
			{
				return Intersects((Mobile)e);
			}

			return Intersects(e.X, e.Y, e.Z);
		}

		public bool Intersects(IPoint3D p)
		{
			if (p is Item)
			{
				return Intersects((Item)p);
			}

			if (p is Mobile)
			{
				return Intersects((Mobile)p);
			}

			if (p is LandTarget)
			{
				return Intersects((LandTarget)p);
			}

			if (p is StaticTarget)
			{
				return Intersects((StaticTarget)p);
			}

			return Intersects(p.X, p.Y, p.Z);
		}

		public bool Intersects(Point3D p, int h)
		{
			return Intersects(p.X, p.Y, p.Z, h);
		}

		public bool Intersects(LandTarget o)
		{
			return Intersects(o.X, o.Y, o.Z, 1);
		}

		public bool Intersects(StaticTarget o)
		{
			return Intersects(o.X, o.Y, o.Z, Math.Max(1, TileData.ItemTable[o.ItemID].CalcHeight));
		}

		public bool Intersects(Mobile m)
		{
			return Intersects(m.X, m.Y, m.Z, 18);
		}

		public bool Intersects(Item i)
		{
			return Intersects(i.X, i.Y, i.Z, Math.Max(1, i.ItemData.CalcHeight));
		}

		public bool Intersects(Block3D b)
		{
			return Intersects(b.X, b.Y, b.Z, b.H);
		}

		public bool Intersects(IBlock3D b)
		{
			return Intersects(b.X, b.Y, b.Z, b.H);
		}

		public bool Intersects(int z)
		{
			return Intersects(X, Y, z);
		}

		public bool Intersects(int z, int h)
		{
			return Intersects(X, Y, z, h);
		}

		public bool Intersects(int x, int y, int z)
		{
			return Intersects(x, y, z, 0);
		}

		public bool Intersects(Rectangle2D b, int z, int h)
		{
			return X >= b.Start.X && X < b.End.X && Y >= b.Start.Y && Y < b.End.Y && Intersects(X, Y, z, h);
		}

		public bool Intersects(Rectangle3D b)
		{
			var z = Math.Min(b.Start.Z, b.End.Z);
			var h = Math.Abs(b.Depth);

			return X >= b.Start.X && X < b.End.X && Y >= b.Start.Y && Y < b.End.Y && Intersects(X, Y, z, h);
		}

		public bool Intersects(int x, int y, int z, int h)
		{
			if (x != X || y != Y)
			{
				return false;
			}

			if (z == Z || z + h == Z + H)
			{
				return true;
			}

			if (z >= Z && z <= Z + H)
			{
				return true;
			}

			if (Z >= z && Z <= z + h)
			{
				return true;
			}

			if (z <= Z && z + h >= Z)
			{
				return true;
			}

			if (Z <= z && Z + H >= z)
			{
				return true;
			}

			return false;
		}

		public Block3D Offset(int x = 0, int y = 0, int z = 0, int h = 0)
		{
			X += x;
			Y += y;
			Z += z;
			H += h;

			return this;
		}

		public Block3D Delta(int x = 0, int y = 0, int z = 0, int h = 0)
		{
			X -= x;
			Y -= y;
			Z -= z;
			H -= h;

			return this;
		}

		public Block3D Normalize(int x = 0, int y = 0, int z = 0, int h = 0)
		{
			X = x - X;
			Y = y - Y;
			Z = z - Z;
			H = h - H;

			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<IPoint3D> GetEnumerator()
		{
			return Scan().GetEnumerator();
		}

		public IEnumerable<IPoint3D> Scan()
		{
			for (var z = Z; z <= Z + H; z++)
			{
				yield return this.ToPoint3D(z);
			}
		}

		public override string ToString()
		{
			return String.Format("{0}, {1}, {2}, {3}", X, Y, Z, H);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 0;
				hash = (hash * 397) ^ X;
				hash = (hash * 397) ^ Y;
				hash = (hash * 397) ^ Z;
				hash = (hash * 397) ^ H;
				return hash;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is IBlock3D && Equals((IBlock3D)obj);
		}

		public bool Equals(IBlock3D b)
		{
			return !ReferenceEquals(b, null) && X == b.X && Y == b.Y && Z == b.Z && H == b.H;
		}

		public static bool operator ==(Block3D a, IBlock3D b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Block3D a, IBlock3D b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(IBlock3D a, Block3D b)
		{
			return b.Equals(a);
		}

		public static bool operator !=(IBlock3D a, Block3D b)
		{
			return !b.Equals(a);
		}

		public static implicit operator Block3D(Point3D p)
		{
			return new Block3D(p, 0);
		}

		public static implicit operator Point3D(Block3D p)
		{
			return new Point3D(p.X, p.Y, p.Z);
		}
	}
}