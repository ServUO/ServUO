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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace System
{
	public struct Numeral
		: IEnumerable<string>,
		  IEquatable<Numeral>,
		  IComparable<Numeral>,
		  IEquatable<double>,
		  IComparable<double>,
		  IEquatable<float>,
		  IComparable<float>,
		  IEquatable<sbyte>,
		  IComparable<sbyte>,
		  IEquatable<byte>,
		  IComparable<byte>,
		  IEquatable<short>,
		  IComparable<ushort>,
		  IEquatable<int>,
		  IComparable<uint>,
		  IEquatable<long>,
		  IComparable<ulong>,
		  IEquatable<string>,
		  IComparable<string>
	{
		private static readonly string[] _EmptyParts = new string[0];

		private static readonly string[][] _Numerals =
		{
			//
			new[] {"", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"}, // 1-9
			new[] {"", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"}, // 10-99
			new[] {"", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"}, // 100-999
			new[] {"", "M", "MM", "MMM", "MMMM"} // 1000-4999
		};

		public static readonly Numeral MinValue = new Numeral(-4999);
		public static readonly Numeral MaxValue = new Numeral(4999);

		private static bool ConvertToNumeral(double value, out double fix, out string to, out string[] numerals)
		{
			fix = 0;
			to = String.Empty;
			numerals = _EmptyParts;

			if (value > -1.0 && value < 1.0)
			{
				return false;
			}

			fix = Math.Truncate(value);

			if (fix == 0 || fix < MinValue || fix > MaxValue)
			{
				return false;
			}

			var negative = fix < 0;
			var number = fix.ToString(CultureInfo.InvariantCulture);

			if (negative)
			{
				number = number.TrimStart('-');
				to += '-';
			}

			numerals = new string[number.Length];

			for (int i = numerals.Length - 1, n = 0; i >= 0 && n < numerals.Length; i--, n++)
			{
				to += numerals[n] = _Numerals[i][Int32.Parse(Char.ToString(number[n]))];
			}

			return true;
		}

		private static bool ConvertToNumber(string value, out string fix, out double to, out string[] numerals)
		{
			fix = String.Empty;
			to = 0;
			numerals = _EmptyParts;

			if (String.IsNullOrWhiteSpace(value))
			{
				return false;
			}

			fix = value.Trim().ToUpper();

			if (String.IsNullOrWhiteSpace(fix) || fix.Length > 24)
			{
				return false;
			}

			var seek = 0;
			var skip = 0;
			var next = false;
			var exit = false;
			var numb = String.Empty;

			double power;
			string numeral;
			int index;

			while (!exit)
			{
				for (var i = _Numerals.Length - 1 - skip; i >= 0 && !next; i--)
				{
					power = Math.Pow(i, 10);

					for (var n = _Numerals[i].Length - 1; n >= 0 && !next; n--)
					{
						numeral = _Numerals[i][n];

						if (seek + numeral.Length > fix.Length)
						{
							continue;
						}

						index = fix.IndexOf(numeral, seek, numeral.Length, StringComparison.OrdinalIgnoreCase);

						if (index != seek)
						{
							continue;
						}

						++skip;
						to += power * n;
						seek += numeral.Length;
						numb += ',' + numeral;
						next = true;
					}
				}

				exit = !next || skip >= _Numerals.Length;
				next = false;
			}

			if (!String.IsNullOrWhiteSpace(numb))
			{
				numerals = numb.Split(',');
			}

			return true;
		}

		private readonly double _Number;
		private readonly string _Numeral;
		private readonly string[] _Parts;

		public int Length => _Parts.Length;

		public string this[int index] => _Parts[index];

		public Numeral(double value)
		{
			ConvertToNumeral(value, out _Number, out _Numeral, out _Parts);
		}

		public Numeral(string value)
		{
			ConvertToNumber(value, out _Numeral, out _Number, out _Parts);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Parts.GetEnumerator();
		}

		public IEnumerator<string> GetEnumerator()
		{
			return _Parts.GetEnumerator<string>();
		}

		public override string ToString()
		{
			return _Numeral;
		}

		public override int GetHashCode()
		{
			return _Numeral.GetHashCode();
		}

		#region Equality
		public override bool Equals(object obj)
		{
			if (obj is Numeral)
			{
				return Equals((Numeral)obj);
			}

			if (obj is double)
			{
				return Equals((double)obj);
			}

			if (obj is float)
			{
				return Equals((float)obj);
			}

			if (obj is sbyte)
			{
				return Equals((sbyte)obj);
			}

			if (obj is byte)
			{
				return Equals((byte)obj);
			}

			if (obj is short)
			{
				return Equals((short)obj);
			}

			if (obj is ushort)
			{
				return Equals((ushort)obj);
			}

			if (obj is int)
			{
				return Equals((int)obj);
			}

			if (obj is uint)
			{
				return Equals((uint)obj);
			}

			if (obj is long)
			{
				return Equals((long)obj);
			}

			if (obj is ulong)
			{
				return Equals((ulong)obj);
			}

			if (obj is string)
			{
				return Equals((string)obj);
			}

			try
			{
				return Equals(Convert.ToDouble(obj));
			}
			catch
			{ }

			try
			{
				return Equals(Convert.ToString(obj));
			}
			catch
			{ }

			return false;
		}

		public bool Equals(Numeral value)
		{
			return Equals(value._Number) && Equals(value._Numeral);
		}

		public bool Equals(double number)
		{
			return Equals(_Number, number);
		}

		public bool Equals(float number)
		{
			return Equals(_Number, number);
		}

		public bool Equals(sbyte number)
		{
			return Equals(_Number, number);
		}

		public bool Equals(byte number)
		{
			return Equals(_Number, number);
		}

		public bool Equals(short number)
		{
			return Equals(_Number, number);
		}

		public bool Equals(ushort number)
		{
			return Equals(_Number, number);
		}

		public bool Equals(int number)
		{
			return Equals(_Number, number);
		}

		public bool Equals(uint number)
		{
			return Equals(_Number, number);
		}

		public bool Equals(long number)
		{
			return Equals(_Number, number);
		}

		public bool Equals(ulong number)
		{
			return Equals(_Number, number);
		}

		public bool Equals(string numeral)
		{
			return Equals(_Numeral, numeral);
		}
		#endregion

		#region Comparison
		public int CompareTo(object obj)
		{
			if (obj is Numeral)
			{
				return CompareTo((Numeral)obj);
			}

			if (obj is double)
			{
				return CompareTo((double)obj);
			}

			if (obj is float)
			{
				return CompareTo((float)obj);
			}

			if (obj is sbyte)
			{
				return CompareTo((sbyte)obj);
			}

			if (obj is byte)
			{
				return CompareTo((byte)obj);
			}

			if (obj is short)
			{
				return CompareTo((short)obj);
			}

			if (obj is ushort)
			{
				return CompareTo((ushort)obj);
			}

			if (obj is int)
			{
				return CompareTo((int)obj);
			}

			if (obj is uint)
			{
				return CompareTo((uint)obj);
			}

			if (obj is long)
			{
				return CompareTo((long)obj);
			}

			if (obj is ulong)
			{
				return CompareTo((ulong)obj);
			}

			if (obj is string)
			{
				return CompareTo((string)obj);
			}

			try
			{
				return CompareTo(Convert.ToDouble(obj));
			}
			catch
			{ }

			try
			{
				return CompareTo(Convert.ToString(obj));
			}
			catch
			{ }

			return 0;
		}

		public int CompareTo(Numeral value)
		{
			return CompareTo(value._Number);
		}

		public int CompareTo(double number)
		{
			return _Number < number ? -1 : _Number > number ? 1 : 0;
		}

		public int CompareTo(float number)
		{
			return _Number < number ? -1 : _Number > number ? 1 : 0;
		}

		public int CompareTo(sbyte number)
		{
			return _Number < number ? -1 : _Number > number ? 1 : 0;
		}

		public int CompareTo(byte number)
		{
			return _Number < number ? -1 : _Number > number ? 1 : 0;
		}

		public int CompareTo(short number)
		{
			return _Number < number ? -1 : _Number > number ? 1 : 0;
		}

		public int CompareTo(ushort number)
		{
			return _Number < number ? -1 : _Number > number ? 1 : 0;
		}

		public int CompareTo(int number)
		{
			return _Number < number ? -1 : _Number > number ? 1 : 0;
		}

		public int CompareTo(uint number)
		{
			return _Number < number ? -1 : _Number > number ? 1 : 0;
		}

		public int CompareTo(long number)
		{
			return _Number < number ? -1 : _Number > number ? 1 : 0;
		}

		public int CompareTo(ulong number)
		{
			return _Number < number ? -1 : _Number > number ? 1 : 0;
		}

		public int CompareTo(string numeral)
		{
			return String.Compare(_Numeral, numeral, StringComparison.OrdinalIgnoreCase);
		}
		#endregion

		#region Implicit Conversion
		public static implicit operator Numeral(double value)
		{
			return new Numeral(value);
		}

		public static implicit operator Numeral(sbyte value)
		{
			return new Numeral(value);
		}

		public static implicit operator Numeral(byte value)
		{
			return new Numeral(value);
		}

		public static implicit operator Numeral(short value)
		{
			return new Numeral(value);
		}

		public static implicit operator Numeral(ushort value)
		{
			return new Numeral(value);
		}

		public static implicit operator Numeral(int value)
		{
			return new Numeral(value);
		}

		public static implicit operator Numeral(uint value)
		{
			return new Numeral(value);
		}

		public static implicit operator Numeral(long value)
		{
			return new Numeral(value);
		}

		public static implicit operator Numeral(ulong value)
		{
			return new Numeral(value);
		}

		/*public static implicit operator Numeral(string value)
		{
			return new Numeral(value);
		}*/

		public static implicit operator sbyte(Numeral value)
		{
			return Convert.ToSByte(value._Number);
		}

		public static implicit operator byte(Numeral value)
		{
			return Convert.ToByte(value._Number);
		}

		public static implicit operator short(Numeral value)
		{
			return Convert.ToInt16(value._Number);
		}

		public static implicit operator ushort(Numeral value)
		{
			return Convert.ToUInt16(value._Number);
		}

		public static implicit operator int(Numeral value)
		{
			return Convert.ToInt32(value._Number);
		}

		public static implicit operator uint(Numeral value)
		{
			return Convert.ToUInt32(value._Number);
		}

		public static implicit operator long(Numeral value)
		{
			return Convert.ToInt64(value._Number);
		}

		public static implicit operator ulong(Numeral value)
		{
			return Convert.ToUInt64(value._Number);
		}

		public static implicit operator string(Numeral value)
		{
			return value._Numeral;
		}
		#endregion

		#region Operator Overloads [Numeral]
		public static bool operator ==(Numeral l, Numeral r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, Numeral r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(Numeral l, Numeral r)
		{
			return l._Number > r._Number;
		}

		public static bool operator <(Numeral l, Numeral r)
		{
			return l._Number < r._Number;
		}

		public static Numeral operator +(Numeral l, Numeral r)
		{
			return l._Number + r._Number;
		}

		public static Numeral operator -(Numeral l, Numeral r)
		{
			return l._Number - r._Number;
		}

		public static Numeral operator *(Numeral l, Numeral r)
		{
			return l._Number * r._Number;
		}

		public static Numeral operator /(Numeral l, Numeral r)
		{
			return l._Number / r._Number;
		}

		public static Numeral operator %(Numeral l, Numeral r)
		{
			return l._Number % r._Number;
		}
		#endregion

		#region Operator Overloads [Double]
		public static bool operator ==(Numeral l, double r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, double r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(Numeral l, double r)
		{
			return l._Number > r;
		}

		public static bool operator <(Numeral l, double r)
		{
			return l._Number < r;
		}

		public static Numeral operator +(Numeral l, double r)
		{
			return l._Number + r;
		}

		public static Numeral operator -(Numeral l, double r)
		{
			return l._Number - r;
		}

		public static Numeral operator *(Numeral l, double r)
		{
			return l._Number * r;
		}

		public static Numeral operator /(Numeral l, double r)
		{
			return l._Number / r;
		}

		public static Numeral operator %(Numeral l, double r)
		{
			return l._Number % r;
		}

		public static bool operator ==(double l, Numeral r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(double l, Numeral r)
		{
			return !r.Equals(l);
		}

		public static bool operator >(double l, Numeral r)
		{
			return l > r._Number;
		}

		public static bool operator <(double l, Numeral r)
		{
			return l < r._Number;
		}

		public static Numeral operator +(double l, Numeral r)
		{
			return l + r._Number;
		}

		public static Numeral operator -(double l, Numeral r)
		{
			return l - r._Number;
		}

		public static Numeral operator *(double l, Numeral r)
		{
			return l * r._Number;
		}

		public static Numeral operator /(double l, Numeral r)
		{
			return l / r._Number;
		}

		public static Numeral operator %(double l, Numeral r)
		{
			return l % r._Number;
		}
		#endregion

		#region Operator Overloads [Float]
		public static bool operator ==(Numeral l, float r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, float r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(Numeral l, float r)
		{
			return l._Number > r;
		}

		public static bool operator <(Numeral l, float r)
		{
			return l._Number < r;
		}

		public static Numeral operator +(Numeral l, float r)
		{
			return l._Number + r;
		}

		public static Numeral operator -(Numeral l, float r)
		{
			return l._Number - r;
		}

		public static Numeral operator *(Numeral l, float r)
		{
			return l._Number * r;
		}

		public static Numeral operator /(Numeral l, float r)
		{
			return l._Number / r;
		}

		public static Numeral operator %(Numeral l, float r)
		{
			return l._Number % r;
		}

		public static bool operator ==(float l, Numeral r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(float l, Numeral r)
		{
			return !r.Equals(l);
		}

		public static bool operator >(float l, Numeral r)
		{
			return l > r._Number;
		}

		public static bool operator <(float l, Numeral r)
		{
			return l < r._Number;
		}

		public static Numeral operator +(float l, Numeral r)
		{
			return l + r._Number;
		}

		public static Numeral operator -(float l, Numeral r)
		{
			return l - r._Number;
		}

		public static Numeral operator *(float l, Numeral r)
		{
			return l * r._Number;
		}

		public static Numeral operator /(float l, Numeral r)
		{
			return l / r._Number;
		}

		public static Numeral operator %(float l, Numeral r)
		{
			return l % r._Number;
		}
		#endregion

		#region Operator Overloads [SByte]
		public static bool operator ==(Numeral l, sbyte r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, sbyte r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(Numeral l, sbyte r)
		{
			return l._Number > r;
		}

		public static bool operator <(Numeral l, sbyte r)
		{
			return l._Number < r;
		}

		public static Numeral operator +(Numeral l, sbyte r)
		{
			return l._Number + r;
		}

		public static Numeral operator -(Numeral l, sbyte r)
		{
			return l._Number - r;
		}

		public static Numeral operator *(Numeral l, sbyte r)
		{
			return l._Number * r;
		}

		public static Numeral operator /(Numeral l, sbyte r)
		{
			return l._Number / r;
		}

		public static Numeral operator %(Numeral l, sbyte r)
		{
			return l._Number % r;
		}

		public static bool operator ==(sbyte l, Numeral r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(sbyte l, Numeral r)
		{
			return !r.Equals(l);
		}

		public static bool operator >(sbyte l, Numeral r)
		{
			return l > r._Number;
		}

		public static bool operator <(sbyte l, Numeral r)
		{
			return l < r._Number;
		}

		public static Numeral operator +(sbyte l, Numeral r)
		{
			return l + r._Number;
		}

		public static Numeral operator -(sbyte l, Numeral r)
		{
			return l - r._Number;
		}

		public static Numeral operator *(sbyte l, Numeral r)
		{
			return l * r._Number;
		}

		public static Numeral operator /(sbyte l, Numeral r)
		{
			return l / r._Number;
		}

		public static Numeral operator %(sbyte l, Numeral r)
		{
			return l % r._Number;
		}
		#endregion

		#region Operator Overloads [Byte]
		public static bool operator ==(Numeral l, byte r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, byte r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(Numeral l, byte r)
		{
			return l._Number > r;
		}

		public static bool operator <(Numeral l, byte r)
		{
			return l._Number < r;
		}

		public static Numeral operator +(Numeral l, byte r)
		{
			return l._Number + r;
		}

		public static Numeral operator -(Numeral l, byte r)
		{
			return l._Number - r;
		}

		public static Numeral operator *(Numeral l, byte r)
		{
			return l._Number * r;
		}

		public static Numeral operator /(Numeral l, byte r)
		{
			return l._Number / r;
		}

		public static Numeral operator %(Numeral l, byte r)
		{
			return l._Number % r;
		}

		public static bool operator ==(byte l, Numeral r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(byte l, Numeral r)
		{
			return !r.Equals(l);
		}

		public static bool operator >(byte l, Numeral r)
		{
			return l > r._Number;
		}

		public static bool operator <(byte l, Numeral r)
		{
			return l < r._Number;
		}

		public static Numeral operator +(byte l, Numeral r)
		{
			return l + r._Number;
		}

		public static Numeral operator -(byte l, Numeral r)
		{
			return l - r._Number;
		}

		public static Numeral operator *(byte l, Numeral r)
		{
			return l * r._Number;
		}

		public static Numeral operator /(byte l, Numeral r)
		{
			return l / r._Number;
		}

		public static Numeral operator %(byte l, Numeral r)
		{
			return l % r._Number;
		}
		#endregion

		#region Operator Overloads [Short]
		public static bool operator ==(Numeral l, short r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, short r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(Numeral l, short r)
		{
			return l._Number > r;
		}

		public static bool operator <(Numeral l, short r)
		{
			return l._Number < r;
		}

		public static Numeral operator +(Numeral l, short r)
		{
			return l._Number + r;
		}

		public static Numeral operator -(Numeral l, short r)
		{
			return l._Number - r;
		}

		public static Numeral operator *(Numeral l, short r)
		{
			return l._Number * r;
		}

		public static Numeral operator /(Numeral l, short r)
		{
			return l._Number / r;
		}

		public static Numeral operator %(Numeral l, short r)
		{
			return l._Number % r;
		}

		public static bool operator ==(short l, Numeral r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(short l, Numeral r)
		{
			return !r.Equals(l);
		}

		public static bool operator >(short l, Numeral r)
		{
			return l > r._Number;
		}

		public static bool operator <(short l, Numeral r)
		{
			return l < r._Number;
		}

		public static Numeral operator +(short l, Numeral r)
		{
			return l + r._Number;
		}

		public static Numeral operator -(short l, Numeral r)
		{
			return l - r._Number;
		}

		public static Numeral operator *(short l, Numeral r)
		{
			return l * r._Number;
		}

		public static Numeral operator /(short l, Numeral r)
		{
			return l / r._Number;
		}

		public static Numeral operator %(short l, Numeral r)
		{
			return l % r._Number;
		}
		#endregion

		#region Operator Overloads [UShort]
		public static bool operator ==(Numeral l, ushort r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, ushort r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(Numeral l, ushort r)
		{
			return l._Number > r;
		}

		public static bool operator <(Numeral l, ushort r)
		{
			return l._Number < r;
		}

		public static Numeral operator +(Numeral l, ushort r)
		{
			return l._Number + r;
		}

		public static Numeral operator -(Numeral l, ushort r)
		{
			return l._Number - r;
		}

		public static Numeral operator *(Numeral l, ushort r)
		{
			return l._Number * r;
		}

		public static Numeral operator /(Numeral l, ushort r)
		{
			return l._Number / r;
		}

		public static Numeral operator %(Numeral l, ushort r)
		{
			return l._Number % r;
		}

		public static bool operator ==(ushort l, Numeral r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(ushort l, Numeral r)
		{
			return !r.Equals(l);
		}

		public static bool operator >(ushort l, Numeral r)
		{
			return l > r._Number;
		}

		public static bool operator <(ushort l, Numeral r)
		{
			return l < r._Number;
		}

		public static Numeral operator +(ushort l, Numeral r)
		{
			return l + r._Number;
		}

		public static Numeral operator -(ushort l, Numeral r)
		{
			return l - r._Number;
		}

		public static Numeral operator *(ushort l, Numeral r)
		{
			return l * r._Number;
		}

		public static Numeral operator /(ushort l, Numeral r)
		{
			return l / r._Number;
		}

		public static Numeral operator %(ushort l, Numeral r)
		{
			return l % r._Number;
		}
		#endregion

		#region Operator Overloads [Int]
		public static bool operator ==(Numeral l, int r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, int r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(Numeral l, int r)
		{
			return l._Number > r;
		}

		public static bool operator <(Numeral l, int r)
		{
			return l._Number < r;
		}

		public static Numeral operator +(Numeral l, int r)
		{
			return l._Number + r;
		}

		public static Numeral operator -(Numeral l, int r)
		{
			return l._Number - r;
		}

		public static Numeral operator *(Numeral l, int r)
		{
			return l._Number * r;
		}

		public static Numeral operator /(Numeral l, int r)
		{
			return l._Number / r;
		}

		public static Numeral operator %(Numeral l, int r)
		{
			return l._Number % r;
		}

		public static bool operator ==(int l, Numeral r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(int l, Numeral r)
		{
			return !r.Equals(l);
		}

		public static bool operator >(int l, Numeral r)
		{
			return l > r._Number;
		}

		public static bool operator <(int l, Numeral r)
		{
			return l < r._Number;
		}

		public static Numeral operator +(int l, Numeral r)
		{
			return l + r._Number;
		}

		public static Numeral operator -(int l, Numeral r)
		{
			return l - r._Number;
		}

		public static Numeral operator *(int l, Numeral r)
		{
			return l * r._Number;
		}

		public static Numeral operator /(int l, Numeral r)
		{
			return l / r._Number;
		}

		public static Numeral operator %(int l, Numeral r)
		{
			return l % r._Number;
		}
		#endregion

		#region Operator Overloads [UInt]
		public static bool operator ==(Numeral l, uint r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, uint r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(Numeral l, uint r)
		{
			return l._Number > r;
		}

		public static bool operator <(Numeral l, uint r)
		{
			return l._Number < r;
		}

		public static Numeral operator +(Numeral l, uint r)
		{
			return l._Number + r;
		}

		public static Numeral operator -(Numeral l, uint r)
		{
			return l._Number - r;
		}

		public static Numeral operator *(Numeral l, uint r)
		{
			return l._Number * r;
		}

		public static Numeral operator /(Numeral l, uint r)
		{
			return l._Number / r;
		}

		public static Numeral operator %(Numeral l, uint r)
		{
			return l._Number % r;
		}

		public static bool operator ==(uint l, Numeral r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(uint l, Numeral r)
		{
			return !r.Equals(l);
		}

		public static bool operator >(uint l, Numeral r)
		{
			return l > r._Number;
		}

		public static bool operator <(uint l, Numeral r)
		{
			return l < r._Number;
		}

		public static Numeral operator +(uint l, Numeral r)
		{
			return l + r._Number;
		}

		public static Numeral operator -(uint l, Numeral r)
		{
			return l - r._Number;
		}

		public static Numeral operator *(uint l, Numeral r)
		{
			return l * r._Number;
		}

		public static Numeral operator /(uint l, Numeral r)
		{
			return l / r._Number;
		}

		public static Numeral operator %(uint l, Numeral r)
		{
			return l % r._Number;
		}
		#endregion

		#region Operator Overloads [Long]
		public static bool operator ==(Numeral l, long r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, long r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(Numeral l, long r)
		{
			return l._Number > r;
		}

		public static bool operator <(Numeral l, long r)
		{
			return l._Number < r;
		}

		public static Numeral operator +(Numeral l, long r)
		{
			return l._Number + r;
		}

		public static Numeral operator -(Numeral l, long r)
		{
			return l._Number - r;
		}

		public static Numeral operator *(Numeral l, long r)
		{
			return l._Number * r;
		}

		public static Numeral operator /(Numeral l, long r)
		{
			return l._Number / r;
		}

		public static Numeral operator %(Numeral l, long r)
		{
			return l._Number % r;
		}

		public static bool operator ==(long l, Numeral r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(long l, Numeral r)
		{
			return !r.Equals(l);
		}

		public static bool operator >(long l, Numeral r)
		{
			return l > r._Number;
		}

		public static bool operator <(long l, Numeral r)
		{
			return l < r._Number;
		}

		public static Numeral operator +(long l, Numeral r)
		{
			return l + r._Number;
		}

		public static Numeral operator -(long l, Numeral r)
		{
			return l - r._Number;
		}

		public static Numeral operator *(long l, Numeral r)
		{
			return l * r._Number;
		}

		public static Numeral operator /(long l, Numeral r)
		{
			return l / r._Number;
		}

		public static Numeral operator %(long l, Numeral r)
		{
			return l % r._Number;
		}
		#endregion

		#region Operator Overloads [ULong]
		public static bool operator ==(Numeral l, ulong r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, ulong r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(Numeral l, ulong r)
		{
			return l._Number > r;
		}

		public static bool operator <(Numeral l, ulong r)
		{
			return l._Number < r;
		}

		public static Numeral operator +(Numeral l, ulong r)
		{
			return l._Number + r;
		}

		public static Numeral operator -(Numeral l, ulong r)
		{
			return l._Number - r;
		}

		public static Numeral operator *(Numeral l, ulong r)
		{
			return l._Number * r;
		}

		public static Numeral operator /(Numeral l, ulong r)
		{
			return l._Number / r;
		}

		public static Numeral operator %(Numeral l, ulong r)
		{
			return l._Number % r;
		}

		public static bool operator ==(ulong l, Numeral r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(ulong l, Numeral r)
		{
			return !r.Equals(l);
		}

		public static bool operator >(ulong l, Numeral r)
		{
			return l > r._Number;
		}

		public static bool operator <(ulong l, Numeral r)
		{
			return l < r._Number;
		}

		public static Numeral operator +(ulong l, Numeral r)
		{
			return l + r._Number;
		}

		public static Numeral operator -(ulong l, Numeral r)
		{
			return l - r._Number;
		}

		public static Numeral operator *(ulong l, Numeral r)
		{
			return l * r._Number;
		}

		public static Numeral operator /(ulong l, Numeral r)
		{
			return l / r._Number;
		}

		public static Numeral operator %(ulong l, Numeral r)
		{
			return l % r._Number;
		}
		#endregion

		#region Operator Overloads [String]
		public static bool operator ==(Numeral l, string r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(Numeral l, string r)
		{
			return !l.Equals(r);
		}

		public static bool operator ==(string l, Numeral r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(string l, Numeral r)
		{
			return !r.Equals(l);
		}
		#endregion
	}
}
