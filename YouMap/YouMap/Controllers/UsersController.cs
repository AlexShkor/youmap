using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.ActionFilters;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Domain.Data;
using YouMap.Framework;
using YouMap.Models;

namespace YouMap.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly UserDocumentService _userDocumentService;

        public UsersController(ICommandService commandService, UserDocumentService userDocumentService) : base(commandService)
        {
            _userDocumentService = userDocumentService;
        }

        [Admin]
        public ActionResult Index(UserFilter filter)
        {
            var model = _userDocumentService.GetByFilter(filter ?? new UserFilter()).Select(MapToListItem);
            return View(model);
        }

        public ActionResult UserInfo(string id)
        {
            //var user = _userDocumentService.GetById(id);
            var model = new UserInfoModel { Id = id };
            return RespondTo(model);
        }

        public ActionResult UpdateFriends(string friends, string names)
        {
            
            var list = friends.Split(new [] {","}, StringSplitOptions.RemoveEmptyEntries);
            var namesList = names.Split(new [] {","}, StringSplitOptions.RemoveEmptyEntries);
            var friendsList = list.Zip(namesList, (x, y) => new Friend {VkId = x, FullName = y});
            var user = _userDocumentService.GetById(User.Id);
            var newFriends = friendsList.Where(x=> !user.Friends.Contains(x.VkId)).ToList();
            if (newFriends.Any())
            {
                var command = new User_AddFriendsCommand
                                  {
                                      UserId = User.Id,
                                      Friends = newFriends
                                  };
                Send(command);
                AjaxResponse.AddJsonItem("added",true);
            }
            return Result();
        }

        private UserListItem MapToListItem(UserDocument doc)
        {
            return new UserListItem
            {
                Id = doc.Id,
                Name = doc.Name,
                IsActive = doc.IsActive
            };
        }
    }
}
