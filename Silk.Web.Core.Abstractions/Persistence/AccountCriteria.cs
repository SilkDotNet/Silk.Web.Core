namespace Silk.Web.Core.Persistence
{
	public class AccountCriteria : Criteria<AccountCriteria>
	{
		public IAccountIdentifier MatchAccountIdentifier { get; set; }
		public string MatchDisplayName { get; set; }
	}
}
