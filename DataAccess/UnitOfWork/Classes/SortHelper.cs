using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using DataAccess.UnitOfWork.Interfaces;

namespace DataAccess.UnitOfWork.Classes
{
    public class SortHelper<T> : ISortHelper<T>
    {
        // Method to apply sorting to entities based on the provided sorting criteria
        public IQueryable<T> ApplySort(IQueryable<T> entities, string orderByQueryString)
        {
            // If there are no entities, return the original IQueryable
            if (!entities.Any())
                return entities;

            // If orderByQueryString is null or whitespace, return the original IQueryable
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                return entities;
            }

            // Split the orderByQueryString by commas to extract individual sorting parameters
            var orderParams = orderByQueryString.Trim().Split(',');

            // Get the properties of the entity type T using reflection
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Initialize a StringBuilder to construct the order query dynamically
            var orderQueryBuilder = new StringBuilder();

            // Iterate through each sorting parameter
            foreach (var param in orderParams)
            {
                // Skip empty or whitespace sorting parameters
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                // Extract the property name from the sorting parameter
                var propertyFromQueryName = param.Split(" ")[0];

                // Find the corresponding property in the entity type T
                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                // If the property does not exist, skip to the next parameter
                if (objectProperty == null)
                    continue;

                // Determine the sorting order (ascending or descending)
                var sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";

                // Append the property name and sorting order to the order query
                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {sortingOrder}, ");
            }

            // Remove the trailing comma and space from the order query
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');

            // Apply sorting to entities using the constructed order query
            return entities.OrderBy(orderQuery);
        }
    }
}
