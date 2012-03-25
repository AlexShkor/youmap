using System.Collections.Generic;
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
        private readonly UserDocumentService _documentService;

        public CheckInsController(ICommandService commandService, UserDocumentService documentService) : base(commandService)
        {
            _documentService = documentService;
        }

        public ActionResult Show(string userId)
        {
            var user = _documentService.GetById(userId);

            //var user = _documentService.GetAll().First(x => x.VkId != null);
            var model  = user.CheckIns.GroupBy(x => x.PlaceId ?? x.Location.ToString()).Select(x=> MapToMarker(x,user)).ToList();
            AjaxResponse.AddJsonItem("model",model);
            return RespondTo(model);
        }

        private CheckInModel Map(CheckInDocument doc)
        {
            return new CheckInModel
                       {
                           Latitude = doc.Location.GetLatitudeString(),
                           Longitude = doc.Location.GetLongitudeString(),
                           Memo = doc.Memo,
                           PlaceId = doc.PlaceId,
                           Visited = doc.Visited
                       };
        }

        private MarkerModel MapToMarker(CheckInDocument doc)
        {
            return new MarkerModel
            {
                X = doc.Location.Latitude,
                Y = doc.Location.Longitude,
                Title = doc.Title,
                InfoWindowUrl = Url.Action("Details"),
                Icon = null,
                Shadow = null
            };
        }

        private MarkerModel MapToMarker(IGrouping<string, CheckInDocument> @group, UserDocument user)
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
                Icon = null,
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
                           Visited = doc.Visited.ToInfoString()
                       };
        }

        public ActionResult List(string placeId)
        {
            var model = _documentService.GetCheckInsGroupsForPlace(placeId, 3).Select(MapToListItem).SelectMany(x=> x);
            AjaxResponse.Render("#checkinsList","CheckInsList",model);
            return Result();
        }

        private IEnumerable<CheckInListItem> MapToListItem(IGrouping<UserDocument, CheckInDocument> arg)
        {
            var model =arg.Select(MapToListItem).ToList();
            foreach (var item in model)
            {
                item.UserName = arg.Key.FullName;
                item.Url = arg.Key.Vk.GetVkUrl();
            }
            return model;
        }
    }
}