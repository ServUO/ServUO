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

using VitaNex.Collections;

using CollectionI =
	System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, System.Action>>;
using Dictionary = System.Collections.Generic.Dictionary<string, System.Action>;
using DictionaryI = System.Collections.Generic.IDictionary<string, System.Action>;
using KeyValuePair = System.Collections.Generic.KeyValuePair<string, System.Action>;
#endregion

namespace VitaNex.SuperGumps
{
	public class SuperGumpLayout : DictionaryI, IDisposable
	{
		private Dictionary _Entries;

		public bool IsDisposed => _Entries == null;

		public int Count => _Entries.Count;

		public Dictionary.KeyCollection Keys => _Entries.Keys;
		public Dictionary.ValueCollection Values => _Entries.Values;

		ICollection<string> DictionaryI.Keys => Keys;
		ICollection<Action> DictionaryI.Values => Values;

		bool CollectionI.IsReadOnly => ((CollectionI)_Entries).IsReadOnly;

		public Action this[string xpath] { get => _Entries.GetValue(xpath); set => _Entries[xpath] = value; }

		public SuperGumpLayout()
		{
			_Entries = DictionaryPool<string, Action>.AcquireObject();
		}

		public SuperGumpLayout(DictionaryI entries)
			: this()
		{
			foreach (var kv in entries)
			{
				Add(kv.Key, kv.Value);
			}
		}

		public SuperGumpLayout(IEnumerable<KeyValuePair> entries)
			: this()
		{
			foreach (var kv in entries)
			{
				Add(kv.Key, kv.Value);
			}
		}

		void CollectionI.Add(KeyValuePair item)
		{
			((CollectionI)_Entries).Add(item);
		}

		bool CollectionI.Remove(KeyValuePair item)
		{
			return ((CollectionI)_Entries).Remove(item);
		}

		bool CollectionI.Contains(KeyValuePair item)
		{
			return ((CollectionI)_Entries).Contains(item);
		}

		void CollectionI.CopyTo(KeyValuePair[] array, int arrayIndex)
		{
			((CollectionI)_Entries).CopyTo(array, arrayIndex);
		}

		bool DictionaryI.TryGetValue(string xpath, out Action value)
		{
			return Find(xpath, out value);
		}

		bool DictionaryI.ContainsKey(string xpath)
		{
			return Contains(xpath);
		}

		public bool Find(string key, out Action value)
		{
			return _Entries.TryGetValue(key, out value);
		}

		public bool Contains(string xpath)
		{
			return _Entries.ContainsKey(xpath);
		}

		public bool Contains(Action value)
		{
			return _Entries.ContainsValue(value);
		}

		public bool Remove(string xpath)
		{
			return _Entries.Remove(xpath);
		}

		public void Clear()
		{
			_Entries.Clear();
		}

		public void Combine(string xpath, Action value)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			if (Contains(xpath))
			{
				_Entries[xpath] += value;
			}
			else
			{
				_Entries[xpath] = value;
			}
		}

		public void Combine<T>(string xpath, Action<T> value, T state)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			if (Contains(xpath))
			{
				_Entries[xpath] += () => value(state);
			}
			else
			{
				_Entries[xpath] = () => value(state);
			}
		}

		public void Combine(string xpath, Action<string> value)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			if (Contains(xpath))
			{
				_Entries[xpath] += () => value(xpath);
			}
			else
			{
				_Entries[xpath] = () => value(xpath);
			}
		}

		public void Combine<T>(string xpath, Action<string, T> value, T state)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			if (Contains(xpath))
			{
				_Entries[xpath] += () => value(xpath, state);
			}
			else
			{
				_Entries[xpath] = () => value(xpath, state);
			}
		}

		public void Replace(string xpath, Action value)
		{
			if (String.IsNullOrWhiteSpace(xpath))
			{
				return;
			}

			if (value == null)
			{
				Remove(xpath);
			}
			else
			{
				_Entries[xpath] = value;
			}
		}

		public void Replace<T>(string xpath, Action<T> value, T state)
		{
			if (String.IsNullOrWhiteSpace(xpath))
			{
				return;
			}

			if (value == null)
			{
				Remove(xpath);
			}
			else
			{
				_Entries[xpath] = () => value(state);
			}
		}

		public void Replace(string xpath, Action<string> value)
		{
			if (String.IsNullOrWhiteSpace(xpath))
			{
				return;
			}

			if (value == null)
			{
				Remove(xpath);
			}
			else
			{
				_Entries[xpath] = () => value(xpath);
			}
		}

		public void Replace<T>(string xpath, Action<string, T> value, T state)
		{
			if (String.IsNullOrWhiteSpace(xpath))
			{
				return;
			}

			if (value == null)
			{
				Remove(xpath);
			}
			else
			{
				_Entries[xpath] = () => value(xpath, state);
			}
		}

		public void Add(string xpath, Action value)
		{
			if (!String.IsNullOrWhiteSpace(xpath) && value != null)
			{
				_Entries.Add(xpath, value);
			}
		}

		public void Add<T>(string xpath, Action<T> value, T state)
		{
			if (!String.IsNullOrWhiteSpace(xpath) && value != null)
			{
				_Entries.Add(xpath, () => value(state));
			}
		}

		public void Add(string xpath, Action<string> value)
		{
			if (!String.IsNullOrWhiteSpace(xpath) && value != null)
			{
				_Entries.Add(xpath, () => value(xpath));
			}
		}

		public void Add<T>(string xpath, Action<string, T> value, T state)
		{
			if (!String.IsNullOrWhiteSpace(xpath) && value != null)
			{
				_Entries.Add(xpath, () => value(xpath, state));
			}
		}

		public void AddBefore(string search, string xpath, Action value)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			var index = Keys.IndexOf(search);

			if (index < 0)
			{
				index = 0;
			}

			_Entries.Insert(index, xpath, value);
		}

		public void AddBefore<T>(string search, string xpath, Action<T> value, T state)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			var index = Keys.IndexOf(search);

			if (index < 0)
			{
				index = 0;
			}

			_Entries.Insert(index, xpath, () => value(state));
		}

		public void AddBefore(string search, string xpath, Action<string> value)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			var index = Keys.IndexOf(search);

			if (index < 0)
			{
				index = 0;
			}

			_Entries.Insert(index, xpath, () => value(xpath));
		}

		public void AddBefore<T>(string search, string xpath, Action<string, T> value, T state)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			var index = Keys.IndexOf(search);

			if (index < 0)
			{
				index = 0;
			}

			_Entries.Insert(index, xpath, () => value(xpath, state));
		}

		public void AddAfter(string search, string xpath, Action value)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			var index = Keys.IndexOf(search);

			if (index < 0)
			{
				index = _Entries.Count - 1;
			}

			_Entries.Insert(index + 1, xpath, value);
		}

		public void AddAfter<T>(string search, string xpath, Action<T> value, T state)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			var index = Keys.IndexOf(search);

			if (index < 0)
			{
				index = _Entries.Count - 1;
			}

			_Entries.Insert(index + 1, xpath, () => value(state));
		}

		public void AddAfter(string search, string xpath, Action<string> value)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			var index = Keys.IndexOf(search);

			if (index < 0)
			{
				index = _Entries.Count - 1;
			}

			_Entries.Insert(index + 1, xpath, () => value(xpath));
		}

		public void AddAfter<T>(string search, string xpath, Action<string, T> value, T state)
		{
			if (String.IsNullOrWhiteSpace(xpath) || value == null)
			{
				return;
			}

			var index = Keys.IndexOf(search);

			if (index < 0)
			{
				index = _Entries.Count - 1;
			}

			_Entries.Insert(index + 1, xpath, () => value(xpath, state));
		}

		public void ApplyTo(SuperGump gump)
		{
			foreach (var renderer in Values.Where(o => o != null))
			{
				renderer();
			}
		}

		public void Dispose()
		{
			if (_Entries != null)
			{
				ObjectPool.Free(ref _Entries);
			}
		}

		public IEnumerator<KeyValuePair> GetEnumerator()
		{
			return _Entries.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}