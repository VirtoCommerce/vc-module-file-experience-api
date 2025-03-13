using System;
using VirtoCommerce.FileExperienceApi.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.FileExperienceApi.Core.Extensions;

public static class FileExtensions
{
    public static bool OwnerIs<T>(this File file, T owner)
        where T : IEntity
    {
        ArgumentNullException.ThrowIfNull(file);

        if (owner is null ||
            string.IsNullOrEmpty(file.OwnerEntityId) ||
            string.IsNullOrEmpty(file.OwnerEntityType) ||
            !file.OwnerEntityId.EqualsIgnoreCase(owner.Id))
        {
            return false;
        }

        var ownerType = typeof(T);

        while (ownerType != null)
        {
            if (file.OwnerEntityType == ownerType.FullName)
            {
                return true;
            }

            ownerType = ownerType.BaseType;
        }

        return false;
    }

    public static bool OwnerIsEmpty(this File file)
    {
        ArgumentNullException.ThrowIfNull(file);

        return string.IsNullOrEmpty(file.OwnerEntityId) &&
               string.IsNullOrEmpty(file.OwnerEntityType);
    }

    public static void ClearOwner(this File file)
    {
        ArgumentNullException.ThrowIfNull(file);

        file.OwnerEntityId = null;
        file.OwnerEntityType = null;
    }

    public static void SetOwner<T>(this File file, T owner)
        where T : IEntity
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(owner);

        file.OwnerEntityId = owner.Id;
        file.OwnerEntityType = typeof(T).FullName;
    }
}
