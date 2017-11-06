namespace Silk.Web.Core.Data
{
	public abstract class PagedCriteria<T> where T : PagedCriteria<T>
	{
		private T _instance;

		public int? FilterOffset { get; protected set; }
		public int? FilterLimit { get; protected set; }

		protected PagedCriteria()
		{
			_instance = this as T;
		}

		public T Offset(int offset)
		{
			FilterOffset = offset;
			return _instance;
		}

		public T Limit(int limit)
		{
			FilterLimit = limit;
			return _instance;
		}

		public T Page(int pageNumber, int resultsPerPage = 10)
		{
			return Offset((pageNumber - 1) * resultsPerPage)
				.Limit(resultsPerPage);
		}
	}
}
