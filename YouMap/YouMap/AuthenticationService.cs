using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Authentication;
using System.Web;
using System.Web.Security;
using Paralect.Domain;
using YouMap.Controllers;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Domain.Data;
using YouMap.Scripts.custom;
using mPower.Framework;
using mPower.Framework.Environment;

namespace YouMap
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserDocumentService _userDocumentService;
        private readonly ICommandService _commandService;
        private readonly IIdGenerator _idGenerator;


        public VkAuthenticationService VkAuth { get; private set; }


        public AuthenticationService(UserDocumentService userDocumentService, ICommandService commandService, IIdGenerator idGenerator)
        {
            _commandService = commandService;
            _userDocumentService = userDocumentService;
            _idGenerator = idGenerator;
            VkAuth = new VkAuthenticationService(userDocumentService);
        }

        public bool ValidateUser(string email, string password)
        {
            return _userDocumentService.GetByFilter(new UserFilter { Email = email }).Any(x => x.Password == password);
        }

        public MembershipCreateStatus CreateUser(string email, string password)
        {
            var user = _userDocumentService.GetByFilter(new UserFilter { Email = email }).FirstOrDefault();
            if (user != null)
            {
                throw new AuthenticationException("User already exist");
            }
            var command = new User_CreateCommand
                              {
                                  UserId = _idGenerator.Generate(),
                                  Password = password,
                                  Email = email
                              };
            command.Metadata.UserId = command.UserId;
            _commandService.Send(command);
            return MembershipCreateStatus.Success;
        }

        public IUserIdentity GetUserByEmail(string email)
        {
            return _userDocumentService.GetByFilter(new UserFilter {Email = email}).First();
        }

        public IUserIdentity GetUserById(object id)
        {
            return _userDocumentService.GetById((string)id);
        }

        public void ChangePassword(string id, string oldPassword, string newPassword)
        {
            var command = new User_ChangePasswordCommand
                              {
                                  UserId = id,
                                  OldPassword = oldPassword,
                                  NewPassword = newPassword
                              };
            command.Metadata.UserId = command.UserId;
            _commandService.Send(command);
        }

        public void CreateUser(VkLoginModel model)
        {
            var user = _userDocumentService.GetByFilter(new UserFilter { VkId = model.Id }).FirstOrDefault();
            if (user != null)
            {
                throw new AuthenticationException("User already exist");
            }
            var command = new User_ImportFromVkCommand
            {
                UserId = _idGenerator.Generate(),
                Vk = new VkData
                         {
                             Domain = model.Domain,
                             Expire = model.Expire,
                             FirstName = model.FirstName,
                             Href = model.Href,
                             Id = model.Id,
                             LastName = model.LastName,
                             Mid = model.Mid,
                             Nickname = model.Nickname,
                             Secret = model.Secret,
                             Sid =  model.Sid,
                             Sig = model.Sig
                         }
            };
            command.Metadata.UserId = command.UserId;
            _commandService.Send(command);
        }

        public UserDocument GetVkUser(string id)
        {
            return _userDocumentService.GetByFilter(new UserFilter { VkId = id }).FirstOrDefault();
        }

    }

    public interface IAuthenticationService
    {
        bool ValidateUser(string email, string password);
        MembershipCreateStatus CreateUser(string email, string password);
        IUserIdentity GetUserByEmail(string email);
        IUserIdentity GetUserById(object id);
        void ChangePassword(string id, string oldPassword, string newPassword);
        void CreateUser(VkLoginModel model);
        UserDocument GetVkUser(string id);
        VkAuthenticationService VkAuth { get;}
    }

    public interface IVkAuthenticationService
    {
        
    }

    public class VkAuthenticationService: IVkAuthenticationService
    {
        private readonly UserDocumentService _userDocumentService;
        private string AppVkSecret;

        const string VkAppId = "2831071";
        protected string VkCookiesKey { get { return "vk_app_" + VkAppId; } }

        public  VkAuthenticationService(UserDocumentService userDocumentService)
        {
            _userDocumentService = userDocumentService;
        }

        public bool ValidateUser(string sig, VkLoginModel model)
        {
            var hash = string.Format("expire={0}mid={1}secret={2}sid={3}{4}",
                                     model.Expire, model.Mid, model.Secret, model.Sid, AppVkSecret);
            return GetMD5Hash(hash) == sig && ConvertFromUnixTimestamp(model.Expire) > DateTime.Now;
        }

        public bool ValidateUser(VkLoginModel model)
        {
            var user = _userDocumentService.GetByFilter(new UserFilter {VkId = model.Id}).FirstOrDefault();
            if (user == null)
            {
                return false;
            }
            return ValidateUser(user.Vk.Sig, model);
        }

        private bool ValidateUser(string sig, HttpCookie coockie)
        {
            var expire = long.Parse(coockie.Values["expire"]);
            var mid = coockie.Values["mid"];
            var secret = coockie.Values["secret"];
            var sid = coockie.Values["sid"];
            return ValidateUser(sig, new VkLoginModel
                                         {
                                             Expire = expire,
                                             Mid = mid,
                                             Secret = secret,
                                             Sid = sid
                                         });
        }

        public  string GetMD5Hash( string input)
        {
            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            var s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }

        public static DateTime ConvertFromUnixTimestamp(long timestamp)
        {

            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public HttpCookie CreateCookie(VkLoginModel model)
        {
            var hash = string.Format("expire={0}&mid={1}&secret={2}&sid={3}&uid={4}",
                                     model.Expire, model.Mid, model.Secret, model.Sid,model.Id);
            var cookie = new HttpCookie(VkCookiesKey, hash);
            //cookie.Expires = ConvertFromUnixTimestamp(model.Expire);
            cookie.Expires = DateTime.Now.AddYears(1);
            return cookie;
        }

        public IUserIdentity LoginFromCookie(HttpCookieCollection cookies)
        {
            var cookie = cookies.Get(VkCookiesKey);
            if (cookie == null)
            {
                throw new KeyNotFoundException("VK key not found in cookie collection");
            }
            var uid = cookie.Values["uid"];
            var user = _userDocumentService.GetByFilter(new UserFilter {VkId = uid}).First();
            if (user == null)
            {
                throw new SecurityException("User Not Found");
                
            }
            return user;
            //if (ValidateUser(user.Vk.Sig, cookie))
            //{
            //    return user;
            //}
            //throw new SecurityException("Auth is expired.");
        }

        
    }

    
}