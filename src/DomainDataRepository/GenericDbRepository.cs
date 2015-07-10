using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using EFDDD.Core;
using EFDDD.DataModel.EF;
using EFDDD.DomainDataMapper;
using EFDDD.DomainDataRepository.Contracts;
using EFDDD.DomainDataRepository.Internal;

namespace EFDDD.DomainDataRepository
{

    public class GenericDbRepository<TDomainEntity, TDataEntity> : IDomainRepository<TDomainEntity, TDataEntity>
        where TDomainEntity : class, IDomainEntity
        where TDataEntity : class, IDataEntity
    {
        public IContext Context { get; private set; }
        internal IMappingWorker Mapper;
        internal IDbSet<TDataEntity> DbSet;

        public GenericDbRepository(IMappingWorker mapper, IContext dbContext)
        {
            Mapper = mapper;
            Context = dbContext;
            DbSet = Context.Set<TDataEntity>();
        }

        /// <summary>
        /// Internally Mapped as a List&lt;TDataEntity&gt; to preserve inheritance of DomainEntities 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TDomainEntity Find(object id)
        {
            var dataEntity = DbSet.Find(id);
            if (dataEntity == null) { return default(TDomainEntity); }

            var dataEntities = new List<TDataEntity> { dataEntity };
            var domainEntities = Mapper.Map<IEnumerable<TDataEntity>, IEnumerable<TDomainEntity>>(dataEntities);
            return domainEntities.FirstOrDefault();
        }

        /// <summary>
        /// Creates the entity if the Id is set to the default value. Otherwise, the entity is updated. Supported entity Id types: int, Guid, string
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Merge(TDomainEntity entity)
        {
            var entityIdName = FindIdName();
            if (entityIdName == null) { throw new NullReferenceException("Id Property could not be detected for the Entity. Conventions supported: Id or <DataEntityName>Id"); }

            var dataEntity = Mapper.Map<TDomainEntity, TDataEntity>(entity);

            var doesIdExist = DoesIdExist(entityIdName, FindEntityIdValue(dataEntity, entityIdName));
            if (!doesIdExist)
            {
                // Create the Entity
                DbSet.Add(dataEntity);
                return;
            }

            // Update the Entity
            var trackedEntity = (Context.ChangeTracker.Entries<TDataEntity>() ?? new List<DbEntityEntry<TDataEntity>>()).FirstOrDefault(x => x.Entity.Equals(dataEntity));
            if (trackedEntity == null)
            {
                DbSet.Attach(dataEntity);
                Context.Entry(dataEntity).State = EntityState.Modified;
                return;
            }

            trackedEntity.CurrentValues.SetValues(dataEntity);
        }

        //public void GetDerivedTrackedEntityOrDefault()

        /// <summary>
        /// Flags the entity in the queue for deletion.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Delete(TDomainEntity entity)
        {
            var dataEntity = Mapper.Map<TDomainEntity, TDataEntity>(entity);

            var trackedEntity = (Context.ChangeTracker.Entries<TDataEntity>() ?? new List<DbEntityEntry<TDataEntity>>()).FirstOrDefault(x => x.Entity.Equals(dataEntity));
            if (trackedEntity == null)
            {
                var entityIdName = FindIdName();
                if (entityIdName == null) { throw new NullReferenceException("Id Property could not be detected for the Entity. Conventions supported: Id or <DataEntityName>Id"); }

                var entityIdValue = FindEntityIdValue(dataEntity, entityIdName);
                Delete(entityIdValue);
                return;
            }

            trackedEntity.State = EntityState.Deleted;
        }

        public virtual void Delete(object id)
        {
            var dataEntity = DbSet.Find(id);
            DbSet.Remove(dataEntity);
        }

        public virtual IEnumerable<TDomainEntity> All(ref int? totalRecords, IFilterParam filters = null, ISortParam sorters = null, int? pageSize = null, int? pageIndex = null)
        {
            var dataEntities = DbSet.Where(x => true);
            dataEntities = GetDerivedQueryOrDefault(dataEntities);
            dataEntities = VerifyAndApplyFiltersForAllMethod(dataEntities, filters);
            totalRecords = totalRecords.HasValue ? dataEntities.Count() : 0;
            dataEntities = VerifyAndApplySortersForAllMethod(dataEntities, sorters);
            if (pageIndex.HasValue && sorters == null)
            {
                var entityIdName = FindIdName();
                if (entityIdName == null) { throw new NullReferenceException("Id Property could not be detected for the Entity. Conventions supported: Id or <DataEntityName>Id"); }

                dataEntities = dataEntities.OrderBy(entityIdName);
            }
            dataEntities = ApplyPageSize(dataEntities, pageSize, pageIndex);

            var domainEntities = Mapper.Map<IEnumerable<TDataEntity>, IEnumerable<TDomainEntity>>(dataEntities);
            return domainEntities;
        }

        internal virtual IQueryable<TDataEntity> GetDerivedQueryOrDefault(IQueryable<TDataEntity> query)
        {
            return query.Where(x => true);
        }

        public virtual IEnumerable<TDomainEntity> All(IFilterParam filters = null, ISortParam sorters = null, int? pageSize = null, int? pageIndex = null)
        {
            int? totalRecords = null;
            return All(ref totalRecords, filters, sorters, pageSize, pageIndex);
        }

        public IDomainRepository<TDomainEntity> WithFastReads(bool isEnabled)
        {
            Context.Configuration.AutoDetectChangesEnabled = !isEnabled;
            return this;
        }

        /// <summary>
        /// Projects to any Type
        /// </summary>
        /// <typeparam name="TProjection">The properties of the Type must follow a exact name match convention to allow materialization</typeparam>
        /// <returns></returns>
        public virtual IEnumerable<TProjection> AllTo<TProjection>(IFilterParam filters = null, ISortParam sorters = null, int? pageSize = null, int? pageIndex = null)
        {
            int? totalRecords = null;
            return AllTo<TProjection>(ref totalRecords, filters, sorters, pageSize, pageIndex);
        }

        /// <summary>
        /// Projects to any Type
        /// </summary>
        /// <typeparam name="TProjection">The properties of the Type must follow a exact name match convention to allow materialization</typeparam>
        /// <returns></returns>
        public virtual IEnumerable<TProjection> AllTo<TProjection>(ref int? totalRecords, IFilterParam filters = null, ISortParam sorters = null, int? pageSize = null, int? pageIndex = null)
        {
            var dataEntities = DbSet.Where(x => true);
            dataEntities = GetDerivedQueryOrDefault(dataEntities);
            dataEntities = VerifyAndApplyFiltersForAllMethod(dataEntities, filters);
            totalRecords = totalRecords.HasValue ? DbSet.Count() : 0;
            dataEntities = VerifyAndApplySortersForAllMethod(dataEntities, sorters);
            if (pageIndex.HasValue && sorters == null)
            {
                var entityIdName = FindIdName();
                if (entityIdName == null) { throw new NullReferenceException("Id Property could not be detected for the Entity. Conventions supported: Id or <DataEntityName>Id"); }

                dataEntities = dataEntities.OrderBy(entityIdName);
            }
            dataEntities = ApplyPageSize(dataEntities, pageSize, pageIndex);

            var value = new List<TProjection>();
            foreach (var dataEntity in dataEntities)
            {
                var asJson = dataEntity.JsonSerialize();
                var asProjection = asJson.JsonDeserialize<TProjection>();
                value.Add(asProjection);
            }
            return value;
        }

        public virtual void Save()
        {
            Context.SaveChanges();
        }

        #region Private Members

        internal virtual string FindIdName()
        {
            return DataModel.EF.Helpers.FindIdName<TDataEntity>();
        }

        internal virtual object FindEntityIdValue<T>(T dataEntity, string entityIdName)
        {
            return DataModel.EF.Helpers.FindEntityIdValue(dataEntity, entityIdName);
        }

        internal virtual bool DoesIdExist(string entityIdName, object id)
        {
            return Context.DoesIdExist<TDataEntity>(entityIdName, id);
        }

        internal virtual IQueryable<TDataEntity> VerifyAndApplyFiltersForAllMethod(IQueryable<TDataEntity> query, IFilterParam filters)
        {
            if (filters == null) { return query; }

            var filterExpressionContainer = filters.Expression as List<Expression<Func<TDataEntity, bool>>>;
            if (filterExpressionContainer == null)
            {
                throw new InvalidCastException(string.Format("Filter Expressions must be of Type: {0} and cannot equal NULL", typeof(List<Expression<Func<TDataEntity, bool>>>).FullName));
            }

            for (var i = 0; i < filterExpressionContainer.Count(); i++)
            {
                var filterExpression = filterExpressionContainer[i];
                if (filterExpression == null)
                {
                    throw new NullReferenceException(string.Format("Filter Expression at Position: {0} cannot equal null", i.ToString()));
                }
                query = ApplyFilter(query, filterExpression);
            }
            return query;
        }

        internal virtual IQueryable<TDataEntity> VerifyAndApplySortersForAllMethod(IQueryable<TDataEntity> query, ISortParam sorters)
        {
            if (sorters == null) { return query; }

            var sortExpression = sorters.Expression as FuncIqueryable<TDataEntity>;
            if (sortExpression == null) { throw new InvalidCastException(string.Format("Sorter Expressions must be of Type: {0}", typeof(FuncIqueryable<TDataEntity>).Name)); }
            return ApplySort(query, sortExpression.Func);
        }

        internal virtual IQueryable<TDataEntity> ApplyFilter(IQueryable<TDataEntity> query, Expression<Func<TDataEntity, bool>> filter = null)
        {
            if (filter != null)
            {
                return query.Where(filter);
            }
            return query;
        }

        internal virtual IQueryable<TType> ApplySort<TType>(IQueryable<TType> query, Func<IQueryable<TType>, IOrderedQueryable<TType>> orderBy = null)
        {
            if (orderBy == null) return query;

            return orderBy(query);
        }

        internal virtual IQueryable<TType> ApplyPageSize<TType>(IQueryable<TType> query, int? pageSize = null, int? pageIndex = null)
        {
            if (pageSize.HasValue && pageIndex.HasValue)
                return query.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value);

            if (pageSize.HasValue)
                return query.Take(pageSize.Value);

            return query;
        }

        internal virtual IQueryable<TDataEntity> ApplyPageSize(IQueryable<TDataEntity> query, int? pageSize = null, int? pageIndex = null)
        {
            return ApplyPageSize<TDataEntity>(query, pageSize, pageIndex);
        }

        #endregion

        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) { return; }

            if (disposing)
            {
                //TODO: Free any other managed objects here.
            }

            _disposed = true;
        }
    }
}
