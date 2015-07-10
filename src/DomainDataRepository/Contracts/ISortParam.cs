using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFDDD.Core;

namespace EFDDD.DomainDataRepository.Contracts
{
    public interface ISortParam
    {
        object Expression { get; }
    }

    public interface ISortParam<TDomainEntity> : ISortParam
        where TDomainEntity : IDomainEntity
    {

    }

    public enum SortOrder
    {
        ASC,
        DESC
    }
}
