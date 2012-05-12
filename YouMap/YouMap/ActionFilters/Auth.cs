using System.Web.Mvc;
using StructureMap.Attributes;

namespace YouMap.ActionFilters
{
    public class Auth: AuthorizeAttribute
    {
        private readonly ISessionContext _sessionContext = new SessionContext();

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return _sessionContext.User != null;
        }
    }
}