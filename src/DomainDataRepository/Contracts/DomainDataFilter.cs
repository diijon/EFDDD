using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EFDDD.Core;

namespace EFDDD.DomainDataRepository.Contracts
{
    public class DomainDataFilter<TDataEntity>
        where TDataEntity : class, IDataEntity
    {
        protected readonly IList<Expression<Func<TDataEntity, bool>>> _expressionContainer;

        public DomainDataFilter()
        {
            _expressionContainer = new List<Expression<Func<TDataEntity, bool>>>();
        } 

        protected void Where(Expression<Func<TDataEntity, bool>> expression)
        {
            _expressionContainer.Add(expression);
        }

        protected void Where(string expression, IEnumerable<object> values)
        {
            _expressionContainer.Add(System.Linq.Dynamic.DynamicExpression.ParseLambda<TDataEntity, bool>(expression, values.ToArray()));
        }
    }
}