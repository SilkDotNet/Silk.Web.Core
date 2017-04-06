namespace Silk.Web.Core.Abstractions
{
	public class PagedResult<TProjection, TCriteria>
	{
		public TCriteria Criteria { get; }
		public TProjection[] Records { get; }

		public PagedResult(TCriteria criteria, TProjection[] records)
		{
			Criteria = criteria;
			Records = records;
		}
	}
}
