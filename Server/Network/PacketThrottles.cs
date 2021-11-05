using System;

using Server.Commands;

namespace Server.Network
{
	public static class PacketThrottles
	{
		private static readonly int[] _Delays = new int[Byte.MaxValue];

		private static readonly bool[] _Reserved = new bool[Byte.MaxValue];

		static PacketThrottles()
		{
			_Delays[0x03] = 5; // speech
			_Delays[0xAD] = 5; // speech

			_Delays[0x32] = 5; // fly toggle
			_Delays[0x72] = 5; // war toggle

			_Delays[0x3B] = 100; // vendor buy response
			_Delays[0x9F] = 100; // vendor sell response

			_Delays[0xB8] = 100; // profile request
			_Delays[0x6F] = 100; // trade request
			_Delays[0x75] = 100; // rename request
			_Delays[0x9B] = 100; // help request

			_Delays[0xEC] = 100; // equip macro
			_Delays[0xED] = 100; // unequip macro

			#region Reserved

			// Reserved packets cannot be overridden by this throttle system

			_Reserved[0x02] = true; // movement request, see: PlayerMobile
			_Reserved[0x80] = true; // login request, see: AccountAttackLimiter
			_Reserved[0x91] = true; // login request, see: AccountAttackLimiter
			_Reserved[0xCF] = true; // login request, see: AccountAttackLimiter

			#endregion
		}

		public static void Configure()
		{
			EventSink.WorldLoad += () => Persistence.Deserialize("Saves/PacketThrottles.bin", Load);
			EventSink.WorldSave += e => Persistence.Serialize("Saves/PacketThrottles.bin", Save);

			CommandSystem.Register("GetThrottle", AccessLevel.Administrator, new CommandEventHandler(GetThrottle));
			CommandSystem.Register("SetThrottle", AccessLevel.Administrator, new CommandEventHandler(SetThrottle));
		}

		public static void Initialize()
		{
			for (byte i = 0; i < Byte.MaxValue; i++)
			{
				if (!_Reserved[i] && _Delays[i] > 0)
				{
					PacketHandlers.RegisterThrottler(i, HandleThrottle);
				}
			}
		}

		private static void GetThrottle(CommandEventArgs e)
		{
			if (e.Length != 1)
			{
				e.Mobile.SendMessage("Usage: {0}GetThrottle <packetID>", CommandSystem.Prefix);
				return;
			}

			var packetID = e.GetInt32(0);

			if (packetID < Byte.MinValue || packetID > Byte.MaxValue)
			{
				e.Mobile.SendMessage("Usage: PacketID must be between {0} and {1} inclusive", Byte.MinValue, Byte.MaxValue);
				return;
			}

			if (_Reserved[packetID])
			{
				e.Mobile.SendMessage("Packet 0x{0:X2} throttle is protected.");
			}
			else
			{
				e.Mobile.SendMessage("Packet 0x{0:X2} throttle is {1}ms", packetID, _Delays[packetID]);
			}
		}

		private static void SetThrottle(CommandEventArgs e)
		{
			if (e.Length < 2)
			{
				e.Mobile.SendMessage("Usage: {0}SetThrottle <packetID> <delayMS>", CommandSystem.Prefix);
				return;
			}

			var packetID = e.GetInt32(0);
			var delay = e.GetInt32(1);

			if (packetID < Byte.MinValue || packetID > Byte.MaxValue)
			{
				e.Mobile.SendMessage("Usage: PacketID must be between {0} and {1} inclusive", Byte.MinValue, Byte.MaxValue);
				return;
			}

			if (_Reserved[packetID])
			{
				e.Mobile.SendMessage("Packet 0x{0:X2} throttle is protected and can not be set");
				return;
			}

			if (delay < 0 || delay > 5000)
			{
				e.Mobile.SendMessage("Usage: Delay must be between 0 and 5000 inclusive");
				return;
			}

			SetThrottle((byte)packetID, delay);

			if (delay > 0)
			{
				e.Mobile.SendMessage("Packet 0x{0:X} throttle is {1}ms", packetID, _Delays[packetID]);
			}
			else
			{
				e.Mobile.SendMessage("Packet 0x{0:X} throttle has been removed", packetID, _Delays[packetID]);
			}
		}

		public static int GetThrottle(byte packetID)
		{
			if (!_Reserved[packetID])
			{
				return _Delays[packetID];
			}

			return 0;
		}

		public static void SetThrottle(byte packetID, int delay)
		{
			if (_Reserved[packetID])
			{
				return;
			}

			delay = Math.Max(0, Math.Min(5000, delay));

			var oldDelay = _Delays[packetID];

			if (oldDelay <= 0 && delay > 0)
			{
				PacketHandlers.RegisterThrottler(packetID, HandleThrottle);
			}
			else if (oldDelay > 0 && delay <= 0)
			{
				PacketHandlers.RegisterThrottler(packetID, null);
			}

			_Delays[packetID] = delay;
		}

		private static bool HandleThrottle(byte packetID, NetState ns, out bool drop)
		{
			drop = false;

			if (ns.Mobile == null || ns.Mobile.AccessLevel >= AccessLevel.Counselor)
			{
				return true;
			}

			if (ns.IsThrottled(packetID, _Delays[packetID]))
			{
				drop = true;
				return false;
			}

			return true;
		}

		private static void Save(GenericWriter writer)
		{
			writer.WriteEncodedInt(0);

			for (var i = 0; i < _Delays.Length; i++)
			{
				writer.WriteEncodedInt(_Delays[i]);
			}
		}

		private static void Load(GenericReader reader)
		{
			reader.ReadEncodedInt();

			for (var i = 0; i < _Delays.Length; i++)
			{
				_Delays[i] = reader.ReadEncodedInt();
			}
		}
	}
}
