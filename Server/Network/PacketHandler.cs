#region Header
// **********
// ServUO - PacketHandler.cs
// **********
#endregion

namespace Server.Network
{
	public delegate void OnPacketReceive(NetState state, PacketReader pvSrc);

	public delegate bool ThrottlePacketCallback(NetState state);

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

		public int PacketID { get { return m_PacketID; } }

		public int Length { get { return m_Length; } }

		public OnPacketReceive OnReceive { get { return m_OnReceive; } }

		public ThrottlePacketCallback ThrottleCallback { get; set; }

		public bool Ingame { get { return m_Ingame; } }
	}
}