namespace VirtoCommerce.FileExperienceApi.Core;

public static class ModuleConstants
{
    public static class Security
    {
        public static class Permissions
        {
            public const string Access = "Files:access";
            public const string Create = "FileExperienceApi:create";
            public const string Read = "FileExperienceApi:read";
            public const string Update = "FileExperienceApi:update";
            public const string Delete = "FileExperienceApi:delete";

            public static string[] AllPermissions { get; } =
            {
                Access,
                Create,
                Read,
                Update,
                Delete,
            };
        }
    }
}
