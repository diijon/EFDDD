using System;
using System.Collections.Generic;
using EFDDD.Core;

namespace EFDDD.DomainModel
{
    public class Home : IDomainEntity, IAggregateRoot
    {
        public object Id { get; set; }
        public DateTime DateBuilt { get; set; }
        public string Address { get; set; }
        public Neighborhood Neighborhood { get; set; }
        public List<Room> Rooms { get; set; }
        public object HomeOwnerId { get; set; }

        public Home()
        {
            Rooms = new List<Room>();
            Neighborhood = new Neighborhood();
        }
    }
}
