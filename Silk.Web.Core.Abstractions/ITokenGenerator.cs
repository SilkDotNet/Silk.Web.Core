namespace Silk.Web.Core
{
	public interface ITokenGenerator
	{
		/// <summary>
		/// Gets a value indicating if the token generator uses a cryptographically secure source for random numbers.
		/// </summary>
		bool IsCryptographicallySecure { get; }

		/// <summary>
		/// Creates a new random token of the desired length, optionally using a specific pool of characters.
		/// </summary>
		/// <param name="length"></param>
		/// <param name="characterPool"></param>
		/// <returns></returns>
		string GenerateToken(int length, string characterPool = null);
	}
}
