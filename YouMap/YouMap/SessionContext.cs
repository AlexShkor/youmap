using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;
using YouMap.Domain.Auth;
using YouMap.Domain.Data;
using YouMap.Domain.Enums;

namespace YouMap
{
    public interface ISessionContext
    {
        UserInfo UserInfo { get; set; }
        IUserIdentity User { get;}
        Location Location { get; set; }
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
        private const string UserInfoKey = "UserInfo";
        private const string UserKey = "UserId";
        private const string LocationSessionKey = "Location";

        public IUserIdentity User
        {
            get { return GetSessionValue<IUserIdentity>(UserKey); }
            private set
            {
                if (value == null)
                {
                    SetSessionValue(UserKey, null);
                    return;
                }
                var user = new UserIdentity
                              {
                                  Email = value.Email,
                                  Id = value.Id,
                                  Name = value.Name,
                                  VkId = value.VkId,
                                  Permissions = value.Permissions
                              };
                SetSessionValue(UserKey,user);
            }
        }

        public UserInfo UserInfo
        {
            get
            {
                var value = GetSessionValue<UserInfo>(UserInfoKey);
                if (value == null)
                {
                    value = new UserInfo();
                    UserInfo = value;
                }
                return value;
            }
            set { SetSessionValue(UserInfoKey, value); }
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

        public Location Location
        {
            get { return GetSessionValue<Location>(LocationSessionKey); }
            set { SetSessionValue(LocationSessionKey, value); }
        }
    }

    public class UserInfo
    {
        public Location GpsLocation { get; set; }
        public Location Location { get; set; }
        public Location MapCenter { get; set; }
        public int MapZoom { get; set; }
    }

    public class UserIdentity : IUserIdentity, IIdentity
    {
        public string Id { get; set; }

        public string VkId { get; set; }

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