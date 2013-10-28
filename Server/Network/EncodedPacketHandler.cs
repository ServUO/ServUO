#region Header
// **********
// ServUO - EncodedPacketHandler.cs
// **********
#endregion

namespace Server.Network
{
	public delegate void OnEncodedPacketReceive(NetState state, IEntity ent, EncodedReader pvSrc);

	public class EncodedPacketHandler
	{
		private readonly int m_PacketID;
		private readonly bool m_Ingame;
		private readonly OnEncodedPacketReceive m_OnReceive;

		public EncodedPacketHandler(int packetID, bool ingame, OnEncodedPacketReceive onReceive)
		{
			m_PacketID = packetID;
			m_Ingame = ingame;
			m_OnReceive = onReceive;
		}

		public int PacketID { get { return m_PacketID; } }

		public OnEncodedPacketReceive OnReceive { get { return m_OnReceive; } }

		public bool Ingame { get { return m_Ingame; } }
	}
}