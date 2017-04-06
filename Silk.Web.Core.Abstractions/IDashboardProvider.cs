using System.Collections.Generic;

namespace Silk.Web.Core.Abstractions
{
	public interface IDashboardProvider
	{
		IEnumerable<Dashboard> Dashboards { get; }
	}
}
