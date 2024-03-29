﻿#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YouMap.ActionFilters;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Commands;
using YouMap.Domain.Data;
using YouMap.Framework;
using YouMap.Framework.Environment;
using YouMap.Framework.Extensions;
using YouMap.Framework.Mvc.Ajax;
using YouMap.Framework.Services;
using YouMap.Framework.Utils;
using YouMap.Framework.Utils.Extensions;
using YouMap.Models;
#endregion

namespace YouMap.Controllers
{
    public class EventsController : BaseController
    {
        protected readonly UserDocumentService _userDocumentService;
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

        //public ActionResult Index(string id)
        //{
        //    id = id ?? User.Id;
        //    var doc = _userDocumentService.GetByFilter(new UserFilter {IdOrVkIdEqual = id}).First();
        //    var events = doc.Events;
        //    if (doc.Id != User.Id)
        //    {
        //        events = events.Where(x=> !x.Private || x.UsersIds.Contains(User.Id) || x.UsersIds.Contains(User.VkId)).ToList();
        //    }
        //    var model = events.Select(MapToListItem);
        //    return RespondTo(model);
        //}      

        public ActionResult Index(EventsFilter filter)
        {
            filter = filter ?? new EventsFilter();
            var now = DateTime.Now;
            var docFilter = new UserFilter {EventStartAfter = now};
            if (filter.HoursNext.HasValue)
            {
                docFilter.EventStartBefore = now.AddHours(filter.HoursNext.Value); 
            }
            if (filter.HoursAgo.HasValue)
            {
                docFilter.EventStartAfter = now.AddHours(-filter.HoursAgo.Value);
            }
            var doc = _userDocumentService.GetByFilter(docFilter);
            var events = doc.SelectMany(x => x.Events).Where(
                x => x.Start >= docFilter.EventStartAfter.Value);
            if (docFilter.EventStartBefore.HasValue)
            {
                events = events.Where(x => x.Start <= docFilter.EventStartBefore);
            }
            filter.PagingInfo.TotalCount = events.Count();
            var model = events.Skip(filter.PagingInfo.Skip).Take(filter.PagingInfo.Take).Select(MapToListItem);
            ViewBag.Filter = filter;
            return View(model);
        }

        public ActionResult ForPlace(string placeId)
        {
            var model = _userDocumentService.GetEventsListForPlace(placeId, 3, 20).Select(MapToListItem);
            return View("Index", model);
        }

        public ActionResult ForUser(EventsFilter filter)
        {
            var user = _userDocumentService.GetById(filter.UserId);
            var model = user.Events.OrderByDescending(x => x.Start).Select(MapToListItem);
            return View("Index", model);
        }

        [HttpGet]
        [VkAccess]
        public ActionResult Create(string placeId = null)
        {
            var model = new EventEditModel();
            UpdateModelIfPost(model);
            placeId = placeId ?? model.PlaceId;
            if (placeId.HasValue())
            {
                var place = _placeDocumentService.GetById(placeId);
                model.PlaceId = place.Id;
                model.PlaceTitle = place.Title;
                model.Latitude = place.Location.GetLatitudeString();
                model.Longitude = place.Location.GetLongitudeString();
            }
            return RespondTo(model);
        }

        [HttpPost]
        [VkAccess]
        public ActionResult Create(EventEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return Create();
            }
            model.Id = _idGenerator.Generate();
            var command = new User_AddEventCommand
                              {
                                  EventId = model.Id,
                                  Location = Location.Parse(model.Latitude, model.Longitude),
                                  Memo = model.Memo,
                                  OwnerId = User.Id,
                                  PlaceId = model.PlaceId,
                                  Private = model.Private,
                                  Start = model.GetStartDateTime(),
                                  Title = model.Title,
                                  Members = model.GetMembers()
                              };
            Send(command);
            AjaxResponse.AddJsonItem("model",MapToListItem(model));
            AjaxResponse.ClosePopup = true;
            return RespondTo(model);
        }

        private EventListItem MapToListItem(EventEditModel model)
        {
            return new EventListItem
                       {
                           Id = model.Id,
                           UsersIds = model.UserIds,
                           OwnerVkId = User.VkId,
                           Title = model.Title,
                           PlaceTitle = model.PlaceTitle,
                           StartDate = model.GetStartDateTime().ToInfoString(),
                           Memo = model.Memo,
                           ShareUrl = Url.Action("Index", "Map", new {placeId = model.PlaceId, eventId = model.Id})
                       };
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
        [VkAccess]
        public ActionResult Details(string id)
        {
            var user = _userDocumentService.GetByFilter(new UserFilter{EventIdEq = id}).First();
            var model = user.Events.Where(x => x.Id == id).Select(MapToListItem).First();
            var place = _placeDocumentService.GetById(model.PlaceId);
            model.PlaceTitle = place.Title;
            model.OwnerId = user.Id;
            model.OwnerVkId = user.Vk.Id;
            model.OwnerName = user.FullName;
            model.ActionUrl = Url.Action("Join", new { eventId = model.Id });
            model.ActionTitle = "Принять участие";
            model.LinkClass = "btn-success";
            if (User != null)
            {
                if (model.OwnerId != User.Id)
                {
                    if (model.UsersIds.Contains(User.VkId))
                    {
                        model.ActionUrl = Url.Action("Left", new {eventId = model.Id});
                        model.ActionTitle = "Покинуть встречу";
                        model.LinkClass = "btn-danger";
                    }
                }
                else
                {
                    model.ActionUrl = Url.Action("Delete", new {eventId = model.Id});
                    model.ActionTitle = "Удалить встречу";
                    model.LinkClass = "btn-danger";
                }
            }
            return RespondTo(model);
        }

        [HttpGet]
        public ActionResult Show(string userid)
        {
            //var user = _userDocumentService.GetById(userid);
            var user = _userDocumentService.GetById(userid);
            var docs = user.Events;

                // -2 hours to display just started events, need to be replaced with filter
               // model = user.Events.Where(x => x.Start >= DateTime.Now.AddHours(-2)).Select(MapToMarker).ToList();
            
            var model = docs.GroupBy(x=> x.PlaceId).Select(x=> MapToMarkerGroup(x,user)).ToList();
            AjaxResponse.AddJsonItem("model",model);
            return RespondTo(model);
        }

        private EventMarkerModel MapToMarkerGroup(IGrouping<string,EventDocument> group, UserDocument user)
        {
            var eventsList = group.OrderByDescending(x=> x.Start).Take(10).Select(MapToListItem).ToList();
            foreach (var item in eventsList)
            {
                item.OwnerName = user.FullName;
                item.OwnerId = user.Id;
                item.OwnerVkId = user.VkId;
                item.DisplayUsers = true;
            }
            var marker =  new EventMarkerModel
                       {
                           PlaceId = group.Key,
                           Icon = _imageService.EventIconModel,
                           Shadow = _imageService.EventShadowModel,
                           InfoWindowUrl = Url.Action("Details", new {id = group.Key}),
                           Content = RenderEventsListToString(eventsList)
                       };
            var place = _placeDocumentService.GetById(group.Key);
            marker.X = place.Location.Latitude;
            marker.Y = place.Location.Longitude;
            return marker;
        }

        private string RenderEventsListToString(List<EventListItem> eventsList)
        {
            return MvcUtils.RenderPartialToStringRazor(ControllerContext, "EventsListWindow", eventsList, ViewData,TempData);
        }

        protected EventListItem MapToListItem(EventDocument doc)
        {
            return new EventListItem
            {
                Id = doc.Id,
                PlaceId = doc.PlaceId,
                Title = doc.Title,
                Private = doc.Private,
                Memo =  doc.Memo,
                StartDate = doc.Start.ToInfoString(),
                Started = doc.Start < DateTime.Now,
                Url = Url.Action("Details",new {id = doc.Id}),
                ShareUrl = Url.RouteUrl("MapIndex", new { placeId = doc.PlaceId, eventId = doc.Id, controller = "Map", action = "Index"}),
                Members = doc.Members.Select(MapFriendListItem),
                UsersIds = doc.Members.Select(x=>x.VkId).ToList()
            };
        }

        private SelectListItem MapFriendListItem(Friend friend)
        {
            return new SelectListItem()
                       {
                           Value = friend.VkId,
                           Text = friend.FullName
                       };
        }

        public ActionResult List(string placeId)
        {
            var model = _userDocumentService.GetEventsListForPlace(placeId,3).Select(MapToListItem);
            AjaxResponse.Render("#eventsList"+placeId, "EventsList", model);
            return Result();
        }

        public ActionResult Join(string eventid)
        {
            if (User == null)
            {
                AjaxResponse.Render("#cboxLoadedContent","VkError",String.Empty);
                return Result();
            }
            var owner = _userDocumentService.GetByFilter(new UserFilter {EventIdEq = eventid}).First();
            var @event = owner.Events.First(x => x.Id == eventid);
            if (@event.Private)
            {
                ModelState.AddModelError("Error", "Вы не можете присоединиться к закрытой встрече.");
            }
            if (ModelState.IsValid)
            {
                var command = new User_JoinToEventCommand
                                  {
                                      EventId = @event.Id,
                                      NewMemberId = User.VkId,
                                      UserId = owner.Id,
                                      NewMemberName = User.Name
                                  };
                Send(command);
                AjaxResponse.ClosePopup = true;
            }
            else
            {
                AjaxResponse.Options.ErrorsSummaryContainer = "#eventValidation";
            }
            return RespondTo(request =>
                                 {
                                     request.Html =
                                         request.Ajax =
                                         () => RedirectToAction("Details", new {id = @event.Id});
                                     request.Json = Result;
                                 });
        }

        public ActionResult Delete(string eventid)
        {
            AjaxResponse.Options.ErrorsSummaryContainer = "#eventValidation";
            ModelState.AddModelError("Error", "Встреча не может быть удалена, пока в ней есть участники.");
            return Result();
        }

        public ActionResult Left(string eventid)
        {
            AjaxResponse.Options.ErrorsSummaryContainer = "#eventValidation";
            ModelState.AddModelError("Error", "Не удалось покинуть встречу.");
            return Result();
        }
    }

    public class EventsFilter
    {
        public int? HoursAgo { get; set; }
        public int? HoursNext { get; set; }
        public string Search { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string UserId { get; set; }
        public string PlaceId { get; set; }

        public EventsFilter()
        {
            PagingInfo = new PagingInfo {Take = 50};
            HoursAgo = 2;
            HoursNext = 24;
        }
    }
}
