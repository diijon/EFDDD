using System;
using System.Collections.Generic;
using EFDDD.DomainDataMapper.Internal;

namespace EFDDD.DomainDataMapper.Strategies
{
    public class Home : EntityMapper<DomainModel.Home, DataModel.Home>
    {
        public override void MapToData()
        {
            ToData()
                .ForMember(x => x.Rooms,
                    opt => opt.MapFrom(src => (src.Rooms ?? new List<DomainModel.Room>()).JsonSerialize(null)))
                .IgnoreAllNonExisting();
        }

        public override void MapToDomain()
        {
            ToDomain()
                .ForMember(x => x.Rooms,
                    opt => opt.MapFrom(src => (src.Rooms ?? string.Empty).JsonDeserialize<List<DomainModel.Room>>(null)))
                .IgnoreAllNonExisting();
        }
    }
}
