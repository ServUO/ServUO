using System;
using Server.Network;

namespace Server.Engines.PartySystem
{
    public sealed class PartyEmptyList : Packet
    {
        public PartyEmptyList(Mobile m)
            : base(0xBF)
        {
            this.EnsureCapacity(7);

            this.m_Stream.Write((short)0x0006);
            this.m_Stream.Write((byte)0x02);
            this.m_Stream.Write((byte)0);
            this.m_Stream.Write((int)m.Serial);
        }
    }

    public sealed class PartyMemberList : Packet
    {
        public PartyMemberList(Party p)
            : base(0xBF)
        {
            this.EnsureCapacity(7 + p.Count * 4);

            this.m_Stream.Write((short)0x0006);
            this.m_Stream.Write((byte)0x01);
            this.m_Stream.Write((byte)p.Count);

            for (int i = 0; i < p.Count; ++i)
                this.m_Stream.Write((int)p[i].Mobile.Serial);
        }
    }

    public sealed class PartyRemoveMember : Packet
    {
        public PartyRemoveMember(Mobile removed, Party p)
            : base(0xBF)
        {
            this.EnsureCapacity(11 + p.Count * 4);

            this.m_Stream.Write((short)0x0006);
            this.m_Stream.Write((byte)0x02);
            this.m_Stream.Write((byte)p.Count);

            this.m_Stream.Write((int)removed.Serial);

            for (int i = 0; i < p.Count; ++i)
                this.m_Stream.Write((int)p[i].Mobile.Serial);
        }
    }

    public sealed class PartyTextMessage : Packet
    {
        public PartyTextMessage(bool toAll, Mobile from, string text)
            : base(0xBF)
        {
            if (text == null)
                text = "";

            this.EnsureCapacity(12 + text.Length * 2);

            this.m_Stream.Write((short)0x0006);
            this.m_Stream.Write((byte)(toAll ? 0x04 : 0x03));
            this.m_Stream.Write((int)from.Serial);
            this.m_Stream.WriteBigUniNull(text);
        }
    }

    public sealed class PartyInvitation : Packet
    {
        public PartyInvitation(Mobile leader)
            : base(0xBF)
        {
            this.EnsureCapacity(10);

            this.m_Stream.Write((short)0x0006);
            this.m_Stream.Write((byte)0x07);
            this.m_Stream.Write((int)leader.Serial);
        }
    }
}