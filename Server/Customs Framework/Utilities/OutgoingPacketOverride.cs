#region Header
// **********
// ServUO - OutgoingPacketOverride.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;

using Server;
using Server.Network;
#endregion

namespace CustomsFramework
{
	public delegate void OutgoingPacketOverrideHandler(
		NetState to, PacketReader reader, ref byte[] packetBuffer, ref int packetLength);

	public static class OutgoingPacketOverrides
	{
		public const int CallPriority = ((byte)'r') << 16 + ((byte)'a') << 8 + ((byte)'d');
		
		private static readonly OutgoingPacketOverrideHandler[] _Handlers;
		private static readonly OutgoingPacketOverrideHandler[] _ExtendedHandlersLow;
		private static readonly Dictionary<int, OutgoingPacketOverrideHandler> _ExtendedHandlersHigh;

		static OutgoingPacketOverrides()
		{
			_Handlers = new OutgoingPacketOverrideHandler[0x100];
			_ExtendedHandlersLow = new OutgoingPacketOverrideHandler[0x100];
			_ExtendedHandlersHigh = new Dictionary<int, OutgoingPacketOverrideHandler>();
		}

		[CallPriority(CallPriority)]
		public static void Configure()
		{
			NetState.CreatedCallback += OnNetStateCreated;
		}

		public static void Register(int packetID, bool compressed, OutgoingPacketOverrideHandler handler)
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
			else
			{
				OutgoingPacketOverrideHandler handler;
				_ExtendedHandlersHigh.TryGetValue(packetID, out handler);
				return handler;
			}
		}

		private static void OnNetStateCreated(NetState n)
		{
			n.PacketEncoder = new PacketOverrideRegistryEncoder(n.PacketEncoder);
		}

		private class PacketOverrideRegistryEncoder : IPacketEncoder
		{
			private static readonly byte[] _UnpackBuffer = new byte[65535];
			private readonly IPacketEncoder _Successor;

			public PacketOverrideRegistryEncoder(IPacketEncoder successor)
			{
				_Successor = successor;
			}

			public void EncodeOutgoingPacket(NetState to, ref byte[] packetBuffer, ref int packetLength)
			{
				byte[] buffer;
				int bufferLength = 0;

				byte packetID;

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

					packetID = firstByte.Value;
				}
				else
				{
					packetID = packetBuffer[0];
				}

				OutgoingPacketOverrideHandler handler = GetHandler(packetID) ?? GetExtendedHandler(packetID);

				if (handler != null)
				{
					if (to.CompressionEnabled)
					{
						Decompressor.DecompressAll(packetBuffer, packetLength, _UnpackBuffer, ref bufferLength);

						buffer = new byte[bufferLength];
						Buffer.BlockCopy(_UnpackBuffer, 0, buffer, 0, bufferLength);
					}
					else
					{
						buffer = packetBuffer;
						bufferLength = packetLength;
					}

					handler(to, new PacketReader(buffer, packetLength, true), ref packetBuffer, ref packetLength);
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