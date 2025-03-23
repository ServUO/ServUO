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

namespace VitaNex
{
	public abstract class SettingsObject<TFlags> : PropertyObject, IEnumerable<TFlags> where TFlags : struct, Enum
	{
		private static readonly Type _FlagsType = typeof(TFlags);

		public static readonly TFlags[] AllFlags = Enum.GetValues(_FlagsType).CastToArray<TFlags>();

		private TFlags _Flags;

		[CommandProperty(AccessLevel.Administrator)]
		public virtual TFlags Flags
		{
			get => _Flags;
			set
			{
				if (Equals(_Flags, value))
				{
					return;
				}

				var old = _Flags;

				_Flags = value;

				OnFlagsChanged(old);
			}
		}

		public virtual TFlags DefaultFlags => default(TFlags);

		public virtual bool this[TFlags f] { get => GetFlag(f); set => SetFlag(f, value); }

		public virtual bool this[int index] { get => this[AllFlags[index]]; set => this[AllFlags[index]] = value; }

		public int Length => AllFlags.Length;

		public Action<TFlags, bool> OnChanged { get; set; }

		public SettingsObject()
		{
			_Flags = DefaultFlags;
		}

		public SettingsObject(TFlags flags)
		{
			_Flags = flags;
		}

		public SettingsObject(GenericReader reader)
			: base(reader)
		{ }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return AllFlags.GetEnumerator();
		}

		IEnumerator<TFlags> IEnumerable<TFlags>.GetEnumerator()
		{
			return AllFlags.GetEnumerator<TFlags>();
		}

		public override void Clear()
		{
			_Flags = DefaultFlags;
		}

		public override void Reset()
		{
			_Flags = DefaultFlags;
		}

		protected virtual void OnFlagsChanged(TFlags old)
		{ }

		public void SetFlag(TFlags flag, bool value)
		{
			var t = Enum.GetUnderlyingType(_FlagsType);

			if (t.Name.StartsWith("U", StringComparison.OrdinalIgnoreCase))
			{
				var ul = Convert.ToUInt64(Enum.ToObject(_FlagsType, Flags));
				var ur = Convert.ToUInt64(Enum.ToObject(_FlagsType, flag));

				if (value)
				{
					flag = (TFlags)Enum.ToObject(_FlagsType, ul | ur);
				}
				else
				{
					flag = (TFlags)Enum.ToObject(_FlagsType, ul & ~ur);
				}
			}
			else
			{
				var l = Convert.ToInt64(Enum.ToObject(_FlagsType, Flags));
				var r = Convert.ToInt64(Enum.ToObject(_FlagsType, flag));

				if (value)
				{
					flag = (TFlags)Enum.ToObject(_FlagsType, l | r);
				}
				else
				{
					flag = (TFlags)Enum.ToObject(_FlagsType, l & ~r);
				}
			}

			Flags = flag;

			if (OnChanged != null)
			{
				OnChanged(flag, value);
			}
		}

		public bool GetFlag(TFlags flag)
		{
			var t = Enum.GetUnderlyingType(_FlagsType);

			if (t.Name.StartsWith("U", StringComparison.OrdinalIgnoreCase))
			{
				var ul = Convert.ToUInt64(Enum.ToObject(_FlagsType, Flags));
				var ur = Convert.ToUInt64(Enum.ToObject(_FlagsType, flag));

				if (ur == 0UL || ur == ~0UL)
				{
					return ul == ur;
				}

				return (ul & ur) != 0;
			}

			var l = Convert.ToInt64(Enum.ToObject(_FlagsType, Flags));
			var r = Convert.ToInt64(Enum.ToObject(_FlagsType, flag));

			if (r == 0L || r == ~0L)
			{
				return l == r;
			}

			return (l & r) != 0;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.WriteFlag(_Flags);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			_Flags = reader.ReadFlag<TFlags>();
		}

		public static implicit operator TFlags(SettingsObject<TFlags> o)
		{
			return o.Flags;
		}
	}
}
