using AutoMapper;
using EFDDD.DomainDataMapper.Internal;

namespace EFDDD.DomainDataMapper
{
    public interface IEntityMapper
    {
        void MapToDomain();
        void MapToData();
    }

    public class EntityMapper<TDomainEntity, TDataEntity> : Profile, IEntityMapper
    {
        public virtual IMappingExpression<TDataEntity, TDomainEntity> ToDomain()
        {
            return Mapper.CreateMap<TDataEntity, TDomainEntity>();
        }

        public virtual void MapToDomain()
        {

            Mapper.CreateMap<TDataEntity, TDomainEntity>().IgnoreAllNonExisting();
        }

        public virtual IMappingExpression<TDomainEntity, TDataEntity> ToData()
        {
            return Mapper.CreateMap<TDomainEntity, TDataEntity>();
        }

        public virtual void MapToData()
        {
            Mapper.CreateMap<TDomainEntity, TDataEntity>().IgnoreAllNonExisting();
        }

        public override string ProfileName { get { return string.Format("{0}<-->{1}", typeof(TDomainEntity), typeof(TDataEntity)); } }

        protected override void Configure()
        {
            MapToDomain();
            MapToData();
        }
    }
}
