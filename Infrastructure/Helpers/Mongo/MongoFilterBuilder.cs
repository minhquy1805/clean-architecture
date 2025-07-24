using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Helpers.Mongo
{
    public static class MongoFilterBuilder
    {
        public static FilterDefinition<T> BuildFilter<T>(object filterDto)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Empty;

            var dtoType = filterDto.GetType();
            var entityType = typeof(T);

            foreach (var prop in dtoType.GetProperties())
            {
                var value = prop.GetValue(filterDto);
                if (value == null || value.ToString() == string.Empty) continue;

                var propName = prop.Name;
                var entityProp = entityType.GetProperty(propName);
                if (entityProp == null) continue;

                var fieldType = entityProp.PropertyType;

                // Support exact match
                if (fieldType == typeof(int) || fieldType == typeof(int?) ||
                    fieldType == typeof(bool) || fieldType == typeof(bool?))
                {
                    filter &= builder.Eq(propName, BsonValue.Create(value));
                }
                else if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
                {
                    if (propName.StartsWith("From"))
                    {
                        var targetField = propName.Replace("From", "");
                        filter &= builder.Gte(targetField, (DateTime)value);
                    }
                    else if (propName.StartsWith("To"))
                    {
                        var targetField = propName.Replace("To", "");
                        filter &= builder.Lte(targetField, (DateTime)value);
                    }
                    else
                    {
                        filter &= builder.Eq(propName, BsonValue.Create(value));
                    }
                }
                else if (fieldType == typeof(string))
                {
                    // Regex match (ignore case)
                    filter &= builder.Regex(propName, new BsonRegularExpression(value.ToString(), "i"));
                }
            }

            return filter;
        }
    }
}
