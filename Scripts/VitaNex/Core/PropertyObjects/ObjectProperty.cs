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
using System.Reflection;

using Server;
#endregion

namespace VitaNex
{
	public sealed class ObjectProperty : PropertyObject
	{
		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public string Name { get; set; }

		public Func<object, object> GetHandler { get; set; }
		public Action<object, object> SetHandler { get; set; }

		public ObjectProperty()
			: this(String.Empty)
		{ }

		public ObjectProperty(string name)
		{
			Name = name ?? String.Empty;
		}

		public ObjectProperty(GenericReader reader)
			: base(reader)
		{ }

		public override void Reset()
		{
			Name = String.Empty;
		}

		public override void Clear()
		{
			Name = String.Empty;
		}

		public override string ToString()
		{
			return Name;
		}

		public bool IsSupported(object o, object a)
		{
			return IsSupported(o as Type ?? o?.GetType(), a as Type ?? a?.GetType());
        }

		public bool IsSupported(Type t, Type v)
		{
			var p = t?.GetProperty(Name);

			return p != null && (v == null || p.PropertyType == v);
		}

		public bool IsSupported(object o)
		{
			return IsSupported(o, null);
		}

		public bool IsSupported(Type t)
		{
			return IsSupported(t, null);
		}

        public bool IsSupported<TObj>(object v)
        {
            return IsSupported(typeof(TObj), v);
		}

		public bool IsSupported<TObj>(Type v)
		{
			return IsSupported(typeof(TObj), v);
		}

		public bool IsSupported<TObj>()
		{
			return IsSupported(typeof(TObj), null);
		}

		public bool IsSupported<TObj, TVal>()
		{
			return IsSupported(typeof(TObj), typeof(TVal));
		}

		public T GetValue<T>(object o, T def = default(T))
		{
			if (String.IsNullOrWhiteSpace(Name) || o == null)
			{
				return def;
			}

			try
			{
				if (GetHandler != null)
				{
					return (T)GetHandler(o);
				}
			}
			catch
			{ }

			try
			{
				var t = o as Type ?? o.GetType();
				var f = o is Type ? BindingFlags.Static : BindingFlags.Instance;

				var p = t.GetProperty(Name, f | BindingFlags.Public);

				return (T)p.GetValue(o is Type ? null : o, null);
			}
			catch
			{
			}
			
			return def;
		}

		public object GetValue(object o, object def = null)
		{
			return GetValue<object>(o, def);
		}

		public bool SetValue(object o, object val)
		{
			if (String.IsNullOrWhiteSpace(Name) || o == null)
			{
				return false;
			}

			try
			{
				if (SetHandler != null)
				{
					SetHandler(o, val);
					return true;
				}
			}
			catch
			{ }

			try
			{
				var t = o as Type ?? o.GetType();
				var f = o is Type ? BindingFlags.Static : BindingFlags.Instance;

				var p = t.GetProperty(Name, f | BindingFlags.Public);

				p.SetValue(o is Type ? null : o, val, null);

				return true;
			}
			catch
			{
			}

			return false;
		}

		public bool Add(object o, object val)
		{
			if (val is IConvertible)
			{
				var cur = GetValue(o);

				if (cur is char c)
				{
					return SetValue(o, unchecked(c + Convert.ToChar(val)));
				}

				if (cur is sbyte b1)
				{
					return SetValue(o, unchecked(b1 + Convert.ToSByte(val)));
				}

				if (cur is byte b2)
				{
					return SetValue(o, unchecked(b2 + Convert.ToByte(val)));
				}

				if (cur is short s1)
				{
					return SetValue(o, unchecked(s1 + Convert.ToInt16(val)));
				}

				if (cur is ushort s2)
				{
					return SetValue(o, unchecked(s2 + Convert.ToUInt16(val)));
				}

				if (cur is int i1)
				{
					return SetValue(o, unchecked(i1 + Convert.ToInt32(val)));
				}

				if (cur is uint i2)
				{
					return SetValue(o, unchecked(i2 + Convert.ToUInt32(val)));
				}

				if (cur is long l1)
				{
					return SetValue(o, unchecked(l1 + Convert.ToInt64(val)));
				}

				if (cur is ulong l2)
				{
					return SetValue(o, unchecked(l2 + Convert.ToUInt64(val)));
				}

				if (cur is float f1)
				{
					return SetValue(o, unchecked(f1 + Convert.ToSingle(val)));
				}

				if (cur is decimal f2)
				{
					return SetValue(o, unchecked(f2 + Convert.ToDecimal(val)));
				}

				if (cur is double f3)
				{
					return SetValue(o, unchecked(f3 + Convert.ToDouble(val)));
				}

				if (cur is Enum e)
				{
					return SetValue(o, Enum.ToObject(e.GetType(), unchecked(Convert.ToInt64(e) + Convert.ToInt64(val))));
				}
			}

			if (val is TimeSpan ts)
			{
				var cur = GetValue(o);

				if (cur is TimeSpan cts)
				{
					return SetValue(o, cts + ts);
				}

				if (cur is DateTime cdt)
				{
					return SetValue(o, cdt + ts);
				}
			}

			return false;
		}

		public bool Subtract(object o, object val)
		{
			return Subtract(o, val, false);
		}

		public bool Subtract(object o, object val, bool limit)
		{
			if (val is IConvertible)
			{
				var cur = GetValue(o);

				if (cur is char c)
				{
					return (!limit || c >= Convert.ToChar(val)) && SetValue(o, unchecked(c - Convert.ToChar(val)));
				}

				if (cur is sbyte b1)
				{
					return (!limit || b1 >= Convert.ToSByte(val)) && SetValue(o, unchecked(b1 - Convert.ToSByte(val)));
				}

				if (cur is byte b2)
				{
					return (!limit || b2 >= Convert.ToByte(val)) && SetValue(o, unchecked(b2 - Convert.ToByte(val)));
				}

				if (cur is short s1)
				{
					return (!limit || s1 >= Convert.ToInt16(val)) && SetValue(o, unchecked(s1 - Convert.ToInt16(val)));
				}

				if (cur is ushort s2)
				{
					return (!limit || s2 >= Convert.ToUInt16(val)) && SetValue(o, unchecked(s2 - Convert.ToUInt16(val)));
				}

				if (cur is int i1)
				{
					return (!limit || i1 >= Convert.ToInt32(val)) && SetValue(o, unchecked(i1 - Convert.ToInt32(val)));
				}

				if (cur is uint i2)
				{
					return (!limit || i2 >= Convert.ToUInt32(val)) && SetValue(o, unchecked(i2 - Convert.ToUInt32(val)));
				}

				if (cur is long l1)
				{
					return (!limit || l1 >= Convert.ToInt64(val)) && SetValue(o, unchecked(l1 - Convert.ToInt64(val)));
				}

				if (cur is ulong l2)
				{
					return (!limit || l2 >= Convert.ToUInt64(val)) && SetValue(o, unchecked(l2 - Convert.ToUInt64(val)));
				}

				if (cur is float f1)
				{
					return (!limit || f1 >= Convert.ToSingle(val)) && SetValue(o, unchecked(f1 - Convert.ToSingle(val)));
				}

				if (cur is decimal f2)
				{
					return (!limit || f2 >= Convert.ToDecimal(val)) && SetValue(o, unchecked(f2 - Convert.ToDecimal(val)));
				}

				if (cur is double f3)
				{
					return (!limit || f3 >= Convert.ToDouble(val)) && SetValue(o, unchecked(f3 - Convert.ToDouble(val)));
				}

				if (cur is Enum e)
				{
					return (!limit || Convert.ToInt64(e) >= Convert.ToInt64(val)) && SetValue(o, Enum.ToObject(e.GetType(), unchecked(Convert.ToInt64(e) - Convert.ToInt64(val))));
				}
			}

			if (val is TimeSpan ts)
			{
				var cur = GetValue(o);

				if (cur is TimeSpan cts)
				{
					return (!limit || cts >= ts) && SetValue(o, cts - ts);
				}

				if (cur is DateTime cdt)
				{
					return (!limit || cdt >= DateTime.MinValue + ts) && SetValue(o, cdt - ts);
				}
			}

			return false;
		}

		public bool Consume(object o, object val)
		{
			return Subtract(o, val, true);
		}

		public bool SetDefault(object o)
		{
			var cur = GetValue(o);

			if (cur is sbyte || cur is byte || cur is short || cur is ushort || cur is int || cur is uint || cur is long || cur is ulong || cur is float || cur is decimal || cur is double || cur is Enum)
			{
				return SetValue(o, 0);
			}

			if (cur is TimeSpan)
			{
				return SetValue(o, TimeSpan.Zero);
			}

			if (cur is DateTime)
			{
				return SetValue(o, DateTime.MinValue);
			}

			if (cur is char)
			{
				return SetValue(o, ' ');
			}

			if (cur is string)
			{
				return SetValue(o, String.Empty);
			}

			return SetValue(o, null);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(Name);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Name = reader.ReadString();
		}
	}
}
