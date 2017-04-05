#region Header
// **********
// ServUO - Random.cs
// **********
#endregion

#region References
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
#endregion

namespace Server
{
	/// <summary>
	///     Handles random number generation.
	/// </summary>
	public static class RandomImpl
	{
		private static readonly IRandomImpl _Random;

		static RandomImpl()
		{
			if (Core.Unix && Core.Is64Bit && File.Exists("libdrng.so"))
			{
				_Random = new RDRand64();
			}
			else if (Core.Unix && File.Exists("libdrng.so"))
			{
				_Random = new RDRand32();
			}
			else if (Core.Unix)
			{
				_Random = new SimpleRandom();
			}
			else if (Core.Is64Bit && File.Exists("drng64.dll"))
			{
				_Random = new RDRand64();
			}
			else if (!Core.Is64Bit && File.Exists("drng32.dll"))
			{
				_Random = new RDRand32();
			}
			else
			{
				_Random = new CSPRandom();
			}

			if (_Random is IHardwareRNG)
			{
				if (!((IHardwareRNG)_Random).IsSupported())
				{
					_Random = new CSPRandom();
				}
			}
		}

		public static bool IsHardwareRNG { get { return _Random is IHardwareRNG; } }

		public static Type Type { get { return _Random.GetType(); } }

		public static int Next(int c)
		{
			return _Random.Next(c);
		}

		public static bool NextBool()
		{
			return _Random.NextBool();
		}

		public static void NextBytes(byte[] b)
		{
			_Random.NextBytes(b);
		}

		public static double NextDouble()
		{
			return _Random.NextDouble();
		}
	}

	public interface IRandomImpl
	{
		int Next(int c);
		bool NextBool();
		void NextBytes(byte[] b);
		double NextDouble();
	}

	public interface IHardwareRNG
	{
		bool IsSupported();
	}

	public sealed class SimpleRandom : IRandomImpl
	{
		private readonly Random m_Random = new Random();

		public int Next(int c)
		{
            if(c <= 0)
                return 0;
            
			int r;
			lock (m_Random)
				r = m_Random.Next(c);
			return r;
		}

		public bool NextBool()
		{
			return NextDouble() >= .5;
		}

		public void NextBytes(byte[] b)
		{
			lock (m_Random)
				m_Random.NextBytes(b);
		}

		public double NextDouble()
		{
			double r;
			lock (m_Random)
				r = m_Random.NextDouble();
			return r;
		}
	}

	public sealed class CSPRandom : IRandomImpl
	{
		private readonly RNGCryptoServiceProvider _CSP = new RNGCryptoServiceProvider();

		private static int BUFFER_SIZE = 0x4000;
		private static int LARGE_REQUEST = 0x40;

		private byte[] _Working = new byte[BUFFER_SIZE];
		private byte[] _Buffer = new byte[BUFFER_SIZE];

		private int _Index;

		private readonly object _sync = new object();
		private readonly object _syncB = new object();

		public CSPRandom()
		{
			_CSP.GetBytes(_Working);
			ThreadPool.QueueUserWorkItem(Fill);
		}

		private void CheckSwap(int c)
		{
			lock (_sync)
			{
				if (_Index + c < BUFFER_SIZE)
				{
					return;
				}

				lock (_syncB)
				{
					var b = _Working;
					_Working = _Buffer;
					_Buffer = b;
					_Index = 0;
				}
			}
			ThreadPool.QueueUserWorkItem(Fill);
		}

		private void Fill(object o)
		{
			lock (_syncB)
				lock (_CSP)
					_CSP.GetBytes(_Buffer);
		}

		private void _GetBytes(byte[] b)
		{
			int c = b.Length;

			CheckSwap(c);

			lock (_sync)
			{
				Buffer.BlockCopy(_Working, _Index, b, 0, c);
				_Index += c;
			}
		}

		private void _GetBytes(byte[] b, int offset, int count)
		{
			CheckSwap(count);

			lock (_sync)
			{
				Buffer.BlockCopy(_Working, _Index, b, offset, count);
				_Index += count;
			}
		}

		public int Next(int c)
		{
			return (int)(c * NextDouble());
		}

		public bool NextBool()
		{
			return (NextByte() & 1) == 1;
		}

		private byte NextByte()
		{
			CheckSwap(1);

			lock (_sync)
				return _Working[_Index++];
		}

		public void NextBytes(byte[] b)
		{
			int c = b.Length;

			if (c >= LARGE_REQUEST)
			{
				lock (_CSP)
					_CSP.GetBytes(b);
				return;
			}
			_GetBytes(b);
		}

		public unsafe double NextDouble()
		{
			var b = new byte[8];

			if (BitConverter.IsLittleEndian)
			{
				b[7] = 0;
				_GetBytes(b, 0, 7);
			}
			else
			{
				b[0] = 0;
				_GetBytes(b, 1, 7);
			}

			ulong r = 0;
			fixed (byte* buf = b)
			{
				r = *(ulong*)(&buf[0]) >> 3;
			}

			/* double: 53 bits of significand precision
			 * ulong.MaxValue >> 11 = 9007199254740991
			 * 2^53 = 9007199254740992
			 */

			return (double)r / 9007199254740992;
		}
	}

	public sealed class RDRand32 : IRandomImpl, IHardwareRNG
	{
		internal class SafeNativeMethods
		{
			[DllImport("drng32")]
			internal static extern RDRandError rdrand_32(ref uint rand, bool retry);

			[DllImport("drng32")]
			internal static extern RDRandError rdrand_get_bytes(int n, byte[] buffer);
		}

		private static int BUFFER_SIZE = 0x10000;
		private static int LARGE_REQUEST = 0x40;

		private byte[] _Working = new byte[BUFFER_SIZE];
		private byte[] _Buffer = new byte[BUFFER_SIZE];

		private int _Index;

		private readonly object _sync = new object();
		private readonly object _syncB = new object();

		public RDRand32()
		{
			SafeNativeMethods.rdrand_get_bytes(BUFFER_SIZE, _Working);
			ThreadPool.QueueUserWorkItem(Fill);
		}

		public bool IsSupported()
		{
			uint r = 0;
			return SafeNativeMethods.rdrand_32(ref r, true) == RDRandError.Success;
		}

		private void CheckSwap(int c)
		{
			lock (_sync)
			{
				if (_Index + c < BUFFER_SIZE)
				{
					return;
				}

				lock (_syncB)
				{
					var b = _Working;
					_Working = _Buffer;
					_Buffer = b;
					_Index = 0;
				}
			}
			ThreadPool.QueueUserWorkItem(Fill);
		}

		private void Fill(object o)
		{
			lock (_syncB)
				SafeNativeMethods.rdrand_get_bytes(BUFFER_SIZE, _Buffer);
		}

		private void _GetBytes(byte[] b)
		{
			int c = b.Length;

			CheckSwap(c);

			lock (_sync)
			{
				Buffer.BlockCopy(_Working, _Index, b, 0, c);
				_Index += c;
			}
		}

		private void _GetBytes(byte[] b, int offset, int count)
		{
			CheckSwap(count);

			lock (_sync)
			{
				Buffer.BlockCopy(_Working, _Index, b, offset, count);
				_Index += count;
			}
		}

		public int Next(int c)
		{
			return (int)(c * NextDouble());
		}

		public bool NextBool()
		{
			return (NextByte() & 1) == 1;
		}

		private byte NextByte()
		{
			CheckSwap(1);

			lock (_sync)
				return _Working[_Index++];
		}

		public void NextBytes(byte[] b)
		{
			int c = b.Length;

			if (c >= LARGE_REQUEST)
			{
				SafeNativeMethods.rdrand_get_bytes(c, b);
				return;
			}
			_GetBytes(b);
		}

		public unsafe double NextDouble()
		{
			var b = new byte[8];

			if (BitConverter.IsLittleEndian)
			{
				b[7] = 0;
				_GetBytes(b, 0, 7);
			}
			else
			{
				b[0] = 0;
				_GetBytes(b, 1, 7);
			}

			ulong r = 0;
			fixed (byte* buf = b)
			{
				r = *(ulong*)(&buf[0]) >> 3;
			}

			/* double: 53 bits of significand precision
			 * ulong.MaxValue >> 11 = 9007199254740991
			 * 2^53 = 9007199254740992
			 */

			return (double)r / 9007199254740992;
		}
	}

	public sealed class RDRand64 : IRandomImpl, IHardwareRNG
	{
		internal class SafeNativeMethods
		{
			[DllImport("drng64")]
			internal static extern RDRandError rdrand_64(ref ulong rand, bool retry);

			[DllImport("drng64")]
			internal static extern RDRandError rdrand_get_bytes(int n, byte[] buffer);
		}

		private static int BUFFER_SIZE = 0x10000;
		private static int LARGE_REQUEST = 0x40;

		private byte[] _Working = new byte[BUFFER_SIZE];
		private byte[] _Buffer = new byte[BUFFER_SIZE];

		private int _Index;

		private readonly object _sync = new object();
		private readonly object _syncB = new object();

		public RDRand64()
		{
			SafeNativeMethods.rdrand_get_bytes(BUFFER_SIZE, _Working);
			ThreadPool.QueueUserWorkItem(Fill);
		}

		public bool IsSupported()
		{
			ulong r = 0;
			return SafeNativeMethods.rdrand_64(ref r, true) == RDRandError.Success;
		}

		private void CheckSwap(int c)
		{
			lock (_sync)
			{
				if (_Index + c < BUFFER_SIZE)
				{
					return;
				}

				lock (_syncB)
				{
					var b = _Working;
					_Working = _Buffer;
					_Buffer = b;
					_Index = 0;
				}
			}
			ThreadPool.QueueUserWorkItem(Fill);
		}

		private void Fill(object o)
		{
			lock (_syncB)
				SafeNativeMethods.rdrand_get_bytes(BUFFER_SIZE, _Buffer);
		}

		private void _GetBytes(byte[] b)
		{
			int c = b.Length;

			CheckSwap(c);

			lock (_sync)
			{
				Buffer.BlockCopy(_Working, _Index, b, 0, c);
				_Index += c;
			}
		}

		private void _GetBytes(byte[] b, int offset, int count)
		{
			CheckSwap(count);

			lock (_sync)
			{
				Buffer.BlockCopy(_Working, _Index, b, offset, count);
				_Index += count;
			}
		}

		public int Next(int c)
		{
			return (int)(c * NextDouble());
		}

		public bool NextBool()
		{
			return (NextByte() & 1) == 1;
		}

		private byte NextByte()
		{
			CheckSwap(1);

			lock (_sync)
				return _Working[_Index++];
		}

		public void NextBytes(byte[] b)
		{
			int c = b.Length;

			if (c >= LARGE_REQUEST)
			{
				SafeNativeMethods.rdrand_get_bytes(c, b);
				return;
			}
			_GetBytes(b);
		}

		public unsafe double NextDouble()
		{
			var b = new byte[8];

			if (BitConverter.IsLittleEndian)
			{
				b[7] = 0;
				_GetBytes(b, 0, 7);
			}
			else
			{
				b[0] = 0;
				_GetBytes(b, 1, 7);
			}

			ulong r = 0;
			fixed (byte* buf = b)
			{
				r = *(ulong*)(&buf[0]) >> 3;
			}

			/* double: 53 bits of significand precision
			 * ulong.MaxValue >> 11 = 9007199254740991
			 * 2^53 = 9007199254740992
			 */

			return (double)r / 9007199254740992;
		}
	}

	public enum RDRandError
	{
		Unknown = -4,
		Unsupported = -3,
		Supported = -2,
		NotReady = -1,

		Failure = 0,

		Success = 1,
	}
}
