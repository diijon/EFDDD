using System;
using AutoMapper;
using EFDDD.DomainDataMapper.Strategies;

namespace EFDDD.DomainDataMapper
{
    public class MappingConfiguration
    {
        public static void Configure()
        {
            AddTypeConverters(Mapper.Configuration);
            AddProfiles(Mapper.Configuration);
            AddDomainToDomainMaps(Mapper.Configuration);

            Mapper.AssertConfigurationIsValid();
        }

        internal static void AddTypeConverters(IConfiguration c)
        {
            c.CreateMap<DateTime?, DateTime?>().ConvertUsing(new NullableDateTimeToNullableDateTimeConverter());
            c.CreateMap<DateTime, DateTime?>().ConvertUsing(new DateTimeToNullableDateTimeConverter());
            c.CreateMap<DateTime?, DateTime>().ConvertUsing(new NullableDateTimeToDateTimeConverter());
            c.CreateMap<Guid?, string>().ConvertUsing(new NullableGuidToStringConverter());
            c.CreateMap<DateTime?, string>().ConvertUsing(new NullableDateTimeToStringConverter());
            c.CreateMap<decimal?, decimal>().ConvertUsing(new NullableDecimalToDecimalConverter());
            c.CreateMap<int?, int>().ConvertUsing(new NullableIntToIntConverter());
            c.CreateMap<Int32?, int>().ConvertUsing(new NullableInt32ToIntConverter());
            c.CreateMap<Int64?, long>().ConvertUsing(new NullableInt64ToLongConverter());
            c.CreateMap<Object, Guid>().ConvertUsing(new ObjectToGuidConverter());
            c.CreateMap<Guid, Object>().ConvertUsing(new GuidToObjectConverter());
        }

        internal static void AddProfiles(IConfiguration c)
        {
            c.AddProfile<Home>();
            c.AddProfile<EntityMapper<DomainModel.HomeOwner, DataModel.HomeOwner>>();
            c.AddProfile<EntityMapper<DomainModel.Neighborhood, DataModel.Neighborhood>>();
        }

        internal static void AddDomainToDomainMaps(IConfiguration c)
        {
            c.AddProfile<EntityMapper<DomainModel.Home, DomainModel.Home>>();
            c.AddProfile<EntityMapper<DomainModel.HomeOwner, DomainModel.HomeOwner>>();
            c.AddProfile<EntityMapper<DomainModel.Room, DomainModel.Room>>();
            c.AddProfile<EntityMapper<DomainModel.Neighborhood, DomainModel.Neighborhood>>();
        }
    }
}
