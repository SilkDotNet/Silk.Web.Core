namespace Silk.Web.Core
{
	/// <summary>
	/// Account credentials.
	/// </summary>
	public interface IAccountCredentials
	{
		bool CompareCredentials(IAccountCredentials credentials);
	}
}
