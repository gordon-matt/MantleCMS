using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using System.Linq.Dynamic.Core;

namespace Mantle.Web.Mvc.KendoUI;

public static class GenericDataServiceExtensions
{
    public static IEnumerable<TEntity> Find<TEntity>(
        this IGenericDataService<TEntity> service,
        KendoGridMvcRequest request,
        Expression<Func<TEntity, bool>> predicate = null)
        where TEntity : class
    {
        using (var connection = service.OpenConnection())
        {
            var query = predicate == null ? connection.Query() : connection.Query(predicate);

            // Filtering
            query = request.FilterObjectWrapper != null ? ApplyFiltering(query, request.FilterObjectWrapper) : query;

            // Sorting
            query = ApplySorting(query, request.SortObjects);

            // Paging
            if (request.Skip.HasValue && request.Skip > 0)
            {
                query = query.Skip(request.Skip.Value);
            }
            if (request.Take.HasValue && request.Take > 0)
            {
                query = query.Take(request.Take.Value);
            }

            return query.ToHashSet();
        }
    }

    private static IQueryable<TEntity> ApplyFiltering<TEntity>(IQueryable<TEntity> query, FilterObjectWrapper filter)
    {
        string filtering = GetFiltering<TEntity>(filter);
        return filtering != null ? query.Where(filtering) : query;
    }

    private static IQueryable<TEntity> ApplySorting<TEntity>(IQueryable<TEntity> query, IEnumerable<SortObject> sortObjects)
    {
        string sorting = GetSorting(sortObjects) ?? query.ElementType.FirstSortableProperty();
        return query.OrderBy(sorting);
    }

    private static string GetFiltering<TEntity>(FilterObjectWrapper filter)
    {
        var finalExpression = string.Empty;

        foreach (var filterObject in filter.FilterObjects)
        {
            filterObject.Field1 = MapFieldfromViewModeltoEntity(filterObject.Field1);
            filterObject.Field2 = MapFieldfromViewModeltoEntity(filterObject.Field2);

            if (finalExpression.Length > 0)
            {
                finalExpression += " " + filter.LogicToken + " ";
            }

            if (filterObject.IsConjugate)
            {
                var expression1 = filterObject.GetExpression1<TEntity>();
                var expression2 = filterObject.GetExpression2<TEntity>();
                var combined = $"({expression1} {filterObject.LogicToken} {expression2})";
                finalExpression += combined;
            }
            else
            {
                var expression = filterObject.GetExpression1<TEntity>();
                finalExpression += expression;
            }
        }

        return finalExpression.Length == 0 ? "true" : finalExpression;
    }

    private static string GetSorting(IEnumerable<SortObject> sortObjects)
    {
        if (sortObjects == null)
        {
            return null;
        }

        var expression = string.Join(",", sortObjects.Select(s => MapFieldfromViewModeltoEntity(s.Field) + " " + s.Direction));
        return expression.Length > 1 ? expression : null;
    }

    private static string MapFieldfromViewModeltoEntity(string field)
    {
        return field;
        //return _mappings != null && field != null && _mappings.ContainsKey(field) ? _mappings[field].Path : field;
    }
}