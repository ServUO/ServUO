#region References
using System;
using System.Security.Cryptography;

using Server.Network;
#endregion

namespace Server.Network.Encryption
{
	public class GameEncryption : IClientEncryption
	{
		private readonly TwofishEncryption m_Encryption;

		private readonly byte[] m_CipherTable, m_XorData;

		private ushort m_RecvPos;
		private byte m_SendPos;

		public GameEncryption(uint seed)
		{
			m_CipherTable = new byte[256];

			var key = new byte[16];

			key[0] = key[4] = key[8] = key[12] = (byte)((seed >> 24) & 0xFF);
			key[1] = key[5] = key[9] = key[13] = (byte)((seed >> 16) & 0xFF);
			key[2] = key[6] = key[10] = key[14] = (byte)((seed >> 8) & 0xFF);
			key[3] = key[7] = key[11] = key[15] = (byte)(seed & 0xFF);

			var iv = new byte[0];

			m_Encryption = new TwofishEncryption(128, key, iv, CipherMode.ECB, EncryptionAction.Decrypting);

			for (var i = 0; i < m_CipherTable.Length; ++i)
			{
				m_CipherTable[i] = (byte)i;
			}

			RefreshCipherTable();

			using (var md5 = MD5.Create())
			{
				m_XorData = md5.ComputeHash(m_CipherTable);
			}
		}

		private void RefreshCipherTable()
		{
			var block = new uint[4];

			for (var i = 0; i < 256; i += 16)
			{
				Buffer.BlockCopy(m_CipherTable, i, block, 0, 16);

				m_Encryption.BlockEncrypt(block);

				Buffer.BlockCopy(block, 0, m_CipherTable, i, 16);
			}

			m_RecvPos = 0;
		}

		public void ServerEncrypt(ref byte[] buffer, ref int length)
		{
			if (buffer == null || length <= 0)
			{
				return;
			}

			byte[] packet;

			if (NetState.BufferStaticPackets)
			{
				packet = buffer;
			}
			else if (length <= NetState.SendBufferSize)
			{
				packet = NetState.SendBuffers.AcquireBuffer();
			}
			else
			{
				packet = new byte[length];
			}

			for (var i = 0; i < length && i < buffer.Length; i++)
			{
				packet[i] = (byte)(buffer[i] ^ m_XorData[m_SendPos++]);

				m_SendPos &= 0x0F;
			}

			buffer = packet;
		}

		public void ClientDecrypt(ref byte[] buffer, ref int length)
		{
			if (buffer == null || length <= 0)
			{
				return;
			}

			for (var i = 0; i < length && i < buffer.Length; i++)
			{
				if (m_RecvPos >= m_CipherTable.Length)
				{
					RefreshCipherTable();
				}

				buffer[i] ^= m_CipherTable[m_RecvPos++];
			}
		}
	}
}
