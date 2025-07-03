using System;
using System.Collections.Generic;

namespace Infrastructure.Abstractions.Metadata
{
    public static class MetadataRegistry
    {
        private static readonly Dictionary<Type, string[]> CreateFieldMap = new();
        private static readonly Dictionary<Type, string[]> UpdateFieldMap = new();

        public static void Register<T>(string[] createFields, string[] updateFields)
        {
            var type = typeof(T);
            CreateFieldMap[type] = createFields;
            UpdateFieldMap[type] = updateFields;
        }

        public static string[] GetCreateFields(Type type)
        {
            return CreateFieldMap.TryGetValue(type, out var fields) ? fields : Array.Empty<string>();
        }

        public static string[] GetUpdateFields(Type type)
        {
            return UpdateFieldMap.TryGetValue(type, out var fields) ? fields : Array.Empty<string>();
        }
    }
}
