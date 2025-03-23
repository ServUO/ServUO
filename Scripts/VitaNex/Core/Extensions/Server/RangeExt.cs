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
	public class ScanRangeResult : IDisposable
	{
		/// <summary>
		///     Gets a value representing the center IPoint3D of the current range function query.
		/// </summary>
		public IPoint3D QueryCenter { get; protected set; }

		/// <summary>
		///     Gets a value representing the current Map used by the current range function query.
		/// </summary>
		public Map QueryMap { get; protected set; }

		/// <summary>
		///     Gets a value representing the min range of the current range function query.
		/// </summary>
		public int QueryRangeMin { get; protected set; }

		/// <summary>
		///     Gets a value representing the min range of the current range function query.
		/// </summary>
		public int QueryRangeMax { get; protected set; }

		/// <summary>
		///     Gets a value representing the current distance from 'QueryCenter'.
		/// </summary>
		public int Distance { get; protected set; }

		/// <summary>
		///     Gets a value representing the current location Point3D of the current range function query.
		/// </summary>
		public Point3D Current { get; protected set; }

		/// <summary>
		///     Gets a value representing if the current ScanRangeResult instance should be excluded in the current range function
		///     query.
		/// </summary>
		public bool Excluded { get; protected set; }

		public ScanRangeResult(IPoint3D center, Map map, Point3D current, int distance, int range)
			: this(center, map, current, distance, 0, range)
		{ }

		public ScanRangeResult(IPoint3D center, Map map, Point3D current, int distance, int minRange, int maxRange)
		{
			QueryCenter = center;
			QueryMap = map;
			Distance = Math.Max(0, distance);
			Current = current;
			QueryRangeMin = Math.Min(minRange, maxRange);
			QueryRangeMax = Math.Max(minRange, maxRange);
			Excluded = false;
		}

		~ScanRangeResult()
		{
			Dispose();
		}

		/// <summary>
		///     Excludes the current ScanRangeResult instance from the current range function query.
		/// </summary>
		public void Exclude()
		{
			Excluded = true;
		}

		public void Dispose()
		{ }
	}

	public static class RangeExtUtility
	{
		/// <summary>
		///     Iterates through a Point3D collection representing all locations within 'range' range of 'center' on the given
		///     'map'.
		/// </summary>
		public static void ScanRange(
			this IPoint2D center,
			Map map,
			int range,
			Predicate<ScanRangeResult> handler,
			bool avgZ = true)
		{
			ScanRange(center.ToPoint3D(), map, range, handler, avgZ);
		}

		/// <summary>
		///     Iterates through a Point3D collection representing all locations within 'range' range of 'center' on the given
		///     'map'.
		/// </summary>
		public static void ScanRange(
			this IPoint3D center,
			Map map,
			int range,
			Predicate<ScanRangeResult> handler,
			bool avgZ = true)
		{
			if (center == null || map == null || map == Map.Internal || handler == null)
			{
				return;
			}

			range = Math.Abs(range);

			var die = false;

			for (var x = -range; x <= range; x++)
			{
				for (var y = -range; y <= range; y++)
				{
					var distance = (int)Math.Sqrt(x * x + y * y);

					if (distance <= range)
					{
						var p = center.Clone3D(x, y);

						p.Z = avgZ ? p.GetAverageZ(map) : center.Z;

						die = handler(new ScanRangeResult(center, map, p, distance, range));
					}

					if (die)
					{
						break;
					}
				}

				if (die)
				{
					break;
				}
			}
		}

		/// <summary>
		///     Iterates through a Point3D collection representing all locations within 'min' and 'max' range of 'center' on the
		///     given 'map'.
		/// </summary>
		public static void ScanRange(
			this IPoint2D center,
			Map map,
			int min,
			int max,
			Predicate<ScanRangeResult> handler,
			bool avgZ = true)
		{
			ScanRange(center.ToPoint3D(), map, min, max, handler, avgZ);
		}

		/// <summary>
		///     Iterates through a Point3D collection representing all locations within 'min' and 'max' range of 'center' on the
		///     given 'map'.
		/// </summary>
		public static void ScanRange(
			this IPoint3D center,
			Map map,
			int min,
			int max,
			Predicate<ScanRangeResult> handler,
			bool avgZ = true)
		{
			if (map == null || map == Map.Internal)
			{
				return;
			}

			var ml = Math.Abs(Math.Min(min, max));
			var mr = Math.Abs(Math.Max(min, max));

			min = ml;
			max = mr;

			var die = false;

			for (var x = -max; x <= max; x++)
			{
				for (var y = -max; y <= max; y++)
				{
					var distance = (int)Math.Sqrt(x * x + y * y);

					if (distance >= min && distance <= max)
					{
						var p = center.Clone3D(x, y);

						p.Z = avgZ ? p.GetAverageZ(map) : center.Z;

						die = handler(new ScanRangeResult(center, map, p, distance, min, max));
					}

					if (die)
					{
						break;
					}
				}

				if (die)
				{
					break;
				}
			}
		}

		/// <summary>
		///     Gets a Point3D collection representing all locations within 'min' and 'max' range of 'center' on the given 'map'.
		///     The first dimension represents the distance from center, the second dimension is the collection of points at that
		///     distance.
		/// </summary>
		public static Point3D[][] ScanRangeGet(
			this IPoint2D center,
			Map map,
			int range,
			Predicate<ScanRangeResult> handler = null,
			bool avgZ = true)
		{
			return ScanRangeGet(center.ToPoint3D(), map, range, handler, avgZ);
		}

		/// <summary>
		///     Gets a Point3D collection representing all locations within 'range' of 'center' on the given 'map'.
		///     The first dimension represents the distance from center, the second dimension is the collection of points at that
		///     distance.
		/// </summary>
		public static Point3D[][] ScanRangeGet(
			this IPoint3D center,
			Map map,
			int range,
			Predicate<ScanRangeResult> handler = null,
			bool avgZ = true)
		{
			if (center == null || map == null || map == Map.Internal)
			{
				return new Point3D[0][];
			}

			var points = new List<Point3D>[range + 1];

			points.SetAll(
				i =>
				{
					var oc = Math.PI * Math.Sqrt(i);
					var nc = Math.PI * Math.Sqrt(i + 1);

					return new List<Point3D>((int)Math.Ceiling(nc - oc));
				});

			ScanRange(
				center,
				map,
				range,
				result =>
				{
					var die = false;

					if (handler != null)
					{
						die = handler(result);
					}

					if (!result.Excluded)
					{
						points[result.Distance].Add(result.Current);
					}

					return die;
				},
				avgZ);

			return points.FreeToMultiArray(true);
		}

		/// <summary>
		///     Gets a Point3D collection representing all locations within 'min' and 'max' range of 'center' on the given 'map'.
		///     The first dimension represents the distance from center, the second dimension is the collection of points at that
		///     distance.
		/// </summary>
		public static Point3D[][] ScanRangeGet(
			this IPoint2D center,
			Map map,
			int min,
			int max,
			Predicate<ScanRangeResult> handler = null,
			bool avgZ = true)
		{
			return ScanRangeGet(center.ToPoint3D(), map, min, max, handler, avgZ);
		}

		/// <summary>
		///     Gets a Point3D collection representing all locations within 'min' and 'max' range of 'center' on the given 'map'.
		///     The first dimension represents the distance from center, the second dimension is the collection of points at that
		///     distance.
		/// </summary>
		public static Point3D[][] ScanRangeGet(
			this IPoint3D center,
			Map map,
			int min,
			int max,
			Predicate<ScanRangeResult> handler = null,
			bool avgZ = true)
		{
			if (center == null || map == null || map == Map.Internal)
			{
				return new Point3D[0][];
			}

			var points = new List<Point3D>[max + 1];

			points.SetAll(
				i =>
				{
					var oc = Math.PI * Math.Sqrt(i);
					var nc = Math.PI * Math.Sqrt(i + 1);

					return new List<Point3D>((int)Math.Ceiling(nc - oc));
				});

			ScanRange(
				center,
				map,
				min,
				max,
				result =>
				{
					var die = false;

					if (handler != null)
					{
						die = handler(result);
					}

					if (!result.Excluded)
					{
						points[result.Distance].Add(result.Current);
					}

					return die;
				},
				avgZ);

			return points.FreeToMultiArray(true);
		}

		/// <summary>
		///     Determines if the given 'target' is within 'range' of 'source'.
		/// </summary>
		public static bool InRange2D(this IPoint2D source, IPoint2D target, int range)
		{
			if (source == null || target == null)
			{
				return false;
			}

			if (source == target)
			{
				return true;
			}

			range = Math.Abs(range);

			var x = source.X - target.X;
			var y = source.Y - target.Y;

			return Math.Sqrt(x * x + y * y) <= range;
		}

		/// <summary>
		///     Determines if the given 'target' is within 'range' of 'source', taking Z-axis into consideration.
		///     'floor' is used as a lower offset for the current Z-axis being checked.
		///     'roof' is used as an upper offset for the current Z-axis being checked.
		/// </summary>
		public static bool InRange3D(this IPoint3D source, IPoint3D target, int range, int floor, int roof)
		{
			if (source == null || target == null)
			{
				return false;
			}

			if (source == target)
			{
				return true;
			}

			var f = Math.Min(floor, roof);
			var r = Math.Max(floor, roof);

			return InRange2D(source, target, range) && target.Z >= source.Z + f && target.Z <= source.Z + r;
		}

		/// <summary>
		///     Gets a BaseMulti collection representing all BaseMultis that are within 'range' of 'center' on the given 'map'
		/// </summary>
		public static List<BaseMulti> GetMultisInRange(this IPoint2D center, Map map, int range)
		{
			return GetMultisInRange(center.ToPoint3D(), map, range);
		}

		/// <summary>
		///     Gets a BaseMulti collection representing all BaseMultis that are within 'range' of 'center' on the given 'map'
		/// </summary>
		public static List<BaseMulti> GetMultisInRange(this IPoint3D center, Map map, int range)
		{
			return FindMultisInRange(center, map, range).ToList();
		}

		/// <summary>
		///     Gets a BaseMulti collection representing all BaseMultis that are within 'range' of 'center' on the given 'map'
		/// </summary>
		public static IEnumerable<BaseMulti> FindMultisInRange(this IPoint2D center, Map map, int range)
		{
			return FindMultisInRange(center.ToPoint3D(), map, range);
		}

		/// <summary>
		///     Gets a BaseMulti collection representing all BaseMultis that are within 'range' of 'center' on the given 'map'
		/// </summary>
		public static IEnumerable<BaseMulti> FindMultisInRange(this IPoint3D center, Map map, int range)
		{
			if (center == null || map == null)
			{
				yield break;
			}

			var scan = map.GetSector(center).Multis.Where(v => v != null && (v.Contains(center) || v.InRange2D(center, range)));

			foreach (var m in scan)
			{
				yield return m;
			}
		}

		/// <summary>
		///     Gets a collection of all Items that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<BaseMulti> FindMultisAt(this IPoint2D point, Map map)
		{
			return FindMultisInRange(point, map, 0);
		}

		/// <summary>
		///     Gets a collection of all Items that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<BaseMulti> FindMultisAt(this IPoint3D point, Map map)
		{
			return FindMultisInRange(point, map, 0);
		}

		/// <summary>
		///     Gets a StaticTile collection representing all StaticTiles that are within 'range' of 'center' on the given 'map'
		/// </summary>
		public static List<StaticTile> GetStaticTilesInRange(this IPoint3D center, Map map, int range)
		{
			if (center == null || map == null)
			{
				return new List<StaticTile>();
			}

			range = Math.Abs(range);

			var tiles = new List<StaticTile>((int)Math.Ceiling(Math.PI * Math.Sqrt(range)));

			ScanRange(
				center,
				map,
				range,
				r =>
				{
					if (!r.Excluded)
					{
						tiles.AddRange(map.GetStaticTiles(r.Current));
					}

					return false;
				},
				false);

			tiles.Free(false);

			return tiles;
		}

		/// <summary>
		///     Gets a LandTile collection representing all LandTiles that are within 'range' of 'center' on the given 'map'
		/// </summary>
		public static List<LandTile> GetLandTilesInRange(this IPoint3D center, Map map, int range)
		{
			if (center == null || map == null)
			{
				return new List<LandTile>();
			}

			range = Math.Abs(range);

			var tiles = new List<LandTile>((int)Math.Ceiling(Math.PI * Math.Sqrt(range)));

			ScanRange(
				center,
				map,
				range,
				r =>
				{
					if (!r.Excluded)
					{
						tiles.Add(map.GetLandTile(r.Current));
					}

					return false;
				},
				false);

			tiles.Free(false);

			return tiles;
		}

		/// <summary>
		///     Gets an ISpawner collection representing all ISpawners that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<ISpawner> GetSpawnersInRange(this IPoint3D center, Map map, int range)
		{
			return FindSpawnersInRange(center, map, range).ToList();
		}

		/// <summary>
		///     Gets an ISpawner collection representing all ISpawners that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<ISpawner> FindSpawnersInRange(this IPoint3D center, Map map, int range)
		{
			if (center == null || map == null || map == Map.Internal)
			{
				yield break;
			}

			range = Math.Abs(range);

			var ipe = map.GetObjectsInRange(center.ToPoint3D(), range);

			foreach (var s in ipe.OfType<ISpawner>())
			{
				yield return s;
			}

			ipe.Free();
		}

		/// <summary>
		///     Gets a collection of all objects of the given Type 'T' that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<T> GetEntitiesInRange<T>(this IPoint2D center, Map map, int range)
			where T : IEntity
		{
			return GetEntitiesInRange<T>(center.ToPoint3D(), map, range);
		}

		/// <summary>
		///     Gets a collection of all objects of the given Type 'T' that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<T> GetEntitiesInRange<T>(this IPoint3D center, Map map, int range)
			where T : IEntity
		{
			return FindEntitiesInRange<T>(center, map, range).ToList();
		}

		/// <summary>
		///     Gets a collection of all objects of the given Type 'T' that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<T> FindEntitiesInRange<T>(this IPoint2D center, Map map, int range)
			where T : IEntity
		{
			return FindEntitiesInRange<T>(center.ToPoint3D(), map, range);
		}

		/// <summary>
		///     Gets a collection of all objects of the given Type 'T' that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<T> FindEntitiesInRange<T>(this IPoint3D center, Map map, int range)
			where T : IEntity
		{
			if (center == null || map == null || map == Map.Internal)
			{
				yield break;
			}

			range = Math.Abs(range);

			var ipe = map.GetObjectsInRange(center.ToPoint3D(), range);

			foreach (var s in ipe.OfType<T>())
			{
				yield return s;
			}

			ipe.Free();
		}

		/// <summary>
		///     Gets a collection of all objects of the given Type 'T' that are at the given 'point' on the given 'map'.
		/// </summary>
		public static List<T> GetEntitiesAt<T>(this IPoint2D point, Map map)
			where T : IEntity
		{
			return GetEntitiesInRange<T>(point, map, 0);
		}

		/// <summary>
		///     Gets a collection of all objects of the given Type 'T' that are at the given 'point' on the given 'map'.
		/// </summary>
		public static List<T> GetEntitiesAt<T>(this IPoint3D point, Map map)
			where T : IEntity
		{
			return GetEntitiesInRange<T>(point, map, 0);
		}

		/// <summary>
		///     Gets a collection of all objects of the given Type 'T' that are at the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<T> FindEntitiesAt<T>(this IPoint2D point, Map map)
			where T : IEntity
		{
			return FindEntitiesInRange<T>(point, map, 0);
		}

		/// <summary>
		///     Gets a collection of all objects of the given Type 'T' that are at the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<T> FindEntitiesAt<T>(this IPoint3D point, Map map)
			where T : IEntity
		{
			return FindEntitiesInRange<T>(point, map, 0);
		}

		#region IEntity
		/// <summary>
		///     Gets a collection of all IEntity that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<IEntity> GetEntitiesInRange(this IPoint2D center, Map map, int range)
		{
			return GetEntitiesInRange<IEntity>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all IEntity that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<IEntity> GetEntitiesInRange(this IPoint3D center, Map map, int range)
		{
			return GetEntitiesInRange<IEntity>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all IEntity that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<IEntity> FindEntitiesInRange(this IPoint2D center, Map map, int range)
		{
			return FindEntitiesInRange<IEntity>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all IEntity that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<IEntity> FindEntitiesInRange(this IPoint3D center, Map map, int range)
		{
			return FindEntitiesInRange<IEntity>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all IEntity that are at the given 'point' on the given 'map'.
		/// </summary>
		public static List<IEntity> GetEntitiesAt(this IPoint2D point, Map map)
		{
			return GetEntitiesAt<IEntity>(point, map);
		}

		/// <summary>
		///     Gets a collection of all IEntity that are at the given 'point' on the given 'map'.
		/// </summary>
		public static List<IEntity> GetEntitiesAt(this IPoint3D point, Map map)
		{
			return GetEntitiesAt<IEntity>(point, map);
		}

		/// <summary>
		///     Gets a collection of all IEntity that are at the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<IEntity> FindEntitiesAt(this IPoint2D point, Map map)
		{
			return FindEntitiesAt<IEntity>(point, map);
		}

		/// <summary>
		///     Gets a collection of all IEntity that are at the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<IEntity> FindEntitiesAt(this IPoint3D point, Map map)
		{
			return FindEntitiesAt<IEntity>(point, map);
		}
		#endregion IEntity

		#region Mobile
		/// <summary>
		///     Gets a collection of all Mobiles that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<Mobile> GetMobilesInRange(this IPoint2D center, Map map, int range)
		{
			return GetEntitiesInRange<Mobile>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all Mobiles that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<Mobile> GetMobilesInRange(this IPoint3D center, Map map, int range)
		{
			return GetEntitiesInRange<Mobile>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all Mobiles that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<Mobile> FindMobilesInRange(this IPoint2D center, Map map, int range)
		{
			return FindEntitiesInRange<Mobile>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all Mobiles that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<Mobile> FindMobilesInRange(this IPoint3D center, Map map, int range)
		{
			return FindEntitiesInRange<Mobile>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all Mobiles that are at the given 'point' on the given 'map'.
		/// </summary>
		public static List<Mobile> GetMobilesAt(this IPoint2D point, Map map)
		{
			return GetEntitiesAt<Mobile>(point, map);
		}

		/// <summary>
		///     Gets a collection of all Mobiles that are at the given 'point' on the given 'map'.
		/// </summary>
		public static List<Mobile> GetMobilesAt(this IPoint3D point, Map map)
		{
			return GetEntitiesAt<Mobile>(point, map);
		}

		/// <summary>
		///     Gets a collection of all Mobiles that are at the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<Mobile> FindMobilesAt(this IPoint2D point, Map map)
		{
			return FindEntitiesAt<Mobile>(point, map);
		}

		/// <summary>
		///     Gets a collection of all Mobiles that are at the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<Mobile> FindMobilesAt(this IPoint3D point, Map map)
		{
			return FindEntitiesAt<Mobile>(point, map);
		}
		#endregion Mobile

		#region BaseVendor
		/// <summary>
		///     Gets a collection of all BaseVendors that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<BaseVendor> GetVendorsInRange(this IPoint2D center, Map map, int range)
		{
			return GetEntitiesInRange<BaseVendor>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all BaseVendors that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<BaseVendor> GetVendorsInRange(this IPoint3D center, Map map, int range)
		{
			return GetEntitiesInRange<BaseVendor>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all BaseVendors that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<BaseVendor> FindVendorsInRange(this IPoint2D center, Map map, int range)
		{
			return FindEntitiesInRange<BaseVendor>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all BaseVendors that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<BaseVendor> FindVendorsInRange(this IPoint3D center, Map map, int range)
		{
			return FindEntitiesInRange<BaseVendor>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all BaseVendors that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static List<BaseVendor> GetVendorsAt(this IPoint2D point, Map map)
		{
			return GetEntitiesAt<BaseVendor>(point, map);
		}

		/// <summary>
		///     Gets a collection of all BaseVendors that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static List<BaseVendor> GetVendorsAt(this IPoint3D point, Map map)
		{
			return GetEntitiesAt<BaseVendor>(point, map);
		}

		/// <summary>
		///     Gets a collection of all BaseVendors that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<BaseVendor> FindVendorsAt(this IPoint2D point, Map map)
		{
			return FindEntitiesAt<BaseVendor>(point, map);
		}

		/// <summary>
		///     Gets a collection of all BaseVendors that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<BaseVendor> FindVendorsAt(this IPoint3D point, Map map)
		{
			return FindEntitiesAt<BaseVendor>(point, map);
		}
		#endregion BaseVendor

		#region PlayerMobile
		/// <summary>
		///     Gets a collection of all PlayerMobiles that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<PlayerMobile> GetPlayersInRange(this IPoint2D center, Map map, int range)
		{
			return GetEntitiesInRange<PlayerMobile>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all PlayerMobiles that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<PlayerMobile> GetPlayersInRange(this IPoint3D center, Map map, int range)
		{
			return GetEntitiesInRange<PlayerMobile>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all PlayerMobiles that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<PlayerMobile> FindPlayersInRange(this IPoint2D center, Map map, int range)
		{
			return FindEntitiesInRange<PlayerMobile>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all PlayerMobiles that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<PlayerMobile> FindPlayersInRange(this IPoint3D center, Map map, int range)
		{
			return FindEntitiesInRange<PlayerMobile>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all PlayerMobiles that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static List<PlayerMobile> GetPlayersAt(this IPoint2D point, Map map)
		{
			return GetEntitiesAt<PlayerMobile>(point, map);
		}

		/// <summary>
		///     Gets a collection of all PlayerMobiles that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static List<PlayerMobile> GetPlayersAt(this IPoint3D point, Map map)
		{
			return GetEntitiesAt<PlayerMobile>(point, map);
		}

		/// <summary>
		///     Gets a collection of all PlayerMobiles that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<PlayerMobile> FindPlayersAt(this IPoint2D point, Map map)
		{
			return FindEntitiesAt<PlayerMobile>(point, map);
		}

		/// <summary>
		///     Gets a collection of all PlayerMobiles that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<PlayerMobile> FindPlayersAt(this IPoint3D point, Map map)
		{
			return FindEntitiesAt<PlayerMobile>(point, map);
		}
		#endregion PlayerMobile

		#region BaseCreature
		/// <summary>
		///     Gets a collection of all BaseCreatures that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<BaseCreature> GetCreaturesInRange(this IPoint2D center, Map map, int range)
		{
			return GetEntitiesInRange<BaseCreature>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all BaseCreatures that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<BaseCreature> GetCreaturesInRange(this IPoint3D center, Map map, int range)
		{
			return GetEntitiesInRange<BaseCreature>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all BaseCreatures that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<BaseCreature> FindCreaturesInRange(this IPoint2D center, Map map, int range)
		{
			return FindEntitiesInRange<BaseCreature>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all BaseCreatures that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<BaseCreature> FindCreaturesInRange(this IPoint3D center, Map map, int range)
		{
			return FindEntitiesInRange<BaseCreature>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all BaseCreatures that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static List<BaseCreature> GetCreaturesAt(this IPoint2D point, Map map)
		{
			return GetEntitiesAt<BaseCreature>(point, map);
		}

		/// <summary>
		///     Gets a collection of all BaseCreatures that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static List<BaseCreature> GetCreaturesAt(this IPoint3D point, Map map)
		{
			return GetEntitiesAt<BaseCreature>(point, map);
		}

		/// <summary>
		///     Gets a collection of all BaseCreatures that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<BaseCreature> FindCreaturesAt(this IPoint2D point, Map map)
		{
			return FindEntitiesAt<BaseCreature>(point, map);
		}

		/// <summary>
		///     Gets a collection of all BaseCreatures that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<BaseCreature> FindCreaturesAt(this IPoint3D point, Map map)
		{
			return FindEntitiesAt<BaseCreature>(point, map);
		}
		#endregion BaseCreature

		#region Item
		/// <summary>
		///     Gets a collection of all Items that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<Item> GetItemsInRange(this IPoint2D center, Map map, int range)
		{
			return GetEntitiesInRange<Item>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all Items that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static List<Item> GetItemsInRange(this IPoint3D center, Map map, int range)
		{
			return GetEntitiesInRange<Item>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all Items that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<Item> FindItemsInRange(this IPoint2D center, Map map, int range)
		{
			return FindEntitiesInRange<Item>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all Items that are within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static IEnumerable<Item> FindItemsInRange(this IPoint3D center, Map map, int range)
		{
			return FindEntitiesInRange<Item>(center, map, range);
		}

		/// <summary>
		///     Gets a collection of all Items that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static List<Item> GetItemsAt(this IPoint2D point, Map map)
		{
			return GetEntitiesAt<Item>(point, map);
		}

		/// <summary>
		///     Gets a collection of all Items that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static List<Item> GetItemsAt(this IPoint3D point, Map map)
		{
			return GetEntitiesAt<Item>(point, map);
		}

		/// <summary>
		///     Gets a collection of all Items that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<Item> FindItemsAt(this IPoint2D point, Map map)
		{
			return FindEntitiesAt<Item>(point, map);
		}

		/// <summary>
		///     Gets a collection of all Items that are atat the given 'point' on the given 'map'.
		/// </summary>
		public static IEnumerable<Item> FindItemsAt(this IPoint3D point, Map map)
		{
			return FindEntitiesAt<Item>(point, map);
		}
		#endregion Item

		/// <summary>
		///     Gets a Point3D collection representing all locations within 'min' and 'max' range of 'center' on the given 'map'.
		/// </summary>
		public static Point3D[] GetAllPointsInRange(this IPoint2D center, Map map, int range, bool avgZ = true)
		{
			return GetAllPointsInRange(center.ToPoint3D(), map, range, avgZ);
		}

		/// <summary>
		///     Gets a Point3D collection representing all locations within 'range' of 'center' on the given 'map'.
		/// </summary>
		public static Point3D[] GetAllPointsInRange(this IPoint3D center, Map map, int range, bool avgZ = true)
		{
			if (map == null || map == Map.Internal)
			{
				return new Point3D[0];
			}

			range = Math.Max(0, range);

			var points = new List<Point3D>((int)Math.Ceiling(Math.PI * Math.Sqrt(range)));

			ScanRange(
				center,
				map,
				range,
				result =>
				{
					if (!result.Excluded)
					{
						points.Add(result.Current);
					}

					return false;
				},
				avgZ);

			return points.FreeToArray(true);
		}

		/// <summary>
		///     Gets a Point3D collection representing all locations within 'min' and 'max' range of 'center' on the given 'map'.
		/// </summary>
		public static Point3D[] GetAllPointsInRange(this IPoint2D center, Map map, int min, int max, bool avgZ = true)
		{
			return GetAllPointsInRange(center.ToPoint3D(), map, min, max, avgZ);
		}

		/// <summary>
		///     Gets a Point3D collection representing all locations within 'min' and 'max' range of 'center' on the given 'map'.
		/// </summary>
		public static Point3D[] GetAllPointsInRange(this IPoint3D center, Map map, int min, int max, bool avgZ = true)
		{
			if (map == null || map == Map.Internal)
			{
				return new Point3D[0];
			}

			var ml = Math.Abs(Math.Min(min, max));
			var mr = Math.Abs(Math.Max(min, max));

			min = ml;
			max = mr;

			var oc = Math.PI * Math.Sqrt(min);
			var nc = Math.PI * Math.Sqrt(max);

			var points = new List<Point3D>((int)Math.Ceiling(nc - oc));

			ScanRange(
				center,
				map,
				min,
				max,
				result =>
				{
					if (!result.Excluded)
					{
						points.Add(result.Current);
					}

					return false;
				},
				avgZ);

			return points.FreeToArray(true);
		}

		public static Point2D GetRandomPoint2D(this IPoint2D start, int range)
		{
			return GetRandomPoint2D(start, 0, range);
		}

		public static Point2D GetRandomPoint2D(this IPoint2D start, int minRange, int maxRange)
		{
			var angle = Utility.RandomDouble() * Math.PI * 2;
			var radius = minRange + (Math.Sqrt(Utility.RandomDouble()) * (maxRange - minRange));

			var x = (int)(radius * Math.Cos(angle));
			var y = (int)(radius * Math.Sin(angle));

			return start.Clone2D(x, y);
		}

		public static Point3D GetRandomPoint3D(this IPoint3D start, int range)
		{
			return GetRandomPoint3D(start, 0, range, null, false, false);
		}

		public static Point3D GetRandomPoint3D(this IPoint3D start, int range, Map map)
		{
			return GetRandomPoint3D(start, 0, range, map, false, false);
		}

		public static Point3D GetRandomPoint3D(this IPoint3D start, int range, Map map, bool checkLOS, bool checkSpawn)
		{
			return GetRandomPoint3D(start, 0, range, map, checkLOS, checkSpawn);
		}

		public static Point3D GetRandomPoint3D(this IPoint3D start, int minRange, int maxRange)
		{
			return GetRandomPoint3D(start, minRange, maxRange, null, false, false);
		}

		public static Point3D GetRandomPoint3D(this IPoint3D start, int minRange, int maxRange, Map map)
		{
			return GetRandomPoint3D(start, minRange, maxRange, map, false, false);
		}

		public static Point3D GetRandomPoint3D(
			this IPoint3D start,
			int minRange,
			int maxRange,
			Map map,
			bool checkLOS,
			bool checkSpawn)
		{
			if ((map == null || map == Map.Internal) && start is IEntity)
			{
				map = ((IEntity)start).Map;
			}

			double a, r;
			int x, y;
			Point3D s, p;

			var c = 30;

			do
			{
				a = Utility.RandomDouble() * Math.PI * 2;
				r = minRange + (Math.Sqrt(Utility.RandomDouble()) * (maxRange - minRange));

				x = (int)(r * Math.Cos(a));
				y = (int)(r * Math.Sin(a));

				s = start.Clone3D(0, 0, 16);
				p = start.Clone3D(x, y);

				if (map != null)
				{
					p = p.GetSurfaceTop(map);
				}

				if (map == null || ((!checkLOS || map.LineOfSight(s, p)) && (!checkSpawn || map.CanSpawnMobile(p))))
				{
					break;
				}
			}
			while (--c >= 0);

			if (c >= 0)
			{
				return p;
			}

			if (map != null)
			{
				return start.GetSurfaceTop(map);
			}

			return start.ToPoint3D();
		}
	}
}