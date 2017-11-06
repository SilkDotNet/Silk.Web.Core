using Microsoft.Extensions.DependencyInjection;
using Silk.Data.SQL.Expressions;
using Silk.Data.SQL.Providers;
using Silk.Data.SQL.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silk.Web.Core.Data
{
	public class DefaultDatabase : IDataProvider
	{
		public const string PROVIDER_NAME = "silk-db-driver";

		private readonly IServiceProvider _serviceProvider;
		public string ProviderName => PROVIDER_NAME;

		public DefaultDatabase(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public void Dispose()
		{
		}

		private IDataProvider GetProvider(QueryExpression queryExpression)
		{
			//  todo: provide a delegate that'll handle custom provider resolution.
			return _serviceProvider.GetRequiredService<IEnumerable<IDataProvider>>()
				.Skip(1)
				.First();
		}

		public int ExecuteNonQuery(QueryExpression queryExpression)
		{
			return GetProvider(queryExpression)
				.ExecuteNonQuery(queryExpression);
		}

		public Task<int> ExecuteNonQueryAsync(QueryExpression queryExpression)
		{
			return GetProvider(queryExpression)
				.ExecuteNonQueryAsync(queryExpression);
		}

		public QueryResult ExecuteReader(QueryExpression queryExpression)
		{
			return GetProvider(queryExpression)
				.ExecuteReader(queryExpression);
		}

		public Task<QueryResult> ExecuteReaderAsync(QueryExpression queryExpression)
		{
			return GetProvider(queryExpression)
				.ExecuteReaderAsync(queryExpression);
		}
	}
}
