using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFDDD.DomainDataRepository.Contracts;
using DomainEntity = EFDDD.DomainModel.Home;
using DataEntity = EFDDD.DataModel.Home;

namespace EFDDD.DomainDataRepository.Filters
{
    public class FilterHomes : DomainDataFilter<DataEntity>, IFilterParam<DomainEntity>
    {
        public FilterHomes(string keyword = null,
            IEnumerable<object> id = null,
            string address = null,
            IEnumerable<string> roomNames = null,
            bool? hasHOA = null,
            IEnumerable<object> homeOwnerId = null)
        {
            Keyword = keyword;
            Id = id;
            Address = address;
            RoomNames = roomNames;
            HasHOA = hasHOA;
            HomeOwnerId = homeOwnerId;
        }
        
        public string Keyword { get; set; }
        public IEnumerable<object> Id { get; set; }
        public string Address { get; set; }
        public IEnumerable<string> RoomNames { get; set; }
        public bool? HasHOA { get; set; }
        public IEnumerable<object> HomeOwnerId { get; set; }

        //TODO: Create Expressions as needed. For advanced querying, look to seperate application where querying data is primary concern (SSRS, etc)
        public object Expression
        {
            get
            {
                if (Id != null && Id.Any())
                {
                    if (Id.Count() == 1)
                    {
                        var id = (Guid)Id.First();
                        Where(x => x.Id == id);
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        for (var i = 0; i < Id.Count(); i++)
                        {
                            if (i != 0) { sb.Append("|| "); }
                            sb.Append(string.Format("Id == @{0}", i));
                        }

                        Where(sb.ToString(), Id);
                    }
                }

                if (!string.IsNullOrEmpty(Address))
                {
                    Where(x => x.Address == Address);
                }

                if (RoomNames != null && RoomNames.Any())
                {
                    if (RoomNames.Count() == 1)
                    {
                        var roomName = RoomNames.First();
                        Where(x => x.Rooms.Contains(("\"" + roomName + "\"")));
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        for (var i = 0; i < RoomNames.Count(); i++)
                        {
                            if (i != 0) { sb.Append("|| "); }
                            sb.Append(string.Format("Rooms.Contains(\"@{0}\")", i));
                        }

                        Where(sb.ToString(), RoomNames);
                    }
                }

                if (HasHOA.HasValue)
                {
                    Where(x => x.Neighborhood.HasHomeownersAssociation == HasHOA);
                }

                if (HomeOwnerId != null && HomeOwnerId.Any())
                {
                    if (HomeOwnerId.Count() == 1)
                    {
                        var id = (Guid)HomeOwnerId.First();
                        Where(x => x.HomeOwnerId == id);
                    }
                    else
                    {
                        var sb = new StringBuilder();
                        for (var i = 0; i < HomeOwnerId.Count(); i++)
                        {
                            if (i != 0) { sb.Append("|| "); }
                            sb.Append(string.Format("HomeOwnerId == @{0}", i));
                        }

                        Where(sb.ToString(), Id);
                    }
                }

                if (!string.IsNullOrEmpty(Keyword))
                {
                    Where(x => x.Address.Contains(Keyword)
                        || x.Rooms.Contains(Keyword)
                        || x.Neighborhood.Name.Contains(Keyword));
                }

                return _expressionContainer;
            }
        }

    }
}
