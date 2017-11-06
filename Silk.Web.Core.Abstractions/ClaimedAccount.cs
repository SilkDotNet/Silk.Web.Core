using System;

namespace Silk.Web.Core
{
	public class ClaimedAccount : Account
	{
		public ClaimedAccount(Guid id, string displayName)
		{
			Id = id;
			DisplayName = displayName;
		}
	}
}
