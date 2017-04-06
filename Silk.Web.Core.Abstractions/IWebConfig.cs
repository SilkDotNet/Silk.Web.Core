using Silk.Data.Schemas;

namespace Silk.Web.Core.Abstractions
{
	public interface IWebConfig
	{
		TableSchema[] DatabaseTables { get; }
	}
}
