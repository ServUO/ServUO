namespace Server.Network
{
    public class DisplayHelpTopic : Packet
    {
        public DisplayHelpTopic(int topicID, bool display)
            : base(0xBF)
        {
            this.EnsureCapacity(11);

            this.m_Stream.Write((short)0x17);
            this.m_Stream.Write((byte)1);
            this.m_Stream.Write(topicID);
            this.m_Stream.Write(display);
        }
    }
}