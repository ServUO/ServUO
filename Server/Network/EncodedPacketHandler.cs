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

		public int PacketID => m_PacketID; 

		public OnEncodedPacketReceive OnReceive => m_OnReceive; 

		public bool Ingame => m_Ingame; 
	}
}