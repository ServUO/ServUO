#region References
using System;
using System.IO;
#endregion

namespace Ultima
{
	/// <summary>
	///     Provides methods to interact with the Ultima Online client.
	/// </summary>
	public sealed class Client
	{
		private const int WM_CHAR = 0x102;

		private static ClientWindowHandle m_Handle = ClientWindowHandle.Invalid;

		private static WindowProcessStream m_ProcStream;
		private static LocationPointer m_LocationPointer;

		private static bool m_Is_Iris2;

		private Client()
		{ }

		/// <summary>
		///     Gets a <see cref="ProcessStream" /> instance which can be used to read the memory. Null is returned if the Client is not running.
		/// </summary>
		public static ProcessStream ProcessStream
		{
			get
			{
				if (m_ProcStream == null || m_ProcStream.Window != Handle)
				{
					if (Running)
					{
						m_ProcStream = new WindowProcessStream(Handle);
					}
					else
					{
						m_ProcStream = null;
					}
				}

				return m_ProcStream;
			}
		}

		/// <summary>
		///     Reads the current <paramref name="x" />, <paramref name="y" />, and <paramref name="z" /> from memory based on a
		///     <see
		///         cref="Calibrate">
		///         calibrated memory location
		///     </see>
		///     .
		///     <seealso cref="Calibrate" />
		///     <seealso cref="ProcessStream" />
		///     <returns>True if the location was found, false if not</returns>
		/// </summary>
		public static bool FindLocation(ref int x, ref int y, ref int z, ref int facet)
		{
			LocationPointer lp = LocationPointer;
			ProcessStream pc = ProcessStream;

			if (pc == null || lp == null)
			{
				return false;
			}

			pc.BeginAccess();

			if (lp.PointerX > 0)
			{
				pc.Seek(lp.PointerX, SeekOrigin.Begin);
				x = Read(pc, lp.SizeX);
			}

			if (lp.PointerY > 0)
			{
				pc.Seek(lp.PointerY, SeekOrigin.Begin);
				y = Read(pc, lp.SizeY);
			}

			if (lp.PointerZ > 0)
			{
				pc.Seek(lp.PointerZ, SeekOrigin.Begin);
				z = Read(pc, lp.SizeZ);
			}

			if (lp.PointerF > 0)
			{
				pc.Seek(lp.PointerF, SeekOrigin.Begin);
				facet = Read(pc, lp.SizeF);
			}

			pc.EndAccess();

			return true;
		}

		public static int Read(ProcessStream pc, int bytes)
		{
			var buffer = new byte[bytes];

			pc.Read(buffer, 0, bytes);

			switch (bytes)
			{
				case 1:
					return (sbyte)buffer[0];
				case 2:
					return (short)(buffer[0] | (buffer[1] << 8));
				case 4:
					return (buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
			}

			int val = 0;
			int bits = 0;

			for (int i = 0; i < buffer.Length; ++i)
			{
				val |= buffer[i] << bits;
				bits += 8;
			}

			return val;
		}

		public static int Search(ProcessStream pc, byte[] mask, byte[] vals)
		{
			if (mask.Length != vals.Length)
			{
				throw new Exception();
			}

			const int chunkSize = 4096;
			int readSize = chunkSize + mask.Length;

			pc.BeginAccess();

			var read = new byte[readSize];

			for (int i = 0;; ++i)
			{
				pc.Seek(0x400000 + (i * chunkSize), SeekOrigin.Begin);
				int count = pc.Read(read, 0, readSize);

				if (count != readSize)
				{
					break;
				}

				for (int j = 0; j < chunkSize; ++j)
				{
					bool ok = true;

					for (int k = 0; ok && k < mask.Length; ++k)
					{
						ok = ((read[j + k] & mask[k]) == vals[k]);
					}

					if (ok)
					{
						pc.EndAccess();
						return 0x400000 + (i * chunkSize) + j;
					}
				}
			}

			pc.EndAccess();
			return 0;
		}

		public static int Search(ProcessStream pc, byte[] buffer)
		{
			const int chunkSize = 4096;
			int readSize = chunkSize + buffer.Length;

			pc.BeginAccess();

			var read = new byte[readSize];

			for (int i = 0;; ++i)
			{
				pc.Seek(0x400000 + (i * chunkSize), SeekOrigin.Begin);
				int count = pc.Read(read, 0, readSize);

				if (count != readSize)
				{
					break;
				}

				for (int j = 0; j < chunkSize; ++j)
				{
					bool ok = true;

					for (int k = 0; ok && k < buffer.Length; ++k)
					{
						ok = (buffer[k] == read[j + k]);
					}

					if (ok)
					{
						pc.EndAccess();
						return 0x400000 + (i * chunkSize) + j;
					}
				}
			}

			pc.EndAccess();
			return 0;
		}

		/// <summary>
		///     Attempts to calibrate the <see cref="FindLocation" /> method based on an input <paramref name="x" />,
		///     <paramref
		///         name="y" />
		///     , and <paramref name="z" />.
		///     <seealso cref="FindLocation" />
		///     <seealso cref="ProcessStream" />
		/// </summary>
		/// <returns>The calibrated memory location -or- 0 if it could not be found.</returns>
		public static void Calibrate(int x, int y, int z)
		{
			m_LocationPointer = null;

			ProcessStream pc = ProcessStream;

			if (pc == null)
			{
				return;
			}

			var buffer = new byte[12];

			buffer[0] = (byte)z;
			buffer[1] = (byte)(z >> 8);
			buffer[2] = (byte)(z >> 16);
			buffer[3] = (byte)(z >> 24);

			buffer[4] = (byte)y;
			buffer[5] = (byte)(y >> 8);
			buffer[6] = (byte)(y >> 16);
			buffer[7] = (byte)(y >> 24);

			buffer[8] = (byte)x;
			buffer[9] = (byte)(x >> 8);
			buffer[10] = (byte)(x >> 16);
			buffer[11] = (byte)(x >> 24);

			int ptr = Search(pc, buffer);

			if (ptr == 0)
			{
				return;
			}

			m_LocationPointer = new LocationPointer(ptr + 8, ptr + 4, ptr, 0, 4, 4, 4, 0);
		}

		/// <summary>
		///     Attempts to automatically calibrate the <see cref="FindLocation" /> method.
		/// </summary>
		/// <returns>The calibrated memory location -or- 0 if it could not be found.</returns>
		public static void Calibrate()
		{
			Calibrate(CalibrationInfo.GetList());
		}

		/// <summary>
		///     Attempts to automatically calibrate the <see cref="FindLocation" /> method.
		/// </summary>
		/// <returns>The calibrated memory location -or- 0 if it could not be found.</returns>
		public static void Calibrate(CalibrationInfo[] info)
		{
			m_LocationPointer = null;

			ProcessStream pc = ProcessStream;

			if (pc == null)
			{
				return;
			}

			int ptrX = 0, sizeX = 0;
			int ptrY = 0, sizeY = 0;
			int ptrZ = 0, sizeZ = 0;
			int ptrF = 0, sizeF = 0;

			for (int i = 0; i < info.Length; ++i)
			{
				CalibrationInfo ci = info[i];

				int ptr = Search(pc, ci.Mask, ci.Vals);

				if (ptr == 0)
				{
					continue;
				}

				if (ptrX == 0 && ci.DetX.Length > 0)
				{
					GetCoordDetails(pc, ptr, ci.DetX, out ptrX, out sizeX);
				}

				if (ptrY == 0 && ci.DetY.Length > 0)
				{
					GetCoordDetails(pc, ptr, ci.DetY, out ptrY, out sizeY);
				}

				if (ptrZ == 0 && ci.DetZ.Length > 0)
				{
					GetCoordDetails(pc, ptr, ci.DetZ, out ptrZ, out sizeZ);
				}

				if (ptrF == 0 && ci.DetF.Length > 0)
				{
					GetCoordDetails(pc, ptr, ci.DetF, out ptrF, out sizeF);
				}

				if (ptrX != 0 && ptrY != 0 && ptrZ != 0 && ptrF != 0)
				{
					break;
				}
			}

			if (ptrX != 0 || ptrY != 0 || ptrZ != 0 || ptrF != 0)
			{
				m_LocationPointer = new LocationPointer(ptrX, ptrY, ptrZ, ptrF, sizeX, sizeY, sizeZ, sizeF);
			}
		}

		private static void GetCoordDetails(ProcessStream pc, int ptr, byte[] dets, out int coordPointer, out int coordSize)
		{
			pc.Seek(ptr + dets[0], SeekOrigin.Begin);
			coordPointer = Read(pc, dets[1]);

			if (dets[2] < 0xFF)
			{
				pc.Seek(coordPointer, SeekOrigin.Begin);
				coordPointer = Read(pc, dets[2]);
			}

			if (dets[3] < 0xFF)
			{
				pc.Seek(ptr + dets[3], SeekOrigin.Begin);
				coordPointer += Read(pc, dets[4]);
			}

			/*
			 * arul:
			 *	The variable 'dets[6]' represents an offset into the struct that holds an info about players current location.
			 *	Added not to break functionality with the older clients (I hope).
			 * 
			 * The struct looks as follows: 
			 * 
			 *  DWORD fLoggedIn; 
			 *	DWORD Z;
			 *  DWORD Y;
			 *	DWORD X;
			 *	DWORD Facet;  
			 *  
			 */
			if (dets.Length == 7 && dets[6] < 0xFF)
			{
				coordPointer += dets[6];
			}
			coordSize = dets[5];
		}

		/// <summary>
		///     Gets or sets the memory location currently used for the <see cref="FindLocation" /> method.
		///     <seealso cref="FindLocation" />
		///     <seealso cref="Calibrate" />
		/// </summary>
		public static LocationPointer LocationPointer { get { return m_LocationPointer; } set { m_LocationPointer = value; } }

		/// <summary>
		///     Gets the current window handle. A value of <c>ClientHandle.Invalid</c> is returned if the Client is not currently running.
		///     <seealso cref="Running" />
		/// </summary>
		public static ClientWindowHandle Handle
		{
			get
			{
				if (NativeMethods.IsWindow(m_Handle) == 0)
				{
					m_Handle = FindHandle();
				}

				return m_Handle;
			}
		}

		/// <summary>
		///     Whether or not the Client is currently running.
		///     <seealso cref="ClientHandle" />
		/// </summary>
		public static bool Running { get { return (!Handle.IsInvalid); } }

		/// <summary>
		///     Is Client Iris2
		/// </summary>
		public static bool Is_Iris2 { get { return m_Is_Iris2; } set { m_Is_Iris2 = value; } }

		private static void SendChar(ClientWindowHandle hWnd, char c)
		{
			int value = c;
			int lParam = 1 | ((NativeMethods.OemKeyScan(value) & 0xFF) << 16) | (0x3 << 30);

			NativeMethods.PostMessage(hWnd, WM_CHAR, value, lParam);
		}

		/// <summary>
		///     Brings the Client window to the foreground.
		/// </summary>
		/// <returns>True if the Client is running, false if not.</returns>
		public static bool BringToTop()
		{
			ClientWindowHandle hWnd = Handle;

			if (!hWnd.IsInvalid)
			{
				NativeMethods.SetForegroundWindow(hWnd);

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		///     Sends a <see cref="string" /> of characters (<paramref name="text" />) to the Client. The string is followed by a carriage return and line feed.
		/// </summary>
		/// <returns>True if the Client is running, false if not.</returns>
		public static bool SendText(string text)
		{
			ClientWindowHandle hWnd = Handle;

			if (!hWnd.IsInvalid)
			{
				for (int i = 0; i < text.Length; ++i)
				{
					SendChar(hWnd, text[i]);
				}

				SendChar(hWnd, '\r');
				SendChar(hWnd, '\n');

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		///     Sends a formatted <see cref="string" /> of characters to the Client. The string is followed by a carriage return and line feed. The format functionality is the same as
		///     <see
		///         cref="string.Format">
		///         String.Format
		///     </see>
		///     .
		/// </summary>
		/// <returns>True if the Client is running, false if not.</returns>
		public static bool SendText(string format, params object[] args)
		{
			return SendText(String.Format(format, args));
		}

		private static ClientWindowHandle FindHandle()
		{
			ClientWindowHandle hWnd;

			if (NativeMethods.IsWindow(hWnd = NativeMethods.FindWindowA("Ultima Online", null)) != 0)
			{
				return hWnd;
			}

			if (NativeMethods.IsWindow(hWnd = NativeMethods.FindWindowA("Ultima Online Third Dawn", null)) != 0)
			{
				return hWnd;
			}
			if (NativeMethods.IsWindow(hWnd = NativeMethods.FindWindowA("OgreGLWindow", null)) != 0)
			{
				m_Is_Iris2 = true;
				return hWnd;
			}

			return ClientWindowHandle.Invalid;
		}
	}
}