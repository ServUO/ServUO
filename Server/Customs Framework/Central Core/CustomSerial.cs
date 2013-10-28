#region Header
// **********
// ServUO - CustomSerial.cs
// **********
#endregion

#region References
using System;

using Server;
#endregion

namespace CustomsFramework
{
	public struct CustomSerial : IComparable, IComparable<CustomSerial>
	{
		public static readonly CustomSerial MinusOne = new CustomSerial(-1);
		public static readonly CustomSerial Zero = new CustomSerial(0);
		private static CustomSerial _LastCustom = Zero;
		private readonly int _Serial;

		private CustomSerial(int serial)
		{
			_Serial = serial;
		}

		public static CustomSerial LastCore { get { return _LastCustom; } }

		public static CustomSerial NewCustom
		{
			get
			{
				while (World.GetData(_LastCustom = (_LastCustom + 1)) != null)
				{ }

				return _LastCustom;
			}
		}

		public int Value { get { return _Serial; } }
		public bool IsValid { get { return (_Serial > 0); } }

		public override int GetHashCode()
		{
			return _Serial;
		}

		public int CompareTo(CustomSerial other)
		{
			return _Serial.CompareTo(other._Serial);
		}

		public int CompareTo(object other)
		{
			if (other is CustomSerial)
			{
				return CompareTo((CustomSerial)other);
			}
			else if (other == null)
			{
				return -1;
			}

			throw new ArgumentException();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is CustomSerial))
			{
				return false;
			}

			return ((CustomSerial)obj)._Serial == _Serial;
		}

		public override string ToString()
		{
			return String.Format("0x{0:X8}", _Serial);
		}

		public static bool operator ==(CustomSerial first, CustomSerial second)
		{
			return first._Serial == second._Serial;
		}

		public static bool operator !=(CustomSerial first, CustomSerial second)
		{
			return first._Serial != second._Serial;
		}

		public static bool operator >(CustomSerial first, CustomSerial second)
		{
			return first._Serial > second._Serial;
		}

		public static bool operator <(CustomSerial first, CustomSerial second)
		{
			return first._Serial < second._Serial;
		}

		public static bool operator >=(CustomSerial first, CustomSerial second)
		{
			return first._Serial >= second._Serial;
		}

		public static bool operator <=(CustomSerial first, CustomSerial second)
		{
			return first._Serial <= second._Serial;
		}

		public static implicit operator int(CustomSerial serial)
		{
			return serial._Serial;
		}

		public static implicit operator CustomSerial(int serial)
		{
			return new CustomSerial(serial);
		}
	}
}