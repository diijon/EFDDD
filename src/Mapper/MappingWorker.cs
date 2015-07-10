namespace EFDDD.DomainDataMapper
{
    public interface IMappingWorker
    {
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
        TDestination Map<TSource, TDestination>(TSource source);
    }

    public class MappingWorker : IMappingWorker
    {
        private AutoMapper.IMappingEngine _mapper;

        public MappingWorker(AutoMapper.IMappingEngine mapper)
        {
            _mapper = mapper;
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _mapper.Map(source, destination);
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }
    }
}