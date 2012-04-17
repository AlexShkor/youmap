using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YouMap.Controllers;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Framework;
using YouMap.Framework.Extensions;
using YouMap.Framework.Utils.Extensions;

namespace YouMap.Areas.Mobile.Controllers
{
    public class FriendsController : BaseController
    {
        private readonly UserDocumentService _userDocumentService;
        private readonly PlaceDocumentService _placeDocumentService;

        public FriendsController(ICommandService commandService, UserDocumentService userDocumentService, PlaceDocumentService placeDocumentService)
            : base(commandService)
        {
            _userDocumentService = userDocumentService;
            _placeDocumentService = placeDocumentService;
        }

        public ActionResult Index()
        {
            var user = _userDocumentService.GetById(User.Id);
            var friends = _userDocumentService.GetByFilter(new UserFilter { VkIdIn = user.Friends, OrderBy = UserOrderByEnum.LastCheckInDate });
            var model = friends.Select(Map).ToList();
            UpdateFriendsPhotos(model);
            return View(model);
        }

        private void UpdateFriendsPhotos(IEnumerable<FriendModel> model)
        {
            if (model != null && model.Any())
            {
                var wc = new WebClient();
                var result = String.Empty;
                try
                {
                    result =
                        wc.DownloadString(
                            String.Format("https://api.vk.com/method/friends.get?uid={0}&fields=photo&access_token={1}",
                                          User.VkId,
                                          SessionContext.AccessToken));
                }
                catch
                {
                }
                if (result.HasValue())
                {
                    var js = new JavaScriptSerializer();
                    var response = js.Deserialize<VkArrayResponse<UsersGetDto>>(result).response;
                    foreach (var item in model)
                    {
                        var friend = item;
                        var usersGetDto = response.SingleOrDefault(x => x.uid == friend.VkId);
                        if (usersGetDto != null)
                            friend.Photo = usersGetDto.photo;
                    }
                }
            }
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
                DetailsLink = Url.Action("Details", "Friends", new { id = doc.Id }),
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

        public ActionResult Details(string id, string vkId)
        {
            UserDocument user = null;
            if (id.HasValue())
            {
                user = _userDocumentService.GetById(id);
            }
            else if (vkId.HasValue())
            {
                user = _userDocumentService.GetByFilter(new UserFilter() { VkId = vkId }).Single();
                if (user == null)
                {
                    return Redirect("http://vk.com/id" + vkId);
                }
            }
            var model = Map(user);
            if (model.PlaceId.HasValue())
            {
                var place = _placeDocumentService.GetById(model.PlaceId);
                model.PlaceTitle = place.Title;
            }
            return View(model);
        }
    }

    public class FriendModel
    {
        public string Name { get; set; }

        public string VkId { get; set; }

        public string Id { get; set; }

        public string CheckInsLink { get; set; }

        public string EventsLink { get; set; }

        public string DetailsLink { get; set; }

        public string LastCheckInTimeAgo { get; set; }

        public string LastCheckInMessage { get; set; }

        public string PlaceTitle { get; set; }

        public string PlaceId { get; set; }

        public string VkLink
        {
            get { return "http://vk.com/id" + VkId; }
        }

        public string Photo { get; set; }
    }
}