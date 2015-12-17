using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


namespace SmiteRepository
{
    public static class ExpressionExtensions
    {
        public static Expression<Predicate<T>> Or<T>(this Expression<Predicate<T>> expr1,
Expression<Func<T, bool>> where)
        {
            Expression<Predicate<T>> expr = Expression.Lambda<Predicate<T>>
            (Expression.Or(expr1.Body, where.Body), expr1.Parameters);
            return expr;

        }

        public static Expression<Predicate<T>> And<T>(this  Expression<Predicate<T>> expr1, Expression<Predicate<T>> where)
        {
           
            Expression<Predicate<T>> expr = Expression.Lambda<Predicate<T>>
         (Expression.And(expr1.Body, where.Body), expr1.Parameters);
            return expr;
        }
       
    }
}
