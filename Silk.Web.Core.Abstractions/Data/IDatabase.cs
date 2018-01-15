using Silk.Data.SQL.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silk.Web.Core.Data
{
	public interface IDatabaseBase<TThis>
		where TThis : IDatabaseBase<TThis>
	{
		TThis Insert<TBusiness>(params TBusiness[] sources)
			where TBusiness : new();
		TThis Insert<TBusiness, TProjection>(params TProjection[] sources)
			where TBusiness : new()
			where TProjection : new();

		TThis Update<TBusiness>(params TBusiness[] sources)
			where TBusiness : new();
		TThis Update<TBusiness, TProjection>(params TProjection[] sources)
			where TBusiness : new()
			where TProjection : new();

		TThis Delete<TBusiness>(params TBusiness[] sources)
			where TBusiness : new();
		TThis Delete<TBusiness, TProjection>(params TProjection[] sources)
			where TBusiness : new()
			where TProjection : new();
		TThis Delete<TBusiness>(QueryExpression where)
			where TBusiness : new();

		TThis AsTransaction();
	}

	public interface IDatabase : IDatabaseBase<IDatabase>
	{
		IDatabaseQuery<TBusiness> Select<TBusiness>(QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] orderBy = null,
			QueryExpression[] groupBy = null,
			int? offset = null,
			int? limit = null)
			where TBusiness : new();

		IDatabaseQuery<TProjection> Select<TBusiness, TProjection>(QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] orderBy = null,
			QueryExpression[] groupBy = null,
			int? offset = null,
			int? limit = null)
			where TBusiness : new()
			where TProjection : new();

		IDatabaseQuery<int> SelectCount<TBusiness>(QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] groupBy = null)
			where TBusiness : new();

		IDatabaseQuery<int> SelectCount<TBusiness, TProjection>(QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] groupBy = null)
			where TBusiness : new()
			where TProjection : new();

		void Execute();
		Task ExecuteAsync();
	}

	public interface IDatabaseQuery<TResult> : IDatabaseBase<IDatabaseQuery<TResult>>
	{
		ICollection<TResult> Execute();
		Task<ICollection<TResult>> ExecuteAsync();
	}

	public interface IDatabaseBase<TBusiness, TThis>
		where TThis : IDatabaseBase<TBusiness, TThis>
		where TBusiness : new()
	{
		TThis Insert(params TBusiness[] sources);
		TThis Insert<TProjection>(params TProjection[] sources)
			where TProjection : new();

		TThis Update(params TBusiness[] sources);
		TThis Update<TProjection>(params TProjection[] sources)
			where TProjection : new();

		TThis Delete(params TBusiness[] sources);
		TThis Delete<TProjection>(params TProjection[] sources)
			where TProjection : new();
		TThis Delete(QueryExpression where);

		TThis AsTransaction();
	}

	public interface IDatabase<TBusiness> : IDatabaseBase<TBusiness, IDatabase<TBusiness>>
		where TBusiness : new()
	{
		ITypedDatabaseQuery<TBusiness, TBusiness> Select(QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] orderBy = null,
			QueryExpression[] groupBy = null,
			int? offset = null,
			int? limit = null);

		ITypedDatabaseQuery<TBusiness, TProjection> Select<TProjection>(QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] orderBy = null,
			QueryExpression[] groupBy = null,
			int? offset = null,
			int? limit = null)
			where TProjection : new();

		ITypedDatabaseQuery<TBusiness, int> SelectCount(QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] groupBy = null);

		ITypedDatabaseQuery<TBusiness, int> SelectCount<TProjection>(QueryExpression where = null,
			QueryExpression having = null,
			QueryExpression[] groupBy = null)
			where TProjection : new();

		void Execute();
		Task ExecuteAsync();
	}

	public interface ITypedDatabaseQuery<TBusiness, TResult> : IDatabaseBase<TBusiness, ITypedDatabaseQuery<TBusiness, TResult>>
		where TBusiness : new()
	{
		ICollection<TResult> Execute();
		Task<ICollection<TResult>> ExecuteAsync();
	}
}
