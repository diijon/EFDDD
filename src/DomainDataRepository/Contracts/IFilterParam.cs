using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EFDDD.Core;

namespace EFDDD.DomainDataRepository.Contracts
{
    public interface IFilterParam
    {
        object Expression { get; }
    }

    public interface IFilterParam<TDomainEntity> : IFilterParam
        where TDomainEntity : IDomainEntity
    {

    }
}
