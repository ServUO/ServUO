using System;

using Server.Network;

namespace Server.Engines.Chat
{
    public sealed class ChatMessagePacket : Packet
    {
        public ChatMessagePacket(Mobile who, int number, string param1, string param2)
            : base(0xB2)
        {
            if (param1 == null)
                param1 = String.Empty;

            if (param2 == null)
                param2 = String.Empty;

            EnsureCapacity(13 + ((param1.Length + param2.Length) * 2));

            m_Stream.Write((ushort)(number - 20));

            if (who != null)
                m_Stream.WriteAsciiFixed(who.Language, 4);
            else
                m_Stream.Write((int)0);

            m_Stream.WriteBigUniNull(param1);
            m_Stream.WriteBigUniNull(param2);
        }
    }
}
