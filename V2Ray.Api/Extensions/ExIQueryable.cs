using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace V2Ray.Api.Extensions
{
    public static class ExIQueryable
    {
        public static IEnumerable<T> SetValue<T>(this IEnumerable<T> items, Action<T>
             updateMethod)
        {
            foreach (T item in items)
            {
                updateMethod(item);
            }
            return items;
        }

        public static async Task<Pagination<TOut>> GetPagination<T, TOut, TPage>(this IQueryable<T> query, TPage input, IMapper _mapper) where T : class
where TPage : PaginationModelInput
        {
            var pagination = new Pagination<TOut>
            {
                TotalItems = query.Count(),
                PageCount = input.ItemsPerPage == -1 ? 1 : Convert.ToInt32(query.Count() / input.ItemsPerPage + 1),
                CurrentPage = input.Page,
            };

            var skip = (input.Page - 1) * input.ItemsPerPage;
            input.ItemsPerPage = input.ItemsPerPage == -1 ? int.MaxValue : input.ItemsPerPage;
            var result = query
                 .OrderBy(input.OrderBy ?? "id-desc")
                 .Skip(skip)
                 .Take(input.ItemsPerPage);
            pagination.Result = _mapper.Map<List<TOut>>(result);
            return pagination;
        }

        public static IQueryable<T> Include<T>(this IQueryable<T> source, params string[] include)
        where T : class
        {
            var entityType = typeof(T);
            object query = source;

            if (include == null) return (IQueryable<T>)query;

            foreach (var propertyPath in include)
            {
                Type prevPropertyType = null;
                foreach (var propertyName in propertyPath.Split('.'))
                {
                    Type parameterType;
                    MethodInfo method;
                    if (prevPropertyType == null)
                    {
                        parameterType = entityType;

                        method = IncludeMethodInfo;
                    }
                    else
                    {
                        parameterType = prevPropertyType;
                        method = IncludeAfterReferenceMethodInfo;
                        if (parameterType.IsConstructedGenericType && parameterType.GenericTypeArguments.Length == 1)
                        {
                            var elementType = parameterType.GenericTypeArguments[0];
                            var collectionType = typeof(IEnumerable<>).MakeGenericType(elementType);
                            var enumerableType = typeof(ICollection<>).MakeGenericType(elementType);
                            if (collectionType.IsAssignableFrom(parameterType) || enumerableType.IsAssignableFrom(parameterType))
                            {
                                parameterType = elementType;
                                method = IncludeAfterCollectionMethodInfo;
                            }
                        }
                    }
                    var parameter = Expression.Parameter(parameterType, "e");
                    var property = Expression.PropertyOrField(parameter, propertyName);
                    if (prevPropertyType == null)
                        method = method.MakeGenericMethod(entityType, property.Type);
                    else
                        method = method.MakeGenericMethod(entityType, parameter.Type, property.Type);
                    query = method.Invoke(null, new object[] { query, Expression.Lambda(property, parameter) });
                    prevPropertyType = property.Type;
                }
            }

            return (IQueryable<T>)query;
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string sort)
        where T : class
        {
            return source.OrderBy(sort, false);
        }

        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> query,
            bool condition,
            Expression<Func<T, bool>> predicate)
        {
            return !condition ? query : query.Where(predicate);
        }

        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> query,
            bool condition,
            Expression<Func<T, int, bool>> predicate)
        {
            return !condition ? query : query.Where(predicate);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IQueryable<T> source, string sort)
        where T : class
        {
            return source.OrderBy(sort, true);
        }

        // -----------------

        private static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string sort, bool prevSort)
        where T : class
        {
            var entityType = typeof(T);
            object query = source;
            var array = sort?.Split(',');
            if (array != null)
            {
                foreach (var item in array)
                {
                    var terms = item.Split('-');
                    var sortBy = terms[0].Trim().ToLower();
                    var orderByAsc = terms.Length > 1 && terms[1].Trim().ToLower() == "asc";

                    var property = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                        .FirstOrDefault(p => p.Name.ToLower() == sortBy);

                    if (property != null)
                    {
                        var expressionParameter = Expression.Parameter(entityType, "x");
                        var expressionProperty = Expression.PropertyOrField(expressionParameter, property.Name);

                        MethodInfo method = orderByAsc ?
                            prevSort ? ThenOrderByMethodInfo : OrderByMethodInfo :
                            prevSort ? ThenOrderByDescendingMethodInfo : orderByDescendingMethodInfo;

                        method = method.MakeGenericMethod(entityType, expressionProperty.Type);
                        query = method.Invoke(null, new object[] { query, Expression.Lambda(expressionProperty, expressionParameter) });

                        prevSort = true;
                    }
                }
            }

            return (IOrderedQueryable<T>)query;
        }

        private static readonly MethodInfo IncludeMethodInfo =
            typeof(EntityFrameworkQueryableExtensions).GetTypeInfo()
            .GetDeclaredMethods(nameof(EntityFrameworkQueryableExtensions.Include))
            .Single(mi => mi.GetParameters()[1].ParameterType != typeof(string));

        private static readonly MethodInfo IncludeAfterCollectionMethodInfo =
            typeof(EntityFrameworkQueryableExtensions).GetTypeInfo()
            .GetDeclaredMethods(nameof(EntityFrameworkQueryableExtensions.ThenInclude))
            .Single(mi => !mi.GetParameters()[0].ParameterType.GenericTypeArguments[1].IsGenericParameter);

        private static readonly MethodInfo IncludeAfterReferenceMethodInfo =
            typeof(EntityFrameworkQueryableExtensions).GetTypeInfo()
            .GetDeclaredMethods(nameof(EntityFrameworkQueryableExtensions.ThenInclude))
            .Single(mi => mi.GetParameters()[0].ParameterType.GenericTypeArguments[1].IsGenericParameter);

        private static readonly MethodInfo OrderByMethodInfo = typeof(Queryable).GetTypeInfo()
            .GetDeclaredMethods(nameof(Queryable.OrderBy))
            .Single(mi => mi.GetParameters().Count() == 2);

        private static readonly MethodInfo ThenOrderByMethodInfo = typeof(Queryable).GetTypeInfo()
            .GetDeclaredMethods(nameof(Queryable.ThenBy))
            .Single(mi => mi.GetParameters().Count() == 2);

        private static readonly MethodInfo orderByDescendingMethodInfo = typeof(Queryable).GetTypeInfo()
            .GetDeclaredMethods(nameof(Queryable.OrderByDescending))
            .Single(mi => mi.GetParameters().Count() == 2);

        private static readonly MethodInfo ThenOrderByDescendingMethodInfo = typeof(Queryable).GetTypeInfo()
            .GetDeclaredMethods(nameof(Queryable.ThenByDescending))
            .Single(mi => mi.GetParameters().Count() == 2);
    }
}