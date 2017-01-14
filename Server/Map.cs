#region Header
// **********
// ServUO - Map.cs
// **********
#endregion

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
//#define Map_AllUpdates

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
//#define Map_UseMaxRange

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
//#define Map_InternalProtection
#endregion

#region References

#region References
using System;
using System.Collections;
using System.Collections.Generic;

using Server.Items;
using Server.Network;
using Server.Targeting;
#endregion

#if Map_NewEnumerables || Map_PoolFixColumn || Map_InternalProtection || Map_AllUpdates
using System.Linq;
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

	public interface IPooledEnumerator<T> : IEnumerator<T>
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
			return s.Clients.Where(o => o != null && o.Mobile != null && !o.Mobile.Deleted && bounds.Contains(o.Mobile));
		}

		public static IEnumerable<IEntity> SelectEntities(Sector s, Rectangle2D bounds)
		{
			return
				Enumerable.Empty<IEntity>()
						  .Union(s.Mobiles.Where(o => o != null && !o.Deleted))
						  .Union(s.Items.Where(o => o != null && !o.Deleted && o.Parent == null))
						  .Where(bounds.Contains);
		}

		public static IEnumerable<Mobile> SelectMobiles(Sector s, Rectangle2D bounds)
		{
			return s.Mobiles.Where(o => o != null && !o.Deleted && bounds.Contains(o));
		}

		public static IEnumerable<Item> SelectItems(Sector s, Rectangle2D bounds)
		{
			return s.Items.Where(o => o != null && !o.Deleted && o.Parent == null && bounds.Contains(o));
		}

		public static IEnumerable<BaseMulti> SelectMultis(Sector s, Rectangle2D bounds)
		{
			return s.Multis.Where(o => o != null && !o.Deleted && bounds.Contains(o.Location));
		}

		public static IEnumerable<StaticTile[]> SelectMultiTiles(Sector s, Rectangle2D bounds)
		{
			foreach (var o in s.Multis.Where(o => o != null && !o.Deleted))
			{
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

			int x1 = bounds.Start.X, y1 = bounds.Start.Y, x2 = bounds.End.X, y2 = bounds.End.Y, xSector, ySector;

			if (!Bound(map, ref x1, ref y1, ref x2, ref y2, out xSector, out ySector))
			{
				yield break;
			}

			Sector s;

			var index = 0;

			while (NextSector(map, x1, y1, x2, y2, ref index, ref xSector, ref ySector, out s))
			{
				yield return s;
			}
		}

		public static bool Bound(Map map, ref int x1, ref int y1, ref int x2, ref int y2, out int xSector, out int ySector)
		{
			if (map == null || map == Map.Internal)
			{
				xSector = ySector = 0;
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

		private static bool NextSector(
			Map map,
			int x1,
			int y1,
			int x2,
			int y2,
			ref int index,
			ref int xSector,
			ref int ySector,
			out Sector s)
		{
			if (map == null)
			{
				s = null;
				xSector = ySector = 0;
				return false;
			}

			if (map == Map.Internal)
			{
				s = map.InvalidSector;
				xSector = ySector = 0;
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
	public sealed class Map : IComparable, IComparable<Map>
	{
		#region Compile-Time -> Run-Time Support
#if Map_NewEnumerables || Map_AllUpdates
		public static readonly bool NewEnumerables = true;
#else
		public static readonly bool NewEnumerables = false;
#endif

#if Map_UseMaxRange || Map_AllUpdates
		public static readonly bool UseMaxRange = true;
#else
		public static readonly bool UseMaxRange = false;
#endif

#if Map_PoolFixColumn || Map_AllUpdates
		public static readonly bool PoolFixColumn = true;
#else
		public static readonly bool PoolFixColumn = false;
#endif

#if Map_InternalProtection || Map_AllUpdates
		public static readonly bool InternalProtection = true;
#else
		public static readonly bool InternalProtection = false;
#endif
		#endregion

		public const int SectorSize = 16;
		public const int SectorShift = 4;
		public static int SectorActiveRange = 2;

		private static readonly Map[] m_Maps = new Map[0x100];

		public static Map[] Maps { get { return m_Maps; } }

		public static Map Felucca { get { return m_Maps[0]; } }
		public static Map Trammel { get { return m_Maps[1]; } }
		public static Map Ilshenar { get { return m_Maps[2]; } }
		public static Map Malas { get { return m_Maps[3]; } }
		public static Map Tokuno { get { return m_Maps[4]; } }
		public static Map TerMur { get { return m_Maps[5]; } }
		public static Map Internal { get { return m_Maps[0x7F]; } }

		private static readonly List<Map> m_AllMaps = new List<Map>();

		public static List<Map> AllMaps { get { return m_AllMaps; } }

		private readonly int m_MapID;
		private readonly int m_MapIndex;
		private readonly int m_FileIndex;

		private readonly int m_Width;
		private readonly int m_Height;
		private readonly int m_SectorsWidth;
		private readonly int m_SectorsHeight;
		private readonly Dictionary<string, Region> m_Regions;
		private Region m_DefaultRegion;

		public int Season { get; set; }

		private readonly Sector[][] m_Sectors;
		private readonly Sector m_InvalidSector;

		private TileMatrix m_Tiles;

#if Map_InternalProtection || Map_AllUpdates
		public static string[] GetMapNames()
		{
			return m_Maps.Where(m => m != null).Select(m => m.Name).ToArray();
		}

		public static Map[] GetMapValues()
		{
			return m_Maps.Where(m => m != null).ToArray();
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

			int index;

			if (!Int32.TryParse(value, out index))
			{
				return m_Maps.FirstOrDefault(m => m != null && Insensitive.Equals(m.Name, value));
			}

			if (index == 127)
			{
				return Internal;
			}

			return m_Maps.FirstOrDefault(m => m != null && m.MapIndex == index);
		}

		public override string ToString()
		{
			return Name;
		}
#else
		private static string[] m_MapNames;
		private static Map[] m_MapValues;

		public static string[] GetMapNames()
		{
			CheckNamesAndValues();
			return m_MapNames;
		}

		public static Map[] GetMapValues()
		{
			CheckNamesAndValues();
			return m_MapValues;
		}

		public static Map Parse(string value)
		{
			CheckNamesAndValues();

			for (int i = 0; i < m_MapNames.Length; ++i)
			{
				if (Insensitive.Equals(m_MapNames[i], value))
				{
					return m_MapValues[i];
				}
			}

			int index;

			if (int.TryParse(value, out index))
			{
				if (index >= 0 && index < m_Maps.Length && m_Maps[index] != null)
				{
					return m_Maps[index];
				}
			}

			throw new ArgumentException("Invalid map name");
		}

		private static void CheckNamesAndValues()
		{
			if (m_MapNames != null && m_MapNames.Length == m_AllMaps.Count)
			{
				return;
			}

			m_MapNames = new string[m_AllMaps.Count];
			m_MapValues = new Map[m_AllMaps.Count];

			for (int i = 0; i < m_AllMaps.Count; ++i)
			{
				Map map = m_AllMaps[i];

				m_MapNames[i] = map.Name;
				m_MapValues[i] = map;
			}
		}

		public override string ToString()
		{
			return Name;
		}
#endif

		public int GetAverageZ(int x, int y)
		{
			int z = 0, avg = 0, top = 0;

			GetAverageZ(x, y, ref z, ref avg, ref top);

			return avg;
		}

		public void GetAverageZ(int x, int y, ref int z, ref int avg, ref int top)
		{
			int zTop = Tiles.GetLandTile(x, y).Z;
			int zLeft = Tiles.GetLandTile(x, y + 1).Z;
			int zRight = Tiles.GetLandTile(x + 1, y).Z;
			int zBottom = Tiles.GetLandTile(x + 1, y + 1).Z;

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
			int v = a + b;

			if (v < 0)
			{
				--v;
			}

			return (v / 2);
		}

#if Map_NewEnumerables || Map_AllUpdates

		#region Get*InRange/Bounds
		public IPooledEnumerable<IEntity> GetObjectsInRange(Point3D p)
		{
#if Map_UseMaxRange || Map_AllUpdates
			return GetObjectsInRange(p, Core.GlobalMaxUpdateRange);
#else
			return GetObjectsInRange(p, 18);
#endif
		}

		public IPooledEnumerable<IEntity> GetObjectsInRange(Point3D p, int range)
		{
			return GetObjectsInBounds(new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1));
		}

		public IPooledEnumerable<IEntity> GetObjectsInBounds(Rectangle2D bounds)
		{
			return PooledEnumeration.GetEntities(this, bounds);
		}

		public IPooledEnumerable<NetState> GetClientsInRange(Point3D p)
		{
#if Map_UseMaxRange || Map_AllUpdates
			return GetClientsInRange(p, Core.GlobalMaxUpdateRange);
#else
			return GetClientsInRange(p, 18);
#endif
		}

		public IPooledEnumerable<NetState> GetClientsInRange(Point3D p, int range)
		{
			return GetClientsInBounds(new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1));
		}

		public IPooledEnumerable<NetState> GetClientsInBounds(Rectangle2D bounds)
		{
			return PooledEnumeration.GetClients(this, bounds);
		}

		public IPooledEnumerable<Item> GetItemsInRange(Point3D p)
		{
#if Map_UseMaxRange || Map_AllUpdates
			return GetItemsInRange(p, Core.GlobalMaxUpdateRange);
#else
			return GetItemsInRange(p, 18);
#endif
		}

		public IPooledEnumerable<Item> GetItemsInRange(Point3D p, int range)
		{
			return GetItemsInBounds(new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1));
		}

		public IPooledEnumerable<Item> GetItemsInBounds(Rectangle2D bounds)
		{
			return PooledEnumeration.GetItems(this, bounds);
		}

		public IPooledEnumerable<Mobile> GetMobilesInRange(Point3D p)
		{
#if Map_UseMaxRange || Map_AllUpdates
			return GetMobilesInRange(p, Core.GlobalMaxUpdateRange);
#else
			return GetMobilesInRange(p, 18);
#endif
		}

		public IPooledEnumerable<Mobile> GetMobilesInRange(Point3D p, int range)
		{
			return GetMobilesInBounds(new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1));
		}

		public IPooledEnumerable<Mobile> GetMobilesInBounds(Rectangle2D bounds)
		{
			return PooledEnumeration.GetMobiles(this, bounds);
		}
		#endregion

		public IPooledEnumerable<StaticTile[]> GetMultiTilesAt(int x, int y)
		{
			return PooledEnumeration.GetMultiTiles(this, new Rectangle2D(x, y, 1, 1));
		}
#else

		#region Get*InRange/Bounds
		public IPooledEnumerable<IEntity> GetObjectsInRange(Point3D p)
		{
#if Map_UseMaxRange || Map_AllUpdates
			return GetObjectsInRange(p, Core.GlobalMaxUpdateRange);
#else
			return GetObjectsInRange(p, 18);
#endif
		}

		public IPooledEnumerable<IEntity> GetObjectsInRange(Point3D p, int range)
		{
			if (this == Internal)
			{
				return NullEnumerable<IEntity>.Instance;
			}

			return
				PooledEnumerable<IEntity>.Instantiate(
					EntityEnumerator.Instantiate(this, new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1)));
		}

		public IPooledEnumerable<IEntity> GetObjectsInBounds(Rectangle2D bounds)
		{
			if (this == Internal)
			{
				return NullEnumerable<IEntity>.Instance;
			}

			return PooledEnumerable<IEntity>.Instantiate(EntityEnumerator.Instantiate(this, bounds));
		}

		public IPooledEnumerable<NetState> GetClientsInRange(Point3D p)
		{
#if Map_UseMaxRange || Map_AllUpdates
			return GetClientsInRange(p, Core.GlobalMaxUpdateRange);
#else
			return GetClientsInRange(p, 18);
#endif
		}

		public IPooledEnumerable<NetState> GetClientsInRange(Point3D p, int range)
		{
			if (this == Internal)
			{
				return NullEnumerable<NetState>.Instance;
			}

			return
				PooledEnumerable<NetState>.Instantiate(
					ClientEnumerator.Instantiate(this, new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1)));
		}

		public IPooledEnumerable<NetState> GetClientsInBounds(Rectangle2D bounds)
		{
			if (this == Internal)
			{
				return NullEnumerable<NetState>.Instance;
			}

			return PooledEnumerable<NetState>.Instantiate(ClientEnumerator.Instantiate(this, bounds));
		}

		public IPooledEnumerable<Item> GetItemsInRange(Point3D p)
		{
#if Map_UseMaxRange || Map_AllUpdates
			return GetItemsInRange(p, Core.GlobalMaxUpdateRange);
#else
			return GetItemsInRange(p, 18);
#endif
		}

		public IPooledEnumerable<Item> GetItemsInRange(Point3D p, int range)
		{
			if (this == Internal)
			{
				return NullEnumerable<Item>.Instance;
			}

			return
				PooledEnumerable<Item>.Instantiate(
					ItemEnumerator.Instantiate(this, new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1)));
		}

		public IPooledEnumerable<Item> GetItemsInBounds(Rectangle2D bounds)
		{
			if (this == Internal)
			{
				return NullEnumerable<Item>.Instance;
			}

			return PooledEnumerable<Item>.Instantiate(ItemEnumerator.Instantiate(this, bounds));
		}

		public IPooledEnumerable<Mobile> GetMobilesInRange(Point3D p)
		{
#if Map_UseMaxRange || Map_AllUpdates
			return GetMobilesInRange(p, Core.GlobalMaxUpdateRange);
#else
			return GetMobilesInRange(p, 18);
#endif
		}

		public IPooledEnumerable<Mobile> GetMobilesInRange(Point3D p, int range)
		{
			if (this == Internal)
			{
				return NullEnumerable<Mobile>.Instance;
			}

			return
				PooledEnumerable<Mobile>.Instantiate(
					MobileEnumerator.Instantiate(this, new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1)));
		}

		public IPooledEnumerable<Mobile> GetMobilesInBounds(Rectangle2D bounds)
		{
			if (this == Internal)
			{
				return NullEnumerable<Mobile>.Instance;
			}

			return PooledEnumerable<Mobile>.Instantiate(MobileEnumerator.Instantiate(this, bounds));
		}
		#endregion

		public IPooledEnumerable<StaticTile[]> GetMultiTilesAt(int x, int y)
		{
			if (this == Internal)
			{
				return NullEnumerable<StaticTile[]>.Instance;
			}

			Sector sector = GetSector(x, y);

			if (sector.Multis.Count == 0)
			{
				return NullEnumerable<StaticTile[]>.Instance;
			}

			return PooledEnumerable<StaticTile[]>.Instantiate(MultiTileEnumerator.Instantiate(sector, new Point2D(x, y)));
		}
#endif

		#region CanFit
		public bool CanFit(Point3D p, int height, bool checkBlocksFit)
		{
			return CanFit(p.m_X, p.m_Y, p.m_Z, height, checkBlocksFit, true, true);
		}

		public bool CanFit(Point3D p, int height, bool checkBlocksFit, bool checkMobiles)
		{
			return CanFit(p.m_X, p.m_Y, p.m_Z, height, checkBlocksFit, checkMobiles, true);
		}

		public bool CanFit(Point2D p, int z, int height, bool checkBlocksFit)
		{
			return CanFit(p.m_X, p.m_Y, z, height, checkBlocksFit, true, true);
		}

		public bool CanFit(Point3D p, int height)
		{
			return CanFit(p.m_X, p.m_Y, p.m_Z, height, false, true, true);
		}

		public bool CanFit(Point2D p, int z, int height)
		{
			return CanFit(p.m_X, p.m_Y, z, height, false, true, true);
		}

		public bool CanFit(int x, int y, int z, int height)
		{
			return CanFit(x, y, z, height, false, true, true);
		}

		public bool CanFit(int x, int y, int z, int height, bool checksBlocksFit)
		{
			return CanFit(x, y, z, height, checksBlocksFit, true, true);
		}

		public bool CanFit(int x, int y, int z, int height, bool checkBlocksFit, bool checkMobiles)
		{
			return CanFit(x, y, z, height, checkBlocksFit, checkMobiles, true);
		}

		public bool CanFit(int x, int y, int z, int height, bool checkBlocksFit, bool checkMobiles, bool requireSurface)
		{
			if (this == Internal)
			{
				return false;
			}

			if (x < 0 || y < 0 || x >= m_Width || y >= m_Height)
			{
				return false;
			}

			bool hasSurface = false;

			LandTile lt = Tiles.GetLandTile(x, y);
			int lowZ = 0, avgZ = 0, topZ = 0;

			GetAverageZ(x, y, ref lowZ, ref avgZ, ref topZ);
			TileFlag landFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;

			if ((landFlags & TileFlag.Impassable) != 0 && avgZ > z && (z + height) > lowZ)
			{
				return false;
			}
			else if ((landFlags & TileFlag.Impassable) == 0 && z == avgZ && !lt.Ignored)
			{
				hasSurface = true;
			}

			StaticTile[] staticTiles = Tiles.GetStaticTiles(x, y, true);

			bool surface, impassable;

			for (int i = 0; i < staticTiles.Length; ++i)
			{
				ItemData id = TileData.ItemTable[staticTiles[i].ID & TileData.MaxItemValue];
				surface = id.Surface;
				impassable = id.Impassable;

				if ((surface || impassable) && (staticTiles[i].Z + id.CalcHeight) > z && (z + height) > staticTiles[i].Z)
				{
					return false;
				}
				else if (surface && !impassable && z == (staticTiles[i].Z + id.CalcHeight))
				{
					hasSurface = true;
				}
			}

			Sector sector = GetSector(x, y);
			List<Item> items = sector.Items;
			List<Mobile> mobs = sector.Mobiles;

			for (int i = 0; i < items.Count; ++i)
			{
				Item item = items[i];

				if (!(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint(x, y))
				{
					ItemData id = item.ItemData;
					surface = id.Surface;
					impassable = id.Impassable;

					if ((surface || impassable || (checkBlocksFit && item.BlocksFit)) && (item.Z + id.CalcHeight) > z &&
						(z + height) > item.Z)
					{
						return false;
					}
					else if (surface && !impassable && !item.Movable && z == (item.Z + id.CalcHeight))
					{
						hasSurface = true;
					}
				}
			}

			if (checkMobiles)
			{
				for (int i = 0; i < mobs.Count; ++i)
				{
					Mobile m = mobs[i];

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
		#endregion

		#region CanSpawnMobile
		public bool CanSpawnMobile(Point3D p)
		{
			return CanSpawnMobile(p.m_X, p.m_Y, p.m_Z);
		}

		public bool CanSpawnMobile(Point2D p, int z)
		{
			return CanSpawnMobile(p.m_X, p.m_Y, z);
		}

		public bool CanSpawnMobile(int x, int y, int z)
		{
			if (!Region.Find(new Point3D(x, y, z), this).AllowSpawn())
			{
				return false;
			}

			return CanFit(x, y, z, 16);
		}
        #endregion

        public Point3D GetSpawnPosition(Point3D center, int range)
        {
            for (int i = 0; i < 10; i++)
            {
                int x = center.X + (Utility.Random((range * 2) + 1) - range);
                int y = center.Y + (Utility.Random((range * 2) + 1) - range);
                int z = GetAverageZ(x, y);

                if (CanSpawnMobile(new Point2D(x, y), center.Z))
                    return new Point3D(x, y, center.Z);
                else if (CanSpawnMobile(new Point2D(x, y), z))
                    return new Point3D(x, y, z);
            }

            return center;
        }

        private class ZComparer : IComparer<Item>
		{
			public static readonly ZComparer Default = new ZComparer();

			public int Compare(Item x, Item y)
			{
				return x.Z.CompareTo(y.Z);
			}
		}

#if Map_PoolFixColumn || Map_AllUpdates
		private static readonly Queue<List<Item>> _FixPool = new Queue<List<Item>>(128);

		private static readonly List<Item> _EmptyFixItems = new List<Item>();

		private static List<Item> AcquireFixItems(Map map, int x, int y)
		{
			if (map == null || map == Internal || x < 0 || x > map.Width || y < 0 || y > map.Height)
			{
				return _EmptyFixItems;
			}

			List<Item> pool = null;

			lock (_FixPool)
			{
				if (_FixPool.Count > 0)
				{
					pool = _FixPool.Dequeue();
				}
			}

			if (pool == null)
			{
				pool = new List<Item>(128); // Arbitrary limit
			}

			var eable = map.GetItemsInRange(new Point3D(x, y, 0), 0);

			pool.AddRange(
				eable.Where(item => item.ItemID <= TileData.MaxItemValue && !(item is BaseMulti))
					 .OrderBy(item => item.Z)
					 .Take(pool.Capacity));

			eable.Free();

			return pool;
		}

		private static void FreeFixItems(List<Item> pool)
		{
			if (pool == _EmptyFixItems)
			{
				return;
			}

			pool.Clear();

			lock (_FixPool)
			{
				if (_FixPool.Count < 128)
				{
					_FixPool.Enqueue(pool);
				}
			}
		}

		public void FixColumn(int x, int y)
		{
			var landTile = Tiles.GetLandTile(x, y);
			var tiles = Tiles.GetStaticTiles(x, y, true);

			int landZ = 0, landAvg = 0, landTop = 0;
			GetAverageZ(x, y, ref landZ, ref landAvg, ref landTop);

			var items = AcquireFixItems(this, x, y);

			for (var i = 0; i < items.Count; i++)
			{
				var toFix = items[i];

				if (!toFix.Movable)
				{
					continue;
				}

				var z = int.MinValue;
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

				for (var j = 0; j < items.Count; ++j)
				{
					if (j == i)
					{
						continue;
					}

					var item = items[j];
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

				if (z != int.MinValue)
				{
					toFix.Location = new Point3D(toFix.X, toFix.Y, z);
				}
			}

			FreeFixItems(items);
		}
#else
		public void FixColumn(int x, int y)
		{
			LandTile landTile = Tiles.GetLandTile(x, y);

			int landZ = 0, landAvg = 0, landTop = 0;
			GetAverageZ(x, y, ref landZ, ref landAvg, ref landTop);

			StaticTile[] tiles = Tiles.GetStaticTiles(x, y, true);

			List<Item> items = new List<Item>();

			IPooledEnumerable<Item> eable = GetItemsInRange(new Point3D(x, y, 0), 0);

			foreach (Item item in eable)
			{
				if (!(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue)
				{
					items.Add(item);

					if (items.Count > 100)
					{
						break;
					}
				}
			}

			eable.Free();

			if (items.Count > 100)
			{
				return;
			}

			items.Sort(ZComparer.Default);

			for (int i = 0; i < items.Count; ++i)
			{
				Item toFix = items[i];

				if (!toFix.Movable)
				{
					continue;
				}

				int z = int.MinValue;
				int currentZ = toFix.Z;

				if (!landTile.Ignored && landAvg <= currentZ)
				{
					z = landAvg;
				}

				for (int j = 0; j < tiles.Length; ++j)
				{
					StaticTile tile = tiles[j];
					ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

					int checkZ = tile.Z;
					int checkTop = checkZ + id.CalcHeight;

					if (checkTop == checkZ && !id.Surface)
					{
						++checkTop;
					}

					if (checkTop > z && checkTop <= currentZ)
					{
						z = checkTop;
					}
				}

				for (int j = 0; j < items.Count; ++j)
				{
					if (j == i)
					{
						continue;
					}

					Item item = items[j];
					ItemData id = item.ItemData;

					int checkZ = item.Z;
					int checkTop = checkZ + id.CalcHeight;

					if (checkTop == checkZ && !id.Surface)
					{
						++checkTop;
					}

					if (checkTop > z && checkTop <= currentZ)
					{
						z = checkTop;
					}
				}

				if (z != int.MinValue)
				{
					toFix.Location = new Point3D(toFix.X, toFix.Y, z);
				}
			}
		}
#endif

		/* This could be probably be re-implemented if necessary (perhaps via an ITile interface?).
		public List<Tile> GetTilesAt( Point2D p, bool items, bool land, bool statics )
		{
			List<Tile> list = new List<Tile>();

			if ( this == Map.Internal )
				return list;

			if ( land )
				list.Add( Tiles.GetLandTile( p.m_X, p.m_Y ) );

			if ( statics )
				list.AddRange( Tiles.GetStaticTiles( p.m_X, p.m_Y, true ) );

			if ( items )
			{
				Sector sector = GetSector( p );

				foreach ( Item item in sector.Items )
					if ( item.AtWorldPoint( p.m_X, p.m_Y ) )
						list.Add( new StaticTile( (ushort)item.ItemID, (sbyte) item.Z ) );
			}

			return list;
		}
		*/

		/// <summary>
		///     Gets the highest surface that is lower than <paramref name="p" />.
		/// </summary>
		/// <param name="p">The reference point.</param>
		/// <returns>A surface <typeparamref name="Tile" /> or <typeparamref name="Item" />.</returns>
		public object GetTopSurface(Point3D p)
		{
			if (this == Internal)
			{
				return null;
			}

			object surface = null;
			int surfaceZ = int.MinValue;

			LandTile lt = Tiles.GetLandTile(p.X, p.Y);

			if (!lt.Ignored)
			{
				int avgZ = GetAverageZ(p.X, p.Y);

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

			StaticTile[] staticTiles = Tiles.GetStaticTiles(p.X, p.Y, true);

			for (int i = 0; i < staticTiles.Length; i++)
			{
				StaticTile tile = staticTiles[i];
				ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				if (id.Surface || (id.Flags & TileFlag.Wet) != 0)
				{
					int tileZ = tile.Z + id.CalcHeight;

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

			Sector sector = GetSector(p.X, p.Y);

			for (int i = 0; i < sector.Items.Count; i++)
			{
				Item item = sector.Items[i];

				if (!(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint(p.X, p.Y) && !item.Movable)
				{
					ItemData id = item.ItemData;

					if (id.Surface || (id.Flags & TileFlag.Wet) != 0)
					{
						int itemZ = item.Z + id.CalcHeight;

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
			}

			return surface;
		}

		public void Bound(int x, int y, out int newX, out int newY)
		{
			if (x < 0)
			{
				newX = 0;
			}
			else if (x >= m_Width)
			{
				newX = m_Width - 1;
			}
			else
			{
				newX = x;
			}

			if (y < 0)
			{
				newY = 0;
			}
			else if (y >= m_Height)
			{
				newY = m_Height - 1;
			}
			else
			{
				newY = y;
			}
		}

		public Point2D Bound(Point2D p)
		{
			int x = p.m_X, y = p.m_Y;

			if (x < 0)
			{
				x = 0;
			}
			else if (x >= m_Width)
			{
				x = m_Width - 1;
			}

			if (y < 0)
			{
				y = 0;
			}
			else if (y >= m_Height)
			{
				y = m_Height - 1;
			}

			return new Point2D(x, y);
		}

		public Map(int mapID, int mapIndex, int fileIndex, int width, int height, int season, string name, MapRules rules)
		{
			m_MapID = mapID;
			m_MapIndex = mapIndex;
			m_FileIndex = fileIndex;
			m_Width = width;
			m_Height = height;
			Season = season;
			Name = name;
			Rules = rules;
			m_Regions = new Dictionary<string, Region>(StringComparer.OrdinalIgnoreCase);
			m_InvalidSector = new Sector(0, 0, this);
			m_SectorsWidth = width >> SectorShift;
			m_SectorsHeight = height >> SectorShift;
			m_Sectors = new Sector[m_SectorsWidth][];
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
			if (x >= 0 && x < m_SectorsWidth && y >= 0 && y < m_SectorsHeight)
			{
				Sector[] xSectors = m_Sectors[x];

				if (xSectors == null)
				{
					m_Sectors[x] = xSectors = new Sector[m_SectorsHeight];
				}

				Sector sec = xSectors[y];

				if (sec == null)
				{
					xSectors[y] = sec = new Sector(x, y, this);
				}

				return sec;
			}
			else
			{
				return m_InvalidSector;
			}
		}
		#endregion

		public void ActivateSectors(int cx, int cy)
		{
			for (int x = cx - SectorActiveRange; x <= cx + SectorActiveRange; ++x)
			{
				for (int y = cy - SectorActiveRange; y <= cy + SectorActiveRange; ++y)
				{
					Sector sect = GetRealSector(x, y);
					if (sect != m_InvalidSector)
					{
						sect.Activate();
					}
				}
			}
		}

		public void DeactivateSectors(int cx, int cy)
		{
			for (int x = cx - SectorActiveRange; x <= cx + SectorActiveRange; ++x)
			{
				for (int y = cy - SectorActiveRange; y <= cy + SectorActiveRange; ++y)
				{
					Sector sect = GetRealSector(x, y);
					if (sect != m_InvalidSector && !PlayersInRange(sect, SectorActiveRange))
					{
						sect.Deactivate();
					}
				}
			}
		}

		private bool PlayersInRange(Sector sect, int range)
		{
			for (int x = sect.X - range; x <= sect.X + range; ++x)
			{
				for (int y = sect.Y - range; y <= sect.Y + range; ++y)
				{
					Sector check = GetRealSector(x, y);
					if (check != m_InvalidSector && check.Players.Count > 0)
					{
						return true;
					}
				}
			}

			return false;
		}

		public void OnClientChange(NetState oldState, NetState newState, Mobile m)
		{
			if (this == Internal)
			{
				return;
			}

			GetSector(m).OnClientChange(oldState, newState);
		}

		public void OnEnter(Mobile m)
		{
			if (this == Internal)
			{
				return;
			}

			Sector sector = GetSector(m);

			sector.OnEnter(m);
		}

		public void OnEnter(Item item)
		{
			if (this == Internal)
			{
				return;
			}

			GetSector(item).OnEnter(item);

			if (item is BaseMulti)
			{
				BaseMulti m = (BaseMulti)item;
				MultiComponentList mcl = m.Components;

				Sector start = GetMultiMinSector(item.Location, mcl);
				Sector end = GetMultiMaxSector(item.Location, mcl);

				AddMulti(m, start, end);
			}
		}

		public void OnLeave(Mobile m)
		{
			if (this == Internal)
			{
				return;
			}

			Sector sector = GetSector(m);

			sector.OnLeave(m);
		}

		public void OnLeave(Item item)
		{
			if (this == Internal)
			{
				return;
			}

			GetSector(item).OnLeave(item);

			if (item is BaseMulti)
			{
				BaseMulti m = (BaseMulti)item;
				MultiComponentList mcl = m.Components;

				Sector start = GetMultiMinSector(item.Location, mcl);
				Sector end = GetMultiMaxSector(item.Location, mcl);

				RemoveMulti(m, start, end);
			}
		}

		public void RemoveMulti(BaseMulti m, Sector start, Sector end)
		{
			if (this == Internal)
			{
				return;
			}

			for (int x = start.X; x <= end.X; ++x)
			{
				for (int y = start.Y; y <= end.Y; ++y)
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

			for (int x = start.X; x <= end.X; ++x)
			{
				for (int y = start.Y; y <= end.Y; ++y)
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

		public void OnMove(Point3D oldLocation, Mobile m)
		{
			if (this == Internal)
			{
				return;
			}

			Sector oldSector = GetSector(oldLocation);
			Sector newSector = GetSector(m.Location);

			if (oldSector != newSector)
			{
				oldSector.OnLeave(m);
				newSector.OnEnter(m);
			}
		}

		public void OnMove(Point3D oldLocation, Item item)
		{
			if (this == Internal)
			{
				return;
			}

			Sector oldSector = GetSector(oldLocation);
			Sector newSector = GetSector(item.Location);

			if (oldSector != newSector)
			{
				oldSector.OnLeave(item);
				newSector.OnEnter(item);
			}

			if (item is BaseMulti)
			{
				BaseMulti m = (BaseMulti)item;
				MultiComponentList mcl = m.Components;

				Sector start = GetMultiMinSector(item.Location, mcl);
				Sector end = GetMultiMaxSector(item.Location, mcl);

				Sector oldStart = GetMultiMinSector(oldLocation, mcl);
				Sector oldEnd = GetMultiMaxSector(oldLocation, mcl);

				if (oldStart != start || oldEnd != end)
				{
					RemoveMulti(m, oldStart, oldEnd);
					AddMulti(m, start, end);
				}
			}
		}

		private readonly object tileLock = new object();

		public TileMatrix Tiles
		{
			get
			{
				if (m_Tiles != null)
				{
					return m_Tiles;
				}

				lock (tileLock)
					return m_Tiles ?? (m_Tiles = new TileMatrix(this, m_FileIndex, m_MapID, m_Width, m_Height));
			}
		}

		public int MapID { get { return m_MapID; } }

		public int MapIndex { get { return m_MapIndex; } }

		public int Width { get { return m_Width; } }

		public int Height { get { return m_Height; } }

		public Dictionary<string, Region> Regions { get { return m_Regions; } }

		public void RegisterRegion(Region reg)
		{
			string regName = reg.Name;

			if (regName != null)
			{
				if (m_Regions.ContainsKey(regName))
				{
					Console.WriteLine("Warning: Duplicate region name '{0}' for map '{1}'", regName, this.Name);
				}
				else
				{
					m_Regions[regName] = reg;
				}
			}
		}

		public void UnregisterRegion(Region reg)
		{
			string regName = reg.Name;

			if (regName != null)
			{
				m_Regions.Remove(regName);
			}
		}

		public Region DefaultRegion
		{
			get
			{
				if (m_DefaultRegion == null)
				{
					m_DefaultRegion = new Region(null, this, 0, new Rectangle3D[0]);
				}

				return m_DefaultRegion;
			}
			set { m_DefaultRegion = value; }
		}

		public MapRules Rules { get; set; }

		public Sector InvalidSector { get { return m_InvalidSector; } }

		public string Name { get; set; }

#if Map_NewEnumerables || Map_AllUpdates
		public class NullEnumerable<T> : IPooledEnumerable<T>
		{
			public static readonly NullEnumerable<T> Instance = new NullEnumerable<T>();

			private readonly IEnumerable<T> _Empty;

			private NullEnumerable()
			{
				_Empty = Enumerable.Empty<T>();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _Empty.GetEnumerator();
			}

			public IEnumerator<T> GetEnumerator()
			{
				return _Empty.GetEnumerator();
			}

			public void Free()
			{ }
		}

		public sealed class PooledEnumerable<T> : IPooledEnumerable<T>, IDisposable
		{
			private static readonly Queue<PooledEnumerable<T>> _Buffer = new Queue<PooledEnumerable<T>>(0x400);

			public static PooledEnumerable<T> Instantiate(Map map, Rectangle2D bounds, PooledEnumeration.Selector<T> selector)
			{
				PooledEnumerable<T> e = null;

				lock (((ICollection)_Buffer).SyncRoot)
				{
					if (_Buffer.Count > 0)
					{
						e = _Buffer.Dequeue();
					}
				}

				var pool = PooledEnumeration.EnumerateSectors(map, bounds).SelectMany(s => selector(s, bounds));

				if (e != null)
				{
					e._Pool.AddRange(pool);
				}
				else
				{
					e = new PooledEnumerable<T>(pool);
				}

				return e;
			}

			private bool _IsDisposed;

			private List<T> _Pool = new List<T>(0x40);

			private IEnumerable<T> InternalPool
			{
				get
				{
					var i = _Pool.Count;

					while (--i >= 0)
					{
						if (i < _Pool.Count)
						{
							yield return _Pool[i];
						}
					}
				}
			}

			public PooledEnumerable(IEnumerable<T> pool)
			{
				_Pool.AddRange(pool);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return InternalPool.GetEnumerator();
			}

			public IEnumerator<T> GetEnumerator()
			{
				return InternalPool.GetEnumerator();
			}

			public void Free()
			{
				if (_IsDisposed)
				{
					return;
				}

				_Pool.Clear();

				if (_Pool.Capacity > 0x100)
				{
					_Pool.Capacity = 0x100;
				}

				lock (((ICollection)_Buffer).SyncRoot)
				{
					_Buffer.Enqueue(this);
				}
			}

			public void Dispose()
			{
				_IsDisposed = true;

				_Pool.Clear();
				_Pool.TrimExcess();
				_Pool = null;
			}
		}
#else

		#region Enumerables
		public class NullEnumerable<T> : IPooledEnumerable<T>
		{
			private readonly InternalEnumerator<T> m_Enumerator;

			public static readonly NullEnumerable<T> Instance = new NullEnumerable<T>();

			private NullEnumerable()
			{
				m_Enumerator = new InternalEnumerator<T>();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return m_Enumerator;
			}

			public IEnumerator<T> GetEnumerator()
			{
				return m_Enumerator;
			}

			public void Free()
			{ }

			private class InternalEnumerator<K> : IEnumerator<K>
			{
				public void Reset()
				{ }

				object IEnumerator.Current { get { return null; } }
				public K Current { get { return default(K); } }

				public bool MoveNext()
				{
					return false;
				}

				void IDisposable.Dispose()
				{ }
			}
		}

		private class PooledEnumerable<T> : IPooledEnumerable<T>, IDisposable
		{
			private IPooledEnumerator<T> m_Enumerator;

			private static readonly Queue<PooledEnumerable<T>> m_InstancePool = new Queue<PooledEnumerable<T>>();

			public static PooledEnumerable<T> Instantiate(IPooledEnumerator<T> etor)
			{
				PooledEnumerable<T> e = null;

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count > 0)
					{
						e = m_InstancePool.Dequeue();
						e.m_Enumerator = etor;
					}
				}

				if (e == null)
				{
					e = new PooledEnumerable<T>(etor);
				}

				return e;
			}

			private PooledEnumerable(IPooledEnumerator<T> etor)
			{
				m_Enumerator = etor;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				if (m_Enumerator == null)
				{
					throw new ObjectDisposedException("PooledEnumerable", "GetEnumerator() called after Free()");
				}

				return m_Enumerator;
			}

			public IEnumerator<T> GetEnumerator()
			{
				if (m_Enumerator == null)
				{
					throw new ObjectDisposedException("PooledEnumerable", "GetEnumerator() called after Free()");
				}

				return m_Enumerator;
			}

			public void Free()
			{
				if (m_Enumerator != null)
				{
					m_Enumerator.Free();
					m_Enumerator = null;
				}

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count < 200) // Arbitrary
					{
						m_InstancePool.Enqueue(this);
					}
				}
			}

			public void Dispose()
			{
				// Don't return disposed objects to the instance pool
				//Free();

				if (m_Enumerator != null)
				{
					m_Enumerator.Free();
					m_Enumerator = null;
				}
			}
		}
		#endregion

		#region Enumerators
		private class ClientEnumerator : IPooledEnumerator<NetState>
		{
			private Map m_Map;
			private Rectangle2D m_Bounds;

			private int m_xSector, m_ySector;
			private int m_xSectorStart, m_ySectorStart;
			private int m_xSectorEnd, m_ySectorEnd;
			private List<NetState> m_CurrentList;
			private int m_CurrentIndex;

			private static readonly Queue<ClientEnumerator> m_InstancePool = new Queue<ClientEnumerator>();

			public static ClientEnumerator Instantiate(Map map, Rectangle2D bounds)
			{
				ClientEnumerator e = null;

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count > 0)
					{
						e = m_InstancePool.Dequeue();

						e.m_Map = map;
						e.m_Bounds = bounds;
					}
				}

				if (e == null)
				{
					e = new ClientEnumerator(map, bounds);
				}

				e.Reset();

				return e;
			}

			public void Free()
			{
				if (m_Map == null)
				{
					return;
				}

				m_Map = null;

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count < 200) // Arbitrary
					{
						m_InstancePool.Enqueue(this);
					}
				}
			}

			private ClientEnumerator(Map map, Rectangle2D bounds)
			{
				m_Map = map;
				m_Bounds = bounds;
			}

			public NetState Current { get { return m_CurrentList[m_CurrentIndex]; } }

			object IEnumerator.Current { get { return m_CurrentList[m_CurrentIndex]; } }

			void IDisposable.Dispose()
			{ }

			public bool MoveNext()
			{
				while (true)
				{
					++m_CurrentIndex;

					if (m_CurrentIndex == m_CurrentList.Count)
					{
						++m_ySector;

						if (m_ySector > m_ySectorEnd)
						{
							m_ySector = m_ySectorStart;
							++m_xSector;

							if (m_xSector > m_xSectorEnd)
							{
								m_CurrentIndex = -1;
								return false;
							}
						}

						m_CurrentIndex = -1;
						m_CurrentList = m_Map.InternalGetSector(m_xSector, m_ySector).Clients;
					}
					else
					{
						Mobile m = m_CurrentList[m_CurrentIndex].Mobile;

						if (m != null && !m.Deleted && m_Bounds.Contains(m.Location))
						{
							return true;
						}
					}
				}
			}

			public void Reset()
			{
				m_Map.Bound(m_Bounds.Start.m_X, m_Bounds.Start.m_Y, out m_xSectorStart, out m_ySectorStart);
				m_Map.Bound(m_Bounds.End.m_X - 1, m_Bounds.End.m_Y - 1, out m_xSectorEnd, out m_ySectorEnd);

				m_xSector = m_xSectorStart >>= SectorShift;
				m_ySector = m_ySectorStart >>= SectorShift;

				m_xSectorEnd >>= SectorShift;
				m_ySectorEnd >>= SectorShift;

				m_CurrentIndex = -1;
				m_CurrentList = m_Map.InternalGetSector(m_xSector, m_ySector).Clients;
			}
		}

		private class EntityEnumerator : IPooledEnumerator<IEntity>
		{
			private Map m_Map;
			private Rectangle2D m_Bounds;

			private int m_xSector, m_ySector;
			private int m_xSectorStart, m_ySectorStart;
			private int m_xSectorEnd, m_ySectorEnd;
			private int m_Stage;
			private IList m_CurrentList;
			private int m_CurrentIndex;

			private static readonly Queue<EntityEnumerator> m_InstancePool = new Queue<EntityEnumerator>();

			public static EntityEnumerator Instantiate(Map map, Rectangle2D bounds)
			{
				EntityEnumerator e = null;

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count > 0)
					{
						e = m_InstancePool.Dequeue();

						e.m_Map = map;
						e.m_Bounds = bounds;
					}
				}

				if (e == null)
				{
					e = new EntityEnumerator(map, bounds);
				}

				e.Reset();

				return e;
			}

			public void Free()
			{
				if (m_Map == null)
				{
					return;
				}

				m_Map = null;

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count < 200) // Arbitrary
					{
						m_InstancePool.Enqueue(this);
					}
				}
			}

			private EntityEnumerator(Map map, Rectangle2D bounds)
			{
				m_Map = map;
				m_Bounds = bounds;
			}

			public IEntity Current { get { return (IEntity)m_CurrentList[m_CurrentIndex]; } }

			object IEnumerator.Current { get { return m_CurrentList[m_CurrentIndex]; } }

			void IDisposable.Dispose()
			{ }

			public bool MoveNext()
			{
				while (true)
				{
					++m_CurrentIndex;

					if (m_CurrentIndex < 0 || m_CurrentIndex > m_CurrentList.Count)
					{
						// Sanity
						Console.WriteLine("EntityEnumerator OOB: {0}", m_CurrentIndex);
						return false;
					}

					if (m_CurrentIndex == m_CurrentList.Count)
					{
						++m_ySector;

						if (m_ySector > m_ySectorEnd)
						{
							m_ySector = m_ySectorStart;
							++m_xSector;

							if (m_xSector > m_xSectorEnd)
							{
								if (m_Stage > 0)
								{
									m_CurrentIndex = -1;
									return false;
								}
								++m_Stage;
								m_xSector = m_xSectorStart >>= SectorShift;
								m_ySector = m_ySectorStart >>= SectorShift;
							}
						}

						m_CurrentIndex = -1;

						if (m_Stage == 0)
						{
							m_CurrentList = m_Map.InternalGetSector(m_xSector, m_ySector).Items;
						}
						else
						{
							m_CurrentList = m_Map.InternalGetSector(m_xSector, m_ySector).Mobiles;
						}
					}
					else
					{
						IEntity e = (IEntity)m_CurrentList[m_CurrentIndex];

						if (e.Deleted)
						{
							continue;
						}

						if (e is Item)
						{
							Item item = (Item)e;

							if (item.Parent != null)
							{
								continue;
							}
						}

						if (m_Bounds.Contains(e.Location))
						{
							return true;
						}
					}
				}
			}

			public void Reset()
			{
				m_Map.Bound(m_Bounds.Start.m_X, m_Bounds.Start.m_Y, out m_xSectorStart, out m_ySectorStart);
				m_Map.Bound(m_Bounds.End.m_X - 1, m_Bounds.End.m_Y - 1, out m_xSectorEnd, out m_ySectorEnd);

				m_xSector = m_xSectorStart >>= SectorShift;
				m_ySector = m_ySectorStart >>= SectorShift;

				m_xSectorEnd >>= SectorShift;
				m_ySectorEnd >>= SectorShift;

				m_CurrentIndex = -1;
				m_Stage = 0;
				m_CurrentList = m_Map.InternalGetSector(m_xSector, m_ySector).Items;
			}
		}

		private class ItemEnumerator : IPooledEnumerator<Item>
		{
			private Map m_Map;
			private Rectangle2D m_Bounds;

			private int m_xSector, m_ySector;
			private int m_xSectorStart, m_ySectorStart;
			private int m_xSectorEnd, m_ySectorEnd;
			private List<Item> m_CurrentList;
			private int m_CurrentIndex;

			private static readonly Queue<ItemEnumerator> m_InstancePool = new Queue<ItemEnumerator>();

			public static ItemEnumerator Instantiate(Map map, Rectangle2D bounds)
			{
				ItemEnumerator e = null;

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count > 0)
					{
						e = m_InstancePool.Dequeue();

						e.m_Map = map;
						e.m_Bounds = bounds;
					}
				}

				if (e == null)
				{
					e = new ItemEnumerator(map, bounds);
				}

				e.Reset();

				return e;
			}

			public void Free()
			{
				if (m_Map == null)
				{
					return;
				}

				m_Map = null;

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count < 200) // Arbitrary
					{
						m_InstancePool.Enqueue(this);
					}
				}
			}

			private ItemEnumerator(Map map, Rectangle2D bounds)
			{
				m_Map = map;
				m_Bounds = bounds;
			}

			public Item Current { get { return m_CurrentList[m_CurrentIndex]; } }

			object IEnumerator.Current { get { return m_CurrentList[m_CurrentIndex]; } }

			void IDisposable.Dispose()
			{ }

			public bool MoveNext()
			{
				while (true)
				{
					++m_CurrentIndex;

					if (m_CurrentIndex == m_CurrentList.Count)
					{
						++m_ySector;

						if (m_ySector > m_ySectorEnd)
						{
							m_ySector = m_ySectorStart;
							++m_xSector;

							if (m_xSector > m_xSectorEnd)
							{
								m_CurrentIndex = -1;
								return false;
							}
						}

						m_CurrentIndex = -1;
						m_CurrentList = m_Map.InternalGetSector(m_xSector, m_ySector).Items;
					}
					else
					{
						Item item = m_CurrentList[m_CurrentIndex];

						if (!item.Deleted && item.Parent == null && m_Bounds.Contains(item.Location))
						{
							return true;
						}
					}
				}
			}

			public void Reset()
			{
				m_Map.Bound(m_Bounds.Start.m_X, m_Bounds.Start.m_Y, out m_xSectorStart, out m_ySectorStart);
				m_Map.Bound(m_Bounds.End.m_X - 1, m_Bounds.End.m_Y - 1, out m_xSectorEnd, out m_ySectorEnd);

				m_xSector = m_xSectorStart >>= SectorShift;
				m_ySector = m_ySectorStart >>= SectorShift;

				m_xSectorEnd >>= SectorShift;
				m_ySectorEnd >>= SectorShift;

				m_CurrentIndex = -1;
				m_CurrentList = m_Map.InternalGetSector(m_xSector, m_ySector).Items;
			}
		}

		private class MobileEnumerator : IPooledEnumerator<Mobile>
		{
			private Map m_Map;
			private Rectangle2D m_Bounds;

			private int m_xSector, m_ySector;
			private int m_xSectorStart, m_ySectorStart;
			private int m_xSectorEnd, m_ySectorEnd;
			private List<Mobile> m_CurrentList;
			private int m_CurrentIndex;

			private static readonly Queue<MobileEnumerator> m_InstancePool = new Queue<MobileEnumerator>();

			public static MobileEnumerator Instantiate(Map map, Rectangle2D bounds)
			{
				MobileEnumerator e = null;

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count > 0)
					{
						e = m_InstancePool.Dequeue();

						e.m_Map = map;
						e.m_Bounds = bounds;
					}
				}

				if (e == null)
				{
					e = new MobileEnumerator(map, bounds);
				}

				e.Reset();

				return e;
			}

			public void Free()
			{
				if (m_Map == null)
				{
					return;
				}

				m_Map = null;

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count < 200) // Arbitrary
					{
						m_InstancePool.Enqueue(this);
					}
				}
			}

			private MobileEnumerator(Map map, Rectangle2D bounds)
			{
				m_Map = map;
				m_Bounds = bounds;
			}

			public Mobile Current { get { return m_CurrentList[m_CurrentIndex]; } }

			object IEnumerator.Current { get { return m_CurrentList[m_CurrentIndex]; } }

			void IDisposable.Dispose()
			{ }

			public bool MoveNext()
			{
				while (true)
				{
					++m_CurrentIndex;

					if (m_CurrentIndex == m_CurrentList.Count)
					{
						++m_ySector;

						if (m_ySector > m_ySectorEnd)
						{
							m_ySector = m_ySectorStart;
							++m_xSector;

							if (m_xSector > m_xSectorEnd)
							{
								m_CurrentIndex = -1;
								return false;
							}
						}

						m_CurrentIndex = -1;
						m_CurrentList = m_Map.InternalGetSector(m_xSector, m_ySector).Mobiles;
					}
					else
					{
						Mobile m = m_CurrentList[m_CurrentIndex];

						if (!m.Deleted && m_Bounds.Contains(m.Location))
						{
							return true;
						}
					}
				}
			}

			public void Reset()
			{
				m_Map.Bound(m_Bounds.Start.m_X, m_Bounds.Start.m_Y, out m_xSectorStart, out m_ySectorStart);
				m_Map.Bound(m_Bounds.End.m_X - 1, m_Bounds.End.m_Y - 1, out m_xSectorEnd, out m_ySectorEnd);

				m_xSector = m_xSectorStart >>= SectorShift;
				m_ySector = m_ySectorStart >>= SectorShift;

				m_xSectorEnd >>= SectorShift;
				m_ySectorEnd >>= SectorShift;

				m_CurrentIndex = -1;
				m_CurrentList = m_Map.InternalGetSector(m_xSector, m_ySector).Mobiles;
			}
		}

		private class MultiTileEnumerator : IPooledEnumerator<StaticTile[]>
		{
			private List<BaseMulti> m_List;
			private Point2D m_Location;
			private StaticTile[] m_Current;
			private int m_Index;

			private static readonly Queue<MultiTileEnumerator> m_InstancePool = new Queue<MultiTileEnumerator>();

			public static MultiTileEnumerator Instantiate(Sector sector, Point2D loc)
			{
				MultiTileEnumerator e = null;

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count > 0)
					{
						e = m_InstancePool.Dequeue();

						e.m_List = sector.Multis;
						e.m_Location = loc;
					}
				}

				if (e == null)
				{
					e = new MultiTileEnumerator(sector, loc);
				}

				e.Reset();

				return e;
			}

			private MultiTileEnumerator(Sector sector, Point2D loc)
			{
				m_List = sector.Multis;
				m_Location = loc;
			}

			public StaticTile[] Current { get { return m_Current; } }

			object IEnumerator.Current { get { return m_Current; } }

			void IDisposable.Dispose()
			{ }

			public bool MoveNext()
			{
				while (++m_Index < m_List.Count)
				{
					BaseMulti m = m_List[m_Index];

					if (m != null && !m.Deleted)
					{
						MultiComponentList list = m.Components;

						int xOffset = m_Location.m_X - (m.Location.m_X + list.Min.m_X);
						int yOffset = m_Location.m_Y - (m.Location.m_Y + list.Min.m_Y);

						if (xOffset >= 0 && xOffset < list.Width && yOffset >= 0 && yOffset < list.Height)
						{
							StaticTile[] tiles = list.Tiles[xOffset][yOffset];

							if (tiles.Length > 0)
							{
								// TODO: How to avoid this copy?
								StaticTile[] copy = new StaticTile[tiles.Length];

								for (int i = 0; i < copy.Length; ++i)
								{
									copy[i] = tiles[i];
									copy[i].Z += m.Z;
								}

								m_Current = copy;
								return true;
							}
						}
					}
				}

				return false;
			}

			public void Free()
			{
				if (m_List == null)
				{
					return;
				}

				lock (m_InstancePool)
				{
					if (m_InstancePool.Count < 200) // Arbitrary
					{
						m_InstancePool.Enqueue(this);
					}

					m_List = null;
				}
			}

			public void Reset()
			{
				m_Current = null;
				m_Index = -1;
			}
		}
		#endregion

#endif

		public Point3D GetPoint(object o, bool eye)
		{
			Point3D p;

			if (o is Mobile)
			{
				p = ((Mobile)o).Location;
				p.Z += 14; //eye ? 15 : 10;
			}
			else if (o is Item)
			{
				p = ((Item)o).GetWorldLocation();
				p.Z += (((Item)o).ItemData.Height / 2) + 1;
			}
			else if (o is Point3D)
			{
				p = (Point3D)o;
			}
			else if (o is LandTarget)
			{
				p = ((LandTarget)o).Location;

				int low = 0, avg = 0, top = 0;
				GetAverageZ(p.X, p.Y, ref low, ref avg, ref top);

				p.Z = top + 1;
			}
			else if (o is StaticTarget)
			{
				StaticTarget st = (StaticTarget)o;
				ItemData id = TileData.ItemTable[st.ItemID & TileData.MaxItemValue];

				p = new Point3D(st.X, st.Y, st.Z - id.CalcHeight + (id.Height / 2) + 1);
			}
			else if (o is IPoint3D)
			{
				p = new Point3D((IPoint3D)o);
			}
			else
			{
				Console.WriteLine("Warning: Invalid object ({0}) in line of sight", o);
				p = Point3D.Zero;
			}

			return p;
		}

		#region Line Of Sight
		private static int m_MaxLOSDistance = 25;

		public static int MaxLOSDistance { get { return m_MaxLOSDistance; } set { m_MaxLOSDistance = value; } }

		public bool LineOfSight(Point3D org, Point3D dest)
		{
			if (this == Internal)
			{
				return false;
			}

			if (!Utility.InRange(org, dest, m_MaxLOSDistance))
			{
				return false;
			}

			Point3D start = org;
			Point3D end = dest;

			if (org.X > dest.X || (org.X == dest.X && org.Y > dest.Y) || (org.X == dest.X && org.Y == dest.Y && org.Z > dest.Z))
			{
				Point3D swap = org;
				org = dest;
				dest = swap;
			}

			double rise, run, zslp;
			double sq3d;
			double x, y, z;
			int xd, yd, zd;
			int ix, iy, iz;
			int height;
			bool found;
			Point3D p;
			Point3DList path = new Point3DList();
			TileFlag flags;

			if (org == dest)
			{
				return true;
			}

			if (path.Count > 0)
			{
				path.Clear();
			}

			xd = dest.m_X - org.m_X;
			yd = dest.m_Y - org.m_Y;
			zd = dest.m_Z - org.m_Z;
			zslp = Math.Sqrt(xd * xd + yd * yd);
			if (zd != 0)
			{
				sq3d = Math.Sqrt(zslp * zslp + zd * zd);
			}
			else
			{
				sq3d = zslp;
			}

			rise = ((float)yd) / sq3d;
			run = ((float)xd) / sq3d;
			zslp = ((float)zd) / sq3d;

			y = org.m_Y;
			z = org.m_Z;
			x = org.m_X;
			while (Utility.NumberBetween(x, dest.m_X, org.m_X, 0.5) && Utility.NumberBetween(y, dest.m_Y, org.m_Y, 0.5) &&
				   Utility.NumberBetween(z, dest.m_Z, org.m_Z, 0.5))
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
				return true; //<--should never happen, but to be safe.
			}

			p = path.Last;

			if (p != dest)
			{
				path.Add(dest);
			}

			Point3D pTop = org, pBottom = dest;
			Utility.FixPoints(ref pTop, ref pBottom);

			int pathCount = path.Count;
			int endTop = end.m_Z + 1;

			for (int i = 0; i < pathCount; ++i)
			{
				Point3D point = path[i];
				int pointTop = point.m_Z + 1;

				LandTile landTile = Tiles.GetLandTile(point.X, point.Y);
				int landZ = 0, landAvg = 0, landTop = 0;
				GetAverageZ(point.m_X, point.m_Y, ref landZ, ref landAvg, ref landTop);

				if (landZ <= pointTop && landTop >= point.m_Z &&
					(point.m_X != end.m_X || point.m_Y != end.m_Y || landZ > endTop || landTop < end.m_Z) && !landTile.Ignored)
				{
					return false;
				}

				/* --Do land tiles need to be checked?  There is never land between two people, always statics.--
				LandTile landTile = Tiles.GetLandTile( point.X, point.Y );
				if ( landTile.Z-1 >= point.Z && landTile.Z+1 <= point.Z && (TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Impassable) != 0 )
					return false;
				*/

				StaticTile[] statics = Tiles.GetStaticTiles(point.m_X, point.m_Y, true);

				bool contains = false;
				int ltID = landTile.ID;

				for (int j = 0; !contains && j < m_InvalidLandTiles.Length; ++j)
				{
					contains = (ltID == m_InvalidLandTiles[j]);
				}

				if (contains && statics.Length == 0)
				{
					IPooledEnumerable<Item> eable = GetItemsInRange(point, 0);

					foreach (Item item in eable)
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

				for (int j = 0; j < statics.Length; ++j)
				{
					StaticTile t = statics[j];

					ItemData id = TileData.ItemTable[t.ID & TileData.MaxItemValue];

					flags = id.Flags;
					height = id.CalcHeight;

					if (t.Z <= pointTop && t.Z + height >= point.Z && (flags & (TileFlag.Window | TileFlag.NoShoot)) != 0)
					{
						if (point.m_X == end.m_X && point.m_Y == end.m_Y && t.Z <= endTop && t.Z + height >= end.m_Z)
						{
							continue;
						}

						return false;
					}

					/*if ( t.Z <= point.Z && t.Z+height >= point.Z && (flags&TileFlag.Window)==0 && (flags&TileFlag.NoShoot)!=0
						&& ( (flags&TileFlag.Wall)!=0 || (flags&TileFlag.Roof)!=0 || (((flags&TileFlag.Surface)!=0 && zd != 0)) ) )*/
					/*{
						//Console.WriteLine( "LoS: Blocked by Static \"{0}\" Z:{1} T:{3} P:{2} F:x{4:X}", TileData.ItemTable[t.ID&TileData.MaxItemValue].Name, t.Z, point, t.Z+height, flags );
						//Console.WriteLine( "if ( {0} && {1} && {2} && ( {3} || {4} || {5} || ({6} && {7} && {8}) ) )", t.Z <= point.Z, t.Z+height >= point.Z, (flags&TileFlag.Window)==0, (flags&TileFlag.Impassable)!=0, (flags&TileFlag.Wall)!=0, (flags&TileFlag.Roof)!=0, (flags&TileFlag.Surface)!=0, t.Z != dest.Z, zd != 0 ) ;
						return false;
					}*/
				}
			}

			Rectangle2D rect = new Rectangle2D(pTop.m_X, pTop.m_Y, (pBottom.m_X - pTop.m_X) + 1, (pBottom.m_Y - pTop.m_Y) + 1);

			IPooledEnumerable<Item> area = GetItemsInBounds(rect);

			foreach (Item i in area)
			{
				if (!i.Visible)
				{
					continue;
				}

				if (i is BaseMulti || i.ItemID > TileData.MaxItemValue)
				{
					continue;
				}

				ItemData id = i.ItemData;
				flags = id.Flags;

				if ((flags & (TileFlag.Window | TileFlag.NoShoot)) == 0)
				{
					continue;
				}

				height = id.CalcHeight;

				found = false;

				int count = path.Count;

				for (int j = 0; j < count; ++j)
				{
					Point3D point = path[j];
					int pointTop = point.m_Z + 1;
					Point3D loc = i.Location;

					//if ( t.Z <= point.Z && t.Z+height >= point.Z && ( height != 0 || ( t.Z == dest.Z && zd != 0 ) ) )
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

				if (!found)
				{
					continue;
				}

				area.Free();
				return false;

				/*if ( (flags & (TileFlag.Impassable | TileFlag.Surface | TileFlag.Roof)) != 0 )

				//flags = TileData.ItemTable[i.ItemID&TileData.MaxItemValue].Flags;
				//if ( (flags&TileFlag.Window)==0 && (flags&TileFlag.NoShoot)!=0 && ( (flags&TileFlag.Wall)!=0 || (flags&TileFlag.Roof)!=0 || (((flags&TileFlag.Surface)!=0 && zd != 0)) ) )
				{
					//height = TileData.ItemTable[i.ItemID&TileData.MaxItemValue].Height;
					//Console.WriteLine( "LoS: Blocked by ITEM \"{0}\" P:{1} T:{2} F:x{3:X}", TileData.ItemTable[i.ItemID&TileData.MaxItemValue].Name, i.Location, i.Location.Z+height, flags );
					area.Free();
					return false;
				}*/
			}

			area.Free();

			return true;
		}

		public bool LineOfSight(object from, object dest)
		{
			if (from == dest || (from is Mobile && ((Mobile)from).AccessLevel > AccessLevel.Player))
			{
				return true;
			}
			else if (dest is Item && from is Mobile && ((Item)dest).RootParent == from)
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

			Point3D eye = from.Location;

			eye.Z += 14;

			return LineOfSight(eye, target);
		}

		public bool LineOfSight(Mobile from, Mobile to)
		{
			if (from == to || from.AccessLevel > AccessLevel.Player)
			{
				return true;
			}

			Point3D eye = from.Location;
			Point3D target = to.Location;

			eye.Z += 14;
			target.Z += 14; //10;

			return LineOfSight(eye, target);
		}
		#endregion

		private static int[] m_InvalidLandTiles = new int[] {0x244};

		public static int[] InvalidLandTiles { get { return m_InvalidLandTiles; } set { m_InvalidLandTiles = value; } }

		public int CompareTo(Map other)
		{
			if (other == null)
			{
				return -1;
			}

			return m_MapID.CompareTo(other.m_MapID);
		}

		public int CompareTo(object other)
		{
			if (other == null || other is Map)
			{
				return this.CompareTo(other);
			}

			throw new ArgumentException();
		}
	}
}