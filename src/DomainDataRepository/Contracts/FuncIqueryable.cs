using System;
using System.Linq;

namespace EFDDD.DomainDataRepository.Contracts
{
    public class FuncIqueryable<TEntity>
    {
        public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> Func { get; set; }
        public FuncIqueryable(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> func)
        {
            if (func != null)
            {
                Func = func;
                return;
            }

            Func = x => x.OrderBy(xx => xx);
        }
    }
}