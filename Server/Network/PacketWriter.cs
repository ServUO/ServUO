#region Header
// **********
// ServUO - PacketWriter.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#endregion

namespace Server.Network
{
	/// <summary>
	///     Provides functionality for writing primitive binary data.
	/// </summary>
	public class PacketWriter
	{
		private static readonly Stack<PacketWriter> m_Pool = new Stack<PacketWriter>();

		public static PacketWriter CreateInstance()
		{
			return CreateInstance(32);
		}

		public static PacketWriter CreateInstance(int capacity)
		{
			PacketWriter pw = null;

			lock (m_Pool)
			{
				if (m_Pool.Count > 0)
				{
					pw = m_Pool.Pop();

					if (pw != null)
					{
						pw.m_Capacity = capacity;
						pw.m_Stream.SetLength(0);
					}
				}
			}

			if (pw == null)
			{
				pw = new PacketWriter(capacity);
			}

			return pw;
		}

		public static void ReleaseInstance(PacketWriter pw)
		{
			lock (m_Pool)
			{
				if (!m_Pool.Contains(pw))
				{
					m_Pool.Push(pw);
				}
				else
				{
					try
					{
						using (StreamWriter op = new StreamWriter("neterr.log"))
						{
							op.WriteLine("{0}\tInstance pool contains writer", DateTime.UtcNow);
						}
					}
					catch
					{
						Console.WriteLine("net error");
					}
				}
			}
		}

		/// <summary>
		///     Internal stream which holds the entire packet.
		/// </summary>
		private readonly MemoryStream m_Stream;

		private int m_Capacity;

		/// <summary>
		///     Internal format buffer.
		/// </summary>
		private readonly byte[] m_Buffer = new byte[4];

		/// <summary>
		///     Instantiates a new PacketWriter instance with the default capacity of 4 bytes.
		/// </summary>
		public PacketWriter()
			: this(32)
		{ }

		/// <summary>
		///     Instantiates a new PacketWriter instance with a given capacity.
		/// </summary>
		/// <param name="capacity">Initial capacity for the internal stream.</param>
		public PacketWriter(int capacity)
		{
			m_Stream = new MemoryStream(capacity);
			m_Capacity = capacity;
		}

		/// <summary>
		///     Writes a 1-byte boolean value to the underlying stream. False is represented by 0, true by 1.
		/// </summary>
		public void Write(bool value)
		{
			m_Stream.WriteByte((byte)(value ? 1 : 0));
		}

		/// <summary>
		///     Writes a 1-byte unsigned integer value to the underlying stream.
		/// </summary>
		public void Write(byte value)
		{
			m_Stream.WriteByte(value);
		}

		/// <summary>
		///     Writes a 1-byte signed integer value to the underlying stream.
		/// </summary>
		public void Write(sbyte value)
		{
			m_Stream.WriteByte((byte)value);
		}

		/// <summary>
		///     Writes a 2-byte signed integer value to the underlying stream.
		/// </summary>
		public void Write(short value)
		{
			m_Buffer[0] = (byte)(value >> 8);
			m_Buffer[1] = (byte)value;

			m_Stream.Write(m_Buffer, 0, 2);
		}

		/// <summary>
		///     Writes a 2-byte unsigned integer value to the underlying stream.
		/// </summary>
		public void Write(ushort value)
		{
			m_Buffer[0] = (byte)(value >> 8);
			m_Buffer[1] = (byte)value;

			m_Stream.Write(m_Buffer, 0, 2);
		}

		/// <summary>
		///     Writes a 4-byte signed integer value to the underlying stream.
		/// </summary>
		public void Write(int value)
		{
			m_Buffer[0] = (byte)(value >> 24);
			m_Buffer[1] = (byte)(value >> 16);
			m_Buffer[2] = (byte)(value >> 8);
			m_Buffer[3] = (byte)value;

			m_Stream.Write(m_Buffer, 0, 4);
		}

		/// <summary>
		///     Writes a 4-byte unsigned integer value to the underlying stream.
		/// </summary>
		public void Write(uint value)
		{
			m_Buffer[0] = (byte)(value >> 24);
			m_Buffer[1] = (byte)(value >> 16);
			m_Buffer[2] = (byte)(value >> 8);
			m_Buffer[3] = (byte)value;

			m_Stream.Write(m_Buffer, 0, 4);
		}

		/// <summary>
		///     Writes a sequence of bytes to the underlying stream
		/// </summary>
		public void Write(byte[] buffer, int offset, int size)
		{
			m_Stream.Write(buffer, offset, size);
		}

		/// <summary>
		///     Writes a fixed-length ASCII-encoded string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
		/// </summary>
		public void WriteAsciiFixed(string value, int size)
		{
			if (value == null)
			{
				Console.WriteLine("Network: Attempted to WriteAsciiFixed() with null value");
				value = String.Empty;
			}

			int length = value.Length;

			m_Stream.SetLength(m_Stream.Length + size);

			if (length >= size)
			{
				m_Stream.Position += Encoding.ASCII.GetBytes(value, 0, size, m_Stream.GetBuffer(), (int)m_Stream.Position);
			}
			else
			{
				Encoding.ASCII.GetBytes(value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position);
				m_Stream.Position += size;
			}

			/*byte[] buffer = Encoding.ASCII.GetBytes( value );

			if ( buffer.Length >= size )
			{
				m_Stream.Write( buffer, 0, size );
			}
			else
			{
				m_Stream.Write( buffer, 0, buffer.Length );
				Fill( size - buffer.Length );
			}*/
		}

		/// <summary>
		///     Writes a dynamic-length ASCII-encoded string value to the underlying stream, followed by a 1-byte null character.
		/// </summary>
		public void WriteAsciiNull(string value)
		{
			if (value == null)
			{
				Console.WriteLine("Network: Attempted to WriteAsciiNull() with null value");
				value = String.Empty;
			}

			int length = value.Length;

			m_Stream.SetLength(m_Stream.Length + length + 1);

			Encoding.ASCII.GetBytes(value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position);
			m_Stream.Position += length + 1;

			/*byte[] buffer = Encoding.ASCII.GetBytes( value );

			m_Stream.Write( buffer, 0, buffer.Length );
			m_Stream.WriteByte( 0 );*/
		}

		/// <summary>
		///     Writes a dynamic-length little-endian unicode string value to the underlying stream, followed by a 2-byte null character.
		/// </summary>
		public void WriteLittleUniNull(string value)
		{
			if (value == null)
			{
				Console.WriteLine("Network: Attempted to WriteLittleUniNull() with null value");
				value = String.Empty;
			}

			int length = value.Length;

			m_Stream.SetLength(m_Stream.Length + ((length + 1) * 2));

			m_Stream.Position += Encoding.Unicode.GetBytes(value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position);
			m_Stream.Position += 2;

			/*byte[] buffer = Encoding.Unicode.GetBytes( value );

			m_Stream.Write( buffer, 0, buffer.Length );

			m_Buffer[0] = 0;
			m_Buffer[1] = 0;
			m_Stream.Write( m_Buffer, 0, 2 );*/
		}

		/// <summary>
		///     Writes a fixed-length little-endian unicode string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
		/// </summary>
		public void WriteLittleUniFixed(string value, int size)
		{
			if (value == null)
			{
				Console.WriteLine("Network: Attempted to WriteLittleUniFixed() with null value");
				value = String.Empty;
			}

			size *= 2;

			int length = value.Length;

			m_Stream.SetLength(m_Stream.Length + size);

			if ((length * 2) >= size)
			{
				m_Stream.Position += Encoding.Unicode.GetBytes(value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position);
			}
			else
			{
				Encoding.Unicode.GetBytes(value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position);
				m_Stream.Position += size;
			}

			/*size *= 2;

			byte[] buffer = Encoding.Unicode.GetBytes( value );

			if ( buffer.Length >= size )
			{
				m_Stream.Write( buffer, 0, size );
			}
			else
			{
				m_Stream.Write( buffer, 0, buffer.Length );
				Fill( size - buffer.Length );
			}*/
		}

		/// <summary>
		///     Writes a dynamic-length big-endian unicode string value to the underlying stream, followed by a 2-byte null character.
		/// </summary>
		public void WriteBigUniNull(string value)
		{
			if (value == null)
			{
				Console.WriteLine("Network: Attempted to WriteBigUniNull() with null value");
				value = String.Empty;
			}

			int length = value.Length;

			m_Stream.SetLength(m_Stream.Length + ((length + 1) * 2));

			m_Stream.Position += Encoding.BigEndianUnicode.GetBytes(
				value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position);
			m_Stream.Position += 2;

			/*byte[] buffer = Encoding.BigEndianUnicode.GetBytes( value );

			m_Stream.Write( buffer, 0, buffer.Length );

			m_Buffer[0] = 0;
			m_Buffer[1] = 0;
			m_Stream.Write( m_Buffer, 0, 2 );*/
		}

		/// <summary>
		///     Writes a fixed-length big-endian unicode string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
		/// </summary>
		public void WriteBigUniFixed(string value, int size)
		{
			if (value == null)
			{
				Console.WriteLine("Network: Attempted to WriteBigUniFixed() with null value");
				value = String.Empty;
			}

			size *= 2;

			int length = value.Length;

			m_Stream.SetLength(m_Stream.Length + size);

			if ((length * 2) >= size)
			{
				m_Stream.Position += Encoding.BigEndianUnicode.GetBytes(
					value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position);
			}
			else
			{
				Encoding.BigEndianUnicode.GetBytes(value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position);
				m_Stream.Position += size;
			}

			/*size *= 2;

			byte[] buffer = Encoding.BigEndianUnicode.GetBytes( value );

			if ( buffer.Length >= size )
			{
				m_Stream.Write( buffer, 0, size );
			}
			else
			{
				m_Stream.Write( buffer, 0, buffer.Length );
				Fill( size - buffer.Length );
			}*/
		}

		/// <summary>
		///     Fills the stream from the current position up to (capacity) with 0x00's
		/// </summary>
		public void Fill()
		{
			Fill((int)(m_Capacity - m_Stream.Length));
		}

		/// <summary>
		///     Writes a number of 0x00 byte values to the underlying stream.
		/// </summary>
		public void Fill(int length)
		{
			if (m_Stream.Position == m_Stream.Length)
			{
				m_Stream.SetLength(m_Stream.Length + length);
				m_Stream.Seek(0, SeekOrigin.End);
			}
			else
			{
				m_Stream.Write(new byte[length], 0, length);
			}
		}

		/// <summary>
		///     Gets the total stream length.
		/// </summary>
		public long Length { get { return m_Stream.Length; } }

		/// <summary>
		///     Gets or sets the current stream position.
		/// </summary>
		public long Position { get { return m_Stream.Position; } set { m_Stream.Position = value; } }

		/// <summary>
		///     The internal stream used by this PacketWriter instance.
		/// </summary>
		public MemoryStream UnderlyingStream { get { return m_Stream; } }

		/// <summary>
		///     Offsets the current position from an origin.
		/// </summary>
		public long Seek(long offset, SeekOrigin origin)
		{
			return m_Stream.Seek(offset, origin);
		}

		/// <summary>
		///     Gets the entire stream content as a byte array.
		/// </summary>
		public byte[] ToArray()
		{
			return m_Stream.ToArray();
		}
	}
}