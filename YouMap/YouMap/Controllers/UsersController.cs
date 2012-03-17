using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.ActionFilters;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
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
            return RespondTo();
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
