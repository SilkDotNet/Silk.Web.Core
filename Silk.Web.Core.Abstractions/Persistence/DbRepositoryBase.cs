using Silk.Web.Core.Data;
using System.Threading.Tasks;
using Silk.Signals;
using Silk.Data.SQL.ORM.Modelling;
using Silk.Data.SQL.Providers;
using Silk.Data.SQL.ORM;
using Silk.Data.SQL.Expressions;
using System.Collections.Generic;

namespace Silk.Web.Core.Persistence
{
	public abstract class DbRepositoryBase<TBusiness> :
		IDataDomainComponent
		where TBusiness : new()
	{
		public static EntityModel<TBusiness> DataModel { get; private set; }

		protected IDataProvider Database { get; }

		protected DbRepositoryBase(IDataProvider database)
		{
			Database = database;
		}

		public virtual void InitializeDataDomainComponent(DataDomainBuilder domainBuilder)
		{
			domainBuilder.AddDataEntity<TBusiness>(dataModel => { DataModel = dataModel; });
		}

		protected virtual async Task CreateAsync(TBusiness[] instances, AsyncSignal<TBusiness> signal = null)
		{
			await DataModel.Insert(instances)
				.ExecuteAsync(Database)
				.ConfigureAwait(false);

			if (signal != null)
			{
				foreach (var instance in instances)
				{
					await signal.InvokeAsync(instance)
						.ConfigureAwait(false);
				}
			}
		}

		protected virtual async Task CreateAsync<TView>(TView[] views, AsyncSignal<TBusiness> signal = null)
			where TView : new()
		{
			await DataModel.Insert(views)
				.ExecuteAsync(Database)
				.ConfigureAwait(false);

			if (signal != null)
			{
				//  todo: load instances and call the signal for each
				//foreach (var instance in instances)
				//{
				//	await signal.InvokeAsync(instance)
				//		.ConfigureAwait(false);
				//}
			}
		}

		protected virtual async Task UpdateAsync(TBusiness[] instances, AsyncSignal<TBusiness> signal = null)
		{
			await DataModel.Update(instances)
				.ExecuteAsync(Database)
				.ConfigureAwait(false);

			if (signal != null)
			{
				foreach (var instance in instances)
				{
					await signal.InvokeAsync(instance)
						.ConfigureAwait(false);
				}
			}
		}

		protected virtual async Task DeleteAsync(TBusiness[] instances, AsyncSignal<TBusiness> signal = null)
		{
			await DataModel.Delete(instances)
				.ExecuteAsync(Database)
				.ConfigureAwait(false);

			if (signal != null)
			{
				foreach (var instance in instances)
				{
					await signal.InvokeAsync(instance)
						.ConfigureAwait(false);
				}
			}
		}

		protected virtual Task<ICollection<TBusiness>> SelectAsync(
			QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] orderBy = null,
			QueryExpression[] groupBy = null,
			int? offset = null,
			int? limit = null
			)
		{
			return SelectAsync<TBusiness>(where, having, orderBy, groupBy, offset, limit);
		}

		protected virtual Task<ICollection<TView>> SelectAsync<TView>(
			QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] orderBy = null,
			QueryExpression[] groupBy = null,
			int? offset = null,
			int? limit = null
			)
			where TView : new()
		{
			return DataModel.Select<TView>(where, having, orderBy, groupBy, offset, limit)
				.ExecuteAsync(Database);
		}

		protected virtual async Task<long> CountAsync(
			QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] groupBy = null
			)
		{
			using (var queryResult = await Database
				.ExecuteReaderAsync(QueryExpression.Select(
					new[] { QueryExpression.CountFunction() },
					from: QueryExpression.Table(DataModel.Schema.EntityTable.TableName),
					where: where,
					having: having,
					groupBy: groupBy
					))
				.ConfigureAwait(false))
			{
				await queryResult.ReadAsync().ConfigureAwait(false);
				return queryResult.GetInt64(0);
			}
		}
	}

	/// <summary>
	/// Provides a base API for building repositories that store data in a database.
	/// </summary>
	public abstract class DbRepositoryBase<TBusiness,TDomain> :
		IDataDomainComponent
		where TBusiness : new()
		where TDomain : new()
	{
		public static EntityModel<TBusiness, TDomain> DataModel { get; private set; }

		protected IDataProvider Database { get; }

		protected DbRepositoryBase(IDataProvider database)
		{
			Database = database;
		}

		public virtual void InitializeDataDomainComponent(DataDomainBuilder domainBuilder)
		{
			domainBuilder.AddDataEntity<TBusiness, TDomain>(dataModel => { DataModel = dataModel; });
		}

		protected virtual async Task CreateAsync(TBusiness[] instances, AsyncSignal<TBusiness> signal = null)
		{
			await DataModel.Insert(instances)
				.ExecuteAsync(Database)
				.ConfigureAwait(false);

			if (signal != null)
			{
				foreach (var instance in instances)
				{
					await signal.InvokeAsync(instance)
						.ConfigureAwait(false);
				}
			}
		}

		protected virtual async Task UpdateAsync(TBusiness[] instances, AsyncSignal<TBusiness> signal = null)
		{
			await DataModel.Update(instances)
				.ExecuteAsync(Database)
				.ConfigureAwait(false);

			if (signal != null)
			{
				foreach (var instance in instances)
				{
					await signal.InvokeAsync(instance)
						.ConfigureAwait(false);
				}
			}
		}

		protected virtual async Task DeleteAsync(TBusiness[] instances, AsyncSignal<TBusiness> signal = null)
		{
			await DataModel.Delete(instances)
				.ExecuteAsync(Database)
				.ConfigureAwait(false);

			if (signal != null)
			{
				foreach (var instance in instances)
				{
					await signal.InvokeAsync(instance)
						.ConfigureAwait(false);
				}
			}
		}

		protected virtual Task<ICollection<TBusiness>> SelectAsync(
			QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] orderBy = null,
			QueryExpression[] groupBy = null,
			int? offset = null,
			int? limit = null
			)
		{
			return SelectAsync<TBusiness>(where, having, orderBy, groupBy, offset, limit);
		}

		protected virtual Task<ICollection<TView>> SelectAsync<TView>(
			QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] orderBy = null,
			QueryExpression[] groupBy = null,
			int? offset = null,
			int? limit = null
			)
			where TView : new()
		{
			return DataModel.Select<TView>(where, having, orderBy, groupBy, offset, limit)
				.ExecuteAsync(Database);
		}

		//protected virtual async Task<TBusiness> GetSingleAsync(Expression<Func<TDomain, bool>> where)
		//{
		//	//var query = new ModelledSelectBuilder<T>(DataModel)
		//	//	.Where(where)
		//	//	.Limit(1);
		//	//using (var reader = await Database.ExecuteReaderAsync(query)
		//	//	.ConfigureAwait(false))
		//	//{
		//	//	if (!reader.HasRows)
		//	//		return default(T);
		//	//	return await DataModel.MapToSourceAsync(reader.First())
		//	//		.ConfigureAwait(false);
		//	//}
		//	return default(TBusiness);
		//}

		//protected virtual async Task<TView> GetSingleAsync<TView>(Expression<Func<TDomain, bool>> where)
		//	where TView : new()
		//{
		//	//var viewModel = GetIntersectedModel<TView>();
		//	//var query = DataQuery.Select<T>(viewModel)
		//	//	.Where(where)
		//	//	.Limit(1);
		//	//using (var reader = await Database.ExecuteReaderAsync(query)
		//	//	.ConfigureAwait(false))
		//	//{
		//	//	if (!reader.HasRows)
		//	//		return default(TView);
		//	//	return await viewModel.MapToViewAsync(reader.First())
		//	//		.ConfigureAwait(false);
		//	//}
		//	return default(TView);
		//}

		//protected virtual async Task<PagedResult<TView, TCriteria>> FindAsync<TCriteria, TView>(TCriteria criteria, bool createPager, Action<ModelledSelectBuilder<T>, TCriteria, bool, ViewModel> criteriaDelegate)
		//	where TView : new()
		//{
		//	//var viewModel = GetIntersectedModel<TView>();
		//	//var selectBuilder = DataQuery.Select<T>(viewModel);

		//	//criteriaDelegate(selectBuilder, criteria, false, viewModel);

		//	//IList<TView> results;
		//	//using (var reader = (await Database.ExecuteReaderAsync(selectBuilder)
		//	//	.ConfigureAwait(false)))
		//	//{
		//	//	results = await viewModel.MapToViewAsync(reader)
		//	//		.ConfigureAwait(false);
		//	//}

		//	//if (createPager)
		//	//{
		//	//	selectBuilder = new ModelledSelectBuilder<T>(DataModel);
		//	//	criteriaDelegate(selectBuilder, criteria, true, viewModel);
		//	//}

		//	//return new PagedResult<TView, TCriteria>(results);
		//	return null;
		//}

		//protected virtual async Task<PagedResult<T,TCriteria>> FindAsync<TCriteria>(TCriteria criteria, bool createPager, Action<ModelledSelectBuilder<T>, TCriteria, bool, ViewModel> criteriaDelegate)
		//{
		//	//var selectBuilder = new ModelledSelectBuilder<T>(DataModel);
		//	//criteriaDelegate(selectBuilder, criteria, false, DataModel);

		//	//IList<T> results;
		//	//using (var reader = (await Database.ExecuteReaderAsync(selectBuilder)
		//	//	.ConfigureAwait(false)))
		//	//{
		//	//	results = await DataModel.MapToSourceAsync(reader)
		//	//		.ConfigureAwait(false);
		//	//}

		//	//if (createPager)
		//	//{
		//	//	selectBuilder = new ModelledSelectBuilder<T>(DataModel);
		//	//	criteriaDelegate(selectBuilder, criteria, true, DataModel);
		//	//}

		//	//return new PagedResult<T, TCriteria>(results);
		//	return null;
		//}
	}
}
