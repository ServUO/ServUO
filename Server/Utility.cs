#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
#endregion

namespace Server
{
	public static class Utility
	{
		private static Encoding m_UTF8, m_UTF8WithEncoding;

		public static Encoding UTF8
		{
			get
			{
				if (m_UTF8 == null)
				{
					m_UTF8 = new UTF8Encoding(false, false);
				}

				return m_UTF8;
			}
		}

		public static Encoding UTF8WithEncoding
		{
			get
			{
				if (m_UTF8WithEncoding == null)
				{
					m_UTF8WithEncoding = new UTF8Encoding(true, false);
				}

				return m_UTF8WithEncoding;
			}
		}

		public static void Separate(StringBuilder sb, string value, string separator)
		{
			if (sb.Length > 0)
			{
				sb.Append(separator);
			}

			sb.Append(value);
		}

		public static string Intern(string str)
		{
			if (str == null)
			{
				return null;
			}
			else if (str.Length == 0)
			{
				return String.Empty;
			}

			return String.Intern(str);
		}

		public static void Intern(ref string str)
		{
			str = Intern(str);
		}

		private static Dictionary<IPAddress, IPAddress> _ipAddressTable;

		public static IPAddress Intern(IPAddress ipAddress)
		{
			if (_ipAddressTable == null)
			{
				_ipAddressTable = new Dictionary<IPAddress, IPAddress>();
			}


			if (!_ipAddressTable.TryGetValue(ipAddress, out var interned))
			{
				interned = ipAddress;
				_ipAddressTable[ipAddress] = interned;
			}

			return interned;
		}

		public static void Intern(ref IPAddress ipAddress)
		{
			ipAddress = Intern(ipAddress);
		}

		public static bool IsValidIP(string text)
		{
			var valid = true;

			IPMatch(text, IPAddress.None, ref valid);

			return valid;
		}

		public static bool IPMatch(string val, IPAddress ip)
		{
			var valid = true;

			return IPMatch(val, ip, ref valid);
		}

		public static string FixHtml(string str)
		{
			if (str == null)
			{
				return "";
			}

			var hasOpen = str.IndexOf('<') >= 0;
			var hasClose = str.IndexOf('>') >= 0;
			var hasPound = str.IndexOf('#') >= 0;

			if (!hasOpen && !hasClose && !hasPound)
			{
				return str;
			}

			var sb = new StringBuilder(str);

			if (hasOpen)
			{
				sb.Replace('<', '(');
			}

			if (hasClose)
			{
				sb.Replace('>', ')');
			}

			if (hasPound)
			{
				sb.Replace('#', '-');
			}

			return sb.ToString();
		}

		public static bool IPMatchCIDR(string cidr, IPAddress ip)
		{
			if (ip == null || ip.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return false; //Just worry about IPv4 for now
			}

			var bytes = new byte[4];
			var split = cidr.Split('.');
			var cidrBits = false;
			var cidrLength = 0;

			for (var i = 0; i < 4; i++)
			{
				var part = 0;

				var partBase = 10;

				var pattern = split[i];

				for (var j = 0; j < pattern.Length; j++)
				{
					var c = pattern[j];

					if (c == 'x' || c == 'X')
					{
						partBase = 16;
					}
					else if (c >= '0' && c <= '9')
					{
						var offset = c - '0';

						if (cidrBits)
						{
							cidrLength *= partBase;
							cidrLength += offset;
						}
						else
						{
							part *= partBase;
							part += offset;
						}
					}
					else if (c >= 'a' && c <= 'f')
					{
						var offset = 10 + (c - 'a');

						if (cidrBits)
						{
							cidrLength *= partBase;
							cidrLength += offset;
						}
						else
						{
							part *= partBase;
							part += offset;
						}
					}
					else if (c >= 'A' && c <= 'F')
					{
						var offset = 10 + (c - 'A');

						if (cidrBits)
						{
							cidrLength *= partBase;
							cidrLength += offset;
						}
						else
						{
							part *= partBase;
							part += offset;
						}
					}
					else if (c == '/')
					{
						if (cidrBits || i != 3) //If there's two '/' or the '/' isn't in the last byte
						{
							return false;
						}

						partBase = 10;
						cidrBits = true;
					}
					else
					{
						return false;
					}
				}

				bytes[i] = (byte)part;
			}

			var cidrPrefix = OrderedAddressValue(bytes);

			return IPMatchCIDR(cidrPrefix, ip, cidrLength);
		}

		public static bool IPMatchCIDR(IPAddress cidrPrefix, IPAddress ip, int cidrLength)
		{
			if (cidrPrefix == null || ip == null || cidrPrefix.AddressFamily == AddressFamily.InterNetworkV6) //Ignore IPv6 for now
			{
				return false;
			}

			var cidrValue = SwapUnsignedInt((uint)GetLongAddressValue(cidrPrefix));
			var ipValue = SwapUnsignedInt((uint)GetLongAddressValue(ip));

			return IPMatchCIDR(cidrValue, ipValue, cidrLength);
		}

		public static bool IPMatchCIDR(uint cidrPrefixValue, IPAddress ip, int cidrLength)
		{
			if (ip == null || ip.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return false;
			}

			var ipValue = SwapUnsignedInt((uint)GetLongAddressValue(ip));

			return IPMatchCIDR(cidrPrefixValue, ipValue, cidrLength);
		}

		public static bool IPMatchCIDR(uint cidrPrefixValue, uint ipValue, int cidrLength)
		{
			if (cidrLength <= 0 || cidrLength >= 32) //if invalid cidr Length, just compare IPs
			{
				return cidrPrefixValue == ipValue;
			}

			var mask = UInt32.MaxValue << 32 - cidrLength;

			return (cidrPrefixValue & mask) == (ipValue & mask);
		}

		private static uint OrderedAddressValue(byte[] bytes)
		{
			if (bytes.Length != 4)
			{
				return 0;
			}

			return (uint)((bytes[0] << 0x18) | (bytes[1] << 0x10) | (bytes[2] << 8) | bytes[3]) & (0xffffffff);
		}

		private static uint SwapUnsignedInt(uint source)
		{
			return ((source & 0x000000FF) << 0x18) | ((source & 0x0000FF00) << 8) | ((source & 0x00FF0000) >> 8) | ((source & 0xFF000000) >> 0x18);
		}

		public static bool TryConvertIPv6toIPv4(ref IPAddress address)
		{
			if (!Socket.OSSupportsIPv6 || address.AddressFamily == AddressFamily.InterNetwork)
			{
				return true;
			}

			var addr = address.GetAddressBytes();
			if (addr.Length == 16) //sanity 0 - 15 //10 11 //12 13 14 15
			{
				if (addr[10] != 0xFF || addr[11] != 0xFF)
				{
					return false;
				}

				for (var i = 0; i < 10; i++)
				{
					if (addr[i] != 0)
					{
						return false;
					}
				}

				var v4Addr = new byte[4];

				for (var i = 0; i < 4; i++)
				{
					v4Addr[i] = addr[12 + i];
				}

				address = new IPAddress(v4Addr);
				return true;
			}

			return false;
		}

		public static bool IPMatch(string val, IPAddress ip, ref bool valid)
		{
			valid = true;

			var split = val.Split('.');

			for (var i = 0; i < 4; ++i)
			{
				int lowPart, highPart;

				if (i >= split.Length)
				{
					lowPart = 0;
					highPart = 255;
				}
				else
				{
					var pattern = split[i];

					if (pattern == "*")
					{
						lowPart = 0;
						highPart = 255;
					}
					else
					{
						lowPart = 0;
						highPart = 0;

						var highOnly = false;
						var lowBase = 10;
						var highBase = 10;

						for (var j = 0; j < pattern.Length; ++j)
						{
							var c = pattern[j];

							if (c == '?')
							{
								if (!highOnly)
								{
									lowPart *= lowBase;
									lowPart += 0;
								}

								highPart *= highBase;
								highPart += highBase - 1;
							}
							else if (c == '-')
							{
								highOnly = true;
								highPart = 0;
							}
							else if (c == 'x' || c == 'X')
							{
								lowBase = 16;
								highBase = 16;
							}
							else if (c >= '0' && c <= '9')
							{
								var offset = c - '0';

								if (!highOnly)
								{
									lowPart *= lowBase;
									lowPart += offset;
								}

								highPart *= highBase;
								highPart += offset;
							}
							else if (c >= 'a' && c <= 'f')
							{
								var offset = 10 + (c - 'a');

								if (!highOnly)
								{
									lowPart *= lowBase;
									lowPart += offset;
								}

								highPart *= highBase;
								highPart += offset;
							}
							else if (c >= 'A' && c <= 'F')
							{
								var offset = 10 + (c - 'A');

								if (!highOnly)
								{
									lowPart *= lowBase;
									lowPart += offset;
								}

								highPart *= highBase;
								highPart += offset;
							}
							else
							{
								valid = false; //high & lowpart would be 0 if it got to here.
							}
						}
					}
				}

				int b = (byte)(GetAddressValue(ip) >> (i * 8));

				if (b < lowPart || b > highPart)
				{
					return false;
				}
			}

			return true;
		}

		public static bool IPMatchClassC(IPAddress ip1, IPAddress ip2)
		{
			return (GetAddressValue(ip1) & 0xFFFFFF) == (GetAddressValue(ip2) & 0xFFFFFF);
		}

		public static int InsensitiveCompare(string first, string second)
		{
			return Insensitive.Compare(first, second);
		}

		public static bool InsensitiveStartsWith(string first, string second)
		{
			return Insensitive.StartsWith(first, second);
		}

		#region To[Something]
		public static bool ToBoolean(string value)
		{
			Boolean.TryParse(value, out var b);

			return b;
		}

		public static double ToDouble(string value)
		{
			Double.TryParse(value, out var d);

			return d;
		}

		public static TimeSpan ToTimeSpan(string value)
		{
			TimeSpan.TryParse(value, out var t);

			return t;
		}

		public static int ToInt32(string value)
		{
			int i;

			if (value.StartsWith("0x"))
			{
				Int32.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out i);
			}
			else
			{
				Int32.TryParse(value, out i);
			}

			return i;
		}

		public static long ToInt64(string value)
		{
			long i;

			if (value.StartsWith("0x"))
				Int64.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out i);
			else
				Int64.TryParse(value, out i);

			return i;
		}

		public static Serial ToSerial(string value)
		{
			return new Serial(ToInt32(value));
		}

		#endregion

		#region Get[Something]

		public static Serial GetXMLSerial(string serialString, Serial defaultValue)
		{
			try
			{
				return new Serial(XmlConvert.ToInt32(serialString));
			}
			catch
			{
				if (serialString.StartsWith("0x"))
				{
					if (Int32.TryParse(serialString.Substring(2), NumberStyles.HexNumber, null, out var val))
					{
						return new Serial(val);
					}
				}
				else
				{
					if (Int32.TryParse(serialString, out var val))
					{
						return new Serial(val);
					}
				}

				return defaultValue;
			}
		}

		public static double GetXMLDouble(string doubleString, double defaultValue)
		{
			try
			{
				return XmlConvert.ToDouble(doubleString);
			}
			catch
			{
				if (Double.TryParse(doubleString, out var val))
				{
					return val;
				}

				return defaultValue;
			}
		}

		public static int GetXMLInt32(string intString, int defaultValue)
		{
			try
			{
				return XmlConvert.ToInt32(intString);
			}
			catch
			{
				if (Int32.TryParse(intString, out var val))
				{
					return val;
				}

				return defaultValue;
			}
		}

		public static DateTime GetXMLDateTime(string dateTimeString, DateTime defaultValue)
		{
			try
			{
				return XmlConvert.ToDateTime(dateTimeString, XmlDateTimeSerializationMode.Utc);
			}
			catch
			{

				if (DateTime.TryParse(dateTimeString, out var d))
				{
					return d;
				}

				return defaultValue;
			}
		}

		public static DateTimeOffset GetXMLDateTimeOffset(string dateTimeOffsetString, DateTimeOffset defaultValue)
		{
			try
			{
				return XmlConvert.ToDateTimeOffset(dateTimeOffsetString);
			}
			catch
			{

				if (DateTimeOffset.TryParse(dateTimeOffsetString, out var d))
				{
					return d;
				}

				return defaultValue;
			}
		}

		public static TimeSpan GetXMLTimeSpan(string timeSpanString, TimeSpan defaultValue)
		{
			try
			{
				return XmlConvert.ToTimeSpan(timeSpanString);
			}
			catch
			{
				return defaultValue;
			}
		}

		public static string GetAttribute(XmlElement node, string attributeName)
		{
			return GetAttribute(node, attributeName, null);
		}

		public static string GetAttribute(XmlElement node, string attributeName, string defaultValue)
		{
			if (node == null)
			{
				return defaultValue;
			}

			var attr = node.Attributes[attributeName];

			if (attr == null)
			{
				return defaultValue;
			}

			return attr.Value;
		}

		public static string GetText(XmlElement node, string defaultValue)
		{
			if (node == null)
			{
				return defaultValue;
			}

			return node.InnerText;
		}

		public static int GetAddressValue(IPAddress address)
		{
#pragma warning disable 618
			return (int)address.Address;
#pragma warning restore 618
		}

		public static long GetLongAddressValue(IPAddress address)
		{
#pragma warning disable 618
			return address.Address;
#pragma warning restore 618
		}
		#endregion

		#region In[...]Range
		public static bool InRange(IPoint2D p1, IPoint2D p2, int range)
		{
			if (p1 is Item i1)
				p1 = i1.GetWorldLocation();

			if (p2 is Item i2)
				p2 = i2.GetWorldLocation();

			return (p1.X >= (p2.X - range)) && (p1.X <= (p2.X + range))
				&& (p1.Y >= (p2.Y - range)) && (p1.Y <= (p2.Y + range));
		}
		#endregion

		public static Direction GetDirection(IPoint2D from, IPoint2D to)
		{
			if (from is Item i1)
				from = i1.GetWorldLocation();

			if (to is Item i2)
				to = i2.GetWorldLocation();

			var dx = to.X - from.X;
			var dy = to.Y - from.Y;

			var adx = Math.Abs(dx);
			var ady = Math.Abs(dy);

			if (adx >= ady * 3)
			{
				if (dx > 0)
				{
					return Direction.East;
				}
				else
				{
					return Direction.West;
				}
			}
			else if (ady >= adx * 3)
			{
				if (dy > 0)
				{
					return Direction.South;
				}
				else
				{
					return Direction.North;
				}
			}
			else if (dx > 0)
			{
				if (dy > 0)
				{
					return Direction.Down;
				}
				else
				{
					return Direction.Right;
				}
			}
			else
			{
				if (dy > 0)
				{
					return Direction.Left;
				}
				else
				{
					return Direction.Up;
				}
			}
		}

		public static object GetArrayCap(Array array, int index)
		{
			return GetArrayCap(array, index, null);
		}

		public static object GetArrayCap(Array array, int index, object emptyValue)
		{
			if (array.Length > 0)
			{
				if (index < 0)
				{
					index = 0;
				}
				else if (index >= array.Length)
				{
					index = array.Length - 1;
				}

				return array.GetValue(index);
			}
			else
			{
				return emptyValue;
			}
		}

		#region Random
		/// <summary>
		/// Enables or disables floating dice. 
		/// Floating dice uses a double to obtain a lower average value range.
		/// Consistent average values for [1,000,000 x 1d6+0] rolls: [Integral: 3.50]  [Floating: 2.25]
		/// </summary>
		public static bool FloatingDice = false;

		//4d6+8 would be: Utility.Dice( 4, 6, 8 )
		public static int Dice(int numDice, int numSides, int bonus)
		{
			return Dice(numDice, numSides, bonus, FloatingDice);
		}

		public static int Dice(int numDice, int numSides, int bonus, bool floating)
		{
			if (floating)
			{
				double min = numDice, max = min;

				for (var i = 0; i < numDice; ++i)
				{
					max += Random(numSides);
				}

				return (int)Math.Round(RandomMinMax(min, max)) + bonus;
			}

			var total = 0;

			for (var i = 0; i < numDice; ++i)
			{
				total += Random(numSides) + 1;
			}

			return total + bonus;
		}

		public static T RandomList<T>(List<T> list)
		{
			return list[RandomImpl.Next(list.Count)];
		}

		public static T RandomList<T>(params T[] list)
		{
			return list[RandomImpl.Next(list.Length)];
		}

		public static bool RandomBool()
		{
			return RandomImpl.NextBool();
		}

#if MONO
		private static class EnumCache<T> where T : struct, IConvertible
#else
		private static class EnumCache<T> where T : struct, Enum
#endif
		{
			public static T[] Values = (T[])Enum.GetValues(typeof(T));
		}

#if MONO
		public static TEnum RandomEnum<TEnum>() where TEnum : struct, IConvertible            
#else
		public static TEnum RandomEnum<TEnum>() where TEnum : struct, Enum
#endif
		{
			return RandomList(EnumCache<TEnum>.Values);
		}

#if MONO
		public static TEnum RandomMinMax<TEnum>(TEnum min, TEnum max) where TEnum : struct, IConvertible            
#else
		public static TEnum RandomMinMax<TEnum>(TEnum min, TEnum max) where TEnum : struct, Enum
#endif
		{
			var values = EnumCache<TEnum>.Values;

			if (values.Length == 0)
			{
				return default(TEnum);
			}

			int curIdx = -1, minIdx = -1, maxIdx = -1;

			while (++curIdx < values.Length)
			{
				if (Equals(values[curIdx], min))
				{
					minIdx = curIdx;
				}
				else if (Equals(values[curIdx], max))
				{
					maxIdx = curIdx;
				}
			}

			if (minIdx == 0 && maxIdx == values.Length - 1)
			{
				return RandomList(values);
			}

			curIdx = -1;

			if (minIdx >= 0)
			{
				if (minIdx == maxIdx)
				{
					curIdx = minIdx;
				}
				else if (maxIdx > minIdx)
				{
					curIdx = RandomMinMax(minIdx, maxIdx);
				}
			}

			if (curIdx >= 0 && curIdx < values.Length)
			{
				return values[curIdx];
			}

			return RandomList(min, max);
		}

		public static double RandomMinMax(double min, double max)
		{
			if (min > max)
			{
				var copy = min;
				min = max;
				max = copy;
			}
			else if (min == max)
			{
				return min;
			}

			return min + (RandomImpl.NextDouble() * (max - min));
		}

		public static int RandomMinMax(int min, int max)
		{
			if (min > max)
			{
				var copy = min;
				min = max;
				max = copy;
			}
			else if (min == max)
			{
				return min;
			}

			return min + RandomImpl.Next(max - min + 1);
		}

		public static int Random(int from, int count)
		{
			if (count == 0)
			{
				return from;
			}
			else if (count > 0)
			{
				return from + RandomImpl.Next(count);
			}
			else
			{
				return from - RandomImpl.Next(-count);
			}
		}

		public static int Random(int count)
		{
			return RandomImpl.Next(count);
		}

		public static void RandomBytes(byte[] buffer)
		{
			RandomImpl.NextBytes(buffer);
		}

		public static double RandomDouble()
		{
			return RandomImpl.NextDouble();
		}
		#endregion

		#region FixValues
		public static void FixMin(ref int value, int min)
		{
			if (value < min)
				value = min;
		}

		public static void FixMin(ref double value, double min)
		{
			if (value < min)
				value = min;
		}

		public static void FixMax(ref int value, int max)
		{
			if (value > max)
				value = max;
		}

		public static void FixMax(ref double value, double max)
		{
			if (value > max)
				value = max;
		}

		public static void FixMinMax(ref int value, int min, int max)
		{
			FixMin(ref value, min);
			FixMax(ref value, max);
		}

		public static void FixMinMax(ref double value, double min, double max)
		{
			FixMin(ref value, min);
			FixMax(ref value, max);
		}

		public static void FixRange(ref double min, ref double max)
		{
			if (min < max)
			{
				var swap = max;
				max = min;
				min = swap;
			}
		}

		public static void FixRange(ref int min, ref int max)
		{
			if (min > max)
			{
				var swap = max;
				max = min;
				min = swap;
			}
		}
		#endregion

		#region Clamp
		public static void Clamp(ref sbyte val, sbyte min, sbyte max)
		{
			val = Clamp(val, min, max);
		}

		public static sbyte Clamp(sbyte val, sbyte min, sbyte max)
		{
			return Math.Max(min, Math.Min(max, val));
		}

		public static void Clamp(ref byte val, byte min, byte max)
		{
			val = Clamp(val, min, max);
		}

		public static byte Clamp(byte val, byte min, byte max)
		{
			return Math.Max(min, Math.Min(max, val));
		}

		public static void Clamp(ref short val, short min, short max)
		{
			val = Clamp(val, min, max);
		}

		public static short Clamp(short val, short min, short max)
		{
			return Math.Max(min, Math.Min(max, val));
		}

		public static void Clamp(ref ushort val, ushort min, ushort max)
		{
			val = Clamp(val, min, max);
		}

		public static ushort Clamp(ushort val, ushort min, ushort max)
		{
			return Math.Max(min, Math.Min(max, val));
		}

		public static void Clamp(ref int val, int min, int max)
		{
			val = Clamp(val, min, max);
		}

		public static int Clamp(int val, int min, int max)
		{
			return Math.Max(min, Math.Min(max, val));
		}

		public static void Clamp(ref uint val, uint min, uint max)
		{
			val = Clamp(val, min, max);
		}

		public static uint Clamp(uint val, uint min, uint max)
		{
			return Math.Max(min, Math.Min(max, val));
		}

		public static void Clamp(ref long val, long min, long max)
		{
			val = Clamp(val, min, max);
		}

		public static long Clamp(long val, long min, long max)
		{
			return Math.Max(min, Math.Min(max, val));
		}

		public static void Clamp(ref ulong val, ulong min, ulong max)
		{
			val = Clamp(val, min, max);
		}

		public static ulong Clamp(ulong val, ulong min, ulong max)
		{
			return Math.Max(min, Math.Min(max, val));
		}

		public static void Clamp(ref float val, float min, float max)
		{
			val = Clamp(val, min, max);
		}

		public static float Clamp(float val, float min, float max)
		{
			return Math.Max(min, Math.Min(max, val));
		}

		public static void Clamp(ref decimal val, decimal min, decimal max)
		{
			val = Clamp(val, min, max);
		}

		public static decimal Clamp(decimal val, decimal min, decimal max)
		{
			return Math.Max(min, Math.Min(max, val));
		}

		public static void Clamp(ref double val, double min, double max)
		{
			val = Clamp(val, min, max);
		}

		public static double Clamp(double val, double min, double max)
		{
			return Math.Max(min, Math.Min(max, val));
		}
		#endregion

		#region Random Hues
		/// <summary>
		///     Random pink, blue, green, orange, red or yellow hue
		/// </summary>
		public static int RandomNondyedHue()
		{
			switch (Random(6))
			{
				case 0:
				return RandomPinkHue();
				case 1:
				return RandomBlueHue();
				case 2:
				return RandomGreenHue();
				case 3:
				return RandomOrangeHue();
				case 4:
				return RandomRedHue();
				case 5:
				return RandomYellowHue();
			}

			return 0;
		}

		/// <summary>
		///     Random hue in the range 1201-1254
		/// </summary>
		public static int RandomPinkHue()
		{
			return Random(1201, 54);
		}

		/// <summary>
		///     Random hue in the range 1301-1354
		/// </summary>
		public static int RandomBlueHue()
		{
			return Random(1301, 54);
		}

		/// <summary>
		///     Random hue in the range 1401-1454
		/// </summary>
		public static int RandomGreenHue()
		{
			return Random(1401, 54);
		}

		/// <summary>
		///     Random hue in the range 1501-1554
		/// </summary>
		public static int RandomOrangeHue()
		{
			return Random(1501, 54);
		}

		/// <summary>
		///     Random hue in the range 1601-1654
		/// </summary>
		public static int RandomRedHue()
		{
			return Random(1601, 54);
		}

		/// <summary>
		///     Random hue in the range 1701-1754
		/// </summary>
		public static int RandomYellowHue()
		{
			return Random(1701, 54);
		}

		/// <summary>
		///     Random hue in the range 1801-1908
		/// </summary>
		public static int RandomNeutralHue()
		{
			return Random(1801, 108);
		}

		/// <summary>
		///     Random hue in the range 2001-2018
		/// </summary>
		public static int RandomSnakeHue()
		{
			return Random(2001, 18);
		}

		/// <summary>
		///     Random hue in the range 2101-2130
		/// </summary>
		public static int RandomBirdHue()
		{
			return Random(2101, 30);
		}

		/// <summary>
		///     Random hue in the range 2201-2224
		/// </summary>
		public static int RandomSlimeHue()
		{
			return Random(2201, 24);
		}

		/// <summary>
		///     Random hue in the range 2301-2318
		/// </summary>
		public static int RandomAnimalHue()
		{
			return Random(2301, 18);
		}

		/// <summary>
		///     Random hue in the range 2401-2430
		/// </summary>
		public static int RandomMetalHue()
		{
			return Random(2401, 30);
		}

		public static int ClipDyedHue(int hue)
		{
			if (hue < 2)
			{
				return 2;
			}
			else if (hue > 1001)
			{
				return 1001;
			}
			else
			{
				return hue;
			}
		}

		/// <summary>
		///     Random hue in the range 2-1001
		/// </summary>
		public static int RandomDyedHue()
		{
			return Random(2, 1000);
		}

		/// <summary>
		///     Random hue from 0x62, 0x71, 0x03, 0x0D, 0x13, 0x1C, 0x21, 0x30, 0x37, 0x3A, 0x44, 0x59
		/// </summary>
		public static int RandomBrightHue()
		{
			if (RandomDouble() < 0.1)
			{
				return RandomList(0x62, 0x71);
			}

			return RandomList(0x03, 0x0D, 0x13, 0x1C, 0x21, 0x30, 0x37, 0x3A, 0x44, 0x59);
		}
		#endregion

		private static readonly SkillName[] m_AllSkills =
		{
			SkillName.Alchemy, SkillName.Anatomy, SkillName.AnimalLore, SkillName.ItemID, SkillName.ArmsLore, SkillName.Parry,
			SkillName.Begging, SkillName.Blacksmith, SkillName.Fletching, SkillName.Peacemaking, SkillName.Camping,
			SkillName.Carpentry, SkillName.Cartography, SkillName.Cooking, SkillName.DetectHidden, SkillName.Discordance,
			SkillName.EvalInt, SkillName.Healing, SkillName.Fishing, SkillName.Forensics, SkillName.Herding, SkillName.Hiding,
			SkillName.Provocation, SkillName.Inscribe, SkillName.Lockpicking, SkillName.Magery, SkillName.MagicResist,
			SkillName.Tactics, SkillName.Snooping, SkillName.Musicianship, SkillName.Poisoning, SkillName.Archery,
			SkillName.SpiritSpeak, SkillName.Stealing, SkillName.Tailoring, SkillName.AnimalTaming, SkillName.TasteID,
			SkillName.Tinkering, SkillName.Tracking, SkillName.Veterinary, SkillName.Swords, SkillName.Macing, SkillName.Fencing,
			SkillName.Wrestling, SkillName.Lumberjacking, SkillName.Mining, SkillName.Meditation, SkillName.Stealth,
			SkillName.RemoveTrap, SkillName.Necromancy, SkillName.Focus, SkillName.Chivalry, SkillName.Bushido,
			SkillName.Ninjitsu, SkillName.Spellweaving, SkillName.Mysticism, SkillName.Imbuing, SkillName.Throwing
		};

		private static readonly SkillName[] m_CombatSkills =
		{
			SkillName.Archery, SkillName.Swords, SkillName.Macing, SkillName.Fencing, SkillName.Wrestling
		};

		private static readonly SkillName[] m_CraftSkills =
		{
			SkillName.Alchemy, SkillName.Blacksmith, SkillName.Fletching, SkillName.Carpentry, SkillName.Cartography,
			SkillName.Cooking, SkillName.Inscribe, SkillName.Tailoring, SkillName.Tinkering
		};

		public static SkillName RandomSkill()
		{
			return m_AllSkills[Random(m_AllSkills.Length - (Core.ML ? 0 : Core.SE ? 1 : Core.AOS ? 3 : 6))];
		}

		public static SkillName RandomCombatSkill()
		{
			return m_CombatSkills[Random(m_CombatSkills.Length)];
		}

		public static SkillName RandomCraftSkill()
		{
			return m_CraftSkills[Random(m_CraftSkills.Length)];
		}

		public static void FixPoint(ref int top, ref int bottom)
		{
			if (bottom < top)
			{
				var swap = top;
				top = bottom;
				bottom = swap;
			}
		}

		public static void FixPoints(ref int topX, ref int topY, ref int bottomX, ref int bottomY)
		{
			FixPoint(ref topX, ref bottomX);
			FixPoint(ref topY, ref bottomY);
		}

		public static void FixPoints(ref int topX, ref int topY, ref int topZ, ref int bottomX, ref int bottomY, ref int bottomZ)
		{
			FixPoint(ref topX, ref bottomX);
			FixPoint(ref topY, ref bottomY);
			FixPoint(ref topZ, ref bottomZ);
		}

		public static void FixPoints(ref Point2D top, ref Point2D bottom)
		{
			FixPoints(ref top.m_X, ref top.m_Y, ref bottom.m_X, ref bottom.m_Y);
		}

		public static void FixPoints(ref Point3D top, ref Point3D bottom)
		{
			FixPoints(ref top.m_X, ref top.m_Y, ref top.m_Z, ref bottom.m_X, ref bottom.m_Y, ref bottom.m_Z);
		}

		public static ArrayList BuildArrayList(IEnumerable enumerable)
		{
			var e = enumerable.GetEnumerator();

			var list = new ArrayList();

			while (e.MoveNext())
			{
				list.Add(e.Current);
			}

			return list;
		}

		public static bool RangeCheck(IPoint2D p1, IPoint2D p2, int range)
		{
			return (p1.X >= (p2.X - range)) && (p1.X <= (p2.X + range)) && (p1.Y >= (p2.Y - range)) && (p2.Y <= (p2.Y + range));
		}

		public static void FormatBuffer(TextWriter output, Stream input, int length)
		{
			output.WriteLine("        0  1  2  3  4  5  6  7   8  9  A  B  C  D  E  F");
			output.WriteLine("       -- -- -- -- -- -- -- --  -- -- -- -- -- -- -- --");

			var byteIndex = 0;

			var whole = length >> 4;
			var rem = length & 0xF;

			for (var i = 0; i < whole; ++i, byteIndex += 16)
			{
				var bytes = new StringBuilder(49);
				var chars = new StringBuilder(16);

				for (var j = 0; j < 16; ++j)
				{
					var c = input.ReadByte();

					bytes.Append(c.ToString("X2"));

					if (j != 7)
					{
						bytes.Append(' ');
					}
					else
					{
						bytes.Append("  ");
					}

					if (c >= 0x20 && c < 0x80)
					{
						chars.Append((char)c);
					}
					else
					{
						chars.Append('.');
					}
				}

				output.Write(byteIndex.ToString("X4"));
				output.Write("   ");
				output.Write(bytes.ToString());
				output.Write("  ");
				output.WriteLine(chars.ToString());
			}

			if (rem != 0)
			{
				var bytes = new StringBuilder(49);
				var chars = new StringBuilder(rem);

				for (var j = 0; j < 16; ++j)
				{
					if (j < rem)
					{
						var c = input.ReadByte();

						bytes.Append(c.ToString("X2"));

						if (j != 7)
						{
							bytes.Append(' ');
						}
						else
						{
							bytes.Append("  ");
						}

						if (c >= 0x20 && c < 0x80)
						{
							chars.Append((char)c);
						}
						else
						{
							chars.Append('.');
						}
					}
					else
					{
						bytes.Append("   ");
					}
				}

				output.Write(byteIndex.ToString("X4"));
				output.Write("   ");
				output.Write(bytes.ToString());
				output.Write("  ");
				output.WriteLine(chars.ToString());
			}
		}

		public static string FormatDelegate(Delegate callback)
		{
			if (callback == null)
			{
				return "null";
			}

			if (callback.Method.DeclaringType == null)
			{
				return callback.Method.Name;
			}

			return String.Format("{0}.{1}", callback.Method.DeclaringType.FullName, callback.Method.Name);
		}

		#region Console

		private static readonly Stack<ConsoleColor> m_ConsoleColors = new Stack<ConsoleColor>();

		public static void Write(ConsoleColor color, string text, params object[] args)
		{
			lock (m_ConsoleColors)
			{
				var oldColor = Console.ForegroundColor;

				try { Console.ForegroundColor = color; }
				catch { return; }

				Console.Write(text, args);

				try { Console.ForegroundColor = oldColor; }
				catch { }
			}
		}

		public static void WriteLine(ConsoleColor color, string text, params object[] args)
		{
			lock (m_ConsoleColors)
			{
				var oldColor = Console.ForegroundColor;

				try { Console.ForegroundColor = color; }
				catch { return; }

				Console.WriteLine(text, args);

				try { Console.ForegroundColor = oldColor; }
				catch { }
			}
		}

		public static void PushColor(ConsoleColor color)
		{
			lock (m_ConsoleColors)
			{
				var oldColor = Console.ForegroundColor;

				try { Console.ForegroundColor = color; }
				catch { return; }

				m_ConsoleColors.Push(oldColor);
			}
		}

		public static void PopColor()
		{
			lock (m_ConsoleColors)
			{
				if (m_ConsoleColors.Count > 0)
				{
					var color = m_ConsoleColors.Pop();

					try { Console.ForegroundColor = color; }
					catch { }
				}
			}
		}

		#endregion

		public static Color ToColor(string input)
		{
			var color = Color.Empty;

			if (!String.IsNullOrEmpty(input))
				input = input.Trim();

			if (!String.IsNullOrEmpty(input))
			{
				if (input[0] == '#')
				{
					input = input.TrimStart('#').Trim();

					if (input.Length >= 8)
					{
						var ap = Byte.TryParse(input.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var a);
						var rp = Byte.TryParse(input.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r);
						var gp = Byte.TryParse(input.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g);
						var bp = Byte.TryParse(input.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b);

						if (ap && rp && gp && bp)
							color = Color.FromArgb(a, r, g, b);
					}
					else if (input.Length >= 6)
					{
						var rp = Byte.TryParse(input.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r);
						var gp = Byte.TryParse(input.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g);
						var bp = Byte.TryParse(input.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b);

						if (rp && gp && bp)
							color = Color.FromArgb(0xFF, r, g, b);
					}
					else if (input.Length >= 3)
					{
						var rp = Byte.TryParse(input.Substring(0, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r);
						var gp = Byte.TryParse(input.Substring(1, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g);
						var bp = Byte.TryParse(input.Substring(2, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b);

						if (rp && gp && bp)
							color = Color.FromArgb(0xFF, r, g, b);
					}
				}
				else if (input.IndexOf(',') >= 0)
				{
					var rgba = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

					if (rgba.Length >= 4)
					{
						var ap = Byte.TryParse(rgba[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var a);
						var rp = Byte.TryParse(rgba[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var r);
						var gp = Byte.TryParse(rgba[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var g);
						var bp = Byte.TryParse(rgba[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out var b);

						if (ap && rp && gp && bp)
							color = Color.FromArgb(a, r, g, b);
					}
					else if (rgba.Length >= 3)
					{
						var rp = Byte.TryParse(rgba[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var r);
						var gp = Byte.TryParse(rgba[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var g);
						var bp = Byte.TryParse(rgba[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var b);

						if (rp && gp && bp)
							color = Color.FromArgb(0xFF, r, g, b);
					}
				}

				if (color.IsEmpty)
				{
					var argb = ToInt32(input);

					if (argb != 0)
						color = Color.FromArgb(argb);
					else if (Enum.TryParse(input, true, out KnownColor kc))
						color = Color.FromKnownColor(kc);
				}
			}

			return color;
		}

		public static bool TryParseColor(string input, out Color color)
		{
			color = ToColor(input);

			return !color.IsEmpty;
		}

		public static bool NumberBetween(double num, int bound1, int bound2, double allowance)
		{
			if (bound1 > bound2)
			{
				var i = bound1;
				bound1 = bound2;
				bound2 = i;
			}

			return num < bound2 + allowance && num > bound1 - allowance;
		}

		public static double GetDistanceToSqrt(IPoint2D p1, IPoint2D p2)
		{
			if (p1 is Item i1)
				p1 = i1.GetWorldLocation();

			if (p2 is Item i2)
				p2 = i2.GetWorldLocation();

			var xDelta = p1.X - p2.X;
			var yDelta = p1.Y - p2.Y;

			return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
		}

		public static void AssignRandomHair(Mobile m)
		{
			AssignRandomHair(m, true);
		}

		public static void AssignRandomHair(Mobile m, int hue)
		{
			m.HairItemID = m.Race.RandomHair(m);
			m.HairHue = hue;
		}

		public static void AssignRandomHair(Mobile m, bool randomHue)
		{
			m.HairItemID = m.Race.RandomHair(m);

			if (randomHue)
			{
				m.HairHue = m.Race.RandomHairHue();
			}
		}

		public static void AssignRandomFacialHair(Mobile m)
		{
			AssignRandomFacialHair(m, true);
		}

		public static void AssignRandomFacialHair(Mobile m, int hue)
		{
			m.FacialHairItemID = m.Race.RandomFacialHair(m);
			m.FacialHairHue = hue;
		}

		public static void AssignRandomFacialHair(Mobile m, bool randomHue)
		{
			m.FacialHairItemID = m.Race.RandomFacialHair(m);

			if (randomHue)
			{
				m.FacialHairHue = m.Race.RandomHairHue();
			}
		}

#if MONO
		public static List<TOutput> CastConvertList<TInput, TOutput>(List<TInput> list ) where TInput : class where TOutput : class
#else
		public static List<TOutput> CastConvertList<TInput, TOutput>(List<TInput> list) where TOutput : TInput
#endif
		{
			return list.ConvertAll<TOutput>(value => (TOutput)value);
		}

		public static List<TOutput> SafeConvertList<TInput, TOutput>(List<TInput> list) where TOutput : class
		{
			var output = new List<TOutput>(list.Capacity);

			for (var i = 0; i < list.Count; i++)
			{
				if (list[i] is TOutput t)
				{
					output.Add(t);
				}
			}

			return output;
		}

		public static string RemoveHtml(string str)
		{
			return str.Replace("<", "").Replace(">", "").Trim();
		}

		public static bool IsNumeric(string str)
		{
			return !Regex.IsMatch(str, "[^0-9]");
		}

		public static bool IsAlpha(string str)
		{
			return !Regex.IsMatch(str, "[^a-z]", RegexOptions.IgnoreCase);
		}

		public static bool IsAlphaNumeric(string str)
		{
			return !Regex.IsMatch(str, "[^a-z0-9]", RegexOptions.IgnoreCase);
		}
	}

	public static class ColUtility
	{
		public static void Free<T>(HashSet<T> l)
		{
			if (l == null)
				return;

			l.Clear();
			l.TrimExcess();
		}

		public static void Free<T>(Queue<T> l)
		{
			if (l == null)
				return;

			l.Clear();
			l.TrimExcess();
		}

		public static void Free<T>(List<T> l)
		{
			if (l == null)
				return;

			l.Clear();
			l.TrimExcess();
		}

		public static void ForEach<T>(IEnumerable<T> list, Action<T> action)
		{
			if (list == null || action == null)
				return;

			var l = list.ToList();

			foreach (var o in l)
				action(o);

			Free(l);
		}

		public static void ForEach<TKey, TValue>(IDictionary<TKey, TValue> dictionary, Action<KeyValuePair<TKey, TValue>> action)
		{
			if (dictionary == null || dictionary.Count == 0 || action == null)
				return;

			var l = dictionary.ToList();

			foreach (var kvp in l)
				action(kvp);

			Free(l);
		}

		public static void ForEach<TKey, TValue>(IDictionary<TKey, TValue> dictionary, Action<TKey, TValue> action)
		{
			if (dictionary == null || dictionary.Count == 0 || action == null)
				return;

			var l = dictionary.ToList();

			foreach (var kvp in l)
				action(kvp.Key, kvp.Value);

			Free(l);
		}

		public static void For<T>(IEnumerable<T> list, Action<int, T> action)
		{
			if (list == null || action == null)
				return;

			var l = list.ToList();

			for (var i = 0; i < l.Count; i++)
				action(i, l[i]);

			Free(l);
		}

		public static void For<TKey, TValue>(IDictionary<TKey, TValue> list, Action<int, TKey, TValue> action)
		{
			if (list == null || action == null)
				return;

			var l = list.ToList();

			for (var i = 0; i < l.Count; i++)
				action(i, l[i].Key, l[i].Value);

			Free(l);
		}

		public static void IterateReverse<T>(T[] list, Action<T> action)
		{
			if (list == null || action == null)
			{
				return;
			}

			var i = list.Length;

			while (--i >= 0)
			{
				if (i < list.Length)
				{
					action(list[i]);
				}
			}
		}

		public static void IterateReverse<T>(List<T> list, Action<T> action)
		{
			if (list == null || action == null)
			{
				return;
			}

			var i = list.Count;

			while (--i >= 0)
			{
				if (i < list.Count)
				{
					action(list[i]);
				}
			}
		}

		public static void IterateReverse<T>(IEnumerable<T> list, Action<T> action)
		{
			if (list == null || action == null)
			{
				return;
			}

			if (list is T[] a)
			{
				IterateReverse(a, action);
				return;
			}

			if (list is List<T> l)
			{
				IterateReverse(l, action);
				return;
			}

			var toList = list.ToList();

			var i = toList.Count;

			while (--i >= 0)
			{
				if (i < toList.Count)
				{
					action(toList[i]);
				}
			}

			Free(toList);
		}

		public static void SafeDelete<T>(List<T> list)
		{
			SafeDelete(list, null);
		}

		/// <summary>
		/// Safely deletes any entities based on predicate from a list that by deleting such entity would cause the collection to be modified.
		/// ie item.Items or mobile.Items. Omitting the predicate will delete all items in the collection.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="predicate"></param>
		public static void SafeDelete<T>(List<T> list, Func<T, bool> predicate)
		{
			if (list == null)
			{
				return;
			}

			var i = list.Count;

			while (--i >= 0)
			{
				if (i < list.Count)
				{
					var entity = list[i] as IEntity;

					if (entity != null && !entity.Deleted && (predicate == null || predicate((T)entity)))
					{
						entity.Delete();
					}
				}
			}
		}

		public static void Shuffle<T>(List<T> list)
		{
			if (list == null || list.Count < 2)
			{
				return;
			}

			for (var i = 0; i < list.Count * 2; i++)
			{
				var select = list[0];
				list.RemoveAt(0);

				list.Insert(Utility.RandomMinMax(0, list.Count - 1), select);
			}
		}
	}
}
