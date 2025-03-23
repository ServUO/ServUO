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

#if ServUO58
#define ServUOX
#endif

#region References
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server
{
	public static class Rectangle3DExtUtility
	{
		public static Point3D GetRandomPoint(this Rectangle3D bounds)
		{
			return new Point3D(
				Utility.RandomMinMax(bounds.Start.X, bounds.End.X),
				Utility.RandomMinMax(bounds.Start.Y, bounds.End.Y),
				Utility.RandomMinMax(bounds.Start.Z, bounds.End.Z));
		}

		public static Point2D GetRandomPoint2D(this Rectangle3D bounds)
		{
			return new Point2D(
				Utility.RandomMinMax(bounds.Start.X, bounds.End.X),
				Utility.RandomMinMax(bounds.Start.Y, bounds.End.Y));
		}

		public static Rectangle2D Combine2D(this IEnumerable<Rectangle3D> bounds)
		{
			int count = 0, minX = Int32.MaxValue, minY = Int32.MaxValue, maxX = Int32.MinValue, maxY = Int32.MinValue;

			foreach (var r in bounds)
			{
				minX = Math.Min(minX, Math.Min(r.Start.X, r.End.X));
				minY = Math.Min(minY, Math.Min(r.Start.Y, r.End.Y));

				maxX = Math.Max(maxX, Math.Max(r.Start.X, r.End.X));
				maxY = Math.Max(maxY, Math.Max(r.Start.Y, r.End.Y));

				++count;
			}

			if (count > 0)
			{
				return new Rectangle2D(new Point2D(minX, minY), new Point2D(maxX, maxY));
			}

			return new Rectangle2D(0, 0, 0, 0);
		}

		public static Rectangle3D Combine(this IEnumerable<Rectangle3D> bounds)
		{
			int count = 0,
				minX = Int32.MaxValue,
				minY = Int32.MaxValue,
				maxX = Int32.MinValue,
				maxY = Int32.MinValue,
				minZ = Region.MaxZ,
				maxZ = Region.MinZ;

			foreach (var r in bounds)
			{
				minX = Math.Min(minX, Math.Min(r.Start.X, r.End.X));
				minY = Math.Min(minY, Math.Min(r.Start.Y, r.End.Y));
				minZ = Math.Min(minZ, Math.Min(r.Start.Z, r.End.Z));

				maxX = Math.Max(maxX, Math.Max(r.Start.X, r.End.X));
				maxY = Math.Max(maxY, Math.Max(r.Start.Y, r.End.Y));
				maxZ = Math.Max(maxZ, Math.Max(r.Start.Z, r.End.Z));

				++count;
			}

			if (count > 0)
			{
				return new Rectangle3D(new Point3D(minX, minY, minZ), new Point3D(maxX, maxY, maxZ));
			}

			return new Rectangle3D(0, 0, 0, 0, 0, 0);
		}

		public static IEnumerable<Rectangle3D> ZFix(this IEnumerable<Rectangle3D> rects)
		{
			return ZFix(rects, Region.MinZ, Region.MaxZ);
		}

		public static IEnumerable<Rectangle3D> ZFix(this IEnumerable<Rectangle3D> rects, int zMin, int zMax)
		{
			if (rects == null)
			{
				yield break;
			}

			foreach (var r in rects.Select(r => r.ZFix(zMin, zMax)))
			{
				yield return r;
			}
		}

		public static Rectangle3D ZFix(this Rectangle3D rect)
		{
			return ZFix(rect, Region.MinZ, Region.MaxZ);
		}

		public static Rectangle3D ZFix(this Rectangle3D rect, int zMin, int zMax)
		{
			return new Rectangle3D(rect.Start.ToPoint3D(Math.Min(zMin, zMax)), rect.End.ToPoint3D(Math.Max(zMin, zMax)));
		}

		public static Rectangle2D ToRectangle2D(this Rectangle3D r)
		{
			return new Rectangle2D(r.Start, r.End);
		}

		public static int GetBoundsHashCode(this Rectangle3D r)
		{
			unchecked
			{
				var hash = r.Width * r.Height * r.Depth;

				hash = (hash * 397) ^ r.Start.GetHashCode();
				hash = (hash * 397) ^ r.End.GetHashCode();

				return hash;
			}
		}

		public static int GetBoundsHashCode(this IEnumerable<Rectangle3D> list)
		{
			unchecked
			{
				return list.Aggregate(0, (hash, r) => (hash * 397) ^ GetBoundsHashCode(r));
			}
		}

#if !ServUOX
		public static bool Contains(this RegionRect[] rects, IPoint3D p)
		{
			return rects.Any(rect => Contains(rect.Rect, p.X, p.Y, p.Z));
		}

		public static bool Contains(this RegionRect[] rects, Point3D p)
		{
			return rects.Any(rect => Contains(rect.Rect, p.X, p.Y, p.Z));
		}
#endif

		public static bool Contains(this Rectangle3D[] rects, IPoint3D p)
		{
			return rects.Any(rect => Contains(rect, p.X, p.Y, p.Z));
		}

		public static bool Contains(this Rectangle3D[] rects, Point3D p)
		{
			return rects.Any(rect => Contains(rect, p.X, p.Y, p.Z));
		}

		public static bool Contains(this Rectangle3D[] rects, IPoint2D p)
		{
			return rects.Any(rect => Contains(rect, p.X, p.Y));
		}

		public static bool Contains(this Rectangle3D[] rects, Point2D p)
		{
			return rects.Any(rect => Contains(rect, p.X, p.Y));
		}

#if !ServUOX
		public static bool Contains(this List<RegionRect> rects, IPoint3D p)
		{
			return rects.Any(rect => Contains(rect.Rect, p.X, p.Y, p.Z));
		}

		public static bool Contains(this List<RegionRect> rects, Point3D p)
		{
			return rects.Any(rect => Contains(rect.Rect, p.X, p.Y, p.Z));
		}
#endif

		public static bool Contains(this List<Rectangle3D> rects, IPoint3D p)
		{
			return rects.Any(rect => Contains(rect, p.X, p.Y, p.Z));
		}

		public static bool Contains(this List<Rectangle3D> rects, Point3D p)
		{
			return rects.Any(rect => Contains(rect, p.X, p.Y, p.Z));
		}

		public static bool Contains(this List<Rectangle3D> rects, IPoint2D p)
		{
			return rects.Any(rect => Contains(rect, p.X, p.Y));
		}

		public static bool Contains(this List<Rectangle3D> rects, Point2D p)
		{
			return rects.Any(rect => Contains(rect, p.X, p.Y));
		}

		public static bool Contains(this Rectangle3D rect, IPoint2D p)
		{
			return Contains(rect, p.X, p.Y);
		}

		public static bool Contains(this Rectangle3D rect, IPoint3D p)
		{
			return Contains(rect, p.X, p.Y, p.Z);
		}

		public static bool Contains(this Rectangle3D rect, int x, int y)
		{
			return x >= rect.Start.X && y >= rect.Start.Y && x < rect.End.X && y < rect.End.Y;
		}

		public static bool Contains(this Rectangle3D rect, int x, int y, int z)
		{
			return Contains(rect, x, y) && z >= rect.Start.Z && z < rect.End.Z;
		}

		public static Point2D GetCenter2D(this Rectangle3D r)
		{
			return r.Start.Clone2D(r.Width / 2, r.Height / 2);
		}

		public static Point3D GetCenter(this Rectangle3D r)
		{
			return r.Start.Clone3D(r.Width / 2, r.Height / 2);
		}

		public static int GetArea(this Rectangle3D r)
		{
			return r.Width * r.Height;
		}

		public static int GetVolume(this Rectangle3D r)
		{
			return r.Width * r.Height * r.Depth;
		}

		public static Rectangle2D Resize2D(
			this Rectangle3D r,
			int xOff = 0,
			int yOff = 0,
			int zOff = 0,
			int wOff = 0,
			int hOff = 0,
			int dOff = 0)
		{
			return ToRectangle2D(r).Resize(xOff, yOff, wOff, hOff);
		}

		public static Rectangle3D Resize(
			this Rectangle3D r,
			int xOff = 0,
			int yOff = 0,
			int zOff = 0,
			int wOff = 0,
			int hOff = 0,
			int dOff = 0)
		{
			var s = r.Start;

			return new Rectangle3D(s.X + xOff, s.Y + yOff, s.Z + zOff, r.Width + wOff, r.Height + hOff, r.Depth + dOff);
		}

		public static IEnumerable<Rectangle2D> Slice2D(this Rectangle3D rect, int w, int h)
		{
			return ToRectangle2D(rect).Slice(w, h);
		}

		public static IEnumerable<Rectangle3D> Slice(this Rectangle3D rect, int w, int h)
		{
			if (rect.Width <= w && rect.Height <= h)
			{
				yield return rect;
				yield break;
			}

			int x, y, z = Math.Min(rect.Start.Z, rect.End.Z);
			int ow, oh, od = rect.Depth;

			x = rect.Start.X;

			while (x < rect.End.X)
			{
				ow = Math.Min(w, rect.End.X - x);

				y = rect.Start.Y;

				while (y < rect.End.Y)
				{
					oh = Math.Min(h, rect.End.Y - y);

					yield return new Rectangle3D(x, y, z, ow, oh, od);

					y += oh;
				}

				x += ow;
			}
		}

		public static IEnumerable<T> FindObjects<T>(this Rectangle3D r, Map m)
		{
			if (m == null || m == Map.Internal)
			{
				yield break;
			}

			var o = m.GetObjectsInBounds(r.ToRectangle2D());

			foreach (var e in o.OfType<T>())
			{
				yield return e;
			}

			o.Free();
		}

		public static IEnumerable<TEntity> FindEntities<TEntity>(this Rectangle3D r, Map m)
			where TEntity : IEntity
		{
			if (m == null || m == Map.Internal)
			{
				yield break;
			}

			var o = m.GetObjectsInBounds(r.ToRectangle2D());

			foreach (var e in o.OfType<TEntity>().Where(e => e.Map == m && r.Contains(e)))
			{
				yield return e;
			}

			o.Free();
		}

		public static IEnumerable<IEntity> FindEntities(this Rectangle3D r, Map m)
		{
			return FindEntities<IEntity>(r, m);
		}

		public static List<TEntity> GetEntities<TEntity>(this Rectangle3D r, Map m)
			where TEntity : IEntity
		{
			return FindEntities<TEntity>(r, m).ToList();
		}

		public static List<IEntity> GetEntities(this Rectangle3D r, Map m)
		{
			return FindEntities<IEntity>(r, m).ToList();
		}

		public static List<Item> GetItems(this Rectangle3D r, Map m)
		{
			return FindEntities<Item>(r, m).ToList();
		}

		public static List<Mobile> GetMobiles(this Rectangle3D r, Map m)
		{
			return FindEntities<Mobile>(r, m).ToList();
		}

		public static IEnumerable<Point2D> EnumeratePoints2D(this Rectangle3D r)
		{
			for (var x = r.Start.X; x <= r.End.X; x++)
			{
				for (var y = r.Start.Y; y <= r.End.Y; y++)
				{
					yield return new Point2D(x, y);
				}
			}
		}

		public static IEnumerable<Point3D> EnumeratePoints(this Rectangle3D r)
		{
			if (r.Depth > 10)
			{
				Utility.PushColor(ConsoleColor.Yellow);
				"> Warning!".ToConsole();
				"> Rectangle3DExtUtility.EnumeratePoints() called on Rectangle3D with depth exceeding 10;".ToConsole();
				"> This may cause serious performance issues.".ToConsole();
				Utility.PopColor();
			}

			for (var z = r.Start.Z; z <= r.End.Z; z++)
			{
				for (var x = r.Start.X; x <= r.End.X; x++)
				{
					for (var y = r.Start.Y; y <= r.End.Y; y++)
					{
						yield return new Point3D(x, y, z);
					}
				}
			}
		}

		public static void ForEach2D(this Rectangle3D r, Action<Point2D> action)
		{
			if (action == null)
			{
				return;
			}

			foreach (var p in EnumeratePoints2D(r))
			{
				action(p);
			}
		}

		public static void ForEach(this Rectangle3D r, Action<Point3D> action)
		{
			if (action == null)
			{
				return;
			}

			foreach (var p in EnumeratePoints(r))
			{
				action(p);
			}
		}

		public static IEnumerable<Point2D> GetBorder2D(this Rectangle3D r, int size)
		{
			size = Math.Max(1, size);

			int x, y;
			int x1 = r.Start.X + size, y1 = r.Start.Y + size;
			int x2 = r.End.X - size, y2 = r.End.Y - size;

			for (x = r.Start.X; x <= r.End.X; x++)
			{
				if (x >= x1 || x <= x2)
				{
					continue;
				}

				for (y = r.Start.Y; y <= r.End.Y; y++)
				{
					if (y >= y1 || y <= y2)
					{
						continue;
					}

					yield return new Point2D(x, y);
				}
			}
		}

		public static IEnumerable<Point2D> GetBorder2D(this Rectangle3D r)
		{
			return GetBorder2D(r, 1);
		}

		public static IEnumerable<Point3D> GetBorder(this Rectangle3D r, int size)
		{
			if (r.Depth > 10)
			{
				Utility.PushColor(ConsoleColor.Yellow);
				"> Warning!".ToConsole();
				"> Rectangle3DExtUtility.EnumeratePoints() called on Rectangle3D with depth exceeding 10;".ToConsole();
				"> This may cause serious performance issues.".ToConsole();
				Utility.PopColor();
			}

			size = Math.Max(1, size);

			int x, y;
			int x1 = r.Start.X + size, y1 = r.Start.Y + size, z1 = r.Start.Z + size;
			int x2 = r.End.X - size, y2 = r.End.Y - size, z2 = r.End.Z - size;

			for (var z = r.Start.Z; z <= r.End.Z; z++)
			{
				if (z >= z1 || z <= z2)
				{
					continue;
				}

				for (x = r.Start.X; x <= r.End.X; x++)
				{
					if (x >= x1 || x <= x2)
					{
						continue;
					}

					for (y = r.Start.Y; y <= r.End.Y; y++)
					{
						if (y >= y1 || y <= y2)
						{
							continue;
						}

						yield return new Point3D(x, y, z);
					}
				}
			}
		}

		public static IEnumerable<Point3D> GetBorder(this Rectangle3D r)
		{
			return GetBorder(r, 1);
		}

		public static bool Intersects2D(this Rectangle3D r, Rectangle3D or)
		{
			return GetBorder2D(r).Any(GetBorder2D(or).Contains);
		}

		public static bool Intersects(this Rectangle3D r, Rectangle3D or)
		{
			var minZL = Math.Min(r.Start.Z, r.End.Z);
			var maxZL = Math.Max(r.Start.Z, r.End.Z);

			var minZR = Math.Min(or.Start.Z, or.End.Z);
			var maxZR = Math.Max(or.Start.Z, or.End.Z);

			if (minZL > maxZR)
			{
				return false;
			}

			if (maxZL < minZR)
			{
				return false;
			}

			return GetBorder2D(r).Union(GetBorder2D(or)).GroupBy(p => p).Any(g => g.Count() > 1);
		}
	}
}
