using Application.Common.Attributes;
using Microsoft.Data.SqlClient;
using Shared.Enums;
using Shared.Helpers;
using System.Reflection;

namespace Application.DTOs.Abstract
{
    public abstract class BaseFilterDto
    {
        /// <summary>
        /// Override ở mỗi DTO để map tên property → FieldType
        /// </summary>
        public virtual Dictionary<string, FieldType> GetFieldTypeMappings() => new();

        // Chỉ dành cho override thủ công
        public virtual List<SqlParameter> ToSqlParametersWithWhereClauseOnly()
        {
            var whereClause = BuildWhereClause();
            return new List<SqlParameter>
            {
                new SqlParameter("@WhereCondition", (object)whereClause ?? DBNull.Value)
            };
        }

        public virtual List<SqlParameter> ToSqlParametersWithPagingAndSorting()
        {
            var parameters = new List<SqlParameter>();

            if (this is BasePagingFilterDto paging)
            {
                parameters.Add(new SqlParameter("@Start", paging.Start)); 
                parameters.Add(new SqlParameter("@NumberOfRows", paging.NumberOfRows));
               
            }

            var whereClause = BuildWhereClause();
            parameters.Add(new SqlParameter("@WhereCondition", (object?)whereClause ?? DBNull.Value));

            return parameters;
        }



        private string BuildWhereClause()
        {
            var conditions = new List<string>();
            var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fieldTypes = GetFieldTypeMappings();

            // ✅ Loại bỏ các field không phải cột trong DB
            var excludedProps = new[] { "Start", "NumberOfRows", "SortBy", "SortDirection", "CurrentPage" };

            foreach (var prop in props)
            {
                if (Attribute.IsDefined(prop, typeof(IgnoreSqlParamAttribute)) || excludedProps.Contains(prop.Name))
                    continue;

                var value = prop.GetValue(this);
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    continue;

                var fieldName = prop.Name;
                var fieldType = fieldTypes.ContainsKey(fieldName) ? fieldTypes[fieldName] : FieldType.String;

                var whereValue = SqlFilterBuilder.GetWhereValue(fieldName, value.ToString()!, fieldType);
                Console.WriteLine($"[WHERE] {whereValue}");
                conditions.Add(whereValue);
            }

            return string.Join(" AND ", conditions);
        }

    }
}
