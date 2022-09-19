#region References
using System;
using System.Security.Cryptography;
#endregion

namespace Server.Network.Encryption
{
	public sealed class TwofishEncryption : TwofishBase, ICryptoTransform
	{
		bool ICryptoTransform.CanReuseTransform { get; } = true;
		bool ICryptoTransform.CanTransformMultipleBlocks { get; } = false;

		int ICryptoTransform.InputBlockSize { get; } = InputBlockSize;
		int ICryptoTransform.OutputBlockSize { get; } = OutputBlockSize;

		private readonly EncryptionAction m_Direction;

		public TwofishEncryption(int keyLen, byte[] key, byte[] iv, CipherMode cMode, EncryptionAction direction)
		{
			for (var i = 0; i < key.Length / 4; i++)
			{
				Key[i] = (uint)(key[i * 4 + 3] << 24) | (uint)(key[i * 4 + 2] << 16) | (uint)(key[i * 4 + 1] << 8) | key[i * 4 + 0];
			}

			CipherMode = cMode;

			if (CipherMode == CipherMode.CBC)
			{
				for (var i = 0; i < 4; i++)
				{
					IV[i] = (uint)(iv[i * 4 + 3] << 24) | (uint)(iv[i * 4 + 2] << 16) | (uint)(iv[i * 4 + 1] << 8) | iv[i * 4 + 0];
				}
			}

			m_Direction = direction;

			Rekey(keyLen, Key);
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			var keys = new uint[4];

			for (var i = 0; i < 4; i++)
			{
				keys[i] = (uint)(inputBuffer[i * 4 + 3 + inputOffset] << 24) 
						| (uint)(inputBuffer[i * 4 + 2 + inputOffset] << 16) 
						| (uint)(inputBuffer[i * 4 + 1 + inputOffset] << 8) 
						| inputBuffer[i * 4 + 0 + inputOffset];
			}

			switch (m_Direction)
			{
				case EncryptionAction.Encrypting: BlockEncrypt(keys); break;
				case EncryptionAction.Decrypting: BlockDecrypt(keys); break;
			}

			for (var i = 0; i < 4; i++)
			{
				outputBuffer[i * 4 + 0 + outputOffset] = RS(keys[i], 0);
				outputBuffer[i * 4 + 1 + outputOffset] = RS(keys[i], 1);
				outputBuffer[i * 4 + 2 + outputOffset] = RS(keys[i], 2);
				outputBuffer[i * 4 + 3 + outputOffset] = RS(keys[i], 3);
			}

			return inputCount;
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (inputCount <= 0)
			{
				return Array.Empty<byte>();
			}

			var outputBuffer = new byte[16]; // blocksize

			var keys = new uint[4];

			for (var i = 0; i < 4; i++)
			{
				keys[i] = (uint)(inputBuffer[i * 4 + 3 + inputOffset] << 24)
						| (uint)(inputBuffer[i * 4 + 2 + inputOffset] << 16)
						| (uint)(inputBuffer[i * 4 + 1 + inputOffset] << 8)
						| (uint)(inputBuffer[i * 4 + 0 + inputOffset] << 0);
			}

			switch (m_Direction)
			{
				case EncryptionAction.Encrypting: BlockEncrypt(keys); break;
				case EncryptionAction.Decrypting: BlockDecrypt(keys); break;
			}

			for (var i = 0; i < 4; i++)
			{
				outputBuffer[i * 4 + 0] = RS(keys[i], 0);
				outputBuffer[i * 4 + 1] = RS(keys[i], 1);
				outputBuffer[i * 4 + 2] = RS(keys[i], 2);
				outputBuffer[i * 4 + 3] = RS(keys[i], 3);
			}

			return outputBuffer;
		}

		void IDisposable.Dispose()
		{ }
	}
}
