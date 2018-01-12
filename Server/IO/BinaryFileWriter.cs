using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using CustomsFramework;
using Server.Guilds;

namespace Server
{
	public class BinaryFileWriter : GenericWriter
	{
		private readonly bool PrefixStrings;
		private readonly Stream m_File;

		protected virtual int BufferSize { get; } = 64 * 1024;

		private readonly byte[] m_Buffer;

		private int m_Index;

		private readonly Encoding m_Encoding;

		public BinaryFileWriter(Stream strm, bool prefixStr)
		{
			PrefixStrings = prefixStr;
			m_Encoding = Utility.UTF8;
			m_Buffer = new byte[BufferSize];
			m_File = strm;
		}

		public BinaryFileWriter(string filename, bool prefixStr)
		{
			PrefixStrings = prefixStr;
			m_Buffer = new byte[BufferSize];
			m_File = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
			m_Encoding = Utility.UTF8WithEncoding;
		}

		public void Flush()
		{
			if (m_Index > 0)
			{
				m_Position += m_Index;

				m_File.Write(m_Buffer, 0, m_Index);
				m_Index = 0;
			}
		}

		private long m_Position;

		public override long Position => m_Position + m_Index;

		public Stream UnderlyingStream
		{
			get
			{
				if (m_Index > 0)
				{
					Flush();
				}

				return m_File;
			}
		}

		public override void Close()
		{
			if (m_Index > 0)
			{
				Flush();
			}

			m_File.Close();
		}

		public override void WriteEncodedInt(int value)
		{
			uint v = (uint)value;

			while (v >= 0x80)
			{
				if ((m_Index + 1) > m_Buffer.Length)
				{
					Flush();
				}

				m_Buffer[m_Index++] = (byte)(v | 0x80);
				v >>= 7;
			}

			if ((m_Index + 1) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index++] = (byte)v;
		}

		private byte[] m_CharacterBuffer;
		private int m_MaxBufferChars;
		private const int LargeByteBufferSize = 256;

		internal void InternalWriteString(string value)
		{
			int length = m_Encoding.GetByteCount(value);

			WriteEncodedInt(length);

			if (m_CharacterBuffer == null)
			{
				m_CharacterBuffer = new byte[LargeByteBufferSize];
				m_MaxBufferChars = LargeByteBufferSize / m_Encoding.GetMaxByteCount(1);
			}

			if (length > LargeByteBufferSize)
			{
				int current = 0;
				int charsLeft = value.Length;

				while (charsLeft > 0)
				{
					int charCount = (charsLeft > m_MaxBufferChars) ? m_MaxBufferChars : charsLeft;
					int byteLength = m_Encoding.GetBytes(value, current, charCount, m_CharacterBuffer, 0);

					if ((m_Index + byteLength) > m_Buffer.Length)
					{
						Flush();
					}

					Buffer.BlockCopy(m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength);
					m_Index += byteLength;

					current += charCount;
					charsLeft -= charCount;
				}
			}
			else
			{
				int byteLength = m_Encoding.GetBytes(value, 0, value.Length, m_CharacterBuffer, 0);

				if ((m_Index + byteLength) > m_Buffer.Length)
				{
					Flush();
				}

				Buffer.BlockCopy(m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength);
				m_Index += byteLength;
			}
		}

		public override void Write(string value)
		{
			if (PrefixStrings)
			{
				if (value == null)
				{
					if ((m_Index + 1) > m_Buffer.Length)
					{
						Flush();
					}

					m_Buffer[m_Index++] = 0;
				}
				else
				{
					if ((m_Index + 1) > m_Buffer.Length)
					{
						Flush();
					}

					m_Buffer[m_Index++] = 1;

					InternalWriteString(value);
				}
			}
			else
			{
				InternalWriteString(value);
			}
		}

		public override void Write(DateTime value)
		{
			Write(value.Ticks);
		}

		public override void Write(DateTimeOffset value)
		{
			Write(value.Ticks);
			Write(value.Offset.Ticks);
		}

		public override void Write(IPAddress value)
		{
			Write(Utility.GetLongAddressValue(value));
		}

		public override void Write(TimeSpan value)
		{
			Write(value.Ticks);
		}

		public override void Write(decimal value)
		{
			var bits = Decimal.GetBits(value);

			for (int i = 0; i < bits.Length; ++i)
			{
				Write(bits[i]);
			}
		}

		public override void Write(long value)
		{
			if ((m_Index + 8) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Buffer[m_Index + 4] = (byte)(value >> 32);
			m_Buffer[m_Index + 5] = (byte)(value >> 40);
			m_Buffer[m_Index + 6] = (byte)(value >> 48);
			m_Buffer[m_Index + 7] = (byte)(value >> 56);
			m_Index += 8;
		}

		public override void Write(ulong value)
		{
			if ((m_Index + 8) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Buffer[m_Index + 4] = (byte)(value >> 32);
			m_Buffer[m_Index + 5] = (byte)(value >> 40);
			m_Buffer[m_Index + 6] = (byte)(value >> 48);
			m_Buffer[m_Index + 7] = (byte)(value >> 56);
			m_Index += 8;
		}

		public override void Write(int value)
		{
			if ((m_Index + 4) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Index += 4;
		}

		public override void Write(uint value)
		{
			if ((m_Index + 4) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Index += 4;
		}

		public override void Write(short value)
		{
			if ((m_Index + 2) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Index += 2;
		}

		public override void Write(ushort value)
		{
			if ((m_Index + 2) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Index += 2;
		}

		public override unsafe void Write(double value)
		{
			if ((m_Index + 8) > m_Buffer.Length)
			{
				Flush();
			}

			fixed (byte* pBuffer = m_Buffer)
			{
				*((double*)(pBuffer + m_Index)) = value;
			}

			m_Index += 8;
		}

		public override unsafe void Write(float value)
		{
			if ((m_Index + 4) > m_Buffer.Length)
			{
				Flush();
			}

			fixed (byte* pBuffer = m_Buffer)
			{
				*((float*)(pBuffer + m_Index)) = value;
			}

			m_Index += 4;
		}

		private readonly char[] m_SingleCharBuffer = new char[1];

		public override void Write(char value)
		{
			if ((m_Index + 8) > m_Buffer.Length)
			{
				Flush();
			}

			m_SingleCharBuffer[0] = value;

			int byteCount = m_Encoding.GetBytes(m_SingleCharBuffer, 0, 1, m_Buffer, m_Index);
			m_Index += byteCount;
		}

		public override void Write(byte value)
		{
			if ((m_Index + 1) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index++] = value;
		}

		public override void Write(sbyte value)
		{
			if ((m_Index + 1) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index++] = (byte)value;
		}

		public override void Write(bool value)
		{
			if ((m_Index + 1) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index++] = (byte)(value ? 1 : 0);
		}
	}
}