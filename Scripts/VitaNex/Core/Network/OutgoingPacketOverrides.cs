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

using Server;
using Server.Network;
#endregion

namespace VitaNex.Network
{
	public delegate void OutgoingPacketOverrideHandler(
		NetState to,
		PacketReader reader,
		ref byte[] packetBuffer,
		ref int packetLength);

	public static class OutgoingPacketOverrides
	{
		private static readonly OutgoingPacketOverrideHandler[] _Handlers;
		private static readonly OutgoingPacketOverrideHandler[] _ExtendedHandlersLow;
		private static readonly Dictionary<int, OutgoingPacketOverrideHandler> _ExtendedHandlersHigh;

		public static bool Initialized { get; private set; }

		static OutgoingPacketOverrides()
		{
			_Handlers = new OutgoingPacketOverrideHandler[0x100];
			_ExtendedHandlersLow = new OutgoingPacketOverrideHandler[0x100];
			_ExtendedHandlersHigh = new Dictionary<int, OutgoingPacketOverrideHandler>();
		}

		public static void Init()
		{
			if (Initialized)
			{
				return;
			}

			NetState.CreatedCallback += OnNetStateCreated;

			Initialized = true;
		}

		private static void OnNetStateCreated(NetState n)
		{
			n.PacketEncoder = new PacketOverrideRegistryEncoder(n.PacketEncoder);
		}

		public static void Register(int packetID, OutgoingPacketOverrideHandler handler)
		{
			_Handlers[packetID] = handler;
		}

		public static OutgoingPacketOverrideHandler GetHandler(int packetID)
		{
			return _Handlers[packetID];
		}

		public static void RegisterExtended(int packetID, OutgoingPacketOverrideHandler handler)
		{
			if (packetID >= 0 && packetID < 0x100)
			{
				_ExtendedHandlersLow[packetID] = handler;
			}
			else
			{
				_ExtendedHandlersHigh[packetID] = handler;
			}
		}

		public static OutgoingPacketOverrideHandler GetExtendedHandler(int packetID)
		{
			if (packetID >= 0 && packetID < 0x100)
			{
				return _ExtendedHandlersLow[packetID];
			}

			_ExtendedHandlersHigh.TryGetValue(packetID, out var handler);
			
			return handler;
		}

		private class PacketOverrideRegistryEncoder : IPacketEncoder
		{
			private static readonly byte[] _UnpackBuffer = new byte[0x10000];
			private readonly IPacketEncoder _Successor;

			public PacketOverrideRegistryEncoder(IPacketEncoder successor)
			{
				_Successor = successor;
			}

			public void EncodeOutgoingPacket(NetState to, ref byte[] packetBuffer, ref int packetLength)
			{
				byte[] buffer;
				byte packetId;

				if (to.CompressionEnabled)
				{
					var firstByte = Decompressor.DecompressFirstByte(packetBuffer, packetLength);

					if (!firstByte.HasValue)
					{
						Utility.PushColor(ConsoleColor.Yellow);
						Console.WriteLine("Outgoing Packet Override: Unable to decompress packet!");
						Utility.PopColor();

						return;
					}

					packetId = firstByte.Value;
				}
				else
				{
					packetId = packetBuffer[0];
				}

				var oHandler = GetHandler(packetId) ?? GetExtendedHandler(packetId);

				if (oHandler != null)
				{
					if (to.CompressionEnabled)
					{
						Decompressor.DecompressAll(packetBuffer, packetLength, _UnpackBuffer, out var bufferLength);

						buffer = new byte[bufferLength];
						Buffer.BlockCopy(_UnpackBuffer, 0, buffer, 0, bufferLength);
					}
					else
					{
						buffer = packetBuffer;
					}

					var reader = new PacketReader(buffer, packetLength, false);
					reader.Seek(0, SeekOrigin.Begin);

					oHandler(to, reader, ref packetBuffer, ref packetLength);
				}

				if (_Successor != null)
				{
					_Successor.EncodeOutgoingPacket(to, ref packetBuffer, ref packetLength);
				}
			}

			public void DecodeIncomingPacket(NetState from, ref byte[] buffer, ref int length)
			{
				if (_Successor != null)
				{
					_Successor.DecodeIncomingPacket(from, ref buffer, ref length);
				}
			}
		}
	}
}