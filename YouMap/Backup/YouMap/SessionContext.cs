using System;
using System.Web;
using mPower.Framework;

namespace YouMap
{
    public interface ISessionContext
    {
        string UserId { get; set; }
        string UserEmail { get; set; }
        string ClientKey { get; set; }
        IUserIdentity User { get;}
        bool IsUserAuthorized();
        void Logout();
        string GetStringSessionValue(string key);
        bool GetBoolSessionValue(string key);
        void SetSessionValue<T>(string key, T value);
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
        private const string _userId = "UserId";
        private const string _userName= "UserName";
        private const string _userEmail = "UserEmail";
        private const string _clientKey = "ClientKey";


        public string UserId
        {
            get { return GetStringSessionValue(_userId); }
            set { SetSessionValue(_userId, value); }
        }

        public string UserEmail
        {
            get { return GetStringSessionValue(_userEmail); }
            set { SetSessionValue(_userEmail, value); }
        }

        public string UserName
        {
            get { return GetStringSessionValue(_userName); }
            set { SetSessionValue(_userName, value); }
        }


        public string ClientKey
        {
            get { return GetStringSessionValue(_clientKey); }
            set { SetSessionValue(_clientKey, value); }
        }

        public IUserIdentity User
        {
            get
            {
                return new UserIdentity
                           {
                               Id = UserId,
                               Email = UserEmail,
                               Name = UserName
                           };
            }
            private set
            {
                UserId = value.Id;
                UserName = value.Name;
                UserEmail = value.Email;
            }
        }

        public bool IsUserAuthorized()
        {
            return !String.IsNullOrEmpty(UserId);
        }

        public void Logout()
        {
            HttpContext.Current.Session.Abandon();
            UserId = null;
            UserEmail = null;
            UserName = null;
        }

        public string GetStringSessionValue(string key)
        {
            var value = HttpContext.Current.Session[key];
            return value == null ? null : value.ToString();
        }

        public bool GetBoolSessionValue(string key)
        {
            var item = HttpContext.Current.Session[key];
            var result = false;
            if (item != null)
            {
                bool.TryParse(item.ToString(), out result);
            }
            return result;
        }

        public void SetSessionValue<T>(string key, T value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public void RemoveSessionValue(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                HttpContext.Current.Session.Remove(key);
            }
        }

        public void SetUser(IUserIdentity user)
        {
            User = user;
        }
    }

    public class UserIdentity : IUserIdentity
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}