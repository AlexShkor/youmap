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
    }
}
