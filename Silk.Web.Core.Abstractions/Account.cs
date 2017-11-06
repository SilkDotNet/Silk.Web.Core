using System;

namespace Silk.Web.Core
{
	/// <summary>
	/// Base account type.
	/// </summary>
	public abstract class Account : IAccountIdentifier
	{
		/// <summary>
		/// Gets the account's ID.
		/// </summary>
		public Guid Id { get; protected set; }

		/// <summary>
		/// Gets or sets a display name for the account.
		/// </summary>
		public string DisplayName { get; set; }
	}
}
