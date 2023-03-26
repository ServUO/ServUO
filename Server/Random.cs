#region References
using System;
using System.IO;
using System.Runtime.CompilerServices;
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

			if (_Random is IHardwareRNG rng)
			{
				if (!rng.IsSupported())
				{
					_Random = new CSPRandom();
				}
			}
		}

		public static bool IsHardwareRNG => _Random is IHardwareRNG;

		public static Type Type => _Random.GetType();

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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public int Next(int c)
		{
			if (c > 0)
			{
				return m_Random.Next(c);
			}

			return 0;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void NextBytes(byte[] b)
		{
			m_Random.NextBytes(b);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public double NextDouble()
		{
			return m_Random.NextDouble();
		}

		public bool NextBool()
		{
			return NextDouble() < 0.5;
		}
	}

	public sealed class CSPRandom : IRandomImpl
	{
		private readonly RNGCryptoServiceProvider _CSP = new RNGCryptoServiceProvider();

		private static readonly int BUFFER_SIZE = 0x4000;
		private static readonly int LARGE_REQUEST = 0x40;

		private byte[] _Working = new byte[BUFFER_SIZE];
		private byte[] _Buffer = new byte[BUFFER_SIZE];

		private int _Index;

		public CSPRandom()
		{
			_CSP.GetBytes(_Working);

			_ = ThreadPool.QueueUserWorkItem(Fill);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void CheckSwap(int c)
		{
			if (_Index + c < BUFFER_SIZE)
			{
				return;
			}

				(_Buffer, _Working) = (_Working, _Buffer);

				_Index = 0;

			_ = ThreadPool.QueueUserWorkItem(Fill);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void Fill(object o)
		{
			_CSP.GetBytes(_Buffer);
		}

		private void InternalGetBytes(byte[] b)
		{
			InternalGetBytes(b, 0, b.Length);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void InternalGetBytes(byte[] b, int offset, int count)
		{
			CheckSwap(count);

			Buffer.BlockCopy(_Working, _Index, b, offset, count);

			_Index += count;
		}

		public int Next(int c)
		{
			return (int)(c * NextDouble());
		}

		public bool NextBool()
		{
			return (NextByte() & 1) == 1;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private byte NextByte()
		{
			CheckSwap(1);

			return _Working[_Index++];
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void NextBytes(byte[] b)
		{
			var c = b.Length;

			if (c >= LARGE_REQUEST)
			{
				_CSP.GetBytes(b);

				return;
			}

			InternalGetBytes(b);
		}

		public unsafe double NextDouble()
		{
			var b = new byte[8];

			if (BitConverter.IsLittleEndian)
			{
				b[7] = 0;

				InternalGetBytes(b, 0, 7);
			}
			else
			{
				b[0] = 0;

				InternalGetBytes(b, 1, 7);
			}

			ulong r = 0;

			fixed (byte* buf = b)
			{
				r = *(ulong*)&buf[0] >> 3;
			}

			/* double: 53 bits of significand precision
			 * ulong.MaxValue >> 11 = 9007199254740991
			 * 2^53 = 9007199254740992
			 */

			return r / 9007199254740992.0;
		}
	}

	public sealed class RDRand32 : IRandomImpl, IHardwareRNG
	{
		internal class SafeNativeMethods
		{
			[DllImport("drng32"), MethodImpl(MethodImplOptions.Synchronized)]
			internal static extern RDRandError rdrand_32(ref uint rand, bool retry);

			[DllImport("drng32"), MethodImpl(MethodImplOptions.Synchronized)]
			internal static extern RDRandError rdrand_get_bytes(int n, byte[] buffer);
		}

		private static readonly int BUFFER_SIZE = 0x10000;
		private static readonly int LARGE_REQUEST = 0x40;

		private byte[] _Working = new byte[BUFFER_SIZE];
		private byte[] _Buffer = new byte[BUFFER_SIZE];

		private int _Index;

		public RDRand32()
		{
			_ = SafeNativeMethods.rdrand_get_bytes(BUFFER_SIZE, _Working);
			_ = ThreadPool.QueueUserWorkItem(Fill);
		}

		public bool IsSupported()
		{
			uint r = 0;

			return SafeNativeMethods.rdrand_32(ref r, true) == RDRandError.Success;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void CheckSwap(int c)
		{
			if (_Index + c < BUFFER_SIZE)
			{
				return;
			}

				(_Buffer, _Working) = (_Working, _Buffer);

				_Index = 0;

			_ = ThreadPool.QueueUserWorkItem(Fill);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void Fill(object o)
		{
			_ = SafeNativeMethods.rdrand_get_bytes(BUFFER_SIZE, _Buffer);
		}

		private void InternalGetBytes(byte[] b)
		{
			InternalGetBytes(b, 0, b.Length);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void InternalGetBytes(byte[] b, int offset, int count)
		{
			CheckSwap(count);

			Buffer.BlockCopy(_Working, _Index, b, offset, count);

			_Index += count;
		}

		public int Next(int c)
		{
			return (int)(c * NextDouble());
		}

		public bool NextBool()
		{
			return (NextByte() & 1) == 1;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private byte NextByte()
		{
			CheckSwap(1);

			return _Working[_Index++];
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void NextBytes(byte[] b)
		{
			var c = b.Length;

			if (c >= LARGE_REQUEST)
			{
				_ = SafeNativeMethods.rdrand_get_bytes(c, b);

				return;
			}

			InternalGetBytes(b);
		}

		public unsafe double NextDouble()
		{
			var b = new byte[8];

			if (BitConverter.IsLittleEndian)
			{
				b[7] = 0;

				InternalGetBytes(b, 0, 7);
			}
			else
			{
				b[0] = 0;

				InternalGetBytes(b, 1, 7);
			}

			ulong r = 0;

			fixed (byte* buf = b)
			{
				r = *(ulong*)&buf[0] >> 3;
			}

			/* double: 53 bits of significand precision
			 * ulong.MaxValue >> 11 = 9007199254740991
			 * 2^53 = 9007199254740992
			 */

			return r / 9007199254740992.0;
		}
	}

	public sealed class RDRand64 : IRandomImpl, IHardwareRNG
	{
		internal class SafeNativeMethods
		{
			[DllImport("drng64"), MethodImpl(MethodImplOptions.Synchronized)]
			internal static extern RDRandError rdrand_64(ref ulong rand, bool retry);

			[DllImport("drng64"), MethodImpl(MethodImplOptions.Synchronized)]
			internal static extern RDRandError rdrand_get_bytes(int n, byte[] buffer);
		}

		private static readonly int BUFFER_SIZE = 0x10000;
		private static readonly int LARGE_REQUEST = 0x40;

		private byte[] _Working = new byte[BUFFER_SIZE];
		private byte[] _Buffer = new byte[BUFFER_SIZE];

		private int _Index;

		public RDRand64()
		{
			_ = SafeNativeMethods.rdrand_get_bytes(BUFFER_SIZE, _Working);
			_ = ThreadPool.QueueUserWorkItem(Fill);
		}

		public bool IsSupported()
		{
			ulong r = 0;

			return SafeNativeMethods.rdrand_64(ref r, true) == RDRandError.Success;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void CheckSwap(int c)
		{
			if (_Index + c < BUFFER_SIZE)
			{
				return;
			}

				(_Buffer, _Working) = (_Working, _Buffer);

				_Index = 0;

			_ = ThreadPool.QueueUserWorkItem(Fill);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void Fill(object o)
		{
				_ = SafeNativeMethods.rdrand_get_bytes(BUFFER_SIZE, _Buffer);
		}

		private void InternalGetBytes(byte[] b)
		{
			InternalGetBytes(b, 0, b.Length);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void InternalGetBytes(byte[] b, int offset, int count)
		{
			CheckSwap(count);

			Buffer.BlockCopy(_Working, _Index, b, offset, count);

			_Index += count;
		}

		public int Next(int c)
		{
			return (int)(c * NextDouble());
		}

		public bool NextBool()
		{
			return (NextByte() & 1) == 1;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private byte NextByte()
		{
			CheckSwap(1);

			return _Working[_Index++];
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void NextBytes(byte[] b)
		{
			var c = b.Length;

			if (c >= LARGE_REQUEST)
			{
				_ = SafeNativeMethods.rdrand_get_bytes(c, b);

				return;
			}

			InternalGetBytes(b);
		}

		public unsafe double NextDouble()
		{
			var b = new byte[8];

			if (BitConverter.IsLittleEndian)
			{
				b[7] = 0;

				InternalGetBytes(b, 0, 7);
			}
			else
			{
				b[0] = 0;

				InternalGetBytes(b, 1, 7);
			}

			ulong r = 0;

			fixed (byte* buf = b)
			{
				r = *(ulong*)&buf[0] >> 3;
			}

			/* double: 53 bits of significand precision
			 * ulong.MaxValue >> 11 = 9007199254740991
			 * 2^53 = 9007199254740992
			 */

			return r / 9007199254740992.0;
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
