using System;

namespace Silk.Web.Core
{
	/// <summary>
	/// Identifies an account.
	/// </summary>
	public interface IAccountIdentifier
	{
		Guid Id { get; }
	}
}
