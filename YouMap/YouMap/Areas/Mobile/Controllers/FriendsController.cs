using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.Controllers;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Framework;

namespace YouMap.Areas.Mobile.Controllers
{
    public class FriendsController : BaseController
    {
        private readonly UserDocumentService _userDocumentService;

        public FriendsController(ICommandService commandService, UserDocumentService userDocumentService) : base(commandService)
        {
            _userDocumentService = userDocumentService;
        }

        public ActionResult Index()
        {
            var user = _userDocumentService.GetById(User.Id);
            var friends = _userDocumentService.GetByFilter(new UserFilter{VkIdIn = user.Friends, OrderBy = UserOrderByEnum.LastCheckInDate});
            var model = friends.Select(Map);
            return View(model);
        }

        private FriendModel Map(UserDocument doc)
        {
            return new FriendModel
                       {
                           Name = doc.FullName,
                           VkId = doc.VkId,
                           Id = doc.Id,
                       };
        }
    }

    public class FriendModel
    {
        public string Name { get; set; }

        public string VkId { get; set; }

        public string Id { get; set; }
    }
}
