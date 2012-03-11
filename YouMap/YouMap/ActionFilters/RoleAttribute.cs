using System.Linq;
using System.Web.Mvc;
using YouMap.Domain.Enums;

namespace YouMap.ActionFilters
{
    public class RoleAttribute: AuthorizeAttribute
    {
        private readonly ISessionContext _sessionContext;
        private readonly UserPermissionEnum[] _permissions;

        public RoleAttribute(params UserPermissionEnum[] permissions)
        {
            _sessionContext = new SessionContext();
            _permissions = permissions;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            try
            {
                return _permissions.Any(x => _sessionContext.User.HasPermissions(x));
            }
            catch
            {
                return false;
            }
        }
    }

    public class AdminAttribute : RoleAttribute
    {
        public AdminAttribute():base(UserPermissionEnum.Admin)
        {
            
        }
    }
}