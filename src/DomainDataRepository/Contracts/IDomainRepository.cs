using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDDD.DomainDataRepository.Contracts
{
    public interface IDomainRepository : IDisposable
    {
        /// <summary>
        /// Deletes a single domain entity. Does NOT commit changes see <see cref="WF.Data.Shared.DomainDataContracts.IDomainRepository.Save()"/>
        /// </summary>
        /// <param name="id"></param>
        void Delete(object id);

        /// <summary>
        /// Commits changes to the repository Store
        /// </summary>
        void Save();
    }

    public interface IDomainRepository<TDomainEntity> : IDomainRepository
    {
        /// <summary>
        /// Gets a single domain entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TDomainEntity Find(object id);

        /// <summary>
        /// Checks whether or not the entity exists before deciding to Create or Update the entity. Does NOT commit changes see <see cref="WF.Data.Shared.DomainDataContracts.IDomainRepository.Save()"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Merge(TDomainEntity entity);

        /// <summary>
        /// Deletes a domain entity. Does NOT commit changes see <see cref="WF.Data.Shared.DomainDataContracts.IDomainRepository.Save()"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Delete(TDomainEntity entity);

        /// <summary>
        /// Gets domain entities with filtering and sorting capabilities
        /// </summary>
        /// <param name="filters">Set using an concrete instance of <see cref="WF.Data.Shared.DomainDataContracts.IFilterParam"/></param>
        /// <param name="sorters">Set using an concrete instance of <see cref="WF.Data.Shared.DomainDataContracts.ISortParam"/></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        IEnumerable<TDomainEntity> All(IFilterParam filters = null, ISortParam sorters = null, int? pageSize = null, int? pageIndex = null);

        /// <summary>
        /// Gets domain entities with filtering and sorting capabilities
        /// </summary>
        /// <param name="totalRecords">Returns count of records before filter operation. Set to null to disable the count query.</param>
        /// <param name="filters">Set using an concrete instance of <see cref="WF.Data.Shared.DomainDataContracts.IFilterParam"/></param>
        /// <param name="sorters">Set using an concrete instance of <see cref="WF.Data.Shared.DomainDataContracts.ISortParam"/></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        IEnumerable<TDomainEntity> All(ref int? totalRecords, IFilterParam filters = null, ISortParam sorters = null, int? pageSize = null, int? pageIndex = null);

        /// <summary>
        /// Allows performance tweaking of queries
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        IDomainRepository<TDomainEntity> WithFastReads(bool isEnabled);
    }

    public interface IDomainRepository<TDomainEntity, TDataEntity> : IDomainRepository<TDomainEntity>
    {
    }
}
