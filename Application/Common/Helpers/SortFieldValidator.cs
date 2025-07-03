namespace Application.Common.Helpers
{
    public static class SortFieldValidator
    {
        public static string Validate(string? sortBy, string[] allowedFields, string defaultField)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return defaultField;

            if (!allowedFields.Contains(sortBy))
                return defaultField;

            return sortBy;
        }

        // ✅ Thêm method này để xử lý SortDirection
        public static string ValidateDirection(string? direction)
        {
            return direction?.ToUpper() switch
            {
                "ASC" => "ASC",
                "DESC" => "DESC",
                _ => "DESC"
            };
        }
    }
}
