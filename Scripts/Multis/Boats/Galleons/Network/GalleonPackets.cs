using Server.Multis;

namespace Server.Network
{
    public sealed class BoatDriverLock : Packet
    {
        public BoatDriverLock(Mobile m) : base(0xBF)
        {
            short length = 19;
            EnsureCapacity(length);

            length = (short)(((length & 0xff) << 8) | ((length >> 8) & 0xff));
            m_Stream.Seek(1, System.IO.SeekOrigin.Begin);
            m_Stream.Write(length);

            m_Stream.Write((short)0x19);
            m_Stream.Write((byte)0x05);
            m_Stream.Write(m.Serial);
            m_Stream.Write((byte)0x00);

            m_Stream.Write((byte)0xFF);
            m_Stream.Write((byte)0x00);
            m_Stream.Write((short)0x04);
            m_Stream.Write((short)0x00);
            m_Stream.Write((byte)0x00);
            m_Stream.Write((byte)0x00);
        }
    }

    public sealed class BoatEquipPacket : Packet
    {
        public BoatEquipPacket(Mobile to, BaseBoat boat)
            : base(0x2E, 15)
        {
            Serial parentSerial = to.Serial;

            int hue = boat.Hue;

            m_Stream.Write(boat.Serial);
            m_Stream.Write((short)boat.ItemID);
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)boat.Layer);
            m_Stream.Write(parentSerial);
            m_Stream.Write((short)hue);
        }
    }
}