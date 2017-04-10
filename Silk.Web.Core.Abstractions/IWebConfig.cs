using Silk.Data.Schemas;
using System.Reflection;

namespace Silk.Web.Core.Abstractions
{
	public interface IWebConfig
	{
		TableSchema[] DatabaseTables { get; }
		MethodInfo[] ServiceInitializers { get; }
	}
}
