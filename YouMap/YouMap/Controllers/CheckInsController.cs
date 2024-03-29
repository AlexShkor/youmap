﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Framework;
using YouMap.Framework.Extensions;
using YouMap.Framework.Utils;
using YouMap.Models;

namespace YouMap.Controllers
{
    public class CheckInsController : BaseController
    {
        private readonly ImageService _imageService;
        protected readonly UserDocumentService _documentService;

        public CheckInsController(ImageService imageService,ICommandService commandService, UserDocumentService documentService) : base(commandService)
        {
            _imageService = imageService;
            _documentService = documentService;
        }

        public ActionResult Show(string userId)
        {
            var user = _documentService.GetById(userId);
            var model  = user.CheckIns.GroupBy(x => x.PlaceId ?? x.Location.ToString()).Select(x=> MapToMarker(x,user)).ToList();
            AjaxResponse.AddJsonItem("model",model);
            return RespondTo(model);
        }

        public ActionResult ForPlace(string placeId)
        {
            var model = _documentService.GetCheckInsGroupsForPlace(placeId, 20).Select(MapToListItem).SelectMany(x => x);
            return View("Index", model);
        }

        public ActionResult ForUser(string userId)
        {
            var user = _documentService.GetById(userId);
            var model = user.CheckIns.OrderByDescending(x => x.Visited).GroupBy(x => user).Select(MapToListItem).SelectMany(x => x);
            return View("Index", model);
        }

        protected MarkerModel MapToMarker(IGrouping<string, CheckInDocument> @group, UserDocument user)
        {
            var checkIns = group.OrderByDescending(x=> x.Visited).Take(10).Select(MapToListItem).ToList();
            foreach (var item in checkIns)
            {
                item.UserName = user.FullName;
                item.Url = user.Vk.GetVkUrl();
            }
            return new MarkerModel
            {
                X = group.First().Location.Latitude,
                Y = group.First().Location.Longitude,
                InfoWindowUrl = Url.Action("Details"),
                Content = MvcUtils.RenderPartialToStringRazor(ControllerContext,"CheckInsList",checkIns,ViewData,TempData),
                Icon = _imageService.CheckInIconModel,
                Shadow = null
            };
        }

        public ActionResult Details()
        {
            throw new System.NotImplementedException();
        }

        public CheckInListItem MapToListItem(CheckInDocument doc)
        {
            return new CheckInListItem
                       {
                           Memo = doc.Memo,
                           Visited = doc.Visited.ToInfoString(),
                           PlaceId = doc.PlaceId,
                           Url = Url.Action("Index","Map",new{placeId = doc.PlaceId})
                       };
        }

        public ActionResult List(string placeId)
        {
            var model = _documentService.GetCheckInsGroupsForPlace(placeId, 3).Select(MapToListItem).SelectMany(x=> x);
            AjaxResponse.Render("#checkinsList" + placeId,"CheckInsList",model);
            return Result();
        }

        public IEnumerable<CheckInListItem> MapToListItem(IGrouping<UserDocument, CheckInDocument> arg)
        {
            var model =arg.Select(MapToListItem).ToList();
            foreach (var item in model)
            {
                item.UserName = arg.Key.FullName;
                //item.Url = arg.Key.Vk.GetVkUrl();
            }
            return model;
        }
    }
}