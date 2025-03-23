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
using System.Collections;
using System.Collections.Generic;
#endregion

namespace VitaNex.Web
{
	public class WebAPIQueries : IDisposable, IEnumerable<KeyValuePair<string, string>>
	{
		private Dictionary<string, string> _Queries;

		public string this[string query]
		{
			get => _Queries.GetValue(query);
			set
			{
				if (value == null)
				{
					_Queries.Remove(query);
				}
				else
				{
					_Queries[query] = value;
				}
			}
		}

		public int Count => _Queries.Count;

		public WebAPIQueries()
		{
			_Queries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		public WebAPIQueries(IEnumerable<KeyValuePair<string, string>> queries)
			: this()
		{
			foreach (var kv in queries.Ensure())
			{
				this[kv.Key] = kv.Value;
			}
		}

		public WebAPIQueries(string query)
			: this(WebAPI.DecodeQuery(query))
		{ }

		public void Clear()
		{
			_Queries.Clear();
		}

		public void Dispose()
		{
			_Queries = null;
		}

		public override string ToString()
		{
			return WebAPI.EncodeQuery(_Queries);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return _Queries.GetEnumerator();
		}
	}
}