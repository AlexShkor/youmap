using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YouMap.ActionFilters;
using YouMap.Areas.Mobile.Controllers;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Domain.Data;
using YouMap.Framework;
using YouMap.Framework.Extensions;
using YouMap.Framework.Services;
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

        [VkAccess]
        public ActionResult Friends(FriendsFilterModel filterModel)
        {
            filterModel = filterModel ?? new FriendsFilterModel();
            var user = _userDocumentService.GetById(User.Id);
            var filter = new UserFilter() {VkIdIn = user.Friends};
           // filter.PagingInfo = filterModel.PagingInfo;
            var model = _userDocumentService.GetByFilter(filter).Select(Map);
            ViewBag.Filter = filterModel;
            return View(model);
        }

        public ActionResult UserInfo(string id)
        {
            var user = _userDocumentService.GetById(id);
            var model = new UserInfoModel
                            {
                                Id = id,
                                Name = user.FullName,
                                CheckInsCount = user.CheckIns.Count,
                                EventsCount = user.Events.Count,
                                FriendsCount = user.Friends.Count
                            };
            if (model.CheckInsCount > 0)
            {
                var lastCheckIn = user.CheckIns.OrderByDescending(x => x.Visited).First();
                model.LastCheckInMessage = lastCheckIn.Memo;
                model.LastCheckInTimeAgo = lastCheckIn.Visited.ToInfoString();
                model.PlaceId = lastCheckIn.PlaceId;
            }
            return RespondTo(model);
        }

        
        public ActionResult UpdateFriends(string json)
        {
            var js = new JavaScriptSerializer();
            var friendsList =
                js.Deserialize<List<VkFriendModel>>(json).Select(
                    x => new Friend {VkId = x.uid, FullName = x.first_name + " " + x.last_name});
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

        public ActionResult Details(string id)
        {
            var user = _userDocumentService.GetById(id);
            var model = Map(user);
            return View(model);
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

        private FriendModel Map(UserDocument doc)
        {
            var result = new FriendModel
            {
                Name = doc.FullName,
                VkId = doc.VkId,
                Id = doc.Id,
                EventsLink = Url.Action("ForUser", "Events", new { userId = doc.Id }),
                CheckInsLink = Url.Action("ForUser", "CheckIns", new { userId = doc.Id }),
                DetailsLink = Url.Action("Details", "Users", new { id = doc.Id }),
                LastCheckInTimeAgo = "-",
                LastCheckInMessage = String.Empty
            };
            var lastCheckIn = doc.CheckIns.OrderByDescending(x => x.Visited).FirstOrDefault();
            if (lastCheckIn != null)
            {
                result.LastCheckInMessage = lastCheckIn.Memo;
                result.LastCheckInTimeAgo = lastCheckIn.Visited.ToInfoString();
                result.PlaceId = lastCheckIn.PlaceId;
                //result.PlaceTitle = lastCheckIn.PlaceTitle;
            }
            return result;
        }
    }

    public class FriendsFilterModel
    {
        public PagingInfo PagingInfo { get; set; }

        public FriendsFilterModel()
        {
            PagingInfo = new PagingInfo() { ItemsPerPage = 25 };
        }
    }

    public class VkFriendModel
    {
        public string uid;
        public string first_name;
        public string last_name;
    }
}
