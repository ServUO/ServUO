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

using VitaNex;
using VitaNex.Network;
#endregion

namespace Server
{
	public static class EntityExtUtility
	{
		public static ObjectPropertyList GetOPL(this IEntity e)
		{
			return ExtendedOPL.ResolveOPL(e);
		}

		public static ObjectPropertyList GetOPL(this IEntity e, Mobile viewer)
		{
			return ExtendedOPL.ResolveOPL(e, viewer);
		}

		public static ObjectPropertyList GetOPL(this IEntity e, bool headerOnly)
		{
			return ExtendedOPL.ResolveOPL(e, headerOnly);
		}

		public static ObjectPropertyList GetOPL(this IEntity e, Mobile viewer, bool headerOnly)
		{
			return ExtendedOPL.ResolveOPL(e, viewer, headerOnly);
		}

		public static string GetOPLHeader(this IEntity e)
		{
			ObjectPropertyList opl = null;

			try
			{
				opl = GetOPL(e, true);

				if (opl != null)
				{
					return opl.DecodePropertyListHeader();
				}

				return String.Empty;
			}
			catch
			{
				return String.Empty;
			}
			finally
			{
				if (opl != null)
				{
					opl.Release();
				}
			}
		}

		public static string GetOPLHeader(this IEntity e, ClilocLNG lng)
		{
			ObjectPropertyList opl = null;

			try
			{
				opl = GetOPL(e, true);

				if (opl != null)
				{
					return opl.DecodePropertyListHeader(lng);
				}

				return String.Empty;
			}
			catch
			{
				return String.Empty;
			}
			finally
			{
				if (opl != null)
				{
					opl.Release();
				}
			}
		}

		public static string GetOPLHeader(this IEntity e, Mobile viewer)
		{
			ObjectPropertyList opl = null;

			try
			{
				opl = GetOPL(e, viewer, true);

				if (opl != null)
				{
					return opl.DecodePropertyListHeader(viewer);
				}

				return String.Empty;
			}
			catch
			{
				return String.Empty;
			}
			finally
			{
				if (opl != null)
				{
					opl.Release();
				}
			}
		}

		public static IEnumerable<string> GetOPLStrings(this IEntity e)
		{
			ObjectPropertyList opl = null;

			try
			{
				opl = GetOPL(e);

				if (opl != null)
				{
					return opl.DecodePropertyList();
				}

				return Enumerable.Empty<string>();
			}
			catch
			{
				return Enumerable.Empty<string>();
			}
			finally
			{
				if (opl != null)
				{
					opl.Release();
				}
			}
		}

		public static IEnumerable<string> GetOPLStrings(this IEntity e, ClilocLNG lng)
		{
			ObjectPropertyList opl = null;

			try
			{
				opl = GetOPL(e);

				if (opl != null)
				{
					return opl.DecodePropertyList(lng);
				}

				return Enumerable.Empty<string>();
			}
			catch
			{
				return Enumerable.Empty<string>();
			}
			finally
			{
				if (opl != null)
				{
					opl.Release();
				}
			}
		}

		public static IEnumerable<string> GetOPLStrings(this IEntity e, Mobile viewer)
		{
			ObjectPropertyList opl = null;

			try
			{
				opl = GetOPL(e, viewer);

				if (opl != null)
				{
					return opl.DecodePropertyList(viewer);
				}

				return Enumerable.Empty<string>();
			}
			catch
			{
				return Enumerable.Empty<string>();
			}
			finally
			{
				if (opl != null)
				{
					opl.Release();
				}
			}
		}

		public static string GetOPLString(this IEntity e)
		{
			return String.Join("\n", GetOPLStrings(e));
		}

		public static string GetOPLString(this IEntity e, ClilocLNG lng)
		{
			return String.Join("\n", GetOPLStrings(e, lng));
		}

		public static string GetOPLString(this IEntity e, Mobile viewer)
		{
			return String.Join("\n", GetOPLStrings(e, viewer));
		}

		public static string ResolveName(this IEntity e, Mobile viewer = null)
		{
			if (e is Item)
			{
				return ((Item)e).ResolveName(viewer);
			}

			if (e is Mobile)
			{
				return ((Mobile)e).GetFullName(viewer);
			}

			if (String.IsNullOrEmpty(e.Name))
			{
				return e.GetTypeName(false);
			}

			return e.Name;
		}

		public static bool IsInside(this IEntity e)
		{
			if (e != null && e.Map != null && e.Map != Map.Internal)
			{
				return e.IsInside(e.Map);
			}

			return false;
		}

		public static bool IsOutside(this IEntity e)
		{
			return !IsInside(e);
		}

		public static bool Intersects(this IEntity e, IPoint3D o)
		{
			return Block3D.Intersects(e, o);
		}
	}
}