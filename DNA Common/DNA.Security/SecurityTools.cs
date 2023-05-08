using System;
using System.IO;
using System.Text;
using DNA.Security.Cryptography.Crypto;
using DNA.Security.Cryptography.Crypto.Engines;
using DNA.Security.Cryptography.Crypto.IO;
using DNA.Security.Cryptography.Crypto.Parameters;

namespace DNA.Security
{
	public static class SecurityTools
	{
		public static char[] DefaultCharSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-=_+[]{}\\|:';<>,.?".ToCharArray();

		public static char[] SimpleAlphanumericCharSet = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

		public static string GeneratePassword(int length)
		{
			new Random();
			return GeneratePassword(length, DefaultCharSet);
		}

		public static string GeneratePassword(int length, char[] charset)
		{
			Random rand = new Random();
			return GeneratePassword(length, charset, rand);
		}

		public static string GeneratePassword(int length, char[] charset, Random rand)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				int num = rand.Next(charset.Length);
				stringBuilder.Append(charset[num]);
			}
			return stringBuilder.ToString();
		}

		public static byte[] EncryptData(byte[] key, byte[] data)
		{
			MemoryStream memoryStream = new MemoryStream();
			AesFastEngine aesFastEngine = new AesFastEngine();
			KeyParameter parameters = new KeyParameter(key);
			aesFastEngine.Init(true, parameters);
			BufferedBlockCipher bufferedBlockCipher = new BufferedBlockCipher(aesFastEngine);
			CipherStream cipherStream = new CipherStream(memoryStream, null, bufferedBlockCipher);
			BinaryWriter binaryWriter = new BinaryWriter(cipherStream);
			binaryWriter.Write(data.Length);
			binaryWriter.Write(data);
			binaryWriter.Flush();
			int blockSize = bufferedBlockCipher.GetBlockSize();
			int bufOff = bufferedBlockCipher.bufOff;
			int num = blockSize - bufOff % blockSize;
			for (int i = 0; i < num; i++)
			{
				binaryWriter.Write((byte)0);
			}
			cipherStream.Close();
			return memoryStream.ToArray();
		}

		public static byte[] DecryptData(byte[] key, byte[] code)
		{
			MemoryStream stream = new MemoryStream(code);
			AesFastEngine aesFastEngine = new AesFastEngine();
			KeyParameter parameters = new KeyParameter(key);
			aesFastEngine.Init(false, parameters);
			BufferedBlockCipher readCipher = new BufferedBlockCipher(aesFastEngine);
			CipherStream input = new CipherStream(stream, readCipher, null);
			BinaryReader binaryReader = new BinaryReader(input);
			int count = binaryReader.ReadInt32();
			return binaryReader.ReadBytes(count);
		}

		public static byte[] EncryptString(byte[] key, string text)
		{
			MemoryStream memoryStream = new MemoryStream();
			AesFastEngine aesFastEngine = new AesFastEngine();
			KeyParameter parameters = new KeyParameter(key);
			aesFastEngine.Init(true, parameters);
			BufferedBlockCipher bufferedBlockCipher = new BufferedBlockCipher(aesFastEngine);
			CipherStream cipherStream = new CipherStream(memoryStream, null, bufferedBlockCipher);
			BinaryWriter binaryWriter = new BinaryWriter(cipherStream);
			binaryWriter.Write(text);
			binaryWriter.Flush();
			int blockSize = bufferedBlockCipher.GetBlockSize();
			int bufOff = bufferedBlockCipher.bufOff;
			int num = blockSize - bufOff % blockSize;
			for (int i = 0; i < num; i++)
			{
				binaryWriter.Write((byte)0);
			}
			cipherStream.Close();
			return memoryStream.ToArray();
		}

		public static string DecryptString(byte[] key, byte[] code)
		{
			MemoryStream stream = new MemoryStream(code);
			AesFastEngine aesFastEngine = new AesFastEngine();
			KeyParameter parameters = new KeyParameter(key);
			aesFastEngine.Init(false, parameters);
			BufferedBlockCipher readCipher = new BufferedBlockCipher(aesFastEngine);
			CipherStream input = new CipherStream(stream, readCipher, null);
			BinaryReader binaryReader = new BinaryReader(input);
			return binaryReader.ReadString();
		}

		public static byte[] GenerateKey(string password)
		{
			throw new NotImplementedException();
		}
	}
}
