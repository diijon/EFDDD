using System.Linq;
using EFDDD.DomainDataRepository.Contracts;
using DomainEntity = EFDDD.DomainModel.Home;
using DataEntity = EFDDD.DataModel.Home;

namespace EFDDD.DomainDataRepository.Sorters
{
    public class SortHomes : ISortParam<DomainEntity>
    {
        public SortHomes(SortOrder sortOrder = SortOrder.ASC, bool address = false, bool dateBuilt = false)
        {
            SortOrder = sortOrder;
            Address = address;
            DateBuilt = dateBuilt;
        }

        public SortOrder SortOrder { get; set; }
        public bool Address { get; set; }
        public bool DateBuilt { get; set; }

        //TODO: If needed, add ability for multi-column sorting
        public object Expression
        {
            get
            {
                if (Address)
                {
                    return new FuncIqueryable<DataEntity>(x => SortOrder == SortOrder.ASC
                        ? (x.OrderBy(key => key.Address))
                        : (x.OrderByDescending(key => key.Address)));
                }

                if (DateBuilt)
                {
                    return new FuncIqueryable<DataEntity>(x => SortOrder == SortOrder.ASC
                        ? (x.OrderBy(key => key.DateBuilt))
                        : (x.OrderByDescending(key => key.DateBuilt)));
                }

                return new FuncIqueryable<DataEntity>(x => SortOrder == SortOrder.ASC
                    ? (x.OrderBy(key => key.DateBuilt))
                    : (x.OrderByDescending(key => key.DateBuilt)));
            }
        }
    }
}
