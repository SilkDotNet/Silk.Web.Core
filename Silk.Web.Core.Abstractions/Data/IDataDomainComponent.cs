using Silk.Data.SQL.ORM;

namespace Silk.Web.Core.Data
{
	public interface IDataDomainComponent
	{
		void InitializeDataDomainComponent(DataDomainBuilder domainBuilder);
	}
}
