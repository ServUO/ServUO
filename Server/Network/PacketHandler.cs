namespace Server.Network
{
	public delegate void OnPacketReceive(NetState state, PacketReader pvSrc);

	public delegate bool ThrottlePacketCallback(byte packetID, NetState state, out bool drop);

	public class PacketHandler
	{
		public int PacketID { get; }
		public int Length { get; }
		public bool Ingame { get; }
		public OnPacketReceive OnReceive { get; }

		public ThrottlePacketCallback ThrottleCallback { get; set; }

		public PacketHandler(int packetID, int length, bool ingame, OnPacketReceive onReceive)
		{
			PacketID = packetID;
			Length = length;
			Ingame = ingame;
			OnReceive = onReceive;
		}
	}
}