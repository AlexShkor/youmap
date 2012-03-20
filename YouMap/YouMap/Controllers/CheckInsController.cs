using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Framework;
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
            var docs = _documentService.GetByFilter(new UserFilter { IdOrVkIdEqual = userId }).First().CheckIns;
            docs = _documentService.GetAll().SelectMany(x => x.CheckIns).ToList();
            var model  =
                _documentService.GetByFilter(new UserFilter {IdOrVkIdEqual = userId}).First()
                    .CheckIns.GroupBy(x => x.PlaceId ?? x.Location.ToString()).Select(MapToMarker).ToList();
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

        private MarkerModel MapToMarker(IGrouping<string,CheckInDocument> group)
        {
            var checkIns = group.Select(MapToListItem);
            return new MarkerModel
            {
                X = group.First().Location.Latitude,
                Y = group.First().Location.Longitude,
                InfoWindowUrl = Url.Action("Details"),
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

                       };
        }
    }
}