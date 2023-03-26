#region References
using System;
#endregion

namespace Server
{
	public struct Serial : IComparable<Serial>, IEquatable<Serial>
	{
		private static Serial m_NextMobileQueued = new Serial(0x3FFFFFFF);
		private static Serial m_NextItemQueued = new Serial(0x7FFFFFFF);

		private static Serial m_NextMobile = new Serial(0x00000001);
		private static Serial m_NextItem = new Serial(0x40000001);

		public static readonly Serial MinValue = new Serial(Int32.MinValue);
		public static readonly Serial MaxValue = new Serial(Int32.MaxValue);

		public static readonly Serial MinusOne = new Serial(-1);
		public static readonly Serial Zero = new Serial(0);

		public static Serial NewMobile
		{
			get
			{
				Serial s;

				if (World.Volatile)
				{
					while (World.FindMobile(m_NextMobileQueued) != null)
						--m_NextMobileQueued._Value;

					s = m_NextMobileQueued;
				}
				else
				{
					while (World.FindMobile(m_NextMobile) != null)
						++m_NextMobile._Value;

					s = m_NextMobile;
				}

				return s;
			}
		}

		public static Serial NewItem
		{
			get
			{
				Serial s;

				if (World.Volatile)
				{
					while (World.FindItem(m_NextItemQueued) != null)
						--m_NextItemQueued._Value;

					s = m_NextItemQueued;
				}
				else
				{
					while (World.FindItem(m_NextItem) != null)
						++m_NextItem._Value;

					s = m_NextItem;
				}

				return s;
			}
		}

		private int _Value;

		public int Value => _Value;

		public bool IsMobile => _Value > 0 && _Value < 0x40000000;

		public bool IsItem => _Value >= 0x40000000 && _Value <= 0x7FFFFFFF;

		public bool IsValid => _Value > 0;

		public Serial(int serial)
		{
			_Value = serial;
		}

		public override int GetHashCode()
		{
			return _Value;
		}

		public int CompareTo(Serial other)
		{
			return _Value.CompareTo(other._Value);
		}

		public int CompareTo(object o)
		{
			return o is Serial s ? CompareTo(s) : -1;
		}

		public bool Equals(Serial other)
		{
			return _Value.Equals(other._Value);
		}

		public override bool Equals(object o)
		{
			return o is Serial s && Equals(s);
		}

		public static bool operator ==(Serial l, Serial r)
		{
			return l._Value == r._Value;
		}

		public static bool operator !=(Serial l, Serial r)
		{
			return l._Value != r._Value;
		}

		public static bool operator >(Serial l, Serial r)
		{
			return l._Value > r._Value;
		}

		public static bool operator <(Serial l, Serial r)
		{
			return l._Value < r._Value;
		}

		public static bool operator >=(Serial l, Serial r)
		{
			return l._Value >= r._Value;
		}

		public static bool operator <=(Serial l, Serial r)
		{
			return l._Value <= r._Value;
		}

		public override string ToString()
		{
			return String.Format("0x{0:X8}", _Value);
		}

		public static implicit operator int(Serial a)
		{
			return a._Value;
		}
		/*
		public static implicit operator Serial(int a)
		{
			return new Serial(a);
		}
		*/
	}
}