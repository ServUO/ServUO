#region Map Updates
/*
 * Name: Map Updates
 * Date: 20th August, 2015
 * Author: Vorspire
 * Testing: Punkte
 * 
 * Test Methods: Stealth; 250+ clients connected, all moving randomly.
 * Test Results: 35ms average latency (ping) under load.
 * 
 * Notes:
 * Until these updates become main-stream, they will remain differentiated
 * by their preprocessor directives. When they are deemed stable enough,
 * old code can be removed, along with the directives.
 */

/*
 * Map_AllUpdates
 * 
 * When defined, enables all updates listed below regardless of whether their
 * preprocessor directives are defined.
 * 
 * This can be used to compile your server with all of the updates enabled by
 * adding a single preprocessor directive definition to your build solution.
 */
#define Map_AllUpdates

/*
 *	Map_NewEnumerables
 *	
 *	When defined, enables a major update to the IPooledEnumerables factory.
 *	
 *	This update removes the need for enumerator instantiation and replaces 
 *	them with simple, yet powerful Linq queries.
 *	
 *	In addition, the PooledEnumerable class is replaced with a compatible,
 *	single generic class template and takes advantage of the nature of 
 *	nested static context to ensure that a buffer pool is available for 
 *	each type of PooledEnumerable<T> result, where result is T.
 *	This update generally increases performance and reduces overall player
 *	connection latency.
 */
#define Map_NewEnumerables

/*
 * UseMaxRange
 * 
 * When defined, enables a minor update that forces Get*InRange methods to
 * use Core.GlobalMaxUpdateRange, when no range is specified.
 * 
 * By default, a constant range of 18 is used however, Core.GlobalMaxUpdateRange
 * is usually greater than that with a default value of 24.
 * 
 * This update will allow things such as Effects to be displayed to more players,
 * as well as increasing the range of player sight.
 * 
 * The benefits of this update appeal to players who choose to increase the
 * dimensions of their game window beyond the client's limits.
 * (This can also be beneficial for shards that mainly target the Enhanced client)
 */
#define Map_UseMaxRange

/*
 * Map_PoolFixColumn
 * 
 * When defined, enables aminor update that attempts to improve the performance
 * of Item stack fixing.
 * 
 * Item stack fixing is a feature that corrects the Z level of items that
 * are stacked on a single tile.
 * 
 * This update also uses linq to increase performance.
 */
#define Map_PoolFixColumn

/*
 * Map_InternalProtection
 * 
 * When defined, enables a minor update that protects the Internal Map from
 * potential name changes and ensures that Maps can be correctly parsed by
 * ID or Name without conflicts.
 * 
 * In some cases where the AllMaps list is modified after all Maps have been
 * defined, the Map names may be cached and that cache will become stale.
 * This update removes the caching and uses Linq to improve performance.
 * 
 * If you have issues with Map parsing where the Map returns null or an
 * unexpected Map instance, try enabling this update.
 * 
 * If your shard implements any kind of feature that modifies (adds or removes)
 * the AllMaps list, then you should enable this update.
 * If this update is not enabled in the case of the above context, issues can
 * be verified by having your system generate a new Map instance and modifying
 * the AllMaps list, then using [Props on an Item or Mobile and selecting the
 * Map property from the Gump; The list of names may not be what you expect,
 * in which case, enabling this update will fix it.
 */
#define Map_InternalProtection
#endregion

#region References

#region References
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

using Server.Items;
using Server.Network;
using Server.Targeting;
#endregion

#if Map_NewEnumerables || Map_PoolFixColumn || Map_InternalProtection || Map_AllUpdates
//using System.Linq;
#endif
#endregion

namespace Server
{
	[Flags]
	public enum MapRules
	{
		None = 0x0000,
		Internal = 0x0001, // Internal map (used for dragging, commodity deeds, etc)
		FreeMovement = 0x0002, // Anyone can move over anyone else without taking stamina loss
		BeneficialRestrictions = 0x0004, // Disallow performing beneficial actions on criminals/murderers
		HarmfulRestrictions = 0x0008, // Disallow performing harmful actions on innocents
		TrammelRules = FreeMovement | BeneficialRestrictions | HarmfulRestrictions,
		FeluccaRules = None
	}

	public interface IPooledEnumerable : IEnumerable
	{
		void Free();
	}

	public interface IPooledEnumerable<out T> : IPooledEnumerable, IEnumerable<T>
	{ }

	public interface IPooledEnumerator<out T> : IEnumerator<T>
	{
		void Free();
	}

#if Map_NewEnumerables || Map_AllUpdates
	public static class PooledEnumeration
	{
		public delegate IEnumerable<T> Selector<out T>(Sector sector, Rectangle2D bounds);

		public static Selector<NetState> ClientSelector { get; set; }
		public static Selector<IEntity> EntitySelector { get; set; }
		public static Selector<Mobile> MobileSelector { get; set; }
		public static Selector<Item> ItemSelector { get; set; }
		public static Selector<BaseMulti> MultiSelector { get; set; }
		public static Selector<StaticTile[]> MultiTileSelector { get; set; }

		static PooledEnumeration()
		{
			ClientSelector = SelectClients;
			EntitySelector = SelectEntities;
			MobileSelector = SelectMobiles;
			ItemSelector = SelectItems;
			MultiSelector = SelectMultis;
			MultiTileSelector = SelectMultiTiles;
		}

		public static IEnumerable<NetState> SelectClients(Sector s, Rectangle2D bounds)
		{
			foreach (var o in s.Clients)
			{
				if (o?.Mobile?.Deleted == false && bounds.Contains(o.Mobile))
				{
					yield return o;
				}
			}
		}

		public static IEnumerable<IEntity> SelectEntities(Sector s, Rectangle2D bounds)
		{
			foreach (var o in s.Mobiles)
			{
				if (o?.Deleted == false && bounds.Contains(o))
				{
					yield return o;
				}
			}

			foreach (var o in s.Items)
			{
				if (o?.Deleted == false && o.Parent == null && bounds.Contains(o))
				{
					yield return o;
				}
			}
		}

		public static IEnumerable<Mobile> SelectMobiles(Sector s, Rectangle2D bounds)
		{
			foreach (var o in s.Mobiles)
			{
				if (o?.Deleted == false && bounds.Contains(o))
				{
					yield return o;
				}
			}
		}

		public static IEnumerable<Item> SelectItems(Sector s, Rectangle2D bounds)
		{
			foreach (var o in s.Items)
			{
				if (o?.Deleted == false && o.Parent == null && bounds.Contains(o))
				{
					yield return o;
				}
			}
		}

		public static IEnumerable<BaseMulti> SelectMultis(Sector s, Rectangle2D bounds)
		{
			foreach (var o in s.Multis)
			{
				if (o?.Deleted == false && o.Parent == null && bounds.Contains(o))
				{
					yield return o;
				}
			}
		}

		public static IEnumerable<StaticTile[]> SelectMultiTiles(Sector s, Rectangle2D bounds)
		{
			foreach (var o in s.Multis)
			{
				if (o?.Deleted != false)
				{
					continue;
				}

				var c = o.Components;

				int x, y, xo, yo;
				StaticTile[] t, r;

				for (x = bounds.Start.X; x < bounds.End.X; x++)
				{
					xo = x - (o.X + c.Min.X);

					if (xo < 0 || xo >= c.Width)
					{
						continue;
					}

					for (y = bounds.Start.Y; y < bounds.End.Y; y++)
					{
						yo = y - (o.Y + c.Min.Y);

						if (yo < 0 || yo >= c.Height)
						{
							continue;
						}

						t = c.Tiles[xo][yo];

						if (t.Length <= 0)
						{
							continue;
						}

						r = new StaticTile[t.Length];

						for (var i = 0; i < t.Length; i++)
						{
							r[i] = t[i];
							r[i].Z += o.Z;
						}

						yield return r;
					}
				}
			}
		}

		public static Map.PooledEnumerable<NetState> GetClients(Map map, Rectangle2D bounds)
		{
			return Map.PooledEnumerable<NetState>.Instantiate(map, bounds, ClientSelector ?? SelectClients);
		}

		public static Map.PooledEnumerable<IEntity> GetEntities(Map map, Rectangle2D bounds)
		{
			return Map.PooledEnumerable<IEntity>.Instantiate(map, bounds, EntitySelector ?? SelectEntities);
		}

		public static Map.PooledEnumerable<Mobile> GetMobiles(Map map, Rectangle2D bounds)
		{
			return Map.PooledEnumerable<Mobile>.Instantiate(map, bounds, MobileSelector ?? SelectMobiles);
		}

		public static Map.PooledEnumerable<Item> GetItems(Map map, Rectangle2D bounds)
		{
			return Map.PooledEnumerable<Item>.Instantiate(map, bounds, ItemSelector ?? SelectItems);
		}

		public static Map.PooledEnumerable<BaseMulti> GetMultis(Map map, Rectangle2D bounds)
		{
			return Map.PooledEnumerable<BaseMulti>.Instantiate(map, bounds, MultiSelector ?? SelectMultis);
		}

		public static Map.PooledEnumerable<StaticTile[]> GetMultiTiles(Map map, Rectangle2D bounds)
		{
			return Map.PooledEnumerable<StaticTile[]>.Instantiate(map, bounds, MultiTileSelector ?? SelectMultiTiles);
		}

		public static IEnumerable<Sector> EnumerateSectors(Map map, Rectangle2D bounds)
		{
			if (map == null || map == Map.Internal)
			{
				yield break;
			}

			int x1 = bounds.Start.X, y1 = bounds.Start.Y, x2 = bounds.End.X, y2 = bounds.End.Y;

			if (!Bound(map, ref x1, ref y1, ref x2, ref y2, out var xSector, out var ySector))
			{
				yield break;
			}

			var index = 0;

			while (NextSector(map, x1, y1, x2, y2, ref index, ref xSector, ref ySector, out var s))
			{
				yield return s;
			}
		}

		public static bool Bound(Map map, ref int x1, ref int y1, ref int x2, ref int y2, out int xSector, out int ySector)
		{
			xSector = ySector = 0;

			if (map == null || map == Map.Internal)
			{
				return false;
			}

			map.Bound(x1, y1, out x1, out y1);
			map.Bound(x2 - 1, y2 - 1, out x2, out y2);

			x1 >>= Map.SectorShift;
			y1 >>= Map.SectorShift;
			x2 >>= Map.SectorShift;
			y2 >>= Map.SectorShift;

			xSector = x1;
			ySector = y1;

			return true;
		}

		private static bool NextSector(Map map, int x1, int y1, int x2, int y2, ref int index, ref int xSector, ref int ySector, out Sector s)
		{
			if (map == null)
			{
				xSector = ySector = 0;

				s = null;

				return false;
			}

			if (map == Map.Internal)
			{
				xSector = ySector = 0;

				s = map.InvalidSector;

				return false;
			}

			if (index++ > 0)
			{
				if (++ySector > y2)
				{
					ySector = y1;

					if (++xSector > x2)
					{
						xSector = x1;

						s = map.InvalidSector;

						return false;
					}
				}
			}

			s = map.GetRealSector(xSector, ySector);

			return true;
		}
	}
#endif

	[Parsable]
	//[CustomEnum( new string[]{ "Felucca", "Trammel", "Ilshenar", "Malas", "Internal" } )]
	public class Map : IComparable, IComparable<Map>
	{
		#region Static Context

		public const int SectorSize = 16;
		public const int SectorShift = 4;

		public static int SectorActiveRange = 2;

		public static int[] InvalidLandTiles = 
		{
			0x244 
		};

		private static int m_MaxLOSDistance;

		public static int MaxLOSDistance
		{
			get => m_MaxLOSDistance > 0 ? m_MaxLOSDistance : (Core.GlobalMaxUpdateRange + 1);
			set => m_MaxLOSDistance = value;
		}

		public static Map[] Maps { get; } = new Map[0x100];

		public static Map Felucca => Maps[0];
		public static Map Trammel => Maps[1];
		public static Map Ilshenar => Maps[2];
		public static Map Malas => Maps[3];
		public static Map Tokuno => Maps[4];
		public static Map TerMur => Maps[5];
		public static Map Internal => Maps[0x7F];

		public static List<Map> AllMaps { get; } = new List<Map>();

		private static readonly List<string> m_MapNames = new List<string>();

		private static readonly ConcurrentQueue<SortedSet<Item>> m_FixPool = new ConcurrentQueue<SortedSet<Item>>();

		public static string[] GetMapNames()
		{
			m_MapNames.Clear();

			foreach (var map in AllMaps)
			{
				m_MapNames.Add(map.Name);
			}

			return m_MapNames.ToArray();
		}

		public static Map[] GetMapValues()
		{
			return AllMaps.ToArray();
		}

		public static Map Parse(string value)
		{
			if (String.IsNullOrWhiteSpace(value))
			{
				return null;
			}

			if (Insensitive.Equals(value, "Internal"))
			{
				return Internal;
			}

			if (!Int32.TryParse(value, out var index))
			{
				return AllMaps.Find(m => m != null && Insensitive.Equals(m.Name, value));
			}

			if (index == 127)
			{
				return Internal;
			}

			return AllMaps.Find(m => m != null && m.MapIndex == index);
		}

		private static SortedSet<Item> AcquireFixItems(Map map, int x, int y)
		{
			if (map == null || map == Internal || x < 0 || x > map.Width || y < 0 || y > map.Height)
			{
				return null;
			}

			if (!m_FixPool.TryDequeue(out var pool))
			{
				pool = new SortedSet<Item>(ZComparer.Default);
			}

			var eable = map.GetItemsInRange(new Point3D(x, y, 0), 0);

			foreach (var item in eable)
			{
				if (item is BaseMulti || item.ItemID > TileData.MaxItemValue)
				{
					continue;
				}

				if (item.Movable)
				{
					pool.Add(item);
				}
			}

			eable.Free();

			return pool;
		}

		private static void FreeFixItems(SortedSet<Item> pool)
		{
			if (pool == null)
			{
				return;
			}

			pool.Clear();

			if (m_FixPool.Count < 128)
			{
				m_FixPool.Enqueue(pool);
			}
		}

		#endregion

		private readonly Sector[][] m_Sectors;

		private readonly object m_TileLock = new object();

		private TileMatrix m_Tiles;

		public TileMatrix Tiles
		{
			get
			{
				if (m_Tiles != null)
				{
					return m_Tiles;
				}

				lock (m_TileLock)
				{
					return m_Tiles ?? (m_Tiles = new TileMatrix(this, FileIndex, MapID, Width, Height));
				}
			}
		}

		public int MapID { get; }
		public int MapIndex { get; }

		public int FileIndex { get; }

		public int Width { get; }
		public int Height { get; }

		public int SectorsWidth { get; }
		public int SectorsHeight { get; }

		public Sector InvalidSector { get; }

		public Dictionary<string, Region> Regions { get; }

		private Region m_DefaultRegion;

		public Region DefaultRegion
		{
			get => m_DefaultRegion ?? (m_DefaultRegion = new Region(null, this, 0, new Rectangle3D[0]));
			set => m_DefaultRegion = value;
		}

		public MapRules Rules { get; set; }

		public string Name { get; set; }

		public int Season { get; set; }

		public Map(int mapID, int mapIndex, int fileIndex, int width, int height, int season, string name, MapRules rules)
		{
			MapID = mapID;
			MapIndex = mapIndex;
			FileIndex = fileIndex;
			Width = width;
			Height = height;

			Season = season;
			Name = name;
			Rules = rules;

			Regions = new Dictionary<string, Region>(StringComparer.InvariantCultureIgnoreCase);

			InvalidSector = new Sector(0, 0, this);

			SectorsWidth = width >> SectorShift;
			SectorsHeight = height >> SectorShift;

			m_Sectors = new Sector[SectorsWidth][];
		}

		public override string ToString()
		{
			return Name;
		}

		public int GetAverageZ(int x, int y)
		{
			int z = 0, avg = 0, top = 0;

			GetAverageZ(x, y, ref z, ref avg, ref top);

			return avg;
		}

		public void GetAverageZ(int x, int y, ref int z, ref int avg, ref int top)
		{
			var zTop = Tiles.GetLandTile(x, y).Z;
			var zLeft = Tiles.GetLandTile(x, y + 1).Z;
			var zRight = Tiles.GetLandTile(x + 1, y).Z;
			var zBottom = Tiles.GetLandTile(x + 1, y + 1).Z;

			z = zTop;

			if (zLeft < z)
			{
				z = zLeft;
			}

			if (zRight < z)
			{
				z = zRight;
			}

			if (zBottom < z)
			{
				z = zBottom;
			}

			top = zTop;

			if (zLeft > top)
			{
				top = zLeft;
			}

			if (zRight > top)
			{
				top = zRight;
			}

			if (zBottom > top)
			{
				top = zBottom;
			}

			if (Math.Abs(zTop - zBottom) > Math.Abs(zLeft - zRight))
			{
				avg = FloorAverage(zLeft, zRight);
			}
			else
			{
				avg = FloorAverage(zTop, zBottom);
			}
		}

		private static int FloorAverage(int a, int b)
		{
			var v = a + b;

			if (v < 0)
			{
				--v;
			}

			return v / 2;
		}

		#region Get*InRange/Bounds/At
		public IPooledEnumerable<IEntity> GetObjectsInRange(Point3D p)
		{
			return GetObjectsInRange(p, Core.GlobalMaxUpdateRange);
		}

		public IPooledEnumerable<IEntity> GetObjectsInRange(Point3D p, int range)
		{
			return GetObjectsInBounds(new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1));
		}

		public IPooledEnumerable<IEntity> GetObjectsInBounds(Rectangle3D bounds)
		{
			return GetObjectsInBounds(new Rectangle2D(bounds.Start, bounds.End));
		}

		public IPooledEnumerable<IEntity> GetObjectsInBounds(Rectangle2D bounds)
		{
			return PooledEnumeration.GetEntities(this, bounds);
		}

		public IPooledEnumerable<NetState> GetClientsInRange(Point3D p)
		{
			return GetClientsInRange(p, Core.GlobalMaxUpdateRange);
		}

		public IPooledEnumerable<NetState> GetClientsInRange(Point3D p, int range)
		{
			return GetClientsInBounds(new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1));
		}

		public IPooledEnumerable<NetState> GetClientsInBounds(Rectangle3D bounds)
		{
			return GetClientsInBounds(new Rectangle2D(bounds.Start, bounds.End));
		}

		public IPooledEnumerable<NetState> GetClientsInBounds(Rectangle2D bounds)
		{
			return PooledEnumeration.GetClients(this, bounds);
		}

		public IPooledEnumerable<Item> GetItemsInRange(Point3D p)
		{
			return GetItemsInRange(p, Core.GlobalMaxUpdateRange);
		}

		public IPooledEnumerable<Item> GetItemsInRange(Point3D p, int range)
		{
			return GetItemsInBounds(new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1));
		}

		public IPooledEnumerable<Item> GetItemsInBounds(Rectangle3D bounds)
		{
			return GetItemsInBounds(new Rectangle2D(bounds.Start, bounds.End));
		}

		public IPooledEnumerable<Item> GetItemsInBounds(Rectangle2D bounds)
		{
			return PooledEnumeration.GetItems(this, bounds);
		}

		public IPooledEnumerable<Mobile> GetMobilesInRange(Point3D p)
		{
			return GetMobilesInRange(p, Core.GlobalMaxUpdateRange);
		}

		public IPooledEnumerable<Mobile> GetMobilesInRange(Point3D p, int range)
		{
			return GetMobilesInBounds(new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1));
		}

		public IPooledEnumerable<Mobile> GetMobilesInBounds(Rectangle3D bounds)
		{
			return GetMobilesInBounds(new Rectangle2D(bounds.Start, bounds.End));
		}

		public IPooledEnumerable<Mobile> GetMobilesInBounds(Rectangle2D bounds)
		{
			return PooledEnumeration.GetMobiles(this, bounds);
		}

		public IPooledEnumerable<BaseMulti> GetMultisInRange(Point3D p)
		{
			return GetMultisInRange(p, Core.GlobalMaxUpdateRange);
		}

		public IPooledEnumerable<BaseMulti> GetMultisInRange(Point3D p, int range)
		{
			return GetMultisInBounds(new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1));
		}

		public IPooledEnumerable<BaseMulti> GetMultisInBounds(Rectangle3D bounds)
		{
			return GetMultisInBounds(new Rectangle2D(bounds.Start, bounds.End));
		}

		public IPooledEnumerable<BaseMulti> GetMultisInBounds(Rectangle2D bounds)
		{
			return PooledEnumeration.GetMultis(this, bounds);
		}

		public IPooledEnumerable<StaticTile[]> GetMultiTilesAt(int x, int y)
		{
			return PooledEnumeration.GetMultiTiles(this, new Rectangle2D(x, y, 1, 1));
		}
		#endregion

		#region CanFit
		public bool CanFit(Point3D p, int height, bool checkBlocksFit)
		{
			return CanFit(p.m_X, p.m_Y, p.m_Z, height, checkBlocksFit, true, true);
		}

		public bool CanFit(Point3D p, int height, bool checkBlocksFit, bool checkMobiles)
		{
			return CanFit(p.m_X, p.m_Y, p.m_Z, height, checkBlocksFit, checkMobiles, true, false);
		}

		public bool CanFit(Point2D p, int z, int height, bool checkBlocksFit)
		{
			return CanFit(p.m_X, p.m_Y, z, height, checkBlocksFit, true, true, false);
		}

		public bool CanFit(Point3D p, int height)
		{
			return CanFit(p.m_X, p.m_Y, p.m_Z, height, false, true, true, false);
		}

		public bool CanFit(Point2D p, int z, int height)
		{
			return CanFit(p.m_X, p.m_Y, z, height, false, true, true, false);
		}

		public bool CanFit(int x, int y, int z, int height)
		{
			return CanFit(x, y, z, height, false, true, true, false);
		}

		public bool CanFit(int x, int y, int z, int height, bool checksBlocksFit)
		{
			return CanFit(x, y, z, height, checksBlocksFit, true, true, false);
		}

		public bool CanFit(int x, int y, int z, int height, bool checkBlocksFit, bool checkMobiles)
		{
			return CanFit(x, y, z, height, checkBlocksFit, checkMobiles, true, false);
		}

		public bool CanFit(int x, int y, int z, int height, bool checkBlocksFit, bool checkMobiles, bool requireSurface)
		{
			return CanFit(x, y, z, height, checkBlocksFit, checkMobiles, requireSurface, false);
		}

		public bool CanFit(int x, int y, int z, int height, bool checkBlocksFit, bool checkMobiles, bool requireSurface, bool ignoreRoof)
		{
			if (this == Internal)
			{
				return false;
			}

			if (x < 0 || y < 0 || x >= Width || y >= Height)
			{
				return false;
			}

			int lowZ = 0, avgZ = 0, topZ = 0;

			GetAverageZ(x, y, ref lowZ, ref avgZ, ref topZ);

			var lt = Tiles.GetLandTile(x, y);

			var landFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;

			if ((landFlags & TileFlag.Impassable) != 0 && avgZ > z && (z + height) > lowZ)
			{
				return false;
			}

			var hasSurface = (landFlags & TileFlag.Impassable) == 0 && z == avgZ && !lt.Ignored;

			var staticTiles = Tiles.GetStaticTiles(x, y, true);

			bool surface, impassable, roof;

			foreach (var t in staticTiles)
			{
				var id = TileData.ItemTable[t.ID & TileData.MaxItemValue];
				surface = id.Surface;
				impassable = id.Impassable;
				roof = (id.Flags & TileFlag.Roof) != 0;

				if ((surface || impassable) && (!ignoreRoof || !roof) && (t.Z + id.CalcHeight) > z && (z + height) > t.Z)
				{
					return false;
				}

				if (surface && !impassable && z == (t.Z + id.CalcHeight))
				{
					hasSurface = true;
				}
			}

			var sector = GetSector(x, y);
			var items = sector.Items;
			var mobs = sector.Mobiles;

			foreach (var item in items)
			{
				if (!(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint(x, y))
				{
					var id = item.ItemData;

					surface = id.Surface;
					impassable = id.Impassable;

					if ((surface || impassable || (checkBlocksFit && item.BlocksFit)) && (item.Z + id.CalcHeight) > z &&
						(z + height) > item.Z)
					{
						return false;
					}

					if (surface && !impassable && !item.Movable && z == (item.Z + id.CalcHeight))
					{
						hasSurface = true;
					}
				}
			}

			if (checkMobiles)
			{
				foreach (var m in mobs)
				{
					if (m.Location.m_X == x && m.Location.m_Y == y && (m.AccessLevel == AccessLevel.Player || !m.Hidden))
					{
						if ((m.Z + 16) > z && (z + height) > m.Z)
						{
							return false;
						}
					}
				}
			}

			return !requireSurface || hasSurface;
		}

		public bool CanFit(int x, int y, int z, int height, bool checkBlocksFit, bool checkMobiles, bool requireSurface, Mobile mob)
		{
			if (this == Internal)
			{
				return false;
			}

			if (x < 0 || y < 0 || x >= Width || y >= Height)
			{
				return false;
			}

			var hasSurface = false;
			var checkmob = false;
			var canswim = false;
			var cantwalk = false;

			if (mob != null)
			{
				checkmob = true;
				canswim = mob.CanSwim;
				cantwalk = mob.CantWalk;
			}

			var lt = Tiles.GetLandTile(x, y);

			int lowZ = 0, avgZ = 0, topZ = 0;

			bool surface, impassable;

			GetAverageZ(x, y, ref lowZ, ref avgZ, ref topZ);

			var landFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;

			impassable = (landFlags & TileFlag.Impassable) != 0;

			bool wet;

			if (checkmob)
			{
				wet = (landFlags & TileFlag.Wet) != 0;

				// dont allow wateronly creatures on land
				if (cantwalk && !wet)
				{
					impassable = true;
				}

				// allow water creatures on water
				if (canswim && wet)
				{
					impassable = false;
				}
			}

			if (impassable && avgZ > z && (z + height) > lowZ)
			{
				return false;
			}

			if (!impassable && z == avgZ && !lt.Ignored)
			{
				hasSurface = true;
			}

			var staticTiles = Tiles.GetStaticTiles(x, y, true);

			for (var i = 0; i < staticTiles.Length; ++i)
			{
				var id = TileData.ItemTable[staticTiles[i].ID & TileData.MaxItemValue];

				surface = id.Surface;
				impassable = id.Impassable;

				if (checkmob)
				{
					wet = (id.Flags & TileFlag.Wet) != 0;

					// dont allow wateronly creatures on land
					if (cantwalk && !wet)
					{
						impassable = true;
					}

					// allow water creatures on water
					if (canswim && wet)
					{
						surface = true;
						impassable = false;
					}
				}

				if ((surface || impassable) && staticTiles[i].Z + id.CalcHeight > z && z + height > staticTiles[i].Z)
				{
					return false;
				}

				if (surface && !impassable && z == staticTiles[i].Z + id.CalcHeight)
				{
					hasSurface = true;
				}
			}

			var sector = GetSector(x, y);

			foreach (var item in sector.Items)
			{
				if (item.ItemID < 0x4000 && item.AtWorldPoint(x, y))
				{
					var id = item.ItemData;

					surface = id.Surface;
					impassable = id.Impassable;

					if (checkmob)
					{
						wet = (id.Flags & TileFlag.Wet) != 0;

						// dont allow wateronly creatures on land
						if (cantwalk && !wet)
						{
							impassable = true;
						}

						// allow water creatures on water
						if (canswim && wet)
						{
							surface = true;
							impassable = false;
						}
					}

					if ((surface || impassable || (checkBlocksFit && item.BlocksFit)) && item.Z + id.CalcHeight > z && z + height > item.Z)
					{
						return false;
					}

					if (surface && !impassable && !item.Movable && z == item.Z + id.CalcHeight)
					{
						hasSurface = true;
					}
				}
			}

			if (checkMobiles)
			{
				foreach (var m in sector.Mobiles)
				{
					if (m.Location.X == x && m.Location.Y == y && (m.AccessLevel == AccessLevel.Player || !m.Hidden))
					{
						if (m.Z + 16 > z && z + height > m.Z)
						{
							return false;
						}
					}
				}
			}

			return !requireSurface || hasSurface;
		}
		#endregion

		#region CanSpawnMobile
		public bool CanSpawnMobile(Point3D p)
		{
			return CanSpawnMobile(p, true);
		}

		public bool CanSpawnMobile(Point2D p, int z)
		{
			return CanSpawnMobile(p, z, true);
		}

		public bool CanSpawnMobile(int x, int y, int z)
		{
			return CanSpawnMobile(x, y, z, true);
		}

		public bool CanSpawnMobile(Point3D p, bool checkRegion)
		{
			return CanSpawnMobile(p.m_X, p.m_Y, p.m_Z, checkRegion);
		}

		public bool CanSpawnMobile(Point2D p, int z, bool checkRegion)
		{
			return CanSpawnMobile(p.m_X, p.m_Y, z, checkRegion);
		}

		public bool CanSpawnMobile(int x, int y, int z, bool checkRegion)
		{
			if (checkRegion && !Region.Find(new Point3D(x, y, z), this).AllowSpawn())
			{
				return false;
			}

			return CanFit(x, y, z, 16);
		}
		#endregion

		#region Find Item/Mobile
		public TItem FindItem<TItem>(Point3D p, int range = 0) where TItem : Item
		{
			var eable = GetItemsInRange(p, range);

			foreach (var item in eable)
			{
				if (item is TItem o)
				{
					eable.Free();
					return o;
				}
			}

			eable.Free();
			return null;
		}

		public IEnumerable<TItem> FindItems<TItem>(Point3D p, int range = 0) where TItem : Item
		{
			var eable = GetItemsInRange(p, range);

			foreach (var item in eable)
			{
				if (item is TItem o)
				{
					yield return o;
				}
			}

			eable.Free();
		}

		public TMob FindMobile<TMob>(Point3D p, int range = 0) where TMob : Mobile
		{
			var eable = GetMobilesInRange(p, range);

			foreach (var m in eable)
			{
				if (m is TMob o)
				{
					eable.Free();
					return o;
				}
			}

			eable.Free();
			return null;
		}

		public IEnumerable<TMob> FindMobiles<TMob>(Point3D p, int range = 0) where TMob : Mobile
		{
			var eable = GetMobilesInRange(p, range);

			foreach (var m in eable)
			{
				if (m is TMob o)
				{
					yield return o;
				}
			}

			eable.Free();
		}
		#endregion

		#region Spawn Position
		public Point3D GetSpawnPosition(Point3D center, int range)
		{
			return GetSpawnPosition(center, range, true);
		}

		public Point3D GetSpawnPosition(Point3D center, int range, bool checkRegion)
		{
			var attempts = (int)Math.Sqrt(range * range);

			for (var i = 0; i < attempts; i++)
			{
				var x = center.X + (Utility.Random((range * 2) + 1) - range);
				var y = center.Y + (Utility.Random((range * 2) + 1) - range);
				var z = GetAverageZ(x, y);

				if (CanSpawnMobile(x, y, z, checkRegion))
				{
					return new Point3D(x, y, z);
				}
			}

			return center;
		}

		public Point3D GetRandomSpawnPoint(Rectangle2D rec)
		{
			if (this == Internal)
			{
				return Point3D.Zero;
			}

			var x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
			var y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
			var z = GetAverageZ(x, y);

			return new Point3D(x, y, z);
		}
		#endregion

		public void FixColumn(int x, int y)
		{
			var landTile = Tiles.GetLandTile(x, y);
			var tiles = Tiles.GetStaticTiles(x, y, true);

			int landZ = 0, landAvg = 0, landTop = 0;

			GetAverageZ(x, y, ref landZ, ref landAvg, ref landTop);

			var items = AcquireFixItems(this, x, y);

			foreach (var toFix in items)
			{
				if (!toFix.Movable)
				{
					continue;
				}

				var z = Int32.MinValue;
				var currentZ = toFix.Z;

				if (!landTile.Ignored && landAvg <= currentZ)
				{
					z = landAvg;
				}

				foreach (var tile in tiles)
				{
					var id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

					var checkZ = tile.Z;
					var checkTop = checkZ + id.CalcHeight;

					if (checkTop == checkZ && !id.Surface)
					{
						++checkTop;
					}

					if (checkTop > z && checkTop <= currentZ)
					{
						z = checkTop;
					}
				}

				foreach (var item in items)
				{
					if (item == toFix)
					{
						continue;
					}

					var id = item.ItemData;

					var checkZ = item.Z;
					var checkTop = checkZ + id.CalcHeight;

					if (checkTop == checkZ && !id.Surface)
					{
						++checkTop;
					}

					if (checkTop > z && checkTop <= currentZ)
					{
						z = checkTop;
					}
				}

				if (z != Int32.MinValue)
				{
					toFix.Location = new Point3D(toFix.X, toFix.Y, z);
				}
			}

			FreeFixItems(items);
		}

		/// <summary>
		///     Gets the highest surface that is lower than <paramref name="p" />.
		/// </summary>
		/// <param name="p">The reference point.</param>
		/// <returns>A surface <typeparamref><name>IEntity</name></typeparamref> or <typeparamref><name>Item</name></typeparamref>.</returns>
		public object GetTopSurface(Point3D p)
		{
			if (this == Internal)
			{
				return null;
			}

			object surface = null;
			var surfaceZ = Int32.MinValue;

			var lt = Tiles.GetLandTile(p.X, p.Y);

			if (!lt.Ignored)
			{
				var avgZ = GetAverageZ(p.X, p.Y);

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

			var staticTiles = Tiles.GetStaticTiles(p.X, p.Y, true);

			foreach (var tile in staticTiles)
			{
				var id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				if (id.Surface || (id.Flags & TileFlag.Wet) != 0)
				{
					var tileZ = tile.Z + id.CalcHeight;

					if (tileZ > surfaceZ && tileZ <= p.Z)
					{
						surface = tile;
						surfaceZ = tileZ;

						if (surfaceZ == p.Z)
						{
							return surface;
						}
					}
				}
			}

			var sector = GetSector(p.X, p.Y);

			foreach (var item in sector.Items)
			{
				if (item is BaseMulti || item.ItemID > TileData.MaxItemValue || item.Movable || !item.AtWorldPoint(p.X, p.Y))
				{
					continue;
				}

				var id = item.ItemData;

				if (id.Surface || (id.Flags & TileFlag.Wet) != 0)
				{
					var itemZ = item.Z + id.CalcHeight;

					if (itemZ > surfaceZ && itemZ <= p.Z)
					{
						surface = item;
						surfaceZ = itemZ;

						if (surfaceZ == p.Z)
						{
							return surface;
						}
					}
				}
			}

			return surface;
		}

		public void Bound(int x, int y, out int newX, out int newY)
		{
			if (x < 0)
			{
				newX = 0;
			}
			else if (x >= Width)
			{
				newX = Width - 1;
			}
			else
			{
				newX = x;
			}

			if (y < 0)
			{
				newY = 0;
			}
			else if (y >= Height)
			{
				newY = Height - 1;
			}
			else
			{
				newY = y;
			}
		}

		public Point2D Bound(IPoint2D p)
		{
			int x = p.X, y = p.Y;

			if (x < 0)
			{
				x = 0;
			}
			else if (x >= Width)
			{
				x = Width - 1;
			}

			if (y < 0)
			{
				y = 0;
			}
			else if (y >= Height)
			{
				y = Height - 1;
			}

			return new Point2D(x, y);
		}

		#region GetSector
		public Sector GetSector(Point3D p)
		{
			return InternalGetSector(p.m_X >> SectorShift, p.m_Y >> SectorShift);
		}

		public Sector GetSector(Point2D p)
		{
			return InternalGetSector(p.m_X >> SectorShift, p.m_Y >> SectorShift);
		}

		public Sector GetSector(IPoint2D p)
		{
			return InternalGetSector(p.X >> SectorShift, p.Y >> SectorShift);
		}

		public Sector GetSector(int x, int y)
		{
			return InternalGetSector(x >> SectorShift, y >> SectorShift);
		}

		public Sector GetRealSector(int x, int y)
		{
			return InternalGetSector(x, y);
		}

		private Sector InternalGetSector(int x, int y)
		{
			if (x >= 0 && x < SectorsWidth && y >= 0 && y < SectorsHeight)
			{
				var xSectors = m_Sectors[x];

				if (xSectors == null)
				{
					m_Sectors[x] = xSectors = new Sector[SectorsHeight];
				}

				var sec = xSectors[y];

				if (sec == null)
				{
					xSectors[y] = sec = new Sector(x, y, this);
				}

				return sec;
			}
			else
			{
				return InvalidSector;
			}
		}
		#endregion

		public void ActivateSectors(int cx, int cy)
		{
			for (var x = cx - SectorActiveRange; x <= cx + SectorActiveRange; ++x)
			{
				for (var y = cy - SectorActiveRange; y <= cy + SectorActiveRange; ++y)
				{
					var sect = GetRealSector(x, y);

					if (sect != InvalidSector)
					{
						sect.Activate();
					}
				}
			}
		}

		public void DeactivateSectors(int cx, int cy)
		{
			for (var x = cx - SectorActiveRange; x <= cx + SectorActiveRange; ++x)
			{
				for (var y = cy - SectorActiveRange; y <= cy + SectorActiveRange; ++y)
				{
					var sect = GetRealSector(x, y);

					if (sect != InvalidSector && !PlayersInRange(sect, SectorActiveRange))
					{
						sect.Deactivate();
					}
				}
			}
		}

		private bool PlayersInRange(Sector sect, int range)
		{
			for (var x = sect.X - range; x <= sect.X + range; ++x)
			{
				for (var y = sect.Y - range; y <= sect.Y + range; ++y)
				{
					var check = GetRealSector(x, y);

					if (check != InvalidSector && check.PlayerCount > 0)
					{
						return true;
					}
				}
			}

			return false;
		}

		public void OnClientChange(NetState oldState, NetState newState, Mobile m)
		{
			if (this != Internal)
			{
				GetSector(m).OnClientChange(oldState, newState);
			}
		}

		public virtual void OnEnter(Mobile m)
		{
			if (this != Internal)
			{
				GetSector(m).OnEnter(m);
			}
		}

		public virtual void OnEnter(Item item)
		{
			if (this == Internal)
			{
				return;
			}

			GetSector(item).OnEnter(item);

			if (item is BaseMulti m)
			{
				var mcl = m.Components;

				var start = GetMultiMinSector(item.Location, mcl);
				var end = GetMultiMaxSector(item.Location, mcl);

				AddMulti(m, start, end);
			}
		}

		public virtual void OnLeave(Mobile m)
		{
			if (this != Internal)
			{
				GetSector(m).OnLeave(m);
			}
		}

		public virtual void OnLeave(Item item)
		{
			if (this == Internal)
			{
				return;
			}

			GetSector(item).OnLeave(item);

			if (item is BaseMulti m)
			{
				var mcl = m.Components;

				var start = GetMultiMinSector(item.Location, mcl);
				var end = GetMultiMaxSector(item.Location, mcl);

				RemoveMulti(m, start, end);
			}
		}

		public void RemoveMulti(BaseMulti m, Sector start, Sector end)
		{
			if (this == Internal)
			{
				return;
			}

			for (var x = start.X; x <= end.X; ++x)
			{
				for (var y = start.Y; y <= end.Y; ++y)
				{
					InternalGetSector(x, y).OnMultiLeave(m);
				}
			}
		}

		public void AddMulti(BaseMulti m, Sector start, Sector end)
		{
			if (this == Internal)
			{
				return;
			}

			for (var x = start.X; x <= end.X; ++x)
			{
				for (var y = start.Y; y <= end.Y; ++y)
				{
					InternalGetSector(x, y).OnMultiEnter(m);
				}
			}
		}

		public Sector GetMultiMinSector(Point3D loc, MultiComponentList mcl)
		{
			return GetSector(Bound(new Point2D(loc.m_X + mcl.Min.m_X, loc.m_Y + mcl.Min.m_Y)));
		}

		public Sector GetMultiMaxSector(Point3D loc, MultiComponentList mcl)
		{
			return GetSector(Bound(new Point2D(loc.m_X + mcl.Max.m_X, loc.m_Y + mcl.Max.m_Y)));
		}

		public virtual void OnMove(Point3D oldLocation, Mobile m)
		{
			if (this == Internal)
			{
				return;
			}

			var oldSector = GetSector(oldLocation);
			var newSector = GetSector(m.Location);

			if (oldSector != newSector)
			{
				oldSector.OnLeave(m);
				newSector.OnEnter(m);
			}
		}

		public virtual void OnMove(Point3D oldLocation, Item item)
		{
			if (this == Internal)
			{
				return;
			}

			var oldSector = GetSector(oldLocation);
			var newSector = GetSector(item.Location);

			if (oldSector != newSector)
			{
				oldSector.OnLeave(item);
				newSector.OnEnter(item);
			}

			if (item is BaseMulti m)
			{
				var mcl = m.Components;

				var start = GetMultiMinSector(item.Location, mcl);
				var end = GetMultiMaxSector(item.Location, mcl);

				var oldStart = GetMultiMinSector(oldLocation, mcl);
				var oldEnd = GetMultiMaxSector(oldLocation, mcl);

				if (oldStart != start || oldEnd != end)
				{
					RemoveMulti(m, oldStart, oldEnd);
					AddMulti(m, start, end);
				}
			}
		}

		public void RegisterRegion(Region reg)
		{
			var regName = reg.Name;

			if (regName != null)
			{
				if (Regions.ContainsKey(regName))
				{
					Console.WriteLine("Warning: Duplicate region name '{0}' for map '{1}'", regName, Name);
				}
				else
				{
					Regions[regName] = reg;
				}
			}
		}

		public void UnregisterRegion(Region reg)
		{
			var regName = reg.Name;

			if (regName != null)
			{
				Regions.Remove(regName);
			}
		}

		#region Line Of Sight
		public Point3D GetPoint(object o, bool eye)
		{
			Point3D p;

			if (o is Mobile m)
			{
				p = m.Location;
				p.Z += 14; //eye ? 15 : 10;
			}
			else if (o is Item i)
			{
				p = i.GetWorldLocation();
				p.Z += (i.ItemData.Height / 2) + 1;
			}
			else if (o is Point3D l)
			{
				p = l;
			}
			else if (o is LandTarget lt)
			{
				p = lt.Location;

				int low = 0, avg = 0, top = 0;

				GetAverageZ(p.X, p.Y, ref low, ref avg, ref top);

				p.Z = top + 1;
			}
			else if (o is StaticTarget st)
			{
				var id = TileData.ItemTable[st.ItemID & TileData.MaxItemValue];

				p = new Point3D(st.X, st.Y, st.Z - id.CalcHeight + (id.Height / 2));
			}
			else if (o is IPoint3D ip)
			{
				p = new Point3D(ip);
			}
			else
			{
				p = Point3D.Zero;
			}

			return p;
		}

		public bool LineOfSight(Point3D org, Point3D dest)
		{
			if (this == Internal)
			{
				return false;
			}

			if (!Utility.InRange(org, dest, MaxLOSDistance))
			{
				return false;
			}

			var end = dest;

			if (org.X > dest.X || (org.X == dest.X && org.Y > dest.Y) || (org.X == dest.X && org.Y == dest.Y && org.Z > dest.Z))
			{
				var swap = org;

				org = dest;
				dest = swap;
			}

			if (org == dest)
			{
				return true;
			}

			var xd = dest.m_X - org.m_X;
			var yd = dest.m_Y - org.m_Y;
			var zd = dest.m_Z - org.m_Z;

			var zslp = Math.Sqrt(xd * xd + yd * yd);

			double sq3d;

			if (zd != 0)
			{
				sq3d = Math.Sqrt(zslp * zslp + zd * zd);
			}
			else
			{
				sq3d = zslp;
			}

			var rise = yd / sq3d;
			var run = xd / sq3d;

			zslp = zd / sq3d;

			double x = org.m_X;
			double y = org.m_Y;
			double z = org.m_Z;

			var path = new Point3DList();

			int ix, iy, iz;
			Point3D p;

			while (Utility.NumberBetween(x, dest.m_X, org.m_X, 0.5) && Utility.NumberBetween(y, dest.m_Y, org.m_Y, 0.5) && Utility.NumberBetween(z, dest.m_Z, org.m_Z, 0.5))
			{
				ix = (int)Math.Round(x);
				iy = (int)Math.Round(y);
				iz = (int)Math.Round(z);

				if (path.Count > 0)
				{
					p = path.Last;

					if (p.m_X != ix || p.m_Y != iy || p.m_Z != iz)
					{
						path.Add(ix, iy, iz);
					}
				}
				else
				{
					path.Add(ix, iy, iz);
				}

				x += run;
				y += rise;
				z += zslp;
			}

			if (path.Count == 0)
			{
				return true;
			}

			p = path.Last;

			if (p != dest)
			{
				path.Add(dest);
			}

			Point3D pTop = org, pBottom = dest;

			Utility.FixPoints(ref pTop, ref pBottom);

			var pathCount = path.Count;
			var endTop = end.m_Z + 1;

			int height, landZ, landAvg, landTop, pointTop, ltID;
			bool contains;
			Point3D point;
			LandTile landTile;
			ItemData id;
			TileFlag flags;
			StaticTile[] statics;
			IPooledEnumerable<Item> eable;

			for (var i = 0; i < pathCount; ++i)
			{
				point = path[i];
				pointTop = point.m_Z;

				landTile = Tiles.GetLandTile(point.X, point.Y);

				landZ = landAvg = landTop = 0;

				GetAverageZ(point.m_X, point.m_Y, ref landZ, ref landAvg, ref landTop);

				if (landZ <= pointTop && landTop >= point.m_Z && (point.m_X != end.m_X || point.m_Y != end.m_Y || landZ > endTop || landTop < end.m_Z) && !landTile.Ignored)
				{
					return false;
				}

				statics = Tiles.GetStaticTiles(point.m_X, point.m_Y, true);

				contains = false;
				ltID = landTile.ID;

				for (var j = 0; !contains && j < InvalidLandTiles.Length; ++j)
				{
					contains = ltID == InvalidLandTiles[j];
				}

				if (contains && statics.Length == 0)
				{
					eable = GetItemsInRange(point, 0);

					foreach (var item in eable)
					{
						if (item.Visible)
						{
							contains = false;
						}

						if (!contains)
						{
							break;
						}
					}

					eable.Free();

					if (contains)
					{
						return false;
					}
				}

				foreach (var t in statics)
				{
					id = TileData.ItemTable[t.ID & TileData.MaxItemValue];

					flags = id.Flags;
					height = id.CalcHeight;

					if (t.Z <= pointTop && t.Z + height >= point.Z && (flags & (TileFlag.Window | TileFlag.NoShoot)) != 0)
					{
						if (point.m_X != end.m_X || point.m_Y != end.m_Y || t.Z > endTop || t.Z + height < end.m_Z)
						{
							return false;
						}
					}
				}
			}

			var rect = new Rectangle2D(pTop.m_X, pTop.m_Y, pBottom.m_X - pTop.m_X + 1, pBottom.m_Y - pTop.m_Y + 1);

			var area = GetItemsInBounds(rect);

			try
			{
				int count;
				bool found;
				Point3D loc;

				foreach (var i in area)
				{
					if (!i.Visible)
					{
						continue;
					}

					if (i is BaseMulti || i.ItemID > TileData.MaxItemValue)
					{
						continue;
					}

					id = i.ItemData;

					flags = id.Flags;

					if ((flags & (TileFlag.Window | TileFlag.NoShoot)) == 0)
					{
						continue;
					}

					height = id.CalcHeight;

					found = false;

					count = path.Count;

					for (var j = 0; j < count; ++j)
					{
						point = path[j];
						pointTop = point.m_Z + 1;
						loc = i.Location;

						if (loc.m_X == point.m_X && loc.m_Y == point.m_Y && loc.m_Z <= pointTop && loc.m_Z + height >= point.m_Z)
						{
							if (loc.m_X == end.m_X && loc.m_Y == end.m_Y && loc.m_Z <= endTop && loc.m_Z + height >= end.m_Z)
							{
								continue;
							}

							found = true;
							break;
						}
					}

					if (found)
					{
						return false;
					}
				}
			}
			finally
			{
				area.Free();
			}

			return true;
		}

		public bool LineOfSight(object from, object dest)
		{
			if (from == dest || (from is Mobile m && m.AccessLevel > AccessLevel.Player))
			{
				return true;
			}

			if (dest is Item d && from is Mobile f && d.RootParent == f)
			{
				return true;
			}

			return LineOfSight(GetPoint(from, true), GetPoint(dest, false));
		}

		public bool LineOfSight(Mobile from, Point3D target)
		{
			if (from.AccessLevel > AccessLevel.Player)
			{
				return true;
			}

			var eye = from.Location;

			eye.Z += 14;

			return LineOfSight(eye, target);
		}

		public bool LineOfSight(Mobile from, Mobile to)
		{
			if (from == to || from.AccessLevel > AccessLevel.Player)
			{
				return true;
			}

			var eye = from.Location;
			var target = to.Location;

			eye.Z += 14;
			target.Z += 14;

			return LineOfSight(eye, target);
		}
		#endregion

		public int CompareTo(Map other)
		{
			if (other == null)
			{
				return -1;
			}

			return MapID.CompareTo(other.MapID);
		}

		public int CompareTo(object other)
		{
			return CompareTo(other as Map);
		}

		private class ZComparer : IComparer<Item>
		{
			public static readonly ZComparer Default = new ZComparer();

			public int Compare(Item x, Item y)
			{
				if (x != null && y != null)
				{
					return x.Z.CompareTo(y.Z);
				}

				return 0;
			}
		}

		public class NullEnumerable<T> : IPooledEnumerable<T>
		{
			public static readonly NullEnumerable<T> Instance = new NullEnumerable<T>();

			private NullEnumerable()
			{ }

			IEnumerator IEnumerable.GetEnumerator()
			{
				yield break;
			}

			public IEnumerator<T> GetEnumerator()
			{
				yield break;
			}

			public void Free()
			{ }
		}

		public sealed class PooledEnumerable<T> : IPooledEnumerable<T>, IDisposable
		{
			private static readonly ConcurrentQueue<PooledEnumerable<T>> _Buffer = new ConcurrentQueue<PooledEnumerable<T>>();

			public static PooledEnumerable<T> Instantiate(Map map, Rectangle2D bounds, PooledEnumeration.Selector<T> selector)
			{
				if (!_Buffer.TryDequeue(out var e))
				{
					e = new PooledEnumerable<T>();
				}

				foreach (var s in PooledEnumeration.EnumerateSectors(map, bounds))
				{
					foreach (var o in selector(s, bounds))
					{
						e._Pool.Add(o);
					}
				}

				return e;
			}

			private bool _IsDisposed;

			private HashSet<T> _Pool = new HashSet<T>(0x40);

			private PooledEnumerable()
			{ }

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _Pool.GetEnumerator();
			}

			public IEnumerator<T> GetEnumerator()
			{
				return _Pool.GetEnumerator();
			}

			public void Free()
			{
				if (_IsDisposed)
				{
					return;
				}

				_Pool.Clear();

				_Buffer.Enqueue(this);
			}

			public void Dispose()
			{
				_IsDisposed = true;

				_Pool.Clear();
				_Pool.TrimExcess();
				_Pool = null;
			}
		}
	}
}
