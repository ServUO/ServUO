namespace Server.Network
{
	public delegate void OnEncodedPacketReceive(NetState state, IEntity ent, EncodedReader pvSrc);

	public class EncodedPacketHandler
	{
		public EncodedPacketHandler(int packetID, bool ingame, OnEncodedPacketReceive onReceive)
		{
			PacketID = packetID;
			Ingame = ingame;
			OnReceive = onReceive;
		}

		public int PacketID { get; }
		public bool Ingame { get; }

		public OnEncodedPacketReceive OnReceive { get; }
	}
}