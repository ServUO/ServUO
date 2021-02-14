using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Network;
using Server.Targeting;

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
			foreach (BaseMulti o in s.Multis.Where(o => o != null && !o.Deleted))
			{
				MultiComponentList c = o.Components;

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

						for (int i = 0; i < t.Length; i++)
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

			if (!Bound(map, ref x1, ref y1, ref x2, ref y2, out int xSector, out int ySector))
			{
				yield break;
			}


			int index = 0;

			while (NextSector(map, x1, y1, x2, y2, ref index, ref xSector, ref ySector, out Sector s))
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

    [Parsable]
    public class Map : IComparable, IComparable<Map>
	{
        public const int SectorSize = 16;
		public const int SectorShift = 4;
		public static int SectorActiveRange = 2;

		private static readonly Map[] m_Maps = new Map[0x100];

        public static Map[] Maps => m_Maps;

		public static Map Felucca => m_Maps[0];
		public static Map Trammel => m_Maps[1];
		public static Map Ilshenar => m_Maps[2];
		public static Map Malas => m_Maps[3];
		public static Map Tokuno => m_Maps[4];
		public static Map TerMur => m_Maps[5];
		public static Map Internal => m_Maps[0x7F];

		private static readonly List<Map> m_AllMaps = new List<Map>();

		public static List<Map> AllMaps => m_AllMaps;

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
			if (string.IsNullOrWhiteSpace(value))
			{
				return null;
			}

			if (Insensitive.Equals(value, "Internal"))
			{
				return Internal;
			}


			if (!int.TryParse(value, out int index))
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

			return v / 2;
		}

        public IPooledEnumerable<IEntity> GetObjectsInRange(Point3D p)
		{
            return GetObjectsInRange(p, Core.GlobalMaxUpdateRange);
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
            return GetClientsInRange(p, Core.GlobalMaxUpdateRange);
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
            return GetItemsInRange(p, Core.GlobalMaxUpdateRange);
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
            return GetMobilesInRange(p, Core.GlobalMaxUpdateRange);
		}

		public IPooledEnumerable<Mobile> GetMobilesInRange(Point3D p, int range)
		{
			return GetMobilesInBounds(new Rectangle2D(p.m_X - range, p.m_Y - range, range * 2 + 1, range * 2 + 1));
		}

		public IPooledEnumerable<Mobile> GetMobilesInBounds(Rectangle2D bounds)
		{
			return PooledEnumeration.GetMobiles(this, bounds);
		}

        public IPooledEnumerable<StaticTile[]> GetMultiTilesAt(int x, int y)
		{
			return PooledEnumeration.GetMultiTiles(this, new Rectangle2D(x, y, 1, 1));
		}

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

			if (x < 0 || y < 0 || x >= m_Width || y >= m_Height)
			{
				return false;
			}

			int lowZ = 0, avgZ = 0, topZ = 0;

			GetAverageZ(x, y, ref lowZ, ref avgZ, ref topZ);

			LandTile lt = Tiles.GetLandTile(x, y);

			TileFlag landFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;

			if ((landFlags & TileFlag.Impassable) != 0 && avgZ > z && (z + height) > lowZ)
			{
				return false;
			}

			bool hasSurface = (landFlags & TileFlag.Impassable) == 0 && z == avgZ && !lt.Ignored;

			StaticTile[] staticTiles = Tiles.GetStaticTiles(x, y, true);

			bool surface, impassable, roof;

			foreach (StaticTile t in staticTiles)
			{
				ItemData id = TileData.ItemTable[t.ID & TileData.MaxItemValue];
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

			Sector sector = GetSector(x, y);
			List<Item> items = sector.Items;
			List<Mobile> mobs = sector.Mobiles;

			foreach (Item item in items)
			{
				if (!(item is BaseMulti) && item.ItemID <= TileData.MaxItemValue && item.AtWorldPoint(x, y))
				{
					ItemData id = item.ItemData;

					surface = id.Surface;
					impassable = id.Impassable;

					if ((surface || impassable || checkBlocksFit && item.BlocksFit) && (item.Z + id.CalcHeight) > z &&
						(z + height) > item.Z)
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
				foreach (Mobile m in mobs)
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
				return false;

			if (x < 0 || y < 0 || x >= Width || y >= Height)
				return false;

			bool hasSurface = false;
			bool checkmob = false;
			bool canswim = false;
			bool cantwalk = false;

			if (mob != null)
			{
				checkmob = true;
				canswim = mob.CanSwim;
				cantwalk = mob.CantWalk;
			}

			LandTile lt = Tiles.GetLandTile(x, y);
			int lowZ = 0, avgZ = 0, topZ = 0;

			bool surface, impassable;
			bool wet = false;

			GetAverageZ(x, y, ref lowZ, ref avgZ, ref topZ);
			TileFlag landFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;

			impassable = (landFlags & TileFlag.Impassable) != 0;

			if (checkmob)
			{
				wet = (landFlags & TileFlag.Wet) != 0;
				
				if (cantwalk && !wet) // dont allow wateronly creatures on land
					impassable = true;
				
				if (canswim && wet) // allow water creatures on water
				{
					impassable = false;
				}
			}

            if (impassable && avgZ > z && (z + height) > lowZ)
				return false;

            if (!impassable && z == avgZ && !lt.Ignored)
                hasSurface = true;

            StaticTile[] staticTiles = Tiles.GetStaticTiles(x, y, true);

			for (int i = 0; i < staticTiles.Length; ++i)
			{
				ItemData id = TileData.ItemTable[staticTiles[i].ID & TileData.MaxItemValue];
				surface = id.Surface;
				impassable = id.Impassable;
				if (checkmob)
				{
					wet = (id.Flags & TileFlag.Wet) != 0;
					
					if (cantwalk && !wet) // dont allow wateronly creatures on land
						impassable = true;
					
					if (canswim && wet) // allow water creatures on water
					{
						surface = true;
						impassable = false;
					}
				}

				if ((surface || impassable) && (staticTiles[i].Z + id.CalcHeight) > z && (z + height) > staticTiles[i].Z)
					return false;

                if (surface && !impassable && z == (staticTiles[i].Z + id.CalcHeight))
                    hasSurface = true;
            }

			Sector sector = GetSector(x, y);
			List<Item> items = sector.Items;
			List<Mobile> mobs = sector.Mobiles;

			for (int i = 0; i < items.Count; ++i)
			{
				Item item = items[i];

				if (item.ItemID < 0x4000 && item.AtWorldPoint(x, y))
				{
					ItemData id = item.ItemData;
					surface = id.Surface;
					impassable = id.Impassable;
					if (checkmob)
					{
						wet = (id.Flags & TileFlag.Wet) != 0;
						
						if (cantwalk && !wet) // dont allow wateronly creatures on land
							impassable = true;
						
						if (canswim && wet) // allow water creatures on water
						{
							surface = true;
							impassable = false;
						}
					}

					if ((surface || impassable || checkBlocksFit && item.BlocksFit) && (item.Z + id.CalcHeight) > z && (z + height) > item.Z)
						return false;

                    if (surface && !impassable && !item.Movable && z == (item.Z + id.CalcHeight))
                        hasSurface = true;
                }
			}

			if (checkMobiles)
			{
				for (int i = 0; i < mobs.Count; ++i)
				{
					Mobile m = mobs[i];

					if (m.Location.X == x && m.Location.Y == y && (m.AccessLevel == AccessLevel.Player || !m.Hidden))
					{
						if ((m.Z + 16) > z && (z + height) > m.Z)
							return false;
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

		#region Find Item/Mobile
		public TItem FindItem<TItem>(Point3D p, int range = 0) where TItem : Item
		{
			IPooledEnumerable<Item> eable = GetItemsInRange(p, range);

			foreach (Item item in eable)
			{
				if (item.GetType() == typeof(TItem))
				{
					eable.Free();
					return item as TItem;
				}
			}

			eable.Free();
			return null;
		}

		public IEnumerable<TItem> FindItems<TItem>(Point3D p, int range = 0) where TItem : Item
		{
			IPooledEnumerable<Item> eable = GetItemsInRange(p, range);

			foreach (Item item in eable)
			{
				if (item.GetType() == typeof(TItem))
				{
					yield return item as TItem;
				}
			}

			eable.Free();
		}

		public TMob FindMobile<TMob>(Point3D p, int range = 0) where TMob : Mobile
		{
			IPooledEnumerable<Mobile> eable = GetMobilesInRange(p, range);

			foreach (Mobile m in eable)
			{
				if (m.GetType() == typeof(TMob))
				{
					eable.Free();
					return m as TMob;
				}
			}

			eable.Free();
			return null;
		}

		public IEnumerable<TMob> FindMobiles<TMob>(Point3D p, int range = 0) where TMob : Mobile
		{
			IPooledEnumerable<Mobile> eable = GetMobilesInRange(p, range);

			foreach (Mobile m in eable)
			{
				if (m.GetType() == typeof(TMob))
				{
					yield return m as TMob;
				}
			}

			eable.Free();
		}
		#endregion

		#region Spawn Position
		public Point3D GetSpawnPosition(Point3D center, int range)
		{
			for (int i = 0; i < 10; i++)
			{
				int x = center.X + (Utility.Random((range * 2) + 1) - range);
				int y = center.Y + (Utility.Random((range * 2) + 1) - range);
				int z = GetAverageZ(x, y);

				if (CanSpawnMobile(new Point2D(x, y), center.Z))
					return new Point3D(x, y, center.Z);

				if (CanSpawnMobile(new Point2D(x, y), z))
					return new Point3D(x, y, z);
			}

			return center;
		}

		public Point3D GetRandomSpawnPoint(Rectangle2D rec)
		{
			if (this == Internal)
				return Point3D.Zero;

			int x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
			int y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
			int z = GetAverageZ(x, y);

			return new Point3D(x, y, z);
		}
		#endregion

		private class ZComparer : IComparer<Item>
		{
			public static readonly ZComparer Default = new ZComparer();

			public int Compare(Item x, Item y)
			{
				if (x == null || y == null)
					return 0;

				return x.Z.CompareTo(y.Z);
			}
		}

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

			IPooledEnumerable<Item> eable = map.GetItemsInRange(new Point3D(x, y, 0), 0);

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
			LandTile landTile = Tiles.GetLandTile(x, y);
			StaticTile[] tiles = Tiles.GetStaticTiles(x, y, true);

			int landZ = 0, landAvg = 0, landTop = 0;

			GetAverageZ(x, y, ref landZ, ref landAvg, ref landTop);

			List<Item> items = AcquireFixItems(this, x, y);

			for (int i = 0; i < items.Count; i++)
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

				foreach (StaticTile tile in tiles)
				{
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

			foreach (StaticTile tile in staticTiles)
			{
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

			foreach (Item item in sector.Items)
			{
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

            return m_InvalidSector;
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
                MultiComponentList mcl = m.Components;

				Sector start = GetMultiMinSector(m.Location, mcl);
				Sector end = GetMultiMaxSector(m.Location, mcl);

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
                MultiComponentList mcl = m.Components;

				Sector start = GetMultiMinSector(m.Location, mcl);
				Sector end = GetMultiMaxSector(m.Location, mcl);

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

		public virtual void OnMove(Point3D oldLocation, Mobile m)
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

		public virtual void OnMove(Point3D oldLocation, Item item)
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

			if (item is BaseMulti m)
			{
                MultiComponentList mcl = m.Components;

				Sector start = GetMultiMinSector(m.Location, mcl);
				Sector end = GetMultiMaxSector(m.Location, mcl);

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

		public int MapID => m_MapID;

		public int MapIndex => m_MapIndex;

		public int Width => m_Width;

		public int Height => m_Height;

		public Dictionary<string, Region> Regions => m_Regions;

		public void RegisterRegion(Region reg)
		{
			string regName = reg.Name;

			if (regName != null)
			{
				if (m_Regions.ContainsKey(regName))
				{
					Console.WriteLine("Warning: Duplicate region name '{0}' for map '{1}'", regName, Name);
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
			get => m_DefaultRegion ?? (m_DefaultRegion = new Region(null, this, 0, new Rectangle3D[0]));
			set => m_DefaultRegion = value;
		}

		public MapRules Rules { get; set; }

		public Sector InvalidSector => m_InvalidSector;

		public string Name { get; set; }

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

				IEnumerable<T> pool = PooledEnumeration.EnumerateSectors(map, bounds).SelectMany(s => selector(s, bounds));

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
					int i = _Pool.Count;

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

        public Point3D GetPoint(object o, bool eye)
		{
			Point3D p;

			if (o is Mobile mobile)
			{
				p = mobile.Location;
				p.Z += 14; //eye ? 15 : 10;
			}
			else if (o is Item item)
			{
				p = item.GetWorldLocation();
				p.Z += (item.ItemData.Height / 2) + 1;
			}
			else if (o is Point3D point)
			{
				p = point;
			}
			else if (o is LandTarget landTarget)
			{
				p = landTarget.Location;

				int low = 0, avg = 0, top = 0;

				GetAverageZ(p.X, p.Y, ref low, ref avg, ref top);

				p.Z = top + 1;
			}
			else if (o is StaticTarget st)
			{
                ItemData id = TileData.ItemTable[st.ItemID & TileData.MaxItemValue];

				p = new Point3D(st.X, st.Y, st.Z - id.CalcHeight + (id.Height / 2));
			}
			else if (o is IPoint3D point3D)
			{
				p = new Point3D(point3D);
			}
			else
			{
				Console.WriteLine("Warning: Invalid object ({0}) in line of sight", o);

				p = Point3D.Zero;
			}

			return p;
		}

		#region Line Of Sight
		private static int m_MaxLOSDistance = Core.GlobalMaxUpdateRange + 1;

		public static int MaxLOSDistance { get => m_MaxLOSDistance; set => m_MaxLOSDistance = value; }

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

			Point3D end = dest;

			if (org.X > dest.X || org.X == dest.X && org.Y > dest.Y || org.X == dest.X && org.Y == dest.Y && org.Z > dest.Z)
			{
				Point3D swap = org;

				org = dest;
				dest = swap;
			}

			if (org == dest)
			{
				return true;
			}

			int xd = dest.m_X - org.m_X;
			int yd = dest.m_Y - org.m_Y;
			int zd = dest.m_Z - org.m_Z;

			double zslp = Math.Sqrt(xd * xd + yd * yd);

			double sq3d;

			if (zd != 0)
			{
				sq3d = Math.Sqrt(zslp * zslp + zd * zd);
			}
			else
			{
				sq3d = zslp;
			}

			double rise = yd / sq3d;
			double run = xd / sq3d;

			zslp = zd / sq3d;

			double x = org.m_X;
			double y = org.m_Y;
			double z = org.m_Z;

			Point3DList path = new Point3DList();

			int ix, iy, iz;
			Point3D p;

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
				return true;
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

			int height, landZ, landAvg, landTop, pointTop, ltID;
			bool contains;
			Point3D point;
			LandTile landTile;
			ItemData id;
			TileFlag flags;
			StaticTile[] statics;
			IPooledEnumerable<Item> eable;

			for (int i = 0; i < pathCount; ++i)
			{
				point = path[i];
				pointTop = point.m_Z;

				landTile = Tiles.GetLandTile(point.X, point.Y);

				landZ = landAvg = landTop = 0;

				GetAverageZ(point.m_X, point.m_Y, ref landZ, ref landAvg, ref landTop);

				if (landZ <= pointTop && landTop >= point.m_Z &&
					(point.m_X != end.m_X || point.m_Y != end.m_Y || landZ > endTop || landTop < end.m_Z) && !landTile.Ignored)
				{
					return false;
				}

				statics = Tiles.GetStaticTiles(point.m_X, point.m_Y, true);

				contains = false;
				ltID = landTile.ID;

				for (int j = 0; !contains && j < m_InvalidLandTiles.Length; ++j)
				{
					contains = ltID == m_InvalidLandTiles[j];
				}

				if (contains && statics.Length == 0)
				{
					eable = GetItemsInRange(point, 0);

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

				foreach (StaticTile t in statics)
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

			Rectangle2D rect = new Rectangle2D(pTop.m_X, pTop.m_Y, pBottom.m_X - pTop.m_X + 1, pBottom.m_Y - pTop.m_Y + 1);

			IPooledEnumerable<Item> area = GetItemsInBounds(rect);

			try
			{
				int count;
				bool found;
				Point3D loc;

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

					id = i.ItemData;

					flags = id.Flags;

					if ((flags & (TileFlag.Window | TileFlag.NoShoot)) == 0)
					{
						continue;
					}

					height = id.CalcHeight;

					found = false;

					count = path.Count;

					for (int j = 0; j < count; ++j)
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
			if (from == dest || from is Mobile mobile && mobile.AccessLevel > AccessLevel.Player)
			{
				return true;
			}

			if (dest is Item item && from is Mobile && item.RootParent == from)
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
			target.Z += 14;

			return LineOfSight(eye, target);
		}
		#endregion

		private static int[] m_InvalidLandTiles = { 0x244 };

		public static int[] InvalidLandTiles { get => m_InvalidLandTiles; set => m_InvalidLandTiles = value; }

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
			return CompareTo(other as Map);
		}
	}
}
