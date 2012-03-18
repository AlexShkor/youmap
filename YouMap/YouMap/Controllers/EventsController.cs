using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Domain.Data;
using YouMap.Framework;
using YouMap.Framework.Environment;
using YouMap.Framework.Utils.Extensions;
using YouMap.Helpers;
using YouMap.Models;

namespace YouMap.Controllers
{
    [Authorize]
    public class EventsController : BaseController
    {
        private readonly UserDocumentService _userDocumentService;
        private readonly PlaceDocumentService _placeDocumentService;
        private readonly IIdGenerator _idGenerator;
        private readonly ImageService _imageService;

        public EventsController(ICommandService commandService, UserDocumentService userDocumentService, PlaceDocumentService placeDocumentService, IIdGenerator idGenerator, ImageService imageService) : base(commandService)
        {
            _userDocumentService = userDocumentService;
            _placeDocumentService = placeDocumentService;
            _idGenerator = idGenerator;
            _imageService = imageService;
        }

        public ActionResult Index(string id)
        {
            id = id ?? User.Id;
            var doc = _userDocumentService.GetByFilter(new UserFilter {IdOrVkIdEqual = id}).First();
            var events = doc.Events;
            if (doc.Id != User.Id)
            {
                var user = _userDocumentService.GetById(User.Id);
                events = events.Where(x=> !x.Private || x.UsersIds.Contains(User.Id) || x.UsersIds.Contains(user.Vk.Id)).ToList();
            }
            var model = events.Select(MapToListItem);
            return RespondTo(model);
        }

        public EventListItem MapToListItem(EventDocument doc)
        {
            return new EventListItem
                       {
                           Id = doc.Id,
                           Title = doc.Title,
                           StartDate = FormatEventStartDate(doc.Start.Date)
                       };
        }

        private static string FormatEventStartDate(DateTime date)
        {
            if (date.Date == DateTime.Now.Date)
            {
                return date.ToShortTimeString();
            }
            if (date.Date == DateTime.Now.AddDays(1).Date)
            {
                return "завтра в " + date.ToShortTimeString();
            }
            return date.ToString("dd/MM/YYYY hh:mm");
        }

        [HttpGet]
        public ActionResult Create(string placeId = null)
        {
            var model = new EventEditModel();
            UpdateModelIfPost(model);
            placeId = placeId ?? model.PlaceId;
            if (placeId.HasValue())
            {
                var place = _placeDocumentService.GetById(placeId);
                model.PlaceId = place.Id;
                model.PlateTitle = place.Title;
            }
            return RespondTo(model);
        }

        [HttpPost]
        public ActionResult Create(EventEditModel model)
        {
            if(model.Start < DateTime.Now.Date)
            {
                ModelState.AddModelError("Start","Начало встречи должно быть в будущем.");
            }
            if (!ModelState.IsValid)
            {
                return Create();
            }
            var command = new User_AddEventCommand
                              {
                                  Id = _idGenerator.Generate(),
                                  Location = model.PlaceId.HasValue() ? null : Location.Parse(model.Latitude, model.Longitude),
                                  Memo = model.Memo,
                                  OwnerId = User.Id,
                                  PlaceId = model.PlaceId,
                                  Private = model.Private,
                                  Start = model.Start.Value,
                                  Title = model.Title,
                                  UsersIds = model.UsersIds
                              };
            Send(command);
            AjaxResponse.ClosePopup = true;
            return RespondTo(model);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var model = new EventEditModel();
            UpdateModelIfPost(model);
            return RespondTo(model);
        }

        [HttpPost]
        public ActionResult Edit(EventEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return Edit(model.Id);
            }
            return RespondTo(model);
        }


        [HttpGet]
        public ActionResult Details(string id)
        {
            return RespondTo();
        }

        [HttpGet]
        public ActionResult Show(string userid)
        {
            var model = new List<MarkerModel>();
            if (true)
            {
                //IT'S FAKE!!!
                model = _userDocumentService.GetAll().SelectMany(x => x.Events).Select(MapToMarker).ToList();
            }
            //TODO: Implement this
            else
            {
                var user = _userDocumentService.GetById(userid);
                // -2 hours to display just started events, need to be replaced with filter
                model = user.Events.Where(x => x.Start >= DateTime.Now.AddHours(-2)).Select(MapToMarker).ToList();
            }
            AjaxResponse.AddJsonItem("model",model);
            return RespondTo(model);
        }

        private MarkerModel MapToMarker(EventDocument doc)
        {
            return new MarkerModel
                       {
                           X = doc.Location.Latitude,
                           Y = doc.Location.Longitude,
                           Icon = _imageService.EventIconModel,
                           Shadow = _imageService.EventShadowModel,
                           InfoWindowUrl = Url.Action("Details",new{id=doc.Id}),
                          Title = doc.Title
                       };
        }
    }
}
