using System.ComponentModel.DataAnnotations.Schema;
using EFDDD.Core;

namespace EFDDD.DataModel
{
    [ComplexType]
    public class Neighborhood : IDataEntity
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