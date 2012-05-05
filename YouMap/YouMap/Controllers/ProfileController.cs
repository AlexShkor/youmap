using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.Documents.Services;
using YouMap.Framework;

namespace YouMap.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly UserDocumentService _userDocumentService;

        public ProfileController(ICommandService commandService, UserDocumentService userDocumentService) : base(commandService)
        {
            _userDocumentService = userDocumentService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult Feeds()
        {
            var user = _userDocumentService.GetById(User.Id);
            var model = user.Feeds;
            return View(model);
        }
    }
}
