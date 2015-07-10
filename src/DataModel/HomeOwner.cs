using System;
using EFDDD.Core;

namespace EFDDD.DataModel
{
    public class HomeOwner : IDataEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}