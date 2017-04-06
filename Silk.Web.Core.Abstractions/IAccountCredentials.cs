namespace Silk.Web.Core.Abstractions
{
	/// <summary>
	/// Marker interface for account credentials implementations.
	/// </summary>
	public interface IAccountCredentials
	{
		bool CompareCredentials(IAccountCredentials credentials);
	}
}
