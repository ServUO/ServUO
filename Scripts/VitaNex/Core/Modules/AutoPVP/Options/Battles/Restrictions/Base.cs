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

using Server;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	[PropertyObject]
	public abstract class PvPBattleRestrictionsBase<TKey> : PropertyObject, IEnumerable<TKey>
	{
		public static void SetFlag(ref ulong flags, ulong f, bool v)
		{
			if (v)
			{
				flags |= f;
			}
			else
			{
				flags &= ~f;
			}
		}

		public static bool GetFlag(ulong flags, ulong f)
		{
			return (flags & f) != 0;
		}

		private Dictionary<TKey, bool> _List = new Dictionary<TKey, bool>();

		public virtual Dictionary<TKey, bool> List
		{
			get => _List;
			set => _List = value ?? new Dictionary<TKey, bool>();
		}

		public bool this[TKey key] { get => IsRestricted(key); set => SetRestricted(key, value); }

		public PvPBattleRestrictionsBase()
		{
			Invalidate();
		}

		public PvPBattleRestrictionsBase(GenericReader reader)
			: base(reader)
		{ }

		public abstract void Invalidate();

		public virtual void Invert()
		{
			_List.Keys.ForEach(t => _List[t] = !_List[t]);
		}

		public virtual void Reset(bool val)
		{
			_List.Keys.ForEach(t => _List[t] = val);
		}

		public virtual bool Remove(TKey key)
		{
			return _List.Remove(key);
		}

		public override void Reset()
		{
			Reset(false);
		}

		public override void Clear()
		{
			_List.Clear();
		}

		public virtual void SetRestricted(TKey key, bool val)
		{
			if (key != null)
			{
				_List[key] = val;
			}
		}

		public virtual bool IsRestricted(TKey key)
		{
			return key != null && _List.ContainsKey(key) && _List[key];
		}

		public IEnumerator<TKey> GetEnumerator()
		{
			return _List.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.WriteDictionary(_List, SerializeEntry);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
					_List = reader.ReadDictionary(DeserializeEntry);
					break;
			}
		}

		public abstract void SerializeEntry(GenericWriter writer, TKey key, bool val);
		public abstract KeyValuePair<TKey, bool> DeserializeEntry(GenericReader reader);
	}
}