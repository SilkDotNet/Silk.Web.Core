using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Silk.Data.SQL.Expressions;
using Silk.Data.SQL.ORM;
using Silk.Data.SQL.ORM.Queries;
using Silk.Data.SQL.Providers;

namespace Silk.Web.Core.Data
{
	public class SilkORMRootDatabase : IDatabase
	{
		private readonly DataDomain _dataDomain;
		private readonly IDataProvider _dataProvider;

		public SilkORMRootDatabase(DataDomain dataDomain, IDataProvider dataProvider)
		{
			_dataDomain = dataDomain;
			_dataProvider = dataProvider;
		}

		public IDatabase AsTransaction()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.AsTransaction();
		}

		public IDatabase Delete<TBusiness>(params TBusiness[] sources) where TBusiness : new()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.Delete<TBusiness>(sources);
		}

		public IDatabase Delete<TBusiness, TProjection>(params TProjection[] sources)
			where TBusiness : new()
			where TProjection : new()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.Delete<TBusiness, TProjection>(sources);
		}

		public IDatabase Delete<TBusiness>(QueryExpression where) where TBusiness : new()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.Delete<TBusiness>(where);
		}

		public void Execute()
		{
			throw new InvalidOperationException();
		}

		public Task ExecuteAsync()
		{
			throw new InvalidOperationException();
		}

		public IDatabase Insert<TBusiness>(params TBusiness[] sources) where TBusiness : new()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.Insert<TBusiness>(sources);
		}

		public IDatabase Insert<TBusiness, TProjection>(params TProjection[] sources)
			where TBusiness : new()
			where TProjection : new()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.Insert<TBusiness,TProjection>(sources);
		}

		public IDatabaseQuery<TBusiness> Select<TBusiness>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] orderBy = null, QueryExpression[] groupBy = null, int? offset = null, int? limit = null) where TBusiness : new()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.Select<TBusiness>(where, having, orderBy, groupBy, offset, limit);
		}

		public IDatabaseQuery<TProjection> Select<TBusiness, TProjection>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] orderBy = null, QueryExpression[] groupBy = null, int? offset = null, int? limit = null)
			where TBusiness : new()
			where TProjection : new()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.Select<TBusiness, TProjection>(where, having, orderBy, groupBy, offset, limit);
		}

		public IDatabaseQuery<int> SelectCount<TBusiness>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] groupBy = null) where TBusiness : new()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.SelectCount<TBusiness>(where, having, groupBy);
		}

		public IDatabaseQuery<int> SelectCount<TBusiness, TProjection>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] groupBy = null)
			where TBusiness : new()
			where TProjection : new()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.SelectCount<TBusiness, TProjection>(where, having, groupBy);
		}

		public IDatabase Update<TBusiness>(params TBusiness[] sources) where TBusiness : new()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.Update<TBusiness>(sources);
		}

		public IDatabase Update<TBusiness, TProjection>(params TProjection[] sources)
			where TBusiness : new()
			where TProjection : new()
		{
			return new SilkORMDatabase(_dataDomain, _dataProvider)
				.Update<TBusiness,TProjection>(sources);
		}
	}

	public class SilkORMRootDatabase<T> : IDatabase<T>
		where T : new()
	{
		private readonly DataDomain _dataDomain;
		private readonly IDataProvider _dataProvider;

		public SilkORMRootDatabase(DataDomain dataDomain, IDataProvider dataProvider)
		{
			_dataDomain = dataDomain;
			_dataProvider = dataProvider;
		}

		public IDatabase<T> AsTransaction()
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.AsTransaction();
		}

		public IDatabase<T> Delete(params T[] sources)
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.Delete(sources);
		}

		public IDatabase<T> Delete<TProjection>(params TProjection[] sources) where TProjection : new()
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.Delete<TProjection>(sources);
		}

		public IDatabase<T> Delete(QueryExpression where)
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.Delete(where);
		}

		public void Execute()
		{
			throw new InvalidOperationException();
		}

		public Task ExecuteAsync()
		{
			throw new InvalidOperationException();
		}

		public IDatabase<T> Insert(params T[] sources)
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.Insert(sources);
		}

		public IDatabase<T> Insert<TProjection>(params TProjection[] sources) where TProjection : new()
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.Insert<TProjection>(sources);
		}

		public ITypedDatabaseQuery<T, T> Select(QueryExpression where = null, QueryExpression having = null, QueryExpression[] orderBy = null, QueryExpression[] groupBy = null, int? offset = null, int? limit = null)
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.Select(where, having, orderBy, groupBy, offset, limit);
		}

		public ITypedDatabaseQuery<T, TProjection> Select<TProjection>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] orderBy = null, QueryExpression[] groupBy = null, int? offset = null, int? limit = null) where TProjection : new()
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.Select<TProjection>(where, having, orderBy, groupBy, offset, limit);
		}

		public ITypedDatabaseQuery<T, int> SelectCount(QueryExpression where = null, QueryExpression having = null, QueryExpression[] groupBy = null)
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.SelectCount(where, having, groupBy);
		}

		public ITypedDatabaseQuery<T, int> SelectCount<TProjection>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] groupBy = null) where TProjection : new()
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.SelectCount<TProjection>(where, having, groupBy);
		}

		public IDatabase<T> Update(params T[] sources)
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.Update(sources);
		}

		public IDatabase<T> Update<TProjection>(params TProjection[] sources) where TProjection : new()
		{
			return new SilkTypedORMDatabase<T>(_dataProvider, _dataDomain)
				.Update<TProjection>(sources);
		}
	}

	public abstract class SilkORMDatabaseBase<TThisInterface, TThisImpl, TQueryCollection>
		where TThisInterface : class
		where TThisImpl : SilkORMDatabaseBase<TThisInterface, TThisImpl, TQueryCollection>, TThisInterface
		where TQueryCollection : QueryCollectionBase<TQueryCollection>
	{
		protected IDataProvider DataProvider { get; }
		protected DataDomain DataDomain { get; }
		protected TThisImpl Self { get; }
		protected TQueryCollection Queries { get; set; }

		protected SilkORMDatabaseBase(DataDomain dataDomain, IDataProvider dataProvider,
			TQueryCollection queryCollection)
		{
			Self = this as TThisImpl;
			Queries = queryCollection;
			DataDomain = dataDomain;
			DataProvider = dataProvider;
		}

		public TThisInterface Insert<TBusiness>(params TBusiness[] sources)
			where TBusiness : new()
		{
			Queries = Queries.Insert<TBusiness>(sources);
			return Self;
		}

		public TThisInterface Insert<TBusiness, TEntity>(params TEntity[] sources)
			where TBusiness : new()
			where TEntity : new()
		{
			Queries = Queries.Insert<TBusiness, TEntity>(sources);
			return Self;
		}

		public TThisInterface Update<TBusiness>(params TBusiness[] sources)
			where TBusiness : new()
		{
			Queries = Queries.Update<TBusiness>(sources);
			return Self;
		}

		public TThisInterface Update<TBusiness, TEntity>(params TEntity[] sources)
			where TBusiness : new()
			where TEntity : new()
		{
			Queries = Queries.Update<TBusiness, TEntity>(sources);
			return Self;
		}

		public TThisInterface Delete<TBusiness>(params TBusiness[] sources)
			where TBusiness : new()
		{
			Queries = Queries.Delete<TBusiness>(sources);
			return Self;
		}

		public TThisInterface Delete<TBusiness, TEntity>(params TEntity[] sources)
			where TBusiness : new()
			where TEntity : new()
		{
			Queries = Queries.Delete<TBusiness, TEntity>(sources);
			return Self;
		}

		public TThisInterface Delete<TBusiness>(QueryExpression where)
			where TBusiness : new()
		{
			Queries = Queries.Delete<TBusiness>(where);
			return Self;
		}

		public TThisInterface AsTransaction()
		{
			Queries = Queries.AsTransaction();
			return Self;
		}
	}

	public class SilkORMDatabase : SilkORMDatabaseBase<IDatabase, SilkORMDatabase, QueryCollection>, IDatabase
	{
		protected SilkORMDatabase(DataDomain dataDomain, IDataProvider dataProvider, QueryCollection queries)
			: base(dataDomain, dataProvider, queries)
		{
		}

		public SilkORMDatabase(DataDomain dataDomain, IDataProvider dataProvider)
			: base(dataDomain, dataProvider, new QueryCollection(dataDomain))
		{
		}

		public void Execute()
		{
			Queries.Execute(DataProvider);
		}

		public Task ExecuteAsync()
		{
			return Queries.ExecuteAsync(DataProvider);
		}

		public IDatabaseQuery<TBusiness> Select<TBusiness>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] orderBy = null, QueryExpression[] groupBy = null, int? offset = null, int? limit = null) where TBusiness : new()
		{
			return new SilkORMDatabase<TBusiness>(DataDomain, DataProvider, Queries.Select<TBusiness>(where, having, orderBy, groupBy, offset, limit));
		}

		public IDatabaseQuery<TProjection> Select<TBusiness, TProjection>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] orderBy = null, QueryExpression[] groupBy = null, int? offset = null, int? limit = null)
			where TBusiness : new()
			where TProjection : new()
		{
			return new SilkORMDatabase<TProjection>(DataDomain, DataProvider, Queries.Select<TBusiness, TProjection>(where, having, orderBy, groupBy, offset, limit));
		}

		public IDatabaseQuery<int> SelectCount<TBusiness>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] groupBy = null) where TBusiness : new()
		{
			return new SilkORMDatabase<int>(DataDomain, DataProvider, Queries.SelectCount<TBusiness>(where, having, groupBy));
		}

		public IDatabaseQuery<int> SelectCount<TBusiness, TProjection>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] groupBy = null)
			where TBusiness : new()
			where TProjection : new()
		{
			return new SilkORMDatabase<int>(DataDomain, DataProvider, Queries.SelectCount<TBusiness, TProjection>(where, having, groupBy));
		}
	}

	public class SilkORMDatabase<TResult> : SilkORMDatabaseBase<IDatabaseQuery<TResult>, SilkORMDatabase<TResult>, QueryCollection<TResult>>, IDatabaseQuery<TResult>
	{
		public SilkORMDatabase(DataDomain dataDomain, IDataProvider dataProvider, QueryCollection<TResult> queryCollection)
			: base(dataDomain, dataProvider, queryCollection)
		{
		}

		public ICollection<TResult> Execute()
		{
			return Queries.Execute(DataProvider);
		}

		public Task<ICollection<TResult>> ExecuteAsync()
		{
			return Queries.ExecuteAsync(DataProvider);
		}
	}

	public abstract class SilkORMDatabaseBase<TBusiness, TThisInterface, TThisImpl, TQueryCollection> : SilkORMDatabaseBase<TThisInterface, TThisImpl, TQueryCollection>
		where TThisInterface : class
		where TThisImpl : SilkORMDatabaseBase<TThisInterface, TThisImpl, TQueryCollection>, TThisInterface
		where TBusiness : new()
		where TQueryCollection : QueryCollectionBase<TQueryCollection>
	{
		protected SilkORMDatabaseBase(DataDomain dataDomain, IDataProvider dataProvider, TQueryCollection queries)
			: base(dataDomain, dataProvider, queries)
		{
		}

		public TThisInterface Insert(params TBusiness[] sources)
		{
			return base.Insert<TBusiness>(sources);
		}

		public new TThisInterface Insert<TEntity>(params TEntity[] sources)
			where TEntity : new()
		{
			return base.Insert<TBusiness, TEntity>(sources);
		}

		public TThisInterface Update(params TBusiness[] sources)
		{
			return base.Update<TBusiness>(sources);
		}

		public new TThisInterface Update<TEntity>(params TEntity[] sources)
			where TEntity : new()
		{
			return base.Update<TBusiness, TEntity>(sources);
		}

		public TThisInterface Delete(params TBusiness[] sources)
		{
			return base.Delete<TBusiness>(sources);
		}

		public new TThisInterface Delete<TEntity>(params TEntity[] sources)
			where TEntity : new()
		{
			return base.Delete<TBusiness, TEntity>(sources);
		}

		public TThisInterface Delete(QueryExpression where)
		{
			return base.Delete<TBusiness>(where);
		}
	}

	public class SilkTypedORMDatabase<TBusiness> : SilkORMDatabaseBase<TBusiness, IDatabase<TBusiness>, SilkTypedORMDatabase<TBusiness>, QueryCollection>, IDatabase<TBusiness>
		where TBusiness : new()
	{
		public SilkTypedORMDatabase(IDataProvider dataProvider, DataDomain dataDomain)
			: base(dataDomain, dataProvider, new QueryCollection(dataDomain))
		{
		}

		public void Execute()
		{
			Queries.Execute(DataProvider);
		}

		public Task ExecuteAsync()
		{
			return Queries.ExecuteAsync(DataProvider);
		}

		public ITypedDatabaseQuery<TBusiness, TBusiness> Select(QueryExpression where = null, QueryExpression having = null, QueryExpression[] orderBy = null, QueryExpression[] groupBy = null, int? offset = null, int? limit = null)
		{
			return new SilkTypedORMDatabase<TBusiness, TBusiness>(DataDomain, DataProvider,
				Queries.Select<TBusiness>(where, having, orderBy, groupBy, offset, limit));
		}

		public ITypedDatabaseQuery<TBusiness, TProjection> Select<TProjection>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] orderBy = null, QueryExpression[] groupBy = null, int? offset = null, int? limit = null) where TProjection : new()
		{
			return new SilkTypedORMDatabase<TBusiness, TProjection>(DataDomain, DataProvider,
				Queries.Select<TBusiness, TProjection>(where, having, orderBy, groupBy, offset, limit));
		}

		public ITypedDatabaseQuery<TBusiness, int> SelectCount(QueryExpression where = null, QueryExpression having = null, QueryExpression[] groupBy = null)
		{
			return new SilkTypedORMDatabase<TBusiness, int>(DataDomain, DataProvider,
				Queries.SelectCount<TBusiness>(where, having, groupBy));
		}

		public ITypedDatabaseQuery<TBusiness, int> SelectCount<TProjection>(QueryExpression where = null, QueryExpression having = null, QueryExpression[] groupBy = null) where TProjection : new()
		{
			return new SilkTypedORMDatabase<TBusiness, int>(DataDomain, DataProvider,
				Queries.SelectCount<TBusiness, TProjection>(where, having, groupBy));
		}
	}

	public class SilkTypedORMDatabase<TBusiness, TResult> : SilkORMDatabaseBase<TBusiness, ITypedDatabaseQuery<TBusiness, TResult>, SilkTypedORMDatabase<TBusiness, TResult>, QueryCollection<TResult>>, ITypedDatabaseQuery<TBusiness, TResult>
		where TBusiness : new()
	{
		public SilkTypedORMDatabase(DataDomain dataDomain, IDataProvider dataProvider, QueryCollection<TResult> queries)
			: base(dataDomain, dataProvider, queries)
		{
		}

		public ICollection<TResult> Execute()
		{
			return Queries.Execute(DataProvider);
		}

		public Task<ICollection<TResult>> ExecuteAsync()
		{
			return Queries.ExecuteAsync(DataProvider);
		}
	}
}
