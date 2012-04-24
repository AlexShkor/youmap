using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.Controllers;
using YouMap.Documents.Services;
using YouMap.Framework;
using YouMap.Framework.Environment;

namespace YouMap.Areas.Mobile.Controllers
{
    public class EventsController : YouMap.Controllers.EventsController
    {
        public EventsController(ICommandService commandService, UserDocumentService userDocumentService, PlaceDocumentService placeDocumentService, IIdGenerator idGenerator, ImageService imageService) : base(commandService, userDocumentService, placeDocumentService, idGenerator, imageService)
        {
        }

        //public ActionResult ForPlace(string placeId)
        //{
        //    var model = _userDocumentService.GetEventsListForPlace(placeId, 3, 20).Select(MapToListItem);
        //    return View("Index", model);
        //}

        //public ActionResult ForUser(string userId)
        //{
        //    var user = _userDocumentService.GetById(userId);
        //    var model = user.Events.OrderByDescending(x => x.Start).Select(MapToListItem);
        //    return View("Index", model);
        //}
    }
}
