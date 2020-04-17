namespace Server.Network
{
    public class UpdateStatueAnimation : Packet
    {
        public UpdateStatueAnimation(Mobile m, int status, int animation, int frame)
            : base(0xBF, 17)
        {
            m_Stream.Write((short)0x11);
            m_Stream.Write((short)0x19);
            m_Stream.Write((byte)0x5);
            m_Stream.Write(m.Serial);
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)0xFF);
            m_Stream.Write((byte)status);
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)animation);
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)frame);
        }
    }
}