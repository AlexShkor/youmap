using System.Linq;
using System.Security.Principal;
using System.Web.SessionState;
using YouMap.Domain.Auth;
using YouMap.Domain.Enums;

namespace YouMap
{
    public interface ISessionContext
    {
        string ClientKey { get; set; }
        IUserIdentity User { get;}
        bool IsUserAuthorized();
        void Logout();
        string GetStringSessionValue(string key);
        bool GetBoolSessionValue(string key);
        void SetSessionValue(string key, object value);
        void RemoveSessionValue(string key);
        void SetUser(IUserIdentity user);
    }

    /// <summary>
    /// SessionContext can be obtained only through the TenantsContainer,
    /// but can't be obtained as usual through the tenant container
    /// We should probably use session only from BaseController, 
    /// no need to send it through Constructur parameter because if it not working
    /// </summary>
    [Serializable]
    public class SessionContext : ISessionContext
    {
        private const string _userKey = "UserId";
        private const string _clientKey = "ClientKey";


        public string ClientKey
        {
            get { return GetStringSessionValue(_clientKey); }
            set { SetSessionValue(_clientKey, value); }
        }

        public IUserIdentity User
        {
            get { return GetSessionValue<IUserIdentity>(_userKey); }
            private set
            {
                if (value == null)
                {
                    SetSessionValue(_userKey, null);
                    return;
                }
                var user = new UserIdentity
                              {
                                  Email = value.Email,
                                  Id = value.Id,
                                  Name = value.Name,
                                  Permissions = value.Permissions
                              };
                SetSessionValue(_userKey,user);
            }
        }

        private T GetSessionValue<T>(string sessionKey)
        {
            return (T) Session[sessionKey];
        }

        protected HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        public bool IsUserAuthorized()
        {
            return User != null && !String.IsNullOrEmpty(User.Id);
        }

        public void Logout()
        {
            HttpContext.Current.Session.Abandon();
            SetUser(null);
        }

        public string GetStringSessionValue(string key)
        {
            var value = HttpContext.Current.Session[key];
            return value == null ? null : value.ToString();
        }

        public bool GetBoolSessionValue(string key)
        {
            var item = Session[key];
            var result = false;
            if (item != null)
            {
                bool.TryParse(item.ToString(), out result);
            }
            return result;
        }

        public void SetSessionValue(string key, object value)
        {
            Session[key] = value;
        }

        public void RemoveSessionValue(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                Session.Remove(key);
            }
        }

        public void SetUser(IUserIdentity user)
        {
            User = user;
        }
    }

    public class UserIdentity : IUserIdentity, IIdentity
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public string AuthenticationType
        {
            get { return "Forms"; }
        }

        public bool IsAuthenticated
        {
            get { return !String.IsNullOrEmpty(Id); }
        }

        public bool HasPermissions(params UserPermissionEnum[] permission)
        {
            return permission.All(x => Permissions.Contains(x));
        }

        public IEnumerable<UserPermissionEnum> Permissions { get; set; }

        public UserIdentity()
        {
            Permissions = new List<UserPermissionEnum>();
        }
    }

    public class UserPrincipal : IPrincipal
    {
        private readonly IUserIdentity _userIdentity;

        public UserPrincipal(UserIdentity identity)
        {
            Identity = identity;
            _userIdentity = identity;
        }

        public bool IsInRole(string role)
        {
            UserPermissionEnum value;
            if (Enum.TryParse(role,out value))
            {
                return _userIdentity.HasPermissions(value);
            }
            return false;
        }

        public IIdentity Identity { get; set; }
    }
}