using System;

namespace Server.Network
{
    public class UpdateStatueAnimation : Packet
    {
        public UpdateStatueAnimation(Mobile m, int status, int animation, int frame)
            : base(0xBF, 17)
        {
            this.m_Stream.Write((short)0x11);
            this.m_Stream.Write((short)0x19);
            this.m_Stream.Write((byte)0x5);
            this.m_Stream.Write((int)m.Serial);
            this.m_Stream.Write((byte)0);
            this.m_Stream.Write((byte)0xFF);
            this.m_Stream.Write((byte)status);
            this.m_Stream.Write((byte)0);
            this.m_Stream.Write((byte)animation);
            this.m_Stream.Write((byte)0);
            this.m_Stream.Write((byte)frame);
        }
    }
}