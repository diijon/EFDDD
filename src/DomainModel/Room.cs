using EFDDD.Core;

namespace EFDDD.DomainModel
{
    public class Room : IDomainEntity, IValueObject
    {
        public string Name { get; private set; }
        public float SizeSqFt { get; private set; }

        internal Room() { }

        public Room(string name, float sizeSqFt)
        {
            Name = name;
            SizeSqFt = sizeSqFt;
        }
    }
}