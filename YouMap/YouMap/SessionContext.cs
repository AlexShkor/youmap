using System;
using System.Web;

namespace YouMap
{
    public interface ISessionContext
    {
        string UserId { get; set; }
        string UserEmail { get; set; }
        string ClientKey { get; set; }
        bool IsUserAuthorized();
        void Logout();
        string GetStringSessionValue(string key);
        bool GetBoolSessionValue(string key);
        void SetSessionValue<T>(string key, T value);
        void RemoveSessionValue(string key);
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


        public string ClientKey
        {
            get { return GetStringSessionValue(_clientKey); }
            set { SetSessionValue(_clientKey, value); }
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
    }
}