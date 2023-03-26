#region References
using System;
#endregion

namespace Server.Network.Encryption
{
	public class LoginEncryption : IClientEncryption
	{
		private uint m_Table1, m_Table2;

		public LoginKeys Keys { get; private set; }

		public bool Init(ClientVersion v, uint seed, byte[] buffer, int offset, int length)
		{
			var keys = LoginKeys.Acquire(v);

			if (keys == LoginKeys.Empty)
			{
				return false;
			}

			var packet = new byte[length];

			Buffer.BlockCopy(buffer, offset, packet, 0, length);

			var table1 = (((~seed) ^ 0x00001357) << 16) | ((seed ^ 0xFFFFAAAA) & 0x0000FFFF);
			var table2 = ((seed ^ 0x43210000) >> 16) | (((~seed) ^ 0xABCDFFFF) & 0xFFFF0000);

			if (ValidateKey(packet, length))
			{
				m_Table1 = table1;
				m_Table2 = table2;

				Keys = keys;

				return true;
			}

			return false;
		}

		private bool ValidateKey(byte[] packet, int length)
		{
			ClientDecrypt(ref packet, ref length);

			if (packet[0] != 0x80 || packet[30] != 0 || packet[60] != 0)
			{
				return false;
			}

			for (var i = 1; i < 31; i++)
			{
				if (i < packet.Length)
				{
					if ((packet[i] == 0 || Char.IsLetterOrDigit((char)packet[i])) && (packet[i + 30] == 0 || Char.IsLetterOrDigit((char)packet[i + 30])))
					{
						continue;
					}
				}

				return false;
			}

			return true;
		}

		void IClientEncryption.ServerEncrypt(ref byte[] buffer, ref int length)
		{ }

		public void ClientDecrypt(ref byte[] buffer, ref int length)
		{
			var key1 = Keys.Key1;
			var key2 = Keys.Key2;

			uint eax, ecx, edx, esi;

			for (var i = 0; i < length && i < buffer.Length; i++)
			{
				buffer[i] = (byte)(buffer[i] ^ (byte)(m_Table1 & 0xFF));

				edx = m_Table2;
				esi = m_Table1 << 31;
				eax = m_Table2 >> 1;

				eax |= esi;
				eax ^= key1 - 1;
				edx <<= 31;
				eax >>= 1;

				ecx = m_Table1 >> 1;

				eax |= esi;
				ecx |= edx;
				eax ^= key1;
				ecx ^= key2;

				m_Table1 = ecx;
				m_Table2 = eax;
			}
		}
	}
}
