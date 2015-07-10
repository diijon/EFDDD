using EFDDD.Core;

namespace EFDDD.DomainModel
{
    public class HomeOwner : IDomainEntity, IAggregateRoot
    {
        public object Id { get; set; }
        public string Name { get; set; }
    }
}