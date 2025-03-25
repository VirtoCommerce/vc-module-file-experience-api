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

        return owner != null && file.OwnerIs(owner.Id, typeof(T));
    }

    public static bool OwnerIs<T>(this File file, string ownerId)
    {
        ArgumentNullException.ThrowIfNull(file);

        return file.OwnerIs(ownerId, typeof(T));
    }

    public static bool OwnerIs(this File file, string ownerId, Type ownerType)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (ownerType is null ||
            string.IsNullOrEmpty(file.OwnerEntityId) ||
            string.IsNullOrEmpty(file.OwnerEntityType) ||
            !file.OwnerEntityId.EqualsIgnoreCase(ownerId))
        {
            return false;
        }


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

        file.SetOwner(owner.Id, typeof(T));
    }

    public static void SetOwner<T>(this File file, string ownerId)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(ownerId);

        file.SetOwner(ownerId, typeof(T));
    }

    public static void SetOwner(this File file, string ownerId, Type ownerType)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(ownerId);
        ArgumentNullException.ThrowIfNull(ownerType);

        file.OwnerEntityId = ownerId;
        file.OwnerEntityType = ownerType.FullName;
    }
}
