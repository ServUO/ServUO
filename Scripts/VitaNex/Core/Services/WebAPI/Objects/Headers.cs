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
using System.Linq;
#endregion

namespace VitaNex.Web
{
	public class WebAPIHeaders : IDisposable, IEnumerable<KeyValuePair<string, string>>
	{
		private Dictionary<string, string> _Headers;

		public string this[string header]
		{
			get => _Headers.GetValue(header);
			set
			{
				if (value == null)
				{
					_Headers.Remove(header);
				}
				else
				{
					_Headers[header] = value;
				}
			}
		}

		public int Count => _Headers.Count;

		public WebAPIHeaders()
		{
			_Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		public WebAPIHeaders(IEnumerable<KeyValuePair<string, string>> headers)
			: this()
		{
			foreach (var kv in headers.Ensure())
			{
				this[kv.Key] = kv.Value;
			}
		}

		public void Clear()
		{
			_Headers.Clear();
		}

		public void Dispose()
		{
			_Headers = null;
		}

		public override string ToString()
		{
			return String.Join("\r\n", _Headers.Select(kv => String.Format("{0}: {1}", kv.Key, kv.Value))) + "\r\n\r\n";
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return _Headers.GetEnumerator();
		}
	}
}