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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Text;
#endregion

namespace VitaNex.Web
{
	public sealed class WebAPIClient : IDisposable
	{
		private static readonly char[] _Separators = { '\r', '\n' };
		private static readonly byte[] _EmptyBuffer = new byte[0];
		private static readonly KeyValueString[] _EmptyHeaders = new KeyValueString[0];

		private static IEnumerable<KeyValueString> ParseHeaders(IEnumerable<string> headers)
		{
			int i;

			foreach (var h in headers)
			{
				i = h.IndexOf(' ');

				if (i > 0)
				{
					var k = h.Substring(0, i).Trim().TrimEnd(':');
					var v = h.Substring(i + 1);

					yield return new KeyValueString(k, v);
				}
			}
		}

		public TcpClient Client { get; private set; }
		public NetworkStream Stream { get; private set; }

		public bool Connected => Client != null && Client.Connected;

		public bool IsDisposed { get; private set; }

		public WebAPIClient(TcpClient client)
		{
			Client = client;
			Stream = Client.GetStream();
		}

		public void Encode(Encoding enc, string data, out byte[] buffer, out int length)
		{
			buffer = enc.GetBytes(data ?? String.Empty);
			length = buffer.Length;
		}

		public void Decode(Encoding enc, byte[] buffer, int length, out string data)
		{
			if (buffer == null || buffer.Length == 0 || length <= 0)
			{
				data = String.Empty;
				return;
			}

			data = enc.GetString(buffer, 0, length);
		}

		public void Compress(ref byte[] buffer, ref int length)
		{
			using (var outS = new MemoryStream())
			{
				using (var ds = new DeflateStream(outS, CompressionMode.Compress, true))
				{
					ds.Write(buffer, 0, length);
				}

				outS.Position = 0;

				// Recycle the buffer?
				if (outS.Length <= buffer.Length)
				{
					length = outS.Read(buffer, 0, buffer.Length);

					// Heartbleed: Nope; zero-fill the remaining buffer!
					for (var i = length; i < buffer.Length; i++)
					{
						buffer[i] = 0;
					}
				}
				else
				{
					buffer = outS.ToArray();
					length = buffer.Length;
				}
			}
		}

		public void Decompress(ref byte[] buffer, ref int length)
		{
			using (MemoryStream inS = new MemoryStream(buffer, 0, length), outS = new MemoryStream())
			{
				using (var ds = new DeflateStream(inS, CompressionMode.Decompress, true))
				{
					ds.CopyTo(outS);
				}

				outS.Position = 0;

				// Recycle the buffer?
				if (outS.Length <= buffer.Length)
				{
					length = outS.Read(buffer, 0, buffer.Length);

					// Heartbleed: Nope; zero-fill the remaining buffer!
					for (var i = length; i < buffer.Length; i++)
					{
						buffer[i] = 0;
					}
				}
				else
				{
					buffer = outS.ToArray();
					length = buffer.Length;
				}
			}
		}

		public int Send(bool compress, string data, Encoding enc)
		{

			Send(compress, data, enc, out var buffer, out var length);

			return length;
		}

		public void Send(bool compress, string data, Encoding enc, out byte[] buffer, out int length)
		{
			Encode(enc, data, out buffer, out length);
			Send(compress, ref buffer, ref length);
		}

		public void Send(bool compress, ref byte[] buffer, ref int length)
		{
			if (compress)
			{
				Compress(ref buffer, ref length);
			}

			if (buffer.Length > Client.SendBufferSize)
			{
				Client.SendBufferSize = Math.Min(1048576, buffer.Length);
			}

			Stream.Write(buffer, 0, length);

			WebAPI.CSOptions.ToConsole(
				"Sent {0:#,0} bytes ({1:#,0} bytes/write)",
				length,
				Math.Min(length, Client.SendBufferSize));
		}

		private static bool Sequence(byte b, ref int seq)
		{
			switch (b)
			{
				case 13:
				{
					if (seq % 2 == 0)
					{
						++seq;
					}
					else
					{
						seq = 0;
					}
				}
				break;
				case 10:
				{
					if (seq % 2 == 1)
					{
						++seq;
					}
					else
					{
						seq = 0;
					}
				}
				break;
				default:
					seq = 0;
					break;
			}

			return seq > 0;
		}

		public bool ReceiveHeaders(out KeyValueString[] headers)
		{
			headers = _EmptyHeaders;

			var buffer = _EmptyBuffer;
			var length = 0;

			if (Stream.CanRead)
			{
				VitaNexCore.WaitWhile(() => !Stream.DataAvailable, TimeSpan.FromMilliseconds(1000));

				if (Stream.DataAvailable)
				{
					buffer = new byte[Client.ReceiveBufferSize];

					using (var ms = new MemoryStream())
					{
						int idx = 0, seq = 0;

						while (Stream.DataAvailable)
						{
							var r = Stream.ReadByte();

							if (r > -1)
							{
								if (++length > WebAPI.CSOptions.MaxReceiveBufferSizeBytes)
								{
									throw new InternalBufferOverflowException(
										String.Format("Received data exceeded {0:#,0} bytes", WebAPI.CSOptions.MaxReceiveBufferSizeBytes));
								}

								var b = (byte)r;

								buffer[idx++] = b;

								if (Sequence(b, ref seq) && seq >= 4)
								{
									break;
								}

								if (idx >= buffer.Length)
								{
									ms.Write(buffer, 0, idx);
									idx = 0;
								}
							}
						}

						if (idx > 0)
						{
							ms.Write(buffer, 0, idx);
						}

						buffer = ms.ToArray();
						length = buffer.Length;
					}
				}
			}

			WebAPI.CSOptions.ToConsole(
				"Received {0:#,0} bytes ({1:#,0} bytes/read)",
				length,
				Math.Min(length, Client.ReceiveBufferSize));

			if (length <= 0)
			{
				return false;
			}

			var raw = Encoding.ASCII.GetString(buffer, 0, length);

			if (String.IsNullOrWhiteSpace(raw))
			{
				return false;
			}

			var h = raw.Split(_Separators, StringSplitOptions.RemoveEmptyEntries);

			if (h.Length == 0)
			{
				return false;
			}

			headers = ParseHeaders(h).ToArray();

			return headers.Length > 0;
		}

		public void Receive(bool decompress, Encoding enc, out string content, out byte[] buffer, out int length)
		{
			content = String.Empty;
			buffer = _EmptyBuffer;
			length = 0;

			if (Stream.CanRead)
			{
				VitaNexCore.WaitWhile(() => !Stream.DataAvailable, TimeSpan.FromMilliseconds(1000));

				if (Stream.DataAvailable)
				{
					buffer = new byte[Client.ReceiveBufferSize];

					using (var ms = new MemoryStream())
					{
						while (Stream.DataAvailable)
						{
							length = Stream.Read(buffer, 0, buffer.Length);

							if (length > 0)
							{
								if (ms.Length + length > WebAPI.CSOptions.MaxReceiveBufferSizeBytes)
								{
									throw new InternalBufferOverflowException(
										String.Format("Received data exceeded {0:#,0} bytes", WebAPI.CSOptions.MaxReceiveBufferSizeBytes));
								}

								ms.SetLength(ms.Length + length);
								ms.Write(buffer, 0, length);
							}
						}

						buffer = ms.ToArray();
						length = buffer.Length;
					}
				}
			}

			WebAPI.CSOptions.ToConsole(
				"Received {0:#,0} bytes ({1:#,0} bytes/read)",
				length,
				Math.Min(length, Client.ReceiveBufferSize));

			if (length > 0)
			{
				if (decompress)
				{
					Decompress(ref buffer, ref length);
				}

				Decode(enc, buffer, length, out content);
			}
		}

		public void Close()
		{
			Close(false);
		}

		public void Close(bool disconnecting)
		{
			if (IsDisposed || !Client.Connected)
			{
				return;
			}

			VitaNexCore.TryCatch(
				() =>
				{
					if (!disconnecting)
					{
						WebAPI.Disconnect(this);
					}

					if (Stream != null)
					{
						using (Stream)
						{
							Stream.Close();
						}
					}

					if (Client != null)
					{
						Client.Close();
					}
				},
				WebAPI.CSOptions.ToConsole);
		}

		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			Close(false);

			IsDisposed = true;

			Client = null;
		}

		public override string ToString()
		{
			if (Client == null || Client.Client == null || Client.Client.RemoteEndPoint == null)
			{
				return "?.?.?.?:?";
			}

			return Client.Client.RemoteEndPoint.ToString();
		}
	}
}