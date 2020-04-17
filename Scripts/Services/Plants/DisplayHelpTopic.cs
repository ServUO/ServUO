namespace Server.Network
{
    public class DisplayHelpTopic : Packet
    {
        public DisplayHelpTopic(int topicID, bool display)
            : base(0xBF)
        {
            EnsureCapacity(11);

            m_Stream.Write((short)0x17);
            m_Stream.Write((byte)1);
            m_Stream.Write(topicID);
            m_Stream.Write(display);
        }
    }
}