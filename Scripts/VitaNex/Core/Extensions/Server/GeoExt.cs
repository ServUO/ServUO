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

using Server.Items;
using Server.Mobiles;
#endregion

namespace Server
{
	public static class GeoExtUtility
	{
		public static Point3D ToPoint3D(this IPoint3D p)
		{
			return new Point3D(p.X, p.Y, p.Z);
		}

		public static Point3D ToPoint3D(this IPoint2D p, int z = 0)
		{
			return new Point3D(p.X, p.Y, z);
		}

		public static Point2D ToPoint2D(this IPoint3D p)
		{
			return new Point2D(p.X, p.Y);
		}

		public static Point2D ToPoint2D(this StaticTile t)
		{
			return new Point2D(t.X, t.Y);
		}

		public static Point3D ToPoint3D(this StaticTile t)
		{
			return new Point3D(t.X, t.Y, t.Z);
		}

		public static string ToCoordsString(this IPoint2D p, Map map)
		{
			return ToCoords(p, map).ToString();
		}

		public static Coords ToCoords(this IPoint2D p, Map m)
		{
			return new Coords(m, p);
		}

		public static Coords ToCoords(this IEntity e)
		{
			return e == null ? Coords.Zero : new Coords(e.Map, e.Location);
		}

		public static Coords ToCoords(this PlayerMobile m)
		{
			if (m == null)
			{
				return Coords.Zero;
			}

			var online = m.IsOnline();
			return new Coords(online ? m.Map : m.LogoutMap, online ? m.Location : m.LogoutLocation);
		}

		public static Coords ToCoords(this StaticTile t, Map m)
		{
			return new Coords(m, ToPoint3D(t));
		}

		public static MapPoint ToMapPoint(this IPoint2D p, Map m, int z = 0)
		{
			return new MapPoint(m, p, z);
		}

		public static MapPoint ToMapPoint(this IPoint3D p, Map m)
		{
			return new MapPoint(m, p);
		}

		public static MapPoint ToMapPoint(this IEntity e)
		{
			return e == null ? MapPoint.Empty : new MapPoint(e.Map, e.Location);
		}

		public static MapPoint ToMapPoint(this PlayerMobile m)
		{
			if (m == null)
			{
				return MapPoint.Empty;
			}

			var online = m.IsOnline();

			return new MapPoint(online ? m.Map : m.LogoutMap, online ? m.Location : m.LogoutLocation);
		}

		public static MapPoint ToMapPoint(this StaticTile t, Map m)
		{
			return new MapPoint(m, ToPoint3D(t));
		}

		public static IEnumerable<Block3D> Flatten(this IEnumerable<Block3D> blocks)
		{
			foreach (var o in blocks.GroupBy(o => o.ToPoint2D()))
			{
				var v = new Block3D(o.Key.X, o.Key.Y, o.Min(b => b.Z), 0);

				v.H = o.Max(b => b.Z + b.H) - v.Z;

				yield return v;
			}
		}

		public static Direction GetDirection(this IPoint2D from, IPoint2D to)
		{
			int dx = to.X - from.X, dy = to.Y - from.Y, adx = Math.Abs(dx), ady = Math.Abs(dy);

			if (adx >= ady * 3)
			{
				if (dx > 0)
				{
					return Direction.East;
				}

				return Direction.West;
			}

			if (ady >= adx * 3)
			{
				if (dy > 0)
				{
					return Direction.South;
				}

				return Direction.North;
			}

			if (dx > 0)
			{
				if (dy > 0)
				{
					return Direction.Down;
				}

				return Direction.Right;
			}

			if (dy > 0)
			{
				return Direction.Left;
			}

			return Direction.Up;
		}

		public static Direction4 GetDirection4(this Direction dir, bool clockwise = true)
		{
			dir = dir & Direction.Mask;

			switch (dir)
			{
				case Direction.Up:
					return clockwise ? Direction4.North : Direction4.West;
				case Direction.Right:
					return clockwise ? Direction4.East : Direction4.North;
				case Direction.Down:
					return clockwise ? Direction4.South : Direction4.East;
				case Direction.Left:
					return clockwise ? Direction4.West : Direction4.South;
			}

			return (Direction4)dir;
		}

		public static Point2D[] GetLine2D(this IPoint2D start, IPoint2D end)
		{
			return PlotLine2D(start, end).ToArray();
		}

		public static Point3D[] GetLine3D(this IPoint3D start, IPoint3D end, bool avgZ = true)
		{
			Map map = null;

			if (avgZ && start is IEntity)
			{
				map = ((IEntity)start).Map;
			}

			return GetLine3D(start, end, map, avgZ);
		}

		public static Point3D[] GetLine3D(this IPoint3D start, IPoint3D end, Map map, bool avgZ = true)
		{
			return PlotLine3D(start, end, map, avgZ).ToArray();
		}

		public static IEnumerable<Point2D> PlotLine2D(this IPoint2D start, IPoint2D end)
		{
			return Line2D.Plot(start, end);
		}

		public static IEnumerable<Point3D> PlotLine3D(this IPoint3D start, IPoint3D end, bool avgZ = true)
		{
			Map map = null;

			if (avgZ && start is IEntity)
			{
				map = ((IEntity)start).Map;
			}

			return PlotLine3D(start, end, map, avgZ);
		}

		public static IEnumerable<Point3D> PlotLine3D(this IPoint3D start, IPoint3D end, Map map, bool avgZ = true)
		{
			var dist = GetDistance(start, end);
			var dZ = end.Z - start.Z;

			return Line2D.Plot(start, end)
						 .Select(
							 (p, i) =>
							 {
								 var z = start.Z;

								 if (avgZ && map != null)
								 {
									 z = map.GetAverageZ(p.X, p.Y);
								 }
								 else
								 {
									 z += (int)(dZ * (i / dist));
								 }

								 return new Point3D(p, z);
							 });
		}

		public static Point2D Rotate2D(this IPoint2D from, IPoint2D to, int count)
		{
			var rx = from.X - to.X;
			var ry = from.Y - to.Y;

			for (var i = 0; i < count; ++i)
			{
				var temp = rx;
				rx = -ry;
				ry = temp;
			}

			return new Point2D(to.X + rx, to.Y + ry);
		}

		public static Point3D Rotate3D(this IPoint3D from, IPoint3D to, int count)
		{
			return new Point3D(Rotate2D(from, to, count), from.Z);
		}

		public static Point2D Clone2D(this IPoint2D p, IPoint2D t)
		{
			return new Point2D(p.X + t.X, p.Y + t.Y);
		}

		public static Point2D Clone2D(this IPoint2D p, int xOffset = 0, int yOffset = 0)
		{
			return new Point2D(p.X + xOffset, p.Y + yOffset);
		}

		public static Point3D Clone3D(this IPoint3D p, IPoint3D t)
		{
			return new Point3D(p.X + t.X, p.Y + t.Y, p.Z + t.Z);
		}

		public static Point3D Clone3D(this IPoint3D p, int xOffset = 0, int yOffset = 0, int zOffset = 0)
		{
			return new Point3D(p.X + xOffset, p.Y + yOffset, p.Z + zOffset);
		}

		public static Point2D Lerp2D(this IPoint2D start, IPoint2D end, double percent)
		{
			return Lerp2D(start, end.X, end.Y, percent);
		}

		public static Point2D Lerp2D(this IPoint2D start, int x, int y, double percent)
		{
			return Clone2D(start, (int)((x - start.X) * percent), (int)((y - start.Y) * percent));
		}

		public static Point3D Lerp3D(this IPoint3D start, IPoint3D end, double percent)
		{
			return Lerp3D(start, end.X, end.Y, end.Z, percent);
		}

		public static Point3D Lerp3D(this IPoint3D start, int x, int y, int z, double percent)
		{
			return Clone3D(
				start,
				(int)((x - start.X) * percent),
				(int)((y - start.Y) * percent),
				(int)((z - start.Z) * percent));
		}

		public static Point2D Delta2D(this IPoint2D start, IPoint2D end)
		{
			return new Point2D(start.X - end.X, start.Y - end.Y);
		}

		public static Point2D Delta2D(this IPoint2D start, int endX, int endY)
		{
			return new Point2D(start.X - endX, start.Y - endY);
		}

		public static Point3D Delta3D(this IPoint3D start, IPoint3D end)
		{
			return new Point3D(start.X - end.X, start.Y - end.Y, start.Z - end.Z);
		}

		public static Point3D Delta3D(this IPoint3D start, int endX, int endY, int endZ)
		{
			return new Point3D(start.X - endX, start.Y - endY, start.Z - endZ);
		}

		public static double GetDistance(this IPoint2D start, IPoint2D end)
		{
			return Math.Abs(Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2)));
		}

		public static double GetDistance(this IPoint3D start, IPoint3D end)
		{
			return Math.Abs(
				Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2) + (Math.Pow(end.Z - start.Z, 2) / 44.0)));
		}

		public static TimeSpan GetTravelTime(this IPoint2D start, IPoint2D end, double speed)
		{
			var span = GetDistance(start, end) / (speed * 1.25);

			span = Math.Max(0, Math.Min(TimeSpan.MaxValue.TotalSeconds, span));

			return TimeSpan.FromSeconds(span);
		}

		public static TimeSpan GetTravelTime(this IPoint3D start, IPoint3D end, double speed)
		{
			var span = GetDistance(start, end) / (speed * 1.25);

			var z1 = (double)start.Z;
			var z2 = (double)end.Z;

			var zDiff = Math.Abs(z2 - z1);

			if (zDiff >= 5)
			{
				span -= (zDiff / 5.0) * 0.1;
			}

			span = Math.Max(0, Math.Min(TimeSpan.MaxValue.TotalSeconds, span));

			return TimeSpan.FromSeconds(span);
		}

		public static object GetTopSurface(this Map map, Point3D p, bool multis)
		{
			if (map == Map.Internal)
			{
				return null;
			}

			object surface = null;
			var surfaceZ = Int32.MinValue;

			var lt = map.Tiles.GetLandTile(p.X, p.Y);

			if (!lt.Ignored)
			{
				var avgZ = map.GetAverageZ(p.X, p.Y);

				if (avgZ <= p.Z)
				{
					surface = lt;
					surfaceZ = avgZ;

					if (surfaceZ == p.Z)
					{
						return surface;
					}
				}
			}

			var staticTiles = map.Tiles.GetStaticTiles(p.X, p.Y, multis);

			ItemData id;
			int tileZ;

			foreach (var tile in staticTiles)
			{
				id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				if (!id.Surface && (id.Flags & TileFlag.Wet) == 0)
				{
					continue;
				}

				tileZ = tile.Z + id.CalcHeight;

				if (tileZ <= surfaceZ || tileZ > p.Z)
				{
					continue;
				}

				surface = tile;
				surfaceZ = tileZ;

				if (surfaceZ == p.Z)
				{
					return surface;
				}
			}

			var sector = map.GetSector(p.X, p.Y);

			int itemZ;

			var items = sector.Items.Where(
				o => o.ItemID <= TileData.MaxItemValue && !o.Movable && !(o is BaseMulti) && o.AtWorldPoint(p.X, p.Y));

			foreach (var item in items)
			{
				id = item.ItemData;

				if (!id.Surface && (id.Flags & TileFlag.Wet) == 0)
				{
					continue;
				}

				itemZ = item.Z + id.CalcHeight;

				if (itemZ <= surfaceZ || itemZ > p.Z)
				{
					continue;
				}

				surface = item;
				surfaceZ = itemZ;

				if (surfaceZ == p.Z)
				{
					return surface;
				}
			}

			return surface;
		}

		public static Point3D GetSurfaceTop(this IPoint2D p, Map map, bool items = true, bool multis = true)
		{
			if (map == null || map == Map.Internal)
			{
				return ToPoint3D(p);
			}

			return GetSurfaceTop(ToPoint3D(p, Region.MaxZ), map, items, multis);
		}

		public static Point3D GetSurfaceTop(this IPoint3D p, Map map, bool items = true, bool multis = true)
		{
			if (map == null || map == Map.Internal)
			{
				return ToPoint3D(p);
			}

			var o = GetTopSurface(map, ToPoint3D(p, Region.MaxZ), multis);

			if (o != null)
			{
				if (o is LandTile)
				{
					var t = (LandTile)o;
					return ToPoint3D(p, t.Z + t.Height);
				}

				if (o is StaticTile)
				{
					var t = (StaticTile)o;
					return ToPoint3D(p, t.Z + TileData.ItemTable[t.ID].CalcHeight);
				}

				if (o is Item && items)
				{
					var t = (Item)o;
					return ToPoint3D(p, t.Z + t.ItemData.CalcHeight);
				}
			}

			return ToPoint3D(p);
		}

		public static Point3D GetWorldTop(this IPoint2D p, Map map)
		{
			return GetSurfaceTop(p, map, false);
		}

		public static Point3D GetWorldTop(this IPoint3D p, Map map)
		{
			return GetSurfaceTop(p, map, false);
		}

		public static int GetTopZ(this Rectangle2D b, Map map)
		{
			return GetTopZ(map, b.EnumeratePoints());
		}

		public static int GetTopZ(this Rectangle3D b, Map map)
		{
			return GetTopZ(map, b.EnumeratePoints2D());
		}

		public static int GetTopZ(this IPoint2D p, Map map, int range)
		{
			return GetTopZ(new Rectangle2D(p.X - range, p.Y - range, (range * 2) + 1, (range * 2) + 1), map);
		}

		public static int GetTopZ(this Map map, Rectangle2D b)
		{
			return GetTopZ(map, b.EnumeratePoints());
		}

		public static int GetTopZ(this Map map, Rectangle3D b)
		{
			return GetTopZ(map, b.EnumeratePoints2D());
		}

		public static int GetTopZ(this Map map, params Point2D[] points)
		{
			return GetTopZ(map, points.Ensure());
		}

		public static int GetTopZ(this Map map, IEnumerable<Point2D> points)
		{
			try
			{
				return points.Max(p => GetTopZ(p, map));
			}
			catch (InvalidCastException)
			{
				return 0;
			}
		}

		public static int GetTopZ(this IPoint2D p, Map map)
		{

			GetAverageZ(p, map, out var c, out var a, out var t);

			return t;
		}

		public static int GetAverageZ(this IPoint2D p, Map map, int range)
		{
			return GetAverageZ(new Rectangle2D(p.X - range, p.Y - range, (range * 2) + 1, (range * 2) + 1), map);
		}

		public static int GetAverageZ(this Rectangle2D b, Map map)
		{
			return GetAverageZ(map, b.EnumeratePoints());
		}

		public static int GetAverageZ(this Rectangle3D b, Map map)
		{
			return GetAverageZ(map, b.EnumeratePoints2D());
		}

		public static int GetAverageZ(this Map map, Rectangle2D b)
		{
			return GetAverageZ(map, b.EnumeratePoints());
		}

		public static int GetAverageZ(this Map map, Rectangle3D b)
		{
			return GetAverageZ(map, b.EnumeratePoints2D());
		}

		public static int GetAverageZ(this Map map, params Point2D[] points)
		{
			return GetAverageZ(map, points.Ensure());
		}

		public static int GetAverageZ(this Map map, IEnumerable<Point2D> points)
		{
			try
			{
				return (int)points.Average(p => GetAverageZ(p, map));
			}
			catch (InvalidCastException)
			{
				return 0;
			}
		}

		/// <summary>
		///     ((  ,A,_,A,
		///     )) ,{=^;^=}
		///     (( {,,}#{,,}
		///     `{,,}{,,}
		/// </summary>
		public static int GetAverageZ(this IPoint2D p, Map map)
		{

			GetAverageZ(p, map, out var c, out var a, out var t);

			return a;
		}

		public static void GetAverageZ(this IPoint2D p, Map map, out int cur, out int avg, out int top)
		{
			var land = new
			{
				T = map.Tiles.GetLandTile(p.X, p.Y),
				L = map.Tiles.GetLandTile(p.X, p.Y + 1),
				R = map.Tiles.GetLandTile(p.X + 1, p.Y),
				B = map.Tiles.GetLandTile(p.X + 1, p.Y + 1)
			};

			var surf = new
			{
				T = GetSurfaceTop(p, map, false, false),
				L = GetSurfaceTop(Clone2D(p, 0, 1), map, false, false),
				R = GetSurfaceTop(Clone2D(p, 1), map, false, false),
				B = GetSurfaceTop(Clone2D(p, 1, 1), map, false, false)
			};

			var zT = (land.T.Ignored || surf.T.Z > Region.MinZ) ? surf.T.Z : land.T.Z;
			var zL = (land.L.Ignored || surf.L.Z > Region.MinZ) ? surf.L.Z : land.L.Z;
			var zR = (land.R.Ignored || surf.R.Z > Region.MinZ) ? surf.R.Z : land.R.Z;
			var zB = (land.B.Ignored || surf.B.Z > Region.MinZ) ? surf.B.Z : land.B.Z;

			cur = zT;

			if (zL > Region.MinZ && zL < cur)
			{
				cur = zL;
			}

			if (zR > Region.MinZ && zR < cur)
			{
				cur = zR;
			}

			if (zB > Region.MinZ && zB < cur)
			{
				cur = zB;
			}

			top = zT;

			if (zL > Region.MinZ && zL > top)
			{
				top = zL;
			}

			if (zR > Region.MinZ && zR > top)
			{
				top = zR;
			}

			if (zB > Region.MinZ && zB > top)
			{
				top = zB;
			}

			if (cur <= Region.MinZ)
			{
				cur = 0;
			}

			if (top <= Region.MinZ)
			{
				top = 0;
			}

			if (zT <= Region.MinZ)
			{
				zT = 0;
			}

			if (zL <= Region.MinZ)
			{
				zL = 0;
			}

			if (zR <= Region.MinZ)
			{
				zR = 0;
			}

			if (zB <= Region.MinZ)
			{
				zB = 0;
			}

			var vL = zL + zR;

			if (vL < 0)
			{
				--vL;
			}

			var vR = zT + zB;

			if (vR < 0)
			{
				--vR;
			}

			avg = Math.Abs(zT - zB) > Math.Abs(zL - zR) ? vL / 2 : vR / 2;
		}

		public static bool IsInside(this IPoint3D p, Map map)
		{
			if (p != null && map != null && map != Map.Internal)
			{
				return IsInside(p, p.Z, map);
			}

			return false;
		}

		public static bool IsOutside(this IPoint3D p, Map map)
		{
			return !IsInside(p, map);
		}

		public static bool IsInside(this IPoint2D p, int z, Map map)
		{
			if (p == null || map == null || map == Map.Internal)
			{
				return false;
			}

			if (p is Item)
			{
				z += Math.Max(0, ((Item)p).ItemData.Height) + 1;
			}

			z = Math.Max(Region.MinZ, Math.Min(Region.MaxZ, z));

			return !map.CanFit(p.X, p.Y, z, Region.MaxZ - z, true, false, false);
		}

		public static bool IsOutside(this IPoint2D p, int z, Map map)
		{
			return !IsInside(p, z, map);
		}
	}
}