using System.Collections.Generic;
using System.Linq;

namespace Silk.Web.Core.Data
{
	/// <summary>
	/// Page of data results.
	/// </summary>
	public abstract class PagedResult
	{
	}

	/// <summary>
	/// Page of data results.
	/// </summary>
	/// <typeparam name="TData"></typeparam>
	/// <typeparam name="TCriteria"></typeparam>
	public class PagedResult<TData, TCriteria> : PagedResult
	{
		public TData[] Items { get; }

		public PagedResult(IEnumerable<TData> items)
		{
			Items = items.ToArray();
		}
	}
}
