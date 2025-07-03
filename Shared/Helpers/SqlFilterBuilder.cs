using Shared.Enums;
using System.Text.RegularExpressions;


    namespace Shared.Helpers
    {
        public static class SqlFilterBuilder
        {
            public static string GetWhereValue(string fieldName, string data, FieldType fieldType) 
            {
                return fieldType switch
                {
                    FieldType.String => $"[{fieldName}] LIKE '%{data}%'",
                    FieldType.Date => $"[{fieldName}] = '{data}'",
                    FieldType.Boolean => data == "false"
                        ? $"([{fieldName}] = 0 OR [{fieldName}] IS NULL)"
                        : $"[{fieldName}] = 1",
                    FieldType.Numeric => data == "0"
                        ? $"([{fieldName}] = {data} OR [{fieldName}] IS NULL)"
                        : $"[{fieldName}] = {data}",
                    FieldType.Decimal => data == "0" || data == "0.0" || data == "0.00"
                        ? $"([{fieldName}] = {data} OR [{fieldName}] IS NULL)"
                        : $"[{fieldName}] = {data}",
                    _ => $"[{fieldName}] = '{data}'"
                };
            }

            public static string RemoveSpecialChars(string text) 
            {
                Regex regex = new("[^a-zA-Z0-9 -]");
                return regex.Replace(text, "");
            }
        }
    }
