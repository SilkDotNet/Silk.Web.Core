using Silk.Web.Core.Data;
using System.Threading.Tasks;
using Silk.Signals;
using Silk.Data.SQL.ORM.Modelling;
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

		protected IDatabase<TBusiness> Database { get; }

		protected DbRepositoryBase(IDatabase<TBusiness> database)
		{
			Database = database;
		}

		public virtual void InitializeDataDomainComponent(DataDomainBuilder domainBuilder)
		{
			domainBuilder.AddDataEntity<TBusiness>(
				dataModel => { DataModel = dataModel; },
				CustomizeModel
				);
		}

		protected virtual void CustomizeModel(ModelCustomizer<TBusiness> modelCustomizer)
		{
		}

		protected virtual async Task CreateAsync(TBusiness[] instances, AsyncSignal<TBusiness> signal = null)
		{
			await Database
				.Insert(instances)
				.AsTransaction()
				.ExecuteAsync()
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
			await Database
				.Insert(views)
				.AsTransaction()
				.ExecuteAsync()
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
			await Database
				.Update(instances)
				.AsTransaction()
				.ExecuteAsync()
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
			await Database
				.Delete(instances)
				.AsTransaction()
				.ExecuteAsync()
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
			return Database
				.Select<TView>(where, having, orderBy, groupBy, offset, limit)
				.ExecuteAsync();
		}

		protected virtual Task<ICollection<int>> CountAsync(
			QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] groupBy = null
			)
		{
			return Database
				.SelectCount(where, having, groupBy)
				.ExecuteAsync();
		}

		protected virtual Task<ICollection<int>> CountAsync<TView>(
			QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] groupBy = null
			)
			where TView : new()
		{
			return Database
				.SelectCount<TView>(where, having, groupBy)
				.ExecuteAsync();
		}
	}
}
