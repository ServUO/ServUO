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

using Server.Targeting;
#endregion

namespace Server
{
	public static class MapExtUtility
	{
		#region StaticWaterTiles
		public static List<int> StaticWaterTiles = new List<int>
		{
			5465,
			6038,
			6039,
			6040,
			6041,
			6042,
			6043,
			6044,
			13422,
			13423,
			13424,
			13425,
			13426,
			13427,
			13428,
			13429,
			13430,
			13431,
			13432,
			13433,
			13434,
			13435,
			13436,
			13437,
			13438,
			13439,
			13440,
			13441,
			13442,
			13443,
			13445,
			13456,
			13457,
			13458,
			13459,
			13460,
			13461,
			13462,
			13463,
			13464,
			13465,
			13466,
			13467,
			13468,
			13469,
			13470,
			13471,
			13472,
			13473,
			13474,
			13475,
			13476,
			13477,
			13478,
			13479,
			13480,
			13481,
			13482,
			13483,
			13494,
			13495,
			13496,
			13497,
			13498,
			13499,
			13500,
			13501,
			13502,
			13503,
			13504,
			13505,
			13506,
			13507,
			13508,
			13509,
			13510,
			13511,
			13512,
			13513,
			13514,
			13515,
			13516,
			13517,
			13518,
			13519,
			13520,
			13521,
			13522,
			13523,
			13524,
			13525,
			13597,
			13598,
			13599,
			13600,
			13601,
			13602,
			13603,
			13604,
			13605,
			13606,
			13607,
			13608,
			13609,
			13610,
			13611,
			13612,
			13613,
			13614,
			13615,
			13616
		};
		#endregion

		#region LandWaterTiles
		public static List<int> LandWaterTiles = new List<int>
		{
			168,
			169,
			170,
			171,
			310,
			311,

			#region Coastlines
			76,
			77,
			78,
			79,
			80,
			81,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			93,
			94,
			95,
			96,
			97,
			98,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			107,
			108,
			109,
			110,
			111
			#endregion
		};
		#endregion

		#region LandCoastlineTiles
		public static List<int> LandCoastlineTiles = new List<int>
		{
			26,
			27,
			28,
			29,
			30,
			31,
			32,
			33,
			34,
			35,
			36,
			37,
			38,
			39,
			40,
			41,
			42,
			43,
			44,
			45,
			46,
			47,
			48,
			49,
			50
		};
		#endregion

		public static bool IsWater(this Item item)
		{
			return StaticWaterTiles.Contains(item.ItemID) || item.ItemData.Flags.HasFlag(TileFlag.Wet) ||
				   Insensitive.Contains(item.Name, "water");
		}

		public static bool IsWater(this LandTarget targ)
		{
			return LandWaterTiles.Contains(targ.TileID) || TileData.LandTable[targ.TileID].Flags.HasFlag(TileFlag.Wet) ||
				   Insensitive.Contains(TileData.LandTable[targ.TileID].Name, "water");
		}

		public static bool IsWater(this StaticTarget targ)
		{
			return StaticWaterTiles.Contains(targ.ItemID) || TileData.ItemTable[targ.ItemID].Flags.HasFlag(TileFlag.Wet) ||
				   Insensitive.Contains(TileData.ItemTable[targ.ItemID].Name, "water");
		}

		public static bool IsWater(this LandTile tile)
		{
			return LandWaterTiles.Contains(tile.ID) || TileData.LandTable[tile.ID].Flags.HasFlag(TileFlag.Wet) ||
				   Insensitive.Contains(TileData.LandTable[tile.ID].Name, "water");
		}

		public static bool IsWater(this StaticTile tile)
		{
			return StaticWaterTiles.Contains(tile.ID) || TileData.ItemTable[tile.ID].Flags.HasFlag(TileFlag.Wet) ||
				   Insensitive.Contains(TileData.ItemTable[tile.ID].Name, "water");
		}

		public static bool IsCoastline(this LandTile tile)
		{
			return LandCoastlineTiles.Contains(tile.ID) || (TileData.LandTable[tile.ID].Flags == TileFlag.Impassable &&
															Insensitive.Contains(TileData.LandTable[tile.ID].Name, "sand"));
		}

		public static bool HasWater(this Map map, IPoint2D p)
		{
			return IsWater(GetLandTile(map, p)) || GetStaticTiles(map, p).Any(IsWater);
		}

		public static bool HasLand(this Map map, IPoint2D p)
		{
			return !GetLandTile(map, p).Ignored;
		}

		public static LandTile GetLandTile(this Map map, IPoint2D p)
		{
			return map.Tiles.GetLandTile(p.X, p.Y);
		}

		public static StaticTile[] GetStaticTiles(this Map map, IPoint2D p)
		{
			return map.Tiles.GetStaticTiles(p.X, p.Y);
		}

		public static StaticTile[] GetStaticTiles(this Map map, IPoint2D p, bool multis)
		{
			return map.Tiles.GetStaticTiles(p.X, p.Y, multis);
		}

		public static Rectangle2D GetInnerBounds2D(this Map map)
		{
			switch (map.MapID)
			{
				case 0:
				case 1:
					return new Rectangle2D(0, 0, Math.Min(5120, map.Width), map.Height);
				case 3:
					return new Rectangle2D(512, 0, map.Width - 512, map.Height);
			}

			return new Rectangle2D(0, 0, map.Width, map.Height);
		}

		public static Rectangle3D GetInnerBounds3D(this Map map)
		{
			switch (map.MapID)
			{
				case 0:
				case 1:
					return new Rectangle3D(0, 0, Region.MinZ, Math.Min(5120, map.Width), map.Height, Region.MaxZ - Region.MinZ);
				case 3:
					return new Rectangle3D(512, 0, Region.MinZ, map.Width - 512, map.Height, Region.MaxZ - Region.MinZ);
			}

			return new Rectangle3D(0, 0, Region.MinZ, map.Width, map.Height, Region.MaxZ - Region.MinZ);
		}

		public static Rectangle2D GetBounds2D(this Map map)
		{
			return new Rectangle2D(0, 0, map.Width, map.Height);
		}

		public static Rectangle3D GetBounds3D(this Map map)
		{
			return new Rectangle3D(0, 0, Region.MinZ, map.Width, map.Height, Region.MaxZ - Region.MinZ);
		}
	}
}