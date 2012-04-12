using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YouMap.Controllers;
using YouMap.Documents.Documents;
using YouMap.Documents.Services;
using YouMap.Domain.Data;
using YouMap.Framework;
using YouMap.Framework.Utils.Extensions;
using YouMap.Models;

namespace YouMap.Areas.Mobile.Controllers
{
    public class PlacesController:BaseController
    {
        private readonly PlaceDocumentService _placeDocumentService;
        private readonly ImageService _imageService;

        public PlacesController(
            ICommandService commandService, 
            PlaceDocumentService placeDocumentService,
            ImageService imageService
            )
            : base(commandService)
        {
            _placeDocumentService = placeDocumentService;
            _imageService = imageService;
        }

        public ActionResult Index(PlaceFilterModel filter)
        {
            var location = filter.HasLocation() ? filter.GetLocation() : SessionContext.Location ?? DefaultLocation;
            var places = _placeDocumentService.GetNear(location, filter.Count ?? 50, 5);
            var model = places.Select(MapListItem).SelectMany(x => x);
            if (filter.ToCheckIn)
            {
                foreach (var item in model)
                {
                    item.MapUrl = Url.Action("CheckIn", "Home", new {placeId = item.Id});
                }
            }
            return View(model);
        }

        public ActionResult Details(string id)
        {
            var doc = _placeDocumentService.GetById(id);
            var model = Map(doc);
            return View(model);
        }

        private IEnumerable<PlaceListItem> MapListItem(IGrouping<double,PlaceDocument> pair)
        {
            return pair.Select(doc =>  new PlaceListItem
                                          {
                                              Id = doc.Id,
                                              Address = doc.Address,
                                              Description = doc.Description,
                                              Icon = _imageService.GetPlaceLogoUrl(doc),
                                              Title = doc.Title,
                                              MapUrl = Url.Action("Details", "Places", new {id = doc.Id}),
                                              Tags = doc.Tags,
                                              Distance = string.Format("{0:0.0} км", pair.Key),
                                              Layer = doc.Layer
                                          });
        }

        private PlaceInfoModel Map(PlaceDocument doc)
        {
            return new PlaceInfoModel
            {
                Id = doc.Id,
                Description = doc.Description,
                Logo = _imageService.GetPlaceLogoUrl(doc),
                Title = doc.Title,
                CheckInsLink = Url.Action("ForPlace","CheckIns", new{placeId = doc.Id}),
                EventsLink = Url.Action("ForPlace","Events", new{placeId = doc.Id}),
                MapLink = Url.Action("Index","Home", new{placeId = doc.Id}),
                CheckInLink = Url.Action("CheckIn","Home",new{placeId = doc.Id,redirectUrl = Url.Action("Details",new{id=doc.Id})})
            };
        }
    }

    public class PlaceFilterModel
    {
        public string NearX { get; set; }

        public string NearY { get; set; }

        public string Term { get; set; }

        public int? Count { get; set; }

        public bool ToCheckIn { get; set; }

        public Location GetLocation()
        {
            return Location.Parse(NearX, NearY);
        }

        public bool HasLocation()
        {
            return NearX.HasValue() && NearY.HasValue();
        }
    }
}
