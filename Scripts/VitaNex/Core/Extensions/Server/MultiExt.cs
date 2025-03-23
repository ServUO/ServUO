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
#endregion

namespace Server
{
	public static class MultiExtUtility
	{
		static MultiExtUtility()
		{
			ComponentsCache = new Dictionary<int, MultiComponentList>();
			WireframeCache = new Dictionary<int, Wireframe>();
			BoundsCache = new Dictionary<int, Rectangle3D>();
		}

		#region Components
		public static Dictionary<int, MultiComponentList> ComponentsCache { get; private set; }

		public static MultiComponentList GetComponents(this BaseMulti multi)
		{
			return GetComponents(multi, multi.ItemID);
		}

		public static MultiComponentList GetComponents(this BaseMulti multi, int multiID)
		{
			multiID &= 0x3FFF;

			if (multiID <= 0)
			{
				multiID = multi.ItemID;
			}

			return GetComponents(multiID);
		}

		public static MultiComponentList GetComponents(int multiID)
		{
			multiID &= 0x3FFF;


			if (ComponentsCache.TryGetValue(multiID, out var mcl) && mcl != null)
			{
				return mcl;
			}

			ComponentsCache[multiID] = mcl = MultiData.GetComponents(multiID);

			// Minax Fortress
			if (multiID == 0x1388)
			{
				// That tree...
				mcl.Remove(3405, 17, -13, 15);
				mcl.Remove(3406, 18, -14, 15);
				mcl.Remove(3393, 18, -14, 17);
			}

			if (mcl.List.Length == 0)
			{
				mcl.Resize(1, 1);
				mcl.Add(0, 0, 0, 0);
			}

			return mcl;
		}

		public static void TileAdd(this MultiComponentList mcl, int itemID, int x, int y, int w, int h, int z = 0, int d = 1)
		{
			TileAdd(mcl, itemID, new Rectangle2D(x, y, w, h), z, d);
		}

		public static void TileAdd(this MultiComponentList mcl, int itemID, Rectangle2D bounds, int z = 0, int d = 1)
		{
			TileAdd(mcl, itemID, bounds.ToRectangle3D(z, d));
		}

		public static void TileAdd(this MultiComponentList mcl, int itemID, Rectangle3D bounds)
		{
			for (var z = bounds.Start.Z; z < bounds.End.Z; z++)
			{
				for (var x = bounds.Start.X; x < bounds.End.X; x++)
				{
					for (var y = bounds.Start.Y; y < bounds.End.Y; y++)
					{
						mcl.Add(itemID, x, y, z);
					}
				}
			}
		}
		#endregion

		#region Wireframes
		public static Dictionary<int, Wireframe> WireframeCache { get; private set; }

		public static Wireframe GetWireframe(this BaseMulti multi, IPoint3D offset)
		{
			return GetWireframe(multi, multi.ItemID, offset);
		}

		public static Wireframe GetWireframe(this BaseMulti multi, IPoint3D offset, int hOffset)
		{
			return GetWireframe(multi, multi.ItemID, offset, hOffset);
		}

		public static Wireframe GetWireframe(this BaseMulti multi, IBlock3D offset)
		{
			return GetWireframe(multi, multi.ItemID, offset);
		}

		public static Wireframe GetWireframe(this BaseMulti multi, int multiID, IPoint3D offset)
		{
			return GetWireframe(multi, multiID, offset, 0);
		}

		public static Wireframe GetWireframe(this BaseMulti multi, int multiID, IBlock3D offset)
		{
			return GetWireframe(multi, multiID, offset, offset.H);
		}

		public static Wireframe GetWireframe(this BaseMulti multi, int multiID, IPoint3D offset, int hOffset)
		{
			var o = GetWireframe(multi, multiID);

			return new Wireframe(o.Select(b => b.Offset(offset.X, offset.Y, offset.Z, hOffset)));
		}

		public static Wireframe GetWireframe(this BaseMulti multi)
		{
			return GetWireframe(multi, multi.ItemID);
		}

		public static Wireframe GetWireframe(this BaseMulti multi, int multiID)
		{
			multiID &= 0x3FFF;

			if (multiID < 0)
			{
				multiID = multi.ItemID;
			}

			return GetWireframe(multiID);
		}

		public static Wireframe GetWireframe(int multiID)
		{
			multiID &= 0x3FFF;


			if (WireframeCache.TryGetValue(multiID, out var frame))
			{
				return frame;
			}

			frame = GetWireframe(GetComponents(multiID));

			WireframeCache[multiID] = frame;

			return frame;
		}

		public static Wireframe GetWireframe(this MultiComponentList mcl, IPoint3D offset)
		{
			return new Wireframe(GetWireframe(mcl).Select(b => b.Offset(offset.X, offset.Y, offset.Z)));
		}

		public static Wireframe GetWireframe(this MultiComponentList mcl, IBlock3D offset)
		{
			return new Wireframe(GetWireframe(mcl).Offset(offset.X, offset.Y, offset.Z, offset.H));
		}

		public static Wireframe GetWireframe(this MultiComponentList mcl)
		{
			if (mcl == null)
			{
				return Wireframe.Empty;
			}

			var frame = new Block3D[mcl.List.Length];

			frame.SetAll(
				i =>
				{
					var o = mcl.List[i];
					var h = Math.Max(5, TileData.ItemTable[o.m_ItemID].Height);

					return new Block3D(o.m_OffsetX, o.m_OffsetY, o.m_OffsetZ, h);
				});

			return new Wireframe(frame);
		}
		#endregion

		#region Bounds
		public static Dictionary<int, Rectangle3D> BoundsCache { get; private set; }

		public static Rectangle3D GetBoundsOffset(this BaseMulti multi)
		{
			return GetBoundsOffset(multi, multi.Location);
		}

		public static Rectangle3D GetBoundsOffset(this BaseMulti multi, Point3D offset)
		{
			return GetBounds(multi).Resize(offset.X, offset.Y, offset.Z);
		}

		public static Rectangle3D GetBounds(this BaseMulti multi)
		{
			return GetBounds(multi, multi.ItemID);
		}

		public static Rectangle3D GetBounds(this BaseMulti multi, int multiID)
		{
			multiID &= 0x3FFF;

			if (multiID <= 0)
			{
				multiID = multi.ItemID;
			}

			return GetBounds(multiID);
		}

		public static Rectangle3D GetBounds(int multiID)
		{
			multiID &= 0x3FFF;


			if (BoundsCache.TryGetValue(multiID, out var bounds))
			{
				return bounds;
			}

			var mcl = GetComponents(multiID);

			int minZ = mcl.List.Min(t => t.m_OffsetZ);
			var maxZ = mcl.List.Max(t => t.m_OffsetZ + Math.Max(1, TileData.ItemTable[t.m_ItemID].Height));

			if (multiID >= 24 && multiID <= 71)
			{
				if (multiID >= 24 && multiID <= 35)
				{
					maxZ = Math.Max(80, maxZ);
				}
				else if (multiID >= 36 && multiID <= 47)
				{
					maxZ = Math.Max(100, maxZ);
				}
				else if (multiID >= 48 && multiID <= 59)
				{
					maxZ = Math.Max(90, maxZ);
				}
				else if (multiID >= 60 && multiID <= 63)
				{
					maxZ = Math.Max(20, maxZ);
				}
				else if (multiID >= 64 && multiID <= 71)
				{
					maxZ = Math.Max(110, maxZ);
				}
			}

			bounds = new Rectangle3D(mcl.Min.X, mcl.Min.Y, minZ, mcl.Width + 1, mcl.Height + 1, maxZ - minZ);

			BoundsCache[multiID] = bounds;

			return bounds;
		}
		#endregion

		/*
		public static bool Contains(Point2D p)
		{
			return Contains(p.m_X, p.m_Y);
		}

		public static bool Contains(Point3D p)
		{
			return Contains(p.m_X, p.m_Y);
		}

		public static bool Contains(IPoint3D p)
		{
			return Contains(p.X, p.Y);
		}

		public static bool Contains(int x, int y)
		{
			MultiComponentList mcl = Components;

			x -= X + mcl.Min.m_X;
			y -= Y + mcl.Min.m_Y;

			return x >= 0 && x < mcl.Width && y >= 0 && y < mcl.Height && mcl.Tiles[x][y].Length > 0;
		}
		*/

		public static bool IsEmpty(this MultiComponentList mcl, int x, int y)
		{
			return x < 0 || x >= mcl.Width || y < 0 || y >= mcl.Height || mcl.Tiles[x][y].Length == 0;
		}

		public static bool HasEntry(this MultiComponentList mcl, int x, int y, int z)
		{
			return !IsEmpty(mcl, x, y) && mcl.Tiles[x][y].Any(t => t.Z == z);
		}

		public static bool HasEntry(this MultiComponentList mcl, int itemID, int x, int y, int z)
		{
			return !IsEmpty(mcl, x, y) && mcl.Tiles[x][y].Any(t => t.ID == itemID && t.Z == z);
		}

		public static Rectangle2D GetAbsoluteBounds(this MultiComponentList mcl)
		{
			return new Rectangle2D(mcl.Min.X, mcl.Min.Y, mcl.Width, mcl.Height);
		}
	}
}