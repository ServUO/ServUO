#region References
using System;
using System.Collections.Generic;

using Server.Items;
using Server.Mobiles;
#endregion

namespace Server.Movement
{
	public class MovementImpl : IMovementImpl
	{
		private const int PersonHeight = 16;
		private const int StepHeight = 2;

		private const TileFlag ImpassableSurface = TileFlag.Impassable | TileFlag.Surface;

		public static Point3D Goal;

		public static void Configure()
		{
			Movement.Impl = new MovementImpl();
		}

		private MovementImpl()
		{
		}

		private bool IsOk(Mobile m, bool ignoreDoors, bool ignoreSpellFields, int ourZ, int ourTop, StaticTile[] tiles, HashSet<Item> items)
		{
			for (var i = 0; i < tiles.Length; ++i)
			{
				var check = tiles[i];
				var itemData = TileData.ItemTable[check.ID & TileData.MaxItemValue];

				if ((itemData.Flags & ImpassableSurface) != 0) // Impassable || Surface
				{
					var checkZ = check.Z;
					var checkTop = checkZ + itemData.CalcHeight;

					if (checkTop > ourZ && ourTop > checkZ)
						return false;
				}
			}

			foreach (var item in items)
			{
				var itemID = item.ItemID & TileData.MaxItemValue;
				var itemData = TileData.ItemTable[itemID];
				var flags = itemData.Flags;

				if ((flags & ImpassableSurface) != 0) // Impassable || Surface
				{
					if (ignoreDoors && ((flags & TileFlag.Door) != 0 || itemID == 0x692 || itemID == 0x846 || itemID == 0x873 || (itemID >= 0x6F5 && itemID <= 0x6F6)))
					{
						if (m != null && item is BaseHouseDoor d && !d.CheckAccess(m))
							return false;

						continue;
					}

					if (ignoreSpellFields && (itemID == 0x82 || itemID == 0x3946 || itemID == 0x3956))
						continue;

					// hidden containers, per EA
					if ((flags & TileFlag.Container) != 0 && !item.Visible)
						continue;

					var checkZ = item.Z;
					var checkTop = checkZ + itemData.CalcHeight;

					if (checkTop > ourZ && ourTop > checkZ)
						return false;
				}
			}

			return true;
		}

		private readonly HashSet<Item>[] m_Pools = new HashSet<Item>[4]
		{
			new HashSet<Item>(),
			new HashSet<Item>(),
			new HashSet<Item>(),
			new HashSet<Item>(),
		};

		private readonly HashSet<Mobile>[] m_MobPools = new HashSet<Mobile>[3]
		{
			new HashSet<Mobile>(),
			new HashSet<Mobile>(),
			new HashSet<Mobile>(),
		};

		private readonly HashSet<Sector> m_Sectors = new HashSet<Sector>();

		private bool Check(Map map, IPoint3D p, HashSet<Item> items, HashSet<Mobile> mobiles, int x, int y, int startTop, int startZ, bool canSwim, bool cantWalk, out int newZ)
		{
			newZ = 0;

			var m = p as Mobile;

			var tiles = map.Tiles.GetStaticTiles(x, y, true);
			var landTile = map.Tiles.GetLandTile(x, y);

			var landBlocks = (TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Impassable) != 0;
			var considerLand = !landTile.Ignored;

			if (landBlocks && canSwim && (TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Wet) != 0)   //Impassable, Can Swim, and Is water.  Don't block it.
				landBlocks = false;
			else if (cantWalk && (TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Wet) == 0)   //Can't walk and it's not water
				landBlocks = true;

			int landZ = 0, landCenter = 0, landTop = 0;

			map.GetAverageZ(x, y, ref landZ, ref landCenter, ref landTop);

			var moveIsOk = false;

			var stepTop = startTop + StepHeight;
			var checkTop = startZ + PersonHeight;

			var ignoreDoors = AlwaysIgnoreDoors(p);
			var ignoreSpellFields = m is PlayerMobile && map != Map.Felucca;

			#region Tiles
			for (var i = 0; i < tiles.Length; ++i)
			{
				var tile = tiles[i];
				var itemData = TileData.ItemTable[tile.ID & TileData.MaxItemValue];
				var flags = itemData.Flags;

				if (m != null && m.Flying && (itemData.Name == "hover over" || (flags & TileFlag.HoverOver) != 0))
				{
					newZ = tile.Z;
					return true;
				}

				if (m is StygianDragon && map == Map.TerMur)
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

				if ((flags & ImpassableSurface) == TileFlag.Surface || (canSwim && (flags & TileFlag.Wet) != 0)) // Surface && !Impassable
				{
					if (cantWalk && (flags & TileFlag.Wet) == 0)
						continue;

					var itemZ = tile.Z;
					var itemTop = itemZ;
					var ourZ = itemZ + itemData.CalcHeight;
					var testTop = checkTop;

					if (moveIsOk)
					{
						var cmp = Math.Abs(ourZ - p.Z) - Math.Abs(newZ - p.Z);

						if (cmp > 0 || (cmp == 0 && ourZ > newZ))
							continue;
					}

					if (ourZ + PersonHeight > testTop)
						testTop = ourZ + PersonHeight;

					if (!itemData.Bridge)
						itemTop += itemData.Height;

					if (stepTop >= itemTop)
					{
						var landCheck = itemZ;

						if (itemData.Height >= StepHeight)
							landCheck += StepHeight;
						else
							landCheck += itemData.Height;

						if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landZ)
							continue;

						if (IsOk(m, ignoreDoors, ignoreSpellFields, ourZ, testTop, tiles, items))
						{
							newZ = ourZ;
							moveIsOk = true;
						}
					}
				}
			}
			#endregion

			#region Items
			foreach (var item in items)
			{
				var itemData = item.ItemData;
				var flags = itemData.Flags;

				if (m != null && m.Flying && (itemData.Name == "hover over" || (flags & TileFlag.HoverOver) != 0))
				{
					newZ = item.Z;
					return true;
				}

				if (!item.Movable && ((flags & ImpassableSurface) == TileFlag.Surface || (m != null && m.CanSwim && (flags & TileFlag.Wet) != 0))) // Surface && !Impassable && !Movable
				{
					if (cantWalk && (flags & TileFlag.Wet) == 0)
						continue;

					var itemZ = item.Z;
					var itemTop = itemZ;
					var ourZ = itemZ + itemData.CalcHeight;
					var ourTop = ourZ + PersonHeight;
					var testTop = checkTop;

					if (moveIsOk)
					{
						var cmp = Math.Abs(ourZ - p.Z) - Math.Abs(newZ - p.Z);

						if (cmp > 0 || (cmp == 0 && ourZ > newZ))
							continue;
					}

					if (ourZ + PersonHeight > testTop)
						testTop = ourZ + PersonHeight;

					if (!itemData.Bridge)
						itemTop += itemData.Height;

					if (stepTop >= itemTop)
					{
						var landCheck = itemZ;

						if (itemData.Height >= StepHeight)
							landCheck += StepHeight;
						else
							landCheck += itemData.Height;

						if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landZ)
							continue;

						if (IsOk(m, ignoreDoors, ignoreSpellFields, ourZ, testTop, tiles, items))
						{
							newZ = ourZ;
							moveIsOk = true;
						}
					}
				}
			}
			#endregion

			if (considerLand && !landBlocks && stepTop >= landZ)
			{
				var ourZ = landCenter;
				var testTop = checkTop;

				if (ourZ + PersonHeight > testTop)
					testTop = ourZ + PersonHeight;

				var shouldCheck = true;

				if (moveIsOk)
				{
					var cmp = Math.Abs(ourZ - p.Z) - Math.Abs(newZ - p.Z);

					if (cmp > 0 || (cmp == 0 && ourZ > newZ))
						shouldCheck = false;
				}

				if (shouldCheck && IsOk(m, ignoreDoors, ignoreSpellFields, ourZ, testTop, tiles, items))
				{
					newZ = ourZ;
					moveIsOk = true;
				}
			}

			#region Mobiles
			if (moveIsOk)
			{
				foreach (var mob in mobiles)
				{
					if (mob != m && (mob.Z + 15) > newZ && (newZ + 15) > mob.Z && !CanMoveOver(m, mob))
					{
						moveIsOk = false;
						break;
					}
				}
			}
			#endregion

			return moveIsOk;
		}

		private bool CanMoveOver(Mobile m, Mobile t)
		{
			return !t.Alive || m == null || !m.Alive || t.IsDeadBondedPet || m.IsDeadBondedPet || (t.Hidden && t.IsStaff());
		}

		public bool CheckMovement(IPoint3D p, Map map, Point3D loc, Direction d, out int newZ)
		{
			newZ = 0;

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

			var itemsStart = m_Pools[0];
			var itemsForward = m_Pools[1];
			var itemsLeft = m_Pools[2];
			var itemsRight = m_Pools[3];

			var ignoreMovableImpassables = IgnoresMovableImpassables(p);
			var reqFlags = ImpassableSurface;

			var m = p as Mobile;

			if (m != null && m.CanSwim)
				reqFlags |= TileFlag.Wet;

			var mobsForward = m_MobPools[0];
			var mobsLeft = m_MobPools[1];
			var mobsRight = m_MobPools[2];

			var checkMobs = p is BaseCreature c && !c.Controlled && (xForward != Goal.X || yForward != Goal.Y);

			if (checkDiagonals)
			{
				var sectorStart = map.GetSector(xStart, yStart);
				var sectorForward = map.GetSector(xForward, yForward);
				var sectorLeft = map.GetSector(xLeft, yLeft);
				var sectorRight = map.GetSector(xRight, yRight);

				var sectors = m_Sectors;

				sectors.Add(sectorStart);
				sectors.Add(sectorForward);
				sectors.Add(sectorLeft);
				sectors.Add(sectorRight);

				foreach (var sector in sectors)
				{
					foreach (var item in sector.Items)
					{
						if (ignoreMovableImpassables && item.Movable && (item.ItemData.Flags & ImpassableSurface) != 0)
							continue;

						if ((item.ItemData.Flags & reqFlags) == 0)
							continue;

						if (item is BaseMulti || item.ItemID > TileData.MaxItemValue)
							continue;

						if (sector == sectorStart && item.AtWorldPoint(xStart, yStart))
							itemsStart.Add(item);
						else if (sector == sectorForward && item.AtWorldPoint(xForward, yForward))
							itemsForward.Add(item);
						else if (sector == sectorLeft && item.AtWorldPoint(xLeft, yLeft))
							itemsLeft.Add(item);
						else if (sector == sectorRight && item.AtWorldPoint(xRight, yRight))
							itemsRight.Add(item);
					}

					if (checkMobs)
					{
						foreach (var mob in sector.Mobiles)
						{
							if (sector == sectorForward && mob.X == xForward && mob.Y == yForward)
								mobsForward.Add(mob);
							else if (sector == sectorLeft && mob.X == xLeft && mob.Y == yLeft)
								mobsLeft.Add(mob);
							else if (sector == sectorRight && mob.X == xRight && mob.Y == yRight)
								mobsRight.Add(mob);
						}
					}
				}

				if (m_Sectors.Count > 0)
					m_Sectors.Clear();
			}
			else
			{
				var sectorStart = map.GetSector(xStart, yStart);
				var sectorForward = map.GetSector(xForward, yForward);

				if (sectorStart == sectorForward)
				{
					foreach (var item in sectorStart.Items)
					{
						if (ignoreMovableImpassables && item.Movable && (item.ItemData.Flags & ImpassableSurface) != 0)
							continue;

						if ((item.ItemData.Flags & reqFlags) == 0)
							continue;

						if (item is BaseMulti || item.ItemID > TileData.MaxItemValue)
							continue;

						if (item.AtWorldPoint(xStart, yStart))
							itemsStart.Add(item);
						else if (item.AtWorldPoint(xForward, yForward))
							itemsForward.Add(item);
					}
				}
				else
				{
					foreach (var item in sectorForward.Items)
					{
						if (ignoreMovableImpassables && item.Movable && (item.ItemData.Flags & ImpassableSurface) != 0)
							continue;

						if ((item.ItemData.Flags & reqFlags) == 0)
							continue;

						if (item is BaseMulti || item.ItemID > TileData.MaxItemValue)
							continue;

						if (item.AtWorldPoint(xForward, yForward))
							itemsForward.Add(item);
					}

					foreach (var item in sectorStart.Items)
					{
						if (ignoreMovableImpassables && item.Movable && (item.ItemData.Flags & ImpassableSurface) != 0)
							continue;

						if ((item.ItemData.Flags & reqFlags) == 0)
							continue;

						if (item is BaseMulti || item.ItemID > TileData.MaxItemValue)
							continue;

						if (item.AtWorldPoint(xStart, yStart))
							itemsStart.Add(item);
					}
				}

				if (checkMobs)
				{
					foreach (var mob in sectorForward.Mobiles)
					{
						if (mob.X == xForward && mob.Y == yForward)
							mobsForward.Add(mob);
					}
				}
			}

			GetStartZ(p, map, loc, itemsStart, out var startZ, out var startTop);

			var moveIsOk = Check(map, p, itemsForward, mobsForward, xForward, yForward, startTop, startZ, m != null && m.CanSwim, m != null && m.CantWalk, out newZ);

			if (moveIsOk && m != null && !m.Player && startZ - newZ >= 20) // fall height
			{
				if (m.Flying)
				{
					newZ = startZ;
				}
				else
				{
					moveIsOk = false;
				}
			}

			if (moveIsOk && checkDiagonals)
			{
				if (m != null && m.Player && m.AccessLevel < AccessLevel.GameMaster)
				{
					if (!Check(map, p, itemsLeft, mobsLeft, xLeft, yLeft, startTop, startZ, m != null && m.CanSwim, m != null && m.CantWalk, out _) || !Check(map, m, itemsRight, mobsRight, xRight, yRight, startTop, startZ, m != null && m.CanSwim, m != null && m.CantWalk, out _))
						moveIsOk = false;
				}
				else
				{
					if (!Check(map, p, itemsLeft, mobsLeft, xLeft, yLeft, startTop, startZ, m != null && m.CanSwim, m != null && m.CantWalk, out _) && !Check(map, p, itemsRight, mobsRight, xRight, yRight, startTop, startZ, m != null && m.CanSwim, m != null && m.CantWalk, out _))
						moveIsOk = false;
				}
			}

			for (var i = 0; i < (checkDiagonals ? 4 : 2); ++i)
				m_Pools[i].Clear();

			for (var i = 0; i < (checkDiagonals ? 3 : 1); ++i)
				m_MobPools[i].Clear();

			if (!moveIsOk)
				newZ = startZ;

			return moveIsOk;
		}

		private void GetStartZ(IPoint3D p, Map map, Point3D loc, HashSet<Item> itemList, out int zLow, out int zTop)
		{
			var m = p as Mobile;

			int xCheck = loc.X, yCheck = loc.Y;

			var landTile = map.Tiles.GetLandTile(xCheck, yCheck);
			int landZ = 0, landCenter = 0, landTop = 0;
			var landBlocks = (TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Impassable) != 0;

			if (landBlocks && m != null && m.CanSwim && (TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Wet) != 0)
				landBlocks = false;
			else if (m != null && m.CantWalk && (TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Wet) == 0)
				landBlocks = true;

			map.GetAverageZ(xCheck, yCheck, ref landZ, ref landCenter, ref landTop);

			var considerLand = !landTile.Ignored;

			var zCenter = zLow = zTop = 0;
			var isSet = false;

			if (considerLand && !landBlocks && loc.Z >= landCenter)
			{
				zLow = landZ;
				zCenter = landCenter;

				if (!isSet || landTop > zTop)
					zTop = landTop;

				isSet = true;
			}

			var staticTiles = map.Tiles.GetStaticTiles(xCheck, yCheck, true);

			for (var i = 0; i < staticTiles.Length; ++i)
			{
				var tile = staticTiles[i];
				var id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

				var calcTop = (tile.Z + id.CalcHeight);

				if ((!isSet || calcTop >= zCenter) && ((id.Flags & TileFlag.Surface) != 0 || (m != null && m.CanSwim && (id.Flags & TileFlag.Wet) != 0)) && loc.Z >= calcTop)
				{
					if (m != null && m.CantWalk && (id.Flags & TileFlag.Wet) == 0)
						continue;

					zLow = tile.Z;
					zCenter = calcTop;

					var top = tile.Z + id.Height;

					if (!isSet || top > zTop)
						zTop = top;

					isSet = true;
				}
			}

			foreach (var item in itemList)
			{
				var id = item.ItemData;

				var calcTop = item.Z + id.CalcHeight;

				if ((!isSet || calcTop >= zCenter) && ((id.Flags & TileFlag.Surface) != 0 || (m != null && m.CanSwim && (id.Flags & TileFlag.Wet) != 0)) && loc.Z >= calcTop)
				{
					if (m != null && m.CantWalk && (id.Flags & TileFlag.Wet) == 0)
						continue;

					zLow = item.Z;
					zCenter = calcTop;

					var top = item.Z + id.Height;

					if (!isSet || top > zTop)
						zTop = top;

					isSet = true;
				}
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

		public static bool IgnoresMovableImpassables(IPoint3D p)
		{
			if (p is BaseCreature bc && bc.CanMoveOverObstacles && !bc.CanDestroyObstacles)
				return true;

			return p is PlayerMobile pm && !pm.Alive;
		}

		public static bool AlwaysIgnoreDoors(IPoint3D p)
		{
			if (p is Mobile m)
				return !m.Alive || m.Body.BodyID == 0x3DBb || m.IsDeadBondedPet;

			return true;
		}
	}
}
