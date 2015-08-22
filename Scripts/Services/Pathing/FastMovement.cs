#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Items;
using Server.Mobiles;
#endregion

namespace Server.Movement
{
	public class FastMovementImpl : IMovementImpl
	{
		public static bool Enabled = false;

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

		private static bool IsOk(Item item, int ourZ, int ourTop, bool ignoreDoors, bool ignoreSpellFields)
		{
			var itemID = item.ItemID & TileData.MaxItemValue;
			var itemData = TileData.ItemTable[itemID];

			if ((itemData.Flags & ImpassableSurface) == 0)
			{
				return true;
			}

			if (((itemData.Flags & TileFlag.Door) != 0 || itemID == 0x692 || itemID == 0x846 || itemID == 0x873 ||
				 (itemID >= 0x6F5 && itemID <= 0x6F6)) && ignoreDoors)
			{
				return true;
			}

			if ((itemID == 0x82 || itemID == 0x3946 || itemID == 0x3956) && ignoreSpellFields)
			{
				return true;
			}

			return item.Z + itemData.CalcHeight <= ourZ || ourTop <= item.Z;
		}

		private static bool IsOk(
			bool ignoreDoors,
			bool ignoreSpellFields,
			int ourZ,
			int ourTop,
			IEnumerable<StaticTile> tiles,
			IEnumerable<Item> items)
		{
			return tiles.All(t => IsOk(t, ourZ, ourTop)) && items.All(i => IsOk(i, ourZ, ourTop, ignoreDoors, ignoreSpellFields));
		}

		private static bool Check(
			Map map,
			Mobile m,
			List<Item> items,
			int x,
			int y,
			int startTop,
			int startZ,
			bool canSwim,
			bool cantWalk,
			out int newZ)
		{
			newZ = 0;

			var tiles = map.Tiles.GetStaticTiles(x, y, true);
			var landTile = map.Tiles.GetLandTile(x, y);
			var landData = TileData.LandTable[landTile.ID & TileData.MaxLandValue];
			var landBlocks = (landData.Flags & TileFlag.Impassable) != 0;
			var considerLand = !landTile.Ignored;

			if (landBlocks && canSwim && (landData.Flags & TileFlag.Wet) != 0)
			{
				//Impassable, Can Swim, and Is water.  Don't block it.
				landBlocks = false;
			}
			else if (cantWalk && (landData.Flags & TileFlag.Wet) == 0)
			{
				//Can't walk and it's not water
				landBlocks = true;
			}

			int landZ = 0, landCenter = 0, landTop = 0;

			map.GetAverageZ(x, y, ref landZ, ref landCenter, ref landTop);

			var moveIsOk = false;

			var stepTop = startTop + StepHeight;
			var checkTop = startZ + PersonHeight;

			var ignoreDoors = MovementImpl.AlwaysIgnoreDoors || !m.Alive || m.IsDeadBondedPet || m.Body.IsGhost ||
							  m.Body.BodyID == 987;
			var ignoreSpellFields = m is PlayerMobile && map.MapID != 0;

			int itemZ, itemTop, ourZ, ourTop, testTop;
			ItemData itemData;
			TileFlag flags;

			#region Tiles
			foreach (var tile in tiles)
			{
				itemData = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				#region SA
				if (m.Flying && Insensitive.Equals(itemData.Name, "hover over"))
				{
					newZ = tile.Z;
					return true;
				}

				// Stygian Dragon
				if (m.Body == 826 && map != null && map.MapID == 5)
				{
					if (x >= 307 && x <= 354 && y >= 126 && y <= 192)
					{
						if (tile.Z > newZ)
						{
							newZ = tile.Z;
						}

						moveIsOk = true;
					}
					else if (x >= 42 && x <= 89)
					{
						if ((y >= 333 && y <= 399) || (y >= 531 && y <= 597) || (y >= 739 && y <= 805))
						{
							if (tile.Z > newZ)
							{
								newZ = tile.Z;
							}

							moveIsOk = true;
						}
					}
				}
				#endregion

				flags = itemData.Flags;

				if ((flags & ImpassableSurface) != TileFlag.Surface && (!canSwim || (flags & TileFlag.Wet) == 0))
				{
					continue;
				}

				if (cantWalk && (flags & TileFlag.Wet) == 0)
				{
					continue;
				}

				itemZ = tile.Z;
				itemTop = itemZ;
				ourZ = itemZ + itemData.CalcHeight;
				ourTop = ourZ + PersonHeight;
				testTop = checkTop;

				if (moveIsOk)
				{
					var cmp = Math.Abs(ourZ - m.Z) - Math.Abs(newZ - m.Z);

					if (cmp > 0 || (cmp == 0 && ourZ > newZ))
					{
						continue;
					}
				}

				if (ourTop > testTop)
				{
					testTop = ourTop;
				}

				if (!itemData.Bridge)
				{
					itemTop += itemData.Height;
				}

				if (stepTop < itemTop)
				{
					continue;
				}

				var landCheck = itemZ;

				if (itemData.Height >= StepHeight)
				{
					landCheck += StepHeight;
				}
				else
				{
					landCheck += itemData.Height;
				}

				if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landZ)
				{
					continue;
				}

				if (!IsOk(ignoreDoors, ignoreSpellFields, ourZ, testTop, tiles, items))
				{
					continue;
				}

				newZ = ourZ;
				moveIsOk = true;
			}
			#endregion

			#region Items
			foreach (var item in items)
			{
				itemData = item.ItemData;
				flags = itemData.Flags;

				#region SA
				if (m.Flying && Insensitive.Equals(itemData.Name, "hover over"))
				{
					newZ = item.Z;
					return true;
				}
				#endregion

				if (item.Movable)
				{
					continue;
				}

				if ((flags & ImpassableSurface) != TileFlag.Surface && (!m.CanSwim || (flags & TileFlag.Wet) == 0))
				{
					continue;
				}

				if (cantWalk && (flags & TileFlag.Wet) == 0)
				{
					continue;
				}

				itemZ = item.Z;
				itemTop = itemZ;
				ourZ = itemZ + itemData.CalcHeight;
				ourTop = ourZ + PersonHeight;
				testTop = checkTop;

				if (moveIsOk)
				{
					var cmp = Math.Abs(ourZ - m.Z) - Math.Abs(newZ - m.Z);

					if (cmp > 0 || (cmp == 0 && ourZ > newZ))
					{
						continue;
					}
				}

				if (ourTop > testTop)
				{
					testTop = ourTop;
				}

				if (!itemData.Bridge)
				{
					itemTop += itemData.Height;
				}

				if (stepTop < itemTop)
				{
					continue;
				}

				var landCheck = itemZ;

				if (itemData.Height >= StepHeight)
				{
					landCheck += StepHeight;
				}
				else
				{
					landCheck += itemData.Height;
				}

				if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landZ)
				{
					continue;
				}

				if (!IsOk(ignoreDoors, ignoreSpellFields, ourZ, testTop, tiles, items))
				{
					continue;
				}

				newZ = ourZ;
				moveIsOk = true;
			}
			#endregion

			if (!considerLand || landBlocks || stepTop < landZ)
			{
				return moveIsOk;
			}

			ourZ = landCenter;
			ourTop = ourZ + PersonHeight;
			testTop = checkTop;

			if (ourTop > testTop)
			{
				testTop = ourTop;
			}

			var shouldCheck = true;

			if (moveIsOk)
			{
				var cmp = Math.Abs(ourZ - m.Z) - Math.Abs(newZ - m.Z);

				if (cmp > 0 || (cmp == 0 && ourZ > newZ))
				{
					shouldCheck = false;
				}
			}

			if (!shouldCheck || !IsOk(ignoreDoors, ignoreSpellFields, ourZ, testTop, tiles, items))
			{
				return moveIsOk;
			}

			newZ = ourZ;
			moveIsOk = true;

			return moveIsOk;
		}

		public bool CheckMovement(Mobile m, Map map, Point3D loc, Direction d, out int newZ)
		{
			if (!Enabled && _Successor != null)
			{
				return _Successor.CheckMovement(m, map, loc, d, out newZ);
			}

			if (map == null || map == Map.Internal)
			{
				newZ = 0;
				return false;
			}

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
			{
				newZ = 0;
				return false;
			}

			int startZ, startTop;

			IEnumerable<Item> itemsStart, itemsForward, itemsLeft, itemsRight;

			var ignoreMovableImpassables = MovementImpl.IgnoreMovableImpassables;
			var reqFlags = ImpassableSurface;

			if (m.CanSwim)
			{
				reqFlags |= TileFlag.Wet;
			}

			if (checkDiagonals)
			{
				var sStart = map.GetSector(xStart, yStart);
				var sForward = map.GetSector(xForward, yForward);
				var sLeft = map.GetSector(xLeft, yLeft);
				var sRight = map.GetSector(xRight, yRight);

				itemsStart = sStart.Items.Where(i => Verify(i, reqFlags, ignoreMovableImpassables, xStart, yStart));
				itemsForward = sForward.Items.Where(i => Verify(i, reqFlags, ignoreMovableImpassables, xForward, yForward));
				itemsLeft = sLeft.Items.Where(i => Verify(i, reqFlags, ignoreMovableImpassables, xLeft, yLeft));
				itemsRight = sRight.Items.Where(i => Verify(i, reqFlags, ignoreMovableImpassables, xRight, yRight));
			}
			else
			{
				var sStart = map.GetSector(xStart, yStart);
				var sForward = map.GetSector(xForward, yForward);

				itemsStart = sStart.Items.Where(i => Verify(i, reqFlags, ignoreMovableImpassables, xStart, yStart));
				itemsForward = sForward.Items.Where(i => Verify(i, reqFlags, ignoreMovableImpassables, xForward, yForward));
				itemsLeft = Enumerable.Empty<Item>();
				itemsRight = Enumerable.Empty<Item>();
			}

			GetStartZ(m, map, loc, itemsStart, out startZ, out startTop);

			List<Item> list = null;

			MovementPool.AcquireMoveCache(ref list, itemsForward);

			var moveIsOk = Check(map, m, list, xForward, yForward, startTop, startZ, m.CanSwim, m.CantWalk, out newZ);

			if (moveIsOk && checkDiagonals)
			{
				int hold;

				if (m.Player && m.AccessLevel < AccessLevel.GameMaster)
				{
					MovementPool.AcquireMoveCache(ref list, itemsLeft);

					if (!Check(map, m, list, xLeft, yLeft, startTop, startZ, m.CanSwim, m.CantWalk, out hold))
					{
						moveIsOk = false;
					}
					else
					{
						MovementPool.AcquireMoveCache(ref list, itemsRight);

						if (!Check(map, m, list, xRight, yRight, startTop, startZ, m.CanSwim, m.CantWalk, out hold))
						{
							moveIsOk = false;
						}
					}
				}
				else
				{
					MovementPool.AcquireMoveCache(ref list, itemsLeft);

					if (!Check(map, m, list, xLeft, yLeft, startTop, startZ, m.CanSwim, m.CantWalk, out hold))
					{
						MovementPool.AcquireMoveCache(ref list, itemsRight);

						if (!Check(map, m, list, xRight, yRight, startTop, startZ, m.CanSwim, m.CantWalk, out hold))
						{
							moveIsOk = false;
						}
					}
				}
			}

			MovementPool.ClearMoveCache(ref list, true);

			if (!moveIsOk)
			{
				newZ = startZ;
			}

			return moveIsOk;
		}

		public bool CheckMovement(Mobile m, Direction d, out int newZ)
		{
			if (!Enabled && _Successor != null)
			{
				return _Successor.CheckMovement(m, d, out newZ);
			}

			return CheckMovement(m, m.Map, m.Location, d, out newZ);
		}

		private static bool Verify(Item item, int x, int y)
		{
			return item != null && item.AtWorldPoint(x, y);
		}

		private static bool Verify(Item item, TileFlag reqFlags, bool ignoreMovableImpassables)
		{
			if (item == null)
			{
				return false;
			}

			if (ignoreMovableImpassables && item.Movable && item.ItemData.Impassable)
			{
				return false;
			}

			if ((item.ItemData.Flags & reqFlags) == 0)
			{
				return false;
			}

			if (item is BaseMulti || item.ItemID > TileData.MaxItemValue)
			{
				return false;
			}

			return true;
		}

		private static bool Verify(Item item, TileFlag reqFlags, bool ignoreMovableImpassables, int x, int y)
		{
			return Verify(item, reqFlags, ignoreMovableImpassables) && Verify(item, x, y);
		}

		private static void GetStartZ(Mobile m, Map map, Point3D loc, IEnumerable<Item> itemList, out int zLow, out int zTop)
		{
			int xCheck = loc.X, yCheck = loc.Y;

			var landTile = map.Tiles.GetLandTile(xCheck, yCheck);
			var landData = TileData.LandTable[landTile.ID & TileData.MaxLandValue];
			var landBlocks = (landData.Flags & TileFlag.Impassable) != 0;

			if (landBlocks && m.CanSwim && (landData.Flags & TileFlag.Wet) != 0)
			{
				landBlocks = false;
			}
			else if (m.CantWalk && (landData.Flags & TileFlag.Wet) == 0)
			{
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
				{
					continue;
				}

				if ((tileData.Flags & TileFlag.Surface) == 0 && (!m.CanSwim || (tileData.Flags & TileFlag.Wet) == 0))
				{
					continue;
				}

				if (loc.Z < calcTop)
				{
					continue;
				}

				if (m.CantWalk && (tileData.Flags & TileFlag.Wet) == 0)
				{
					continue;
				}

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
				{
					continue;
				}

				if ((itemData.Flags & TileFlag.Surface) == 0 && (!m.CanSwim || (itemData.Flags & TileFlag.Wet) == 0))
				{
					continue;
				}

				if (loc.Z < calcTop)
				{
					continue;
				}

				if (m.CantWalk && (itemData.Flags & TileFlag.Wet) == 0)
				{
					continue;
				}

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
			{
				zLow = zTop = loc.Z;
			}
			else if (loc.Z > zTop)
			{
				zTop = loc.Z;
			}
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
			private static readonly object _MovePoolLock = new object();
			private static readonly Queue<List<Item>> _MoveCachePool = new Queue<List<Item>>(0x400);

			public static void AcquireMoveCache(ref List<Item> cache, IEnumerable<Item> items)
			{
				if (cache == null)
				{
					lock (_MovePoolLock)
					{
						cache = _MoveCachePool.Count > 0 ? _MoveCachePool.Dequeue() : new List<Item>(0x10);
					}
				}
				else
				{
					cache.Clear();
				}

				cache.AddRange(items);
			}

			public static void ClearMoveCache(ref List<Item> cache, bool free)
			{
				if (cache != null)
				{
					cache.Clear();
				}

				if (!free)
				{
					return;
				}

				lock (_MovePoolLock)
				{
					if (_MoveCachePool.Count < 0x400)
					{
						_MoveCachePool.Enqueue(cache);
					}
				}

				cache = null;
			}
		}
	}
}