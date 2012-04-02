using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.Controllers;
using YouMap.Documents.Services;
using YouMap.Framework;

namespace YouMap.Areas.Mobile.Controllers
{
    public class HomeController : MapController
    {
        public HomeController(ICommandService commandService, PlaceDocumentService placeDocumentService, ImageService imageService, UserDocumentService userDocumentService) : base(commandService, placeDocumentService, imageService, userDocumentService)
        {
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Main()
        {
            return View("Index");
        }

        public ActionResult Events()
        {
            throw new NotImplementedException();
        }

        public ActionResult Friends()
        {
            throw new NotImplementedException();
        }

        public ActionResult Places()
        {
            throw new NotImplementedException();
        }


        public ActionResult Settings()
        {
            throw new NotImplementedException();
        }
    }
}
