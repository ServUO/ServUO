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

#if ServUO58
#define ServUOX
#endif

#region References
using System;

using Server.Network;
#endregion

namespace VitaNex.Network
{
	public static class PacketExtUtility
	{
#if ServUOX
		public static bool RewriteItemID(this WorldItem p, int itemID, bool reset = true)
		{
			if (p == null)
			{
				return false;
			}

			int offset;

			if (p.PacketID == 0x1A)
			{
				offset = 7;
			}
			else
			{
				offset = 8;
			}

			return Rewrite(p, offset, (ushort)itemID, reset);
		}

		public static bool RewriteBody(this MobileIncoming p, int itemID, bool reset = true)
		{
			return Rewrite(p, 7, (short)itemID, reset);
		}

		public static bool RewriteHue(this MobileIncoming p, int hue, bool reset = true)
		{
			return Rewrite(p, 15, (short)hue, reset);
		}
#else
		public static bool RewriteItemID(this WorldItem p, int itemID, bool reset = true)
		{
			return Rewrite(p, 7, (short)itemID, reset);
		}

		public static bool RewriteItemID(this WorldItemSA p, int itemID, bool reset = true)
		{
			return Rewrite(p, 8, (short)itemID, reset);
		}

		public static bool RewriteItemID(this WorldItemHS p, int itemID, bool reset = true)
		{
			return Rewrite(p, 8, (short)itemID, reset);
		}

		public static bool RewriteBody(this MobileIncomingOld p, int itemID, bool reset = true)
		{
			return Rewrite(p, 7, (short)itemID, reset);
		}

		public static bool RewriteHue(this MobileIncomingOld p, int hue, bool reset = true)
		{
			return Rewrite(p, 15, (short)hue, reset);
		}

		public static bool RewriteBody(this MobileIncoming p, int itemID, bool reset = true)
		{
			return Rewrite(p, 7, (short)itemID, reset);
		}

		public static bool RewriteHue(this MobileIncoming p, int hue, bool reset = true)
		{
			return Rewrite(p, 15, (short)hue, reset);
		}

		public static bool RewriteBody(this MobileIncomingSA p, int itemID, bool reset = true)
		{
			return Rewrite(p, 7, (short)itemID, reset);
		}

		public static bool RewriteHue(this MobileIncomingSA p, int hue, bool reset = true)
		{
			return Rewrite(p, 15, (short)hue, reset);
		}
#endif

		public static bool Rewrite(this Packet packet, int offset, bool value, bool reset = true)
		{
			var writer = GetWriter(packet);

			return writer != null && Rewrite(writer, offset, reset, writer.Write, value);
		}

		public static bool Rewrite(this Packet packet, int offset, sbyte value, bool reset = true)
		{
			var writer = GetWriter(packet);

			return writer != null && Rewrite(writer, offset, reset, writer.Write, value);
		}

		public static bool Rewrite(this Packet packet, int offset, byte value, bool reset = true)
		{
			var writer = GetWriter(packet);

			return writer != null && Rewrite(writer, offset, reset, writer.Write, value);
		}

		public static bool Rewrite(this Packet packet, int offset, short value, bool reset = true)
		{
			var writer = GetWriter(packet);

			return writer != null && Rewrite(writer, offset, reset, writer.Write, value);
		}

		public static bool Rewrite(this Packet packet, int offset, ushort value, bool reset = true)
		{
			var writer = GetWriter(packet);

			return writer != null && Rewrite(writer, offset, reset, writer.Write, value);
		}

		public static bool Rewrite(this Packet packet, int offset, int value, bool reset = true)
		{
			var writer = GetWriter(packet);

			return writer != null && Rewrite(writer, offset, reset, writer.Write, value);
		}

		public static bool Rewrite(this Packet packet, int offset, uint value, bool reset = true)
		{
			var writer = GetWriter(packet);

			return writer != null && Rewrite(writer, offset, reset, writer.Write, value);
		}

		private static bool Rewrite<T>(PacketWriter writer, int offset, bool reset, Action<T> write, T value)
		{
			var pos = -1L;

			try
			{
				pos = writer.Position;

				writer.Position = offset;

				write(value);

				return true;
			}
			catch
			{
			}
			finally
			{
				if (reset && pos >= 0)
				{
					writer.Position = pos;
				}
			}

			return false;
		}

		public static byte[] GetBuffer(this Packet packet)
		{
			var writer = GetWriter(packet);

			if (writer != null)
			{
				return writer.ToArray();
			}

			if (GetCompiledBuffer(packet, out var buffer))
			{
				return buffer;
			}

			return Array.Empty<byte>();
		}

		public static bool GetCompiledBuffer(this Packet packet, out byte[] buffer)
		{
			return packet.GetFieldValue("m_CompiledBuffer", out buffer);
		}

		public static bool SetCompiledBuffer(this Packet packet, byte[] buffer)
		{
			return packet.SetFieldValue("m_CompiledBuffer", buffer);
		}

		public static PacketWriter GetWriter(this Packet packet)
		{
#if ServUOX
			return packet?.Stream;
#else
			return packet?.UnderlyingStream;
#endif
		}
	}
}
