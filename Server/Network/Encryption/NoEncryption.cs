namespace Server.Network.Encryption
{
	public sealed class NoEncryption : IClientEncryption
	{
		void IClientEncryption.ServerEncrypt(ref byte[] buffer, ref int length)
		{ }

		void IClientEncryption.ClientDecrypt(ref byte[] buffer, ref int length)
		{ }
	}
}
