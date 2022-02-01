#region References
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Server.Items;
using Server.Mobiles;
#endregion

namespace Server.Movement
{
	public class FastMovementImpl : IMovementImpl
	{
		public static bool Enabled = true;

		private const int PersonHeight = 16;
		private const int StepHeight = 2;

		private const TileFlag ImpassableSurface = TileFlag.Impassable | TileFlag.Surface;

		private static IMovementImpl _Successor;

		public static void Initialize()
		{
			_Successor = Movement.Impl;

			Movement.Impl = new FastMovementImpl();
		}

		private FastMovementImpl()
		{ }

		private static bool IsOk(StaticTile tile, int ourZ, int ourTop)
		{
			var itemData = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

			return tile.Z + itemData.CalcHeight <= ourZ || ourTop <= tile.Z || (itemData.Flags & ImpassableSurface) == 0;
		}

		private static bool IsOk(Mobile m, Item item, int ourZ, int ourTop, bool ignoreDoors, bool ignoreSpellFields)
		{
			var itemID = item.ItemID & TileData.MaxItemValue;
			var itemData = TileData.ItemTable[itemID];

			if ((itemData.Flags & ImpassableSurface) == 0)
				return true;

			if (((itemData.Flags & TileFlag.Door) != 0 || itemID == 0x692 || itemID == 0x846 || itemID == 0x873 || (itemID >= 0x6F5 && itemID <= 0x6F6)) && ignoreDoors)
				return m == null || !(item is BaseHouseDoor d) || d.CheckAccess(m);

			if ((itemID == 0x82 || itemID == 0x3946 || itemID == 0x3956) && ignoreSpellFields)
				return true;

			// hidden containers, per EA
			if ((itemData.Flags & TileFlag.Container) != 0 && !item.Visible)
				return true;

			return item.Z + itemData.CalcHeight <= ourZ || ourTop <= item.Z;
		}

		private static bool IsOk(Mobile m, bool ignoreDoors, bool ignoreSpellFields, int ourZ, int ourTop, StaticTile[] tiles, HashSet<Item> items)
		{
			for (var i = 0; i < tiles.Length; i++)
			{
				if (!IsOk(tiles[i], ourZ, ourTop))
					return false;

				foreach (var item in items)
				{
					if (!IsOk(m, item, ourZ, ourTop, ignoreDoors, ignoreSpellFields))
						return false;
				}
			}

			return true;
		}

		private static bool Check(Map map, IPoint3D p, HashSet<Item> items, int x, int y, int startTop, int startZ, bool canSwim, bool cantWalk, out int newZ)
		{
			newZ = 0;

			var tiles = map.Tiles.GetStaticTiles(x, y, true);
			var landTile = map.Tiles.GetLandTile(x, y);
			var landData = TileData.LandTable[landTile.ID & TileData.MaxLandValue];
			var landBlocks = (landData.Flags & TileFlag.Impassable) != 0;
			var considerLand = !landTile.Ignored;

			if (landBlocks && canSwim && (landData.Flags & TileFlag.Wet) != 0)
				landBlocks = false; // Impassable, Can Swim, and Is water.  Don't block it.
			else if (cantWalk && (landData.Flags & TileFlag.Wet) == 0)
				landBlocks = true; // Can't walk and it's not water

			int landZ = 0, landCenter = 0, landTop = 0;

			map.GetAverageZ(x, y, ref landZ, ref landCenter, ref landTop);

			var moveIsOk = false;

			var stepTop = startTop + StepHeight;
			var checkTop = startZ + PersonHeight;

			var m = p as Mobile;

			var ignoreDoors = MovementImpl.AlwaysIgnoreDoors(p) || m == null || !m.Alive || m.IsDeadBondedPet || m.Body.IsGhost ||
							  m.Body.BodyID == 987;
			var ignoreSpellFields = m is PlayerMobile && map.MapID != 0;

			int itemZ, itemTop, ourZ, ourTop, testTop;
			ItemData itemData;
			TileFlag flags;

			#region Tiles
			foreach (var tile in tiles)
			{
				itemData = TileData.ItemTable[tile.ID & TileData.MaxItemValue];
				flags = itemData.Flags;

				if (m != null && m.Flying && (Insensitive.Equals(itemData.Name, "hover over") || (flags & TileFlag.HoverOver) != 0))
				{
					newZ = tile.Z;
					return true;
				}

				// Stygian Dragon
				if (m != null && m.Body == 826 && map != null && map.MapID == 5)
				{
					if (x >= 307 && x <= 354 && y >= 126 && y <= 192)
					{
						if (tile.Z > newZ)
							newZ = tile.Z;

						moveIsOk = true;
					}
					else if (x >= 42 && x <= 89)
					{
						if ((y >= 333 && y <= 399) || (y >= 531 && y <= 597) || (y >= 739 && y <= 805))
						{
							if (tile.Z > newZ)
								newZ = tile.Z;

							moveIsOk = true;
						}
					}
				}

				if ((flags & ImpassableSurface) != TileFlag.Surface && (!canSwim || (flags & TileFlag.Wet) == 0))
					continue;

				if (cantWalk && (flags & TileFlag.Wet) == 0)
					continue;

				itemZ = tile.Z;
				itemTop = itemZ;
				ourZ = itemZ + itemData.CalcHeight;
				ourTop = ourZ + PersonHeight;
				testTop = checkTop;

				if (moveIsOk)
				{
					var cmp = Math.Abs(ourZ - p.Z) - Math.Abs(newZ - p.Z);

					if (cmp > 0 || (cmp == 0 && ourZ > newZ))
						continue;
				}

				if (ourTop > testTop)
					testTop = ourTop;

				if (!itemData.Bridge)
					itemTop += itemData.Height;

				if (stepTop < itemTop)
					continue;

				var landCheck = itemZ;

				if (itemData.Height >= StepHeight)
					landCheck += StepHeight;
				else
					landCheck += itemData.Height;

				if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landZ)
					continue;

				if (!IsOk(m, ignoreDoors, ignoreSpellFields, ourZ, testTop, tiles, items))
					continue;

				newZ = ourZ;
				moveIsOk = true;
			}
			#endregion

			#region Items
			foreach (var item in items)
			{
				itemData = item.ItemData;
				flags = itemData.Flags;

				if (m != null && m.Flying && (Insensitive.Equals(itemData.Name, "hover over") || (flags & TileFlag.HoverOver) != 0))
				{
					newZ = item.Z;
					return true;
				}

				if (item.Movable)
					continue;

				if ((flags & ImpassableSurface) != TileFlag.Surface && ((m != null && !m.CanSwim) || (flags & TileFlag.Wet) == 0))
					continue;

				if (cantWalk && (flags & TileFlag.Wet) == 0)
					continue;

				itemZ = item.Z;
				itemTop = itemZ;
				ourZ = itemZ + itemData.CalcHeight;
				ourTop = ourZ + PersonHeight;
				testTop = checkTop;

				if (moveIsOk)
				{
					var cmp = Math.Abs(ourZ - p.Z) - Math.Abs(newZ - p.Z);

					if (cmp > 0 || (cmp == 0 && ourZ > newZ))
						continue;
				}

				if (ourTop > testTop)
					testTop = ourTop;

				if (!itemData.Bridge)
					itemTop += itemData.Height;

				if (stepTop < itemTop)
					continue;

				var landCheck = itemZ;

				if (itemData.Height >= StepHeight)
					landCheck += StepHeight;
				else
					landCheck += itemData.Height;

				if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landZ)
					continue;

				if (!IsOk(m, ignoreDoors, ignoreSpellFields, ourZ, testTop, tiles, items))
					continue;

				newZ = ourZ;
				moveIsOk = true;
			}
			#endregion

			if (!considerLand || landBlocks || stepTop < landZ)
				return moveIsOk;

			ourZ = landCenter;
			ourTop = ourZ + PersonHeight;
			testTop = checkTop;

			if (ourTop > testTop)
				testTop = ourTop;

			var shouldCheck = true;

			if (moveIsOk)
			{
				var cmp = Math.Abs(ourZ - p.Z) - Math.Abs(newZ - p.Z);

				if (cmp > 0 || (cmp == 0 && ourZ > newZ))
					shouldCheck = false;
			}

			if (!shouldCheck || !IsOk(m, ignoreDoors, ignoreSpellFields, ourZ, testTop, tiles, items))
				return moveIsOk;

			newZ = ourZ;
			moveIsOk = true;

			return moveIsOk;
		}

		public bool CheckMovement(IPoint3D p, Map map, Point3D loc, Direction d, out int newZ)
		{
			newZ = 0;

			if (!Enabled && _Successor != null)
				return _Successor.CheckMovement(p, map, loc, d, out newZ);

			if (map == null || map == Map.Internal)
				return false;

			var xStart = loc.X;
			var yStart = loc.Y;

			int xForward = xStart, yForward = yStart;
			int xRight = xStart, yRight = yStart;
			int xLeft = xStart, yLeft = yStart;

			var checkDiagonals = ((int)d & 0x1) == 0x1;

			Offset(d, ref xForward, ref yForward);
			Offset((Direction)(((int)d - 1) & 0x7), ref xLeft, ref yLeft);
			Offset((Direction)(((int)d + 1) & 0x7), ref xRight, ref yRight);

			if (xForward < 0 || yForward < 0 || xForward >= map.Width || yForward >= map.Height)
				return false;

			var ignoreMovableImpassables = MovementImpl.IgnoresMovableImpassables(p);

			var reqFlags = ImpassableSurface;

			var m = p as Mobile;

			if (m != null && m.CanSwim)
				reqFlags |= TileFlag.Wet;

			HashSet<Item> list = null;

			MovementPool.AcquireMoveCache(ref list, map.GetSector(xStart, yStart), i => Verify(i, reqFlags, ignoreMovableImpassables, xStart, yStart));

			GetStartZ(p, map, loc, list, out var startZ, out var startTop);

			MovementPool.AcquireMoveCache(ref list, map.GetSector(xForward, yForward), i => Verify(i, reqFlags, ignoreMovableImpassables, xForward, yForward));

			var moveIsOk = Check(map, p, list, xForward, yForward, startTop, startZ, m != null && m.CanSwim, m != null && m.CantWalk, out newZ);

			if (moveIsOk && m != null && !m.Player && startZ - newZ >= 20) // fall height
			{
				if (m.Flying)
					newZ = startZ;
				else
					moveIsOk = false;
			}

			if (m != null && moveIsOk && checkDiagonals)
			{
				MovementPool.AcquireMoveCache(ref list, map.GetSector(xLeft, yLeft), i => Verify(i, reqFlags, ignoreMovableImpassables, xLeft, yLeft));

				if (!Check(map, m, list, xLeft, yLeft, startTop, startZ, m.CanSwim, m.CantWalk, out _))
					moveIsOk = false;
				else
				{
					MovementPool.AcquireMoveCache(ref list, map.GetSector(xRight, yRight), i => Verify(i, reqFlags, ignoreMovableImpassables, xRight, yRight));

					if (!Check(map, m, list, xRight, yRight, startTop, startZ, m.CanSwim, m.CantWalk, out _))
						moveIsOk = false;
				}
			}

			MovementPool.ClearMoveCache(ref list, true);

			if (!moveIsOk)
				newZ = startZ;

			return moveIsOk;
		}

		private static bool Verify(Item item, int x, int y)
		{
			return item != null && item.AtWorldPoint(x, y);
		}

		private static bool Verify(Item item, TileFlag reqFlags, bool ignoreMovableImpassables)
		{
			if (item == null)
				return false;

			if (ignoreMovableImpassables && item.Movable && item.ItemData.Impassable)
				return false;

			if ((item.ItemData.Flags & reqFlags) == 0)
				return false;

			if (item is BaseMulti || item.ItemID > TileData.MaxItemValue)
				return false;

			return true;
		}

		private static bool Verify(Item item, TileFlag reqFlags, bool ignoreMovableImpassables, int x, int y)
		{
			return Verify(item, reqFlags, ignoreMovableImpassables) && Verify(item, x, y);
		}

		private static void GetStartZ(IPoint3D p, Map map, Point3D loc, IEnumerable<Item> itemList, out int zLow, out int zTop)
		{
			int xCheck = loc.X, yCheck = loc.Y;

			var landTile = map.Tiles.GetLandTile(xCheck, yCheck);
			var landData = TileData.LandTable[landTile.ID & TileData.MaxLandValue];
			var landBlocks = (landData.Flags & TileFlag.Impassable) != 0;

			var m = p as Mobile;

			if (m != null)
			{
				if (landBlocks && m.CanSwim && (landData.Flags & TileFlag.Wet) != 0)
					landBlocks = false;
				else if (m.CantWalk && (landData.Flags & TileFlag.Wet) == 0)
					landBlocks = true;
			}

			int landZ = 0, landCenter = 0, landTop = 0;

			map.GetAverageZ(xCheck, yCheck, ref landZ, ref landCenter, ref landTop);

			var considerLand = !landTile.Ignored;

			var zCenter = zLow = zTop = 0;
			var isSet = false;

			if (considerLand && !landBlocks && loc.Z >= landCenter)
			{
				zLow = landZ;
				zCenter = landCenter;
				zTop = landTop;
				isSet = true;
			}

			var staticTiles = map.Tiles.GetStaticTiles(xCheck, yCheck, true);

			foreach (var tile in staticTiles)
			{
				var tileData = TileData.ItemTable[tile.ID & TileData.MaxItemValue];
				var calcTop = (tile.Z + tileData.CalcHeight);

				if (isSet && calcTop < zCenter)
					continue;

				if ((tileData.Flags & TileFlag.Surface) == 0 && ((m != null && !m.CanSwim) || (tileData.Flags & TileFlag.Wet) == 0))
					continue;

				if (loc.Z < calcTop)
					continue;

				if (m != null && m.CantWalk && (tileData.Flags & TileFlag.Wet) == 0)
					continue;

				zLow = tile.Z;
				zCenter = calcTop;

				var top = tile.Z + tileData.Height;

				if (!isSet || top > zTop)
				{
					zTop = top;
				}

				isSet = true;
			}

			ItemData itemData;

			foreach (var item in itemList)
			{
				itemData = item.ItemData;

				var calcTop = item.Z + itemData.CalcHeight;

				if (isSet && calcTop < zCenter)
					continue;

				if ((itemData.Flags & TileFlag.Surface) == 0 && ((m != null && !m.CanSwim) || (itemData.Flags & TileFlag.Wet) == 0))
					continue;

				if (loc.Z < calcTop)
					continue;

				if (m != null && m.CantWalk && (itemData.Flags & TileFlag.Wet) == 0)
					continue;

				zLow = item.Z;
				zCenter = calcTop;

				var top = item.Z + itemData.Height;

				if (!isSet || top > zTop)
				{
					zTop = top;
				}

				isSet = true;
			}

			if (!isSet)
				zLow = zTop = loc.Z;
			else if (loc.Z > zTop)
				zTop = loc.Z;
		}

		public void Offset(Direction d, ref int x, ref int y)
		{
			switch (d & Direction.Mask)
			{
				case Direction.North:
					--y;
					break;
				case Direction.South:
					++y;
					break;
				case Direction.West:
					--x;
					break;
				case Direction.East:
					++x;
					break;
				case Direction.Right:
					++x;
					--y;
					break;
				case Direction.Left:
					--x;
					++y;
					break;
				case Direction.Down:
					++x;
					++y;
					break;
				case Direction.Up:
					--x;
					--y;
					break;
			}
		}

		private static class MovementPool
		{
			private static readonly ConcurrentQueue<HashSet<Item>> _MoveCachePool = new ConcurrentQueue<HashSet<Item>>();

			public static void AcquireMoveCache(ref HashSet<Item> cache, Sector s, Predicate<Item> predicate)
			{
				if (cache != null)
					cache.Clear();
				else if (!_MoveCachePool.TryDequeue(out cache))
					cache = new HashSet<Item>(0x10);

				if (predicate != null)
				{
					foreach (var item in s.Items)
					{
						if (predicate(item))
							cache.Add(item);
					}
				}
				else
					cache.UnionWith(s.Items);
			}

			public static void ClearMoveCache(ref HashSet<Item> cache, bool free)
			{
				if (cache == null)
					return;

				cache.Clear();

				if (!free)
					return;

				if (_MoveCachePool.Count < 0x400)
					_MoveCachePool.Enqueue(cache);
				else
					cache.TrimExcess();

				cache = null;
			}
		}
	}
}
