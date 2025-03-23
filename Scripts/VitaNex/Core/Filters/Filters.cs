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
using System.IO;
using System.Linq;

using Server.Mobiles;

using VitaNex;
using VitaNex.IO;
#endregion

namespace Server
{
	public static class Filters
	{
		private static readonly Dictionary<PlayerMobile, List<IFilter>> _Filters;

		private static readonly FileInfo _Persistence;

		private static bool _Configured;

		static Filters()
		{
			_Filters = new Dictionary<PlayerMobile, List<IFilter>>();

			_Persistence = IOUtility.EnsureFile(VitaNexCore.SavesDirectory + "/Filters/Profiles.bin");
		}

		public static void Configure()
		{
			if (_Configured)
			{
				return;
			}

			_Configured = true;

			EventSink.WorldLoad += Load;
			EventSink.WorldSave += Save;
		}

		private static void Save(WorldSaveEventArgs e)
		{
			_Persistence.Serialize(SerializeFilters);
		}

		private static void Load()
		{
			_Persistence.Deserialize(DeserializeFilters);
		}

		public static IEnumerable<IFilter> GetFilters(this PlayerMobile m)
		{
			return (_Filters.GetValue(m) ?? Enumerable.Empty<IFilter>()).AsEnumerable();
		}

		public static IFilter GetFilter(this PlayerMobile m, Type type)
		{
			if (!_Filters.TryGetValue(m, out var filters) || filters == null)
			{
				filters = _Filters[m] = new List<IFilter>();
			}

			var filter = filters.Find(f => f.TypeEquals(type));

			if (filter == null)
			{
				filters.Add(filter = type.CreateInstance<IFilter>());
			}

			return filter;
		}

		public static TFilter GetFilter<TFilter>(this PlayerMobile m)
			where TFilter : IFilter
		{
			if (!_Filters.TryGetValue(m, out var filters) || filters == null)
			{
				filters = _Filters[m] = new List<IFilter>();
			}

			var type = typeof(TFilter);
			var filter = (TFilter)filters.Find(f => f.TypeEquals(type));

			if (filter == null)
			{
				filters.Add(filter = typeof(TFilter).CreateInstance<TFilter>());
			}

			return filter;
		}

		private static void SerializeFilters(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.WriteDictionary(
				_Filters,
				(w, m, f) =>
				{
					w.Write(m);

					w.WriteBlockList(
						f,
						(w2, o) =>
						{
							w2.WriteType(o);
							o.Serialize(w2);
						});
				});
		}

		private static void DeserializeFilters(GenericReader reader)
		{
			reader.GetVersion();

			reader.ReadDictionary(
				r =>
				{
					var m = r.ReadMobile<PlayerMobile>();
					var f = r.ReadBlockList(
						r2 =>
						{
							var t = r2.ReadType();
							var o = t.CreateInstanceSafe<IFilter>(r2);

							if (o != null)
							{
								return o;
							}

							o = t.CreateInstanceSafe<IFilter>();

							if (o != null)
							{
								o.Deserialize(r2);
							}

							return o;
						});

					return new KeyValuePair<PlayerMobile, List<IFilter>>(m, f);
				},
				_Filters);
		}
	}
}