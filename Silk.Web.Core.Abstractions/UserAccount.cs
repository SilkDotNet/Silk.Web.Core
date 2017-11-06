namespace Silk.Web.Core
{
	/// <summary>
	/// A user account.
	/// </summary>
	public class UserAccount : Account
	{
		/// <summary>
		/// Gets or sets the user's username.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the user's email address.
		/// </summary>
		public string EmailAddress { get; set; }
	}
}
