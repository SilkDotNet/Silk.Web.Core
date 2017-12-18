using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Silk.Data.SQL.Expressions;
using Silk.Data.SQL.ORM;
using Silk.Data.SQL.ORM.Queries;
using Silk.Data.SQL.Providers;

namespace Silk.Web.Core.Data
{
	public abstract class SilkORMDatabaseBase<TThisInterface, TThisImpl, TQueryCollection>
		where TThisInterface : class
		where TThisImpl : SilkORMDatabaseBase<TThisInterface, TThisImpl, TQueryCollection>, TThisInterface
		where TQueryCollection : QueryCollectionBase<TQueryCollection>
	{
		protected abstract Func<DataDomain, IDataProvider, TThisImpl> InstanceFactory { get; }

		protected IDataProvider DataProvider { get; }
		protected DataDomain DataDomain { get; }
		protected TThisImpl Self { get; }
		protected bool IsRoot { get; set; }
		protected TQueryCollection Queries { get; set; }

		protected SilkORMDatabaseBase(DataDomain dataDomain, IDataProvider dataProvider,
			TQueryCollection queryCollection)
		{
			IsRoot = true;
			Self = this as TThisImpl;
			Queries = queryCollection;
			DataDomain = dataDomain;
			DataProvider = dataProvider;
		}

		private TThisImpl SelfOrNew()
		{
			if (IsRoot)
			{
				var ret = InstanceFactory(DataDomain, DataProvider);
				ret.IsRoot = false;
				return ret;
			}
			return Self;
		}

		public TThisInterface Insert<TBusiness>(params TBusiness[] sources)
			where TBusiness : new()
		{
			var ret = SelfOrNew();
			ret.Queries = ret.Queries.Insert<TBusiness>(sources);
			return ret;
		}

		public TThisInterface Insert<TBusiness, TEntity>(params TEntity[] sources)
			where TBusiness : new()
			where TEntity : new()
		{
			var ret = SelfOrNew();
			ret.Queries = ret.Queries.Insert<TBusiness, TEntity>(sources);
			return ret;
		}

		public TThisInterface Update<TBusiness>(params TBusiness[] sources)
			where TBusiness : new()
		{
			var ret = SelfOrNew();
			ret.Queries = ret.Queries.Update<TBusiness>(sources);
			return ret;
		}

		public TThisInterface Update<TBusiness, TEntity>(params TEntity[] sources)
			where TBusiness : new()
			where TEntity : new()
		{
			var ret = SelfOrNew();
			ret.Queries = ret.Queries.Update<TBusiness, TEntity>(sources);
			return ret;
		}

		public TThisInterface Delete<TBusiness>(params TBusiness[] sources)
			where TBusiness : new()
		{
			var ret = SelfOrNew();
			ret.Queries = ret.Queries.Delete<TBusiness>(sources);
			return ret;
		}

		public TThisInterface Delete<TBusiness, TEntity>(params TEntity[] sources)
			where TBusiness : new()
			where TEntity : new()
		{
			var ret = SelfOrNew();
			ret.Queries = ret.Queries.Delete<TBusiness, TEntity>(sources);
			return ret;
		}

		public TThisInterface Delete<TBusiness>(QueryExpression where)
			where TBusiness : new()
		{
			var ret = SelfOrNew();
			ret.Queries = ret.Queries.Delete<TBusiness>(where);
			return ret;
		}

		public TThisInterface AsTransaction()
		{
			var ret = SelfOrNew();
			ret.Queries = ret.Queries.AsTransaction();
			return ret;
		}
	}

	public class SilkORMDatabase : SilkORMDatabaseBase<IDatabase, SilkORMDatabase, QueryCollection>, IDatabase
	{
		protected override Func<DataDomain, IDataProvider, SilkORMDatabase> InstanceFactory =>
			(dataDomain, dataProvider) => new SilkORMDatabase(dataDomain, dataProvider);

		public SilkORMDatabase(DataDomain dataDomain, IDataProvider dataProvider, QueryCollection queries)
			: base(dataDomain, dataProvider, new QueryCollection(dataDomain))
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
	}

	public class SilkORMDatabase<TResult> : SilkORMDatabaseBase<IDatabaseQuery<TResult>, SilkORMDatabase<TResult>, QueryCollection<TResult>>, IDatabaseQuery<TResult>
	{
		public SilkORMDatabase(DataDomain dataDomain, IDataProvider dataProvider, QueryCollection<TResult> queryCollection)
			: base(dataDomain, dataProvider, queryCollection)
		{
			IsRoot = false;
		}

		protected override Func<DataDomain, IDataProvider, SilkORMDatabase<TResult>> InstanceFactory => throw new NotImplementedException();

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
		public IDatabase AllDatabase => new SilkORMDatabase(DataDomain, DataProvider, Queries);

		protected override Func<DataDomain, IDataProvider, SilkTypedORMDatabase<TBusiness>> InstanceFactory =>
			(dataDomain, dataProvider) => new SilkTypedORMDatabase<TBusiness>(dataProvider, dataDomain);

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
	}

	public class SilkTypedORMDatabase<TBusiness, TResult> : SilkORMDatabaseBase<TBusiness, ITypedDatabaseQuery<TBusiness, TResult>, SilkTypedORMDatabase<TBusiness, TResult>, QueryCollection<TResult>>, ITypedDatabaseQuery<TBusiness, TResult>
		where TBusiness : new()
	{
		protected override Func<DataDomain, IDataProvider, SilkTypedORMDatabase<TBusiness, TResult>> InstanceFactory => throw new NotImplementedException();

		public SilkTypedORMDatabase(DataDomain dataDomain, IDataProvider dataProvider, QueryCollection<TResult> queries)
			: base(dataDomain, dataProvider, queries)
		{
			IsRoot = false;
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
