using System;

namespace Silk.Web.Core
{
	/// <summary>
	/// Base account type.
	/// </summary>
	public class Account
	{
		/// <summary>
		/// Gets the account's ID.
		/// </summary>
		public Guid Id { get; private set; }

		/// <summary>
		/// Gets or sets a display name for the account.
		/// </summary>
		public string DisplayName { get; set; }
	}
}
