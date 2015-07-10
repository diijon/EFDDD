using EFDDD.Core;

namespace EFDDD.DomainModel
{
    public class Neighborhood : IDomainEntity, IValueObject
    {
        public string Name { get; private set; }
        public bool HasHomeownersAssociation { get; set; }

        internal Neighborhood() { }

        public Neighborhood(string name, bool hasHomeOwnersAssociation)
        {
            Name = name;
            HasHomeownersAssociation = hasHomeOwnersAssociation;
        }
    }
}