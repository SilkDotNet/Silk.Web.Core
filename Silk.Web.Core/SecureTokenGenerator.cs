using System;
using System.Security.Cryptography;
using System.Text;

namespace Silk.Web.Core
{
	public class SecureTokenGenerator : ITokenGenerator, IDisposable
	{
		private RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();
		private string _defaultCharacterPool = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

		public bool IsCryptographicallySecure => true;

		public void Dispose()
		{
			_rng.Dispose();
		}

		public string GenerateToken(int length, string characterPool = null)
		{
			if (characterPool == null)
				characterPool = _defaultCharacterPool;
			var buffer = new byte[length * 2];
			var dismissCutoff = (255 / characterPool.Length) * characterPool.Length;
			var builder = new StringBuilder();
			while (builder.Length < length)
			{
				lock (_rng)
					_rng.GetBytes(buffer);
				for (var i = 0; i < length; i++)
				{
					if (buffer[i] > dismissCutoff)
						continue;
					builder.Append(characterPool[buffer[i] % characterPool.Length]);
				}
			}
			return builder.ToString();
		}
	}
}
