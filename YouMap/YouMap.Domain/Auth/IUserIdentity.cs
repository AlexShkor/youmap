using System.Collections.Generic;
using YouMap.Domain.Enums;

namespace YouMap.Domain.Auth
{
    public interface IUserIdentity
    {
        string Id { get; }
        string Email { get; }
        string Name { get; }
        bool HasPermissions(params UserPermissionEnum[] permission);
        IEnumerable<UserPermissionEnum> Permissions { get; }
    }
}