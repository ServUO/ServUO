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
using System.Drawing;

using Server;
using Server.Mobiles;

using VitaNex.IO;
using VitaNex.Network;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public static partial class TrashCollection
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static Type[] HandlerTypes { get; private set; }

		public static TrashCollectionOptions CMOptions { get; private set; }

		public static BinaryDataStore<string, BaseTrashHandler> Handlers { get; private set; }
		public static BinaryDataStore<Mobile, TrashProfile> Profiles { get; private set; }

		public static event Action<ItemTrashedEventArgs> OnTrashed;

		public static List<TrashProfile> GetSortedProfiles(bool today = false)
		{
			var list = new List<TrashProfile>(Profiles.Values);

			if (today)
			{
				list.Sort(InternalProfileSortToday);
			}
			else
			{
				list.Sort(InternalProfileSort);
			}

			return list;
		}

		private static int InternalProfileSort(TrashProfile a, TrashProfile b)
		{
			if (a == b)
			{
				return 0;
			}

			if (a == null)
			{
				return 1;
			}

			if (b == null)
			{
				return -1;
			}

			if (a.Deleted && b.Deleted)
			{
				return 0;
			}

			if (a.Deleted)
			{
				return 1;
			}

			if (b.Deleted)
			{
				return -1;
			}

			var aTotal = a.GetTokenTotal();
			var bTotal = b.GetTokenTotal();

			if (aTotal > bTotal)
			{
				return -1;
			}

			if (aTotal < bTotal)
			{
				return 1;
			}

			return 0;
		}

		private static int InternalProfileSortToday(TrashProfile a, TrashProfile b)
		{
			if (a == b)
			{
				return 0;
			}

			if (a == null)
			{
				return 1;
			}

			if (b == null)
			{
				return -1;
			}

			if (a.Deleted && b.Deleted)
			{
				return 0;
			}

			if (a.Deleted)
			{
				return 1;
			}

			if (b.Deleted)
			{
				return -1;
			}

			var when = DateTime.UtcNow;

			var aTotal = a.GetTokenTotal(when);
			var bTotal = b.GetTokenTotal(when);

			if (aTotal > bTotal)
			{
				return -1;
			}

			if (aTotal < bTotal)
			{
				return 1;
			}

			return 0;
		}

		private static void InternalHandlerSort()
		{
			if (Handlers.Count < 2)
			{
				return;
			}

			var list = new List<BaseTrashHandler>(Handlers.Values);
			list.Sort(InternalHandlerSort);

			Handlers.Clear();

			list.ForEach(
				h =>
				{
					if (!Handlers.ContainsKey(h.UID))
					{
						Handlers.Add(h.UID, h);
					}
					else
					{
						Handlers[h.UID] = h;
					}
				});

			list.Clear();
		}

		private static int InternalHandlerSort(BaseTrashHandler a, BaseTrashHandler b)
		{
			if (a == b)
			{
				return 0;
			}

			if (a == null)
			{
				return 1;
			}

			if (b == null)
			{
				return -1;
			}

			if (!a.Enabled && !b.Enabled)
			{
				return 0;
			}

			if (!a.Enabled)
			{
				return 1;
			}

			if (!b.Enabled)
			{
				return -1;
			}

			if (a.Priority > b.Priority)
			{
				return 1;
			}

			if (a.Priority < b.Priority)
			{
				return -1;
			}

			return 0;
		}

		public static void InvalidateHandlers()
		{
			InternalHandlerSort();
		}

		public static void Invoke(this ItemTrashedEventArgs e)
		{
			if (OnTrashed != null && e != null)
			{
				OnTrashed(e);
			}
		}

		public static TrashProfile EnsureProfile(Mobile m, bool replace = false)
		{
			if (m == null)
			{
				return null;
			}

			if (!Profiles.ContainsKey(m))
			{
				Profiles.Add(m, new TrashProfile(m));
			}
			else if (replace || Profiles[m] == null || Profiles[m].Deleted)
			{
				Profiles[m] = new TrashProfile(m);
			}

			return Profiles[m];
		}

		public static void Remove(this TrashProfile profile)
		{
			if (profile == null)
			{
				return;
			}

			if (Profiles.ContainsKey(profile.Owner))
			{
				Profiles.Remove(profile.Owner);
			}

			profile.Delete();
		}

		public static void AddTrashProperties(Item item, Mobile viewer, ExtendedOPL list)
		{
			if (!CMOptions.ModuleEnabled || !CMOptions.UseTrashedProps || item == null || item.Deleted || viewer == null ||
				viewer.Deleted || list == null || !(item is ITrashTokenProperties) || !(viewer is PlayerMobile))
			{
				return;
			}

			InternalAddTrashProperties(viewer, list);
		}

		public static void AddTrashProperties(Mobile m, Mobile viewer, ExtendedOPL list)
		{
			if (!CMOptions.ModuleEnabled || !CMOptions.UseTrashedProps || m == null || m.Deleted || viewer == null ||
				viewer.Deleted || list == null || !(m is ITrashTokenProperties) || !(viewer is PlayerMobile))
			{
				return;
			}

			InternalAddTrashProperties(viewer, list);
		}

		private static void InternalAddTrashProperties(Mobile viewer, ExtendedOPL list)
		{
			if (!CMOptions.ModuleEnabled || !CMOptions.UseTrashedProps || viewer == null || viewer.Deleted || list == null ||
				!(viewer is PlayerMobile))
			{
				return;
			}

			var p = EnsureProfile(viewer);

			var todayTotal = p.GetTokenTotal(DateTime.UtcNow);
			var total = p.GetTokenTotal();

			list.Add(
				"<basefont color=#{0:X6}>Total Tokens Earned: {1}",
				Color.SkyBlue.ToRgb(),
				total <= 0 ? "0" : total.ToString("#,#"));

			if (CMOptions.DailyLimit > 0)
			{
				const int blocks = 10;
				var cur = (Math.Max(0, Math.Min(CMOptions.DailyLimit, (double)todayTotal)) / CMOptions.DailyLimit) * 100.0;
				var left = (int)Math.Floor(cur / blocks);
				var right = blocks - left;

				list.Add("<basefont color=#{0:X6}>Tokens Earned Today:", Color.SkyBlue.ToRgb());
				list.Add(
					"[<basefont color=#{0:X6}>{1}<basefont color=#{2:X6}>{3}<basefont color=#{4:X6}>] {5}/{6}",
					Color.LimeGreen.ToRgb(),
					new string('=', left),
					Color.OrangeRed.ToRgb(),
					new string('=', right),
					Color.SkyBlue.ToRgb(),
					todayTotal <= 0 ? "0" : todayTotal.ToString("#,#"),
					CMOptions.DailyLimit.ToString("#,#"));
			}
			else
			{
				list.Add("Tokens Earned Today: {0}", todayTotal <= 0 ? "0" : todayTotal.ToString("#,#"));
			}

			list.Add("<basefont color=#ffffff>");
		}
	}
}