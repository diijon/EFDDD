using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFDDD.Core;
using EFDDD.DomainDataRepository.Contracts;

namespace EFDDD.DomainDataRepository
{
    public static class Extensions
    {
        public static GenericDbRepository<TDomainEntity, TDataEntity> WithFastReads<TDomainEntity, TDataEntity>(this GenericDbRepository<TDomainEntity, TDataEntity> repository, bool isEnabled)
            where TDomainEntity : class, IDomainEntity
            where TDataEntity : class, IDataEntity
        {
            return repository.WithFastReads(isEnabled) as GenericDbRepository<TDomainEntity, TDataEntity>;
        }

        public static int Count<TDomainEntity>(this IDomainRepository<TDomainEntity> repository, IFilterParam filters = null)
            where TDomainEntity : IDomainEntity
        {
            int? totalRecords = 0;
            repository.WithFastReads(true).All(ref totalRecords, filters: filters, pageSize: 1, pageIndex: 0);

            return totalRecords.GetValueOrDefault();
        }
    }
}
