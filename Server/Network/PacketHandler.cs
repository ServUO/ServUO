namespace Server.Network
{
	public delegate void OnPacketReceive(NetState state, PacketReader pvSrc);

	public delegate bool ThrottlePacketCallback(NetState state, out bool drop);

	public class PacketHandler
	{
		private readonly int m_PacketID;
		private readonly int m_Length;
		private readonly bool m_Ingame;
		private readonly OnPacketReceive m_OnReceive;

		public PacketHandler(int packetID, int length, bool ingame, OnPacketReceive onReceive)
		{
			m_PacketID = packetID;
			m_Length = length;
			m_Ingame = ingame;
			m_OnReceive = onReceive;
		}

		public int PacketID => m_PacketID; 

		public int Length => m_Length; 

		public OnPacketReceive OnReceive => m_OnReceive; 

		public ThrottlePacketCallback ThrottleCallback { get; set; }

		public bool Ingame => m_Ingame; 
	}
}