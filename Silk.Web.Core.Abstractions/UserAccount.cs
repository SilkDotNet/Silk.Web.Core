using System;

namespace Silk.Web.Core.Abstractions
{
	public class UserAccount : IAccount
	{
		public Guid AccountId { get; private set; }

		public string DisplayName { get; set; }
		public string Slug { get; set; }

		public UserAccount() { }

		public UserAccount(Guid accountId)
		{
			AccountId = accountId;
		}
	}
}
